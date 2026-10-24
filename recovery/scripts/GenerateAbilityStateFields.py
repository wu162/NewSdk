"""Generate seven complete SpecialAbility state/object derivations from fixed original methods.
Original PE supplies ReplaceSelfOptions7; shared parent/filter/list implementations are reused.
"""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
E=ROOT/'recovery/evidence/gameobject-modules/ability-state-source'
GIVE=[('CommandButton',240,'String',None,None),('UpgradeToGive',248,'Reference',None,None)]
RECON=[('UpdateModuleId',240,'AssetId',None,None),('TimeStepsBack',244,'Time','0s',None),('LeavePositionFX',248,'Reference',None,None),('ReturnPositionFX',252,'Reference',None,None),('PostReconstituteAttributeModifier',256,'Reference',None,None)]
DOMINATE=[('DominateRadius',240,'Float',None,None),('TriggerFX',244,'Reference',None,None),('DominatedFX',248,'Reference',None,None),('PermanentlyConvert',256,'Boolean','false',None)]
LEVEL=[('Experience',240,'Signed','0',None),('RadiusEffect',244,'Float','0',None),('LevelFX',248,'Reference',None,None)]
CLONE=[('MinDestinationRadius',240,'Float','0',None),('MaxDestinationRadius',244,'Float','0',None),('TriggerAttributeModifierOnClones',248,'Reference',None,None),('TriggerFX',252,'Reference',None,None),('TargetFX',256,'Reference',None,None),('ClonedObjectLifetime',260,'Time','0s',None)]
REPLACE=[('NewObjectUnpackTime',240,'Time','0s',None),('ReplaceOptions',244,'Bits','DISABLE_DURING_REPLACE','abilityStateReplaceOptions'),('ClearTriggerDistance',248,'Float','120.0',None)]
ASSEMBLE=[('NewObjectAssemblyTime',240,'Time','0s',None),('BusyDuringAssembly',256,'Boolean','false',None),('ModelConditionsOnAssembly',244,'Enum','INVALID','aiModelConditionFlagType')]
SETS=[GIVE,RECON,DOMINATE,LEVEL,CLONE,REPLACE,ASSEMBLE]
CHILDREN={2:('AttributeModifierAffects',252,7),3:('AcceptanceFilter',252,7),5:('ReplacementTemplate',252,3),6:('StructureTemplate',248,3)}
def generate(enums):
 manifest=json.loads((E/'manifest.json').read_text());assert manifest['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
 bodies={}
 for m in manifest['methods']:
  text=(E/m['file']).read_text();body='\n'.join(text.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==m['bodySha256'],m['file'];bodies[m['signature']]=(body,json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
 assert len(enums)==7 and enums['DISABLE_DURING_REPLACE']==3
 bitbody=next(b for sig,(b,l) in bodies.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AReplaceSelfOptions'))
 assert 'num4 = 4;' in bitbody and '7u' in bitbody
 lines=['// Generated complete state/object SpecialAbility derivations; original blob '+manifest['sourceBlob']+'.','const EnumNameValue abilityStateReplaceOptionsNames[] = {']+['    { '+json.dumps(n)+', '+str(v)+' },' for n,v in sorted(enums.items())]+['};','const AIEnum abilityStateReplaceOptions = { abilityStateReplaceOptionsNames, 7u, 4, 7u };']
 for i,(meta,fields) in enumerate(zip(manifest['modules'],SETS)):
  name=meta['name'];mb,literals=bodies['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'];actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+([(CHILDREN[i][0],None)] if i in CHILDREN else []),(name,actual)
  assert 'Marshaler_002EMarshal(node, (SpecialAbilityUpdateModuleData*)@object, state);' in mb
  lines+=['const ModuleField abilityStateFields'+str(i)+'[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e) for n,o,k,d,e in fields]+['};']
  tb,_=bodies['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'];chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for chunk in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff
   off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;mode=0
   if 'ObjectFilter*' in chunk:mode=7
   if 'Tokenizer_002ETokenize_003Cstruct_0020TypedAssetId_' in chunk:mode=3
   records.append((key,typ,off,'&abilitySpecialAbilityUpdateModuleData' if key==1 else 'nullptr',mode,4 if mode==3 else 0))
  assert [r[2] for r in records]==[0]+[f[1] for f in fields]+([CHILDREN[i][1]] if i in CHILDREN else []),name
  assert [r[4] for r in records]==[0]*(len(fields)+1)+([CHILDREN[i][2]] if i in CHILDREN else []),name
  assert records[0][1]==2891550594 and len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
  for word in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert word in chunks[-1]
  lines+=['const ModuleToken abilityState'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout abilityState{name} = {{ abilityState{name}Tokens, {len(records)}u }};']
 lines+=['const ModuleFieldSet abilityStateFieldSets[] = {']+[f'    {{ abilityStateFields{i}, {len(f)}u }},' for i,f in enumerate(SETS)]+['};','const ModuleChoice abilityStateChoices[] = {']+[f'    {{ {m["typeId"]}u, &abilityState{m["name"]}, {m["nativeSize"]}u, {204+i}u }},' for i,m in enumerate(manifest['modules'])]+['};','']
 return '\n'.join(lines)
if __name__=='__main__':
 from GenerateUpgradeFields import enum_values
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()=='6e40a9ed6c9f961d294d49768d911e6a6f354619'
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
 output=generate(enum_values(source,pe,'ReplaceSelfOptions'));path=ROOT/'recovery/src/Native/GameObjectAbilityState.h'
 if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAbilityState.h'
 else:path.write_text(output)
 print('PASS seven complete SpecialAbility state/object field/Token contracts, 52 original methods and ReplaceSelfOptions7 from original PE')
