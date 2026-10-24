"""Complete object replacement/disguise/upgrade and shield contracts from original methods."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/ability-finish-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[
 [('GiveUpgradeEffect',240,'Reference',None,None),('SpawnOutFX',244,'Reference',None,None),('FadeOutSpeed',248,'Float','0.025',None),('DeliverUpgrade',252,'Boolean','false',None)],
 [('ReplaceRadius',240,'Float',None,None),('ReplaceFX',244,'Reference',None,None),('Scatter',256,'Boolean',None,None)],
 [('RadiusMax',8,'Float','40',None),('RadiusMin',12,'Float','10',None),('DamageTypesNotToAbsorb',16,'Bits',None,'abilityFinishDamageType'),('ScanFrequency',24,'Time','1s',None),('Duration',28,'Time','10s',None),('MaxDamage',32,'Float','0',None),('ObjectStatus',36,'ObjectStatus','',None),('ModelCondition',64,'ModelCondition','',None),('InitiallyActive',260,'Boolean','false',None),('ShieldBoneName',124,'String',None,None),('ShieldSizeMultiplier',132,'Float','1.0',None),('Options',136,'Bits',None,'abilityFinishShieldSphereUpdateOption')],
 [('TriggerInstantlyOnCreate',260,'Boolean','false',None),('OpacityTarget',240,'Float','0',None),('DisguiseFX',244,'Reference',None,None),('ScanRadiusForPropDisguise',248,'Float','0.0',None)]
]
TERRAIN=[('TerrainClass',0,'Enum',None,'abilityFinishTerrainClassType'),('ObjectTemplate',4,'AssetId',None,None)]
def generate():
 manifest=json.loads((E/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==51
 bodies={}
 for m in manifest['methods']:
  text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'];bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
 def method(op,name):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{name}* @object, Tracker* state)']
 lines=['// Generated complete control/shield group; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('ShieldSphereUpdateOption',1,0,1),('TerrainClassType',11,3,0),('DamageType',39,38,39)]:
  values=manifest['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue abilityFinish'+name+'Names[] = {']+['    { '+json.dumps(n)+', '+str(v)+' },' for n,v in sorted(values.items())]+['};',f'const AIEnum abilityFinish{name} = {{ abilityFinish{name}Names, {count}u, {unknown}, {bits}u }};']
 def emit(name,fields,children=[],index=-1):
  mb,lits=method('Marshaler_002EMarshal',name);actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+children,(name,actual)
  if fields:lines.extend(['const ModuleField abilityFinish'+name+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};'])
  tb,_=method('Tokenizer_002ETokenize',name);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for chunk in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff;off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=0;stride=0
   if key==1:nested='&spawnUpdateModuleData' if index==2 else '&abilitySpecialAbilityUpdateModuleData'
   if index==1 and off==248:nested='&abilityFinishReplaceObjectNugget';mode=1;stride=12
   if index==3 and off==252:nested='&abilityFinishSpecialDisguiseTerrainData';mode=1;stride=8
   if index==2:mode={140:2,144:8,256:7}.get(off,0)
   if name=='ReplaceObjectNugget':mode=7 if off==0 else 3
   records.append((key,typ,off,nested,mode,stride))
  expected=[o for n,o,k,d,e in fields]
  if index>=0:expected=[0]+expected+({1:[248],2:[140,144,256],3:[252]}.get(index,[]))
  if name=='ReplaceObjectNugget':expected=[0,4]
  assert [r[2] for r in records]==expected,(name,records,expected);assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines.extend(['const ModuleToken abilityFinish'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityFinish{name} = {{ abilityFinish{name}Tokens, {len(records)}u }};'])
 emit('SpecialDisguiseTerrainData',TERRAIN)
 emit('ReplaceObjectNugget',[],[('TargetObjectFilter',None),('ReplacementObject',None)])
 for i,(meta,fields) in enumerate(zip(manifest['modules'],SETS)):
  emit(meta['name'],fields,{1:[('ReplaceObjectNugget',None)],2:[('PositionOffset',None),('ObjectFilter',None),('IgnoreInsideToInsideCheck',None)],3:[('TerrainData',None)]}.get(i,[]),i)
  mb,_=method('Marshaler_002EMarshal',meta['name']);assert 'Marshaler_002EMarshal(node, ('+('ModuleData' if i==2 else 'SpecialAbilityUpdateModuleData')+'*)@object, state);' in mb
 lines+=['const ModuleFieldSet abilityFinishFieldSets[] = {']+[f'    {{ abilityFinish{meta["name"]}Fields, {len(fields)}u }},' for meta,fields in zip(manifest['modules'],SETS)]+['};','const ModuleChoice abilityFinishChoices[] = {']+[f'    {{ {meta["typeId"]}u, &abilityFinish{meta["name"]}, {meta["nativeSize"]}u, {216+i}u }},' for i,meta in enumerate(manifest['modules'])]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 from GenerateUpgradeFields import enum_values
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
 manifest=json.loads((E/'manifest.json').read_text());assert manifest['peEnums']=={name:enum_values(source,pe,name) for name in manifest['peEnums']}
 output=generate();path=ROOT/'recovery/src/Native/GameObjectAbilityFinish.h'
 if '--check' in sys.argv:assert path.read_text()==output,'Regenerate control/shield group'
 else:path.write_text(output)
 print('PASS four complete control/shield contracts and replacement12/terrain8 dependencies; 51 original methods and three original PE maps')
