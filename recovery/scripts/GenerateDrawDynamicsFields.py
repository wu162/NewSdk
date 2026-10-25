"""Pinned Dynamics shape/volume contracts extracted from original Draw evidence."""
import hashlib,json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]; E=ROOT/'recovery/evidence/gameobject-modules/draw-source'
def f(n,o,k='Float',d=None,e=None):return n,o,k,d,e
F={
'DynamicsShapeType':[],
'DynamicsSphereShapeType':[f('Radius',8)],
'DynamicsCapsuleShapeType':[f('Radius',8),f('HalfLength',12)],
'DynamicsBoxShapeType':[f(n,8+4*i,d='0' if n=='EdgeRadius' else None) for i,n in enumerate(['HalfSizeX','HalfSizeY','HalfSizeZ','EdgeRadius'])],
'DynamicsCylinderShapeType':[f('Radius',8),f('HalfLength',12),f('EdgeRadius',16,d='0')],
'DynamicsTriangleShapeType':[f(n,8+4*i,d='0' if n=='EdgeRadius' else None) for i,n in enumerate(['X1','Y1','Z1','X2','Y2','Z2','X3','Y3','Z3','EdgeRadius'])],
'DynamicsVolumeType':[f('Mass',0,d='0'),f('Bounciness',4,'Percent','30%')]+[f(n,8+4*i,d=d) for i,(n,d) in enumerate([('Spinniness','2.0'),('StaticFriction','0.6'),('KineticFriction','0.5'),('AngularDrag','0.1'),('LinearDrag','0.1')])]+[f('ReportContacts',72,'Boolean','false'),f('ContactTag',28,'Enum','NONE','DynamicsContactTag')],
'DynamicsBoneVolumeType':[f('BoneName',76,'String')],
'DynamicsBoneVolumeSetType':[],
'DynamicsLifetime':[f('Delay',0,'Time','0s'),f('FadeTime',4,'Time')],
'W3DDynamicsDrawModuleData':[f('Collision',216,'Enum',e='DynamicsCollisionType'),f('IntuitiveCollision',220,'Enum','COLLIDES_WITH_GROUND_ONLY','IntuitiveCollisionType'),f('DrawPriority',224,'Enum','MUST_BE_DRAWN','DynamicsDrawPriority'),f('LODPriority',228,'Enum','VERY_LOW','EffectsLODType'),f('Explodiness',232,d='0'),f('FlingPerturbation',236,d='0'),f('InitiallyActive',248,'Boolean','true')],
}
# child name, offset, mode, nested type, native stride
C={'DynamicsShapeType':[('Translation',0,2,None,12),('Rotation',4,2,None,16)],'DynamicsVolumeType':[(n,32+8*i,1,'Dynamics'+n+'ShapeType',s) for i,(n,s) in enumerate([('Sphere',12),('Capsule',16),('Box',24),('Cylinder',20),('Triangle',48)])],'DynamicsBoneVolumeSetType':[('BoneVolume',0,1,'DynamicsBoneVolumeType',84)],'W3DDynamicsDrawModuleData':[('BoneVolumes',240,4,'DynamicsBoneVolumeSetType',8),('Lifetime',244,4,'DynamicsLifetime',8)]}
P={n:'DynamicsShapeType' for n in F if n.endswith('ShapeType') and n!='DynamicsShapeType'}; P['DynamicsBoneVolumeType']='DynamicsVolumeType';P['W3DDynamicsDrawModuleData']='W3DScriptedModelDrawModuleData'
S={'DynamicsShapeType':8,'DynamicsSphereShapeType':12,'DynamicsCapsuleShapeType':16,'DynamicsBoxShapeType':24,'DynamicsCylinderShapeType':20,'DynamicsTriangleShapeType':48,'DynamicsVolumeType':76,'DynamicsBoneVolumeType':84,'DynamicsBoneVolumeSetType':8,'DynamicsLifetime':8,'W3DDynamicsDrawModuleData':252}
def generate():
 m=json.loads((E/'manifest.json').read_text());bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def direct(op,n):return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void '+op+'(') and ' '+n+'* @object,' in k)
 lines=['// Generated from pinned original Dynamics Draw methods; source blob '+m['sourceBlob']+'.']
 for en,unknown in [('DynamicsCollisionType',1),('IntuitiveCollisionType',2),('DynamicsDrawPriority',2),('EffectsLODType',3),('DynamicsContactTag',7)]:
  mb,_=next(v for k,v in bodies.items() if 'IValue* value' in k and en+'_002C' in k);assert '? '+str(unknown)+' :' in mb
  v=m['peEnums'][en];lines+=['const EnumNameValue drawDynamics'+en+'Names[] = {']+['    { '+json.dumps(k)+', '+str(val)+' },' for k,val in sorted(v.items())]+['};','const AIEnum drawDynamics'+en+' = { drawDynamics'+en+'Names, '+str(len(v))+'u, '+str(unknown)+', 0u };']
 for n,fs in F.items():
  if n!='DynamicsBoneVolumeSetType':
   mb,lits=direct('Marshaler_002EMarshal',n);actual=[]
   for l in mb.splitlines():
    syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
    if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
   expectedActual=[(fn,d) for fn,o,k,d,e in fs]+[(cn,None) for cn,o,mode,nest,stride in C.get(n,[])]
   if n=='W3DDynamicsDrawModuleData':expectedActual.insert(-1,('BoneVolume',None))
   assert actual==expectedActual,(n,actual)
  if fs:lines+=['const ModuleField drawDynamics'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(fn),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&drawDynamics'+e) for fn,o,k,d,e in fs]+['};']
  tb,_=direct('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');records=[]
  expected=([(0,'&'+('drawMain' if P[n]=='W3DScriptedModelDrawModuleData' else 'drawDynamics')+P[n],0,0)] if n in P else [])+[(o,'nullptr',0,0) for _,o,_,_,_ in fs]+[(o,'nullptr' if nest is None else '&drawDynamics'+nest,mode,stride if mode==1 else 0) for _,o,mode,nest,stride in C.get(n,[])]
  for c,(off,nested,mode,stride) in zip(chunks[:-1],expected):
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
   actual=re.search(r'\(byte\*\)@object \+ (\d+)',c);actual=int(actual[1]) if actual else 0;assert actual==off,(n,actual,off)
   records.append((key,typ,off,nested,mode,stride))
  assert len(records)==len(chunks)-1==int(re.search(r'12u, (\d+)u',tb)[1])-1,(n,len(records),len(chunks))
  lines+=['const ModuleToken drawDynamics'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in records]+['};',f'const ModuleLayout drawDynamics{n} = {{ drawDynamics{n}Tokens, {len(records)}u }};']
 # Check exact list element strides against original list marshaling methods.
 for n in ['DynamicsBoneVolumeType','DynamicsSphereShapeType','DynamicsCapsuleShapeType','DynamicsBoxShapeType','DynamicsCylinderShapeType','DynamicsTriangleShapeType']:
  listBody=next(v[0] for k,v in bodies.items() if 'Marshaler_002EMarshal_003Cstruct_' in k and n+'_003E(IList*' in k)
  assert ', '+str(S[n])+'u, num);' in listBody,(n,S[n])
 # Runtime descriptors retain marshal order: derived fields, derived child lists, parent.
 lines+=['struct DrawDynamicsSchema;','struct DrawDynamicsChild {const char* name;unsigned offset,mode;const DrawDynamicsSchema* nested;unsigned size;};','struct DrawDynamicsSchema {ModuleFieldSet fields;const DrawDynamicsChild* children;unsigned count;const DrawDynamicsSchema* parent;};']
 for n in F:lines+=['extern const DrawDynamicsSchema drawDynamicsSchema'+n+';']
 for n,fs in F.items():
  cs=C.get(n,[])
  if cs:lines+=['const DrawDynamicsChild drawDynamics'+n+'Children[] = {']+['    { %s, %du, %du, %s, %du },'%(json.dumps(cn),o,mode,'nullptr' if nest is None else '&drawDynamicsSchema'+nest,size) for cn,o,mode,nest,size in cs]+['};']
  parent=P.get(n);parent='nullptr' if parent is None or parent=='W3DScriptedModelDrawModuleData' else '&drawDynamicsSchema'+parent
  lines+=['const DrawDynamicsSchema drawDynamicsSchema'+n+' = { { '+('drawDynamics'+n+'Fields' if fs else 'nullptr')+', '+str(len(fs))+'u }, '+('drawDynamics'+n+'Children' if cs else 'nullptr')+', '+str(len(cs))+'u, '+parent+' };']
 lines+=[''];return '\n'.join(lines)
if __name__=='__main__':
 out=generate();p=ROOT/'recovery/src/Native/GameObjectDrawDynamics.h'
 if '--check' in sys.argv:assert p.read_text()==out
 else:p.write_text(out)
 print('PASS complete Dynamics Draw original contracts')
