#!/usr/bin/env python3
"""Prepare one fixed /sc test replay without rebuilding production binaries.

The original run's failure is preserved. Only new session/SDK test results may be
accepted; the historical production/Core/CLI receipts retain their own source.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import zipfile
import PrepareCompilerCliReuse as guard

SOURCE = '4992236fc0659192d1ee50211617db67af23c15b'
RUN = 40405225193
ATTEMPT = 1
# Set only from the completed fixed run's actual uploaded artifact metadata.
# Zero/empty constants deliberately reject preparation before that evidence exists.
ARTIFACT = 0
SHA256 = ''
REPOSITORY = 'wu162/NewSdk'
WORKFLOW = '.github/workflows/refactor-core-lifecycle.yml'
NAME = f'refactor-core-lifecycle-{RUN}-{ATTEMPT}'
WORKFLOW_BYTES = {
    'source': 'c15da43d21c9c92e8b2b33b5bd6a25fe08265495d0dc1da3b27ca296f7fca3ad',
    # Exactly three test-only additions: invoke fixture preflight, check its exit,
    # and upload its evidence. No setup/build/deployment command may change.
    'candidate': 'a1e5436db10bffac0073cb1b90588c34fd5925ab2c99e97d81c16351c3f086a8',
}
ALLOWED = {
    '.github/workflows/refactor-session-replay.yml',
    'recovery/scripts/PrepareCompilerSessionReplay.py',
    'recovery/scripts/TestCompilerSessionCache.ps1',
    'recovery/scripts/TestCompilerSdk.ps1',
    'recovery/REFACTOR_RESUME.md', 'recovery/P5_LIFECYCLE_AUDIT.md',
}
# The exact failure set must be confirmed against the completed source run.
FAILED_STEPS = {'Actual session-cache documents intermediate and CDATA reuse matrix'}
SUCCESS_STEPS = {
    'Standalone P2 application contracts in Debug',
    'Standalone P2 application contracts in Release',
    'Validate CLI scripts and output contracts before long Native linking',
    'Build managed Foundation inputs',
    'Build full production combination and verify deployment and route metadata',
    'Targeted real Core, backend and application pipeline in Debug and Release',
    'Isolated original BAB baseline for the two targeted backend cases',
    'Validate actual application JSONL and reports against protocol schemas',
    'Host-bound Texture payload and deployment rejection in Debug and Release',
    'Real production CLI, cold processes and actual CtrlBreak cancellation',
    'Validate real production CLI protocol outputs',
    'Actual build-cache cold hot invalidation and retry matrix',
    'Effective input roots and original BAB output differential',
    'Actual Audio CDATA and W3X build-cache payload replay',
    'Validate actual cache and input-root protocol outputs',
    'Run actions/upload-artifact@v4',
}
require = guard.require


def api(endpoint):
    return json.loads(subprocess.run(['gh', 'api', endpoint], check=True, capture_output=True).stdout)


def closure(repo, source, candidate):
    require(source == SOURCE, 'Session replay source must be the fixed production commit')
    for value in (source, candidate):
        require(re.fullmatch('[0-9a-f]{40}', value), 'Use complete lowercase commit SHAs')
        guard.git(repo, 'cat-file', '-e', value + '^{commit}')
    guard.git(repo, 'merge-base', '--is-ancestor', source, candidate)
    # Complete changed-path closure also rejects removed dependencies and build
    # recipes outside src/SDK, rather than assuming their directory names.
    changed = sorted(p for p in guard.git(repo, 'diff', '--name-only', '--no-renames', '-z', source, candidate).decode().split('\0') if p)
    require(all(path in ALLOWED or path == WORKFLOW for path in changed),
            'Production/build input changed: ' + ', '.join(p for p in changed if p not in ALLOWED and p != WORKFLOW))
    guard.git(repo, 'diff', '--exit-code', source, candidate, '--', 'recovery/src', 'SDK')
    if WORKFLOW in changed:
        for ref, key in ((source, 'source'), (candidate, 'candidate')):
            require(guard.digest(guard.git(repo, 'show', ref + ':' + WORKFLOW)) == WORKFLOW_BYTES[key],
                    'Core workflow change exceeds the exact reviewed preflight additions')
    return changed


def metadata(run, artifact, jobs):
    require(ARTIFACT > 0 and re.fullmatch('[0-9a-f]{64}', SHA256), 'Fixed archive has not yet been pinned')
    require(run.get('id') == RUN and run.get('head_sha') == SOURCE and run.get('run_attempt') == ATTEMPT and
            run.get('status') == 'completed' and run.get('conclusion') == 'failure' and
            run.get('path') == WORKFLOW and run.get('head_branch') == 'refactor/modcompiler-p0-p3', 'Fixed source run identity/terminal mismatch')
    for key in ('repository', 'head_repository'):
        require(run.get(key, {}).get('full_name') == REPOSITORY, 'Replay repository mismatch')
    require(artifact.get('id') == ARTIFACT and artifact.get('name') == NAME and artifact.get('expired') is False and
            artifact.get('digest') == 'sha256:' + SHA256, 'Fixed artifact identity/digest mismatch')
    origin = artifact.get('workflow_run', {})
    require(origin.get('id') == RUN and origin.get('head_sha') == SOURCE and origin.get('head_branch') == 'refactor/modcompiler-p0-p3' and
            origin.get('repository_id') == run['repository']['id'] and origin.get('head_repository_id') == run['head_repository']['id'], 'Fixed artifact origin mismatch')
    rows = jobs.get('jobs', [])
    require(jobs.get('total_count') == 1 and len(rows) == 1 and rows[0].get('run_id') == RUN and
            rows[0].get('status') == 'completed' and rows[0].get('conclusion') == 'failure', 'Incomplete fixed source jobs')
    steps = rows[0].get('steps', [])
    names = [s.get('name') for s in steps]
    require(len(names) == len(set(names)), 'Duplicate source step names')
    require({s['name'] for s in steps if s.get('conclusion') == 'failure'} == FAILED_STEPS,
            'Fixed run failure set differs from the reviewed test-tool failures')
    for name in SUCCESS_STEPS:
        matched = [s for s in steps if s.get('name') == name]
        require(len(matched) == 1 and matched[0].get('status') == 'completed' and matched[0].get('conclusion') == 'success',
                'Required independent production scope failed: ' + name)


def prepare(archive, candidate, output, repo):
    changed = closure(repo, SOURCE, candidate)
    require(hashlib.sha256(Path(archive).read_bytes()).hexdigest() == SHA256, 'Fixed archive SHA256 mismatch')
    with zipfile.ZipFile(archive) as z:
        def document(path):
            return json.loads(z.read(path).decode('utf-8-sig'))
        for section in ('build-cache', 'input-roots', 'cache-payloads'):
            base = f'evidence/refactor/{section}/{RUN}-{ATTEMPT}'
            receipt = document(base + '/validation.json')
            require(receipt.get('sourceCommit') == SOURCE and str(receipt.get('runId')) == str(RUN) and
                    str(receipt.get('runAttempt')) == str(ATTEMPT) and receipt.get('succeeded') is True,
                    'Independent cache/input-root receipt incomplete: ' + section)
            guard.modes(receipt)
            protocol = document(base + '/protocol.json')
            require(protocol.get('sourceCommit') == SOURCE and protocol.get('status') == 'passed' and
                    protocol.get('errorCount') == 0 and protocol.get('errors') == [], 'Independent protocol failed: ' + section)
            require(len(protocol.get('matrix', [])) == 2 and {r.get('configuration') for r in protocol['matrix']} == guard.MODES,
                    'Independent protocol configurations incomplete: ' + section)
            require(protocol.get('validatedFiles'), 'Independent protocol has no input hashes')
            for row in protocol['validatedFiles']:
                relative = guard.safe_name(row['path'])
                require(guard.digest(z.read(base + '/' + relative)) == row['sha256'], 'Independent protocol raw bytes differ')
        for mode in sorted(guard.MODES):
            base = f'artifacts/core-lifecycle-product/{mode}/bin/'
            patch = document(base + 'BinaryAssetBuilder.TextureCompiler.dll.patch.json')
            texture = guard.digest(z.read(base + 'BinaryAssetBuilder.TextureCompiler.dll'))
            exe = guard.digest(z.read(base + 'NewModCompiler.exe'))
            require(patch.get('patchedSha256') == texture and patch.get('targetExeSha256') == exe and
                    patch.get('originalSha256') == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' and
                    patch.get('protectedPeAudit', {}).get('status') == 'passed' and
                    patch.get('protectedPeAudit', {}).get('outputSha256') == texture, 'Fixed EXE-bound Texture identity mismatch')
    previous = guard.closure, guard.FIXTURES, guard.CLI_CASES
    try:
        guard.closure = lambda *args, **kwargs: changed
        guard.FIXTURES = previous[1] | {'CompilerCacheContent', 'CompilerCacheBoundary', 'CompilerInputRoots'}
        guard.CLI_CASES = (previous[2] - {'cache-rejected'}) | {'compressed-session-cache-rejected'}
        receipt = guard.prepare(archive, SHA256, SOURCE, candidate, output, repo)
    finally:
        guard.closure, guard.FIXTURES, guard.CLI_CASES = previous
    require(receipt['runId'] == str(RUN) and receipt['runAttempt'] == str(ATTEMPT), 'Fixed archive run identity mismatch')
    receipt.update(binarySourceCommit=SOURCE, testSourceCommit=candidate, artifactId=ARTIFACT,
                   productionSourcesUnchanged=True, sourceRunConclusion='failure', sourceFailedSteps=sorted(FAILED_STEPS),
                   requiredNextSteps=['Run corrected session-cache tests and protocol validation', 'Run complete mymod original/production SDK comparison after session success'],
                   scope='Only session test replay and subsequent SDK results are new; fixed production bytes, original failure and independent successful scopes retain source identities')
    target = Path(output)
    (target / 'session-replay.json').write_text(json.dumps(receipt, indent=2) + '\n', encoding='utf-8')
    (target / 'reuse.json').unlink()
    return receipt


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for name in ('repo', 'candidate-commit', 'output', 'archive'):
        parser.add_argument('--' + name, required=True)
    args = parser.parse_args()
    try:
        closure(args.repo, SOURCE, args.candidate_commit)
        metadata(api(f'repos/{REPOSITORY}/actions/runs/{RUN}'), api(f'repos/{REPOSITORY}/actions/artifacts/{ARTIFACT}'),
                 api(f'repos/{REPOSITORY}/actions/runs/{RUN}/jobs?filter=latest&per_page=100'))
        with Path(args.archive).open('xb') as stream:
            subprocess.run(['gh', 'api', f'repos/{REPOSITORY}/actions/artifacts/{ARTIFACT}/zip'], check=True, stdout=stream, stderr=subprocess.PIPE)
        receipt = prepare(args.archive, args.candidate_commit, args.output, args.repo)
        print(json.dumps({k: receipt[k] for k in ('prepared', 'candidateValidated', 'binarySourceCommit', 'testSourceCommit', 'runId')}))
    except (ValueError, OSError, KeyError, TypeError, zipfile.BadZipFile, subprocess.CalledProcessError) as error:
        parser.exit(1, 'Fixed session replay rejected: ' + str(error) + '\n')


if __name__ == '__main__':
    main()
