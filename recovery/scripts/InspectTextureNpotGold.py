#!/usr/bin/env python3
"""Cheap, exact immutable NPOT input preflight; no build or codec execution."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--plan', type=Path, required=True)
    parser.add_argument('--out', type=Path, required=True)
    parser.add_argument('--windows-checkout', action='store_true',
                        help='Audit Git Windows checkout filters before allocating a Windows runner')
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[2]
    plan = json.loads(args.plan.read_text(encoding='utf-8-sig'))
    result = dict(sourceCommit=plan['sourceCommit'], scope='exact immutable input preflight; not component acceptance',
                  windowsCheckoutFilters=args.windows_checkout, files=[])
    if 'npot-rgba8' not in plan['selected']:
        result['status'] = 'not-selected'
    else:
        head = subprocess.check_output(['git', '-C', str(root), 'rev-parse', 'HEAD']).decode().strip()
        assert plan['sourceCommit'] == head, 'Selection plan/source mismatch'

        def verify(name, expected):
            if args.windows_checkout:
                data = subprocess.check_output(['git', '-C', str(root), '-c', 'core.autocrlf=true',
                                                'cat-file', '--filters', head + ':' + name])
            else:
                data = (root / name).read_bytes()
            actual = hashlib.sha256(data).hexdigest()
            assert actual == expected, 'Immutable input SHA256 mismatch: ' + name
            result['files'].append(dict(file=name, bytes=len(data), sha256=actual))

        original = '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
        verify('SDK/tools/BinaryAssetBuilder.TextureCompiler.dll', original)
        load = 'recovery/evidence/texture/npot-load/40528237009-1'
        numeric = 'recovery/evidence/texture/npot-numeric/40529918922-1'
        for directory, source, run in ((load, 'a19bb879704cd80ad55202760a160c8af57bbe2d', '40528237009'),
                                       (numeric, '678db0127a2ea3620700b44e46093dd6dc2050d1', '40529918922')):
            receipt = json.loads((root / directory / 'validation.json').read_text(encoding='utf-8-sig'))
            assert receipt['sourceCommit'] == source and str(receipt['runId']) == run
            assert receipt['status'] == 'probe-only' and receipt['originalSha256'] == original
            subprocess.run(['git', '-C', str(root), 'merge-base', '--is-ancestor', source, head], check=True)
            if directory == load:
                assert receipt['fixtureCount'] == receipt['decodedCount'] == 1012
                verify(directory + '/samples.zip', receipt['samplesSha256'])
            else:
                for item in receipt['tableFiles']:
                    verify(directory + '/tables/' + item['name'], item['sha256'])
        result['status'] = 'passed'
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('NPOT immutable input preflight: ' + result['status'] + '; files=' + str(len(result['files'])))


if __name__ == '__main__':
    main()
