#!/usr/bin/env python3
"""Compare recorded BI_RGB storage and original RGBA to INFO40, without codecs."""
import argparse
from collections import Counter
import csv
import io
import json
from pathlib import Path
import struct
import zipfile

from InspectTextureBmpHeaderArchive import audit


def compare(root):
    source = '511896747c32c7863078b36ad781caae4f3763c1'
    run = '40799686293'
    # Independently verify every input/output hash and repeat classification
    # before deriving a layout rule from this immutable corpus.
    original = audit(root, run, source)
    directory = root / 'recovery/evidence/texture/bmp-header-load/40799686293-1'
    counts = Counter()
    storage_bytes = rgba_bytes = 0
    with zipfile.ZipFile(directory / 'samples.zip') as files:
        rows = list(csv.DictReader(io.StringIO(files.read('transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
        by_label = {r['label']: r for r in rows}
        for row in rows:
            if row['compression'] != '0':
                continue
            label = 'h40' + row['label'][row['label'].index('-d'):]
            label = label[:-len(row['profile'])] + 'plain'
            baseline = by_label[label]
            assert row['originalStable'] == baseline['originalStable'] == 'True'
            assert not row['error'] and not baseline['error']
            source_bytes = files.read(row['label'] + '.bmp')
            old = files.read(label + '.bmp')
            dib, depth = int(row['dib']), int(row['depth'])
            width = struct.unpack_from('<H' if dib == 12 else '<i', source_bytes, 18)[0]
            height = abs(struct.unpack_from('<H' if dib == 12 else '<i', source_bytes, 20 if dib == 12 else 22)[0])
            length = ((width * depth + 31) // 32 * 4) * height
            assert len(source_bytes) >= 14 + dib + length
            assert len(old) >= 54 + length
            assert source_bytes[14 + dib:14 + dib + length] == old[54:54 + length], row['label']
            for field in ('width', 'height', 'hasAlpha', 'error', 'bytes', 'sha256'):
                assert row[field] == baseline[field], (row['label'], field)
            rgba = files.read(row['label'] + '.rgba')
            assert rgba == files.read(label + '.rgba'), row['label']
            counts[row['dib']] += 1
            storage_bytes += length
            rgba_bytes += len(rgba)
    assert dict(counts) == {'40': 216, '52': 216, '56': 216, '108': 504, '124': 720, '12': 12, '64': 12}
    return {
        'scope': 'independent original archived BI_RGB storage/descriptor/full RGBA comparison; not production or SDK acceptance',
        'sourceCommit': source, 'runId': run, 'archiveSha256': original['archiveSha256'],
        'fixtures': sum(counts.values()), 'byDib': dict(counts),
        'fullSourceStorageBytesCompared': storage_bytes,
        'fullOriginalRgbaBytesCompared': rgba_bytes,
        'baselineRule': 'INFO40 with the same dimensions/depth/direction/pattern/gap; metadata replaced by plain',
        'observation': 'Bytes at 14+DIB and full original dimensions/hasAlpha/RGBA match INFO40 for all 1896 BI_RGB inputs, including NPOT and 792 bounded metadata profiles.',
        'limits': 'Recorded corpus only; malformed/other OS2 storage and all extended-DIB bitfields remain separate boundaries. No codec executed.'
    }


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = compare(args.root.resolve())
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited BI_RGB layouts: 1896 complete source storage/original RGBA pairs match INFO40')
