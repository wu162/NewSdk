"""Complete SpecialPowerUpdate and the two structured routing list contracts."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values
ROOT=Path(__file__).resolve().parents[2];BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619';E=ROOT/'recovery/evidence/gameobject-modules/power-routing-source'
s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=s.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
inv=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text());assert inv['sourceBlob']==BLOB
peEvidence=json.loads((ROOT/'recovery/evidence/gameobject-modules/power-continuation-enums.json').read_text());assert peEvidence['sourceBlob']==BLOB and peEvidence['peSha256']=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
triggers=peEvidence['tables']['TriggerType'];assert len(triggers)==3
if '--exported-enums' not in sys.argv:
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()==peEvidence['peSha256'];assert enum_values(s,pe,'TriggerType')==triggers
literals={}
for l in s.splitlines():
 m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',l)
 if m:
  try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
  except UnicodeDecodeError:pass
manifest={'sourceBlob':BLOB,'parentManifest':'../special-power-base-source/manifest.json','modules':[],'methods':[]};exports={}
def original(sig):
 b=body_of(s,sig)
 if sig not in exports:
  file=f'{len(exports):03}.txt';sha=hashlib.sha256((b+'\n').encode()).hexdigest();used={m:literals[m] for m in re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',b) if m in literals};exports[sig]=(file,'// Original blob '+BLOB+'; body SHA256 '+sha+'\n// Literals: '+json.dumps(used,ensure_ascii=False)+'\n'+b+'\n');manifest['methods'].append({'file':file,'signature':sig,'bodySha256':sha})
 return b
def method(name,op):
 sig='internal unsafe static void '+op+'(INode* node, '+name+'* @object, Tracker* state)' if op=='Marshaler_002EMarshal' else 'internal unsafe static void '+op+'('+name+'* @object, EndianHelper* state)' if op=='Marshaler_002EEndianCorrect' else 'internal unsafe static void '+op+'(void** table, '+name+'* @object, Tracker* state)'
 return original(sig) if sig in s else None
update=[('SpecialPowerTemplate',8,'Reference',None,None),('StartsPaused',20,'Boolean','false',None),('DisabledTypesToIgnore',12,'Bits','HELD','specialPowerDisabled')]
info=[('ModuleId',0,'AssetId',None,None),('TriggerAtTargetPosition',8,'Boolean','true',None),('TriggerType',4,'Enum','TARGET_POSITION','powerRoutingTrigger')]
activate=[('Deactivate',12,'Boolean','false',None)]
dispatch=[('SpecialPowerTemplate',0,'Reference',None,None)]
specs=[(3115493811,24,188,'SpecialPowerUpdateModuleData',update),(1213608295,460,189,'ActivateModuleSpecialPower',[]),(3631725172,464,190,'SpecialPowerDispatchSpecialPowerModuleData',[('RequireAllPowersToFunction',460,'Boolean','false',None)])]
lines=['// Complete Power routing contracts; original blob '+BLOB+'.','const EnumNameValue powerRoutingTriggerNames[] = {']+['    { '+json.dumps(n)+f', {v} }},' for n,v in sorted(triggers.items())]+['};','const AIEnum powerRoutingTrigger = { powerRoutingTriggerNames, 3u, 0, 0u };']
def fields(label,rows):
 if rows:lines.extend(['const ModuleField powerRouting'+label+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if enum is None else '&'+enum) for n,o,k,d,enum in rows]+['};'])
fields('Info',info);fields('Activate',activate);fields('Dispatch',dispatch)
def tokens(label,body,parent=None,modes=None,offsets=None,fieldCount=None):
 records=[];ptr=re.search(r'ProcTableElement\* (ptr\d*) =',body)[1]
 chunks=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);')
 for chunk in chunks:
  match=re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);\s*\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',chunk)
  if not match or match[1]=='0':continue
  key,typ=[int(x)&0xffffffff for x in match.groups()];off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk) if '* @object' in body.splitlines()[0] else re.search(r'\(byte\*\)ptr \+ (\d+)',chunk);off=int(off[1]) if off and key!=1 else 0
  nested=parent if key==1 else 'nullptr';mode=0;stride=0
  if modes and off in modes and key!=1:nested,mode,stride=modes[off]
  records.append((key,typ,off,nested or 'nullptr',mode,stride))
 assert len(records)==fieldCount,(label,records)
 if offsets is not None:assert [r[2] for r in records]==offsets,(label,records,offsets)
 assert re.search(r'\*\(int\*\)'+ptr+r' = 0;\s*\(\(int\*\)'+ptr+r'\)\[1\] = 0;\s*\(\(int\*\)'+ptr+r'\)\[2\] = 0;',chunks[-1])
 lines.extend(['const ModuleToken powerRouting'+label+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout powerRouting{label} = {{ powerRouting{label}Tokens, {len(records)}u }};'])
 return records
ib=method('SpecialPowerModuleInfo','Marshaler_002EMarshal');it=method('SpecialPowerModuleInfo','Tokenizer_002ETokenize');assert 'ModuleId' in literals.values();tokens('Info',it,offsets=[0,8,4],fieldCount=3)
at=method('ActivateModuleInfo','Tokenizer_002ETokenize');tokens('Activate',at,'&powerRoutingInfo',offsets=[0,12],fieldCount=2)
# Original Dispatch list has a nested one-field+sentinel table for each 4-byte item.
dt=next(l.strip() for l in s.splitlines() if l.strip().startswith('internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020SageBinaryData_003A_003ADispatchSpecialPowerType_003E('));db=original(dt);assert '12u, 2u' in db and '*(int*)ptr8 = -2116526342;' in db and '((int*)ptr8)[1] = -1360512888;' in db
lines+=['const ModuleToken powerRoutingDispatchTokens[] = {','    { 2178440954u, 2934454408u, 0u, nullptr, 0u, 0u },','};','const ModuleLayout powerRoutingDispatch = { powerRoutingDispatchTokens, 1u };']
for typ,size,kind,name,rows in specs:
 assert inv['marshal'][str(typ)]['name']==name and inv['marshal'][str(typ)]['nativeSize']==size
 wrappers={op:original(inv[op][str(typ)]['helperSignature']) for op in ['marshal','endian','token']}
 mb=method(name,'Marshaler_002EMarshal') or wrappers['marshal'];eb=method(name,'Marshaler_002EEndianCorrect') or wrappers['endian'];tb=method(name,'Tokenizer_002ETokenize') or wrappers['token']
 actual=[]
 for line in mb.splitlines():
  syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
  if syms:actual.append((literals[syms[0]],literals[syms[1]] if len(syms)>1 else None))
 extra=[('InitiateSound',None)] if kind==188 else [('Modules',None)] if kind==189 else [('SpecialPower',None)]
 assert actual==[(n,d) for n,o,k,d,e in rows]+extra,(name,actual)
 fields(name,rows)
 modes={16:('&specialPowerSound',4,0)} if kind==188 else {452:('&powerRoutingActivate',1,16)} if kind==189 else {452:('&powerRoutingDispatch',1,4)}
 parent='&spawnUpdateModuleData' if kind==188 else '&specialPowerSpecialPowerModuleData';offsets=[0]+[r[1] for r in rows]+[16 if kind==188 else 452];records=tokens(name,tb,parent,modes,offsets,len(offsets))
 assert records[0][1]==(4043318533 if kind==188 else 3472757011)
 manifest['modules'].append({'typeId':typ,'nativeSize':size,'kind':kind,'name':name,'fields':rows,'tokenCount':len(records)})
# Fingerprint list/entry contracts, weak ID and TriggerType fallback helper too.
for l in s.splitlines():
 sig=l.strip()
 if not sig.startswith('internal unsafe static void '):continue
 if ('ActivateModuleInfo_003E(IList*' in sig or 'ActivateModuleInfo_003E(List_' in sig or 'ActivateModuleInfo_003E(void**' in sig or 'DispatchSpecialPowerType_003E(IList*' in sig or 'SpecialPowerModuleInfo* @object' in sig or sig.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003ATriggerType_')):original(sig)
lines+=['const ModuleFieldSet powerRoutingFieldSets[] = {']+[f'    {{ '+('powerRouting'+n+'Fields' if f else 'nullptr')+f', {len(f)}u }},' for t,z,k,n,f in specs]+['};','const ModuleChoice powerRoutingChoices[] = {']+[f'    {{ {t}u, &powerRouting{n}, {z}u, {k}u }},' for t,z,k,n,f in specs]+['};','']
out='\n'.join(lines);p=ROOT/'recovery/src/Native/GameObjectPowerRouting.h'
if '--check' in sys.argv:
 assert p.read_text()==out;assert json.loads((E/'manifest.json').read_text())==json.loads(json.dumps(manifest))
 for file,text in exports.values():assert (E/file).read_text()==text
else:
 p.write_text(out);E.mkdir(parents=True,exist_ok=True);(E/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 for file,text in exports.values():(E/file).write_text(text)
print('PASS complete PowerUpdate/routing contracts, '+str(len(exports))+' original fingerprints and TriggerType3'+(' (exported enum evidence)' if '--exported-enums' in sys.argv else ' (original PE verified)'))
