#!/usr/bin/env python3
"""Execute original RVA 0x44d40, including its uint16 arithmetic and mutations.

No recovered parser is imported. The raw x86 routine receives the header word
and a fully initialized SINFO record; no SIMEX importer or decoder is invoked.
"""
import argparse
import base64
import hashlib
import itertools
import json
import pathlib
import struct

import pefile
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32, UC_HOOK_CODE
from unicorn.x86_const import UC_X86_REG_EAX, UC_X86_REG_EDX, UC_X86_REG_EIP, UC_X86_REG_ESP, UC_X86_REG_FPCW

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
    # Execute the original scanner too. Hooks implement only a deterministic
    # bounded file's tell/absolute-seek/read and the byte-order helper. No hook
    # contains a frame parser or reproduces the scanner's branch decisions.
    state = {}

    def file_hook(machine, address, size, unused):
        if address not in (0x1003a310, 0x1003a1c0, 0x10039ef0, 0x100398c0):
            return
        stack = machine.reg_read(UC_X86_REG_ESP)
        ret, a, b, c = struct.unpack("<4I", machine.mem_read(stack, 16))
        if address == 0x1003a310:
            result = state["position"]
            machine.reg_write(UC_X86_REG_EDX, result >> 32)
        elif address == 0x1003a1c0:
            state["position"] = b | c << 32
            result = 0
        elif address == 0x10039ef0:
            result = max(0, min(c, len(state["data"]) - state["position"]))
            if result:
                machine.mem_write(b, state["data"][state["position"]:state["position"] + result])
            state["position"] += result
        else:
            if b != 4:
                raise ValueError("Unexpected original header endian read")
            result = int.from_bytes(machine.mem_read(a, b), "big")
        machine.reg_write(UC_X86_REG_EAX, result & 0xffffffff)
        machine.reg_write(UC_X86_REG_ESP, stack + 4)
        machine.reg_write(UC_X86_REG_EIP, ret)

    cpu.hook_add(UC_HOOK_CODE, file_hook)
    identifier_inputs = []
    header = bytes.fromhex("fffb10c4")
    stride = 104
    for prefix, frames, last_payload in itertools.product((0, 1, 127), (0, 1, 6, 7, 8), (0, 1, 100)):
        data = bytearray([0x55] * prefix)
        if frames:
            data.extend(bytes((frames - 1) * stride + 4 + last_payload))
            for index in range(frames):
                data[prefix + index * stride:prefix + index * stride + 4] = header
        identifier_inputs.append((f"threshold-{prefix}-{frames}-{last_payload}", bytes(data)))
    valid = bytearray(6 * stride + 4)
    for index in range(7):
        valid[index * stride:index * stride + 4] = header
    # Early plausible frame spans can terminate the original scan at EOF before
    # a later valid seven-header chain is considered. Include complete field
    # combinations and explicit long-frame regression inputs.
    poison_words = (0xffe4e800, 0xffe4d800, 0xffe4c800, 0xffe4e000, *words[::257])
    for index, word in enumerate(poison_words):
        for prefix in (0, 1, 127):
            identifier_inputs.append((f"candidate-{index}-{prefix}", bytes([0x55]) * prefix + struct.pack(">I", word) + valid))
    identifier_rows = []
    for label, data in identifier_inputs:
        state.update(data=data, position=0)
        cpu.mem_write(0x20008000, struct.pack("<II", 0x40000000, 0x30000000))
        cpu.reg_write(UC_X86_REG_ESP, 0x20008000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x37f)
        cpu.emu_start(0x1002f6f0, 0x40000000, count=1000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x20008004:
            raise ValueError("Original MPEG identifier did not return normally")
        low = cpu.reg_read(UC_X86_REG_EAX)
        high = cpu.reg_read(UC_X86_REG_EDX)
        offset = -1 if low == high == 0xffffffff else low
        if offset != -1 and high != 0:
            raise ValueError("Unexpected original identifier high offset")
        identifier_rows.append(f"{label}\t{offset}\t{base64.b64encode(data).decode('ascii')}\n")
    identifier_path = output.with_name(output.stem + "-identifier.tsv")
    identifier_path.write_text("".join(identifier_rows), encoding="ascii")
    receipt = dict(originalSha256=EXPECTED_SHA, nativeRva="0x44d40", records=len(rows),
                   transcriptSha256=hashlib.sha256(output.read_bytes()).hexdigest(),
                   identifierRva="0x2f6f0", identifierRecords=len(identifier_rows),
                   identifierSha256=hashlib.sha256(identifier_path.read_bytes()).hexdigest(),
                   emulator="Unicorn x86; initialized SINFO; x87 control 0x037f",
                   scope="MPEG header return/channel mutation and raw file identification; not decoder acceptance")
    output.with_suffix(".json").write_text(json.dumps(receipt, indent=2) + "\n")
    print(f"PASS original MPEG machine-code oracle: {len(rows)} header and {len(identifier_rows)} identifier records")


if __name__ == "__main__":
    main()
