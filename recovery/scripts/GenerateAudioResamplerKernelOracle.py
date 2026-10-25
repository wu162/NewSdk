#!/usr/bin/env python3
"""Pinned original coefficient+whole resampler kernel oracle, no candidate import."""
import argparse
import base64
import gzip
import pathlib
import random
import struct
from GenerateAudioResamplerOracle import OriginalResampler, EXPECTED_SHA, floats, b64
from unicorn import UC_HOOK_CODE
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_EIP, UC_X86_REG_EAX, UC_X86_REG_ECX

F = lambda x: struct.unpack('<f', struct.pack('<f', x))[0]

def profile(factor, frequencies, amplitudes, gain=1., invert=False, phase_rate=1.):
    result = bytearray(0x338)
    struct.pack_into('<iiiffi', result, 0, 256, int(invert), factor, phase_rate, gain, len(frequencies))
    struct.pack_into('<%df' % len(frequencies), result, 0x18, *frequencies)
    struct.pack_into('<%df' % len(amplitudes), result, 0x1a8, *amplitudes)
    return bytes(result)

def rate_profile(old, new):
    ratio = F(F(new) / F(old))
    if new > old:
        factor, phase, frequencies, amplitudes = 2, F(ratio * .5), [0., F(.95), 1.], [1., 1., 0.]
    elif new * 2 >= old:
        factor, phase = 2, F(ratio * .5)
        frequencies, amplitudes = [0., F(ratio * F(.95)), ratio, 1.], [1., 1., 0., 0.]
    else:
        factor, phase = 1, ratio
        frequencies, amplitudes = [0., F(ratio * F(.95)), ratio, 1.], [1., 1., 0., 0.]
    return profile(factor, frequencies, amplitudes, phase_rate=phase)

class OriginalKernel(OriginalResampler):
    def __init__(self, dll):
        super().__init__(dll)
        self.cpu.mem_map(0x30200000, 0x1000000)
        for rva in (0x39630, 0x39690, 0x26fd0):
            self.cpu.hook_add(UC_HOOK_CODE, self.service, begin=0x10000000 + rva, end=0x10000000 + rva)
        self.allocation = 0x30200000

    def service(self, cpu, address, size, data):
        # Only host-service calls are replaced. All signal processing instructions
        # (including original FFT, FCOS, interpolation and FISTP) run unchanged.
        if address not in (0x10039630, 0x10039690, 0x10026fd0):
            return
        esp = cpu.reg_read(UC_X86_REG_ESP)
        ret = struct.unpack('<I', cpu.mem_read(esp, 4))[0]
        pop = 4
        if address == 0x10039630:
            count = struct.unpack('<I', cpu.mem_read(esp + 4, 4))[0]
            target = self.allocate(count)
            cpu.reg_write(UC_X86_REG_EAX, target)
        cpu.reg_write(UC_X86_REG_ESP, esp + pop)
        cpu.reg_write(UC_X86_REG_EIP, ret)

    def allocate(self, count):
        target = self.allocation
        self.allocation += (count + 15) & ~15
        if self.allocation >= 0x31200000:
            raise ValueError('Oracle allocation budget exceeded')
        self.cpu.mem_write(target, bytes(count))
        return target

    def coefficients(self, prof):
        self.cpu.mem_write(0x30050000, prof)
        self.cpu.mem_write(0x30040000, struct.pack('<3I', 0x30010000, 0x30020000, 13))
        self.invoke(0x41a90, (0x30050000, 0x30040000, 0x30030000))
        return bytes(self.cpu.mem_read(0x30030000, 32768))

    def kernel(self, source, prof):
        factor = struct.unpack_from('<i', prof, 8)[0]
        phase = struct.unpack_from('<f', prof, 12)[0]
        count = int(F(F(factor * (len(source) // 2)) * phase))
        self.cpu.mem_write(0x30000000, source or bytes(2))
        self.cpu.mem_write(0x30060000, prof)
        self.cpu.mem_write(0x30070000, bytes(2 * count + 16))
        self.allocation = 0x30200000
        self.invoke(0x42480, (0x30000000, len(source) // 2, 0x30070000, 0),
                    ((UC_X86_REG_EAX, 1), (UC_X86_REG_ECX, 0x30060000)))
        if bytes(self.cpu.mem_read(0x30070000 + count * 2, 16)) != bytes(16):
            raise ValueError('Native kernel wrote past expected output')
        return bytes(self.cpu.mem_read(0x30070000, count * 2))

def fixtures(oracle):
    cases = []
    for factor in (1, 2):
        for ratio in (F(.03125), F(.49), F(.5), F(.500001), F(.75), F(1.0001), F(2.), F(240.)):
            freq = [0., F(ratio * F(.95)), ratio, 1.] if ratio < 1 else [0., F(.95), 1.]
            amp = [1., 1., 0., 0.] if ratio < 1 else [1., 1., 0.]
            cases.append(profile(factor, freq, amp))
        cases.append(profile(factor, [0., .125, .5, 1.], [.3, .7, -.4, .2], F(.8), True))
    for i, prof in enumerate(cases):
        yield ['coefficients', str(i), b64(prof), b64(oracle.coefficients(prof))]
    rng = random.Random(0x42480)
    for old, new in ((44100, 48000), (48000, 44100), (44100, 22050), (48000, 16000)):
        prof = rate_profile(old, new)
        for length in (0, 1, 257, 4096, 8193):
            for pattern in ('impulse', 'dc', 'extrema', 'noise'):
                samples = ([32767] + [0] * (length - 1) if pattern == 'impulse' and length else
                           [12345] * length if pattern == 'dc' else
                           [-32768 if i % 2 else 32767 for i in range(length)] if pattern == 'extrema' else
                           [rng.randrange(-32768, 32768) for _ in range(length)])
                source = struct.pack('<%dh' % length, *samples)
                yield ['kernel', '%d-%d-%d-%s' % (old, new, length, pattern), b64(prof), b64(source), b64(oracle.kernel(source, prof))]

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dll', default='SDK/tools/BinaryAssetBuilder.AudioCompiler.dll')
    parser.add_argument('--output', default='recovery/evidence/audio/native-resampler/kernel.tsv.gz')
    args = parser.parse_args()
    oracle = OriginalKernel(args.dll)
    rows = ['# originalDllSha256=' + EXPECTED_SHA + '; nativeRvas=0x41a90,0x42480; fpuControl=0x027f; oracle=unicorn-2.1.4; support=256']
    counts = {}
    for row in fixtures(oracle):
        counts[row[0]] = counts.get(row[0], 0) + 1
        rows.append('\t'.join(row))
    if counts != {'coefficients': 18, 'kernel': 80}:
        raise ValueError('Incomplete kernel matrix: ' + str(counts))
    path = pathlib.Path(args.output)
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open('wb') as raw:
        with gzip.GzipFile(filename='', mode='wb', fileobj=raw, mtime=0) as stream:
            stream.write(('\n'.join(rows) + '\n').encode('ascii'))
    print('PASS original native resampler kernel oracle:', counts)

if __name__ == '__main__':
    main()
