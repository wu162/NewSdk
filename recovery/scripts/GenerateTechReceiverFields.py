"""Complete TechUpgradeReceiver contract pinned to original Marshal/Endian/Token bodies."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
FIELDS=[('Radius',36,'Float',None),('RadiusY',40,'NullableFloat',None),('DistanceType',44,'Enum','CIRCULAR'),('FXLineBoneName',48,'String',None),('RadarDrawModule',56,'AssetId',None),('RadarRotateSpeed',60,'Angle','0.1r'),('RadarRotateOffset',64,'Angle','0r')]
EVIDENCE=ROOT/'recovery/evidence/gameobject-modules/tech-receiver-source'
def generate(source=None,pe=None):
    manifest=json.loads((EVIDENCE/'manifest.json').read_text());assert manifest['sourceBlob']==BLOB
    module=manifest['modules'][0];assert (module['typeId'],module['nativeSize'])==(393460526,180)
    methods={}
    for meta in module['methods']+manifest['helpers']:
        exported=(EVIDENCE/meta['file']).read_text();sig=meta['signature'];body=body_of(exported,sig)
        assert hashlib.sha256((body+"\n").encode()).hexdigest()==meta['bodySha256'],meta['file']
        if source is not None:assert body_of(source,sig)==body,meta['file']
        methods[meta['file']]=(body,json.loads(exported.splitlines()[1].removeprefix('// Literals: ')))
    assert len(methods)==44
    enums=json.loads((EVIDENCE/'enum-source.json').read_text());assert enums['sourceBlob']==BLOB
    entries=enums['ReceiverDistanceType'];assert entries=={'CIRCULAR':0,'RECTANGULAR':1}
    if source is not None:assert enum_values(source,pe,'ReceiverDistanceType')==entries
    body,literals=methods['003.txt'];actual=[]
    for line in body.splitlines():
        syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
    assert actual==[(n,d)for n,o,k,d in FIELDS]+[('ObjectFilter',None)]
    for fragment in ['(float*)@object)[9]', '(byte*)@object + 40','(ReceiverDistanceType*)@object + 11','(byte*)@object + 48','(byte*)@object + 56','(float*)@object + 15','(float*)@object + 16','(byte*)@object + 68','(UpgradeModuleData*)@object']:assert fragment in body,fragment
    assert '? ((int*)ptr)[1] : 0' in methods['021.txt'][0] and '2u)' in methods['021.txt'][0]
    assert 'FastHash' in methods['023.txt'][0] and 'CharToLower' in methods['023.txt'][0]
    endian=methods['004.txt'][0]
    for fragment in ['((int*)@object)[9]','((uint*)@object)[10]','((int*)@object)[11]','((int*)@object)[12]','((int*)@object)[14]','((int*)@object)[15]','((int*)@object)[16]','(ObjectFilter*)((byte*)@object + 68)','(UpgradeModuleData*)@object']:assert fragment in endian,fragment
    lines=['// Generated complete TechUpgradeReceiver; original blob '+BLOB+'.','const EnumNameValue receiverDistanceNames[] = {']
    lines+=['    { %s, %d },'%(json.dumps(n),v)for n,v in entries.items()]
    lines+=['};','const AIEnum receiverDistance = { receiverDistanceNames, 2u, 0, 0u };','const ModuleField receiverFields[] = {']
    for n,o,k,d in FIELDS:lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),'&receiverDistance'if k=='Enum'else 'nullptr'))
    lines+=['};','const ModuleFieldSet receiverFieldSet = { receiverFields, 7u };','const ModuleToken receiverTokens[] = {']
    token=methods['005.txt'][0];parts=token.split('ptr = (ProcTableElement*)((byte*)ptr + 12);');assert len(parts)==10 and '12u, 10u)'in parts[0]
    records=[]
    for part in parts[:-1]:
        key=int(re.search(r'\*\(int\*\)ptr = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)ptr\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        off=re.search(r'\(byte\*\)@object \+ (\d+)',part);off=int(off[1])if off else 0
        nested='&upgradeUpgradeModuleData'if '(UpgradeModuleData*)@object'in part else 'nullptr'
        mode=8 if '(ObjectFilter*)'in part else 2 if 'if (num2 != 0)'in part else 0
        records.append((key,typ,off,mode));lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,off,nested,mode))
    assert records==[(1,2925571736,0,0),(1328662213,1683514611,36,0),(3519330798,1683514611,40,2),(236086057,873624354,44,0),(2718491374,438907818,48,0),(2110540067,3186653541,56,0),(2948402585,1310124621,60,0),(4223502151,1310124621,64,0),(3832456114,3832456114,68,8)],records
    for fragment in ['*(int*)ptr = 0;','((int*)ptr)[1] = 0;','((int*)ptr)[2] = 0;']:assert fragment in parts[-1]
    lines+=['};','const ModuleLayout receiverLayout = { receiverTokens, 9u };','const ModuleChoice receiverChoice = { 393460526u, &receiverLayout, 180u, 60u };','']
    return '\n'.join(lines)
if __name__=='__main__':
    source=pe=None
    if '--exported-evidence'not in sys.argv:
        source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
        pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes()
    output=generate(source,pe);path=ROOT/'recovery/src/Native/GameObjectTechReceiver.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectTechReceiver.h'
    else:path.write_text(output)
    print('PASS complete TechUpgradeReceiver seven fields, original Endian, independent enum type, nine Tokens and sentinel'+(' (exported evidence only)'if source is None else ' (original blob and PE verified)'))
