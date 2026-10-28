#!/usr/bin/env python3
"""Audit saved SDK products and every actual trace file without running BAB/codecs."""
import argparse
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


def digest(data):
    return hashlib.sha256(data).hexdigest()


def component(value):
    # Match the trace wrapper's Windows filename encoding on every host.
    return ''.join(f'%{ord(c):04X}' if ord(c) < 32 or c in '%/\\:<>"|?*' else c for c in value)


def audit(directory, source):
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['sourceCommit'] == source and receipt['status'] == 'passed'
    assert receipt['sdkValidated'] is True and not receipt.get('probeOnly', False)
    modes = {m['mode']: m for m in receipt['modes']}
    assert set(modes) == {'Original', 'Recovered', 'NoOriginal'} and len(receipt['modes']) == 3
    assert modes['NoOriginal']['withoutOriginalTextureCompiler'] is True
    assert modes['NoOriginal']['assertedOriginalNotLoadedDuringEveryCall'] is True
    result = {'profile': receipt['profile'], 'sourceCommit': source, 'runId': receipt['runId'],
              'receipt': directory.as_posix(), 'products': [], 'modes': []}
    products = {}
    for mode in ('Original', 'Recovered', 'NoOriginal'):
        row = modes[mode]
        assert row['passed'] is True and row['uniqueAssets'] > 0 and row['actualCalls'] >= row['uniqueAssets']
        files = {f['file']: f for f in row['files']}
        assert len(row['files']) == 4 and set(files) == {'mod.bin', 'mod.manifest', 'mod.imp', 'mod.relo'}
        for name, info in files.items():
            data = (directory / (mode + '-' + name)).read_bytes()
            assert len(data) == info['bytes'] and digest(data) == info['sha256']
            if mode == 'Original':
                products[name] = data
                result['products'].append({'file': name, 'bytes': len(data), 'sha256': digest(data)})
            else:
                assert data == products[name], (mode, name, 'full product bytes')
    archive = (directory / 'traces.zip').read_bytes()
    assert digest(archive) == receipt['traceArchiveSha256']
    result['traceArchiveSha256'] = digest(archive)
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = [f.filename for f in files.infolist() if not f.is_dir()]
        assert len(names) == len(set(names)), 'Duplicate trace members'
        by_mode = {mode: {} for mode in modes}
        for name in names:
            family, relative = name.split('/', 1)
            mode = family.removesuffix('-trace')
            assert mode in by_mode and '/' not in relative and relative
            by_mode[mode][relative] = name
        expected_names = set(by_mode['Original'])
        for mode in modes:
            assert set(by_mode[mode]) == expected_names, (mode, 'complete trace file set')
            trace_index = []
            for relative in sorted(expected_names):
                data = files.read(by_mode[mode][relative])
                if mode != 'Original':
                    assert data == files.read(by_mode['Original'][relative]), (mode, relative, 'full trace bytes')
                trace_index.append([relative, len(data), digest(data)])
            rows = list(csv.DictReader(io.StringIO(files.read(by_mode[mode]['texture.calls.tsv']).decode('utf-8-sig')), delimiter='\t'))
            assert not any(r['Stage'] == 'failure' for r in rows)
            success = [r for r in rows if r['Stage'] == 'success']
            assert len(success) == modes[mode]['actualCalls']
            assert len({(r['TypeId'], r['InstanceId']) for r in success}) == modes[mode]['uniqueAssets']
            required = {'texture.calls.tsv'}
            for row in success:
                prefix = component(row['TypeName']) + '_' + component(row['InstanceName'])
                for extension, field in [('instance', 'InstanceLength'), ('relo', 'RelocationLength'), ('imp', 'ImportsLength')]:
                    name = prefix + '.' + extension
                    assert name in by_mode[mode]
                    expected = int(row[field])
                    assert expected >= -1 and len(files.read(by_mode[mode][name])) == max(expected, 0)
                    required.add(name)
                name = prefix + '.cdata'
                if row['HasCData'] == 'true':
                    assert name in by_mode[mode] and digest(files.read(by_mode[mode][name])) == row['CDataSHA256']
                    required.add(name)
                else:
                    assert row['HasCData'] == 'false' and name not in by_mode[mode] and not row['CDataSHA256']
            assert required == expected_names, (mode, 'unrecorded or missing actual trace')
            result['modes'].append({'mode': mode, 'uniqueAssets': modes[mode]['uniqueAssets'],
                                    'actualCalls': len(success), 'completeTraceFiles': len(expected_names),
                                    'traceIndexSha256': digest(json.dumps(trace_index, separators=(',', ':')).encode())})
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--expected-profiles', help='Exact comma-separated profile set selected for this SDK batch')
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    root = args.root.resolve()
    profiles = []
    for path in sorted((root / 'recovery/evidence/texture').glob('**/' + args.run_id + '-*/validation.json')):
        row = json.loads(path.read_text(encoding='utf-8-sig'))
        if row.get('sdkValidated') is True:
            item = audit(path.parent, args.source_commit)
            assert item['runId'] == args.run_id
            item['receipt'] = path.relative_to(root).as_posix()
            profiles.append(item)
    assert profiles, 'No successful SDK profiles in this run'
    assert len({p['profile'] for p in profiles}) == len(profiles), 'Duplicate SDK profile receipts'
    if args.expected_profiles:
        assert {p['profile'] for p in profiles} == set(args.expected_profiles.split(',')), 'Missing or extra SDK profiles'
    result = {'scope': 'independent recorded products/complete actual trace audit; no BAB/codec execution',
              'sourceCommit': args.source_commit, 'runId': args.run_id, 'profiles': profiles}
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited SDK profiles=' + str(len(profiles)) + ' actualAssetsPerMode=' +
          str(sum(p['modes'][0]['uniqueAssets'] for p in profiles)))


if __name__ == '__main__':
    main()
