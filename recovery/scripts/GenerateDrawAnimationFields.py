"""Animation/AnimationState/LuaEvent contracts from pinned original evidence."""
import hashlib,json,re,sys
from pathlib import Path
R=Path(__file__).resolve().parents[2];E=R/'recovery/evidence/gameobject-modules/draw-source'
def f(n,o,k,d=None,e=None):return n,o,k,d,e
FIELDS={
'Animation':[f('Flags',0,'String'),f('AnimationName',8,'Reference'),f('AnimationMode',12,'Enum',None,'AnimationMode'),f('AnimNickName',16,'String'),f('Distance',24,'Float','0.0'),f('AnimationBlendTime',28,'Float','5.0'),f('AnimationMustCompleteBlend',56,'Boolean','false'),f('AnimationSpeedFactorMin',32,'Float','1.0'),f('AnimationSpeedFactorMax',36,'Float','1.0'),f('AnimationAbsoluteTime',40,'Time','0.0s'),f('UseWeaponTiming',57,'Boolean','false'),f('WeaponTimingOrdering',44,'Enum',None,'WeaponSlotType'),f('WeaponTimingSlotID',48,'Signed','1'),f('AnimationPriority',52,'Signed','1')],
'AnimationState':[f('ParseCondStateType',0,'Enum',None,'ParseCondStateType'),f('AnimNickName',4,'String'),f('ConditionsYes',12,'ModelCondition'),f('Name',72,'String'),f('StateName',80,'String'),f('Flags',88,'Bits','NONE','AnimationStateFlag'),f('ShareAnimation',136,'Boolean','false'),f('EnteringStateFX',92,'Reference'),f('FrameForPristineBonePositions',96,'Signed','0'),f('AllowRepeatInRandomPick',137,'Boolean','false'),f('SimilarRestart',138,'Boolean','false')],
'LuaEvent':[f('Frame',0,'Signed'),f('Data',4,'String'),f('Trigger',12,'Enum','TRIGGER_ONFRAME','LuaEventTriggerType')]}
def generate():
 m=json.loads((E/'manifest.json').read_text());bodies={}
 for x in m['methods']:
  s=(E/x['file']).read_text();body='\n'.join(s.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(body.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(body,json.loads(s.splitlines()[1][13:]))
 def method(op,n):return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void '+op+'(') and ' '+n+'* @object,' in k)
 # Independent native list strides and enum unknown fallbacks, pinned from helper bodies.
 for file,stride in [('152.txt',60),('161.txt',28),('163.txt',16),('168.txt',36)]:
  assert f', {stride}u, num);' in (E/file).read_text()
 assert '? 1 :' in (E/'122.txt').read_text() and '? 1 :' in (E/'130.txt').read_text()
 assert 'num4 = 10;' in (E/'123.txt').read_text() and 'else' in (E/'123.txt').read_text()
 assert ': 0);' in (E/'128.txt').read_text()
 lines=['// Generated pinned original Animation60, AnimationState140, LuaEvent16 contracts.']
 for n,unknown,bits in [('AnimationMode',1,0),('ParseCondStateType',1,0),('AnimationStateFlag',10,12),('LuaEventTriggerType',0,0),('WeaponSlotType',5,0)]:
  vals=m['peEnums'][n];lines+=['const EnumNameValue drawAnim'+n+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(vals.items())]+['};',f'const AIEnum drawAnim{n} = {{ drawAnim{n}Names, {len(vals)}u, {unknown}, {bits}u }};']
 children={100:('&drawAnimation',1,60),108:('nullptr',2,0),112:('&drawModelFXEvent',1,28),120:('&drawLuaEvent',1,16),128:('&drawModelParticleSysBone',1,36)}
 for n in ['Animation','LuaEvent','AnimationState']:
  fields=FIELDS[n];mb,lits=method('Marshaler_002EMarshal',n)
  attrs=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:attrs.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  expect=[(n,d) for n,o,k,d,e in fields]+([(n,None) for n in ['Animation','Script','FXEvent','LuaEvent','ParticleSysBone']] if n=='AnimationState' else [])
  assert attrs==expect,(n,attrs)
  prefix={'Animation':'drawAnimation','AnimationState':'drawAnimationState','LuaEvent':'drawLuaEvent'}[n]
  lines+=['const ModuleField '+prefix+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&drawAnim'+e) for n,o,k,d,e in fields]+['};',f'const ModuleFieldSet {prefix}FieldSet = {{ {prefix}Fields, {len(fields)}u }};']
  tb,_=method('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
   offm=re.search(r'\(byte\*\)@object \+ (\d+)',c);off=int(offm[1]) if offm else 0
   nest,mode,stride=children[off] if n=='AnimationState' and off in children else ('nullptr',0,0)
   records.append((key,typ,off,nest,mode,stride))
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert [r[2] for r in records]==[o for _,o,_,_,_ in fields]+([100,108,112,120,128] if n=='AnimationState' else [])
  lines+=['const ModuleToken '+prefix+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout {prefix} = {{ {prefix}Tokens, {len(records)}u }};']
 return '\n'.join(lines)+'\n'
if __name__=='__main__':
 p=R/'recovery/src/Native/GameObjectDrawAnimation.h';out=generate()
 if '--check' in sys.argv:assert p.read_text()==out
 else:p.write_text(out)
 print('PASS original Animation/AnimationState/LuaEvent contracts')
