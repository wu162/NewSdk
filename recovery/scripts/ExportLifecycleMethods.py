"""Export original lifetime/health/death methods without enabling runtime support."""
import hashlib
import json
import re
import sys
from pathlib import Path
from ExtractGameObjectEvidence import method
ROOT=Path(__file__).resolve().parents[2]
SOURCE_BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=source.encode()
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==SOURCE_BLOB
inventory=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text())
signatures=re.findall(r'\t(internal unsafe static void [^\n]+)',source)
literals={}
for line in source.splitlines():
    match=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
    if match:
        try:literals[match[1]]=bytes.fromhex(match[2]).rstrip(b'\0').decode('ascii')
        except UnicodeDecodeError:pass
names=['DeletionUpdateModuleData','DieOnProducerDeathUpdateModuleData','InvalidTargetsDeathUpdateModuleData','HeightDieUpdateModuleData','LoseHealthUpdateModuleData','LifetimeUpdateModuleData','DestroyDieModuleData']
if '--die-family' in sys.argv:
    names=['DieModuleData','DestroyDieModuleData','UpgradeDieModuleData','KeepObjectDieModuleData','CreateObjectDieModuleData','CrushDieModuleData','RefundDieModuleData']
if '--upgrade-family' in sys.argv:
    names=['UpgradeModuleData','PowerUpgradeModuleData','DelayedUpgradeModuleData','AttributeModifierUpgradeModuleData','InvisibilityUpgradeModuleData','CommandSetUpgradeModuleData','ExperienceScalarUpgradeModuleData','MaxHealthUpgradeModuleData','ArmorUpgradeModuleData','LevelUpUpgradeModuleData','AudioLoopUpgradeModuleData']
if '--upgrade-remaining' in sys.argv:
    names=["SubStructureBuildUpgradeModuleData","ModelConditionUpgradeModuleData","ReplaceSelfUpgradeModuleData","SubObjectsUpgradeModuleData","ObjectCreationUpgradeModuleData","RemoveUpgradeUpgradeModuleData","StatusBitsUpgradeModuleData","UnpauseSpecialPowerUpgradeModuleData","CostModifierUpgradeModuleData","TechUpgradeModuleData","GeometryUpgradeModuleData"]
if '--production-family' in sys.argv:
    names=['ProductionUpdateModuleData','DefaultProductionExitUpdateModuleData','QueueProductionExitUpdateModuleData']
if '--create-family' in sys.argv:
    names=['GrantUpgradeCreateModuleData','InheritUpgradeCreateModuleData','ExperienceLevelCreateModuleData','ActivateSpecialPowerOnCreate','EvaAnnounceClientCreateModuleData']
if '--slaved-family' in sys.argv:
    names=['SlavedUpdateModuleData']
if '--tech-receiver' in sys.argv:
    names=['TechUpgradeReceiverModuleData']
if '--vision-family' in sys.argv:
    names=['CaptureBehaviorModuleData','UnifiedControlBehaviorModuleData','RadarUpdateModuleData','UnitVisionBehaviorModuleData','RadarMarkerClientUpdateModuleData','BeaconClientUpdateModuleData','SelectiveVisibilityBehaviorModuleData','InvisibilityBehaviorModuleData','InvisibilityBroadcastUpdateModuleData','UnifiedShroudRevealBehaviorModuleData']
if '--weapon-behaviors' in sys.argv:
    names=['ChargeWeaponUpdateModuleData','FireWeaponUpdateModuleData','AimWeaponBehaviorModuleData','LaserUpdateModuleData','LaserScatterUpdateModuleData','LinearDamageUpdateModuleData','SecondaryDamageTrackerModuleData','TrackDamageModuleData']
if '--beam-states' in sys.argv:
    names=['LaserStateModuleData','SweepingLaserStateModuleData','StreamStateModuleData']
if '--motion-family' in sys.argv:
    names=['ProjectileUpdateModuleData','DynamicsUpdateModuleData','FreefallUpdateModuleData','EnableDynamicsCollideModuleData']
out=ROOT/('recovery/evidence/gameobject-modules/motion-source' if '--motion-family' in sys.argv else 'recovery/evidence/gameobject-modules/beam-state-source' if '--beam-states' in sys.argv else 'recovery/evidence/gameobject-modules/weapon-behavior-source' if '--weapon-behaviors' in sys.argv else 'recovery/evidence/gameobject-modules/vision-source' if '--vision-family' in sys.argv else 'recovery/evidence/gameobject-modules/tech-receiver-source' if '--tech-receiver' in sys.argv else 'recovery/evidence/gameobject-modules/slaved-source' if '--slaved-family' in sys.argv else 'recovery/evidence/gameobject-modules/create-source' if '--create-family' in sys.argv else 'recovery/evidence/gameobject-modules/production-source' if '--production-family' in sys.argv else 'recovery/evidence/gameobject-modules/upgrade-remaining-source' if '--upgrade-remaining' in sys.argv else 'recovery/evidence/gameobject-modules/upgrade-source' if '--upgrade-family' in sys.argv else 'recovery/evidence/gameobject-modules/die-source' if '--die-family' in sys.argv else 'recovery/evidence/gameobject-modules/lifecycle-source')
out.mkdir(exist_ok=True)
report={'status':'source evidence only; runtime support and DLL validation are tracked separately in GameObject validation.json','sourceBlob':SOURCE_BLOB,'modules':[],'helpers':[]}
selected={}
def add(signature):
    if signature not in selected:selected[signature]=method(source,signature)
for name in names:
    key,row=next((k,v) for k,v in inventory['marshal'].items() if v['name']==name)
    item={'name':name,'typeId':int(key),'nativeSize':row['nativeSize'],'methods':[]}
    for operation in ['marshal','token','endian']:
        add(inventory[operation][key]['helperSignature'])
    for signature in signatures:
        if re.search(r'[(,] '+re.escape(name)+r'\* @object[,)]',signature) or '('+name+'* @object,' in signature:
            add(signature)
    report['modules'].append(item)
# Include called concrete bases and named template helpers, preserving exact overloads.
queue=list(selected)
while queue:
    signature=queue.pop()
    body,_=selected[signature]
    required=[]
    for call in re.finditer(r'\b((?:Marshaler_002E(?:Marshal(?:SinglePolymorphic|PolymorphicType)?|EndianCorrect(?:PolymorphicType)?)|Tokenizer_002ETokenize(?:SinglePolymorphic|PolymorphicType)?)(?:_\w+)?)\(([^;\n]+)',body):
        name,args=call.groups()
        if name in ['Marshaler_002EMarshal','Marshaler_002EEndianCorrect','Tokenizer_002ETokenize']:
            types=re.findall(r'\(([\w.]+)\*\)',args)
            for typ in types:
                required.extend(s for s in signatures if s.startswith('internal unsafe static void '+name+'(') and (', '+typ+'* @object,' in s or '('+typ+'* @object,' in s))
        else:
            required.extend(s for s in signatures if s.startswith('internal unsafe static void '+name+'('))
    for helper in required:
        if helper not in selected:
            add(helper);queue.append(helper)
    assert len(selected)<150,'Unexpected dependency expansion'
for index,(signature,(body,line)) in enumerate(selected.items()):
    filename='%03d.txt'%index
    decoded={key:value for key,value in literals.items() if key in body}
    digest=hashlib.sha256(body.encode()).hexdigest()
    text='// Original blob '+SOURCE_BLOB+'; line '+str(line)+'; body SHA256 '+digest+'\n// Literals: '+json.dumps(decoded,ensure_ascii=False)+'\n'+body+'\n'
    (out/filename).write_text(text,encoding='utf-8')
    meta={'file':filename,'signature':signature,'line':line,'bodySha256':digest,'literals':list(decoded.values())}
    matched=False
    for item in report['modules']:
        if '('+item['name']+'* @object,' in signature or ', '+item['name']+'* @object,' in signature or '_003A_003A'+item['name']+'_002Cstruct_' in signature:item['methods'].append(meta);matched=True
    if not matched:report['helpers'].append(meta)
assert all(item['methods'] for item in report['modules']), 'Missing exact module methods'
assert sum(len(item['methods']) for item in report['modules'])+len(report['helpers'])==len(selected), 'Duplicate method owner index'
(out/'manifest.json').write_text(json.dumps(report,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
print('PASS original module source evidence:',len(names),'module source contracts;',len(selected),'exact methods; no runtime support claimed')

if '--tech-receiver' in sys.argv:
    from GenerateUpgradeFields import enum_values
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    (out/'enum-source.json').write_text(json.dumps({'sourceBlob':SOURCE_BLOB,'ReceiverDistanceType':enum_values(source,pe,'ReceiverDistanceType')},indent=2)+'\n',encoding='utf-8')


if '--vision-family' in sys.argv:
    from GenerateUpgradeFields import enum_values
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    (out/'enum-source.json').write_text(json.dumps({'sourceBlob':SOURCE_BLOB,**{g:enum_values(source,pe,g)for g in ['UnifiedControlType','RelationshipType']}},indent=2)+'\n',encoding='utf-8')

if any(flag in sys.argv for flag in ['--weapon-behaviors','--beam-states','--motion-family']):
    from GenerateUpgradeFields import enum_values
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    groups=sorted(set(re.findall(r'W4([A-Za-z0-9]+)_0040', '\n'.join(body for body,line in selected.values()))))
    maps={}
    for group in groups:
        try: maps[group]=enum_values(source,pe,group)
        except StopIteration: pass
    (out/'enum-source.json').write_text(json.dumps({'sourceBlob':SOURCE_BLOB,**maps},indent=2)+'\n',encoding='utf-8')

if '--weapon-behaviors' in sys.argv:
    symbol='DataInitializers_002ESECONDS_PER_LOGICFRAME_REAL'
    rows=[{'line':i+1,'text':line.strip()}for i,line in enumerate(source.splitlines())if any(key in line for key in ['SECONDS_PER_LOGICFRAME_REAL','LOGICFRAMES_PER_SECONDS_REAL'])]
    (out/'constants-source.json').write_text(json.dumps({'sourceBlob':SOURCE_BLOB,'logicFrameReferences':rows},indent=2)+'\n',encoding='utf-8')
