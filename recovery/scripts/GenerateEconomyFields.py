"""Complete seven-type Dock/Ore/gathering group from fixed original contracts."""
import hashlib,json,re,sys
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/economy-source'
SPECS=[
('DockUpdateModuleData',28,[('NumberApproachPositions',8,'Signed',None),('AllowsPassthrough',24,'Boolean',None),('GoToRallyPointAfterDock',25,'Boolean','false'),('ShouldDockInReverse',26,'Boolean','false'),('MinDockTime',12,'Time','0s'),('ObjectsInLineWeight',16,'Time','13s')],[('ForVoiceRetreatThisIsASafeHarborToObjectFilter',None)]),
('OreNodeModuleData',72,[('MaximumGatheredValue',8,'Unsigned','20000'),('DeliveryAmount',12,'Unsigned','1000'),('DeliveryAmountWhenEmpty',16,'Unsigned','0'),('CashMultiplier',20,'Percent','100%'),('MoveToExtractTime',24,'Time','1s'),('ExtractTime',28,'Time','1s'),('MoveToDeliverTime',32,'Time','1s'),('DeliveryTime',36,'Time','1s'),('ExtractAnimation',40,'Enum',None),('DeliveryAnimation',44,'Enum',None),('ReturnAnimation',48,'Enum',None),('MoveBackAnimation',52,'Enum',None),('LowNotificationPercentage',56,'Percent',None),('LowNotificationEvaEventRef',60,'Reference',None),('DepletedNotificationEvaEventRef',64,'Reference',None),('DepletedNotificationAudioEventInfoRef',68,'Reference',None)],[]),
('OreMinerModuleData',20,[('DeliveryCapacity',8,'Unsigned','1000'),('DeliveryTime',12,'Time','1s')],[('ObjectStatusValidation',None)]),
('SupplyCenterDockUpdateModuleData',36,[('ValueMultiplier',28,'Float','1.0'),('DistributedDeposit',32,'Boolean',None)],[]),
('SupplyWarehouseDockUpdateModuleData',36,[('StartingBoxes',28,'Signed','1'),('DeleteWhenEmpty',32,'Boolean','false')],[]),
('GatherSlavesUpdateModuleData',20,[('SlaveTemplate',8,'Reference',None),('Radius',12,'Float',None),('Amount',16,'Unsigned',None)],[]),
('GatherEjectUpdateModuleData',172,[('Lifetime',8,'Time','10s'),('Slots',12,'Signed','5'),('ScanTime',16,'Time','0.2s'),('HeightOffGround',20,'Float','50.0'),('EffectDistance',24,'Float','125.0'),('LiftOffGroundDistance',28,'Float','75.0'),('GatherAcceleration',32,'Float','0.01')],[('RotationSpeed',None),('UnitSpacing',None),('Filter',None)]),
('Vector3',12,[('x',0,'Float','0.0'),('y',4,'Float','0.0'),('z',8,'Float','0.0')],[])]
EXTERNAL={'ModuleData':'reactiveModuleData','BehaviorModuleData':'reactiveBehaviorModuleData','UpdateModuleData':'lifecycleUpdateModuleData'}
def ident(n):return 'economy'+n

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    metas=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in metas:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(metas)==75
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    if source is not None:
        for group,entries in enums.items():
            if group!='sourceBlob':assert enum_values(source,pe,group)==entries,group
    enumBody=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020ModelConditionFlagType_002Cclass_'))
    assert '((ptr == null) ? 233 :'in enumBody
    lines=['// Generated complete seven-type Dock/Ore/gathering contracts; original blob '+BLOB+'.','const EnumNameValue economyModelConditionNames[] = {']
    for n,v in enums['ModelConditionFlagType'].items():lines.append('    { %s, %d },'%(json.dumps(n),v))
    lines+=['};','const AIEnum economyModelConditionEnum = { economyModelConditionNames, %du, 233, 0u };'%len(enums['ModelConditionFlagType'])]
    for index,(name,size,fields,children)in enumerate(SPECS):
        if index<7:
            row=manifest['modules'][index];assert(row['name'],row['nativeSize'])==(name,size)
        body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object,'));actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        lines.append('const ModuleField '+ident(name)+'Fields[] = {')
        for n,o,k,d in fields:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&economyModelConditionEnum'if k=='Enum'else 'nullptr'))
        lines.append('};')
    lines.append('const ModuleFieldSet economyFieldSets[] = {')
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
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0;mode=0;child=None
            if 'BookmarkMemberOffset'not in part:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \(([\w.]+)\*\)',part);assert nested,(name,part);child=nested[1]
                if '(int)num2'in part:mode=4
                if child=='ObjectFilter':child=None;mode=7 if mode==4 else 8
            records.append((key,typ,off,emit(child)if child else None,mode))
        spec=next((x for x in SPECS if x[0]==name),None)
        if spec:assert set(f[1]for f in spec[2])<=set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken '+ident(name)+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&'+child if child else 'nullptr',mode))
        lines.extend(['};','const ModuleLayout %s = { %sTokens, %du };'%(ident(name),ident(name),len(records))]);emitted.add(name);return ident(name)
    for name,*_ in SPECS[:7]:emit(name)
    lines.append('const ModuleChoice economyChoices[] = {')
    for index,row in enumerate(manifest['modules'],112):lines.append('    { %du, &%s, %du, %du },'%(row['typeId'],ident(row['name']),row['nativeSize'],index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectEconomy.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectEconomy.h'
    else:path.write_text(output)
    print('PASS complete seven-type Dock/Ore/gathering contracts and 75 original method fingerprints'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
