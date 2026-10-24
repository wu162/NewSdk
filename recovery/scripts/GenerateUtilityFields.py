"""Complete remaining scalar/string utility modules, pinned to original methods and wrappers."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
SETS=[(783731100,'StorePurchasedUpgradeBehaviorModuleData',12,'BehaviorModuleData','lifecycleBehaviorModuleData',[('MaxPlayerUpgrades',8,'Unsigned','9999')]),(880602396,'AIGateUpdateModuleData',16,'UpdateModuleData','spawnUpdateModuleData',[('TriggerWidthX',8,'Float','0'),('TriggerWidthY',12,'Float','0')]),(2018429368,'CombinedInfoModuleData',16,'CreateModuleData','createCreateModuleData',[('DockingBone',8,'String',None)]),(4216826759,'TimedOpacityUpdateModuleData',16,'UpdateModuleData','spawnUpdateModuleData',[('Opacity',8,'Float','0.5'),('Duration',12,'Time','5s')])]
def generate():
 s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB;literals={};lines=['// Complete scalar/string utilities; original blob '+BLOB+'.']
 for line in s.splitlines():
  m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
  if m:
   try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
   except UnicodeDecodeError:pass
 for idx,(typ,n,size,parent,layout,fields) in enumerate(SETS):
  assert inv['marshal'][str(typ)]['name']==n and inv['marshal'][str(typ)]['nativeSize']==size
  mb=body_of(s,f'internal unsafe static void Marshaler_002EMarshal(INode* node, {n}* @object, Tracker* state)');eb=body_of(s,f'internal unsafe static void Marshaler_002EEndianCorrect({n}* @object, EndianHelper* state)');tb=body_of(s,f'internal unsafe static void Tokenizer_002ETokenize(void** table, {n}* @object, Tracker* state)')
  actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d in fields];assert 'Marshaler_002EMarshal(node, (ModuleData*)@object, state);' in mb
  assert all('((byte*)@object + '+str(o)+')' in eb for o in [4]+[f[1] for f in fields])
  # Confirm dispatcher helpers retain the complete direct method (no inline derived contract).
  for family in ['marshal','endian','token']:
   wrapper=body_of(s,inv[family][str(typ)]['helperSignature']);assert n+'*' in wrapper
  lines+=['const ModuleField utility'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, nullptr },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d)) for n,o,k,d in fields]+['};']
  records=[]
  for i,c in enumerate(tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]):
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;kind=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff;o=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 0
   if i==0:assert key==1 and '('+parent+'*)@object' in c
   records.append((key,kind,off,'&'+layout if i==0 else 'nullptr'))
  assert [x[2] for x in records]==[0]+[o for n,o,k,d in fields] and len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1])
  lines+=['const ModuleToken utility'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, 0u, 0u },'%r for r in records]+['};',f'const ModuleLayout utility{n} = {{ utility{n}Tokens, {len(records)}u }};']
 lines+=['const ModuleFieldSet utilityFieldSets[] = {']+[f'    {{ utility{n}Fields, {len(f)}u }},' for typ,n,size,parent,layout,f in SETS]+['};','const ModuleChoice utilityChoices[] = {']+[f'    {{ {typ}u, &utility{n}, {size}u, {225+i}u }},' for i,(typ,n,size,parent,layout,f) in enumerate(SETS)]+['};',''];return '\n'.join(lines)
if __name__=='__main__':
 output=generate();p=ROOT/'recovery/src/Native/GameObjectUtility.h'
 if '--check' in sys.argv:assert p.read_text()==output,'Regenerate complete utility modules'
 else:p.write_text(output)
 print('PASS four complete scalar/string utility modules, exact Marshal/Endian/Token and dispatcher helper contracts')
