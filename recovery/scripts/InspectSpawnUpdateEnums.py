"""Read-only PE enum evidence for the complete Spawn/OCL Update capability group."""
import hashlib,json
from GenerateUpgradeFields import ROOT,BLOB,enum_values
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
evidence={'sourceBlob':BLOB,'peSha256':hashlib.sha256(pe).hexdigest(),**{g:enum_values(source,pe,g) for g in ['DamageType','DeathType','OCLMonitorOption','SpawnCrateUpdateFlagsType']}}
print('SPAWN_UPDATE_ENUM_SOURCE '+json.dumps(evidence,sort_keys=True))

