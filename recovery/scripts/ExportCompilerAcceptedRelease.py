#!/usr/bin/env python3
"""Export original accepted five-file release ZIPs without rebuilding/repacking.

Requires the strict targeted-reuse controller's staged runtime and receipt, and
independently rechecks the pinned successful workflow/artifact metadata. This
exports historical acceptance; it never accepts the current test candidate.
"""
import argparse
import hashlib
import io
import json
from pathlib import Path
import re
import shutil
import stat
import subprocess
import tempfile
import zipfile

import DownloadCompilerTargetedReuse as trusted
from PrepareCompilerCliReuse import digest, require, safe_name

MAX_ARCHIVE = 512 * 1024 * 1024
MAX_MEMBER = 128 * 1024 * 1024
MAX_PACKAGE = 32 * 1024 * 1024
MAX_JSON = 16 * 1024 * 1024
MAX_EXPANDED = 2 * 1024 * 1024 * 1024
MODES = ('Debug', 'Release')


def json_object(data, description):
    require(len(data) <= MAX_JSON, 'Receipt exceeds bound: ' + description)
    value = json.loads(data.decode('utf-8-sig'))
    require(isinstance(value, dict), 'Expected object receipt: ' + description)
    return value


def regular(path, directory=False):
    info = Path(path).lstat()
    require(not stat.S_ISLNK(info.st_mode) and not getattr(info, 'st_file_attributes', 0) & 0x400,
            'Runtime path is a symlink/reparse point: ' + str(path))
    require(stat.S_ISDIR(info.st_mode) if directory else stat.S_ISREG(info.st_mode), 'Unexpected runtime path type: ' + str(path))
    return info


def index_zip(archive):
    entries, folded, expanded = {}, set(), 0
    for entry in archive.infolist():
        normalized = safe_name(entry.orig_filename)
        require(entry.orig_filename == entry.filename and normalized.casefold() not in folded,
                'ZIP normalization or duplicate/case conflict')
        mode = entry.external_attr >> 16
        require(not stat.S_ISLNK(mode) and not entry.flag_bits & 1, 'ZIP symlink/encryption forbidden')
        require(stat.S_IFMT(mode) in (0, stat.S_IFREG, stat.S_IFDIR), 'ZIP special file forbidden')
        require(0 <= entry.file_size <= MAX_MEMBER, 'ZIP member exceeds bound')
        expanded += entry.file_size
        require(expanded <= MAX_EXPANDED, 'ZIP expanded inventory exceeds bound')
        folded.add(normalized.casefold())
        entries[entry.filename] = entry
    return entries


def read_member(archive, entries, name, limit=MAX_MEMBER):
    require(name in entries and not entries[name].is_dir(), 'Missing original archive member: ' + name)
    require(entries[name].file_size <= limit, 'Original archive member exceeds bound: ' + name)
    data = archive.read(entries[name])
    require(len(data) == entries[name].file_size, 'Original archive member size mismatch')
    return data


def source_identity(receipt):
    require(receipt.get('sourceCommit') == trusted.SOURCE and receipt.get('binarySourceCommit') == trusted.SOURCE and
            str(receipt.get('runId')) == str(trusted.RUN) and str(receipt.get('runAttempt')) == str(trusted.ATTEMPT),
            'Published release source/run identity mismatch')


def export(archive, reuse_receipt, compiler_directory, output):
    compiler = Path(compiler_directory)
    regular(compiler, directory=True)
    receipt_path = Path(reuse_receipt)
    require(regular(receipt_path).st_size <= MAX_JSON, 'Reuse receipt exceeds bound')
    require(receipt_path.resolve() == (compiler / 'reuse.json').resolve(), 'Reuse receipt is not the staged controller receipt')
    reuse_data = receipt_path.read_bytes()
    reuse = json_object(reuse_data, 'reuse.json')
    require(reuse.get('schemaVersion') == 1 and reuse.get('prepared') is True and reuse.get('candidateValidated') is False,
            'Strict runtime preparation receipt required')
    require(reuse.get('repository') == trusted.REPOSITORY and reuse.get('sourceWorkflow') == trusted.WORKFLOW and
            reuse.get('originalRunConclusion') == 'success' and reuse.get('sourceCommit') == trusted.SOURCE and
            reuse.get('binarySourceCommit') == trusted.SOURCE and
            str(reuse.get('runId')) == str(reuse.get('sourceRunId')) == str(trusted.RUN) and
            str(reuse.get('runAttempt')) == str(reuse.get('sourceRunAttempt')) == str(trusted.ATTEMPT),
            'Reuse receipt repository/source/run chain mismatch')
    require(re.fullmatch(r'[0-9a-f]{40}', reuse.get('testSourceCommit') or '') and
            reuse.get('candidateCommit') == reuse.get('testSourceCommit'), 'Reuse candidate identity mismatch')
    # This performs the existing controller's live exact repo/source/run/attempt,
    # successful-workflow and artifact-origin checks. A self-reported success in
    # reuse.json is never enough to authorize an export.
    artifact = trusted.accepted_metadata()
    archive_sha = artifact['digest'].split(':', 1)[1]
    require(reuse.get('artifactId') == artifact['id'] and reuse.get('artifactName') == artifact['name'] and
            reuse.get('archiveSha256') == reuse.get('artifactSha256') == archive_sha and
            reuse.get('archiveBytes') == artifact['size_in_bytes'] and 0 < artifact['size_in_bytes'] <= MAX_ARCHIVE,
            'Reuse receipt artifact digest/size identity mismatch')
    staged = reuse.get('files')
    require(isinstance(staged, list) and staged, 'Missing staged runtime inventory')
    staged_hashes = {}
    for item in staged:
        require(isinstance(item, dict) and isinstance(item.get('path'), str), 'Invalid staged runtime record')
        name = safe_name(item['path'])
        require(name == item['path'] and name not in staged_hashes and re.fullmatch(r'[0-9a-f]{64}', item.get('sha256') or ''),
                'Invalid/duplicate staged runtime identity')
        staged_hashes[name] = item['sha256']
    payload, packages = {}, []
    with Path(archive).open('rb') as stream:
        algorithm = hashlib.sha256()
        remaining = artifact['size_in_bytes']
        while remaining:
            block = stream.read(min(1024 * 1024, remaining))
            require(block, 'Original archive truncated')
            algorithm.update(block)
            remaining -= len(block)
        require(not stream.read(1) and algorithm.hexdigest() == archive_sha, 'Original archive SHA256/size mismatch')
        stream.seek(0)
        with zipfile.ZipFile(stream) as outer:
            entries = index_zip(outer)
            release_root = f'artifacts/final-release/{trusted.RUN}-{trusted.ATTEMPT}/'
            release = json_object(read_member(outer, entries, release_root + 'release.json', MAX_JSON), 'release.json')
            source_identity(release)
            require(release.get('succeeded') is True and release.get('status') == 'verified-small-mod-deployment', 'Original release acceptance did not pass')
            configurations = release.get('configurations', [])
            require(isinstance(configurations, list) and len(configurations) == 2 and all(isinstance(row, dict) for row in configurations) and {row.get('configuration') for row in configurations} == set(MODES), 'Original release configuration coverage mismatch')
            for mode in MODES:
                stem = 'NewModCompiler-Win32-' + mode
                zip_name, manifest_name = stem + '.zip', stem + '.manifest.json'
                package = read_member(outer, entries, release_root + zip_name, MAX_PACKAGE)
                manifest_data = read_member(outer, entries, release_root + manifest_name, MAX_JSON)
                manifest = json_object(manifest_data, manifest_name)
                require(manifest.get('schemaVersion') == 1 and manifest.get('status') == 'verified-small-mod-deployment' and
                        manifest.get('platform') == 'Win32' and manifest.get('runtime') == '.NET Framework 4.8' and
                        manifest.get('configuration') == mode and manifest.get('binarySourceCommit') == trusted.SOURCE and
                        manifest.get('packagingSourceCommit') == trusted.SOURCE and
                        str(manifest.get('runId')) == str(trusted.RUN) and str(manifest.get('runAttempt')) == str(trusted.ATTEMPT),
                        'Original package manifest source/configuration mismatch')
                require(manifest.get('archive') == {'name': zip_name, 'bytes': len(package), 'sha256': digest(package)}, 'Original package ZIP identity mismatch')
                accepted = next(row for row in configurations if row['configuration'] == mode)
                require(accepted.get('succeeded') is True and accepted.get('rawOutputEquality') is True and accepted.get('runtimeUnchanged') is True and
                        accepted.get('zipSha256') == digest(package), 'Original package deployment acceptance mismatch')
                acceptance = manifest.get('acceptance', {})
                require(acceptance.get('sourceCommit') == trusted.SOURCE and acceptance.get('binarySourceCommit') == trusted.SOURCE and
                        str(acceptance.get('runId')) == str(trusted.RUN) and str(acceptance.get('runAttempt')) == str(trusted.ATTEMPT),
                        'Package CLI acceptance source/run mismatch')
                validation = manifest.get('validation', {})
                big_deployment = validation.get('bigDeployment')
                require(validation.get('productionDoctor') is True and validation.get('sourceRuntimeRawOutputEquality') is True and
                        isinstance(big_deployment, dict) and big_deployment.get('succeeded') is True, 'Original package deployment gates incomplete')
                prefix = f'artifacts/core-lifecycle-product/{mode}/'
                for member, key in (('build.json', 'buildReceiptSha256'), ('link-inputs.json', 'linkRecipeSha256')):
                    require(digest(read_member(outer, entries, prefix + member, MAX_JSON)) == manifest.get(key), 'Package production build/link receipt mismatch')
                rows = manifest.get('files', [])
                require(isinstance(rows, list) and len(rows) == 5 and all(isinstance(row, dict) for row in rows) and [row.get('name') for row in rows] == list(trusted.RUNTIME), 'Package does not list the exact five runtime files')
                require(accepted.get('runtimeFiles') == rows, 'Published release/runtime manifest inventories differ')
                regular(compiler / mode, directory=True); regular(compiler / mode / 'bin', directory=True)
                require({p.name for p in (compiler / mode / 'bin').iterdir()} == set(trusted.RUNTIME), 'Staged product runtime is not exactly five files')
                with zipfile.ZipFile(io.BytesIO(package)) as inner:
                    inner_entries = index_zip(inner)
                    require(set(inner_entries) == set(trusted.RUNTIME), 'Original release ZIP is not exactly five root files')
                    for row in rows:
                        name = row['name']; data = read_member(inner, inner_entries, name)
                        require(isinstance(row.get('bytes'), int) and row['bytes'] == len(data) and row.get('sha256') == digest(data), 'Original ZIP runtime bytes/hash mismatch')
                        relative = mode + '/bin/' + name
                        require(staged_hashes.get(relative) == digest(data), 'Package differs from strict controller runtime inventory')
                        target = compiler / mode / 'bin' / name; info = regular(target)
                        require(info.st_size == len(data) and info.st_size <= MAX_MEMBER, 'Package/runtime staged file size differs')
                        with target.open('rb') as staged_file:
                            staged_data = staged_file.read(MAX_MEMBER + 1)
                        require(staged_data == data, 'Package/runtime staged bytes differ')
                payload[zip_name] = package; payload[manifest_name] = manifest_data
                packages.append({'configuration': mode, 'archiveMember': release_root + zip_name, 'zip': zip_name,
                                 'zipSha256': digest(package), 'zipBytes': len(package), 'manifest': manifest_name,
                                 'manifestSha256': digest(manifest_data), 'runtimeFiles': rows})
    result = {'schemaVersion': 1, 'exported': True, 'repository': trusted.REPOSITORY, 'sourceWorkflow': trusted.WORKFLOW,
              'binarySourceCommit': trusted.SOURCE, 'packagingSourceCommit': trusted.SOURCE,
              'sourceRunId': str(trusted.RUN), 'sourceRunAttempt': str(trusted.ATTEMPT), 'originalRunConclusion': 'success',
              'testSourceCommit': reuse['testSourceCommit'], 'candidateValidated': False, 'artifactId': artifact['id'],
              'archiveSha256': archive_sha, 'archiveBytes': artifact['size_in_bytes'], 'reuseReceiptSha256': digest(reuse_data),
              'rebuildPerformed': False, 'zipBytesRepacked': False, 'packages': packages,
              'scope': 'Original accepted Debug/Release ZIP and manifest bytes exported unchanged and matched against strict staged production runtimes; no current candidate or new deployment acceptance'}
    destination = Path(output)
    require(not destination.exists() and not destination.is_symlink(), 'Refusing to overwrite export output')
    destination.parent.mkdir(parents=True, exist_ok=True)
    temporary = Path(tempfile.mkdtemp(prefix=destination.name + '-', dir=destination.parent))
    try:
        for name, data in payload.items():
            (temporary / name).write_bytes(data)
        (temporary / 'export-receipt.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
        temporary.rename(destination)
    finally:
        if temporary.exists():
            shutil.rmtree(temporary)
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for option in ('archive', 'reuse-receipt', 'compiler-directory', 'output'):
        parser.add_argument('--' + option, required=True)
    args = parser.parse_args()
    try:
        receipt = export(**vars(args))
        print(json.dumps({'exported': True, 'binarySourceCommit': receipt['binarySourceCommit'], 'sourceRunId': receipt['sourceRunId'], 'packages': len(receipt['packages'])}))
    except (ValueError, OSError, KeyError, TypeError, zipfile.BadZipFile, subprocess.CalledProcessError) as error:
        parser.exit(1, 'Accepted release export rejected: ' + str(error) + '\n')


if __name__ == '__main__':
    main()
