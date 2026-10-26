import argparse,base64,hashlib,json,struct,zipfile
from pathlib import Path

def f32(x): return struct.unpack('<f',struct.pack('<f',x))[0]
def be32(b,p): return struct.unpack_from('>I',b,p)[0]
def read_json(p): return json.loads(p.read_text(encoding='utf-8-sig'))
p=argparse.ArgumentParser();p.add_argument('evidence');p.add_argument('output');args=p.parse_args()
root=Path(args.evidence); receipt=read_json(root/'validation.json')
assert receipt['sourceCommit']=='0d744a716df7b487816dc0fadf4f12dc82c5b66c'
assert str(receipt['runId'])=='40251368891' and receipt['status']=='completed-original-only'
assert receipt['originalOnly'] and not receipt['candidateValidated'] and not receipt['sdkValidated']
assert receipt['caseCount']==192 and len(receipt['cases'])==192
assert receipt['originalSha256']=='a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'
assert len({c['label'] for c in receipt['cases']})==192
inputs=read_json(root/'inputs.json');assert len(inputs)==192
input_hashes={}
for item in inputs:
    file=root/item['file'];assert file.stat().st_size==item['bytes']
    assert hashlib.sha256(file.read_bytes()).hexdigest()==item['sha256']
    assert item['case'] not in input_hashes
    input_hashes[item['case']]=item['sha256']

records=none_cases=samples=0; mappings={}; groups={}; errors=[]
for case in receipt['cases']:
    attempts=case['attempts']; assert len(attempts)==2
    raw=[]
    for attempt in attempts:
        record=root/(case['label']+'-'+str(attempt['iteration'])+'.json')
        assert record.exists(); data=record.read_bytes(); raw.append(data)
        assert hashlib.sha256(data).hexdigest()==attempt['recordSha256']
        r=json.loads(data.decode('utf-8-sig'));records+=1
        assert r['inputSha256']==input_hashes[case['label']]
        if not case['allComplete'] or attempt['timedOut'] or attempt['exitCode']!=0 or r['status']!='BUFFER':
            errors.append({'case':case['label'],'iteration':attempt['iteration'],'outcome':r.get('status'),'error':r.get('error')}); continue
        for field in ['instance','relocation','imports','custom']: assert field in r
        assert r['channels']==case['channels'] and r['frames']==129 and r['outputFrames']==129 and r['outputChannels']==case['channels'] and r['outputSampleRate']==22050
        if case['codec']!='NONE': continue
        channels=case['channels']; custom=base64.b64decode(r['custom']); instance=base64.b64decode(r['instance'])
        pointer,size=struct.unpack_from('<II',instance,20)
        header=instance[pointer:pointer+size] if case['streamed'] else custom[:8]
        assert len(header)==8 and ((be32(header,0)>>24)&15)==2
        assert ((be32(header,0)>>18)&63)+1==channels and (be32(header,0)&0x3ffff)==22050
        assert (be32(header,4)&0x1fffffff)==129
        block=0 if case['streamed'] else 8
        assert be32(custom,block+4)==129
        payload=custom[block+8:block+8+129*channels*2]
        actual=struct.unpack('>'+str(129*channels)+'h',payload)
        expected=[]
        for frame in range(129):
            for channel in range(channels):
                original=(channel+1)*2048 if case['signal']=='constant' else (12000+channel*512 if frame==channel*11+1 else 0)
                if case['bits']==8: original=(original>>8)<<8
                expected.append(int(f32(f32(original/32768.0)*32767.0)))
        if list(actual)!=expected:
            mismatch=next(i for i,(a,b) in enumerate(zip(actual,expected)) if a!=b)
            raise AssertionError('Source order mismatch '+case['label']+' at '+str(mismatch)+': '+str(actual[mismatch])+' != '+str(expected[mismatch]))
        samples+=len(expected)
    assert raw[0]==raw[1] and case['repeatedBytesIdentical']
    if case['codec']=='NONE':
        none_cases+=1; key=case['container']+'-'+str(case['channels']); mappings[key]=list(range(case['channels']));groups[key]=groups.get(key,0)+1
assert records==384 and none_cases==96
result={'scope':'Original-only AIFF/AIFC NONE ordering evidence; not candidate or SDK acceptance','sourceCommit':receipt['sourceCommit'],'runId':str(receipt['runId']),'attempt':str(receipt['runAttempt']),'originalSha256':receipt['originalSha256'],'caseCount':192,'processRecords':records,'completeCases':receipt['completeCases'],'stableCases':receipt.get('stableCases'),'noneCases':none_cases,'noneRecordsDecoded':none_cases*2,'expectedPcmQuantization':'truncate(binary32((signed16/32768)*32767)); PCM8 imports signed8<<8; PCM24 low byte discarded','noneExactSamplesCompared':samples,'outputToInputChannelMaps':mappings,'noneCasesPerContainerAndChannels':groups,'errors':errors,'candidateValidated':False,'sdkValidated':False,'receiptCommit':'5f2574fc7f60b5b63db4100f7b86e9e832efe15d','receipt':'recovery/evidence/audio/multichannel-aiff-original/40251368891-1/validation.json','receiptSha256':hashlib.sha256((root/'validation.json').read_bytes()).hexdigest(),'artifactId':12620461433,'artifactSha256':'50342ada48b370cbe19a2a30918935e8c5fc119eed44b32d10273f22398ab69b','inputFilesVerified':len(inputs),'auditScriptSha256':hashlib.sha256(Path(__file__).read_bytes()).hexdigest()}
assert not errors and receipt['completeCases']==192
Path(args.output).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
