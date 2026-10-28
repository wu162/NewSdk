"""Replay accepted production cache operations without modifying the compiler."""
import argparse,json,os,statistics,subprocess,sys
from pathlib import Path
import benchmark as B
import expanded as E

def event_times(path):
    starts={};streams={};stages={}
    for line in path.read_text(encoding='utf-8-sig').splitlines():
        row=json.loads(line);kind=row['eventType']
        if kind=='StreamStarted':starts[row['streamId']]=row['elapsedMs']
        if kind=='StreamCompleted':
            name=row['streamId'];streams[name]=(row['elapsedMs']-starts.pop(name))/1000
        if kind=='StageCompleted':stages[row['stageId']]=row['payload']['elapsedMs']/1000
    if starts or len(streams)!=5:raise ValueError('Incomplete full-stream event timing')
    return dict(streamSeconds=streams,stageSeconds=stages)

def main():
    output=Path('recovery/artifacts/upheaval-cost').resolve()
    args=argparse.Namespace(repo='.',output=str(output),runtime='recovery/artifacts/upheaval-runtime/bin',runs=3,changes=3,replica=1,timeout=1200,only_package=False,followup_group=None)
    E.initialize(args);x=E.Experiment(args)
    probe=output/'probe';probe.mkdir()
    exe=probe/'CacheCostProbe.exe'
    csc=Path(os.environ['WINDIR'])/'Microsoft.NET/Framework/v4.0.30319/csc.exe'
    subprocess.run([str(csc),'/nologo','/optimize+','/platform:x86','/target:exe','/out:'+str(exe),'/r:'+str(x.runtime/'NewModCompiler.exe'),'/r:System.Core.dll','/r:System.Web.Extensions.dll',str(Path(__file__).with_name('CacheCostProbe.cs'))],check=True)
    exe.with_suffix('.exe.config').write_bytes((x.runtime/'NewModCompiler.exe.config').read_bytes())
    subprocess.run([str(exe),str(x.runtime),'--check-runtime'],cwd=x.runtime,env=x.env,check=True)
    expected_hash=json.loads((Path(args.repo)/'recovery/docs/UPHEAVAL_BENCHMARK_RESULT.json').read_text())['outputGate']['inventorySha256']
    expected=None;rows=[]
    for label,mode,bc in [('profile-no-cache','profile-plain',False),('profile-bc-cold','bc',True),('profile-bc-hot','bc',True)]:
        if label!='profile-bc-hot':x.reset('new',mode)
        row,products=x.build('new',label,mode=mode,bc=bc)
        if E.fingerprint(products)!=expected_hash:raise ValueError('Profiling output differs from accepted full Upheaval')
        x.accept((row,products),products)
        if label=='profile-bc-hot':E.require_hot(row)
        row['eventTimings']=event_times(output/row['case']/'build.events.jsonl');rows.append(row)
    hot=rows[-1];before=B.snapshot(x.cache_root('new'))
    probe_result=output/'probe-cost.json'
    process=E.execute(exe,[str(x.runtime),str(output/hot['case']/'command.json'),str(probe_result)],x.runtime,output/'probe-process',1200,x.env)
    if not process['succeeded']:raise ValueError('Original-runtime cost replay failed')
    result=json.loads(probe_result.read_text(encoding='utf-8-sig'))
    if not result['succeeded'] or result['architecture']!=32 or B.snapshot(x.cache_root('new'))!=before:
        raise ValueError('Cost probe changed confirmed cache or runtime identity failed')
    x.finalize()
    grouped={}
    for row in result['measurements']:
        key=(row['kind'],row['repetition']);grouped[key]=grouped.get(key,0)+row['wallSeconds']
    statistics_by_kind={kind:dict(perRepetition=[grouped[kind,i] for i in range(3)],medianSeconds=statistics.median(grouped[kind,i] for i in range(3))) for kind in sorted({k[0] for k in grouped})}
    summary=dict(schemaVersion=1,succeeded=True,sourceCommit=x.state['sourceCommit'],binarySourceCommit=x.state['binarySourceCommit'],runId=os.getenv('GITHUB_RUN_ID'),environment=x.state['environment'],compilerEnvironmentSha256=x.c['compilerEnvironmentSha256'],
                 productionInputsUnchanged=x.state['inputsUnchanged'],confirmedCacheUnchanged=True,reuse=x.state['reuse'],probeExeSha256=B.digest(exe),probeRuntimeAssemblySha256=B.digest(x.runtime/'NewModCompiler.exe'),
                 inputFileCount=result['inputFilesPerInventory'],inputBytesExcludingRuntime=sum(v['bytes'] for f in ('sdk','mod') for v in json.loads((output/(f+'-inputs.json')).read_text()).values()),
                 builds=[{k:v for k,v in row.items() if k!='diagnostics'} for row in rows],isolatedCosts=statistics_by_kind,probeMetadata={k:v for k,v in result.items() if k!='measurements'},methodology=result['methodology'])
    B.write(output/'cost-summary.json',summary)
    text='# Upheaval cache-cost replay\n\nUnchanged accepted Release, one Windows VM; diagnostic timings, not replacement benchmark means.\n\n'
    text+='| Build | wall s | CPU s |\n| --- | ---: | ---: |\n'
    for row in rows:text+=f'| {row["label"]} | {row["wallSeconds"]:.6f} | {row["cpuSeconds"]:.6f} |\n'
    text+='\n| Exact method replay | median total s (5 streams) | 3 repetitions |\n| --- | ---: | --- |\n'
    for kind,s in statistics_by_kind.items():text+=f'| {kind} | {s["medianSeconds"]:.6f} | {s["perRepetition"]} |\n'
    (output/'cost-summary.md').write_text(text);print(text)
    if os.getenv('GITHUB_STEP_SUMMARY'):
        with open(os.environ['GITHUB_STEP_SUMMARY'],'a',encoding='utf-8') as f:f.write(text)
if __name__=='__main__':main()
