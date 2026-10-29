#!/usr/bin/env python3
"""Preserve the original raw-DIB dispatch bytes; no native execution."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--root', type=Path, default=ROOT)
parser.add_argument('--out', type=Path, required=True)
args = parser.parse_args()
path = args.root / 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll'
data = path.read_bytes()
assert hashlib.sha256(data).hexdigest() == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
pe = struct.unpack_from('<I', data, 0x3c)[0]
sections = struct.unpack_from('<H', data, pe + 6)[0]
optional_size = struct.unpack_from('<H', data, pe + 20)[0]
image_base = struct.unpack_from('<I', data, pe + 24 + 28)[0]
def offset(rva):
    for index in range(sections):
        at = pe + 24 + optional_size + index * 40
        size, address, raw_size, raw_at = struct.unpack_from('<IIII', data, at + 8)
        if address <= rva < address + max(size, raw_size):
            assert rva - address < raw_size
            return raw_at + rva - address
    raise AssertionError('Unmapped RVA')
args.out.mkdir(parents=True, exist_ok=True)
start, end = 0x16804, 0x16a9f
body = data[offset(start):offset(end - 1) + 1]
assert len(body) == end - start and body[-3:] == b'\xc2\x10\x00'
(args.out / 'ImageDispatcher.bin').write_bytes(body)
listing = subprocess.run(['objdump', '-d', '-Mintel', '--start-address=' + hex(image_base + start),
                          '--stop-address=' + hex(image_base + end), str(path)], check=True, capture_output=True, text=True).stdout
(args.out / 'ImageDispatcher.asm').write_text(listing)
table = data[offset(0x16aa0):offset(0x16aa0) + 36]
entries = [value - image_base for value in struct.unpack('<9I', table)]
assert entries[0] == 0x168d3 and entries[6] == 0x1694b
assert data[offset(0x16953)] == 0xe8
assert 0x16958 + struct.unpack_from('<i', data, offset(0x16953) + 1)[0] == 0x13f3d
(args.out / 'FormatJumpTable.bin').write_bytes(table)
manifest = dict(scope='read-only original raw-DIB dispatch; no runtime or production acceptance',
                originalSha256=hashlib.sha256(data).hexdigest(), imageBase=hex(image_base),
                dispatcher=dict(startRva=hex(start), endExclusiveRva=hex(end), fileOffset=hex(offset(start)),
                                bytes=len(body), sha256=hashlib.sha256(body).hexdigest()),
                jumpTable=dict(rva='0x16aa0', bytes=36, sha256=hashlib.sha256(table).hexdigest(), entries=[hex(x) for x in entries]),
                rawDib=dict(formatIndex=6, branchRva='0x1694b', parserCallRva='0x16953', parserRva='0x13f3d',
                            observation='Raw DIB passes the original data pointer/size directly to the same DIB parser called by BMP after stripping14 bytes.'))
(args.out / 'manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
print('Preserved original raw-DIB dispatch and exact format6 call to parser0x13f3d')
