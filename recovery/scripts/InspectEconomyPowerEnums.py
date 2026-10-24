"""Read-only original PE enum evidence for the complete economy/power group."""
import hashlib,json
from GenerateUpgradeFields import ROOT,BLOB,enum_values
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=source.encode()
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
groups={'PowerUpdateType':2,'AutoDepositFlagsType':2,'MoneyTransactionCategory':30}
tables={g:enum_values(source,pe,g) for g in groups}
for group,count in groups.items():
 assert len(tables[group])==count,(group,tables[group])
 assert sorted(tables[group].values())==list(range(count)),(group,tables[group])
evidence={'sourceBlob':BLOB,'peSha256':hashlib.sha256(pe).hexdigest(),**tables}
print('ECONOMY_POWER_ENUM_SOURCE '+json.dumps(evidence,sort_keys=True))

