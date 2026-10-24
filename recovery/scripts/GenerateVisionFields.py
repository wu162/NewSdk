"""Complete radar/visibility group with Capture and UnifiedControl dependencies."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/vision-source'
CAPTURE=[('GrantMoney',8,'Unsigned','0'),('GrantMoneyOneTime',16,'Boolean','false'),('SlavedCaptureObject',12,'AssetId',None)]
UNIFIED=[('UnificationType',20,'Enum','SAME_PLAYER')]
SPECS=[('CaptureBehaviorModuleData',4173638251,20,CAPTURE,[]),('UnifiedControlBehaviorModuleData',3179117790,24,UNIFIED+CAPTURE,[]),('RadarUpdateModuleData',1768048089,12,[('RadarExtendTime',8,'Float','0')],[]),('UnitVisionBehaviorModuleData',4070192848,12,[('UpdateRate',8,'Signed','1')],[]),('SelectiveVisibilityBehaviorModuleData',1632936356,16,[('UpdateRate',8,'NullableTime',None),('AllowedPlayers',12,'NullableBits',None)],[]),('InvisibilityBehaviorModuleData',88803780,12,[('InvisibilityTemplate',8,'Reference',None)],[]),('InvisibilityBroadcastUpdateModuleData',2984624090,44,[('InvisibilityTemplate',8,'Reference',None),('AffectsSelf',40,'Boolean','true'),('UpdatePeriod',12,'Time','1s'),('BroadcastRange',16,'Float','0'),('ScalableDrawModuleId',20,'AssetId',None),('ScaledBoneName',24,'String',None)],[('BroadcastObjectFilter',None),('ObjectStatusValidation',None)]),('UnifiedShroudRevealBehaviorModuleData',2339238473,24,UNIFIED+CAPTURE,[])]
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==87
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated original radar/visibility/Capture contracts; blob '+BLOB+'.']
    for group,count,unknown,bits in [('UnifiedControlType',2,1,0),('RelationshipType',3,2,3)]:
        entries=enums[group];assert len(entries)==count
        if source is not None:assert enum_values(source,pe,group)==entries
        helper=next(b for sig,b in methods.items()if 'Marshaler_002EMarshal_003Cenum_0020' in sig and '('in sig and group+'_'in sig)
        assert ('? 1 :'in helper)if group=='UnifiedControlType'else ('num4 = 2;'in helper and '*(int*)target = -1;'in helper)
        lines.append('const EnumNameValue vision'+group+'Names[] = {')
        lines+=['    { %s, %d },'%(json.dumps(n),v)for n,v in entries.items()]
        lines+=['};','const AIEnum vision%s = { vision%sNames, %du, %d, %du };'%(group,group,count,unknown,bits)]
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
        if name in ['UnifiedControlBehaviorModuleData','UnifiedShroudRevealBehaviorModuleData']:expected=[('UnificationType','SAME_PLAYER')];assert '(CaptureBehaviorModuleData*)ptr' in body
        assert actual==expected,(name,actual,expected)
        lines.append('const ModuleField vision'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='&visionUnifiedControlType'if k=='Enum'else '&visionRelationshipType'if k=='NullableBits'else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        lines.append('};')
    lines.append('const ModuleFieldSet visionFieldSets[] = {')
    for name,typ,size,fields,children in SPECS:lines.append('    { vision%sFields, %du },'%(name,len(fields)))
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
            if nested:
                child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                if child=='ObjectFilter':child=None;mode=7
                else:emit(child)
            elif 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            else:raise AssertionError((name,part))
            records.append((key,typ,off,child,mode))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken vision'+name+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&vision'+child if child else 'nullptr',mode))
        lines+=['};','const ModuleLayout vision%s = { vision%sTokens, %du };'%(name,name,len(records))];emitted.add(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice visionChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS,61):lines.append('    { %du, &vision%s, %du, %du },'%(typ,name,size,index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectVision.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectVision.h'
    else:path.write_text(output)
    print('PASS eight complete radar/visibility/Capture modules, nullable time/relationship, exact Token inheritance and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
