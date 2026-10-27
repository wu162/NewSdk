import importlib.util
from pathlib import Path
import tempfile
import unittest

spec = importlib.util.spec_from_file_location('benchmark', Path(__file__).with_name('benchmark.py'))
benchmark = importlib.util.module_from_spec(spec)
spec.loader.exec_module(benchmark)


class BenchmarkTests(unittest.TestCase):
    def test_paired_ratio_with_shared_machine_noise(self):
        # Both engines see the same changing machine load. Independent resampling
        # would lose that pairing and incorrectly widen the interval.
        pairs = [dict(old=dict(wallSeconds=x * 2), new=dict(wallSeconds=x))
                 for x in (1, 10, 3, 20, 5, 7, 8, 30, 4, 12)]
        result = benchmark.summarize(pairs)
        self.assertEqual(result['speedup'], 2)
        self.assertEqual(result['speedup95CI'], [2, 2])
        self.assertEqual(result['old']['n'], 10)
        self.assertGreater(result['new']['cv'], .5)

    def test_equal_speed_is_not_a_speedup(self):
        pairs = [dict(old=dict(wallSeconds=x), new=dict(wallSeconds=x)) for x in (4, 8, 12)]
        result = benchmark.summarize(pairs)
        self.assertEqual(result['speedup'], 1)
        self.assertEqual(result['speedup95CI'], [1, 1])

    def test_output_gate_covers_cdata_and_missing_stream_members(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for extension in ('.manifest', '.bin', '.imp', '.relo'):
                (root / ('mod' + extension)).write_bytes(b'original')
            cdata = root / 'mod/cdata/00000001.00000002.00000003.00000004.cdata'
            cdata.parent.mkdir(parents=True)
            cdata.write_bytes(b'audio payload')
            before = benchmark.inventory(root)
            cdata.write_bytes(b'other payload')
            self.assertNotEqual(before, benchmark.inventory(root))
            (root / 'mod.relo').unlink()
            with self.assertRaisesRegex(ValueError, 'Incomplete'):
                benchmark.inventory(root)

    def test_output_gate_rejects_unlisted_files(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for extension in ('.manifest', '.bin', '.imp', '.relo'):
                (root / ('mod' + extension)).touch()
            (root / 'stale.bin').touch()
            with self.assertRaisesRegex(ValueError, 'Unexpected'):
                benchmark.inventory(root)


if __name__ == '__main__':
    unittest.main()
