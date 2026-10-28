#!/usr/bin/env python3
"""Audit original JPEG colour/APP metadata captures; no codec or DLL execution."""
import argparse
from collections import Counter
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile


def digest(data):
    return hashlib.sha256(data).hexdigest()


def audit(root):
    directory = root / 'recovery/evidence/texture/jpeg-metadata-load/40783041481-1'
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    raw = (directory / 'samples.zip').read_bytes()
    archive_hash = 'ef7313c2061dcc28b580e8447c9dc8e01a8dfe7b08025fd19bab8ffe5ee5acca'
    assert digest(raw) == receipt['samplesSha256'] == archive_hash
    assert receipt['status'] == 'probe-only' and receipt['productionAccepted'] is False
    assert receipt['sourceCommit'] == '7ff6dc66e8d847718fcaf4926b7eecdce46c05ac'
    assert receipt['fixtureCount'] == 4000 and receipt['originalCalls'] == 8000
    assert receipt['decodedCount'] == 1440 and receipt['rejectedCount'] == 2560
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    base_dir = root / 'recovery/evidence/texture/jpeg-load/40625499544-1'
    base_raw = (base_dir / 'samples.zip').read_bytes()
    assert digest(base_raw) == 'b758465f88b831f5952e4700531ffe5b83a9696be6889a8b9d0defdc1eee24a1'
    metrics = Counter()
    with zipfile.ZipFile(io.BytesIO(raw)) as files, zipfile.ZipFile(io.BytesIO(base_raw)) as base:
        names = files.namelist()
        assert len(names) == len(set(names))
        rows = list(csv.DictReader(io.StringIO(files.read('transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
        assert rows == list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
        assert len(rows) == 4000 and len({r['label'] for r in rows}) == 4000
        for row in rows:
            label = row['label']
            source = files.read(label + '.jpg')
            assert digest(source) == row['inputSha256']
            assert row['candidateEqual'] == 'True' and row['candidateError'] == row['error']
            metrics[row['family'] + ('-rejected' if row['error'] else '-decoded')] += 1
            if row['error']:
                assert label.startswith(('s1-', 's2-'))
                assert row['error'] == 'BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/D3DX cannot load the texture from disk.'
                assert label + '.rgba' not in names and label + '.candidate.rgba' not in names
                assert row['candidateSha256'] == ''
            else:
                rgba = files.read(label + '.rgba')
                assert len(rgba) == int(row['bytes']) == int(row['width']) * int(row['height']) * 4
                assert digest(rgba) == row['sha256'] == row['candidateSha256']
                assert files.read(label + '.candidate.rgba') == rgba
                metrics['completeOriginalCandidateRgbaBytes'] += len(rgba)
            if row['family'] == 'metadata':
                profile = next(p for p in ['exif1', 'exif3', 'exif6', 'exif8', 'icc-sRGB-v2-micro',
                                           'icc-sRGB-v4', 'icc-AdobeCompat-v2', 'icc-sGrey-v4',
                                           'xmp', 'comment'] if label.endswith('-' + p))
                stem = label.removeprefix('metadata-').removesuffix('-' + profile)
                inserted_length = int.from_bytes(source[4:6], 'big') + 2
                assert source[:2] + source[2 + inserted_length:] == base.read(stem + '.jpg')
                assert files.read(label + '.rgba') == base.read(stem + '.loaded.rgba')
                metrics['metadataFullByteBaselineComparisons'] += 1
        for row in rows:
            if row['label'].startswith('s0-') and row['label'].endswith('-native'):
                context = row['label'].removesuffix('-native')
                native = files.read(context + '-native.rgba')
                assert native == files.read(context + '-no-adobe.rgba') == files.read(context + '-adobe0.rgba')
                assert files.read(context + '-adobe1.rgba') == files.read(context + '-adobe2.rgba')
                if native != files.read(context + '-adobe1.rgba'):
                    metrics['rgbContextsChangedByAdobeTransform'] += 1
                metrics['rgbAdobeContexts'] += 1
        assert metrics['metadata-decoded'] == 160 and metrics['colour-decoded'] == 1280
        assert metrics['colour-rejected'] == 2560 and metrics['metadataFullByteBaselineComparisons'] == 160
    return {'scope': 'independent recorded original/candidate byte audit; probe-only, not recovered/plugin/SDK acceptance',
            'sourceCommit': receipt['sourceCommit'], 'runId': receipt['runId'],
            'archiveSha256': archive_hash, 'fixtureCount': 4000, 'metrics': dict(metrics)}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve())
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result['metrics'], sort_keys=True))


if __name__ == '__main__':
    main()
