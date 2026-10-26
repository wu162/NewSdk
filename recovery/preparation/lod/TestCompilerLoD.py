"""Isolated original-tool differential for the production RA3 v6 LoD stage.

Not a production CLI/base-package acceptance receipt. Raw inputs and all eight
outputs from both implementations are retained; the original exit code alone
does not establish success (its exception handler may return zero).
"""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def fixture(count, linked=True, streams=None, checksum=0xf1234567):
    assets=b''.join(struct.pack('<12I',*(0x1000*(i+1)+j for j in range(12))) for i in range(count))
    refs=b'\x10\x20\x30\x40\x50\x60\x70\x80' if count else b''
    if streams is None:streams=b'\x02old.manifest\0\x01other.manifest\0'
    names=b'Test:one\0Test:two\0' if count else b''
    sources=b'Data/test.xml\0' if count else b''
    header=struct.pack('<BBH11I',0,int(linked),6,checksum,0x12345678,count,1234,456,88,99,len(refs),len(streams),len(names),len(sources))
    return header+assets+refs+streams+names+sources


def run(exe, manifest, directory, extra=()):
    p=subprocess.run([str(exe),str(manifest),*extra],cwd=directory,capture_output=True,timeout=30)
    (directory/'stdout.log').write_bytes(p.stdout)
    (directory/'stderr.log').write_bytes(p.stderr)
    return p.returncode


def main():
    p=argparse.ArgumentParser()
    for n in ('original','candidate','output'):p.add_argument('--'+n,required=True)
    p.add_argument('--real-manifest',action='append',default=[])
    a=p.parse_args();out=Path(a.output).resolve();out.mkdir(parents=True,exist_ok=False)
    original=Path(a.original).resolve();candidate=Path(a.candidate).resolve()
    state={'schemaVersion':1,'sourceCommit':subprocess.check_output(['git','rev-parse','HEAD'],text=True).strip(),
           'originalSha256':sha(original),'candidateSha256':sha(candidate),
           'productionSource':{'path':'recovery/src/App/Application/LoDStreamBuilder.cs',
                               'sha256':sha(Path(__file__).resolve().parents[2]/'src/App/Application/LoDStreamBuilder.cs')},
           'succeeded':False,'cases':[],
           'scope':'RA3 PC v6 LoD component only; original eight-output byte differential and rejection tests; complete production base packaging acceptance is separate'}
    try:
        inputs=[(f'assets-{count}',fixture(count),'mod.test.manifest') for count in (0,1,3,128)]
        inputs += [
            ('no-old-dependencies',fixture(3,streams=b''),'mod.test.manifest'),
            ('zero-checksum',fixture(1,checksum=0),'mod.test.manifest'),
            ('full-checksum',fixture(1,checksum=0xffffffff),'Mod Space.TEST.MANIFEST'),
        ]
        inputs += [('sdk-'+Path(file).stem,Path(file).read_bytes(),'mod.test.manifest') for file in a.real_manifest]
        for name,data,basename in inputs:
            case=out/name;case.mkdir()
            for name in ('original','candidate'):
                work=case/name;work.mkdir();(work/basename).write_bytes(data)
            expected=[]
            for suffix in ('_l','_m'):
                for ext in ('.manifest','.bin','.imp','.relo'):expected.append(Path(basename).stem+suffix+ext)
            row={'name':case.name,'inputSha256':hashlib.sha256(data).hexdigest(),'files':[]};state['cases'].append(row)
            for name,exe in (('original',original),('candidate',candidate)):
                work=case/name;before=sha(work/basename)
                code=run(exe,work/basename,work)
                assert code==0,(name,code,(work/'stderr.log').read_bytes())
                assert sha(work/basename)==before,'Input manifest changed'
                assert {f.name for f in work.iterdir()}==set(expected+[basename,'stdout.log','stderr.log']),'Output inventory differs'
            for file in expected:
                left=case/'original'/file;right=case/'candidate'/file
                assert left.read_bytes()==right.read_bytes(),f'Original/candidate raw bytes differ: {case.name}/{file}'
                row['files'].append({'path':file,'sha256':sha(left),'length':left.stat().st_size,'byteIdentical':True})
            row['passed']=True
        for name,mutate in (
            ('truncated',lambda b:b[:-1]),
            ('bad-count',lambda b:b[:12]+struct.pack('<I',0xffffffff)+b[16:]),
            ('wrong-version',lambda b:b[:2]+b'\x07\x00'+b[4:]),
            ('unlinked',lambda b:b[:1]+b'\0'+b[2:]),
            ('big-endian',lambda b:b'\x01'+b[1:]),
            ('trailing-data',lambda b:b+b'X'),
            ('pre-cancel',lambda b:b),
            ('existing-output',lambda b:b),
        ):
            work=out/name;work.mkdir();manifest=work/'mod.manifest';manifest.write_bytes(mutate(fixture(3)))
            before={f.name:sha(f) for f in work.iterdir()}
            if name=='existing-output':
                (work/'mod_m.relo').write_bytes(b'previous');before['mod_m.relo']=sha(work/'mod_m.relo')
            expected=6 if name=='pre-cancel' else 7
            assert run(candidate,manifest,work,('cancel',) if name=='pre-cancel' else ())==expected,name
            assert {f.name:sha(f) for f in work.iterdir() if f.suffix!='.log'}==before,'Rejected input changed output'
            state['cases'].append({'name':name,'passed':True,'expectedExit':expected})
        state['succeeded']=True
    finally:
        (out/'validation.json').write_text(json.dumps(state,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'succeeded':True,'cases':len(state['cases'])}))


if __name__=='__main__':main()
