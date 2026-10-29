#!/usr/bin/env python3
"""Audit original indexed BMP probe storage/results without executing a codec."""
import argparse
from collections import Counter, defaultdict
import csv
import hashlib
import io
import json
from pathlib import Path
import struct
import zipfile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def expected_cases():
    cases = {}

    def add(kind, dib, depth, size, top, pattern=0, gap=0, alpha=0):
        width, height = size
        label = (f'{"control" if kind == "control" else "index"}-h{dib}-d{depth}-'
                 f'{width}x{height}-top{top}-p{pattern}-g{gap}-a{alpha}')
        assert label not in cases
        cases[label] = (kind, dib, depth, width, height, top, pattern, gap, alpha)

    for size in ((16, 8), (7, 5), (1, 1)):
        for dib in (12, 40, 52, 56, 64, 108, 124):
            for depth in ((24,) if dib in (12, 64) else (16, 24, 32)):
                for top in ((False,) if dib in (12, 64) else (False, True)):
                    add('control', dib, depth, size, top)
    for dib in (12, 40, 52, 56, 64, 108, 124):
        for depth in (1, 4, 8):
            for size in ((16, 8), (7, 5), (1, 1)):
                for top in ((False,) if dib == 12 else (False, True)):
                    for pattern in (0, 1, 2):
                        for gap in (0, 3, 17):
                            for alpha in ((0,) if dib == 12 else (0, 255)):
                                add('indexed', dib, depth, size, top, pattern, gap, alpha)
    assert len(cases) == 2121
    return cases


def audit_storage(data, case):
    kind, dib, depth, width, height, top, pattern, gap, alpha = case
    assert data[:2] == b'BM' and struct.unpack_from('<I', data, 2)[0] == len(data)
    assert struct.unpack_from('<I', data, 14)[0] == dib
    if dib == 12:
        assert struct.unpack_from('<4H', data, 18) == (width, height, 1, depth)
    else:
        assert struct.unpack_from('<iiHHI', data, 18) == (width, -height if top else height, 1, depth, 0)
    entries = 1 << depth if depth <= 8 else 0
    palette_width = 3 if dib == 12 else 4
    start = 14 + dib
    offset = start + entries * palette_width + gap
    stride = (width * depth + 31) // 32 * 4
    assert struct.unpack_from('<I', data, 10)[0] == offset
    assert len(data) == offset + stride * height
    if dib != 12:
        assert struct.unpack_from('<I', data, 46)[0] == entries
    for index in range(entries):
        palette = bytes(((index * 47 + 71) & 255, (index * 13 + 59) & 255,
                         (index * 23 + 17) & 255, alpha))[:palette_width]
        assert data[start + index * palette_width:start + (index + 1) * palette_width] == palette
    assert data[offset - gap:offset] == bytes((index * 17 + 111) & 255 for index in range(gap))
    if kind == 'indexed':
        for fy in range(height):
            y = fy if top else height - 1 - fy
            for x in range(width):
                index = ((data[offset + fy * stride + x * depth // 8] >>
                          (8 - depth - x * depth % 8)) & ((1 << depth) - 1))
                expected = 0 if pattern == 1 else ((1 << depth) - 1 if pattern == 2 else
                                                  ((y * width + x) * 71 + 17) & ((1 << depth) - 1))
                assert index == expected


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/bmp-indexed-header-load' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['sourceCommit'] == source and str(receipt['runId']) == run
    assert str(receipt['runAttempt']) == '1' and receipt['status'] == 'probe-only'
    assert receipt['probeOnly'] is True and receipt['productionAccepted'] is False and receipt['sdkValidated'] is False
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    archive = (directory / 'samples.zip').read_bytes()
    assert sha(archive) == receipt['samplesSha256']
    expected = expected_cases()
    totals, errors = Counter(), Counter()
    groups = defaultdict(Counter)
    unstable = []
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = set(files.namelist())
        assert len(names) == len(files.namelist())
        transcript = files.read('transcript.tsv')
        rows = list(csv.DictReader(io.StringIO(transcript.decode('utf-8-sig')), delimiter='\t'))
        external = list(csv.DictReader((directory / 'transcript.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
        assert rows == external and len(rows) == len({r['label'] for r in rows}) == len(expected)
        assert {r['label'] for r in rows} == expected.keys()
        members = {'transcript.tsv', 'progress.tsv'}
        assert files.read('progress.tsv').decode('utf-8-sig').splitlines() == ['label\tphase', 'all\tcompleted']
        for row in rows:
            label = row['label']
            kind, dib, depth, width, height, top, pattern, gap, alpha = expected[label]
            assert row['kind'] == kind and row['topDown'] == str(top)
            for field, value in [('dib', dib), ('depth', depth), ('sourceWidth', width),
                                 ('sourceHeight', height), ('pattern', pattern), ('gap', gap),
                                 ('paletteReserved', alpha), ('paletteEntryBytes', 3 if dib == 12 else 4),
                                 ('paletteEntries', 1 << depth if depth <= 8 else 0)]:
                assert int(row[field]) == value, (label, field)
            members.add(label + '.bmp')
            source_bytes = files.read(label + '.bmp')
            assert sha(source_bytes) == row['inputSha256']
            audit_storage(source_bytes, expected[label])
            totals['sourceBytes'] += len(source_bytes)
            outcomes = []
            for prefix, suffix in [('', '.rgba'), ('repeat', '.repeat.rgba')]:
                def field(name):
                    return row[prefix + name[0].upper() + name[1:]] if prefix else row[name]
                error = field('error')
                rgba = None
                if error:
                    assert not field('sha256') and int(field('bytes')) == 0
                    assert int(field('width')) == int(field('height')) == 0 and field('hasAlpha') == 'False'
                    assert label + suffix not in names
                    errors[error] += 1
                else:
                    members.add(label + suffix)
                    rgba = files.read(label + suffix)
                    assert len(rgba) == int(field('bytes')) == int(field('width')) * int(field('height')) * 4
                    assert len(rgba) > 0 and sha(rgba) == field('sha256') and field('hasAlpha') in ('True', 'False')
                    totals['repeatRgbaBytes' if prefix else 'originalRgbaBytes'] += len(rgba)
                outcomes.append((error, field('width'), field('height'), field('hasAlpha'), rgba))
            stable = outcomes[0] == outcomes[1]
            assert row['originalStable'] == str(stable)
            outcome = ('decoded' if not row['error'] else 'rejected') + ('/repeat-equal' if stable else '/repeat-different')
            totals[kind + '/' + outcome] += 1
            groups[f'{kind}/h{dib}/d{depth}'][outcome] += 1
            if not stable:
                unstable.append(label)
            if kind == 'control':
                assert not row['error'] and stable
                assert int(row['width']) == 1 << (width - 1).bit_length()
                assert int(row['height']) == 1 << (height - 1).bit_length()
        assert names == members, 'Missing or unexpected archive members'
    indexed = [r for r in rows if r['kind'] == 'indexed']
    assert len(indexed) == receipt['fixtureCount'] == 2025
    assert len(rows) - len(indexed) == receipt['controlCount'] == 96
    assert len(rows) == receipt['sampleCount'] == 2121
    assert sum(not r['error'] for r in indexed) == receipt['decodedCount']
    assert sum(bool(r['error']) for r in indexed) == receipt['rejectedCount']
    assert sum(r['originalStable'] == 'True' for r in indexed) == receipt['stableOriginals']
    assert sum(r['originalStable'] == 'False' for r in indexed) == receipt['unstableOriginals']
    assert receipt['originalCalls'] == 4243 and receipt['fixtureOriginalCalls'] == 4242 and receipt['initializationControlCalls'] == 1
    return dict(scope='independent original BMP indexed archive storage/full RGBA/error audit; no codec or production acceptance',
                sourceCommit=source, runId=run, archiveSha256=sha(archive), samples=len(rows),
                indexedFixtures=len(indexed), controls=96, totals=dict(totals), originalErrors=dict(errors),
                groups={k: dict(v) for k, v in sorted(groups.items())}, unstableLabels=unstable,
                determinismLimit='Two consecutive calls in one original process only; no cross-process determinism claim.')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve(), args.run_id, args.source_commit)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result['totals'], sort_keys=True))
