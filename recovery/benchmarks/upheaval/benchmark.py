#!/usr/bin/env python3
"""Full unchanged Upheaval, clean compile/link, paired process benchmarks on Windows."""
import argparse
import ctypes
from ctypes import wintypes
import hashlib
import json
import math
import os
from pathlib import Path
import platform
import random
import re
import shutil
import statistics
import subprocess
import tempfile
import tarfile
import time
import traceback
import xml.etree.ElementTree as ET

MOD_SOURCE = 'bacfc69fc3e71c8116d1f22b933b8b6173170ecf'
MOD_TREE = '5551fc26a6f96b62613443e0d0fa2958c14cbce0'


def digest(path):
    h = hashlib.sha256()
    with Path(path).open('rb') as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b''):
            h.update(block)
    return h.hexdigest()


def snapshot(root):
    return {p.relative_to(root).as_posix(): dict(bytes=p.stat().st_size, sha256=digest(p))
            for p in sorted(root.rglob('*')) if p.is_file()}


def write(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2) + '\n', encoding='utf-8')


def inventory(root):
    result = snapshot(root)
    manifests = [n[:-9] for n in result if n.endswith('.manifest')]
    if not manifests:
        raise ValueError('No linked manifest: ' + str(root))
    accepted = {stem + ext for stem in manifests for ext in ('.manifest', '.bin', '.imp', '.relo')}
    if not accepted <= result.keys():
        raise ValueError('Incomplete linked stream: ' + str(root))
    for name in result.keys() - accepted:
        if not any(re.fullmatch(re.escape(stem) + r'/cdata/(?:[0-9a-fA-F]{8}\.){4}cdata', name)
                   for stem in manifests):
            raise ValueError('Unexpected linked output: ' + name)
    return result


def metrics(process):
    # Query the still-open Windows process handle after exit. Wall time is the
    # primary metric; CPU and peak working set are supporting process metrics.
    class Counters(ctypes.Structure):
        _fields_ = [('cb', wintypes.DWORD), ('PageFaultCount', wintypes.DWORD)] + [
            (n, ctypes.c_size_t) for n in ('PeakWorkingSetSize', 'WorkingSetSize',
            'QuotaPeakPagedPoolUsage', 'QuotaPagedPoolUsage', 'QuotaPeakNonPagedPoolUsage',
            'QuotaNonPagedPoolUsage', 'PagefileUsage', 'PeakPagefileUsage')]
    counters = Counters()
    counters.cb = ctypes.sizeof(counters)
    handle = wintypes.HANDLE(int(process._handle))
    if not ctypes.windll.psapi.GetProcessMemoryInfo(handle, ctypes.byref(counters), counters.cb):
        raise ctypes.WinError()
    times = [wintypes.FILETIME() for _ in range(4)]
    if not ctypes.windll.kernel32.GetProcessTimes(handle, *(ctypes.byref(t) for t in times)):
        raise ctypes.WinError()
    ticks = lambda t: (t.dwHighDateTime << 32) + t.dwLowDateTime
    return dict(cpuSeconds=(ticks(times[2]) + ticks(times[3])) / 10_000_000,
                peakWorkingSetBytes=counters.PeakWorkingSetSize)


def execute(exe, arguments, cwd, logs, timeout, environment):
    logs.mkdir(parents=True)
    write(logs / 'command.json', dict(exe=str(exe), arguments=arguments, cwd=str(cwd)))
    with (logs / 'stdout.log').open('wb') as stdout, (logs / 'stderr.log').open('wb') as stderr:
        started = time.perf_counter()
        process = subprocess.Popen([str(exe), *arguments], cwd=cwd, env=environment,
                                   stdout=stdout, stderr=stderr)
        try:
            code = process.wait(timeout=timeout)
        except subprocess.TimeoutExpired:
            subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'],
                           stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            process.wait()
            write(logs / 'process.json', dict(succeeded=False, timedOut=True,
                  wallSeconds=time.perf_counter() - started))
            raise
        elapsed = time.perf_counter() - started
        result = dict(wallSeconds=elapsed, exitCode=code, pid=process.pid, **metrics(process))
    write(logs / 'process.json', result)
    text = '\n'.join((logs / n).read_text(encoding='utf-8-sig', errors='replace')
                     for n in ('stdout.log', 'stderr.log'))
    if code or re.search(r'^\s*(Critical|Error):', text, re.M | re.I):
        raise ValueError('Compiler failed; see ' + str(logs))
    return result


def describe(values):
    ordered = sorted(values)
    p = (len(ordered) - 1) * .95
    low = math.floor(p)
    p95 = ordered[low] + (ordered[math.ceil(p)] - ordered[low]) * (p - low)
    mean = statistics.mean(values)
    sd = statistics.stdev(values) if len(values) > 1 else 0
    return dict(n=len(values), mean=mean, median=statistics.median(values), min=min(values),
                max=max(values), stddev=sd, cv=sd / mean, p95=p95)


def summarize(pairs):
    old = [p['old']['wallSeconds'] for p in pairs]
    new = [p['new']['wallSeconds'] for p in pairs]
    rng = random.Random(162)
    ratios = []
    for _ in range(10000):
        indices = rng.choices(range(len(pairs)), k=len(pairs))
        ratios.append(statistics.mean(old[i] for i in indices) /
                      statistics.mean(new[i] for i in indices))
    ratios.sort()
    return dict(old=describe(old), new=describe(new),
                speedup=statistics.mean(old) / statistics.mean(new),
                speedup95CI=[ratios[249], ratios[9749]],
                method='Ratio of arithmetic mean wall times; paired percentile bootstrap, 10000 resamples, seed 162')


def run(args):
    if os.name != 'nt':
        raise ValueError('Use a Windows runner with the SDK runtime prerequisites')
    repo = Path(args.repo).resolve()
    out = Path(args.output).resolve()
    out.mkdir(parents=True, exist_ok=False)
    sdk = repo / 'SDK'
    runtime = Path(args.runtime).resolve()
    state = dict(schemaVersion=1, succeeded=False, runId=os.getenv('GITHUB_RUN_ID'),
                 runAttempt=os.getenv('GITHUB_RUN_ATTEMPT'), pairs=[], warmups=[],
                 scope='Unmodified full Upheaval base/APT/maps clean compile and link; no LoD, BIG or SKUDEF packaging',
                 environment=dict(platform=platform.platform(), python=platform.python_version(),
                                  processor=platform.processor(), logicalCpus=os.cpu_count()),
                 methodology=dict(warmups=args.warmups, repetitions=args.runs,
                     order='Alternating old-new / new-old pairs, including warmup pairs',
                     cache='All compiler caches off, fresh processes and output/intermediate directories; OS filesystem cache warmed, not evicted',
                     timing='perf_counter immediately before process creation through process exit; input hashing, output checks and preparation excluded',
                     outliers='No samples removed; all successful pairs retained',
                     old='One original BAB process per discovered stream, aggregated',
                     new='One Release production CLI process for all discovered streams'))
    work = Path(tempfile.mkdtemp(prefix='ub', dir=os.environ.get('RUNNER_TEMP')))
    sdk_before = runtime_before = mod_before = None
    mod = work / 'historical/SDK/Mods/Upheaval'
    try:
        state['testSourceCommit'] = subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD']).decode().strip()
        state['reuse'] = json.loads((runtime.parent / 'reuse.json').read_text())
        if state['reuse']['testSourceCommit'] != state['testSourceCommit']:
            raise ValueError('Runtime was not prepared for this benchmark commit')
        sdk_before, runtime_before = snapshot(sdk), snapshot(runtime)
        write(out / 'sdk-inputs.json', sdk_before)
        state['runtimeInputs'] = runtime_before
        state['sdkGitTree'] = subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD:SDK']).decode().strip()
        # The original SDK import and recovered compiler use separate histories.
        # This is a pinned workload, not a reused compiler binary; ancestry is
        # required for the binary source in prepare.py, not for the MOD dataset.
        subprocess.run(['git', '-C', str(repo), 'cat-file', '-e', MOD_SOURCE + '^{commit}'], check=True)
        mod_tree = subprocess.check_output(['git', '-C', str(repo), 'rev-parse',
                    MOD_SOURCE + ':SDK/Mods/Upheaval']).decode().strip()
        if mod_tree != MOD_TREE:
            raise ValueError('Pinned historical Upheaval tree mismatch')
        archive = work / 'upheaval.tar'
        with archive.open('xb') as stream:
            subprocess.run(['git', '-C', str(repo), 'archive', '--format=tar', MOD_SOURCE,
                            'SDK/Mods/Upheaval'], check=True, stdout=stream)
        historical = work / 'historical'
        historical.mkdir()
        with tarfile.open(archive) as tar:
            # Fixed Git tree, with explicit archive safety checks before extraction.
            for member in tar.getmembers():
                target = (historical / member.name).resolve()
                if (not target.is_relative_to(historical.resolve()) or
                        not (member.isfile() or member.isdir())):
                    raise ValueError('Unsafe historical MOD archive entry')
            tar.extractall(historical, filter='data')
        archive.unlink()
        state['modGitTree'] = mod_tree
        state['modSourceCommit'] = MOD_SOURCE
        mod_before = snapshot(mod)
        if len(mod_before) != 2723:
            raise ValueError('Pinned full Upheaval file count changed')
        write(out / 'mod-inputs.json', mod_before)
        tasks = [('base', mod / 'Data/Mod.xml')]
        tasks += [('apt', p) for p in sorted((mod / 'Data/AptUI').glob('*.xml'))]
        tasks += [('maps', p) for p in sorted((mod / 'Data/AdditionalMaps').glob('MapMetaData_*.xml'))]
        if not tasks[0][1].is_file():
            raise ValueError('Full Upheaval entry missing')
        state['tasks'] = [dict(content=c, input=str(p)) for c, p in tasks]
        tools = work / 't'
        shutil.copytree(sdk / 'tools', tools)
        tree = ET.parse(tools / 'BinaryAssetBuilder.exe.config')
        section = tree.getroot().find('assetbuilder')
        paths = {kind: ';'.join(map(str, (sdk, mod / leaf, sdk / 'Mods', sdk / shared)))
                 for kind, leaf, shared in [('data', 'Data', 'SageXml'),
                     ('art', 'Art', 'Art'), ('audio', 'Audio', 'Audio')]}
        schema = sdk / 'Schemas/xsd/CnC3Types.xsd'
        section.set('schema', str(schema))
        section.set('dataRoot', str(sdk))
        for kind, value in paths.items():
            section.set('default' + kind.title() + 'Paths', value)
        startup = tree.getroot().find('startup')
        if startup is None:
            startup = ET.SubElement(tree.getroot(), 'startup')
        startup.clear()
        startup.set('useLegacyV2RuntimeActivationPolicy', 'true')
        ET.SubElement(startup, 'supportedRuntime', version='v4.0')
        tree.write(tools / 'BinaryAssetBuilder.exe.config', encoding='utf-8', xml_declaration=True)
        shutil.copy2(tools / 'BinaryAssetBuilder.exe.config', out / 'original-effective.config')
        state['originalRuntime'] = snapshot(tools)
        environment = {k: v for k, v in os.environ.items()
                       if not k.startswith('RA3_RECOVERED_') and k != 'RECOVERED_W3X_TRACE_DIRECTORY'}
        environment['PATH'] = ';'.join(p for p in environment['PATH'].split(';')
                                      if not re.search(r'[\\/]SDK[\\/]tools', p, re.I))
        baseline = None
        for index in range(args.warmups + args.runs):
            warmup = index < args.warmups
            pair_dir = out / (('warmup' if warmup else 'pair') + f'-{index:02}')
            pair = dict(index=index, warmup=warmup, order=['old', 'new'] if index % 2 == 0 else ['new', 'old'])
            state['warmups' if warmup else 'pairs'].append(pair)
            for engine in pair['order']:
                destination = work / 'o'
                if destination.exists():
                    shutil.rmtree(destination)
                destination.mkdir()
                rows, products = [], {}
                if engine == 'old':
                    # All directory creation and argument preparation precede timing.
                    commands = []
                    for number, (content, entry) in enumerate(tasks):
                        task_dir = destination / str(number)
                        for leaf in ('s', 'i', 'h', 'c'):
                            (task_dir / leaf).mkdir(parents=True, exist_ok=True)
                        arguments = [str(entry), f'/sp:{schema}', f'/dr:{sdk}', f'/od:{task_dir / "s"}',
                            f'/iod:{task_dir / "i"}', f'/scd:{task_dir / "h"}', f'/bcp:{task_dir / "c"}',
                            '/tp:Win32', '/sc:false', '/bc:false', '/psc:false', '/fsc:false', '/csc:false',
                            '/atc:false', '/sh:false', '/pc:true', '/res:true', '/gui:false', '/poe:false',
                            '/mr:false', '/oof:false', '/sf:false', '/ls:true', '/ss:true', '/oix:false',
                            '/vf:false', '/osh:false', '/oar:false', '/slowclean:true', '/tl:3', '/el:0']
                        arguments += [f'/{k}:{v}' for k, v in paths.items()]
                        commands.append((task_dir, arguments))
                    for number, (task_dir, arguments) in enumerate(commands):
                        rows.append(execute(tools / 'BinaryAssetBuilder.exe', arguments, sdk,
                                    pair_dir / engine / str(number), args.timeout, environment))
                    for number, (content, entry) in enumerate(tasks):
                        products[content + '/' + entry.stem] = inventory(destination / str(number) / 's')
                else:
                    arguments = ['build', '--mod', str(mod), '--sdk-root', str(sdk), '--out', str(destination),
                                 '--package-content', 'base,apt,maps', '--package-format', 'loose',
                                 '/sc:false', '/bc:false', '/pc:true', '/ss:true', '/slowclean:true', '/tl:3']
                    arguments += [f'/{k}:{v}' for k, v in paths.items()]
                    rows.append(execute(runtime / 'NewModCompiler.exe', arguments, runtime,
                                        pair_dir / engine, args.timeout, environment))
                    report = json.loads((destination / 'build-report.json').read_text(encoding='utf-8-sig'))
                    for name in ('build-report.json', 'build.events.jsonl', 'build.log'):
                        if (destination / name).exists():
                            shutil.copy2(destination / name, pair_dir / engine / name)
                    if not (report['status'] == 'succeeded' and report['exitCode'] == 0
                            and report['executionMode'] == 'InProcess' and not report['externalTools']
                            and report['publicationStatus'] == 'PublishedDirectory'
                            and {(t['Content'], t['InputPath'].casefold()) for t in report['plan']['Tasks']}
                            == {(c, str(p).casefold()) for c, p in tasks}):
                        raise ValueError('Production report did not confirm exact full MOD build')
                    for content, entry in tasks:
                        products[content + '/' + entry.stem] = inventory(destination / 'published' / content / entry.stem / 'streams')
                    if sum(len(v) for v in products.values()) != len(report['artifacts']):
                        raise ValueError('Production artifact count differs from raw outputs')
                pair[engine] = dict(wallSeconds=sum(r['wallSeconds'] for r in rows),
                                    cpuSeconds=sum(r['cpuSeconds'] for r in rows),
                                    peakWorkingSetBytes=max(r['peakWorkingSetBytes'] for r in rows),
                                    processes=rows, outputs=products)
                write(pair_dir / engine / 'outputs.json', products)
                if baseline is None:
                    baseline = products
                    write(out / 'baseline.json', baseline)
                if products != baseline:
                    shutil.copytree(destination, pair_dir / engine / 'mismatched-output')
                    raise ValueError('Raw output mismatch; no speed conclusion: ' + str(pair_dir / engine))
                pair[engine]['byteIdentical'] = True
                if index == 0:
                    # Keep raw first-pair oracle and product outputs, not ten copies.
                    if engine == 'new':
                        shutil.copytree(destination / 'published', pair_dir / engine / 'raw-output')
                    else:
                        for number in range(len(tasks)):
                            shutil.copytree(destination / str(number) / 's', pair_dir / engine / 'raw-output' / str(number))
                write(out / 'validation.json', state)
                print(f'{pair_dir.name} {engine}: {pair[engine]["wallSeconds"]:.3f}s, raw outputs identical', flush=True)
        if (snapshot(sdk) != sdk_before or snapshot(mod) != mod_before or
                snapshot(runtime) != runtime_before or snapshot(tools) != state['originalRuntime']):
            raise ValueError('Compiler or SDK inputs changed during benchmark')
        state['inputsUnchanged'] = True
        state['statistics'] = summarize(state['pairs'])
        state['succeeded'] = True
        s = state['statistics']
        summary = ('# Upheaval compilation benchmark\n\n'
                   f'Validated pairs: {args.runs}; warmup pairs excluded: {args.warmups}. All linked outputs byte-identical.\n\n'
                   '| Compiler | Mean (s) | Median (s) | Stddev (s) | CV | p95 (s) |\n'
                   '| --- | ---: | ---: | ---: | ---: | ---: |\n')
        for key, name in [('old', 'Original SDK BAB'), ('new', 'NewModCompiler Release')]:
            row = s[key]
            summary += f'| {name} | {row["mean"]:.3f} | {row["median"]:.3f} | {row["stddev"]:.3f} | {row["cv"]:.1%} | {row["p95"]:.3f} |\n'
        summary += (f'\nOld/new mean wall-time ratio: **{s["speedup"]:.3f}x**, paired bootstrap 95% CI '
                    f'[{s["speedup95CI"][0]:.3f}, {s["speedup95CI"][1]:.3f}].\n\n'
                    'Clean compile/link with warmed filesystem cache; includes process startup and normal CLI overhead. '
                    'Compiler caches disabled. No packaging, incremental edits, cold OS cache or game runtime claim. '
                    'Shared hosted-runner noise and this single MOD limit generalization. CI is exploratory, not a universal speed guarantee.\n')
        (out / 'summary.md').write_text(summary, encoding='utf-8')
        if os.getenv('GITHUB_STEP_SUMMARY'):
            with open(os.environ['GITHUB_STEP_SUMMARY'], 'a', encoding='utf-8') as stream:
                stream.write(summary)
    except BaseException:
        state['error'] = traceback.format_exc()
        partial = work / 'o'
        if partial.exists():
            shutil.copytree(partial, out / 'failed-partial-output')
        raise
    finally:
        # Verify unchanged inputs even on a failed sample. Preserve failure state.
        state['inputsUnchanged'] = (sdk_before is not None and runtime_before is not None and mod_before is not None
                                    and snapshot(sdk) == sdk_before and snapshot(runtime) == runtime_before
                                    and snapshot(mod) == mod_before)
        if not state['inputsUnchanged']:
            state['succeeded'] = False
        write(out / 'validation.json', state)
        shutil.rmtree(work, ignore_errors=True)
        if not state['succeeded']:
            (out / 'summary.md').write_text('Benchmark failed. No accepted speed comparison. See validation.json and raw logs.\n')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--repo', default='.')
    parser.add_argument('--runtime', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--runs', type=int, choices=range(3, 31), default=10)
    parser.add_argument('--warmups', type=int, choices=range(1, 6), default=1)
    parser.add_argument('--timeout', type=int, default=1200)
    run(parser.parse_args())
