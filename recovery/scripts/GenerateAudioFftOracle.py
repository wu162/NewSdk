#!/usr/bin/env python3
"""Original native FFT byte oracle; no candidate FFT is imported.

Pinned original DLL, Unicorn 2.1.4, x87 precision/rounding 0x027f.
"""
import argparse
import base64
import gzip
import hashlib
import pathlib
import random
import struct

import pefile
import unicorn
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32, UC_HOOK_CODE
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_ECX, UC_X86_REG_FPCW

EXPECTED_SHA = 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'


class OriginalFft:
    def __init__(self, dll):
        raw = pathlib.Path(dll).read_bytes()
        if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
            raise ValueError('Original DLL fingerprint changed')
        if unicorn.__version__ != '2.1.4':
            raise ValueError('Use Unicorn 2.1.4')
        self.cpu = Uc(UC_ARCH_X86, UC_MODE_32)
        self.cpu.mem_map(0x10000000, 0x140000)
        self.cpu.mem_write(0x10000000, pefile.PE(data=raw).get_memory_mapped_image())
        for address in (0x20000000, 0x30000000, 0x40000000, 0x50000000):
            self.cpu.mem_map(address, 0x100000)
        self.twiddles = {}
        self.direction = None
        self.cpu.hook_add(UC_HOOK_CODE, self.capture_twiddle, begin=0x1004ef03, end=0x1004ef03)

    def capture_twiddle(self, cpu, address, size, data):
        esp = cpu.reg_read(UC_X86_REG_ESP)
        width = struct.unpack('<I', cpu.mem_read(esp + 0x1c, 4))[0]
        a = bytes(cpu.mem_read(esp + 0x10, 4))
        b = bytes(cpu.mem_read(esp + 0x24, 4))
        key = (self.direction, width)
        pair = (a, b)
        if key in self.twiddles and self.twiddles[key] != pair:
            raise ValueError('Original stage constants were inconsistent')
        self.twiddles[key] = pair

    def encode(self, real, imag, order, direction):
        n = 1 << order
        cpu = self.cpu
        self.direction = direction
        cpu.mem_write(0x30000000, struct.pack('<%df' % n, *real))
        cpu.mem_write(0x40000000, struct.pack('<%df' % n, *imag))
        cpu.mem_write(0x50001000, struct.pack('<3I', 0x30000000, 0x40000000, order))
        cpu.mem_write(0x200f0000, struct.pack('<3I', 0x50000000, order, direction))
        cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
        cpu.reg_write(UC_X86_REG_ECX, 0x50001000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x027f)
        cpu.emu_start(0x1004eca0, 0x50000000, count=20000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x200f000c:
            raise ValueError('Original FFT did not return normally')
        return bytes(cpu.mem_read(0x30000000, n * 4)), bytes(cpu.mem_read(0x40000000, n * 4))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dll', default='SDK/tools/BinaryAssetBuilder.AudioCompiler.dll')
    parser.add_argument('--output', default='recovery/evidence/audio/native-fft/original.tsv.gz')
    parser.add_argument('--twiddles', default='recovery/evidence/audio/native-fft/twiddles.tsv')
    args = parser.parse_args()
    oracle = OriginalFft(args.dll)
    lines = ['# originalDllSha256=' + EXPECTED_SHA + '; nativeRva=0x4eca0; fpuControl=0x027f; oracle=unicorn-2.1.4']
    rng = random.Random(0x4eca0)
    for order in range(1, 14):
        n = 1 << order
        fixtures = [('impulse', [1.0] + [0.0] * (n - 1), [0.0] * n),
                    ('dc', [0.25] * n, [-0.125] * n),
                    ('complex-noise', [rng.uniform(-1, 1) for _ in range(n)], [rng.uniform(-1, 1) for _ in range(n)])]
        for direction in (0, 1):
            for name, real, imag in fixtures:
                real_bytes = struct.pack('<%df' % n, *real)
                imag_bytes = struct.pack('<%df' % n, *imag)
                real = list(struct.unpack('<%df' % n, real_bytes))
                imag = list(struct.unpack('<%df' % n, imag_bytes))
                out_real, out_imag = oracle.encode(real, imag, order, direction)
                arrays = [base64.b64encode(x).decode() for x in (real_bytes, imag_bytes, out_real, out_imag)]
                lines.append('\t'.join([name, str(order), str(direction)] + arrays))
    path = pathlib.Path(args.output)
    path.parent.mkdir(parents=True, exist_ok=True)
    # Fixed mtime makes regenerated binary evidence deterministic.
    with path.open('wb') as raw:
        with gzip.GzipFile(filename='', mode='wb', fileobj=raw, mtime=0) as stream:
            stream.write(('\n'.join(lines) + '\n').encode())
    twiddles = ['# Original FFT stage float bits; same DLL/FPU fingerprint as original.tsv.gz']
    for (direction, width), (a, b) in sorted(oracle.twiddles.items()):
        twiddles.append('%d\t%d\t%08x\t%08x' % (direction, width, struct.unpack('<I', a)[0], struct.unpack('<I', b)[0]))
    pathlib.Path(args.twiddles).write_text('\n'.join(twiddles) + '\n')
    if len(lines) != 79 or len(oracle.twiddles) != 26:
        raise ValueError('Incomplete FFT matrix')
    print('PASS original FFT oracle: 78 cases, orders 1..13, both directions, 26 native stage constants')


if __name__ == '__main__':
    main()
