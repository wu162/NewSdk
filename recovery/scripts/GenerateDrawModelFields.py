"""Scripted model root and Spotlight/Tank/Truck contracts from immutable original methods."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]; E=ROOT/'recovery/evidence/gameobject-modules/draw-source'
def source(i):
 t=(E/f'{i:03}.txt').read_text(); b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';m=json.loads((E/'manifest.json').read_text());r=next(x for x in m['methods'] if x['file']==f'{i:03}.txt');assert hashlib.sha256(b.encode()).hexdigest()==r['bodySha256'];return b,json.loads(t.splitlines()[1][13:])
def records(i):
 b,_=source(i);out=[]
 for c in b.split('ptr = (ProcTableElement*)((byte*)ptr + 12);')[:-1]:
  k=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;t=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
  o=re.search(r'\(byte\*\)@object \+ (\d+)',c);scalar=re.search(r'\((bool|float|int|\w+Type)\*\)@object \+ (\d+)',c);off=int(o[1]) if o else (1 if scalar[1]=='bool' else 4)*int(scalar[2]) if scalar else 0
  out.append((k,t,off))
 return out
# field kinds for unusual physical Token types. Offsets and order are independently extracted from the original Token body.
OV={'Object':('String',None),'InitialRecoilSpeed':('FrameSpeed',None),'RecoilSettleSpeed':('FrameSpeed',None),'MinLODRequired':('Enum','drawMainModelLODType'),'ProjectileBoneFeedbackEnabledSlots':('Bits','drawMainWeaponSlotType'),'ExtraPublicBone':('StringList',None),'DependencySharedModelFlags':('ModelCondition',None),'AlphaCameraAtInnerRadius':('Percent',None),'FlagMask':('ModelCondition',None),'PortalType':('Enum','drawMainWaypointPortalType'),'TreadPivotSpeedFraction':('Percent',None),'TreadDriveSpeedFraction':('Percent',None)}
BASIC={1139920438:'Reference',2153588212:'String',438907818:'String',1683514611:'Float',2011428087:'Boolean',602296504:'Signed',2575722984:'Time',1279146296:'Reference',219975299:'Reference',2054604302:'Float'}
SPECS=[('AttachModelStruct',68,180,None,{}),('ScriptedModelDrawAttachModel',91,204,None,{'Model':(0,'&drawMainAttachModelStruct',0)}),('ScriptedModelDrawEmbedPortal',92,205,None,{}),('ArticulationBone',67,179,None,{}),('TreadScrollingMapping',95,208,None,{}),('W3DScriptedModelDrawModuleData',99,211,'&drawBeamDrawModuleData',{'ModelConditionState':(1,'&drawModelModelConditionState',196),'AnimationState':(1,'&drawAnimationState',140),'AttachModel':(1,'&drawMainScriptedModelDrawAttachModel',80),'EmbedPortal':(1,'&drawMainScriptedModelDrawEmbedPortal',12)}),('W3DSpotlightDrawModuleData',100,212,'&drawMainW3DScriptedModelDrawModuleData',{}),('W3DTankDrawModuleData',102,214,'&drawMainW3DScriptedModelDrawModuleData',{'TreadTerrainParticles':(2,'nullptr',0),'ArticulationBone':(1,'&drawMainArticulationBone',16)}),('W3DTruckDrawModuleData',104,216,'&drawMainW3DScriptedModelDrawModuleData',{'TreadTerrainParticles':(2,'nullptr',0),'TreadScrollingMap':(1,'&drawMainTreadScrollingMapping',28)})]
def generate():
 lines=['// Generated from original draw-source Marshal/Token bodies; do not edit.'];manifest=json.loads((E/'manifest.json').read_text())
 for en,unknown,bits in [('ModelLODType',2,0),('WaypointPortalType',5,0),('WeaponSlotType',5,6)]:
  vals=manifest['peEnums'][en];lines+=['const EnumNameValue drawMain'+en+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(vals.items())]+['};',f'const AIEnum drawMain{en} = {{ drawMain{en}Names, {len(vals)}u, {unknown}, {bits}u }};']
 for name,mi,ti,parent,children in SPECS:
  b,lits=source(mi);attrs=[];blocks=[];current=[]
  for l in b.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:
    if current:blocks.append('\n'.join(current))
    current=[l];attrs.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
   elif current:current.append(l)
  if current:blocks.append('\n'.join(current))
  rs=records(ti);assert len(rs)==len(attrs)+(1 if parent else 0),(name,len(rs),len(attrs));fields=[];tokens=[]
  if parent:tokens.append((*rs[0],parent,0,0));rs=rs[1:]
  for (n,d),(k,t,o),block in zip(attrs,rs,blocks):
   # Native marshaling address is independent of the Token member slot.
   native=re.search(r'\(byte\*\)@object \+ (\d+)|\(\((float|int)\*\)@object\)\[(\d+)\]|\((bool|float|int|\w+Type)\*\)@object \+ (\d+)',block)
   if native:
    no=int(native[1]) if native[1] else 4*int(native[3]) if native[3] else (1 if native[4]=='bool' else 4)*int(native[5])
    assert no==o,(name,n,'native/token offsets',no,o)
   mode,nested,stride=children.get(n,(0,'nullptr',0))
   if n not in children:
    kind,en=OV.get(n,(BASIC.get(t),None));assert kind,(name,n,t)
    fields.append((n,o,kind,d,en))
    if kind=='StringList':mode=6
   tokens.append((k,t,o,nested,mode,stride))
  if fields:lines+=['const ModuleField drawMain'+name+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'&'+en if en else 'nullptr') for n,o,k,d,en in fields]+['};']
  lines+=['const ModuleFieldSet drawMain'+name+'FieldSet = { '+('drawMain'+name+'Fields' if fields else 'nullptr')+', '+str(len(fields))+'u };','const ModuleToken drawMain'+name+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in tokens]+['};',f'const ModuleLayout drawMain{name} = {{ drawMain{name}Tokens, {len(tokens)}u }};']
 lines+=['const ModuleChoice drawModelChoices[] = {','    {1322199627u,&drawMainW3DScriptedModelDrawModuleData,216u,286u},','    {1121390653u,&drawMainW3DSpotlightDrawModuleData,224u,287u},','    {2323717438u,&drawMainW3DTankDrawModuleData,228u,288u},','    {2615586235u,&drawDynamicsW3DDynamicsDrawModuleData,252u,289u},','    {2756454809u,&drawMainW3DTruckDrawModuleData,396u,290u},','};',''];return '\n'.join(lines)
if __name__=='__main__':
 p=ROOT/'recovery/src/Native/GameObjectDrawModel.h';out=generate()
 if '--check' in sys.argv:assert p.read_text()==out
 else:p.write_text(out)
 print('PASS original Scripted/Spotlight/Tank/Truck field/token contracts')
