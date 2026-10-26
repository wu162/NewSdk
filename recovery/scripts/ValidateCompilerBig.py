#!/usr/bin/env python3
"""Strict original MakeBig BIG4 contract and exact current-artifact inventory."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import tempfile


def digest(data):
    return hashlib.sha256(data).hexdigest()


def read_archive(path):
    data = Path(path).read_bytes()
    if len(data) < 24 or data[:4] != b'BIG4':
        raise ValueError('Expected original MakeBig BIG4 header')
    total, = struct.unpack_from('<I', data, 4)
    count, header = struct.unpack_from('>II', data, 8)
    if total != len(data) or not 0 < count <= 1000000 or not 24 <= header <= total:
        raise ValueError('BIG4 length/count/header bounds differ')
    position = 16
    entries = []
    names = set()
    next_offset = header
    for _ in range(count):
        if position + 9 > header - 8:
            raise ValueError('Truncated BIG4 index')
        offset, length = struct.unpack_from('>II', data, position)
        position += 8
        end = data.find(b'\0', position, header - 8)
        if end < position:
            raise ValueError('Unterminated BIG4 member name')
        name = data[position:end].decode('utf-8', errors='strict')
        position = end + 1
        if not name or name != name.lower() or '/' in name or ':' in name or any(p in ('', '.', '..') for p in name.split('\\')) or name in names:
            raise ValueError('Noncanonical/duplicate BIG4 member name')
        if offset != next_offset or offset + length > total:
            raise ValueError('BIG4 data overlap/gap/order/bounds differ')
        names.add(name)
        payload = data[offset:offset + length]
        entries.append({'name': name, 'offset': offset, 'bytes': length, 'sha256': digest(payload), 'data': payload})
        next_offset += length
    if data[position:header] != b'L219' + b'\0' * 4 or next_offset != total:
        raise ValueError('BIG4 L219 header trailer or complete payload boundary differs')
    return {'magic': 'BIG4', 'bytes': total, 'headerBytes': header, 'members': entries, 'sha256': digest(data)}


def validate(path, expected):
    result = read_archive(path)
    actual = {e['name']: e for e in result['members']}
    names = {e['name'] for e in expected}
    if len(names) != len(expected) or names != set(actual):
        raise ValueError('BIG4 inventory differs from exact current build artifacts')
    for entry in expected:
        data = Path(entry['source']).read_bytes()
        if len(data) != entry['bytes'] or digest(data) != entry['sha256'] or actual[entry['name']]['data'] != data:
            raise ValueError('BIG4 source/member bytes differ: ' + entry['name'])
    for entry in result['members']:
        del entry['data']
    return result


def self_test():
    with tempfile.TemporaryDirectory(prefix='big-contract-') as work:
        root = Path(work)
        name = b'data\\aptui\\a.bin'
        payload = b'\x01\x02\x03'
        header = 16 + 8 + len(name) + 1 + 8
        data = b'BIG4' + struct.pack('<I', header + len(payload)) + struct.pack('>II', 1, header) + struct.pack('>II', header, len(payload)) + name + b'\0L219' + b'\0' * 4 + payload
        source = root / 'a.bin'; source.write_bytes(payload)
        archive = root / 'fixture.big'; archive.write_bytes(data)
        expected = [{'name': name.decode(), 'source': str(source), 'bytes': len(payload), 'sha256': digest(payload)}]
        validate(archive, expected)
        checks = 1
        for start, replacement in ((0, b'BIGF'), (4, struct.pack('>I', len(data))), (12, struct.pack('<I', header)), (16, struct.pack('>I', header + 1)), (header - 8, b'BAD!'), (header, b'\xff')):
            changed = bytearray(data); changed[start:start + len(replacement)] = replacement; archive.write_bytes(changed)
            try:
                validate(archive, expected)
            except ValueError:
                checks += 1
            else:
                raise AssertionError('BIG parser accepted corrupted contract')
        print(json.dumps({'succeeded': True, 'checks': checks, 'scope': 'Independent BIG4 parser preflight only; no production writer/original tool execution'}))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--self-test', action='store_true')
    parser.add_argument('--archive', type=Path)
    parser.add_argument('--oracle', type=Path)
    parser.add_argument('--expected', type=Path)
    parser.add_argument('--output', type=Path)
    args = parser.parse_args()
    if args.self_test:
        self_test(); return
    expected = json.loads(args.expected.read_text(encoding='utf-8-sig'))
    product = validate(args.archive, expected)
    oracle = validate(args.oracle, expected)
    receipt = {'succeeded': True, 'product': product, 'originalMakeBig': oracle, 'exactMemberNamesAndRawContents': True, 'scope': 'Original MakeBig format and exact raw member comparison; original traversal order may differ'}
    args.output.write_text(json.dumps(receipt, indent=2) + '\n')


if __name__ == '__main__':
    main()
