#!/usr/bin/env python3
"""Additional fixtures and the exact original SDK CopyFolders overlay oracle."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[2]


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def originals():
    script = (ROOT / 'SDK/defaultscript.cs').read_text(errors='replace')
    start = script.index('        public static void CopyFolders(')
    end = script.index('\t\t// This function is called', start)
    methods = script[start:end]
    host = ROOT / 'recovery/preparation/additional/AdditionalControl.cs'
    if methods not in host.read_text():
        raise ValueError('Additional oracle differs from the exact original SDK CopyFolders/CopyFiles methods')
    return {'scriptSha256': sha(ROOT / 'SDK/defaultscript.cs'), 'controlSourceSha256': sha(host)}


def inputs(sdk, mod):
    result = []
    for label, root in (('sdk', sdk), ('mod', mod)):
        directory = root / 'Additional'
        if directory.exists():
            for path in sorted(directory.rglob('*')):
                if path.is_file():
                    result.append({'root': label, 'relative': path.relative_to(directory).as_posix(),
                                   'source': str(path), 'bytes': path.stat().st_size, 'sha256': sha(path)})
    return result


def prepare(args):
    out = args.output.resolve(); out.mkdir(parents=True, exist_ok=False)
    evidence = originals()
    fixture = {
        'sdk': {
            'Config/shared.ini': b'SDK shared\r\n',
            'Config/sdk-only.ini': b'SDK only\r\n',
            'Data/AptUI/FileBacked.version': b'SDK Additional version override\r\n',
            'ignored-sdk.asset': b'Excluded SDK precompiled asset\0',
        },
        'mod': {
            'config/SHARED.ini': b'MOD overwrites SDK shared\r\n',
            'Data/AptUI/FileBacked.version': b'MOD overwrites derived APT version\r\n',
            'Data/Mod.imp': b'MOD step8 intentional raw stream overlay\0',
            'Shaders/nested/arbitrary.bin': bytes(range(256)),
            'ignored.asset': b'Excluded MOD asset\0',
            'ignored-upper.ASSET': b'Excluded MOD upper-case asset\0',
        },
    }
    for label, root in (('sdk', args.sdk_root.resolve()), ('mod', args.mod_root.resolve())):
        if not root.is_dir() or (root / 'Additional').exists():
            raise ValueError('Additional fixture requires an existing owned root without Additional: ' + str(root))
    for label, root in (('sdk', args.sdk_root.resolve()), ('mod', args.mod_root.resolve())):
        for name, data in fixture[label].items():
            path = root / 'Additional' / name
            path.parent.mkdir(parents=True, exist_ok=True); path.write_bytes(data)
    evidence.update(succeeded=True, executed=False, inputs=inputs(args.sdk_root.resolve(), args.mod_root.resolve()),
                    scope='Owned test SDK/MOD Additional overlays only; intentional raw Mod.imp and APT version collisions exercise old step8 overwrite behavior, not game runtime acceptance')
    (out / 'fixture.json').write_text(json.dumps(evidence, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'succeeded': True, 'fixtureInputs': len(evidence['inputs'])}))


def oracle(args):
    out = args.output.resolve(); out.mkdir(parents=True, exist_ok=False)
    state = dict(originals(), succeeded=False, scope='Exact original SDK CopyFolders SDK-then-MOD overlay; MakeBig raw format/content comparison is performed by caller')
    sdk, mod = args.sdk_root.resolve(), args.mod_root.resolve()
    state['inputs'] = inputs(sdk, mod)
    state['controlSha256'] = sha(args.control)
    try:
        work = out / 'input'; work.mkdir()
        expected = json.loads(args.expected.read_text(encoding='utf-8-sig'))
        for item in expected:
            source = Path(item['source'])
            if source.stat().st_size != item['bytes'] or sha(source) != item['sha256']:
                raise ValueError('Current pre-Additional oracle member changed: ' + item['name'])
            target = work.joinpath(*item['name'].replace('\\', '/').split('/'))
            target.parent.mkdir(parents=True, exist_ok=True); shutil.copyfile(source, target)
        process = subprocess.run([str(args.control.resolve()), str(sdk / 'Additional'), str(mod / 'Additional'), str(work)], capture_output=True, timeout=30)
        (out / 'stdout.log').write_bytes(process.stdout); (out / 'stderr.log').write_bytes(process.stderr)
        state['processExitCode'] = process.returncode
        if process.returncode:
            raise ValueError('Original SDK CopyFolders overlay failed: ' + str(process.returncode))
        if inputs(sdk, mod) != state['inputs']:
            raise ValueError('Original CopyFolders modified SDK/MOD Additional source bytes')
        inventory = []
        for path in sorted(work.rglob('*')):
            if path.is_file() and path.suffix.lower() != '.asset':
                inventory.append({'name': path.relative_to(work).as_posix().replace('/', '\\').lower(),
                                  'source': str(path), 'bytes': path.stat().st_size, 'sha256': sha(path)})
        if len({item['name'] for item in inventory}) != len(inventory):
            raise ValueError('Original CopyFolders produced case-ambiguous archive members')
        state['succeeded'] = True; state['files'] = inventory
        (out / 'inventory.json').write_text(json.dumps({'succeeded': True, 'files': inventory}, indent=2) + '\n', encoding='utf-8')
        print(json.dumps({'succeeded': True, 'files': len(inventory), 'inventory': str(out / 'inventory.json')}))
    finally:
        (out / 'validation.json').write_text(json.dumps(state, indent=2) + '\n', encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    subs = parser.add_subparsers(dest='action', required=True)
    for name in ('prepare', 'oracle'):
        sub = subs.add_parser(name)
        for option in ('sdk-root', 'mod-root', 'output'):
            sub.add_argument('--' + option, required=True, type=Path)
        if name == 'oracle':
            sub.add_argument('--expected', required=True, type=Path)
            sub.add_argument('--control', required=True, type=Path)
    args = parser.parse_args()
    (prepare if args.action == 'prepare' else oracle)(args)


if __name__ == '__main__':
    main()
