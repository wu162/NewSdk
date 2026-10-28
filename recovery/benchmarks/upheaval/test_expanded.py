from pathlib import Path
import struct
import tempfile
import unittest
import aggregate
import expanded


class ExpandedTests(unittest.TestCase):
    def test_mutations_are_real_and_length_preserving(self):
        xml=b'<AmbientStream id="Reachable" Volume = "60"/>'
        self.assertEqual(expanded.mutate(None,'xml',xml),b'<AmbientStream id="Reachable" Volume = "61"/>')
        dds=bytearray(640);dds[:4]=b'DDS ';dds[84:88]=b'DXT1'
        changed=expanded.mutate(None,'texture',bytes(dds))
        self.assertEqual(changed[:128],dds[:128]);self.assertEqual(len(changed),640)
        wav=b'RIFF'+struct.pack('<I',140)+b'WAVEdata'+struct.pack('<I',128)+bytes(range(128))
        changed=expanded.mutate(None,'audio',wav)
        self.assertEqual(changed[:20],wav[:20]);self.assertEqual(len(changed),len(wav));self.assertNotEqual(changed,wav)

    def test_bad_input_cannot_become_a_fake_mutation(self):
        for kind,data in [('xml',b'<Other/>'),('texture',b'DDS '),('audio',b'RIFFbad')]:
            with self.assertRaises(ValueError):expanded.mutate(None,kind,data)

    def test_bootstrap_preserves_hardware_strata_and_pairing(self):
        strata=[[dict(old=dict(wallSeconds=x*2),new=dict(wallSeconds=x)) for x in group]
                for group in ((1,2,3),(100,200,300))]
        row=aggregate.summarize_strata(strata)
        self.assertEqual(row['speedup'],2);self.assertEqual(row['speedup95CI'],[2,2]);self.assertEqual(row['old']['n'],6)
        with self.assertRaises(ValueError):aggregate.summarize_strata([strata[0][:2]])

    def test_aggregate_rejects_duplicate_runner_identity(self):
        import json
        with tempfile.TemporaryDirectory() as folder:
            root=Path(folder)
            for number in (1,2):
                path=root/str(number);path.mkdir()
                (path/'state.json').write_text(json.dumps(dict(replica=number,machine='same-host')))
            with self.assertRaisesRegex(ValueError,'distinct'):aggregate.aggregate(root,root/'output')


if __name__=='__main__':unittest.main()
