"""Original HashFix differential in an isolated SDK layout without registry edits."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import subprocess


def manifest(rows):
    table=b''.join(struct.pack('<12I',typeid,instance,99,hash,0,0,0,0,16,4,4,88) for typeid,instance,hash in rows)
    tail=b'\x02base.manifest\0Test:item\0Data/test.xml\0'
    return struct.pack('<BBH11I',0,1,6,0xf1234567,0x12345678,len(rows),1234,456,88,99,0,15,10,14)+table+tail


def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    p=argparse.ArgumentParser()
    for n in ('original','candidate','output'):p.add_argument('--'+n,required=True)
    a=p.parse_args();out=Path(a.output).resolve();out.mkdir(parents=True,exist_ok=False)
    original=Path(a.original).resolve();candidate=Path(a.candidate).resolve()
    state={'schemaVersion':1,'succeeded':False,'cases':[],
           'sourceCommit':subprocess.check_output(['git','rev-parse','HEAD'],text=True).strip(),
           'originalSha256':sha(original),'candidateSha256':sha(candidate),
           'scope':'Prepared HashFix component; no production base packaging or complete neutral-asset repair claim'}
    cases=[('no-match',[(1,2,3)],[(4,5,6)],0),('change',[(1,2,3)],[(1,2,7)],1),
           ('same',[(1,2,3)],[(1,2,3)],0),('duplicate-same-then-different',[(1,2,3)],[(1,2,3),(1,2,4)],1),
           ('duplicate-first-different',[(1,2,3)],[(1,2,4),(1,2,5)],1),
           ('high-bit',[(0xffffffff,0x80000000,0xffffffff)],[(0xffffffff,0x80000000,0)],1),
           ('type-mismatch',[(1,2,3)],[(2,2,7)],0),('empty',[],[],0),
           ('mixed',[(1,2,3),(4,5,6),(7,8,9)],[(7,8,9),(1,2,4)],1)]
    try:
        for name,mod,wb,changed in cases:
            case=out/name;case.mkdir();row={'name':name,'expectedChanged':changed};state['cases'].append(row)
            for kind,exe in [('original',original),('candidate',candidate)]:
                work=case/kind;work.mkdir();tools=work/'tools';tools.mkdir();builtmods=work/'BuiltMods'/'SageXml';builtmods.mkdir(parents=True)
                source=work/'mod.manifest';source.write_bytes(manifest(mod))
                ref=builtmods/'Worldbuilder.manifest';ref.write_bytes(manifest(wb));before=sha(ref)
                args=[str(exe),str(source)]+([str(ref)] if kind=='candidate' else [])
                result=subprocess.run(args,cwd=tools,capture_output=True,timeout=30)
                (work/'stdout.log').write_bytes(result.stdout);(work/'stderr.log').write_bytes(result.stderr)
                assert result.returncode==0,(kind,name,result.returncode,result.stderr)
                assert sha(ref)==before,'HashFix modified Worldbuilder input'
                if kind=='original':assert f'Instance hashes fixed: {changed}'.encode() in result.stdout,result.stdout
                else:assert result.stdout.strip()==str(changed).encode(),result.stdout
            left=case/'original/mod.manifest';right=case/'candidate/mod.manifest'
            assert left.read_bytes()==right.read_bytes(),'Raw HashFix manifest difference: '+name
            row.update(passed=True,byteIdentical=True,sha256=sha(left))
        state['succeeded']=True
    finally:(out/'validation.json').write_text(json.dumps(state,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'succeeded':True,'cases':len(cases)}))


if __name__=='__main__':main()
