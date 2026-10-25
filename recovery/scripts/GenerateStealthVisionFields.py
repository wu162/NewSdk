"""Generate complete detector/invisibility contracts from 58 fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/stealth-vision-source'
SPECS=[('StealthDetectorUpdateModuleData',1951832746,156,145,[('DetectionRate',36,'Time','0.4s'),('DetectionRange',40,'Float','0.0'),('InitiallyDisabled',152,'Boolean','false'),('PingSound',44,'Reference',None),('LoudPingSound',48,'Reference',None),('IRBeaconParticleSys',52,'Reference',None),('IRParticleSys',56,'Reference',None),('IRBrightParticleSys',60,'Reference',None),('IRGridParticleSys',64,'Reference',None),('IRParticleSysBone',68,'String',None),('ExtraRequiredKindOf',76,'Bits',None),('ExtraForbiddenKindOf',112,'Bits',None),('CanDetectWhileGarrisoned',153,'Boolean','false'),('CanDetectWhileContained',154,'Boolean','false'),('RequiredUpgrade',148,'Reference',None)],[]),('InvisibilityUpdateModuleData',895696920,140,146,[('InvisibilityTemplate',8,'Reference',None),('UpdatePeriod',12,'Time','1s'),('RequiredNearbyObjectRange',16,'Float','0'),('NamedVoiceNameToUseAsVoiceMoveToStealthyArea',20,'NullableExactHash',None),('NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea',24,'NullableExactHash',None)],[('RequiresNearbyObjectFilter',None)])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==58
assert [(m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in SPECS]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
lines=['// Complete detector/invisibility contracts; original blob '+BLOB+'.','// Invisibility voice hashes are nullable exact FastHashFn values; the family marshals them explicitly.','const char* stealthVisionVoiceNames[] = {']+['    '+json.dumps(n)+',' for n,o,k,d in SPECS[1][4] if k=='NullableExactHash']+['};']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'));actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField stealthVision'+name+'Fields[] = {']
 for n,o,k,d in fields:
  if k=='NullableExactHash':continue
  lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&aiKindOfType' if k=='Bits' else 'nullptr')]
 lines+=['};']
lines+=['const ModuleFieldSet stealthVisionFieldSets[] = {']+[f'    {{ stealthVision{n}Fields, {sum(k!="NullableExactHash" for a,o,k,d in f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=0
  if key==1:nested='&upgradeUpgradeModuleData' if kind==145 else '&spawnUpdateModuleData'
  if kind==146 and off in [20,24]:mode=2
  if kind==146 and off==28:mode=8
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==len(fields)+len(children)+1,(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken stealthVision'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout stealthVision{name} = {{ stealthVision{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice stealthVisionChoices[] = {']+[f'    {{ {t}u, &stealthVision{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectStealthVision.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectStealthVision.h'
else:path.write_text(output)
print('PASS complete detector/invisibility contracts and 58 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
