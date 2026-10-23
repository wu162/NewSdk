"""Original complete remaining four Create-family modules; EVA Create already supported."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import BLOB,ROOT
SPECS=[('GrantUpgradeCreateModuleData',79217712,44,[('UpgradeToGrant',8,'Reference',None),('ExemptStatus',12,'ObjectStatus',None),('GiveOnBuildComplete',40,'Boolean','false')],[]),('InheritUpgradeCreateModuleData',3221293211,132,[('Radius',8,'Float','0')],[('ObjectFilter',None),('Upgrade',None)]),('ExperienceLevelCreateModuleData',3270227277,16,[('Level',8,'Signed','0'),('MPOnly',12,'Boolean','false')],[]),('ActivateSpecialPowerOnCreate',3310015739,16,[],[('SpecialPowerTemplate',None)])]
def generate(source):
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated from original source blob '+BLOB+'; four complete create Create modules.']
    allfields=[(n,f,c)for n,t,s,f,c in SPECS]
    for name,fields,children in allfields:
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state'+(', VectorHack* tag)' if name=='Vector3' else ')')
        if sig not in source:sig='internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(INode* node, ModuleData** pointer, Tracker* tracker)'
        body=body_of(source,sig);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        if fields:
            lines.append('const ModuleField create'+name+'Fields[] = {')
            for n,o,k,d in fields:
                values='nullptr'
                lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
            lines.append('};')
    lines.append('const ModuleFieldSet createFieldSets[] = {')
    for n,f,c in allfields:lines.append('    { %s, %du },'%('create'+n+'Fields'if f else 'nullptr',len(f)))
    lines.append('};')
    emitted={'BehaviorModuleData','UpdateModuleData'}
    def layout(n):return ('lifecycle'if n in ['BehaviorModuleData','UpdateModuleData']else 'create')+n
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
            elif 'TypedAssetId_003Cstruct'in part or 'AssetReference_003Cstruct_0020SageBinaryData_003A_003ASpecialPowerTemplate'in part:mode=3;stride=4
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
        lines.append('const ModuleToken create'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&'+layout(child)if child else 'nullptr',mode,stride))
        lines.extend(['};','const ModuleLayout create'+name+' = { create'+name+'Tokens, '+str(len(records))+'u };']);emitted.add(name)
    for n,*_ in SPECS:emit(n)
    lines.append('const ModuleChoice createChoices[] = {')
    for kind,(n,t,s,f,c)in enumerate(SPECS,55):lines.append('    { %du, &create%s, %du, %du },'%(t,n,s,kind))
    lines.extend(['};','']);return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    output=generate(source);path=ROOT/'recovery/src/Native/GameObjectCreate.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectCreate.h'
    else:path.write_text(output)
    print('PASS four complete Create original field/Token/dispatch contracts')
