"""Original complete ProductionUpdate and both production exit contracts."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
SPECS=[('ProductionUpdate',1858556985,84,[('MaxQueueEntries',8,'Unsigned','99'),('NumDoorAnimations',12,'Signed','0'),('DoorOpeningTime',16,'Time','0s'),('DoorWaitOpenTime',20,'Time','0s'),('DoorCloseTime',24,'Time','0s'),('ConstructionCompleteDuration',28,'Unsigned','0'),('DisabledTypesToProcess',32,'ReplaceOptions',None),('GiveNoXP',80,'Boolean','false'),('UnitInvulnerableTime',36,'Unsigned','0'),('SpecialPrepModelconditionTime',40,'Unsigned','0'),('VeteranUnitsFromVeteranFactory',81,'Boolean','false'),('BonusForType',44,'AssetId',None),('SecondaryQueue',82,'Boolean','false'),('Type',48,'Enum','INVALID'),('IgnorePreReqs',83,'Boolean','false'),('ProductionTimeDelayScalar',52,'Time','0s'),('NewObjectFadeInTime',56,'Time','0s')],[('QuantityModifier',None),('ProductionModifier',None),('GiveUpgradeToProduced',None)]),('DefaultProductionExitUpdate',1785063229,16,[],[('UnitCreatePoint',None),('NaturalRallyPoint',None)]),('QueueProductionExitUpdate',2422471532,48,[('PlacementViewAngle',8,'Angle',None),('ExitDelaySeconds',12,'Time','0s'),('AllowAirborneCreation',44,'Boolean','false'),('InitialBurst',16,'Unsigned','0'),('NoExitPath',45,'Boolean','false'),('CanRallyToSlaughter',46,'Boolean','false'),('ClearAlliesFromDestination',47,'Boolean','true')],[('UnitCreatePoint',None),('NaturalRallyPoint',None)])]
NESTED=[('QuantityModifier',[('Name',0,'AssetId',None),('Count',4,'Signed',None)],[]),('ProductionModifier',[('RequiredUpgrade',0,'Reference',None),('CostMultiplier',4,'Float','1.0'),('TimeMultiplier',8,'Float','1.0')],[('ModifierFilter',None)]),('UpgradeTranslation',[('RequiredUpgrade',0,'Reference',None),('ProvidedUpgrade',4,'Reference',None)],[])]
def generate(source,enums):
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated from original source blob '+BLOB+'; complete Production Update and exit family.']
    for group,count,fallback,bits in [('ProductionQueueType',8,4,0),('DisabledType',12,0,12)]:
        entries=enums[group];assert len(entries)==count,(group,entries)
        lines.append('const EnumNameValue production'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { '+json.dumps(n)+', '+str(v)+' },')
        lines+=['};','const AIEnum production'+group+' = { production'+group+'Names, '+str(count)+'u, '+str(fallback)+', '+str(bits)+'u };']
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
            lines.append('const ModuleField production'+name+'Fields[] = {')
            for n,o,k,d in fields:
                values='&productionProductionQueueType'if n=='Type'else '&productionDisabledType'if k=='ReplaceOptions'else 'nullptr'
                lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
            lines.append('};')
    lines.append('const ModuleFieldSet productionFieldSets[] = {')
    for n,f,c in allfields:lines.append('    { %s, %du },'%('production'+n+'Fields'if f else 'nullptr',len(f)))
    lines.append('};')
    emitted={'UpdateModuleData'}
    def layout(n):return ('lifecycle'if n=='UpdateModuleData'else 'production')+n
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
                child=re.search(r'List_003CSageBinaryData_003A_003A(\w+?)_003E',part)[1];mode=1;stride={'QuantityModifier':8,'ProductionModifier':124,'UpgradeTranslation':8}[child];emit(child)
                helper=body_of(source,'internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020SageBinaryData_003A_003A'+child+'_003E(void** table, List_003CSageBinaryData_003A_003A'+child+'_003E* list, Tracker* tracker)');assert '1820487795' in helper and '-1320050453' in helper
            elif 'Tokenize_003Cstruct_0020SageBinaryData_003A_003AGiveUpgradeToProduced_003E'in part:child='GiveUpgradeToProduced';mode=4;emit(child)
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
        lines.append('const ModuleToken production'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&'+layout(child)if child else 'nullptr',mode,stride))
        lines.extend(['};','const ModuleLayout production'+name+' = { production'+name+'Tokens, '+str(len(records))+'u };']);emitted.add(name)
    for n,*_ in SPECS:emit(n+'ModuleData')
    lines.append('const ModuleChoice productionChoices[] = {')
    for kind,(n,t,s,f,c)in enumerate(SPECS,52):lines.append('    { %du, &production%sModuleData, %du, %du },'%(t,n,s,kind))
    lines.extend(['};','']);return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();enums={g:enum_values(source,pe,g)for g in ['ProductionQueueType','DisabledType']}
    output=generate(source,enums);path=ROOT/'recovery/src/Native/GameObjectProduction.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectProduction.h'
    else:path.write_text(output)
    print('PASS three complete Production Update/exit original field/Token/enum contracts')
