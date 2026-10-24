"""Complete effect/locomotor control group; exact pinned original methods and PE maps."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/object-control-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[
 [('ToppleFX',8,'Reference',None,None),('BounceFX',12,'Reference',None,None),('StumpId',16,'AssetId',None,None),('InitialVelocityPercent',20,'Percent','20%',None),('InitialAccelPercent',24,'Percent','1%',None),('BounceVelocityPercent',28,'Percent','20%',None),('MinimumToppleSpeed',32,'Float','0.5',None)]+[(n,o,'Boolean','false',None) for n,o in [('KillWhenToppled',36),('KillWhenStartToppled',37),('KillStumpWhenToppled',38),('ToppleLeftOrRightOnly',39),('ReorientToppledRubble',40)]],
 [('InitialDelay',8,'Time','0.5s',None),('LiftVelocity',12,'Float','1.0',None),('DeathHeight',16,'Float','420',None),('Radius',20,'Float','10.0',None),('Duration',24,'Time','10.0s',None)],
 [('TriggerFX',8,'Reference',None,None),('TargetFX',12,'Reference',None,None),('FadeInTime',16,'Time','0s',None),('InitialDelayTime',20,'Time','0s',None),('DelayTime',24,'Time','0s',None),('ScaleFadeInTimeByDistance',28,'Float','0.0',None),('GetObjectsCurrentlyAtStorePosition',56,'Boolean','false',None),('Telefrag',32,'Enum','NONE','TelefragType'),('SpecialPowerTemplate',36,'Reference',None,None),('Shader',40,'Reference',None,None),('DisabledTypesToProcess',44,'Bits','HELD','DisabledType')],
 [('UnificationType',20,'Enum','SAME_PLAYER','UnifiedControlType')],
 [('WeaponTemplate',8,'Reference',None,None),('RandomScatter3D',132,'Boolean','false',None)],
 [('LocomotorSet',8,'Enum',None,'LocomotorSetType'),('UpdatePeriod',12,'Time','1.0s',None),('RevealShroudToAllPlayersRadius',16,'Float','0',None),('MinimumHealth',20,'Percent','0%',None),('ObjectStatusWhileActive',24,'ObjectStatus',None,None),('ModelConditionWhileActive',52,'ModelCondition',None,None)],
 [('ConvergingAngle',44,'Angle',None,None),('AngleRotation',48,'Angle','0d',None),('Radius',52,'Float','10.0',None),('SweepFXList',56,'Reference',None,None),('VeteranSweepFXList',60,'Reference',None,None),('SweepFXTimeout',64,'Time','0s',None),('SweepWeapon',68,'Reference',None,None),('ModelConditions',72,'ModelCondition',None,None),('Lifetime',132,'Time','1s',None)],
 [('ScanTime',8,'Time','0.2s',None),('DelayBetweenVictimsTime',12,'Time','0.3s',None),('EffectDistance',16,'Float','125.0',None),('GatherAcceleration',20,'Float','0.01',None),('ModelLength',24,'Float','100.0',None),('ArmTemplate',28,'Reference',None,None),('VictimShader',32,'Reference',None,None),('HeightOffGround',36,'Float','50.0',None)],
 [('Enabled',36,'Boolean','false',None),('Size',8,'Enum','LIGHT','VehicleSize')],
 [('ScalableDrawModuleId',8,'AssetId',None,None),('ScaledBoneName',12,'String',None,None),('Radius',20,'Float','250.0',None),('MaximumPullVelocity',24,'Float','20.0',None),('MinimumPullVelocity',28,'Float','4.0',None),('VictimStatusToSet',32,'Enum','MAGNETIZED','ObjectStatusType')],
 [('Flags',8,'Bits',None,'SecondaryObjectListenerOption')],
 [('ThingTemplate',8,'Reference',None,None),('Count',12,'Unsigned',None,None),('BuildableDistance',16,'Float',None,None)],
 [('Radius',8,'Float','0',None),('ScanFrequency',12,'Time','1s',None)]
]
CHILDREN=[[],[('ObjectFilter',28,8,'nullptr')],[('InstaKillObjectFilter',48,7,'nullptr'),('CollidesWithOnTeleport',52,7,'nullptr')],[('UpgradeMap',24,1,'&controlUpgradeMap')],[('ScatterSettings',12,0,'&controlScatterDamage'),('Filter',20,8,'nullptr')],[],[],[('Filter',40,8,'nullptr'),('KillFilter',152,7,'nullptr'),('ParticleData',156,1,'&controlSqueezeFlingParticleDataType')],[('LeftOffset',12,0,'nullptr'),('RightOffset',24,0,'nullptr')],[('ObjectStatusValidation',36,4,'&weaponObjectStatusValidationDataType'),('ObjectFilter',40,8,'nullptr')],[('SelfValidation',12,4,'&weaponObjectStatusValidationDataType'),('ObjectEffectTuple',16,1,'&controlObjectEffectTuple')],[('ConnectionShadowInfo',20,4,'&controlShadowInfo')],[('ObjectFilter',16,8,'nullptr')]]
UPGRADE_MAP=[('OwnershipAmount',0,'Signed',None,None),('ProvidedUpgrade',4,'Reference',None,None)]
SCATTER=[('ScatterAmount',0,'Signed','0',None),('ScatterDelay',4,'Time','0s',None)]
PARTICLE=[('SourceBone',0,'String',None,None),('DestinationBone',8,'String',None,None),('ParticleSystem',16,'Reference',None,None)]
SHADOW=[('Type',0,'Enum',None,'ShadowType'),('Texture',4,'Reference',None,None),('AdditionalTexture',8,'Reference',None,None)]+[(n,o,'Float','0.0',None) for n,o in [('SizeX',12),('SizeY',16),('OffsetX',20),('OffsetY',24)]]+[('OpacityStart',28,'Float','0',None),('OpacityFadeInTime',32,'Time','0s',None),('OpacityPeak',36,'Float','1',None),('OpacityFadeOutTime',40,'Time','0s',None),('OpacityEnd',44,'Float','0',None),('MaxHeight',48,'Float','20',None)]+[(n,o,'Boolean',d,None) for n,o,d in [('OverrideLODVisibility',56,'false'),('UseHouseColor',57,'false'),('IsRotatingWithObject',58,'true'),('LocalPlayerOnly',59,'false')]]+[('SunAngle',52,'Float','0.0',None)]
VECTOR=[('x',0,'Float','0.0',None),('y',4,'Float','0.0',None),('z',8,'Float','0.0',None)]
PARENTS=['&visionCaptureBehaviorModuleData' if i==3 else '&beamLaserStateModuleData' if i==6 else '&lifecycleBehaviorModuleData' if i in [10,11] else '&spawnUpdateModuleData' for i in range(13)]

def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==166;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Generated complete control group; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('TelefragType',4,1,0),('UnifiedControlType',2,1,0),('VehicleSize',3,2,0),('SecondaryObjectListenerOption',2,1,2),('ShadowType',7,3,0),('LocomotorSetType',19,12,0),('DisabledType',12,0,12),('ObjectStatusType',223,198,0)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue control'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum control{name} = {{ control{name}Names, {count}u, {unknown}, {bits}u }};']
 def emit(n,fields,children,parent,index=-1):
  nonlocal lines
  ms=f'internal unsafe static void Marshaler_002EMarshal(INode* node, {n}* @object, Tracker* state)'
  if ms in bodies:mb,lits=bodies[ms]
  elif n=='Vector3':mb,lits=next(value for sig,value in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, Vector3* @object, Tracker* state,'))
  else:mb,lits=next(value for sig,value in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and ('_003A_003A'+n+'_002C') in sig)
  actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  expected=[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nested in children]
  if index==3:expected=[('UpgradeMap',None),('UnificationType','SAME_PLAYER')]
  assert actual==expected,(n,actual,expected)
  if parent:assert ('Marshaler_002EMarshal(node, ('+('CaptureBehaviorModuleData' if index==3 else 'LaserStateModuleData' if index==6 else 'ModuleData')+'*)') in mb
  if fields:lines.extend(['const ModuleField control'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&control'+e) for n,o,k,d,e in fields]+['};'])
  if n=='Vector3':return
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0;nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for name,o,k,d,e in fields:
    if off==o and k=='NullableReference':mode=2
   for name,o,cm,cn in children:
    if off==o:mode=cm;nested=cn;stride={'UpgradeMap':8,'ParticleData':20,'ObjectEffectTuple':120}.get(name,0)
   records.append((key,typ,off,nested,mode,stride))
  assert [r[2] for r in records]==([0] if parent else [])+[o for name,o,k,d,e in fields if not(index==3 and o==20)]+[o for name,o,cm,cn in children],(n,records)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken control'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout control{n} = {{ control{n}Tokens, {len(records)}u }};']
 emit('UpgradeMap',UPGRADE_MAP,[],None)
 emit('ScatterDamage',SCATTER,[],None)
 emit('SqueezeFlingParticleDataType',PARTICLE,[],None)
 emit('ShadowInfo',SHADOW,[],None)
 emit('Vector3',VECTOR,[],None)
 emit('ObjectEffectTuple',[],[('ObjectFilter',0,8,'nullptr'),('UpgradeSelf',112,3,'nullptr')],None)
 for i,meta in enumerate(m['modules']):emit(meta['name'],SETS[i],CHILDREN[i],PARENTS[i],i)
 lines+=['const ModuleFieldSet controlFieldSets[] = {']+[f'    {{ '+('control'+x['name']+'Fields' if f else 'nullptr')+f', {len(f)}u }},' for x,f in zip(m['modules'],SETS)]+['};','const ModuleChoice controlChoices[] = {']+[f'    {{ {x["typeId"]}u, &control{x["name"]}, {x["nativeSize"]}u, {252+i}u }},' for i,x in enumerate(m['modules'])]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603';m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(source,pe,n) for n in m['peEnums']}
 output=generate();p=ROOT/'recovery/src/Native/GameObjectObjectControl.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete control group'
 else:p.write_text(output)
 print('PASS complete thirteen-type object-control group with full Capture20/LaserState44, tuple120, shadow60, particle20 and UpgradeMap8; 166 original methods and thirteen PE maps')
