import copy
import importlib.util
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch

SCRIPTS = Path(__file__).resolve().parents[1] / 'scripts'
sys.path.insert(0, str(SCRIPTS))
spec = importlib.util.spec_from_file_location('download_cli_reuse', SCRIPTS / 'DownloadCompilerCliReuse.py')
download = importlib.util.module_from_spec(spec)
spec.loader.exec_module(download)


class DownloadTests(unittest.TestCase):
    def setUp(self):
        self.run = dict(id=download.RUN, head_sha=download.SOURCE, run_attempt=download.ATTEMPT,
                        status='completed', conclusion='success', path=download.WORKFLOW,
                        head_branch='refactor/modcompiler-p0-p3',
                        repository=dict(id=7, full_name=download.REPOSITORY),
                        head_repository=dict(id=7, full_name=download.REPOSITORY))
        self.artifact = dict(id=download.ARTIFACT, name=download.NAME, expired=False,
                             digest='sha256:' + download.SHA256,
                             workflow_run=dict(id=download.RUN, head_sha=download.SOURCE,
                                 head_branch='refactor/modcompiler-p0-p3', repository_id=7,
                                 head_repository_id=7))

    def test_exact_successful_origin_accepted(self):
        download.verify_metadata(self.run, self.artifact)

    def test_run_identity_success_and_attempt_rejected(self):
        for key, value in [('id', 0), ('head_sha', '0' * 40), ('run_attempt', 2),
                           ('status', 'in_progress'), ('conclusion', 'failure'),
                           ('path', '.github/workflows/other.yml'), ('head_branch', 'other')]:
            with self.subTest(key=key):
                row = dict(self.run, **{key: value})
                with self.assertRaises(ValueError):
                    download.verify_metadata(row, self.artifact)

    def test_fork_repository_rejected(self):
        for key in ('repository', 'head_repository'):
            row = copy.deepcopy(self.run)
            row[key]['full_name'] = 'other/NewSdk'
            with self.subTest(key=key), self.assertRaises(ValueError):
                download.verify_metadata(row, self.artifact)

    def test_artifact_mismatch_expiry_and_digest_rejected(self):
        for key, value in [('id', 0), ('name', 'wrong'), ('expired', True),
                           ('digest', 'sha256:' + '0' * 64)]:
            with self.subTest(key=key), self.assertRaises(ValueError):
                download.verify_metadata(self.run, dict(self.artifact, **{key: value}))
        for key, value in [('id', 0), ('head_sha', '0' * 40), ('head_branch', 'other'),
                           ('repository_id', 9), ('head_repository_id', 9)]:
            row = copy.deepcopy(self.artifact)
            row['workflow_run'][key] = value
            with self.subTest(origin=key), self.assertRaises(ValueError):
                download.verify_metadata(self.run, row)

    def test_closure_failure_happens_before_network(self):
        with patch.object(download, 'closure', side_effect=ValueError('changed production')), \
             patch.object(download, 'api_json') as api:
            with self.assertRaises(ValueError):
                download.download('.', 'candidate', 'output', 'archive')
            api.assert_not_called()

    def test_existing_archive_not_replaced_and_no_zip_command(self):
        with tempfile.TemporaryDirectory() as folder:
            archive = Path(folder) / 'artifact.zip'
            archive.write_bytes(b'preserve')
            with patch.object(download, 'closure'), \
                 patch.object(download, 'api_json', side_effect=[self.run, self.artifact]), \
                 patch.object(download.subprocess, 'run') as run:
                with self.assertRaises(FileExistsError):
                    download.download('.', 'candidate', 'output', archive)
                run.assert_not_called()
            self.assertEqual(archive.read_bytes(), b'preserve')

    def test_only_fixed_endpoint_downloaded_then_pinned_preparer_called(self):
        with tempfile.TemporaryDirectory() as folder:
            archive = Path(folder) / 'artifact.zip'
            receipt = dict(runId=str(download.RUN), runAttempt=str(download.ATTEMPT))
            with patch.object(download, 'closure'), \
                 patch.object(download, 'api_json', side_effect=[self.run, self.artifact]), \
                 patch.object(download.subprocess, 'run') as run, \
                 patch.object(download, 'prepare', return_value=receipt) as prepare:
                self.assertEqual(download.download('.', 'candidate', 'output', archive), receipt)
                self.assertEqual(run.call_args.args[0], ['gh', 'api',
                    f'repos/{download.REPOSITORY}/actions/artifacts/{download.ARTIFACT}/zip'])
                prepare.assert_called_once_with(archive, download.SHA256, download.SOURCE,
                                                'candidate', 'output', '.')


if __name__ == '__main__':
    unittest.main()
