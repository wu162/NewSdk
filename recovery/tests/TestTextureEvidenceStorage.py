import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
import zipfile

spec = importlib.util.spec_from_file_location('storage', Path(__file__).parents[1] / 'scripts/PackTextureSdkProducts.py')
storage = importlib.util.module_from_spec(spec)
spec.loader.exec_module(storage)


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


if __name__ == '__main__':
    unittest.main()
