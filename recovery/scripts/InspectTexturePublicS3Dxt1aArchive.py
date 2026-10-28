#!/usr/bin/env python3
"""Independently inspect complete experimental DXT1A records, without running a codec."""
import argparse
from collections import Counter
import hashlib
import json
import math
from pathlib import Path
import struct
import zipfile

class Reader:
    def __init__(self, data):
        self.data, self.offset = data, 0
    def take(self, size):
        if size < 0 or size > len(self.data) - self.offset:
            raise ValueError('Truncated or invalid record size')
        result = self.data[self.offset:self.offset + size]
        self.offset += size
        return result
    def number(self, fmt):
        return struct.unpack('<' + fmt, self.take(struct.calcsize('<' + fmt)))[0]
    def string(self):
        size = 0
        for shift in range(0, 35, 7):
            byte = self.number('B')
            size |= (byte & 0x7f) << shift
            if byte < 0x80:
                return self.take(size).decode('utf-8')
        raise ValueError('Invalid .NET string length')
    def boolean(self):
        value = self.number('B')
        if value not in (0, 1):
            raise ValueError('Invalid boolean')
        return bool(value)

def digest(data):
    return hashlib.sha256(data).hexdigest()

def dds_size(w, h, d, mips):
    total = 128
    while True:
        total += ((w + 3) // 4) * ((h + 3) // 4) * d * 8
        if not mips or (w == 1 and h == 1 and d == 1):
            return total
        w, h, d = max(1, w // 2), max(1, h // 2), max(1, d // 2)

def check_dds(data, w, h, d=1, volume=False, mips=False):
    def word(offset):
        return struct.unpack_from('<I', data, offset)[0]
    if len(data) != dds_size(w, h, d, mips):
        raise ValueError('DDS complete payload length mismatch')
    fields = {0: 0x20534444, 4: 124, 8: (0x8a1007 if mips else 0x881007) if volume else (0xa1007 if mips else 0x81007),
              12: h, 16: w, 20: ((w + 3) // 4) * ((h + 3) // 4) * 8,
              24: d if volume else 0, 28: max(w, h).bit_length() if mips else 0,
              76: 32, 80: 4, 84: 0x31545844, 108: 0x401008 if mips else 0x1000,
              112: 0x200000 if volume else 0}
    if any(word(offset) != value for offset, value in fields.items()):
        raise ValueError('Independent DDS header/layout mismatch')

def analyze_records(data):
    r = Reader(data)
    magic = r.number('I')
    if magic not in (0x31415844, 0x33415844, 0x32415844):
        raise ValueError('Unsupported record version')
    block_mode = magic != 0x32415844
    if block_mode and r.number('f') != 0.5:
        raise ValueError('Unexpected original alpha threshold')
    count = r.number('I')
    matches = dds = dds_matches = rejected = 0
    differences, shapes, categories = [], Counter(), Counter()
    for ordinal in range(count):
        label = r.string()
        w, h = r.number('i'), r.number('i')
        if w < 1 or h < 1 or (w & (w - 1)) or (h & (h - 1)):
            raise ValueError('Invalid power-of-two dimensions')
        if block_mode:
            size = r.number('i')
            if size != w * h * 4 or w not in (1, 2, 4) or h not in (1, 2, 4):
                raise ValueError('Incomplete block source')
            source = struct.unpack('<' + 'f' * size, r.take(size * 4))
            if any(not math.isfinite(value) or not 0 <= value <= 1 for value in source):
                raise ValueError('Invalid normalized float source')
            if not any(source[i] <= 0.5 for i in range(3, size, 4)):
                raise ValueError('Opaque source in transparent acceptance corpus')
            expected, actual = r.take(8), r.take(8)
            same = expected == actual
            matches += same
            shapes[f'{w}x{h}'] += 1
            if not same:
                differences.append({'ordinal': ordinal, 'label': label, 'kind': 'block'})
            if r.boolean():
                if magic == 0x33415844:
                    byte_size = r.number('i')
                    if byte_size != size:
                        raise ValueError('Incomplete byte source')
                    byte_source = r.take(byte_size)
                    normalized = tuple(struct.unpack('<f', struct.pack('<f', value / 255))[0] for value in byte_source)
                    if normalized != source:
                        raise ValueError('Byte/float source mismatch')
                original_dds, candidate_dds = r.take(136), r.take(136)
                check_dds(original_dds, w, h)
                check_dds(candidate_dds, w, h)
                if original_dds[128:] != expected or candidate_dds[128:] != actual:
                    raise ValueError('Float/byte wrapper payload mismatch')
                dds += 1
                dds_matches += original_dds == candidate_dds
                if original_dds != candidate_dds:
                    differences.append({"ordinal": ordinal, "label": label, "kind": "byte-dds"})
        else:
            depth = r.number('i')
            volume, mips, was_rejected = r.boolean(), r.boolean(), r.boolean()
            if depth < 1 or (depth & (depth - 1)) or (not volume and depth != 1):
                raise ValueError('Invalid depth')
            size = r.number('i')
            if size != w * h * depth * 4:
                raise ValueError('Incomplete DDS source')
            r.take(size)
            key = ('volume' if volume else '2d') + ('-mips' if mips else '-no-mips')
            categories[key + ('-rejected' if was_rejected else '-compared')] += 1
            if was_rejected:
                rejected += 1
            else:
                size = r.number('i')
                expected, actual = r.take(size), r.take(size)
                check_dds(expected, w, h, depth, volume, mips)
                check_dds(actual, w, h, depth, volume, mips)
                matches += expected == actual
                dds += 1
                dds_matches += expected == actual
                if expected != actual:
                    differences.append({'ordinal': ordinal, 'label': label, 'kind': 'full-dds'})
    if r.offset != len(data):
        raise ValueError('Trailing record data')
    return {'version': hex(magic), 'inputs': count, 'compared': count - rejected, 'matched': matches,
            'opaqueRejected': rejected, 'completeDDS': dds, 'ddsMatched': dds_matches,
            'recordSha256': digest(data), 'recordBytes': len(data), 'consumedBytes': r.offset,
            'byteInputsStored': magic != 0x31415844, 'shapes': dict(shapes),
            'categories': dict(categories), 'differences': differences}

def inspect(archive_path, receipt_path=None):
    data = archive_path.read_bytes()
    archive_sha = digest(data)
    receipt = json.loads(receipt_path.read_text()) if receipt_path else None
    if receipt and receipt['archiveSha256'] != archive_sha:
        raise ValueError('Archive hash differs from original receipt')
    result = {'status': 'offline-full-record-inspection', 'productionAccepted': False,
              'scope': 'Exact complete raw outputs and independent DDS layout; not codec execution, plugin or SDK acceptance',
              'archiveSha256': archive_sha, 'configurations': []}
    if receipt:
        result.update({key: receipt[key] for key in ('sourceCommit', 'runId', 'runAttempt')})
    with zipfile.ZipFile(archive_path) as archive:
        for name in sorted(n for n in archive.namelist() if n.endswith('.records')):
            result['configurations'].append({'file': name, **analyze_records(archive.read(name))})
        result['options'] = [{'file': n, 'bytes': archive.getinfo(n).file_size, 'sha256': digest(archive.read(n))}
                             for n in sorted(archive.namelist()) if n.endswith('.options')]
    return result

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('archive', type=Path)
    parser.add_argument('--receipt', type=Path)
    parser.add_argument('--out', type=Path)
    args = parser.parse_args()
    output = json.dumps(inspect(args.archive, args.receipt), indent=2) + '\n'
    if args.out:
        args.out.write_text(output)
    else:
        print(output, end='')
