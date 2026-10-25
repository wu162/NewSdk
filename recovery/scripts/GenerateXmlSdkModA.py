#!/usr/bin/env python3
"""Generate source-XSD-valid original/recovered SDK integration fixtures, Mod A.
No SDK execution. Does not modify original schemas or existing Mods.
"""
import copy,json,re,sys
from pathlib import Path
from lxml import etree as E
ROOT=Path(__file__).resolve().parents[2]
X='http://www.w3.org/2001/XMLSchema';NS='uri:ea.com:eala:asset';T='{'+X+'}';A='{'+NS+'}'
MOD=ROOT/'SDK/Mods/XmlCompilerRecoveryA';DATA=MOD/'Data';S=ROOT/'SDK/Schemas/xsd'
FILES={p.relative_to(S).as_posix().lower():p for p in S.rglob('*.xsd')}
DEFS={};DOCS=[];SEEN=set()
def load(path):
 path=path.resolve()
 if path in SEEN:return
 SEEN.add(path);doc=E.parse(str(path));DOCS.append(doc)
 for i in doc.getroot().findall(T+'include'):
  location=i.get('schemaLocation').replace('\\','/')
  p=path.parent/location
  if not p.is_file():
   rel=p.resolve().relative_to(S.resolve()).as_posix().lower();p=FILES[rel]
  load(p)
 for c in doc.getroot():
  if c.get('name') and c.tag in [T+'complexType',T+'simpleType',T+'group',T+'attributeGroup',T+'element']:
   key=(E.QName(c).localname,c.get('name'))
   if key not in DEFS:DEFS[key]=c
load(S/'CnC3Types.xsd')
CAT=json.loads((ROOT/'recovery/catalog/XmlComponentOnlyTypes.json').read_text())
GROUPS=sorted(CAT['groups'])[:len(CAT['groups'])//2]
ROWS={r['name']:r for r in CAT['roots']}
NAMES=[name for g in GROUPS for name in CAT['groups'][g]]
GLOBALS={c.get('name'):c for d in DOCS for c in d.getroot().iter(T+'element') if c.get('name') in ROWS and c.get('type')==c.get('name')}
# Selected original supported owner assets cover abstract/nested registered types.
OWNERS={'BaseConditionData':[('GameplayConditionData','PrimaryObject')], 'ObjectConditionData':[('GameplayConditionData','PrimaryObject')], 'UpgradeConditionData':[('GameplayConditionData','RequiredUpgradeCondition')], 'ConquerCardDefinitionBase':[('ConquerCardDefinitionUnit','inherited base fields')], 'ShaderOverrideRule':[('ShaderOverride','Rule')]}
IDS={name:'XmlRecoveryA_'+name+'_defaults' for name in NAMES if ROWS[name].get('schemaGlobal')}
IDS['LogicCommand']='XmlRecoveryA_Support_LogicCommand'
ACTIVE_VARIANT='defaults'
NESTED_IDS=0
BUILTINS={'string','normalizedString','token','anyURI','base64Binary','hexBinary','boolean','float','double','decimal','integer','int','long','short','byte','unsignedInt','unsignedLong','unsignedShort','unsignedByte','nonNegativeInteger','positiveInteger','negativeInteger','nonPositiveInteger'}
def local(name):return (name or '').strip().split(':')[-1]
def definition(typ):
 d=DEFS.get(('complexType',local(typ)))
 return d if d is not None else DEFS.get(('simpleType',local(typ)))
def simple(node,rich=False,seen=()):
 if node is None:return ('BoundaryA' if ACTIVE_VARIANT=='repeat' else 'SampleA') if rich else ''
 if isinstance(node,str):
  name=local(node)
  if name in seen:return ''
  if name in ('SageInt','SageUnsignedInt','SageUnsignedShort','SageDurationUnsignedInt','DurationUnsignedInt'):return ('27' if ACTIVE_VARIANT=='repeat' else '17') if rich else '0'
  if name in ('SageReal','PositiveNonZeroReal','Velocity'):return ('0.125' if ACTIVE_VARIANT=='repeat' else '1.25') if rich else '1'
  if name=='SageBool':return 'true' if rich else 'false'
  if name=='Time':return ('1500ms' if ACTIVE_VARIANT=='repeat' else '250ms') if rich else '0s'
  if name=='Percentage':return ('75%' if ACTIVE_VARIANT=='repeat' else '25%') if rich else '0%'
  if name=='Angle':return ('-45d' if ACTIVE_VARIANT=='repeat' else '30d') if rich else '0d'
  if name in BUILTINS:
   if name=='boolean':return 'true' if rich else 'false'
   if name in ('float','double','decimal'):return '1.25' if rich else '0'
   if name in ('byte','unsignedByte','short','unsignedShort','int','integer','long','unsignedInt','unsignedLong','nonNegativeInteger','positiveInteger'):return '3' if rich else '1'
   if name in ('negativeInteger','nonPositiveInteger'):return '-1'
   return 'SampleA' if rich else ''
  if name in ('AssetReference','WeakReference'):return ''
  if name.endswith('Ref') and name[:-3] in IDS:return IDS[name[:-3]]
  if name in ('InstanceId','Poid'):return 'SampleA'
  if name=='HashCode':return '0123abcd'
  if name in ('DataBlob','FileReference'):return 'Mods/XmlCompilerRecoveryA/Data/fixture.bin'
  d=definition(name)
  return simple(d,rich,seen+(name,)) if d is not None else ('SampleA' if rich else '')
 restriction=node.find(T+'restriction')
 if restriction is not None:
  enums=restriction.findall(T+'enumeration')
  if enums:return enums[-1 if rich else 0].get('value')
  base=local(restriction.get('base'));v=simple(base,rich,seen)
  if node.get('name','').endswith('Ref') and node.get('name')[:-3] in IDS:return IDS[node.get('name')[:-3]]
  if restriction.find(T+'minInclusive') is not None:v=restriction.find(T+'minInclusive').get('value')
  if restriction.find(T+'minExclusive') is not None:v=str(float(restriction.find(T+'minExclusive').get('value'))+1)
  if restriction.find(T+'maxInclusive') is not None:
   try:v=str(min(float(v),float(restriction.find(T+'maxInclusive').get('value'))))
   except ValueError:pass
  pattern=restriction.find(T+'pattern')
  if pattern is not None:
   p=pattern.get('value')
   if 'ms' in p or 's|m' in p:v=('1500ms' if ACTIVE_VARIANT=='repeat' else '250ms') if rich else '0s'
   elif '%' in p:v=('75%' if ACTIVE_VARIANT=='repeat' else '25%') if rich else '0%'
   elif '[0-9a-fA-F]{8}' in p:v='0123abcd'
   elif base in ('AssetReference','WeakReference'):v=''
   elif '\\' in p and ':' in p:v='SampleA'
  return v
 lst=node.find(T+'list')
 if lst is not None:
  t=lst.get('itemType');v=simple(t or lst.find(T+'simpleType'),rich,seen)
  return v+(' '+v if rich and v else '')
 union=node.find(T+'union')
 if union is not None:
  m=(union.get('memberTypes') or '').split()
  return simple(m[0],rich,seen) if m else simple(union.find(T+'simpleType'),rich,seen)
 return 'SampleA' if rich else ''

def attrs(model):
 out=[]
 ext=model.find('.//'+T+'extension')
 if ext is not None:
  base=definition(ext.get('base'))
  if base is not None:out+=attrs(base)
 # Attributes belonging to this type, excluding nested element definitions.
 def walk(n):
  for a in n:
   if a.tag==T+'attribute':out.append(a)
   elif a.tag==T+'attributeGroup':
    d=DEFS.get(('attributeGroup',local(a.get('ref'))))
    if d is not None:walk(d)
   elif a.tag in (T+'complexContent',T+'simpleContent',T+'extension',T+'restriction'):walk(a)
 walk(model);return out

def sequence_models(model):
 result=[]
 ext=model.find('.//'+T+'extension')
 if ext is not None:
  base=definition(ext.get('base'))
  if base is not None:result+=sequence_models(base)
 def walk(n):
  for a in n:
   if a.tag in (T+'sequence',T+'choice',T+'all',T+'group'):result.append(a)
   elif a.tag in (T+'complexContent',T+'simpleContent',T+'extension',T+'restriction'):walk(a)
 walk(model);return result

def field_value(attr,rich):
 if attr.get('fixed') is not None:return attr.get('fixed')
 if attr.get('default') is not None and not rich:return attr.get('default')
 t=attr.get('type');node=t or attr.find(T+'simpleType');return simple(node,rich)

def populate(element,typ,rich,depth=0,stack=()):
 global NESTED_IDS
 model=definition(typ) if isinstance(typ,str) else typ
 if model is None:
  element.text=simple(typ,rich);return
 if model.tag==T+'simpleType':element.text=simple(model,rich);return
 typename=model.get('name','anonymous')
 for a in attrs(model):
  name=a.get('name')
  if not name or a.get('use')=='prohibited':continue
  if name in ('typeHashCode','buildRule','TypeId','inheritFrom'):continue
  required=a.get('use')=='required'
  if not(required or rich):continue
  # Avoid inventing strong references to types not defined in this test stream.
  typeName=local(a.get('type'))
  if not required and (typeName.endswith('Ref') or typeName=='AssetReference') and typeName[:-3] not in IDS:continue
  value=field_value(a,rich)
  if name=='id':
   NESTED_IDS+=1;value='NestedA_'+str(NESTED_IDS)
  if name.lower() in ('file','filename') and typeName in ('DataBlob','FileReference'):value='Mods/XmlCompilerRecoveryA/Data/fixture.bin'
  element.set(name,value)
 for seq in sequence_models(model):
  particles(element,seq,rich,depth,stack+(typename,))
 # SimpleContent types retain text and attrs.
 simplecontent=model.find(T+'simpleContent')
 if simplecontent is not None:
  ext=simplecontent.find(T+'extension')
  if ext is not None:element.text=simple(ext.get('base'),rich)

def particles(element,particle,rich,depth,stack):
 minimum=int(particle.get('minOccurs','1'))
 if not rich and minimum==0:return
 if depth>5 and minimum==0:return
 if particle.tag==T+'element':
  count=minimum
  if rich:count=max(count,1)
  maximum=particle.get('maxOccurs','1')
  if rich and maximum=='unbounded' and depth<2:count=max(count,7 if ACTIVE_VARIANT=='repeat' else 3)
  count=min(count,int(maximum)) if maximum!='unbounded' else count
  for _ in range(count):
   name=particle.get('name') or local(particle.get('ref'))
   if not name:continue
   c=E.SubElement(element,A+name)
   typ=particle.get('type')
   if typ is None:
    typ=particle.find(T+'complexType')
    if typ is None:typ=particle.find(T+'simpleType')
   if typ is None:
    ref=DEFS.get(('element',local(particle.get('ref'))));typ=ref.get('type') if ref is not None else 'xs:string'
   if isinstance(typ,str) and local(typ) in stack and depth>4:
    if minimum:populate(c,typ,False,depth+1,stack)
    continue
   populate(c,typ,rich,depth+1,stack)
  return
 if particle.tag==T+'group':
  d=DEFS.get(('group',local(particle.get('ref'))))
  if d is not None:
   for p in d:particles(element,p,rich,depth,stack)
  return
 children=[c for c in particle if c.tag in (T+'element',T+'sequence',T+'choice',T+'all',T+'group')]
 if particle.tag==T+'choice':
  if children:
   # Schema-order polymorphic branch is a real concrete element; BAB supplies TypeId.
   particles(element,children[(len(children)//2 if ACTIVE_VARIANT=='repeat' else len(children)-1) if rich else 0],rich,depth,stack)
 else:
  for c in children:particles(element,c,rich,depth,stack)

def aggregate_schema():
 schema=E.Element(T+'schema',nsmap={'xs':X,None:NS,'at':'uri:ea.com:eala:asset:type','xas':'uri:ea.com:eala:asset:schema','balance':'uri:ea.com:eala:balance'},targetNamespace=NS,elementFormDefault='qualified')
 for (kind,name),node in DEFS.items():
  node=copy.deepcopy(node)
  for n in node.iter():
   for a in ('type','base','ref','itemType'):
    if n.get(a) is not None:n.set(a,n.get(a).strip())
  schema.append(node)
 return E.XMLSchema(E.ElementTree(schema))

def create():
 global ACTIVE_VARIANT
 DATA.mkdir(parents=True,exist_ok=True)
 (DATA/'fixture.bin').write_bytes(bytes(range(32))+b'XMLRECOVERY-A\0')
 (DATA/'fixture-empty.bin').write_bytes(b'')
 (DATA/'fixture-odd.bin').write_bytes(b'padding-check')
 document=E.Element(A+'AssetDeclaration',nsmap={None:NS})
 support=E.SubElement(document,A+'LogicCommand');populate(support,'LogicCommand',False);support.set('id',IDS['LogicCommand'])
 manifest={'mod':'XmlCompilerRecoveryA','scope':'24 sorted component-only groups; actual SDK integration fixtures, source XSD validated before BAB','groups':GROUPS,'roots':{},'nestedRegisteredMappings':OWNERS,'sdkUnreachableRegisteredRoots':{'CommandSet':'Declared complexType but no AssetDeclaration global element and no nested owner uses CommandSet in source XSD; LogicCommandSet is a different type. Component validation only.'},'files':['Data/Mod.xml','Data/FixturesA.xml','Data/fixture.bin','Data/fixture-empty.bin','Data/fixture-odd.bin'],'validation':'pending','originalSchemasModified':False}
 for group in GROUPS:
  for name in CAT['groups'][group]:
   if not ROWS[name].get('schemaGlobal'):continue
   cases=[]
   for variant,rich in [('defaults',False),('rich',True),('repeat',True)]:
    ACTIVE_VARIANT=variant
    inherited=variant=='repeat' and any(a.get('name')=='inheritFrom' for a in attrs(definition(name)))
    node=E.SubElement(document,A+name);populate(node,name,False if inherited else rich)
    if inherited:node.set('inheritFrom','XmlRecoveryA_'+name+'_rich')
    node.set('id','XmlRecoveryA_'+name+'_'+variant)
    # File blob inputs deliberately cover actual file content; defaults remain valid source literals.
    for item in node.iter():
     for key in list(item.attrib):
      if key in ('File','FileName') and item.get(key)=='Mods/XmlCompilerRecoveryA/Data/fixture.bin':item.set(key,'Mods/XmlCompilerRecoveryA/Data/'+{'defaults':'fixture-empty.bin','rich':'fixture.bin','repeat':'fixture-odd.bin'}[variant])
    cases.append({'id':node.get('id'),'variant':('inherited' if inherited else variant),'attributes':len(node.attrib)-1,'descendants':len(list(node.iter()))-1})
   manifest['roots'][name]={'group':group,'schemaGlobal':True,'cases':cases}
 # Genuine nested owner for the non-global CommandSet entry.
 if 'LogicCommandSet' not in manifest['roots']:
  support=E.SubElement(document,A+'LogicCommandSet');populate(support,'LogicCommandSet',True);support.set('id','XmlRecoveryA_Support_LogicCommandSet')
 # Deliberate XML-valid text boundaries in the existing rich GameMap case.
 for asset in document:
  if E.QName(asset).localname=='GameMap' and asset.get('id','').endswith('_rich'):
   world=asset.find(A+'WorldDict')
   if world is not None:
    string_pair=world.find(A+'StringProperty')
    unicode_pair=world.find(A+'UnicodeStringProperty')
    token_pair=world.find(A+'AssetIdListProperty')
    if string_pair is None:string_pair=E.SubElement(world,A+'StringProperty',Key='EscapedXml')
    string_pair.set('Value','space & <tag> "quote"')
    if unicode_pair is None:unicode_pair=E.SubElement(world,A+'UnicodeStringProperty',Key='UnicodeText')
    unicode_pair.set('Value','汉字é😀')
    if token_pair is not None:token_pair.set('Value','  Alpha\tBeta\nGamma  ')
 # Refresh per-case shape after curated text fixtures append valid dictionary entries.
 for asset in document:
  root=manifest['roots'].get(E.QName(asset).localname)
  if root:
   for case in root['cases']:
    if case['id']==asset.get('id'):
     case['attributes']=len(asset.attrib)-1;case['descendants']=len(list(asset.iter()))-1
 schema=aggregate_schema()
 if not schema.validate(document):
  (MOD/'xsd-errors.txt').write_text('\n'.join(error.message for error in schema.error_log))
  for message in list(dict.fromkeys(error.message for error in schema.error_log))[:30]:print(message,file=sys.stderr)
  raise RuntimeError('Generated Mod A fixtures are not yet XSD valid')
 manifest['validation']='source XSD passed (in-memory include flattening solely for case-insensitive Windows schema paths; original files unchanged)'
 manifest['assetCount']=len(document);manifest['globalRoots']=len(manifest['roots']);manifest['declaredAssets']=[{'type':E.QName(n).localname,'id':n.get('id')} for n in document]
 errorfile=MOD/'xsd-errors.txt'
 if errorfile.exists():errorfile.unlink()
 (DATA/'FixturesA.xml').write_bytes(E.tostring(document,pretty_print=True,xml_declaration=True,encoding='utf-8'))
 mod=E.Element(A+'AssetDeclaration',nsmap={None:NS});includes=E.SubElement(mod,A+'Includes')
 for source in ('DATA:static.xml','DATA:global.xml','DATA:audio.xml'):E.SubElement(includes,A+'Include',type='reference',source=source)
 E.SubElement(includes,A+'Include',type='all',source='FixturesA.xml')
 (DATA/'Mod.xml').write_bytes(E.tostring(mod,pretty_print=True,xml_declaration=True,encoding='utf-8'))
 (MOD/'fixture-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
 print(json.dumps({'mod':str(MOD),'groups':len(GROUPS),'globalRoots':manifest['globalRoots'],'assets':manifest['assetCount'],'xsd':True}))
if __name__=='__main__':create()
