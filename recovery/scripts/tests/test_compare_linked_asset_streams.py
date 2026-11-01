"""Independent small v6 binary fixtures for read-only stream difference analysis."""
import hashlib
import importlib.util
import json
from pathlib import Path
import struct
import subprocess
import sys
import tempfile
import unittest

SCRIPT = Path(__file__).resolve().parents[1] / 'CompareLinkedAssetStreams.py'
spec = importlib.util.spec_from_file_location('linked_asset_analysis', SCRIPT)
analysis = importlib.util.module_from_spec(spec)
spec.loader.exec_module(analysis)


class LinkedAssetAnalysisTests(unittest.TestCase):
    def test_independent_manifests_and_unexpected_differences_remain_visible(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            header = bytearray(128)
            header[:4] = b'DDS '
            struct.pack_into('<I', header, 4, 124)
            struct.pack_into('<II', header, 12, 4, 4)
            header[84:88] = b'DXT1'
            dds = bytes(header) + b'ABCDEFGH'
            old_texture = struct.pack('<III', 0, 12, len(dds)) + dds
            new_texture = old_texture[:-1] + b'Z'
            texture = (568797146, 1, 'Texture:Probe', old_texture, b'')
            game = (2486173485, 2, 'GameObject:Probe', b'abcd', b'RELO')
            equal = (1, 3, 'Other:Equal', b'equal', b'')
            joypad = (2, 4, 'Joypad:Added', b'joypad', b'')

            # Independent encoding of the documented v6 header/48-byte entries;
            # changed entry order and a new asset force different chunk offsets.
            def write(directory, rows):
                manifest = directory / 'base/mod/mod.manifest'
                manifest.parent.mkdir(parents=True)
                names, entries = b'', b''
                for type_id, instance_id, name, payload, relocations in rows:
                    offset = len(names)
                    names += name.encode() + b'\0'
                    entries += struct.pack('<12I', type_id, instance_id, 123, 456,
                                           0, 0, offset, 0, len(payload), len(relocations), 0, 1)
                source = b'DATA:test.xml\0'
                checksum = 12345
                manifest.write_bytes(struct.pack('<BBH11I', 0, 1, 6, checksum, 999,
                    len(rows), sum(len(row[3]) for row in rows), 999, 4, 0, 0, 0,
                    len(names), len(source)) + entries + names + source)
                for extension, column in (('.bin', 3), ('.relo', 4)):
                    manifest.with_suffix(extension).write_bytes(
                        struct.pack('<I', checksum) + b''.join(row[column] for row in rows))
                manifest.with_suffix('.imp').write_bytes(struct.pack('<I', checksum))
                return manifest

            old = write(root / 'original', [texture, game, equal])
            new = write(root / 'new', [joypad, equal,
                (texture[0], texture[1], texture[2], new_texture, texture[4]),
                (game[0], game[1], game[2], b'abce', b'RELZ')])
            report = analysis.analyze_stream(old, new)
            self.assertEqual([row['name'] for row in report['onlyNew']], ['Joypad:Added'])
            self.assertFalse(report['onlyOld'])
            self.assertFalse(report['manifestEqual'])
            by_name = {row['name']: row for row in report['assets']}
            change = by_name['Texture:Probe']['chunks']['.bin']
            self.assertEqual(change['classification'], 'texture-dds-DXT1-payload-only')
            self.assertEqual(change['differentBytes'], 1)
            self.assertNotEqual(change['oldOffset'], change['newOffset'])
            self.assertEqual(change['firstBlockDifference']['blockIndex'], 0)
            self.assertEqual(by_name['GameObject:Probe']['chunks']['.bin']['classification'],
                             'non-texture-instance-change')
            self.assertIn('.relo', by_name['GameObject:Probe']['chunks'])
            # Identical payload with changed position/name-table offset retains
            # metadata differences rather than disappearing from the report.
            self.assertFalse(by_name['Other:Equal']['chunks'])
            self.assertTrue(by_name['Other:Equal']['metadataFields'])
            baseline = root / 'baseline.json'
            baseline.write_text(json.dumps({'assets': [dict(name='Texture:Probe',
                bytes=len(old_texture), differentBytes=1,
                oldSha256=hashlib.sha256(old_texture).hexdigest(),
                newSha256=hashlib.sha256(new_texture).hexdigest())]}))
            history = analysis.historical_comparison({'base/mod/mod.manifest': report}, baseline)
            self.assertEqual(history['addedDifferingAssets'], ['GameObject:Probe'])
            self.assertFalse(history['exactHistoricalDifferingChunks'])
            # Exercise the public command, including raw manifest/bin/relo changes.
            output = root / 'report.json'
            subprocess.run([sys.executable, str(SCRIPT), '--old-root', str(root/'original'),
                '--new-root', str(root/'new'), '--baseline', str(baseline), '--output', str(output)],
                check=True, capture_output=True, text=True)
            command_report = json.loads(output.read_text())
            self.assertEqual(sorted(command_report['differentFiles']),
                ['base/mod/mod.bin', 'base/mod/mod.manifest', 'base/mod/mod.relo'])
            self.assertTrue(command_report['allSharedStreamsStructurallyValid'])
            self.assertFalse(command_report['rawAllArchivedFilesEqual'])
            self.assertEqual(command_report['archivedCdataFilesOld'], 0)
            # Corrupt companion data must be rejected rather than guessed.
            new.with_suffix('.imp').write_bytes(b'bad!')
            with self.assertRaisesRegex(ValueError, 'checksum'):
                analysis.analyze_stream(old, new)


if __name__ == '__main__':
    unittest.main()
