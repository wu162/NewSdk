#!/usr/bin/env python3
"""Independent full saved PC-native capture audit; no compiler, DLL or codec execution."""
import argparse
import hashlib
import json
from pathlib import Path
import struct

ROOT=Path(__file__).resolve().parents[2]

def sha(b):return hashlib.sha256(b).hexdigest()
def words(*values):return struct.pack('<'+'I'*len(values),*(v&0xffffffff for v in values))
def store(b,offset,value):struct.pack_into('<I',b,offset,value&0xffffffff)

def device_model():
    snapshot=bytearray(0x5500)
    for offset,value in [(0x3c,1),(0x2a80,0x1ddd4000),(0x2a84,0x20000),(0x2a88,1),(0x3864,1)]:store(snapshot,offset,value)
    snapshot+=bytes(0x12c0)
    result=bytearray(b'TXDEV001')
    def case(adapter,kind,flags,seed,parameters):
        result.extend(words(adapter,kind,flags,parameters,0));result.extend(snapshot)
        refs=2+seed%7
        result.extend(words(*range(refs-1,0,-1),0))
    values=[0,1,3,4,0x7fffffff,0x80000000,0xfffffffe,0xffffffff]
    seed=0
    for adapter in values:
        for kind in values:
            for flags in values:
                for parameters in [0,1]:case(adapter,kind,flags,seed,parameters);seed+=1
    result.extend(snapshot*16)
    for thread in range(4):
        for i in range(16):case(i,1,0xffffffff,thread*16+i,i&1)
    for count in [0,2,3,0x7fffffff,0x80000000,0xffffffff]:
        b=bytearray(b'\xa5'*0x5500);store(b,0x3c,count-1);result.extend(words(count,count-1));result.extend(b)
    return bytes(result)

def resource_model():
    result=bytearray(b'TXRES001')
    for kind in range(16):
        for locks in range(16):
            for high in [0,0x40000000,0x80000000,0xf0000000]:
                for secondary in [False,True]:
                    for level in [0,1,31,0xffffffff]:
                        b=bytearray(b'\xa5'*64);before=high|(locks<<8)|kind;store(b,0,before-256);store(b,0x30,0x12345003 if secondary else 3)
                        if before&0xf00==0x100:
                            store(b,0x14,0xffff0000)
                            if secondary:store(b,0x18,0xffff0000)
                        result.extend(words(level,0));result.extend(b)
    counts=[0,1,2,3,0x7ffffffe,0x7fffffff,0x80000000,0xfffffffe,0xffffffff]
    for kind in range(16):
        for high in [0,0x40000000,0x80000000]:
            for count in counts:
                for add in [False,True]:
                    if kind==4 and high&0x40000000 or not add and count==1:continue
                    b=bytearray(b'\xa5'*64);store(b,0,high|kind);value=count+(1 if add else -1);store(b,4,value)
                    result.extend(words(count,int(add),value));result.extend(b)
    payload=bytes((i*17)&255 for i in range(0x2000))
    for kind in range(9):
        for variant in range(8):
            result.extend(words(*range(2,10),*range(8,0,-1)))
            owned=(2 if variant&1 else 1) if kind==3 else int(kind in [1,2,6,7,8])
            result.extend(payload*owned);result.extend(words(kind,variant,owned,0))
    for children in range(1,17):result.extend(words(*range(2,children+2),*range(children-1,-1,-1)))
    for depth in range(1,17):result.extend(words(depth,0))
    result.extend(words(16385,1))
    return bytes(result)

def header(dimension,format,size,flags):
    widths=[1,4,15,16,17,31,64,127];depths=[1,2,3,4,5,7,8,16]
    w,h,d=widths[size],widths[(size*3+1)%8],depths[size];p=[0]*16
    p[0]=3|((size&3)<<8)|(0x200000 if size&1 else 0);p[1]=1;p[5]=0x20001 if size&1 else 0xffff0000;p[6]=0x30002 if size&1 else 0xffff0000
    p[7]=((w+31)//32)<<22|(0x80000000 if flags&1 else 0);p[8]=0x10000000|format
    if dimension==1 and size&1:p[8]|=0x400
    p[9]=w-1 if dimension==0 else (w-1)|((h-1)<<11)|((d-1)<<22) if dimension==2 else (w-1)|((h-1)<<13)
    if dimension==3 or p[8]&0x400:p[9]|=5<<26
    p[10]=0x80000000 if flags&2 else 0;p[11]=0x40 if size&1 else 0;p[12]=0x20000000|(dimension<<9)|(0x800 if flags&4 else 0)
    return p

def surface_archive(data):
    assert data[:8]==b'TXSRF001';offset=8;ordinal=0;mask=0xffffffff
    table=(ROOT/'recovery/evidence/texture/native-surface-static/20261030/FormatBits.bin').read_bytes()[::2]
    levels=[0,1,2,3,4,5,7,8];lock_flags=[0,1,0x10,0x11,0x20,0x200,0x1000,0xffffffff]
    # Check every saved original public LockRect against its independently saved
    # internal layout, without using the recovered layout algorithm as a model.
    for dimension in range(4):
        for format in range(64):
            block_y=4 if format in [18,19,20,49,51,52,53,58,59,60,61] else 1
            for size in range(8):
                for flags in range(8):
                    for level in levels:
                        row=struct.unpack_from('<28I',data,offset);offset+=112
                        assert row[0]==ordinal and row[9]==0
                        pitch,slice_size,total,packed,*dims=row[1:8]
                        address=(row[8]-(size%6)*((slice_size+4095)&~4095))&mask
                        expected=header(dimension,format,size,flags)
                        lock=lock_flags[(size+flags+level)%8]|2
                        if (lock&0x10 or expected[0]&0x200000) and not lock&1:
                            base=0x20000000 if level else 0x10000000;dirty=6 if level else 5
                            start=(address-packed-base)&mask;old=expected[dirty]
                            end=max(((start+total+127)&mask)>>7,old&0xffff);begin=min(start>>7,old>>16)
                            expected[dirty]=((begin<<16)|end)&mask
                        expected[0]=(expected[0]+256)&mask
                        assert list(row[10:26])==expected, ('lock-state',ordinal)
                        assert row[26]==pitch*block_y&mask,('block-row',ordinal)
                        if ordinal&1:
                            top=(format+level)%5-1;left=(size+flags)%9-1
                            address=(address+top*(pitch*block_y&mask)+(((left&mask)*table[format]&mask)>>3))&mask
                        assert row[27]==address,('rect-address',ordinal)
                        ordinal+=1
    assert ordinal==131072
    for format in [2,3,6,18,19,20,35,57]:
        for level in [0,1]:
            fmt,lvl,pitch,relative=struct.unpack_from('<4I',data,offset);offset+=16
            assert (fmt,lvl)==(format,level) and pitch>0 and relative<0x40000-64
            expected=header(1,format,6,0);expected[0]=259;expected[8]=0x10000000|format;expected[9]=63|(63<<13);expected[11]=0;expected[12]=0x20000200
            assert data[offset:offset+64]==words(*expected);offset+=64
            for storage in [0,1]:
                payload=bytearray((i*(17 if storage==0 else 29)+format)&255 for i in range(0x40000))
                if storage==level:
                    for i in range(64):payload[relative+i]^=(i*13)&255
                assert data[offset:offset+len(payload)]==payload,('owned-storage',format,level,storage);offset+=len(payload)
    assert offset==len(data),('trailing/truncated',offset,len(data))
    return {'completeLayoutRecords':ordinal,'completePublicLockStateRecords':ordinal,'actualStorageRecords':16,'fullOwnedStorageBytes':16*2*0x40000}

def descriptor_archive(data):
    assert data[:8]==b'TXDSC001' and len(data)==8+135168*124
    table=(ROOT/'recovery/evidence/texture/native-descriptor-static/20261030/XgFormatBits.bin').read_bytes()
    mask=0xffffffff;ordinal=0
    def verify(p,level):
        nonlocal ordinal
        row=struct.unpack_from('<31I',data,8+ordinal*124)
        assert row[0]==ordinal
        fmt=((((p[10]&0x1fff)<<10)|(p[7]&0x3fc))<<7)|((p[7]>>23)&0x100)|(p[8]&0xff);fmt&=mask
        dimension=p[12]>>9&3;kind=20 if dimension==0 else 17 if dimension==2 else 18 if dimension==3 else 19 if p[8]&0x400 else 3
        pad=(p[10]>>31)*2;add=pad+1;n=p[9]
        if dimension==0:w,h,d=(n&0xffffff)+add,1,1
        elif dimension==2:w,h,d=(n&0x7ff)+add,(n>>11&0x7ff)+add,(n>>22)+add
        else:w,h,d=(n&0x1fff)+add,(n>>13&0x1fff)+add,1
        if level:
            w=(max(((w-pad)&mask)>>(level&31),1)+pad)&mask;h=(max(((h-pad)&mask)>>(level&31),1)+pad)&mask
            d=(max(((d-pad)&mask)>>(level&31),1)+pad)&mask if dimension==2 else 1
        assert list(row[1:9])==[fmt,kind,0,0,0,0,w,h],('complete-level',ordinal)
        assert list(row[9:16])==[fmt,kind,0,0,w,h,d],('complete-volume',ordinal)
        x=row[16:31];height=1 if kind==20 else h;depth=p[9]>>26 if kind==18 else d if kind==17 else 1
        bits=table[fmt&63];f=fmt&63
        bx,by=(2,1) if f in [11,12,40] else (4,1) if f in [39,44] else (4,4) if f in [18,19,20,49,51,52,53,58,59,60,61] else (1,1)
        exponent=p[10]>>13&63;exponent=(exponent-64 if exponent&32 else exponent)&mask
        assert list(x[:5])==[kind,w,height,depth,fmt],('xg-dimensions',ordinal)
        assert list(x[7:13])==[bits,((w+bx-1)&mask)//bx,((height+by-1)&mask)//by,depth,bits*bx*by>>3,exponent],('xg-blocks',ordinal)
        assert x[13]&~11==0 and x[13]&1==p[7]>>31 and bool(x[13]&8)==bool(p[10]>>31) and x[14]==0
        ordinal+=1
    for dimension in range(4):
        for format in range(64):
            for size in range(8):
                for flags in range(8):
                    for level in [0,1,2,3,4,5,7,8]:verify(header(dimension,format,size,flags),level)
    state=0x4871bcee
    for i in range(4096):
        p=header(i%4,i//4%64,i//256%8,i%8)
        for offset in [7,8,10,11,12]:state=(state*1664525+1013904223)&mask;p[offset]=state
        verify(p,i%9)
    assert ordinal==135168
    return {'completeHeaderRecords':ordinal,'completeNativeOutputStructures':ordinal*3,'levelStructureBytes':32,'volumeStructureBytes':28,'xgStructureBytes':60,'metadataRecords':4096}


def copy_archive(data):
    assert data[:8]==b'TXCPY001';offset=8;ordinal=0;payload_bytes=0
    table=(ROOT/'recovery/evidence/texture/native-copy-static/20261030/CopyFormatBits.bin').read_bytes()
    widths=[1,4,7,17];heights=[1,2,5,13]
    for format in range(64):
        bx,by=(2,1) if format in [11,12,40] else (4,1) if format in [39,44] else (4,4) if format in [18,19,20,49,51,52,53,58,59,60,61] else (1,1)
        block_bytes=table[format]*bx*by//8
        for size in range(4):
            width,height=widths[size],heights[size]
            for padding in [0,7]:
                stride=((width+bx-1)//bx)*(block_bytes or 1)+padding;rows=(height+by-1)//by;length=stride*rows+64
                for orientation in range(4):
                    source_origin=32+((rows-1)*stride if orientation&1 else 0);destination_origin=32+((rows-1)*stride if orientation&2 else 0)
                    source_pitch=-stride if orientation&1 else stride;destination_pitch=-stride if orientation&2 else stride
                    for flags in [0,3,12,0xffffffff]:
                        for region in range(4):
                            row=struct.unpack_from('<4I',data,offset);offset+=16
                            assert row==(ordinal,0x80004001 if format<2 else 0,length,length)
                            source=bytes((i*13+ordinal*17)&255 for i in range(length));destination=bytearray((i*31+ordinal*19)&255 for i in range(length))
                            if format>=2:
                                left,top,right,bottom,x,y=0,0,width,height,0,0
                                if region in [1,3]:
                                    left=bx if width>=2*bx else 0;top=by if height>=2*by else 0
                                    if region==3:x,y=left,top
                                if region==2:
                                    x=bx if width>=2*bx else 0;y=by if height>=2*by else 0
                                    right=(width-bx)//bx*bx if width>=2*bx else 0;bottom=(height-by)//by*by if height>=2*by else 0
                                count=(right-left+bx-1)//bx*block_bytes;copy_rows=(bottom-top+by-1)//by
                                src=source_origin+(top//by)*source_pitch+(left//bx)*block_bytes;dst=destination_origin+(y//by)*destination_pitch+(x//bx)*block_bytes
                                for y_index in range(copy_rows):
                                    position=dst+y_index*destination_pitch;input_position=src+y_index*source_pitch
                                    assert 0<=position<=length and 0<=input_position<=length
                                    destination[position:position+count]=source[input_position:input_position+count]
                            assert data[offset:offset+length]==source,('complete-source',ordinal);offset+=length
                            assert data[offset:offset+length]==destination,('complete-destination-padding-guards',ordinal);offset+=length
                            payload_bytes+=length*2;ordinal+=1
    assert ordinal==32768 and offset==len(data)
    return {'completeIdentityCopyRecords':ordinal,'successfulCopyRecords':31744,'originalUnsupportedOneBitRecords':1024,'completeSourceDestinationBytes':payload_bytes,'allPaddingAndGuardsVerified':True,'independentCompleteRowCopyModel':True}


def audit(group,run,source):
    directory=ROOT/'recovery/evidence/texture'/group/(run+'-1');r=json.loads((directory/'validation.json').read_text(encoding='utf-8-sig'))
    assert r['status']=='passed' and r['sourceCommit']==source and str(r['runId'])==run and not r['sdkValidated']
    assert r['originalSha256']=='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    assert len(r['configurations'])==2 and {m['configuration'] for m in r['configurations']}=={'Debug','Release'}
    model={'native-device':device_model,'native-resource':resource_model}.get(group)
    expected=model() if model else None;digest=None;details={}
    for m in r['configurations']:
        assert m['passed'] and m['withoutOriginalTextureCompiler'];configuration=m['configuration'];b=(directory/(configuration+'.capture')).read_bytes()
        assert sha(b)==m['captureSha256'] and (digest is None or digest==sha(b));digest=sha(b)
        if expected is not None:assert b==expected,'Complete independent model differs: '+group
        else:details=({'native-descriptor':descriptor_archive,'native-copy':copy_archive}.get(group,surface_archive))(b)
        dlls=list(directory.glob(configuration+'-Recovered.*.dll'));assert len(dlls)==1 and sha(dlls[0].read_bytes())==m['nativeSha256']
        dll=dlls[0].read_bytes();pe=struct.unpack_from('<I',dll,60)[0];assert dll[pe:pe+4]==b'PE\0\0' and struct.unpack_from('<H',dll,pe+4)[0]==0x14c
    return {'group':group,'sourceCommit':source,'runId':run,'completeDebugReleaseCaptureBytes':len(b),'captureSha256':digest,'independentCompleteFixtureModel':model is not None,**details}

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--group',choices=['native-device','native-resource','native-surface','native-descriptor','native-copy'],required=True);p.add_argument('--run-id',required=True);p.add_argument('--source-commit',required=True);p.add_argument('--out',type=Path,required=True);a=p.parse_args()
    result=audit(a.group,a.run_id,a.source_commit);a.out.parent.mkdir(parents=True,exist_ok=True);a.out.write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result))
