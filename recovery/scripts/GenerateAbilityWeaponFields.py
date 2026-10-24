"""Generate four complete SpecialAbility weapon derivations from bounded original evidence.

TestGameObject also checks ExportAbilityWeaponMethods against the fixed full source.
"""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
E=ROOT/'recovery/evidence/gameobject-modules/ability-weapon-next-source'
manifest=json.loads((E/'manifest.json').read_text())
assert manifest['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
bodies={}
for m in manifest['methods']:
 text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n'
 assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'],m['file']
 bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
FIRE=[('SpecialWeapon',240,'Reference',None,None),('WhichSpecialWeapon',244,'Signed','0',None),('SkipContinue',264,'Boolean','false',None),('BusyForDuration',248,'Time','0s',None),('NeedLivingTargets',265,'Boolean','false',None),('PlayWeaponPreFireFX',266,'Boolean','false',None),('OffsetTargetLocationFromObject',267,'Boolean','false',None),('WeaponSlotID',252,'Signed','1',None),('WeaponSlotType',256,'Enum','PRIMARY_WEAPON','weaponSetWeaponSlotTypeEnum'),('DestealthClosestObjectMatchingFilter',268,'Boolean','false',None),('ModelConditionsOnUnpack',260,'Enum','INVALID','aiModelConditionFlagType')]
DELAY=[('FireDelay',272,'Time','0s',None)]
BEAM=[('DamagePerSecond',240,'Float',None,None),('JoinWithOtherBeams',268,'Boolean','false',None),('DissapateWhenTargetDead',269,'Boolean','true',None),('LineType',270,'Boolean','false',None),('TargetAttributeModifierAdd',244,'Reference',None,None),('TargetObjectStatusAdd',248,'NullableObjectStatus',None,None),('SweepFX',252,'Reference',None,None),('ReflectorExtendDistance',256,'Float','0',None),('TargetSamePlayerOnly',271,'Boolean','false',None),('PreferredTargetBone',260,'String',None,None)]
STATUS=[('Status1',240,'ObjectStatus',None,None),('Status2',268,'ObjectStatus',None,None)]
sets=[FIRE,DELAY,BEAM,STATUS]
parents=['abilitySpecialAbilityUpdateModuleData','abilityWeaponWeaponFireSpecialAbilityUpdateModuleData','abilitySpecialAbilityUpdateModuleData','abilitySpecialAbilityUpdateModuleData']
lines=['// Generated complete weapon/beam/status SpecialAbility derivations; fixed original blob '+manifest['sourceBlob']+'.']
for i,(meta,fields) in enumerate(zip(manifest['modules'],sets)):
 name=meta['name'];mb,literals=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)']
 actual=[]
 for line in mb.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 assert actual==[(n,d) for n,o,k,d,e in fields],(name,actual)
 parent='WeaponFireSpecialAbilityUpdateModuleData' if i==1 else 'SpecialAbilityUpdateModuleData'
 assert 'Marshaler_002EMarshal(node, ('+parent+'*)@object, state);' in mb
 lines+=['const ModuleField abilityWeaponFields'+str(i)+'[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};']
 tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)']
 chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks[:-1]:
  key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff
  typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff
  off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0
  mode=2 if 'BookmarkMemberOffset(state, (void*)(int)' in chunk else 0
  records.append((key,typ,off,'&'+parents[i] if key==1 else 'nullptr',mode,0))
 assert [r[2] for r in records]==[0]+[f[1] for f in fields],name
 assert records[0][1]==(507880339 if i==1 else 2891550594)
 assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
 for word in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert word in chunks[-1]
 lines+=['const ModuleToken abilityWeapon'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityWeapon{name} = {{ abilityWeapon{name}Tokens, {len(records)}u }};']
lines+=['const ModuleFieldSet abilityWeaponFieldSets[] = {']+[f'    {{ abilityWeaponFields{i}, {len(f)}u }},' for i,f in enumerate(sets)]+['};','const ModuleChoice abilityWeaponChoices[] = {']+[f'    {{ {m["typeId"]}u, &abilityWeapon{m["name"]}, {m["nativeSize"]}u, {194+i}u }},' for i,m in enumerate(manifest['modules'])]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectAbilityWeapon.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAbilityWeapon.h'
else:path.write_text(output)
print('PASS four complete SpecialAbility weapon/beam/status field and Token layouts; 32 original method fingerprints')
