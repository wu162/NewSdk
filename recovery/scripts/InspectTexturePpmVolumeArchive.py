from collections import Counter
from pathlib import Path
import hashlib,json,struct,sys,zipfile
sys.path.insert(0,str(Path('recovery/scripts').resolve()))
from InspectTextureDibArchive import Replay
run,source=sys.argv[1:3]
d=Path(sys.argv[3]) if len(sys.argv)>3 else Path('recovery/evidence/texture/ppm-volume-boundaries')/(run+'-1')
r=json.loads((d/'validation.json').read_text(encoding='utf-8-sig'))
assert r['sourceCommit']==source and str(r['runId'])==run and r['status']=='passed'
assert not r['probeOnly'] and not r['sdkValidated']
assert len(r['configurations'])==2 and {m['configuration'] for m in r['configurations']}=={'Debug','Release'}
assert all(m['passed'] and m['withoutOriginalTextureCompiler'] for m in r['configurations'])
a=d/'corpus.zip'
assert hashlib.sha256(a.read_bytes()).hexdigest()==r['corpusSha256']
counts,roots,formats,kinds=Counter(),Counter(),Counter(),Counter()
sources=set(); compared=0
with zipfile.ZipFile(a) as z:
 assert len(z.namelist())==2 and set(z.namelist())=={'Debug-pipeline.replay','Release-pipeline.replay'}
 with z.open('Debug-pipeline.replay') as debug,z.open('Release-pipeline.replay') as release:
  while True:
   b=debug.read(65536);assert b==release.read(65536)
   if not b:break
 with z.open('Debug-pipeline.replay') as stream:
  p=Replay(stream);assert p.integer()==11520
  for i in range(11520):
   ext,image,root,fmt=p.string(),p.blob(),p.string(),p.string()
   stale,mip,kind=p.boolean(),p.boolean(),p.string()
   error=p.string(); instance,relo,imports,cdata=[p.blob() for _ in range(4)]
   assert ext=='ppm' and image[:2] in (b'P3',b'P6')
   assert root in ('Texture','OnDemandTexture') and fmt in ('A8R8G8B8','X8R8G8B8','R5G6B5','DXT3','DXT5')
   roots[root]+=1;formats[fmt]+=1;kinds[kind]+=1;sources.add(hashlib.sha256(image).hexdigest())
   if error:
    assert kind=='VolumeTexture' and error.startswith('BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/')
    assert instance is relo is imports is None and (cdata is not None)==stale
    counts['originalVolumeError']+=1
   else:
    assert kind=='StandardTexture' and instance is not None and relo is not None and imports==b''
    if root=='Texture':
     assert struct.unpack_from('<I',instance,4)[0]==12 and instance[12:16]==b'DDS ' and (cdata is not None)==stale
    else:assert len(instance)==8 and cdata is not None and cdata[40:44]==b'DDS '
    counts['successfulStandard']+=1
   compared+=sum(len(b) for b in (instance,relo,imports,cdata) if b is not None)
  assert stream.read(1)==b''
assert counts=={'originalVolumeError':5760,'successfulStandard':5760}
assert roots=={'Texture':5760,'OnDemandTexture':5760}
assert set(formats.values())=={2304} and kinds=={'StandardTexture':5760,'VolumeTexture':5760}
result=dict(scope='independent saved complete PPM tall-volume original corpus audit; no codec execution',sourceCommit=source,runId=run,corpusSha256=r['corpusSha256'],fixturesPerConfiguration=11520,originalConfigurationsByteIdentical=True,outcomes=dict(counts),roots=dict(roots),formats=dict(formats),uniqueSourceHashes=len(sources),completeExpectedBufferBytesPerConfiguration=compared)
out=Path('recovery/evidence/texture/handoffs/20261029-ppm/volume-boundary-archive-audit.json')
out.write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result))
