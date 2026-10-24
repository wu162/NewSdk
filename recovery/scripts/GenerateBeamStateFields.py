"""Complete LaserState/SweepingLaserState/StreamState original contracts."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/beam-state-source'
SPECS=[('LaserStateModuleData',3887509261,44,[('LaserId',8,'Signed','0'),('OriginBoneName',12,'String',None)],[('LaserEndParticleSystem',None),('LaserStartParticleSystem',None),('EndOffset',None),('ObjectStatusValidation',None)]),('SweepingLaserStateModuleData',1016285011,64,[('Radius',44,'Float','10.0'),('SweepFXList',48,'Reference',None),('VeteranSweepFXList',52,'Reference',None),('SweepFXTimeout',56,'Time','0s'),('SweepWeapon',60,'Reference',None)],[]),('StreamStateModuleData',4277690261,12,[('StreamId',8,'Signed','0')],[])]
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==38
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated original beam state contracts; blob '+BLOB+'.']
    entries=enums['ObjectStatusType'];assert len(entries)==223
    if source is not None:assert enum_values(source,pe,'ObjectStatusType')==entries
    def marshal(name):
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if sig not in methods:sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_')and '_003A_003A'+name+'_002Cstruct_'in sig)
        return methods[sig]
    for name,typ,size,fields,children in SPECS:
        row=next(x for x in manifest['modules']if x['name']==name);assert (row['typeId'],row['nativeSize'])==(typ,size)
        body=marshal(name);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        expected=[(n,d)for n,o,k,d in fields]+children
        assert actual==expected,(name,actual,expected)
        lines.append('const ModuleField beam'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        lines.append('};')
    lines.append('const ModuleFieldSet beamFieldSets[] = {')
    for name,typ,size,fields,children in SPECS:lines.append('    { beam%sFields, %du },'%(name,len(fields)))
    lines.append('};');emitted=set()
    def emit(name):
        nonlocal lines
        if name in emitted:return
        sig='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
        if sig in methods:body=methods[sig]
        else:
            sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Tokenizer_002ETokenizePolymorphicType_')and '_003A_003A'+name+'_002Cstruct_'in sig)
            body=methods[sig];obj=re.search(re.escape(name)+r'\* (\w+) = \('+re.escape(name)+r'\*\)\(int\)',body)[1];body=re.sub(r'\b'+obj+r'\b','@object',body)
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0;mode=0;child=None
            nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
            if 'List_003C'in part:
                assert 'FXParticleSystemTemplate'in part;mode=3
                helper=next(b for sig,b in methods.items()if sig.startswith('internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020Relo_')and '(void** table, List_'in sig);assert '1820487795'in helper and '-1320050453'in helper and 'num * 4'in helper
            elif nested:
                child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                if child=='ObjectFilter':child=None;mode=7
                else:emit(child)
            elif 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            else:raise AssertionError((name,part))
            records.append((key,typ,off,child,mode))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken beam'+name+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&beam'+child if child else 'nullptr',mode))
        lines+=['};','const ModuleLayout beam%s = { beam%sTokens, %du };'%(name,name,len(records))];emitted.add(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice beamChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS,77):lines.append('    { %du, &beam%s, %du, %du },'%(typ,name,size,index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectBeamState.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectBeamState.h'
    else:path.write_text(output)
    print('PASS three complete beam state modules, strong-reference lists, exact Token inheritance and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
