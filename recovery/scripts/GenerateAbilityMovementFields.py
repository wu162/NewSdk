"""Generate four complete SpecialAbility movement derivations from bounded original evidence.

TestGameObject also checks ExportAbilityMovementMethods against the fixed full source.
"""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
E=ROOT/'recovery/evidence/gameobject-modules/ability-movement-next-source'
manifest=json.loads((E/'manifest.json').read_text())
assert manifest['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
bodies={}
for m in manifest['methods']:
 text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n'
 assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'],m['file']
 bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
SWITCH=[('UseUpgradedLocomotor',256,'Boolean',None,None),('TriggersLanding',257,'Boolean','false',None),('TriggersFlight',258,'Boolean','false',None),('BusyForDuration',240,'Time','0s',None),('LandingRange',244,'Float','0',None),('HordeMembersSpecialPowerTemplate',248,'Reference',None,None),('LocomotorToSet',252,'Enum',None,'abilityMovementLocomotorEnum')]
EJECT=[('TimePerPassenger',240,'Time','1s',None),('Speed',244,'Float','10.0',None),('Radius',248,'Float','20.0',None),('Height',252,'Float','100.0',None),('ApexHeight',256,'Float','0.0',None),('EjectionFX',260,'Reference',None,None),('EjectionBoneName',264,'String',None,None),('RunOffMapWhenDone',276,'Boolean','false',None),('IgnoreTargetLocation',277,'Boolean','false',None)]
TELEPORT=[('BusyDuration',240,'Time','0s',None),('DestinationWeapon',244,'Reference',None,None),('SourceWeapon',248,'Reference',None,None),('MaxDistance',252,'Float','0',None),('HeightToTeleportTo',256,'Float','0',None),('FadeInTime',260,'Time','0s',None)]
CASTER=[(n,240+4*i,'Float','0',None) for i,n in enumerate(['Radius','MinDestinationRadius','MaxDestinationRadius','MaxRangeFromCasterSP','MaxRangeFromCasterMP'])]+[('TriggerFX',260,'Reference',None,None),('TargetFX',264,'Reference',None,None),('RequireStoredObjects',268,'Boolean','false',None),('KillObjectsIfPlacedInBadSpot',269,'Boolean','false',None)]
sets=[SWITCH,EJECT,TELEPORT,CASTER]
lines=['// Generated complete movement/teleport SpecialAbility derivations; fixed original blob '+manifest['sourceBlob']+'.']
# Reuse the exact LocomotorSet table already generated and verified from the original PE.
source=(ROOT/'recovery/src/Native/GameObjectFields.h').read_text()
names=re.findall(r'\{ "LocomotorSet", "([^"]+)", (-?\d+) \}',source)
assert len(names)==19 and dict(names)['INVALID']=='-1'
assert re.search(r'\{ "LocomotorSet", 12 \}',source)
lines+=['const EnumNameValue abilityMovementLocomotorNames[] = {']+['    { '+json.dumps(n)+', '+v+' },' for n,v in names]+['};','const AIEnum abilityMovementLocomotorEnum = { abilityMovementLocomotorNames, 19u, 12, 0u };']
for i,(meta,fields) in enumerate(zip(manifest['modules'],sets)):
 name=meta['name'];mb,literals=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)']
 actual=[]
 for line in mb.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 assert actual==[(n,d) for n,o,k,d,e in fields]+([('EjectionBoneOffset',None)] if i==1 else []),(name,actual)
 parent='SpecialAbilityUpdateModuleData'
 assert 'Marshaler_002EMarshal(node, ('+parent+'*)@object, state);' in mb
 lines+=['const ModuleField abilityMovementFields'+str(i)+'[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};']
 tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)']
 chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks[:-1]:
  key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff
  typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff
  off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0
  mode=2 if 'BookmarkMemberOffset(state, (void*)(int)' in chunk else 0
  records.append((key,typ,off,'&abilitySpecialAbilityUpdateModuleData' if key==1 else 'nullptr',mode,0))
 assert [r[2] for r in records]==[0]+[f[1] for f in fields]+([272] if i==1 else []),name
 assert [r[4] for r in records]==[0]*(len(fields)+1)+([2] if i==1 else []),name
 assert records[0][1]==2891550594
 assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
 for word in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert word in chunks[-1]
 lines+=['const ModuleToken abilityMovement'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityMovement{name} = {{ abilityMovement{name}Tokens, {len(records)}u }};']
lines+=['const ModuleFieldSet abilityMovementFieldSets[] = {']+[f'    {{ abilityMovementFields{i}, {len(f)}u }},' for i,f in enumerate(sets)]+['};','const ModuleChoice abilityMovementChoices[] = {']+[f'    {{ {m["typeId"]}u, &abilityMovement{m["name"]}, {m["nativeSize"]}u, {200+i}u }},' for i,m in enumerate(manifest['modules'])]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectAbilityMovement.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAbilityMovement.h'
else:path.write_text(output)
print('PASS four complete SpecialAbility movement/teleport field and Token layouts; 32 original method fingerprints')

