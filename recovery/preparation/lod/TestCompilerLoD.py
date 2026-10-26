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



def probe_original(original, out):
    """Retain three independent original invocations for an aligned table.

    This is diagnostic evidence, never a replacement for the raw differential.
    The old Delphi code allocates Length bytes then passes array+1 to BlockRead
    and BlockWrite (0040fb23 / 0040fcc2), touching one byte past the array.
    """
    original=Path(original).resolve();out=Path(out).resolve();out.mkdir(parents=True,exist_ok=False)
    assets=struct.pack('<12I',1,1,0x12345678,3,0,0,0,0,2,8,4,0)
    dependencies=b'\x02Static.manifest\0\x02Global.manifest\0\x02Audio.manifest\0'
    names=b'Test:asset1\0';results=[]
    for padding in (0,1,2,3,4):
        sources=b'Data/asset1'+b'x'*padding+b'.xml\0'
        header=struct.pack('<BBH11I',0,1,6,0xf1234567,0x12345678,1,2,2,8,4,0,len(dependencies),len(names),len(sources))
        data=header+assets+dependencies+names+sources
        for repeat in range(3):
            work=out/('names-sources-'+str(len(names)+len(sources))+'-repeat-'+str(repeat));work.mkdir()
            manifest=work/'mod.manifest';manifest.write_bytes(data)
            code=run(original,manifest,work)
            expected=bytearray(data[:96]);expected[80:92]=b'\0'*12
            dependency=b'\x02mod.manifest\0';struct.pack_into('<I',expected,36,len(dependency));expected+=dependency+names+sources
            row={'namesAndSourcesBytes':len(names)+len(sources),'repeat':repeat,'processExitCode':code,
                 'inputSha256':sha(manifest),'mainInputUnchanged':manifest.read_bytes()==data,'outputs':[]}
            for suffix in ('_l','_m'):
                target=work/('mod'+suffix+'.manifest');actual=target.read_bytes()
                row['outputs'].append({'name':target.name,'sha256':sha(target),'bytes':len(actual),
                    'differencesFromLogicalTableCopy':[{'offset':i,'logical':a,'original':b} for i,(a,b) in enumerate(zip(expected,actual)) if a!=b],
                    'lengthDifference':len(actual)-len(expected),'sourceTableFinalByte':actual[-1]})
            results.append(row)
    (out/'probe.json').write_text(json.dumps({'schemaVersion':1,'diagnosticOnly':True,'originalSha256':sha(original),
        'scope':'Original Delphi array+1 boundary investigation; no component/production acceptance claim','cases':results},indent=2)+'\n')
    print(json.dumps({'diagnosticOnly':True,'invocations':len(results),'differences':sum(bool(output['differencesFromLogicalTableCopy'] or output['lengthDifference']) for row in results for output in row['outputs'])}))


def main():
    p=argparse.ArgumentParser()
    for n in ('original','output'):p.add_argument('--'+n,required=True)
    p.add_argument('--candidate')
    p.add_argument('--probe-original',action='store_true')
    p.add_argument('--real-manifest',action='append',default=[])
    a=p.parse_args()
    if a.probe_original:
        probe_original(a.original,a.output);return
    if not a.candidate:p.error('--candidate is required for differential validation')
    out=Path(a.output).resolve();out.mkdir(parents=True,exist_ok=False)
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
