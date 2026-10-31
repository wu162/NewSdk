#!/usr/bin/env python3
"""Reuse one built production artifact and overlay only explicit validation fixes."""
import argparse, hashlib, json, os, shutil, subprocess
from pathlib import Path

PRODUCTION = ('SDK','recovery/src','recovery/build','recovery/probes','ui',
              'recovery/scripts/Build*','recovery/scripts/PrepareLegacyTexture.ps1','recovery/scripts/RestoreUiDependencies.ps1','recovery/tests/CompilerValidationEntry.cpp')
FIXED_SCRIPTS = {'TestCompilerCoreLifecycle.ps1','TestCompilerPerformance.py','TestCompilerBackendBaseline.ps1'}
REUSED = {
 'core-lifecycle-deployment':'validation.json', 'ui-integration':'summary.json', 'backend-parallel':'validation.json',
 'core-lifecycle-texture':'validation.json', 'core-cli':'validation.json', 'build-cache':'validation.json',
 'input-roots':'validation.json', 'cache-payloads':'validation.json', 'session-cache':'validation.json',
 'big':'validation.json','hot-cache':'validation.json'}

def need(value,message):
 if not value:raise ValueError(message)
def git(repo,*args):return subprocess.check_output(['git','-C',str(repo),*args]).decode().strip()
def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()
def read(path):return json.loads(path.read_text(encoding='utf-8-sig'))
def passed(x):return x.get('succeeded') is True or x.get('status')=='passed'
def main():
 p=argparse.ArgumentParser(description=__doc__)
 for key in ('repo','original-root','output'):p.add_argument('--'+key,type=Path,required=True)
 for key in ('production-source','validation-source','original-run'):p.add_argument('--'+key,required=True)
 p.add_argument('--previous-repair-root',type=Path)
 p.add_argument('--auxiliary-root',type=Path)
 a=p.parse_args();repo=a.repo.resolve();original=a.original_root.resolve();out=a.output.resolve()
 need(not out.exists(),'Repair preparation already exists');out.mkdir(parents=True)
 source,test=a.production_source,a.validation_source
 need(git(repo,'rev-parse','HEAD')==test,'Checkout is not the requested validation source')
 need(git(repo,'rev-parse',source+'^{commit}')==source and len(test)==40,'Exact production/test SHAs are required')
 subprocess.run(['git','-C',str(repo),'merge-base','--is-ancestor',source,test],check=True)
 need(not git(repo,'diff','--name-only',source,test,'--',*PRODUCTION),'Validation source changes the production input closure')
 run=read(original/'original-run.json');jobs=read(original/'original-jobs.json')['jobs'];artifact=read(original/'original-artifact.json')
 need(str(run['id'])==a.original_run and run['head_sha']==source and run['status']=='completed' and run['path']=='.github/workflows/refactor-core-lifecycle.yml','Original run/source/workflow differs')
 steps=[s for job in jobs for s in job['steps']]
 allowed={'Targeted real Core, backend and application pipeline in Debug and Release','Existing bounded performance levels and original byte gates with current Release'}
 need(all(s['name'] in allowed for s in steps if s.get('conclusion')=='failure'),'Original run has failures outside the explicitly repaired scopes')
 need(any(s['name']=='Build full production combination and verify deployment and route metadata' and s.get('conclusion')=='success' for s in steps),'Original production build did not pass')
 attempt=str(run['run_attempt']);tag=a.original_run+'-'+attempt
 need(artifact['name']=='refactor-core-lifecycle-'+tag and not artifact['expired'],'Original artifact identity differs')
 runtime={};builds={}
 for mode in ('Debug','Release'):
  directory=original/'artifacts/core-lifecycle-product'/mode
  build=read(directory/'build.json');host=read(directory/'validation/build.json')
  need(build['sourceCommit']==source and build['configuration']==mode and build['backendIntegrated'],'Original production receipt differs')
  files={f.name:sha(f) for f in (directory/'bin').iterdir() if f.is_file()}
  expected={'NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll','BinaryAssetBuilder.TextureCompiler.dll','BinaryAssetBuilder.TextureCompiler.dll.patch.json'}
  need(set(files)==expected==set(build['runtimeFiles']) and files['NewModCompiler.exe']==build['exeSha256'],'Original runtime differs from its build receipt')
  need(host['sourceCommit']==source and host['sameProductionObjects'] and host['productExeSha256']==build['exeSha256'] and sha(directory/'validation/bin/NewModCompiler.exe')==host['exeSha256'],'Original validation host differs')
  runtime[mode]=files;builds[mode]=build
 overlays={}
 for name in git(repo,'diff','--name-only',source,test,'--','recovery/tests','recovery/scripts').splitlines():
  if name.startswith('recovery/tests/') and name.endswith('.cs') or name.startswith('recovery/scripts/') and Path(name).name in FIXED_SCRIPTS:
   overlays[name]=subprocess.check_output(['git','-C',str(repo),'show',test+':'+name])
  elif Path(name).name not in ('PrepareCompilerHotEditRepair.py','CompleteCompilerHotEditRepair.py'):
   raise ValueError('Unexpected validation-side change: '+name)
 core=read(original/'evidence/refactor/core-lifecycle'/tag/'validation.json')
 need(core['sourceCommit']==source and str(core['runId'])==a.original_run and len(core['modes'])==2,'Original Core receipt identity differs')
 selected=[];reused=[]
 for item in core['fixtures']:
  path=item['path'];content=overlays.get(path,(repo/path).read_bytes());same=hashlib.sha256(content).hexdigest()==item['sha256']
  rows=[row for mode in core['modes'] for row in mode['fixtures'] if row['name']==item['name']]
  (reused if same and len(rows)==2 and all(row['passed'] for row in rows) else selected).append(item['name'])
 prior=None;prior_fixtures={}
 if a.previous_repair_root:
  previous=a.previous_repair_root.resolve();prun=read(previous/'previous-run.json');partifact=read(previous/'previous-artifact.json')
  need(prun['status']=='completed' and prun['path']=='.github/workflows/compiler-hot-edit-validation.yml','Previous repair run identity differs')
  need(not git(repo,'diff','--name-only',source,prun['head_sha'],'--',*PRODUCTION),'Previous repair changed production inputs')
  ptag=str(prun['id'])+'-'+str(prun['run_attempt'])
  pprep=read(previous/'evidence/refactor/hot-edit-repair'/ptag/'preparation.json')
  pcore=read(previous/'evidence/refactor/core-lifecycle'/ptag/'validation.json')
  need(pprep['binarySourceCommit']==source and pprep['originalRunId']==a.original_run and pprep['runtimeFiles']==runtime,'Previous repair used a different production artifact')
  need(pcore['binarySourceCommit']==source and pcore['testSourceCommit']==prun['head_sha'],'Previous Core fixture sources differ')
  need(partifact['workflow_run']['head_sha']==prun['head_sha'] and partifact['name']=='compiler-hot-edit-validation-'+ptag,'Previous repair artifact differs')
  for name in list(selected):
   item=next(row for row in core['fixtures'] if row['name']==name);current=overlays.get(item['path'],(repo/item['path']).read_bytes())
   pitem=next((row for row in pcore['fixtures'] if row['name']==name),None)
   rows=[row for mode in pcore['modes'] for row in mode['fixtures'] if row['name']==name]
   if pitem and hashlib.sha256(current).hexdigest()==pitem['sha256'] and len(rows)==2 and all(row['passed'] for row in rows):
    results={}
    for mode in ('Debug','Release'):
     path=previous/'evidence/refactor/core-lifecycle'/ptag/mode/name/'result.json';value=read(path)
     need(value['succeeded'] and value['binarySourceCommit']==source and value['testSourceCommit']==prun['head_sha'],'Previous successful fixture identity differs')
     results[mode]=dict(path=str(path),member=path.relative_to(previous).as_posix(),sha256=sha(path))
    selected.remove(name);prior_fixtures[name]=dict(sourceSha256=pitem['sha256'],results=results)
  prior=dict(runId=str(prun['id']),runAttempt=str(prun['run_attempt']),conclusion=prun['conclusion'],testSourceCommit=prun['head_sha'],artifact=partifact)
 need(selected,'No failed or changed fixture requires repair')
 receipts={}
 for group,filename in REUSED.items():
  path=original/'evidence/refactor'/group/tag/filename;x=read(path)
  need(passed(x) and x.get('binarySourceCommit',x.get('sourceCommit'))==source,'Original reused scope did not pass current production: '+group)
  receipts[group]=dict(path=path.relative_to(original).as_posix(),sha256=sha(path),sourceCommit=source)
 auxiliary=None;auxiliary_receipts={}
 if a.auxiliary_root:
  aux=a.auxiliary_root.resolve();arun=read(aux/'auxiliary-run.json');aart=read(aux/'auxiliary-artifact.json')
  need(arun['status']=='completed' and arun['path']=='.github/workflows/compiler-hot-edit-validation.yml','Auxiliary repair run identity differs')
  need(not git(repo,'diff','--name-only',source,arun['head_sha'],'--',*PRODUCTION),'Auxiliary repair changed production inputs')
  need(not git(repo,'diff','--name-only',arun['head_sha'],test,'--','recovery/scripts/TestCompilerPerformance.py','recovery/scripts/TestCompilerBackendBaseline.ps1','recovery/scripts/ValidateAppProtocol.py','recovery/protocol','recovery/tests/CompilerBackendLifecycle.cs'),'Successful auxiliary gate inputs changed')
  atag=str(arun['id'])+'-'+str(arun['run_attempt']);aprep=read(aux/'evidence/refactor/hot-edit-repair'/atag/'preparation.json')
  need(aprep['binarySourceCommit']==source and aprep['originalArtifact']['id']==artifact['id'] and aprep['runtimeFiles']==runtime,'Auxiliary gates used a different original production artifact')
  need(aart['workflow_run']['head_sha']==arun['head_sha'],'Auxiliary artifact source differs')
  members={
   'performance':('evidence/refactor/performance/'+atag+'/validation.json','recovery/evidence/refactor/performance/'+out.name+'/validation.json'),
   'original-backend-Debug':('evidence/refactor/core-backend-baseline/'+atag+'/Debug/validation.json','recovery/evidence/refactor/core-backend-baseline/'+out.name+'/Debug/validation.json'),
   'original-backend-Release':('evidence/refactor/core-backend-baseline/'+atag+'/Release/validation.json','recovery/evidence/refactor/core-backend-baseline/'+out.name+'/Release/validation.json'),
   'original-core-protocol':('evidence/refactor/hot-edit-repair/'+atag+'/original-core-protocol.json','recovery/evidence/refactor/hot-edit-repair/'+out.name+'/original-core-protocol.json')}
  for label,(member,target) in members.items():
   path=aux/member;x=read(path);need(passed(x) and x.get('binarySourceCommit',x.get('sourceCommit'))==source,'Independent auxiliary gate did not pass: '+label)
   destination=repo/target;destination.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(path,destination)
   auxiliary_receipts[label]=dict(path=target,originalMember=member,sha256=sha(path))
  auxiliary=dict(runId=str(arun['id']),runAttempt=str(arun['run_attempt']),conclusion=arun['conclusion'],testSourceCommit=arun['head_sha'],artifact=aart)
 # Save the exact completion helper before detaching the checkout at production.
 tools=repo/'work/hot-edit-repair-tools';tools.mkdir(parents=True,exist_ok=True)
 helper=repo/'recovery/scripts/CompleteCompilerHotEditRepair.py';shutil.copy2(helper,tools/helper.name)
 subprocess.run(['git','-C',str(repo),'switch','--detach',source],check=True)
 for name,content in overlays.items():target=repo/name;target.parent.mkdir(parents=True,exist_ok=True);target.write_bytes(content)
 product=repo/'recovery/artifacts/core-lifecycle-product';need(not product.exists(),'Product destination already exists')
 shutil.copytree(original/'artifacts/core-lifecycle-product',product)
 # Transport original backend oracle inputs without rerunning their successful fixture.
 adapters={}
 for mode in ('Debug','Release'):
  fixture=original/'evidence/refactor/core-lifecycle'/tag/mode/'CompilerBackendLifecycle/result.json';x=read(fixture)
  need(x['succeeded'] and x['sourceCommit']==source and str(x['runId'])==a.original_run,'Backend oracle fixture was not successful')
  old=x['fixtureWorkRoot'];new=str(fixture.parent/'work')
  def transport(value):
   if isinstance(value,str) and (value==old or value.startswith(old+'\\') or value.startswith(old+'/')):return new+value[len(old):]
   if isinstance(value,list):return [transport(v) for v in value]
   if isinstance(value,dict):return {k:transport(v) for k,v in value.items()}
   return value
  transported=transport(x);transported['originalResultSha256']=sha(fixture);transported['transportedFromRunId']=a.original_run
  target=out/(mode+'-backend-transport.json');target.write_text(json.dumps(transported,indent=2)+'\n')
  adapters[mode]=dict(path=str(target),originalPath=str(fixture),originalSha256=sha(fixture))
 state=dict(schemaVersion=1,status='prepared',binarySourceCommit=source,testSourceCommit=test,originalRunId=a.original_run,originalRunAttempt=attempt,
            originalRunConclusion=run['conclusion'],originalArtifact=artifact,productionClosureUnchanged=True,
            productionTrees={name:git(repo,'rev-parse',source+':'+name) for name in ('SDK','recovery/src','recovery/build','recovery/probes')},
            runtimeFiles=runtime,selectedFixtures=selected,reusedFixtures=reused,allFixtureNames=[row['name'] for row in core['fixtures']],
            previousRepair=prior,reusedSuccessfulRepairFixtures=prior_fixtures,reusedScopes=receipts,backendTransports=adapters,
            auxiliaryRepair=auxiliary,reusedAuxiliaryGates=auxiliary_receipts,
            overlays={name:hashlib.sha256(content).hexdigest() for name,content in overlays.items()},originalRoot=str(original),productDirectory=str(product),
            scope='Exact original production and validation hosts; only named validation fixes overlaid; original failed terminal status remains unchanged')
 (out/'preparation.json').write_text(json.dumps(state,indent=2)+'\n')
 if os.getenv('GITHUB_OUTPUT'):
  with open(os.environ['GITHUB_OUTPUT'],'a') as f:
   f.write('fixtures='+','.join(selected)+'\n')
   f.write('auxiliaryReused='+str(bool(auxiliary_receipts)).lower()+'\n')
 print(json.dumps({k:state[k] for k in ('binarySourceCommit','testSourceCommit','selectedFixtures','reusedFixtures')},indent=2))
if __name__=='__main__':main()
