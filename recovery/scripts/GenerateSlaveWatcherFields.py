"""Generate complete SlaveWatcher contracts from fixed original evidence."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/spawn-crate-watcher-source'
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
source=None
if '--exported-evidence' not in sys.argv:
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode()
    assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
assert len(manifest['methods'])==44
methods={};literals={}
for meta in manifest['methods']:
    text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature'])
    assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
    if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
    methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
SPECS=[('SlaveWatcherBehaviorModuleData',3148647452,20,127,[('GrantUpgrade',8,'Reference',None),('RemoveUpgrade',12,'Reference',None),('ShareUpgrades',16,'Boolean','false'),('LetSlaveLive',17,'Boolean','false')])]
assert [(r['name'],r['typeId'],r['nativeSize']) for r in manifest['modules'] if r['name']=='SlaveWatcherBehaviorModuleData']==[(n,t,s) for n,t,s,k,f in SPECS]
lines=['// Complete SlaveWatcher contracts; original blob '+BLOB+'.']
for name,typ,size,kind,fields in SPECS:
    body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'))
    attrs=[]
    for line in body.splitlines():
        symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if symbols:attrs.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
    assert attrs==[(n,d) for n,o,k,d in fields]+([('CrateList',None),('BlockingUnits',None)] if kind==126 else []),(name,attrs)
    lines+=['const ModuleField crateWatcher'+name+'Fields[] = {']
    for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&spawnCrateFlagsEnum' if k=='Bits' else 'nullptr')]
    lines+=['};']
lines+=['const ModuleFieldSet crateWatcherFieldSets[] = {']+[f'    {{ crateWatcher{n}Fields, {len(f)}u }},' for n,t,s,k,f in SPECS]+['};']
for name,typ,size,kind,fields in SPECS:
    body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'))
    chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
    for chunk in chunks:
        match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
        if not match or match[1]=='0':continue
        key,tokenType=[int(x)&0xffffffff for x in match.groups()];offset=re.search(r'\(byte\*\)@object \+ (\d+)',chunk)
        offset=int(offset[1]) if offset else 0;nested='&spawnUpdateModuleData' if key==1 else 'nullptr';mode=stride=0
        if 'List_003CRelo_003A_003AAssetReference' in chunk:mode,stride=3,4
        if 'ObjectFilter' in chunk:mode=7
        records.append((key,tokenType,offset,nested,mode,stride))
    assert len(records)==(10 if kind==126 else 5)
    assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
    lines+=['const ModuleToken crateWatcher'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout crateWatcher{name} = {{ crateWatcher{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice crateWatcherChoices[] = {']+[f'    {{ {t}u, &crateWatcher{n}, {s}u, {k}u }},' for n,t,s,k,f in SPECS]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectSlaveWatcher.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectSlaveWatcher.h'
else:path.write_text(output)
print('PASS complete SlaveWatcher contracts and 44 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
