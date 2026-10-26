#!/usr/bin/env python3
"""Run the original RA3 LoD stage against an isolated current loose stream.

The returned inventory is an oracle for production BIG members. This process
never runs in the compiler and does not establish complete MOD/game acceptance.
"""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import struct
import subprocess


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def describe(path):
    return {'name': path.name, 'source': str(path.resolve()),
            'bytes': path.stat().st_size, 'sha256': sha(path)}


def create_oracle(manifest, original, output, timeout=30):
    manifest, original, output = (Path(p).resolve() for p in (manifest, original, output))
    if manifest.suffix.lower() != '.manifest':
        raise ValueError('LoD oracle requires a linked .manifest file')
    output.mkdir(parents=True, exist_ok=False)
    state = {'schemaVersion': 1, 'succeeded': False, 'originalExeSha256': sha(original),
             'inputs': [], 'outputs': [],
             'scope': 'Original RA3 PC LoD eight-file oracle for this exact current loose stream; original four files remain unchanged'}
    try:
        work = output / 'stream'; work.mkdir()
        tools = output / 'tools'; tools.mkdir()
        exe = tools / 'LoDStreamBuilder.exe'; shutil.copyfile(original, exe)
        if sha(exe) != state['originalExeSha256']:
            raise ValueError('Copied original LoD executable differs')
        # Match the SDK step-5 lower-case mod.manifest argument, even when
        # the source XML/loose artifact basename is Mod on Windows.
        stem = manifest.stem.lower()
        extensions = ('.manifest', '.bin', '.imp', '.relo')
        for extension in extensions:
            source = manifest if extension == '.manifest' else manifest.with_suffix(extension)
            target = work / (stem + extension)
            shutil.copyfile(source, target)
            if sha(source) != sha(target):
                raise ValueError('Copied current loose input differs: ' + extension)
            row = describe(target); row['sourceInput'] = str(source)
            state['inputs'].append(row)
        local_manifest = work / (stem + '.manifest')
        try:
            process = subprocess.run([str(exe), str(local_manifest)], cwd=tools,
                                     capture_output=True, timeout=timeout)
        except subprocess.TimeoutExpired as error:
            (output / 'stdout.log').write_bytes(error.stdout or b'')
            (output / 'stderr.log').write_bytes(error.stderr or b'')
            state['timedOut'] = True
            raise
        (output / 'stdout.log').write_bytes(process.stdout)
        (output / 'stderr.log').write_bytes(process.stderr)
        state['processExitCode'] = process.returncode
        if process.returncode:
            raise ValueError('Original LoD process failed: ' + str(process.returncode))
        expected = [stem + suffix + extension for suffix in ('_l', '_m') for extension in extensions]
        if {p.name for p in work.iterdir()} != set(expected + [i['name'] for i in state['inputs']]):
            raise ValueError('Original LoD did not produce exactly eight additional files')
        for row in state['inputs']:
            path = Path(row['source'])
            if path.stat().st_size != row['bytes'] or sha(path) != row['sha256']:
                raise ValueError('Original LoD changed a main stream input: ' + path.name)
        # The original may catch an exception and still return zero. Fresh,
        # complete output bytes, rather than exit status alone, establish this
        # oracle. RA3 v6 has a FOUR-byte stream checksum in each empty payload.
        source = local_manifest.read_bytes()
        if len(source) < 48 or source[:4] != b'\0\1\6\0':
            raise ValueError('Original LoD oracle input is not linked little-endian RA3 v6')
        for suffix in ('_l', '_m'):
            result = (work / (stem + suffix + '.manifest')).read_bytes()
            if len(result) < 48 or result[:4] != source[:4]:
                raise ValueError('Original LoD emitted an incomplete v6 manifest')
            count = struct.unpack_from('<I', result, 12)[0]
            sizes = struct.unpack_from('<4I', result, 32)
            if len(result) != 48 + 48 * count + sum(sizes):
                raise ValueError('Original LoD manifest tables do not consume output')
            for extension in extensions[1:]:
                if (work / (stem + suffix + extension)).read_bytes() != source[4:8]:
                    raise ValueError('Original LoD emitted a non-v6 empty payload')
        if (work / (stem + '_l.manifest')).read_bytes() != (work / (stem + '_m.manifest')).read_bytes():
            raise ValueError('Original LoD low/medium manifests differ')
        state['outputs'] = [describe(work / name) for name in expected]
        (output / 'inventory.json').write_text(json.dumps({'succeeded': True, 'files': state['outputs']}, indent=2) + '\n', encoding='utf-8')
        state['succeeded'] = True
        return state['outputs']
    finally:
        (output / 'validation.json').write_text(json.dumps(state, indent=2) + '\n', encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for name in ('manifest', 'original', 'output'):
        parser.add_argument('--' + name, required=True, type=Path)
    parser.add_argument('--timeout', type=int, default=30)
    args = parser.parse_args()
    inventory = create_oracle(args.manifest, args.original, args.output, args.timeout)
    print(json.dumps({'succeeded': True, 'files': len(inventory),
                      'inventory': str(args.output.resolve() / 'inventory.json')}))


if __name__ == '__main__':
    main()
