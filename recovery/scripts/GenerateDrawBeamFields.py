"""Complete W3D Laser/Tracer/Stream and shader contracts from pinned original methods."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/draw-source';BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
def f(n,o,k,d=None,e=None):return n,o,k,d,e
LASER=[f('UseDistortionShader',108,'Boolean','false')]+[f(n,8+4*i,'Float','1.0' if i<4 else '0.0') for i,n in enumerate(['Texture1_UTile','Texture1_VTile','Texture2_UTile','Texture2_VTile','Texture1_UScrollRate','Texture1_VScrollRate','Texture2_UScrollRate','Texture2_VScrollRate'])]+[f('Texture1_NumFrames',40,'Signed','1'),f('Texture1_FrameRate',44,'Float','30.0'),f('Texture2_NumFrames',48,'Signed','1'),f('Texture2_FrameRate',52,'Float','30.0'),f('LaserWidth',56,'Float','5.0'),f('FadeInTime',60,'Time','0s'),f('FadeOutTime',64,'Time','0s'),f('WeaponSlotID',68,'Signed','-1'),f('LaserStateID',72,'Signed','0')]
TRACER=[f(n,8+4*i,'Float','1.0' if n=='SweepSpeed' else None) for i,n in enumerate(['MinLength','MaxLength','Width','MinSpeed','MaxSpeed','SweepSpeed','SpreadAngle','MinTracersPerFrame','MaxTracersPerFrame','FrameLifeTime'])]+[f('WeaponSlotType',48,'Enum',None,'WeaponSlotType'),f('WeaponSlotID',52,'Signed','1'),f('Texture',56,'Reference'),f('UseAdditiveBlending',108,'Boolean','false'),f('TracerHitFx',60,'Reference'),f('TracerEmitFx',64,'Reference'),f('TextureIndex',68,'Unsigned')]
STREAM=[f('UseDistortionShader',72,'Boolean','false'),f('Velocity',8,'Float','200.0'),f('ArcHeightFactor',12,'Float','0.0'),f('UVWorldSize',16,'Float','10.0'),f('NumTubeSides',20,'Signed','8'),f('TubeRadius',24,'Float','1.0'),f('HitFx',28,'Reference'),f('WeaponSlotID',32,'Signed','-1'),f('StreamStateID',36,'Signed','0')]
NAMES=['W3DLaserDrawModuleData','W3DTracerDrawModuleData','W3DStreamDrawModuleData'];FIELDS=[LASER,TRACER,STREAM]
CHILDREN=[[('FXShader',76,0,'&beamFXShaderMaterial',0),('ObjectStatusValidation',104,4,'&lifecycleObjectStatusValidationDataType',0)],[('HeadColor',72,0,'&beamColor4f',0),('TailColor',88,0,'&beamColor4f',0),('ObjectStatusValidation',104,4,'&lifecycleObjectStatusValidationDataType',0)],[('FXShader',40,0,'&beamFXShaderMaterial',0),('ParticleSystems',68,4,'&beamParticleSystemList',0)]]
SHADER=[f('ShaderName',0,'String'),f('TechniqueName',8,'String',''),f('TechniqueIndex',24,'Byte','0')]
COLOR=[f(n,4*i,'Float') for i,n in enumerate(['r','g','b','a'])]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']==BLOB;bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def direct(op,n):return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void '+op+'(') and ' '+n+'* @object,' in k)
 def poly(n):return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshalPolymorphicType_') and '_003A_003A'+n+'_002C' in k)
 lines=['// Generated original beam Draw group and complete FXShader28 dependency; blob '+BLOB+'.']
 values=m['peEnums']['WeaponSlotType'];eb,_=next(v for k,v in bodies.items() if 'Marshaler_002EMarshal_003Cenum_0020WeaponSlotType_002Cclass' in k);assert '? 5 :' in eb
 lines+=['const EnumNameValue beamWeaponSlotTypeNames[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(values.items())]+['};',f'const AIEnum beamWeaponSlotType = {{ beamWeaponSlotTypeNames, {len(values)}u, 5, 0u }};']
 def emit(n,fields,children,parent=None,marshal=None):
  nonlocal lines
  if marshal is None:mb,lits=direct('Marshaler_002EMarshal',n)
  else:mb,lits=marshal
  actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(n,d) for n,o,k,d,e in fields]+[(n,None) for n,o,mode,nest,stride in children],(n,actual)
  if fields:lines+=['const ModuleField beam'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&beam'+e) for n,o,k,d,e in fields]+['};']
  tb,_=direct('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
   o=re.search(r'\(byte\*\)@object \+ (\d+)',c);scalar=re.search(r'\((?:float|int|\w+Type)\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 4*int(scalar[1]) if scalar else 0
   nested=parent if key==1 else 'nullptr';mode=0;stride=0
   for name,o,cm,cn,cs in children:
    if off==o:mode=cm;nested=cn;stride=cs
   records.append((key,typ,off,nested,mode,stride))
  assert len(records)+1==int(re.search(r'12u, (\d+)u',tb)[1]);assert [r[2] for r in records]==([0] if parent or n=='FXShaderConstant' else [])+[o for _,o,_,_,_ in fields]+[o for _,o,_,_,_ in children],(n,records)
  assert all(s in chunks[-1] for s in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;'])
  lines+=['const ModuleToken beam'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout beam{n} = {{ beam{n}Tokens, {len(records)}u }};']
 emit('DrawModuleData',[],[],'&dieModuleData')
 emit('Color4f',COLOR,[])
 emit('FXShaderConstant',[f('Name',4,'String')],[])
 for n in ['Bool','Int','Texture']:emit('FXShaderConstant'+n,[],[('Value',12,0,'nullptr',0)],'&beamFXShaderConstant')
 emit('FXShaderConstantFloat',[],[('Value',12,3,'nullptr',0)],'&beamFXShaderConstant',poly('FXShaderConstantFloat'))
 # Original nullable ParticleSystemList marshaler is an inlined List8 of strong references.
 pm=next(v for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AParticleSystemList_003E('))
 emit('ParticleSystemList',[],[('ParticleSys',0,3,'nullptr',0)],marshal=pm)
 emit('FXShaderMaterial',SHADER,[('Constants',16,10,'nullptr',0)])
 for n,fs,cs,parent in zip(NAMES,FIELDS,CHILDREN,['&beamDrawModuleData','&dieModuleData','&beamDrawModuleData']):emit(n,fs,cs,parent)
 lines+=['const ModuleFieldSet beamFieldSets[] = {']+[f'    {{ beam{n}Fields, {len(fs)}u }},' for n,fs in zip(NAMES,FIELDS)]+['};','const ModuleChoice beamChoices[] = {']
 for i,n in enumerate(NAMES):
  meta=next(x for x in m['modules'] if x['name']==n);lines+=[f'    {{ {meta["typeId"]}u, &beam{n}, {meta["nativeSize"]}u, {283+i}u }},']
 lines+=['};',''];return '\n'.join(lines)
if __name__=='__main__':
 out=generate();p=ROOT/'recovery/src/Native/GameObjectDrawBeam.h'
 if '--check' in sys.argv:assert p.read_text()==out,'Regenerate beam Draw contracts'
 else:p.write_text(out)
 print('PASS complete Laser/Tracer/Stream and Shader/Color/ParticleSystemList original contracts')
