#!/usr/bin/env python3
"""Stage the accepted Release runtime, refusing changed production inputs."""
import hashlib
import io
import json
from pathlib import Path
import subprocess
import sys
import zipfile

SOURCE = '8b3e146cb5220e830ff6a0ce4718f47af052f166'
RUN = 40446975355
ARTIFACT = 12708969379
SHA256 = '4354fd09d9390e4224152fc48b2ca9ff302cde5dcda01f244d2e3c74c4ad12e9'
FILES = {'NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll',
         'BinaryAssetBuilder.TextureCompiler.dll', 'BinaryAssetBuilder.TextureCompiler.dll.patch.json'}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def git(*args):
    return subprocess.check_output(['git', *args]).decode().strip()


def api(path):
    return json.loads(subprocess.check_output(['gh', 'api', 'repos/wu162/NewSdk/' + path]))


def prepare(output):
    candidate = git('rev-parse', 'HEAD')
    subprocess.run(['git', 'merge-base', '--is-ancestor', SOURCE, candidate], check=True)
    changed = git('diff', '--name-only', '--no-renames', SOURCE, candidate).splitlines()
    allowed = lambda p: (p.endswith('.md') or p.startswith('recovery/benchmarks/upheaval/')
                         or p == '.github/workflows/benchmark-upheaval.yml')
    denied = [p for p in changed if not allowed(p)]
    if denied:
        raise ValueError('Production/build/SDK input changed: ' + ', '.join(denied))
    # Documentation exceptions never permit changed runtime or build sources.
    for path in ('recovery/src', 'SDK', 'recovery/scripts', 'recovery/probes', 'recovery/catalog'):
        subprocess.run(['git', 'diff', '--exit-code', SOURCE, candidate, '--', path],
                       check=True, stdout=subprocess.DEVNULL)
    run = api(f'actions/runs/{RUN}')
    artifact = api(f'actions/artifacts/{ARTIFACT}')
    if not (run['head_sha'] == SOURCE and run['conclusion'] == 'success'
            and run['repository']['full_name'] == 'wu162/NewSdk'
            and artifact['id'] == ARTIFACT and not artifact['expired']
            and artifact['digest'] == 'sha256:' + SHA256
            and artifact['workflow_run']['head_sha'] == SOURCE
            and artifact['workflow_run']['id'] == RUN):
        raise ValueError('Accepted runtime origin or artifact identity mismatch')
    output = Path(output)
    output.mkdir(parents=True, exist_ok=False)
    archive = output / 'accepted.zip'
    with archive.open('xb') as stream:
        subprocess.run(['gh', 'api', f'repos/wu162/NewSdk/actions/artifacts/{ARTIFACT}/zip'],
                       check=True, stdout=stream)
    if sha(archive.read_bytes()) != SHA256:
        raise ValueError('Accepted archive SHA256 mismatch')
    with zipfile.ZipFile(archive) as outer:
        matches = [n for n in outer.namelist() if n.endswith('/NewModCompiler-Win32-Release.zip')]
        if len(matches) != 1:
            raise ValueError('Expected one accepted Release package')
        package = outer.read(matches[0])
        with zipfile.ZipFile(io.BytesIO(package)) as inner:
            if len(inner.namelist()) != 5 or set(inner.namelist()) != FILES:
                raise ValueError('Release package must contain exactly five runtime files')
            runtime = output / 'bin'
            runtime.mkdir()
            identities = {}
            for name in sorted(FILES):
                data = inner.read(name)
                (runtime / name).write_bytes(data)
                identities[name] = sha(data)
        patch = json.loads((runtime / 'BinaryAssetBuilder.TextureCompiler.dll.patch.json').read_text())
        if not (patch['targetExeSha256'] == identities['NewModCompiler.exe']
                and patch['patchedSha256'] == identities['BinaryAssetBuilder.TextureCompiler.dll']
                and patch['protectedPeAudit']['status'] == 'passed'):
            raise ValueError('EXE-bound Texture runtime identity mismatch')
    receipt = dict(prepared=True, candidateValidated=False, binarySourceCommit=SOURCE,
                   testSourceCommit=candidate, sourceRunId=RUN, artifactId=ARTIFACT,
                   archiveSha256=SHA256, packageSha256=sha(package), files=identities,
                   changedPaths=changed, productionInputsUnchanged=True)
    (output / 'reuse.json').write_text(json.dumps(receipt, indent=2) + '\n')
    archive.unlink()
    print(json.dumps(receipt))


if __name__ == '__main__':
    prepare(sys.argv[1])
