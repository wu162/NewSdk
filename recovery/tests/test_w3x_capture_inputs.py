"""Regression checks for historical-capture reuse across unrelated refactor work."""
import importlib.util
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

MODULE = Path(__file__).resolve().parents[1] / "scripts/CheckW3xCaptureInputs.py"
spec = importlib.util.spec_from_file_location("w3x_capture_inputs", MODULE)
checker = importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)


class CaptureInputsTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.previous = os.getcwd()
        os.chdir(self.directory.name)
        self.addCleanup(self.directory.cleanup)
        self.addCleanup(os.chdir, self.previous)
        self.git("init", "-q")
        for path in checker.SHARED + checker.FIXTURES:
            self.write(path, "// fixture\n")
        self.write("recovery/src/Native/W3x/Compiler.cpp", '#include "../Shared.h"\n')
        self.write("recovery/src/Native/Shared.h", '#include "Nested.h"\n')
        self.write("recovery/src/Native/Nested.h", "// original\n")
        self.baseline = self.commit()

    def git(self, *args):
        return subprocess.check_output(["git", *args]).decode().strip()

    def write(self, path, text):
        Path(path).parent.mkdir(parents=True, exist_ok=True)
        Path(path).write_text(text)

    def commit(self):
        self.git("add", ".")
        self.git("-c", "user.name=Test", "-c", "user.email=test@example.invalid",
                 "commit", "-qm", "fixture")
        return self.git("rev-parse", "HEAD")

    def test_unrelated_xml_change_preserves_capture(self):
        self.write("recovery/src/Native/XmlUnrelated.cpp", "// unrelated\n")
        self.assertTrue(checker.check(self.baseline, self.commit())["unchanged"])

    def test_transitive_shared_header_change_rejects_capture(self):
        self.write("recovery/src/Native/Nested.h", "// changed\n")
        result = checker.check(self.baseline, self.commit())
        self.assertEqual(result["changedInputs"], ["recovery/src/Native/Nested.h"])

    def test_removed_include_remains_in_dependency_union(self):
        self.write("recovery/src/Native/W3x/Compiler.cpp", "// include removed\n")
        Path("recovery/src/Native/Shared.h").unlink()
        result = checker.check(self.baseline, self.commit())
        self.assertIn("recovery/src/Native/Shared.h", result["changedInputs"])

    def test_missing_transitive_header_rejects_capture(self):
        Path("recovery/src/Native/Nested.h").unlink()
        with self.assertRaisesRegex(ValueError, "Missing W3X dependency"):
            checker.check(self.baseline, self.commit())

    def test_new_w3x_source_invalidates_capture(self):
        self.write("recovery/src/Native/W3x/Additional.cpp", "// new implementation\n")
        self.assertFalse(checker.check(self.baseline, self.commit())["unchanged"])


if __name__ == "__main__":
    unittest.main()
