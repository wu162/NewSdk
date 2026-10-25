"""Complete SlowDeath and destruction contracts, including GeometryInfo48 and original phase lists."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/slow-death-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
BASE=[('SinkRate',8,'FrameSpeed','0',None),('ProbabilityModifier',12,'Signed','10',None),('ModifierBonusPerOverkillPercent',16,'Percent','0',None)]+[(n,o,'Time','0s',None) for n,o in [('SinkDelay',20),('SinkDelayVariance',24),('DestructionDelay',28),('DestructionDelayVariance',32),('DecayBeginTime',36)]]+[('FlingForce',40,'Float','0',None),('FlingForceVariance',44,'Float','0',None),('FlingPitch',48,'Angle',None,None),('FlingPitchVariance',52,'Angle',None,None),('DeathFlags',56,'NullableModelCondition',None,None),('ShadowWhenDead',112,'Boolean','false',None),('Fade',113,'Boolean','false',None),('FadeTime',60,'Time','0s',None),('FadeDelay',64,'Time','0s',None),('DeathTypes',68,'NullableModelCondition',None,None),('DeathObjectStatusBits',72,'NullableObjectStatus',None,None)]
CLEAR=[('ClearanceMaxHeight',116,'Float','0',None),('ClearanceMaxHeightFraction',120,'Float','0',None),('ClearanceMinHeight',124,'Float','0',None),('ClearanceMinHeightFraction',128,'Float','0',None)]
HELI=[(n,o,'Float',d,None) for n,o,d in [('SpiralOrbitTurnRate',116,None),('SpiralOrbitForwardSpeed',120,None),('SpiralOrbitForwardSpeedDamping',124,'1.0'),('MinSelfSpin',128,None),('MaxSelfSpin',132,None)]]+[('SelfSpinUpdateDelay',136,'Time',None,None),('SelfSpinUpdateAmount',140,'Float',None,None),('FallHowFast',144,'Float',None,None),('MinBladeFlyOffDelay',148,'Time',None,None),('MaxBladeFlyOffDelay',152,'Time',None,None),('BladeBone',156,'String',None,None)]+[(n,o,'Reference',None,None) for n,o in [('OclEjectPilot',164),('FxBlade',168),('OclBlade',172),('FxHitGround',176),('OclHitGround',180),('FxFinalBlowUp',184),('OclFinalBlowUp',188)]]+[('DelayFromGroundToFinalDeath',192,'Float',None,None),('FinalRubbleObject',196,'AssetId',None,None),('MaxBraking',200,'Float','99999.9',None),('DeathSound',204,'Reference',None,None)]
BUILD=[('AssociatedModule',52,'AssetId',None,None),('MaxDebrisCloudsPerSecond',56,'Float','2.0',None),('FillerMesh',60,'AssetId','NEWSKIN_FILL',None)]
ITEM=[('BonePrefix',0,'String',None,None),('FX',8,'Reference',None,None),('CreationObject',12,'AssetId',None,None),('FlingVelocity',16,'Float','1.0',None)]
GEOMETRY=[('IsSmall',44,'Boolean','false',None)]+[(n,o,'Float','0',None) for n,o in [('BoundingCircleRadius',0),('BoundingSphereRadius',4),('XWidth',8),('YDepth',12)]]
SHAPE=[('Type',0,'Enum',None,'GeometryType'),('Height',4,'Float','0',None),('MajorRadius',8,'Float','0',None),('MinorRadius',12,'Float','0',None),('Name',16,'String',None,None),('BActive',28,'Boolean','false',None)]
VECTOR2=[('x',0,'Float','0.0',None),('y',4,'Float','0.0',None)];VECTOR3=VECTOR2+[('z',8,'Float','0.0',None)]
PHASE=[('Type',0,'Enum',None,'SlowDeathPhaseType')];SOUND=[('List',4,'StringList',None,None)]+PHASE
CHILD_BASE=[('FX',76,1,'&slowSlowDeathFXListType',12),('OCL',84,1,'&slowSlowDeathOCLType',12),('Weapon',92,1,'&slowSlowDeathWeaponType',12),('Sound',100,1,'&slowSlowDeathSoundType',12),('DieMuxData',108,4,'&dieDieMuxDataType',0)]
CHILD_GEO=[('Shapes',16,1,'&slowGeometryShape',32),('RotationAnchorOffset',24,2,'nullptr',0),('Center',28,2,'nullptr',0),('HighestContactPoint',32,2,'nullptr',0),('InnermostContactPoint',36,2,'nullptr',0),('ContactPoints',40,4,'&slowContactPoint',0)]
SETS=[BASE,CLEAR,HELI,[],BUILD]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==130;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 for n,size in [('GeometryShape',32),('BuildingDestructionItem',20)]:
  body=next(v[0] for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003A'+n+'_003E(IList*'))
  assert str(size)+'u, num' in body,(n,size)
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Complete SlowDeath/destruction group; original blob '+BLOB+'.']
 for name,count,unknown in [('GeometryType',4,1),('SlowDeathPhaseType',4,2)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue slow'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum slow{name} = {{ slow{name}Names, {count}u, {unknown}, 0u }};']
 def fields(n,fs):
  if fs:lines.extend(['const ModuleField slow'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(k),o,kind,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&slow'+e) for k,o,kind,d,e in fs]+['};'])
 def tokens(n,fs,ch,parent,expected=None):
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;ty=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0;nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for k,o,kind,d,e in fs:
    if off==o and kind.startswith('Nullable'):mode=2
    if off==o and kind=='StringList':mode=6
   for k,o,cm,cn,cs in ch:
    if off==o:mode=cm;nested=cn;stride=cs
   records.append((key,ty,off,nested,mode,stride))
  expected=expected if expected is not None else ([0] if parent else [])+[o for k,o,kind,d,e in fs]+[o for k,o,cm,cn,cs in ch]
  assert [r[2] for r in records]==expected,(n,records,expected);assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines.extend(['const ModuleToken slow'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout slow{n} = {{ slow{n}Tokens, {len(records)}u }};'])
 def emit(n,fs,ch=[],parent=None):
  sig=f'internal unsafe static void Marshaler_002EMarshal(INode* node, {n}* @object, Tracker* state)'
  if sig in bodies:mb,lits=bodies[sig]
  else:mb,lits=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+n+'* @object, Tracker* state,'))
  actual=[]
  for l in mb.splitlines():
   z=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if z:actual.append((lits[z[0]],lits[z[1]] if len(z)>1 else None))
  assert actual==[(k,d) for k,o,kind,d,e in fs]+[(k,None) for k,o,cm,cn,cs in ch],(n,actual)
  fields(n,fs)
  if n not in ['Vector2','Vector3','SlowDeathSoundType']:tokens(n,fs,ch,parent)
 fields('SlowDeathBaseType',PHASE);tokens('SlowDeathBaseType',PHASE,[],None)
 for n,member in [('SlowDeathFXListType','FX'),('SlowDeathOCLType','OCL'),('SlowDeathWeaponType','Weapon')]:
  # The original IList helper inlines the per-row Marshal; retain its complete archived body.
  mb,lits=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003A'+n+'_003E(IList*'))
  assert member in lits.values() and 'Type' in lits.values() and '12u, num' in mb and '(SlowDeathPhaseType*)ptr2' in mb
  tokens(n,[],[(member,4,3,'nullptr',0)],'&slowSlowDeathBaseType',[0,4])
 emit('SlowDeathSoundType',SOUND,[],None); # Token parent handled below by re-emitting with exact base/list contract.
 # Replace the emitted Sound token segment with the original two-record inherited representation.
 tokens('SlowDeathSoundType',[('List',4,'StringList',None,None)],[],'&slowSlowDeathBaseType',[0,4])
 emit('BuildingDestructionItem',ITEM)
 emit('Vector2',VECTOR2);emit('Vector3',VECTOR3)
 emit('ContactPoint',[('Label',0,'String',None,None)],[('Pos',8,0,'nullptr',0)])
 emit('GeometryShape',SHAPE,[('Offset',24,2,'nullptr',0)])
 emit('GeometryInfo',GEOMETRY,CHILD_GEO)
 emit('SlowDeathBehaviorModuleData',BASE,CHILD_BASE,'&spawnUpdateModuleData')
 emit('ClearanceTestingSlowDeathBehaviorModuleData',CLEAR,[('ClearanceGeometry',132,4,'&slowGeometryInfo',0),('ClearanceGeometryOffset',136,2,'nullptr',0)],'&slowSlowDeathBehaviorModuleData')
 emit('HelicopterSlowDeathBehaviorModuleData',HELI,[],'&slowSlowDeathBehaviorModuleData')
 emit('BuildingDestructionModuleData',BUILD,[('Item',64,1,'&slowBuildingDestructionItem',20)],'&dieDieModuleData')
 ship=next(v[0] for k,v in bodies.items() if k.startswith('internal unsafe static void Tokenizer_002ETokenizePolymorphicType_') and 'ShipSlowDeath' in k);assert '12u, 2u' in ship and '((int*)ptr2)[1] = -1737058412;' in ship
 lines+=['const ModuleToken slowShipSlowDeathBehaviorModuleDataTokens[] = { { 1u, 2557908884u, 0u, &slowSlowDeathBehaviorModuleData, 0u, 0u } };','const ModuleLayout slowShipSlowDeathBehaviorModuleData = { slowShipSlowDeathBehaviorModuleDataTokens, 1u };']
 lines+=['const ModuleFieldSet slowFieldSets[] = {']+[('    { slow'+x['name']+'Fields, '+str(len(fs))+'u },' if fs else '    { nullptr, 0u },') for x,fs in zip(m['modules'],SETS)]+['};','const ModuleChoice slowChoices[] = {']+[f'    {{ {x["typeId"]}u, &slow{x["name"]}, {x["nativeSize"]}u, {265+i}u }},' for i,x in enumerate(m['modules'])]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603';m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(s,pe,n) for n in m['peEnums']}
 output=generate();p=ROOT/'recovery/src/Native/GameObjectSlowDeath.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete SlowDeath group'
 else:p.write_text(output)
 print('PASS five complete SlowDeath/destruction modules, Geometry48/Shape32/Contact20 and nested phase/reference/string lists; 130 original methods/five PE maps')
