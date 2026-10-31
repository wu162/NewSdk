import sys
import tempfile
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parents[1] / 'scripts'))
from BenchmarkFullUpheaval import outputs, tasks, cdata_diagnostic
from BenchmarkCompilerSdkComparison import compare_pair


class FullUpheavalChecks(unittest.TestCase):
    def test_cdata_diagnostic_keeps_multiple_variants_of_one_instance(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            metadata, paths = [], []
            for engine, suffixes in (('old', ('aaaa', 'bbbb')), ('new', ('cccc', 'dddd'))):
                mapping, sources = {}, {}
                for index, suffix in enumerate(suffixes):
                    key = 'base/mod/mod/cdata/166b084d.53c81e47.12345678.' + suffix + '.cdata'
                    p = root / (engine + suffix)
                    p.write_bytes(bytes([index]))
                    mapping[key] = {'bytes': 1, 'sha256': str(index)}
                    sources[key] = p
                metadata.append(mapping)
                paths.append(sources)
            result = cdata_diagnostic(*metadata, *paths)
            self.assertEqual(result, {'originalCount': 2, 'newCount': 2, 'commonLogicalIdentities': 1, 'equalPayloads': 2})
    def test_five_streams_cdata_versions_and_unknown_members(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            selected = [{'id': str(i), 'stem': 'stream' + str(i)} for i in range(5)]
            for engine in ('old', 'new'):
                for task in selected:
                    folder = root / engine / ('different-layout-' + task['id'])
                    folder.mkdir(parents=True)
                    for ext in ('.manifest', '.bin', '.imp', '.relo', '.version'):
                        (folder / (task['stem'] + ext)).write_bytes(b'exact bytes')
                    cdata = folder / task['stem'] / 'cdata'
                    cdata.mkdir(parents=True)
                    (cdata / '12345678.cdata').write_bytes(b'payload')
            old, op = outputs(root / 'old', selected)
            new, np = outputs(root / 'new', selected)
            self.assertEqual(len(old), 30)
            compare_pair(old, new, op, np)
            np['0/stream0.version'].write_bytes(b'changed')
            with self.assertRaises(ValueError):
                compare_pair(old, new, op, np)
            (root / 'old/unreported.bin').write_bytes(b'extra')
            with self.assertRaises(ValueError):
                outputs(root / 'old', selected)

    def test_complete_discovery_includes_apt_and_global_metadata(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'Data/AptUI').mkdir(parents=True)
            (root / 'Data/AdditionalMaps').mkdir()
            (root / 'Data/Mod.xml').touch()
            for i in range(3):
                (root / ('Data/AptUI/ui' + str(i) + '.xml')).touch()
            metadata = root / 'Data/AdditionalMaps/MapMetaData_Global.xml'
            metadata.touch()
            self.assertEqual([t['content'] for t in tasks(root)], ['base', 'apt', 'apt', 'apt', 'maps'])
            metadata.unlink()
            with self.assertRaises(ValueError):
                tasks(root)
