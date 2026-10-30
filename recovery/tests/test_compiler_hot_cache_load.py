import sys
from pathlib import Path
import tempfile
import unittest
import xml.etree.ElementTree as ET

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'scripts'))
import TestCompilerHotCacheLoad as load


class LoadChecks(unittest.TestCase):
    def test_fixture_ids_respect_the_actual_case_insensitive_hash(self):
        # These two names were rejected by the actual product in the first run.
        self.assertEqual(load.asset_hash('LoadArmor001016'), 0xd5b4a3c1)
        self.assertEqual(load.asset_hash('LoadArmor000008'), 0xd5b4a3c1)
        names = load.AssetNames().extend(2048)
        self.assertEqual(len({load.asset_hash(name) for name in names}), 2048)

    def test_requires_two_individual_samples_not_their_sum(self):
        self.assertFalse(load.meets_minimum([4.1, 4.2], 8))
        self.assertFalse(load.meets_minimum([8.1, 7.9], 8))
        self.assertFalse(load.meets_minimum([9], 8))
        self.assertTrue(load.meets_minimum([8, 8.2], 8))

    def test_growth_preserves_prior_inputs_and_exact_include_closure(self):
        with tempfile.TemporaryDirectory() as temp:
            mod = Path(temp)
            first = load.grow_fixture(mod, 0, 2)
            before = {p.relative_to(mod): p.read_bytes() for p in (mod / 'Data').rglob('armor-*.xml')}
            second = load.grow_fixture(mod, 2, 4)
            for name, data in before.items():
                self.assertEqual((mod / name).read_bytes(), data)
            ns = '{uri:ea.com:eala:asset}'
            entries = list((mod / 'Data/AdditionalMaps').glob('MapMetaData_*.xml'))
            self.assertEqual(len(entries), load.STREAMS)
            for entry in entries:
                includes = ET.parse(entry).getroot().findall(ns + 'Includes/' + ns + 'Include')
                self.assertEqual(len(includes), 4)
                for node in includes:
                    self.assertTrue((entry.parent / node.get('source')).is_file())
            nodes = [n for p in (mod / 'Data').rglob('armor-*.xml') for n in ET.parse(p).getroot()]
            self.assertEqual(len(nodes), second['assets'])
            self.assertEqual(len({n.get('id') for n in nodes}), second['assets'])
            self.assertEqual(first['assets'], 8 * load.STREAMS)
            self.assertEqual(second['newDocuments'], 2 * load.STREAMS)


if __name__ == '__main__':
    unittest.main()
