#!/usr/bin/env python3
"""Measure original raw-ADP4 reader I/O, including partial native read blocks.

Executes the original mono/stereo stream readers and original nibble decoder;
only the native file read callback is modeled. This is independent original
machine-code evidence for the SSND offset byte-window bound, not plugin/SDK
acceptance. Requires pefile and pinned Unicorn 2.1.4.
"""
import argparse
import hashlib
import pathlib
import struct

import pefile
import unicorn
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32, UC_HOOK_CODE, UC_HOOK_MEM_READ, UC_HOOK_MEM_WRITE
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_EIP, UC_X86_REG_EAX

SHA = "a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dll", default="SDK/tools/BinaryAssetBuilder.AudioCompiler.dll")
    parser.add_argument("--output", default="recovery/evidence/audio/native-ssnd-offset/original-adp4-read-requests.tsv")
    args = parser.parse_args()
    raw = pathlib.Path(args.dll).read_bytes()
    if hashlib.sha256(raw).hexdigest() != SHA or unicorn.__version__ != "2.1.4":
        raise ValueError("Original DLL fingerprint or pinned Unicorn version mismatch")
    cpu = Uc(UC_ARCH_X86, UC_MODE_32)
    cpu.mem_map(0x10000000, 0x140000)
    cpu.mem_write(0x10000000, pefile.PE(data=raw).get_memory_mapped_image())
    for address in (0x20000000, 0x30000000, 0x50000000):
        cpu.mem_map(address, 0x100000)
    context = {}

    def code(machine, address, size, unused):
        if address == 0x10039ef0:
            stack = machine.reg_read(UC_X86_REG_ESP)
            ret, stream, destination, count = struct.unpack("<4I", machine.mem_read(stack, 16))
            start = context["position"]
            if stream != 1 or start + count > len(context["source"]):
                raise AssertionError("Native requested bytes outside initialized input")
            machine.mem_write(destination, context["source"][start:start + count])
            context["requests"].append(count)
            context["position"] += count
            context["scratch"] = (destination, destination + count)
            machine.reg_write(UC_X86_REG_EAX, count)
            machine.reg_write(UC_X86_REG_ESP, stack + 4)
            machine.reg_write(UC_X86_REG_EIP, ret)

    def read(machine, access, address, size, value, unused):
        # These are the original readers' byte loads, including mono final
        # odd-nibble and stereo final partial-block branches.
        if machine.reg_read(UC_X86_REG_EIP) in (0x1003ba0c, 0x1003ba4a, 0x1003bb40,
                                               0x1003bb56, 0x1003bb90, 0x1003bba6):
            low, high = context["scratch"]
            if not low <= address or address + size > high:
                raise AssertionError("Native consumed uninitialized read-buffer bytes")

    def write(machine, access, address, size, value, unused):
        for channel in range(context["channels"]):
            start = 0x30010000 + channel * 0x10000
            end = start + context["frames"] * 2
            if start - 16 <= address < end + 16:
                if not start <= address or address + size > end:
                    raise AssertionError("Native wrote beyond planar sample output")
                context["written"].update(range(address, address + size))

    cpu.hook_add(UC_HOOK_CODE, code)
    cpu.hook_add(UC_HOOK_MEM_READ, read)
    cpu.hook_add(UC_HOOK_MEM_WRITE, write)
    rows = ["# originalDllSha256=" + SHA + "; oracle=unicorn-2.1.4; nativeRva=0x3b970,0x3bab0,0x48500; allFileBytesInitialized=true",
            "channels\tframes\tpattern\trequestedBytes\treadBlocks\tinitializedOutputBytes"]
    for channels in (1, 2):
        for frames in (2, 3, 7, 129, 511, 512, 513, 1023, 1024, 1025, 2041, 4096):
            for pattern in (0, 255, 37):
                needed = (frames * channels + 1) // 2
                context.clear()
                context.update(position=0, requests=[], channels=channels, frames=frames,
                               written=set(), scratch=(0, 0),
                               source=bytes(((i * 37 + 19) & 255) if pattern == 37 else pattern for i in range(needed)))
                cpu.mem_write(0x30000000, bytes([0xa5]) * 0x50000)
                for channel in range(channels):
                    cpu.mem_write(0x30000000 + channel * 4, struct.pack("<I", 0x30010000 + channel * 0x10000))
                    cpu.mem_write(0x30000100 + channel * 4, struct.pack("<I", 0x30000200 + channel * 0x40))
                    cpu.mem_write(0x30000200 + channel * 0x40, bytes(32))
                stack = 0x200f0000
                # cdecl(stream, firstFrame, inclusiveLastFrame, states[], outputs[])
                cpu.mem_write(stack, struct.pack("<6I", 0x50000000, 1, 0, frames - 1, 0x30000100, 0x30000000))
                cpu.reg_write(UC_X86_REG_ESP, stack)
                cpu.emu_start(0x1003b970 if channels == 1 else 0x1003bab0, 0x50000000, count=5000000)
                if cpu.reg_read(UC_X86_REG_EAX) != 1 or context["position"] != needed:
                    raise AssertionError("Original read request total differs from ceil(frames*channels/2)")
                if cpu.reg_read(UC_X86_REG_ESP) != stack + 4 or len(context["written"]) != frames * channels * 2:
                    raise AssertionError("Original return ABI or initialized sample coverage differs")
                rows.append("\t".join(map(str, (channels, frames, pattern, needed,
                             ",".join(map(str, context["requests"])), len(context["written"])))))
    output = pathlib.Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text("\n".join(rows) + "\n", encoding="utf-8")
    print("PASS original ADP4 stream readers:72 initialized request/output/ABI cases; mono ceil(F/2), stereo F bytes")


if __name__ == "__main__":
    main()
