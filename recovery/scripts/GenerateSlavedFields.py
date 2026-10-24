"""Complete original SlavedUpdate source, PE enums and scalar helper Token bindings."""
import hashlib,json,re,sys
from pathlib import Path
from GenerateLifecycleFields import body_of
from GenerateUpgradeFields import enum_values,BLOB,ROOT
FIELDS=[(n,8+4*i,'Signed','0')for i,n in enumerate(['LeashRange','GuardMaxRange','GuardWanderRange','AttackRange','AttackWanderRange','ScoutRange','ScoutWanderRange','DistToTargetToGrantRangeBonus','RepairRange'])]+[('RepairMinAltitude',44,'Float','0'),('RepairMaxAltitude',48,'Float','0'),('RepairWhenHealthBelowPercentage',52,'Signed','0'),('RepairRatePerSecond',56,'Float','0')]+[(n,60+4*i,'Signed','0')for i,n in enumerate(['MinReadyFrames','MaxReadyFrames','MinWeldFrames','MaxWeldFrames'])]+[('WeldingSysId',76,'Reference',None),('WeldingFXBone',80,'String',None),('StayOnSameLayerAsMaster',108,'Boolean','false'),('DieOnMastersDeath',109,'Boolean','false'),('DieOnMastersDeathDamageType',88,'Enum','UNRESISTABLE'),('DieOnMastersDeathType',92,'Enum','NORMAL'),('FadeOutRange',96,'Signed','0'),('FadeTime',100,'Unsigned','0'),('MarkUnselectable',110,'Boolean','false'),('UseSlaverAsControlForEvaObjectSightedEvents',111,'Boolean','false')]
def generate(source,enums):
    literals={}
    for line in source.splitlines():
        m=re.search(r'internal static .*? (\S+)/\* Not supported: data\(([^)]+)\)',line)
        if m:
            try:literals[m[1]]=bytes.fromhex(m[2]).rstrip(b'\0').decode('ascii')
            except UnicodeDecodeError:pass
    lines=['// Generated complete SlavedUpdate contract; source blob '+BLOB+'.']
    for group,count,fallback in [('DamageType',39,38),('DeathType',34,33)]:
        entries=enums[group];assert len(entries)==count
        helper=body_of(source,'internal unsafe static void Marshaler_002EMarshal_003Cenum_0020'+group+'_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, '+group+'* @object, Tracker* state)')
        assert '? '+str(fallback)+' :'in helper
        lines.append('const EnumNameValue slaved'+group+'Names[] = {')
        for n,v in entries.items():lines.append('    { %s, %d },'%(json.dumps(n),v))
        lines+=['};','const AIEnum slaved'+group+' = { slaved'+group+'Names, %du, %d, 0u };'%(count,fallback)]
    body=body_of(source,'internal unsafe static void Marshaler_002EMarshal(INode* node, SlavedUpdateModuleData* @object, Tracker* state)');actual=[]
    for line in body.splitlines():
        syms=re.findall(r'Unsafe.AsPointer\(ref ([^)]+)\)',line)
        if syms:actual.append((literals[syms[0]],literals[syms[1]]if len(syms)>1 else None))
    assert actual==[(n,d)for n,o,k,d in FIELDS]+[('GuardPosOffset',None)],actual
    for fragment in ['(int*)@object + 13','(int*)@object + 24','(uint*)@object + 25','(byte*)@object + 104']:assert fragment in body,fragment
    lines.append('const ModuleField slavedFields[] = {')
    for n,o,k,d in FIELDS:
        values='&slavedDamageType'if n=='DieOnMastersDeathDamageType'else '&slavedDeathType'if n=='DieOnMastersDeathType'else 'nullptr'
        lines.append('    { %s, %du, ModuleValue::%s, %s, %s },'%(json.dumps(n),o,k,'nullptr'if d is None else json.dumps(d),values))
    lines+=['};','const ModuleFieldSet slavedFieldSet = { slavedFields, %du };'%len(FIELDS)]
    body=body_of(source,'internal unsafe static void Tokenizer_002ETokenize(void** table, SlavedUpdateModuleData* @object, Tracker* state)')
    push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body);assert push
    ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);');assert len(parts)==int(push[2])==30
    lines.append('const ModuleToken slavedTokens[] = {')
    offsets=[]
    for part in parts[:-1]:
        key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff;typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
        off=re.search(r'\(byte\*\)@object \+ (\d+)',part);offset=int(off[1])if off else 0;mode=0;child='nullptr'
        if '(UpdateModuleData*)@object'in part:child='&lifecycleUpdateModuleData';assert key==1 and typ==4043318533
        elif 'Tokenizer_002ETokenize_003Cstruct_0020SageMathOnly'in part:
            mode=2;helper=body_of(source,'internal unsafe static void Tokenizer_002ETokenize_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(void** table, Vector3** pointer, Tracker* tracker)');assert 'BookmarkMemberOffset(tracker, (void*)(int)num, table)'in helper and 'if (num != 0)'in helper
        elif 'Tokenizer_002ETokenize_003C'in part:
            helpername=re.search(r'(Tokenizer_002ETokenize_003C\w+)\(',part)[1];scalar=re.search(r'\((int|uint|bool)\*\)@object \+ (\d+)',part);assert scalar,part
            offset=int(scalar[2])*(1 if scalar[1]=='bool'else 4)
            sig='internal unsafe static void '+helpername+'(void** table, '+scalar[1]+'* @object, Tracker* state)';helper=body_of(source,sig);assert 'BookmarkMemberOffset(state, @object, table)'in helper
        else:assert 'BookmarkMemberOffset(state,'in part
        offsets.append(offset);lines.append('    { %du, %du, %du, %s, %du, 0u },'%(key,typ,offset,child,mode))
    assert offsets==[0]+[o for n,o,k,d in FIELDS]+[104],offsets
    assert all(re.search(p,parts[-1])for p in [r'\*\(int\*\)'+ptr+r' = 0;',r'\(\(int\*\)'+ptr+r'\)\[1\] = 0;',r'\(\(int\*\)'+ptr+r'\)\[2\] = 0;'])
    lines+=['};','const ModuleLayout slavedLayout = { slavedTokens, 29u };','const ModuleChoice slavedChoice = { 3370197599u, &slavedLayout, 112u, 59u };',''];return '\n'.join(lines)
if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig');raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    pe=(ROOT/'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').read_bytes();enums={g:enum_values(source,pe,g)for g in ['DamageType','DeathType']}
    output=generate(source,enums);path=ROOT/'recovery/src/Native/GameObjectSlaved.h'
    if '--check'in sys.argv:assert path.read_text()==output,'Regenerate GameObjectSlaved.h'
    else:path.write_text(output)
    print('PASS complete SlavedUpdate 27 fields, nullable Vector3, PE enums and 29 Token records')
