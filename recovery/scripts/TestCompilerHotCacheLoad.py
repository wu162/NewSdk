#!/usr/bin/env python3
"""Calibrate a real hot-cache workload to at least eight seconds per process."""
import argparse
import ctypes
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import time
import zipfile

from BenchmarkCompilerCache import inventory, process_metrics, validate_sample
from TestCompilerHotCache import declaration, need, published, remaining_timeout, stage_runtime, validate_hot, write

BINARY_SOURCE = '91b040d271b7a3cc25d32f0f923c018c740edf4a'
SOURCE_RUN = 41010073140
ARTIFACT_ID = 12957004213
ARCHIVE_SHA256 = '9f8b1ad1bba8c3f330475586496722773714fa99143d75adf136ae464493680c'
DOCUMENT_LEVELS = (8192, 16384, 32768)
ASSETS_PER_DOCUMENT = 4


def asset_hash(name):
    """The existing FastHash ASCII arithmetic; only used to prepare valid IDs."""
    data = name.lower().encode('ascii')
    mask = 0xffffffff
    value = len(data)
    offset = 0
    while offset + 4 <= len(data):
        value = (value + int.from_bytes(data[offset:offset + 2], 'little')) & mask
        value ^= ((int.from_bytes(data[offset + 2:offset + 4], 'little') ^ (value << 5)) << 11) & mask
        value = (value + (value >> 11)) & mask
        offset += 4
    tail = data[offset:]
    if len(tail) == 3:
        value = (value + int.from_bytes(tail[:2], 'little')) & mask
        value ^= (((tail[2] << 2) ^ value) << 16) & mask
        value = (value + (value >> 11)) & mask
    elif len(tail) == 2:
        value = (value + int.from_bytes(tail, 'little')) & mask
        value ^= (value << 11) & mask
        value = (value + (value >> 17)) & mask
    elif tail:
        value = (value + tail[0]) & mask
        value ^= (value << 10) & mask
        value = (value + (value >> 1)) & mask
    value ^= (value << 3) & mask
    value = (value + (value >> 5)) & mask
    value ^= (value << 2) & mask
    value = (value + (value >> 15)) & mask
    return value ^ ((value << 10) & mask)


class AssetNames:
    def __init__(self):
        self.names, self.hashes, self.cursor, self.skipped = [], set(), 0, 0

    def extend(self, count):
        while len(self.names) < count:
            name = 'Load' + hashlib.sha256(str(self.cursor).encode('ascii')).hexdigest()[:12]
            self.cursor += 1
            identity = asset_hash(name)
            if identity in self.hashes:
                self.skipped += 1
                continue
            self.hashes.add(identity)
            self.names.append(name)
        return self.names


def prepare_runtime(repo, work):
    # Production inputs are unchanged: reuse the exact already accepted bytes.
    subprocess.run(['git', '-C', str(repo), 'merge-base', '--is-ancestor', BINARY_SOURCE, 'HEAD'], check=True)
    subprocess.run(['git', '-C', str(repo), 'diff', '--exit-code', BINARY_SOURCE, '--',
                    'SDK', 'recovery/src', 'recovery/build', 'recovery/probes',
                    'recovery/scripts/Build*', 'recovery/scripts/PrepareLegacyTexture.ps1'], check=True)
    archive = work / 'accepted.zip'
    with archive.open('wb') as stream:
        subprocess.run(['gh', 'api', 'repos/wu162/NewSdk/actions/artifacts/%d/zip' % ARTIFACT_ID],
                       stdout=stream, check=True, timeout=30)
    with archive.open('rb') as stream:
        need(hashlib.file_digest(stream, 'sha256').hexdigest() == ARCHIVE_SHA256, 'Accepted archive digest differs')
    build = work / 'accepted-build'
    (build / 'bin').mkdir(parents=True)
    with zipfile.ZipFile(archive) as bundle:
        prefix = 'artifacts/performance-product/'
        (build / 'build.json').write_bytes(bundle.read(prefix + 'build.json'))
        receipt = json.loads((build / 'build.json').read_text(encoding='utf-8-sig'))
        for name in receipt['runtimeFiles']:
            need(Path(name).name == name and '/' not in name and '\\' not in name, 'Unexpected runtime member')
            (build / 'bin' / name).write_bytes(bundle.read(prefix + 'bin/' + name))
    return stage_runtime(build, BINARY_SOURCE, work / 'runtime')


def grow_fixture(mod, previous, documents, asset_names=None):
    """Append documents/assets; keep prior documents byte-identical for reuse."""
    need(0 <= previous < documents, 'Fixture must grow')
    data = mod / 'Data'
    data.mkdir(parents=True, exist_ok=True)
    asset_names = asset_names or AssetNames()
    names = asset_names.extend(documents * ASSETS_PER_DOCUMENT)
    for index in range(previous, documents):
        assets = ''.join('<ArmorTemplate id="%s" Default="72.5%%"/>' % names[asset]
                         for asset in range(index * ASSETS_PER_DOCUMENT, (index + 1) * ASSETS_PER_DOCUMENT))
        (data / ('armor-%05d.xml' % index)).write_text(declaration(assets), encoding='utf-8')
    includes = ''.join('<Include type="all" source="armor-%05d.xml"/>' % i for i in range(documents))
    (data / 'Mod.xml').write_text(declaration('<Includes>' + includes + '</Includes>'), encoding='utf-8')
    return {'documents': documents + 1, 'armorDocuments': documents,
            'assets': documents * ASSETS_PER_DOCUMENT, 'newDocuments': documents - previous,
            'hashCollisionsSkipped': asset_names.skipped,
            'scope': 'Synthetic Armor document/asset scale, loose base stream; not full Upheaval or a complete MOD'}


def meets_minimum(durations, minimum):
    return len(durations) == 2 and all(value >= minimum for value in durations)


def save(out, state):
    write(out / 'load.json', state)
    lines = ['# Hot-cache workload duration floor', '', 'Status: ' + state['status'], '',
             'Each hot sample is one fresh compiler process with real retained caches. No sleep or inner repetition.',
             'Summary profiling; loose base stream. Wall time includes startup/build/publication, excludes byte comparison.',
             'Fixture growth, preparation, warmups, comparisons and all hot samples share the total load budget.', '',
             '| Documents | Assets | Phase | Process seconds | XML parses | Compiles | Reused documents |',
             '| ---: | ---: | --- | ---: | ---: | ---: | ---: |']
    for row in state['samples']:
        cache = row.get('cache', {})
        lines.append('| {} | {} | {} | {:.3f} | {} | {} | {} |'.format(
            row['fixture']['documents'], row['fixture']['assets'], row['phase'], row.get('wallSeconds', 0),
            cache.get('xmlParse', ''), cache.get('instanceCompile', ''), cache.get('documentReuse', '')))
    if state.get('error'):
        lines += ['', state['error']]
    (out / 'load.md').write_text('\n'.join(lines) + '\n', encoding='utf-8')


def run(args):
    if os.name == 'nt':
        ctypes.windll.kernel32.SetErrorMode(0x0001 | 0x0002 | 0x8000)
    repo = Path(__file__).resolve().parents[2]
    work, out = Path(args.work).resolve(), Path(args.output).resolve()
    need(not work.exists() and not out.exists(), 'Refusing to overwrite work/evidence')
    work.mkdir(parents=True)
    out.mkdir(parents=True)
    state = {'status': 'started', 'samples': [], 'binarySourceCommit': BINARY_SOURCE,
             'testSourceCommit': subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD'], text=True).strip(),
             'runId': os.getenv('GITHUB_RUN_ID'), 'runAttempt': os.getenv('GITHUB_RUN_ATTEMPT'),
             'sourceRunId': SOURCE_RUN, 'sourceArtifactId': ARTIFACT_ID, 'sourceArchiveSha256': ARCHIVE_SHA256,
             'limits': {'minimumHotSeconds': args.minimum_hot_seconds, 'totalSeconds': args.budget,
                        'perProcessSeconds': args.timeout, 'maximumCalibrationLevels': len(DOCUMENT_LEVELS)},
             'scope': 'Duration floor for a large synthetic hot-cache load; does not represent full-MOD performance'}
    started = time.perf_counter()
    try:
        receipt, identities = prepare_runtime(repo, work)
        write(out / 'product-build.json', receipt)
        state['runtimeFiles'] = identities
        runtime = work / 'runtime'
        mod = work / 'LoadMod'
        destination = work / 'build'
        # Runtime reuse/download is recorded separately; everything from fixture
        # generation through the final comparison is governed by the load budget.
        state['runtimePreparationSeconds'] = time.perf_counter() - started
        started = time.perf_counter()

        def sample(fixture, phase, baseline=None):
            label = '%02d-%s' % (len(state['samples']), phase)
            evidence = out / label
            evidence.mkdir()
            command = [str(runtime / 'NewModCompiler.exe'), 'build', '--sdk-root', str(repo / 'SDK'),
                       '--mod', str(mod), '--out', str(destination), '--package-content', 'base',
                       '--package-format', 'loose', '--include-additional=false', '--jobs', '4', '--performance', 'summary']
            row = {'label': label, 'phase': phase, 'fixture': fixture, 'status': 'started', 'command': command}
            state['samples'].append(row)
            write(evidence / 'request.json', row)
            save(out, state)
            before = {p.name: p.stat().st_mtime_ns for p in (destination / 'published').rglob('*') if p.is_file()}
            tick = time.perf_counter()
            try:
                with (evidence / 'stdout.log').open('wb') as stdout, (evidence / 'stderr.log').open('wb') as stderr:
                    process = subprocess.Popen(command, cwd=runtime, stdout=stdout, stderr=stderr)
                    try:
                        process.wait(timeout=remaining_timeout(started, args.budget, args.timeout))
                    except (subprocess.TimeoutExpired, TimeoutError):
                        process.kill()
                        process.wait(timeout=5)
                        row['status'] = 'timed-out'
                        raise
                    row.update(wallSeconds=time.perf_counter() - tick, exitCode=process.returncode,
                               **process_metrics(process))
                need(process.returncode == 0, label + ' build failed')
            finally:
                for name in ('build-report.json', 'build.events.jsonl'):
                    if (destination / name).exists():
                        shutil.copy2(destination / name, evidence / name)
            report = json.loads((evidence / 'build-report.json').read_text(encoding='utf-8-sig'))
            validate_sample(report, 'both-deflate', 'hot' if phase.startswith('hot') else 'warmup')
            need(report['diagnosticCounts']['error'] == 0, 'Compilation errors')
            content = published(destination, report)
            need(len(content) == 4 and {Path(p).suffix for p in content} == {'.manifest', '.bin', '.imp', '.relo'},
                 'Expected exactly four linked output files')
            row['counters'] = report['performance']['counters']
            if phase.startswith('hot'):
                after = {p.name: p.stat().st_mtime_ns for p in (destination / 'published').rglob('*') if p.is_file()}
                validate_hot(report, 'both-deflate', 'loose', before, after, has_payload=False)
                need(content == baseline, 'Hot output raw bytes differ from warmup')
                need(not any(c['name'] == 'cache.invalidation.reloads' and c['value'] for c in row['counters']),
                     'Hot workload contained invalidations')
            row.update(status='passed', cache=report['cache'], rawBytesEqual=baseline is not None,
                       outputBytes=sum(map(len, content.values())))
            save(out, state)
            print(label + ' %.3f seconds' % row['wallSeconds'], flush=True)
            return content

        previous = 0
        asset_names = AssetNames()
        for documents in DOCUMENT_LEVELS:
            remaining_timeout(started, args.budget, args.timeout)
            fixture = grow_fixture(mod, previous, documents, asset_names)
            previous = documents
            # Later warmups are incremental growth, not independent cold builds.
            baseline = sample(fixture, 'warmup')
            sample(fixture, 'hot-calibration', baseline)
            durations = [state['samples'][-1]['wallSeconds']]
            if durations[0] >= args.minimum_hot_seconds:
                sample(fixture, 'hot-confirmation', baseline)
                durations.append(state['samples'][-1]['wallSeconds'])
                if meets_minimum(durations, args.minimum_hot_seconds):
                    remaining_timeout(started, args.budget, args.timeout)
                    state.update(status='passed', selectedFixture=fixture, qualifiedHotSeconds=durations)
                    need(inventory(runtime) == identities, 'Runtime changed during load validation')
                    return 0
        raise AssertionError('No workload produced two individual hot processes at or above the duration floor')
    except Exception as error:
        state.update(status='failed', error=type(error).__name__ + ': ' + str(error))
        for row in state['samples']:
            if row['status'] == 'started':
                row['status'] = 'failed'
        return 1
    finally:
        state['elapsedSeconds'] = time.perf_counter() - started
        save(out, state)
        print(json.dumps({k: state.get(k) for k in ('status', 'elapsedSeconds', 'qualifiedHotSeconds', 'error')}), flush=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--work', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--minimum-hot-seconds', type=float, default=8)
    parser.add_argument('--budget', type=int, choices=range(30, 181), default=120)
    parser.add_argument('--timeout', type=int, choices=range(10, 91), default=60)
    raise SystemExit(run(parser.parse_args()))
