"""Full scalar/filter/list SpecialPower derivations, including eleven parent-only types."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/special-power-derived-source'
# (typeId, nativeSize, own fields in original Marshal order, nullable Filter node/offset, strong reference text-child/offset)
SPECS=[(134128162,452,[],None,None),(597529935,452,[],None,None),(671107761,452,[],None,None),(968369663,452,[],None,None),(1411399584,452,[],None,None),(2083097860,452,[],None,None),(2384652488,452,[],None,None),(2427007323,452,[],None,None),(2645545868,452,[],None,None),(2893949514,452,[],None,None),(3598262113,452,[],None,None),
(1205836826,464,[('TargetFilter',452,'StringList',None),('SpyDuration',460,'Time','0s')],None,None),
(1391381229,464,[('TargetFilter',452,'StringList',None),('FreezeDuration',460,'Time','0s')],None,None),
(1503617237,456,[],('CombineWithObjectFilter',452),None),
(1311610569,460,[('PlayReturnVoiceFromRecalledUnit',456,'Boolean','true'),('MaxAircraftToRecall',452,'Signed','4')],None,None),
(3719921227,456,[('CommandSet',452,'AssetId',None)],None,None),
(831374550,464,[('GetObjectsCurrentlyAtStorePosition',460,'Boolean','false'),('StoreObjectsLinkID',452,'Signed','0'),('OCL',456,'Reference',None)],None,None),
(3535916994,472,[('InvisibilityTemplate',452,'Reference',None),('BroadcastRadius',456,'Float','0'),('Duration',460,'Time','0s'),('Permanent',468,'Boolean','false')],('ObjectFilter',464),None),
(2806653772,468,[('Radius',452,'Float','0'),('TeleportLinkID',456,'Signed','0'),('OCL',460,'Reference',None),('TargetMarkerObjectRef',464,'AssetId',None)],None,None),
(2396208723,476,[(n,452+4*i,'Percent','0') for i,n in enumerate(['BuildCostBonus','BuildCostBonusAllies','BuildTimeBonus','BuildTimeBonusAllies','HarvesterBonus','HarvesterBonusAllies'])],None,None),
(3328368308,480,[('SlaveStatus',452,'ObjectStatus',None)],None,None),
(5980302,460,[],None,('Upgrade',452)),
(3109462554,468,[('Radius',452,'Float','0')],('AcceptObjectFilter',456),('UpgradeTemplate',460))]
source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
inventory=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inventory['sourceBlob']==BLOB
sourceLines=source.splitlines();literals={}
for line in sourceLines:
 m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
 if m:
  try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
  except UnicodeDecodeError:pass
manifest={'sourceBlob':BLOB,'parentManifest':'../special-power-base-source/manifest.json','modules':[],'methods':[]};exported={}
def original(sig):
 body=body_of(source,sig)
 if sig not in exported:
  file=f'{len(exported):03}.txt';sha=hashlib.sha256((body+'\n').encode()).hexdigest();used={m:literals[m] for m in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',body) if m in literals}
  exported[sig]=(file,'// Original blob '+BLOB+'; body SHA256 '+sha+'\n// Literals: '+json.dumps(used,ensure_ascii=False)+'\n'+body+'\n')
  manifest['methods'].append({'file':file,'signature':sig,'bodySha256':sha})
 return body
def contract(name,op):
 sig='internal unsafe static void '+op+'(INode* node, '+name+'* @object, Tracker* state)' if op=='Marshaler_002EMarshal' else 'internal unsafe static void '+op+'('+name+'* @object, EndianHelper* state)' if op=='Marshaler_002EEndianCorrect' else 'internal unsafe static void '+op+'(void** table, '+name+'* @object, Tracker* state)'
 return original(sig) if sig in source else None
lines=['// Complete SpecialPower scalar/filter/list derivations; original blob '+BLOB+'.']
for index,(typ,size,fields,filterInfo,listInfo) in enumerate(SPECS):
 meta=inventory['marshal'][str(typ)];name=meta['name'];assert meta['nativeSize']==size
 wrappers={family:original(inventory[family][str(typ)]['helperSignature']) for family in ['marshal','endian','token']}
 marshal=contract(name,'Marshaler_002EMarshal') or wrappers['marshal'];endian=contract(name,'Marshaler_002EEndianCorrect') or wrappers['endian'];token=contract(name,'Tokenizer_002ETokenize') or wrappers['token']
 # All concrete types have one complete SpecialPower parent contract.
 assert 'Marshaler_002EMarshal(node, (SpecialPowerModuleData*)' in marshal
 actual=[]
 for line in marshal.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 expected=[(n,d) for n,o,k,d in fields]+([filterInfo[:1]+(None,)] if filterInfo else [])+([listInfo[:1]+(None,)] if listInfo else [])
 assert actual==expected,(name,actual,expected)
 kind=165+index;fieldName='specialDerived'+name+'Fields'
 if fields:
  lines+=['const ModuleField '+fieldName+'[] = {']+['    { %s, %du, ModuleValue::%s, %s, nullptr },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d)) for n,o,k,d in fields]+['};']
 chunks=re.split(r'ptr\d* = \(ProcTableElement\*\)\(\(byte\*\)ptr\d* \+ 12\);',token);records=[]
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)(ptr\d*) = (-?\d+);\s*\(\(int\*\)\1\)\[1\] = (-?\d+);',chunk)
  if not match or match[2]=='0':continue
  key,tokenType=[int(x)&0xffffffff for x in match.groups()[1:]];off=re.search(r'\(byte\*\)(?:@object|ptr) \+ (\d+)',chunk);off=int(off[1]) if off else 0
  # An inline wrapper uses ptr2 as Token table and ptr as native object.
  mode=0;nested='&specialPowerSpecialPowerModuleData' if key==1 else 'nullptr'
  if key!=1 and any(o==off and k=='StringList' for n,o,k,d in fields):mode=6
  if key!=1 and filterInfo and off==filterInfo[1]:mode=7
  if key!=1 and listInfo and off==listInfo[1]:mode=3
  records.append((key,tokenType,off,nested,mode,0))
 assert len(records)==1+len(fields)+(filterInfo is not None)+(listInfo is not None),(name,records)
 assert records[0][:2]==(1,3472757011),(name,records[0])
 lines+=['const ModuleToken specialDerived'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout specialDerived{name} = {{ specialDerived{name}Tokens, {len(records)}u }};']
 manifest['modules'].append({'name':name,'typeId':typ,'nativeSize':size,'kind':kind,'tokenCount':len(records),'fields':fields,'filter':filterInfo,'strongList':listInfo})
lines+=['const ModuleFieldSet specialDerivedFieldSets[] = {']+['    { %s, %du },'%('specialDerived'+m['name']+'Fields' if m['fields'] else 'nullptr',len(m['fields'])) for m in manifest['modules']]+['};','const ModuleChoice specialDerivedChoices[] = {']+[f'    {{ {m["typeId"]}u, &specialDerived{m["name"]}, {m["nativeSize"]}u, {m["kind"]}u }},' for m in manifest['modules']]+['};','struct SpecialDerivedChildren {const char* filter;unsigned filterOffset;const char* strongList;unsigned listOffset;};','const SpecialDerivedChildren specialDerivedChildren[] = {']+['    { %s, %du, %s, %du },'%('nullptr' if not m['filter'] else json.dumps(m['filter'][0]),0 if not m['filter'] else m['filter'][1],'nullptr' if not m['strongList'] else json.dumps(m['strongList'][0]),0 if not m['strongList'] else m['strongList'][1]) for m in manifest['modules']]+['};','']
# New list contracts use the same full Tracker strong-reference helper but their
# IList Marshal/Endian/Token routines remain independently fingerprinted.
for typ in [5980302,3109462554]:
 b=original(inventory['marshal'][str(typ)]['helperSignature']) if typ==5980302 else contract(inventory['marshal'][str(typ)]['name'],'Marshaler_002EMarshal')
 helper=re.search(r'(Marshaler_002EMarshal_\w+)\(\(\(delegate\* unmanaged\[Thiscall, Thiscall\]<IntPtr, sbyte\*, IList\*>',b)[1]
 sig=next(l.strip() for l in sourceLines if l.strip().startswith('internal unsafe static void '+helper+'('));original(sig)
 for prefix in ['Tokenizer_002ETokenize']:
  sig=next(l.strip() for l in sourceLines if l.strip().startswith('internal unsafe static void '+prefix+'_003Cstruct_0020Relo_003A_003AAssetReference_003Cstruct_0020SageBinaryData_003A_003AUpgradeTemplate_') and 'List_' in l);original(sig)
output='\n'.join(lines);path=ROOT/'recovery/src/Native/GameObjectSpecialPowerDerived.h'
if '--check' in sys.argv:
 assert path.read_text()==output,'Regenerate SpecialPower derived metadata'
 saved=json.loads((EVIDENCE/'manifest.json').read_text());assert saved==json.loads(json.dumps(manifest))
 for file,content in exported.values():assert (EVIDENCE/file).read_text()==content,file
else:
 path.write_text(output);EVIDENCE.mkdir(parents=True,exist_ok=True)
 (EVIDENCE/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 for file,content in exported.values():(EVIDENCE/file).write_text(content)
print('PASS complete '+str(len(SPECS))+' SpecialPower derivations and '+str(len(exported))+' original method fingerprints')
