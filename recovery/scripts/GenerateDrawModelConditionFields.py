"""Pinned complete ModelConditionState and shared particle/FX contracts."""
import json,re,hashlib,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2];E=ROOT/'recovery/evidence/gameobject-modules/draw-source'
def f(n,o,k,d=None,e=None):return n,o,k,d,e
FIELDS={
'ScriptedModelDrawModel':[f('Name',0,'Reference'),f('ExtraMesh',4,'Boolean','false')],
'ReplaceTexture':[f('Original',0,'Reference'),f('New',4,'Reference')],
'BoneAttachPoint':[f('WeaponSlotID',0,'Signed','1'),f('WeaponSlotType',4,'Enum','PRIMARY_WEAPON','WeaponSlotType'),f('BoneName',8,'String')],
'ParticleSysBone':[f('BoneName',0,'String'),f('FXParticleSystemTemplate',8,'Reference'),f('FollowBone',32,'Boolean','false'),f('FXTrigger',12,'Enum','NONE','FXTriggerType'),f('FXAction',16,'Enum','NONE','FXActionType'),f('PersistID',20,'Signed','0'),f('OnlyIfOnWater',33,'Boolean','false'),f('OnlyIfOnLand',34,'Boolean','false'),f('id',24,'String')],
'FXEvent':[f('Frame',0,'Signed'),f('FrameStep',4,'Signed'),f('FrameStop',8,'Signed'),f('FireWhenSkipped',24,'Boolean','false'),f('Effect',12,'Reference'),f('Bone',16,'String')],
'ModelConditionStateTurret':[f('TurretNameKey',0,'String'),f('TurretArtAngle',8,'Angle'),f('TurretPitch',12,'String'),f('TurretArtPitch',20,'Angle'),f('TurretID',24,'Signed','-1')],
'ShadowInfo':[f('Type',0,'Enum',None,'ShadowType'),f('Texture',4,'Reference'),f('AdditionalTexture',8,'Reference')]+[f(n,12+4*i,k,d) for i,(n,k,d) in enumerate([('SizeX','Float','0.0'),('SizeY','Float','0.0'),('OffsetX','Float','0.0'),('OffsetY','Float','0.0'),('OpacityStart','Float','0'),('OpacityFadeInTime','Time','0s'),('OpacityPeak','Float','1'),('OpacityFadeOutTime','Time','0s'),('OpacityEnd','Float','0'),('MaxHeight','Float','20')])]+[f('OverrideLODVisibility',56,'Boolean','false'),f('UseHouseColor',57,'Boolean','false'),f('IsRotatingWithObject',58,'Boolean','true'),f('LocalPlayerOnly',59,'Boolean','false'),f('SunAngle',52,'Float','0.0')],
'ModelConditionState':[f('ParseCondStateType',0,'Enum',None,'ParseCondStateType'),f('Name',4,'String'),f('ConditionsYes',12,'ModelCondition'),f('Skeleton',72,'String'),f('PortraitImage',80,'Reference'),f('ButtonImage',84,'Reference'),f('OverrideTooltip',88,'String'),f('RetainSubObjects',192,'Boolean','false'),f('id',96,'String')]}
CHILDREN=[('Model',104,1,'&drawModelScriptedModelDrawModel',8),('Material',112,4,'&drawBeamFXShaderMaterial',0),('Texture',116,1,'&drawModelReplaceTexture',8)]+[(n,124+8*i,1,'&drawModelBoneAttachPoint',16) for i,n in enumerate(['WeaponFireFXBone','WeaponRecoilBone','WeaponMuzzleFlash','WeaponLaunchBone'])]+[('ParticleSysBone',156,1,'&drawModelParticleSysBone',36),('FXEvent',164,1,'&drawModelFXEvent',28),('ShadowInfo',172,4,'&drawModelShadowInfo',0),('Turret',176,1,'&drawModelModelConditionStateTurret',28),('SubObject',184,3,'nullptr',0)]
def generate():
 m=json.loads((E/'manifest.json').read_text());assert m['sourceBlob']=='6e40a9ed6c9f961d294d49768d911e6a6f354619'; bodies={}
 for x in m['methods']:
  t=(E/x['file']).read_text();b='\n'.join(t.splitlines()[2:]).rstrip()+'\n';assert hashlib.sha256(b.encode()).hexdigest()==x['bodySha256'];bodies[x['signature']]=(b,json.loads(t.splitlines()[1][13:]))
 def direct(op,n):return next(v for k,v in bodies.items() if k.startswith('internal unsafe static void '+op+'(') and ' '+n+'* @object,' in k)
 for name,stride in [('ScriptedModelDrawModel',8),('ReplaceTexture',8),('BoneAttachPoint',16),('ParticleSysBone',36),('FXEvent',28),('ModelConditionStateTurret',28)]:
  lb=next(v[0] for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003A'+name+'_003E(IList*'))
  assert re.search(r'Push\(tracker, [^;]*, '+str(stride)+r'u, num\);',lb),(name,stride)
 lines=['// Generated from pinned original draw-source methods; complete model conditions.']
 for n in ['ParseCondStateType','FXTriggerType','FXActionType','ShadowType','WeaponSlotType']:
  vals=m['peEnums'][n];eb=next(v[0] for k,v in bodies.items() if k.startswith('internal unsafe static void Marshaler_002EMarshal_003Cenum_0020') and n+'_002Cclass' in k);unknown=int(re.search(r'ptr == null\) \? (\d+)',eb)[1]);lines+=['const EnumNameValue drawModel'+n+'Names[] = {']+['    { '+json.dumps(k)+', '+str(v)+' },' for k,v in sorted(vals.items())]+['};',f'const AIEnum drawModel{n} = {{ drawModel{n}Names, {len(vals)}u, {unknown}, 0u }};']
 for n,fs in FIELDS.items():
  cs=CHILDREN if n=='ModelConditionState' else [];mb,lits=direct('Marshaler_002EMarshal',n);actual=[]
  for l in mb.splitlines():
   syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',l)
   if syms:actual.append((lits[syms[0]],lits[syms[1]] if len(syms)>1 else None))
  assert actual==[(x[0],x[3]) for x in fs]+[(x[0],None) for x in cs],(n,actual)
  lines+=['const ModuleField drawModel'+n+'Fields[] = {']+['    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(fn),o,k,'nullptr' if d is None else json.dumps(d),'nullptr' if e is None else '&drawModel'+e) for fn,o,k,d,e in fs]+['};']
  tb,_=direct('Tokenizer_002ETokenize',n);chunks=tb.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');rs=[]
  for c in chunks[:-1]:
   key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',c)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',c)[1])&0xffffffff
   o=re.search(r'\(byte\*\)@object \+ (\d+)',c);scalar=re.search(r'\((?:float|int|\w+Type)\*\)@object \+ (\d+)',c);off=int(o[1]) if o else 4*int(scalar[1]) if scalar else 0
   nested='nullptr';mode=0;stride=0
   for cn,co,cm,cl,cz in cs:
    if co==off:mode=cm;nested=cl;stride=cz
   rs.append((key,typ,off,nested,mode,stride))
  assert [r[2] for r in rs]==[x[1] for x in fs]+[x[1] for x in cs],(n,rs)
  assert len(rs)+1==int(re.search(r'12u, (\d+)u',tb)[1])
  lines+=['const ModuleToken drawModel'+n+'Tokens[] = {']+['    { %du, %du, %du, %s, %du, %du },'%r for r in rs]+['};',f'const ModuleLayout drawModel{n} = {{ drawModel{n}Tokens, {len(rs)}u }};']
 lines+=[''];return '\n'.join(lines)
if __name__=='__main__':
 out=generate();p=ROOT/'recovery/src/Native/GameObjectDrawModelConditions.h'
 if '--check' in sys.argv:assert p.read_text()==out
 else:p.write_text(out)
 print('PASS complete model condition/particle/FX pinned contracts')
