"""Complete Attach family; fields and Tokens checked against pinned original methods."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/attach-next-source'
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
BASE=[(n,o,'ObjectStatus',None,None) for n,o in [('ParentStatus',8),('ParentStatusAttached',36),('AttachedObjectStatus',64)]]+[
 ('AttachedModelConditions',92,'ModelCondition',None,None),('ForbiddenParentStatus',152,'ObjectStatus',None,None),('IgnoreForbiddenParentStatusStatus',180,'ObjectStatus',None,None),('ParentStatusToCopy',208,'NullableObjectStatus',None,None),('ParentStatusToPrefer',212,'NullableObjectStatus',None,None),('Range',216,'Float',None,None),('CloseEnoughRange',220,'Float','1000.0',None)]+[(n,o,'Reference','',None) for n,o in [('ParentOwnerAttachmentEvaEvent',224),('ParentAllyAttachmentEvaEvent',228),('ParentEnemyAttachmentEvaEvent',232)]]+[
 ('AttachFXList',236,'Reference',None,None),('DetachFXList',240,'Reference',None,None)]+[(n,o,'Reference','',None) for n,o in [('ParentOwnerDiedEvaEvent',244),('ParentAllyDiedEvaEvent',248),('ParentEnemyDiedEvaEvent',252)]]+[
 ('InitialAttachDelay',256,'Time',None,None),('IdleScanDelay',260,'Time','0.35s',None),('Flags',264,'Bits','STICK_TO_PARENT TELEPORT USE_GEOMETRY','AttachUpdateFlagsType'),('ParentDeathTypeToListenFor',268,'Enum','ALL','DeathType'),('NoDieIfStatusActive',272,'ObjectStatus',None,None),('ParentPositionElevation',300,'Float',None,None),('BounceAmount',304,'Float','0.2',None),('BounceTimeout',308,'Time','0.33s',None),('DamageTypesToNotLeech',312,'NullableWideBits',None,'DamageType'),('DeathTypesToNotLeech',316,'NullableWideBits',None,'DeathType'),('ForbiddenParentStatusDieDeathType',320,'Enum','NORMAL','DeathType')]
MONEY=[('ActionType',336,'Enum',None,'MoneyGainAttachActionFlag'),('PurchasePricePercent',340,'Percent','100%',None)]
BOOBY=[('FireWeaponConditionsOnParent',336,'ObjectStatus',None,None),('PreventEnteringSetStatusOnParent',364,'ObjectStatus',None,None),('AllowTriggerOnAllies',400,'Boolean','false',None),('SpecialWeapon',392,'Reference',None,None)]
VALIDATION=[('ForbiddenStatus',0,'ObjectStatus',None,None),('RequiredStatus',28,'ObjectStatus',None,None)]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB and len(m['methods'])==78;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def method(op,n):return bodies[f'internal unsafe static void {op}({"INode* node, " if op=="Marshaler_002EMarshal" else "void** table, "}{n}* @object, Tracker* state)']
 lines=['// Generated complete Attach336 family; original blob '+BLOB+'.']
 for name,count,unknown,bits in [('AttachUpdateFlagsType',27,22,27),('DamageType',39,38,39),('DeathType',34,33,34),('MoneyGainAttachActionFlag',1,0,0)]:
  values=m['peEnums'][name];assert len(values)==count
  lines+=['const EnumNameValue attach'+name+'Names[] = {']+['    { '+json.dumps(n)+', '+str(v)+' },' for n,v in sorted(values.items())]+['};',f'const AIEnum attach{name} = {{ attach{name}Names, {count}u, {unknown}, {bits}u }};']
 def emit(n,fields,children,parent):
  mb,lits=method('Marshaler_002EMarshal',n);actual=[]
  for line in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nested in children],(n,actual)
  if parent:assert 'Marshaler_002EMarshal(node, ('+('ModuleData' if n=='AttachUpdateModuleData' else 'AttachUpdateModuleData')+'*)@object, state);' in mb
  lines.extend(['const ModuleField attach'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&attach'+e) for n,o,k,d,e in fields]+['};'])
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for chunk in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',chunk)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',chunk)[1])&0xffffffff;off=re.search(r'\(byte\*\)@object \+ (\d+)',chunk);scalar=re.search(r'\((?:float|DeathType)\*\)@object \+ (\d+)',chunk);off=int(off[1]) if off else 4*int(scalar[1]) if scalar else 0;nested='nullptr';mode=0
   if key==1:nested=parent
   for name,o,k,d,e in fields:
    if off==o and k.startswith('Nullable'):mode=2
   for name,o,cm,cn in children:
    if off==o:mode=cm;nested=cn
   records.append((key,typ,off,nested,mode,0))
  assert [r[2] for r in records]==([0] if parent else [])+[o for name,o,k,d,e in fields]+[o for name,o,cm,cn in children],(n,records)
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert all(w in chunks[-1] for w in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines.extend(['const ModuleToken attach'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout attach{n} = {{ attach{n}Tokens, {len(records)}u }};'])
 emit('ObjectStatusValidationDataType',VALIDATION,[],None)
 emit('AttachUpdateModuleData',BASE,[('ObjectFilter',324,7,'nullptr'),('ModifierToLeechFromParent',328,3,'nullptr')],'&spawnUpdateModuleData')
 emit('MoneyGainAttachUpdateModuleData',MONEY,[('MoneyGainObjectStatusValidation',344,4,'&attachObjectStatusValidationDataType')],'&attachAttachUpdateModuleData')
 emit('BoobyTrapUpdateModuleData',BOOBY,[('PreventEnteringFilter',396,7,'nullptr')],'&attachAttachUpdateModuleData')
 lines+=['const ModuleChoice attachChoices[] = {']+[f'    {{ {x["typeId"]}u, &attach{x["name"] if i>=3 else "AttachUpdateModuleData"}, {x["nativeSize"]}u, {220+i}u }},' for i,x in enumerate(m['modules'])]+['};','']
 return '\n'.join(lines)
if __name__=='__main__':
 from GenerateUpgradeFields import enum_values
 source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
 pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();assert hashlib.sha256(pe).hexdigest()=='ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603'
 m=json.loads((E/'manifest.json').read_text());assert m['peEnums']=={n:enum_values(source,pe,n) for n in m['peEnums']}
 output=generate();path=ROOT/'recovery/src/Native/GameObjectAttach.h'
 if '--check' in sys.argv:assert path.read_text()==output,'Regenerate complete Attach contracts'
 else:path.write_text(output)
 print('PASS complete five-type Attach336 family, MoneyGain348/validation56 and BoobyTrap404; 78 original methods and ten PE maps')
