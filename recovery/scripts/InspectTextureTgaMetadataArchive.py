#!/usr/bin/env python3
"""Independently audit immutable original TGA metadata records, without codecs."""
import argparse
from collections import Counter, defaultdict
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


def digest(data):
    return hashlib.sha256(data).hexdigest()


def audit(root):
    directory = root / 'recovery/evidence/texture/tga-metadata-load/40793184986-1'
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    raw = (directory / 'samples.zip').read_bytes()
    expected_hash = 'e55d7f704fd1d592593843e7444fa231ff05a3824ea3895a08ba50bb4b149993'
    assert digest(raw) == receipt['samplesSha256'] == expected_hash
    assert receipt['sourceCommit'] == '63dcfd2b527284ff153e07e7badc997a3d2bfb49'
    assert receipt['status'] == 'probe-only' and receipt['fixtureCount'] == receipt['decodedCount'] == 12480
    assert receipt['originalCalls'] == 24960 and receipt['rejectedCount'] == 0
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    groups = defaultdict(dict)
    formats, alpha = Counter(), Counter()
    byte_count = comparisons = 0
    with zipfile.ZipFile(io.BytesIO(raw)) as files:
        names = files.namelist()
        assert len(names) == len(set(names))
        rows = list(csv.DictReader(io.StringIO(files.read('transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
        assert rows == list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
        assert len(rows) == len({r['label'] for r in rows}) == 12480
        for row in rows:
            label = row['label']
            assert row['error'] == '' and digest(files.read(label + '.tga')) == row['inputSha256']
            rgba = files.read(label + '.rgba')
            assert len(rgba) == int(row['bytes']) == int(row['width']) * int(row['height']) * 4
            assert digest(rgba) == row['sha256']
            context = label.removesuffix('-' + row['profile'])
            assert row['profile'] not in groups[context]
            groups[context][row['profile']] = row
            formats[row['format']] += 1
            alpha[row['format'] + '/' + row['hasAlpha']] += 1
            byte_count += len(rgba)
        assert len(groups) == 480 and len(formats) == 10 and set(formats.values()) == {1248}
        for context, profiles in groups.items():
            assert len(profiles) == 26
            baseline = profiles['none']
            expected = files.read(baseline['label'] + '.rgba')
            for profile, row in profiles.items():
                for field in ['width', 'height', 'hasAlpha', 'bytes', 'sha256', 'error']:
                    assert row[field] == baseline[field], (context, profile, field)
                assert files.read(row['label'] + '.rgba') == expected, (context, profile, 'full RGBA')
                comparisons += 1
    return {'scope': 'independent original archive audit; probe-only, not recovered/plugin/SDK acceptance',
            'sourceCommit': receipt['sourceCommit'], 'runId': receipt['runId'],
            'archiveSha256': expected_hash, 'fixtures': 12480, 'originalCalls': 24960,
            'decoded': 12480, 'rejected': 0, 'contexts': 480, 'profilesPerContext': 26,
            'completeRgbaBytes': byte_count, 'fullByteBaselineComparisons': comparisons,
            'formats': dict(formats), 'alphaDistribution': dict(alpha)}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve())
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited original TGA metadata: 12480 full input/RGBA records; 480 contexts, 26 profiles; all equal to bare-image baseline')


if __name__ == '__main__':
    main()
