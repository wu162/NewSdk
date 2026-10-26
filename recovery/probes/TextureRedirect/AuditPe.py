"""Fail closed on protected PE/native ranges; report metadata/token invariants.

Requires pinned dnfile/pefile. Passing this audit does not prove runtime correctness.
"""
import argparse
import hashlib
import json
import pathlib
import dnfile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def audit(original, rewritten, mode):
    a, b = dnfile.dnPE(str(original)), dnfile.dnPE(str(rewritten))
    checks = {}
    checks['machine-x86'] = a.FILE_HEADER.Machine == b.FILE_HEADER.Machine == 0x14c
    checks['native-entry-rva'] = a.OPTIONAL_HEADER.AddressOfEntryPoint == b.OPTIONAL_HEADER.AddressOfEntryPoint
    checks['clr-flags-entry'] = (a.net.struct.Flags, a.net.struct.EntryPointTokenOrRva) == (b.net.struct.Flags, b.net.struct.EntryPointTokenOrRva)
    directories = {}
    # Includes Win32 resources, base relocations, imports, TLS and IAT.
    for name, index in [('imports', 1), ('resources', 2), ('relocations', 5), ('tls', 9), ('iat', 12)]:
        left, right = a.OPTIONAL_HEADER.DATA_DIRECTORY[index], b.OPTIONAL_HEADER.DATA_DIRECTORY[index]
        ab = a.get_data(left.VirtualAddress, left.Size) if left.Size else b''
        bb = b.get_data(right.VirtualAddress, right.Size) if right.Size else b''
        checks[name] = (left.VirtualAddress, left.Size, ab) == (right.VirtualAddress, right.Size, bb)
        directories[name] = {'originalRva': left.VirtualAddress, 'outputRva': right.VirtualAddress, 'originalSha256': sha(ab), 'outputSha256': sha(bb)}
    for name in ['VTableFixups', 'ManagedNativeHeader', 'ExportAddressTableJumps', 'Resources']:
        ar, az = getattr(a.net.struct, name + 'Rva'), getattr(a.net.struct, name + 'Size')
        br, bz = getattr(b.net.struct, name + 'Rva'), getattr(b.net.struct, name + 'Size')
        checks['clr-' + name] = (ar, az, a.get_data(ar, az) if az else b'') == (br, bz, b.get_data(br, bz) if bz else b'')
    # Every original TypeRef namespace/name/scope and signature blob is invariant.
    # AssemblyRef RID 5 is the sole permitted identity change, separately inspected.
    for table, fields in [('TypeRef', ['TypeNamespace', 'TypeName']), ('MemberRef', ['Name', 'Signature']), ('TypeSpec', ['Signature']), ('StandAloneSig', ['Signature']), ('MethodDef', ['Name', 'Signature']), ('Field', ['Name', 'Signature'])]:
        def records(pe):
            rows = getattr(pe.net.mdtables, table, None) or []
            return [tuple((getattr(row, f).value if isinstance(getattr(row, f).value, bytes) else str(getattr(row, f))) for f in fields) for row in rows]
        checks['metadata-' + table] = records(a) == records(b)
    def scopes(pe):
        return [(r.ResolutionScope.table.name, r.ResolutionScope.row_index) for r in pe.net.mdtables.TypeRef]
    checks['TypeRef-scope-tokens'] = scopes(a) == scopes(b)
    refs_a, refs_b = a.net.mdtables.AssemblyRef, b.net.mdtables.AssemblyRef
    def identity(r):
        return [str(r.Name), r.MajorVersion, r.MinorVersion, r.BuildNumber, r.RevisionNumber, str(r.Culture), r.PublicKey.value.hex()]
    checks['AssemblyRef-count'] = len(refs_a) == len(refs_b)
    changes = [{'rid': i, 'before': identity(x), 'after': identity(y)} for i, (x, y) in enumerate(zip(refs_a, refs_b), 1) if identity(x) != identity(y)]
    checks['AssemblyRef-change-allowlist'] = not changes if mode == 'roundtrip' else len(changes) == 1 and changes[0]['rid'] == 5 and changes[0]['before'][0] == 'BinaryAssetBuilder.Core'
    # Native method starts are fixed; compare bytes to next known method/section end.
    # Reports coverage explicitly: native functions without MethodDef entries need
    # additional linker/disassembly evidence before production acceptance.
    starts = sorted(set(r.Rva for r in a.net.mdtables.MethodDef if r.Rva))
    native = []
    metadata_start, metadata_size = a.net.struct.MetaDataRva, a.net.struct.MetaDataSize
    clr = a.OPTIONAL_HEADER.DATA_DIRECTORY[14]
    excluded = [(metadata_start, metadata_start + metadata_size), (clr.VirtualAddress, clr.VirtualAddress + clr.Size)]
    for index, row in enumerate(a.net.mdtables.MethodDef, 1):
        if not row.Rva or not row.ImplFlags.miNative:
            continue
        following = [r for r in starts if r > row.Rva]
        section = a.get_section_by_rva(row.Rva)
        end = min(following + [section.VirtualAddress + section.Misc_VirtualSize])
        other = b.net.mdtables.MethodDef[index - 1]
        left, right = a.get_data(row.Rva, end-row.Rva), b.get_data(other.Rva, end-row.Rva)
        protected = [i for i in range(len(left)) if not any(lo <= row.Rva + i < hi for lo, hi in excluded)]
        same = other.Rva == row.Rva and len(left) == len(right) and all(left[i] == right[i] for i in protected)
        native.append({'token': hex(0x06000000 + index), 'rva': row.Rva, 'bytes': len(protected), 'unchanged': same})
    checks['native-MethodDef-ranges'] = bool(native) and all(n['unchanged'] for n in native)
    return {'schemaVersion': 1, 'mode': mode, 'originalSha256': sha(original.read_bytes()), 'outputSha256': sha(rewritten.read_bytes()), 'checks': checks, 'directories': directories, 'assemblyRefChanges': changes, 'nativeRanges': native, 'status': 'passed' if all(checks.values()) else 'failed', 'limitation': 'Static audit only; native functions without MethodDef rows are not exhaustively disassembled. Full G2 runtime and integration checks still required.'}


def main():
    p = argparse.ArgumentParser()
    p.add_argument('original', type=pathlib.Path)
    p.add_argument('rewritten', type=pathlib.Path)
    p.add_argument('output', type=pathlib.Path)
    p.add_argument('--mode', choices=['roundtrip', 'redirect'], default='roundtrip')
    args = p.parse_args()
    result = audit(args.original, args.rewritten, args.mode)
    args.output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    failed = [name for name, value in result['checks'].items() if not value]
    print('PE audit ' + result['status'] + (': ' + ', '.join(failed) if failed else ''))
    return bool(failed)

if __name__ == '__main__':
    raise SystemExit(main())
