#!/usr/bin/env python3
"""Bounded same-VM diagnostics for representative edits after warm caches."""
import argparse
import ctypes
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import subprocess
import time

from BenchmarkCompilerCache import inventory, process_metrics
from HotCacheWorkload import allocate_id, instance_hash
from MeasureCompilerCachePaired import (BASELINE_BINARY_SOURCE, BASELINE_FILES, BASELINE_HEAD,
                                        BASELINE_RUN, git, need, product)
from MeasureCompilerModScale import phases, write
from TestCompilerHotCache import declaration, published, same_stamp_edit


def prepare(work):
    mod = work / 'Incremental'
    data = mod / 'Data'
    data.mkdir(parents=True)
    occupied = {instance_hash(name):name for name in ('IncrementalBase','IncrementalDerived','IncrementalSibling')}
    includes = []
    def armor(name, default='72.5%'):
        return '<ArmorTemplate id="%s" Default="%s"/>' % (allocate_id(name, occupied), default)
    for doc in range(32):
        name = 'armor-%02d.xml' % doc
        (data / name).write_text(declaration(''.join(armor('Incremental_%02d_%d' % (doc, i)) for i in range(8))), encoding='utf-8')
        includes.append('<Include type="all" source="%s"/>' % name)
    (data / 'large.xml').write_text(declaration(''.join(armor('IncrementalLarge_%03d' % i) for i in range(256))), encoding='utf-8')
    (data / 'defines.xml').write_text(declaration('<Defines><Define name="INCREMENTAL_SCALE" value="83.25"/></Defines>'), encoding='utf-8')
    (data / 'base.xml').write_text(declaration(
        '<Includes><Include type="all" source="defines.xml"/></Includes>'
        '<ArmorTemplate id="IncrementalBase" Default="72.5%" DamageScalar="=$INCREMENTAL_SCALE"/>'
        '<ArmorTemplate id="IncrementalSibling" Default="64.5%"/>'), encoding='utf-8')
    (data / 'derived.xml').write_text(declaration(
        '<Includes><Include type="instance" source="base.xml"/></Includes>'
        '<ArmorTemplate id="IncrementalDerived" inheritFrom="IncrementalBase" DamageScalar="=$INCREMENTAL_SCALE"/>'), encoding='utf-8')
    includes += ['<Include type="all" source="large.xml"/>', '<Include type="all" source="derived.xml"/>']
    (data / 'Mod.xml').write_text(declaration('<Includes>'+''.join(includes)+'</Includes>'), encoding='utf-8')
    return mod


def save(out, state):
    state['summary']=[]
    for scenario in state.get('scenarios', []):
        for phase in ('hot_before','modified','hot_after'):
            rows={x['implementation']:x for x in state['samples'] if x['scenario']==scenario and x['phase']==phase and x['status']=='passed'}
            if len(rows)==2:
                item=dict(mode='incremental-detailed',scenario=scenario,phase=phase)
                for label,row in rows.items():item[label]=dict(n=1,medianMs=row['wallMs'],minMs=row['wallMs'],maxMs=row['wallMs'])
                item['reductionPercent']=100*(1-item['candidate']['medianMs']/item['baseline']['medianMs'])
                state['summary'].append(item)
    write(out / 'measurement.json', state)
    lines = ['# Warm-cache edit paired diagnostics', '', 'Status: '+state['status'], '',
             'Synthetic 512 Armor + inheritance/Define workload, one process per implementation/phase. '
             'All samples use detailed instrumentation; these are diagnostic paired observations, not off-mode throughput medians. '
             'New process each time; OS page cache uncontrolled. Setup and raw byte comparison are outside process wall.', '',
             '| Scenario | Phase | Baseline ms | Candidate ms | Baseline parse/compile | Candidate parse/compile |',
             '| --- | --- | ---: | ---: | --- | --- |']
    for scenario in state.get('scenarios', []):
        for phase in ('hot_before','modified','hot_after'):
            rows={x['implementation']:x for x in state['samples'] if x['scenario']==scenario and x['phase']==phase and x['status']=='passed'}
            if len(rows)!=2: continue
            b,c=rows['baseline'],rows['candidate']
            lines.append('| %s | %s | %.2f | %.2f | %s/%s | %s/%s |' % (
                scenario,phase,b['wallMs'],c['wallMs'],b['cache']['xmlParse'],b['cache']['instanceCompile'],c['cache']['xmlParse'],c['cache']['instanceCompile']))
    lines += ['', 'Every generation compares actual published bytes across implementations; hot phases also compare their current generation. '
              'Unrelated sibling edits and lost session metadata must preserve publication bytes. '
              'Counters retain the difference between frontend work and actual backend calls. '
              'This bounded diagnostic does not replace the complete SDK/production regression.']
    if state.get('error'): lines += ['',state['error']]
    (out/'report.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')


def run(args):
    repo=Path(__file__).resolve().parents[2]
    work,out=args.work.resolve(),args.output.resolve()
    need(not work.exists() and not out.exists(),'Refusing to overwrite incremental evidence')
    work.mkdir(parents=True);out.mkdir(parents=True)
    if os.name=='nt':ctypes.windll.kernel32.SetErrorMode(0x0001|0x0002|0x8000)
    started=time.perf_counter()
    scenarios=['single-in-large-document','inheritance-source','unrelated-parent-sibling','define-value','session-metadata-missing']
    state=dict(schemaVersion=1,status='preparing',sourceCommit=git(repo,'rev-parse','HEAD'),runId=os.getenv('GITHUB_RUN_ID'),
               runAttempt=os.getenv('GITHUB_RUN_ATTEMPT'),baselineHead=BASELINE_HEAD,baselineBinarySource=BASELINE_BINARY_SOURCE,
               baselineRunId=BASELINE_RUN,candidateSource=args.candidate_source,candidateRunId=args.candidate_run_id,
               measurementMode='incremental',performance='detailed',armorPartitions=0,pairCount=1,budgetSeconds=args.budget,scenarios=scenarios,samples=[],mutations=[],
               machine=dict(system=platform.platform(),runnerImage=os.getenv('ImageVersion')),
               scope='Synthetic 512 Armor + three dependency assets, five representative warm-cache events; not full MOD or L8 Common')
    references={}
    def remaining():
        seconds=args.budget-(time.perf_counter()-started)
        need(seconds>0,'Incremental total budget exhausted')
        return min(args.timeout,seconds)
    try:
        source=git(repo,'rev-parse',args.candidate_source+'^{commit}')
        need(source==args.candidate_source,'Candidate source must be full SHA')
        subprocess.run(['git','-C',str(repo),'merge-base','--is-ancestor',BASELINE_HEAD,source],check=True)
        closure=('SDK','recovery/src','recovery/build','recovery/probes','recovery/scripts/Build*','recovery/scripts/PrepareLegacyTexture.ps1')
        need(not git(repo,'diff','--name-only',BASELINE_BINARY_SOURCE,BASELINE_HEAD,'--',*closure),'Accepted baseline input closure changed')
        changes=git(repo,'diff','--name-only',BASELINE_HEAD,source,'--',*closure).splitlines()
        need(changes and all(p.startswith(('recovery/src/App/','recovery/src/Managed/Core/','recovery/src/Managed/Metrics/')) for p in changes),'Unexpected candidate production closure')
        state['productionChanges']=changes
        binaries,identities={},{}
        for label,directory,commit,pin in [('baseline',args.baseline_build.resolve(),BASELINE_BINARY_SOURCE,BASELINE_FILES),('candidate',args.candidate_build.resolve(),source,None)]:
            binaries[label],identities[label]=product(directory,commit,pin)
            shutil.copy2(directory/'build.json',out/(label+'-build.json'))
        state['runtimeFiles']=identities
        mod=prepare(work);data=mod/'Data';state['initialInputs']=inventory(mod)
        destinations={label:work/('out-'+label) for label in binaries}
        state['status']='running';save(out,state)

        def sample(label,scenario,phase,generation):
            name='%02d-%s-%s-%s' % (len(state['samples']),label,scenario,phase)
            evidence=out/name;evidence.mkdir()
            destination=destinations[label]
            report_path=destination/'build-report.json';report_path.unlink(missing_ok=True)
            command=[str(binaries[label]),'build','--sdk-root',str(repo/'SDK'),'--mod',str(mod),'--out',str(destination),
                     '--package-content','base','--package-format','loose','--include-additional=false','--jobs','4','--performance','detailed']
            row=dict(implementation=label,scenario=scenario,phase=phase,generation=generation,status='started',command=command)
            state['samples'].append(row);save(out,state)
            try:
                with (evidence/'stdout.log').open('wb') as stdout,(evidence/'stderr.log').open('wb') as stderr:
                    tick=time.perf_counter();process=subprocess.Popen(command,cwd=binaries[label].parent,stdout=stdout,stderr=stderr)
                    try:process.wait(timeout=remaining())
                    except subprocess.TimeoutExpired:
                        process.kill();process.wait(timeout=5);row.update(status='timed_out',wallMs=(time.perf_counter()-tick)*1000);raise
                    row.update(wallMs=(time.perf_counter()-tick)*1000,exitCode=process.returncode,**process_metrics(process))
                report=json.loads(report_path.read_text(encoding='utf-8-sig'))
                row.update(reportElapsedMs=report['elapsedMs'],cache=report['cache'],diagnosticCounts=report['diagnosticCounts'],phases=phases(report))
                counts={}
                for counter in report['performance'].get('counters',[]):
                    if counter.get('streamId')=='base/Mod': counts[counter['name']]=counter['value']
                row['counters']=counts
                for key in ('core.xml.instance.hash','core.xml.instance.references','core.xml.schema.validate','core.xml.instances.validate','core.xml.overrides','core.xml.expressions'):
                    row['phases'][key]=sum(e['totalMs'] for e in report['performance']['entries'] if e['name']==key)
                need(process.returncode==0 and report['status']=='succeeded' and report['diagnosticCounts']['error']==0,name+' failed')
                if phase.startswith('hot'):
                    need(report['cache']['xmlParse']==0 and report['cache']['instanceCompile']==0 and report['cache']['documentReuse']>0,name+' failed to become hot')
                    need(counts.get('cache.invalidation.reloads',0)==0,'Hot process reported invalidation')
                if phase=='modified':
                    if scenario=='session-metadata-missing':
                        need(report['cache']['xmlParse']>0 and report['cache']['instanceCompile']==0,'Metadata loss must rebuild frontend without backend calls')
                    else:
                        need(report['cache']['xmlParse']>0,'Edit did not reload XML')
                        reason='cache.invalidation.'+('contentChanged' if scenario in ('single-in-large-document','inheritance-source','unrelated-parent-sibling') else 'definesChanged')
                        need(counts.get(reason,0)>0,'Expected actual invalidation reason missing: '+reason)
                        if scenario=='single-in-large-document':need(report['cache']['instanceCompile']==1,'Single Armor edit recompiled unrelated siblings')
                        if scenario in ('inheritance-source','unrelated-parent-sibling'):
                            need(counts.get('cache.invalidation.dependentFileChanged',0)>0,'Instance-include dependency did not trigger frontend propagation')
                        if scenario=='define-value':need(counts.get('cache.invalidation.inheritanceChanged',0)>0,'Define did not propagate through inheritance')
                content=published(destination,report)
                if generation not in references: references[generation]=content
                need(content==references[generation],'Published bytes differ across implementation/generation: '+name)
                if phase=='modified' and scenario in ('unrelated-parent-sibling','session-metadata-missing'):
                    need(content==references[generation-1],'Unrelated sibling/metadata edit changed published assets')
                if phase=='modified' and scenario in ('single-in-large-document','inheritance-source','define-value'):
                    need(content!=references[generation-1],'Effective asset edit did not change any published bytes')
                row.update(status='passed',rawPublishedIdentical=True,publishedBytes=sum(map(len,content.values())))
            except Exception as error:
                row['error']=str(error)
                if row['status']=='started':row['status']='failed'
                raise
            finally:
                if report_path.exists():shutil.copy2(report_path,evidence/'build-report.json')
                save(out,state)
                print(json.dumps({k:row.get(k) for k in ('implementation','scenario','phase','status','wallMs')}),flush=True)

        generation=0
        for label in ('baseline','candidate'):sample(label,'warmup','cold',generation)
        for index,scenario in enumerate(scenarios):
            order=('baseline','candidate') if index%2==0 else ('candidate','baseline')
            for label in order:sample(label,scenario,'hot_before',generation)
            if scenario=='session-metadata-missing':
                deleted={}
                for label,destination in destinations.items():
                    paths=[p for p in (destination/'BinaryAssetBuilder.SessionCache.xml',destination/'BinaryAssetBuilder.SessionCache.xml.deflate') if p.exists()]
                    need(paths,'No session metadata to remove')
                    deleted[label]=[str(p) for p in paths]
                    for p in paths:p.unlink()
                state['mutations'].append(dict(scenario=scenario,deleted=deleted))
            else:
                path,old,new={
                    'single-in-large-document':(data/'large.xml',b'72.5%',b'62.5%'),
                    'inheritance-source':(data/'base.xml',b'72.5%',b'62.5%'),
                    'unrelated-parent-sibling':(data/'base.xml',b'64.5%',b'54.5%'),
                    'define-value':(data/'defines.xml',b'83.25',b'73.25')}[scenario]
                mutation=same_stamp_edit(path,lambda b:b.replace(old,new,1));mutation['scenario']=scenario;state['mutations'].append(mutation)
            generation+=1
            for label in order:sample(label,scenario,'modified',generation)
            for label in reversed(order):sample(label,scenario,'hot_after',generation)
        remaining();state['status']='passed'
    except Exception as error:
        state.update(status='failed',error=str(error),errorType=type(error).__name__);raise
    finally:
        state['elapsedSeconds']=time.perf_counter()-started;save(out,state)


if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__)
    for name in ('baseline-build','candidate-build','work','output'):p.add_argument('--'+name,type=Path,required=True)
    p.add_argument('--candidate-source',required=True);p.add_argument('--candidate-run-id',required=True)
    p.add_argument('--budget',type=int,choices=range(60,241),default=240)
    p.add_argument('--timeout',type=int,choices=range(5,31),default=30)
    run(p.parse_args())
