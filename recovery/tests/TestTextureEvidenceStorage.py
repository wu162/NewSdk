import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
import zipfile

spec = importlib.util.spec_from_file_location('storage', Path(__file__).parents[1] / 'scripts/PackTextureSdkProducts.py')
storage = importlib.util.module_from_spec(spec)
spec.loader.exec_module(storage)
capture_spec = importlib.util.spec_from_file_location('capture_storage', Path(__file__).parents[1] / 'scripts/PackTextureCaptures.py')
captures = importlib.util.module_from_spec(capture_spec)
capture_spec.loader.exec_module(captures)


class StorageTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.modes = []
        for mode in ('Original', 'Recovered', 'NoOriginal'):
            files = []
            for ext in ('bin', 'manifest', 'imp', 'relo'):
                data = b'' if ext == 'imp' else b'\x00\xff\r\n\x00' * 100
                (self.root / (mode + '-mod.' + ext)).write_bytes(data)
                files.append(dict(file='mod.' + ext, bytes=len(data), sha256=storage.sha(data)))
            self.modes.append(dict(mode=mode, files=files))
        self.receipt = json.dumps(dict(sourceCommit='a' * 40, runId='123', modes=self.modes)).encode() + b'\r\n'
        (self.root / 'validation.json').write_bytes(self.receipt)

    def tearDown(self):
        self.temp.cleanup()

    def test_full_binary_roundtrip_and_immutable_receipt(self):
        original = {p.name: p.read_bytes() for p in self.root.glob('*-mod.*')}
        result = storage.pack(self.root, threshold=1)
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)
        self.assertEqual(len(result['files']), 12)
        with zipfile.ZipFile(self.root / 'products.zip') as archive:
            self.assertEqual({n: archive.read(n) for n in archive.namelist()}, original)
        self.assertEqual(list(self.root.glob('*-mod.*')), [])
        self.assertIsNone(storage.pack(self.root, threshold=1))

    def test_small_products_stay_unchanged(self):
        self.assertIsNone(storage.pack(self.root))
        self.assertEqual(len(list(self.root.glob('*-mod.*'))), 12)
        self.assertFalse((self.root / 'products.zip').exists())

    def test_wrong_receipt_hash_keeps_all_raw_evidence(self):
        (self.root / 'Original-mod.bin').write_bytes(b'corrupted')
        with self.assertRaises(AssertionError):
            storage.pack(self.root, threshold=1)
        self.assertEqual(len(list(self.root.glob('*-mod.*'))), 12)
        self.assertFalse((self.root / 'products.zip').exists())
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)


class CaptureStorageTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.receipt = b'\xef\xbb\xbf{\r\n "status":"passed", "sourceCommit":"fixed", "runId":"123"\r\n}\r\n'
        (self.root / 'validation.json').write_bytes(self.receipt)
        self.original = b'PK\x03\x04' + bytes(range(256)) * 3 + b'\x00\xff\r\n\x00'
        (self.root / 'captures.zip').write_bytes(self.original)

    def tearDown(self):
        self.temp.cleanup()

    def snapshot(self):
        return {path.relative_to(self.root).as_posix(): path.read_bytes()
                for path in self.root.rglob('*') if path.is_file()}

    def test_exact_binary_roundtrip_receipt_and_idempotence(self):
        result = captures.pack(self.root, threshold=100, chunk_size=64)
        record = result['archives'][0]
        self.assertEqual(record['bytes'], len(self.original))
        self.assertEqual(record['sha256'], storage.sha(self.original))
        self.assertTrue(all(part['bytes'] <= 64 for part in record['parts']))
        self.assertEqual((self.root / 'captures-parts/.gitattributes').read_bytes(), b'* -text\n')
        self.assertTrue((self.root / '.gitattributes').read_bytes().endswith(captures.RUN_ATTRIBUTES))
        self.assertFalse((self.root / 'captures.zip').exists())
        self.assertEqual(captures.read_archive(self.root), self.original)
        output = self.root / 'reconstructed.zip'
        self.assertEqual(captures.reconstruct(self.root, output),
                         dict(bytes=len(self.original), sha256=storage.sha(self.original)))
        self.assertEqual(output.read_bytes(), self.original)
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)
        before = self.snapshot()
        self.assertEqual(captures.pack(self.root, threshold=100, chunk_size=64), result)
        self.assertEqual(self.snapshot(), before)
        # A retry after the manifest was persisted but before raw deletion is
        # also idempotent, provided the remaining raw archive is identical.
        (self.root / 'captures.zip').write_bytes(self.original)
        captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual(self.snapshot(), before)

    def test_small_capture_and_unrelated_files_are_unchanged(self):
        (self.root / 'unrelated.zip').write_bytes(self.original * 2)
        before = self.snapshot()
        self.assertIsNone(captures.pack(self.root))
        self.assertEqual(self.snapshot(), before)

    def test_both_capture_archive_names_are_preserved(self):
        failed = self.original[::-1]
        (self.root / 'failure-captures.zip').write_bytes(failed)
        result = captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual([record['file'] for record in result['archives']],
                         ['captures.zip', 'failure-captures.zip'])
        self.assertEqual(captures.read_archive(self.root), self.original)
        self.assertEqual(captures.read_archive(self.root, 'failure-captures.zip'), failed)
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)

    def test_missing_part_is_rejected_without_rewriting_evidence(self):
        result = captures.pack(self.root, threshold=100, chunk_size=64)
        (self.root / result['archives'][0]['parts'][1]['file']).unlink()
        before = self.snapshot()
        with self.assertRaises(ValueError):
            captures.pack(self.root, threshold=100, chunk_size=64)
        with self.assertRaises(ValueError):
            captures.read_archive(self.root)
        with self.assertRaises(ValueError):
            captures.reconstruct(self.root, self.root / 'incomplete.zip')
        self.assertEqual(self.snapshot(), before)

    def test_corrupted_part_is_rejected_without_partial_reconstruction(self):
        result = captures.pack(self.root, threshold=100, chunk_size=64)
        part = self.root / result['archives'][0]['parts'][0]['file']
        part.write_bytes(b'wrong' + part.read_bytes()[5:])
        before = self.snapshot()
        with self.assertRaises(ValueError):
            captures.pack(self.root, threshold=100, chunk_size=64)
        with self.assertRaises(ValueError):
            captures.read_archive(self.root)
        with self.assertRaises(ValueError):
            captures.reconstruct(self.root, self.root / 'incomplete.zip')
        self.assertEqual(self.snapshot(), before)

    def test_changed_receipt_and_conflicting_raw_archive_are_rejected(self):
        captures.pack(self.root, threshold=100, chunk_size=64)
        (self.root / 'validation.json').write_bytes(self.receipt + b' ')
        before = self.snapshot()
        with self.assertRaises(ValueError):
            captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual(self.snapshot(), before)
        (self.root / 'validation.json').write_bytes(self.receipt)
        (self.root / 'captures.zip').write_bytes(b'different')
        with self.assertRaises(ValueError):
            captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual((self.root / 'captures.zip').read_bytes(), b'different')
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)

    def test_raw_archive_must_match_existing_validation_fingerprint(self):
        receipt = dict(status='passed', sourceCommit='fixed', runId='123',
                       captureArchiveSha256='0' * 64)
        (self.root / 'validation.json').write_text(json.dumps(receipt) + '\n')
        before = self.snapshot()
        with self.assertRaises(ValueError):
            captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual(self.snapshot(), before)

    def test_current_run_attributes_preserve_existing_rules_and_are_required(self):
        previous = b'products.zip -text\r\n'
        (self.root / '.gitattributes').write_bytes(previous)
        captures.pack(self.root, threshold=100, chunk_size=64)
        self.assertEqual((self.root / '.gitattributes').read_bytes(), previous + captures.RUN_ATTRIBUTES)
        (self.root / '.gitattributes').write_bytes(previous)
        with self.assertRaises(ValueError):
            captures.read_archive(self.root)
        self.assertEqual((self.root / 'validation.json').read_bytes(), self.receipt)


if __name__ == '__main__':
    unittest.main()
