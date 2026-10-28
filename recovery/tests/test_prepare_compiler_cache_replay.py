"""Rejection contracts for test-only replay of successful cache-parallel builds.

All Git repositories and ZIP members below are synthetic, isolated fixtures.
Production source closure remains real Git ancestry/diff validation in these tests.
"""
import copy
import importlib.util
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
import warnings
import zipfile
from unittest.mock import patch

SCRIPTS = Path(__file__).resolve().parents[1] / 'scripts'
sys.path.insert(0, str(SCRIPTS))
spec = importlib.util.spec_from_file_location('cache_replay', SCRIPTS / 'PrepareCompilerCacheReplay.py')
replay = importlib.util.module_from_spec(spec)
spec.loader.exec_module(replay)


class GitFixture(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.repo = self.root / 'repo'
        self.repo.mkdir()
        self.git('init', '-q')
        self.git('config', 'user.email', 'fixture@example.invalid')
        self.git('config', 'user.name', 'Fixture')
        for path in ('recovery/src/App/Application/PreparedBackend.cs',
                     'recovery/src/App/Application/CacheInputLeaseSet.cs',
                     'recovery/src/App/Application/CacheInputSnapshot.cs',
                     'recovery/src/App/Application/CoreStreamCompiler.cs',
                     'recovery/src/App/Application/BackendResourcePolicy.cs',
                     'SDK/Schemas/xsd/input.xsd', 'SDK/tools/BinaryAssetBuilder.exe.config',
                     'recovery/build/BuildCompiler.ps1', '.github/workflows/refactor-core-lifecycle.yml'):
            target = self.repo / path
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text('original production closure\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'production')
        self.source = self.git('rev-parse', 'HEAD').strip()
        self.commit('recovery/scripts/TestCompilerBuildCachePayloads.ps1', 'candidate payload assertions\n')
        self.candidate = self.git('rev-parse', 'HEAD').strip()

    def git(self, *args):
        return subprocess.check_output(['git', '-C', str(self.repo), *args], text=True,
                                       stderr=subprocess.DEVNULL)

    def commit(self, path, text):
        target = self.repo / path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text)
        self.git('add', '.')
        self.git('commit', '-qm', 'fixture')


class ClosureTests(GitFixture):
    def test_upheaval_tools_are_exact_and_purpose_scoped(self):
        for path in sorted(replay.UPHEAVAL_MUTABLE):
            self.git('reset', '--hard', self.candidate)
            self.commit(path, 'reviewed isolated benchmark tool\n')
            candidate = self.git('rev-parse', 'HEAD').strip()
            with self.subTest(path=path):
                self.assertIn(path, replay.closure(self.repo, self.source, candidate, 'upheaval-metrics'))
                with self.assertRaises(ValueError):
                    replay.closure(self.repo, self.source, candidate)
        for path in ('recovery/scripts/UnreviewedUpheaval.py', 'SDK/Mods/Upheaval/Data/Mod.xml',
                     '.github/workflows/refactor-core-lifecycle.yml'):
            self.git('reset', '--hard', self.candidate)
            self.commit(path, 'unreviewed input\n')
            with self.subTest(path=path), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, self.git('rev-parse', 'HEAD').strip(), 'upheaval-metrics')

    def test_unknown_purpose_and_nonancestor_upheaval_rejected(self):
        for purpose in ('', None, 'release', 'Upheaval', '../upheaval-metrics'):
            with self.subTest(purpose=purpose), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, self.candidate, purpose)
        with self.assertRaises(subprocess.CalledProcessError):
            replay.closure(self.repo, self.candidate, self.source, 'upheaval-metrics')

    def test_upheaval_tool_identity_requires_committed_exact_bytes(self):
        for path in replay.UPHEAVAL_SCRIPTS:
            self.commit(path, 'reviewed benchmark script\n')
        candidate = self.git('rev-parse', 'HEAD').strip()
        rows = replay.measurement_scripts(self.repo, candidate, 'upheaval-metrics')
        self.assertEqual([row['path'] for row in rows], list(replay.UPHEAVAL_SCRIPTS))
        for row in rows:
            self.assertEqual(row['sha256'], replay.digest((self.repo / row['path']).read_bytes()))
        (self.repo / replay.UPHEAVAL_SCRIPTS[0]).write_bytes(b'tampered uncommitted benchmark')
        with self.assertRaises(ValueError):
            replay.measurement_scripts(self.repo, candidate, 'upheaval-metrics')
        with self.assertRaises(subprocess.CalledProcessError):
            replay.measurement_scripts(self.repo, self.candidate, 'upheaval-metrics')

    def test_only_candidate_payload_changes_accepted(self):
        self.assertEqual(replay.closure(self.repo, self.source, self.candidate),
                         ['recovery/scripts/TestCompilerBuildCachePayloads.ps1'])

    def test_nonancestor_rejected(self):
        with self.assertRaises((ValueError, subprocess.CalledProcessError)):
            replay.closure(self.repo, self.candidate, self.source)

    def test_full_lowercase_real_commit_ids_required(self):
        for value in (self.source[:8], self.source.upper(), '0' * 40, '../HEAD', 'HEAD'):
            for source, candidate in ((value, self.candidate), (self.source, value)):
                with self.subTest(value=value, source=source), \
                     self.assertRaises((ValueError, subprocess.CalledProcessError)):
                    replay.closure(self.repo, source, candidate)

    def test_production_sdk_recipes_and_unreviewed_tools_rejected(self):
        for path in ('recovery/src/App/Application/PreparedBackend.cs',
                     'recovery/src/App/Application/CacheInputLeaseSet.cs',
                     'recovery/src/App/Application/CacheInputSnapshot.cs',
                     'recovery/src/App/Application/CoreStreamCompiler.cs',
                     'recovery/src/App/Application/BackendResourcePolicy.cs',
                     'SDK/Schemas/xsd/input.xsd', 'SDK/tools/BinaryAssetBuilder.exe.config',
                     'recovery/build/BuildCompiler.ps1', 'recovery/scripts/BuildCompilerApp.ps1',
                     '.github/workflows/refactor-core-lifecycle.yml',
                     'recovery/tests/UnreviewedCompilerFixture.cs', 'new-production-input.txt'):
            # Reset only this test's TemporaryDirectory repository. Each case
            # independently differs from the same known production source.
            self.git('reset', '--hard', self.candidate)
            self.commit(path, 'changed production or unreviewed tool\n')
            changed = self.git('rev-parse', 'HEAD').strip()
            with self.subTest(path=path), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, changed)

    def test_new_production_sources_are_immutable_without_a_maintained_source_list(self):
        self.commit('recovery/src/App/Application/FutureDigestProvider.cs', 'new production input\n')
        with self.assertRaisesRegex(ValueError, 'Non-replay input changed'):
            replay.closure(self.repo, self.source, self.git('rev-parse', 'HEAD').strip())

    def test_a_new_successful_binary_baseline_includes_its_added_production_sources(self):
        # A later successful build is selected explicitly; the replay is not
        # constrained to the first historical cache-parallel source revision.
        self.commit('recovery/src/App/Application/FutureDigestProvider.cs', 'new accepted production input\n')
        later_source = self.git('rev-parse', 'HEAD').strip()
        self.commit('recovery/scripts/TestCompilerBuildCachePayloads.ps1', 'later payload assertions\n')
        later_tests = self.git('rev-parse', 'HEAD').strip()
        self.assertEqual(replay.closure(self.repo, later_source, later_tests),
                         ['recovery/scripts/TestCompilerBuildCachePayloads.ps1'])
        self.commit('recovery/src/App/Application/FutureDigestProvider.cs', 'changed after accepted binary\n')
        with self.assertRaisesRegex(ValueError, 'Non-replay input changed'):
            replay.closure(self.repo, later_source, self.git('rev-parse', 'HEAD').strip())

    def test_deletion_and_rename_rejected(self):
        for path in ('recovery/src/App/Application/PreparedBackend.cs',
                     'recovery/src/App/Application/CacheInputLeaseSet.cs'):
            self.git('reset', '--hard', self.candidate)
            self.git('rm', path)
            self.git('commit', '-qm', 'deleted source')
            with self.subTest(deleted=path), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, self.git('rev-parse', 'HEAD').strip())
            self.git('reset', '--hard', self.candidate)
            self.git('mv', path, 'recovery/src/App/Application/RenamedBackend.cs')
            self.git('commit', '-qm', 'renamed source')
            with self.subTest(renamed=path), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, self.git('rev-parse', 'HEAD').strip())


    def test_dirty_tracked_bytes_are_not_confused_with_candidate_objects(self):
        path = self.repo / 'SDK/Schemas/xsd/input.xsd'
        path.write_text('uncommitted schema mutation')
        with self.assertRaises(subprocess.CalledProcessError):
            replay.closure(self.repo, self.source, self.candidate)

    def test_untracked_production_and_sdk_inputs_rejected(self):
        for folder in ('recovery/src', 'recovery/build', 'recovery/catalog', 'SDK'):
            path = self.repo / folder / 'untracked-input'
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_bytes(b'untracked production input')
            with self.subTest(folder=folder), self.assertRaises(ValueError):
                replay.closure(self.repo, self.source, self.candidate)
            path.unlink()


class ProfileTests(unittest.TestCase):
    def test_full_sha_and_positive_integer_identity_required(self):
        valid = dict(binarySourceCommit='a' * 40, archiveSha256='b' * 64,
                     runId=123, runAttempt=1, artifactId=456)
        replay.profile_identity(valid)
        for key in ('binarySourceCommit', 'archiveSha256'):
            for value in ('short', None, 'A' * len(valid[key]), ''):
                with self.subTest(key=key, value=value), self.assertRaises((ValueError, TypeError)):
                    replay.profile_identity(dict(valid, **{key: value}))
        for key in ('runId', 'runAttempt', 'artifactId'):
            for value in (0, -1, '1', None, True, 1.0):
                with self.subTest(key=key, value=value), self.assertRaises(ValueError):
                    replay.profile_identity(dict(valid, **{key: value}))


class UpheavalEventTests(unittest.TestCase):
    def setUp(self):
        self.profile = dict(source_commit='a' * 40, run_id='123', run_attempt='1',
                            artifact_id=456, archive_sha256='b' * 64)

    def resolve(self, profile=None, marker='[upheaval-preflight]', reference=None):
        message = marker + '\n\nUpheavalProfile: ' + json.dumps(self.profile if profile is None else profile)
        return replay.resolve_upheaval_profile(message, replay.UPHEAVAL_REF if reference is None else reference)

    def test_ordinary_push_never_runs_measurement(self):
        self.assertEqual(replay.resolve_upheaval_profile('ordinary checkpoint', replay.UPHEAVAL_REF), {'run_measure': 'false'})

    def test_marked_profile_selects_preflight_or_one_measurement(self):
        row = self.resolve()
        self.assertEqual(row['run_measure'], 'true')
        self.assertEqual(row['preflight_only'], 'true')
        self.assertEqual(row['artifact_id'], '456')
        self.assertEqual(self.resolve(marker='[upheaval-measure-once]')['preflight_only'], 'false')

    def test_ambiguous_marker_missing_duplicate_profile_or_wrong_branch_rejected(self):
        for marker in ('[upheaval-preflight] [upheaval-measure-once]', '[upheaval-preflight] [upheaval-preflight]'):
            with self.subTest(marker=marker), self.assertRaises(ValueError): self.resolve(marker=marker)
        for message in ('[upheaval-preflight]', '[upheaval-preflight]\nUpheavalProfile: {}\nUpheavalProfile: invalid',
                        '[upheaval-preflight]\nUpheavalProfile: {"run_id":1,"run_id":2}'):
            with self.subTest(message=message), self.assertRaises(ValueError):
                replay.resolve_upheaval_profile(message, replay.UPHEAVAL_REF)
        with self.assertRaises(ValueError): self.resolve(reference='refs/heads/compiler-main')

    def test_profile_has_only_exact_hex_and_positive_decimal_fields(self):
        for key, values in (('source_commit', ['HEAD', 'A' * 40, 'a' * 39, None, 123]),
                            ('archive_sha256', ['B' * 64, 'b' * 63, '../archive', None]),
                            ('run_id', [0, -1, True, 1.0, '01', '1\nother=value', '$(exit)'])):
            for value in values:
                with self.subTest(key=key, value=value), self.assertRaises(ValueError):
                    self.resolve(dict(self.profile, **{key: value}))
        for profile in (dict(self.profile, extra='unreviewed'), {k: v for k, v in self.profile.items() if k != 'run_id'}, []):
            with self.subTest(profile=profile), self.assertRaises(ValueError): self.resolve(profile)

    def test_dispatch_requires_explicit_boolean_and_same_profile_rules(self):
        dispatch = dict(self.profile, preflight_only='true')
        self.assertEqual(replay.resolve_upheaval_profile('', replay.UPHEAVAL_REF, dispatch)['preflight_only'], 'true')
        for value in (True, None, 'yes', ''):
            with self.subTest(value=value), self.assertRaises(ValueError):
                replay.resolve_upheaval_profile('', replay.UPHEAVAL_REF, dict(dispatch, preflight_only=value))
class MetadataTests(unittest.TestCase):
    def setUp(self):
        self.profile = dict(binarySourceCommit='a' * 40, runId=123, runAttempt=1,
                            artifactId=456, archiveSha256='b' * 64)
        self.run = dict(id=123, head_sha='a' * 40, run_attempt=1,
                        status='completed', conclusion='success',
                        path='.github/workflows/refactor-core-lifecycle.yml',
                        head_branch='feature/cache-parallel',
                        repository=dict(id=7, full_name='wu162/NewSdk'),
                        head_repository=dict(id=7, full_name='wu162/NewSdk'))
        self.artifact = dict(id=456, name='refactor-core-lifecycle-123-1', expired=False,
                             digest='sha256:' + 'b' * 64,
                             workflow_run=dict(id=123, head_sha='a' * 40,
                                               head_branch='feature/cache-parallel',
                                               repository_id=7, head_repository_id=7))
        self.jobs = dict(total_count=1, jobs=[dict(run_id=123, status='completed',
                         conclusion='success', steps=[dict(name=name, status='completed',
                         conclusion='success') for name in sorted(replay.REQUIRED_STEPS)])])

    def verify(self, run=None, artifact=None, jobs=None, profile=None):
        return replay.metadata(self.run if run is None else run,
                               self.artifact if artifact is None else artifact,
                               self.jobs if jobs is None else jobs,
                               self.profile if profile is None else profile)

    def test_complete_successful_origin_accepted(self):
        self.verify()

    def test_run_identity_attempt_branch_and_success_rejected(self):
        for key, value in (('id', 0), ('head_sha', '0' * 40), ('run_attempt', 2),
                           ('status', 'in_progress'), ('conclusion', 'failure'),
                           ('conclusion', 'cancelled'), ('path', '.github/workflows/other.yml'),
                           ('head_branch', 'other')):
            with self.subTest(key=key, value=value), self.assertRaises(ValueError):
                self.verify(run=dict(self.run, **{key: value}))

    def test_fork_or_wrong_repository_rejected(self):
        for key in ('repository', 'head_repository'):
            run = copy.deepcopy(self.run)
            run[key]['full_name'] = 'other/NewSdk'
            with self.subTest(key=key), self.assertRaises(ValueError):
                self.verify(run=run)

    def test_artifact_identity_expiry_and_digest_rejected(self):
        for key, value in (('id', 999), ('name', 'different-artifact'), ('expired', True),
                           ('digest', 'sha256:' + '0' * 64), ('digest', None)):
            with self.subTest(key=key), self.assertRaises(ValueError):
                self.verify(artifact=dict(self.artifact, **{key: value}))
        for key, value in (('id', 999), ('head_sha', '0' * 40), ('head_branch', 'other'),
                           ('repository_id', 9), ('head_repository_id', 9)):
            artifact = copy.deepcopy(self.artifact)
            artifact['workflow_run'][key] = value
            with self.subTest(origin=key), self.assertRaises(ValueError):
                self.verify(artifact=artifact)

    def test_incomplete_paginated_duplicate_or_failed_jobs_rejected(self):
        for jobs in (dict(total_count=2, jobs=self.jobs['jobs']), dict(total_count=0, jobs=[]),
                     dict(total_count=2, jobs=self.jobs['jobs'] * 2)):
            with self.subTest(jobs=jobs), self.assertRaises(ValueError):
                self.verify(jobs=jobs)
        for key, value in (('run_id', 999), ('status', 'in_progress'), ('conclusion', 'failure')):
            jobs = copy.deepcopy(self.jobs)
            jobs['jobs'][0][key] = value
            with self.subTest(key=key), self.assertRaises(ValueError):
                self.verify(jobs=jobs)

    def test_each_required_acceptance_step_must_be_completed_success(self):
        for name in replay.REQUIRED_STEPS:
            for failure in ('missing', 'skipped', 'failure', 'cancelled', 'in_progress'):
                jobs = copy.deepcopy(self.jobs)
                steps = jobs['jobs'][0]['steps']
                step = next(row for row in steps if row['name'] == name)
                if failure == 'missing':
                    steps.remove(step)
                elif failure == 'in_progress':
                    step['status'] = failure
                else:
                    step['conclusion'] = failure
                with self.subTest(name=name, failure=failure), self.assertRaises(ValueError):
                    self.verify(jobs=jobs)

    def test_duplicate_step_names_rejected(self):
        jobs = copy.deepcopy(self.jobs)
        jobs['jobs'][0]['steps'].append(dict(jobs['jobs'][0]['steps'][0]))
        with self.assertRaises(ValueError):
            self.verify(jobs=jobs)


class ZipSafetyTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.archive = Path(self.temp.name) / 'archive.zip'

    def archive_bytes(self, entries, raw_names=()):
        with warnings.catch_warnings():
            warnings.simplefilter('ignore', UserWarning)
            with zipfile.ZipFile(self.archive, 'w') as archive:
                for name, data in entries:
                    archive.writestr(name, data)
        raw = self.archive.read_bytes()
        for placeholder, malicious in raw_names:
            placeholder, malicious = placeholder.encode('ascii'), malicious.encode('ascii')
            self.assertEqual(len(placeholder), len(malicious))
            self.assertEqual(raw.count(placeholder), 2)
            raw = raw.replace(placeholder, malicious)
        self.archive.write_bytes(raw)

    def validate(self):
        with zipfile.ZipFile(self.archive) as archive:
            return replay.safe_zip(archive)

    def test_regular_files_and_directories_accepted(self):
        self.archive_bytes([('Debug/bin/', b''), ('Debug/bin/NewModCompiler.exe', b'opaque binary')])
        self.validate()

    def test_unsafe_raw_zip_names_rejected(self):
        for name in ('../escape', '/absolute', 'C:/escape', 'a\\escape', 'a/../escape',
                     'a//b', './file', 'safe\0hidden', '//host/share', 'C:relative'):
            placeholder = 'Q' * len(name)
            self.archive_bytes([(placeholder, b'opaque')], [(placeholder, name)])
            with self.subTest(name=name), self.assertRaises(ValueError):
                self.validate()

    def test_duplicate_casefold_and_file_directory_collisions_rejected(self):
        for left, right in (('Debug/bin/runtime', 'Debug/bin/runtime'),
                            ('Debug/bin/runtime', 'DEBUG/BIN/RUNTIME'), ('Debug/bin/', 'Debug/bin')):
            placeholder = 'Q' * len(right)
            self.archive_bytes([(left, b'first'), (placeholder, b'second')], [(placeholder, right)])
            with self.subTest(left=left, right=right), self.assertRaises(ValueError):
                self.validate()

    def test_symlink_rejected(self):
        link = zipfile.ZipInfo('Debug/bin/runtime')
        link.create_system = 3
        link.external_attr = 0o120777 << 16
        self.archive_bytes([(link, b'../../target')])
        with self.assertRaises(ValueError):
            self.validate()

    def test_encrypted_flag_rejected_before_any_member_read(self):
        self.archive_bytes([('Debug/bin/runtime', b'opaque')])
        raw = bytearray(self.archive.read_bytes())
        # Set flags in the real local and central headers. writestr otherwise
        # clears the encrypted bit and would never exercise the actual guard.
        local, central = raw.index(b'PK\x03\x04'), raw.index(b'PK\x01\x02')
        raw[local + 6] |= 1
        raw[central + 8] |= 1
        self.archive.write_bytes(raw)
        with self.assertRaises(ValueError):
            self.validate()


class ArchiveTests(GitFixture):
    def setUp(self):
        super().setUp()
        self.profile = dict(binarySourceCommit=self.source, runId=123, runAttempt=1,
                            artifactId=456, archiveSha256='0' * 64)
        self.archive = self.root / 'production.zip'
        self.output = self.root / 'prepared'
        self.entries = self.artifact()

    def artifact(self):
        entries = {}

        def put(path, value):
            entries[path] = json.dumps(value).encode()
            return replay.digest(entries[path])

        common = dict(sourceCommit=self.source, runId='123', runAttempt='1')
        receipts = {scope: dict(common, succeeded=True, status='passed', modes=[])
                    for scope in ('core-lifecycle-deployment', 'core-lifecycle', 'cache-payloads', 'core-cli')}
        release_prefix = 'artifacts/final-release/123-1/'
        release = dict(common, binarySourceCommit=self.source, succeeded=True,
                       status='verified-small-mod-deployment', configurations=[])
        for mode in ('Debug', 'Release'):
            prefix = 'artifacts/core-lifecycle-product/' + mode + '/'
            runtime = {name: (mode + ':' + name).encode() for name in replay.FIVE_RUNTIME}
            exe_sha = replay.digest(runtime['NewModCompiler.exe'])
            toolchain = dict(compiler='Microsoft.Net.Compilers.Toolset/4.0.1',
                             vcToolsVersion='14.44.fixture', windowsSdkVersion='10.0.fixture')
            build = dict(sourceCommit=self.source, configuration=mode, schemaVersion=2,
                         runtime='net48/Win32', containsProbeOrTestSources=False,
                         backendIntegrated=True, entryPoint='NewModCompiler.Hosting.CompilerEntry.Main',
                         runtimeFiles=list(replay.FIVE_RUNTIME), exeSha256=exe_sha, **toolchain)
            link = dict(sourceCommit=self.source, configuration=mode,
                        arguments=['/target:exe', '/platform:x86'],
                        inputs=[dict(path='production-input.obj', sha256='c' * 64)])
            build_sha, link_sha = put(prefix + 'build.json', build), put(prefix + 'link-inputs.json', link)
            inventory = []
            for name, data in runtime.items():
                entries[prefix + 'bin/' + name] = data
                inventory.append(dict(name=name, bytes=len(data), sha256=replay.digest(data)))
            put(release_prefix + 'NewModCompiler-Win32-' + mode + '.manifest.json',
                dict(binarySourceCommit=self.source, packagingSourceCommit=self.source,
                     runId='123', runAttempt='1', configuration=mode, toolchain=toolchain,
                     buildReceiptSha256=build_sha, linkRecipeSha256=link_sha, files=inventory))
            for scope, receipt in receipts.items():
                row = dict(configuration=mode, passed=True, exeSha256=exe_sha,
                           productExeSha256=exe_sha)
                if scope == 'core-cli':
                    row['runtimeInputs'] = {'\\' + name: replay.digest(runtime[name]) for name in replay.RUNTIME}
                receipt['modes'].append(row)
            release['configurations'].append(dict(configuration=mode, succeeded=True))
        for scope, receipt in receipts.items():
            put('evidence/refactor/' + scope + '/123-1/validation.json', receipt)
        put(release_prefix + 'release.json', release)
        return entries

    def change_json(self, path, mutation):
        value = json.loads(self.entries[path])
        mutation(value)
        self.entries[path] = json.dumps(value).encode()

    def write_archive(self):
        with zipfile.ZipFile(self.archive, 'w') as archive:
            for name, data in self.entries.items():
                archive.writestr(name, data)
        self.profile['archiveSha256'] = replay.digest(self.archive.read_bytes())

    def prepare(self, purpose='payload'):
        self.write_archive()
        return replay.prepare(self.archive, self.profile, self.candidate, self.output, self.repo, purpose)

    def upheaval_candidate(self):
        for path in sorted(replay.UPHEAVAL_MUTABLE):
            self.commit(path, 'reviewed isolated benchmark tool\n')
        self.candidate = self.git('rev-parse', 'HEAD').strip()

    def test_upheaval_receipt_binds_exact_scripts_and_rechecks_original_archive(self):
        self.upheaval_candidate()
        receipt = self.prepare('upheaval-metrics')
        self.assertEqual(receipt['purpose'], 'upheaval-metrics')
        self.assertFalse(receipt['candidateValidated'])
        self.assertEqual(receipt['measurementScripts'], replay.measurement_scripts(self.repo, self.candidate, 'upheaval-metrics'))
        expected = [mode + '/bin/' + name for mode in ('Debug', 'Release') for name in replay.FIVE_RUNTIME]
        self.assertEqual(sorted(row['path'] for row in receipt['files']), sorted(expected))
        for row in receipt['files']:
            self.assertEqual((self.output / row['path']).read_bytes(), self.entries['artifacts/core-lifecycle-product/' + row['path']])
        path = self.output / 'cache-payload-replay.json'
        with patch.object(replay, 'check_api') as api:
            self.assertEqual(replay.verify_prepared(path, self.candidate, self.repo, 'upheaval-metrics'), receipt)
        api.assert_called_once_with(self.profile)
        with patch.object(replay, 'check_api'), self.assertRaisesRegex(ValueError, 'purpose mismatch'):
            replay.verify_prepared(path, self.candidate, self.repo)
        for mutate in (lambda row: row.update(purpose='payload'),
                       lambda row: row['measurementScripts'][0].update(sha256='0' * 64),
                       lambda row: row['measurementScripts'].pop(),
                       lambda row: row['measurementScripts'][0].update(path='recovery/scripts/Other.ps1')):
            changed = copy.deepcopy(receipt); mutate(changed)
            path.write_text(json.dumps(changed))
            with self.subTest(receipt=changed), patch.object(replay, 'check_api'), self.assertRaises(ValueError):
                replay.verify_prepared(path, self.candidate, self.repo, 'upheaval-metrics')
        path.write_text(json.dumps(receipt))
        (self.output / 'Release/bin/NewModCompiler.exe').write_bytes(b'tampered benchmark runtime')
        with patch.object(replay, 'check_api'), self.assertRaises(ValueError):
            replay.verify_prepared(path, self.candidate, self.repo, 'upheaval-metrics')

    def test_upheaval_still_rejects_bad_digest_or_incomplete_production_acceptance(self):
        self.upheaval_candidate()
        self.write_archive()
        self.profile['archiveSha256'] = '0' * 64
        with self.assertRaisesRegex(ValueError, 'SHA256 mismatch'):
            replay.prepare(self.archive, self.profile, self.candidate, self.output, self.repo, 'upheaval-metrics')
        self.change_json('evidence/refactor/cache-payloads/123-1/validation.json', lambda row: row.update(succeeded=False))
        with self.assertRaises(ValueError):
            self.prepare('upheaval-metrics')
        self.assertFalse(self.output.exists())

    def test_valid_archive_extracts_exactly_six_files_and_separate_sources(self):
        receipt = self.prepare()
        self.assertTrue(receipt['prepared'])
        self.assertFalse(receipt['candidateValidated'])
        self.assertTrue(receipt['productionSourcesUnchanged'])
        self.assertEqual(receipt['binarySourceCommit'], self.source)
        self.assertEqual(receipt['testSourceCommit'], self.candidate)
        self.assertEqual(receipt['changedPaths'], ['recovery/scripts/TestCompilerBuildCachePayloads.ps1'])
        expected = [mode + '/bin/' + name for mode in ('Debug', 'Release') for name in replay.RUNTIME]
        actual = [path.relative_to(self.output).as_posix() for path in self.output.rglob('*') if path.is_file()]
        self.assertEqual(sorted(actual), sorted(expected + ['cache-payload-replay.json']))
        self.assertEqual(len(receipt['files']), 6)
        self.assertTrue(receipt['originalReceipts'])
        for row in receipt['files']:
            self.assertEqual(replay.digest((self.output / row['path']).read_bytes()), row['sha256'])

    def test_wrong_archive_digest_rejected_before_output(self):
        self.write_archive()
        self.profile['archiveSha256'] = '0' * 64
        with self.assertRaisesRegex(ValueError, 'SHA256 mismatch'):
            replay.prepare(self.archive, self.profile, self.candidate, self.output, self.repo)
        self.assertFalse(self.output.exists())

    def test_each_scope_needs_both_complete_successful_modes(self):
        original = copy.deepcopy(self.entries)
        for scope in ('core-lifecycle-deployment', 'core-lifecycle', 'cache-payloads', 'core-cli'):
            path = 'evidence/refactor/' + scope + '/123-1/validation.json'
            for mutation in ('missing-release', 'duplicate-debug', 'failed-release', 'failed-scope'):
                self.entries = copy.deepcopy(original)
                def change(row):
                    if mutation == 'missing-release': row['modes'].pop()
                    elif mutation == 'duplicate-debug': row['modes'][1]['configuration'] = 'Debug'
                    elif mutation == 'failed-release': row['modes'][1]['passed'] = False
                    else: row.update(succeeded=False, status='failed')
                self.change_json(path, change)
                with self.subTest(scope=scope, mutation=mutation), self.assertRaises(ValueError):
                    self.prepare()
                self.assertFalse(self.output.exists())

    def test_run_source_or_attempt_receipts_cannot_be_mixed(self):
        path = 'evidence/refactor/cache-payloads/123-1/validation.json'
        original = self.entries[path]
        for key, value in (('sourceCommit', '0' * 40), ('runId', '999'), ('runAttempt', '2')):
            self.entries[path] = original
            self.change_json(path, lambda row: row.update({key: value}))
            with self.subTest(key=key), self.assertRaises(ValueError):
                self.prepare()
        self.assertFalse(self.output.exists())

    def test_tampered_runtime_in_each_mode_rejected_with_valid_archive_hash(self):
        original = copy.deepcopy(self.entries)
        for mode in ('Debug', 'Release'):
            for name in replay.FIVE_RUNTIME:
                self.entries = copy.deepcopy(original)
                self.entries['artifacts/core-lifecycle-product/' + mode + '/bin/' + name] = b'tampered runtime'
                with self.subTest(mode=mode, name=name), self.assertRaises(ValueError):
                    self.prepare()
                self.assertFalse(self.output.exists())

    def test_build_link_recipe_toolchain_and_release_inventory_rejected(self):
        prefix = 'artifacts/core-lifecycle-product/Debug/'
        manifest = 'artifacts/final-release/123-1/NewModCompiler-Win32-Debug.manifest.json'
        cases = [(prefix + 'build.json', lambda row: row.update(containsProbeOrTestSources=True)),
                 (prefix + 'build.json', lambda row: row.update(exeSha256='0' * 64)),
                 (prefix + 'build.json', lambda row: row.update(compiler='other compiler')),
                 (prefix + 'build.json', lambda row: row.update(runtimeFiles=list(replay.RUNTIME))),
                 (prefix + 'link-inputs.json', lambda row: row.update(arguments=[])),
                 (prefix + 'link-inputs.json', lambda row: row['inputs'][0].update(sha256='invalid')),
                 (manifest, lambda row: row.update(buildReceiptSha256='0' * 64)),
                 (manifest, lambda row: row.update(linkRecipeSha256='0' * 64)),
                 (manifest, lambda row: row['toolchain'].update(vcToolsVersion='other')),
                 (manifest, lambda row: row['files'].pop()),
                 (manifest, lambda row: row['files'][0].update(bytes=999))]
        original = copy.deepcopy(self.entries)
        for index, (path, mutation) in enumerate(cases):
            self.entries = copy.deepcopy(original)
            self.change_json(path, mutation)
            with self.subTest(case=index), self.assertRaises(ValueError):
                self.prepare()
            self.assertFalse(self.output.exists())

    def test_release_requires_debug_and_release_and_binary_source_identity(self):
        path = 'artifacts/final-release/123-1/release.json'
        original = self.entries[path]
        for mutation in (lambda row: row['configurations'].pop(),
                         lambda row: row['configurations'][1].update(configuration='Debug'),
                         lambda row: row['configurations'][1].update(succeeded=False),
                         lambda row: row.update(binarySourceCommit='0' * 40),
                         lambda row: row.update(succeeded=False)):
            self.entries[path] = original
            self.change_json(path, mutation)
            with self.assertRaises(ValueError):
                self.prepare()
        self.assertFalse(self.output.exists())

    def test_cli_runtime_hashes_and_exe_suite_agreement_rejected(self):
        original = copy.deepcopy(self.entries)
        path = 'evidence/refactor/core-cli/123-1/validation.json'
        for name in replay.RUNTIME:
            self.entries = copy.deepcopy(original)
            self.change_json(path, lambda row: row['modes'][0]['runtimeInputs'].update({'\\' + name: '0' * 64}))
            with self.subTest(runtime=name), self.assertRaises(ValueError):
                self.prepare()
        for scope, field in (('core-lifecycle-deployment', 'exeSha256'), ('core-lifecycle', 'productExeSha256'),
                             ('cache-payloads', 'exeSha256'), ('core-cli', 'exeSha256')):
            self.entries = copy.deepcopy(original)
            self.change_json('evidence/refactor/' + scope + '/123-1/validation.json',
                             lambda row: row['modes'][0].update({field: '0' * 64}))
            with self.subTest(scope=scope), self.assertRaises(ValueError):
                self.prepare()
        self.assertFalse(self.output.exists())

    def test_existing_output_is_never_replaced(self):
        self.output.mkdir()
        keep = self.output / 'user-file'
        keep.write_bytes(b'preserve existing output')
        with self.assertRaises(ValueError):
            self.prepare()
        self.assertEqual(keep.read_bytes(), b'preserve existing output')

    def test_verify_prepared_accepts_unchanged_receipt_and_runtime(self):
        original = self.prepare()
        with patch.object(replay, 'check_api') as api:
            verified = replay.verify_prepared(self.output / 'cache-payload-replay.json', self.candidate, self.repo)
        api.assert_called_once_with(self.profile)
        self.assertEqual(verified, original)

    def test_verify_prepared_rejects_modified_receipt_and_runtime(self):
        self.prepare()
        receipt_path = self.output / 'cache-payload-replay.json'
        original = receipt_path.read_bytes()
        row = json.loads(original)
        row['productionSourcesUnchanged'] = False
        receipt_path.write_text(json.dumps(row))
        with patch.object(replay, 'check_api'), self.assertRaises(ValueError):
            replay.verify_prepared(receipt_path, self.candidate, self.repo)
        receipt_path.write_bytes(original)
        (self.output / 'Release/bin/NewModCompiler.exe').write_bytes(b'tampered prepared runtime')
        with patch.object(replay, 'check_api'), self.assertRaises(ValueError):
            replay.verify_prepared(receipt_path, self.candidate, self.repo)


if __name__ == '__main__':
    unittest.main()
