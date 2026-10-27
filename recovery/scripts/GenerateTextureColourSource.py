#!/usr/bin/env python3
"""Restore PC Production=71 colour instructions and owned data as MASM source.
Analysis/generation only. Compile and execute differential tests in Actions.
Requires capstone==5.0.6. Original hash and graph anchor are fixed below.
"""
import argparse, hashlib, json, re, struct
from pathlib import Path
from capstone import Cs, CS_ARCH_X86, CS_MODE_32, CS_GRP_CALL, CS_GRP_JUMP, CS_GRP_RET
from capstone.x86 import X86_OP_IMM,X86_OP_MEM

ROOT=Path(__file__).resolve().parents[2]
BASE=0x10000000
ORIGINAL_SHA='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
GRAPH='recovery/evidence/texture/dxt-decompile/40496512115-1/graph.json'

def generate():
    data=(ROOT/'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll').read_bytes()
    if hashlib.sha256(data).hexdigest()!=ORIGINAL_SHA: raise ValueError('Original hash mismatch')
    pe=struct.unpack_from('<I',data,60)[0];sh=pe+24+struct.unpack_from('<H',data,pe+20)[0]
    sections=[]
    for n in range(struct.unpack_from('<H',data,pe+6)[0]):
        s=sh+n*40;vs,va,size,offset=struct.unpack_from('<IIII',data,s+8);sections.append((va,vs,size,offset))
    def read(rva,size):
        for va,vs,raw,off in sections:
            if va<=rva<va+max(vs,raw):
                n=min(size,max(0,raw-(rva-va)))
                return data[off+rva-va:off+rva-va+n]+bytes(size-n)
        raise ValueError('Unmapped original data '+hex(rva))
    md=Cs(CS_ARCH_X86,CS_MODE_32);md.detail=True
    graph=json.loads((ROOT/GRAPH).read_text());functions={f['entry']:f for f in graph['functions']}
    queue=['10083cd0'];selected=set()
    while queue:
        entry=queue.pop()
        if entry=='100833f0' or entry in selected: continue # quality 72 is explicitly rejected by the API
        selected.add(entry);queue.extend(functions[entry]['callees'])
    # The Ghidra body omits the two 511-way tables. Recover every reachable block
    # from their verified targets; neither table contains runtime original pointers.
    tables={0x7a3f4:511,0x7abf0:511,0x7be38:3,0x840c0:3,0x840e8:3,0x84110:3}
    pointers={a:list(struct.unpack('<'+'I'*n,read(a,4*n))) for a,n in tables.items()}
    round_queue=[0x61460]+[x-BASE for a in (0x7a3f4,0x7abf0) for x in pointers[a]]
    round_ins={}
    while round_queue:
        r=round_queue.pop()
        while r not in round_ins:
            if not 0x61460<=r<0x7a3f4:raise ValueError('RoundMult flow escapes code')
            ins=next(md.disasm(read(r,15),BASE+r,count=1));round_ins[r]=(r,ins.size)
            if ins.group(CS_GRP_RET):break
            if ins.group(CS_GRP_JUMP):
                op=ins.operands[0]
                if op.type==X86_OP_IMM:round_queue.append(op.imm-BASE)
                elif op.type!=X86_OP_MEM or op.mem.disp-BASE not in (0x7a3f4,0x7abf0):raise ValueError('Unknown RoundMult jump')
                if ins.mnemonic=='jmp':break
            if ins.mnemonic in ('int','int3'):raise ValueError('Reachable trap in original')
            r+=ins.size
    cursor=0x61460
    for addr,size in sorted(round_ins.values()):
        if addr!=cursor:raise ValueError('Overlapping or missing RoundMult instructions')
        cursor+=size
    if sum(n for _,n in round_ins.values())!=0x7a3f4-0x61460:raise ValueError('RoundMult instruction coverage incomplete')
    helpers={0x4e746,0x4e80a,0x4e81c,0x4ed94,0x4e830}
    rows={}
    for entry in selected:
        f=functions[entry]
        if f['external']:continue
        rva=f['rva']
        if rva in helpers:continue
        pairs=round_ins.values() if rva==0x61460 else ((i['rva'],i['size']) for i in f['instructions'])
        rows[rva]=[]
        for address,size in sorted(pairs):
            ins=next(md.disasm(read(address,size),BASE+address,count=1))
            if ins.size!=size:raise ValueError('Instruction boundary disagreement')
            rows[rva].append(ins)
    code_addresses={i.address for seq in rows.values() for i in seq}|{BASE+x for x in helpers}|{BASE+0x833f0}
    for a,targets in pointers.items():
        if not all(x in code_addresses for x in targets):raise ValueError('Unresolved owned jump target '+hex(a))
    # Two original baked grayscale tables and their PC interpolation coefficients.
    # They are data, not encoded test answers. Writable LUT starts empty, as in PE BSS.
    ranges=[(0xb9608,0xbd950),(0xc015c,0xc0160),(0xc0160,0xc1c64)]
    for a,n in [(0x7be44,25),(0x840cc,25),(0x840f4,25),(0x8411c,25)]:ranges.append((a,a+n))
    refs={}
    for seq in rows.values():
        for ins in seq:
            for op in ins.operands:
                if op.type==X86_OP_MEM and BASE<=op.mem.disp<BASE+0xd1000:refs[op.mem.disp-BASE]=max(refs.get(op.mem.disp-BASE,0),op.size)
                if op.type==X86_OP_IMM and not ins.group(CS_GRP_CALL) and not ins.group(CS_GRP_JUMP) and BASE<=op.imm<BASE+0xd1000:refs[op.imm-BASE]=max(refs.get(op.imm-BASE,0),1)
    for a in sorted(refs):
        if a in tables or a==0x970e0:continue
        if not any(lo<=a<hi for lo,hi in ranges):ranges.append((a,a+refs[a]))
    merged=[]
    for lo,hi in sorted(ranges):
        if merged and lo<=merged[-1][1]:merged[-1]=(merged[-1][0],max(hi,merged[-1][1]))
        else:merged.append((lo,hi))
    def dsym(addr):
        if addr in tables:return 'T_%08X'%addr
        for lo,hi in merged:
            if lo<=addr<hi:return 'D_%08X'%lo+(' + %d'%(addr-lo) if addr!=lo else '')
        raise ValueError('Unresolved owned datum '+hex(addr))
    # Refuse absolute relocation pointers in copied data. Every code-pointer table
    # above is instead emitted as symbolic DD OFFSET Q_ labels in this module.
    reloc_rva,reloc_size=struct.unpack_from('<II',data,pe+24+96+5*8)
    pos=0
    while pos<reloc_size:
        page,size=struct.unpack('<II',read(reloc_rva+pos,8))
        if not size:break
        for off in range(8,size,2):
            value=struct.unpack('<H',read(reloc_rva+pos+off,2))[0]
            location=page+(value&4095)
            if value>>12==3 and any(lo<=location<hi for lo,hi in merged):raise ValueError('Unexpected pointer in owned data '+hex(location))
        pos+=size
    def number(s):
        return re.sub(r'0x([0-9a-fA-F]+)',lambda m:'0'+m[1].upper()+'h',s)
    def operand_symbol(value,memory):
        if value in code_addresses:return 'Q_%08X'%value
        symbol=dsym(value-BASE)
        return symbol if memory else 'OFFSET '+symbol
    def asm(ins):
        raw=bytes(ins.bytes)
        # Capstone shortens x87 register arithmetic to an implicit operand;
        # MASM requires both. Decode the destination from the opcode byte.
        if len(raw)==2 and raw[0] in (0xd8,0xdc,0xde) and raw[1]>=0xc0:
            group=raw[1]&0xf8;reg=raw[1]&7
            names={0xd8:{0xc0:'fadd',0xc8:'fmul',0xe0:'fsub',0xe8:'fsubr',0xf0:'fdiv',0xf8:'fdivr'},
                   0xdc:{0xc0:'fadd',0xc8:'fmul',0xe0:'fsubr',0xe8:'fsub',0xf0:'fdivr',0xf8:'fdiv'},
                   0xde:{0xc0:'faddp',0xc8:'fmulp',0xe0:'fsubrp',0xe8:'fsubp',0xf0:'fdivrp',0xf8:'fdivp'}}
            if group in names[raw[0]]:
                operands=('st(0),st(%d)'%reg) if raw[0]==0xd8 else ('st(%d),st(0)'%reg)
                return names[raw[0]][group]+' '+operands
        if ins.mnemonic in ('fcomi','fcomip','fucomi','fucomip') and ',' not in ins.op_str:
            return ins.mnemonic+' st(0),'+ins.op_str
        if ins.mnemonic=='rep stosd':return 'rep stosd'
        if ins.mnemonic=='rep movsd':return 'rep movsd'
        if ins.address==BASE+0x8b342:return 'call ColorCeil'
        s=ins.mnemonic+' '+ins.op_str
        if ins.group(CS_GRP_CALL) or ins.group(CS_GRP_JUMP):
            if ins.operands[0].type==X86_OP_IMM:
                address=ins.operands[0].imm
                if address not in code_addresses:raise ValueError('Unresolved call/jump '+hex(address))
                s=ins.mnemonic+' Q_%08X'%address
        def replace(m):
            value=int(m[0],16)
            if not BASE<=value<BASE+0xd1000:return m[0]
            before=s[:m.start()]
            return operand_symbol(value,before.rfind('[')>before.rfind(']'))
        s=re.sub(r'0x[0-9a-fA-F]+',replace,s)
        return number(s)
    lines=['; PC Production=71 colour search. Generated as assembly source, not a code blob.',
           '; Original SHA256 '+ORIGINAL_SHA,'; Graph source: '+GRAPH,
           '; 72/exhaustive is rejected before entry. All constants and jump tables are owned.',
           '.686p','.xmm','.model flat','option casemap:none','option prologue:none','option epilogue:none','.data']
    datamanifest=[]
    for lo,hi in merged:
        blob=read(lo,hi-lo);lines.append('ALIGN 16');lines.append('D_%08X LABEL BYTE'%lo)
        if not any(blob):lines.append('DB %d DUP (0)'%len(blob))
        else:
            for p in range(0,len(blob),16):lines.append('DB '+','.join('0%02Xh'%x for x in blob[p:p+16]))
        datamanifest.append(dict(rva=lo,bytes=len(blob),sha256=hashlib.sha256(blob).hexdigest()))
    for a,targets in pointers.items():
        lines+=['ALIGN 4','T_%08X LABEL DWORD'%a]
        for p in range(0,len(targets),8):lines.append('DD '+','.join('OFFSET Q_%08X'%x for x in targets[p:p+8]))
    lines+=['.code','PUBLIC _ColorCoreAsm','PUBLIC _ColorNoModeAsm','PUBLIC _ColorErrorAsm','PUBLIC _memset','PUBLIC _memcpy']
    helper_text=(ROOT/'recovery/scripts/TextureColourHelpers.inc').read_text()
    lines.append(helper_text)
    aliases={0x7c420:'_ColorCoreAsm',0x83cd0:'_ColorNoModeAsm',0x7be60:'_ColorErrorAsm'}
    for f,seq in sorted(rows.items()):
        lines+=['ALIGN 16','; Original function RVA %08X'%f,'C_%08X LABEL NEAR'%f]
        if f in aliases:lines.append(aliases[f]+' LABEL NEAR')
        for ins in seq:lines+=['Q_%08X:'%ins.address,'    '+asm(ins)]

    lines+=['END']
    out=ROOT/'src/TextureCompiler/Colour';out.mkdir(exist_ok=True)
    (out/'TextureColour.asm').write_text('\n'.join(lines)+'\n')
    manifest=dict(originalSha256=ORIGINAL_SHA,graphSourceCommit='7367b839f2a7fa780b2bf5040dbe3d059c674940',graphRunId='40496512115',scope='PC quality71 code/data reconstruction; not acceptance',functions=sorted(rows),instructions=sum(map(len,rows.values())),roundMultInstructions=len(round_ins),roundMultCodeBytes=sum(n for _,n in round_ins.values()),jumpTables={hex(a):len(v) for a,v in pointers.items()},data=datamanifest)
    (out/'SOURCE_MAP.json').write_text(json.dumps(manifest,indent=2)+'\n')
    print(json.dumps({k:manifest[k] for k in ('instructions','roundMultInstructions','roundMultCodeBytes','jumpTables')}))

if __name__=='__main__':generate()
