"""Generate complete Tiberium contracts from 66 fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/tiberium-source'
SPECS=[('TiberiumGrowthModBehaviorModuleData',2923073644,60,147,[('GreenTiberiumMultiplier',52,'Float','1.0'),('BlueTiberiumMultiplier',56,'Float','1.0')],[]),('TiberiumCrystalBehaviorModuleData',254628242,32,148,[('NumBoxes',8,'Unsigned','0'),('ValuePerBox',12,'Unsigned','0'),('GrowthStages',16,'Unsigned','1'),('TimeBetweenGrowthStages',20,'Time','5s'),('GrowthTimePerStage',24,'Time','3s')],[('RadarColor',None)]),('TiberiumFieldBehaviorModuleData',1808399630,76,149,[('DamageUpdateFrequencySeconds',8,'Time','0.67s'),('HealUpdateFrequencySeconds',12,'Time','0.60s'),('Radius',16,'Float','0'),('DamageRadiusAdd',20,'Float','20'),('HealRadiusAdd',24,'Float','20'),('FieldResolution',28,'Signed','40'),('MaxFieldMoney',32,'Signed','0'),('StartingFieldMoney',36,'Signed','0'),('ThingToSpawn',40,'AssetId',None),('SpawnOffset',44,'Float','0.0'),('ClusterScaleFactor',48,'Float','1.0'),('ClusterPowerFactor',52,'Float','1.0'),('CrystalGrowthRate',56,'Signed','0'),('LifetimeFieldMoney',60,'Unsigned','0'),('KillWhenEmptyAndFinishedSpawning',72,'Boolean','false'),('AllowStartingFieldCrystalsToGrow',73,'Boolean','false')],[('DamageFilter',None),('HealFilter',None)])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==66
assert [(m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in SPECS]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
colorBody=next(b for sig,b in methods.items() if 'Marshaler_002EMarshal(INode* node, uint* @object, Tracker* tracker, ColorTag*' in sig);colorAttrs=[]
for line in colorBody.splitlines():
 symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
 if symbols:colorAttrs.append(tuple(literals[s] for s in symbols))
assert colorAttrs==[('R','0'),('G','0'),('B','0'),('A','255')]
lines=['// Complete Tiberium contracts; original blob '+BLOB+'.','// RadarColor uses unsigned parse -> byte truncation -> ARGB packed word; no color node leaves zero.','const char* tiberiumColorNames[] = { "R", "G", "B", "A" };','const char* tiberiumColorDefaults[] = { "0", "0", "0", "255" };']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'));actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField tiberium'+name+'Fields[] = {']
 for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, nullptr },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d))]
 lines+=['};']
lines+=['const ModuleFieldSet tiberiumFieldSets[] = {']+[f'    {{ tiberium{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=0
  if key==1:nested='&dieDieModuleData' if kind==147 else '&spawnUpdateModuleData'
  if kind==149 and off in [64,68]:mode=7
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==len(fields)+len(children)+1,(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken tiberium'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout tiberium{name} = {{ tiberium{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice tiberiumChoices[] = {']+[f'    {{ {t}u, &tiberium{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectTiberium.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectTiberium.h'
else:path.write_text(output)
print('PASS complete Tiberium contracts and 66 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
