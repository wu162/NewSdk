#!/usr/bin/env python3
"""Verify two distinct production artifacts; never reuse old bytes as the candidate."""
import argparse
import hashlib
import io
import json
from pathlib import Path
import subprocess
import zipfile

PRIOR_SOURCE = '8b3e146cb5220e830ff6a0ce4718f47af052f166'
PRIOR_RUN = 40446975355
PRIOR_ARTIFACT = 12708969379
PRIOR_DIGEST = '4354fd09d9390e4224152fc48b2ca9ff302cde5dcda01f244d2e3c74c4ad12e9'
FILES = {'NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll',
         'BinaryAssetBuilder.TextureCompiler.dll', 'BinaryAssetBuilder.TextureCompiler.dll.patch.json'}

def sha(data):
    return hashlib.sha256(data).hexdigest()

def api(path):
    return json.loads(subprocess.check_output(['gh', 'api', 'repos/wu162/NewSdk/' + path]))

def git(*args):
    return subprocess.check_output(['git', *args]).decode().strip()

def stage(output, source, run_id, artifact_id=None, expected_digest=None):
    head = git('rev-parse', 'HEAD')
    subprocess.run(['git', 'merge-base', '--is-ancestor', source, head], check=True)
    run = api(f'actions/runs/{run_id}')
    if run['head_sha'] != source or run['conclusion'] != 'success' or run['repository']['full_name'] != 'wu162/NewSdk':
        raise ValueError('Production acceptance run is not successful for the pinned source')
    if artifact_id is None:
        artifacts = api(f'actions/runs/{run_id}/artifacts')['artifacts']
        matches = [a for a in artifacts if a['name'] == f'refactor-core-lifecycle-{run_id}-1']
        if len(matches) != 1:
            raise ValueError('Expected one successful production lifecycle artifact')
        artifact_id = matches[0]['id']
    artifact = api(f'actions/artifacts/{artifact_id}')
    if artifact['expired'] or artifact['workflow_run']['id'] != run_id or artifact['workflow_run']['head_sha'] != source:
        raise ValueError('Production artifact origin differs')
    digest = artifact['digest']
    if expected_digest is not None and digest != 'sha256:' + expected_digest:
        raise ValueError('Pinned original artifact digest differs')
    if not digest.startswith('sha256:'):
        raise ValueError('Artifact SHA256 missing')
    output.mkdir(parents=True, exist_ok=False)
    archive = output/'archive.zip'
    with archive.open('xb') as stream:
        subprocess.run(['gh', 'api', f'repos/wu162/NewSdk/actions/artifacts/{artifact_id}/zip'], check=True, stdout=stream)
    if 'sha256:' + sha(archive.read_bytes()) != digest:
        raise ValueError('Downloaded artifact SHA256 differs')
    with zipfile.ZipFile(archive) as outer:
        matches = [n for n in outer.namelist() if n.endswith('/NewModCompiler-Win32-Release.zip')]
        if len(matches) != 1:
            raise ValueError('Expected one accepted five-file Release package')
        package = outer.read(matches[0])
        with zipfile.ZipFile(io.BytesIO(package)) as inner:
            if set(inner.namelist()) != FILES or len(inner.namelist()) != len(FILES):
                raise ValueError('Unexpected runtime package layout')
            runtime = output/'bin'; runtime.mkdir()
            identities = {}
            for name in sorted(FILES):
                data = inner.read(name); (runtime/name).write_bytes(data); identities[name] = sha(data)
    patch = json.loads((runtime/'BinaryAssetBuilder.TextureCompiler.dll.patch.json').read_text())
    if not (patch['targetExeSha256'] == identities['NewModCompiler.exe']
            and patch['patchedSha256'] == identities['BinaryAssetBuilder.TextureCompiler.dll']
            and patch['protectedPeAudit']['status'] == 'passed'):
        raise ValueError('EXE-bound Texture package identity differs')
    receipt = dict(prepared=True, testSourceCommit=head, binarySourceCommit=source,
                   sourceRunId=run_id, artifactId=artifact_id, archiveSha256=digest[7:],
                   packageSha256=sha(package), files=identities, candidateValidated=True)
    (output/'reuse.json').write_text(json.dumps(receipt, indent=2)+'\n')
    archive.unlink()
    return receipt

def main():
    p = argparse.ArgumentParser()
    p.add_argument('--source', required=True)
    p.add_argument('--run', required=True, type=int)
    p.add_argument('--output', default='recovery/artifacts/cache-speed-runtime')
    args = p.parse_args()
    head = git('rev-parse', 'HEAD')
    # Candidate production inputs must still be precisely the accepted build's.
    for path in ('recovery/src', 'SDK', 'recovery/scripts', 'recovery/probes', 'recovery/build', 'recovery/catalog'):
        subprocess.run(['git', 'diff', '--exit-code', args.source, head, '--', path], check=True, stdout=subprocess.DEVNULL)
    subprocess.run(['git', 'diff', '--exit-code', PRIOR_SOURCE, head, '--', 'SDK'], check=True, stdout=subprocess.DEVNULL)
    output = Path(args.output)
    receipts = dict(before=stage(output/'before', PRIOR_SOURCE, PRIOR_RUN, PRIOR_ARTIFACT, PRIOR_DIGEST),
                    after=stage(output/'after', args.source, args.run))
    if receipts['before']['files']['NewModCompiler.exe'] == receipts['after']['files']['NewModCompiler.exe']:
        raise ValueError('Candidate EXE unexpectedly equals the original')
    (output/'provenance.json').write_text(json.dumps(receipts, indent=2)+'\n')
    print(json.dumps(receipts))

if __name__ == '__main__':
    main()
