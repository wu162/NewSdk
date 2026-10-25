"""Complete Contain core group; exact pinned original methods and PE maps."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/contain-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
NAMES=['OpenContainModuleData','GarrisonContainModuleData','HordeGarrisonContainModuleData','ContestableGarrisonContainModuleData','TunnelContainModuleData','TunnelTeleportContainModuleData','HealContainModuleData','SlaughterHordeContainModuleData']
BASE=[('ContainMax',8,'Unsigned','0',None),('EnterSound',12,'Reference',None,None),('ExitSound',16,'Reference',None,None),('DamagePercentToUnits',20,'Percent','0',None),('PassengersTestCollisionHeight',24,'Float','-1000',None),('PassengersInTurret',136,'Boolean','false',None),('NumberOfExitPaths',28,'Signed','1',None),('DoorOpenTime',32,'Unsigned','1',None)]+[(n,o,'Boolean',d,None) for n,o,d in [('AllowOwnPlayerInsideOverride',137,'false'),('AllowAlliesInside',138,'true'),('AllowEnemiesInside',139,'true'),('AllowNeutralInside',140,'true'),('PassDisabilityToRiders',141,'false')]]+[('IgnoreDisabledBitsForRiders',36,'Bits','UNDERPOWERED EMP','DisabledType')]+[(n,o,'Boolean',d,None) for n,o,d in [('ShowPips',142,'true'),('CollidePickup',143,'true'),('EjectPassengersOnDeath',144,'true'),('KillPassengersOnDeath',145,'false'),('HasObjectStatusOfContainedEntry',146,'true'),('Enabled',147,'true')]]+[('ObjectStatusOfContained',40,'ObjectStatus',None,None),('ObjectStatusWhileContaining',68,'ObjectStatus','CONTAINER_OCCUPIED',None),('ModifierRequiredTime',96,'Unsigned','100',None),('KillIfEmptyTime',100,'Time','0s',None)]
GARRISON=[('MobileGarrison',156,'Boolean',None,None),('ImmuneToClearBuildingAttacks',157,'Boolean',None,None),('ResetInitialTeamOnCapture',158,'Boolean','false',None)]
HORDE_GARRISON=[('ExitDelay',160,'Unsigned','0',None)]
CONTEST=[('RequiredClearingObjectStatus',160,'ObjectStatus','',None),('ForbiddenContainerObjectStatus',188,'ObjectStatus','UNDER_IRON_CURTAIN',None),('EjectSpeed',216,'Float','1.0',None)]
TUNNEL=[('DeleteRemoved',180,'Boolean','false',None),('TunnelMasterObject',176,'AssetId',None,None)]
HEAL=[('TimeForFullHeal',176,'Time',None,None)]
SLAUGHTER=[('CashBackPercent',176,'Percent',None,None),('CanAlwaysEnterStatus',180,'ObjectStatus',None,None),('SlaughterFX',208,'Reference',None,None)]
PASSENGER=[('BonePrefix',0,'String',None,None),('MaxPassengers',8,'Signed','0',None),('SlingUnderBone',124,'Boolean','false',None)]
MEMBER=[('ThingTemplate',0,'AssetId',None,None),('ObjectStatus',4,'Enum',None,'ObjectStatusType')]
ROSTER=[('TemplateId',0,'AssetId',None,None),('Count',4,'Signed',None,None)]
SETS=[BASE,GARRISON,HORDE_GARRISON,CONTEST,TUNNEL,[],HEAL,SLAUGHTER]
CHILDREN=[[('PassengerFilter',104,7,'nullptr'),('DieMuxData',108,4,'&dieDieMuxDataType'),('PassengerData',112,1,'&containPassengerDataType'),('ModifierToGiveOnExit',120,3,'nullptr'),('MemberTemplateStatusInfo',128,1,'&containMemberTemplateStatusData')],[('InitialRoster',148,0,'&containInitialRoster')],[('EntryOffset',164,2,'nullptr'),('EntryPosition',168,2,'nullptr'),('ExitOffset',172,2,'nullptr')],[],[],[],[],[('CanAlwaysEnterObjectFilter',212,8,'nullptr')]]
PARENTS=['&spawnUpdateModuleData','&containOpenContainModuleData','&containGarrisonContainModuleData','&containGarrisonContainModuleData','&containHordeGarrisonContainModuleData','&containTunnelContainModuleData','&containHordeGarrisonContainModuleData','&containHordeGarrisonContainModuleData']

def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==197;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Generated complete contain group; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('DisabledType',12,0,12),('ObjectStatusType',223,198,0)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue contain'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum contain{name} = {{ contain{name}Names, {count}u, {unknown}, {bits}u }};']
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
  assert actual==expected,(n,actual,expected)
  if parent:assert ('Marshaler_002EMarshal(node, ('+('ModuleData' if index==0 else 'OpenContainModuleData' if index==1 else 'GarrisonContainModuleData' if index in [2,3] else 'HordeGarrisonContainModuleData')+'*)') in mb
  if fields:lines.extend(['const ModuleField contain'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&contain'+e) for n,o,k,d,e in fields]+['};'])
  if n=='Vector3':return
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);scalar=re.search(r'\((?:float|int|\w+Type)\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 4*int(scalar[1]) if scalar else 0;nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for name,o,k,d,e in fields:
    if off==o and k=='NullableReference':mode=2
   for name,o,cm,cn in children:
    if off==o:mode=cm;nested=cn;stride={'PassengerData':128,'MemberTemplateStatusInfo':8}.get(name,0)
   records.append((key,typ,off,nested,mode,stride))
  assert [r[2] for r in records]==([0] if parent else [])+[o for name,o,k,d,e in fields]+[o for name,o,cm,cn in children],(n,records)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken contain'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout contain{n} = {{ contain{n}Tokens, {len(records)}u }};']
 emit('InitialRoster',ROSTER,[],None)
 emit('PassengerDataType',PASSENGER,[('Filter',12,8,'nullptr')],None)
 emit('MemberTemplateStatusData',MEMBER,[],None)
 modules=[next(x for x in m['modules'] if x['name']==n) for n in NAMES]
 for i,meta in enumerate(modules):
  if i==5:
   tb=next(v[0] for k,v in bodies.items() if k.startswith('internal unsafe static void Tokenizer_002ETokenizePolymorphicType_') and 'TunnelTeleportContain' in k)
   assert '12u, 2u' in tb and '((int*)ptr2)[1] = -1266409813;' in tb
   lines+=['const ModuleToken containTunnelTeleportContainModuleDataTokens[] = { { 1u, 3028557483u, 0u, &containTunnelContainModuleData, 0u, 0u } };','const ModuleLayout containTunnelTeleportContainModuleData = { containTunnelTeleportContainModuleDataTokens, 1u };']
  else:emit(meta['name'],SETS[i],CHILDREN[i],PARENTS[i],i)
 lines+=['const int containParents[] = { -1,0,1,1,2,4,2,2 };','const ModuleFieldSet containFieldSets[] = {']+[f'    {{ '+('contain'+x['name']+'Fields' if f else 'nullptr')+f', {len(f)}u }},' for x,f in zip(modules,SETS)]+['};','const ModuleChoice containChoices[] = {']+[f'    {{ {x["typeId"]}u, &contain{x["name"]}, {x["nativeSize"]}u, {270+i}u }},' for i,x in enumerate(modules)]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603';m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(source,pe,n) for n in m['peEnums']}
 output=generate();p=ROOT/'recovery/src/Native/GameObjectContainCore.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete contain group'
 else:p.write_text(output)
 print('PASS complete eight-type Contain core with full Open148/Garrison160/HordeGarrison176, passenger128, roster8 and member8; 197 original methods and eleven PE maps')
