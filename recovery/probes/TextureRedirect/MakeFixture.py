"""Deterministic 256/512-square uncompressed top-left BGRA TGA fixture, no SDK assets."""
import pathlib
import struct
import sys
p = pathlib.Path(sys.argv[1])
size = int(sys.argv[2]) if len(sys.argv) > 2 else 256
if size not in (256, 512):
    raise ValueError("Only the valid terrain tile and known wrong-size negative fixture are allowed")
header = struct.pack('<BBBHHBHHHHBB', 0, 0, 2, 0, 0, 0, 0, 0, size, size, 32, 0x28)
p.write_bytes(header + b''.join(bytes((x % 256, y % 256, (x ^ y) % 256, 255))
                              for y in range(size) for x in range(size)))
