"""Complete remaining scalar/string utility modules, pinned to original methods and wrappers."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[(783731100,'StorePurchasedUpgradeBehaviorModuleData',12,'BehaviorModuleData','lifecycleBehaviorModuleData',[('MaxPlayerUpgrades',8,'Unsigned','9999')]),(880602396,'AIGateUpdateModuleData',16,'UpdateModuleData','spawnUpdateModuleData',[('TriggerWidthX',8,'Float','0'),('TriggerWidthY',12,'Float','0')]),(2018429368,'CombinedInfoModuleData',16,'CreateModuleData','createCreateModuleData',[('DockingBone',8,'String',None)]),(4216826759,'TimedOpacityUpdateModuleData',16,'UpdateModuleData','spawnUpdateModuleData',[('Opacity',8,'Float','0.5'),('Duration',12,'Time','5s')])]
SETS += [
 (344630372,'W3DPropDrawModuleData',12,'DrawModuleData','moduleTable13',[('Model',8,'Reference',None)]),
 (2739727708,'AttributeModifierNavalModuleData',12,'CreateModuleData','createCreateModuleData',[('AttributeModifier',8,'Reference',None)]),
 (3032642002,'ReplaceDrawModuleBehaviorModuleData',12,'BehaviorModuleData','lifecycleBehaviorModuleData',[('ConsumeItem',8,'Boolean','true')]),
 (3231835197,'StancesBehaviorModuleData',12,'UpdateModuleData','spawnUpdateModuleData',[('StanceTemplate',8,'Reference',None)]),
 (3052093933,'SquishCollideModuleData',20,'CollideModuleData','motionCollideModuleData',[('CrushAnimTime',8,'Time','1s'),('CrushKillDelay',12,'Time','.5s'),('UseDirectionCheck',16,'Boolean','true'),('UseVehicleStyleCrush',17,'Boolean','false'),('AllowAllyCrush',18,'Boolean','false')]),
 (1206381257,'AreaRestrictSpecialPowerBehaviorModuleData',20,'BehaviorModuleData','lifecycleBehaviorModuleData',[('RestrictionType',8,'Enum',None),('AreaName',12,'String',None)]),
 (3040483501,'MinefieldBehaviorModuleData',28,'UpdateModuleData','spawnUpdateModuleData',[('DetonationWeapon',8,'Reference',None),('DetonatedBy',12,'Bits',None),('DetonationFX',16,'Reference',None),('NumVirtualMines',20,'Unsigned',None),('RepeatDetonateMoveThresh',24,'Float','5.0')]),
 (3214000077,'LiftObjectUpdateModuleData',40,'UpdateModuleData','spawnUpdateModuleData',[('CrusherModifiesVelocity',36,'Boolean',None),('LiftVelocity',8,'Float',None),('MaxElevationFromGround',12,'Float',None),('TimeIncrement',16,'Time','0s'),('MaxTimeLifted',20,'Time','1s'),('MaxStructureShakeVelocity',24,'Float','0'),('AirplaneCrashWeapon',28,'Reference',None),('SoftLandingWeapon',32,'Reference',None)]),
 (3833989627,'ClusterBombUpdateModuleData',36,'UpdateModuleData','spawnUpdateModuleData',[('WeaponName',8,'Reference',None),('InitialWeaponName',12,'NullableReference',None),('Radius',16,'Float','0.0'),('NumBomblets',20,'Signed','0'),('MinDelay',24,'Time','0s'),('MaxDelay',28,'Time','0s'),('BombletFX',32,'Reference',None)])]

def generate():
 s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB;literals={};lines=['// Complete scalar/string utilities; original blob '+BLOB+'.']
 for line in s.splitlines():
  m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
  if m:
   try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
   except UnicodeDecodeError:pass
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
 for name,unknown,bits in [('SpecialPowerRestrictionType',1,0),('ObjectFilterRelationship',0,4)]:
  values=enum_values(s,pe,name)
  lines+=['const EnumNameValue utility'+name+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum utility{name} = {{ utility{name}Names, {len(values)}u, {unknown}, {bits}u }};']
 enumField={'RestrictionType':'&utilitySpecialPowerRestrictionType','DetonatedBy':'&utilityObjectFilterRelationship'}
 for idx,(typ,n,size,parent,layout,fields) in enumerate(SETS):
  assert inv['marshal'][str(typ)]['name']==n and inv['marshal'][str(typ)]['nativeSize']==size
  ms=f'internal unsafe static void Marshaler_002EMarshal(INode* node, {n}* @object, Tracker* state)';es=f'internal unsafe static void Marshaler_002EEndianCorrect({n}* @object, EndianHelper* state)'
  mb=body_of(s,ms if ms in s else inv['marshal'][str(typ)]['helperSignature']);eb=body_of(s,es if es in s else inv['endian'][str(typ)]['helperSignature']);tb=body_of(s,f'internal unsafe static void Tokenizer_002ETokenize(void** table, {n}* @object, Tracker* state)')
  actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d in fields];assert 'Marshaler_002EMarshal(node, (ModuleData*)@object, state);' in mb or 'Marshaler_002EMarshal(node, (ModuleData*)ptr, tracker);' in mb
  assert all(('((byte*)@object + '+str(o)+')' in eb or '((byte*)ptr + '+str(o)+')' in eb or (o%4==0 and '((int*)@object)['+str(o//4)+']' in eb)) for o in [4]+[f[1] for f in fields if f[2] not in ['Reference','NullableReference','Boolean']]),n
  # Confirm dispatcher helpers retain the complete direct method (no inline derived contract).
  for family in ['marshal','endian','token']:
   wrapper=body_of(s,inv[family][str(typ)]['helperSignature']);assert n+'*' in wrapper
  lines+=['const ModuleField utility'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),enumField.get(n,'nullptr')) for n,o,k,d in fields]+['};']
  records=[]
  for i,c in enumerate(tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]):
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;kind=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0
   if i==0:assert key==1 and '('+parent+'*)@object' in c
   records.append((key,kind,off,'&'+layout if i==0 else 'nullptr',2 if any(f[1]==off and f[2]=='NullableReference' for f in fields) else 0))
  assert [x[2] for x in records]==[0]+[o for n,o,k,d in fields] and len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
  lines+=['const ModuleToken utility'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, 0u },'%r for r in records]+['};',f'const ModuleLayout utility{n} = {{ utility{n}Tokens, {len(records)}u }};']
 lines+=['const ModuleFieldSet utilityFieldSets[] = {']+[f'    {{ utility{n}Fields, {len(f)}u }},' for typ,n,size,parent,layout,f in SETS]+['};','const ModuleChoice utilityChoices[] = {']+[f'    {{ {typ}u, &utility{n}, {size}u, {225+i}u }},' for i,(typ,n,size,parent,layout,f) in enumerate(SETS)]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 output=generate();p=ROOT/'recovery/src/Native/GameObjectUtility.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete utility modules'
 else:p.write_text(output)
 print('PASS thirteen complete utility modules, exact Marshal/Endian/Token and dispatcher helper contracts')
