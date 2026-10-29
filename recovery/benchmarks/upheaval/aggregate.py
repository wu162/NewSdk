#!/usr/bin/env python3
"""Summarize matched experiments, stratifying resampling by independent runner."""
import argparse
import json
import os
from pathlib import Path
import random
import statistics
import benchmark as B


def summarize_strata(strata):
    if not strata or any(len(rows)<3 for rows in strata):
        raise ValueError('Each runner needs at least three accepted pairs')
    pairs=[pair for rows in strata for pair in rows]
    result={engine:B.describe([p[engine]['wallSeconds'] for p in pairs]) for engine in ('old','new')}
    rng=random.Random(162)
    values=[]
    for _ in range(10000):
        sampled=[]
        for rows in strata:
            sampled.extend(rng.choices(rows,k=len(rows)))
        values.append(statistics.mean(p['old']['wallSeconds'] for p in sampled)/
                      statistics.mean(p['new']['wallSeconds'] for p in sampled))
    values.sort()
    result.update(speedup=result['old']['mean']/result['new']['mean'],
                  speedup95CI=[values[249],values[9749]],runners=len(strata),
                  method='Paired percentile bootstrap, resampled within each runner, 10000 iterations, seed162; conditional on observed runner strata')
    return result


def verify_runners(states,jobs=None):
    # Hosted images can retain an identical Windows hostname. GitHub's assigned
    # runner names/IDs, not the image hostname, identify the execution instances.
    names=[state.get('runner') for state in states]
    if any(not name for name in names) or len(set(names))!=len(states):
        raise ValueError('Runner execution identities are not distinct')
    verified=[]
    if jobs is not None:
        for state in states:
            group=state.get('followupGroup')
            if group is not None and group not in ('xml','texture','audio','recovery'):
                raise ValueError('Unknown followup measurement scope')
            name=f'cache-followup ({group}, {state["replica"]})' if group else f'expanded ({state["replica"]})'
            matched=[job for job in jobs['jobs'] if job['name']==name]
            if len(matched)!=1:raise ValueError('Missing unique measuring job identity')
            job=matched[0]
            if (job.get('runner_name')!=state['runner'] or not job.get('runner_id')
                    or str(job['run_id'])!=str(state['runId'])):
                raise ValueError('Artifact runner identity differs from GitHub job metadata')
            verified.append({key:job[key] for key in ('id','name','run_id','runner_id','runner_name','started_at','completed_at','conclusion')})
        if len({job['runner_id'] for job in verified})!=len(states):
            raise ValueError('GitHub measuring runner IDs are not distinct')
    return verified


def profile_exclusion(name,states):
    if ('-changed' in name and any(not s.get('compilerEnvironmentSha256') for s in states)):
        return 'Original cross-step baseline inherited volatile Actions environment; use fixed-environment followup instead'
    if name=='big-end-to-end' and any('directly' not in s.get('methodology',{}).get('packagingLayout','') for s in states):
        return 'Original layout includes observer collection copy; use the direct-output packaging run instead'
    return None


def aggregate(root,output,expected=2,jobs_metadata=None):
    paths=sorted(Path(root).rglob('state.json'))
    states=[json.loads(path.read_text()) for path in paths]
    if len(states)!=expected or len({s['replica'] for s in states})!=expected:
        raise ValueError('Missing or duplicate independent runner summaries')
    jobs=json.loads(Path(jobs_metadata).read_text()) if jobs_metadata else None
    verified=verify_runners(states,jobs)
    if (len({s['sourceCommit'] for s in states})!=1 or len({s['binarySourceCommit'] for s in states})!=1
            or len({str(s['runId']) for s in states})!=1 or len({s.get('followupGroup') for s in states})!=1):
        raise ValueError('Runner source/runtime/run/scope identities differ')
    names=set.intersection(*(set(s['pairs']) for s in states))
    stats={};excluded={}
    for name in sorted(names):
        reason=profile_exclusion(name,states)
        if reason:
            excluded[name]=reason
            continue
        if all(len(s['pairs'][name])>=3 for s in states):
            if any(not p[e]['byteIdentical'] for s in states for p in s['pairs'][name] for e in ('old','new')):
                raise ValueError('Rejected output entered statistics')
            stats[name]=summarize_strata([s['pairs'][name] for s in states])
    effects={}
    for engine in ('old','new'):
        if all({'no-cache','bc-cold','bc-hot'}<=s['pairs'].keys() for s in states):
            for mode in ('bc-cold','bc-hot'):
                strata=[]
                for state in states:
                    plain={p['index']:p for p in state['pairs']['no-cache']}
                    cached={p['index']:p for p in state['pairs'][mode]}
                    if plain.keys()!=cached.keys(): raise ValueError('Cache/no-cache pairing differs')
                    strata.append([dict(old=plain[i][engine],new=cached[i][engine]) for i in sorted(plain)])
                effect=summarize_strata(strata)
                effects[engine+'/'+mode]=dict(noCache=effect['old'],cached=effect['new'],
                    noCacheToCachedRatio=effect['speedup'],ratio95CI=effect['speedup95CI'],method=effect['method'])
    result=dict(schemaVersion=1,succeeded=all(s['succeeded'] for s in states),
                runId=os.getenv('GITHUB_RUN_ID'),sourceCommit=states[0]['sourceCommit'],
                binarySourceCommit=states[0]['binarySourceCommit'],statistics=stats,cacheEffects=effects,
                excludedProfiles=excluded,
                recoveryCaveat=('New-side original recovery observations lack intended-namespace proof' if any(
                    x.get('engine')=='new' and x.get('kind','').startswith('cache-') and not x.get('intendedNamespaceVerified')
                    for state in states for x in state['observations']) else None),
                verifiedRunnerJobs=verified,replicas=states,scope=('Two independent hosted Windows runner VMs for '+str(states[0].get('followupGroup') or 'expanded')+'; full historical Upheaval except explicitly labelled metadata-only SC subset; no physical-hardware diversity claim'))
    output=Path(output);output.mkdir(parents=True,exist_ok=False)
    B.write(output/'expanded-result.json',result)
    summary='# Upheaval cache and packaging comparison\n\n'
    summary+='Two independent Windows VMs; confidence intervals resample paired observations within each VM.\n\n'
    summary+='| Scenario | Pairs | Old s | New s | Old/new | 95% CI |\n| --- | ---: | ---: | ---: | ---: | --- |\n'
    for name,row in stats.items():
        summary+=f'| {name} | {row["old"]["n"]} | {row["old"]["mean"]:.3f} | {row["new"]["mean"]:.3f} | {row["speedup"]:.3f} | {row["speedup95CI"][0]:.3f}–{row["speedup95CI"][1]:.3f} |\n'
    summary+='\n| Engine/profile | Cache-off s | Cache-on s | Off/on ratio | 95% CI |\n| --- | ---: | ---: | ---: | --- |\n'
    for name,row in effects.items():
        summary+=f'| {name} | {row["noCache"]["mean"]:.3f} | {row["cached"]["mean"]:.3f} | {row["noCacheToCachedRatio"]:.3f} | {row["ratio95CI"][0]:.3f}–{row["ratio95CI"][1]:.3f} |\n'
    summary+='\nFull-MOD new SC rejection and cache recovery/failure observations are recorded separately, never as speedups.\n'
    if excluded: summary+='\nExcluded original profiles: '+json.dumps(excluded)+'\n'
    if result['recoveryCaveat']:summary+='\nRecovery caveat: '+result['recoveryCaveat']+'\n'
    summary+='\nAcceptance: '+str(result['succeeded'])+'; consult per-runner phase errors before accepting individual groups.\n'
    (output/'summary.md').write_text(summary,encoding='utf-8')
    print(summary)
    if os.getenv('GITHUB_STEP_SUMMARY'):
        with open(os.environ['GITHUB_STEP_SUMMARY'],'a',encoding='utf-8') as stream:stream.write(summary)
    if not result['succeeded']:raise ValueError('One or more independent runner gates failed; partial results preserved')


if __name__=='__main__':
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--input',required=True)
    parser.add_argument('--output',required=True)
    parser.add_argument('--expected',type=int,default=2)
    parser.add_argument('--jobs-metadata')
    args=parser.parse_args()
    aggregate(args.input,args.output,args.expected,args.jobs_metadata)
