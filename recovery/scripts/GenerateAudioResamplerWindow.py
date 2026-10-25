#!/usr/bin/env python3
"""Regenerate and verify the recovered filter120 x87 half-window constants.

The original RVA 0x41d05..0x41d20 computes:
    binary53((FCOS(binary53(binary53(i * binary32(pi)) / support)) + 1) * 0.5)
Support is 256 or 512. The scratch thunk uses equivalent x87 integer-memory
operands for these exactly representable bounded integers, then stores the
original 53-bit result as binary64. The script does not call Python math.cos,
but Unicorn 2.1.4 helper_fcos_x86_64 internally converts floatx80 to float64
and calls the host cos function. Thus Linux/Windows emulator results can
differ, and this is only a pinned-host emulator/source consistency check.
No candidate window algorithm is used. Real original
Windows CPU and plugin differential acceptance are recorded separately.
"""
import argparse
import base64
import hashlib
import pathlib
import re
import struct

import pefile
import unicorn
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_FPCW

EXPECTED_SHA = 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'
EXPECTED_WINDOW_SHA = 'd3d7520ba6eef9d7d56262014843ae9c402c9b58e4bcfbb21d12ae315e89a2c6'
FPCW = 0x027f  # 53-bit precision, nearest/even rounding, exceptions masked.


def extract(dll):
    raw = pathlib.Path(dll).read_bytes()
    if hashlib.sha256(raw).hexdigest() != EXPECTED_SHA:
        raise ValueError('Original AudioCompiler DLL fingerprint changed')
    if unicorn.__version__ != '2.1.4':
        raise ValueError('Use pinned Unicorn 2.1.4')
    pe = pefile.PE(data=raw)
    if pe.FILE_HEADER.Machine != 0x14c or pe.OPTIONAL_HEADER.ImageBase != 0x10000000:
        raise ValueError('Unexpected original image layout')
    # Keep the claimed original FCOS/arithmetic sequence and constants explicit.
    if pe.get_data(0x41d14, 14) != bytes.fromhex('d8cc89742430d8f1d9ffd8c3d8ca'):
        raise ValueError('Original window instruction evidence changed')
    if pe.get_data(0xb94dc, 4) != struct.pack('<I', 0x40490fdb):
        raise ValueError('Original binary32 pi constant changed')
    if pe.get_data(0xac888, 4) != struct.pack('<f', 0.5):
        raise ValueError('Original half constant changed')
    cpu = Uc(UC_ARCH_X86, UC_MODE_32)
    cpu.mem_map(0x10000000, 0x140000)
    cpu.mem_write(0x10000000, pe.get_memory_mapped_image())
    cpu.mem_map(0x20000000, 0x100000)
    cpu.mem_map(0x50000000, 0x100000)
    # FLD original pi; FIMUL i; FIDIV support; FCOS; FLD1; FADDP;
    # FMUL original half; FSTP binary64; RET. Each arithmetic operation uses
    # the pinned original x87 precision/rounding mode.
    thunk = bytes.fromhex(
        'd905dc940b10da0d00010050da3504010050d9ffd9e8dec1'
        'd80d88c80a10dd1d08010050c3')
    cpu.mem_write(0x50002000, thunk)
    windows = {}
    for support in (256, 512):
        cpu.mem_write(0x50000104, struct.pack('<i', support))
        values = []
        for index in range(1, support + 1):
            cpu.mem_write(0x50000100, struct.pack('<i', index))
            cpu.mem_write(0x200f0000, struct.pack('<I', 0x50000000))
            cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
            cpu.reg_write(UC_X86_REG_FPCW, FPCW)
            cpu.emu_start(0x50002000, 0x50000000, count=100)
            if cpu.reg_read(UC_X86_REG_ESP) != 0x200f0004 or cpu.reg_read(UC_X86_REG_FPCW) != FPCW:
                raise ValueError('Window thunk did not return under the pinned FPU control')
            values.append(bytes(cpu.mem_read(0x50000108, 8)))
        windows[support] = values
    if windows[256] != windows[512][1::2]:
        raise ValueError('Support256 differs from the even-index support512 constants')
    result = b''.join(windows[512])
    if hashlib.sha256(result).hexdigest() != EXPECTED_WINDOW_SHA:
        raise ValueError('Regenerated original window fingerprint changed')
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dll', default='SDK/tools/BinaryAssetBuilder.AudioCompiler.dll')
    parser.add_argument('--source', default='src/AudioCompiler/Native/ResamplerCoefficients.cs')
    parser.add_argument('--output', help='Optional raw little-endian binary64 output file')
    args = parser.parse_args()
    result = extract(args.dll)
    source = pathlib.Path(args.source).read_text()
    match = re.search(r'WindowBits\s*=\s*Convert\.FromBase64String\((.*?)\);', source, re.S)
    if not match:
        raise ValueError('Candidate WindowBits declaration was not found')
    encoded = ''.join(re.findall(r'"([A-Za-z0-9+/=]+)"', match.group(1)))
    candidate = base64.b64decode(encoded, validate=True)
    if len(candidate) != 512 * 8 or candidate != result:
        raise ValueError('Candidate window constants differ from the original x87 extraction')
    if args.output:
        pathlib.Path(args.output).write_bytes(result)
    print('PASS original x87 window/source bytes: 512 binary64 constants; '
          'support256 even-index identity; FPCW=0x027f; sha256=' + hashlib.sha256(result).hexdigest())


if __name__ == '__main__':
    main()
