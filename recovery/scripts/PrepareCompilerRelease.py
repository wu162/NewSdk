#!/usr/bin/env python3
"""Prepare a fixed fully accepted P7 runtime for P6 ZIP deployment validation.

Business tests are not rerun. The prior complete mymod evidence is explicitly
retained at its f2 binary / 9d test source, rather than relabelled as a P7 run.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import zipfile
import PrepareCompilerCliReuse as guard

SOURCE = '6bc790e91c92891088f3e1b0d671e3ce85050a08'
RUN = 40411445532
ATTEMPT = 1
ARTIFACT = 0
SHA256 = ''  # Pin only after this exact source run has reached full success.
REPOSITORY = 'wu162/NewSdk'
WORKFLOW = '.github/workflows/refactor-core-lifecycle.yml'
SDK_SOURCE = '9dcaac8212a5d5dfd690af0d1e69f58cc6c3cee6'
SDK_BINARY = 'f2ba141284469411d604d756f7916b0cf6aecf0a'
SDK_RUN = 40409382810
SDK_ARTIFACT = 12692512691
SDK_SHA256 = '128cc5333661ac1b3f793d47402c3c99399331b146985e1f32d5df3ec89922ad'
RUNTIME = guard.RUNTIME + ('BinaryAssetBuilder.TextureCompiler.dll','BinaryAssetBuilder.TextureCompiler.dll.patch.json')
ALLOWED = {
    'recovery/scripts/PrepareCompilerRelease.py', '.github/workflows/refactor-release.yml',
    'recovery/REFACTOR_RESUME.md', 'recovery/P5_LIFECYCLE_AUDIT.md', 'recovery/COMPILER_RELEASE.md',
    'recovery/scripts/PrepareCompilerSessionReplay.py',
}
SUCCESS_STEPS = {
    'Standalone P2 application contracts in Debug', 'Standalone P2 application contracts in Release',
    'Validate CLI scripts and output contracts before long Native linking', 'Build managed Foundation inputs',
    'Build full production combination and verify deployment and route metadata',
    'Targeted real Core, backend and application pipeline in Debug and Release',
    'Isolated original BAB baseline for the two targeted backend cases',
    'Validate actual application JSONL and reports against protocol schemas',
    'Host-bound Texture payload and deployment rejection in Debug and Release',
    'Real production CLI, cold processes and actual CtrlBreak cancellation',
    'Validate real production CLI protocol outputs', 'Actual build-cache cold hot invalidation and retry matrix',
    'Effective input roots and original BAB output differential', 'Actual Audio CDATA and W3X build-cache payload replay',
    'Actual session-cache documents intermediate and CDATA reuse matrix', 'Validate actual session-cache protocol outputs',
    'Production BIG apt maps selection and original MakeBig differential', 'Validate actual BIG production protocol outputs',
    'Validate actual cache and input-root protocol outputs', 'Run actions/upload-artifact@v4',
}
require = guard.require


def api(endpoint):
    return json.loads(subprocess.run(['gh','api',endpoint],check=True,capture_output=True).stdout)


def closure(repo, source, candidate):
    require(source == SOURCE, 'Release source differs from the fixed P7 candidate')
    for ref in (source,candidate):
        require(re.fullmatch('[0-9a-f]{40}',ref), 'Complete commit SHA required')
        guard.git(repo,'cat-file','-e',ref+'^{commit}')
    guard.git(repo,'merge-base','--is-ancestor',source,candidate)
    changed = sorted(p for p in guard.git(repo,'diff','--name-only','--no-renames','-z',source,candidate).decode().split('\0') if p)
    require(all(p in ALLOWED for p in changed), 'Production, SDK, dependency or build input changed: '+', '.join(p for p in changed if p not in ALLOWED))
    guard.git(repo,'diff','--exit-code',candidate,'--','recovery/src','SDK')
    require(not guard.git(repo,'ls-files','--others','--exclude-standard','-z','--','recovery/src','SDK'), 'Untracked production input')
    return changed


def metadata(run, artifact, jobs):
    require(ARTIFACT > 0 and re.fullmatch('[0-9a-f]{64}',SHA256), 'P7 archive is not pinned to successful evidence')
    require(run.get('id') == RUN and run.get('head_sha') == SOURCE and run.get('run_attempt') == ATTEMPT and
            run.get('status') == 'completed' and run.get('conclusion') == 'success' and run.get('path') == WORKFLOW and
            run.get('head_branch') == 'refactor/modcompiler-p0-p3', 'P7 run identity/full success mismatch')
    require(all(run.get(k,{}).get('full_name') == REPOSITORY for k in ('repository','head_repository')), 'Release repository mismatch')
    origin=artifact.get('workflow_run',{})
    require(artifact.get('id') == ARTIFACT and artifact.get('name') == f'refactor-core-lifecycle-{RUN}-{ATTEMPT}' and
            artifact.get('expired') is False and artifact.get('digest') == 'sha256:'+SHA256 and origin.get('id') == RUN and
            origin.get('head_sha') == SOURCE and origin.get('repository_id') == run['repository']['id'] and
            origin.get('head_repository_id') == run['head_repository']['id'], 'P7 artifact identity/digest mismatch')
    rows=jobs.get('jobs',[])
    require(jobs.get('total_count') == 1 and len(rows) == 1 and rows[0].get('run_id') == RUN and
            rows[0].get('conclusion') == 'success' and rows[0].get('status') == 'completed', 'P7 job did not fully succeed')
    steps=rows[0].get('steps',[])
    require(len(steps)==len({s['name'] for s in steps}), 'Duplicate P7 steps')
    for name in SUCCESS_STEPS:
        require([(s.get('status'),s.get('conclusion')) for s in steps if s.get('name') == name] == [('completed','success')], 'Required P7 scope failed/skipped: '+name)


def sdk_receipt(archive, repo, candidate):
    guard.git(repo,'merge-base','--is-ancestor',SDK_SOURCE,candidate)
    guard.git(repo,'diff','--exit-code',SDK_BINARY,SOURCE,'--','SDK')
    with open(archive,'rb') as stream:
        require(hashlib.sha256(stream.read()).hexdigest()==SDK_SHA256, 'Accepted SDK archive digest mismatch')
        stream.seek(0)
        with zipfile.ZipFile(stream) as z:
            path=f'evidence/refactor/sdk-mymod/{SDK_RUN}-1/validation.json'
            raw=z.read(path);d=json.loads(raw.decode('utf-8-sig'))
            require(d.get('sourceCommit')==SDK_SOURCE and d.get('binarySourceCommit')==SDK_BINARY and str(d.get('runId'))==str(SDK_RUN) and
                    str(d.get('runAttempt'))=='1' and all(d.get(k) is True for k in ('succeeded','originalBaselineSucceeded','originalBaselinePassed','inputsUnchanged')),
                    'Historical complete mymod receipt identity/acceptance mismatch')
            require(d.get('sdkGitTree')==guard.git(repo,'rev-parse',SOURCE+':SDK').decode().strip() and
                    d.get('modGitTree')==guard.git(repo,'rev-parse',SOURCE+':SDK/Mods/mymod').decode().strip(), 'Historical SDK/MOD inputs changed')
            modes=d.get('modes',[])
            require(len(modes)==2 and {m.get('configuration') for m in modes}==guard.MODES and all(m.get('succeeded') is True for m in modes), 'Historical SDK modes incomplete')
            for mode in modes:
                require(mode.get('comparisons') and all(c.get('outputs') and all(o.get('byteIdentical') is True and o['original']['sha256']==o['product']['sha256'] for o in c['outputs']) for c in mode['comparisons']), 'Historical raw SDK output comparison incomplete')
            return raw, {'sourceCommit':SDK_SOURCE,'binarySourceCommit':SDK_BINARY,'runId':SDK_RUN,'artifactId':SDK_ARTIFACT,'archiveSha256':SDK_SHA256,
                         'currentBinaryRetested':False,'scope':d['scope'],'emptyOptionalGroups':d.get('emptyOptionalGroups'),
                         'sdkInputsUnchanged':True,'reason':'P7 only adds explicit BIG apt/maps; current affected Core/CLI/cache/Texture/BIG groups must pass; historical full-mymod binary identity is retained'}


def prepare(archive, sdk_archive, candidate, output, repo):
    changed=closure(repo,SOURCE,candidate)
    sdk_raw,sdk_scope=sdk_receipt(sdk_archive,repo,candidate)
    # Reuse the complete existing archive/path/integrity and full Core/CLI guard.
    previous=guard.closure,guard.FIXTURES,guard.CLI_CASES
    try:
        guard.closure=lambda *a,**k: changed
        guard.FIXTURES=previous[1]|{'CompilerCacheContent','CompilerCacheBoundary','CompilerInputRoots'}
        guard.CLI_CASES=(previous[2]-{'cache-rejected','session-cache-rejected'})|{'compressed-session-cache-rejected'}
        receipt=guard.prepare(archive,SHA256,SOURCE,candidate,output,repo)
    finally:
        guard.closure,guard.FIXTURES,guard.CLI_CASES=previous
    target=Path(output)
    with zipfile.ZipFile(archive) as z:
        def doc(path):return json.loads(z.read(path).decode('utf-8-sig'))
        def take(path,relative):
            data=z.read(path);p=target/relative;p.parent.mkdir(parents=True,exist_ok=True);p.write_bytes(data)
            receipt['files'].append({'path':relative,'sha256':guard.digest(data)})
        cli_path=f'evidence/refactor/core-cli/{RUN}-{ATTEMPT}/validation.json';cli=doc(cli_path)
        for section in ('build-cache','input-roots','cache-payloads','session-cache','big'):
            base=f'evidence/refactor/{section}/{RUN}-{ATTEMPT}';d=doc(base+'/validation.json')
            require(d.get('succeeded') is True and d.get('sourceCommit')==SOURCE and str(d.get('runId'))==str(RUN) and str(d.get('runAttempt'))==str(ATTEMPT), 'P7 affected receipt failed: '+section)
            guard.modes(d);protocol=doc(base+'/protocol.json')
            require(protocol.get('sourceCommit')==SOURCE and protocol.get('status')=='passed' and protocol.get('errorCount')==0 and protocol.get('errors')==[] and
                    len(protocol.get('matrix',[]))==2 and {m.get('configuration') for m in protocol['matrix']}==guard.MODES and protocol.get('validatedFiles'), 'P7 affected protocol failed/incomplete')
            for row in protocol['validatedFiles']:
                require(guard.digest(z.read(base+'/'+guard.safe_name(row['path'])))==row['sha256'], 'P7 affected protocol input hash differs')
            take(base+'/validation.json','acceptance/'+section+'/validation.json');take(base+'/protocol.json','acceptance/'+section+'/protocol.json')
        for mode in sorted(guard.MODES):
            base=f'artifacts/core-lifecycle-product/{mode}/';cm=[m for m in cli['modes'] if m['configuration']==mode][0]
            require({k.replace('\\','/').lstrip('/') for k in cm['runtimeInputs']}==set(RUNTIME), 'P7 CLI runtime contains extra/missing files')
            for name in RUNTIME:
                sha=guard.digest(z.read(base+'bin/'+name))
                require([v for k,v in cm['runtimeInputs'].items() if k.replace('\\','/').lstrip('/')==name]==[sha], 'P7 runtime does not match accepted CLI bytes')
                if name not in guard.RUNTIME:take(base+'bin/'+name,mode+'/bin/'+name)
            patch=doc(base+'bin/BinaryAssetBuilder.TextureCompiler.dll.patch.json')
            require(patch.get('targetExeSha256')==cm['exeSha256'] and patch.get('patchedSha256')==guard.digest(z.read(base+'bin/BinaryAssetBuilder.TextureCompiler.dll')) and
                    patch.get('protectedPeAudit',{}).get('status')=='passed', 'P7 Texture deployment audit mismatch')
            take(base+'build.json',mode+'/build.json');take(base+'link-inputs.json',mode+'/link-inputs.json')
        take(cli_path,'accepted-cli.json')
    (target/'accepted-mymod.json').write_bytes(sdk_raw)
    receipt.update(binarySourceCommit=SOURCE,testSourceCommit=candidate,artifactId=ARTIFACT,productionSourcesUnchanged=True,
                   reusedMymod=sdk_scope,requiredNextSteps=['Run P6 five-file ZIP deployment validation only'],
                   scope='Current P7 production/cache/session/BIG acceptance plus historical f2/9d complete mymod scope; packaging preparation only, no new business execution')
    (target/'release-preparation.json').write_text(json.dumps(receipt,indent=2)+'\n',encoding='utf-8');(target/'reuse.json').unlink()
    return receipt


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    for n in ('repo','candidate-commit','output','archive','sdk-archive'):parser.add_argument('--'+n,required=True)
    a=parser.parse_args()
    try:
        closure(a.repo,SOURCE,a.candidate_commit)
        require(ARTIFACT>0 and re.fullmatch('[0-9a-f]{64}',SHA256), 'P7 archive is not pinned to successful evidence')
        metadata(api(f'repos/{REPOSITORY}/actions/runs/{RUN}'),api(f'repos/{REPOSITORY}/actions/artifacts/{ARTIFACT}'),api(f'repos/{REPOSITORY}/actions/runs/{RUN}/jobs?filter=latest&per_page=100'))
        old=api(f'repos/{REPOSITORY}/actions/artifacts/{SDK_ARTIFACT}')
        old_run=api(f'repos/{REPOSITORY}/actions/runs/{SDK_RUN}')
        require(old_run.get('head_sha')==SDK_SOURCE and old_run.get('run_attempt')==1 and old_run.get('status')=='completed' and
                old_run.get('conclusion')=='success' and old_run.get('path')=='.github/workflows/refactor-session-replay.yml', 'Historical SDK run is not the accepted successful source')
        require(old.get('id')==SDK_ARTIFACT and old.get('expired') is False and old.get('digest')=='sha256:'+SDK_SHA256 and
                old.get('workflow_run',{}).get('id')==SDK_RUN and old['workflow_run'].get('head_sha')==SDK_SOURCE,'Historical SDK artifact origin/digest mismatch')
        for file,artifact in ((a.archive,ARTIFACT),(a.sdk_archive,SDK_ARTIFACT)):
            with Path(file).open('xb') as stream:subprocess.run(['gh','api',f'repos/{REPOSITORY}/actions/artifacts/{artifact}/zip'],check=True,stdout=stream,stderr=subprocess.PIPE)
        r=prepare(a.archive,a.sdk_archive,a.candidate_commit,a.output,a.repo)
        print(json.dumps({k:r[k] for k in ('prepared','candidateValidated','binarySourceCommit','testSourceCommit','runId')}))
    except (ValueError,OSError,KeyError,TypeError,zipfile.BadZipFile,subprocess.CalledProcessError) as error:
        parser.exit(1,'Release preparation rejected: '+str(error)+'\n')


if __name__=='__main__':main()
