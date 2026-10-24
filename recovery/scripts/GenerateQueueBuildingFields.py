"""Derive queue/parking and building/structure contracts from fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
GROUPS=[('queue-parking',47,'queueParking','GameObjectQueueParking.h',[
 ('QueueDockModuleData',2941001505,52,128,[(n,8+8*i,'String','') for i,n in enumerate(['DockStartBoneName','DockActionBoneName','DockEndBoneName','QueueBonePrefix','TurnTableBoneName'])]+[('DockingAnimation',48,'Enum',None)],[]),
 ('ParkingPlaceBehaviorModuleData',3921409083,56,129,[('NumRows',8,'Signed',None),('NumCols',12,'Signed',None),('ApproachHeight',16,'Float',None),('ParkInHangars',52,'Boolean',None),('DoorOpenTime',20,'Time','0s'),('DoorCloseTime',24,'Time','0s'),('LowersOntoParkingPlaceOnProduction',53,'Boolean','false'),('CanSetRallyPoint',54,'Boolean','true'),('DoorsUnpackOpen',55,'Boolean','true'),('QueueDistance',28,'Float','100'),('LimitedProductionAircraftSpots',32,'Signed','0'),('HelipadBoneName',36,'String','HeliPark01')],[('CanParkHereFilter',None),('DefaultRallyPointOffset',None)])]),
 ('building-structure',31,'buildingStructure','GameObjectBuildingStructure.h',[
 ('BuildingBehaviorModuleData',2170196321,40,130,[(n,8+8*i,'String',None) for i,n in enumerate(['NightWindowObject','FireWindowObject','GlowWindowObject','NightFireWindowObject'])],[]),
 ('StructureUnpackUpdateModuleData',2541716369,20,131,[('UnpackTime',8,'Time','0s'),('UnpackCompleteSound',12,'NullableReference',None),('OffsetHeightAboveWater',16,'Float','5.0')],[]),
 ('StructureSellUpdateModuleData',1318893585,16,132,[('SellTime',8,'Time','0s'),('ActiveModelCondition',12,'Enum',None)],[])])]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
for group,methodCount,prefix,filename,specs in GROUPS:
 evidence=ROOT/'recovery/evidence/gameobject-modules'/(group+'-source');manifest=json.loads((evidence/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==methodCount
 methods={};literals={}
 for meta in manifest['methods']:
  text=(evidence/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
  if source is not None:assert body_of(source,meta['signature'])==body
  methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
 assert [(r['name'],r['typeId'],r['nativeSize']) for r in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in specs]
 lines=['// Complete '+group+' contracts; original blob '+BLOB+'.']
 if group=='queue-parking':lines+=['const ModuleToken queueParkingCreateModuleDataTokens[] = {','    { 1u, 2769121946u, 0u, &dieBehaviorModuleData, 0u, 0u },','};','const ModuleLayout queueParkingCreateModuleData = { queueParkingCreateModuleDataTokens, 1u };']
 for name,typ,size,kind,fields,children in specs:
  body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'))
  actual=[]
  for line in body.splitlines():
   symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
  assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
  lines+=['const ModuleField '+prefix+name+'Fields[] = {']
  for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&aiModelConditionFlagType' if k=='Enum' else 'nullptr')]
  lines+=['};']
 lines+=['const ModuleFieldSet '+prefix+'FieldSets[] = {']+[f'    {{ {prefix}{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in specs]+['};']
 for name,typ,size,kind,fields,children in specs:
  body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for chunk in chunks:
   match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
   if not match or match[1]=='0':continue
   key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=0
   if key==1:nested='&queueParkingCreateModuleData' if kind==128 else '&spawnUpdateModuleData'
   if 'ObjectFilter' in chunk:mode=7
   if 'Vector3' in chunk:mode=2
   if kind==131 and off==12:mode=2
   records.append((key,tokenType,off,nested,mode,0))
  assert len(records)==1+len(fields)+len(children),(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
  lines+=['const ModuleToken '+prefix+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout {prefix}{name} = {{ {prefix}{name}Tokens, {len(records)}u }};']
 lines+=['const ModuleChoice '+prefix+'Choices[] = {']+[f'    {{ {t}u, &{prefix}{n}, {z}u, {k}u }},' for n,t,z,k,f,c in specs]+['};','']
 output='\n'.join(lines);path=ROOT/'recovery/src/Native'/filename
 if '--check' in sys.argv:assert path.read_text()==output,'Regenerate '+filename
 else:path.write_text(output)
 print('PASS complete '+group+' contracts and '+str(methodCount)+' original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
