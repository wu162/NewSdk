"""Derive the complete shared DieMux/Die-module family from original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
def generate(source):
    specs=[('DieModuleData',2655988594,52,[]),('DestroyDieModuleData',259966977,52,[]),('UpgradeDieModuleData',1359837639,56,[('UpgradeId',52,'Reference',None)]),('KeepObjectDieModuleData',3491013058,60,[('CollapsingTime',52,'Time','0s'),('StayOnRadar',56,'Boolean','false')]),('CreateObjectDieModuleData',996718440,72,[('CreationList',52,'Reference',None)]),('CrushDieModuleData',2448240860,76,[(n,52+4*i,'Reference',None) for i,n in enumerate(['TotalCrushSound','BackEndCrushSound','FrontEndCrushSound'])]+[(n,64+4*i,'Percent','100') for i,n in enumerate(['TotalCrushSoundPercent','BackEndCrushSoundPercent','FrontEndCrushSoundPercent'])]),('RefundDieModuleData',3787260944,72,[('UpgradeRequired',52,'Reference',None),('BuildingRequired',56,'StringList',None),('RefundPercent',64,'Percent','0')])]
    mux=[('VeterancyLevels',0,'StringList',None),('ExemptStatus',8,'NullableObjectStatus',None),('RequiredStatus',12,'NullableObjectStatus',None),('DamageAmountRequired',16,'Float','-1'),('MinKillerAngle',20,'Angle','1d'),('MaxKillerAngle',24,'Angle','-1d'),('DeathTypes',28,'DeathBits',None),('DeathTypesForbidden',36,'DeathBits',None)]
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated from original source blob '+BLOB+'; shared DieMux and seven complete concrete dispatches.']
    for name,fields in [(a,d) for a,b,c,d in specs]+[('DieMuxDataType',mux)]:
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if sig not in source:sig='internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(INode* node, ModuleData** pointer, Tracker* tracker)'
        body=body_of(source,sig);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
        expected=[(n,f) for n,o,k,f in fields]
        if name=='DieModuleData':expected.append(('DieMuxData',None))
        if name=='CreateObjectDieModuleData':expected.extend([('UpgradeRequired',None),('UpgradeForbidden',None)])
        if name=='RefundDieModuleData':expected.append(('ObjectFilter',None))
        assert actual==expected,(name,actual,expected)
        if fields:
            lines.append('const ModuleField die'+name+'Fields[] = {')
            for n,off,kind,fallback in fields:lines.append('    { '+json.dumps(n)+', '+str(off)+'u, ModuleValue::'+kind+', '+('nullptr' if fallback is None else json.dumps(fallback))+', nullptr },')
            lines.append('};')
    lines.append('const ModuleFieldSet dieFieldSets[] = {')
    for name,fields in [(a,d) for a,b,c,d in specs]+[('DieMuxDataType',mux)]:lines.append('    { '+('die'+name+'Fields' if fields else 'nullptr')+', '+str(len(fields))+'u },')
    lines.append('};')
    emitted=set()
    def emit(name):
        nonlocal lines
        if name in emitted:return
        sig='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)';body=body_of(source,sig)
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body)
        if not push:
            names=re.findall(r'Tokenizer_002ETokenize\(table, \((\w+)\*\)@object, state\)',body);assert len(names)==1,(name,body);emit(names[0]);lines.append('const ModuleLayout die'+name+' = die'+names[0]+';');emitted.add(name);return
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1]) if off else 0
            mode=0;child=None;stride=0
            if 'String_003Cchar_003E_0020_003E(' in part:mode=6;stride=8
            elif 'AssetReference_003Cstruct_0020SageBinaryData_003A_003AUpgradeTemplate' in part:mode=3;stride=4
            else:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
                if nested:
                    child=nested[1];mode=4 if ')(int)' in part else 0
                    if child=='ObjectFilter':mode=7;child=None
                    else:emit(child)
                elif re.search(r'BookmarkMemberOffset\(state, \(void\*\)\(int\)',part):mode=2
                else:assert 'BookmarkMemberOffset(state,' in part,(name,part)
            records.append((key,typ,off,child,mode,stride))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken die'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&die'+child if child else 'nullptr',mode,stride))
        lines+=['};','const ModuleLayout die'+name+' = { die'+name+'Tokens, '+str(len(records))+'u };'];emitted.add(name)
    for name,*_ in specs:emit(name)
    lines.append('const ModuleChoice dieChoices[] = {')
    for kind,(name,typeid,size,fields) in enumerate(specs,23):lines.append('    { %du, &die%s, %du, %du },'%(typeid,name,size,kind))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    output=generate(source);path=ROOT/'recovery/src/Native/GameObjectDie.h'
    if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectDie.h'
    else:path.write_text(output)
    print('PASS seven complete Die-family source field/Token/dispatch contracts including inline DieMux')
