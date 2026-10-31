#!/usr/bin/env python3
"""Combine unchanged successful scopes with the explicitly repaired and missing gates."""
import argparse,hashlib,json,os,subprocess,zipfile
from pathlib import Path

def need(v,m):
 if not v:raise ValueError(m)
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--preparation',type=Path,required=True);p.add_argument('--repo',type=Path,required=True);p.add_argument('--output',type=Path,required=True);a=p.parse_args()
 prep=read(a.preparation);repo=a.repo.resolve();out=a.output.resolve();source,test=prep['binarySourceCommit'],prep['testSourceCommit'];run=os.environ['GITHUB_RUN_ID'];attempt=os.environ['GITHUB_RUN_ATTEMPT'];tag=run+'-'+attempt
 need(subprocess.check_output(['git','-C',str(repo),'rev-parse','HEAD'],text=True).strip()==source,'Production HEAD changed during validation')
 for path,expected in prep['overlays'].items():need(sha(repo/path)==expected,'Overlaid validation source changed: '+path)
 original=Path(prep['originalRoot']);product=Path(prep['productDirectory']);completed={}
 def receipt(label,path):
  x=read(path);need(x.get('succeeded') is True or x.get('status')=='passed','Required gate did not pass: '+label)
  if x.get('sourceCommit'):need(x['sourceCommit']==source,'Gate production source differs: '+label)
  completed[label]=dict(path=path.relative_to(repo).as_posix(),sha256=sha(path),sourceCommit=x.get('sourceCommit'));return x
 core=receipt('repaired-core',repo/'recovery/evidence/refactor/core-lifecycle'/tag/'validation.json')
 need(core['binarySourceCommit']==source and core['testSourceCommit']==test and core['fixtureSelection']=='selected','Core repair source/selection differs')
 need(set(row['name'] for row in core['fixtures'])==set(prep['selectedFixtures']),'Core repair omitted requested fixtures')
 for mode in core['modes']:
  need(mode['passed'] and all(row['passed'] for row in mode['fixtures']),'A selected fixture failed')
  for row in mode['fixtures']:
   result=read(repo/'recovery/evidence/refactor/core-lifecycle'/tag/mode['configuration']/row['name']/'result.json')
   need(result['binarySourceCommit']==source and result['testSourceCommit']==test,'Fixture result source differs')
 receipt('performance',repo/'recovery/evidence/refactor/performance'/tag/'validation.json')
 for mode in ('Debug','Release'):
  receipt('original-backend-'+mode,repo/'recovery/evidence/refactor/core-backend-baseline'/tag/mode/'validation.json')
 receipt('original-core-protocol',repo/'recovery/evidence/refactor/hot-edit-repair'/tag/'original-core-protocol.json')
 receipt('complete-mymod',repo/'recovery/evidence/refactor/sdk-mymod'/tag/'validation.json')
 release=receipt('five-file-packages',repo/'recovery/artifacts/final-release'/tag/'release.json')
 need(len(release['configurations'])==2 and release['bigDeployment']['succeeded'],'Final package acceptance is incomplete')
 measure=receipt('complete-Upheaval',repo/'recovery/evidence/refactor/backend-parallel-performance'/tag/'measurement.json')
 need(len(measure['samples'])==22 and all(row['status']=='passed' for row in measure['samples']),'Complete Upheaval matrix did not finish')
 covered=set(prep['selectedFixtures'])|set(prep['reusedFixtures'])|set(prep['reusedSuccessfulRepairFixtures'])
 need(covered==set(prep['allFixtureNames']),'Composed Core acceptance omitted fixtures')
 for label,row in prep['reusedAuxiliaryGates'].items():
  need(sha(repo/row['path'])==row['sha256'],'Reused auxiliary gate receipt changed: '+label)
 reused={}
 with zipfile.ZipFile(out/'reused-original-receipts.zip','w',zipfile.ZIP_DEFLATED) as z:
  for label,row in prep['reusedScopes'].items():
   path=original/row['path'];need(sha(path)==row['sha256'],'Original successful receipt changed: '+label)
   z.writestr(row['path'],path.read_bytes());reused[label]=row
  corepath='evidence/refactor/core-lifecycle/'+prep['originalRunId']+'-'+prep['originalRunAttempt']+'/validation.json'
  z.writestr(corepath,(original/corepath).read_bytes())
  for mode in ('Debug','Release'):
   for name in prep['reusedFixtures']:
    path=original/'evidence/refactor/core-lifecycle'/(prep['originalRunId']+'-'+prep['originalRunAttempt'])/mode/name/'result.json'
    if path.exists():z.writestr(path.relative_to(original).as_posix(),path.read_bytes())
  for name,fixture in prep['reusedSuccessfulRepairFixtures'].items():
   for mode,row in fixture['results'].items():
    path=Path(row['path']);need(sha(path)==row['sha256'],'Previous successful fixture receipt changed: '+name)
    z.writestr(row['member'],path.read_bytes())
 for mode in ('Debug','Release'):
  actual={p.name:sha(p) for p in (product/mode/'bin').iterdir() if p.is_file()}
  need(actual==prep['runtimeFiles'][mode],'Reused runtime changed during repair')
 state=dict(schemaVersion=1,status='passed',sourceCommit=source,binarySourceCommit=source,testSourceCommit=test,runId=run,runAttempt=attempt,
            originalRunId=prep['originalRunId'],originalRunAttempt=prep['originalRunAttempt'],originalRunConclusion=prep['originalRunConclusion'],
            originalArtifact=prep['originalArtifact'],productionClosureUnchanged=True,productionTrees=prep['productionTrees'],
            runtimeFiles=prep['runtimeFiles'],overlays=prep['overlays'],selectedFixtures=prep['selectedFixtures'],reusedFixtures=prep['reusedFixtures'],
            previousRepair=prep['previousRepair'],reusedSuccessfulRepairFixtures=prep['reusedSuccessfulRepairFixtures'],
            auxiliaryRepair=prep['auxiliaryRepair'],reusedAuxiliaryGates=prep['reusedAuxiliaryGates'],
            reusedScopes=reused,completedGates=completed,reusedReceiptArchiveSha256=sha(out/'reused-original-receipts.zip'),
            scope='Composed acceptance of the unchanged original production binary: successful original scopes reused, failed validation fixtures repaired, all missing original SDK/package/complete MOD gates completed; original failed run retained')
 (out/'repair.json').write_text(json.dumps(state,indent=2,ensure_ascii=False)+'\n');print(json.dumps({k:state[k] for k in ('status','binarySourceCommit','testSourceCommit','selectedFixtures','reusedFixtures')},indent=2))
if __name__=='__main__':main()
