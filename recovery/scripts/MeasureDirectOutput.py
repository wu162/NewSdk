#!/usr/bin/env python3
"""Same-VM before/after BIG bytes, wall time and Windows logical process I/O.

All byte comparisons and hashes run in this controller after the child exits.
Process I/O includes runtime loading and diagnostics, and is not physical disk I/O.
"""
import argparse
import ctypes
from ctypes import wintypes
import hashlib
import json
import os
from pathlib import Path
import statistics
import subprocess
import time

from TestCompilerHotCache import need, prepare


class IoCounters(ctypes.Structure):
    _fields_ = [(name, ctypes.c_ulonglong) for name in (
        'readOperations', 'writeOperations', 'otherOperations',
        'readBytes', 'writeBytes', 'otherBytes')]


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def measure(args):
    need(os.name == 'nt', 'Windows process I/O counters are required')
    repo = Path(__file__).resolve().parents[2]
    work, out = Path(args.work).resolve(), Path(args.output).resolve()
    need(not work.exists() and not out.exists(), 'Refusing to overwrite evidence')
    work.mkdir(parents=True)
    out.mkdir(parents=True)
    mod = Path(prepare(repo, work / 'mods')['medium']['modRoot'])
    additional = mod / 'Additional' / 'payload.bin'
    additional.parent.mkdir()
    executables = {name: Path(getattr(args, name)).resolve() for name in ('before', 'after')}
    api = ctypes.WinDLL('kernel32', use_last_error=True)
    api.GetProcessIoCounters.argtypes = [wintypes.HANDLE, ctypes.POINTER(IoCounters)]
    api.GetProcessIoCounters.restype = wintypes.BOOL
    api.SetErrorMode(0x0001 | 0x0002 | 0x8000)
    state = {'status': 'started', 'runId': os.getenv('GITHUB_RUN_ID'),
             'sourceCommit': args.source_commit, 'baselineCommit': args.baseline_commit,
             'binaries': {name: {'path': str(path), 'sha256': digest(path)} for name, path in executables.items()},
             'scope': 'Synthetic 256 Armor, 8 Texture, 2 PCM Audio, 2 animation and 1 hierarchy base stream; Additional absent/16 MiB. Not a full MOD benchmark.',
             'ioMeaning': 'Windows GetProcessIoCounters logical process transfers; includes runtime loading and report/log I/O, excludes controller comparisons; OS page cache uncontrolled.',
             'samples': [], 'medians': []}
    started = time.perf_counter()
    expected = {}

    def sample(engine, scenario, phase, round_index):
        label = f'{scenario}-{round_index}-{phase}-{engine}'
        evidence = out / label
        evidence.mkdir()
        destination = work / 'builds' / scenario / engine / (f'none-{round_index}' if phase == 'disabled' else f'cached-{round_index}')
        enabled = phase != 'disabled'
        command = [str(executables[engine]), 'build', '--mod', str(mod), '--sdk-root', str(repo / 'SDK'),
                   '--out', str(destination), '--package-content', 'base', '--package-format', 'big',
                   '--include-additional=true', '--jobs=4', '--performance=summary',
                   '--bc=' + str(enabled).lower(), '--sc=' + str(enabled).lower()]
        need(time.perf_counter() - started < args.budget, 'Benchmark budget exhausted')
        tick = time.perf_counter()
        with (evidence / 'stdout.log').open('wb') as stdout, (evidence / 'stderr.log').open('wb') as stderr:
            with subprocess.Popen(command, cwd=executables[engine].parent, stdout=stdout, stderr=stderr) as child:
                child.wait(timeout=args.timeout)
                elapsed = (time.perf_counter() - tick) * 1000
                counters = IoCounters()
                need(api.GetProcessIoCounters(int(child._handle), ctypes.byref(counters)), 'GetProcessIoCounters failed')
                exit_code = child.returncode
        report = json.loads((destination / 'build-report.json').read_text(encoding='utf-8-sig'))
        (evidence / 'build-report.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
        row = {'engine': engine, 'scenario': scenario, 'phase': phase, 'round': round_index,
               'wallMs': elapsed, 'exitCode': exit_code, 'io': {name: getattr(counters, name) for name, _ in counters._fields_}}
        state['samples'].append(row)
        need(exit_code == report['exitCode'] == 0, label + ' failed')
        packages = {Path(item['Path']).name: Path(item['Path']).read_bytes()
                    for item in report['artifacts'] if item['Kind'] in ('package.big', 'package.skudef')}
        need(len(packages) == 2, label + ' omitted BIG/SKUDEF')
        if scenario not in expected:
            expected[scenario] = packages
        need(packages == expected[scenario], label + ' BIG/SKUDEF bytes differ')
        entries = report['performance']['entries']
        row['publicationMs'] = sum(entry['totalMs'] for entry in entries if entry['name'] == 'stage.publish')
        row['packageMs'] = sum(entry['totalMs'] for entry in entries if entry['name'] == 'stage.package')
        row['outputBytes'] = sum(map(len, packages.values()))
        row['rawBytesEqual'] = True
        row['cache'] = report['cache']
        print(label + ' passed: %.1f ms' % elapsed, flush=True)

    try:
        for scenario in ('compiled-only', 'additional-16MiB'):
            if scenario == 'additional-16MiB':
                additional.write_bytes(bytes(range(256)) * (16 * 1024 * 1024 // 256))
            for round_index in range(3):
                order = ('before', 'after') if round_index % 2 == 0 else ('after', 'before')
                for phase in ('disabled', 'cold', 'hot'):
                    for engine in order:
                        sample(engine, scenario, phase, round_index)
        for scenario in ('compiled-only', 'additional-16MiB'):
            for phase in ('disabled', 'cold', 'hot'):
                for engine in ('before', 'after'):
                    rows = [row for row in state['samples'] if (row['scenario'], row['phase'], row['engine']) == (scenario, phase, engine)]
                    state['medians'].append({'engine': engine, 'scenario': scenario, 'phase': phase, 'n': len(rows),
                        **{name: statistics.median(row[name] for row in rows) for name in ('wallMs', 'publicationMs', 'packageMs')},
                        **{name: statistics.median(row['io'][name] for row in rows) for name in ('readBytes', 'writeBytes')}})
        state['status'] = 'passed'
    except Exception as error:
        state.update(status='failed', error=str(error))
        raise
    finally:
        state['elapsedSeconds'] = time.perf_counter() - started
        (out / 'measurement.json').write_text(json.dumps(state, indent=2), encoding='utf-8')
        lines = ['# Direct output paired measurement', '', 'Status: ' + state['status'], '', state['scope'], '', state['ioMeaning'], '',
                 '| Scenario | Phase | Engine | n | Wall ms | Package ms | Publish ms | Logical read MiB | Logical write MiB |',
                 '| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |']
        for row in state['medians']:
            lines.append('| {scenario} | {phase} | {engine} | {n} | {wallMs:.2f} | {packageMs:.2f} | {publicationMs:.2f} | {read:.2f} | {write:.2f} |'.format(
                **row, read=row['readBytes'] / 1048576, write=row['writeBytes'] / 1048576))
        (out / 'measurement.md').write_text('\n'.join(lines) + '\n', encoding='utf-8')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--before', required=True)
    parser.add_argument('--after', required=True)
    parser.add_argument('--work', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--baseline-commit', required=True)
    parser.add_argument('--budget', type=float, default=120)
    parser.add_argument('--timeout', type=float, default=20)
    measure(parser.parse_args())
