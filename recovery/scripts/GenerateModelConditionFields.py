"""Generate complete model condition contracts from 42 fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/model-condition-source'
SPECS=[('MonitorSpecialPowerTimerUpdateModuleData',2376148578,72,156,[('SpecialPowerTemplate',8,'Reference',None),('ReadyCondition',12,'ModelCondition',None)],[]),('RandomModelConditionBehaviorModuleData',3721928425,16,157,[('Conditions',8,'StringList',None)],[]),('MonitorConditionUpdateModuleData',267133387,84,158,[('ModelConditionFlags',8,'ModelCondition',None),('OCL',68,'Reference',None),('PreDelay',72,'Time',None),('PostDelay',76,'Time',None),('ActionsOnTriggered',80,'Bits',None)],[])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==42
assert sorted((m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules'])==sorted((n,t,z) for n,t,z,k,f,c in SPECS)
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
assert enums['peSha256']=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
values=enums['MonitorConditionActionType'];assert len(values)==1 and set(values.values())=={0}
if source is not None:
 from GenerateUpgradeFields import enum_values
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==enums['peSha256']
 assert enum_values(source,pe,'MonitorConditionActionType')==values
entries=['// Original Tokenizer PE SHA256 '+enums['peSha256']+'.','// Complete MonitorConditionActionType table; unknown words set bit0, ALL fills its complete word.','const EnumNameValue modelConditionActionNames[] = {']+['    { %s, %d },'%(json.dumps(n),v) for n,v in values.items()]+['};','const AIEnum modelConditionActionEnum = { modelConditionActionNames, 1u, 0, 1u };','']
enumOutput='\n'.join(entries);enumPath=ROOT/'recovery/src/Native/GameObjectModelConditionEnums.h'
if '--check' in sys.argv:assert enumPath.read_text()==enumOutput,'Regenerate GameObjectModelConditionEnums.h'
else:enumPath.write_text(enumOutput)
lines=['// Complete model condition contracts; original blob '+BLOB+'.','#include "GameObjectModelConditionEnums.h"']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'));actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField modelCondition'+name+'Fields[] = {']
 for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&modelConditionActionEnum' if k=='Bits' else 'nullptr')]
 lines+=['};']
lines+=['const ModuleFieldSet modelConditionFieldSets[] = {']+[f'    {{ modelCondition{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested=('&queueParkingCreateModuleData' if kind==157 else '&spawnUpdateModuleData') if key==1 else 'nullptr';mode=stride=0
  if kind==157 and off==8:mode,stride=6,8
  records.append((key,tokenType,off,nested,mode,stride))
 assert len(records)==len(fields)+len(children)+1,(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken modelCondition'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout modelCondition{name} = {{ modelCondition{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice modelConditionChoices[] = {']+[f'    {{ {t}u, &modelCondition{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectModelCondition.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectModelCondition.h'
else:path.write_text(output)
print('PASS complete model condition contracts and 42 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
