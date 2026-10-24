"""Complete theft SpecialAbility344 and nullable-vector OCL CreateAndEnter504 contracts."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/ability-resource-source'
m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
bodies={}
for x in m['methods']:
 text=(E/x['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
fields=[('TiberiumStolenPerUpdate',240,'Signed','1'),('DelayBetweenThefts',244,'Time','0s'),('TiberiumCapacity',248,'Signed','1'),('ObjectStatusWhenNotEmpty',252,'ObjectStatus',''),('ModelConditionWhenNotEmpty',280,'ModelCondition',None),('PlayWeaponPreFireFX',340,'Boolean','false')]
lines=['// Generated complete resource/creation contracts; original blob '+m['sourceBlob']+'.','const ModuleField abilityResourceTheftFields[] = {']+['    { %s, %du, ModuleValue::%s, %s, nullptr },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d)) for n,o,k,d in fields]+['};']
for i,meta in enumerate(m['modules']):
 name=meta['name'];parent=['SpecialAbilityUpdateModuleData','OCLSpecialPowerModuleData'][i];layout=['abilitySpecialAbilityUpdateModuleData','powerCreationOCLSpecialPowerModuleData'][i]
 mb,lits=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'];actual=[]
 for line in mb.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
 assert actual==([(n,d) for n,o,k,d in fields] if i==0 else [('FXOffset',None)])
 assert 'Marshaler_002EMarshal(node, ('+parent+'*)@object, state);' in mb
 tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'];chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for part in chunks[:-1]:
  key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',part)[1])&0xffffffff;off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1]) if off else 0;mode=2 if 'BookmarkMemberOffset(state, (void*)(int)' in part else 0
  records.append((key,typ,off,'&'+layout if key==1 else 'nullptr',mode,0))
 assert [r[2] for r in records]==[0]+([f[1] for f in fields] if i==0 else [500]);assert records[0][1]==[2891550594,2308556452][i];assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert [r[4] for r in records]==([0]*7 if i==0 else [0,2])
 for word in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert word in chunks[-1]
 lines+=['const ModuleToken abilityResource'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityResource{name} = {{ abilityResource{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice abilityResourceChoices[] = {']+[f'    {{ {x["typeId"]}u, &abilityResource{x["name"]}, {x["nativeSize"]}u, {213+i}u }},' for i,x in enumerate(m['modules'])]+['};','']
output='\n'.join(lines);p=ROOT/'recovery/src/Native/GameObjectAbilityResource.h'
if '--check' in sys.argv:assert p.read_text()==output,'Regenerate GameObjectAbilityResource.h'
else:p.write_text(output)
print('PASS complete TiberiumThief344/CreateAndEnter504 fields, full inheritance, nullable vector and 17 original methods')
