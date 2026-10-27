#!/usr/bin/env python3
"""Stage pinned, accepted production/validation binaries for test-only Actions.

No extracted program is executed here. Candidate production inputs must be
unchanged; the caller must compile and run the current targeted fixtures.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import stat
import subprocess
import tempfile
import zipfile

from PrepareCompilerCliReuse import digest, git, require, safe_name

REPOSITORY = 'wu162/NewSdk'
SOURCE = '68f38abb1ff649857ff786aa8a6bcf657553b8da'
RUN = 40476134103
ATTEMPT = 1
BRANCH = 'feature/compiler-optimization'
WORKFLOW = '.github/workflows/refactor-core-lifecycle.yml'
REUSE_WORKFLOW = '.github/workflows/refactor-targeted-reuse.yml'
RUNTIME = ('NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll',
           'BinaryAssetBuilder.TextureCompiler.dll', 'BinaryAssetBuilder.TextureCompiler.dll.patch.json')
VALIDATION_RUNTIME = ('NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll')
# Exact current targeted suite only. Adding a different test or changing a
# production build driver still requires fresh production acceptance.
TARGETED_FIXTURES = (
    'CompilerInputDigests', 'CompilerPhysicalInputs', 'CompilerManifestInputs',
    'CompilerStreamBindings', 'CompilerTransferBuffers', 'CompilerDeclarationSnapshots',
    'CompilerExpressionPrograms', 'CompilerDependencyObservations', 'CompilerBaseManifestIndex',
    'CompilerNodeJoiner', 'CompilerSchemaDescriptors', 'CompilerSchemaMetadata',
    'CompilerCacheContent', 'CompilerCacheBoundary', 'CompilerInputRoots',
    'CompilerCorePaths', 'CompilerUnicodeSettings', 'CompilerCoreLifecycle',
    'CompilerResourceLifecycle', 'CompilerStateLifecycle', 'CompilerBackendLifecycle',
    'CompilerApplicationBuild',
)
ALLOWED = {REUSE_WORKFLOW, 'recovery/scripts/DownloadCompilerTargetedReuse.py',
           'recovery/scripts/TestCompilerTargetedReuse.py', 'recovery/scripts/TestCompilerCoreLifecycle.ps1',
           'recovery/scripts/ExportCompilerAcceptedRelease.py', 'recovery/scripts/TestCompilerAcceptedRelease.py'}
ALLOWED.update('recovery/tests/' + name + '.cs' for name in TARGETED_FIXTURES)
ROOT_DOCS = {'AGENTS.md', 'README.md', 'recovery/README.md', 'recovery/REFACTOR_RESUME.md',
             'recovery/EFFICIENCY.md', 'recovery/RESUME.md', 'recovery/STAGE_DURATION.md'}
PRODUCTION_ROOTS = ('recovery/src', 'SDK', 'recovery/build')


def closure(repo, candidate):
    require(re.fullmatch(r'[0-9a-f]{40}', candidate), 'Use the full lowercase test commit')
    for commit in (SOURCE, candidate):
        git(repo, 'cat-file', '-e', commit + '^{commit}')
    git(repo, 'merge-base', '--is-ancestor', SOURCE, candidate)
    require(git(repo, 'rev-parse', 'HEAD').decode().strip() == candidate, 'Checkout differs from test source')
    git(repo, 'diff', '--quiet')
    git(repo, 'diff', '--cached', '--quiet')
    # Name-only diffs would silently admit deletions/renames between two
    # allowlisted files. Only additive or modified reviewed test/doc paths qualify.
    changes = [part for part in git(repo, 'diff', '--name-status', '--no-renames', '-z', SOURCE, candidate).decode().split('\0') if part]
    require(len(changes) % 2 == 0, 'Unexpected changed-path record shape')
    rows = list(zip(changes[::2], changes[1::2]))
    require(all(status in ('A', 'M') for status, path in rows), 'Deleted, renamed or type-changed input cannot reuse production')
    paths = sorted(path for status, path in rows)
    forbidden = [p for p in paths if not (p in ALLOWED or p in ROOT_DOCS or (p.startswith('recovery/docs/') and p.endswith('.md')))]
    require(not forbidden, 'Production or unrelated test inputs changed: ' + ', '.join(forbidden))
    # Deliberately omit --exclude-standard: ignored untracked files can still
    # enter recursive production source, SDK, or build-recipe discovery. Scan
    # only production roots so importing this controller's scripts/__pycache__
    # cannot create a false rejection. Do not exempt such names inside a source
    # root: a .cs file there is still a potential production input.
    untracked = [p for p in git(repo, 'ls-files', '--others', '-z', '--', *PRODUCTION_ROOTS).decode().split('\0') if p]
    require(not untracked, 'Untracked production inputs present (including ignored): ' + ', '.join(sorted(untracked)))
    return paths


def api(endpoint):
    return json.loads(subprocess.run(['gh', 'api', endpoint], check=True, capture_output=True).stdout)


def accepted_metadata():
    run = api(f'repos/{REPOSITORY}/actions/runs/{RUN}')
    require(run.get('id') == RUN and run.get('head_sha') == SOURCE and run.get('run_attempt') == ATTEMPT and
            run.get('status') == 'completed' and run.get('conclusion') == 'success' and
            run.get('head_branch') == BRANCH and run.get('path') == WORKFLOW,
            'Pinned full workflow has not succeeded with the required identity')
    for key in ('repository', 'head_repository'):
        require(run.get(key, {}).get('full_name') == REPOSITORY, 'Workflow repository mismatch')
    listing = api(f'repos/{REPOSITORY}/actions/runs/{RUN}/artifacts?per_page=100')
    rows = listing.get('artifacts', [])
    require(listing.get('total_count') == len(rows), 'Artifact listing is incomplete')
    name = f'refactor-core-lifecycle-{RUN}-{ATTEMPT}'
    found = [a for a in rows if a.get('name') == name]
    require(len(found) == 1, 'Expected exactly one pinned complete artifact')
    artifact = found[0]
    require(artifact.get('expired') is False and isinstance(artifact.get('id'), int) and
            re.fullmatch(r'sha256:[0-9a-f]{64}', artifact.get('digest') or '') and
            isinstance(artifact.get('size_in_bytes'), int) and 0 < artifact['size_in_bytes'] <= 512 * 1024 * 1024,
            'Artifact expired, digest missing or archive exceeds the bound')
    origin = artifact.get('workflow_run', {})
    require(origin.get('id') == RUN and origin.get('head_sha') == SOURCE and origin.get('head_branch') == BRANCH and
            origin.get('repository_id') == run['repository']['id'] and
            origin.get('head_repository_id') == run['head_repository']['id'], 'Artifact workflow origin mismatch')
    return artifact


def prepare(archive, artifact, candidate, output, changed):
    files, payload = [], {}
    expected_digest = artifact['digest'].split(':', 1)[1]
    with Path(archive).open('rb') as stream:
        hasher = hashlib.sha256()
        for block in iter(lambda: stream.read(1024 * 1024), b''):
            hasher.update(block)
        require(hasher.hexdigest() == expected_digest and stream.tell() == artifact['size_in_bytes'], 'Downloaded archive digest/size mismatch')
        stream.seek(0)
        with zipfile.ZipFile(stream) as z:
            names, folded = set(), set()
            for entry in z.infolist():
                name = safe_name(entry.orig_filename)
                require(entry.orig_filename == entry.filename and name.casefold() not in folded, 'ZIP name normalization or duplicate/case conflict')
                require(not stat.S_ISLNK(entry.external_attr >> 16) and not entry.flag_bits & 1, 'ZIP symlink/encryption forbidden')
                names.add(entry.filename); folded.add(name.casefold())

            def read(path):
                require(path in names, 'Missing pinned archive member: ' + path)
                require(z.getinfo(path).file_size <= 128 * 1024 * 1024, 'Pinned archive member exceeds bound')
                return z.read(path)

            def document(path):
                data = read(path)
                require(len(data) <= 16 * 1024 * 1024, 'Receipt exceeds bound')
                value = json.loads(data.decode('utf-8-sig'))
                require(isinstance(value, dict), 'Expected object receipt: ' + path)
                return value

            core = document(f'evidence/refactor/core-lifecycle/{RUN}-{ATTEMPT}/validation.json')
            deployment = document(f'evidence/refactor/core-lifecycle-deployment/{RUN}-{ATTEMPT}/validation.json')
            for receipt in (core, deployment):
                require(receipt.get('sourceCommit') == SOURCE and str(receipt.get('runId')) == str(RUN) and
                        str(receipt.get('runAttempt')) == str(ATTEMPT), 'Acceptance receipt identity mismatch')
            require(core.get('succeeded') is True and deployment.get('status') == 'passed', 'Original targeted/deployment receipts did not pass')
            core_modes = {r['configuration']: r for r in core.get('modes', [])}
            dep_modes = {r['configuration']: r for r in deployment.get('modes', [])}
            require(len(core.get('modes', [])) == len(deployment.get('modes', [])) == 2 and
                    set(core_modes) == set(dep_modes) == {'Debug', 'Release'}, 'Acceptance configuration coverage mismatch')
            for mode in ('Debug', 'Release'):
                prefix = f'artifacts/core-lifecycle-product/{mode}/'
                product = document(prefix + 'build.json')
                validation = document(prefix + 'validation/build.json')
                recipe_bytes = read(prefix + 'link-inputs.json')
                recipe = json.loads(recipe_bytes.decode('utf-8-sig'))
                for receipt in (product, validation, recipe):
                    require(receipt.get('sourceCommit') == SOURCE and receipt.get('configuration') == mode, 'Binary receipt source/configuration mismatch')
                require(product.get('schemaVersion') == validation.get('schemaVersion') == 2 and recipe.get('schemaVersion') == 1 and
                        product.get('containsProbeOrTestSources') is False and product.get('backendIntegrated') is True and
                        product.get('entryPoint') == 'NewModCompiler.Hosting.CompilerEntry.Main' and
                        validation.get('entryPoint') == 'NewModCompiler.Validation.ValidationEntry.Main' and
                        validation.get('productionEntryPoint') == product['entryPoint'] and
                        validation.get('sameProductionObjects') is True and validation.get('productionExeUnchanged') is True and
                        validation.get('productExeSha256') == product.get('exeSha256') and
                        validation.get('linkRecipeSha256') == digest(recipe_bytes), 'Production/validation linkage identity mismatch')
                inputs = recipe.get('inputs', [])
                require(isinstance(inputs, list) and inputs and len([x for x in inputs if x.get('kind') == 'entry']) == 1,
                        'Production object inventory incomplete')
                expected = [r for r in inputs if r.get('kind') != 'entry']
                require(validation.get('inputs') == expected, 'Validation objects differ from production inventory')
                for item in inputs:
                    require(isinstance(item.get('path'), str) and re.fullmatch(r'[0-9a-f]{64}', item.get('sha256') or ''), 'Invalid object inventory')
                require([r.get('sha256') for r in inputs if r.get('kind') == 'foundation'] == [product.get('foundationSha256')] and
                        [r.get('sha256') for r in inputs if r.get('kind') == 'application'] == [product.get('applicationSha256')], 'Managed module inventory differs from build receipt')
                require(core_modes[mode].get('passed') is True and dep_modes[mode].get('passed') is True and
                        core_modes[mode].get('productExeSha256') == dep_modes[mode].get('exeSha256') == product.get('exeSha256') and
                        core_modes[mode].get('validationHostSha256') == validation.get('exeSha256'), 'Accepted EXE identities differ')
                require(product.get('runtimeFiles') == list(RUNTIME), 'Production runtime inventory changed')
                runtimes = validation.get('runtimeFiles', [])
                require([r.get('name') for r in runtimes] == list(VALIDATION_RUNTIME[1:]), 'Validation runtime inventory changed')
                require(core_modes[mode].get('runtimeFiles') == runtimes and
                        core_modes[mode].get('linkRecipeSha256') == digest(recipe_bytes), 'Accepted runtime/link recipe inventory differs')
                for group, expected_names in (('bin/', RUNTIME), ('validation/bin/', VALIDATION_RUNTIME)):
                    actual = {n[len(prefix + group):] for n in names if n.startswith(prefix + group) and not n.endswith('/')}
                    require(actual == set(expected_names), 'Unexpected runtime files in ' + mode + '/' + group)
                    for name in expected_names:
                        data = read(prefix + group + name)
                        relative = mode + '/' + group + name
                        payload[relative] = data; files.append({'path': relative, 'sha256': digest(data)})
                require(digest(payload[mode + '/bin/NewModCompiler.exe']) == product['exeSha256'] and
                        digest(payload[mode + '/validation/bin/NewModCompiler.exe']) == validation['exeSha256'], 'EXE hash mismatch')
                for row in runtimes:
                    sha = row.get('sha256')
                    require(re.fullmatch(r'[0-9a-f]{64}', sha or ''), 'Invalid runtime digest')
                    for group in ('bin/', 'validation/bin/'):
                        require(digest(payload[mode + '/' + group + row['name']]) == sha, 'Runtime hash mismatch')
                patch = json.loads(payload[mode + '/bin/BinaryAssetBuilder.TextureCompiler.dll.patch.json'].decode('utf-8-sig'))
                require(patch.get('targetExeSha256') == product['exeSha256'] and
                        patch.get('patchedSha256') == digest(payload[mode + '/bin/BinaryAssetBuilder.TextureCompiler.dll']), 'Production texture binding/hash mismatch')
                for name in ('build.json', 'link-inputs.json', 'validation/build.json'):
                    data = read(prefix + name); relative = mode + '/' + name
                    payload[relative] = data; files.append({'path': relative, 'sha256': digest(data)})
    receipt = {'schemaVersion': 1, 'prepared': True, 'candidateValidated': False,
               'binarySourceCommit': SOURCE, 'testSourceCommit': candidate, 'sourceCommit': SOURCE, 'candidateCommit': candidate,
               'runId': str(RUN), 'runAttempt': str(ATTEMPT), 'sourceRunId': str(RUN), 'sourceRunAttempt': str(ATTEMPT),
               'artifactId': artifact['id'], 'artifactName': artifact['name'], 'artifactSha256': expected_digest,
               'archiveSha256': expected_digest, 'archiveBytes': artifact['size_in_bytes'],
               'repository': REPOSITORY, 'sourceWorkflow': WORKFLOW, 'originalRunConclusion': 'success',
               'changedPaths': changed, 'changedFiles': changed, 'files': files,
               'scope': 'Strict pinned binary preparation only; current targeted fixtures must be compiled/executed; original receipts remain unchanged'}
    output = Path(output)
    require(not output.exists() and not output.is_symlink(), 'Refusing to replace an existing output')
    output.parent.mkdir(parents=True, exist_ok=True)
    temporary = Path(tempfile.mkdtemp(prefix=output.name + '-', dir=output.parent))
    try:
        for relative, data in payload.items():
            target = temporary / relative; target.parent.mkdir(parents=True, exist_ok=True); target.write_bytes(data)
        (temporary / 'reuse.json').write_text(json.dumps(receipt, indent=2) + '\n', encoding='utf-8')
        temporary.rename(output)
    finally:
        if temporary.exists():
            shutil.rmtree(temporary)
    return receipt


def download(repo, candidate_commit, output, archive):
    changed = closure(repo, candidate_commit)
    artifact = accepted_metadata()
    with Path(archive).open('xb') as stream:
        subprocess.run(['gh', 'api', f'repos/{REPOSITORY}/actions/artifacts/{artifact["id"]}/zip'], check=True, stdout=stream, stderr=subprocess.PIPE)
    return prepare(archive, artifact, candidate_commit, output, changed)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for option in ('repo', 'candidate-commit', 'output', 'archive'):
        parser.add_argument('--' + option, required=True)
    args = parser.parse_args()
    try:
        receipt = download(**vars(args))
        print(json.dumps({'prepared': True, 'candidateValidated': False, 'binarySourceCommit': SOURCE,
                          'testSourceCommit': receipt['testSourceCommit'], 'runId': str(RUN), 'artifactId': receipt['artifactId'], 'files': len(receipt['files'])}))
    except (ValueError, OSError, KeyError, TypeError, zipfile.BadZipFile, subprocess.CalledProcessError) as error:
        parser.exit(1, 'Targeted reuse rejected: ' + type(error).__name__ + '\n')


if __name__ == '__main__':
    main()
