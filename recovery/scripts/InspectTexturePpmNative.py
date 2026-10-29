#!/usr/bin/env python3
"""Preserve original PC PPM parser bytes and pinned public parser source identity."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
parser.add_argument('--out', type=Path, required=True)
args = parser.parse_args()
path = args.root / 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll'
data = path.read_bytes()
sha = lambda b: hashlib.sha256(b).hexdigest()
assert sha(data) == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
pe = struct.unpack_from('<I', data, 0x3c)[0]
sections = struct.unpack_from('<H', data, pe + 6)[0]
optional = struct.unpack_from('<H', data, pe + 20)[0]
base = struct.unpack_from('<I', data, pe + 52)[0]


def offset(rva):
    for index in range(sections):
        size, address, raw_size, raw_at = struct.unpack_from('<IIII', data, pe + 24 + optional + index * 40 + 8)
        if address <= rva < address + max(size, raw_size):
            assert rva - address < raw_size
            return raw_at + rva - address
    raise AssertionError('Unmapped RVA')


assert struct.unpack_from('<I', data, offset(0x16aa0) + 5 * 4)[0] == base + 0x168e2
assert data[offset(0x168ea)] == 0xe8
assert 0x168ef + struct.unpack_from('<i', data, offset(0x168eb))[0] == 0x15240
start, end = 0x15240, 0x154b0
body = data[offset(start):offset(end - 1) + 1]
assert len(body) == end - start
args.out.mkdir(parents=True, exist_ok=True)
(args.out / 'PpmParser.bin').write_bytes(body)
listing = subprocess.run(['objdump', '-d', '-Mintel', '--start-address=' + hex(base + start),
                          '--stop-address=' + hex(base + end), str(path)], check=True, capture_output=True, text=True).stdout
(args.out / 'PpmParser.asm').write_text(listing)
public = args.root / 'recovery/reference/ijg6b/rdppm.c'
provenance = json.loads((public.parent / 'PROVENANCE.json').read_text())
assert sha(public.read_bytes()) == provenance['files']['rdppm.c']
manifest = dict(scope='static original PPM dispatch/parser bytes and public-source candidate only; no original runtime calls or recovered/SDK acceptance',
                originalSha256=sha(data), parserStartRva=hex(start), parserEndExclusiveRva=hex(end),
                parserBytes=len(body), parserSha256=sha(body),
                dispatch=dict(formatIndex=5, branchRva='0x168e2', callRva='0x168ea', parserRva='0x15240'),
                publicCandidate=dict(file='recovery/reference/ijg6b/rdppm.c', sha256=sha(public.read_bytes()),
                                     upstreamArchive=provenance['upstream'], archiveSha256=provenance['archiveSha256'],
                                     state='unmodified source already retained; no PPM candidate implemented or accepted'),
                staticObservations=[
                    'Only P3 and P6 signatures enter this parser; runtime LoadImageA behaviour still needs full gold.',
                    'Original decimal scanner requires digits and handles comments between tokens; ASCII divides sample*255 by maxval with integer truncation.',
                    'P6 rejects maxval>255, skips one separator or CRLF, and consumes exactly three bytes per pixel; no standard 16-bit fallback.',
                    'Original parser rejects zero width/height/maxval but does not bound allocation arithmetic or truncated binary reads; quarantine overflow/truncation before probing.',
                    'hasAlpha, channel order and NPOT conversion remain runtime questions, not inferred acceptance.'])
(args.out / 'manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
print('Preserved original PPM parser624 bytes and pinned IJG6b candidate identity; static only')
