#!/usr/bin/env python3
"""Same-machine original BAB and accepted current Release, complete MOD base streams."""
import argparse
import ctypes
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import subprocess
import time

from BenchmarkCompilerCache import inventory, output_inventory, validate_sample
from BenchmarkCompilerSdkComparison import (compare_pair, invocation, measure, summaries,
    stream_inventory, validate_shared_report)
from HotCacheWorkload import HISTORICAL, UPHEAVAL_TREE, extract_upheaval
from PrepareCompilerSdkBenchmark import prepare, inventory as file_rows

SOURCE = '8050f22de81539c65aeca3903e9b48f53a572068'
RUN = '41244132060'
HEAD = '1a73017a5847a5657944866a0b25f4320c2ba00d'
FILES = {'NewModCompiler.exe': '2e04db720b2860bf337a66557572eb7ad2bc667b950e5a2ec69d81181c222b43', 'NewModCompiler.exe.config': '94eedb0cd0150b90c8115aaee53f64ab0523b39bd67887991b1b62679a752e68', 'antlr.runtime.dll': 'febdc86d6299a550c6ec87050b0cbf5205e821851e75494c699c17ce8ba2c62a'}


def need(value, message):
    if not value:
        raise ValueError(message)


def write(path, body):
    Path(path).write_text(json.dumps(body, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')


def git(repo, *args):
    return subprocess.check_output(['git', '-C', str(repo), *args], text=True).strip()


def report(out, state):
    state['summary'] = summaries(state['samples'])
    write(out / 'measurement.json', state)
    lines = ['# Original SDK versus current compiler', '', 'Status: ' + state['status'], '',
        state['scope'],
        'Base stream only, loose output. No BIG/SKUDEF, APT or global-metadata timing.',
        'Same Windows VM, SDK, physical MOD input, Schema, cwd and ordered search roots. Separate caches.',
        'Fresh process per sample, process launch through exit includes CLI/report overhead. Hash checks excluded.',
        'Cold = initially absent compiler cache/output namespace; OS page cache is uncontrolled.',
        'New Release jobs4, performance off; original BAB has its original serial implementation.', '',
        '| MOD | Mode | Phase | n/engine | Original seconds | New seconds | New time change |',
        '| --- | --- | --- | ---: | ---: | ---: | ---: |']
    for r in state['summary']:
        lines.append('| %s | %s | %s | %d | %.6f | %.6f | %+.2f%% |' % (
            r['workload'], r['mode'], r['phase'], r['nPerEngine'],
            r['originalWallMsMedian'] / 1000, r['newWallMsMedian'] / 1000,
            100 * (r['newOverOriginalWallRatio'] - 1)))
    lines += ['', 'All raw samples, CPU/peak working set and output identities are in measurement.json.',
        'All cache-state repeats must match their own engine baseline byte for byte.',
        'Cross-engine differences, if present, are retained below; such results do not establish equivalent output.',
        'Small sample counts describe this VM and workload; no statistical significance or game-runtime claim.', '']
    for p in state.get('pairs', []):
        if not p['rawBytesEqual']:
            lines.append('- Output mismatch: %s trial %s %s %s: %s' % (
                p['workload'], p['trial'], p['mode'], p['phase'], ', '.join(p['differentMembers'][:8])))
    if state.get('error'):
        lines += ['', 'Error: ' + state['error']]
    if state.get('edit'):
        lines += ['', 'Incremental edit: ' + json.dumps(state['edit'], ensure_ascii=False),
                  'Edited outputs are checked against a separate cache-disabled rebuild per engine. The source copy is restored.']
    if state.get('schemaOverlay'):
        lines += ['', 'Shared runtime Schema overlay: ' + json.dumps(state['schemaOverlay'], ensure_ascii=False)]
    (out / 'report.md').write_text('\n'.join(lines) + '\n', encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--accepted-build', type=Path, required=True)
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
    args = parser.parse_args()
    need(os.name == 'nt', 'Execution requires Windows')
    ctypes.WinDLL('kernel32', use_last_error=True).SetErrorMode(0x0001 | 0x0002 | 0x8000)
    repo = Path(__file__).resolve().parents[2]
    out, work = args.output.resolve(), args.work.resolve()
    out.mkdir(parents=True, exist_ok=False)
    work.mkdir(parents=True, exist_ok=False)
    state = dict(status='preparing', binarySourceCommit=SOURCE, sourceRunId=RUN,
        requestedHead=args.requested_head, measurementSource=git(repo, 'rev-parse', 'HEAD'),
        runId=os.environ.get('GITHUB_RUN_ID'), runAttempt=os.environ.get('GITHUB_RUN_ATTEMPT'),
        machine=dict(platform=platform.platform(), processor=platform.processor(),
                     cpuCount=os.cpu_count(), imageVersion=os.environ.get('ImageVersion')),
        scope='Complete mymod and untouched full historical Upheaval inputs, base loose stream only.',
        samples=[], pairs=[], errors=[], jobs=4, performance='off')
    started = time.perf_counter()
    schema_backup = None
    edit_path = None
    edit_original = None
    try:
        subprocess.run(['git', '-C', str(repo), 'merge-base', '--is-ancestor', SOURCE, 'HEAD'], check=True)
        closure = ('SDK', 'recovery/src', 'recovery/build', 'recovery/probes', 'ui',
            'recovery/scripts/Build*', 'recovery/scripts/PrepareLegacyTexture.ps1',
            'recovery/scripts/RestoreUiDependencies.ps1')
        need(not git(repo, 'diff', '--name-only', SOURCE, 'HEAD', '--', *closure),
             'Accepted binary production/SDK closure differs')
        receipt = json.loads((args.accepted_build / 'build.json').read_text(encoding='utf-8-sig'))
        need(receipt['sourceCommit'] == SOURCE and receipt['configuration'] == 'Release', 'Build source differs')
        runtime = args.accepted_build.resolve() / 'bin'
        actual = inventory(runtime)
        need({n: r['sha256'] for n, r in actual.items()} == FILES, 'Accepted three runtime files differ')
        shutil.copy2(args.accepted_build / 'build.json', out / 'accepted-build.json')
        if args.mod:
            mod = args.mod.resolve()
            need(mod.is_dir() and (mod / 'Data/Mod.xml').is_file(), 'External MOD entry is absent')
            target = work / 'inputs' / mod.name
            shutil.copytree(mod, target)
            need(inventory(target) == inventory(mod), 'External MOD copy differs')
            selected = ((mod.name, target, None, None),)
            state['scope'] = 'Complete downloaded ' + mod.name + ' source copy; no assets or includes removed.'
        else:
            mymod = work / 'inputs/mymod'
            shutil.copytree(repo / 'SDK/Mods/mymod', mymod)
            need(inventory(mymod) == inventory(repo / 'SDK/Mods/mymod'), 'mymod copy differs')
            upheaval = extract_upheaval(repo, work)
            selected = (('mymod', mymod, SOURCE, None), ('upheaval', upheaval, HISTORICAL, UPHEAVAL_TREE))
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
        prep = dict(succeeded=True, binarySourceCommit=SOURCE, sourceRunId=RUN,
            benchmarkSourceCommit=state['measurementSource'], sdkRoot=str(repo / 'SDK'),
            runtimeDirectory=str(runtime), runtimeFiles=file_rows(runtime, 'name'), workloads=workloads)
        write(out / 'preparation.json', prep)
        original_path = prepare(out / 'preparation.json', work / 'original', repo, SOURCE, RUN)
        original = json.loads(original_path.read_text(encoding='utf-8'))
        shutil.copy2(original_path, out / 'original-preparation.json')
        workloads = original['workloads']
        sdk = repo / 'SDK'
        sdk_before = inventory(sdk)
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

        def one(engine, w, trial, mode, phase, round_index):
            profile = work / 'builds' / w['name'] / ('%s-%d' % (mode, trial)) / engine
            if phase in ('cold', 'uncached', 'edited-uncached'):
                need(not profile.exists(), 'Cold namespace already exists')
            for name in ('assets', 'session', 'streams', 'product', 'intermediate',
                         'fresh-' + str(round_index), 'fresh-session-' + str(round_index)):
                (profile / name).mkdir(parents=True, exist_ok=True)
            command = invocation(engine, w, mode, profile, round_index, prep, original)
            if engine == 'new':
                command += ['--jobs', '4', '--performance', 'off']
            label = '%s-%d-%s-%s-%d-%s' % (w['name'], trial, mode, phase, round_index, engine)
            remaining = args.budget_seconds - (time.perf_counter() - started)
            need(remaining > 1, 'Total benchmark budget exhausted')
            row = measure(command, sdk, out / label, min(args.timeout_seconds, remaining), environment)
            row.update(engine=engine, workload=w['name'], trial=trial, mode=mode, phase=phase, round=round_index)
            state['samples'].append(row)
            report(out, state)
            if engine == 'new':
                build_report = json.loads((profile / 'product/build-report.json').read_text(encoding='utf-8-sig'))
                write(out / label / 'build-report.json', build_report)
                validation_phase = 'hot' if phase == 'hot-edited' else ('uncached' if phase == 'cold' else phase)
                validate_sample(build_report, mode, validation_phase)
                validate_shared_report(build_report, w, sdk)
                output_inventory(profile / 'product', build_report)
                row.update(cache=build_report['cache'], reportElapsedMs=build_report['elapsedMs'])
                if phase in ('hot', 'hot-edited'):
                    need(row['cache']['xmlParse'] == 0, 'Hot build unexpectedly parsed XML')
                if phase == 'edited':
                    need(row['cache']['instanceCompile'] > 0, 'Edited XML did not recompile an asset')
            elif phase in ('hot', 'hot-edited'):
                need(row['traceSessionLoaded'], 'Original hot process did not load session')
            elif mode == 'none':
                need(not row['traceStringHashesLoaded'], 'No-session original reused string hashes')
            files, paths = stream_inventory(profile / ('product/published' if engine == 'new' else 'streams'))
            row['outputFiles'] = files
            input_state = 'modified' if phase in ('edited', 'hot-edited', 'edited-uncached') else 'original'
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
            row.update(status='passed', cacheBytes=sum(p.stat().st_size for name in ('assets', 'session', 'intermediate')
                for p in (profile / name).rglob('*') if p.is_file()))
            print(json.dumps({k: row[k] for k in ('engine', 'workload', 'trial', 'mode', 'phase', 'wallMs')}), flush=True)
            report(out, state)
            return row, paths

        for w in workloads:
            for trial in range(args.pairs + 1):
                mode = 'both-deflate' if trial < args.pairs else 'none'
                phases = ('cold', 'hot', 'hot') if mode != 'none' else ('uncached',)
                if edit_path and mode != 'none':
                    phases += ('edited', 'hot-edited', 'restored')
                for index, phase in enumerate(phases):
                    if phase == 'edited':
                        edit_path.write_bytes(edit_modified)
                    elif phase == 'restored':
                        edit_path.write_bytes(edit_original)
                    engines = ('original', 'new') if (trial + index) % 2 == 0 else ('new', 'original')
                    pair = {engine: one(engine, w, trial, mode, phase, index) for engine in engines}
                    a, b = pair['original'], pair['new']
                    differences = sorted(n for n in a[0]['outputFiles'].keys() | b[0]['outputFiles'].keys()
                        if a[0]['outputFiles'].get(n) != b[0]['outputFiles'].get(n))
                    if trial == 0 and phase == 'cold':
                        for engine in ('original', 'new'):
                            for member, path in pair[engine][1].items():
                                if '/' not in member:
                                    target = out / 'baseline-streams' / w['name'] / engine / member
                                    target.parent.mkdir(parents=True, exist_ok=True)
                                    shutil.copy2(path, target)
                    if not differences:
                        compare_pair(a[0]['outputFiles'], b[0]['outputFiles'], a[1], b[1])
                    state['pairs'].append(dict(workload=w['name'], trial=trial, mode=mode, phase=phase,
                        round=index, engineOrder=engines, rawBytesEqual=not differences, differentMembers=differences))
                    report(out, state)
                    if phase == 'edited' and trial == 0:
                        fresh = {engine: one(engine, w, trial, 'none', 'edited-uncached', 0)
                                 for engine in reversed(engines)}
                        a, b = fresh['original'], fresh['new']
                        differences = sorted(n for n in a[0]['outputFiles'].keys() | b[0]['outputFiles'].keys()
                            if a[0]['outputFiles'].get(n) != b[0]['outputFiles'].get(n))
                        state['pairs'].append(dict(workload=w['name'], trial=trial, mode='none',
                            phase='edited-uncached', round=0, engineOrder=tuple(reversed(engines)),
                            rawBytesEqual=not differences, differentMembers=differences))
                        state['edit']['freshRebuildMatchesIncremental'] = True
                        report(out, state)
        state['sdkUnchanged'] = inventory(sdk) == sdk_before
        state['modInputsUnchanged'] = all(inventory(w['modRoot']) == inputs_before[w['name']] for w in workloads)
        state['runtimeUnchanged'] = inventory(runtime) == actual and inventory(original['originalRuntimeDirectory']) == old_before
        state['rootExistenceUnchanged'] = all(Path(p).is_dir() == v for p, v in root_before.items())
        need(all(state[k] for k in ('sdkUnchanged', 'modInputsUnchanged', 'runtimeUnchanged', 'rootExistenceUnchanged')),
             'Shared input or runtime changed')
        state['status'] = 'passed' if all(p['rawBytesEqual'] for p in state['pairs']) else 'outputs-differ'
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
    return 0 if state['status'] == 'passed' else 1


if __name__ == '__main__':
    raise SystemExit(main())
