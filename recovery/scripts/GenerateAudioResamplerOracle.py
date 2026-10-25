#!/usr/bin/env python3
"""Execute pinned original native source-window/filter/derivative code.

No recovered implementation is imported. Outputs exact binary32 arrays and
checks ABI return-stack balance. Regenerate with pefile and Unicorn 2.1.4.
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
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_EAX, UC_X86_REG_EBX, UC_X86_REG_ESI, UC_X86_REG_EDI, UC_X86_REG_EDX, UC_X86_REG_FPCW

EXPECTED_SHA = 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'
N = 8192

def floats(values):
    return struct.pack('<%df' % len(values), *values)

def b64(value):
    return base64.b64encode(value).decode('ascii')

class OriginalResampler:
    def __init__(self, dll):
        raw = pathlib.Path(dll).read_bytes()
        if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
            raise ValueError('Original DLL fingerprint changed')
        if unicorn.__version__ != '2.1.4':
            raise ValueError('Use Unicorn 2.1.4')
        self.cpu = Uc(UC_ARCH_X86, UC_MODE_32)
        self.cpu.mem_map(0x10000000, 0x140000)
        self.cpu.mem_write(0x10000000, pefile.PE(data=raw).get_memory_mapped_image())
        self.cpu.mem_map(0x20000000, 0x200000)
        self.cpu.mem_map(0x30000000, 0x200000)

    def invoke(self, rva, args=(), registers=()):
        cpu = self.cpu
        cpu.mem_write(0x200f0000, struct.pack('<%dI' % (len(args) + 1), 0x20000000, *args))
        cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x027f)
        for reg, value in registers:
            cpu.reg_write(reg, value & 0xffffffff)
        cpu.emu_start(0x10000000 + rva, 0x20000000, count=40000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x200f0004:
            raise ValueError('Original native routine did not return normally')

    def source(self, raw, position, factor):
        self.cpu.mem_write(0x30000000, raw)
        self.cpu.mem_write(0x30010000, floats([-123.5] * N))
        self.invoke(0x41f40, (0x30000000, len(raw) // 2),
                    ((UC_X86_REG_EAX, position), (UC_X86_REG_EDI, factor), (UC_X86_REG_EBX, 0x30010000)))
        return bytes(self.cpu.mem_read(0x30010000, N * 4))

    def process(self, real, imaginary, coefficients, factor):
        self.cpu.mem_write(0x30010000, real)
        self.cpu.mem_write(0x30020000, imaginary)
        self.cpu.mem_write(0x30030000, coefficients)
        self.cpu.mem_write(0x30040000, struct.pack('<3I', 0x30010000, 0x30020000, 13))
        self.invoke(0x41fa0, (0x30030000,), ((UC_X86_REG_EAX, factor), (UC_X86_REG_ESI, 0x30040000)))
        return (bytes(self.cpu.mem_read(0x30010000, N * 4)), bytes(self.cpu.mem_read(0x30020000, N * 4)))

    def derivatives(self, raw, support):
        self.cpu.mem_write(0x30010000, raw)
        self.cpu.mem_write(0x30020000, floats([-123.5] * N))
        self.invoke(0x42250, (0x30010000, 0x30020000), ((UC_X86_REG_EDX, support),))
        return bytes(self.cpu.mem_read(0x30020000, N * 4))


def fixtures(oracle):
    rng = random.Random(0x41fa0)
    for pattern in ('impulse', 'dc', 'extrema', 'noise'):
        samples = ([32767] + [0] * 8999 if pattern == 'impulse' else
                   [12345] * 9000 if pattern == 'dc' else
                   [-32768, 32767] * 4500 if pattern == 'extrema' else
                   [rng.randrange(-32768, 32768) for _ in range(9000)])
        raw = struct.pack('<9000h', *samples)
        for factor in (1, 2):
            for position in (-513, -1, 0, 4095, 8191, 9000):
                yield ['source', pattern, str(factor), str(position), b64(raw), b64(oracle.source(raw, position, factor))]
    for factor in (1, 2):
        for pattern in ('impulse', 'dc', 'alternating', 'noise'):
            real = floats([1.] + [0.] * (N - 1) if pattern == 'impulse' else
                          [.25] * N if pattern == 'dc' else
                          [32767., -32768.] * (N // 2) if pattern == 'alternating' else
                          [rng.uniform(-32768, 32768) for _ in range(N)])
            imaginary = floats([0.] * N if pattern == 'impulse' else
                               [-.125] * N if pattern == 'dc' else
                               [-32768., 32767.] * (N // 2) if pattern == 'alternating' else
                               [rng.uniform(-32768, 32768) for _ in range(N)])
            for coeff_pattern in ('identity', 'signed-noise'):
                coefficients = floats([1.] * N if coeff_pattern == 'identity' else
                                      [rng.uniform(-2, 2) for _ in range(N)])
                out_real, out_imaginary = oracle.process(real, imaginary, coefficients, factor)
                yield ['process', pattern + '-' + coeff_pattern, str(factor), '0', b64(real), b64(imaginary), b64(coefficients), b64(out_real), b64(out_imaginary)]
                for support in (1, 256, 512):
                    for channel, filtered in (('real', out_real), ('imaginary', out_imaginary)):
                        yield ['derivatives', pattern + '-' + coeff_pattern + '-' + channel + '-f' + str(factor), '0', str(support), b64(filtered), b64(oracle.derivatives(filtered, support))]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dll', default='SDK/tools/BinaryAssetBuilder.AudioCompiler.dll')
    parser.add_argument('--output', default='recovery/evidence/audio/native-resampler/filter.tsv.gz')
    args = parser.parse_args()
    from GenerateAudioResamplerKernelOracle import OriginalKernel, fixtures as kernel_fixtures
    oracle = OriginalKernel(args.dll)
    rows = ['# originalDllSha256=' + EXPECTED_SHA + '; nativeRvas=0x41f40,0x41fa0,0x42250,0x41a90,0x42480; fpuControl=0x027f; oracle=unicorn-2.1.4; untouchedSentinel=-123.5']
    counts = {}
    from itertools import chain
    for row in chain(fixtures(oracle), kernel_fixtures(oracle)):
        counts[row[0]] = counts.get(row[0], 0) + 1
        rows.append('\t'.join(row))
    if counts != {'source': 48, 'process': 16, 'derivatives': 96, 'coefficients': 18, 'kernel': 80}:
        raise ValueError('Incomplete matrix: ' + str(counts))
    path = pathlib.Path(args.output)
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open('wb') as raw:
        with gzip.GzipFile(filename='', mode='wb', fileobj=raw, mtime=0) as stream:
            stream.write(('\n'.join(rows) + '\n').encode('ascii'))
    print('PASS original native resampler filter oracle:', counts)

if __name__ == '__main__':
    main()
