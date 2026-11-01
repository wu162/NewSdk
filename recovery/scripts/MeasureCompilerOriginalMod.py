#!/usr/bin/env python3
"""Same-machine original BAB and accepted current Release, complete MOD base streams."""
import argparse
import ctypes
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import statistics
import subprocess
import time

from BenchmarkCompilerCache import inventory, output_inventory, validate_sample
from BenchmarkCompilerSdkComparison import (compare_pair, invocation, measure, summaries,
    stream_inventory, validate_shared_report, cache_profile, sdk_source_inventory)
from HotCacheWorkload import HISTORICAL, UPHEAVAL_TREE, extract_upheaval
from PrepareCompilerSdkBenchmark import prepare, inventory as file_rows

SOURCE = '24f83d2b53c0ad14c7bfc17db643dc035e769639'
RUN = '41260620635'
HEAD = '1a73017a5847a5657944866a0b25f4320c2ba00d'
FILES = {'NewModCompiler.exe': '5aa008d05ca4288265f7b51a15b09c01a1d10be305fbdf9616ec4946468cd4db', 'NewModCompiler.exe.config': '94eedb0cd0150b90c8115aaee53f64ab0523b39bd67887991b1b62679a752e68', 'antlr.runtime.dll': 'febdc86d6299a550c6ec87050b0cbf5205e821851e75494c699c17ce8ba2c62a'}


def need(value, message):
    if not value:
        raise ValueError(message)


def write(path, body):
    Path(path).write_text(json.dumps(body, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')


def git(repo, *args):
    return subprocess.check_output(['git', '-C', str(repo), *args], text=True).strip()


def binary_identity(manifest=None):
    identity = (json.loads(Path(manifest).read_text(encoding='utf-8-sig')) if manifest else
                dict(sourceCommit=SOURCE, runId=RUN, runtimeFiles=FILES.copy()))
    source, run, files = identity.get('sourceCommit'), identity.get('runId'), identity.get('runtimeFiles')
    need(isinstance(source, str) and re.fullmatch(r'[0-9a-f]{40}', source), 'Binary source must be a full commit SHA')
    need(str(run).isdigit() and int(run) > 0, 'Binary run identity is missing')
    need(isinstance(files, dict) and set(files) == set(FILES), 'Expected the accepted three-file runtime manifest')
    need(all(isinstance(value, str) and re.fullmatch(r'[0-9a-f]{64}', value) for value in files.values()),
         'Binary runtime identities must be SHA256 values')
    return dict(sourceCommit=source, runId=str(run), runtimeFiles=files)


def stream_timing_summary(product, tasks, build_report):
    execution = build_report.get('contentExecution')
    if execution is not None:
        intervals = [dict(streamId=row['streamId'], startedMs=row['startedMs'], completedMs=row['completedMs'])
                     for group in execution['groups'] for row in group['streams']]
        boundary = 'Owner Compile intervals; initialization/disposal excluded'
    else:
        starts, intervals = {}, []
        for line in (product / 'build.events.jsonl').read_text(encoding='utf-8-sig').splitlines():
            event = json.loads(line)
            if event['eventType'] == 'StreamStarted':
                starts[event['streamId']] = event['elapsedMs']
            elif event['eventType'] == 'StreamCompleted':
                need(event['payload']['status'] == 'succeeded', 'Failed stream in successful sample')
                intervals.append(dict(streamId=event['streamId'], startedMs=starts[event['streamId']],
                                      completedMs=event['elapsedMs']))
        boundary = 'StreamStarted/StreamCompleted elapsedMs; includes stream initialization'
    expected = {task['id'] for task in tasks}
    need(len(intervals) == len(expected) and {row['streamId'] for row in intervals} == expected,
         'Missing or duplicate timed MOD streams')
    parts, points = {}, []
    for row in intervals:
        need(row['completedMs'] >= row['startedMs'], 'Negative stream interval')
        group = row['streamId'].split('/')[0]
        parts[group] = parts.get(group, 0) + row['completedMs'] - row['startedMs']
        if row['completedMs'] > row['startedMs']:
            points.extend(((row['startedMs'], 1), (row['completedMs'], -1)))
    active = peak = 0
    covered = 0.0
    previous = None
    for tick, delta in sorted(points):
        if previous is not None and active:
            covered += tick - previous
        active += delta
        peak = max(peak, active)
        previous = tick
    return dict(partWallMs=parts, streamIntervals=intervals, streamTimingBoundary=boundary,
                streamIntervalUnionMs=covered, peakConcurrentStreams=peak)


def discover_tasks(mod, contents='base,apt,global-metadata'):
    selected = set(contents.replace('global-metadata', 'maps').split(','))
    tasks = ([dict(id='base/Mod', content='base', inputPath=str(mod / 'Data/Mod.xml'), stem='Mod')]
             if 'base' in selected else [])
    for content, directory, pattern in (('apt', 'Data/AptUI', '*.xml'),
                                        ('maps', 'Data/AdditionalMaps', 'MapMetaData_*.xml')):
        if content not in selected:
            continue
        entries = sorted((mod / directory).glob(pattern), key=lambda p: (str(p).upper(), str(p)))
        need(entries, 'Requested content has no source entries: ' + content)
        tasks.extend(dict(id=content + '/' + p.stem, content=content, inputPath=str(p), stem=p.stem)
                     for p in entries)
    return tasks


def full_inventory(directory, tasks):
    result, paths = {}, {}
    for task in tasks:
        files, sources = stream_inventory(directory / task['id'], task['stem'])
        prefix = task['id'].lower() + '/'
        result.update({prefix + name: row for name, row in files.items()})
        paths.update({prefix + name: path for name, path in sources.items()})
    return result, paths


def report(out, state):
    state['summary'] = summaries(state['samples'])
    parts = {}
    for row in state['samples']:
        if row.get('status') == 'passed':
            for part, milliseconds in row.get('partWallMs', {}).items():
                key = row['mode'], row['phase'], part
                parts.setdefault(key, {}).setdefault(row['engine'], []).append(milliseconds)
    state['partSummary'] = [dict(mode=mode, phase=phase, content=part,
        nPerEngine=len(values['original']), originalMsMedian=statistics.median(values['original']),
        newMsMedian=statistics.median(values['new'])) for (mode, phase, part), values in parts.items()
        if set(values) == {'original', 'new'} and len(values['original']) == len(values['new'])]
    write(out / 'measurement.json', state)
    lines = ['# Original SDK versus current compiler', '', 'Status: ' + state['status'], '',
        state['scope'],
        ('All entries in selected groups ' + state['contentSelection'] + ', loose output; no BIG/SKUDEF packaging.' if state.get('fullContent')
         else 'Base stream only, loose output. No BIG/SKUDEF, APT or global-metadata timing.'),
        'Same Windows VM, SDK, physical MOD input, Schema, cwd and ordered search roots. Separate caches.',
        'Fresh process per sample, process launch through exit includes CLI/report overhead. Hash checks excluded.',
        'Cold = initially absent compiler cache/output namespace; OS page cache is uncontrolled.',
        'New Release jobs4, performance off; original BAB has its original serial implementation.',
        'First paired cold engine: ' + state.get('firstEngine', 'original') + '; trial/phase order alternates.', '',
        '| MOD | Mode | Phase | n/engine | Original seconds | New seconds | New time change |',
        '| --- | --- | --- | ---: | ---: | ---: | ---: |']
    for r in state['summary']:
        lines.append('| %s | %s | %s | %d | %.6f | %.6f | %+.2f%% |' % (
            r['workload'], r['mode'], r['phase'], r['nPerEngine'],
            r['originalWallMsMedian'] / 1000, r['newWallMsMedian'] / 1000,
            100 * (r['newOverOriginalWallRatio'] - 1)))
    if state.get('fullContent'):
        lines += ['', 'Original total/parts sum each sequential BAB process launch-to-exit; new total is one complete CLI process.',
            'New parts use Owner Compile intervals when available, otherwise StreamStarted/StreamCompleted events. Parallel group durations overlap and must not be added to obtain total wall time. Shared overhead subtracts the interval union, never the sum of overlapping groups.',
            '', '| Mode | Phase | Content | n/engine | Original seconds | New stream seconds |',
            '| --- | --- | --- | ---: | ---: | ---: |']
        for row in state['partSummary']:
            lines.append('| %s | %s | %s | %d | %.6f | %.6f |' % (row['mode'], row['phase'],
                row['content'], row['nPerEngine'], row['originalMsMedian']/1000, row['newMsMedian']/1000))
    lines += ['', 'All raw samples, CPU/peak working set and output identities are in measurement.json.',
        'All cache-state repeats must match their own engine baseline byte for byte.',
        'Cross-engine differences, if present, are retained below; such results do not establish equivalent output.',
        'Small sample counts describe this VM and workload; no statistical significance or game-runtime claim.', '']
    for p in state.get('pairs', []):
        if not p['rawBytesEqual']:
            lines.append('- Output mismatch: %s trial %s %s %s: %s' % (
                p['workload'], p['trial'], p['mode'], p['phase'], ', '.join(p['differentMembers'][:8])))
    for failure in state.get('errors', []):
        lines.append('- Failed sample: %s trial %s %s %s: %s' % (
            failure['engine'], failure['trial'], failure['mode'], failure['phase'], failure['error']))
    if state.get('error'):
        lines += ['', 'Error: ' + state['error']]
    if state.get('edit'):
        lines += ['', 'Incremental edit: ' + json.dumps(state['edit'], ensure_ascii=False),
                  ('Edited outputs are checked against a separate session-disabled rebuild per engine, seeded only with that engine\'s asset cache. The edited GameObject is not build-cache eligible; unchanged media payloads may be reused.'
                   if state['edit'].get('freshRebuildMode') == 'asset' else
                   'Edited outputs are checked against a separate cache-disabled rebuild per engine.'),
                  'The source copy is restored.']
    if state.get('schemaOverlay'):
        lines += ['', 'Shared runtime Schema overlay: ' + json.dumps(state['schemaOverlay'], ensure_ascii=False)]
    if state.get('sourceAdjustments'):
        changes = state['sourceAdjustments']
        lines += ['', 'Supported-input variant: removed %d unsupported base declarations; normalized %d unused TGA headers. Same adjusted input for both engines.' %
                  (changes['removedAssetCount'], changes['normalizedTgaCount'])]
    if state.get('engineSelection') == 'new':
        lines += ['', 'New-compiler preflight only. No original/new performance comparison is claimed.']
    if state.get('coldOnly'):
        lines += ['', 'Cold-only selection. No hot-cache or incremental-edit result is claimed by this run.']
    if state.get('stoppedAfterFirstNewColdFailure'):
        lines += ['', 'The selected first new cold build failed; the original long cold build was not started.']
    (out / 'report.md').write_text('\n'.join(lines) + '\n', encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--accepted-build', type=Path, required=True)
    parser.add_argument('--binary-manifest', type=Path,
                        help='Explicit accepted {sourceCommit,runId,runtimeFiles:{name:sha256}}; historical default remains pinned')
    parser.add_argument('--hot-rounds', type=int, choices=range(3), default=2,
                        help='Hot samples per cold namespace, default 2')
    parser.add_argument('--expected-streams', type=int, help='Require the exact selected full-MOD stream count before timing')
    parser.add_argument('--continue-on-build-failure', action='store_true',
                        help='Retain independent engine/phase failures and continue other samples; final status still fails')
    parser.add_argument('--work', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--pairs', type=int, choices=range(1, 4), default=3)
    parser.add_argument('--mod', type=Path, help='Complete external MOD directory; replaces the default workloads')
    parser.add_argument('--mod-schema', type=Path, help='Use the MOD-provided xsd tree for both engines')
    parser.add_argument('--edit-file', help='Relative XML path in the temporary MOD copy')
    parser.add_argument('--edit-old')
    parser.add_argument('--edit-new')
    parser.add_argument('--timeout-seconds', type=float, default=180)
    parser.add_argument('--budget-seconds', type=float, default=900)
    parser.add_argument('--requested-head', default=HEAD)
    parser.add_argument('--content', choices=('base', 'apt', 'global-metadata', 'base,apt,global-metadata', 'apt,global-metadata'), default='base')
    parser.add_argument('--rebuild-check', choices=('none', 'asset'), default='none',
                        help='Independent changed-input rebuild: disable all caches or just session state')
    parser.add_argument('--skip-extra-uncached', action='store_true',
                        help='Omit the extra unchanged cache-disabled timing run for very large MODs')
    parser.add_argument('--source-adjustments', type=Path)
    parser.add_argument('--engine', choices=('both', 'new'), default='both')
    parser.add_argument('--first-engine', choices=('original', 'new'), default='original',
                        help='First paired cold engine; later trial/phase orders alternate')
    parser.add_argument('--cold-only', action='store_true',
                        help='Measure complete cold builds only; leave warm-cache failures as separate evidence')
    args = parser.parse_args()
    identity = binary_identity(args.binary_manifest)
    accepted_source, accepted_run, accepted_files = identity['sourceCommit'], identity['runId'], identity['runtimeFiles']
    need(not (args.cold_only and args.edit_file), 'Cold-only selection cannot include an edit workload')
    need(os.name == 'nt', 'Execution requires Windows')
    ctypes.WinDLL('kernel32', use_last_error=True).SetErrorMode(0x0001 | 0x0002 | 0x8000)
    repo = Path(__file__).resolve().parents[2]
    out, work = args.output.resolve(), args.work.resolve()
    out.mkdir(parents=True, exist_ok=False)
    work.mkdir(parents=True, exist_ok=False)
    state = dict(status='preparing', binarySourceCommit=accepted_source, sourceRunId=accepted_run,
        requestedHead=args.requested_head, measurementSource=git(repo, 'rev-parse', 'HEAD'),
        runId=os.environ.get('GITHUB_RUN_ID'), runAttempt=os.environ.get('GITHUB_RUN_ATTEMPT'),
        machine=dict(platform=platform.platform(), processor=platform.processor(),
                     cpuCount=os.cpu_count(), imageVersion=os.environ.get('ImageVersion')),
        scope='Complete mymod and untouched full historical Upheaval inputs, base loose stream only.',
        samples=[], pairs=[], errors=[], jobs=4, performance='off',
        fullContent=args.content != 'base', contentSelection=args.content)
    state['engineSelection'] = args.engine
    state['firstEngine'] = args.first_engine
    state['coldOnly'] = args.cold_only
    state['hotRounds'] = args.hot_rounds
    state['continueOnBuildFailure'] = args.continue_on_build_failure
    state['progressEnvironment'] = os.environ.get('NEWSDK_PROGRESS', 'default')
    state['binaryIdentity'] = identity
    if args.binary_manifest:
        shutil.copy2(args.binary_manifest, out / 'binary-manifest.json')
    if args.source_adjustments:
        state['sourceAdjustments'] = json.loads(args.source_adjustments.read_text(encoding='utf-8'))
    started = time.perf_counter()
    schema_backup = None
    edit_path = None
    edit_original = None
    try:
        subprocess.run(['git', '-C', str(repo), 'merge-base', '--is-ancestor', accepted_source, 'HEAD'], check=True)
        closure = ('SDK', 'recovery/src', 'recovery/build', 'recovery/probes', 'ui',
            'recovery/scripts/Build*', 'recovery/scripts/PrepareLegacyTexture.ps1',
            'recovery/scripts/RestoreUiDependencies.ps1')
        need(not git(repo, 'diff', '--name-only', accepted_source, 'HEAD', '--', *closure),
             'Accepted binary production/SDK closure differs')
        receipt = json.loads((args.accepted_build / 'build.json').read_text(encoding='utf-8-sig'))
        need(receipt['sourceCommit'] == accepted_source and receipt['configuration'] == 'Release', 'Build source differs')
        runtime = args.accepted_build.resolve() / 'bin'
        actual = inventory(runtime)
        need({n: r['sha256'] for n, r in actual.items()} == accepted_files, 'Accepted three runtime files differ')
        shutil.copy2(args.accepted_build / 'build.json', out / 'accepted-build.json')
        if args.mod:
            mod = args.mod.resolve()
            need(mod.is_dir() and (mod / 'Data/Mod.xml').is_file(), 'External MOD entry is absent')
            target = work / 'inputs' / mod.name
            shutil.copytree(mod, target)
            need(inventory(target) == inventory(mod), 'External MOD copy differs')
            selected = ((mod.name, target, None, None),)
            state['scope'] = ('Shared supported-input variant of ' + mod.name + '; source adjustments are recorded.'
                              if args.source_adjustments else
                              'Complete downloaded ' + mod.name + ' source copy; no assets or includes removed.')
        else:
            mymod = work / 'inputs/mymod'
            shutil.copytree(repo / 'SDK/Mods/mymod', mymod)
            need(inventory(mymod) == inventory(repo / 'SDK/Mods/mymod'), 'mymod copy differs')
            upheaval = extract_upheaval(repo, work)
            selected = (('mymod', mymod, accepted_source, None), ('upheaval', upheaval, HISTORICAL, UPHEAVAL_TREE))
        if args.mod_schema:
            need(args.mod is not None, 'Schema overlay requires an external MOD')
            schema = repo / 'SDK/Schemas/xsd'
            supplied = args.mod_schema.resolve()
            need((supplied / 'CnC3Types.xsd').is_file(), 'MOD Schema entry is absent')
            schema_backup = work / 'original-schema'
            shutil.copytree(schema, schema_backup)
            changed = []
            for path in supplied.rglob('*'):
                if path.is_file():
                    relative = path.relative_to(supplied)
                    destination = schema / relative
                    data = path.read_bytes()
                    if not destination.is_file() or destination.read_bytes() != data:
                        changed.append(dict(path=relative.as_posix(), sha256=hashlib.sha256(data).hexdigest()))
                    destination.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copy2(path, destination)
            state['schemaOverlay'] = dict(source=str(supplied), changed=changed,
                scope='MOD-provided runtime Schema shared by both engines; production binaries unchanged')
        if args.edit_file:
            need(args.mod and args.edit_old and args.edit_new and args.edit_old != args.edit_new,
                 'An external MOD and distinct edit literals are required')
            edit_path = (selected[0][1] / args.edit_file).resolve()
            need(edit_path.is_relative_to(selected[0][1].resolve()), 'Edit path escapes temporary input')
            edit_original = edit_path.read_bytes()
            old, new = args.edit_old.encode('utf-8'), args.edit_new.encode('utf-8')
            need(edit_original.count(old) == 1, 'Edit literal must occur exactly once')
            edit_modified = edit_original.replace(old, new, 1)
            state['edit'] = dict(path=args.edit_file, old=args.edit_old, new=args.edit_new,
                originalSha256=hashlib.sha256(edit_original).hexdigest(),
                modifiedSha256=hashlib.sha256(edit_modified).hexdigest())
        workloads = [dict(name=name, modRoot=str(mod), files=file_rows(mod),
            scope='Complete input copy; base loose stream timing only',
            sourceCommit=source, sourceTree=tree) for name, mod, source, tree in selected]
        if state['fullContent']:
            for workload in workloads:
                workload['tasks'] = discover_tasks(Path(workload['modRoot']), args.content)
                workload['scope'] = 'All source entries in selected groups ' + args.content
                if args.expected_streams is not None:
                    need(len(workload['tasks']) == args.expected_streams, 'Selected MOD stream count differs from the pinned workload')
        prep = dict(succeeded=True, binarySourceCommit=accepted_source, sourceRunId=accepted_run,
            benchmarkSourceCommit=state['measurementSource'], sdkRoot=str(repo / 'SDK'),
            runtimeDirectory=str(runtime), runtimeFiles=file_rows(runtime, 'name'), workloads=workloads)
        write(out / 'preparation.json', prep)
        original_path = prepare(out / 'preparation.json', work / 'original', repo, accepted_source, accepted_run)
        original = json.loads(original_path.read_text(encoding='utf-8'))
        shutil.copy2(original_path, out / 'original-preparation.json')
        workloads = original['workloads']
        sdk = repo / 'SDK'
        sdk_before = sdk_source_inventory(sdk, workloads)
        inputs_before = {w['name']: inventory(w['modRoot']) for w in workloads}
        old_before = inventory(original['originalRuntimeDirectory'])
        root_before = {p: Path(p).is_dir() for w in workloads for key in ('dataPaths', 'artPaths', 'audioPaths')
                       for p in w['effectiveInputRoots'][key]}
        state['workloads'] = workloads
        state['runtimeFiles'] = actual
        state['status'] = 'running'
        report(out, state)
        environment = os.environ.copy()
        for name in list(environment):
            if name.startswith('RA3_RECOVERED_') or name == 'RECOVERED_W3X_TRACE_DIRECTORY':
                del environment[name]
        environment['PATH'] = os.pathsep.join(p for p in environment.get('PATH', '').split(os.pathsep)
            if '/sdk/tools' not in p.replace('\\', '/').lower())
        baselines = {}

        def execute(engine, w, mode, profile, round_index, label):
            command = invocation(engine, w, mode, profile, round_index, prep, original)
            remaining = args.budget_seconds - (time.perf_counter() - started)
            need(remaining > 1, 'Total benchmark budget exhausted')
            if engine == 'new':
                command[command.index('--package-content') + 1] = args.content
                command += ['--jobs', '4', '--performance', 'off']
            if not state['fullContent'] or engine == 'new':
                return measure(command, sdk, out / label, min(args.timeout_seconds, remaining), environment)
            rows, failures = [], []
            for task in w['tasks']:
                current = command.copy()
                current[1] = task['inputPath']
                stream_root = profile / 'streams' / task['id']
                stream_root.mkdir(parents=True, exist_ok=True)
                current = ['/od:' + str(stream_root) if argument.startswith('/od:') else argument
                           for argument in current]
                remaining = args.budget_seconds - (time.perf_counter() - started)
                need(remaining > 1, 'Total benchmark budget exhausted')
                evidence = out / label / task['id']
                try:
                    row = measure(current, sdk, evidence, min(args.timeout_seconds, remaining), environment)
                except Exception as error:
                    receipt = evidence / 'process.json'
                    row = json.loads(receipt.read_text(encoding='utf-8')) if receipt.is_file() else dict(status='failed')
                    failures.append(task['id'] + ': ' + str(error))
                row.update(taskId=task['id'], content=task['content'])
                rows.append(row)
            aggregate = dict(command=[row.get('command') for row in rows], cwd=str(sdk), streams=rows,
                wallMs=sum(row.get('wallMs', 0) for row in rows), cpuMs=sum(row.get('cpuMs', 0) for row in rows),
                peakWorkingSetBytes=max(row.get('peakWorkingSetBytes', 0) for row in rows),
                status='failed' if failures else 'exited', exitCode=1 if failures else 0,
                traceSessionLoaded=all(row.get('traceSessionLoaded') for row in rows),
                traceStringHashesLoaded=any(row.get('traceStringHashesLoaded') for row in rows),
                traceAssetsCopiedFromCache=sum(row.get('traceAssetsCopiedFromCache', 0) for row in rows),
                partWallMs={group: sum(row.get('wallMs', 0) for row in rows if row['content'] == group)
                            for group in sorted({task['content'] for task in w['tasks']})}, errors=failures)
            write(out / label / 'process.json', aggregate)
            need(not failures, 'Original SDK stream failures: ' + '; '.join(failures))
            return aggregate

        def one(engine, w, trial, mode, phase, round_index):
            profile = work / 'builds' / w['name'] / ('%s-%d' % (mode, trial)) / engine
            storage = cache_profile(engine, w, profile, prep)
            if phase in ('cold', 'uncached', 'edited-uncached'):
                need(not profile.exists() and not storage.exists(), 'Cold namespace already exists')
            for name in ('assets', 'session', 'streams', 'product', 'intermediate',
                         'fresh-' + str(round_index), 'fresh-session-' + str(round_index)):
                (profile / name).mkdir(parents=True, exist_ok=True)
            label = '%s-%d-%s-%s-%d-%s' % (w['name'], trial, mode, phase, round_index, engine)
            row = dict(engine=engine, workload=w['name'], trial=trial, mode=mode, phase=phase,
                       round=round_index, status='started', cacheDirectory=str(storage))
            state['samples'].append(row)
            report(out, state)
            try:
                row.update(execute(engine, w, mode, profile, round_index, label))
                if engine == 'new':
                    build_report = json.loads((profile / 'product/build-report.json').read_text(encoding='utf-8-sig'))
                    validation_phase = 'hot' if phase == 'hot-edited' else ('uncached' if phase == 'cold' else phase)
                    validate_sample(build_report, mode, validation_phase)
                    validate_shared_report(build_report, w, sdk)
                    output_inventory(profile / 'product', build_report)
                    row.update(cache=build_report['cache'], reportElapsedMs=build_report['elapsedMs'])
                    if state['fullContent']:
                        row.update(stream_timing_summary(profile / 'product', w['tasks'], build_report))
                        row['sharedOverheadMs'] = row['wallMs'] - row['streamIntervalUnionMs']
                        if 'contentExecution' in build_report:
                            row['contentExecution'] = build_report['contentExecution']
                    if phase in ('hot', 'hot-edited'):
                        need(row['cache']['xmlParse'] == 0, 'Hot build unexpectedly parsed XML')
                    if phase == 'edited':
                        need(row['cache']['instanceCompile'] > 0, 'Edited XML did not recompile an asset')
                elif phase in ('hot', 'hot-edited'):
                    need(row['traceSessionLoaded'], 'Original hot process did not load session')
                elif mode == 'none':
                    need(not row['traceStringHashesLoaded'], 'No-session original reused string hashes')
                destination = profile / ('product/published' if engine == 'new' else 'streams')
                files, paths = (full_inventory(destination, w['tasks']) if state['fullContent']
                                else stream_inventory(destination))
                row['outputFiles'] = files
                input_state = 'modified' if phase in ('edited', 'hot-edited', 'edited-uncached', 'edited-session-reset') else 'original'
                row['inputState'] = input_state
                key = w['name'], engine, input_state
                if key not in baselines:
                    reference = work / 'reference' / w['name'] / engine / input_state
                    copied = {}
                    for name, path in paths.items():
                        target = reference / name
                        target.parent.mkdir(parents=True, exist_ok=True)
                        shutil.copy2(path, target)
                        copied[name] = target
                    baselines[key] = files, copied
                compare_pair(baselines[key][0], files, baselines[key][1], paths)
                if phase == 'edited':
                    need(files != baselines[(w['name'], engine, 'original')][0], 'XML edit did not change output')
                if trial == 0 and phase == 'cold':
                    for member, path in paths.items():
                        if '/' not in member or state['fullContent'] and '/cdata/' not in member:
                            target = out / 'baseline-streams' / w['name'] / engine / member
                            target.parent.mkdir(parents=True, exist_ok=True)
                            shutil.copy2(path, target)
                row.update(status='passed', cacheBytes=sum(p.stat().st_size for name in ('assets', 'session', 'intermediate')
                    for p in (storage / name).rglob('*') if p.is_file()))
                return row, paths
            except Exception as error:
                process_receipt = out / label / 'process.json'
                if process_receipt.is_file():
                    row.update(json.loads(process_receipt.read_text(encoding='utf-8')))
                row.update(status='failed', error=type(error).__name__ + ': ' + str(error))
                if engine == 'new':
                    failed_report = profile / 'product/build-report.json'
                    if failed_report.is_file():
                        body = json.loads(failed_report.read_text(encoding='utf-8-sig'))
                        row.update(cache=body.get('cache'), diagnosticCounts=body.get('diagnosticCounts'),
                                   reportStatus=body.get('status'), reportElapsedMs=body.get('elapsedMs'))
                raise
            finally:
                if engine == 'new':
                    for name in ('build-report.json', 'build.events.jsonl'):
                        generated = profile / 'product' / name
                        if generated.is_file():
                            (out / label).mkdir(parents=True, exist_ok=True)
                            shutil.copy2(generated, out / label / name)
                print(json.dumps({k: row.get(k) for k in ('engine', 'workload', 'trial', 'mode', 'phase', 'wallMs', 'status')}), flush=True)
                report(out, state)

        for w in workloads:
            for trial in range(args.pairs + (0 if args.skip_extra_uncached else 1)):
                mode = 'both-deflate' if trial < args.pairs else 'none'
                phases = ('cold',) + ('hot',) * args.hot_rounds if mode != 'none' else ('uncached',)
                if args.cold_only and mode != 'none':
                    phases = ('cold',)
                if edit_path and mode != 'none':
                    phases += ('edited', 'hot-edited', 'restored')
                for index, phase in enumerate(phases):
                    if phase == 'edited':
                        edit_path.write_bytes(edit_modified)
                    elif phase == 'restored':
                        edit_path.write_bytes(edit_original)
                    engines = ('original', 'new') if (trial + index) % 2 == 0 else ('new', 'original')
                    if args.first_engine == 'new':
                        engines = tuple(reversed(engines))
                    if args.engine == 'new':
                        engines = ('new',)
                    pair, failures = {}, []
                    for engine in engines:
                        try:
                            pair[engine] = one(engine, w, trial, mode, phase, index)
                        except Exception as error:
                            failure = dict(engine=engine, trial=trial, mode=mode, phase=phase,
                                           error=type(error).__name__ + ': ' + str(error))
                            state['errors'].append(failure)
                            failures.append(failure)
                            if (args.engine == 'both' and args.first_engine == 'new' and trial == 0
                                    and phase == 'cold' and engine == 'new' and not args.continue_on_build_failure):
                                state['stoppedAfterFirstNewColdFailure'] = True
                                break
                    if failures:
                        report(out, state)
                        need(args.continue_on_build_failure, 'Paired build failed: ' + json.dumps(failures))
                        continue
                    if args.engine == 'new':
                        report(out, state)
                        continue
                    a, b = pair['original'], pair['new']
                    differences = sorted(n for n in a[0]['outputFiles'].keys() | b[0]['outputFiles'].keys()
                        if a[0]['outputFiles'].get(n) != b[0]['outputFiles'].get(n))
                    if not differences:
                        compare_pair(a[0]['outputFiles'], b[0]['outputFiles'], a[1], b[1])
                    state['pairs'].append(dict(workload=w['name'], trial=trial, mode=mode, phase=phase,
                        round=index, engineOrder=engines, rawBytesEqual=not differences, differentMembers=differences))
                    report(out, state)
                    if phase == 'edited' and trial == 0:
                        fresh_mode = args.rebuild_check
                        fresh_phase = 'edited-uncached' if fresh_mode == 'none' else 'edited-session-reset'
                        if fresh_mode == 'asset':
                            for engine in engines:
                                source = work / 'builds' / w['name'] / ('%s-%d' % (mode, trial)) / engine / 'assets'
                                destination = work / 'builds' / w['name'] / ('asset-%d' % trial) / engine / 'assets'
                                shutil.copytree(source, destination)
                        fresh = {engine: one(engine, w, trial, fresh_mode, fresh_phase, 0)
                                 for engine in reversed(engines)}
                        a, b = fresh['original'], fresh['new']
                        differences = sorted(n for n in a[0]['outputFiles'].keys() | b[0]['outputFiles'].keys()
                            if a[0]['outputFiles'].get(n) != b[0]['outputFiles'].get(n))
                        state['pairs'].append(dict(workload=w['name'], trial=trial, mode=fresh_mode,
                            phase=fresh_phase, round=0, engineOrder=tuple(reversed(engines)),
                            rawBytesEqual=not differences, differentMembers=differences))
                        state['edit']['freshRebuildMode'] = fresh_mode
                        state['edit']['freshRebuildMatchesIncremental'] = True
                        report(out, state)
        state['sdkUnchanged'] = sdk_source_inventory(sdk, workloads) == sdk_before
        state['modInputsUnchanged'] = all(inventory(w['modRoot']) == inputs_before[w['name']] for w in workloads)
        state['runtimeUnchanged'] = inventory(runtime) == actual and inventory(original['originalRuntimeDirectory']) == old_before
        state['rootExistenceUnchanged'] = all(Path(p).is_dir() == v for p, v in root_before.items())
        need(all(state[k] for k in ('sdkUnchanged', 'modInputsUnchanged', 'runtimeUnchanged', 'rootExistenceUnchanged')),
             'Shared input or runtime changed')
        state['outputComparisonStatus'] = ('outputs-differ' if any(not p['rawBytesEqual'] for p in state['pairs'])
                                           else 'equal' if state['pairs'] else 'not-compared')
        state['status'] = ('failed' if state['errors'] else 'outputs-differ'
                           if state['outputComparisonStatus'] == 'outputs-differ' else
                           'preflight-passed' if args.engine == 'new' else 'passed')
    except Exception as error:
        state.update(status='failed', error=type(error).__name__ + ': ' + str(error))
        raise
    finally:
        if edit_path and edit_original is not None:
            edit_path.write_bytes(edit_original)
        if schema_backup is not None:
            shutil.rmtree(repo / 'SDK/Schemas/xsd')
            shutil.copytree(schema_backup, repo / 'SDK/Schemas/xsd')
            state['schemaRestored'] = True
        state['elapsedSeconds'] = time.perf_counter() - started
        report(out, state)
    return 0 if state['status'] in ('passed', 'preflight-passed') else 1


if __name__ == '__main__':
    raise SystemExit(main())
