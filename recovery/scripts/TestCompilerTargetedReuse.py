#!/usr/bin/env python3
"""Actions-only real-git contracts for the strict targeted reuse controller.

This creates temporary repositories, never downloads artifacts and never runs a
compiler or a staged runtime. Production SOURCE/RUN pins are not changed on disk.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock

import DownloadCompilerTargetedReuse as controller


class ClosureContracts(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix='targeted-reuse-contract-')
        self.repo = Path(self.temporary.name)
        self.saved_source = controller.SOURCE
        self.git('init', '--quiet')
        self.git('config', 'user.name', 'Targeted reuse contract')
        self.git('config', 'user.email', 'targeted-reuse-contract@example.invalid')
        self.git('config', 'core.autocrlf', 'false')
        self.write('.gitignore', '*.ignored\n**/ignored-*.cs\n**/__pycache__/\n')
        for path in sorted(controller.ALLOWED - {'recovery/scripts/TestCompilerTargetedReuse.py'}):
            self.write(path)
        for path in ('AGENTS.md', 'README.md', 'recovery/docs/contract.md',
                     'recovery/src/Production.cs', 'SDK/schema.xsd', 'recovery/build/recipe.json',
                     'recovery/scripts/BuildManagedFoundation.ps1', controller.WORKFLOW,
                     'recovery/tests/Unrelated.cs'):
            self.write(path)
        self.source = self.commit('base production and exact targeted inventory')
        # Only the imported module in this test process sees this fixture pin.
        # The actual downloader on disk and accepted_metadata() remain unchanged.
        controller.SOURCE = self.source
        self.no_network = mock.patch.object(controller, 'api', side_effect=AssertionError('Closure contracts must not perform network requests'))
        self.no_network.start()

    def tearDown(self):
        self.no_network.stop()
        controller.SOURCE = self.saved_source
        self.temporary.cleanup()

    def git(self, *args):
        return subprocess.run(['git', '-C', str(self.repo), *args], check=True, capture_output=True).stdout.decode().strip()

    def write(self, relative, text='base\n'):
        target = self.repo / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text, encoding='utf-8')

    def commit(self, message):
        self.git('add', '--all')
        self.git('commit', '--quiet', '-m', message)
        return self.git('rev-parse', 'HEAD')

    def rejects(self, candidate=None, message=None):
        with self.assertRaises((ValueError, subprocess.CalledProcessError)) as captured:
            controller.closure(self.repo, candidate or self.git('rev-parse', 'HEAD'))
        if message:
            self.assertIn(message, str(captured.exception))

    def test_exact_current_targeted_fixtures_and_docs_pass(self):
        self.assertEqual(len(controller.TARGETED_FIXTURES), 25)
        paths = ['recovery/tests/' + name + '.cs' for name in controller.TARGETED_FIXTURES]
        paths += ['recovery/scripts/TestCompilerTargetedReuse.py', 'recovery/STAGE_DURATION.md',
                  'recovery/scripts/TestCompilerBackendBaseline.ps1',
                  'recovery/tests/CompilerDocumentSnapshotOracle.cs',
                  'recovery/tests/fixtures/UpheavalSmall/Data/FutureTankArmor.xml',
                  'AGENTS.md', 'recovery/docs/contract.md', controller.REUSE_WORKFLOW,
                  'recovery/scripts/ExportCompilerAcceptedRelease.py', 'recovery/scripts/TestCompilerAcceptedRelease.py']
        for path in paths:
            self.write(path, 'reviewed test or doc change\n')
        candidate = self.commit('current exact targeted tests and permitted docs')
        self.assertEqual(controller.closure(self.repo, candidate), sorted(paths))

    def test_unchanged_source_passes(self):
        self.assertEqual(controller.closure(self.repo, self.source), [])

    def test_each_production_root_and_build_recipe_change_is_rejected(self):
        for path in ('recovery/src/Production.cs', 'SDK/schema.xsd', 'recovery/build/recipe.json',
                     'recovery/scripts/BuildManagedFoundation.ps1', controller.WORKFLOW):
            with self.subTest(path=path):
                self.git('reset', '--hard', self.source)
                self.write(path, 'changed production input\n')
                candidate = self.commit('production input must rebuild')
                self.rejects(candidate, 'Production or unrelated test inputs changed')

    def test_unrelated_test_and_non_markdown_doc_are_rejected(self):
        for path in ('recovery/tests/Unrelated.cs', 'recovery/tests/NewUnreviewedFixture.cs', 'recovery/docs/recipe.json'):
            with self.subTest(path=path):
                self.git('reset', '--hard', self.source)
                self.write(path, 'unreviewed input\n')
                self.rejects(self.commit('unrelated source'), 'Production or unrelated test inputs changed')

    def test_allowed_fixture_deletion_is_rejected(self):
        (self.repo / 'recovery/tests/CompilerPhysicalInputs.cs').unlink()
        self.rejects(self.commit('delete an otherwise allowed fixture'), 'Deleted, renamed or type-changed')

    def test_rename_between_two_allowed_doc_paths_is_rejected(self):
        self.git('mv', 'AGENTS.md', 'recovery/STAGE_DURATION.md')
        self.rejects(self.commit('rename between allowlisted documents'), 'Deleted, renamed or type-changed')

    def test_non_ancestor_and_checkout_mismatch_are_rejected(self):
        self.write('README.md', 'allowed candidate\n')
        candidate = self.commit('candidate')
        self.git('checkout', '--quiet', self.source)
        self.rejects(candidate, 'Checkout differs from test source')
        self.git('checkout', '--quiet', '--orphan', 'unrelated-history')
        unrelated = self.commit('unrelated root')
        self.rejects(unrelated)

    def test_staged_and_unstaged_production_edits_are_rejected(self):
        self.write('recovery/src/Production.cs', 'unstaged production\n')
        self.rejects(self.source)
        self.git('add', 'recovery/src/Production.cs')
        self.rejects(self.source)

    def test_untracked_production_inputs_in_all_roots_are_rejected(self):
        for root in controller.PRODUCTION_ROOTS:
            with self.subTest(root=root):
                target = root + '/untracked-production.cs'
                self.write(target, 'untracked but discoverable\n')
                self.rejects(self.source, 'Untracked production inputs present')
                (self.repo / target).unlink()

    def test_ignored_untracked_production_inputs_are_rejected(self):
        for root in controller.PRODUCTION_ROOTS:
            with self.subTest(root=root):
                target = root + '/ignored-generated.cs'
                self.write(target, 'ignored but discoverable\n')
                self.assertEqual(self.git('check-ignore', target), target)
                self.rejects(self.source, 'Untracked production inputs present')
                (self.repo / target).unlink()

    def test_script_pycache_does_not_mask_or_falsely_block_production_changes(self):
        self.write('recovery/scripts/__pycache__/controller.cpython.pyc', 'ignored script cache\n')
        self.assertEqual(controller.closure(self.repo, self.source), [])
        self.write('recovery/src/__pycache__/hidden.cs', 'still a recursive production source\n')
        self.rejects(self.source, 'Untracked production inputs present')

    def test_candidate_sha_shape_is_strict(self):
        for candidate in ('HEAD', self.source[:12], 'A' * 40, '0' * 40):
            with self.subTest(candidate=candidate):
                self.rejects(candidate)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output')
    args = parser.parse_args()
    suite = unittest.defaultTestLoader.loadTestsFromTestCase(ClosureContracts)
    result = unittest.TextTestRunner(verbosity=2).run(suite)
    report = {
        'succeeded': result.wasSuccessful(), 'sourceCommit': os.environ.get('GITHUB_SHA'),
        'runId': os.environ.get('GITHUB_RUN_ID'), 'runAttempt': os.environ.get('GITHUB_RUN_ATTEMPT'),
        'controllerSha256': hashlib.sha256(Path(controller.__file__).read_bytes()).hexdigest(),
        'testsRun': result.testsRun,
        'failures': [{'test': str(case), 'traceback': trace} for case, trace in result.failures],
        'errors': [{'test': str(case), 'traceback': trace} for case, trace in result.errors],
        'pinnedBinarySourceUnchanged': controller.SOURCE,
        'scope': 'Real temporary git closure contracts only; no network, artifact download, runtime execution, compilation or candidate acceptance',
    }
    if args.output:
        output = Path(args.output)
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    if result.wasSuccessful():
        print('PASS TargetedReuseController: ' + str(result.testsRun) + ' real-git contracts')
    return 0 if result.wasSuccessful() else 1


if __name__ == '__main__':
    raise SystemExit(main())
