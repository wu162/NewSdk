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
import random
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
    cpu.mem_map(0x30000000, 0x10000)
    cpu.mem_map(0x40000000, 0x1000)
    cpu.mem_map(0x50000000, 0x1000)
    # Deterministic CRT memcmp hook used by enclosing-format identifiers.
    cpu.mem_write(0x100ab19c, struct.pack("<I", 0x50000000))
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
        if address in (0x1003bc00, 0x1002fa80, 0x100404e0):
            raise ValueError("MPEG info unexpectedly entered a PCM sample-read path")
        if address not in (0x1003a300, 0x1003a310, 0x1003a1c0, 0x10039ef0, 0x100398c0, 0x100398f0, 0x10039630, 0x10009aae, 0x50000000):
            return
        stack = machine.reg_read(UC_X86_REG_ESP)
        ret, a, b, c = struct.unpack("<4I", machine.mem_read(stack, 16))
        if address in (0x1003a300, 0x1003a310):
            result = len(state["data"]) if address == 0x1003a300 else state["position"]
            machine.reg_write(UC_X86_REG_EDX, result >> 32)
        elif address == 0x1003a1c0:
            state["position"] = b | c << 32
            result = 0
        elif address == 0x10039ef0:
            result = max(0, min(c, len(state["data"]) - state["position"]))
            if result:
                machine.mem_write(b, state["data"][state["position"]:state["position"] + result])
            state["position"] += result
        elif address in (0x100398c0, 0x100398f0):
            if b not in (2, 4):
                raise ValueError("Unexpected original header endian read")
            result = int.from_bytes(machine.mem_read(a, b), "little" if address == 0x100398f0 else "big")
        elif address == 0x50000000:
            result = 0 if machine.mem_read(a, c) == machine.mem_read(b, c) else 1
        elif address == 0x10039630:
            result = state["heap"]
            state["heap"] += (a + 15) & ~15
            if state["heap"] > 0x30010000:
                raise ValueError("Original MPEG info allocator exceeded its bound")
            machine.mem_write(result, bytes([0xa5]) * a)
        else:
            machine.mem_write(a, bytes([b & 255]) * c)
            result = a
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
    info_rows = []
    patterns = ("zero", "one", "55", "aa", "ff", "seed1", "seed7", "seed12345", "seed19088743")
    for mode, frames, pattern, prefix, last_payload in itertools.product(
            (0, 3), (7, 8, 13), patterns, (0, 17), (0, 1, 100)):
        rng = random.Random(int(pattern[4:])) if pattern.startswith("seed") else None
        fill = {"zero": 0, "one": 1, "55": 85, "aa": 170, "ff": 255}.get(pattern, 0)
        data = bytearray([0x55] * prefix)
        data.extend(bytes((frames - 1) * stride + 4 + last_payload))
        for index in range(frames):
            start = prefix + index * stride
            data[start:start + 4] = bytes([255, 251, 16, mode << 6])
            for at in range(start + 4, min(len(data), start + stride)):
                data[at] = rng.randrange(256) if rng else fill
        state.update(data=bytes(data), position=0, heap=0x30001000)
        cpu.mem_write(0x30000000, bytes(0x440))
        cpu.mem_write(0x30000004, struct.pack("<I", 34))
        cpu.mem_write(0x30000010, struct.pack("<I", 0x30000600))
        cpu.mem_write(0x30000800, bytes(4))
        cpu.mem_write(0x20008000, struct.pack("<4I", 0x40000000, 0x30000000, 0x30000800, 0))
        cpu.reg_write(UC_X86_REG_ESP, 0x20008000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x37f)
        # Actual public dispatch, native format34 callback and SINFO constructors;
        # only OS file I/O, allocator and memset are hooks.
        cpu.emu_start(0x10025490, 0x40000000, count=1000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x20008004 or cpu.reg_read(UC_X86_REG_EAX) != 1:
            raise ValueError("Original MPEG SIMEX_info did not succeed normally")
        info = struct.unpack("<I", cpu.mem_read(0x30000800, 4))[0]
        metadata = struct.unpack("<I", cpu.mem_read(info, 4))[0]
        channels = cpu.mem_read(metadata + 0x1f6, 1)[0]
        sample_count = struct.unpack("<I", cpu.mem_read(metadata + 0x218, 4))[0]
        label = f"info-{mode}-{frames}-{pattern}-{prefix}-{last_payload}"
        info_rows.append(f"{label}\t{prefix}\t{channels}\t{base64.b64encode(data).decode('ascii')}\t{sample_count}\n")
    info_path = output.with_name(output.stem + "-info.tsv")
    info_path.write_text("".join(info_rows), encoding="ascii")
    container_inputs = []

    def enclose(magic, kind, payload, big):
        return magic + struct.pack(">I" if big else "<I", len(payload) + 4) + kind + payload

    wave_junk = b"JUNK" + struct.pack("<I", len(valid)) + valid
    aifc_junk = b"JUNK" + struct.pack(">I", len(valid)) + valid
    for fmt, declared, actual in itertools.product((1, 0xfffe, 3, 0x55), (16, 32), (2, 3, 15, 16, 32)):
        body = struct.pack("<H", fmt) + bytes(actual - 2)
        data = enclose(b"RIFF", b"WAVE", wave_junk + b"fmt " + struct.pack("<I", declared) + body, False)
        container_inputs.append((f"wave-partial-{fmt}-{declared}-{actual}", data))
    for compression, declared, actual in itertools.product((b"NONE", b"ADP4", b"sowt"), (22, 24, 64), (22, 23, 24)):
        body = bytes(18) + compression + bytes(actual - 22)
        data = enclose(b"FORM", b"AIFC", aifc_junk + b"COMM" + struct.pack(">I", declared) + body, True)
        container_inputs.append((f"aifc-partial-{compression.decode()}-{declared}-{actual}", data))
    for magic in (b"RIFF", b"FORM", b"OggS", b"fLaC", b"SCHl"):
        container_inputs.append((f"magic-{magic.decode()}", magic + valid))
    container_rows = []
    for label, data in container_inputs:
        scores = []
        for rva in (0x370d0, 0x38010):
            sentinel_scores = []
            for sentinel in (0, 0xa5):
                state.update(data=bytes(data), position=0)
                # Initialize scratch stack as well: original bounded partial reads
                # must depend only on bytes actually read, never prior stack data.
                cpu.mem_write(0x20000000, bytes([sentinel]) * 0x10000)
                cpu.mem_write(0x20008000, struct.pack("<5I", 0x40000000, 0, 0, 0, 0x30000600))
                cpu.reg_write(UC_X86_REG_ESP, 0x20008000)
                cpu.reg_write(UC_X86_REG_FPCW, 0x37f)
                cpu.emu_start(0x10000000 + rva, 0x40000000, count=1000000)
                if cpu.reg_read(UC_X86_REG_ESP) != 0x20008004:
                    raise ValueError("Original container identifier did not return normally")
                sentinel_scores.append(cpu.reg_read(UC_X86_REG_EAX))
            if len(set(sentinel_scores)) != 1:
                raise ValueError("Container identification depends on unread stack data")
            scores.append(sentinel_scores[0])
        blocked = int(max(scores) > 75)
        container_rows.append(f"{label}\t{blocked}\t{base64.b64encode(data).decode('ascii')}\t{scores[0]}\t{scores[1]}\n")
    container_path = output.with_name(output.stem + "-container.tsv")
    container_path.write_text("".join(container_rows), encoding="ascii")
    receipt = dict(originalSha256=EXPECTED_SHA, nativeRva="0x44d40", records=len(rows),
                   transcriptSha256=hashlib.sha256(output.read_bytes()).hexdigest(),
                   identifierRva="0x2f6f0", identifierRecords=len(identifier_rows),
                   identifierSha256=hashlib.sha256(identifier_path.read_bytes()).hexdigest(),
                   infoDispatchRva="0x25490", infoCallbackRva="0x2f8c0", infoRecords=len(info_rows),
                   infoSha256=hashlib.sha256(info_path.read_bytes()).hexdigest(),
                   containerIdentifierRvas=["0x370d0", "0x38010"], containerRecords=len(container_rows), containerStackPatterns=[0, 165],
                   containerSha256=hashlib.sha256(container_path.read_bytes()).hexdigest(),
                   emulator="Unicorn x86; initialized SINFO; x87 control 0x037f",
                   scope="MPEG header return/channel, raw identification and SIMEX_info payload independence; no PCM decoder path")
    output.with_suffix(".json").write_text(json.dumps(receipt, indent=2) + "\n")
    print(f"PASS original MPEG machine-code oracle: {len(rows)} header, {len(identifier_rows)} identifier, {len(info_rows)} SIMEX_info and {len(container_rows)} container records")


if __name__ == "__main__":
    main()
