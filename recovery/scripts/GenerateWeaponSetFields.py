"""Complete WeaponSetUpdate contracts from fixed original methods and PE maps."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/weapon-state-source'
# Fields are in original Marshal call order, including defaults and byte offsets.
SPECS=[
('WeaponSetUpdateModuleData',40,[],[('WeaponSlotHardpoint',None),('WeaponSlotTurret',None),('WeaponSlotHierarchicalTurret',None),('WeaponSlotContained',None)]),
('WeaponSlot_Hardpoint',28,[('ID',4,'Unsigned',None),('AllowInterleavedFiring',24,'Boolean','false'),('InterleavedStyle',8,'Enum','INTERLEAVE_RANDOM'),('WeaponChoiceCriteria',12,'Enum','PREFER_MOST_DAMAGE'),('AutoChooseWeapon',25,'Boolean','true')],[('Weapon',None)]),
('WeaponSlot_Turret',124,[],[('TurretSettings',None)]),
('WeaponSlot_HierarchicalTurret',128,[('ParentID',124,'Unsigned','0')],[]),
('WeaponSlot_Contained',28,[],[]),
('WeaponSlot_WeaponData',72,[('Ordering',0,'Enum',None),('Template',4,'Reference',None),('Upgrade',8,'Reference',None),('ObjectStatus',12,'ObjectStatus',''),('ForbiddenObjectStatus',40,'ObjectStatus',''),('IsPlayerUpgradePermanent',68,'Boolean','false'),('ConquerMetricsAlwaysCount',69,'Boolean','false')],[]),
('TurretAIData',96,[('TurretTurnRate',0,'Float','0.0'),('TurretPitchRate',4,'Float','0.0'),('AllowsPitch',92,'Boolean','false'),('MinimumPitch',8,'Angle','0d'),('PitchHeight',12,'Percent','50%'),('ControlledWeaponSlots',16,'Bits',None),('InvalidIdleScanObjectStatus',20,'ObjectStatus',None),('IdleTurnRateModifier',48,'Percent','50%'),('MinIdleScanTime',52,'Time','0.0s'),('MaxIdleScanTime',56,'Time','0.0s'),('MinIdleScanAngle',60,'Float','0.0'),('MaxIdleScanAngle',64,'Float','0.0'),('MaxDeflectionClockwise',68,'Angle','180d'),('MaxDeflectionAntiClockwise',72,'Angle','180d'),('NaturalTurretAngle',76,'Angle','0d'),('RecenterWhenOutOfTurnRange',93,'Boolean','true'),('BlindFireDistance',80,'Float','0'),('ComeToHaltJiggle',84,'Angle','0d'),('SetAngleModelStates',94,'Boolean','false')],[('TurretAITargetChooserData',None)]),
('TurretAITargetChooserData',44,[('TargetingCompareList',36,'Reference','DefaultTurretTargetingCompareList'),('ActiveWhenPerformingSpecialAbility',40,'Boolean','false'),('CanAcquireDynamicIfAssignedOutOfRange',41,'Boolean','false'),('CanPickTargetsOutOfTurretAngle',42,'Boolean','false')],[]),
('BaseAITargetChooserData',36,[('IdleScanDelay',0,'Time','0.8s'),('ReacquireDelay',4,'Time','60s'),('StartupDelay',8,'Time','0s'),('SympathyRange',12,'Float','25.0'),('CheckVisionRange',28,'Boolean','false'),('CanPickDynamicTargets',29,'Boolean','true'),('CanPickDynamicTargetType',16,'Bits','ALWAYS'),('RotateToTargetWhenAiming',30,'Boolean','false'),('CanAutoAcquireNonAutoAcquirable',31,'Boolean','false'),('CanChaseAttack',32,'Boolean','true'),('CanOnlyAttackTargetsInFront',33,'Boolean','false')],[('ObjectVisionRange',None)]),
('ObjectVisionRangeItem',120,[('Range',4,'Percent','100')],[('Filter',None)])]
ENUM_FIELDS={'Ordering':'WeaponSlotType','ControlledWeaponSlots':'WeaponSlotType','InterleavedStyle':'WeaponSlotInterleavedStyleType','WeaponChoiceCriteria':'WeaponChoiceCriteria','CanPickDynamicTargetType':'TargetChooserDynamicTargetFlag'}
ENUMS={'WeaponSlotType':(5,6),'WeaponSlotInterleavedStyleType':(0,0),'WeaponChoiceCriteria':(2,0),'TargetChooserDynamicTargetFlag':(2,4)}

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers']
    required={f'{i:03}.txt'for i in [*range(6),*range(82,143)]};methods={};literals={}
    for meta in allmeta:
        if meta['file']not in required:continue
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature'])
        assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(required)
    row=next(x for x in manifest['modules']if x['name']=='WeaponSetUpdateModuleData');assert(row['typeId'],row['nativeSize'])==(1125589673,40)
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated complete WeaponSetUpdate contracts; original blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue weaponSet'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum weaponSet%sEnum = { weaponSet%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
    for name,size,fields,children in SPECS:
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        # Contained directly delegates to Hardpoint; it has no scalar method.
        if name=='WeaponSlot_Contained':continue
        body=methods[sig];actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        if fields:lines.append('const ModuleField weaponSet'+name+'Fields[] = {')
        for n,o,k,d in fields:
            values='&weaponSet'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'
            lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
        if fields:lines.append('};')
    lines.append('const ModuleFieldSet weaponSetFieldSets[] = {')
    for name,size,fields,children in SPECS:lines.append('    { %s, %du },'%('weaponSet'+name+'Fields'if fields else 'nullptr',len(fields)))
    lines.append('};');emitted=set()
    def emit(name):
        if name=='UpdateModuleData':return 'lifecycleUpdateModuleData'
        if name in emitted:return 'weaponSet'+name
        if name=='WeaponSlot_Contained':
            body=next(b for sig,b in methods.items()if 'Tokenizer_002ETokenize_003Cstruct_0020SageBinaryData_003A_003AWeaponSlot_Contained_003E('in sig)
            assert '12u, 2u)'in body and '((int*)ptr4)[1] = -833072169;'in body and '(WeaponSlot_Hardpoint*)ptr3'in body
            emit('WeaponSlot_Hardpoint')
            lines.extend(['const ModuleToken weaponSetWeaponSlot_ContainedTokens[] = {','    { 1u, 3461895127u, 0u, &weaponSetWeaponSlot_Hardpoint, 0u, 0u },','};','const ModuleLayout weaponSetWeaponSlot_Contained = { weaponSetWeaponSlot_ContainedTokens, 1u };'])
            emitted.add(name);return 'weaponSet'+name
        sig='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
        body=methods[sig];push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name
        records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0
            mode=0;child=None;stride=0
            if 'BookmarkMemberOffset'in part:
                if '(void*)(int)'in part:mode=2
            elif 'List_003C'in part:
                child=re.search(r'List_003CSageBinaryData_003A_003A(\w+)_003E',part)[1];mode=1
                stride=next(sz for nm,sz,*_ in SPECS if nm==child)
            else:
                nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \((\w+)\*\)',part)
                if nested:child=nested[1]
                else:
                    child=re.search(r'Tokenizer_002ETokenize_003Cstruct_0020SageBinaryData_003A_003A(\w+)_003E',part)[1]
                    assert child=='TurretAITargetChooserData' and '(TurretAITargetChooserData**)'in part;mode=4
                if child=='ObjectFilter':child=None;mode=8
            nestedName=emit(child)if child else None
            records.append((key,typ,off,nestedName,mode,stride))
        spec=next((spec for spec in SPECS if spec[0]==name),None)
        if spec:assert set(f[1]for f in spec[2])<=set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken weaponSet'+name+'Tokens[] = {')
        for key,typ,off,child,mode,stride in records:lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&'+child if child else 'nullptr',mode,stride))
        lines.extend(['};','const ModuleLayout weaponSet%s = { weaponSet%sTokens, %du };'%(name,name,len(records))]);emitted.add(name);return 'weaponSet'+name
    for name,*_ in SPECS:emit(name)
    lines+=['const ModuleChoice weaponSetChoice = { 1125589673u, &weaponSetWeaponSetUpdateModuleData, 40u, 91u };',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectWeaponSet.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectWeaponSet.h'
    else:path.write_text(output)
    print('PASS complete WeaponSetUpdate four-slot/WeaponData/TurretAI/Chooser/VisionRange contracts'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
