"""Complete crate group from fixed original Marshal/Token/endian methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/crate-source'
from GenerateHealingFields import SPECS as HEALING_SPECS
SPECS=[
('CrateCollideModuleData',108,HEALING_SPECS[4][2],[]),
('MoneyCrateCollideModuleData',112,[('MoneyProvided',108,'Unsigned','0')],[]),
('UnitCrateCollideModuleData',116,[('UnitCount',108,'Unsigned','0'),('UnitType',112,'AssetId',None)],[]),
('VeterancyCrateCollideModuleData',120,[('RangeOfEffect',108,'Unsigned','0'),('AddsOwnerVeterancy',116,'Boolean','false'),('IsPilot',117,'Boolean','false'),('AffectsUpToLevel',112,'Signed','0')],[]),
('AttributeCrateCollideModuleData',112,[('AttributeModifier',108,'Reference','AttributeModifier_InvulnerabilityCrate')],[]),
('RandomCrateCollideModuleData',140,[(n,108+4*i,'Float','0')for i,n in enumerate(['AttributeCrateChance','UnitCrateChance','BombCrateChance','HealCrateChance','MoneyCrateChance','ShroudCrateChance','VeterancyCrateChance'])]+[('AllowAIPickup',136,'Boolean','false')],[]),
('SalvageCrateCollideModuleData',144,[(n,108+4*i,'Float','0')for i,n in enumerate(['PorterChance','BannerChance','LevelUpChance','LevelUpRadius','ResourceChance'])]+[('MinimumResource',128,'Signed','0'),('MaximumResource',132,'Signed','0'),('Upgrade',136,'Reference',None),('AllowAIPickup',140,'Boolean','false')],[]),
('SpawnCrateUpdateModuleData',48,[('MaxCrates',8,'Unsigned','20'),('CreateFrequency',12,'Time','30s'),('CrateLifetime',16,'Time','120s'),('MapExclusionMargin',20,'Float','50.0'),('BlockingUnitRange',24,'Float','30.0'),('BlockingStartPositionRange',28,'Float','300.0'),('Flags',32,'Bits','DELETE_EXPIRED_CRATES')],[('CrateList',None),('BlockingUnits',None)])]
ENUM_FIELDS={'KindOf':'KindOfType','KindOfNot':'KindOfType','Flags':'SpawnCrateUpdateFlagsType'}
ENUMS={'KindOfType':(149,284),'SpawnCrateUpdateFlagsType':(1,2)}
EXTERNAL={'ModuleData':'reactiveModuleData','BehaviorModuleData':'reactiveBehaviorModuleData','CollideModuleData':'reactiveCollideModuleData','UpdateModuleData':'lifecycleUpdateModuleData','UpgradeModuleData':'reactiveUpgradeModuleData','DieMuxDataType':'reactiveDieMuxDataType','ObjectStatusValidationDataType':'lifecycleObjectStatusValidationDataType'}
def ident(name):return 'crate'+name.replace('.','_')
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    allmeta=[v for m in manifest['modules']for v in m['methods']]+manifest['helpers'];methods={};literals={}
    for meta in allmeta:
        text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,meta['signature'])==body,meta['file']
        methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==len(allmeta)==82
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    lines=['// Generated complete crate and repair contracts; original blob '+BLOB+'.']
    for group,(unknown,bits)in ENUMS.items():
        entries=enums[group]
        if source is not None:assert enum_values(source,pe,group)==entries
        lines.append('const EnumNameValue crate'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum crate%sEnum = { crate%sNames, %du, %d, %du };'%(group,group,len(entries),unknown,bits)]
    for index,(name,size,fields,children)in enumerate(SPECS):
        if index<8:
            row=manifest['modules'][index];assert (row['name'],row['nativeSize'])==(name,size)
        sig='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
        if sig not in methods:sig=next(sig for sig in methods if sig.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_')and '_003A_003A'+name+'_002Cstruct_'in sig)
        body=methods[sig];actual=[]
        for line in body.splitlines():
            syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
            if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
        assert actual==[(n,d)for n,o,k,d in fields]+children,(name,actual)
        lines.append('const ModuleField '+ident(name)+'Fields[] = {')
        for n,o,k,d in fields:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&crate'+ENUM_FIELDS[n]+'Enum'if n in ENUM_FIELDS else 'nullptr'))
        lines.append('};')
    lines.append('const ModuleFieldSet crateFieldSets[] = {')
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
            if 'List_003CRelo_003A_003AAssetReference_'in part:
                assert name=='SpawnCrateUpdateModuleData' and off==36;mode=3
            elif 'BookmarkMemberOffset'not in part:
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
    lines.append('const ModuleChoice crateChoices[] = {')
    for index,row in enumerate(manifest['modules'],96):lines.append('    { %du, &%s, %du, %du },'%(row['typeId'],ident(row['name']),row['nativeSize'],index))
    lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectCrate.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectCrate.h'
    else:path.write_text(output)
    print('PASS complete CrateCollide/Money/Unit/Veterancy/Attribute/Random/Salvage/Spawn contracts'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
