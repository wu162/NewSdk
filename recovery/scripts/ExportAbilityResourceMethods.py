"""Bounded exact original methods for the resource theft and OCL create-enter group.
Source preparation only: no new module is enabled or counted by this script.
"""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';E=ROOT/'recovery/evidence/gameobject-modules/ability-resource-source'
s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB
TYPES=[4099311701,952407768];literals={};signatures={}
for line in s.splitlines():
 sig=line.strip()
 if sig.startswith('internal unsafe static void '):signatures.setdefault(sig.split('(')[0].split()[-1],[]).append(sig)
 m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
 if m:
  try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
  except UnicodeDecodeError:pass
manifest={'status':'original source evidence for resource theft and OCL create-enter','sourceBlob':BLOB,'sharedParentManifest':'../special-ability-source/manifest.json','modules':[],'methods':[]};bodies={};queue=[]
for typ in TYPES:
 meta=inv['marshal'][str(typ)];manifest['modules'].append({'name':meta['name'],'typeId':typ,'nativeSize':meta['nativeSize']})
 for family in ['marshal','endian','token']:queue.append(inv[family][str(typ)]['helperSignature'])
 name=meta['name']
 for sigs in signatures.values():
  for sig in sigs:
   if (sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*') or sig.startswith('internal unsafe static void Marshaler_002EEndianCorrect('+name+'*') or sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*')):queue.append(sig)
for sigs in signatures.values():
 for sig in sigs:
  if 'Vector3* @object' in sig and any(sig.startswith('internal unsafe static void '+op) for op in ['Marshaler_002EMarshal(', 'Marshaler_002EEndianCorrect(', 'Tokenizer_002ETokenize(']):queue.append(sig)
while queue:
 sig=queue.pop(0)
 if sig in bodies:continue
 body=body_of(s,sig);bodies[sig]=body
 # Helper names are exact decorated names, not native size/name substring guesses.
 for helper in re.findall(r'\b((?:Marshaler_002EMarshal_|Marshaler_002EEndianCorrect_|Tokenizer_002ETokenize_)\w+)\(',body):queue.extend(signatures.get(helper,[]))
exports={}
for index,(sig,body) in enumerate(bodies.items()):
 file=f'{index:03}.txt';sha=hashlib.sha256((body+'\n').encode()).hexdigest();used={x:literals[x] for x in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',body) if x in literals};exports[file]='// Original blob '+BLOB+'; body SHA256 '+sha+'\n// Literals: '+json.dumps(used,ensure_ascii=False)+'\n'+body+'\n';manifest['methods'].append({'file':file,'signature':sig,'bodySha256':sha})
if '--check' in sys.argv:
 assert json.loads((E/'manifest.json').read_text())==manifest
 for file,text in exports.items():assert (E/file).read_text()==text
else:
 E.mkdir(parents=True,exist_ok=True);(E/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 for file,text in exports.items():(E/file).write_text(text)
print('PASS source preparation only: two ability-resource modules, '+str(len(exports))+' complete original method fingerprints')


