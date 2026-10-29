#!/usr/bin/env python3
"""Audit every captured PPM source/original/repeat/candidate byte; no codec calls."""
import argparse
from collections import Counter
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


def audit(directory):
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['status'] == 'probe-only' and receipt['productionAccepted'] is False
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    archive = (directory / 'samples.zip').read_bytes()
    sha = lambda b: hashlib.sha256(b).hexdigest()
    assert sha(archive) == receipt['samplesSha256']
    counts, families, mismatched_profiles, flags, dimensions = (Counter() for _ in range(5))
    examples, consumed = [], set()
    total_bytes = 0
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = [n for n in files.namelist() if not n.endswith('/')]
        assert len(names) == len(set(names))
        rows = list(csv.DictReader(io.StringIO(files.read('transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
        assert files.read('transcript.tsv') == (directory / 'transcript.tsv').read_bytes()
        assert len(rows) == len({r['label'] for r in rows}) == receipt['fixtureCount'] == 8438
        consumed.add('transcript.tsv')
        for row in rows:
            label = row['label']
            source = files.read(label + '.ppm')
            consumed.add(label + '.ppm')
            assert sha(source) == row['inputSha256']
            assert row['originalStable'] == 'True' and row['error'] == row['repeatError']
            families[row['family']] += 1
            if row['error']:
                counts['rejected'] += 1
                assert int(row['width']) == int(row['height']) == int(row['bytes']) == 0
                assert row['sha256'] == row['repeatSha256'] == ''
            else:
                counts['decoded'] += 1
                expected = files.read(label + '.rgba')
                repeat = files.read(label + '.repeat.rgba')
                consumed.update((label + '.rgba', label + '.repeat.rgba'))
                assert expected == repeat
                assert sha(expected) == row['sha256'] == row['repeatSha256']
                assert len(expected) == int(row['bytes']) == int(row['width']) * int(row['height']) * 4
                total_bytes += len(expected)
                flags[row['hasAlpha']] += 1
                dimensions[row['width'] + 'x' + row['height']] += 1
            if row['candidateSha256']:
                actual = files.read(label + '.candidate.rgba')
                consumed.add(label + '.candidate.rgba')
                assert sha(actual) == row['candidateSha256']
            else:
                actual = None
            if row['candidateEqual'] == 'True':
                counts['matches'] += 1
                if not row['error']:
                    assert actual == expected
                else:
                    assert actual is None and row['candidateError'] == row['error']
            else:
                counts['differences'] += 1
                mismatched_profiles[label.rsplit('-h', 1)[-1] if '-h' in label else row['family']] += 1
                if len(examples) < 24:
                    examples.append(dict(label=label, originalError=row['error'], candidateError=row['candidateError'],
                                         hasAlpha=row['hasAlpha'], dimensions=row['width'] + 'x' + row['height'],
                                         originalPrefix=None if row['error'] else expected[:32].hex(),
                                         candidatePrefix=None if actual is None else actual[:32].hex(),
                                         completePixelsEqual=not row['error'] and actual == expected))
        assert consumed == set(names), 'Unaccounted or absent captured files'
    assert counts['decoded'] == receipt['decodedCount'] and counts['rejected'] == receipt['rejectedCount']
    assert counts['matches'] == receipt['candidateMatches'] and counts['differences'] == receipt['candidateDifferences']
    assert receipt['unstableOriginals'] == 0 and receipt['originalCalls'] == 8438 * 2
    return dict(scope='independent full recorded byte/hash/determinism audit, not component or SDK acceptance',
                sourceCommit=receipt['sourceCommit'], runId=receipt['runId'], archiveSha256=sha(archive),
                filesVerified=len(consumed), originalRgbaBytes=total_bytes, outcomes=dict(counts),
                families=dict(families), originalHasAlpha=dict(flags), originalDimensions=dict(dimensions),
                differenceHeaderProfiles=dict(mismatched_profiles), examples=examples)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('directory', type=Path)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.directory)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps({k: v for k, v in result.items() if k != 'examples'}))
