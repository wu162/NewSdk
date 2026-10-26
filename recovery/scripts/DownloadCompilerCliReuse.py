#!/usr/bin/env python3
"""Download one pinned, successful Actions archive and prepare historical runtimes.

GH_TOKEN is consumed by the GitHub CLI, never passed on its command line or logged.
No archive member is executed by this script. Current CLI tests remain mandatory.
"""
import argparse
import json
from pathlib import Path
import subprocess

from PrepareCompilerCliReuse import closure, prepare, require

REPOSITORY = 'wu162/NewSdk'
SOURCE = 'b8952e6f8813802de82eed363e168c05afc7f02e'
RUN = 40333013142
ATTEMPT = 1
ARTIFACT = 12657090511
SHA256 = '5407c5ac65528a81eb16090aae5e4dde05ced165d4ffddc024d5ae8184870dc3'
NAME = f'refactor-core-lifecycle-{RUN}-{ATTEMPT}'
WORKFLOW = '.github/workflows/refactor-core-lifecycle.yml'


def api_json(endpoint):
    return json.loads(subprocess.run(['gh', 'api', endpoint], check=True,
                                    capture_output=True).stdout)


def verify_metadata(run, artifact):
    require(run.get('id') == RUN and run.get('head_sha') == SOURCE and
            run.get('run_attempt') == ATTEMPT and run.get('status') == 'completed' and
            run.get('conclusion') == 'success' and run.get('path') == WORKFLOW and
            run.get('head_branch') == 'refactor/modcompiler-p0-p3',
            'Pinned workflow run identity or success mismatch')
    for key in ('repository', 'head_repository'):
        require(run.get(key, {}).get('full_name') == REPOSITORY,
                'Workflow repository identity mismatch')
    require(artifact.get('id') == ARTIFACT and artifact.get('name') == NAME and
            artifact.get('expired') is False and artifact.get('digest') == 'sha256:' + SHA256,
            'Pinned artifact identity, expiry or digest mismatch')
    origin = artifact.get('workflow_run', {})
    require(origin.get('id') == RUN and origin.get('head_sha') == SOURCE and
            origin.get('head_branch') == 'refactor/modcompiler-p0-p3' and
            origin.get('repository_id') == run['repository']['id'] and
            origin.get('head_repository_id') == run['head_repository']['id'],
            'Artifact workflow origin mismatch')


def download(repo, candidate_commit, output, archive):
    closure(repo, SOURCE, candidate_commit)
    run = api_json(f'repos/{REPOSITORY}/actions/runs/{RUN}')
    artifact = api_json(f'repos/{REPOSITORY}/actions/artifacts/{ARTIFACT}')
    verify_metadata(run, artifact)
    # Exclusive creation prevents silently replacing either a previous download
    # or a different process's evidence. gh follows authenticated GitHub redirects.
    with Path(archive).open('xb') as stream:
        subprocess.run(['gh', 'api', f'repos/{REPOSITORY}/actions/artifacts/{ARTIFACT}/zip'],
                       check=True, stdout=stream, stderr=subprocess.PIPE)
    receipt = prepare(archive, SHA256, SOURCE, candidate_commit, output, repo)
    require(receipt['runId'] == str(RUN) and receipt['runAttempt'] == str(ATTEMPT),
            'Downloaded archive receipt run mismatch')
    return receipt


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for option in ('repo', 'candidate-commit', 'output', 'archive'):
        parser.add_argument('--' + option, required=True)
    args = parser.parse_args()
    try:
        receipt = download(**vars(args))
        print(json.dumps({'prepared': True, 'candidateValidated': False,
                          'historicalSourceCommit': receipt['sourceCommit'],
                          'currentTestCommit': receipt['candidateCommit'],
                          'runId': receipt['runId'], 'files': len(receipt['files'])}))
    except (ValueError, OSError, KeyError, TypeError, subprocess.CalledProcessError) as error:
        # Do not expose subprocess stderr: authentication diagnostics may include
        # request context. The fixed action and exception type suffice for triage.
        parser.exit(1, 'CLI reuse download rejected: ' + type(error).__name__ + '\n')


if __name__ == '__main__':
    main()
