"""Original source/PE-derived complete Upgrade base and ten concrete derivations."""
import hashlib,json,re,struct,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
def enum_values(source,pe,group):
    po=struct.unpack_from('<I',pe,60)[0];ib=struct.unpack_from('<I',pe,po+52)[0];sb=po+24+struct.unpack_from('<H',pe,po+20)[0]
    def string_at(va):
        for i in range(struct.unpack_from('<H',pe,po+6)[0]):
            _,size,base,rs,off=struct.unpack_from('<8sIIII',pe,sb+40*i)
            if base<=va-ib<base+max(size,rs):
                at=off+va-ib-base;return pe[at:pe.index(b'\0',at)].decode('ascii')
        raise ValueError(hex(va))
    line=next(x for x in source.splitlines() if 'internal static' in x and 'enumMap' in x and 'W4'+group+'_0040' in x)
    return {string_at(p):v for p,v in struct.iter_unpack('<Ii',bytes.fromhex(re.search(r'data\(([^)]+)',line)[1]))}
def generate(source,enums):
    base=[(n,28+i,'Boolean','true' if i==4 else 'false',None) for i,n in enumerate(['RequiresAllTriggers','RequiresAllConflictingTriggers','Permanent','WaypointMode','WaypointModeTerminal'])]
    specs=[('UpgradeModuleData',2925571736,36,base),('PowerUpgradeModuleData',3282447464,36,[]),('DelayedUpgradeModuleData',1518063315,40,[('DelayTime',36,'Unsigned','0',None)]),('AttributeModifierUpgradeModuleData',1956793413,40,[('AttributeModifier',36,'Reference',None,None)]),('InvisibilityUpgradeModuleData',1220819118,40,[('InvisibilityTemplate',36,'Reference',None,None)]),('CommandSetUpgradeModuleData',2125984281,40,[('CommandSet',36,'AssetId',None,None)]),('ExperienceScalarUpgradeModuleData',3765090183,40,[('AddXPScalar',36,'Float','0',None)]),('MaxHealthUpgradeModuleData',2090477367,44,[('AddMaxHealth',36,'Float','0.0',None),('ChangeType',40,'Enum',None,'MaxHealthChangeType')]),('ArmorUpgradeModuleData',2717792172,44,[('KillArmorUpgrade',40,'Boolean','false',None),('IgnoreArmorUpgrade',41,'Boolean','false',None),('ArmorSetFlag',36,'Enum',None,'ArmorSetType')]),('LevelUpUpgradeModuleData',2541561249,48,[('LevelsToGain',36,'Signed','0',None),('LevelCap',40,'Signed','0',None),('DoFlash',44,'Boolean','true',None)]),('AudioLoopUpgradeModuleData',585970648,48,[('SoundToPlay',36,'Reference',None,None),('KillAfterMS',40,'Unsigned',None,None),('KillOnDeath',44,'Boolean',None,None)])]
    anim=[('AnimState',0,'Enum',None,'ModelConditionFlagType'),('Frames',4,'Unsigned','0',None),('Trigger',8,'Unsigned','0',None)]
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated original Upgrade contracts; source blob '+BLOB+'.']
    for group,count,fallback in [('ArmorSetType',23,12),('MaxHealthChangeType',3,2)]:
        entries=enums[group];assert len(entries)==count
        signature='internal unsafe static void Marshaler_002EMarshal_003Cenum_0020'+group+'_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, '+group+'* @object, Tracker* state)'
        body=body_of(source,signature);assert '? '+str(fallback)+' :' in body
        lines.append('const EnumNameValue upgrade'+group+'Names[] = {')
        for name,value in entries.items():lines.append('    { '+json.dumps(name)+', '+str(value)+' },')
        lines+=['};','const AIEnum upgrade'+group+' = { upgrade'+group+'Names, '+str(count)+'u, '+str(fallback)+', 0u };']
    for name,fields in [(a,d) for a,b,c,d in specs]+[('AnimAndDuration',anim)]:
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if sig not in source:sig='internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(INode* node, ModuleData** pointer, Tracker* tracker)'
        body=body_of(source,sig);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
        expected=[(n,f) for n,o,k,f,g in fields]
        if name=='UpgradeModuleData':expected.extend([('CustomAnimAndDuration',None),('TriggeredBy',None),('ConflictsWith',None)])
        assert actual==expected,(name,actual,expected)
        if fields:
            lines.append('const ModuleField upgrade'+name+'Fields[] = {')
            for n,off,kind,fallback,group in fields:
                enum='&aiModelConditionFlagType' if group=='ModelConditionFlagType' else '&upgrade'+group if group else 'nullptr'
                lines.append('    { '+json.dumps(n)+', '+str(off)+'u, ModuleValue::'+kind+', '+('nullptr' if fallback is None else json.dumps(fallback))+', '+enum+' },')
            lines.append('};')
    lines.append('const ModuleFieldSet upgradeFieldSets[] = {')
    for name,fields in [(a,d) for a,b,c,d in specs]+[('AnimAndDuration',anim)]:lines.append('    { '+('upgrade'+name+'Fields' if fields else 'nullptr')+', '+str(len(fields))+'u },')
    lines.append('};')
    emitted=set()
    def emit(name):
        nonlocal lines
        if name in emitted:return
        sig='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
        if sig in source:body=body_of(source,sig)
        else:
            sig='internal unsafe static void Tokenizer_002ETokenizePolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(void** table, ModuleData** pointer, Tracker* tracker)'
            body=body_of(source,sig);obj=re.search(re.escape(name)+r'\* (\w+) = \('+re.escape(name)+r'\*\)\(int\)',body)[1];body=re.sub(r'\b'+obj+r'\b','@object',body)
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1]) if off else 0;mode=0;child=None;stride=0
            if 'TypedAssetId_003Cstruct_0020SageBinaryData_003A_003AUpgradeTemplate' in part:mode=3;stride=4
            else:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
                if nested:
                    child=nested[1];mode=4 if '('+child+'*)(int)' in part else 0;emit(child)
                else:assert 'BookmarkMemberOffset(state,' in part,(name,part)
            records.append((key,typ,off,child,mode,stride))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken upgrade'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&upgrade'+child if child else 'nullptr',mode,stride))
        lines+=['};','const ModuleLayout upgrade'+name+' = { upgrade'+name+'Tokens, '+str(len(records))+'u };'];emitted.add(name)
    for name,*_ in specs:emit(name)
    lines.append('const ModuleChoice upgradeChoices[] = {')
    for kind,(name,typeid,size,fields) in enumerate(specs,30):lines.append('    { %du, &upgrade%s, %du, %du },'%(typeid,name,size,kind))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();enums={g:enum_values(source,pe,g) for g in ['ArmorSetType','MaxHealthChangeType']}
    output=generate(source,enums);path=ROOT/'recovery/src/Native/GameObjectUpgrade.h'
    if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectUpgrade.h'
    else:path.write_text(output)
    print('PASS eleven complete original Upgrade field/Token/enum/dispatch contracts')
