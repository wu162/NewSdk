#!/usr/bin/env python3
"""Original pinned XMA packet integer helpers, bounded source fixtures only.

Every output/return/out-parameter is produced by the original PE instructions.
No candidate implementation is imported; no codec, allocator or libm hooks.
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
from unicorn import Uc, UC_ARCH_X86, UC_MODE_32, UC_HOOK_MEM_READ
from unicorn.x86_const import UC_X86_REG_ESP, UC_X86_REG_EAX, UC_X86_REG_FPCW

SHA = 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'
START = 0x30001000
OUT = 0x30020000
SENTINEL = bytes([0xa5]) * 4096
SPAN = 0x3fe0

def b64(value): return base64.b64encode(value).decode('ascii')

class OriginalPackets:
    def __init__(self, dll):
        raw = pathlib.Path(dll).read_bytes()
        if hashlib.sha256(raw).hexdigest() != SHA: raise ValueError('Original DLL changed')
        if unicorn.__version__ != '2.1.4': raise ValueError('Use Unicorn 2.1.4')
        pe = pefile.PE(data=raw)
        self.cpu = Uc(UC_ARCH_X86, UC_MODE_32)
        self.cpu.mem_map(0x10000000, 0x140000)
        self.cpu.mem_write(0x10000000, pe.get_memory_mapped_image())
        self.cpu.mem_map(0x20000000, 0x100000)
        self.cpu.mem_map(0x30000000, 0x100000)
        self.source_end = START
        self.cpu.hook_add(UC_HOOK_MEM_READ, self.source_guard, begin=START, end=OUT - 1)

    def source_guard(self, cpu, access, address, size, value, data):
        if address + size > self.source_end:
            raise ValueError('Unsafe original read outside fixture: 0x%x+%d > 0x%x' % (address, size, self.source_end))

    def call(self, rva, args, source=b''):
        cpu = self.cpu
        self.source_end = START + len(source)
        if source: cpu.mem_write(START, source)
        cpu.mem_write(OUT, SENTINEL)
        cpu.mem_write(0x30030000, bytes(32))
        cpu.mem_write(0x200f0000, struct.pack('<%dI' % (len(args)+1), 0x20000000, *[x & 0xffffffff for x in args]))
        cpu.reg_write(UC_X86_REG_ESP, 0x200f0000)
        cpu.reg_write(UC_X86_REG_FPCW, 0x027f)
        cpu.emu_start(0x10000000+rva, 0x20000000, count=1000000)
        if cpu.reg_read(UC_X86_REG_ESP) != 0x200f0004: raise ValueError('Original ABI return differs')
        if source and bytes(cpu.mem_read(START, len(source))) != source: raise ValueError('Original mutated source')
        return cpu.reg_read(UC_X86_REG_EAX)

    def written(self, count):
        raw = bytes(self.cpu.mem_read(OUT, 4096))
        if count < 0 or count > 4096 or raw[count:] != SENTINEL[count:]: raise ValueError('Original output guard changed')
        return raw[:count]

    def count(self, value):
        count = self.call(0xe890, (OUT, value))
        return self.written(count)

    def bits(self, initial, source, offset, count):
        self.cpu.mem_write(0x30040000, struct.pack('<I', initial))
        self.call(0xea80, (0x30040000, START, offset, count), source)
        return struct.unpack('<I', self.cpu.mem_read(0x30040000,4))[0]

    def frame(self, source, offset):
        result = self.call(0xeae0, (START, offset, 0x30030000), source) & 255
        length = struct.unpack('<i', self.cpu.mem_read(0x30030000,4))[0]
        if result not in (0,1): raise ValueError('Bad boolean return')
        return length, result

    def scan(self, source, maximum):
        frames = self.call(0xef60, (START, maximum, OUT, 0x30030000, 0x30030004), source)
        block, written = struct.unpack('<ii', self.cpu.mem_read(0x30030000,8))
        return frames, block, self.written(written)

def set_bits(raw, start, count, value):
    for i in range(count):
        at = start+i
        mask = 1 << (7 - at % 8)
        raw[at//8] = (raw[at//8] & ~mask) | (((value >> (count-1-i)) & 1) * mask)

def payload_bit(position):
    return 32 + (position // SPAN) * 16384 + position % SPAN

def frame_source(offset, length, continues):
    # Use four complete packets: every native three-byte lookahead and final
    # continuation read is in-bounds, including declared arithmetic edge cases.
    source = bytearray(8192)
    for i in range(15): set_bits(source, payload_bit(offset+i), 1, (length >> (14-i)) & 1)
    # Precisely reproduce the original helper's one-header skip policy for the
    # continuation position. Do not silently generalize it to arbitrary frames.
    end = offset+length
    flag = end+31+(32 if end > SPAN else 0)
    set_bits(source, flag, 1, continues)
    return bytes(source)

def scan_source(lengths, initial=0, flags=0, sentinel=False):
    positions=[]; position=initial
    for length in lengths:
        if length < 16: raise ValueError('Scanner fixtures require positive progressing frames')
        positions.append(position); position += length
    packets=max(1, (position+SPAN-1)//SPAN)
    source=bytearray(4+packets*2048)
    struct.pack_into('>I',source,0,(len(source)<<2)|flags)
    for i in range(packets): struct.pack_into('>I',source,4+i*2048,0x3fe0<<11 if sentinel else initial<<11 if i==0 else 0)
    for index,(offset,length) in enumerate(zip(positions,lengths)):
        for i in range(15): set_bits(source,32+payload_bit(offset+i),1,(length>>(14-i))&1)
        set_bits(source,32+payload_bit(offset+length-1),1,index+1 < len(lengths))
    return bytes(source) + bytes(3)

def fixtures(oracle):
    rng=random.Random(0xea80)
    values={0,1,0xffffffff}
    for threshold in (0xc0,0x30c0,0xc30c0,0x30c30c0): values.update((threshold-1,threshold,threshold+1))
    values.update(rng.randrange(0x100000000) for _ in range(128))
    for value in sorted(values): yield ['count',str(value),b64(oracle.count(value))]
    sources=[bytes(range(32)),bytes([255])*32,bytes(32),bytes(rng.randrange(256) for _ in range(32))]
    bit_counts=(-2147483648,-1,0,1,7,8,15,16,23,24,25,31,32,33,63,64,2147483647)
    for pattern,source in enumerate(sources):
        for offset in (0,1,7,8,15,16,23,24,31,63):
            for count in bit_counts:
                initial=(0,0xffffffff,0x12345678,0x80000001)[pattern]
                yield ['bits','%d-%d-%d'%(pattern,offset,count),str(initial),str(offset),str(count),b64(source),str(oracle.bits(initial,source,offset,count))]
    for offset in (0,1,7,8,0x3fd0,0x3fd1,0x3fd2,0x3fdf):
        for length in (0,16,31,511,16352,32767):
            for continues in (0,1):
                source=frame_source(offset,length,continues)
                actual_length,actual_flag=oracle.frame(source,offset)
                yield ['frame','%d-%d-%d'%(offset,length,continues),str(offset),b64(source),str(actual_length),str(actual_flag)]
    cases=[]
    for initial in (0,7,SPAN-16,SPAN-1):
        for lengths in ([16],[31,31],[511]*7,[1024]*20):
            for flags in (0,3):
                cases.append(('%d-%d-%d-%d'%(initial,len(lengths),lengths[0],flags),scan_source(lengths,initial,flags),len(lengths)))
    cases.append(('sentinel',scan_source([],sentinel=True),1))
    for name,source,frames in cases:
        for maximum in sorted({1,frames,frames+1}):
            total,block,encoded=oracle.scan(source,maximum)
            yield ['scan',name+'-max'+str(maximum),str(maximum),b64(source),str(total),str(block),b64(encoded)]

def real_fixtures(oracle):
    # Trace the existing SIMEX/SNR container, rather than treating CustomData as
    # a raw codec API return. RAM: SNR8 + block8. Stream: block8 in CustomData.
    path=pathlib.Path('recovery/evidence/audio/plugin-validation/40199000936-1/xma-original.txt')
    # Git text checkout may use CRLF on Windows. Pin canonical LF text;
    # the base64-decoded original packet bytes remain unchanged.
    transcript=path.read_bytes().replace(b'\r\n', b'\n')
    if hashlib.sha256(transcript).hexdigest() != 'e7b2befa6c59fc0cce64ed7959d1b65f83cb1adeed5e6cb100758d68583a35d0': raise ValueError('Original XMA transcript changed')
    lines=transcript.decode().splitlines()
    if len(lines) != 8: raise ValueError('Expected eight original XMA cases')
    for line in lines:
        label=line.split(':',1)[0]
        custom=base64.b64decode(line.split(':CUSTOM:',1)[1])
        streamed=label.endswith('-True')
        offset=8 if streamed else 16
        if not streamed and custom[0] != 3: raise ValueError('Expected original RAM XMA SNR codec3 header')
        block_size=struct.unpack_from('>I',custom,0 if streamed else 8)[0] & 0x7fffffff
        if block_size > len(custom)-(0 if streamed else 8): raise ValueError('Outer original block exceeds CustomData')
        inner_size=struct.unpack_from('>I',custom,offset)[0] >> 2
        if inner_size < 8 or inner_size > len(custom)-offset: raise ValueError('Inner original EA block exceeds CustomData')
        source=custom[offset:offset+inner_size]+bytes(3)
        packet=source[4:]
        # Public xma2defs.h GetXmaPacketFirstFrameOffsetInBits and
        # GetXmaFrameLengthInBits (blob13a4306...) independently check the
        # ordinary first-frame layout; no third-party source is copied.
        frame_offset=((packet[0]&3)<<13)|(packet[1]<<5)|(packet[2]>>3)
        if frame_offset != ((struct.unpack_from('>I',packet)[0]>>11)&0x7fff): raise ValueError('Independent public header layout disagrees')
        length,flag=oracle.frame(packet,frame_offset)
        physical=32+frame_offset; byte,bit=divmod(physical,8)
        if physical+15 <= 16384:
            independent=(int.from_bytes(packet[byte:byte+3],'big')>>(9-bit))&0x7fff
            if length!=independent: raise ValueError('Independent public first-frame length disagrees')
        yield ['frame','real-'+label,str(frame_offset),b64(packet),str(length),str(flag)]
        total,block,encoded=oracle.scan(source,2147483647)
        yield ['scan','real-'+label,'2147483647',b64(source),str(total),str(block),b64(encoded)]

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--dll',default='SDK/tools/BinaryAssetBuilder.AudioCompiler.dll')
    parser.add_argument('--output',default='recovery/evidence/audio/native-xma-packets/original.tsv.gz')
    args=parser.parse_args();oracle=OriginalPackets(args.dll)
    rows=['# originalDllSha256='+SHA+'; nativeRvas=0xe890,0xea80,0xeae0,0xef60; oracle=unicorn-2.1.4; scope=bounded-integer-packet-helpers; realEaBlocks=8; rawCodecPackets=0']
    counts={}
    from itertools import chain
    for row in chain(fixtures(oracle),real_fixtures(oracle)):
        rows.append('\t'.join(row));counts[row[0]]=counts.get(row[0],0)+1
    if counts != {'count':143,'bits':680,'frame':104,'scan':98}: raise ValueError('Unexpected coverage '+str(counts))
    output=pathlib.Path(args.output);output.parent.mkdir(parents=True,exist_ok=True)
    with output.open('wb') as raw:
        with gzip.GzipFile(filename='',mode='wb',fileobj=raw,mtime=0) as stream: stream.write(('\n'.join(rows)+'\n').encode('ascii'))
    print('PASS original XMA packet integer oracle: '+str(counts)+'; bounded reads, source immutability, output guards; real EA blocks=8; raw codec packets=0')
if __name__=='__main__':main()
