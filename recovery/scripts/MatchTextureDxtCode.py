"""Evidence-only byte matching of legacy COFF functions against the original PE.
Relocation operands are ignored; a match is not proof of reachable/equivalent code.
Run only in GitHub Actions via InspectTextureDxtLibrary.ps1.
"""
import hashlib
import json
from pathlib import Path
import struct
import sys


def u16(b, p): return struct.unpack_from('<H', b, p)[0]
def u32(b, p): return struct.unpack_from('<I', b, p)[0]


def members(data):
    if data[:8] != b'!<arch>\n': raise ValueError('Not a COFF archive')
    p = 8
    while p < len(data):
        h = data[p:p+60]
        if len(h) != 60 or h[58:] != b'`\n': raise ValueError('Invalid archive header')
        size = int(h[48:58])
        body = data[p+60:p+60+size]
        if len(body) != size: raise ValueError('Truncated archive member')
        yield p, body
        p += 60 + size + (size & 1)


def code_sections(pe):
    if pe[:2] != b'MZ': raise ValueError('Not PE')
    p = u32(pe, 0x3c)
    if pe[p:p+4] != b'PE\0\0' or u16(pe,p+4) != 0x14c: raise ValueError('Not x86 PE')
    table = p + 24 + u16(pe, p+20)
    for i in range(u16(pe,p+6)):
        s = table + i*40
        if u32(pe,s+36) & 0x20000000:
            offset, size = u32(pe,s+20), u32(pe,s+16)
            yield u32(pe,s+12), pe[offset:offset+size]


def functions(obj):
    if len(obj)<20 or u16(obj,0)!=0x14c: return
    count, sympos, nsyms, opts = u16(obj,2), u32(obj,8), u32(obj,12), u16(obj,16)
    if not sympos or sympos + nsyms*18+4 > len(obj): return
    strings = sympos + nsyms*18
    names = {}
    i = 0
    while i < nsyms:
        p = sympos+i*18
        raw = obj[p:p+8]
        if raw[:4] == b'\0'*4:
            start = strings + u32(raw,4)
            end = obj.find(b'\0',start)
            name = obj[start:end].decode('ascii','replace') if end>=start else ''
        else: name = raw.rstrip(b'\0').decode('ascii','replace')
        section = struct.unpack_from('<h',obj,p+12)[0]
        if section>0 and u32(obj,p+8)==0 and u16(obj,p+14)&0x20:
            names.setdefault(section,[]).append(name)
        i += 1+obj[p+17]
    for section, symbols in names.items():
        relevant = [n for n in symbols if any(x in n for x in (
            'CodeRGBBlock','CodeAlphaBlock','QuantizeAlpha','AllSame','EncodeDXT','RoundMult','Search43Mult','RmsError'))]
        if not relevant: continue
        if section>count: raise ValueError('Invalid COFF symbol section')
        s = 20+opts+(section-1)*40
        if not u32(obj,s+36)&0x20000000: continue
        size, offset, reloc, nr = u32(obj,s+16), u32(obj,s+20), u32(obj,s+24), u16(obj,s+32)
        code = obj[offset:offset+size]
        if len(code)!=size: raise ValueError('Truncated COFF code')
        mask = bytearray(b'\1'*size)
        for j in range(nr):
            p = reloc+j*10
            address, kind = u32(obj,p), u16(obj,p+8)
            if kind not in (6,20): raise ValueError('Unsupported x86 relocation '+str(kind))
            if address+4>size: raise ValueError('Relocation outside code')
            mask[address:address+4]=b'\0'*4
        yield relevant, code, mask


def match(code, mask, sections):
    # Use the longest relocation-free run as an exact search anchor.
    runs=[]; start=0
    for i in range(len(mask)+1):
        if i==len(mask) or not mask[i]:
            runs.append((i-start,start)); start=i+1
    length, start=max(runs)
    if length<16: return [], length
    anchor=code[start:start+length]
    hits=[]
    for rva, body in sections:
        p=body.find(anchor)
        while p>=0:
            base=p-start
            if base>=0 and base+len(code)<=len(body) and all(
                    not mask[i] or body[base+i]==v for i,v in enumerate(code)):
                hits.append('0x%08X'%(rva+base))
            p=body.find(anchor,p+1)
    return hits,length


def main():
    original, reference, output = map(Path,sys.argv[1:])
    pe, lib = original.read_bytes(), reference.read_bytes()
    if hashlib.sha256(pe).hexdigest()!='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e': raise ValueError('Original hash')
    if hashlib.sha256(lib).hexdigest()!='572aa5d1d3ff26050bdc101fddacf0a4f5af360bb5e2bbe1b6bc95a615d2524b': raise ValueError('Reference hash')
    sections=list(code_sections(pe)); rows=[]
    for offset,obj in members(lib):
        for names,code,mask in functions(obj):
            hits,anchor=match(code,mask,sections)
            rows.append(dict(archiveOffset=offset,symbols=names,bytes=len(code),comparedBytes=sum(mask),anchorBytes=anchor,originalRvas=hits,codeSha256=hashlib.sha256(code).hexdigest()))
    if not rows: raise ValueError('No target functions extracted')
    output.write_text(json.dumps(dict(evidenceOnly=True,probeOnly=True,scope='Relocation-masked code matches only; not reachability, same-version or compression acceptance',functions=rows),indent=2)+'\n')
    print('PROBE Texture DXT code sections=%d matched=%d; no compressor equivalence claim'%(len(rows),sum(bool(x['originalRvas']) for x in rows)))


if __name__=='__main__': main()
