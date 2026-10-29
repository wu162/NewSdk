#!/usr/bin/env python3
"""Independently inspect every saved real-resource consumer byte; never execute a DLL."""
import argparse
import hashlib
import json
from pathlib import Path
import struct

ROOT=Path(__file__).resolve().parents[2]
MASK=0xffffffff

def sha(b):return hashlib.sha256(b).hexdigest()
class Reader:
    def __init__(self,data):self.data=data;self.offset=0
    def take(self,n):
        assert n>=0 and self.offset+n<=len(self.data),'truncated resource archive'
        b=self.data[self.offset:self.offset+n];self.offset+=n;return b
    def word(self):return struct.unpack('<I',self.take(4))[0]
    def words(self,n):return list(struct.unpack('<'+'I'*n,self.take(n*4)))
    def string(self):
        n=self.word();assert n<256;return self.take(n).decode('ascii')
    def end(self):assert self.offset==len(self.data),'trailing resource archive bytes'

def inspect(fixtures,capture):
    f=Reader(fixtures);c=Reader(capture)
    assert f.take(8)==b'TXFIX001' and c.take(8)==b'TXCNS001'
    assert f.word()==c.word()==12
    names=[];success=0;levels_verified=0;payload_bytes=0;copy_calls=0;copy_pixels=0
    bits=(ROOT/'recovery/evidence/texture/native-descriptor-static/20261030/XgFormatBits.bin').read_bytes()
    for _ in range(12):
        name=f.string();status=f.word();assert c.string()==name and c.word()==status;names.append(name)
        if status&0x80000000:
            assert name=='LURomania_vol.dds';continue
        success+=1;header=f.words(13)
        primary=f.take(f.word());secondary=f.take(f.word())
        assert len(primary)>0 and len(primary)%4096==len(secondary)%4096==0
        assert header[0]&15==3 and header[1]==1
        assert header[8]&~0xfff==0x10000000 and header[12]&~0xfff==(0x20000000 if secondary else 0)
        levels=(header[11]>>6&15)+1;assert c.word()==levels and c.words(13)==header
        def pointer(n):return 0x10000000<=n<0x10000000+len(primary) or bool(secondary) and 0x20000000<=n<0x20000000+len(secondary)
        for level in range(levels):
            surface=c.words(8);volume=c.words(7);xg=c.words(15)
            assert surface[:4]==volume[:4] and surface[6:8]==volume[4:6]
            assert xg[0]==surface[1] and xg[1]==surface[6] and xg[4]==surface[0] and xg[14]==0
            layout=c.words(7);address=c.word();assert pointer(address)
            format=header[8]&63;block_y=4 if format in [18,19,20,49,51,52,53,58,59,60,61] else 1
            pitch=layout[0]*block_y&MASK
            for mode in range(3):
                assert c.word()==pitch;locked=c.word();assert pointer(locked)
                expected_address=address
                if mode==1:
                    left=4 if surface[6]>=8 else 0;top=4 if surface[7]>=8 else 0
                    expected_address=(address+top*pitch+(left*bits[format]>>3))&MASK
                assert locked==expected_address
                flags=1 if mode==2 else 0x10 if mode==1 else 0
                if (flags&0x10 or header[0]&0x200000) and not flags&1:
                    offset=6 if level and secondary else 5;base=0x20000000 if level and secondary else 0x10000000
                    start=(address-layout[3]-base)&MASK;old=header[offset]
                    end=max(((start+layout[2]+127)&MASK)>>7,old&0xffff);begin=min(start>>7,old>>16)
                    header[offset]=((begin<<16)|end)&MASK
                header[0]=(header[0]+256)&MASK
                assert c.words(13)==header,('complete-lock-state',name,level,mode)
                if format==6:
                    width=c.word();height=c.word();full=c.word();n=c.word()
                    assert full==surface[0] and not full&0x3fe00
                    left=4 if mode==1 and surface[6]>=8 else 0;top=4 if mode==1 and surface[7]>=8 else 0
                    assert width==surface[6]-left and height==surface[7]-top
                    output=c.take(n);destination_pitch=width*4+7;assert n==destination_pitch*height+64
                    expected=bytearray([0xa5])*n
                    data=primary if locked<0x20000000 else secondary;base=0x10000000 if locked<0x20000000 else 0x20000000
                    source_offset=locked-base;xor=[0,1,3,2][(full>>6)&3];selectors=[(full>>(18+ch*3))&7 for ch in range(4)]
                    assert all(ch<6 for ch in selectors)
                    for row in range(height):
                        pixels=data[source_offset+row*pitch:source_offset+row*pitch+width*4];assert len(pixels)==width*4
                        for column in range(width):
                            physical=[pixels[(column*4+byte)^xor] for byte in range(4)]+[0,255]
                            for byte in range(4):expected[32+row*destination_pitch+column*4+byte]=physical[selectors[byte]]
                    assert output==expected,('complete-endian-copy',name,level,mode)
                    copy_calls+=1;copy_pixels+=width*height
                old=header[0];header[0]=(old-256)&MASK
                if old&0xf00==0x100:
                    header[5]=0xffff0000
                    if secondary:header[6]=0xffff0000
                assert c.words(13)==header,('complete-unlock-state',name,level,mode)
            levels_verified+=1
        assert c.take(len(primary))==primary and c.take(len(secondary))==secondary
        payload_bytes+=len(primary)+len(secondary)
    assert c.word()==copy_calls==120 and c.word()==copy_pixels;f.end();c.end()
    manifest=json.loads((ROOT/'recovery/tests/fixtures/texture-main/manifest.json').read_text())
    assert sorted(names)==sorted(Path(x['fixture']).name for x in manifest['files'])
    assert success>=11
    return {'actualRawCopyCalls':copy_calls,'completeConvertedPixels':copy_pixels,'allRealSources':12,'successfulNativeResources':success,'completeLevelsVerified':levels_verified,'lockUnlockPairsVerified':levels_verified*3,'completeOwnedStorageBytes':payload_bytes,'allRecordsAndSourceAllocationsParsed':True}

def audit(run,source,directory=None):
    directory=Path(directory) if directory else ROOT/'recovery/evidence/texture/native-copy-consumers'/(run+'-1');r=json.loads((directory/'validation.json').read_text(encoding='utf-8-sig'))
    assert r['sourceCommit']==source and str(r['runId'])==run and r['status']=='passed' and not r['sdkValidated']
    assert r['originalSha256']=='4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    manifest=json.loads((ROOT/'recovery/tests/fixtures/texture-main/manifest.json').read_text())
    assert len(r['sourceFixtures'])==12
    for item in manifest['files']:
        b=(ROOT/item['fixture']).read_bytes();name=Path(item['fixture']).name
        assert len(b)==item['bytes'] and sha(b)==item['sha256']
        assert next(f for f in r['sourceFixtures'] if f['file']==name)=={'file':name,'bytes':len(b),'sha256':sha(b)}
    assert len(r['configurations'])==2 and {x['configuration'] for x in r['configurations']}=={'Debug','Release'}
    digests=None
    for m in r['configurations']:
        assert m['passed'] and m['withoutOriginalTextureCompiler'];mode=m['configuration']
        capture=(directory/(mode+'.capture')).read_bytes();fixtures=(directory/(mode+'.fixtures')).read_bytes()
        now=(sha(capture),sha(fixtures));assert now==(m['captureSha256'],m['fixtureSha256']) and (digests is None or now==digests);digests=now
        details=inspect(fixtures,capture)
        assert len(m['nativeFiles'])==5 and {f['file'] for f in m['nativeFiles']}=={'Recovered.TextureNative'+k+'.dll' for k in ['Device','Resource','Surface','Descriptor','Copy']}
        for f in m['nativeFiles']:
            b=(directory/(mode+'-'+f['file'])).read_bytes();assert sha(b)==f['sha256'];pe=struct.unpack_from('<I',b,60)[0];assert b[pe:pe+4]==b'PE\0\0' and struct.unpack_from('<H',b,pe+4)[0]==0x14c
    return {'scope':'complete native resource fixtures/consumer traces, every copied RGBA/padding/guard byte, all ownership/lock-state/source fingerprints and five native DLLs; no recovered file-loader or production-package claim','sourceCommit':source,'runId':run,'completeDebugReleaseCapturesIdentical':True,'captureSha256':digests[0],'resourceFixtureSha256':digests[1],**details}

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--run-id',required=True);p.add_argument('--source-commit',required=True);p.add_argument('--out',type=Path,required=True);p.add_argument('--directory',type=Path);a=p.parse_args();result=audit(a.run_id,a.source_commit,a.directory);a.out.parent.mkdir(parents=True,exist_ok=True);a.out.write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result))
