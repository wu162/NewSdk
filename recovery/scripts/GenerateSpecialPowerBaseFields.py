"""Recover the complete SpecialPower452 parent and both Radar children.
Original methods and enum PE tables are pinned; exported mode is for local QA only.
"""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/special-power-base-source'
BASE=[('SpecialPowerTemplate',8,'Reference',None,None),('UpdateModuleStartsAttack',436,'Boolean','false',None),('StartsPaused',437,'Boolean','false',None),('AntiFX',12,'Reference',None,None),('AttributeModifier',16,'Reference',None,None),('AttributeModifierRange',20,'Float','0.0',None),('AttributeModifierAffectsSelf',438,'Boolean','false',None),('AttributeModifierFX',24,'Reference',None,None),('AttributeModifierWeatherBased',439,'Boolean','false',None),('WeatherDuration',28,'Duration','0',None),('RequirementsFilterMPSkirmish',32,'StringList',None,None),('InitiateFX',40,'Reference',None,None),('TriggerFX',44,'Reference',None,None),('SetModelCondition',48,'Enum',None,'aiModelConditionFlagType'),('SetModelConditionTime',52,'Time','0s',None),('GiveLevels',56,'Signed','0',None),('DisableDuringAnimDuration',440,'Boolean','false',None),('IdleWhenStartingPower',441,'Boolean','false',None),('AffectGood',442,'Boolean','false',None),('AffectEvil',443,'Boolean','false',None),('AffectAllies',444,'Boolean','true',None),('AvailableAtStart',445,'Boolean','true',None),('ChangeWeather',60,'Enum',None,'specialPowerWeather'),('AdjustVictim',446,'Boolean','false',None),('OnTriggerRechargeSpecialPower',64,'AssetId',None,None),('BurnDecayModifier',68,'Unsigned','0',None),('UseDistanceFromCommandCenter',447,'Boolean','false',None),('MaxDistanceFromCommandCenter',72,'Float','0',None),('ChildModuleHandlesFX',448,'Boolean','false',None),('LeechRechargePercentPlayerTechTrigger',76,'Reference',None,None),('LeechRechargePercentFromSpecialPower',80,'Reference',None,None),('DisabledTypesToIgnore',84,'Bits','HELD','specialPowerDisabled'),('CanAffectObjectFilter',88,'Reference','GenericSpecialPowerObjectFilter',None),('ObjectFilterDistType',92,'Enum','CENTER_2D','specialPowerDistance')]
SPECS=[('SpecialPowerModuleData',3472757011,452,162,BASE,0,11),('RadarHackSpecialPowerModuleData',3201673786,464,163,[('NumFalseReturns',452,'Unsigned','0',None),('Radius',456,'Float','0.0',None),('HackDuration',460,'Time','0s',None)],2,12),('RadarJamSpecialPowerModuleData',3870511801,460,164,[('JamRadius',452,'Float','0.0',None),('JamDuration',456,'Time','0s',None)],4,13)]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB and len(manifest['methods'])==78
assert sorted((m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules'])==sorted((n,t,z) for n,t,z,k,f,m,q in SPECS)
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[int(Path(meta['file']).stem)]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
if source is not None:
 from GenerateUpgradeFields import enum_values
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==enums['peSha256']
 for group in ['WeatherType','DisabledType','PartitionManagerDistTestType']:assert enum_values(source,pe,group)==enums[group]
lines=['// Complete SpecialPower parent and Radar contracts; original blob '+BLOB+'.']
for group,label,count,unknown,bits in [('WeatherType','Weather',7,1,0),('DisabledType','Disabled',12,0,12),('PartitionManagerDistTestType','Distance',5,0,0)]:
 assert len(enums[group])==count
 lines+=['const EnumNameValue specialPower'+label+'Names[] = {']+['    { '+json.dumps(n)+f', {v} }},' for n,v in sorted(enums[group].items())]+['};',f'const AIEnum specialPower{label} = {{ specialPower{label}Names, {count}u, {unknown}, {bits}u }};']
for name,typ,size,kind,fields,m,q in SPECS:
 actual=[]
 for line in methods[m].splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 expected=[(n,d) for n,o,k,d,e in fields]+([('AttributeModifierAffects',None),('RequirementsFilterMP',None),('RequirementsFilterStrategic',None),('InitiateSound',None)] if m==0 else [])
 assert actual==expected,(name,actual)
 lines+=['const ModuleField specialPower'+name+'Fields[] = {']
 for n,o,k,d,e in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&'+e)]
 lines+=['};']
lines+=['const ModuleField specialPowerSoundFields[] = {','    { "Sound", 0u, ModuleValue::NullableReference, nullptr, nullptr },','    { "EvaEvent", 4u, ModuleValue::Reference, "", nullptr },','};','const ModuleToken specialPowerSoundTokens[] = {','    { 805758285u, 1675080808u, 0u, nullptr, 2u, 0u },','    { 357806494u, 3303818161u, 4u, nullptr, 0u, 0u },','};','const ModuleLayout specialPowerSound = { specialPowerSoundTokens, 2u };','const ModuleFieldSet specialPowerFieldSets[] = {']+[f'    {{ specialPower{n}Fields, {len(f)}u }},' for n,t,z,k,f,m,q in SPECS]+['};']
for name,typ,size,kind,fields,m,q in SPECS:
 chunks=methods[q].split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk)
  if off:off=int(off[1])
  else:
   narrow=re.search(r'\((uint|float|bool|PartitionManagerDistTestType)\*\)@object \+ (\d+)',chunk);off=int(narrow[2])*(1 if narrow[1]=='bool' else 4) if narrow else 0
  nested=('&dieBehaviorModuleData' if kind==162 else '&specialPowerSpecialPowerModuleData') if key==1 else 'nullptr';mode=0
  if kind==162:
   if off==32:mode=6
   if off in [96,208,320]:mode=8
   if off==432:mode=4;nested='&specialPowerSound'
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==len(fields)+(5 if kind==162 else 1),(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 assert [r[2] for r in records[1:1+len(fields)]]==[f[1] for f in fields]
 lines+=['const ModuleToken specialPower'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout specialPower{name} = {{ specialPower{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice specialPowerChoices[] = {']+[f'    {{ {t}u, &specialPower{n}, {z}u, {k}u }},' for n,t,z,k,f,m,q in SPECS]+['};','']
# Verify the small nested Token contract against the pinned original too.
assert '805758285' in methods[25] and '1675080808' in methods[25] and '357806494' in methods[25] and '-991149135' in methods[25]
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectSpecialPowerBase.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectSpecialPowerBase.h'
else:path.write_text(output)
print('PASS complete SpecialPower452/Radar464/Radar460 contracts, 78 method fingerprints and three PE enums'+(' (exported evidence only)' if source is None else ' (original blob/PE verified)'))
