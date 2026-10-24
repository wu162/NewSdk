"""Complete DamageState and reactive weapon contracts; WeaponSet is implemented separately by GenerateWeaponSetFields.py."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/weapon-state-source'
SPECS=[
('DamageStateListModuleData',4161248261,16,[],[('Data',None)]),
('FireWeaponWhenDamagedBehaviorModuleData',144160085,80,[('InitiallyActive',76,'Boolean','false'),('DamageTypes',36,'Unsigned',None),('DamageAmount',40,'Float','0')]+[(n,44+4*i,'Reference',None)for i,n in enumerate(['ReactionWeaponPristine','ReactionWeaponDamaged','ReactionWeaponReallyDamaged','ReactionWeaponRubble','ContinuousWeaponPristine','ContinuousWeaponDamaged','ContinuousWeaponReallyDamaged','ContinuousWeaponRubble'])],[]),
('FireWeaponCollideModuleData',2924124080,80,[('CollideWeapon',8,'Reference',None),('FireOnce',76,'Boolean','false'),('Flags',12,'ReplaceOptions','NONE'),('RequiredStatus',16,'ObjectStatus',None),('ForbiddenStatus',44,'ObjectStatus',None),('WhereToFire',72,'Enum','TARGET_OBJECT')],[]),
('FireWeaponWhenDeadBehaviorModuleData',2429465756,144,[('InitiallyActive',136,'Boolean','false'),('ActiveDuringConstruction',137,'Boolean','false'),('ContinueToBezierDestination',138,'Boolean','false'),('FireAtGround',139,'Boolean','false'),('DelayTime',36,'Time','0s'),('DeathWeapon',40,'Reference',None),('FocusFireKindOfFlag',44,'Bits',None),('UseMyKillerAsWeaponSource',140,'Boolean','false')],[('WeaponOffset',None),('DieMuxData',None),('WeaponFireProbability',None)]),
('DamageState',0,96,[('MinHealth',0,'Percent','0'),('MaxHealth',4,'Percent','0'),('ObjectStatus',8,'ObjectStatus',None),('ModelConditions',36,'ModelCondition',None)],[]),
('WeaponFireProbabilityType',0,12,[('DeathType',0,'DeathBits',None),('ChancePercentage',8,'Unsigned',None)],[])]
ENUM_FIELDS={'Flags':'FireWeaponFlagsType','WhereToFire':'WhereToFireType','FocusFireKindOfFlag':'KindOfType'}
ENUMS={'FireWeaponFlagsType':(2,5),'WhereToFireType':(2,0),'KindOfType':(149,284)}

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==143
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated original reactive state contracts; blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue reactive'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum reactive%sEnum = { reactive%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
    def marshal(name):
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if sig not in methods:sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_')and '_003A_003A'+name+'_002Cstruct_'in sig)
        return methods[sig]
    for name,typ,size,fields,children in SPECS[:4]:
        row=next(x for x in manifest['modules']if x['name']==name);assert (row['typeId'],row['nativeSize'])==(typ,size)
    for name,typ,size,fields,children in SPECS:
        body=marshal(name);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        expected=[(n,d)for n,o,k,d in fields]+children
        assert actual==expected,(name,actual,expected)
        if fields:lines.append('const ModuleField reactive'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='&reactive'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        if fields:lines.append('};')
    lines.append('const ModuleFieldSet reactiveFieldSets[] = {')
    for name,typ,size,fields,children in SPECS:lines.append('    { %s, %du },'%('reactive'+name+'Fields'if fields else 'nullptr',len(fields)))
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
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else None
            if off is None:
                typed=re.search(r'\((float|int|uint|bool|EmotionType)\*\)@object \+ (\d+)',part)
                off=int(typed[2])*(1 if typed[1]=='bool'else 4)if typed else 0
            mode=0;child=None
            nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
            if 'List_003C'in part:
                if 'String_003Cchar'in part:mode=6
                elif 'TypedAssetId'in part:mode=3
                else:
                    child=re.search(r'List_003CSageBinaryData_003A_003A(\w+)_003E',part)[1];mode=1;emit(child)
            elif 'Tokenizer_002ETokenize_003C'in part:
                helperName=re.search(r'(Tokenizer_002ETokenize_003C\w+)\(',part)[1]
                helper=next(b for sig,b in methods.items()if 'void '+helperName+'('in sig)
                assert 'BookmarkMemberOffset(state, @object, table)'in helper and 'Tracker_002EPush'not in helper
            elif nested:
                child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                if child=='ObjectFilter':child=None;mode=7
                else:emit(child)
            elif 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            else:raise AssertionError((name,part))
            stride=next((sz for nm,_,sz,*_ in SPECS if nm==child),0)if mode==1 else 0
            records.append((key,typ,off,child,mode,stride))
        spec=next((spec for spec in SPECS if spec[0]==name),None)
        if spec:assert set(f[1]for f in spec[3]) <= set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken reactive'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&reactive'+child if child else 'nullptr',mode,stride))
        lines+=['};','const ModuleLayout reactive%s = { reactive%sTokens, %du };'%(name,name,len(records))];emitted.add(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice reactiveChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS[:4],87):lines.append('    { %du, &reactive%s, %du, %du },'%(typ,name,size,index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectReactive.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectReactive.h'
    else:path.write_text(output)
    print('PASS complete DamageState and three reactive fire modules, full Upgrade DieMux dependencies and Tokens'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))

