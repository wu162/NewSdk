"""Complete original Physics/Bezier/Missile contracts."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/flight-source'
SPECS=[('PhysicsBehaviorModuleData',2042621387,104,[
('TumbleRandomly',96,'Boolean','false'),('MaxXRotationVelocity',8,'Float','360.0'),('MaxYRotationVelocity',12,'Float','360.0'),('MaxZRotationVelocity',16,'Float','360.0'),('AllowBouncing',97,'Boolean','false'),('KillWhenRestingOnGround',98,'Boolean','false'),('GravityMultiplier',20,'Float','1.0'),('OrientToFlightPath',99,'Boolean','false'),('ShockStunnedTimeLow',24,'Time','1s'),('ShockStunnedTimeHigh',28,'Time','1s'),('ShockStandingTime',32,'Time','1.5s'),('FirstHeight',36,'Float','1.3'),('SecondHeight',40,'Float','1.3'),('FirstPercentIndent',44,'Percent','33'),('SecondPercentIndent',48,'Percent','66'),('BounceCount',52,'Signed','2'),('BounceDistance',56,'Float','5.0'),('BounceFirstHeight',60,'Float','1.3'),('BounceSecondHeight',64,'Float','1.3'),('BounceFirstPercentIndent',68,'Percent','33'),('BounceSecondPercentIndent',72,'Percent','66'),('GroundHitFX',76,'Reference',None),('GroundBounceFX',80,'Reference',None),('IgnoreTerrainHeight',100,'Boolean','false'),('FirstPercentHeight',84,'Percent','33'),('SecondPercentHeight',88,'Percent','66'),('CurveFlattenMinDist',92,'Float','0')],[]),
('BezierProjectileBehaviorModuleData',197077561,244,[
('Flags',8,'ReplaceOptions','PING_PONG_SIDEWAYS_DRIFT'),('Type',12,'Enum','MISSILE'),('FireStraightDistance',16,'Float',None),('FirstHeight',20,'Float',None),('SecondHeight',24,'Float',None),('FirstHeightMin',28,'Float',None),('FirstHeightMax',32,'Float',None),('SecondHeightMin',36,'Float',None),('SecondHeightMax',40,'Float',None),('FirstPercentIndent',44,'Percent',None),('SecondPercentIndent',48,'Percent',None),('FinalStuckTime',52,'Time',None),('PreLandingStateTime',56,'Time',None),('BounceCount',60,'Signed',None),('BounceDistance',64,'Float',None),('BounceFirstHeight',68,'Float',None),('BounceSecondHeight',72,'Float',None),('BounceFirstPercentIndent',76,'Percent',None),('BounceSecondPercentIndent',80,'Percent',None),('GarrisonHitKillRequiredKindOf',84,'Bits',None),('GarrisonHitKillForbiddenKindOf',120,'Bits',None),('GarrisonHitKillCount',156,'Unsigned',None),('GarrisonHitKillFX',160,'Reference',None),('GroundHitFX',164,'Reference',None),('MaxDistanceReachedFX',168,'Reference',None),('GroundBounceFX',172,'Reference',None),('GroundHitWeapon',176,'Reference',None),('GroundBounceWeapon',180,'Reference',None),('FlightPathAdjustDistPerSecond',184,'FrameSpeed',None),('FirstPercentHeight',188,'Percent',None),('SecondPercentHeight',192,'Percent',None),('CurveFlattenMinDist',196,'Float',None),('PreLandingEmotion',200,'Enum',None),('PreLandingEmotionRadius',204,'Float',None),('InvisibleTime',208,'Time',None),('FadeInTime',212,'Time',None),('PostLandingStateTime',216,'Time',None),('PostLandingEmotion',220,'Enum',None),('PostLandingEmotionRadius',224,'Float',None),('SidewaysDrift',228,'Float',None),('MaxDistanceToTravel',232,'Float','1000000'),('TargetVectorOffset',236,'Float',None),('Acceleration',240,'Float','0.0')],[]),
('MissileUpdateModuleData',2441348999,268,[('FuelLifetime',244,'Time','0s'),('IgnitionDelay',248,'Time','0s'),('DistanceToTravelBeforeTurning',252,'Float','0.0'),('DistanceToTargetBeforeDiving',256,'Float','0'),('IgnitionFX',260,'Reference',None),('DetonateOnNoFuel',264,'Boolean','false')],[])]
ENUM_FIELDS={'Flags':'BezierProjectileFlag','Type':'BezierProjectileType','PreLandingEmotion':'EmotionType','PostLandingEmotion':'EmotionType','GarrisonHitKillRequiredKindOf':'KindOfType','GarrisonHitKillForbiddenKindOf':'KindOfType'}
ENUMS={'BezierProjectileFlag':(8,18),'BezierProjectileType':(0,0),'EmotionType':(8,0),'KindOfType':(149,284)}

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==49
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated original flight state contracts; blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue flight'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum flight%sEnum = { flight%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
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
        lines.append('const ModuleField flight'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='&flight'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        lines.append('};')
    lines.append('const ModuleFieldSet flightFieldSets[] = {')
    for name,typ,size,fields,children in SPECS:lines.append('    { flight%sFields, %du },'%(name,len(fields)))
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
                assert 'FXParticleSystemTemplate'in part;mode=3
                helper=next(b for sig,b in methods.items()if sig.startswith('internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020Relo_')and '(void** table, List_'in sig);assert '1820487795'in helper and '-1320050453'in helper and 'num * 4'in helper
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
            records.append((key,typ,off,child,mode))
        spec=next((spec for spec in SPECS if spec[0]==name),None)
        if spec:assert sorted(r[2]for r in records[1:])==sorted(f[1]for f in spec[3]),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken flight'+name+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&flight'+child if child else 'nullptr',mode))
        lines+=['};','const ModuleLayout flight%s = { flight%sTokens, %du };'%(name,name,len(records))];emitted.add(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice flightChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS,84):lines.append('    { %du, &flight%s, %du, %du },'%(typ,name,size,index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectFlight.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectFlight.h'
    else:path.write_text(output)
    print('PASS complete Physics Bezier Missile modules with both KindOf masks, inheritance, exact Tokens and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
