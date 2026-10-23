"""Source-only contract plan for the remaining complete sound-selector group.

Does not emit runtime code, enable module TypeIds, or claim DLL validation.
The original module fingerprint is checked before reading exact overloads.
"""
import hashlib
import json
import re
import sys
from pathlib import Path
from ExtractGameObjectEvidence import method

ROOT=Path(__file__).resolve().parents[2]
s=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw=s.encode('utf-8')
SOURCE_BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==SOURCE_BLOB
inventory=json.loads((ROOT/'recovery/evidence/gameobject-modules/source-inventory.json').read_text())
literals={}
for line in s.splitlines():
    m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
    if m:
        try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
        except UnicodeDecodeError:pass
signatures=[m[0].strip() for m in re.finditer(r'\tinternal unsafe static void [^\n]+',s)]

def exact(signature):
    b,l=method(s,signature)
    return b,{'signature':signature,'line':l,'bodySha256':hashlib.sha256(b.encode()).hexdigest()}

def marshal_contract(signature):
    body,out=exact(signature);lookups=[]
    for i,line in enumerate(body.splitlines()):
        refs=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if not refs:continue
        assert refs[0] in literals,(signature,refs[0])
        vtable=re.search(r'\*\(int\*\)\w+ \+ (4|8|12)\)',line)
        lookups.append({'name':literals[refs[0]],'fallback':literals[refs[1]] if len(refs)>1 else None,
                        'kind':{'4':'list','8':'optional-child','12':'attribute'}.get(vtable[1] if vtable else '', 'inspect-call'),
                        'line':out['line']+i})
    out['lookups']=lookups
    out['pushes']=[{'elementSize':int(a),'countExpression':b.strip()} for a,b in re.findall(r'Relo_002ETracker_002EPush\([^;\n]+?, (\d+)u, ([^;)]+)\)',body)]
    out['marshalCasts']=re.findall(r'Marshaler_002EMarshal\([^;\n]*?, \((\w+)\*\)',body)
    return out

def token_contract(signature):
    body,out=exact(signature)
    push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body)
    if not push:
        out['forwardCalls']=re.findall(r'Tokenizer_002E\w+\([^;\n]+',body)
        assert out['forwardCalls'],signature
        return out
    ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);')
    assert len(parts)==int(push[2]),(signature,len(parts),push[2])
    records=[]
    for part in parts[:-1]:
        key=re.search(r'\*\((?:int|uint)\*\)'+ptr+r' = (-?\d+);',part)
        typ=re.search(r'\(\((?:int|uint)\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)
        assert key and typ,(signature,part)
        direct=re.search(r'\(byte\*\)@object \+ (\d+)',part)
        assert '@object' in part or 'pointer' in part,(signature,part)
        helpers=re.findall(r'(Tokenizer_002E\w+)\(',part)
        semantics='helper' if helpers else 'member' if 'BookmarkMemberOffset(state, @object,' in part or 'BookmarkMemberOffset(state, (byte*)@object' in part else 'nullable-pointee'
        records.append({'nameHash':int(key[1])&0xffffffff,'typeHash':int(typ[1])&0xffffffff,
                        'objectOffset':int(direct[1]) if direct else 0,'binding':semantics,'helpers':helpers})
    assert re.search(r'\*\(int\*\)'+ptr+r' = 0;',parts[-1])
    assert re.search(r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;',parts[-1])
    out.update({'recordCountIncludingSentinel':int(push[2]),'records':records,'zeroSentinel':True})
    return out

def endian_contract(signature):
    body,out=exact(signature)
    out['calls']=re.findall(r'(Marshaler_002EEndianCorrect\w*)\([^;\n]+',body)
    return out

modules=['RandomSoundSelectorClientBehaviorModuleData','ModelConditionSoundSelectorClientBehaviorModuleData','UpgradeSoundSelectorClientBehaviorModuleData','ModelConditionEvaEventSelectorClientBehaviorModuleData']
nested=['ModelConditionSoundSelectorOverrideBlock','UpgradeSoundSelectorOverrideBlock','ModelConditionEvaEventSelectorOverrideBlock','AudioArrayVoice','AudioArraySound','EvaEventArray','AudioVoiceEntry','AudioObjectSpecificVoiceEntry','AudioVoiceReferentialEntry','SoundOrEvaEvent','AudioSoundEntry','EvaEventEntry']
module_rows=[]
for name in modules:
    key,row=next((k,v) for k,v in inventory['marshal'].items() if v['name']==name)
    direct='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
    signature=direct if direct in s else row['helperSignature']
    token=inventory['token'][key]['helperSignature']
    module_rows.append({'name':name,'typeId':int(key),'nativeSize':row['nativeSize'],
                        'marshal':marshal_contract(signature),'token':token_contract(token),
                        'endian':endian_contract(inventory['endian'][key]['helperSignature'])})
nested_rows=[]
for name in nested:
    marshal='internal unsafe static void Marshaler_002EMarshal(INode* node, '+name+'* @object, Tracker* state)'
    token='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
    if marshal in s: m=marshal_contract(marshal)
    else:
        m={'status':'no exact standalone Marshal overload; inspect list/parent inline implementation','name':name}
    candidates=[x for x in signatures if x.startswith('internal unsafe static void Marshaler_002EMarshal_') and name+'_003E(IList* list' in x]
    lists=[marshal_contract(x) for x in candidates]
    direct_endian='internal unsafe static void Marshaler_002EEndianCorrect('+name+'* @object, EndianHelper* state)'
    endian=endian_contract(direct_endian) if direct_endian in s else {'status':'no exact standalone endian overload; inspect list/parent inline implementation'}
    list_endian=[endian_contract(x) for x in signatures if x.startswith('internal unsafe static void Marshaler_002EEndianCorrect_') and name+'_003E(' in x and 'List_' in x]
    nested_rows.append({'name':name,'marshal':m,'listMarshal':lists,'token':token_contract(token),'endian':endian,'listEndian':list_endian})
# Exact sizes from original IList allocation helpers, never inferred from XSD.
expected={'ModelConditionSoundSelectorOverrideBlock':132,'UpgradeSoundSelectorOverrideBlock':148,'ModelConditionEvaEventSelectorOverrideBlock':124,'AudioVoiceEntry':12,'AudioObjectSpecificVoiceEntry':16,'AudioVoiceReferentialEntry':12,'AudioSoundEntry':8,'EvaEventEntry':8}
for row in nested_rows:
    if row['name'] in expected:
        assert row['listMarshal'],row['name']
        assert any(p['elementSize']==expected[row['name']] for m in row['listMarshal'] for p in m['pushes']),row
        row['nativeListStride']=expected[row['name']]
        assert row['listEndian'],('Missing list endian',row['name'])
report={'status':'source contract plan only; runtime support and DLL validation are tracked separately in GameObject validation.json','sourceBlob':SOURCE_BLOB,
        'modules':module_rows,'nested':nested_rows,
        'reuseEvidence':'Existing root AudioArrayVoice/AudioArraySound/EvaEventArray are DLL validated; nested selector integration is not.'}
output=json.dumps(report,indent=2,ensure_ascii=False)+'\n'
path=ROOT/'recovery/evidence/gameobject-modules/sound-selector-source-plan.json'
if '--check' in sys.argv:assert path.read_text()==output,'Regenerate selector source plan'
else:path.write_text(output)
print('PASS sound selector source plan: 4 selector module source contracts; 12 nested contracts; 8 independently checked list strides; no runtime support claimed')
