"""Generate complete economy/power contracts from 46 original method fingerprints."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/economy-power-source'
SPECS=[('PowerUpdateModuleData',4100895620,24,133,[('UpdatePeriod',8,'Time','1s'),('ReloadTime',12,'Time','0s'),('Duration',16,'Time','0s'),('Type',20,'Enum','INVALID')],[]),('PlayerPowerManagerBehaviorModuleData',771889415,16,134,[('SpecialPowerTemplate',8,'Reference',None),('OnAvailableEvaEvent',12,'Reference','')],[]),('AutoDepositUpdateModuleData',1300449569,48,135,[('DepositInterval',8,'Time','0.0s'),('GiveNoXP',44,'Boolean','false'),('OnlyWhenGarrisoned',45,'Boolean','false'),('Flags',12,'Bits','NONE'),('TransactionCategory',16,'Enum','AUTO_DEPOSIT')],[('InitialCaptureBonus',None),('Deposit',None),('ResourceCap',None)]),('DistributedMoneyBehaviorModuleData',1023139366,20,136,[('Capacity',8,'Unsigned','1'),('OnDieSpawnPercentage',12,'Percent','1.0'),('RedistributeMoneyOnDeathType',16,'Enum','SUICIDED')],[]),('MoneyTransaction',0,8,0,[('Account',0,'Unsigned',None),('Amount',4,'Unsigned',None)],[])]
manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB;assert len(manifest['methods'])==46
assert [(m['name'],m['typeId'],m['nativeSize']) for m in manifest['modules']]==[(n,t,z) for n,t,z,k,f,c in SPECS[:4]]
source=None
if '--exported-evidence' not in sys.argv:
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
methods={};literals={}
for meta in manifest['methods']:
 text=(EVIDENCE/meta['file']).read_text();body=body_of(text,meta['signature']);assert hashlib.sha256((body+'\n').encode()).hexdigest()==meta['bodySha256'],meta['file']
 if source is not None:assert body_of(source,meta['signature'])==body
 methods[meta['signature']]=body;literals.update(json.loads(text.splitlines()[1].removeprefix('// Literals: ')))
enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
assert enums['peSha256']=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
if source is not None:
 from GenerateUpgradeFields import enum_values
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==enums['peSha256']
 for group in ['PowerUpdateType','AutoDepositFlagsType','MoneyTransactionCategory']:assert enum_values(source,pe,group)==enums[group],group
enumLines=['// Original Tokenizer PE SHA256 '+enums['peSha256']+'.','// Complete original name/value tables; bitset unknown index and single enum fallback stay distinct.']
for group,stem,count,unknown,bits in [('PowerUpdateType','Type',2,0,0),('AutoDepositFlagsType','Flags',2,1,2),('MoneyTransactionCategory','Category',30,22,0)]:
 values=enums[group];assert len(values)==count
 enumLines+=['const EnumNameValue economyPower'+stem+'Names[] = {']+['    { %s, %du },'%(json.dumps(n),v) for n,v in values.items()]+['};','const AIEnum economyPower%sEnum = { economyPower%sNames, %du, %d, %du };'%(stem,stem,count,unknown,bits)]
enumLines.append('');enumOutput='\n'.join(enumLines);enumPath=ROOT/'recovery/src/Native/GameObjectEconomyPowerEnums.h'
if '--check' in sys.argv:assert enumPath.read_text()==enumOutput,'Regenerate GameObjectEconomyPowerEnums.h'
else:enumPath.write_text(enumOutput)
lines=['// Complete economy/power contracts; original blob '+BLOB+'.','#include "GameObjectEconomyPowerEnums.h"','const AIEnum economyPowerDeathTypeEnum = { lifecycleDeathNames, 34u, 33, 0u };']
for name,typ,size,kind,fields,children in SPECS:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'*'))
 actual=[]
 for line in body.splitlines():
  symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if symbols:actual.append((literals[symbols[0]],literals[symbols[1]] if len(symbols)>1 else None))
 assert actual==[(n,d) for n,o,k,d in fields]+children,(name,actual)
 lines+=['const ModuleField economyPower'+name+'Fields[] = {']
 for n,o,k,d in fields:
  en={'Type':'economyPowerTypeEnum','Flags':'economyPowerFlagsEnum','TransactionCategory':'economyPowerCategoryEnum','RedistributeMoneyOnDeathType':'economyPowerDeathTypeEnum'}.get(n)
  lines+=['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&'+en if en else 'nullptr')]
 lines+=['};']
lines+=['const ModuleFieldSet economyPowerFieldSets[] = {']+[f'    {{ economyPower{n}Fields, {len(f)}u }},' for n,t,z,k,f,c in SPECS]+['};']
for name,typ,size,kind,fields,children in [SPECS[4]]+SPECS[:4]:
 body=next(b for sig,b in methods.items() if sig.startswith('internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'*'));chunks=body.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)ptr = (-?\d+);\s*\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 0;nested='nullptr';mode=stride=0
  if key==1:nested='&queueParkingCreateModuleData' if kind==136 else '&spawnUpdateModuleData'
  if 'List_003CSageBinaryData_003A_003AMoneyTransaction' in chunk:nested='&economyPowerMoneyTransaction';mode,stride=1,8
  records.append((key,tokenType,off,nested,mode,stride))
 assert len(records)==len(fields)+len(children)+(1 if kind else 0),(name,len(records));assert re.search(r'\*\(int\*\)ptr = 0;\s*\(\(int\*\)ptr\)\[1\] = 0;\s*\(\(int\*\)ptr\)\[2\] = 0;',chunks[-1])
 lines+=['const ModuleToken economyPower'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout economyPower{name} = {{ economyPower{name}Tokens, {len(records)}u }};']
lines+=['const ModuleChoice economyPowerChoices[] = {']+[f'    {{ {t}u, &economyPower{n}, {z}u, {k}u }},' for n,t,z,k,f,c in SPECS[:4]]+['};','']
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectEconomyPower.h'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectEconomyPower.h'
else:path.write_text(output)
print('PASS complete economy/power contracts and 46 original method fingerprints'+(' (exported evidence only)' if source is None else ' (original blob verified)'))
