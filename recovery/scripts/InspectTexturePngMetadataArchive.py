#!/usr/bin/env python3
"""Independently audit recorded original bytes; never execute a codec or a DLL."""
import argparse
import base64
import collections
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


ORIGINAL = '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
BASELINES = [
    ('png-metadata-load/40779895712-1', 'c812b8094a032dba0a29475d90e563cef211116e',
     '5e698ae274086468e3efa5520847fa9127acc4de62c1b96060a5ac0cbab0cac6', 4680, 3120),
    ('png-metadata-order/40780410178-1', '2cab493118bb33992a762ee191af392d10fe30eb',
     'e804e2d5099f73c6be676a87153b02806fc0237827dc0da657c48503cdbd964f', 3240, 2160),
]
VALID_SRGB = {'srgb-only', 'srgb-first', 'srgb-last', 'srgb-low', 'srgb-high',
              'srgb-intent1', 'srgb-intent2', 'srgb-intent3', 'srgb-duplicate',
              'srgb-duplicate-bad', 'srgb-gamma-after'}


def digest(data):
    return hashlib.sha256(data).hexdigest()


def audit(root):
    result = {'scope': 'recorded original archives only; not recovered/plugin/SDK acceptance',
              'originalSha256': ORIGINAL, 'baselines': [], 'iccFixtures': []}
    for path, source, archive_hash, count, decoded in BASELINES:
        directory = root / 'recovery/evidence/texture' / path
        receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
        archive = (directory / 'samples.zip').read_bytes()
        assert receipt['sourceCommit'] == source and receipt['status'] == 'probe-only'
        assert receipt['originalSha256'] == ORIGINAL and receipt['fixtureCount'] == count
        assert receipt['decodedCount'] == decoded and receipt['rejectedCount'] == count - decoded
        assert digest(archive) == archive_hash == receipt['samplesSha256']
        groups = collections.defaultdict(dict)
        rgba_bytes = comparisons = gamma_changes = 0
        with zipfile.ZipFile(io.BytesIO(archive)) as files:
            names = files.namelist()
            assert len(names) == len(set(names)), 'Duplicate archive members'
            rows = list(csv.DictReader(io.StringIO(files.read('transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
            assert rows == list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
            assert len(rows) == count and len({r['label'] for r in rows}) == count
            assert sum(not r['error'] for r in rows) == decoded
            for row in rows:
                assert digest(files.read(row['label'] + '.png')) == row['inputSha256']
                if not row['error']:
                    rgba = files.read(row['label'] + '.rgba')
                    assert len(rgba) == int(row['bytes']) == int(row['width']) * int(row['height']) * 4
                    assert digest(rgba) == row['sha256']
                    rgba_bytes += len(rgba)
                else:
                    assert row['error'] == 'BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/D3DX cannot load the texture from disk.'
                    assert row['label'] + '.rgba' not in names
                context = row['label'].removesuffix('-' + row['profile'])
                assert row['profile'] not in groups[context]
                groups[context][row['profile']] = row
            assert len(groups) == 180
            for context, profiles in groups.items():
                assert len(profiles) == (26 if count == 4680 else 18)
                for profile, row in profiles.items():
                    if count == 4680:
                        reference = 'icc-gamma' if 'gamma' in profile and profile != 'icc-srgb-gamma' else 'none'
                    else:
                        reference = 'srgb-only' if profile in VALID_SRGB else 'gamma-only'
                    baseline = profiles[reference]
                    for field in ['width', 'height', 'hasAlpha', 'bytes', 'sha256', 'error']:
                        assert row[field] == baseline[field], (context, profile, reference, field)
                    if not row['error']:
                        assert files.read(row['label'] + '.rgba') == files.read(baseline['label'] + '.rgba')
                    comparisons += 1
                a, b = ('none', 'icc-gamma') if count == 4680 else ('srgb-only', 'gamma-only')
                if profiles[a]['sha256'] != profiles[b]['sha256']:
                    gamma_changes += 1
        result['baselines'].append({'path': path, 'sourceCommit': source, 'runId': receipt['runId'],
                                    'archiveSha256': archive_hash, 'fixtures': count, 'decoded': decoded,
                                    'rejected': count - decoded, 'contexts': len(groups),
                                    'originalRgbaBytes': rgba_bytes, 'fullByteRuleComparisons': comparisons,
                                    'gammaChanges': gamma_changes})
    reference = root / 'recovery/reference/compact-icc'
    provenance = json.loads((reference / 'PROVENANCE.json').read_text())
    for row in provenance['files']:
        data = base64.b64decode((reference / row['storedAs']).read_text())
        assert digest(data) == row['sha256'] and len(data) == row['bytes']
        result['iccFixtures'].append({'path': row['path'], 'bytes': len(data), 'sha256': digest(data)})
    result['upstreamCommit'] = provenance['commit']
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve())
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited original PNG metadata: 7920 input/error records; 5280 full RGBA files; 5 upstream ICC fixtures')


if __name__ == '__main__':
    main()
