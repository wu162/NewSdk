#!/usr/bin/env python3
"""Static inventory for future AudioCompiler EXE integration; never runs SDK."""
import argparse
import hashlib
import json
import pathlib
import re
import xml.etree.ElementTree as ET


def git_blob(data):
    return hashlib.sha1(b'blob ' + str(len(data)).encode() + b'\0' + data).hexdigest()


def source_tree(entries):
    def walk(items):
        children = {}
        for parts, blob in items:
            children.setdefault(parts[0], []).append((parts[1:], blob))
        content = bytearray()
        for name in sorted(children, key=lambda n: n + ('/' if children[n][0][0] else '')):
            values = children[name]
            directory = bool(values[0][0])
            sha = walk(values) if directory else values[0][1]
            content.extend((b'40000 ' if directory else b'100644 ') + name.encode() + b'\0' + bytes.fromhex(sha))
        return hashlib.sha1(b'tree ' + str(len(content)).encode() + b'\0' + content).hexdigest()
    return walk([(tuple(path.split('/')), blob) for path, blob in entries])


def main():
    args = argparse.ArgumentParser(description=__doc__)
    args.add_argument('--source', default='src/AudioCompiler')
    args.add_argument('--source-commit', required=True)
    args.add_argument('--expected-source-tree', required=True)
    args.add_argument('--output', required=True)
    options = args.parse_args()
    root = pathlib.Path(options.source)
    project = root / 'BinaryAssetBuilder.AudioCompiler.csproj'
    xml = ET.parse(project).getroot()
    properties = {child.tag: child.text for group in xml.findall('PropertyGroup') for child in group}
    references = [{'assembly': node.attrib['Include'], 'hintPath': node.findtext('HintPath'),
                   'copyLocal': node.findtext('Private')} for node in xml.findall('.//Reference')]
    records, entries = [], []
    patterns = {
        'nativeRuntimeImport': r'\b(?:DllImport|NativeLibrary|LoadLibrary|GetProcAddress)\b',
        'assemblyLoading': r'\bAssembly\.Load(?:From|File)?\s*\(',
        'resourceLookup': r'\b(?:GetManifestResourceStream|GetExecutingAssembly)\s*\(',
    }
    declarations = re.compile(r'^\s*(?:(?:public|internal|private|protected|static|sealed|abstract|unsafe|readonly|partial)\s+)*(?:class|struct|interface|enum)\s+(\w+)', re.M)
    for path in sorted(root.rglob('*')):
        if not path.is_file() or path.suffix not in ('.cs', '.csproj') or any(part in ('bin', 'obj') for part in path.relative_to(root).parts):
            continue
        relative = path.relative_to(root).as_posix()
        data = path.read_bytes()
        text = data.decode('utf-8-sig')
        blob = git_blob(data)
        entries.append((relative, blob))
        code = re.sub(r'/\*.*?\*/|//[^\n]*', '', text, flags=re.S)
        records.append({'path': relative, 'gitBlob': blob, 'sha256': hashlib.sha256(data).hexdigest(),
                        'bytes': len(data), 'declaredTypes': declarations.findall(code) if path.suffix == '.cs' else [],
                        'staticFindings': {name: bool(re.search(pattern, code)) for name, pattern in patterns.items()},
                        'futureHandling': 'deduplicate-host-assembly-attributes' if path.name == 'AssemblyInfo.cs' else 'standalone-validation-project' if path.suffix == '.csproj' else 'compile-business-source',
                        'futureDestinationHint': None if path.suffix == '.csproj' or path.name == 'AssemblyInfo.cs' else 'Compilers/Audio/' + relative})
    tree = source_tree(entries)
    if tree != options.expected_source_tree:
        raise SystemExit('Audio source tree changed: expected ' + options.expected_source_tree + ', found ' + tree)
    inventory = {
        'kind': 'static-integration-input-inventory', 'sourceCommit': options.source_commit,
        'sourceTree': tree, 'executionValidated': False, 'sdkStarted': False,
        'finalExeIntegrated': False, 'files': records,
        'csharpFiles': sum(row['path'].endswith('.cs') for row in records),
        'businessCsharpFiles': sum(row['futureHandling'] == 'compile-business-source' for row in records),
        'standaloneProject': properties, 'explicitAssemblyReferences': references,
        'coreTypeNamesManuallyReviewed': ['IAssetBuilderPlugin', 'InstanceDeclaration', 'AssetBuffer', 'TargetPlatform', 'ExtendedTypeInformation', 'Tracer', 'HashProvider', 'BinaryAssetBuilderException', 'ErrorCode'],
        'limitations': ['Pattern scan is not a compiled AssemblyRef/import/runtime-load audit.',
                        'Destination hints are a proposed inventory, not moved or integrated source.',
                        'Original Core DLL reference remains in the standalone validation project.',
                        'AssemblyInfo title/version attributes need explicit host-owned deduplication, not duplicate compilation or omission of business source.'],
    }
    destination = pathlib.Path(options.output)
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(inventory, indent=2) + '\n')
    print('PASS static Audio EXE inventory: ' + str(inventory['csharpFiles']) + ' C# files (' + str(inventory['businessCsharpFiles']) + ' business), tree ' + tree + '; no SDK or EXE execution')


if __name__ == '__main__':
    main()
