"""Gate tests use actual Git trees and accepted repository bytes, not fake hashes."""
import importlib.util
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location("audio_identity", ROOT / "recovery/scripts/ValidateAudioSourceTree.py")
identity = importlib.util.module_from_spec(spec)
spec.loader.exec_module(identity)


class AudioSourceIdentityTest(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix="audio-source-identity-")
        self.repo = Path(self.temp.name)
        self.audio = self.repo / "recovery/src/Managed/Audio"
        shutil.copytree(ROOT / "recovery/src/Managed/Audio", self.audio)
        self.project = self.audio / identity.PROJECT
        migrated_project = self.project.read_bytes()
        self.project.write_bytes(migrated_project.replace(identity.NEW_HINT, identity.OLD_HINT))
        self.git("init", "-q")
        self.git("config", "core.autocrlf", "false")
        self.git("config", "core.filemode", "true")
        self.commit()
        # Independent Git itself establishes the accepted tree before migration.
        self.expected = self.git("rev-parse", "HEAD:recovery/src/Managed/Audio").decode().strip()
        self.project.write_bytes(migrated_project)
        self.commit()

    def tearDown(self):
        self.temp.cleanup()

    def git(self, *args):
        return subprocess.check_output(["git", "-C", str(self.repo), *args], stderr=subprocess.PIPE)

    def commit(self):
        self.git("add", ".")
        self.git("-c", "user.name=Gate test", "-c", "user.email=gate@example.invalid", "commit", "-qm", "fixture", "--allow-empty")

    def rejects(self):
        self.commit()
        with self.assertRaises(ValueError):
            identity.validate(self.repo, self.expected)

    def test_exact_legacy_bytes_still_match(self):
        self.project.write_bytes(self.project.read_bytes().replace(identity.NEW_HINT, identity.OLD_HINT))
        self.commit()
        result = identity.validate(self.repo, self.expected)
        self.assertEqual(result["equivalence"], "exact")

    def test_only_migrated_hintpath_reuses_original_tree(self):
        result = identity.validate(self.repo, self.expected)
        self.assertEqual(result["equivalence"], "project-hintpath-migration")
        self.assertEqual(result["acceptanceTree"], self.expected)
        self.assertNotEqual(result["currentTree"], self.expected)

    def test_csharp_change_rejected(self):
        path = self.audio / "Plugin.cs"
        path.write_bytes(path.read_bytes() + b"\n// altered implementation\n")
        self.rejects()

    def test_other_project_change_rejected(self):
        self.project.write_bytes(self.project.read_bytes().replace(b"net472", b"net48"))
        self.rejects()

    def test_added_file_rejected(self):
        (self.audio / "Injected.cs").write_text("class Injected {}")
        self.rejects()

    def test_removed_file_rejected(self):
        (self.audio / "Plugin.cs").unlink()
        self.rejects()

    def test_file_mode_change_rejected(self):
        self.git("update-index", "--chmod=+x", "recovery/src/Managed/Audio/Plugin.cs")
        self.git("-c", "user.name=Gate test", "-c", "user.email=gate@example.invalid", "commit", "-qm", "mode change")
        with self.assertRaises(ValueError):
            identity.validate(self.repo, self.expected)

    def test_missing_migrated_hintpath_rejected(self):
        self.project.write_bytes(self.project.read_bytes().replace(identity.NEW_HINT, b"<HintPath>other.dll</HintPath>"))
        self.rejects()

    def test_repeated_migrated_hintpath_rejected(self):
        self.project.write_bytes(self.project.read_bytes() + identity.NEW_HINT)
        self.rejects()


if __name__ == "__main__":
    unittest.main()
