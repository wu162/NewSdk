"""Derive six complete lifecycle Update layouts from original methods and PE maps."""
import hashlib,json,re,struct,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
def body_of(source,signature):
    start=source.index(signature);opening=source.index('{',start);depth=1;end=opening+1
    while depth:depth+=(source[end]=='{')-(source[end]=='}');end+=1
    return source[start:end]
def generate(source,death):
    specs=[
      ('DeletionUpdateModuleData',1111529555,16,[('MinFrames',8,'Unsigned','0'),('MaxFrames',12,'Unsigned','1')]),
      ('DieOnProducerDeathUpdateModuleData',474181074,16,[('SleepInterval',8,'Time','0.5s'),('DieOnProducerDisabled',12,'Boolean','false')]),
      ('InvalidTargetsDeathUpdateModuleData',1461180892,12,[('InvalidStatus',8,'NullableObjectStatus',None)]),
      ('HeightDieUpdateModuleData',1964506706,20,[('TargetHeight',8,'Float',None),('TargetHeightIncludesStructures',16,'Boolean','false'),('OnlyWhenMovingDown',17,'Boolean','false'),('SnapToGroundOnDeath',18,'Boolean','false'),('InitialDelay',12,'Duration','0')]),
      ('LoseHealthUpdateModuleData',2974418213,24,[('HealthTrigger',8,'Percent','10%'),('DamagePerSecond',12,'Float','100'),('DeathType',16,'Enum','NONE')]),
      ('LifetimeUpdateModuleData',1814576242,44,[('MinLifetime',8,'Time','0s'),('MaxLifetime',12,'Time','0s'),('WaitForWakeUp',40,'Boolean','false'),('ScoreKill',41,'Boolean','false'),('DeathType',16,'Enum',None),('IgnoreProjectileState',42,'Boolean',None),('CountdownSound',20,'Reference',None),('CountdownCompleteSound',24,'Reference',None),('TimeBeforeDeathThatCountdownCompletes',28,'Time','0s'),('TimeBetweenCountdownSounds',32,'Time','1s'),('NumberOfCountdownSoundPlays',36,'NullableSigned',None)])]
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated lifecycle Update contracts; original source blob '+BLOB+'.', 'const EnumNameValue lifecycleDeathNames[] = {']
    assert len(death)==34 and sorted(death.values())==list(range(34))
    lines+=['    { '+json.dumps(n)+', '+str(v)+' },' for n,v in death.items()]
    lines+=['};','const AIEnum lifecycleDeath = { lifecycleDeathNames, 34u, 33, 0u };']
    for name,typeid,size,fields in specs:
        signature='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if signature not in source:
            signature='internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003A'+name+'_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E(INode* node, ModuleData** pointer, Tracker* tracker)'
        b=body_of(source,signature);actual=[]
        for line in b.splitlines():
            symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
        expected=[(n,fallback) for n,off,kind,fallback in fields]
        if name=='LoseHealthUpdateModuleData':expected.append(('ObjectStatusValidation',None))
        assert actual==expected,(name,actual,expected)
        lines.append('const ModuleField lifecycle'+name+'Fields[] = {')
        for n,off,kind,fallback in fields:
            lines.append('    { '+json.dumps(n)+', '+str(off)+'u, ModuleValue::'+kind+', '+('nullptr' if fallback is None else json.dumps(fallback))+', '+('&lifecycleDeath' if kind=='Enum' else 'nullptr')+' },')
        lines.append('};')
    lines+=['const ModuleField lifecycleValidationFields[] = {','    { "ForbiddenStatus", 0u, ModuleValue::ObjectStatus, nullptr, nullptr },','    { "RequiredStatus", 28u, ModuleValue::ObjectStatus, nullptr, nullptr },','};','const ModuleFieldSet lifecycleFieldSets[] = {']
    for name,typeid,size,fields in specs:lines.append('    { lifecycle'+name+'Fields, '+str(len(fields))+'u },')
    lines+=['    { lifecycleValidationFields, 2u },','};']
    emitted=set()
    def emit(name):
        nonlocal lines
        if name in emitted:return
        signature='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
        b=body_of(source,signature)
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',b)
        assert push,name
        ptr=push[1];parts=b.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);')
        assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff
            typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1]) if off else 0
            child=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
            mode=0;nested=None
            if child:
                nested=child[1];mode=4 if ')(int)' in part else 0;emit(nested)
            elif re.search(r'BookmarkMemberOffset\(state, \(void\*\)\(int\)',part):mode=2
            else:assert 'BookmarkMemberOffset(state,' in part,(name,part)
            records.append((key,typ,off,nested,mode))
        for word in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(word,parts[-1]),name
        lines.append('const ModuleToken lifecycle'+name+'Tokens[] = {')
        for key,typ,off,nested,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&lifecycle'+nested if nested else 'nullptr',mode))
        lines+=['};','const ModuleLayout lifecycle'+name+' = { lifecycle'+name+'Tokens, '+str(len(records))+'u };']
        emitted.add(name)
    for name,*_ in specs:emit(name)
    lines.append('const ModuleChoice lifecycleChoices[] = {')
    for kind,(name,typeid,size,fields) in enumerate(specs,17):lines.append('    { %du, &lifecycle%s, %du, %du },'%(typeid,name,size,kind))
    lines+=['};','']
    return '\n'.join(lines)

def original_death_enum(source):
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();po=struct.unpack_from('<I',pe,60)[0];ib=struct.unpack_from('<I',pe,po+52)[0];sb=po+24+struct.unpack_from('<H',pe,po+20)[0]
    def string_at(va):
        for i in range(struct.unpack_from('<H',pe,po+6)[0]):
            _,size,base,rs,off=struct.unpack_from('<8sIIII',pe,sb+40*i)
            if base<=va-ib<base+max(size,rs):
                start=off+va-ib-base;return pe[start:pe.index(b'\0',start)].decode('ascii')
        raise ValueError(hex(va))
    line=next(l for l in source.splitlines() if 'internal static' in l and 'enumMap' in l and 'W4DeathType_0040' in l)
    return {string_at(p):v for p,v in struct.iter_unpack('<Ii',bytes.fromhex(re.search(r'data\(([^)]+)',line)[1]))}
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    death=original_death_enum(source);output=generate(source,death);path=ROOT/'recovery/src/Native/GameObjectLifecycle.h'
    if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectLifecycle.h'
    else:path.write_text(output)
    print('PASS six original lifecycle Update field/Token/enum/dispatch layouts')
