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
    totals, classifications = Counter(), Counter()
    unstable_labels = []
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = set(files.namelist())
        assert len(files.namelist()) == len(names)
        transcript = files.read('transcript.tsv')
        rows = list(csv.DictReader(io.StringIO(transcript.decode('utf-8-sig')), delimiter='\t'))
        external = list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
        assert rows == external and len(rows) == len({r['label'] for r in rows}) == receipt['fixtureCount'] == 2616
        expected_names = {'transcript.tsv'}
        for row in rows:
            label = row['label']
            expected_names.add(label + '.bmp')
            assert sha(files.read(label + '.bmp')) == row['inputSha256']
            decoded = not row['error']
            same = row['candidateEqual'] == 'True'
            assert row['candidateEqual'] in ('True', 'False')
            if decoded:
                expected_names.add(label + '.rgba')
                original = files.read(label + '.rgba')
                assert sha(original) == row['sha256']
                assert len(original) == int(row['bytes']) == 4 * int(row['width']) * int(row['height'])
                totals['originalRgbaBytes'] += len(original)
            else:
                assert not row['sha256'] and int(row['bytes']) == 0
                assert label + '.rgba' not in names
                errors[row['error']] += 1
            stable = row['originalStable'] == 'True'
            assert row['originalStable'] in ('True', 'False')
            totals['stableOriginals' if stable else 'unstableOriginals'] += 1
            if row['repeatSha256']:
                expected_names.add(label + '.repeat.rgba')
                repeated = files.read(label + '.repeat.rgba')
                assert sha(repeated) == row['repeatSha256'] and not row['repeatError']
                assert len(repeated) == int(row['repeatBytes']) == 4 * int(row['repeatWidth']) * int(row['repeatHeight'])
                totals['repeatRgbaBytes'] += len(repeated)
            else:
                assert row['repeatError'] and label + '.repeat.rgba' not in names
                assert int(row['repeatBytes']) == int(row['repeatWidth']) == int(row['repeatHeight']) == 0
            actual_stable = row['error'] == row['repeatError']
            if decoded:
                actual_stable = actual_stable and bool(row['repeatSha256']) and all(row[a] == row[b] for a, b in [('width','repeatWidth'),('height','repeatHeight'),('hasAlpha','repeatHasAlpha')])
                if actual_stable:
                    actual_stable = original == repeated
            else:
                actual_stable = actual_stable and not row['repeatSha256']
            assert stable == actual_stable
            if not stable:
                unstable_labels.append(label)
                summary['unstable:' + '/'.join(row[k] for k in ('dib', 'depth', 'compression', 'topDown'))]['inputs'] += 1
            # A matching pair does not prove determinism for a family with observed
            # counterexamples. Preserve all its inputs as uncertain, including pairs
            # that happened to match in this one process.
            uncertain_family = row['dib'] in ('52', '56', '108', '124') and row['depth'] in ('24', '32') and row['compression'] == '3'
            classification = ('uncertain-family/' + ('repeat-equal' if stable else 'repeat-different')
                              if uncertain_family else 'repeat-equal/' + ('decoded' if decoded else 'rejected'))
            assert stable or uncertain_family, ('New unstable family requires review', label)
            classifications[classification] += 1
            if row['candidateSha256']:
                expected_names.add(label + '.candidate.rgba')
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
        assert totals['stableOriginals'] == receipt['stableOriginals']
        assert totals['unstableOriginals'] == receipt['unstableOriginals']
        assert receipt['originalCalls'] == 5232
        assert names == expected_names, 'Missing or unexpected archive members'
        by_label = {row['label']: row for row in rows}
        metadata_baselines = Counter()
        for row in rows:
            if row['profile'] == 'plain':
                continue
            baseline_label = row['label'][:-len(row['profile'])] + 'plain'
            baseline = by_label[baseline_label]
            assert row['originalStable'] == baseline['originalStable'] == 'True'
            for field in ('width', 'height', 'hasAlpha', 'bytes', 'sha256', 'error'):
                assert row[field] == baseline[field], (row['label'], 'metadata baseline', field)
            assert not row['error']
            assert files.read(row['label'] + '.rgba') == files.read(baseline_label + '.rgba')
            metadata_baselines[row['profile']] += 1
        assert sum(metadata_baselines.values()) == 792
    return dict(scope='independent immutable BMP archive audit; probe-only, no production acceptance',
                sourceCommit=source, runId=run, archiveSha256=receipt['samplesSha256'],
                fixtures=len(rows), originalCalls=receipt['originalCalls'], totals=dict(totals),
                classifications=dict(classifications), unstableLabels=unstable_labels,
                fullByteMetadataBaselineComparisons=dict(metadata_baselines),
                determinismLimit='Two consecutive calls in one process only; repeat-equal is not a cross-process determinism proof. Extended-DIB 24/32-bit BI_BITFIELDS is quarantined as a whole family.',
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
