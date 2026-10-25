#!/usr/bin/env python3
"""Execute the original DLL's pure x86 XAS encoder to generate byte oracles.

Requires pefile and unicorn 2.1.4. No candidate algorithm is imported or used.
The DLL is mapped at its preferred base; only its original routines run.
"""
import argparse
import base64
import hashlib
import math
import pathlib
import random
import struct
import platform
import re

import pefile
import unicorn
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_ECX, UC_X86_REG_FPCW

EXPECTED_SHA = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139"


def f32(value):
    return struct.unpack("<f", struct.pack("<f", value))[0]


class OriginalEncoder:
    def __init__(self, dll):
        raw = pathlib.Path(dll).read_bytes()
        if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
            raise ValueError("The original AudioCompiler DLL fingerprint changed")
        image = pefile.PE(data=raw).get_memory_mapped_image()
        self.cpu = Uc(UC_ARCH_X86, UC_MODE_32)
        self.cpu.mem_map(0x10000000, 0x140000)
        self.cpu.mem_write(0x10000000, image)
        for address, size in [(0x20000000, 0x100000), (0x30000000, 0x100000),
                              (0x40000000, 0x100000), (0x50000000, 0x1000)]:
            self.cpu.mem_map(address, size)

    def encode(self, samples, channels):
        if len(samples) != 128 * channels or channels not in (1, 2):
            raise ValueError("Invalid native oracle input")
        cpu = self.cpu
        cpu.mem_write(0x30000000, struct.pack("<%df" % len(samples), *samples))
        cpu.mem_write(0x40000000, bytes(76 * channels))
        cpu.mem_write(0x200f0000, struct.pack("<4I", 0x50000000, 0x30000000, 0x40000000, channels))
        cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
        cpu.reg_write(UC_X86_REG_ECX, 0)
        cpu.reg_write(UC_X86_REG_FPCW, 0x37f)
        cpu.emu_start(0x1000f700, 0x50000000, count=1000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x200f0010:
            raise ValueError("The original native encoder did not return normally")
        return bytes(cpu.mem_read(0x40000000, 76 * channels))


def cases():
    rng = random.Random(123)
    for channels in (1, 2):
        for amplitude in (0, .001, .1, .5, .999):
            yield "constant-%s" % amplitude, channels, [f32(amplitude)] * (128 * channels)
            for frequency in (1, 3, 8, 20, 48):
                yield "sine-%s-%s" % (amplitude, frequency), channels, [
                    f32(math.sin(2 * math.pi * (i // channels) / 128 * frequency) * amplitude)
                    for i in range(128 * channels)]
        yield "ramp", channels, [f32((i // channels - 64) / 64) for i in range(128 * channels)]
        for index in range(100):
            yield "random-%d" % index, channels, [f32(rng.uniform(-1, 1)) for _ in range(128 * channels)]


def precision_cases():
    # Read the checked-in regression source so Python/C# cannot silently drift.
    source = pathlib.Path('recovery/tests/AudioXasPrecisionFixture.cs').read_text()
    match = re.search(r'Signed16\s*=\s*new short\[\]\s*\{(.*?)\};', source, re.S)
    if not match:
        raise ValueError('Precision fixture Signed16 declaration missing')
    samples = [int(value) for value in re.findall(r'-?\d+', match.group(1))]
    if len(samples) != 32 or any(value < -32768 or value > 32767 for value in samples):
        raise ValueError('Precision fixture must hold exactly32 signed16 samples')
    for channels in (1, 2):
        yield 'precision-resampled-sine6144', channels, [
            f32(samples[(index // channels) % 32] / 32768.0)
            for index in range(128 * channels)]


def assert_precision_group(encoded, channels):
    expected = bytes.fromhex('c2f536fc464666576676766765665555344241')
    for channel in range(channels):
        for group in range(4):
            block = encoded[channel * 76:(channel + 1) * 76]
            actual = block[group * 4:group * 4 + 4] + bytes(block[16 + row * 4 + group] for row in range(15))
            if actual != expected:
                raise ValueError('Original precision fixture does not match the actual plugin group bytes')

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dll", default="SDK/tools/BinaryAssetBuilder.AudioCompiler.dll")
    parser.add_argument("--output", default="recovery/evidence/audio/native-xas/original-blocks.tsv")
    parser.add_argument("--include-precision", action="store_true", help="Add the two signed16-derived precision regression cases (264 total)")
    args = parser.parse_args()
    encoder = OriginalEncoder(args.dll)
    if unicorn.__version__ != "2.1.4":
        raise ValueError("Use the pinned unicorn 2.1.4 oracle version")
    lines = ["# originalDllSha256=" + EXPECTED_SHA + "; nativeRva=0xf700; oracle=unicorn-2.1.4; "
             + "python=" + platform.python_version() + "; pefile=" + pefile.__version__
             + "; fpuControl=0x037f; codeMode=x86-32; platform=" + platform.system()]
    filters = set()
    shifts = set()
    from itertools import chain
    matrix = chain(cases(), precision_cases()) if args.include_precision else cases()
    for name, channels, samples in matrix:
        encoded = encoder.encode(samples, channels)
        if name == 'precision-resampled-sine6144':
            assert_precision_group(encoded, channels)
        filters.update(encoded[c * 76 + group * 4] & 15 for c in range(channels) for group in range(4))
        shifts.update(encoded[c * 76 + group * 4 + 2] & 15 for c in range(channels) for group in range(4))
        source = struct.pack("<%df" % len(samples), *samples)
        lines.append("\t".join([name, str(channels), base64.b64encode(source).decode(), base64.b64encode(encoded).decode()]))
    if len(lines) - 1 != (264 if args.include_precision else 262):
        raise ValueError('Original XAS matrix count differs')
    if filters != {0, 1, 2, 3}:
        raise ValueError("The native oracle did not cover all four predictors")
    if shifts != set(range(13)):
        raise ValueError("The native oracle did not cover all thirteen quantizer shifts")
    pathlib.Path(args.output).parent.mkdir(parents=True, exist_ok=True)
    pathlib.Path(args.output).write_text("\n".join(lines) + "\n")
    print("PASS original x86 XAS oracle: %d cases; predictors %s; shifts %s" % (len(lines) - 1, sorted(filters), sorted(shifts)))


if __name__ == "__main__":
    main()
