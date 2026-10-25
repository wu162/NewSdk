#!/usr/bin/env python3
"""Execute original RVA 0x44d40, including its uint16 arithmetic and mutations.

No recovered parser is imported. The raw x86 routine receives the header word
and a fully initialized SINFO record; no SIMEX importer or decoder is invoked.
"""
import argparse
import hashlib
import itertools
import json
import pathlib
import struct

import pefile
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32
from unicorn.x86_const import UC_X86_REG_EAX, UC_X86_REG_ESP, UC_X86_REG_FPCW

EXPECTED_SHA = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--dll", default="SDK/tools/BinaryAssetBuilder.AudioCompiler.dll")
    parser.add_argument("--output", required=True)
    args = parser.parse_args()
    raw = pathlib.Path(args.dll).read_bytes()
    if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
        raise ValueError("Original AudioCompiler fingerprint changed")
    pe = pefile.PE(data=raw)
    if pe.OPTIONAL_HEADER.ImageBase != 0x10000000:
        raise ValueError("Unexpected original preferred base")
    cpu = Uc(UC_ARCH_X86, UC_MODE_32)
    cpu.mem_map(0x10000000, 0x140000)
    cpu.mem_write(0x10000000, pe.get_memory_mapped_image())
    cpu.mem_map(0x20000000, 0x10000)
    cpu.mem_map(0x30000000, 0x1000)
    cpu.mem_map(0x40000000, 0x1000)
    words = []
    # All return/channel-affecting header fields. All four mode values matter
    # even though only mode3 is mono. CRC/private/copyright/original/emphasis
    # flags are varied on every point as an independent four-pattern axis.
    for version, layer, bitrate, rate, mode, pad, flags in itertools.product(
            range(4), range(4), range(16), range(4), range(4), range(2),
            (0x00000, 0x10000, 0x0010f, 0x1013f)):
        words.append(0xffe00000 | version << 19 | layer << 17 |
                     bitrate << 12 | rate << 10 | pad << 9 | mode << 6 | flags)
    # Sync detection is part of the contract; preserve all other header fields.
    for bit in range(21, 32):
        words.append(0xfffb50c4 & ~(1 << bit))
    words.extend((0, 0xffffffff, 0xfffb50c4, 0xfff38064))
    output = pathlib.Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for word in words:
        cpu.mem_write(0x30000000, bytes([0xa5]) * 32)
        cpu.mem_write(0x20008000, struct.pack("<III", 0x40000000, word, 0x30000000))
        cpu.reg_write(UC_X86_REG_ESP, 0x20008000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x37f)
        cpu.emu_start(0x10044d40, 0x40000000, count=1000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x20008004:
            raise ValueError("Original MPEG header parser did not return normally")
        result = cpu.reg_read(UC_X86_REG_EAX)
        channels = cpu.mem_read(0x3000000e, 1)[0]
        rows.append(f"{word:08x}\t{result}\t{channels}\n")
    output.write_text("".join(rows), encoding="ascii")
    receipt = dict(originalSha256=EXPECTED_SHA, nativeRva="0x44d40", records=len(rows),
                   transcriptSha256=hashlib.sha256(output.read_bytes()).hexdigest(),
                   emulator="Unicorn x86; initialized SINFO; x87 control 0x037f",
                   scope="MPEG header return payload and channel mutation only; not decoder acceptance")
    output.with_suffix(".json").write_text(json.dumps(receipt, indent=2) + "\n")
    print(f"PASS original MPEG header machine-code oracle: {len(rows)} records")


if __name__ == "__main__":
    main()
