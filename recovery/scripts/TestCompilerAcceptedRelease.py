#!/usr/bin/env python3
"""Actions-only rejection contracts for original accepted-package export.

Small synthetic archives test transport/identity rejection only. No fake payload
is executed or represented as a compiler acceptance result.
"""
import argparse
import copy
import io
import json
import os
from pathlib import Path
import tempfile
import unittest
from unittest import mock
import zipfile

import ExportCompilerAcceptedRelease as exporter


def encoded(value):
    return (json.dumps(value, indent=2) + '\n').encode()


def archive_bytes(files):
    stream = io.BytesIO()
    with zipfile.ZipFile(stream, 'w', zipfile.ZIP_STORED) as archive:
        for name, data in files.items():
            archive.writestr(name, data)
    return stream.getvalue()


class AcceptedReleaseContracts(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix='accepted-release-contract-')
        self.root = Path(self.temporary.name)
        self.compiler = self.root / 'staged'
        self.compiler.mkdir()
        self.archive = self.root / 'accepted.zip'
        self.output = self.root / 'exported'
        self.release_root = f'artifacts/final-release/{exporter.trusted.RUN}-{exporter.trusted.ATTEMPT}/'
        self.members, self.packages, self.manifests, configurations = {}, {}, {}, []
        self.reuse = {'schemaVersion': 1, 'prepared': True, 'candidateValidated': False,
                      'repository': exporter.trusted.REPOSITORY, 'sourceWorkflow': exporter.trusted.WORKFLOW,
                      'originalRunConclusion': 'success', 'sourceCommit': exporter.trusted.SOURCE,
                      'binarySourceCommit': exporter.trusted.SOURCE, 'testSourceCommit': 'a' * 40, 'candidateCommit': 'a' * 40,
                      'runId': str(exporter.trusted.RUN), 'sourceRunId': str(exporter.trusted.RUN),
                      'runAttempt': str(exporter.trusted.ATTEMPT), 'sourceRunAttempt': str(exporter.trusted.ATTEMPT), 'files': []}
        for mode in exporter.MODES:
            bin_directory = self.compiler / mode / 'bin'
            bin_directory.mkdir(parents=True)
            runtime = {}
            for name in exporter.trusted.RUNTIME:
                data = (mode + ':' + name + ':payload').encode()
                runtime[name] = data
                (bin_directory / name).write_bytes(data)
                self.reuse['files'].append({'path': mode + '/bin/' + name, 'sha256': exporter.digest(data)})
            rows = [{'name': name, 'bytes': len(runtime[name]), 'sha256': exporter.digest(runtime[name])} for name in exporter.trusted.RUNTIME]
            package = archive_bytes(runtime)
            self.packages[mode] = package
            stem = 'NewModCompiler-Win32-' + mode
            prefix = 'artifacts/core-lifecycle-product/' + mode + '/'
            build, recipe = encoded({'sourceCommit': exporter.trusted.SOURCE, 'configuration': mode}), encoded({'configuration': mode, 'sourceCommit': exporter.trusted.SOURCE})
            self.members[prefix + 'build.json'] = build
            self.members[prefix + 'link-inputs.json'] = recipe
            self.manifests[mode] = {'schemaVersion': 1, 'status': 'verified-small-mod-deployment', 'platform': 'Win32', 'runtime': '.NET Framework 4.8',
                                    'configuration': mode, 'binarySourceCommit': exporter.trusted.SOURCE, 'packagingSourceCommit': exporter.trusted.SOURCE,
                                    'runId': str(exporter.trusted.RUN), 'runAttempt': str(exporter.trusted.ATTEMPT),
                                    'archive': {'name': stem + '.zip', 'bytes': len(package), 'sha256': exporter.digest(package)},
                                    'files': rows, 'buildReceiptSha256': exporter.digest(build), 'linkRecipeSha256': exporter.digest(recipe),
                                    'acceptance': {'sourceCommit': exporter.trusted.SOURCE, 'binarySourceCommit': exporter.trusted.SOURCE,
                                                   'runId': str(exporter.trusted.RUN), 'runAttempt': str(exporter.trusted.ATTEMPT)},
                                    'validation': {'productionDoctor': True, 'sourceRuntimeRawOutputEquality': True, 'bigDeployment': {'succeeded': True}}}
            configurations.append({'configuration': mode, 'succeeded': True, 'rawOutputEquality': True, 'runtimeUnchanged': True,
                                   'zipSha256': exporter.digest(package), 'runtimeFiles': copy.deepcopy(rows)})
        self.release = {'sourceCommit': exporter.trusted.SOURCE, 'binarySourceCommit': exporter.trusted.SOURCE,
                        'runId': str(exporter.trusted.RUN), 'runAttempt': str(exporter.trusted.ATTEMPT),
                        'succeeded': True, 'status': 'verified-small-mod-deployment', 'configurations': configurations}
        self.refresh()
        self.metadata = mock.patch.object(exporter.trusted, 'accepted_metadata', side_effect=lambda: dict(self.artifact))
        self.metadata.start()

    def tearDown(self):
        self.metadata.stop()
        self.temporary.cleanup()

    def refresh(self, extra=None):
        for mode in exporter.MODES:
            stem = 'NewModCompiler-Win32-' + mode
            self.members[self.release_root + stem + '.zip'] = self.packages[mode]
            self.members[self.release_root + stem + '.manifest.json'] = encoded(self.manifests[mode])
        self.members[self.release_root + 'release.json'] = encoded(self.release)
        members = dict(self.members)
        if extra:
            members.update(extra)
        data = archive_bytes(members)
        self.archive.write_bytes(data)
        self.artifact = {'id': 123, 'name': f'refactor-core-lifecycle-{exporter.trusted.RUN}-{exporter.trusted.ATTEMPT}',
                         'digest': 'sha256:' + exporter.digest(data), 'size_in_bytes': len(data)}
        self.reuse.update({'artifactId': self.artifact['id'], 'artifactName': self.artifact['name'],
                           'archiveSha256': exporter.digest(data), 'artifactSha256': exporter.digest(data), 'archiveBytes': len(data)})
        self.save_reuse()

    def save_reuse(self):
        (self.compiler / 'reuse.json').write_bytes(encoded(self.reuse))

    def run_export(self):
        return exporter.export(self.archive, self.compiler / 'reuse.json', self.compiler, self.output)

    def rejects(self):
        with self.assertRaises((ValueError, OSError, zipfile.BadZipFile)):
            self.run_export()
        self.assertFalse(self.output.exists())

    def changed_package(self, data):
        mode = 'Debug'
        self.packages[mode] = data
        self.manifests[mode]['archive'].update({'bytes': len(data), 'sha256': exporter.digest(data)})
        self.release['configurations'][0]['zipSha256'] = exporter.digest(data)
        self.refresh()

    def test_export_preserves_original_zip_and_manifest_bytes(self):
        receipt = self.run_export()
        self.assertEqual(len(receipt['packages']), 2)
        self.assertFalse(receipt['rebuildPerformed'])
        self.assertFalse(receipt['zipBytesRepacked'])
        self.assertFalse(receipt['candidateValidated'])
        for mode in exporter.MODES:
            stem = 'NewModCompiler-Win32-' + mode
            self.assertEqual((self.output / (stem + '.zip')).read_bytes(), self.packages[mode])
            self.assertEqual((self.output / (stem + '.manifest.json')).read_bytes(), self.members[self.release_root + stem + '.manifest.json'])
        self.assertEqual({p.name for p in self.output.iterdir()}, {'export-receipt.json', *('NewModCompiler-Win32-' + mode + suffix for mode in exporter.MODES for suffix in ('.zip', '.manifest.json'))})

    def test_live_acceptance_gate_is_required(self):
        with mock.patch.object(exporter.trusted, 'accepted_metadata', side_effect=ValueError('full workflow still running')):
            self.rejects()

    def test_reuse_source_repository_run_attempt_and_size_are_exact(self):
        original = copy.deepcopy(self.reuse)
        for key, bad in (('prepared', False), ('candidateValidated', True), ('repository', 'other/NewSdk'),
                         ('sourceWorkflow', '.github/workflows/other.yml'), ('sourceCommit', 'b' * 40),
                         ('sourceRunId', '0'), ('runAttempt', '0'), ('archiveBytes', self.reuse['archiveBytes'] + 1),
                         ('originalRunConclusion', 'in_progress'), ('archiveSha256', '0' * 64)):
            with self.subTest(key=key):
                self.reuse = copy.deepcopy(original)
                self.reuse[key] = bad
                self.save_reuse()
                self.rejects()

    def test_downloaded_archive_digest_and_size_are_rechecked(self):
        self.archive.write_bytes(self.archive.read_bytes() + b'tampered')
        self.rejects()

    def test_original_release_success_and_both_configurations_are_required(self):
        self.release['succeeded'] = False
        self.refresh()
        self.rejects()
        self.release['succeeded'] = True
        self.release['configurations'] = self.release['configurations'][:1]
        self.refresh()
        self.rejects()

    def test_original_binary_packaging_and_cli_acceptance_identity_are_required(self):
        for key in ('binarySourceCommit', 'packagingSourceCommit', 'runId', 'runAttempt'):
            with self.subTest(key=key):
                original = self.manifests['Debug'][key]
                self.manifests['Debug'][key] = '0'
                self.refresh()
                self.rejects()
                self.manifests['Debug'][key] = original
        self.manifests['Debug']['acceptance']['binarySourceCommit'] = 'b' * 40
        self.refresh()
        self.rejects()

    def test_missing_published_package_or_changed_build_receipt_is_rejected(self):
        self.refresh()
        key = self.release_root + 'NewModCompiler-Win32-Debug.zip'
        data = self.members.pop(key)
        self.archive.write_bytes(archive_bytes(self.members))
        self.artifact.update({'digest': 'sha256:' + exporter.digest(self.archive.read_bytes()), 'size_in_bytes': self.archive.stat().st_size})
        self.reuse.update({'archiveSha256': self.artifact['digest'][7:], 'artifactSha256': self.artifact['digest'][7:], 'archiveBytes': self.artifact['size_in_bytes']})
        self.save_reuse()
        self.rejects()
        self.members[key] = data
        self.members['artifacts/core-lifecycle-product/Debug/build.json'] = b'changed'
        self.refresh()
        self.rejects()

    def test_staged_runtime_tamper_or_extra_file_is_rejected(self):
        path = self.compiler / 'Debug' / 'bin' / exporter.trusted.RUNTIME[0]
        original = path.read_bytes()
        path.write_bytes(b'changed')
        self.rejects()
        path.write_bytes(original)
        (path.parent / 'extra.dll').write_bytes(b'extra')
        self.rejects()

    def test_runtime_symlink_is_rejected(self):
        path = self.compiler / 'Debug' / 'bin' / exporter.trusted.RUNTIME[0]
        target = self.root / 'symlink-target'
        target.write_bytes(path.read_bytes())
        path.unlink()
        try:
            path.symlink_to(target)
        except OSError as error:
            self.skipTest('Executor cannot create symlinks: ' + str(error))
        self.rejects()

    def test_nested_zip_traversal_duplicate_case_and_extra_file_are_rejected(self):
        with zipfile.ZipFile(io.BytesIO(self.packages['Debug'])) as archive:
            files = {name: archive.read(name) for name in archive.namelist()}
        for name in ('../escape', 'newmodcompiler.exe', 'extra.dll'):
            with self.subTest(name=name):
                extra = dict(files)
                extra[name] = b'bad'
                self.changed_package(archive_bytes(extra))
                self.rejects()

    def test_nested_zip_crc_corruption_is_rejected_even_with_updated_outer_digest(self):
        package = self.packages['Debug']
        original = b'Debug:NewModCompiler.exe:payload'
        self.assertIn(original, package)
        self.changed_package(package.replace(original, b'X' + original[1:], 1))
        self.rejects()

    def test_zip_symlink_and_oversize_package_are_rejected(self):
        stream = io.BytesIO()
        with zipfile.ZipFile(stream, 'w') as archive:
            entry = zipfile.ZipInfo('NewModCompiler.exe')
            entry.create_system = 3
            entry.external_attr = 0o120777 << 16
            archive.writestr(entry, 'target')
        self.changed_package(stream.getvalue())
        self.rejects()
        with mock.patch.object(exporter, 'MAX_PACKAGE', 1):
            self.rejects()

    def test_outer_zip_unsafe_name_is_rejected(self):
        self.refresh({'../escape': b'bad'})
        self.rejects()

    def test_existing_output_is_not_replaced(self):
        self.output.mkdir()
        sentinel = self.output / 'keep'
        sentinel.write_bytes(b'untouched')
        with self.assertRaises(ValueError):
            self.run_export()
        self.assertEqual(sentinel.read_bytes(), b'untouched')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output')
    args = parser.parse_args()
    suite = unittest.defaultTestLoader.loadTestsFromTestCase(AcceptedReleaseContracts)
    result = unittest.TextTestRunner(verbosity=2).run(suite)
    report = {'succeeded': result.wasSuccessful(), 'sourceCommit': os.environ.get('GITHUB_SHA'),
              'runId': os.environ.get('GITHUB_RUN_ID'), 'runAttempt': os.environ.get('GITHUB_RUN_ATTEMPT'),
              'testsRun': result.testsRun, 'failures': [(str(test), error) for test, error in result.failures],
              'errors': [(str(test), error) for test, error in result.errors],
              'scope': 'Small synthetic package transport and rejection contracts only; no runtime execution or compiler acceptance'}
    if args.output:
        output = Path(args.output)
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    if result.wasSuccessful():
        print('PASS AcceptedReleaseExport: ' + str(result.testsRun) + ' package transport contracts')
    return 0 if result.wasSuccessful() else 1


if __name__ == '__main__':
    raise SystemExit(main())
