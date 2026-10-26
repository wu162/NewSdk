#!/usr/bin/env python3
"""Prepare real-stream neutral references and retain the original SDK chain oracle.

Only caller-owned test SDK directories are modified by prepare. The oracle
always runs original tools and edits the stream in a private disposable copy.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import struct
import subprocess

ROOT = Path(__file__).resolve().parents[2]
EXTENSIONS = ('.manifest', '.bin', '.imp', '.relo')
TOOLS = ('HashFix.exe', 'ModAssetResolver.exe', 'LoDStreamBuilder.exe',
         'ModAssetResolver_TypeHashes.txt', 'ModAssetResolver_InstanceHashes.txt')


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def describe(path, name=None):
    row = {'source': str(path.resolve()), 'bytes': path.stat().st_size, 'sha256': sha(path)}
    if name is not None: row['name'] = name
    return row


def snapshot(directory):
    return {path.relative_to(directory).as_posix(): describe(path)
            for path in sorted(directory.rglob('*')) if path.is_file()}


def read_manifest(path):
    data = path.read_bytes()
    if len(data) < 48 or data[:4] != b'\0\1\6\0':
        raise ValueError('Neutral fixture requires a linked little-endian RA3 v6 stream')
    count = struct.unpack_from('<I', data, 12)[0]
    if len(data) != 48 + 48 * count + sum(struct.unpack_from('<4I', data, 32)):
        raise ValueError('Neutral fixture manifest table lengths differ')
    return data, count


def stream_inputs(manifest):
    files = [manifest if ext == '.manifest' else manifest.with_suffix(ext) for ext in EXTENSIONS]
    cdata = manifest.with_suffix('') / 'cdata'
    if cdata.exists(): files += [p for p in sorted(cdata.rglob('*')) if p.is_file()]
    return {str(path): describe(path) for path in files}


def dummy(identity, checksum):
    names, sources = b'Test:neutral-dummy\0', b'Data/neutral-dummy.xml\0'
    header = struct.pack('<BBH11I', 0, 1, 6, checksum, 0, 1, 0, 0, 0, 0, 0, 0, len(names), len(sources))
    table = struct.pack('<12I', identity[0], identity[1], 0, 1, 0, 0, 0, 0, 0, 0, 0, 0)
    return header + table + names + sources


def prepare(args):
    out = args.output.resolve(); out.mkdir(parents=True, exist_ok=False)
    manifest, sdk = args.manifest.resolve(), args.sdk_root.resolve()
    state = {'schemaVersion': 1, 'succeeded': False, 'scope': 'Real loose stream cloned as altered-hash Worldbuilder; no insertion production fixture only'}
    try:
        data, count = read_manifest(manifest)
        if count == 0: raise ValueError('Neutral fixture must exercise at least one actual asset hash')
        state['sourceInputs'] = stream_inputs(manifest)
        built = sdk / 'BuiltMods'
        if built.exists(): raise ValueError('Neutral fixture requires owned SDK without preexisting BuiltMods')
        built.mkdir(parents=True)
        changed = bytearray(data)
        for index in range(count):
            offset = 48 + 48 * index + 12
            struct.pack_into('<I', changed, offset, struct.unpack_from('<I', changed, offset)[0] ^ 1)
        (built / 'Worldbuilder.manifest').write_bytes(changed)
        for ext in EXTENSIONS[1:]: shutil.copyfile(manifest.with_suffix(ext), built / ('Worldbuilder' + ext))
        sage = built / 'SageXml'; sage.mkdir()
        shutil.copyfile(built / 'Worldbuilder.manifest', sage / 'Worldbuilder.manifest')
        identities = {struct.unpack_from('<II', data, 48 + 48 * index) for index in range(count)}
        instance = 0x7ffffffe
        while (0x7fffffff, instance) in identities: instance -= 1
        for name in ('Static', 'Global', 'Audio'):
            (built / (name + '.manifest')).write_bytes(dummy((0x7fffffff, instance), struct.unpack_from('<I', data, 4)[0]))
            for ext in EXTENSIONS[1:]: (built / (name + ext)).write_bytes(data[4:8])
        (built / 'StringHashes.xml').write_text('<AssetDeclaration/>', encoding='utf-8')
        state['referenceInputs'] = snapshot(built)
        state['changedWorldbuilderHashCount'] = count
        if stream_inputs(manifest) != state['sourceInputs']: raise ValueError('Neutral preparation modified the loose source')
        state['succeeded'] = True
    finally:
        (out / 'fixture.json').write_text(json.dumps(state, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'succeeded': True, 'changedWorldbuilderHashCount': count}))


def invoke(exe, manifest, tools, out, name):
    process = subprocess.run([str(exe), str(manifest)], cwd=tools, capture_output=True, timeout=30)
    (out / (name + '.stdout.log')).write_bytes(process.stdout)
    (out / (name + '.stderr.log')).write_bytes(process.stderr)
    if process.returncode or b'Error:' in process.stdout or b'Error:' in process.stderr:
        raise ValueError('Original neutral stage failed: ' + name)
    return process.stdout


def oracle(args):
    out = args.output.resolve(); out.mkdir(parents=True, exist_ok=False)
    source, sdk = args.manifest.resolve(), args.sdk_root.resolve()
    state = {'schemaVersion': 1, 'succeeded': False,
             'scope': 'Original HashFix then ModAssetResolver then LoD against current real loose input; no-insertion subset only; exact raw oracle remains required'}
    try:
        data, count = read_manifest(source)
        state['sourceInputs'] = stream_inputs(source)
        state['referenceInputs'] = snapshot(sdk / 'BuiltMods')
        private = out / 'SDK'; private.mkdir()
        shutil.copytree(sdk / 'BuiltMods', private / 'BuiltMods')
        tools = private / 'tools'; tools.mkdir()
        for name in TOOLS: shutil.copyfile(ROOT / 'SDK/tools' / name, tools / name)
        state['originalTools'] = {name: sha(tools / name) for name in TOOLS}
        work = out / 'stream'; work.mkdir(); manifest = work / 'mod.manifest'
        for ext in EXTENSIONS:
            shutil.copyfile(source if ext == '.manifest' else source.with_suffix(ext), work / ('mod' + ext))
        source_cdata = source.with_suffix('') / 'cdata'
        if source_cdata.exists(): shutil.copytree(source_cdata, work / 'mod/cdata')
        before = snapshot(private / 'BuiltMods')
        stdout = invoke(tools / 'HashFix.exe', manifest, tools, out, 'hashfix')
        changed = re.search(rb'Instance hashes fixed:\s*(\d+)', stdout)
        if not changed or int(changed.group(1)) == 0 or manifest.read_bytes() == data:
            raise ValueError('Neutral oracle did not exercise an actual HashFix change')
        state['hashFixChanged'] = True; state['hashFixChangedCount'] = int(changed.group(1))
        stage = out / 'after-hashfix'; stage.mkdir()
        for ext in EXTENSIONS: shutil.copyfile(work / ('mod' + ext), stage / ('mod' + ext))
        stdout = invoke(tools / 'ModAssetResolver.exe', manifest, tools, out, 'resolver')
        if b'Assets prepended: 0' not in stdout or read_manifest(manifest)[1] != count:
            raise ValueError('Neutral oracle escaped the no-insertion scope')
        state['resolverInsertedAssets'] = 0
        stage = out / 'after-resolver'; stage.mkdir()
        for ext in EXTENSIONS: shutil.copyfile(work / ('mod' + ext), stage / ('mod' + ext))
        before_lod = {ext: sha(work / ('mod' + ext)) for ext in EXTENSIONS}
        invoke(tools / 'LoDStreamBuilder.exe', manifest, tools, out, 'lod')
        if before_lod != {ext: sha(work / ('mod' + ext)) for ext in EXTENSIONS}:
            raise ValueError('Original LoD unexpectedly modified its main stream')
        expected = ['mod' + suffix + ext for suffix in ('', '_l', '_m') for ext in EXTENSIONS]
        files = [describe(work / name, 'data\\' + name) for name in expected]
        if (work / 'mod/cdata').exists():
            for path in sorted((work / 'mod/cdata').rglob('*')):
                if path.is_file(): files.append(describe(path, 'data\\mod\\cdata\\' + path.relative_to(work / 'mod/cdata').as_posix().replace('/', '\\')))
        if {p.relative_to(work).as_posix() for p in work.rglob('*') if p.is_file()} != set(expected + [item['name'][5:].replace('\\', '/') for item in files[12:]]):
            raise ValueError('Original neutral chain output inventory differs')
        if snapshot(private / 'BuiltMods') != before: raise ValueError('Original neutral tools modified reference inputs')
        if stream_inputs(source) != state['sourceInputs'] or snapshot(sdk / 'BuiltMods') != state['referenceInputs']:
            raise ValueError('Original neutral oracle modified caller loose/SDK inputs')
        state['succeeded'] = True; state['files'] = files
        (out / 'inventory.json').write_text(json.dumps({'succeeded': True, 'files': files, 'hashFixChanged': True,
                    'hashFixChangedCount': state['hashFixChangedCount'], 'resolverInsertedAssets': 0}, indent=2) + '\n', encoding='utf-8')
        print(json.dumps({'succeeded': True, 'files': len(files), 'hashFixChanged': True}))
    finally:
        (out / 'validation.json').write_text(json.dumps(state, indent=2) + '\n', encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    subs = parser.add_subparsers(dest='action', required=True)
    for name in ('prepare', 'oracle'):
        sub = subs.add_parser(name)
        for option in ('manifest', 'sdk-root', 'output'): sub.add_argument('--' + option, required=True, type=Path)
    args = parser.parse_args()
    (prepare if args.action == 'prepare' else oracle)(args)


if __name__ == '__main__': main()
