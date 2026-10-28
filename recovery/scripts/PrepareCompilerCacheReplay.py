#!/usr/bin/env python3
"""Prepare a successful cache-parallel production archive for new payload tests.

This never builds production or treats preparation as candidate acceptance. Only
an explicitly identified successful run is accepted; failed-run partial reuse is
deliberately unsupported. The default purpose requires current payload/protocol
tests; the separate upheaval-metrics purpose only measures an already accepted
runtime and never grants candidate payload, SDK or release acceptance.

This entry is not pinned to a historical binary source. Once the complete Core
lifecycle run succeeds, dispatch refactor-cache-payload-replay.yml on the current
test commit with source_commit, run_id, run_attempt, artifact_id and archive_sha256
from that successful run. Source must be an ancestor of the test commit. Every
tracked production input, including newly added files such as
Application/CacheInputLeaseSet.cs, is immutable by default; MUTABLE only names
reviewed test/receipt paths. The whole production recipe, SDK and runtime source
closure must agree. Dirty or untracked production inputs reject preparation.

Preparation rechecks run/attempt/repository, successful acceptance steps, archive
digest, build/link/toolchain receipts and exact runtime bytes. It records separate
binarySourceCommit/testSourceCommit and still requires the current payload and
protocol matrix. This is not a Core validation-host, SDK or final-release replay.
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

from PrepareCompilerCliReuse import digest, git, modes, require, safe_name

REPOSITORY = 'wu162/NewSdk'
BRANCH = 'feature/cache-parallel'
WORKFLOW = '.github/workflows/refactor-core-lifecycle.yml'
RUNTIME = ('NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll')
FIVE_RUNTIME = RUNTIME + ('BinaryAssetBuilder.TextureCompiler.dll', 'BinaryAssetBuilder.TextureCompiler.dll.patch.json')
# Every tracked input is immutable except these reviewed test/receipt tools and
# documentation. In particular there is no directory, extension or workflow
# wildcard, and the entire production workflow/build recipe remains identical.
MUTABLE = {
    'README.md',
    '.github/workflows/refactor-cache-payload-replay.yml',
    'recovery/scripts/PrepareCompilerCacheReplay.py',
    'recovery/tests/test_prepare_compiler_cache_replay.py',
    'recovery/scripts/TestCompilerBuildCachePayloads.ps1',
    'recovery/scripts/BuildCompilerCliControl.ps1',
    'recovery/tests/CompilerCliControl.cs',
    'recovery/scripts/ValidateAppProtocol.py',
    'recovery/REFACTOR_RESUME.md', 'recovery/COMPILER_USAGE.md',
    'recovery/docs/CACHE_PARALLEL.md', 'recovery/docs/CACHE_PARALLEL_EVIDENCE.json',
}
UPHEAVAL_SCRIPTS = (
    'recovery/scripts/TestCompilerUpheavalMetrics.ps1',
    'recovery/scripts/PrepareUpheavalInputs.py',
)
UPHEAVAL_MUTABLE = set(UPHEAVAL_SCRIPTS) | {
    '.github/workflows/refactor-upheaval-cache-metrics.yml',
}
PURPOSES = ('payload', 'upheaval-metrics')
UPHEAVAL_REF = 'refs/heads/test/upheaval-cache-parallel-20261028'
UPHEAVAL_PROFILE_KEYS = {'source_commit', 'run_id', 'run_attempt', 'artifact_id', 'archive_sha256'}
REQUIRED_STEPS = {
    'Build full production combination and verify deployment and route metadata',
    'Targeted real Core, backend and application pipeline in Debug and Release',
    'Actual Audio CDATA and W3X build-cache payload replay',
    'Complete mymod original BAB and production SDK differential',
    'Publish final five-file packages after complete production acceptance',
}


def resolve_upheaval_profile(message, reference, dispatch=None):
    """Resolve inert event data; an ordinary push never requests a product run."""
    if dispatch is None:
        markers = [message.count('[upheaval-preflight]'), message.count('[upheaval-measure-once]')]
        require(sum(markers) <= 1, 'Upheaval markers must be unique and mutually exclusive')
        if sum(markers) == 0:
            return dict(run_measure='false')
        lines = [line for line in message.splitlines() if line.startswith('UpheavalProfile:')]
        require(len(lines) == 1 and lines[0].startswith('UpheavalProfile: '), 'One explicit UpheavalProfile JSON line is required')
        def unique(pairs):
            result = {}
            for key, value in pairs:
                require(key not in result, 'Duplicate profile key')
                result[key] = value
            return result
        profile = json.loads(lines[0][len('UpheavalProfile: '):], object_pairs_hook=unique)
        preflight = 'true' if markers[0] else 'false'
    else:
        require(isinstance(dispatch, dict) and set(dispatch) == UPHEAVAL_PROFILE_KEYS | {'preflight_only'}, 'Dispatch profile keys differ')
        profile = {key: dispatch[key] for key in UPHEAVAL_PROFILE_KEYS}
        preflight = dispatch['preflight_only']
        require(preflight in ('true', 'false'), 'Dispatch preflight must be an explicit boolean')
    require(reference == UPHEAVAL_REF, 'Measurements require the dedicated test branch')
    require(isinstance(profile, dict) and set(profile) == UPHEAVAL_PROFILE_KEYS, 'Profile must contain exactly five reviewed keys')
    require(isinstance(profile['source_commit'], str) and re.fullmatch(r'[0-9a-f]{40}', profile['source_commit']), 'Full lowercase production source SHA required')
    require(isinstance(profile['archive_sha256'], str) and re.fullmatch(r'[0-9a-f]{64}', profile['archive_sha256']), 'Full lowercase archive SHA256 required')
    for key in ('run_id', 'run_attempt', 'artifact_id'):
        value = profile[key]
        require(type(value) in (str, int) and re.fullmatch(r'[1-9][0-9]*', str(value)), 'Positive decimal profile identity required')
        profile[key] = str(value)
    return dict(profile, run_measure='true', preflight_only=preflight)


def closure(repo, source, candidate, purpose='payload'):
    require(purpose in PURPOSES, 'Unreviewed replay purpose')
    mutable = MUTABLE | (UPHEAVAL_MUTABLE if purpose == 'upheaval-metrics' else set())
    for commit in (source, candidate):
        require(re.fullmatch('[0-9a-f]{40}', commit), 'Use full lowercase commit SHAs')
        git(repo, 'cat-file', '-e', commit + '^{commit}')
    git(repo, 'merge-base', '--is-ancestor', source, candidate)
    # The source comparison must describe bytes actually consumed by this replay,
    # including SDK schemas/configuration, rather than only clean Git objects.
    git(repo, 'diff', '--exit-code', candidate, '--')
    extra = git(repo, 'ls-files', '--others', '--exclude-standard', '--',
                'recovery/src', 'recovery/build', 'recovery/catalog', 'SDK')
    require(not extra.strip(), 'Untracked production or SDK input present')
    rows = git(repo, 'diff', '--name-status', '--no-renames', '-z', source, candidate).decode().split('\0')
    changed = []
    for index in range(0, len(rows) - 1, 2):
        status, path = rows[index:index + 2]
        require(status in ('M', 'A') and path in mutable, 'Non-replay input changed: ' + path)
        changed.append(path)
    return sorted(changed)


def measurement_scripts(repo, candidate, purpose):
    require(purpose in PURPOSES, 'Unreviewed replay purpose')
    if purpose == 'payload':
        return []
    rows = []
    for path in UPHEAVAL_SCRIPTS:
        # Authorize Git's canonical clean identity, including this path's actual
        # checkout attributes/core.autocrlf. Windows CRLF consumption need not
        # equal the raw LF blob, and must never be approved by ad-hoc stripping.
        # Keep a separate SHA256 of the exact consumed bytes in the receipt.
        committed = git(repo, 'rev-parse', candidate + ':' + path).decode().strip()
        require(git(repo, 'cat-file', '-t', committed).strip() == b'blob', 'Measurement input is not a Git blob: ' + path)
        actual = (Path(repo) / path).read_bytes()
        cleaned = subprocess.check_output(['git', '-C', str(repo), 'hash-object',
                                           '--path=' + path, '--stdin'], input=actual).decode().strip()
        require(cleaned == committed, 'Measurement script differs from candidate clean identity: ' + path)
        rows.append(dict(path=path, sha256=digest(actual), canonicalGitBlob=committed))
    return rows


def profile_identity(profile):
    require(re.fullmatch('[0-9a-f]{40}', profile.get('binarySourceCommit', '')), 'Invalid binary source SHA')
    require(re.fullmatch('[0-9a-f]{64}', profile.get('archiveSha256', '')), 'Invalid archive SHA256')
    for key in ('runId', 'runAttempt', 'artifactId'):
        require(type(profile.get(key)) is int and profile[key] > 0, 'Invalid profile ' + key)


def metadata(run, artifact, jobs, profile):
    profile_identity(profile)
    source, run_id, attempt = (profile[k] for k in ('binarySourceCommit', 'runId', 'runAttempt'))
    require(run.get('id') == run_id and run.get('head_sha') == source and run.get('run_attempt') == attempt and
            run.get('status') == 'completed' and run.get('conclusion') == 'success' and
            run.get('head_branch') == BRANCH and run.get('path') == WORKFLOW, 'Successful production run identity mismatch')
    for key in ('repository', 'head_repository'):
        require(run.get(key, {}).get('full_name') == REPOSITORY, 'Production repository mismatch')
    require(artifact.get('id') == profile['artifactId'] and artifact.get('expired') is False and
            artifact.get('name') == f'refactor-core-lifecycle-{run_id}-{attempt}' and
            artifact.get('digest') == 'sha256:' + profile['archiveSha256'], 'Artifact identity/digest mismatch')
    origin = artifact.get('workflow_run', {})
    require(origin.get('id') == run_id and origin.get('head_sha') == source and origin.get('head_branch') == BRANCH and
            origin.get('repository_id') == run['repository']['id'] and
            origin.get('head_repository_id') == run['head_repository']['id'], 'Artifact origin mismatch')
    rows = jobs.get('jobs', [])
    require(jobs.get('total_count') == 1 and len(rows) == 1 and rows[0].get('run_id') == run_id and
            rows[0].get('status') == 'completed' and rows[0].get('conclusion') == 'success', 'Production jobs incomplete')
    steps = rows[0].get('steps', [])
    names = [row.get('name') for row in steps]
    require(len(names) == len(set(names)), 'Duplicate production step names')
    for name in REQUIRED_STEPS:
        found = [row for row in steps if row.get('name') == name]
        require(len(found) == 1 and found[0].get('status') == 'completed' and found[0].get('conclusion') == 'success',
                'Missing successful production scope: ' + name)


def safe_zip(z):
    seen = set()
    for entry in z.infolist():
        name = safe_name(entry.orig_filename)
        require(entry.orig_filename == entry.filename and name.casefold() not in seen,
                'Duplicate or normalized ZIP path: ' + name)
        require(not stat.S_ISLNK(entry.external_attr >> 16) and not entry.flag_bits & 1, 'Unsafe ZIP entry: ' + name)
        seen.add(name.casefold())


def validate_archive(z, profile, purpose='payload'):
    """Validate both accepted configurations; stage only exact approved bytes."""
    require(purpose in PURPOSES, 'Unreviewed replay purpose')
    safe_zip(z)
    source = profile['binarySourceCommit']
    run, attempt = str(profile['runId']), str(profile['runAttempt'])
    tag = run + '-' + attempt
    receipts = []

    def document(path):
        data = z.read(path)
        value = json.loads(data.decode('utf-8-sig'))
        require(isinstance(value, dict), 'Receipt must be an object: ' + path)
        receipts.append({'path': path, 'sha256': digest(data)})
        return value

    def identity(row):
        require(row.get('sourceCommit') == source and str(row.get('runId')) == run and
                str(row.get('runAttempt')) == attempt, 'Archive receipt source/run mismatch')

    accepted = {}
    for scope in ('core-lifecycle-deployment', 'core-lifecycle', 'cache-payloads', 'core-cli'):
        row = document(f'evidence/refactor/{scope}/{tag}/validation.json')
        identity(row)
        require(row.get('status') == 'passed' if scope == 'core-lifecycle-deployment' else row.get('succeeded') is True,
                'Archived production scope did not pass: ' + scope)
        accepted[scope] = modes(row)
    release_prefix = f'artifacts/final-release/{tag}/'
    release = document(release_prefix + 'release.json')
    identity(release)
    require(release.get('succeeded') is True and release.get('binarySourceCommit') == source and
            release.get('status') == 'verified-small-mod-deployment', 'Release acceptance incomplete')
    configurations = release.get('configurations', [])
    require(len(configurations) == 2 and {r.get('configuration') for r in configurations} == {'Debug', 'Release'} and
            all(r.get('succeeded') is True for r in configurations), 'Missing Debug/Release release success')
    payload, files, toolchains = {}, [], {}
    for mode in ('Debug', 'Release'):
        prefix = f'artifacts/core-lifecycle-product/{mode}/'
        build_path, link_path = prefix + 'build.json', prefix + 'link-inputs.json'
        build, link = document(build_path), document(link_path)
        require(build.get('sourceCommit') == source and link.get('sourceCommit') == source and
                build.get('configuration') == mode and link.get('configuration') == mode and
                build.get('schemaVersion') == 2 and build.get('runtime') == 'net48/Win32' and
                build.get('containsProbeOrTestSources') is False and build.get('backendIntegrated') is True and
                build.get('entryPoint') == 'NewModCompiler.Hosting.CompilerEntry.Main' and
                build.get('runtimeFiles') == list(FIVE_RUNTIME), 'Production build recipe identity mismatch')
        require(link.get('arguments') and link.get('inputs') and
                all(re.fullmatch('[0-9a-f]{64}', r.get('sha256', '')) for r in link['inputs']), 'Missing link inputs')
        toolchain = {key: build.get(key) for key in ('compiler', 'vcToolsVersion', 'windowsSdkVersion')}
        require(toolchain['compiler'] == 'Microsoft.Net.Compilers.Toolset/4.0.1' and
                all(isinstance(v, str) and v for v in toolchain.values()), 'Missing original toolchain identity')
        manifest = document(release_prefix + f'NewModCompiler-Win32-{mode}.manifest.json')
        require(manifest.get('binarySourceCommit') == source and manifest.get('packagingSourceCommit') == source and
                str(manifest.get('runId')) == run and str(manifest.get('runAttempt')) == attempt and
                manifest.get('configuration') == mode and manifest.get('toolchain') == toolchain and
                manifest.get('buildReceiptSha256') == digest(z.read(build_path)) and
                manifest.get('linkRecipeSha256') == digest(z.read(link_path)), 'Release build/link/toolchain identity mismatch')
        inventory = manifest.get('files', [])
        require(len(inventory) == 5 and {r.get('name') for r in inventory} == set(FIVE_RUNTIME), 'Release runtime inventory mismatch')
        for row in inventory:
            data = z.read(prefix + 'bin/' + row['name'])
            require(row.get('bytes') == len(data) and row.get('sha256') == digest(data), 'Runtime hash mismatch: ' + row['name'])
            staged = FIVE_RUNTIME if purpose == 'upheaval-metrics' else RUNTIME
            if row['name'] in staged:
                path = mode + '/bin/' + row['name']
                payload[path] = data
                files.append({'path': path, 'sha256': digest(data)})
            if row['name'] in RUNTIME:
                runtime_inputs = accepted['core-cli'][mode].get('runtimeInputs', {})
                cli_hashes = [sha for name, sha in runtime_inputs.items()
                              if name.replace('\\', '/').lstrip('/') == row['name']]
                require(cli_hashes == [digest(data)], 'CLI runtime hash mismatch: ' + row['name'])
        exe_sha = digest(payload[mode + '/bin/NewModCompiler.exe'])
        require(build.get('exeSha256') == exe_sha and
                accepted['core-lifecycle-deployment'][mode].get('exeSha256') == exe_sha and
                accepted['core-lifecycle'][mode].get('productExeSha256') == exe_sha and
                accepted['cache-payloads'][mode].get('exeSha256') == exe_sha and
                accepted['core-cli'][mode].get('exeSha256') == exe_sha, 'Accepted EXE identities differ')
        toolchains[mode] = toolchain
    return payload, files, receipts, toolchains


def prepare(archive, profile, candidate, output, repo, purpose='payload'):
    profile_identity(profile)
    changed = closure(repo, profile['binarySourceCommit'], candidate, purpose)
    scripts = measurement_scripts(repo, candidate, purpose)
    with Path(archive).open('rb') as stream:
        hasher = hashlib.sha256()
        for block in iter(lambda: stream.read(1024 * 1024), b''):
            hasher.update(block)
        require(hasher.hexdigest() == profile['archiveSha256'], 'Archive SHA256 mismatch')
        stream.seek(0)
        with zipfile.ZipFile(stream) as z:
            payload, files, receipts, toolchains = validate_archive(z, profile, purpose)
    receipt = dict(schemaVersion=1, prepared=True, candidateValidated=False, productionSourcesUnchanged=True,
                   purpose=purpose, measurementScripts=scripts,
                   binarySourceCommit=profile['binarySourceCommit'], testSourceCommit=candidate,
                   profile=profile, archivePath=str(Path(archive).resolve()), archiveSha256=profile['archiveSha256'],
                   runId=str(profile['runId']), runAttempt=str(profile['runAttempt']),
                   files=files, originalReceipts=receipts, toolchains=toolchains, changedPaths=changed,
                   testScriptSha256=digest((Path(repo) / 'recovery/scripts/TestCompilerBuildCachePayloads.ps1').read_bytes()),
                   scope=('Successful original production archive; current payload/protocol tests required; no production rebuild'
                          if purpose == 'payload' else
                          'Successful production archive for isolated Upheaval measurements only; no production rebuild or new payload/SDK/release acceptance'))
    output = Path(output)
    require(not output.exists(), 'Refusing to replace replay output')
    output.parent.mkdir(parents=True, exist_ok=True)
    temporary = Path(tempfile.mkdtemp(prefix=output.name + '-', dir=output.parent))
    try:
        for path, data in payload.items():
            target = temporary / path
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(data)
        (temporary / 'cache-payload-replay.json').write_text(json.dumps(receipt, indent=2) + '\n', encoding='utf-8')
        temporary.rename(output)
    finally:
        if temporary.exists():
            shutil.rmtree(temporary)
    return receipt


def api(endpoint):
    return json.loads(subprocess.run(['gh', 'api', endpoint], check=True, capture_output=True).stdout)


def check_api(profile):
    run_id, attempt = profile['runId'], profile['runAttempt']
    metadata(api(f'repos/{REPOSITORY}/actions/runs/{run_id}/attempts/{attempt}'),
             api(f'repos/{REPOSITORY}/actions/artifacts/{profile["artifactId"]}'),
             api(f'repos/{REPOSITORY}/actions/runs/{run_id}/attempts/{attempt}/jobs?per_page=100'), profile)


def verify_prepared(receipt_path, candidate, repo, purpose='payload'):
    path = Path(receipt_path)
    row = json.loads(path.read_text(encoding='utf-8'))
    require(purpose in PURPOSES and row.get('purpose') == purpose, 'Replay purpose mismatch')
    profile = row['profile']
    check_api(profile)
    with tempfile.TemporaryDirectory(prefix='verify-cache-replay-') as directory:
        expected = prepare(row['archivePath'], profile, candidate, Path(directory) / 'verified', repo, purpose)
    require(row == expected, 'Replay preparation receipt differs from independently verified archive/closure')
    for identity in row['files']:
        require(digest((path.parent / identity['path']).read_bytes()) == identity['sha256'], 'Prepared runtime hash mismatch')
    return row


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for name in ('repo', 'candidate-commit'):
        parser.add_argument('--' + name, required=True)
    parser.add_argument('--verify-prepared')
    parser.add_argument('--purpose', choices=PURPOSES, default='payload')
    for name in ('source-commit', 'run-id', 'run-attempt', 'artifact-id', 'archive-sha256', 'archive', 'output'):
        parser.add_argument('--' + name)
    args = parser.parse_args()
    try:
        if args.verify_prepared:
            receipt = verify_prepared(args.verify_prepared, args.candidate_commit, args.repo, args.purpose)
        else:
            profile = dict(binarySourceCommit=args.source_commit, runId=int(args.run_id), runAttempt=int(args.run_attempt),
                           artifactId=int(args.artifact_id), archiveSha256=args.archive_sha256)
            profile_identity(profile)
            closure(args.repo, profile['binarySourceCommit'], args.candidate_commit, args.purpose)
            check_api(profile)
            with Path(args.archive).open('xb') as stream:
                subprocess.run(['gh', 'api', f'repos/{REPOSITORY}/actions/artifacts/{profile["artifactId"]}/zip'],
                               check=True, stdout=stream, stderr=subprocess.PIPE)
            receipt = prepare(args.archive, profile, args.candidate_commit, args.output, args.repo, args.purpose)
        print(json.dumps({k: receipt[k] for k in ('prepared', 'candidateValidated', 'binarySourceCommit', 'testSourceCommit', 'runId')}))
    except (ValueError, OSError, KeyError, TypeError, zipfile.BadZipFile, subprocess.CalledProcessError) as error:
        parser.exit(1, 'Cache payload replay rejected: ' + type(error).__name__ + '\n')


if __name__ == '__main__':
    main()
