#!/usr/bin/env python3
"""Audit archived BMP probe bytes and summarize frontiers; never run a codec."""
import argparse
from collections import Counter, defaultdict
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/bmp-header-load' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['sourceCommit'] == source and str(receipt['runId']) == run
    assert receipt['status'] == 'probe-only' and receipt['productionAccepted'] is False
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    archive = (directory / 'samples.zip').read_bytes()
    assert sha(archive) == receipt['samplesSha256']
    summary, errors = defaultdict(Counter), Counter()
    totals = Counter()
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = set(files.namelist())
        assert len(files.namelist()) == len(names)
        transcript = files.read('transcript.tsv')
        rows = list(csv.DictReader(io.StringIO(transcript.decode('utf-8-sig')), delimiter='\t'))
        external = list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
        assert rows == external and len(rows) == len({r['label'] for r in rows}) == receipt['fixtureCount'] == 2616
        for row in rows:
            label = row['label']
            assert sha(files.read(label + '.bmp')) == row['inputSha256']
            decoded = not row['error']
            same = row['candidateEqual'] == 'True'
            assert row['candidateEqual'] in ('True', 'False')
            if decoded:
                original = files.read(label + '.rgba')
                assert sha(original) == row['sha256']
                assert len(original) == int(row['bytes']) == 4 * int(row['width']) * int(row['height'])
                totals['originalRgbaBytes'] += len(original)
            else:
                assert not row['sha256'] and int(row['bytes']) == 0
                assert label + '.rgba' not in names
                errors[row['error']] += 1
            if row['candidateSha256']:
                candidate = files.read(label + '.candidate.rgba')
                assert sha(candidate) == row['candidateSha256'] and not row['candidateError']
                totals['candidateRgbaBytes'] += len(candidate)
            else:
                assert row['candidateError'] and label + '.candidate.rgba' not in names
            if same:
                assert row['error'] == row['candidateError']
                if decoded:
                    assert original == candidate
            outcome = ('decoded' if decoded else 'rejected') + ('/match' if same else '/difference')
            totals[outcome] += 1
            for axis, key in [('dib', row['dib']), ('depth', row['depth']),
                              ('compression', row['compression']), ('profile', row['profile']),
                              ('gap', row['gap']), ('dib-depth-compression', '/'.join(row[k] for k in ('dib', 'depth', 'compression')))]:
                summary[axis + ':' + key][outcome] += 1
        assert sum(v for k, v in totals.items() if k.startswith('decoded/')) == receipt['decodedCount']
        assert sum(v for k, v in totals.items() if k.startswith('rejected/')) == receipt['rejectedCount']
        assert sum(v for k, v in totals.items() if k.endswith('/match')) == receipt['candidateMatches']
        assert sum(v for k, v in totals.items() if k.endswith('/difference')) == receipt['candidateDifferences']
        assert receipt['originalCalls'] == 5232
    return dict(scope='independent immutable BMP archive audit; probe-only, no production acceptance',
                sourceCommit=source, runId=run, archiveSha256=receipt['samplesSha256'],
                fixtures=len(rows), originalCalls=receipt['originalCalls'], totals=dict(totals),
                groups={k: dict(v) for k, v in sorted(summary.items())}, originalErrors=dict(errors))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root, args.run_id, args.source_commit)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result['totals'], sort_keys=True))
