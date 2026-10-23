"""Derive selector Token names/types from exact fingerprint-checked original methods."""
import hashlib
import re
import sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
BLOB='6e40a9ed6c9f961d294d49768d911e6a6f354619'

def generate(source):
    # offset, nested layout, binding mode, list stride. Modes share ModuleToken.
    def member(offset):return (offset,None,0,0)
    def nested(name,offset=0,mode=0,stride=0):return (offset,name,mode,stride)
    specs={
      'ModuleData':[member(0),member(4)],
      'ClientBehaviorModuleData':[nested('ModuleData')],
      'SoundOrEvaEvent':[(0,None,2,0),member(4)],
      'AudioVoiceEntry':[nested('SoundOrEvaEvent'),member(8)],
      'AudioObjectSpecificVoiceEntry':[nested('SoundOrEvaEvent'),member(8),member(12)],
      'AudioVoiceReferentialEntry':[nested('SoundOrEvaEvent'),member(8)],
      'AudioSoundEntry':[member(0),member(4)],
      'EvaEventEntry':[member(0),member(4)],
      'AudioArrayVoice':[nested('AudioVoiceEntry',0,1,12),nested('AudioObjectSpecificVoiceEntry',8,1,16),nested('AudioVoiceReferentialEntry',16,1,12)],
      'AudioArraySound':[nested('AudioSoundEntry',0,1,8)],
      'EvaEventArray':[nested('EvaEventEntry',0,1,8)],
      'RandomSoundSelectorClientBehaviorModuleData':[nested('ClientBehaviorModuleData'),member(8),(12,None,2,0),member(24),nested('AudioArrayVoice',16,4),nested('AudioArraySound',20,4)],
      'ModelConditionSoundSelectorOverrideBlock':[member(0),member(60),(120,None,2,0),nested('AudioArrayVoice',124,4),nested('AudioArraySound',128,4)],
      'UpgradeSoundSelectorOverrideBlock':[member(0),member(60),(120,None,2,0),nested('AudioArrayVoice',124,4),nested('AudioArraySound',128,4),(132,None,3,4),(140,None,3,4)],
      'ModelConditionEvaEventSelectorOverrideBlock':[member(0),member(60),nested('EvaEventArray',120,4)],
      'ModelConditionSoundSelectorClientBehaviorModuleData':[nested('ClientBehaviorModuleData'),nested('ModelConditionSoundSelectorOverrideBlock',8,1,132)],
      'UpgradeSoundSelectorClientBehaviorModuleData':[nested('ClientBehaviorModuleData'),nested('UpgradeSoundSelectorOverrideBlock',8,1,148)],
      'ModelConditionEvaEventSelectorClientBehaviorModuleData':[nested('ClientBehaviorModuleData'),nested('ModelConditionEvaEventSelectorOverrideBlock',8,1,124)]}
    lines=['// Generated from original blob '+BLOB+'; do not edit.']
    for name,spec in specs.items():
        signature='internal unsafe static void Tokenizer_002ETokenize(void** table, '+name+'* @object, Tracker* state)'
        start=source.index(signature);opening=source.index('{',start);depth=1;end=opening+1
        while depth:
            depth+=(source[end]=='{')-(source[end]=='}');end+=1
        body=source[start:end]
        push=re.search(r'ProcTableElement\* (\w+) = \(ProcTableElement\*\)Relo_002ETracker_002EPush\([^;]+12u, (\d+)u\)',body)
        assert push and int(push[2])==len(spec)+1,name
        ptr=push[1];parts=body.split(ptr+' = (ProcTableElement*)((byte*)'+ptr+' + 12);')
        assert len(parts)==len(spec)+1,name
        lines.append('const ModuleToken selector'+name+'Tokens[] = {')
        for part,(off,child,mode,stride) in zip(parts,spec):
            key=int(re.search(r'\*\(int\*\)'+ptr+r' = (-?\d+);',part)[1])&0xffffffff
            typ=int(re.search(r'\(\(int\*\)'+ptr+r'\)\[1\] = (-?\d+);',part)[1])&0xffffffff
            if off:assert '(byte*)@object + '+str(off) in part,(name,off)
            if mode==4:assert '('+child+'*)(int)' in part,(name,child)
            if mode==0 and child:assert '('+child+'*)@object' in part,(name,child)
            lines.append('    { %du, %du, %du, %s, %du, %du },'%(key,typ,off,'&selector'+child if child else 'nullptr',mode,stride))
        assert re.search(r'\*\(int\*\)'+ptr+r' = 0;',parts[-1]),name
        lines+=['};','const ModuleLayout selector'+name+' = { selector'+name+'Tokens, '+str(len(spec))+'u };']
    lines.append('const ModuleChoice selectorChoices[] = {')
    for kind,(name,typeid,size) in enumerate([
        ('RandomSoundSelectorClientBehaviorModuleData',3030954484,28),
        ('ModelConditionSoundSelectorClientBehaviorModuleData',847051638,16),
        ('UpgradeSoundSelectorClientBehaviorModuleData',4178664105,16),
        ('ModelConditionEvaEventSelectorClientBehaviorModuleData',1640547431,16)],13):
        lines.append('    { %du, &selector%s, %du, %du },'%(typeid,name,size,kind))
    lines+=['};','']
    return '\n'.join(lines)

if __name__=='__main__':
    source=(ROOT/'recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs').read_text(encoding='utf-8-sig')
    raw=source.encode();assert hashlib.sha1(b'blob '+str(len(raw)).encode()+b'\0'+raw).hexdigest()==BLOB
    output=generate(source);path=ROOT/'recovery/src/Native/GameObjectSoundSelectors.h'
    if '--check' in sys.argv:assert path.read_text()==output,'Regenerate GameObjectSoundSelectors.h'
    else:path.write_text(output)
    print('PASS 18 original selector Token layouts and four dispatch choices')
