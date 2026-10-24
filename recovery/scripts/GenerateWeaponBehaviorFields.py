"""Complete direct firing and damage tracking group with original nested dependencies."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/weapon-behavior-source'
SPECS=[('ChargeWeaponUpdateModuleData',602268606,24,[('ChargeTime',8,'Time','1.0s'),('OverChargeTime',12,'Time','0.0s'),('OverChargePercent',16,'Percent','0%')],[('ObjectStatusValidation',None)]),('FireWeaponUpdateModuleData',2995535186,32,[('FireInterval',8,'Time',None),('AliveOnly',28,'Boolean','false'),('ActiveWhenDisabled',12,'ReplaceOptions',None)],[('FireWeaponNugget',None),('ObjectStatusValidation',None)]),('AimWeaponBehaviorModuleData',3828176408,24,[('AimLowThreshold',8,'Float','-0.4'),('AimHighThreshold',12,'Float','0.4'),('AimNearDistance',16,'Float','0'),('AimFarDistance',20,'Float','0')],[]),('LaserUpdateModuleData',3399407677,32,[('ParticleSystemId',8,'Reference',None),('ParentFireBoneName',12,'String',None),('ParentFireBoneOnTurret',28,'Boolean','false'),('TargetParticleSystemId',20,'Reference',None),('LaserLifetime',24,'Float','0')],[]),('LaserScatterUpdateModuleData',4198440042,16,[('LaserLifetime',8,'Time','1s'),('LaserHitGroundWidthMultiplier',12,'Percent','10%')],[]),('LinearDamageUpdateModuleData',509375610,40,[('StartDistanceOffset',8,'Float','0'),('Radius',12,'Float','10.0'),('Length',16,'Float',None),('SweepFXList',20,'Reference',None),('SweepWeapon',24,'Reference',None),('Speed',28,'FrameSpeed','1.0'),('FollowTerrain',36,'Boolean','true')],[('ObjectStatusValidation',None)]),('SecondaryDamageTrackerModuleData',2988211815,36,[('UpdateDuration',8,'Time','1s'),('DissipateAmount',12,'Unsigned','0'),('MaxDamageClampScaler',16,'Percent','125%')],[('Color',None),('EffectInfo',None),('ObjectStatusValidation',None)]),('TrackDamageModuleData',3579087005,16,[('TimeToTrack',8,'Time',None),('InstantWeaponsOnly',12,'Boolean','false')],[])]
NESTED=[('RGBColor',[('R',0,'Float',None),('G',4,'Float',None),('B',8,'Float',None)],[]),('EffectInfoType',[('Fraction',0,'Float',None),('AttributeModifier',4,'Reference',None),('OnEnterDisable',12,'Boolean','false'),('OnEnterCancelSpecialAbilities',13,'Boolean','false'),('SetUpgradedArmor',14,'Boolean','false'),('Options',8,'ReplaceOptions','')],[])]

NESTED += [('FireWeaponUpdateModuleData.FireWeaponNugget',[('WeaponName',0,'Reference',None),('FireDelay',4,'Time',None),('OneShot',12,'Boolean','false')],[('Offset',None)]),('Vector3',[(n,4*i,'Float','0.0')for i,n in enumerate('xyz')],[])]
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==85
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    constants=json.loads((EVIDENCE/'constants-source.json').read_text());assert constants['sourceBlob']==BLOB
    rows=constants['logicFrameReferences'];frequency=next(x['text']for x in rows if 'f4a5029c_002ELOGICFRAMES_PER_SECONDS_REAL/* Not supported: data'in x['text']);assert 'data(00 00 A0 40)'in frequency
    formula=next(x['text']for x in rows if 'f4a5029c_002ESECONDS_PER_LOGICFRAME_REAL ='in x['text']);assert '(float)(1.0 / (double)'in formula and 'f4a5029c_002ELOGICFRAMES_PER_SECONDS_REAL' in formula
    if source is not None:
        for row in rows:assert source.splitlines()[row['line']-1].strip()==row['text']
    lines=['// Generated original weapon firing/tracking contracts; blob '+BLOB+'.','const float weaponSecondsPerLogicFrame = 0.2f;']
    for group,count,unknown,bits in [('DisabledType',12,0,12),('EffectInfoFlagType',1,0,1)]:
        entries=enums[group];assert len(entries)==count
        if source is not None:assert enum_values(source,pe,group)==entries
        helper=next(b for sig,b in methods.items()if 'Marshaler_002EMarshal_003Cenum_0020' in sig and '('in sig and group+'_'in sig)
        assert 'num4 = 0;'in helper and '*(int*)target = -1;'in helper
        lines.append('const EnumNameValue weapon'+group+'Names[] = {')
        lines+=['    { %s, %d },'%(json.dumps(n),v)for n,v in entries.items()]
        lines+=['};','const AIEnum weapon%s = { weapon%sNames, %du, %d, %du };'%(group,group,count,unknown,bits)]
    def marshal(name):
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state'+(', VectorHack* tag)'if name=='Vector3'else ')')
        if sig not in methods:sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_')and '_003A_003A'+name+'_002Cstruct_'in sig)
        return methods[sig]
    for name,typ,size,fields,children in SPECS+[(n,0,0,f,c)for n,f,c in NESTED]:
        if typ:
            row=next(x for x in manifest['modules']if x['name']==name);assert (row['typeId'],row['nativeSize'])==(typ,size)
        body=marshal(name);actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        expected=[(n,d)for n,o,k,d in fields]+children
        assert actual==expected,(name,actual,expected)
        lines.append('const ModuleField weapon'+name.replace('.','_')+'Fields[] = {')
        for n,o,k,d in fields:
            values='&weaponDisabledType'if n=='ActiveWhenDisabled'else '&weaponEffectInfoFlagType'if n=='Options'else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        lines.append('};')
    lines.append('const ModuleFieldSet weaponFieldSets[] = {')
    for name,fields in [(n,f)for n,t,z,f,c in SPECS]+[(n,f)for n,f,c in NESTED]:lines.append('    { weapon%sFields, %du },'%(name.replace('.','_'),len(fields)))
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
            nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \(([\w.]+)\*\)',part)
            if 'List_003C'in part:
                child='EffectInfoType'if 'EffectInfoType'in part else 'FireWeaponUpdateModuleData.FireWeaponNugget';mode=1;emit(child)
            elif nested:
                child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                if child=='ObjectFilter':child=None;mode=7
                else:emit(child)
            elif 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            else:raise AssertionError((name,part))
            records.append((key,typ,off,child,mode))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken weapon'+name.replace('.','_')+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&weapon'+child.replace('.','_')if child else 'nullptr',mode,16 if mode==1 else 0))
        lines+=['};','const ModuleLayout weapon%s = { weapon%sTokens, %du };'%(name.replace('.','_'),name.replace('.','_'),len(records))];emitted.add(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice weaponChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS,69):
        if typ==3399407677:continue # Existing complete ClientUpdate implementation.
        lines.append('    { %du, &weapon%s, %du, %du },'%(typ,name,size,index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectWeaponBehavior.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectWeaponBehavior.h'
    else:path.write_text(output)
    print('PASS seven new complete weapon firing/tracking modules plus existing LaserUpdate source, nested Nugget/EffectInfo/Color/validation, original Token and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
