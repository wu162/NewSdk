"""Read-only PE map probe for the next complete assistance capability group."""
import hashlib,json
from GenerateUpgradeFields import ROOT,BLOB,enum_values
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
print('ASSIST_ENUM_SOURCE '+json.dumps({'sourceBlob':BLOB,**{g:enum_values(source,pe,g)for g in ['AssistanceHintType','WeaponSlotType']}},sort_keys=True))
