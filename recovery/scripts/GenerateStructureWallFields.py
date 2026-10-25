"""Generate complete wall/bridge/placement contracts from 60 original method fingerprints."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/structure-wall-source'
SPECS=[('WallBuilderBehaviorModuleData',2509263754,216,137,[('MaxNumSegments',8,'Signed','5'),('SecondaryModelConditions',12,'ModelCondition','WALL_SEGMENT'),('SecondaryObjectStatus',72,'ObjectStatus','WALL_SEGMENT'),('StartInSecondaryModel',212,'Boolean','false')],[('ConnectingObjectFilter',None)]),('BridgeBuilderModuleData',2734068631,52,138,[('EndCap',8,'Reference',None),('EndCap2',12,'NullableReference',None),('EndCapLen',16,'Float',None),('CenterPiece',20,'Reference',None),('CenterPieceLen',24,'Float',None),('CenterPieceB',28,'NullableReference',None),('CenterPieceBLen',32,'NullableFloat',None),('Width',36,'Float',None),('GateHouse',40,'Reference',None),('GateHouse2',44,'NullableReference',None),('DamageNeighborSegementsAtHealthPercent',48,'Percent','66.66666')],[]),('StructurePlacementBehaviorModuleData',3922835435,56,139,[],[('RestrictedAreaShape',None)]),('Shape',0,48,0,[('Type',0,'Enum','SPHERE'),('ContactPointGeneration',4,'Enum','NONE'),('Name',8,'String',None),('Active',44,'Boolean','true'),('UsedForHealthBox',45,'Boolean','true'),('MajorRadius',16,'Float','1.0'),('MinorRadius',20,'Float','1.0'),('Other',24,'String',None),('Height',32,'Float','1.0'),('FrontAngle',36,'Angle',None)],[('Offset',None)])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==60
assert [(m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in SPECS[:3]]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
lines=['// Complete wall/bridge/placement contracts; original blob '+BLOB+'.']
# Reuse the complete root Shape enums already validated against this fixed original blob/PE.
rootFields=(ROOT/'recovery/src/Native/GameObjectFields.h').read_text()
for group in ['GeometryShapeType','ContactPointGeneration']:
 rows=re.findall(r'\{ "'+group+r'", "([^"]+)", (-?\d+) \}',rootFields);assert len(rows)==(4 if group=='GeometryShapeType' else 5)
 fallback=re.search(r'\{ "'+group+r'", (-?\d+) \}',rootFields);assert fallback
 lines+=['const EnumNameValue structureWall'+group+'Names[] = {']+['    { %s, %s },'%(json.dumps(n),v) for n,v in rows]+['};','const AIEnum structureWall%sEnum = { structureWall%sNames, %du, %s, 0u };'%(group,group,len(rows),fallback[1])]
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'))
 actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 if fields:
  lines+=['const ModuleField structureWall'+name+'Fields[] = {']
  for n,o,k,d in fields:
   en={'Type':'structureWallGeometryShapeTypeEnum','ContactPointGeneration':'structureWallContactPointGenerationEnum'}.get(n)
   lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&'+en if en else 'nullptr')]
  lines+=['};']
lines+=['const ModuleFieldSet structureWallFieldSets[] = {']+[f'    {{ '+('structureWall'+n+'Fields' if f else 'nullptr')+f', {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in [SPECS[3]]+SPECS[:3]:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=0
  if key==1:nested='&spawnUpdateModuleData' if kind==138 else '&dieBehaviorModuleData'
  if 'ObjectFilter' in chunk:mode=8
  if 'Vector3' in chunk or (name=='Shape' and off==40):mode=2
  if kind==139 and 'Shape*' in chunk:nested='&structureWallShape'
  if kind==138 and off in [12,28,32,44]:mode=2
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==len(fields)+len(children)+(1 if kind else 0),(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken structureWall'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout structureWall{name} = {{ structureWall{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice structureWallChoices[] = {']+[f'    {{ {t}u, &structureWall{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS[:3]]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectStructureWall.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectStructureWall.h'
else:path.write_text(output)
print('PASS complete wall/bridge/placement contracts and 60 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
