"""Complete healing/repair group from fixed original Marshal/Token/endian methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/healing-source'
SPECS=[
('AutoHealBehaviorModuleData',112,[('InitiallyActive',104,'Boolean','false'),('HealingAmount',36,'Signed','0'),('HealingDelay',40,'Time','0s'),('StartHealingDelay',44,'Time','0s'),('Radius',48,'Signed','0'),('AffectsWholePlayer',105,'Boolean','false'),('AffectsContained',106,'Boolean','false'),('HealOnlyIfNotUnderAttack',107,'Boolean','false'),('HealOnlyIfNotInCombat',108,'Boolean','false'),('KindOf',52,'Bits',None),('HealOnlyOthers',109,'Boolean','false'),('HealFXList',88,'Reference',None),('SpawnFXList',92,'Reference',None),('NonStackable',110,'Boolean','false'),('RespawnNearbyHordeMembers',111,'Boolean','false'),('RespawnMinimumDelay',96,'Time','0s')],[('ObjectStatusValidation',None)]),
('RepairUpdateModuleData',44,[('Toggleable',40,'Boolean','true'),('RepairableWhenDead',41,'Boolean','false'),('RepopThreshold',8,'Float','0.2'),('HealAmountPerSecond',12,'Unsigned','10')],[('PercentOfBuildCostToRebuildInfo',None),('SelfAudio',None)]),
('InitiateRepairDieModuleData',32,[('DelayTime',8,'Time','0s'),('MasterDeadDieFX',12,'Reference',None),('Options',16,'Bits',None),('InstantRepairModelCondition',20,'Enum','INVALID'),('InstantRepairAnimDuration',24,'Time',None)],[('Die',None)]),
('HealCrateCollideModuleData',116,[('Range',108,'Float','75')],[('ObjectFilter',None)]),
('CrateCollideModuleData',108,[('KindOf',8,'Bits',None),('KindOfNot',44,'Bits',None),('IsForbidOwnerPlayer',100,'Boolean','false'),('IsBuildingPickup',101,'Boolean','false'),('IsHumanOnlyPickup',102,'Boolean','false'),('IsHumanOnlyPickupInSinglePlayerCampaign',103,'Boolean','true'),('ExecuteFX',80,'Reference',None),('ExecutionAnimationTemplate',84,'AssetId',None),('ExecuteAnimationDisplayTimeInSeconds',88,'Float','0'),('ExecuteAnimationZRisePerSecond',92,'Float','0'),('ExecuteAnimationFades',104,'Boolean','false'),('ExecuteAudioEvent',96,'Reference','CratePickup')],[]),
('RepairUpdateModuleData.PercentOfBuildCostToRebuildInfo',16,[(n,4*i,'Percent','50')for i,n in enumerate(['Pristine','Damaged','ReallyDamaged','Rubble'])],[]),
('RepairUpdateModuleData.SelfAudio',8,[('SelfRepairFromDamageLoop',0,'Reference',None),('SelfRepairFromRubbleLoop',4,'Reference',None)],[])]
ENUM_FIELDS={'KindOf':'KindOfType','KindOfNot':'KindOfType','Options':'InitiateRepairDieOptions','InstantRepairModelCondition':'ModelConditionFlagType'}
ENUMS={'KindOfType':(149,284),'InitiateRepairDieOptions':(0,1),'ModelConditionFlagType':(233,0)}
EXTERNAL={'ModuleData':'reactiveModuleData','BehaviorModuleData':'reactiveBehaviorModuleData','CollideModuleData':'reactiveCollideModuleData','UpdateModuleData':'lifecycleUpdateModuleData','UpgradeModuleData':'reactiveUpgradeModuleData','DieMuxDataType':'reactiveDieMuxDataType','ObjectStatusValidationDataType':'lifecycleObjectStatusValidationDataType'}
def ident(name):return 'healing'+name.replace('.','_')
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==86
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated complete healing and repair contracts; original blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue healing'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum healing%sEnum = { healing%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
    for index,(name,size,fields,children)in enumerate(SPECS):
        if index<4:
            row=manifest['modules'][index];assert (row['name'],row['nativeSize'])==(name,size)
        body=methods['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'];actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        lines.append('const ModuleField '+ident(name)+'Fields[] = {')
        for n,o,k,d in fields:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&healing'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'))
        lines.append('};')
    lines.append('const ModuleFieldSet healingFieldSets[] = {')
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
                if '('+child+'*)(int)'in part:mode=4
                if child=='ObjectFilter':child=None;mode=7
            records.append((key,typ,off,emit(child)if child else None,mode))
        spec=next(x for x in SPECS if x[0]==name);assert set(f[1]for f in spec[2])<=set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken '+ident(name)+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&'+child if child else 'nullptr',mode))
        lines.extend(['};','const ModuleLayout %s = { %sTokens, %du };'%(ident(name),ident(name),len(records))]);emitted.add(name);return ident(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice healingChoices[] = {')
    for index,row in enumerate(manifest['modules'],92):lines.append('    { %du, &%s, %du, %du },'%(row['typeId'],ident(row['name']),row['nativeSize'],index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectHealing.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectHealing.h'
    else:path.write_text(output)
    print('PASS complete AutoHeal/Repair/InitiateRepair/HealCrate contracts and required bases'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
