"""Read-only original PE map for complete model-condition monitor recovery."""
import hashlib,json
from GenerateUpgradeFields import ROOT,BLOB,enum_values
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=source.encode()
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
table=enum_values(source,pe,'MonitorConditionActionType')
assert len(table)==1,table
evidence={'sourceBlob':BLOB,'peSha256':hashlib.sha256(pe).hexdigest(),'MonitorConditionActionType':table}
print('MODEL_CONDITION_ENUM_SOURCE '+json.dumps(evidence,sort_keys=True))

