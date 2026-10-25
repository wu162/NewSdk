"""Bounded exact original methods for the complete remaining W3D Draw family and complete direct dependencies.
Source evidence validation is independent of implementation/support status.
"""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';E=ROOT/'recovery/evidence/gameobject-modules/draw-source'
s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB
TYPES=[803073101,1121390653,1322199627,1409388128,1610744897,2323717438,2615586235,2756454809];literals={};signatures={}
for line in s.splitlines():
 sig=line.strip()
 if sig.startswith('internal unsafe static void '):signatures.setdefault(sig.split('(')[0].split()[-1],[]).append(sig)
 m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
 if m:
  try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
  except UnicodeDecodeError:pass
manifest={'status':'original method evidence only; W3D Draw family remains rejected','sourceBlob':BLOB,'dependencyClosure':'direct methods and decorated helpers; existing ModuleData** dispatch is the source-inventory boundary','modules':[],'methods':[]};bodies={};queue=[]
for typ in TYPES:
 meta=inv['marshal'][str(typ)];manifest['modules'].append({'name':meta['name'],'typeId':typ,'nativeSize':meta['nativeSize']})
 for family in ['marshal','endian','token']:queue.append(inv[family][str(typ)]['helperSignature'])
 name=meta['name']
 for sigs in signatures.values():
  for sig in sigs:
   if (sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*') or sig.startswith('internal unsafe static void Marshaler_002EEndianCorrect('+name+'*') or sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*')):queue.append(sig)
while queue:
 sig=queue.pop(0)
 if sig in bodies:continue
 body=body_of(s,sig);bodies[sig]=body
 # Helper names are exact decorated names, not native size/name substring guesses.
 for helper in re.findall(r'\b((?:Marshaler_002EMarshal_|Marshaler_002EEndianCorrect_|Tokenizer_002ETokenize_)\w+)\(',body):queue.extend(signatures.get(helper,[]))
 for name in set(re.findall(r'\(([A-Za-z][A-Za-z0-9_]*)\*+\)',body)):
  for sigs in signatures.values():
   for direct in sigs:
    if name=='ModuleData' and 'ModuleData**' in direct:continue # Existing module dispatcher is a separately inventoried boundary.
    if re.search(r'(?:\(|, )'+re.escape(name)+r'\*+ (?:@object|pointer)\b',direct) and any(direct.startswith('internal unsafe static void '+op) for op in ['Marshaler_002EMarshal(', 'Marshaler_002EEndianCorrect(', 'Tokenizer_002ETokenize(']):queue.append(direct)
from GenerateUpgradeFields import enum_values
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
enumNames=sorted(set(re.findall(r'EnumNameGetter_0040W4([A-Za-z][A-Za-z0-9]*)_0040', '\n'.join(bodies.values()))))
manifest['peEnums']={name:enum_values(s,pe,name) for name in enumNames}
for line in s.splitlines():
 if line.strip().startswith('internal') and 'FastHashInternal_002EFastHashFn(' in line:
  sig=line.strip();bodies[sig]=body_of(s,sig)
manifest['durationConstantEvidence']=[line.strip() for line in s.splitlines() if 'LOGICFRAMES_PER_MSEC_REAL =' in line]
# Preserve archive numbering independently of hash-randomized dependency traversal.
# New archives use sorted signatures; existing archives retain their verified signature order.
if (E/'manifest.json').exists():
 old=json.loads((E/'manifest.json').read_text())
 order=[x['signature'] for x in old['methods']]
 assert set(order)<=set(bodies), 'Previously captured W3D method disappeared'
 order += sorted(set(bodies)-set(order))
 bodies={sig:bodies[sig] for sig in order}
else:
 bodies=dict(sorted(bodies.items()))
exports={}

for index,(sig,body) in enumerate(bodies.items()):
 file=f'{index:03}.txt';sha=hashlib.sha256((body+'\n').encode()).hexdigest();used={x:literals[x] for x in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',body) if x in literals};exports[file]='// Original blob '+BLOB+'; body SHA256 '+sha+'\n// Literals: '+json.dumps(used,ensure_ascii=False)+'\n'+body+'\n';manifest['methods'].append({'file':file,'signature':sig,'bodySha256':sha})
if '--check' in sys.argv:
 assert json.loads((E/'manifest.json').read_text())==manifest
 for file,text in exports.items():assert (E/file).read_text()==text
else:
 E.mkdir(parents=True,exist_ok=True);(E/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 for file,text in exports.items():(E/file).write_text(text)
print('PASS W3D Draw family source preparation only, '+str(len(exports))+' complete original method fingerprints')




