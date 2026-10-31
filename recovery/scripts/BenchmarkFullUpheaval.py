"""Original/new SDK comparison for every stream of the untouched full Upheaval."""
import argparse
import csv
import ctypes
import json
import os
from pathlib import Path
import shutil
import statistics
import subprocess
import tarfile
import time
import xml.etree.ElementTree as ET

from BenchmarkCompilerCache import inventory, output_inventory, read_json, require
from BenchmarkCompilerSdkComparison import invocation, measure, path_identity, compare_pair, write_json
from PrepareCompilerCacheBenchmark import HISTORICAL, UPHEAVAL_TREE, git
from PrepareCompilerSdkBenchmark import effective_roots


def tasks(mod):
    result = [('base', mod / 'Data/Mod.xml')]
    for content, directory, pattern in (('apt', 'AptUI', '*.xml'),
                                        ('maps', 'AdditionalMaps', 'MapMetaData_*.xml')):
        result += [(content, p) for p in sorted((mod / 'Data' / directory).glob(pattern))]
    require(len(result) == 5, 'Pinned full Upheaval must have all five streams')
    return [{'id': content + '/' + p.stem.lower(), 'content': content, 'input': str(p),
             'stem': p.stem.lower()} for content, p in result]


def outputs(directory, selected):
    root = Path(directory)
    files = [p for p in root.rglob('*') if p.is_file()]
    accepted, normalized, paths = set(), {}, {}
    manifests = [p for p in files if p.suffix.lower() == '.manifest']
    require(len(manifests) == len(selected), 'Full MOD manifest count differs')
    for task in selected:
        matches = [p for p in manifests if p.stem.lower() == task['stem']]
        require(len(matches) == 1, 'Missing or duplicate stream: ' + task['id'])
        manifest = matches[0]
        siblings = {p.suffix.lower(): p for p in files if p.parent == manifest.parent
                    and p.stem.lower() == task['stem']}
        require(set(siblings) >= {'.manifest', '.bin', '.imp', '.relo'}, 'Incomplete four-file stream')
        members = [siblings[s] for s in ('.manifest', '.bin', '.imp', '.relo')]
        if '.version' in siblings:
            members.append(siblings['.version'])
        cdata = manifest.parent / manifest.stem / 'cdata'
        members += [p for p in files if p.is_relative_to(cdata)]
        for p in members:
            key = task['id'] + '/' + p.relative_to(manifest.parent).as_posix().lower()
            require(key not in normalized, 'Duplicate output identity')
            content = p.read_bytes()
            import hashlib
            normalized[key] = {'bytes': len(content), 'sha256': hashlib.sha256(content).hexdigest()}
            paths[key] = p
            accepted.add(p)
    require(accepted == set(files), 'Unexpected unreported full MOD output members')
    return normalized, paths


def save(out, state):
    rows = []
    for phase in ('uncached', 'cold', 'hot'):
        engines = {e: [s for s in state['samples'] if s['phase'] == phase and s['engine'] == e
                       and s['status'] == 'passed'] for e in ('original', 'new')}
        if not engines['original'] or len(engines['original']) != len(engines['new']):
            continue
        row = {'phase': phase, 'nPerEngine': len(engines['original'])}
        for engine, samples in engines.items():
            for metric in ('wallMs', 'cpuMs', 'peakWorkingSetBytes'):
                row[engine + metric[0].upper() + metric[1:] + 'Median'] = statistics.median(s[metric] for s in samples)
            row[engine + 'WallMsMin'] = min(s['wallMs'] for s in samples)
            row[engine + 'WallMsMax'] = max(s['wallMs'] for s in samples)
        row['newWallChangePercent'] = (row['newWallMsMedian'] / row['originalWallMsMedian'] - 1) * 100
        rows.append(row)
    state['summary'] = rows
    write_json(out / 'full-upheaval.json', state)
    if rows:
        with (out / 'full-upheaval.csv').open('w', newline='') as f:
            writer = csv.DictWriter(f, fieldnames=rows[0].keys())
            writer.writeheader()
            writer.writerows(rows)
    lines = ['# Full untouched Upheaval original/new SDK', '', 'Status: ' + state['status'], '',
             'All five streams, base/APT/global metadata, loose outputs including CDATA/version files.',
             'Same physical inputs and roots, separate caches. Original runs sequential BAB processes; new uses one process and jobs=auto.',
             'Launch-to-exit costs include reporting; preparation and output verification are outside timing. No BIG/LoD packaging or game-runtime timing.', '',
             '| Phase | n/engine | Old ms | New ms | New change |', '| --- | ---: | ---: | ---: | ---: |']
    for r in rows:
        lines.append('| {phase} | {nPerEngine} | {originalWallMsMedian:.2f} | {newWallMsMedian:.2f} | {newWallChangePercent:+.2f}% |'.format(**r))
    if state.get('error'):
        lines += ['', 'Failure retained: ' + state['error']]
    (out / 'full-upheaval.md').write_text('\n'.join(lines) + '\n')


def run(args):
    require(os.name == 'nt', 'Full paired measurement requires Windows')
    ctypes.windll.kernel32.SetErrorMode(0x0001 | 0x0002 | 0x8000)
    prepared, old = read_json(args.preparation), read_json(args.original_preparation)
    repo, work, out = (Path(v).resolve() for v in (args.repo, args.work_directory, args.output))
    require(not work.exists() and not out.exists(), 'Refusing to overwrite prior results')
    work.mkdir(parents=True)
    out.mkdir(parents=True)
    require(git(repo, 'rev-parse', HISTORICAL + ':SDK/Mods/Upheaval').stdout.decode().strip() == UPHEAVAL_TREE,
            'Full historical MOD tree differs')
    archive = work / 'mod.tar'
    with archive.open('wb') as f:
        subprocess.run(['git', '-C', str(repo), 'archive', HISTORICAL, 'SDK/Mods/Upheaval'], stdout=f, check=True, timeout=90)
    with tarfile.open(archive) as f:
        f.extractall(work / 'input', filter='data')
    archive.unlink()
    mod, sdk = work / 'input/SDK/Mods/Upheaval', Path(prepared['sdkRoot'])
    selected = tasks(mod)
    roots, missing = effective_roots(sdk, mod, ET.parse(sdk / 'tools/BinaryAssetBuilder.exe.config').getroot().find('assetbuilder'))
    workload = {'modRoot': str(mod), 'effectiveInputRoots': roots}
    runtime = Path(prepared['runtimeDirectory'])
    original_runtime = Path(old['originalRuntimeDirectory'])
    initial = {'mod': inventory(mod), 'sdk': inventory(sdk), 'newRuntime': inventory(runtime),
               'originalRuntime': inventory(original_runtime)}
    for key, rows in (('newRuntime', prepared['runtimeFiles']), ('originalRuntime', old['originalRuntimeFiles'])):
        require(initial[key] == {r['name']: {'bytes': r['bytes'], 'sha256': r['sha256']} for r in rows}, 'Runtime identity differs')
    write_json(out / 'input-identities.json', initial)
    for name, path in (('preparation.json', args.preparation), ('original-preparation.json', args.original_preparation)):
        shutil.copyfile(path, out / name)
    state = {'status': 'running', 'runId': os.getenv('GITHUB_RUN_ID'), 'runAttempt': os.getenv('GITHUB_RUN_ATTEMPT'),
             'binarySourceCommit': prepared['binarySourceCommit'], 'sourceRunId': prepared['sourceRunId'],
             'benchmarkSourceCommit': git(repo, 'rev-parse', 'HEAD').stdout.decode().strip(),
             'inputSourceCommit': HISTORICAL, 'inputTree': UPHEAVAL_TREE, 'fullModInputCopy': True,
             'inputFiles': len(initial['mod']), 'inputBytes': sum(r['bytes'] for r in initial['mod'].values()),
             'tasks': selected, 'effectiveInputRoots': roots, 'missingInputRoots': missing,
             'newRuntimeFiles': prepared['runtimeFiles'], 'originalRuntimeFiles': old['originalRuntimeFiles'],
             'scope': 'Complete untouched historical Upheaval, all five streams and assets, loose output. No BIG/LoD packaging.',
             'limitations': ['OS page cache uncontrolled; compiler cold means empty compiler caches',
                             'Small sample counts on one Windows VM', 'New default jobs=auto versus original sequential BAB',
                             'Original total sums process launch-to-exit times; Python bookkeeping and version-marker writes excluded',
                             'No game-runtime or BIG/LoD packaging timings; no warmup'], 'samples': [], 'pairs': []}
    environment = os.environ.copy()
    for key in list(environment):
        if key.startswith('RA3_RECOVERED_') or key == 'RECOVERED_W3X_TRACE_DIRECTORY':
            del environment[key]
    environment['PATH'] = os.pathsep.join(p for p in environment.get('PATH', '').split(os.pathsep)
                                          if '/sdk/tools' not in p.replace('\\', '/').lower())
    started = time.perf_counter()
    baseline = None

    def sample(engine, phase, index):
        mode = 'none' if phase == 'uncached' else 'both-deflate'
        profile = work / ('none-' + str(index) if phase == 'uncached' else
                          'cold-' + str(index) if phase == 'cold' else 'cold-' + str(args.cold_runs - 1)) / engine
        label = phase + '-' + str(index) + '-' + engine
        evidence = out / label
        command = invocation(engine, workload, mode, profile, index, prepared, old)
        process_rows = []
        if engine == 'new':
            pos = command.index('--package-content')
            del command[pos:pos + 2]  # Default discovers base, all APT UI and global metadata.
            process_rows.append(measure(command, sdk, evidence, min(args.timeout_seconds, args.budget_seconds - (time.perf_counter() - started)), environment))
            report = read_json(profile / 'product/build-report.json')
            write_json(evidence / 'report.json', report)
            require(report['status'] == 'succeeded' and report['exitCode'] == 0, 'New build failed')
            require({path_identity(t['InputPath']) for t in report['plan']['Tasks']} ==
                    {path_identity(t['input']) for t in selected} and len(report['plan']['Tasks']) == len(selected), 'New task coverage differs')
            for key in ('dataRoot', 'dataPaths', 'artPaths', 'audioPaths'):
                actual, expected = report['inputRoots'][key], roots[key]
                require(([path_identity(p) for p in actual] if isinstance(actual, list) else path_identity(actual)) ==
                        ([path_identity(p) for p in expected] if isinstance(expected, list) else path_identity(expected)), 'New search roots differ')
            output_inventory(profile / 'product', report)
            if phase == 'hot':
                require(report['cache']['instanceCompile'] == 0 and report['cache']['xmlParse'] == 0 and
                        report['cache']['documentReuse'] > 0, 'Full hot session was not reused')
            directory = profile / 'product/published'
        else:
            for task in selected:
                task_profile = profile / task['stem']
                task_profile.mkdir(parents=True, exist_ok=True)
                for name in ('streams', 'assets', 'session', 'intermediate', 'fresh-' + str(index), 'fresh-session-' + str(index)):
                    (task_profile / name).mkdir(exist_ok=True)
                command = invocation(engine, workload, mode, task_profile, index, prepared, old)
                command[1] = task['input']
                remaining = args.budget_seconds - (time.perf_counter() - started)
                require(remaining > 0, 'Total full MOD budget exhausted')
                row = measure(command, sdk, evidence / task['stem'], min(args.timeout_seconds, remaining), environment)
                require(phase != 'hot' or row['traceSessionLoaded'], 'Original hot stream session was not loaded')
                require(mode != 'none' or not row['traceStringHashesLoaded'], 'Original no-cache reused string hashes')
                process_rows.append(row)
                if task['content'] == 'apt':
                    manifest = list((task_profile / 'streams').rglob('*.manifest'))
                    require(len(manifest) == 1, 'Original APT output ambiguous')
                    manifest[0].with_suffix('.version').write_bytes(b'\r\n')
            directory = profile
            # Collect only output streams; caches remain outside the compared output directory.
            directory = profile / 'all-streams'
            if directory.exists():
                shutil.rmtree(directory)
            for task in selected:
                shutil.copytree(profile / task['stem'] / 'streams', directory / task['stem'])
            report = None
        files, paths = outputs(directory, selected)
        result = {'engine': engine, 'phase': phase, 'round': index, 'status': 'validated',
                  'wallMs': sum(p['wallMs'] for p in process_rows), 'cpuMs': sum(p['cpuMs'] for p in process_rows),
                  'peakWorkingSetBytes': max(p['peakWorkingSetBytes'] for p in process_rows),
                  'processes': process_rows, 'outputFiles': files, 'cache': report['cache'] if report else None}
        state['samples'].append(result)
        return result, paths

    schedule = [('uncached', i) for i in range(args.cold_runs)] + [('cold', i) for i in range(args.cold_runs)] + [('hot', i) for i in range(args.hot_runs)]
    try:
        for pair_index, (phase, index) in enumerate(schedule):
            order = ('original', 'new') if pair_index % 2 == 0 else ('new', 'original')
            pair = {engine: sample(engine, phase, index) for engine in order}
            left, right = pair['original'], pair['new']
            compare_pair(left[0]['outputFiles'], right[0]['outputFiles'], left[1], right[1])
            if baseline is None:
                baseline = left[0]['outputFiles']
                for engine in order:
                    for name, path in pair[engine][1].items():
                        target = out / 'baseline-outputs' / engine / name
                        target.parent.mkdir(parents=True, exist_ok=True)
                        shutil.copyfile(path, target)
            require(left[0]['outputFiles'] == baseline, 'Full cold/hot output changed')
            for engine in order:
                pair[engine][0]['status'] = 'passed'
            state['pairs'].append({'phase': phase, 'round': index, 'order': order, 'rawBytesEqual': True})
            save(out, state)
            print('FULL_UPHEAVAL_PAIR ' + json.dumps({'phase': phase, 'round': index, 'oldMs': left[0]['wallMs'], 'newMs': right[0]['wallMs']}), flush=True)
        state['status'] = 'passed'
    except Exception as error:
        state['status'], state['error'] = 'failed', type(error).__name__ + ': ' + str(error)
    finally:
        state['elapsedSeconds'] = time.perf_counter() - started
        state['inputsUnchanged'] = (initial == {'mod': inventory(mod), 'sdk': inventory(sdk), 'newRuntime': inventory(runtime), 'originalRuntime': inventory(original_runtime)})
        require(state['inputsUnchanged'], 'MOD/SDK/runtime input changed')
        save(out, state)
        print('FULL_UPHEAVAL_RESULT ' + json.dumps({k: state.get(k) for k in ('status', 'error', 'elapsedSeconds', 'summary')}), flush=True)
    return state['status'] == 'passed'


if __name__ == '__main__':
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--preparation', required=True)
    p.add_argument('--original-preparation', required=True)
    p.add_argument('--repo', default='.')
    p.add_argument('--output', required=True)
    p.add_argument('--work-directory', required=True)
    p.add_argument('--timeout-seconds', type=int, default=180)
    p.add_argument('--budget-seconds', type=int, default=900)
    p.add_argument('--cold-runs', type=int, choices=range(1, 4), default=2)
    p.add_argument('--hot-runs', type=int, choices=range(1, 6), default=3)
    raise SystemExit(0 if run(p.parse_args()) else 1)
