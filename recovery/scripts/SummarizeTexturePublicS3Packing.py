#!/usr/bin/env python3
"""Analyze fixed initialized states; normalized state packing is not DDS acceptance."""
import argparse
import hashlib
import json
import struct
import zipfile
from pathlib import Path

STATE_SIZE = 0x3850
RECORD_SIZE = 4 + 3 * STATE_SIZE

def u32(data, offset):
    return struct.unpack_from('<I', data, offset)[0]

def packed(state):
    fmt, flag, count = struct.unpack_from('<III', state)
    levels = u32(state, 0x3034)
    if fmt not in (0, 1) or flag != 0 or not 1 <= count <= 16 or levels not in (3, 4):
        raise ValueError('Unsupported explicitly initialized state')
    endpoints = []
    for offset in (0x3038, 0x3044):
        r, g, b = struct.unpack_from('<III', state, offset)
        if r > 31 or g > 63 or b > 31:
            raise ValueError('RGB565 endpoint out of range')
        endpoints.append((r << 11) | (g << 5) | b)
    indices = list(struct.unpack_from('<' + 'I' * count, state, 0x3050))
    if any(index >= levels and not (fmt == 0 and levels == 3 and index == 3) for index in indices):
        raise ValueError('Colour index out of range')
    first, second = endpoints
    if first == second:
        if fmt == 1:
            second = (second + 1) & 0xffff
            levels = 3
            indices = [0] * count
        else:
            first = (first + 1) & 0xffff
            indices = [1] * count
    swap = (second < first) == (levels == 3)
    if swap:
        first, second = second, first
        indices = [index ^ 1 if levels == 4 or index < 2 else index for index in indices]
    return struct.pack('<HH', first, second) + bytes(indices)

def analyze(path):
    result = {'archiveSha256': hashlib.sha256(path.read_bytes()).hexdigest(),
              'status': 'state-packing-analysis', 'productionAccepted': False,
              'scope': 'Full fixed initialized states, retaining raw differences; compact indices only, no dimensions/alpha/DDS claim',
              'configurations': []}
    with zipfile.ZipFile(path) as archive:
        for name in sorted(n for n in archive.namelist() if n.endswith('.records')):
            with archive.open(name) as stream:
                header = stream.read(4)
                count = struct.unpack('<I', header)[0]
                if archive.getinfo(name).file_size != 4 + count * RECORD_SIZE:
                    raise ValueError('Record length/count mismatch')
                raw_matches = packing_matches = 0
                differences = []
                for ordinal in range(count):
                    record = stream.read(RECORD_SIZE)
                    if len(record) != RECORD_SIZE:
                        raise ValueError('Truncated state record')
                    mode = u32(record, 0)
                    source = record[4:4 + STATE_SIZE]
                    original = record[4 + STATE_SIZE:4 + 2 * STATE_SIZE]
                    candidate = record[4 + 2 * STATE_SIZE:]
                    raw_same = original == candidate
                    pack_same = packed(original) == packed(candidate)
                    raw_matches += raw_same
                    packing_matches += pack_same
                    if not raw_same:
                        differences.append({'ordinal': ordinal, 'mode': mode, 'format': u32(source, 0),
                                            'inputSha256': hashlib.sha256(source).hexdigest(),
                                            'firstRawDifference': next(i for i, (a, b) in enumerate(zip(original, candidate)) if a != b),
                                            'packedEqual': pack_same})
                if stream.read(1):
                    raise ValueError('Trailing record data')
                result['configurations'].append({'file': name, 'fixtures': count,
                                                'rawMatches': raw_matches, 'packingMatches': packing_matches,
                                                'rawDifferences': differences})
    return result

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('archive', type=Path)
    parser.add_argument('--out', type=Path)
    args = parser.parse_args()
    report = analyze(args.archive)
    text = json.dumps(report, indent=2) + '\n'
    if args.out:
        args.out.write_text(text)
    else:
        print(text, end='')
