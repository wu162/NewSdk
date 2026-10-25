#!/usr/bin/env python3
"""Execute the original DLL's x87 block-size calculation with the CLR FPU mode.

The machine-code sequence includes the original _ftol helper. No candidate
arithmetic is imported. Requires pefile and pinned Unicorn 2.1.4.
"""
import argparse
import hashlib
import pathlib
import random
import struct

import pefile
import unicorn
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32
from unicorn.x86_const import (UC_X86_REG_ESP, UC_X86_REG_EBP,
                              UC_X86_REG_FPCW, UC_X86_REG_EAX)

EXPECTED_SHA = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dll", default="SDK/tools/BinaryAssetBuilder.AudioCompiler.dll")
    parser.add_argument("--output", default="recovery/evidence/audio/native-block-frames/original.tsv")
    args = parser.parse_args()
    raw = pathlib.Path(args.dll).read_bytes()
    if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
        raise ValueError("The original AudioCompiler DLL fingerprint changed")
    if unicorn.__version__ != "2.1.4":
        raise ValueError("Use the pinned Unicorn 2.1.4 oracle version")
    cpu = Uc(UC_ARCH_X86, UC_MODE_32)
    cpu.mem_map(0x10000000, 0x140000)
    cpu.mem_write(0x10000000, pefile.PE(data=raw).get_memory_mapped_image())
    for address, size in [(0x20000000, 0x100000), (0x30000000, 0x100000),
                          (0x50000000, 0x1000)]:
        cpu.mem_map(address, size)
    # fld dword ptr [0x30001000]; jmp original fdivr/fimul/_ftol sequence.
    stub = b"\xd9\x05" + struct.pack("<I", 0x30001000)
    stub += b"\xe9" + struct.pack("<i", 0x1002dad1 - (0x50000000 + 11))
    cpu.mem_write(0x50000000, stub)
    # Match the non-SSE _ftol path; it truncates the unspilled x87 value.
    cpu.mem_write(0x1012f018, bytes(4))
    rates = set(range(400, 513)) | {1, 399, 8000, 11025, 16000, 22050,
                                  32000, 44100, 48000, 96000, 262143}
    rng = random.Random(0x027f)
    rates.update(rng.randint(400, 96000) for _ in range(128))
    lines = ["# originalDllSha256=" + EXPECTED_SHA
             + "; nativeRva=0x2dad1..0x2dae2; fpuControl=0x027f; oracle=unicorn-2.1.4"]
    for rate in sorted(rates):
        for codec, bytes_per_sample in [("PCM", 2.0), ("XAS", 0.59375)]:
            for channels in (1, 2):
                bitrate = struct.pack("<f", rate * channels * bytes_per_sample)
                cpu.mem_write(0x30001000, bitrate)
                cpu.mem_write(0x30000214, struct.pack("<i", rate))
                cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
                cpu.reg_write(UC_X86_REG_EBP, 0x30000000)
                cpu.reg_write(UC_X86_REG_FPCW, 0x027f)
                cpu.emu_start(0x50000000, 0x1002dae2, count=1000)
                frames = cpu.reg_read(UC_X86_REG_EAX)
                lines.append("\t".join([codec, str(channels), str(rate),
                                        bitrate.hex(), str(frames)]))
    output = pathlib.Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text("\n".join(lines) + "\n")
    print("PASS original x87 block-size oracle: %d cases" % (len(lines) - 1))


if __name__ == "__main__":
    main()
