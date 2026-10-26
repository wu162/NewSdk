"""Raw original AssetMerge differential; component preparation, no CLI enablement."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess

EXTS=('.manifest','.bin','.imp','.relo')

def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()
def row(t=1,i=2,source=b'old/a.xml',refs=(),size=12):
    return dict(type=t,id=i,source=source,refs=refs,size=size)

def linked(stem,rows):
    table=bytearray();refs=bytearray();names=bytearray();sources=bytearray();data=bytearray();imp=bytearray();relo=bytearray()
    for index,r in enumerate(rows):
        name=f'Type:old{index}'.encode();source=r['source'];reference=b''.join(struct.pack('<II',*x) for x in r['refs']);size=r['size']
        table+=struct.pack('<12I',r['type'],r['id'],0x12345678,0x87654321,len(refs),len(r['refs']),len(names),len(sources),size,4,4,0)
        refs+=reference;names+=name+b'\0';sources+=source+b'\0';data+=bytes([index+17])*size;imp+=b'\xff'*4;relo+=b'\xff'*4
    # Original CC3Tools only allocates ReferencedManifests[0] when this
    # table is nonempty, while AssetMerge unconditionally indexes it. A
    # complete ordinary MOD stream contains a base reference; an empty
    # table crashes the legacy vector access before merge/no-assets logic.
    streams=b'\x01base.manifest\0'
    header=struct.pack('<BBH11I',0,1,6,0x44332211,0x54ee1e64,len(rows),len(data),max([r['size'] for r in rows] or [0]),4,4,len(refs),len(streams),len(names),len(sources))
    stem.with_suffix('.manifest').write_bytes(header+table+refs+streams+names+sources)
    for ext,payload in [('.bin',data),('.imp',imp),('.relo',relo)]:stem.with_suffix(ext).write_bytes(struct.pack('<I',0x44332211)+payload)

def precompiled(path,r,index):
    name=f'Type:new{index}'.encode();source=r['source'];size=r['size']
    value=struct.pack('<8I',r['type'],r['id'],0x88888888,0x99999999,len(r['refs']),size,4,4)
    value+=b''.join(struct.pack('<II',*x) for x in r['refs'])+struct.pack('<I',len(name))+name+struct.pack('<I',len(source))+source
    path.write_bytes(value+bytes([index+77])*size+b'\xff'*8)

def invoke(exe,stem,assets,logs,cancel=False):
    result=subprocess.run([str(exe),str(stem),str(assets)]+(['cancel'] if cancel else []),capture_output=True,timeout=60,cwd=exe.parent)
    (logs/'stdout.log').write_bytes(result.stdout);(logs/'stderr.log').write_bytes(result.stderr);return result

def main():
    parser=argparse.ArgumentParser()
    for name in ('original','candidate','output'):parser.add_argument('--'+name,required=True)
    a=parser.parse_args();original=Path(a.original).resolve();candidate=Path(a.candidate).resolve();out=Path(a.output).resolve();out.mkdir(parents=True,exist_ok=False)
    state=dict(schemaVersion=1,succeeded=False,sourceCommit=subprocess.check_output(['git','rev-parse','HEAD'],text=True).strip(),originalSha256=sha(original),candidateSha256=sha(candidate),cases=[],scope='AssetMerge component defined-output subset; duplicate/substring source-name layouts rejected before mutation; no production CLI merge support')
    cases=[('no-assets',[row()],[]),('prepend',[row()],[row(3,4,b'new/b.xml')]),
      ('duplicate-identity-prepended',[row()],[row(1,2,b'new/c.xml')]),
      ('all-maxima-use-instance-size',[row(size=4)],[row(3,4,b'new/d.xml',size=24)]),
      ('polymorphic-last-match',[row(refs=[(0xb22f87c9,100)])],[row(0xe3181c04,100,b'new/e.xml'),row(0xf0f08712,100,b'new/f.xml')]),
      ('incoming-references-unchanged',[row()],[row(3,4,b'new/g.xml',refs=[(0xb22f87c9,100)]),row(0xc2b1a262,100,b'new/h.xml')]),
      ('high-bits',[row(0xffffffff,0x80000000)],[row(0x80000000,0xffffffff,b'new/i.xml')])]
    try:
        for name,current,incoming in cases:
            case=out/name;case.mkdir();record=dict(name=name,passed=False);state['cases'].append(record)
            for kind,exe in [('original',original),('candidate',candidate)]:
                work=case/kind;work.mkdir();stem=work/'mod';assets=work/'assets';assets.mkdir();linked(stem,current)
                for index,r in enumerate(incoming):precompiled(assets/f'{index:03}.asset',r,index)
                (assets/'00000000.11111111.22222222.33333333.cdata').write_bytes(b'new-cdata')
                cdata=stem/'cdata';cdata.mkdir(parents=True);(cdata/'existing.cdata').write_bytes(b'existing');(cdata/'00000000.11111111.22222222.33333333.cdata').write_bytes(b'old-cdata')
                before={p.name:sha(p) for p in assets.iterdir()};result=invoke(exe,stem,assets,work)
                record.setdefault('invocations',[]).append(dict(kind=kind,arguments=[str(exe),str(stem),str(assets)],cwd=str(exe.parent),exitCode=result.returncode,streamTableBytes=struct.unpack_from('<I',stem.with_suffix('.manifest').read_bytes(),36)[0]));assert result.returncode==0,(kind,name,result.returncode,result.stderr)
                assert before=={p.name:sha(p) for p in assets.iterdir()},'input assets modified'
                if kind=='candidate':assert result.stdout.strip()==str(len(incoming)).encode(),result.stdout
            left=case/'original';right=case/'candidate'
            for ext in EXTS:assert (left/('mod'+ext)).read_bytes()==(right/('mod'+ext)).read_bytes(),f'{name}: raw {ext} differs'
            left_cdata={p.name:p.read_bytes() for p in (left/'mod/cdata').iterdir()};right_cdata={p.name:p.read_bytes() for p in (right/'mod/cdata').iterdir()}
            assert left_cdata==right_cdata,'CDATA overwrite/preservation differs'
            record.update(passed=True,rawFourFilesIdentical=True,cdataIdentical=True,manifestSha256=sha(left/'mod.manifest'))
        for name,corrupt,cancel in [('cancel',False,True),('malformed',True,False),('undefined-source-tail',False,False)]:
            case=out/name;case.mkdir();stem=case/'mod';assets=case/'assets';assets.mkdir();linked(stem,[row()]);precompiled(assets/'a.asset',row(3,4,b'old/a.xml' if name=='undefined-source-tail' else b'new/j.xml'),0)
            if corrupt:(assets/'a.asset').write_bytes(b'truncated')
            before={ext:sha(stem.with_suffix(ext)) for ext in EXTS};result=invoke(candidate,stem,assets,case,cancel)
            assert result.returncode==(6 if cancel else 7),(name,result.returncode,result.stderr)
            assert before=={ext:sha(stem.with_suffix(ext)) for ext in EXTS},'candidate changed output before rejection'
            state['cases'].append(dict(name=name,passed=True,refusedBeforeMutation=True))
        state['succeeded']=True
    finally:(out/'validation.json').write_text(json.dumps(state,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(dict(succeeded=True,cases=len(state['cases']))))

if __name__=='__main__':main()
