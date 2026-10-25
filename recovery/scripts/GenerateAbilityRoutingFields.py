"""Two complete SpecialAbility routing/radar types, including exact structured list contracts."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/ability-routing-source'
manifest=json.loads((E/'manifest.json').read_text());assert manifest['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
bodies={}
for m in manifest['methods']:
 text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'];bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
lines=['// Generated complete routing/radar SpecialAbility contracts; fixed original blob '+manifest['sourceBlob']+'.']
radar=[('ObjectStatus',0,'ObjectStatus',None,None),('ModelCondition',28,'ModelCondition',None,None),('KindOf',88,'Bits',None,'aiKindOfType'),('UseLocalPlayerFilter',124,'Boolean','true',None)]
info=[('ModuleId',0,'AssetId',None,None),('TriggerAtTargetPosition',8,'Boolean','true',None),('TriggerType',4,'Enum','TARGET_POSITION','powerRoutingTrigger')]
for name,fields in [('KindOfAndStatusAndModelConditionType',radar),('SpecialPowerModuleInfo',info)]:
 mb,literals=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'];actual=[]
 for line in mb.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 assert actual==[(n,d) for n,o,k,d,e in fields],(name,actual)
 lines+=['const ModuleField abilityRouting'+name+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};']
def emit(name,child=None,stride=0):
 tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'];chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks[:-1]:
  key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff;off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0
  nested='&abilitySpecialAbilityUpdateModuleData' if key==1 else '&abilityRouting'+child if child else 'nullptr';mode=1 if stride and key!=1 else 0
  if key!=1 and not stride:nested='nullptr'
  records.append((key,typ,off,nested,mode,stride if mode else 0))
 assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
 lines.extend(['const ModuleToken abilityRouting'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityRouting{name} = {{ abilityRouting{name}Tokens, {len(records)}u }};'])
 return records
assert [r[2] for r in emit('SpecialPowerModuleInfo')]==[0,8,4]
assert [r[2] for r in emit('KindOfAndStatusAndModelConditionType')]==[0,28,88,124]
for meta,child,stride in zip(manifest['modules'],['SpecialPowerModuleInfo','KindOfAndStatusAndModelConditionType'],[12,128]):
 name=meta['name'];mb,lits=next((b,l) for sig,(b,l) in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and name+'_002C' in sig)
 assert meta['nativeSize']==248 and '248u, 1u' in mb and '(SpecialAbilityUpdateModuleData*)ptr, tracker);' in mb and '((byte*)ptr + 240)' in mb
 helper=next(b for sig,(b,l) in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003A'+child+'_003E('));assert f'{stride}u, num' in helper
 assert [r[2] for r in emit(name,child,stride)]==[0,240]
lines+=['const ModuleChoice abilityRoutingChoices[] = {']+[f'    {{ {m["typeId"]}u, &abilityRouting{m["name"]}, 248u, {211+i}u }},' for i,m in enumerate(manifest['modules'])]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectAbilityRouting.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAbilityRouting.h'
else:path.write_text(output)
print('PASS two complete SpecialAbility routing/radar types, SpecialPowerModuleInfo12 and intersection128; 26 original methods')
