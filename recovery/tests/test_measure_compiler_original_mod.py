import contextlib
import hashlib
import io
import json
import os
from pathlib import Path
import sys
import tempfile
import types
import unittest
from unittest import mock

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'scripts'))
import MeasureCompilerOriginalMod as meter


class OriginalModMeterContracts(unittest.TestCase):
    def test_historical_identity_defaults_are_preserved(self):
        identity = meter.binary_identity()
        self.assertEqual(identity, dict(sourceCommit=meter.SOURCE, runId=meter.RUN, runtimeFiles=meter.FILES))
        self.assertIsNot(identity['runtimeFiles'], meter.FILES)

    def test_explicit_binary_manifest(self):
        with tempfile.TemporaryDirectory() as temporary:
            path = Path(temporary) / 'identity.json'
            expected = dict(sourceCommit='1' * 40, runId=123, runtimeFiles={name: '2' * 64 for name in meter.FILES})
            path.write_text(json.dumps(expected))
            identity = meter.binary_identity(path)
            self.assertEqual(identity['sourceCommit'], expected['sourceCommit'])
            self.assertEqual(identity['runId'], '123')
            self.assertEqual(identity['runtimeFiles'], expected['runtimeFiles'])

    def test_incomplete_runtime_manifest_is_rejected(self):
        with tempfile.TemporaryDirectory() as temporary:
            path = Path(temporary) / 'identity.json'
            path.write_text(json.dumps(dict(sourceCommit='1' * 40, runId=123,
                                           runtimeFiles={'NewModCompiler.exe': '2' * 64})))
            with self.assertRaisesRegex(ValueError, 'three-file'):
                meter.binary_identity(path)

    def test_overlapping_owner_intervals_use_union_and_exact_streams(self):
        tasks = [dict(id=name, content=name.split('/')[0]) for name in ('base/Mod', 'apt/One', 'maps/MapMetaData_One')]
        intervals = [dict(streamId=task['id'], startedMs=start, completedMs=end)
                     for task, start, end in zip(tasks, (0, 2, 3), (10, 5, 8))]
        result = meter.stream_timing_summary(Path('.'), tasks, dict(contentExecution=dict(groups=[dict(streams=intervals)])))
        self.assertEqual(result['partWallMs'], dict(base=10, apt=3, maps=5))
        self.assertEqual(result['streamIntervalUnionMs'], 10)
        self.assertEqual(result['peakConcurrentStreams'], 3)
        with self.assertRaisesRegex(ValueError, 'Missing or duplicate'):
            meter.stream_timing_summary(Path('.'), tasks, dict(contentExecution=dict(groups=[dict(streams=intervals[:1])])) )

    def test_hot_failure_keeps_cold_raw_difference_and_other_engine_result(self):
        # Run the actual meter orchestration with tiny file-backed runtimes and
        # process results. Neither subcase invokes a compiler or Windows API.
        for failing_engine in ('original', 'new'):
            with self.subTest(failing_engine=failing_engine), tempfile.TemporaryDirectory() as temporary:
                root = Path(temporary)
                source = '1' * 40
                product = root / 'product'
                runtime = product / 'bin'
                runtime.mkdir(parents=True)
                for name in meter.FILES:
                    (runtime / name).write_bytes(name.encode())
                files = {path.name: hashlib.sha256(path.read_bytes()).hexdigest() for path in runtime.iterdir()}
                (product / 'build.json').write_text(json.dumps(dict(sourceCommit=source, configuration='Release', exeSha256=files['NewModCompiler.exe'])))
                manifest = root / 'identity.json'
                manifest.write_text(json.dumps(dict(sourceCommit=source, runId=123, runtimeFiles=files)))
                mod = root / 'Input'
                (mod / 'Data').mkdir(parents=True)
                (mod / 'Data/Mod.xml').write_text('<AssetDeclaration xmlns="uri:ea.com:eala:asset"/>')
                calls = []

                def prepare(preparation, destination, repo, accepted_source, accepted_run):
                    shared = json.loads(Path(preparation).read_text())
                    original = destination / 'original-runtime'
                    original.mkdir(parents=True)
                    (original / 'BAB.exe').write_bytes(b'original')
                    for workload in shared['workloads']:
                        workload['effectiveInputRoots'] = dict(dataRoot=str(repo / 'SDK'), dataPaths=[workload['modRoot'] + '/Data'], artPaths=[], audioPaths=[])
                    result = destination / 'original-preparation.json'
                    result.write_text(json.dumps(dict(workloads=shared['workloads'], originalRuntimeDirectory=str(original), originalExe=str(original / 'BAB.exe'))))
                    return result

                def measure(command, cwd, evidence, timeout, environment):
                    engine, profile = command[0], Path(command[3])
                    phase = 'hot' if '-hot-' in evidence.name else 'cold'
                    calls.append((engine, phase))
                    evidence.mkdir(parents=True)
                    row = dict(wallMs=100.0, cpuMs=80.0, peakWorkingSetBytes=1024, exitCode=0, status='exited',
                               traceSessionLoaded=phase == 'hot', traceStringHashesLoaded=phase == 'hot')
                    cache = dict(sessionCache=True, compressedSessionCache=True, hits=0, misses=0,
                                 documentReuse=1 if phase == 'hot' else 0, xmlParse=0, instanceCompile=0)
                    if engine == failing_engine and phase == 'hot':
                        row.update(status='failed', exitCode=4)
                        (evidence / 'process.json').write_text(json.dumps(row))
                        if engine == 'new':
                            (profile / 'product/build-report.json').write_text(json.dumps(dict(status='failed', cache=cache, elapsedMs=100, diagnosticCounts=dict(error=1))))
                        raise ValueError('AlliedLegionnaireInfantry/BaseInfantry source XML is missing')
                    destination = profile / ('product/published' if engine == 'new' else 'streams')
                    destination.mkdir(parents=True, exist_ok=True)
                    artifacts = []
                    for name in ('mod.bin', 'mod.manifest', 'mod.imp', 'mod.relo'):
                        path = destination / name
                        path.write_bytes((name + ('BC1-different' if engine == 'new' and name == 'mod.bin' else '')).encode())
                        artifacts.append(dict(Path=str(path), Size=path.stat().st_size))
                    if engine == 'new':
                        body = dict(exitCode=0, status='succeeded', publicationStatus='PublishedDirectory', executionMode='InProcess', externalTools=[],
                                    effectiveOptions=dict(bc=dict(Value='true')), cache=cache, artifacts=artifacts, elapsedMs=100)
                        (profile / 'product/build-report.json').write_text(json.dumps(body))
                        (profile / 'product/build.events.jsonl').write_text('')
                    (evidence / 'process.json').write_text(json.dumps(row))
                    return row

                arguments = ['meter', '--accepted-build', str(product), '--binary-manifest', str(manifest), '--mod', str(mod),
                             '--work', str(root / 'work'), '--output', str(root / 'out'), '--pairs', '1', '--hot-rounds', '1',
                             '--skip-extra-uncached', '--first-engine', 'new', '--continue-on-build-failure', '--budget-seconds', '60']
                windows = types.SimpleNamespace(name='nt', environ=os.environ, pathsep=os.pathsep, cpu_count=os.cpu_count)
                patches = dict(os=windows, prepare=prepare, measure=measure, sdk_source_inventory=lambda *args: {},
                               validate_shared_report=lambda *args: None, git=lambda repo, *args: source if args == ('rev-parse', 'HEAD') else '',
                               subprocess=types.SimpleNamespace(run=lambda *args, **kwargs: None),
                               invocation=lambda engine, workload, mode, profile, index, prepared, original: [engine, '--package-content', 'base', str(profile)])
                with mock.patch.multiple(meter, **patches), mock.patch.object(meter.ctypes, 'WinDLL', lambda *args, **kwargs: types.SimpleNamespace(SetErrorMode=lambda value: None), create=True), mock.patch.object(sys, 'argv', arguments), contextlib.redirect_stdout(io.StringIO()):
                    result = meter.main()
                state = json.loads((root / 'out/measurement.json').read_text())
                self.assertEqual(result, 1)
                self.assertEqual(state['status'], 'failed')
                self.assertEqual(state['outputComparisonStatus'], 'outputs-differ')
                self.assertEqual(calls, [('new', 'cold'), ('original', 'cold'), ('original', 'hot'), ('new', 'hot')])
                self.assertEqual(len(state['samples']), 4)
                self.assertEqual([row['phase'] for row in state['summary']], ['cold'])
                self.assertEqual(state['pairs'][0]['differentMembers'], ['mod.bin'])
                self.assertFalse(state['pairs'][0]['rawBytesEqual'])
                self.assertEqual([(row['engine'], row['phase']) for row in state['errors']], [(failing_engine, 'hot')])
                self.assertNotEqual((root / 'out/baseline-streams/Input/new/mod.bin').read_bytes(),
                                    (root / 'out/baseline-streams/Input/original/mod.bin').read_bytes())


if __name__ == '__main__':
    unittest.main()
