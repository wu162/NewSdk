"""Export fingerprint-checked original selector methods for bounded source review.
This is source evidence only; no runtime support or DLL validation is claimed.
"""
import hashlib
import json
from pathlib import Path
import ExtractSoundSelectorEvidence as source

output=source.ROOT/'recovery/evidence/gameobject-modules/sound-selector-methods'
output.mkdir(exist_ok=True)
manifest={'sourceBlob':source.SOURCE_BLOB,'files':[]}
def contracts(value):
    if isinstance(value,dict):
        if 'signature' in value:
            yield value
        for child in value.values():
            yield from contracts(child)
    elif isinstance(value,list):
        for child in value:
            yield from contracts(child)
for row in source.report['modules']+source.report['nested']:
    chunks=[]
    seen=set()
    for item in contracts(row):
        signature=item['signature']
        if signature in seen: continue
        seen.add(signature)
        body,meta=source.exact(signature)
        assert meta['bodySha256']==item['bodySha256']
        literals={key:value for key,value in source.literals.items() if key in body}
        chunks.append('// Original line '+str(meta['line'])+'; SHA256 '+meta['bodySha256']+'\n'
                      +'// Literals: '+json.dumps(literals,ensure_ascii=False)+'\n'+body)
    text='// Source blob '+source.SOURCE_BLOB+'; evidence only, not compilable C#\n'+'\n\n'.join(chunks)+'\n'
    name=row['name']+'.txt'
    (output/name).write_text(text,encoding='utf-8')
    manifest['files'].append({'name':name,'methods':len(seen),'sha256':hashlib.sha256(text.encode()).hexdigest()})
(output/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
print('PASS exported',len(manifest['files']),'fingerprint-checked selector source files')
