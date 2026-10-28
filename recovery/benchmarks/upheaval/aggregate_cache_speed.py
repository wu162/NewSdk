#!/usr/bin/env python3
"""Validate independent measuring VMs and summarize paired three-way comparisons."""
import argparse
import json
from pathlib import Path
import random
import statistics

import benchmark as B
from cache_speed import VARIANTS, SCENARIOS


def validate(states, jobs):
    if len(states) != 2 or {s['replica'] for s in states} != {1,2}:
        raise ValueError('Expected both independent measuring replicas')
    verified = []
    for state in states:
        if not state['succeeded'] or not all(state['inputsUnchanged'].values()):
            raise ValueError('Failed input or output gate')
        if any(not row['succeeded'] or not row['byteIdentical'] for row in state['samples']):
            raise ValueError('Unaccepted build in measuring VM')
        match = [j for j in jobs['jobs'] if j['name'] == f'cache-speed ({state["replica"]})']
        if len(match) != 1:
            raise ValueError('Missing unique measuring job')
        job = match[0]
        if (not job.get('runner_id') or job['runner_name'] != state['runner']
                or str(job['run_id']) != state['runId'] or job['conclusion'] != 'success'):
            raise ValueError('Measuring runner identity or job acceptance differs')
        verified.append({k:job[k] for k in ('id','name','run_id','runner_id','runner_name','conclusion','started_at','completed_at')})
        for scenario in SCENARIOS:
            rows = state['triplets'][scenario]
            if len(rows) != state['methodology']['runsPerVM'] or [r['index'] for r in rows] != list(range(1,len(rows)+1)):
                raise ValueError('Missing paired measurement block')
            for row in rows:
                if set(row['order']) != set(VARIANTS):
                    raise ValueError('Incomplete variant order')
                for variant in VARIANTS:
                    if row[variant]['variant'] != variant or not row[variant]['byteIdentical']:
                        raise ValueError('Wrong engine pairing')
    if len({j['runner_id'] for j in verified}) != 2:
        raise ValueError('Measuring jobs reused the same VM')
    if len({s['sourceCommit'] for s in states}) != 1 or len({s['modGitTree'] for s in states}) != 1:
        raise ValueError('Measuring source/workload differs')
    if states[0]['runtimeProvenance'] != states[1]['runtimeProvenance']:
        raise ValueError('Measuring binary provenance differs')
    return verified


def ratio(groups, left, right):
    rng = random.Random(162)
    samples = []
    flattened = [row for group in groups for row in group]
    for _ in range(10000):
        resampled = [row for group in groups for row in rng.choices(group, k=len(group))]
        samples.append(statistics.mean(r[left]['wallSeconds'] for r in resampled) /
                       statistics.mean(r[right]['wallSeconds'] for r in resampled))
    samples.sort()
    return dict(ratio=statistics.mean(r[left]['wallSeconds'] for r in flattened) /
                      statistics.mean(r[right]['wallSeconds'] for r in flattened),
                ratio95CI=[samples[249],samples[9749]],
                method='Ratio of arithmetic means; VM-stratified paired percentile bootstrap, 10000 resamples, seed 162')


def aggregate(states, jobs):
    verified = validate(states, jobs)
    summaries = {}
    for scenario in SCENARIOS:
        groups = [s['triplets'][scenario] for s in states]
        flattened = [row for group in groups for row in group]
        summaries[scenario] = dict(n=len(flattened), engines={v:dict(
            wallSeconds=B.describe([r[v]['wallSeconds'] for r in flattened]),
            cpuSeconds=B.describe([r[v]['cpuSeconds'] for r in flattened]),
            peakWorkingSetBytes=B.describe([r[v]['peakWorkingSetBytes'] for r in flattened])) for v in VARIANTS},
            comparisons={left+'/'+right:ratio(groups,left,right) for left,right in
                         (('sdk','after'),('before','after'),('sdk','before'))},
            perVM=[dict(replica=s['replica'],means={v:statistics.mean(r[v]['wallSeconds'] for r in group) for v in VARIANTS}) for s,group in zip(states,groups)])
    return dict(schemaVersion=1, succeeded=True, sourceCommit=states[0]['sourceCommit'],
                runId=states[0]['runId'], verifiedRunnerJobs=verified,
                runtimeProvenance=states[0]['runtimeProvenance'],
                modSourceCommit=states[0]['modSourceCommit'],modGitTree=states[0]['modGitTree'],
                methodology=states[0]['methodology'], statistics=summaries, replicas=states)


def markdown(result):
    lines=['# Upheaval cache overhead optimization benchmark','',
           '| Scenario | n | SDK mean s | Before mean s | After mean s | Before/after (95% CI) | SDK/after (95% CI) |',
           '| --- | ---: | ---: | ---: | ---: | --- | --- |']
    for name, s in result['statistics'].items():
        ratios=[]
        for key in ('before/after','sdk/after'):
            c=s['comparisons'][key];ratios.append(f'{c["ratio"]:.3f} [{c["ratio95CI"][0]:.3f}, {c["ratio95CI"][1]:.3f}]')
        values=[s['engines'][v]['wallSeconds']['mean'] for v in VARIANTS]
        lines.append(f'| {name} | {s["n"]} | {values[0]:.3f} | {values[1]:.3f} | {values[2]:.3f} | {ratios[0]} | {ratios[1]} |')
    lines+=['','Two separate Windows VMs; fresh processes, warm OS page cache; one warm-up block per VM excluded.',
            'All full linked streams and CDATA inventories must be exactly identical. No samples or outliers removed.',
            'BC cold/hot means native compiler asset cache state; no claim of cold filesystem caches.',
            'CPU sums sequential SDK processes; memory is maximum single-process peak, not system/process-tree memory.',
            'Prior and candidate binaries are separately accepted and SHA256 verified; no cross-host subtraction.','']
    return '\n'.join(lines)


def main():
    p=argparse.ArgumentParser()
    p.add_argument('--input',required=True)
    p.add_argument('--jobs-metadata',required=True)
    p.add_argument('--output',required=True)
    args=p.parse_args()
    states=[json.loads(f.read_text()) for f in sorted(Path(args.input).rglob('state.json'))]
    result=aggregate(states,json.loads(Path(args.jobs_metadata).read_text()))
    out=Path(args.output);out.mkdir(parents=True,exist_ok=False)
    B.write(out/'result.json',result)
    report=markdown(result);(out/'summary.md').write_text(report)
    print(report)

if __name__=='__main__':
    main()
