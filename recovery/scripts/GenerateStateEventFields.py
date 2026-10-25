"""Generate complete state/event contracts from 23 fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/state-event-source'
SPECS=[('StoreCurrentStateUpdateModuleData',3201389610,16,154,[('SleepTime',8,'Time','0s'),('MaxTime',12,'Time','5s')],[]),('LUAEventListBehaviorModuleData',4201302198,16,155,[('EventListName',8,'String',None)],[])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==23
assert sorted((m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules'])==sorted((n,t,z) for n,t,z,k,f,c in SPECS)
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
lines=['// Complete state/event contracts; original blob '+BLOB+'.']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'));actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField stateEvent'+name+'Fields[] = {']
 for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr')]
 lines+=['};']
lines+=['const ModuleFieldSet stateEventFieldSets[] = {']+[f'    {{ stateEvent{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested=('&dieBehaviorModuleData' if kind==155 else '&spawnUpdateModuleData') if key==1 else 'nullptr';mode=stride=0
  records.append((key,tokenType,off,nested,mode,stride))
 assert len(records)==len(fields)+len(children)+1,(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken stateEvent'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout stateEvent{name} = {{ stateEvent{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice stateEventChoices[] = {']+[f'    {{ {t}u, &stateEvent{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectStateEvent.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectStateEvent.h'
else:path.write_text(output)
print('PASS complete state/event contracts and 23 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
