import importlib.util
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
import warnings
import zipfile
from unittest.mock import patch

SCRIPT = Path(__file__).resolve().parents[1] / 'scripts' / 'PrepareCompilerCliReuse.py'
spec = importlib.util.spec_from_file_location('cli_reuse', SCRIPT)
reuse = importlib.util.module_from_spec(spec)
spec.loader.exec_module(reuse)


class CliReuseTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.repo = self.root / 'repo'
        self.repo.mkdir()
        self.git('init', '-q')
        self.git('config', 'user.email', 'fixture@example.invalid')
        self.git('config', 'user.name', 'Fixture')
        self.commit('recovery/src/App/Cli/Program.cs', 'production-v1')
        self.source = self.git('rev-parse', 'HEAD').strip()
        self.initial_source = self.source
        self.commit('recovery/scripts/TestCompilerCli.ps1', 'new CLI-only tool')
        self.candidate = self.git('rev-parse', 'HEAD').strip()
        self.entries = self.artifact()
        self.archive = self.root / 'artifact.zip'
        self.output = self.root / 'prepared'

    def git(self, *args):
        return subprocess.check_output(['git', '-C', str(self.repo), *args], text=True, stderr=subprocess.DEVNULL)

    def commit(self, path, text):
        target = self.repo / path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text)
        self.git('add', '.')
        self.git('commit', '-qm', 'fixture')

    def artifact(self):
        files = {}
        def add(path, value):
            data = json.dumps(value).encode()
            files[path] = data
            return reuse.digest(data)
        common = dict(sourceCommit=self.source, runId='123', runAttempt='1')
        sections = {}
        for section in ('core-lifecycle-deployment', 'core-lifecycle', 'core-lifecycle-texture', 'core-cli'):
            sections[section] = dict(common, status='passed', succeeded=True, modes=[])
        protocol = dict(sourceCommit=self.source, status='passed', errorCount=0, errors=[], matrix=[], validatedFiles=[])
        core_base = 'evidence/refactor/core-lifecycle/123-1/'
        cli_base = 'evidence/refactor/core-cli/123-1/'
        for mode in ('Debug', 'Release'):
            runtime = {name: (mode + ':' + name).encode() for name in reuse.RUNTIME}
            exe = reuse.digest(runtime['NewModCompiler.exe'])
            prefix = 'artifacts/core-lifecycle-product/' + mode + '/'
            for name, data in runtime.items():
                files[prefix + 'bin/' + name] = data
            add(prefix + 'build.json', dict(sourceCommit=self.source, configuration=mode, containsProbeOrTestSources=False, exeSha256=exe))
            sections['core-lifecycle-deployment']['modes'].append(dict(configuration=mode, passed=True, exeSha256=exe))
            sections['core-lifecycle-texture']['modes'].append(dict(configuration=mode, passed=True, exeSha256=exe))
            fixtures = []
            for fixture in sorted(reuse.FIXTURES):
                sha = add(core_base + mode + '/' + fixture + '/result.json', dict(common, succeeded=True))
                fixtures.append(dict(name=fixture, passed=True, resultSha256=sha))
            sections['core-lifecycle']['modes'].append(dict(configuration=mode, passed=True, productExeSha256=exe,
                fixtures=fixtures, runtimeFiles=[dict(name=n, sha256=reuse.digest(runtime[n])) for n in reuse.RUNTIME if not n.endswith('.exe')]))
            sections['core-cli']['modes'].append(dict(configuration=mode, passed=True, exeSha256=exe,
                business=dict(common, configuration=mode, succeeded=True, inputsUnchanged=True, coldGroupOutputsIdentical=True, originalFourOutputsIdentical=True),
                cases=[dict(name=n, passed=True) for n in sorted(reuse.CLI_CASES)], runtimeInputs={'\\' + n: reuse.digest(d) for n, d in runtime.items()}))
            for baseline in ('original-baseline', 'business-original-baseline'):
                add(cli_base + mode + '/' + baseline + '/validation.json', dict(common, succeeded=True))
            protocol['matrix'].append(dict(configuration=mode, reports=1, eventFiles=1, events=2))
            path = mode + '/output/report.json'
            sha = add(cli_base + path, dict(status='succeeded'))
            protocol['validatedFiles'].append(dict(path=path, sha256=sha))
        sections['core-lifecycle-texture']['configurations'] = sections['core-lifecycle-texture'].pop('modes')
        for section, doc in sections.items():
            add('evidence/refactor/' + section + '/123-1/validation.json', doc)
        add(cli_base + 'cli-protocol.json', protocol)
        app = dict(protocol, validatedFiles=[])
        for mode in ('Debug', 'Release'):
            path = mode + '/output/report.json'
            sha = add(core_base + path, dict(status='succeeded'))
            app['validatedFiles'].append(dict(path=path, sha256=sha))
        add(core_base + 'app-protocol.json', app)
        return files

    def change_json(self, path, mutate):
        value = json.loads(self.entries[path])
        mutate(value)
        self.entries[path] = json.dumps(value).encode()

    def run_prepare(self, expected=None, extra=(), raw_names=()):
        with warnings.catch_warnings():
            warnings.simplefilter('ignore', UserWarning)
            with zipfile.ZipFile(self.archive, 'w') as z:
                for name, data in list(self.entries.items()) + list(extra):
                    z.writestr(name, data)
        # ZipInfo normalizes host separators at construction time (notably on
        # Windows), so malicious path fixtures must patch the actual ZIP local
        # and central header bytes after writing an otherwise valid archive.
        raw = self.archive.read_bytes()
        for before, after in raw_names:
            before, after = before.encode('ascii'), after.encode('ascii')
            self.assertEqual(len(before), len(after))
            self.assertEqual(raw.count(before), 2, 'Expected local and central ZIP header names')
            raw = raw.replace(before, after)
        self.archive.write_bytes(raw)
        sha = expected or reuse.digest(raw)
        return reuse.prepare(self.archive, sha, self.source, self.candidate, self.output, self.repo)

    def test_valid_preserves_history_extracts_only_six_files(self):
        result = self.run_prepare()
        self.assertFalse(result['candidateValidated'])
        self.assertEqual(result['sourceCommit'], self.source)
        self.assertEqual(result['candidateCommit'], self.candidate)
        self.assertEqual(result['changedPaths'], ['recovery/scripts/TestCompilerCli.ps1'])
        files = sorted(p.relative_to(self.output).as_posix() for p in self.output.rglob('*') if p.is_file())
        self.assertEqual(files, sorted([m + '/bin/' + n for m in ('Debug', 'Release') for n in reuse.RUNTIME] + ['reuse.json']))

    def test_wrong_digest_rejected_before_output(self):
        with self.assertRaisesRegex(ValueError, 'SHA256 mismatch'):
            self.run_prepare('0' * 64)
        self.assertFalse(self.output.exists())

    def test_tampered_bin_rejected_even_with_correct_archive_digest(self):
        self.entries['artifacts/core-lifecycle-product/Debug/bin/NewModCompiler.exe'] = b'tampered'
        with self.assertRaisesRegex(ValueError, 'hash mismatch'):
            self.run_prepare()
        self.assertFalse(self.output.exists())

    def test_missing_release_rejected(self):
        self.change_json('evidence/refactor/core-cli/123-1/validation.json', lambda d: d['modes'].pop())
        with self.assertRaisesRegex(ValueError, 'Debug/Release'):
            self.run_prepare()

    def test_production_changes_rejected(self):
        self.commit('recovery/src/App/Cli/Program.cs', 'production-v2')
        self.candidate = self.git('rev-parse', 'HEAD').strip()
        with self.assertRaisesRegex(ValueError, 'Non-CLI'):
            self.run_prepare()

    def test_nonancestor_rejected(self):
        self.source, self.candidate = self.candidate, self.source
        with self.assertRaises(subprocess.CalledProcessError):
            self.run_prepare()

    def test_zip_paths_and_duplicates_rejected(self):
        for path in ('../escape', '/absolute', 'C:/escape', 'a\\escape', 'a/../escape', 'a//b'):
            with self.subTest(path=path), self.assertRaisesRegex(ValueError, 'Unsafe ZIP'):
                placeholder = 'Q' * len(path)
                self.run_prepare(extra=[(placeholder, b'bad')], raw_names=[(placeholder, path)])
        original = next(iter(self.entries))
        for path in (original, original.upper()):
            with self.subTest(path=path), self.assertRaisesRegex(ValueError, 'Duplicate ZIP'):
                placeholder = 'Q' * len(path)
                self.run_prepare(extra=[(placeholder, b'bad')], raw_names=[(placeholder, path)])

    def test_raw_nul_name_rejected_before_zipinfo_truncation_can_hide_it(self):
        # Python's ZipInfo.filename truncates at NUL while orig_filename retains
        # the full central-directory value. Exercise the archive bytes, not a
        # ZipInfo constructor that would already have discarded the suffix.
        path = 'safe\0hidden'
        placeholder = 'Q' * len(path)
        with self.assertRaisesRegex(ValueError, 'Unsafe ZIP'):
            self.run_prepare(extra=[(placeholder, b'bad')], raw_names=[(placeholder, path)])
        self.assertFalse(self.output.exists())

    def test_symlink_rejected(self):
        link = zipfile.ZipInfo('link')
        link.create_system = 3
        link.external_attr = 0o120777 << 16
        with self.assertRaisesRegex(ValueError, 'symlink'):
            self.run_prepare(extra=[(link, b'../target')])

    def test_run_identity_mismatch_rejected(self):
        self.change_json('evidence/refactor/core-lifecycle/123-1/validation.json', lambda d: d.update(runId='999'))
        with self.assertRaisesRegex(ValueError, 'identity differs'):
            self.run_prepare()

    def test_protocol_error_and_input_tamper_rejected(self):
        path = 'evidence/refactor/core-cli/123-1/cli-protocol.json'
        original = self.entries[path]
        self.change_json(path, lambda d: d.update(errorCount=1, errors=['schema failed']))
        with self.assertRaisesRegex(ValueError, 'protocol failed'):
            self.run_prepare()
        self.entries[path] = original
        self.entries['evidence/refactor/core-cli/123-1/Debug/output/report.json'] = b'changed'
        with self.assertRaisesRegex(ValueError, 'Protocol input hash'):
            self.run_prepare()

    def test_core_fixture_failure_and_product_identity_rejected(self):
        path = 'evidence/refactor/core-lifecycle/123-1/validation.json'
        original = self.entries[path]
        self.change_json(path, lambda d: d['modes'][0]['fixtures'].pop())
        with self.assertRaisesRegex(ValueError, 'Incomplete Core'):
            self.run_prepare()
        self.entries[path] = original
        self.change_json('artifacts/core-lifecycle-product/Debug/build.json', lambda d: d.update(exeSha256='0' * 64))
        with self.assertRaisesRegex(ValueError, 'EXE identity'):
            self.run_prepare()

    def workflow_change(self, updated):
        original = "name: Fixture\non:\n  push:\n    paths:\n      - 'old'\njobs:\n  build:\n    env: {FLAGS: old}\n    steps:\n      - run: compile-old\n"
        self.git('reset', '--hard', self.initial_source)
        self.commit(reuse.WORKFLOW, original)
        self.source = self.git('rev-parse', 'HEAD').strip()
        self.commit(reuse.WORKFLOW, updated(original))
        self.candidate = self.git('rev-parse', 'HEAD').strip()
        self.entries = self.artifact()

    def test_workflow_only_trigger_paths_allowed(self):
        self.workflow_change(lambda s: s.replace("      - 'old'", "      - 'new'\n      - 'extra'"))
        self.assertEqual(self.run_prepare()['changedPaths'], [reuse.WORKFLOW])

    def test_workflow_build_and_environment_changes_rejected(self):
        for old, new in [('FLAGS: old', 'FLAGS: unsafe'), ('compile-old', 'compile-new')]:
            with self.subTest(field=old):
                self.workflow_change(lambda s: s.replace(old, new))
                with self.assertRaisesRegex(ValueError, 'Workflow change exceeds'):
                    self.run_prepare()

    def test_exact_pinned_integration_migration_and_further_changes(self):
        self.workflow_change(lambda s: s.replace('compile-old', 'strict-archive-reuse'))
        old = reuse.workflow_without_trigger_paths(reuse.git(self.repo, 'show', self.source + ':' + reuse.WORKFLOW))
        new = reuse.workflow_without_trigger_paths(reuse.git(self.repo, 'show', self.candidate + ':' + reuse.WORKFLOW))
        pinned = (reuse.digest(old), reuse.digest(new))
        with patch.object(reuse, 'WORKFLOW_MIGRATION', pinned):
            self.assertEqual(self.run_prepare()['changedPaths'], [reuse.WORKFLOW])
            self.commit(reuse.WORKFLOW, (self.repo / reuse.WORKFLOW).read_text().replace('FLAGS: old', 'FLAGS: changed'))
            self.candidate = self.git('rev-parse', 'HEAD').strip()
            with self.assertRaisesRegex(ValueError, 'Workflow change exceeds'):
                reuse.closure(self.repo, self.source, self.candidate)

    def test_dedicated_runtime_workflow_requires_exact_reviewed_bytes(self):
        path = '.github/workflows/refactor-ui-unicode.yml'
        content = 'name: Fixed historical runtime validation\n'
        self.commit(path, content)
        self.candidate = self.git('rev-parse', 'HEAD').strip()
        with patch.dict(reuse.PINNED_TOOL_WORKFLOWS, {path: reuse.digest(reuse.git(self.repo, 'show', self.candidate + ':' + path))}):
            self.assertIn(path, reuse.closure(self.repo, self.source, self.candidate))
            self.commit(path, content + 'env: {BUILD_FLAGS: changed}\n')
            self.candidate = self.git('rev-parse', 'HEAD').strip()
            with self.assertRaisesRegex(ValueError, 'differs from reviewed bytes'):
                reuse.closure(self.repo, self.source, self.candidate)
            self.git('rm', path)
            self.git('commit', '-qm', 'delete fixed workflow')
            self.candidate = self.git('rev-parse', 'HEAD').strip()
            # Adding then deleting produces no final tree change, so there is
            # no workflow to execute and no binary-input closure expansion.
            self.assertNotIn(path, reuse.closure(self.repo, self.source, self.candidate))

    def test_baseline_tool_change_allowed_as_tool_only(self):
        self.commit('recovery/scripts/TestCompilerBackendBaseline.ps1', 'new baseline acceptance tool')
        self.candidate = self.git('rev-parse', 'HEAD').strip()
        self.assertFalse(self.run_prepare()['candidateValidated'])

    def test_app_protocol_missing_error_and_input_tamper_rejected(self):
        path = 'evidence/refactor/core-lifecycle/123-1/app-protocol.json'
        original = self.entries.pop(path)
        with self.assertRaisesRegex(ValueError, 'Missing artifact input'):
            self.run_prepare()
        self.entries[path] = original
        self.change_json(path, lambda d: d.update(errorCount=1, errors=['schema failed']))
        with self.assertRaisesRegex(ValueError, 'protocol failed'):
            self.run_prepare()
        self.entries[path] = original
        self.entries['evidence/refactor/core-lifecycle/123-1/Debug/output/report.json'] = b'changed'
        with self.assertRaisesRegex(ValueError, 'Protocol input hash'):
            self.run_prepare()

    def test_existing_output_not_overwritten(self):
        self.output.mkdir()
        sentinel = self.output / 'sentinel'
        sentinel.write_text('preserve')
        with self.assertRaisesRegex(ValueError, 'existing output'):
            self.run_prepare()
        self.assertEqual(sentinel.read_text(), 'preserve')


if __name__ == '__main__':
    unittest.main()
