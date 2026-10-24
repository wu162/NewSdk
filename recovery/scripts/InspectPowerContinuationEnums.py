"""Read-only PE evidence for the next complete ability/routing/OCL groups."""
import hashlib,json,os
from pathlib import Path
from GenerateUpgradeFields import ROOT,BLOB,enum_values
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode()
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();digest=hashlib.sha256(pe).hexdigest()
assert digest=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
counts={'TriggerType':3,'ConditionsType':3,'SpecialAbilityUpdateOptionsType':31,'SPMovementType':3,'OCLCreateLocType':15}
tables={name:enum_values(source,pe,name) for name in counts}
for name,count in counts.items():assert len(tables[name])==count,(name,tables[name])
output={'status':'read-only original PE evidence; no implementation/support claim','sourceBlob':BLOB,'peSha256':digest,'sourceCommit':os.environ.get('GITHUB_SHA'),'runId':os.environ.get('GITHUB_RUN_ID'),'tables':tables}
path=ROOT/'recovery/evidence/gameobject-modules/power-continuation-enums.json';path.write_text(json.dumps(output,indent=2,sort_keys=True)+'\n')
print('PASS original Power continuation PE enums '+json.dumps({name:len(table) for name,table in tables.items()},sort_keys=True))
