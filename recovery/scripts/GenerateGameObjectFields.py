"""Generate readable root metadata from original source and PE; no buffer snapshots."""
import json, re, struct, sys, hashlib, math
from pathlib import Path
import xml.etree.ElementTree as ET
from ExtractGameObjectEvidence import method, token_table

root = Path(__file__).resolve().parents[2]
s = (root/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
# Require the exact characterized original module.
# Git for Windows checks out CRLF; hash the canonical LF source, not checkout EOL.
raw = s.encode('utf-8')
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest() == '6e40a9ed6c9f961d294d49768d911e6a6f354619'
marshal, first = method(s, 'internal unsafe static void Marshaler_002EMarshal(INode* node, GameObject* @object, Tracker* state)')
token_body, token_first = method(s, 'internal unsafe static void Tokenizer_002ETokenize(void** table, GameObject* @object, Tracker* state)')
tokens = token_table(token_body, token_first)
token_parts = token_body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')
schema = ET.parse(root/'SDK/Schemas/xsd/AssetTypeGameObject.xsd')
names = {n.attrib['name'] for n in schema.iter() if 'name' in n.attrib}

def decode(symbol):
    text = symbol.split('_0040')[-2].removesuffix('_003F_0024AA')
    return text.replace('_003F4', '.').replace('_003F9', '-')

pe = (root/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
po = struct.unpack_from('<I', pe, 60)[0]
ib = struct.unpack_from('<I', pe, po+52)[0]
sb = po+24+struct.unpack_from('<H', pe, po+20)[0]
def string_at(va):
    for i in range(struct.unpack_from('<H', pe, po+6)[0]):
        _,size,base,rs,off = struct.unpack_from('<8sIIII', pe, sb+40*i)
        if base <= va-ib < base+max(size,rs):
            start = off+va-ib-base
            return pe[start:pe.index(b'\0',start)].decode('ascii')
    raise ValueError(hex(va))

def enum_values(group):
    line = next(l for l in s.splitlines() if 'internal static' in l and 'enumMap' in l and 'W4'+group+'_0040' in l)
    return {string_at(p):v for p,v in struct.iter_unpack('<Ii',bytes.fromhex(re.search(r'data\(([^)]+)',line)[1]))}

fields=[]
enums=[]
enum_fallbacks=[]
for index,line in enumerate(marshal.splitlines()):
    # Direct root scalar/string/resource attributes; lists, masks and blocks remain unsupported.
    m=re.search(r'(Marshaler_002EMarshal_\w+)\(',line)
    if not m or 'Unsafe.AsPointer' not in line: continue
    helper=m[1]
    typ=re.search(r'\((float|int|uint|ushort|bool|byte|\w+Type|UnitCategory|WeaponCategory)\*\)@object \+ (\d+), state',line)
    string='Relo_003A_003AString_003Cchar_003E_002Cclass' in helper
    resource=re.search(r'\((TypedAssetId_\w+|AssetReference_\w+)\*\)\(\(byte\*\)@object \+ (\d+)\)',line)
    if not typ and not string and not resource: continue
    symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
    prefix=decode(symbols[0])
    candidates=[prefix] if prefix in names else [n for n in names if n.startswith(prefix)]
    if len(candidates)>1 and prefix=='EvaEventSecondDamageFarFromFirst':
        candidates=[n for n in candidates if n.endswith('TimeoutMS' if 'DurationTag' in helper else 'ScanRange')]
    if len(candidates)!=1: raise ValueError((prefix,candidates))
    name=candidates[0]
    fallback=decode(symbols[1]) if len(symbols)>1 else None
    if resource:
        offset=int(resource[2]);kind='AssetId' if resource[1].startswith('TypedAssetId_') else 'Reference';default=None
    elif string:
        offset=int(re.search(r'\(byte\*\)@object \+ (\d+)\), state',line)[1]);kind='String';default=None
    else:
        t=typ[1];offset=int(typ[2])*({'ushort':2,'bool':1,'byte':1}.get(t,4))
        if t=='float': kind='Time' if 'TimeTag' in helper else 'Angle' if 'AngleTag' in helper else 'Float'
        else: kind={'int':'Signed','uint':'Unsigned','ushort':'Short','bool':'Boolean','byte':'Byte'}.get(t,'Enum')
        default=None
        if kind=='Enum':
            group='BuildPlacementType' if 'BuildPlacementType' in helper else t
            entries=enum_values(group)
            default=entries[fallback]
            original,_=method(s,'internal unsafe static void '+helper+'(')
            unknown=int(re.search(r'ptr == null\) \? (-?\d+)',original)[1])
            enum_fallbacks.append((name,unknown))
            enums.extend((name,n,v) for n,v in entries.items())
        if 'DurationTag' in helper:
            kind='DurationDefault'
            constant=next(l for l in s.splitlines() if 'internal static float DataInitializers_002E_003FA0xf4a5029c_002ELOGICFRAMES_PER_MSEC_REAL' in l)
            scale=struct.unpack('<f',bytes.fromhex(re.search(r'data\(([^)]+)',constant)[1]))[0]
            single=lambda x:struct.unpack('<f',struct.pack('<f',x))[0]
            default=math.ceil(single(scale*single(float(fallback))))
    fields.append((name,offset,kind,fallback,default,first+index))
# Browser uses the shared whitespace tokenizer at original line 12996;
# its root lookup/Marshal is split across statements (50082-50085).
assert 'Browser_003F_0024AA' in marshal and '((byte*)@object + 40)' in marshal
fields.insert(0,('Browser',40,'StringList',None,None,50082))
fields.insert(0,('KindOf',4,'Bits',None,None,50079))
fields.append(('BuildOnRequiredObjectKindOf',312,'Bits',None,None,50167))
assert len(fields)==88, len(fields)
lines=['// Generated from original Marshal/Tokenize and PE enum maps. See GenerateGameObjectFields.py.', 'const GameObjectField fields[] = {']
for n,o,k,f,d,l in fields:
    lines.append('    { '+json.dumps(n)+', '+str(o)+'u, GameObjectValue::'+k+', '+('nullptr' if f is None else json.dumps(f))+', '+str(d or 0)+' }, // original line '+str(l))
lines+=['};','const EnumNameValue kindOfEntries[] = {']
for name,value in enum_values('KindOfType').items():
    lines.append('    { '+json.dumps(name)+', '+str(value)+' },')
# Child enums use the same original PE maps and exact-name lookup.
for field, group, fallback in [('Buildable','BuildableStatus',1),('ThingClass','ThingClassType',15)]:
    enums.extend((field,n,v) for n,v in enum_values(group).items())
    enum_fallbacks.append((field,fallback))
lines+=['};','const EnumNameValue armorSetEntries[] = {']
for name,value in enum_values('ArmorSetType').items():
    lines.append('    { '+json.dumps(name)+', '+str(value)+' },')
enums.extend(('LocomotorSet',n,v) for n,v in enum_values('LocomotorSetType').items())
enum_fallbacks.append(('LocomotorSet',12))
enums.extend(('ShadowInfo',n,v) for n,v in enum_values('ShadowType').items())
enum_fallbacks.append(('ShadowInfo',3))
lines+=['};','const GameObjectEnum enumEntries[] = {']
enums.extend(('ExtraCrushLevels',n,v) for n,v in enum_values('ModelConditionFlagType').items())
enum_fallbacks.append(('ExtraCrushLevels',233)) # original scalar helper 52035
enums.extend(('GeometryShapeType',n,v) for n,v in enum_values('GeometryType').items())
enum_fallbacks.append(('GeometryShapeType',1)) # original scalar helper 57939
enums.extend(('ContactPointGeneration',n,v) for n,v in enum_values('ContactPointGenerationType').items())
enum_fallbacks.append(('ContactPointGeneration',3)) # original scalar helper 57950
for field,typ,fallback in [('AudioArraySound','ThingTemplateSoundType',31),('EvaEventArray','ThingTemplateEvaEventType',11),('AudioVoiceEntry','ThingTemplateVoiceType',32),('AudioObjectSpecificVoiceEntry','ThingTemplateObjectSpecificVoiceType',0)]:
    enums.extend((field,n,v) for n,v in enum_values(typ).items())
    enum_fallbacks.append((field,fallback))
for field,name,value in enums:
    lines.append('    { '+json.dumps(field)+', '+json.dumps(name)+', '+str(value)+' },')
lines+=['};','const GameObjectEnumFallback enumFallbacks[] = {']
for field,value in enum_fallbacks:
    lines.append('    { '+json.dumps(field)+', '+str(value)+' },')
lines+=['};','const GameObjectToken tokens[] = {']
list_offsets={40,412,420,444,472,536,560,568}
for t in tokens[:-1]:
    kind='List' if t['nativeOffset'] in list_offsets else 'Member' if t['kind']=='member-bookmark' else 'Null'
    if kind=='Null' and t['helpers']:
        h=t['helpers'][0]
        call=next(l for l in token_parts[t['index']].splitlines() if h+'(' in l)
        cast=re.search(r'ptr \+ 8\), \((\w+\*+)\)',call)
        if not cast: raise ValueError('Unparsed helper argument: '+call)
        # Resolve the exact overload and accept only a one-statement bookmark body.
        direct=r'internal unsafe static void '+re.escape(h)+r'\(void\*\* \w+, '+re.escape(cast[1])+r' [@\w]+, Tracker\* \w+\)\s*\{\s*Relo_002ETracker_002EBookmarkMemberOffset\([^;]+\);\s*\}'
        if re.search(direct,s): kind='Member'
    lines.append('    { '+str(t['nameHash'])+'u, '+str(t['typeHash'])+'u, '+str(t['nativeOffset'])+'u, GameObjectTokenKind::'+kind+' },')
lines+=['};','']
# Non-polymorphic visual leaf layouts, transcribed from original Marshal
# methods 14189, 49185, 49243; tokens are extracted independently below.
leaf_specs = {
    'FormationPreviewDecal': (16,[('Texture',0,'String',None),('Width',8,'Float','50.0'),('Height',12,'Float','50.0')]),
    'ShadowInfo': (60,[('Type',0,'Enum',None),('Texture',4,'Reference',None),('AdditionalTexture',8,'Reference',None),
        ('SizeX',12,'Float','0.0'),('SizeY',16,'Float','0.0'),('OffsetX',20,'Float','0.0'),('OffsetY',24,'Float','0.0'),
        ('OpacityStart',28,'Float','0'),('OpacityFadeInTime',32,'Time','0s'),('OpacityPeak',36,'Float','1'),
        ('OpacityFadeOutTime',40,'Time','0s'),('OpacityEnd',44,'Float','0'),('MaxHeight',48,'Float','20'),
        ('OverrideLODVisibility',56,'Boolean','false'),('UseHouseColor',57,'Boolean','false'),
        ('IsRotatingWithObject',58,'Boolean','true'),('LocalPlayerOnly',59,'Boolean','false'),('SunAngle',52,'Float','0.0')]),
    'AutoResolveInfo': (36,[('AutoResolveUnitType',0,'String',None),('AutoResolveBody',8,'AssetId',None),
        ('AutoResolveArmor',12,'Reference',None),('AutoResolveWeapon',16,'Reference',None),
        ('AutoResolveLeadership',20,'String',None),('AutoResolveCombatChain',28,'String',None)]),
    'VisionInfo': (40,[('VisionRange',0,'Float','0.00'),('ShroudClearingRange',4,'Float','0.0'),('ShroudClearingHeight',8,'Float','50.0'),
        ('VisionSide',12,'Percent','100'),('VisionRear',16,'Percent','100'),('VisionBonusTestRadius',20,'Float','100'),
        ('VisionBonusTestSegments',24,'Unsigned','8'),('VisionBonusPercentPerFoot',28,'Percent','0'),
        ('MaxVisionBonusPercent',32,'Percent','50'),('MinVisionBonusPercent',36,'Percent','-50')])
}
for name,(size,entries) in leaf_specs.items():
    original, first = method(s,'internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)')
    symbols=[decode(x) for x in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',original)]
    assert set(n for n,o,k,f in entries).issubset(symbols), (name,symbols)
    lines+=['const LeafField '+name+'Fields[] = {']
    for n,o,k,f in entries:
        lines.append('    { '+json.dumps(n)+', '+str(o)+'u, LeafValue::'+k+', '+('nullptr' if f is None else json.dumps(f))+' },')
    lines+=['};','const LeafToken '+name+'Tokens[] = {']
    token_body, token_line = method(s,'internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)')
    parsed=[]
    for part in token_body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]:
        key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',part)[1])&0xffffffff
        typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        bookmark=re.search(r'BookmarkMemberOffset\(state, (?:\(byte\*\))?@object(?: \+ (\d+))?,',part)
        assert bookmark, part
        off=int(bookmark[1] or 0);parsed.append(off)
        lines.append('    { '+str(key)+'u, '+str(typ)+'u, '+str(off)+'u },')
    assert parsed==[o for n,o,k,f in entries], (name,parsed)
    assert '12u, '+str(len(entries)+1)+'u' in token_body
    lines+=['};']
lines+=['const LeafLayout leafLayouts[] = {']
for name,typ,off in [('FormationPreviewDecal','FormationPreviewDecal',512),('FormationPreviewItemDecal','FormationPreviewDecal',516),('ShadowInfo','ShadowInfo',544),('AutoResolveInfo','AutoResolveInfo',548),('VisionInfo','VisionInfo',552)]:
    size,entries=leaf_specs[typ]
    lines.append('    { '+json.dumps(name)+', '+str(off)+'u, '+str(size)+'u, '+typ+'Fields, '+str(len(entries))+'u, '+typ+'Tokens },')
lines+=['};','']
# Complete raw Geometry dependency group. Keys/types come from the original
# Tokenize methods; offsets are separately transcribed from native layouts.
for typ,offsets in [('Geometry',[24,0,4,8,16]),('Shape',[0,4,8,44,45,16,20,24,32,36,40]),('ContactPoint',[0,8]),('AudioArraySound',[0]),('EvaEventArray',[0]),('AudioSoundEntry',[0,4]),('EvaEventEntry',[0,4]),('AudioArrayVoice',[0,8,16]),('AudioVoiceEntry',[0,8]),('AudioObjectSpecificVoiceEntry',[0,8,12]),('AudioVoiceReferentialEntry',[0,8]),('SoundOrEvaEvent',[0,4])]:
    body,first=method(s,'internal unsafe static void Tokenizer_002ETokenize(void** table, '+typ+'* @object, Tracker* state)')
    parts=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]
    assert len(parts)==len(offsets)
    lines+=['const LeafToken '+typ+'Proc[] = {']
    for part,offset in zip(parts,offsets):
        key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',part)[1])&0xffffffff
        value=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        direct=re.search(r'BookmarkMemberOffset\(state, (?:\(byte\*\))?@object(?: \+ (\d+))?,',part)
        if direct: assert int(direct[1] or 0)==offset
        lines.append('    { '+str(key)+'u, '+str(value)+'u, '+str(offset)+'u },')
    lines+=['};','']
# Integrated into the existing source-derived generator; field order is original Marshal order.
ai_specs = {
'SkirmishAIInformation': (88, [
('BaseBuildingLocation',0,'Enum','FRONT','SkirmishAIBaseLocation'),
('AdditionalRotation',4,'Angle','0r',None),('BaseBuildingMaxNudges',8,'Signed','3',None),
('BlockIfUnpackTargetBuildingCannotBeRebuilt',80,'Boolean','true',None),('UnitBuilderStandardCombatUnit',81,'Boolean','false',None),('TargetDuringCrush',82,'Boolean','false',None),
('CrushSpeedBonus',12,'Float','0.0',None),('OutOfRangePenalty',16,'Float','1.3',None),('ContainedMembersProvideWeapon',83,'Boolean','false',None),
('ConquerMetricsOverrideDPS',20,'NullableFloat',None,None),('ConquerMetricsOverrideDamageType',24,'Enum','UNDEFINED','DamageType'),
('ConquerMetricsOverrideAntiMask',28,'Bits','ANTI_GROUND ANTI_WATER','WpnAntiT'),('ConquerMetricsOverrideForbiddenAntiMask',32,'Bits','','WpnAntiT'),('ConquerMetricsOverrideDefendingAntiMask',36,'NullableBits',None,'WpnAntiT'),
('ConquerMetricsDpsBonusAgainstDefenses',40,'Float','1.0',None),('NearResourceNodeType',44,'Enum','ORE','EconomyResourceType'),('DodgeSafetyDistance',48,'Float','300.0',None),
('OverrideInnerDistanceToTarget',52,'NullableFloat',None,None),('OverrideOuterDistanceToTarget',56,'NullableFloat',None,None),('CanFireUpCliffs',84,'Boolean','false',None),('DoesSecondaryDamage',85,'Boolean','false',None)], [60,68,72,76]),
'AISpecialPowerInfo':(172,[
('Heuristic',0,'Enum',None,'AISpecialPowerHeuristic'),('Manager',4,'Enum',None,'AISpecialPowerManager'),('Distance',8,'Float','100.0',None),('HealthFraction',12,'Float','0.0',None),
('TargetObjectInclude',16,'Bits',None,'KindOfType'),('TargetObjectExclude',52,'Bits',None,'KindOfType'),('UseOnlyWhenPowerAdvantageIsAbove',88,'NullableFloat',None,None),
('NeverUseWhileRetreating',164,'Boolean','false',None),('NeverUseWhilePullingBack',165,'Boolean','false',None),
('InvulnerabilityDamageLookAheadTime',92,'Time','1s',None),('InvulnerabilityDamageLookBehindTime',96,'Time','1s',None),('InvulnerabilityDamageFractionWhenOutOfRangeOfTarget',100,'Float','.3',None),('InvulnerabilityDamageFractionWhenInRangeOfTarget',104,'Float','.6',None),
('InvulnerabilityAvoidUseIfAngleToNextPathNodeIsMoreThan',108,'Angle','360d',None),('MaxTargetSpeed',112,'Float','99999.0',None),('RequireKillTargets',166,'Boolean','true',None),
('MinKillValue',116,'Float','0.0',None),('MinTargetsHit',120,'Signed','1',None),('MinEnemiesNearby',124,'Signed','1',None),('NextLogicCommand',128,'NullableReference',None,None),('UseInMicroManagerState',132,'Bits','','MicroManagerState'),
('UseNearTacticalTarget',167,'Boolean','false',None),('UseWhenOutOfAmmo',168,'Boolean','false',None),('UpdateObject',136,'NullableReference',None,None),('SearchRadius',140,'Float','150.0',None),('Location',144,'Bits','Water Land','AITargetLocationType'),('MaxUpdatables',148,'NullableSigned',None,None),('UseToCounterGarrison',152,'NullableBoolean',None,None)], [156,160]),
'AIBaseSelectionCriteria':(20,[(n,i*4,'Bits','','AIBaseType') for i,n in enumerate(['RequiredBaseTypes','ExcludedBaseTypes','PreferredBaseTypes','PreferredNotBaseTypes'])]+[('SortOrder',16,'Enum','PREFER_NEWEST','AIBaseSortingOrder')],[]),
'AISpecialPowerWeaponData':(28,[('Weapon',0,'Reference',None,None),('IsDisablingAttack',24,'Boolean','false',None),('AffectsFriendlies',25,'Boolean','false',None),('AffectsSelf',26,'Boolean','false',None),('TotalValueGreaterThan',4,'Signed','0',None),('AoeAttackPowerLocationPreference',8,'Enum','PREFER_NEAREST','AIAoeAttackPowerLocationPreference'),('AoeAttackPowerBlockAreaAfterActivationTime',12,'Time','0s',None),('AoeAttackEmanationLocation',16,'Enum','EMANATE_FROM_SELF','AIAoeAttackPowerEmanationLocation'),('ToggleRangeHysteresis',20,'Float','0.0',None),('ToggleShortRangeTargetForcesTarget',27,'Boolean','false',None)],[])
}
ai_enums=[('SkirmishAIBaseLocation',2,0),('DamageType',38,0),('EconomyResourceType',3,0),('AISpecialPowerHeuristic',4,0),('AISpecialPowerManager',3,0),('AIBaseSortingOrder',6,0),('AIAoeAttackPowerLocationPreference',1,0),('AIAoeAttackPowerEmanationLocation',0,0),('WpnAntiT',7,14),('MicroManagerState',0,10),('AITargetLocationType',1,2),('AIBaseType',2,10),('KindOfType',149,284),('ModelConditionFlagType',233,0)]
for group,unknown,bits in ai_enums:
    entries=enum_values(group)
    lines+=['const EnumNameValue ai'+group+'Names[] = {']+[ '    { '+json.dumps(n)+', '+str(v)+' },' for n,v in entries.items()]+['};']
    lines+=['const AIEnum ai'+group+' = { ai'+group+'Names, '+str(len(entries))+'u, '+str(unknown)+', '+str(bits)+'u };']
    # Confirm scalar unknowns against the actual original helper.
    if not bits:
        helpers=[l for l in s.splitlines() if 'internal unsafe static void Marshaler_002EMarshal_003Cenum_0020' in l and '(IValue* value,' in l and ('_003A_003A'+group+'_002C' in l or '_0020'+group+'_002C' in l)]
        assert len(helpers)==1,(group,helpers)
        helper_body,_=method(s,helpers[0].strip())
        match=re.search(r'ptr == null\) \? (-?\d+)',helper_body)
        assert (int(match[1])==unknown if match else unknown==0 and 'ptr != null' in helper_body),(group,helper_body)
# Resolve complete literal strings from original data, rather than trusting
# truncated decorated names or XSD defaults.
ai_literals={}
for line in s.splitlines():
    m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
    if m:
        try: ai_literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
        except UnicodeDecodeError: pass
for typ,(size,entries,child_offsets) in ai_specs.items():
    body,first=method(s,'internal unsafe static void Marshaler_002EMarshal(INode* node, '+typ+'* @object, Tracker* state)')
    # Every attribute is an actual lookup in the original, including truncated names.
    prefixes=[decode(x) for l in body.splitlines() if 'Unsafe.AsPointer' in l for x in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)[:1]]
    assert all(any(n.startswith(p) for p in prefixes) for n,o,k,f,g in entries)
    lookups=[]
    for line in body.splitlines():
        symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if not symbols: continue
        literal=ai_literals.get(symbols[0])
        if literal in {e[0] for e in entries}:
            lookups.append((literal,ai_literals[symbols[1]] if len(symbols)>1 else None))
    assert lookups==[(n,f) for n,o,k,f,g in entries],(typ,lookups)

    lines+=['const AIField '+typ+'AIFields[] = {']
    for n,o,k,f,g in entries:
        lines.append('    { '+json.dumps(n)+', '+str(o)+'u, AIValue::'+k+', '+('nullptr' if f is None else json.dumps(f))+', '+('nullptr' if g is None else '&ai'+g)+' },')
    lines+=['};']
    body,first=method(s,'internal unsafe static void Tokenizer_002ETokenize(void** table, '+typ+'* @object, Tracker* state)')
    parts=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]
    offsets=[e[1] for e in entries]+child_offsets
    assert len(parts)==len(offsets) and ('12u, '+str(len(parts)+1)+'u') in body
    lines+=['const LeafToken '+typ+'AIProc[] = {']
    for part,offset in zip(parts,offsets):
        key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',part)[1])&0xffffffff
        value=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        direct=re.search(r'(?:\(byte\*\)@object|\(byte\*\)@object) \+ (\d+)',part)
        if direct: assert int(direct[1])==offset,(typ,offset,part)
        elif offset: raise ValueError((typ,offset,part))
        lines.append('    { '+str(key)+'u, '+str(value)+'u, '+str(offset)+'u },')
    lines+=['};']
lines+=['const AILayout aiLayouts[] = {']
for typ,(size,entries,children) in ai_specs.items():
    lines+=['    { '+str(size)+'u, '+typ+'AIFields, '+str(len(entries))+'u, '+typ+'AIProc, '+str(len(entries)+len(children))+'u },']
lines+=['};','']

body_specs={
'ActiveBodyModuleData':[
('MaxHealth',8,'Float','0',None),('MaxHealthDamagedPercentage',12,'Percent','66.666666',None),('MaxHealthReallyDamagedPercentage',16,'Percent','33.333333',None),('InitialHealth',20,'Float','-1.0',None),('DodgePercent',24,'Float','0',None),('EnteringDamagedTransitionTime',28,'Time','0s',None),('EnteringReallyDamagedTransitionTime',32,'Time','0s',None),('GrabObject',36,'String',None,None),('HealingBuffFX',44,'Reference',None,None),('DamagedAttributeModifier',48,'Reference',None,None),('ReallyDamagedAttributeModifier',52,'Reference',None,None),('CheerRadius',56,'Float','200.0',None),('RemoveUpgradesOnDeath',72,'Boolean','false',None),('BurningDeathBehavior',73,'Boolean','false',None),('BurningDeathFX',60,'Reference',None,None)],
'ShieldBodyModuleData':[
('ShieldEnabledFX',76,'Reference',None,None),('ShieldDepleteFX',80,'Reference',None,None),('ShieldRechargeEndFX',84,'Reference',None,None),('ShieldTakeDamageFX',88,'Reference',None,None),('ShieldAmount',92,'Float',None,None),('ShieldArmor',96,'NullableReference',None,None),('ShieldRechargeIdleTime',100,'Time','0s',None),('ShieldActiveModelCondition',104,'Enum','INVALID','ModelConditionFlagType')],
'ActiveBodyDamageRangeModifier':[
('Min',0,'Percent',None,None),('Max',4,'Percent',None,None),('AttributeModifier',8,'Reference',None,None)]}
client_names=['BeaconClientUpdateModuleData','EvaAnnounceClientCreateModuleData','EvaAnnounceSpecialPowerReadyClientUpdateModuleData','LaserUpdateModuleData','RadarMarkerClientUpdateModuleData','LargeGroupAudioClientUpdateModuleData']
client_specs={
'BeaconClientUpdateModuleData':[('FramesBetweenRadarPulses',8,'Unsigned','0',None),('RadarPulseDuration',12,'Unsigned','0',None)],
'EvaAnnounceClientCreateModuleData':[(n,8+4*i,'Reference','',None) for i,n in enumerate(['AnnouncementEventEnemy','AnnouncementEventAlly','AnnouncementEventOwner'])]+[('DelayBeforeAnnouncementLogicFrames',20,'Unsigned','0',None)]+[(n,24+i,'Boolean','false',None) for i,n in enumerate(['OnlyIfVisible','CountAsFirstSightedAnnouncement','UseObjectsPosition','CreateFakeRadarEvent'])],
'EvaAnnounceSpecialPowerReadyClientUpdateModuleData':[(n,8+4*i,'Reference','',None) for i,n in enumerate(['AnnouncementEventEnemy','AnnouncementEventAlly','AnnouncementEventOwner'])]+[('SpecialPowerTemplate',20,'Reference',None,None),('JustMonitorPercentReady',24,'Boolean','false',None),('CheckPlayerPowerVisibility',25,'Boolean','false',None)],
'LaserUpdateModuleData':[('ParticleSystemId',8,'Reference',None,None),('ParentFireBoneName',12,'String',None,None),('ParentFireBoneOnTurret',28,'Boolean','false',None),('TargetParticleSystemId',20,'Reference',None,None),('LaserLifetime',24,'Float','0',None)],
'RadarMarkerClientUpdateModuleData':[('TypeName',8,'String',None,None)],
'LargeGroupAudioClientUpdateModuleData':[('FramesBetweenUpdatesMin',8,'Unsigned','3',None),('FramesBetweenUpdatesVariation',12,'Unsigned','2',None),('UnitWeight',24,'Short','1',None)]}
body_specs.update(client_specs)

for typ,entries in body_specs.items():
    body,first=method(s,'internal unsafe static void Marshaler_002EMarshal(INode* node, '+typ+'* @object, Tracker* state)')
    lookups=[]
    for line in body.splitlines():
        symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if symbols: lookups.append((ai_literals[symbols[0]],ai_literals[symbols[1]] if len(symbols)>1 else None))
    expected=[(n,f) for n,o,k,f,g in entries]
    if typ=='ActiveBodyModuleData': expected.append(('DamageRangeModifier',None))
    if typ=='LargeGroupAudioClientUpdateModuleData': expected.append(('Key',None))
    assert lookups==expected,(typ,lookups,expected)
    lines+=['const ModuleField '+typ+'ModuleFields[] = {']
    for n,o,k,f,g in entries:
        lines+=['    { '+json.dumps(n)+', '+str(o)+'u, ModuleValue::'+k+', '+('nullptr' if f is None else json.dumps(f))+', '+('nullptr' if g is None else '&ai'+g)+' },']
    lines+=['};']

lines+=['const ModuleFieldSet moduleFieldSets[] = {','    { nullptr, 0u },']
for typ in ['ActiveBodyModuleData','ShieldBodyModuleData']+client_names:
    lines+=['    { '+typ+'ModuleFields, '+str(len(body_specs[typ]))+'u },']
lines+=['};']
# Full module dispatch inventory is validated separately and reused here.
import runpy
module_source=runpy.run_path(str(root/'recovery/scripts/ExtractGameObjectModuleEvidence.py'))
module_inventory=module_source['report']
module_choices=[]
def emit_module_graph(graph):
    global lines
    if 'forwardTo' in graph: return emit_module_graph(graph['forwardTo'])
    records=[]
    for item in graph['records']:
        nested=emit_module_graph(item['nested']) if 'nested' in item else None
        records.append((item['nameHash'],item['typeHash'],item.get('memberOffset',0),nested,{'list':1,'nullable-member':2,'hash-list':3}.get(item.get('mode'),0)))
    name='moduleTable'+str(len(module_choices))
    module_choices.append(name)
    lines.append('const ModuleToken '+name+'Tokens[] = {')
    for key,typ,off,nested,mode in records:
        lines.append('    { '+str(key)+'u, '+str(typ)+'u, '+str(off)+'u, '+('&'+nested if nested else 'nullptr')+', '+str(mode)+'u },')
    lines+=['};','const ModuleLayout '+name+' = { '+name+'Tokens, '+str(len(records))+'u };']
    return name
module_rows=[]
for entry in module_inventory['baseOnlyCandidates']:
    module_rows.append((entry['typeId'],emit_module_graph(entry['tokenGraph']),8,0))
for entry in module_inventory['bodyCandidates']:
    module_rows.append((entry['typeId'],emit_module_graph(entry['tokenGraph']),entry['nativeSize'],2 if entry['name']=='ShieldBodyModuleData' else 1))
for kind,entry in enumerate(module_inventory['clientUpdateCandidates'],3):
    module_rows.append((entry['typeId'],emit_module_graph(entry['tokenGraph']),entry['nativeSize'],kind))
lines+=['const ModuleChoice moduleChoices[] = {']
for value,table,size,kind in module_rows: lines+=['    { '+str(value)+'u, &'+table+', '+str(size)+'u, '+str(kind)+'u },']
lines+=['};','const U32 knownModuleTypes[] = {']
lines+=['    '+str(key)+'u,' for key in module_inventory['marshal'] if key!='default']
lines+=['};','const ModuleChildName moduleChildNames[] = {']
containers={'Draws':'DrawModules','Behaviors':'BehaviorModules','AI':'AIModule','Body':'BodyModule','ClientUpdates':'ClientUpdateModules','ClientBehaviors':'ClientBehaviorModules'}
for name,typ in containers.items():
    node=next(n for n in schema.getroot() if n.attrib.get('name')==typ)
    for child in node.iter('{http://www.w3.org/2001/XMLSchema}element'):
        lines+=['    { '+json.dumps(name)+', '+json.dumps(child.attrib['name'])+' },']
lines+=['};','']

output='\n'.join(lines)
path=root/'recovery/src/Native/GameObjectFields.h'
if '--check' in sys.argv:
    assert path.read_text()==output, 'Regenerate GameObjectFields.h'
else: path.write_text(output)
print('PASS source-derived GameObject metadata: '+str(len(fields))+' root fields, 126 tokens; no frozen buffers')



