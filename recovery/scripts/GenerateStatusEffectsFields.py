"""Generate complete EMP/suppression contracts from 28 fixed original methods."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/status-effects-source'
SPECS=[('EMPUpdateModuleData',3543149950,40,143,[('Lifetime',8,'Time','0s'),('StartFadeDelay',12,'Time','0s'),('DisabledDuration',16,'Time','0s'),('StartScale',20,'Float','1'),('TargetScaleMin',24,'Float','1'),('TargetScaleMax',28,'Float','1')],[('StartColor',None),('EndColor',None)]),('SuppressionUpdateModuleData',3170589623,56,144,[('UpdateDelay',8,'Time','1s'),('Suppressability',12,'Float','0'),('SuppressionDuration',16,'Time','3s'),('AttributeModifierSuppressed',20,'Reference',None),('AttributeModifierForceMove',24,'Reference',None),('IgnoreSuppressionObjectStatus',28,'ObjectStatus',None)],[]),('Color3f',0,12,0,[(n,4*i,'Float',None) for i,n in enumerate(['r','g','b'])],[])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==28
assert [(m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in SPECS[:2]]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
lines=['// Complete EMP/suppression contracts; original blob '+BLOB+'.']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'));actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField statusEffects'+name+'Fields[] = {']
 for n,o,k,d in fields:lines+=['    { %s, %du, ModuleValue::%s, %s, nullptr },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d))]
 lines+=['};']
lines+=['const ModuleFieldSet statusEffectsFieldSets[] = {']+[f'    {{ statusEffects{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in [SPECS[2]]+SPECS[:2]:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='&spawnUpdateModuleData' if key==1 else 'nullptr';mode=0
  if kind==143 and off in [32,36]:mode=4;nested='&statusEffectsColor3f'
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==len(fields)+len(children)+(1 if kind else 0),(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken statusEffects'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout statusEffects{name} = {{ statusEffects{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice statusEffectsChoices[] = {']+[f'    {{ {t}u, &statusEffects{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS[:2]]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectStatusEffects.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectStatusEffects.h'
else:path.write_text(output)
print('PASS complete EMP/suppression contracts and 28 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
