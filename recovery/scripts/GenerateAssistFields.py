"""Complete four-type assistance/placement group from fixed original contracts."""
import hashlib,json,re,sys
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/assist-source'
SPECS=[
('AssistedPlacementInfoBehaviorModuleData',36,[('AllowShrouded',32,'Boolean','true')],[('HintObject',None),('PlacementRestriction',None),('SuggestedOffset',None)]),
('AssistedPlacementBehaviorModuleData',20,[('PlacementHint',8,'Bits','NONE'),('SnappingDistance',12,'Float','0')],[('RequirementObjectFilter',None)]),
('AttackAssistResponseBehaviorModuleData',44,[('ObjectStatus',8,'ObjectStatus',''),('WeaponSlotID',36,'Unsigned',None),('WeaponSlotType',40,'Enum',None)],[]),
('SpecialPowerAssistResponseBehaviorModuleData',40,[('SpecialPowerTemplate',8,'Reference',None),('ObjectStatus',12,'ObjectStatus','')],[]),
('HintObjectMapping',20,[('PlayerTemplate',0,'NullableAssetId',None),('HintDisplayObject',4,'AssetId',None),('HintOpacity',8,'Float','1.0'),('HintUsesShaders',16,'Boolean','false')],[('ObjectFilter',None)]),
('PlacementRestriction',8,[('FilterRadius',0,'Float','0.0')],[('RestrictedObjectFilter',None)]),
('Vector3',12,[('x',0,'Float','0.0'),('y',4,'Float','0.0'),('z',8,'Float','0.0')],[])]

EXTERNAL={'ModuleData':'reactiveModuleData','BehaviorModuleData':'reactiveBehaviorModuleData','UpdateModuleData':'lifecycleUpdateModuleData'}
def ident(n):return 'assist'+n

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    metas=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in metas:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(metas)==63
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    if source is not None:
        for group,entries in enums.items():
            if group!='sourceBlob':assert enum_values(source,pe,group)==entries,group
    hintBody=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AAssistanceHintType'))
    assert re.search(r'else\s*\{\s*num4 = 3;',hintBody)
    slotBody=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020WeaponSlotType_002Cclass_'))
    assert '((ptr == null) ? 5 :'in slotBody
    lines=['// Generated complete four-type assistance/placement contracts; original blob '+BLOB+'.']
    for group,unknown,bits in [('AssistanceHintType',3,4),('WeaponSlotType',5,0)]:
        lines.append('const EnumNameValue assist'+group+'Names[] = {')
        for n,v in enums[group].items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum assist%sEnum = { assist%sNames, %du, %d, %du };'%(group,group,len(enums[group]),unknown,bits)]
    for index,(name,size,fields,children)in enumerate(SPECS):
        if index<4:
            row=manifest['modules'][index];assert(row['name'],row['nativeSize'])==(name,size)
        body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object,'));actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        lines.append('const ModuleField '+ident(name)+'Fields[] = {')
        for n,o,k,d in fields:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&assistWeaponSlotTypeEnum'if k=='Enum'else '&assistAssistanceHintTypeEnum'if k=='Bits'else 'nullptr'))
        lines.append('};')
    lines.append('const ModuleFieldSet assistFieldSets[] = {')
    for name,size,fields,children in SPECS:lines.append('    { %sFields, %du },'%(ident(name),len(fields)))
    lines.append('};');emitted=set()
    def emit(name):
        if name in EXTERNAL:return EXTERNAL[name]
        if name in emitted:return ident(name)
        body=methods['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)']
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name;records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0;mode=0;child=None;stride=0
            if 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            elif 'List_003C'in part:
                assert 'HintObjectMapping'in part;child='HintObjectMapping';mode=1;stride=20
            else:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \(([\w.]+)\*\)',part);assert nested,(name,part);child=nested[1]
                if '('+child+'*)(int)'in part:mode=4
                if child=='ObjectFilter':child=None;mode=7 if mode==4 else 8
            records.append((key,typ,off,emit(child)if child else None,mode,stride))
        spec=next((x for x in SPECS if x[0]==name),None)
        if spec:assert set(f[1]for f in spec[2])<=set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken '+ident(name)+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&'+child if child else 'nullptr',mode,stride))
        lines.extend(['};','const ModuleLayout %s = { %sTokens, %du };'%(ident(name),ident(name),len(records))]);emitted.add(name);return ident(name)
    for name,*_ in SPECS[:6]:emit(name)
    lines.append('const ModuleChoice assistChoices[] = {')
    for index,row in enumerate(manifest['modules'],119):lines.append('    { %du, &%s, %du, %du },'%(row['typeId'],ident(row['name']),row['nativeSize'],index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectAssist.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAssist.h'
    else:path.write_text(output)
    print('PASS complete four-type assistance/placement contracts and 63 original method fingerprints'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
