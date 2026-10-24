"""Complete effect/locomotor support group; exact pinned original methods and PE maps."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/support-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[
 [('KillLocomotorUpgrade',36,'Boolean','false',None)],
 [('Type',8,'Enum','INVALID','SetLocomotorSpeedUpdateType')]+[(n,o,'Float','0.0',None) for n,o in [('LocomotorSpeedToSet',12),('LocomotorDeltaPerUpdate',16),('OtherScaredScanRangeMultiplier',20),('ObjectFilterRange',24)]]+[('UpdatePeriod',28,'Time','1.0s',None),('VisionArc',32,'Angle','180d',None)],
 [('RestrictionType',8,'Enum',None,'SpecialPowerRestrictionType'),('DependentObject',12,'AssetId',None,None),('ConsiderSpecialPowerRadius',24,'Boolean','true',None)],
 [('EffectRadius',8,'Float','0',None),('MaxBeneficiaries',12,'Unsigned','0',None),('HealingPercentPerSecond',16,'Percent','0',None),('HealingPointsPerSecond',20,'Float','0',None),('SurveyDelayFrames',24,'Unsigned','20',None),('UpgradeRequired',28,'Reference',None,None),('NonStackable',68,'Boolean','false',None),('AntiCategoryMask',32,'Bits',None,'AttributeModifierCategoryType'),('AntiFX',36,'Reference',None,None),('HealFX',40,'Reference',None,None)]+[(n,o,'Boolean','false',None) for n,o in [('AffectAttached',69),('AffectWoundedOnly',70),('AffectUnderAttack',71),('AffectContained',72)]]+[('DisabledTypesToProcess',44,'Bits',None,'DisabledType')],
 [('AttributeModifierName',36,'Reference',None,None),('RefreshDelay',40,'Time',None,None),('Range',44,'Float',None,None),('AllowPowerWhenAttacking',96,'Boolean','true',None),('InitiallyActive',97,'Boolean','false',None),('RequiredConditions',48,'Bits',None,'AttributeModifierAuraUpdateRequired'),('AntiCategory',52,'Bits',None,'AttributeModifierCategoryType'),('AntiFX',56,'Reference',None,None)]+[(n,o,'Boolean',None,None) for n,o in [('RunWhileDead',98),('AllowSelf',99),('AffectContainedOnly',100),('AffectHordeMembersOnly',101),('AffectAttacheeOnly',102)]]+[('MaxActiveRank',60,'Signed',None,None),('RequiredObjectStatusFlags',64,'ObjectStatus',None,None),('ShowDebugCircle',103,'Boolean','false',None)],
 [('MaxScanRange',8,'Float','1',None),('MinScanRange',12,'Float','0',None),('Radius',16,'Float','0',None),('IdleSeconds',20,'Time','0s',None),('SpecialAbilityName',24,'Reference',None,None),('ForbiddenStatus',28,'ObjectStatus',None,None)]+[(n,o,'Boolean','false',None) for n,o in [('StartsActive',64),('UseStartPosition',65),('AdjustAttackPos',66),('AllowSelf',67)]],
 [('TauntAndPointDistance',8,'Float','0.0',None),('HeroScanDistance',12,'Float','0.0',None),('TauntAndPointUpdateDelay',16,'Time','2s',None),('FearScanDistance',20,'Float','0.0',None),('IgnoreVeterancy',52,'Boolean','false',None),('ImmuneToFearLevel',24,'Signed','5',None)],
 []]
CHILDREN=[[],[('ObjectFilter',36,7,'nullptr')],[('DependentObjectRadius',16,3,'nullptr')],[('Modifier',48,3,'nullptr'),('AllowFilter',56,7,'nullptr'),('ContainerFilter',60,7,'nullptr'),('ObjectStatusValidation',64,4,'&weaponObjectStatusValidationDataType')],[('ObjectFilter',92,7,'nullptr')],[('Queries',56,1,'&supportQuery')],[('TauntAndPointExcluded',28,7,'nullptr'),('AfraidOf',32,7,'nullptr'),('AlwaysAfraidOf',36,7,'nullptr'),('PointAt',40,7,'nullptr'),('AddEmotion',44,3,'nullptr')],[('Event',52,1,'&supportFXListBehaviorEvent')]]
QUERY=[('EMax',0,'Signed','6',None),('Value',4,'Signed','-1',None)]
EVENT=[('Index',0,'Enum',None,'FXListBehaviorType'),('FX',4,'Reference',None,None),('MinThreshold',8,'Float','0',None),('MaxThreshold',12,'Float','0',None),('OrientToObject',20,'Boolean','true',None),('ForceUseDoFXObj',21,'Boolean','false',None),('Direction',16,'Enum','Default','FXListBehaviorDirection')]
PARENTS=['&upgradeUpgradeModuleData','&spawnUpdateModuleData','&lifecycleBehaviorModuleData','&spawnUpdateModuleData','&upgradeUpgradeModuleData','&spawnUpdateModuleData','&spawnUpdateModuleData','&dieDieModuleData']

def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==140;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Generated complete support group; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('SetLocomotorSpeedUpdateType',2,0,0),('SpecialPowerRestrictionType',3,1,0),('AttributeModifierCategoryType',15,7,15),('AttributeModifierAuraUpdateRequired',1,0,1),('DisabledType',12,0,12),('FXListBehaviorType',14,7,0),('FXListBehaviorDirection',5,4,0)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue support'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum support{name} = {{ support{name}Names, {count}u, {unknown}, {bits}u }};']
 def emit(n,fields,children,parent,index=-1):
  nonlocal lines
  ms=f'internal unsafe static void Marshaler_002EMarshal(INode* node, {n}* @object, Tracker* state)'
  if ms in bodies:mb,lits=bodies[ms]
  else:mb,lits=next(value for sig,value in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and ('_003A_003A'+n+'_002C') in sig)
  actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nested in children],(n,actual)
  if parent:assert ('Marshaler_002EMarshal(node, ('+('UpgradeModuleData' if index in [0,4] else 'DieModuleData' if index==7 else 'ModuleData')+'*)') in mb
  if fields:lines.extend(['const ModuleField support'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&support'+e) for n,o,k,d,e in fields]+['};'])
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0;nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for name,o,k,d,e in fields:
    if off==o and k=='NullableReference':mode=2
   for name,o,cm,cn in children:
    if off==o:mode=cm;nested=cn;stride=12 if name=='Queries' else 24 if name=='Event' else 0
   records.append((key,typ,off,nested,mode,stride))
  assert [r[2] for r in records]==([0] if parent else [])+[o for name,o,k,d,e in fields]+[o for name,o,cm,cn in children],(n,records)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken support'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout support{n} = {{ support{n}Tokens, {len(records)}u }};']
 emit('Query',QUERY,[('ObjectFilter',8,7,'nullptr')],None)
 emit('FXListBehaviorEvent',EVENT,[],None)
 for i,meta in enumerate(m['modules']):emit(meta['name'],SETS[i],CHILDREN[i],PARENTS[i],i)
 lines+=['const ModuleFieldSet supportFieldSets[] = {']+[f'    {{ '+('support'+x['name']+'Fields' if f else 'nullptr')+f', {len(f)}u }},' for x,f in zip(m['modules'],SETS)]+['};','const ModuleChoice supportChoices[] = {']+[f'    {{ {x["typeId"]}u, &support{x["name"]}, {x["nativeSize"]}u, {244+i}u }},' for i,x in enumerate(m['modules'])]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603';m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(source,pe,n) for n in m['peEnums']}
 output=generate();p=ROOT/'recovery/src/Native/GameObjectSupport.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete support group'
 else:p.write_text(output)
 print('PASS complete eight-type support group with full Upgrade36/Die52, Query12/Event24 and filters; 140 original methods and fourteen PE maps')
