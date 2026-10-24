"""Generate the complete Spawn/OCL group from 61 fixed original method contracts."""
from pathlib import Path
import hashlib,json,re,sys
from GenerateLifecycleFields import body_of
base=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
evidence=base/'recovery/evidence/gameobject-modules/spawn-update-source'
manifest=json.loads((evidence/'manifest.json').read_text())
assert manifest['sourceBlob']==BLOB
metas=[m for row in manifest['modules'] for m in row['methods']]+manifest['helpers']
assert len(metas)==61
source=None
if '--exported-evidence' not in sys.argv:
    source=(base/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
    raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
for meta in metas:
    body=body_of((evidence/meta['file']).read_text(),meta['signature'])
    assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
    if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
assert [(r['name'],r['typeId'],r['nativeSize']) for r in manifest['modules']]==[
    ('SpawnBehaviorModuleData',227633887,92),('OCLUpdateModuleData',3098903621,28),('OCLMonitorUpdateModuleData',4047873639,40)]
out=['// Complete Spawn/OCL update contracts; original Tokenizer blob 6e40a9ed6c9f961d294d49768d911e6a6f354619.','// OCLMonitor names must come from the original PE table; no schema-derived ordering.','#include "GameObjectSpawnEnums.h"','const AIEnum spawnDamageTypeEnum = { aiDamageTypeNames, 39u, 38, 39u };']
fields=[('SpawnBehavior', [('SpawnNumberData',36,'Signed','0'),('SpawnReplaceDelayData',40,'Time','0s'),('InitialBurst',44,'Signed','0')]+[(n,o,'Boolean','false') for n,o in zip(['IsOneShotData','CanReclaimOrphans','AggregateHealth','ExitByBudding','SpawnedRequireSpawner','RespectCommandLimit'],range(76,82))]+[('DamageTypesToPropagateToSlaves',48,'Bits','ALL','spawnDamageTypeEnum'),('FadeInTime',56,'Time','0s')]+[(n,o,'Boolean','false') for n,o in [('KillSpawnBasedOnModelConditionState',82),('ShareUpgrades',83),('SpawnInsideBuilding',84)]]+[('InitialDelay',60,'Time','0s')]+[(n,o,'Boolean','false') for n,o in zip(['CombineOnCreate','KillSpawnsOnDisabled','PassExperienceToSpawned','KillSpawnsOnCaptured','SpawnOnUnpack','SetPurchasePrice'],range(85,91))]),('OCLUpdate',[('OCL',8,'Reference',None),('MinDelay',12,'Duration','0'),('MaxDelay',16,'Duration','0'),('CreateAtEdge',24,'Boolean','false'),('Amount',20,'Signed','-1')]),('OCLMonitorUpdate',[('Options',8,'Bits',None,'spawnMonitorOptionEnum'),('RequiredMasterObjectStatus',12,'ObjectStatus',None)])]
for index,(name,fs) in enumerate(fields):
    row=manifest['modules'][index]
    meta=next(m for m in row['methods'] if m['signature'].startswith('internal unsafe static void Marshaler_002EMarshal(INode*'))
    original=(evidence/meta['file']).read_text();literals=json.loads(original.splitlines()[1].removeprefix('// Literals: '))
    actual=[]
    for line in body_of(original,meta['signature']).splitlines():
        symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
    children=[('Die',None),('SpawnTemplate',None)] if index==0 else []
    assert actual==[(f[0],f[3]) for f in fs]+children,(name,actual)
for name,fs in fields:
 out+=['const ModuleField spawn'+name+'Fields[] = {']
 for f in fs:
  n,o,k,d,*e=f; fallback='nullptr' if d is None else '"'+d+'"'; enum='&'+e[0] if e else 'nullptr'
  out+=[f'    {{ "{n}", {o}u, ModuleValue::{k}, {fallback}, {enum} }},']
 out+=['};']
out+=['const ModuleFieldSet spawnFieldSets[] = {']+[f'    {{ spawn{n}Fields, {len(fs)}u }},' for n,fs in fields]+['};']
out += ['const ModuleToken spawnContainModuleDataTokens[] = {','    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },','};','const ModuleLayout spawnContainModuleData = { spawnContainModuleDataTokens, 1u };','const ModuleToken spawnUpdateModuleDataTokens[] = {','    { 1u, 687874333u, 0u, &spawnContainModuleData, 0u, 0u },','};','const ModuleLayout spawnUpdateModuleData = { spawnUpdateModuleDataTokens, 1u };']
for name,num in [('SpawnBehavior',5),('OCLUpdate',11),('OCLMonitorUpdate',17)]:
 text=(evidence/f'{num:03}.txt').read_text();text=text[text.index('internal unsafe'):]; chunks=text.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')
 out+=['const ModuleToken spawn'+name+'Tokens[] = {']; count=0
 for c in chunks:
  m=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)
  if not m or m[1]=='0':continue
  n,t=[int(v)&0xffffffff for v in m.groups()]; offset=0; nested='nullptr';mode=0;stride=0
  off=re.search(r'\(byte\*\)@object \+ (\d+)',c)
  if off:offset=int(off[1])
  if n==1:nested='&upgradeUpgradeModuleData' if name=='SpawnBehavior' else '&spawnUpdateModuleData'
  elif 'DieMuxDataType' in c:nested='&dieDieMuxDataType';mode=4
  elif 'List_003CTypedAssetId' in c:mode=3;stride=4
  out+=[f'    {{ {n}u, {t}u, {offset}u, {nested}, {mode}u, {stride}u }},'];count+=1
 assert count=={'SpawnBehavior':24,'OCLUpdate':6,'OCLMonitorUpdate':3}[name],name
 assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1]),name
 out+=['};',f'const ModuleLayout spawn{name} = {{ spawn{name}Tokens, {count}u }};']
out+=['const ModuleChoice spawnChoices[] = {','    { 227633887u, &spawnSpawnBehavior, 92u, 123u },','    { 3098903621u, &spawnOCLUpdate, 28u, 124u },','    { 4047873639u, &spawnOCLMonitorUpdate, 40u, 125u },','};','']
output='\n'.join(out);path=base/'recovery/src/Native/GameObjectSpawnUpdate.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectSpawnUpdate.h'
else:path.write_text(output)
print('PASS complete Spawn/OCL contracts and 61 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
