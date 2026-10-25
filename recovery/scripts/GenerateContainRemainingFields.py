"""Complete remaining Contain contracts, pinned to all 197 original method hashes."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
E=ROOT/'recovery/evidence/gameobject-modules/contain-source'
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
def f(n,o,k,d=None,e=None):return (n,o,k,d,e)
ENGINEER=[f('ImmediatelyEnabled',84,'Boolean','false'),f('ReplaceWith',8,'Reference'),f('NameOfVoiceToUseForFriendlyEnter',12,'NullableStringHash'),f('NameOfVoiceToUseForHostileEnter',16,'NullableStringHash'),f('ObjectStatusToSet',20,'ObjectStatus','NO_REFUND')]+[f(n,48+4*i,'NullableReference') for i,n in enumerate(['FXForRepair','FXForCapture','FXForReplace','FXForCaptureAndReplace'])]+[f(n,64+4*i,'Reference','') for i,n in enumerate(['EvaEventForRepair','EvaEventForCapture','EvaEventForReplace','EvaEventForCaptureAndReplace'])]
INFILTRATOR=[f('BlockedDuration',8,'Time','2s'),f('InfiltratedStatusFilter',12,'ObjectStatus','DESTROYED UNDER_CONSTRUCTION SOLD UNPACKING'),f('CanEnterFilter',40,'Reference','InfiltrationCanEnterObjectFilter'),f('ImmediatelyEnabled',124,'Boolean','false'),f('ReplaceWith',44,'Reference'),f('NameOfVoiceToUseForHostileEnter',48,'StringHash','VoiceInfiltrate'),f('NameOfVoiceToUseForInfiltrationDeath',52,'StringHash','VoiceInfiltrateDeath'),f('EvaEventForInfiltratingPlayer',56,'Reference','EnemyBuildingInfiltrated'),f('EvaEventForInfiltratedPlayer',60,'Reference','OurBuildingInfiltrated'),f('FXForInfiltrate',64,'Reference','FX_Building_Infiltrated_Generic'),f('Duration',68,'Time','0s'),f('UnitDuration',72,'Time','0s'),f('Amount',76,'Float','0.0'),f('Effect',80,'Enum',None,'InfiltratorEffectType'),f('ObjectRef',84,'AssetId'),f('UnitFilter',88,'NullableReference'),f('StructureFilter',92,'NullableReference'),f('ObjectStatusToSet',96,'ObjectStatus','NO_REFUND')]
TRANSPORT=[f('ScatterNearbyOnExit',320,'Boolean','true'),f('OrientLikeContainerOnExit',321,'Boolean','false'),f('DestroyRidersWhoAreNotFreeToExit',322,'Boolean','false'),f('ExitBone',148,'String'),f('HealthRegenPercentPerSec',156,'Float','0'),f('ExitDelay',160,'Duration','0')]+[f(n,164+36*i,'Bits',None,'KindOfType') for i,n in enumerate(['TypeOneForWeaponState','TypeTwoForWeaponState','TypeThreeForWeaponState'])]+[f('ForceOrientationContainer',323,'Boolean','true'),f('ConditionForEntry',272,'Enum',None,'ModelConditionFlagType'),f('ShouldThrowOutPassengers',324,'Boolean','false'),f('ThrowOutPassengersDelay',276,'Duration','0'),f('ThrowOutPassengersLandingWarhead',280,'Reference'),f('FadePassengerOnEnter',325,'Boolean','false'),f('FadePassengerOnExit',326,'Boolean','false'),f('EnterFadeTime',284,'Float','0'),f('ExitFadeTime',288,'Float','0'),f('FadeReverse',327,'Boolean','false'),f('ReleaseSnappyness',292,'Float','0.7'),f('ExtendedExitContainerChecks',328,'Boolean','false')]
HORDE=[f('Formation',332,'Enum',None,'FormationType'),f('AlternateFormation',336,'AssetId'),f('ForcedLocomotorSet',340,'Enum',None,'LocomotorSetType'),f('UseSlowHordeMovement',496,'Boolean','true'),f('MeleeAttackLeashDistance',344,'Float','60.0'),f('GeometryFrontAngleRadians',348,'Float','0.0'),f('EvaEventLastMemberDeath',352,'Reference',''),f('FrontAngle',356,'Float','6.283'),f('FlankedDelaySeconds',360,'Float','0'),f('FlankedDurationSeconds',364,'Float','5.0'),f('MinimumHordeSize',368,'Unsigned','0'),f('BackupMinDelayTime',372,'Time','0s'),f('BackupMaxDelayTime',376,'Time','0s'),f('BackupMinDistance',380,'Float','0'),f('BackupMaxDistance',384,'Float','0'),f('BackupPercentage',388,'Float','0'),f('RadiusCowerOverride',392,'Float','0'),f('VisionOverrideRear',396,'Float','0.0'),f('VisionOverrideSide',400,'Float','0.0'),f('SpawnBannerCarrierImmediately',497,'Boolean','false'),f('BannerCarrierByUpgradeOnly',498,'Boolean','false'),f('ForbiddenCoverStatus',404,'ObjectStatus')]
NAMES=['EngineerContainModuleData','InfiltratorContainModuleData','TransportContainModuleData','HordeTransportContainModuleData','HordeContainModuleData']
FIELDS=[ENGINEER,INFILTRATOR,TRANSPORT,[f('FlyOffMapOnEmpty',332,'Boolean','false')],HORDE]
# child name, byte offset, Token mode, nested layout, structure stride
CHILDREN=[[('CanEnterFilter',80,7,'nullptr',0)],[],[('InitialPayload',296,1,'&containRestInitialPayload',8),('ThrowOutPassengersVelocity',304,2,'nullptr',0),('UpgradeCreationTrigger',308,1,'&containRestUpgradeCreation',12),('FadeFilter',316,7,'nullptr',0)],[],[('RandomOffset',432,2,'nullptr',0),('RankInfo',436,1,'&containRestRankInfoType',16),('RankThatStopsAdvance',444,3,'nullptr',0),('RankToReleaseWhenAttacking',452,3,'nullptr',0),('LeaderPosition',460,2,'nullptr',0),('BannerCarrierPosition',464,1,'&containRestBannerCarrierPosType',12),('BannerCarriersAllowed',472,3,'nullptr',0),('LeadersAllowed',480,3,'nullptr',0),('AttributeModifier',488,3,'nullptr',0)]]
NESTED=[('InitialPayload',[f('Name',0,'AssetId'),f('Count',4,'Signed')],[]),('UpgradeCreation',[f('Upgrade',0,'Reference'),f('Template',4,'AssetId'),f('Num',8,'Unsigned')],[]),('PositionAndLeaderType',[f('X',0,'Float','0'),f('Y',4,'Float','0'),f('LeaderRank',8,'Signed','-1'),f('LeaderIndex',12,'Signed','0')],[]),('RankInfoType',[f('RankID',0,'Signed','0'),f('UnitType',4,'AssetId')],[('Position',8,1,'&containRestPositionAndLeaderType',16)]),('BannerCarrierPosType',[],[('UnitType',0,0,'nullptr',0),('Pos',4,0,'nullptr',0)])]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==197
 bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):
  return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void '+op+'(') and (' '+n+'* @object,') in k)
 lines=['// Generated complete remaining Contain contracts; original blob '+BLOB+'.']
 for n,unknown,bits in [('InfiltratorEffectType',1,0),('FormationType',0,0),('LocomotorSetType',12,0),('ModelConditionFlagType',233,0),('KindOfType',149,284)]:
  values=m['peEnums'][n]
  if n=='KindOfType':
   kb,_=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020KindOfType_002C284_003E('));assert 'num5 = 149;' in kb
  if not bits:
   eb,_=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020'+n+'_002Cclass_0020Relo_003A_003ATracker_003E('))
   assert ('? '+str(unknown)+' :' in eb) or (': '+str(unknown)+');' in eb),(n,unknown)
  lines+=['const EnumNameValue containRest'+n+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum containRest{n} = {{ containRest{n}Names, {len(values)}u, {unknown}, {bits}u }};']
 recordsByName={}
 def emit(n,fields,children,parent=None):
  nonlocal lines
  if n=='HordeTransportContainModuleData':mb,lits=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and '_003A_003A'+n+'_002C' in k)
  else:mb,lits=method('Marshaler_002EMarshal',n)
  actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nest,stride in children],(n,actual)
  if n in NAMES[:2]:assert 'Marshaler_002EMarshal(node, (ModuleData*)@object, state);' in mb
  if fields:lines+=['const ModuleField containRest'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&containRest'+e) for n,o,k,d,e in fields]+['};']
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
   o=re.search(r'\(byte\*\)@object \+ (\d+)',c);scalar=re.search(r'\((?:float|int|\w+Type)\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 4*int(scalar[1]) if scalar else 0
   nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for name,o,k,d,e in fields:
    if off==o and k in ['NullableReference','NullableStringHash']:mode=2
   for name,o,cm,cn,cs in children:
    if off==o:mode=cm;nested=cn;stride=cs
   records.append((key,typ,off,nested,mode,stride))
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert [r[2] for r in records]==([0] if parent else [])+[o for _,o,_,_,_ in fields]+[o for _,o,_,_,_ in children],(n,records)
  assert all(s in chunks[-1] for s in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  recordsByName[n]=records
  lines+=['const ModuleToken containRest'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout containRest{n} = {{ containRest{n}Tokens, {len(records)}u }};']
 for n,fs,cs in NESTED:emit(n,fs,cs)
 parents=['&spawnContainModuleData','&spawnContainModuleData','&containOpenContainModuleData','&containRestTransportContainModuleData','&containRestTransportContainModuleData']
 for n,fs,cs,p in zip(NAMES,FIELDS,CHILDREN,parents):emit(n,fs,cs,p)
 lines+=['const ModuleFieldSet containRestFieldSets[] = {']+[f'    {{ containRest{n}Fields, {len(fs)}u }},' for n,fs in zip(NAMES,FIELDS)]+['};','const ModuleChoice containRestChoices[] = {']
 for i,n in enumerate(NAMES):
  meta=next(x for x in m['modules'] if x['name']==n);lines+=[f'    {{ {meta["typeId"]}u, &containRest{n}, {meta["nativeSize"]}u, {278+i}u }},']
 lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
 out=generate();p=ROOT/'recovery/src/Native/GameObjectContainRemaining.h'
 if '--check' in sys.argv:assert p.read_text()==out,'Regenerate remaining Contain contracts'
 else:p.write_text(out)
 print('PASS complete five-type remaining Contain original contracts; 197 method hashes')
