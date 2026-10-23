"""Original-source contracts for eleven remaining named Upgrade derivations."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
# Root fields are in original Marshal order, not memory order.
SPECS=[('SubStructureBuildUpgrade',829862040,52,[],[('AvailableSubStructure',None),('StructureOffset',None)]),
('ModelConditionUpgrade',1306352767,164,[('AddConditionFlags',36,'ModelCondition',None),('RemoveConditionFlags',96,'ModelCondition',None),('AddTempConditionFlag',156,'Enum',None),('TempConditionTime',160,'Time','0.0s')],[]),
('ReplaceSelfUpgrade',1409001909,52,[('NewObjectUnpackTime',36,'Time','0s'),('Options',40,'ReplaceOptions','DISABLE_DURING_UNPACK')],[('ReplacementTemplate',None)]),
('SubObjectsUpgrade',1594276481,44,[],[('Nugget',None)]),
('ObjectCreationUpgrade',1776471404,72,[('UpgradeObject',36,'Reference',None),('Delay',40,'Unsigned','0'),('RemoveUpgrade',44,'Reference',None),('GrantUpgrade',48,'Reference',None),('ThingToSpawn',52,'AssetId',None),('Angle',56,'Angle',None),('DestroyWhenSold',68,'Boolean','false'),('FadeInTime',60,'Unsigned','0'),('UseBuildingProduction',69,'Boolean','false')],[('Offset',None)]),
('RemoveUpgradeUpgrade',1973457077,56,[('UpradesGroupsToRemove',36,'StringList',None),('SuppressEvaEventForRemoval',52,'Boolean','false'),('RemoveFromAllPlayerObjects',53,'Boolean','false')],[('UpgradeToRemove',None)]),
('StatusBitsUpgrade',2020381784,92,[('StatusToSet',36,'ObjectStatus',None),('StatusToClear',64,'ObjectStatus',None)],[]),
('UnpauseSpecialPowerUpgrade',1072475067,44,[('SpecialPowerTemplate',36,'Reference',None),('ObeyRechageOnTrigger',40,'Boolean','false')],[]),
('CostModifierUpgrade',3451973623,68,[('Percentages',36,'FloatList',None),('IsUpgradeDiscountUpgrade',64,'Boolean','false'),('StartsActive',65,'Boolean','false'),('Slaughter',66,'Boolean','false'),('UILabel',44,'String',None)],[('ObjectFilter',None),('ApplyToUpgrade',None)]),
('TechUpgrade',4286303667,168,[('Radius',36,'Float',None),('RadiusY',40,'NullableFloat',None),('DistanceType',44,'Enum','CIRCULAR'),('FXLineBoneName',48,'String',None)],[('ObjectFilter',None)]),
('GeometryUpgrade',4286905310,52,[('ShowGeometry',36,'StringList',None),('HideGeometry',44,'StringList',None)],[])]
NESTED=[('SubObjectsUpgradeNugget',[('ShowSubObjects',0,'StringList',None),('HideSubObjects',8,'StringList',None),('FadeTime',16,'Time','0.5s'),('WaitBeforeFadeTime',20,'Time','0.0s'),('SkipFadeOnCreate',40,'Boolean','false'),('HideSubObjectsOnRemove',41,'Boolean','false'),('UnHideSubObjectsOnRemove',42,'Boolean','false'),('Upgrade',24,'AssetId',None)],[('UpgradeTexture',None),('CustomAnimAndDuration',None)]),('ReplaceTexture',[('Original',0,'Reference',None),('New',4,'Reference',None)],[]),('OffsetInfo',[('ModelCondition',0,'ModelCondition',None)],[('Offset',None)]),('AvailableSubStructure',[],[('UpgradeTrigger',None),('ObjectTemplate',None)]),('Vector3',[(a,4*i,'Float','0.0')for i,a in enumerate('xyz')],[])]
def generate(source,enums):
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated from original source blob '+BLOB+'; eleven complete Upgrade derivations.']
    for group,count,fallback,bits in [('DistanceCheckType',2,0,0),('ReplaceSelfUpgradeOptions',6,5,5)]:
        entries=enums[group];assert len(entries)==count,(group,entries)
        lines.append('const EnumNameValue remaining'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { '+json.dumps(n)+', '+str(v)+' },')
        lines+=['};','const AIEnum remaining'+group+' = { remaining'+group+'Names, '+str(count)+'u, '+str(fallback)+', '+str(bits)+'u };']
    allfields=[(n+'ModuleData',f,c)for n,t,s,f,c in SPECS]+NESTED
    for name,fields,children in allfields:
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state'+(', VectorHack* tag)' if name=='Vector3' else ')')
        if sig not in source:sig='internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(INode* node, ModuleData** pointer, Tracker* tracker)'
        body=body_of(source,sig);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        if fields:
            lines.append('const ModuleField remaining'+name+'Fields[] = {')
            for n,o,k,d in fields:
                values='&aiModelConditionFlagType'if n=='AddTempConditionFlag'else '&remainingDistanceCheckType'if n=='DistanceType'else '&remainingReplaceSelfUpgradeOptions'if k=='ReplaceOptions'else 'nullptr'
                lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
            lines.append('};')
    lines.append('const ModuleFieldSet remainingFieldSets[] = {')
    for n,f,c in allfields:lines.append('    { %s, %du },'%('remaining'+n+'Fields'if f else 'nullptr',len(f)))
    lines.append('};')
    emitted={'UpgradeModuleData','AnimAndDuration'}
    def layout(n):return ('upgrade'if n in ['UpgradeModuleData','AnimAndDuration']else 'remaining')+n
    def emit(name):
        if name in emitted:return
        body=body_of(source,'internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)')
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0;mode=0;child=None;stride=0
            if 'String_003Cchar_003E_0020_003E('in part or 'Relo_003A_003AAssetId_003E('in part:mode=6;stride=8
            elif 'TypedAssetId_003Cstruct'in part or 'Tokenize_003Cfloat_003E'in part:mode=3;stride=4
            elif 'List_003CSageBinaryData'in part:
                child=re.search(r'List_003CSageBinaryData_003A_003A(\w+?)_003E',part)[1];mode=1;stride={'AvailableSubStructure':8,'OffsetInfo':72,'SubObjectsUpgradeNugget':44,'ReplaceTexture':8}[child];emit(child)
                helper=body_of(source,'internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020SageBinaryData_003A_003A'+child+'_003E(void** table, List_003CSageBinaryData_003A_003A'+child+'_003E* list, Tracker* tracker)');assert '1820487795' in helper and '-1320050453' in helper
            else:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
                if nested:
                    child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                    if child=='ObjectFilter':mode=7 if mode==4 else 8;child=None
                    else:emit(child)
                elif re.search(r'BookmarkMemberOffset\(state, \(void\*\)\(int\)',part):mode=2
                else:assert 'BookmarkMemberOffset(state,'in part,(name,part)
            records.append((key,typ,off,child,mode,stride))
        assert all(re.search(p,parts[-1])for p in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']),name
        lines.append('const ModuleToken remaining'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&'+layout(child)if child else 'nullptr',mode,stride))
        lines.extend(['};','const ModuleLayout remaining'+name+' = { remaining'+name+'Tokens, '+str(len(records))+'u };']);emitted.add(name)
    for n,*_ in SPECS:emit(n+'ModuleData')
    lines.append('const ModuleChoice remainingChoices[] = {')
    for kind,(n,t,s,f,c)in enumerate(SPECS,41):lines.append('    { %du, &remaining%sModuleData, %du, %du },'%(t,n,s,kind))
    lines.extend(['};','']);return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();enums={g:enum_values(source,pe,g)for g in ['DistanceCheckType','ReplaceSelfUpgradeOptions']}
    output=generate(source,enums);path=ROOT/'recovery/src/Native/GameObjectUpgradeRemaining.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectUpgradeRemaining.h'
    else:path.write_text(output)
    print('PASS eleven remaining Upgrade original field/Token/enum contracts')
