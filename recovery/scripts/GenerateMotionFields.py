"""Complete Projectile/Dynamics/Freefall contracts pinned to the original blob/PE."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/motion-source'
SPECS=[
('ProjectileUpdateModuleData',290244143,28,[('StateMachine',8,'Enum',None),('MinDetonationHeight',12,'Float',None),('MinTargetDistance',16,'Float',None),('Flags',20,'ReplaceOptions',None)],[('Movement',None)]),
('DynamicsUpdateModuleData',358254523,32,[('Motion',8,'Enum',None),('Collision',12,'Enum',None),('IntuitiveCollision',16,'Enum','COLLIDES_WITH_GROUND_ONLY'),('ContactTag',20,'Enum','NONE'),('DrawPriority',24,'Enum','REMOVABLE')],[('Volume',None)]),
('FreefallUpdateModuleData',4230098692,80,[('ModelConditionsToSet',8,'ModelCondition',None),('InitialVelocity',68,'Float','0'),('Acceleration',72,'Float','1'),('TerminalVelocity',76,'Float','15')],[]),
('EnableDynamicsCollideModuleData',4235893941,12,[('Weight',8,'Float','0.1')],[]),
('ProjectileMoveInStraightLineData',2240593884,16,[('MuzzleVelocity',4,'Float',None),('Acceleration',8,'Float',None),('TerminalVelocity',12,'Float',None)],[]),
('ProjectileMoveToGroundData',2272689654,28,[('InitialSpeed',4,'Float','0'),('Acceleration',8,'Float',None),('TerminalVelocity',12,'Float',None),('RotationSpeedPerSecond',16,'Angle','0d'),('TerminalRotation',20,'Angle','90d'),('ForwardVelocityDamping',24,'Percent','100%')],[]),
('BezierSplineProjectileData',3012512132,24,[('Velocity',4,'Float',None),('Range',8,'Float',None),('Gravity',12,'Float','1.0')],[('Path',None)]),
('DynamicsVolumeType',0,76,[('Mass',0,'Float','0'),('Bounciness',4,'Percent','30%'),('Spinniness',8,'Float','2.0'),('StaticFriction',12,'Float','0.6'),('KineticFriction',16,'Float','0.5'),('AngularDrag',20,'Float','0.1'),('LinearDrag',24,'Float','0.1'),('ReportContacts',72,'Boolean','false'),('ContactTag',28,'Enum','NONE')],[(n,None)for n in ['Sphere','Capsule','Box','Cylinder','Triangle']]),
('DynamicsSphereShapeType',0,12,[('Radius',8,'Float',None)],[]),
('DynamicsCapsuleShapeType',0,16,[('Radius',8,'Float',None),('HalfLength',12,'Float',None)],[]),
('DynamicsBoxShapeType',0,24,[('HalfSizeX',8,'Float',None),('HalfSizeY',12,'Float',None),('HalfSizeZ',16,'Float',None),('EdgeRadius',20,'Float','0')],[]),
('DynamicsCylinderShapeType',0,20,[('Radius',8,'Float',None),('HalfLength',12,'Float',None),('EdgeRadius',16,'Float','0')],[]),
('DynamicsTriangleShapeType',0,48,[(n,8+4*i,'Float',None)for i,n in enumerate(['X1','Y1','Z1','X2','Y2','Z2','X3','Y3','Z3'])]+[('EdgeRadius',44,'Float','0')],[]),
('Vector3',0,12,[(n,4*i,'Float','0.0')for i,n in enumerate('xyz')],[]),
('Quaternion',0,16,[(n,4*i,'Float','0.0')for i,n in enumerate('xyzw')],[])]
ENUM_FIELDS={'StateMachine':'ProjectileStateMachineType','Motion':'DynamicsMotionType','Collision':'DynamicsCollisionType','IntuitiveCollision':'IntuitiveCollisionType','ContactTag':'DynamicsContactTag','DrawPriority':'DynamicsDrawPriority','Flags':'ProjectileUpdateOptions'}
FALLBACK={'ProjectileStateMachineType':2,'DynamicsMotionType':1,'DynamicsCollisionType':1,'IntuitiveCollisionType':2,'DynamicsContactTag':7,'DynamicsDrawPriority':2,'ProjectileUpdateOptions':0}

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==97
    for name,typ,size,*_ in SPECS[:7]+[('BalloonProjectileMoveToGroundData',442194537,4)]:
        sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and '_003A_003A'+name+'_002Cstruct_'in sig)
        wrapper=methods[sig];assert ', %du, 1u)'%size in wrapper,name
        stored=int(re.search(r'\*\(int\*\)\w+ = (-?\d+);',wrapper)[1])&0xffffffff;assert stored==typ,name
    for name,_,size,*_ in SPECS[8:13]:
        helper=next(body for sig,body in methods.items()if '(IList* list, List_'in sig and '_003A_003A'+name+'_003E'in sig)
        assert ', %du, num)'%size in helper,name
    for group,unknown in FALLBACK.items():
        if group=='ProjectileUpdateOptions':continue
        helper=next(body for sig,body in methods.items()if '(IValue* value, '+group+'* @object,'in sig)
        assert '? %d :'%unknown in helper,group
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated original motion contracts; blob '+BLOB+'.']
    for group,unknown in FALLBACK.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue motion'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum motion%sEnum = { motion%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,1 if group=='ProjectileUpdateOptions'else 0)]
    assert len(enums['ModelConditionFlagType'])==457
    if source is not None:assert enum_values(source,pe,'ModelConditionFlagType')==enums['ModelConditionFlagType']
    def marshal(name):
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if name in ('Vector3','Quaternion'):sig=next(s for s in methods if s.startswith(sig[:-1]+', VectorHack*'))
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
        assert sorted(actual)==sorted(expected),(name,actual,expected)
        lines.append('const ModuleField motion'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='&motion'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        lines.append('};')
    lines.append('const ModuleFieldSet motionFieldSets[] = {')
    for name,typ,size,fields,children in SPECS:lines.append('    { motion%sFields, %du },'%(name,len(fields)))
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
                if 'AssetReference'in part:mode=3
                else:
                    child=re.search(r'List_003CSageBinaryData_003A_003A(\w+)_003E',part)[1];mode=1;emit(child)
            elif '(ProjectileMoveBase**)((byte*)@object'in part:mode=9
            elif nested:
                child=nested[1];mode=4 if '('+child+'*)(int)'in part else 0
                if child=='ObjectFilter':child=None;mode=7
                else:emit(child)
            elif 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            else:raise AssertionError((name,part))
            stride=next((sz for nm,_,sz,*_ in SPECS if nm==child),0)if mode==1 else 0
            records.append((key,typ,off,child,mode,stride))
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken motion'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&motion'+child if child else 'nullptr',mode,stride))
        lines+=['};','const ModuleLayout motion%s = { motion%sTokens, %du };'%(name,name,len(records))];emitted.add(name)
    for name,*_ in SPECS:
        if name not in ('Vector3','Quaternion'):emit(name)
    emit('BalloonProjectileMoveToGroundData')
    lines.append('const ModuleChoice motionChoices[] = {')
    for index,(name,typ,size,fields,children)in enumerate(SPECS[:4],80):lines.append('    { %du, &motion%s, %du, %du },'%(typ,name,size,index))
    lines+=['};','const ModuleChoice motionMoveChoices[] = {']
    for name,typ,size,*_ in SPECS[4:7]:lines.append('    { %du, &motion%s, %du, 0u },'%(typ,name,size))
    lines+=['    { 442194537u, &motionBalloonProjectileMoveToGroundData, 4u, 0u },','};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectMotion.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectMotion.h'
    else:path.write_text(output)
    print('PASS four complete motion modules, polymorphic Movement, five shapes, exact Token inheritance and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
