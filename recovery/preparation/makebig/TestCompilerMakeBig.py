"""Recovered original MakeBig CLI: full archive bytes on the same input tree."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import struct
import subprocess

def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()
def members(path):
    b=path.read_bytes();assert b[:4]==b'BIG4';assert struct.unpack_from('<I',b,4)[0]==len(b)
    count,start=struct.unpack_from('>II',b,8);offset=16;names=[];result={}
    for _ in range(count):
        position,size=struct.unpack_from('>II',b,offset);offset+=8;end=b.index(b'\0',offset);name=b[offset:end].decode('utf-8');offset=end+1
        assert name not in result;assert position+size<=len(b);names.append(name);result[name]=b[position:position+size]
    assert b[offset:start]==b'L219\0\0\0\0';return names,result

def invoke(exe,args,cwd,log,stdin=''):
    result=subprocess.run([str(exe),*args],cwd=cwd,input=stdin.encode(),capture_output=True,timeout=60)
    log.with_suffix('.stdout.log').write_bytes(result.stdout);log.with_suffix('.stderr.log').write_bytes(result.stderr)
    return result

def main():
    p=argparse.ArgumentParser()
    for n in ('original','candidate','output'):p.add_argument('--'+n,required=True)
    a=p.parse_args();original=Path(a.original).resolve();candidate=Path(a.candidate).resolve();out=Path(a.output).resolve();out.mkdir(parents=True,exist_ok=False)
    tree=out/'input';tree.mkdir();child=tree/'ZChild';child.mkdir();nested=child/'Nested';nested.mkdir();(tree/'Empty').mkdir()
    fixtures={'Root.TXT':b'root\0\xff','ignore.asset':b'not shipped','ZChild/Child.BIN':b'child','ZChild/drop.asset':b'excluded','ZChild/Nested/A?.txt':b''}
    # Windows disallows '?' in a filename; exercise '?' in exclusion patterns.
    fixtures.pop('ZChild/Nested/A?.txt');fixtures['ZChild/Nested/Ab.txt']=b'nested';fixtures['ZChild/Nested/zero.bin']=b''
    fixtures['ZChild/界面_É.TXT']='unicode filename'.encode()
    for name,value in fixtures.items():(tree/name).write_bytes(value)
    state=dict(schemaVersion=1,succeeded=False,sourceCommit=subprocess.check_output(['git','rev-parse','HEAD'],text=True).strip(),originalSha256=sha(original),candidateSha256=sha(candidate),cases=[],scope='Original MakeBig recovered parser, fixed BIG4 UTF8 format, child-first native traversal, full-path wildcard exclusion, empty tree, quiet/verbose, force/refusal; same input tree full archive byte identity; production exact-inventory writer tested separately')
    cases=[('default',[],tree),('exclude',['-x:*.asset'],tree),('multi-exclude',['--exclude:*.asset','-x:*a?.txt'],tree),('anchored-exclude',['-x:Root.TXT'],tree),('unknown-option',['-this-is-ignored'],tree),('quiet-verbose',['-q','-v'],tree),('empty',[],tree/'Empty')]
    try:
        for name,options,base in cases:
            case=out/name;case.mkdir();record=dict(name=name,passed=False);state['cases'].append(record)
            results=[]
            for kind,exe in [('original',original),('candidate',candidate)]:
                target=case/(kind+'.big');args=[*options,'--outputFile:'+str(target),str(base)];result=invoke(exe,args,tree,case/kind)
                assert result.returncode==0,(kind,name,result.returncode,result.stderr)
                names,contents=members(target);results.append((target,names,contents));record[kind]=dict(exitCode=result.returncode,sha256=sha(target),names=names)
            assert results[0][0].read_bytes()==results[1][0].read_bytes(),f'{name}: same-tree archive bytes differ'
            names,contents=results[0][1:]
            if name in ('exclude','multi-exclude'):assert not any(n.endswith('.asset') for n in names)
            if name=='multi-exclude':assert not any(n.endswith('ab.txt') for n in names)
            if name=='anchored-exclude':assert 'root.txt' in names,'Filter unexpectedly matched basename instead of full absolute path'
            if name=='default':assert any('界面_é.txt' in n for n in names),'Original UTF8/lowercase filename omitted';assert names.index('zchild\\nested\\ab.txt')<names.index('zchild\\child.bin')<names.index('root.txt')
            if name=='empty':assert not names and results[0][0].stat().st_size==24
            record.update(passed=True,archiveBytesIdentical=True)
        for name,extra,stdin,expected in [('quiet-refuse',['-q'],'',1),('force',['--force'],'',0),('prompt-lower-y',[],'y\n',0),('prompt-upper-second',[],'n\nY\n',0),('prompt-no',[],'n\nn\n',1)]:
            case=out/name;case.mkdir();record=dict(name=name,passed=False);state['cases'].append(record);results=[]
            for kind,exe in [('original',original),('candidate',candidate)]:
                target=case/(kind+'.big');target.write_bytes(b'previous generation');result=invoke(exe,[*extra,'-o:'+str(target),str(tree)],tree,case/kind,stdin)
                assert result.returncode==expected,(kind,name,result.returncode,result.stderr)
                if expected:assert target.read_bytes()==b'previous generation'
                results.append(target.read_bytes())
            assert results[0]==results[1],name;record.update(passed=True,exitCode=expected,archiveBytesIdentical=True)
        for name,args,expected in [('help',['--help'],0),('quiet-missing',['-q'],1),('missing',[],1)]:
            case=out/name;case.mkdir();record=dict(name=name,passed=False);state['cases'].append(record)
            for kind,exe in [('original',original),('candidate',candidate)]:
                result=invoke(exe,args,tree,case/kind);assert result.returncode==expected,(name,kind,result.returncode)
                if name=='quiet-missing':assert not result.stdout
                if name=='help':assert b'Create a big file from a directory' in result.stdout
            record.update(passed=True,exitCode=expected)
        state['succeeded']=True
    finally:(out/'validation.json').write_text(json.dumps(state,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    print(json.dumps(dict(succeeded=True,cases=len(state['cases']))))

if __name__=='__main__':main()
