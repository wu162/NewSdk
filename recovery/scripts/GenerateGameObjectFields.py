"""Generate readable root metadata from original source and PE; no buffer snapshots."""
import json, re, struct, sys, hashlib, math
from pathlib import Path
import xml.etree.ElementTree as ET
from ExtractGameObjectEvidence import method, token_table

root = Path(__file__).resolve().parents[2]
s = (root/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
# Require the exact characterized original module.
raw = (root/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_bytes()
assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest() == '6e40a9ed6c9f961d294d49768d911e6a6f354619'
marshal, first = method(s, 'internal unsafe static void Marshaler_002EMarshal(INode* node, GameObject* @object, Tracker* state)')
tokens = token_table(*method(s, 'internal unsafe static void Tokenizer_002ETokenize(void** table, GameObject* @object, Tracker* state)'))
schema = ET.parse(root/'SDK/Schemas/xsd/AssetTypeGameObject.xsd')
names = {n.attrib['name'] for n in schema.iter() if 'name' in n.attrib}

def decode(symbol):
    text = symbol.split('_0040')[-2].removesuffix('_003F_0024AA')
    return text.replace('_003F4', '.').replace('_003F9', '-')

pe = (root/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
po = struct.unpack_from('<I', pe, 60)[0]
ib = struct.unpack_from('<I', pe, po+52)[0]
sb = po+24+struct.unpack_from('<H', pe, po+20)[0]
def string_at(va):
    for i in range(struct.unpack_from('<H', pe, po+6)[0]):
        _,size,base,rs,off = struct.unpack_from('<8sIIII', pe, sb+40*i)
        if base <= va-ib < base+max(size,rs):
            start = off+va-ib-base
            return pe[start:pe.index(b'\0',start)].decode('ascii')
    raise ValueError(hex(va))

def enum_values(group):
    line = next(l for l in s.splitlines() if 'internal static' in l and 'enumMap' in l and 'W4'+group+'_0040' in l)
    return {string_at(p):v for p,v in struct.iter_unpack('<Ii',bytes.fromhex(re.search(r'data\(([^)]+)',line)[1]))}

fields=[]
for index,line in enumerate(marshal.splitlines()):
    # Direct scalar/string attributes; lists, references, IDs, masks and blocks remain unsupported.
    m=re.search(r'(Marshaler_002EMarshal_\w+)\(',line)
    if not m or 'Unsafe.AsPointer' not in line: continue
    helper=m[1]
    typ=re.search(r'\((float|int|uint|ushort|bool|byte|\w+Type|UnitCategory|WeaponCategory)\*\)@object \+ (\d+), state',line)
    string='Relo_003A_003AString_003Cchar_003E_002Cclass' in helper
    if not typ and not string: continue
    symbols=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
    prefix=decode(symbols[0])
    candidates=[prefix] if prefix in names else [n for n in names if n.startswith(prefix)]
    if len(candidates)>1 and prefix=='EvaEventSecondDamageFarFromFirst':
        candidates=[n for n in candidates if n.endswith('TimeoutMS' if 'DurationTag' in helper else 'ScanRange')]
    if len(candidates)!=1: raise ValueError((prefix,candidates))
    name=candidates[0]
    fallback=decode(symbols[1]) if len(symbols)>1 else None
    if string:
        offset=int(re.search(r'\(byte\*\)@object \+ (\d+)\), state',line)[1]);kind='String';default=None
    else:
        t=typ[1];offset=int(typ[2])*({'ushort':2,'bool':1,'byte':1}.get(t,4))
        if t=='float': kind='Time' if 'TimeTag' in helper else 'Angle' if 'AngleTag' in helper else 'Float'
        else: kind={'int':'Signed','uint':'Unsigned','ushort':'Short','bool':'Boolean','byte':'Byte'}.get(t,'Enum')
        default=None
        if kind=='Enum':
            group='BuildPlacementType' if 'BuildPlacementType' in helper else t
            default=enum_values(group)[fallback]
        if 'DurationTag' in helper:
            kind='DurationDefault'
            constant=next(l for l in s.splitlines() if 'internal static float DataInitializers_002E_003FA0xf4a5029c_002ELOGICFRAMES_PER_MSEC_REAL' in l)
            scale=struct.unpack('<f',bytes.fromhex(re.search(r'data\(([^)]+)',constant)[1]))[0]
            single=lambda x:struct.unpack('<f',struct.pack('<f',x))[0]
            default=math.ceil(single(scale*single(float(fallback))))
    fields.append((name,offset,kind,fallback,default,first+index))
assert len(fields)==75, len(fields)
lines=['// Generated from original Marshal/Tokenize and PE enum maps. See GenerateGameObjectFields.py.', 'const GameObjectField fields[] = {']
for n,o,k,f,d,l in fields:
    lines.append('    { '+json.dumps(n)+', '+str(o)+'u, GameObjectValue::'+k+', '+('nullptr' if f is None else json.dumps(f))+', '+str(d or 0)+' }, // original line '+str(l))
lines+=['};','const GameObjectToken tokens[] = {']
list_offsets={40,412,420,444,472,536,560,568}
for t in tokens[:-1]:
    kind='List' if t['nativeOffset'] in list_offsets else 'Member' if t['kind']=='member-bookmark' or t['nativeOffset']<396 or t['nativeOffset']>=576 else 'Null'
    lines.append('    { '+str(t['nameHash'])+'u, '+str(t['typeHash'])+'u, '+str(t['nativeOffset'])+'u, GameObjectTokenKind::'+kind+' },')
lines+=['};','']
output='\n'.join(lines)
path=root/'recovery/src/Native/GameObjectFields.h'
if '--check' in sys.argv:
    assert path.read_text()==output, 'Regenerate GameObjectFields.h'
else: path.write_text(output)
print('PASS source-derived GameObject metadata: '+str(len(fields))+' scalar/string fields, 126 tokens; no frozen buffers')
