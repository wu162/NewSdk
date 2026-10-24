"""Complete OCL creation and Movement SpecialPower452 derivations."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/power-creation-source'
m=json.loads((E/'manifest.json').read_text());BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';assert m['sourceBlob']==BLOB
bodies={}
for item in m['methods']:
 text=(E/item['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==item['bodySha256'];bodies[item['signature']]=(body,json.loads(text.splitlines()[1][13:]))
e=json.loads((ROOT/'recovery/evidence/gameobject-modules/power-continuation-enums.json').read_text());assert e['sourceBlob']==BLOB and e['peSha256']=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
tables={k:e['tables'][k] for k in ['OCLCreateLocType','SPMovementType']}
if '--exported-enums' not in sys.argv:
 from GenerateUpgradeFields import enum_values
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==e['peSha256']
 for name,values in tables.items():assert enum_values(source,pe,name)==values
FIELDS=[[
 ('OCL',452,'Reference',None,None),('CreateLocation',456,'Enum',None,'powerCreationOCLCreateLocType'),('CreateLocationOffset',460,'Float',None,None),('MaxCreateCount',464,'Unsigned','0',None),('NumberToSpawn',468,'Unsigned','1',None),('DestinationOCL',472,'Reference',None,None),('RegisterObjectsWithSpecialAbilityUpdate',496,'Boolean','false',None)
 ],[
 ('MovementType',452,'Enum','NONE','powerCreationSPMovementType'),('Duration',456,'Time','0s',None),('ModelCondition',460,'NullableModelCondition',None,None),('ObjectStatus',464,'NullableObjectStatus',None,None),('LocomotorSet',468,'Enum','INVALID','aiUpdateLocomotorSetTypeEnum'),('BusyDurationOnExit',472,'Time','0s',None),('CollisionBounceHeight',476,'Float','20.0',None),('CollisionBounceDistance',480,'Float','50.0',None),('CollisionBounceSpeedScalar',484,'Float','0.5',None)
 ]]
lines=['// Complete OCL/Movement SpecialPower derivations; fixed original blob '+BLOB+'.']
for name,unknown,count in [('OCLCreateLocType',5,15),('SPMovementType',0,3)]:
 assert len(tables[name])==count
 lines+=['const EnumNameValue powerCreation'+name+'Names[] = {']+[f'    {{ {json.dumps(n)}, {v} }},' for n,v in sorted(tables[name].items())]+['};',f'const AIEnum powerCreation{name} = {{ powerCreation{name}Names, {count}u, {unknown}, 0u }};']
lines+=['const ModuleToken powerCreationOCLPairTokens[] = {','    { 2969981770u, 1122298321u, 0u, nullptr, 0u, 0u },','};','const ModuleLayout powerCreationOCLPair = { powerCreationOCLPairTokens, 1u };']
for i,(meta,fields) in enumerate(zip(m['modules'],FIELDS)):
 name=meta['name'];body,literals=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'];actual=[]
 for line in body.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 assert actual==[(n,d) for n,o,k,d,e in fields]+([('UpgradeOCL',None),('NearestSecondaryObjectFilter',None),('Upgrade',None)] if i==0 else []),(name,actual)
 assert 'Marshaler_002EMarshal(node, (SpecialPowerModuleData*)@object, state);' in body
 lines+=['const ModuleField powerCreationFields'+str(i)+'[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};']
 tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'];chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks[:-1]:
  key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff
  off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;mode=0;nested='nullptr';stride=0
  if key==1:nested='&specialPowerSpecialPowerModuleData'
  elif i==0 and off==476:mode=1;nested='&powerCreationOCLPair';stride=4
  elif i==0 and off==484:mode=7
  elif i==0 and off==488:mode=3;stride=4
  elif i==1 and off in [460,464]:mode=2
  records.append((key,typ,off,nested,mode,stride))
 expected=[0]+[f[1] for f in fields]+([476,484,488] if i==0 else [])
 assert [r[2] for r in records]==expected and records[0][1]==3472757011,(name,records)
 assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
 for word in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert word in chunks[-1]
 lines+=['const ModuleToken powerCreation'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout powerCreation{name} = {{ powerCreation{name}Tokens, {len(records)}u }};']
lines+=['const ModuleFieldSet powerCreationFieldSets[] = {']+[f'    {{ powerCreationFields{i}, {len(f)}u }},' for i,f in enumerate(FIELDS)]+['};','const ModuleChoice powerCreationChoices[] = {']+[f'    {{ {meta["typeId"]}u, &powerCreation{meta["name"]}, {meta["nativeSize"]}u, {198+i}u }},' for i,meta in enumerate(m['modules'])]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectPowerCreation.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectPowerCreation.h'
else:path.write_text(output)
print('PASS complete OCL500/Movement488 fields, lists, nullable masks, inheritance and Token layouts')
