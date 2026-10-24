"""Complete area damage/targeting group; exact pinned original methods and PE maps."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/area-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[
 [('NodeLifetime',8,'Time',None,None),('DamageInterval',12,'Time',None,None),('Distance',16,'Float',None,None),('Damage',20,'Float',None,None),('DamageType',24,'Enum','UNDEFINED','DamageType'),('DeathType',28,'Enum','NORMAL','DeathType'),('DamageFXType',32,'Enum','UNDEFINED','DamageFXType'),('ObjectStatusForbiddenForDamage',36,'ObjectStatus',None,None),('MinDistanceTolerance',64,'Float','10.0',None),('MaxValidHeightOffGround',68,'Float','0.0',None),('FXParticleSystemTemplate',72,'Reference',None,None),('NodeFXList',76,'Reference',None,None)],
 [('WeaponName',8,'Reference',None,None),('SecondaryWeaponName',12,'NullableReference',None,None)]+[(n,o,'Float','0',None) for n,o in [('Radius',16),('MinRadius',20),('PreattackDistance',24),('SweepFrequency',28),('SweepAmplitude',32),('DivingFloor',36),('InitialSweepPhase',40),('TargetJitterOffset',44)]]+[('ShotsToFirePerFrame',48,'Unsigned','1',None),('Options',52,'Bits',None,'StrafeAreaUpdateFlagType')],
 [('UsePlayerStore',40,'Boolean','false',None),('IncomingAngle',8,'Angle','0d',None),('WeaponName',12,'Reference',None,None),('MaxShots',16,'Signed','1',None)],
 [('Radius',8,'Float','0',None),('MaxAttacksPerTarget',12,'Signed','-1',None),('OverflowOptions',16,'Bits',None,'StrafeAreaTargetChooserFlagType')],
 [('Flags',8,'Bits',None,'CircularDamageUpdateOptionsType'),('MaxRadius',12,'Float','100.0',None),('ArcDistance',16,'Float','10.0',None),('SweepFXList',20,'Reference',None,None),('SweepWeapon',24,'Reference',None,None),('FXTimeout',28,'Time','0.25s',None)],
 [('Radius',32,'Signed','0',None),('RequiredUpgrade',36,'Reference',None,None)]
]
CHILDREN=[ [('ObjectFilter',80,7,'nullptr'),('ObjectStatusValidation',84,4,'&weaponObjectStatusValidationDataType')], [], [('ObjectStatusValidation',20,4,'&weaponObjectStatusValidationDataType'),('TimeDelayRange',24,0,'&areaTimeRange'),('RadiusSpreadRange',32,0,'&areaRealRange')], [('ObjectFilter',20,7,'nullptr')], [('ObjectFilter',32,8,'nullptr')], [('ObjectFilter',40,7,'nullptr')] ]
RANGES=[('TimeRange','Time'),('RealRange','Float')]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==94;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Generated complete area group; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('DamageType',39,38,0),('DeathType',34,33,0),('DamageFXType',114,35,0),('StrafeAreaUpdateFlagType',4,3,4),('StrafeAreaTargetChooserFlagType',2,1,2),('CircularDamageUpdateOptionsType',1,0,1)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue area'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum area{name} = {{ area{name}Names, {count}u, {unknown}, {bits}u }};']
 def emit(n,fields,children,parent,index=-1):
  nonlocal lines
  mb,lits=method('Marshaler_002EMarshal',n);actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nested in children],(n,actual)
  if parent:assert 'Marshaler_002EMarshal(node, ('+('FireWeaponUpdateModuleData' if index==5 else 'ModuleData')+'*)@object, state);' in mb
  lines.extend(['const ModuleField area'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&area'+e) for n,o,k,d,e in fields]+['};'])
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0;nested=parent if key==1 else 'nullptr';mode=0
   for name,o,k,d,e in fields:
    if off==o and k=='NullableReference':mode=2
   for name,o,cm,cn in children:
    if off==o:mode=cm;nested=cn
   records.append((key,typ,off,nested,mode,0))
  assert [r[2] for r in records]==([0] if parent else [])+[o for name,o,k,d,e in fields]+[o for name,o,cm,cn in children],(n,records)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken area'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout area{n} = {{ area{n}Tokens, {len(records)}u }};']
 for n,k in RANGES:emit(n,[('Low',0,k,None,None),('High',4,k,None,None)],[],None)
 for i,meta in enumerate(m['modules']):emit(meta['name'],SETS[i],CHILDREN[i],'&weaponFireWeaponUpdateModuleData' if i==5 else '&lifecycleBehaviorModuleData' if i==3 else '&spawnUpdateModuleData',i)
 lines+=['const ModuleFieldSet areaFieldSets[] = {']+[f'    {{ area{x["name"]}Fields, {len(f)}u }},' for x,f in zip(m['modules'],SETS)]+['};','const ModuleChoice areaChoices[] = {']+[f'    {{ {x["typeId"]}u, &area{x["name"]}, {x["nativeSize"]}u, {238+i}u }},' for i,x in enumerate(m['modules'])]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603';m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(source,pe,n) for n in m['peEnums']}
 output=generate();p=ROOT/'recovery/src/Native/GameObjectArea.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete area group'
 else:p.write_text(output)
 print('PASS complete six-type area group with full FireWeapon32, inline ranges and Filter112/validation56; 94 original methods and twelve PE maps')
