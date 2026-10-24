"""Complete SpecialAbility240 parent and two parent-only Update derivations."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';E=ROOT/'recovery/evidence/gameobject-modules/special-ability-source'
s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB
peEvidence=json.loads((ROOT/'recovery/evidence/gameobject-modules/power-continuation-enums.json').read_text());assert peEvidence['sourceBlob']==BLOB and peEvidence['peSha256']=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
tables={name:peEvidence['tables'][name] for name in ['SpecialAbilityUpdateOptionsType','ConditionsType']};assert len(tables['SpecialAbilityUpdateOptionsType'])==31 and len(tables['ConditionsType'])==3
if '--exported-enums' not in sys.argv:
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==peEvidence['peSha256']
 for name,table in tables.items():assert enum_values(s,pe,name)==table
FIELDS=[('SpecialPowerTemplate',8,'Reference',None,None),('StartAbilityRange',12,'Float','10000000.0',None),('AbilityAbortRange',16,'Float','10000000.0',None),('PreparationTime',20,'Time','0s',None),('PersistentPrepTime',24,'Time','0s',None),('PersistentCount',28,'Signed','-1',None),('PackTime',32,'Time','0s',None),('UnpackTime',36,'Time','0s',None),('PreTriggerUnstealthTime',40,'Time','0s',None),('Options',44,'Bits',None,'abilityOptions'),('PackUnpackVariationFactor',48,'Float','0.0',None),('ParalyzeDurationWhenCompleted',52,'Time','0s',None),('ParalyzeDurationWhenAborted',56,'Time','0s',None),('SpecialObject',60,'AssetId',None,None),('SpecialObjectAttachToBone',64,'String',None,None),('MaxSpecialObjects',72,'Unsigned','1',None),('EffectDuration',76,'Time','0s',None),('EffectValue',80,'Signed','1',None),('EffectRange',84,'Float','0',None),('FleeRangeAfterCompletion',88,'Float','0.0',None),('DisableFXParticleSystem',92,'Reference',None,None),('PackSound',96,'Reference',None,None),('UnpackSound',100,'Reference',None,None),('PrepSoundLoop',104,'Reference',None,None),('TriggerSound',108,'Reference',None,None),('ActiveLoopSound',112,'Reference',None,None),('AwardXPForTriggering',116,'Signed','0',None),('SkillPointsForTriggering',120,'Signed','-1',None),('UnpackingVariation',124,'Signed','0',None),('FreezeAfterTriggerDuration',128,'Time','0s',None),('RequiredConditions',132,'Bits',None,'abilityConditions'),('RejectedConditions',136,'Bits',None,'abilityConditions'),('SetObjectStatusOnTrigger',140,'ObjectStatus',None,None),('ClearObjectStatusOnExit',168,'ObjectStatus',None,None),('ContactPointOverride',196,'String',None,None),('TriggerAttributeModifier',204,'Reference',None,None),('AttributeModifierDuration',208,'Time','0s',None),('ChainedButton',212,'Reference',None,None),('StartRechargeOnExit',236,'Boolean','false',None),('RequireAndSpendTiberiumOnCaster',216,'Unsigned','0',None),('GoIdleInStartPreparation',237,'Boolean','true',None),('FaceTarget',238,'Boolean','true',None),('DisabledTypesToProcess',220,'Bits','HELD','specialPowerDisabled'),('DisabledTypesToContinueSoundsFor',224,'Bits',None,'specialPowerDisabled'),('ActiveModelCondition',228,'Enum',None,'aiModelConditionFlagType')]
SPECS=[('SpecialAbilityUpdateModuleData',2891550594,191),('CaptureSpecialAbilityUpdateModuleData',825858229,192),('OCLMonitorSpecialAbilityUpdateModuleData',3356383339,193)]
literals={}
for l in s.splitlines():
 m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',l)
 if m:
  try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
  except UnicodeDecodeError:pass
manifest={'sourceBlob':BLOB,'sharedManifests':['../special-power-base-source/manifest.json','../upgrade-source/manifest.json'],'modules':[],'methods':[]};exports={}
def original(sig):
 b=body_of(s,sig)
 if sig not in exports:
  file=f'{len(exports):03}.txt';sha=hashlib.sha256((b+'\n').encode()).hexdigest();used={m:literals[m] for m in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',b) if m in literals};exports[sig]=(file,'// Original blob '+BLOB+'; body SHA256 '+sha+'\n// Literals: '+json.dumps(used,ensure_ascii=False)+'\n'+b+'\n');manifest['methods'].append({'file':file,'signature':sig,'bodySha256':sha})
 return b
def method(name,op):
 sig='internal unsafe static void '+op+'(INode* node, '+name+'* @object, Tracker* state)' if op=='Marshaler_002EMarshal' else 'internal unsafe static void '+op+'('+name+'* @object, EndianHelper* state)' if op=='Marshaler_002EEndianCorrect' else 'internal unsafe static void '+op+'(void** table, '+name+'* @object, Tracker* state)'
 return original(sig) if sig in s else None
lines=['// Complete SpecialAbility Update contracts; original blob '+BLOB+'.']
for group,label,count,fallback in [('SpecialAbilityUpdateOptionsType','Options',31,5),('ConditionsType','Conditions',3,0)]:
 lines+=['const EnumNameValue ability'+label+'Names[] = {']+['    { '+json.dumps(n)+f', {v} }},' for n,v in sorted(tables[group].items())]+['};',f'const AIEnum ability{label} = {{ ability{label}Names, {count}u, {fallback}, {count}u }};']
lines+=['const ModuleField abilityFields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if enum is None else '&'+enum) for n,o,k,d,enum in FIELDS]+['};',f'const ModuleFieldSet abilityFieldSet = {{ abilityFields, {len(FIELDS)}u }};']
for name,typ,kind in SPECS:
 assert inv['marshal'][str(typ)]['nativeSize']==240 and inv['marshal'][str(typ)]['name']==name
 wrappers={op:original(inv[op][str(typ)]['helperSignature']) for op in ['marshal','endian','token']}
 mb=method(name,'Marshaler_002EMarshal') or wrappers['marshal'];eb=method(name,'Marshaler_002EEndianCorrect') or wrappers['endian'];tb=method(name,'Tokenizer_002ETokenize') or wrappers['token']
 if kind==191:
  actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in FIELDS]+[('CustomAnimAndDuration',None)],actual
 else:assert 'Marshaler_002EMarshal(node, (SpecialAbilityUpdateModuleData*)' in mb
 ptr=re.search(r'ProcTableElement\* (ptr\d*) =',tb)[1];chunks=tb.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');records=[]
 for chunk in chunks:
  m=re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);\s*\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',chunk)
  if not m or m[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in m.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk)
  if off:off=int(off[1])
  else:
   narrow=re.search(r'\((uint|int|float|bool|ModelConditionFlagType)\*\)@object \+ (\d+)',chunk);off=int(narrow[2])*(1 if narrow[1]=='bool' else 4) if narrow else 0
  nested=('&spawnUpdateModuleData' if kind==191 else '&abilitySpecialAbilityUpdateModuleData') if key==1 else 'nullptr';mode=0
  if kind==191 and off==232:nested='&upgradeAnimAndDuration';mode=4
  records.append((key,tokenType,off,nested,mode,0))
 offsets=[0]+[r[1] for r in FIELDS]+[232] if kind==191 else [0];assert [r[2] for r in records]==offsets,(name,records,offsets)
 assert records[0][1]==(4043318533 if kind==191 else 2891550594)
 assert re.search(r'\*\(int\*\)'+ptr+r' = 0;\s*\(\(int\*\)'+ptr+r'\)\[1\] = 0;\s*\(\(int\*\)'+ptr+r'\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken ability'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout ability{name} = {{ ability{name}Tokens, {len(records)}u }};']
 manifest['modules'].append({'name':name,'typeId':typ,'kind':kind,'nativeSize':240,'tokenCount':len(records)})
# Keep full new bit helpers and the shared AnimAndDuration contract in this evidence.
for l in s.splitlines():
 sig=l.strip()
 if not sig.startswith('internal unsafe static void '):continue
 if (sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SpecialAbilityUpdateOptionsType_') or sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020ConditionsType_') or ('AnimAndDuration* @object' in sig and ('Marshaler_002EMarshal(' in sig or 'Marshaler_002EEndianCorrect(' in sig or 'Tokenizer_002ETokenize(' in sig)) or ('AnimAndDuration_003E(INode*' in sig) or ('AnimAndDuration_003E(void**' in sig)):original(sig)
lines+=['const ModuleChoice abilityChoices[] = {']+[f'    {{ {t}u, &ability{n}, 240u, {k}u }},' for n,t,k in SPECS]+['};','']
out='\n'.join(lines);p=ROOT/'recovery/src/Native/GameObjectSpecialAbility.h'
if '--check' in sys.argv:
 assert p.read_text()==out;assert json.loads((E/'manifest.json').read_text())==json.loads(json.dumps(manifest))
 for file,text in exports.values():assert (E/file).read_text()==text
else:
 p.write_text(out);E.mkdir(parents=True,exist_ok=True);(E/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 for file,text in exports.values():(E/file).write_text(text)
print('PASS complete SpecialAbility240/Capture/OCLMonitor, '+str(len(FIELDS))+' root fields and '+str(len(exports))+' original method fingerprints'+(' (exported PE enums)' if '--exported-enums' in sys.argv else ' (original PE verified)'))
