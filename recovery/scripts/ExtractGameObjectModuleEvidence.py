"""Inventory original GameObject module dispatch; no recovered support claim.

Records all three switches separately. Token-only cases are intentional evidence,
not permission to synthesize a Marshal branch. All values are source-derived.
"""
import hashlib
import json
import re
import sys
from pathlib import Path
from ExtractGameObjectEvidence import method

ROOT = Path(__file__).resolve().parents[2]
SOURCE_BLOB = '6e40a9ed6c9f961d294d49768d911e6a6f354619'
source = (ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
raw = source.encode('utf-8')
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest() == SOURCE_BLOB
# Retain overloaded signatures; do not choose methods solely by their short name.
methods = {}
for m in re.finditer(r'\tinternal unsafe static void ([^(\n]+)(\([^\n]+)', source):
    signature = m[0].strip()
    body, line = method(source, signature)
    methods.setdefault(m[1], []).append((signature, body, line))


def dispatch(signature, operation):
    body, first = method(source, signature)
    labels = list(re.finditer(r'\n\t\t\t(case (-?\d+)u?:|default:)', body))
    rows = {}
    for i, label in enumerate(labels):
        block = body[label.end():labels[i+1].start() if i+1<len(labels) else len(body)]
        # Every case ends with break; the final block also contains method epilogue.
        block = block[:block.index('break;')]
        call = re.search(r'(\w+(?:PolymorphicType_\w+))\(', block)
        if call:
            helper = call[1]
            concrete = re.search(r'_003A_003A(\w+)_002Cstruct_0020SageBinaryData_003A_003AModuleData_003E', helper)
            name = concrete[1] if concrete else 'ModuleData'
            definitions = methods[helper]
            assert len(definitions)==1, helper
            helper_signature, helper_body, helper_line = definitions[0]
        else:
            direct = re.search(r'(Marshaler_002EEndianCorrect|Tokenizer_002ETokenize)\([^;\n]*?\((\w+)\*\)', block)
            assert direct, (operation, label[0], block)
            name = next(n for n in re.findall(r'\((\w+)\*\)',block) if n.endswith('Data'))
            helper = direct[1]
            definitions = [x for x in methods[helper] if re.search(r'[(,] '+re.escape(name)+r'\* @object,', x[0]) or ('('+name+'* @object,' in x[0])]
            assert len(definitions)==1, (operation, name)
            helper_signature, helper_body, helper_line = definitions[0]
        row = {'name':name, 'caseLine':first+body.count('\n',0,label.start())+1,
               'helperLine':helper_line, 'helperSignature':helper_signature}
        if operation=='marshal':
            size = re.search(r'Relo_002ETracker_002EPush\(tracker, \(void\*\*\)pointer, (\d+)u, 1u\)', helper_body)
            value = re.search(r'\*\(int\*\)ptr = (-?\d+);', helper_body)
            assert size and value, (name, helper_body)
            row['nativeSize']=int(size[1]);row['emittedTypeId']=int(value[1])&0xffffffff
            marshal_methods = [x for x in methods['Marshaler_002EMarshal'] if ', '+name+'* @object,' in x[0]]
            row['concreteMarshalLines']=[x[2] for x in marshal_methods]
            # The wrapper can contain an inlined body even when no separate
            # concrete Marshal overload survived compiler folding/decompilation.
            row['wrapperHasInlineLookups']='Unsafe.AsPointer' in helper_body
        key='default' if label[2] is None else str(int(label[2])&0xffffffff)
        assert key not in rows, (operation,key)
        if operation=='marshal' and key!='default': assert row['emittedTypeId']==int(key), (key,row)
        rows[key]=row
    return rows

marshal=dispatch('internal unsafe static void Marshaler_002EMarshal(INode* node, ModuleData** pointer, Tracker* state)','marshal')
endian=dispatch('internal unsafe static void Marshaler_002EEndianCorrect(ModuleData** pointer, EndianHelper* state)','endian')
token=dispatch('internal unsafe static void Tokenizer_002ETokenize(void** table, ModuleData** source, Tracker* state)','token')
assert len(marshal)==323 and len(endian)==323 and len(token)==380
assert set(marshal)==set(endian)
assert set(marshal)-{'default'} <= set(token)
aliases=[{'typeId':k,'marshal':marshal[k]['name'],'endian':endian[k]['name'],'token':token[k]['name']} for k in marshal if k!='default' and not marshal[k]['name']==endian[k]['name']==token[k]['name']]
assert aliases==[{'typeId':'2427007323','marshal':'HordeDispatchSpecialPowerModuleData','endian':'SpecialPowerModuleData','token':'HordeDispatchSpecialPowerModuleData'},{'typeId':'3395303590','marshal':'LeechTargetingAttachUpdateModuleData','endian':'AttachUpdateModuleData','token':'LeechTargetingAttachUpdateModuleData'}]
token_only=sorted(set(token)-set(marshal),key=int)
assert len(token_only)==58
def token_graph(signature, seen=()):
    assert signature not in seen, ('Recursive token graph',signature)
    body,line=method(source,signature)
    push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body)
    if not push:
        calls=re.findall(r'Tokenizer_002ETokenize\([^;]+?\((\w+Data)\*\)',body)
        assert len(calls)==1,(signature,body)
        candidates=[x for x in methods['Tokenizer_002ETokenize'] if ', '+calls[0]+'* @object,' in x[0]]
        assert len(candidates)==1,(calls,candidates)
        return {'sourceLine':line,'forwardTo':token_graph(candidates[0][0],seen+(signature,))}
    ptr=push[1]
    delimiter=ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);'
    parts=body.split(delimiter)
    assert len(parts)==int(push[2]),(signature,len(parts),push[2])
    records=[]
    for part in parts[:-1]:
        key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff
        typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        row={'nameHash':key,'typeHash':typ}
        bookmark=re.search(r'BookmarkMemberOffset\(state, (?:\(byte\*\))?@object(?: \+ (\d+))?,',part)
        if bookmark:
            row['memberOffset']=int(bookmark[1] or 0)
            assert row['memberOffset'] in (0,4),(signature,row)
        else:
            calls=re.findall(r'Tokenizer_002ETokenize\([^;]+?\((\w+Data)\*\)',part)
            assert len(calls)==1,(signature,part)
            candidates=[x for x in methods['Tokenizer_002ETokenize'] if ', '+calls[0]+'* @object,' in x[0]]
            assert len(candidates)==1,(calls,candidates)
            row['nested']=token_graph(candidates[0][0],seen+(signature,))
        records.append(row)
    sentinel=parts[-1]
    assert re.search(r'\*\(int\*\)'+ptr+r' = 0;',sentinel)
    assert re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',sentinel)
    assert re.search(r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;',sentinel)
    return {'sourceLine':line,'records':records,'zeroSentinel':True}

base_only=[]
for key,row in marshal.items():
    if key=='default' or row['nativeSize']!=8: continue
    body,_=method(source,row['helperSignature'])
    assert 'Unsafe.AsPointer' not in body
    assert re.findall(r'Marshaler_002EMarshal\(node, \((\w+)\*\)ptr, tracker\)',body)==['ModuleData'],row
    base_only.append({'typeId':int(key),'name':row['name'],'marshalLine':row['helperLine'],
                      'tokenGraph':token_graph(token[key]['helperSignature'])})
assert len(base_only)==31

report={'status':'source inventory only; not implemented or DLL validated', 'sourceBlob':SOURCE_BLOB,
        'dispatchCounts':{'marshalExplicit':322,'endianExplicit':322,'tokenExplicit':380,'tokenOnly':58},
        'marshal':marshal,'endian':endian,'token':token,'tokenOnly':token_only,'aliasCandidates':aliases,'baseOnlyCandidates':base_only}
output=json.dumps(report,indent=2,ensure_ascii=False)+'\n'
path=ROOT/'recovery/evidence/gameobject-modules/source-inventory.json'
if '--check' in sys.argv:
    assert path.read_text()==output, 'Regenerate module source inventory'
else:
    path.parent.mkdir(parents=True,exist_ok=True)
    path.write_text(output)
print('PASS original module source inventory: 322 Marshal/endian cases, 380 Token cases, 58 Token-only; no runtime validation claimed')
if '--summary' in sys.argv:
    small=sorted((v['nativeSize'],v['name'],k,v['helperLine']) for k,v in marshal.items() if k!='default')
    print(json.dumps({'smallest':small[:24],'tokenOnlyNames':[token[k]['name'] for k in token_only]},indent=2))
