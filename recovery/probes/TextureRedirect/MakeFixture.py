"""Deterministic 256x256 uncompressed top-left BGRA TGA fixture, no SDK assets."""
import pathlib
import struct
import sys
p = pathlib.Path(sys.argv[1])
header = struct.pack('<BBBHHBHHHHBB', 0, 0, 2, 0, 0, 0, 0, 0, 256, 256, 32, 0x28)
p.write_bytes(header + b''.join(bytes((x % 256, y % 256, (x ^ y) % 256, 255))
                              for y in range(256) for x in range(256)))
