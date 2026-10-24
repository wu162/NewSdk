"""Complete eight-type AI Update contracts from fixed original methods and PE maps."""
import hashlib,json,re,sys
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/ai-update-source'
SPECS=[
('AIUpdateModuleData',76,[('AttackPriority',8,'String',None),('AutoAcquireEnemiesWhenIdle',16,'Bits',None),('StopChaseDistance',20,'Float','500.0'),('StandGround',72,'Boolean','false'),('MinCowerTime',24,'Time','0s'),('MaxCowerTime',28,'Time','0s'),('BurningDeathTime',32,'Time',None),('ComboLocoAttackDistance',36,'Float','80'),('SpecialContactPoints',40,'StringList',None),('FadeOnPortals',73,'Boolean','false'),('IdleTargetTime',48,'Time','0s'),('MaxCombineChildren',52,'Unsigned','1'),('SpawnOffsetRadius',56,'Float','0.0'),('StateMachine',60,'Enum','AIStateMachine'),('DefaultLocomotorSet',64,'Enum','NORMAL')],[('UnitAITargetChooserData',None)]),
('JetAIUpdateModuleData',100,[('OutOfAmmoDamagePerSecond',76,'Percent','0'),('OutOfAmmoDamageDelay',80,'Time',None),('NeedsRunway',96,'Boolean','false'),('ReturnToBaseWhenVictimDead',97,'Boolean','false'),('KeepsParkingSpaceWhenAirborne',98,'Boolean','false'),('ParkingOffset',84,'Float','0.0'),('CirclesForAttack',99,'Boolean','false'),('UseForOutOfAmmoCheck',88,'Reference',None),('OutOfAmmoEvaEvent',92,'Reference','')],[]),
('HordeAIUpdateModuleData',76,[],[]),
('SupplyTruckAIUpdateModuleData',116,[('MaxBoxes',76,'Unsigned',None),('SupplyCenterActionDelay',80,'Time',None),('SupplyWarehouseActionDelay',84,'Time',None),('SupplyWarehouseScanDistance',88,'Float',None),('DepletedAnnounceDistance',92,'Float','2000'),('HarvestTrees',108,'Boolean',None),('HarvestTiberium',109,'Boolean',None),('HarvestActivationRange',96,'Float',None),('HarvestPreparationTime',100,'Time',None),('HarvestActionTime',104,'Time',None),('HarvestsFromRearEnd',110,'Boolean','false'),('CanPathThroughUnitsWhileDocking',111,'Boolean','true'),('CanPathThroughUnitsWhileHarvesting',112,'Boolean','true')],[]),
('TransportAIUpdateModuleData',76,[],[]),
('AnimalAIUpdateModuleData',116,[('FleeRadius',76,'Signed','20'),('FleeDistance',80,'Signed','100'),('WanderPercentage',84,'Signed','35'),('MinDistance',88,'Signed','15'),('MaxDistance',92,'Signed','50'),('Radius',96,'Signed','25'),('FleeUpdateTimer',100,'Time','0.8s'),('WanderUpdateTimer',104,'Time','2.0s'),('AfraidOfCastles',112,'Boolean','true'),('InitialFleeBlindlyRadius',108,'Float','50.0')],[]),
('WanderAIUpdateModuleData',144,[('AttackAll',140,'Boolean','false'),('EntryCondition',76,'ModelCondition',None),('Selectable',141,'Boolean','false'),('WanderDistance',136,'Unsigned','0')],[]),
('DeployStyleAIUpdateModuleData',92,[('UnpackTime',76,'Time','0s'),('PackTime',80,'Time','0s'),('ResetTurretBeforePacking',88,'Boolean','false'),('TurretsFunctionOnlyWhenDeployed',89,'Boolean','false'),('TurretsMustCenterBeforePacking',90,'Boolean','false'),('MustDeployToAttack',91,'Boolean','false'),('DeployedAttributeModifierName',84,'Reference',None)],[]),
('UnitAITargetChooserData',40,[('TargetingCompareList',36,'Reference','DefaultTargetingCompareList')],[])]
ENUM_FIELDS={'AutoAcquireEnemiesWhenIdle':'AutoAcquireEnemiesType','StateMachine':'StateMachineType','DefaultLocomotorSet':'LocomotorSetType'}
ENUMS={'AutoAcquireEnemiesType':(4,5),'StateMachineType':(0,0),'LocomotorSetType':(12,0)}
def ident(n):return 'aiUpdate'+n

def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    metas=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in metas:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(metas)==96
    # Unknown AutoAcquire tokens use bit 4 (ATTACK_BUILDINGS), not an ignored bit.
    auto=next(body for sig,body in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AAutoAcquireEnemiesType'))
    assert re.search(r'else\s*\{\s*num4 = 4;',auto)
    assert ENUMS['AutoAcquireEnemiesType']==(4,5)
    # Shared dependency equivalence is proven against the accepted WeaponSet evidence.
    old=json.loads((ROOT/'recovery/evidence/gameobject-modules/weapon-state-source/manifest.json').read_text())
    previous={v['signature']:v for row in old['modules']for v in row['methods']};previous.update({v['signature']:v for v in old['helpers']})
    shared=[m for m in metas if any(t+'* @object' in m['signature'] for t in ['BaseAITargetChooserData','ObjectVisionRangeItem','ObjectFilter'])]
    assert len(shared)==8
    for m in shared:assert previous[m['signature']]['bodySha256']==m['bodySha256'],m['signature']
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated complete eight-type AI Update contracts; original blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue aiUpdate'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum aiUpdate%sEnum = { aiUpdate%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
    for index,(name,size,fields,children)in enumerate(SPECS):
        if index<8:
            row=manifest['modules'][index];assert(row['name'],row['nativeSize'])==(name,size)
        if not fields:continue
        if index==8:body=(EVIDENCE/'076.txt').read_text()
        else:body=methods['internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)']
        actual=[]
        for line in body.splitlines()[2:] if index==8 else body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        lines.append('const ModuleField '+ident(name)+'Fields[] = {')
        for n,o,k,d in fields:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&aiUpdate'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'))
        lines.append('};')
    lines.append('const ModuleFieldSet aiUpdateFieldSets[] = {')
    for name,size,fields,children in SPECS:lines.append('    { %s, %du },'%(ident(name)+'Fields'if fields else 'nullptr',len(fields)))
    lines.append('};');emitted=set()
    def emit(name):
        if name=='UpdateModuleData':return 'lifecycleUpdateModuleData'
        if name=='BaseAITargetChooserData':return 'weaponSetBaseAITargetChooserData'
        if name in emitted:return ident(name)
        if name in ['HordeAIUpdateModuleData','TransportAIUpdateModuleData']:
            meta=next(m for m in metas if 'Tokenizer_002ETokenizePolymorphicType' in m['signature'] and '_003A'+name+'_' in m['signature']);body=methods[meta['signature']]
        else:body=methods['internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)']
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2]),name;records=[]
        for part in parts[:-1]:
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0;mode=0;child=None
            if 'BookmarkMemberOffset'not in part:
                if 'List_003CRelo_'in part:mode=6
                else:
                    nested=re.search(r'Tokenizer_002ETokenize\([^;\n]+?, \(([\w.]+)\*\)',part);assert nested,(name,part);child=nested[1]
                    if child=='UnitAITargetChooserData':mode=4
            records.append((key,typ,off,emit(child)if child else None,mode))
        spec=next(x for x in SPECS if x[0]==name);assert set(f[1]for f in spec[2])<=set(r[2]for r in records),(name,records)
        for pattern in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;']:assert re.search(pattern,parts[-1]),name
        lines.append('const ModuleToken '+ident(name)+'Tokens[] = {')
        for key,typ,off,child,mode in records:lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,'&'+child if child else 'nullptr',mode))
        lines.extend(['};','const ModuleLayout %s = { %sTokens, %du };'%(ident(name),ident(name),len(records))]);emitted.add(name);return ident(name)
    for name,*_ in SPECS:emit(name)
    lines.append('const ModuleChoice aiUpdateChoices[] = {')
    for index,row in enumerate(manifest['modules'],104):lines.append('    { %du, &%s, %du, %du },'%(row['typeId'],ident(row['name']),row['nativeSize'],index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectAIUpdate.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectAIUpdate.h'
    else:path.write_text(output)
    print('PASS complete AI Update eight-type contracts, UnitChooser40 and eight shared method fingerprints'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
