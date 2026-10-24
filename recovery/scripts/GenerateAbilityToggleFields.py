"""Complete ToggleStatus252 / nugget280 / inline AI148 / nullable weapon28.
Original method bodies and PE enum maps are checked before accepting generated output.
"""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
E=ROOT/'recovery/evidence/gameobject-modules/ability-toggle-next-source'
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
# name, byte offset, representation, default, enum contract; original Marshal order.
WEAPON=[('Weapon',0,'Reference',None,None),('IsDisablingAttack',24,'Boolean','false',None),('AffectsFriendlies',25,'Boolean','false',None),('AffectsSelf',26,'Boolean','false',None),('TotalValueGreaterThan',4,'Signed','0',None),('AoeAttackPowerLocationPreference',8,'Enum','PREFER_NEAREST','AIAoeAttackPowerLocationPreference'),('AoeAttackPowerBlockAreaAfterActivationTime',12,'Time','0s',None),('AoeAttackEmanationLocation',16,'Enum','EMANATE_FROM_SELF','AIAoeAttackPowerEmanationLocation'),('ToggleRangeHysteresis',20,'Float','0.0',None),('ToggleShortRangeTargetForcesTarget',27,'Boolean','false',None)]
AI=[('ToggleHint',0,'Enum',None,'ToggleStatusAiHint')]+[(n,4+4*i,'NullableFloat',None,None) for i,n in enumerate(['UseOnlyWhenPowerAdvantageIsAbove','UseOnlyWhenPowerAdvantageIsBelow','UseOnlyWhenCostAdvantageIsAbove','UseOnlyWhenCostAdvantageIsBelow','NeverUseWhenFurtherThanNUnitsFromTarget','NeverUseWhenCloserThanNUnitsFromTarget','NeverUseWhenHealthIsBelow'])]+[
('OnlyUseWhenHaveTarget',144,'Boolean','false',None),('MaxZDifferenceFromTarget',32,'NullableFloat',None,None),('NeverUseInState',36,'Bits','','MicroManagerState'),('NeverUseDuringTask',40,'Bits','','MicroManagerUnitTask'),('NeverUseWhenClear',145,'Boolean','false',None),('SelfDefenseDamageLookAheadTime',44,'NullableTime',None,None),('SelfDefenseDamageLookBehindTime',48,'NullableTime',None,None),('SelfDefenseDamageFraction',52,'NullableFloat',None,None),('DontUseIfInRange',146,'Boolean','false',None),('NumOtherAttackersOnTarget',56,'Signed','1',None),('LeechWeaponUpgrade',60,'Reference','',None),('LeechWeaponTargetHealthFraction',64,'Float','0.1',None),('LeechWeaponSOLModuleId',68,'NullableAssetId',None,None),('LeechHealthActivatePowerAdvantage',72,'Float','1.7',None),('LeechHealthDeactivatePowerAdvantage',76,'Float','1.2',None),('StealthOthersInvisibilityModuleId',80,'NullableAssetId',None,None),('ShieldMinCostProtected',84,'Signed','0',None),('ShieldMinEnemyDPS',88,'Float','1000.0',None),('ShieldCostAdjustPastDistanceFraction',92,'Float','0.5',None),('ShieldCostAdjustMultiplierAtEdge',96,'Float','0.0',None),('EnemyMustBeInVisionRange',147,'Boolean','false',None),('NeverUseWhenTargetIsAnyKindOf',100,'Bits',None,'@aiKindOfType'),('Location',136,'Bits','Water Land','AITargetLocationType')]
NUGGET=[('AttributeModifier',0,'Reference',None,None),('ModelConditions',4,'ModelCondition',None,None),('ObjectStatus',64,'ObjectStatus',None,None),('Upgrade',92,'Reference',None,None),('ArmorSet',96,'Enum','INVALID','ArmorSetType'),('LocomotorSet',100,'Enum','INVALID','LocomotorSetType'),('AIUpdateModuleId',104,'AssetId',None,None),('GenericLuaEventParameter',108,'String','',None),('EnterStateSound',116,'Reference',None,None),('DisabledType',120,'Enum',None,'DisabledType')]
ENUMS={'ToggleStatusAiHint':(16,1,0),'MicroManagerState':(10,0,10),'MicroManagerUnitTask':(10,7,10),'AITargetLocationType':(2,1,2),'AIAoeAttackPowerLocationPreference':(3,1,0),'AIAoeAttackPowerEmanationLocation':(2,0,0),'ArmorSetType':(23,12,0),'LocomotorSetType':(19,12,0),'DisabledType':(12,0,0)}
def generate(enums):
 manifest=json.loads((E/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
 bodies={}
 for m in manifest['methods']:
  text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'];bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
 def method(op,name):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{name}* @object, Tracker* state)']
 lines=['// Generated complete ToggleStatus dependency group; original blob '+BLOB+'.']
 for name,(count,unknown,bits) in ENUMS.items():
  values=enums[name];assert len(values)==count
  lines+=['const EnumNameValue abilityToggle'+name+'Names[] = {']+['    { '+json.dumps(n)+', '+str(v)+' },' for n,v in sorted(values.items())]+['};',f'const AIEnum abilityToggle{name} = {{ abilityToggle{name}Names, {count}u, {unknown}, {bits}u }};']
 for name,fields,children in [('AISpecialPowerWeaponData',WEAPON,[]),('AIToggleSpecialPowerInfo',AI,[('StateWeapon',None)]),('ToggleStatusNugget',NUGGET,[('SkirmishAiInfo',None),('Module',None)]),('ToggleStatusSpecialAbilityUpdateModuleData',[('ReloadAllWeaponsOnToggle',248,'Boolean','true',None)],[('ToggleState',None)])]:
  mb,lits=method('Marshaler_002EMarshal',name);actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+children,(name,actual)
  lines+=['const ModuleField abilityToggle'+name+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+(e[1:] if e.startswith('@') else 'abilityToggle'+e)) for n,o,k,d,e in fields]+['};']
  tb,_=method('Tokenizer_002ETokenize',name);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for chunk in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff
   off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk)
   if off:off=int(off[1])
   else:
    off=re.search(r'\((float|bool)\*\)@object \+ (\d+)',chunk);off=int(off[2])*(4 if off[1]=='float' else 1) if off else 0
   nested='nullptr';mode=0;stride=0
   if name=='ToggleStatusSpecialAbilityUpdateModuleData':
    if key==1:nested='&abilitySpecialAbilityUpdateModuleData'
    if off==240:nested='&abilityToggleToggleStatusNugget';mode=1;stride=280
   if name=='ToggleStatusNugget':
    if off==124:nested='&abilityToggleAIToggleSpecialPowerInfo'
    if off==272:nested='&abilityRoutingSpecialPowerModuleInfo';mode=1;stride=12
   if name=='AIToggleSpecialPowerInfo':
    if off in [4,8,12,16,20,24,28,32,44,48,52,68,80]:mode=2
    if off==140:nested='&abilityToggleAISpecialPowerWeaponData';mode=4
   records.append((key,typ,off,nested,mode,stride))
  expected=[o for n,o,k,d,e in fields]+([140] if name=='AIToggleSpecialPowerInfo' else [124,272] if name=='ToggleStatusNugget' else [])
  if name=='ToggleStatusSpecialAbilityUpdateModuleData':expected=[0,248,240]
  assert [r[2] for r in records]==expected,(name,records,expected)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken abilityToggle'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityToggle{name} = {{ abilityToggle{name}Tokens, {len(records)}u }};']
 assert '280u, num' in (E/'018.txt').read_text() and '28u, 1u' in (E/'038.txt').read_text() and '12u, num' in (E/'049.txt').read_text()
 lines+=['const ModuleChoice abilityToggleChoice = { 3977962844u, &abilityToggleToggleStatusSpecialAbilityUpdateModuleData, 252u, 215u };','']
 return '\n'.join(lines)
if __name__=='__main__':
 from GenerateUpgradeFields import enum_values
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
 output=generate({name:enum_values(source,pe,name) for name in ENUMS});path=ROOT/'recovery/src/Native/GameObjectAbilityToggle.h'
 if '--check' in sys.argv:assert path.read_text()==output,'Regenerate complete ToggleStatus contract'
 else:path.write_text(output)
 print('PASS ToggleStatus252, nugget280, inline AI148, nullable weapon28 and ModuleInfo12; 59 original methods and 9 original PE maps')
