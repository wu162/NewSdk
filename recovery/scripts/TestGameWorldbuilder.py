#!/usr/bin/env python3
"""Focused real-game-layout WorldBuilder extraction, configuration and ZIP gate.

Builds a four-member WBData_12.big from unchanged official stream bytes. Private
SDKs have no resolver quartet or packed payload, so they must use game discovery
and the new extraction cache. No installed game, repository SDK or MOD is edited.
"""
import argparse
from contextlib import contextmanager
import hashlib
import json
import os
from pathlib import Path
import shutil
import struct
import subprocess
import tempfile
import traceback
import xml.etree.ElementTree as ET
import zipfile

from ImportWorldbuilderReferences import unpack
from TestNeutralWorldbuilder import ROOT, EXTENSIONS, RUNTIME, need, run, oracle, preflight
from ValidateCompilerBig import validate


def digest(path):
    result = hashlib.sha256()
    with path.open('rb') as stream:
        for chunk in iter(lambda: stream.read(8 * 1024 * 1024), b''):
            result.update(chunk)
    return result.hexdigest()


def identity(path):
    return {'name': path.name, 'bytes': path.stat().st_size, 'sha256': digest(path)}


def metadata_snapshot(directory):
    return {p.relative_to(directory).as_posix(): {'bytes': p.stat().st_size, 'mtimeNs': p.stat().st_mtime_ns}
            for p in sorted(directory.rglob('*')) if p.is_file()}


def input_snapshot(directory):
    return {p.relative_to(directory).as_posix(): {**identity(p), 'mtimeNs': p.stat().st_mtime_ns}
            for p in sorted(directory.rglob('*')) if p.is_file() and 'SuperCompilerCache' not in p.parts}


def copy_sdk(target):
    def ignored(path, names):
        result = {n for n in names if n in ('Mods', 'SuperCompilerCache')}
        if 'builtmods' in [part.lower() for part in Path(path).parts]:
            result.update(n for n in names if n.lower().startswith('worldbuilder') and
                          (Path(path).name.lower() == 'builtmods' or n.lower() != 'worldbuilder.manifest'))
        return result
    shutil.copytree(ROOT / 'SDK', target, ignore=ignored)
    for folder in (target / 'builtmods', target / 'builtmods/sagexml'):
        for extension in ('.bin', '.relo', '.imp', '.bin.parts'):
            need(not (folder / ('Worldbuilder' + extension)).exists(), 'Private SDK still masks automatic game extraction')


def settings(config_path, game_root=None):
    tree = ET.parse(config_path)
    node = tree.getroot().find('appSettings')
    if node is None:
        node = ET.SubElement(tree.getroot(), 'appSettings')
    for child in list(node):
        if child.get('key') in ('game-root', 'fix-neutral-assets'):
            node.remove(child)
    ET.SubElement(node, 'add', key='fix-neutral-assets', value='false')
    if game_root is not None:
        ET.SubElement(node, 'add', key='game-root', value=str(game_root))
    tree.write(config_path, encoding='utf-8', xml_declaration=True)


def create_game(work):
    source = ROOT / 'SDK/builtmods/sagexml/Worldbuilder.manifest'
    raw = work / 'Worldbuilder.bin'
    restored = unpack(source.with_suffix('.bin').with_name('Worldbuilder.bin.parts'), raw)
    members = [(('data\\worldbuilder' + ext).encode('ascii'), raw if ext == '.bin' else source.with_suffix(ext))
               for ext in EXTENSIONS]
    table_bytes = sum(8 + len(name) + 1 for name, _ in members)
    header_bytes = 16 + table_bytes
    total = header_bytes + sum(path.stat().st_size for _, path in members)
    game = work / 'game'
    big = game / 'Data/WBData_12.big'
    big.parent.mkdir(parents=True)
    stream_ids = {path.suffix: identity(path) for _, path in members}
    with big.open('wb') as target:
        target.write(b'BIG4' + struct.pack('<I', total) + struct.pack('>II', len(members), header_bytes))
        offset = header_bytes
        for name, path in members:
            target.write(struct.pack('>II', offset, path.stat().st_size) + name + b'\0')
            offset += path.stat().st_size
        for _, path in members:
            with path.open('rb') as source_stream:
                shutil.copyfileobj(source_stream, target, 8 * 1024 * 1024)
    raw.unlink()
    need(big.stat().st_size == total, 'Game-layout BIG length differs')
    return game, {**identity(big), 'mtimeNs': big.stat().st_mtime_ns, 'officialStreams': stream_ids,
                  'officialBinRestored': restored}


@contextmanager
def registry_game(game, active_view):
    """Temporarily set original EA Install Dir, restoring values and created keys."""
    import winreg
    key_path = r'SOFTWARE\Electronic Arts\Electronic Arts\Red Alert 3'
    value_name = 'Install Dir'
    snapshots = []
    try:
        for view_name, view in ((32, winreg.KEY_WOW64_32KEY), (64, winreg.KEY_WOW64_64KEY)):
            absent = []
            parts = key_path.split('\\')
            for count in range(1, len(parts) + 1):
                partial = '\\'.join(parts[:count])
                try:
                    with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, partial, 0, winreg.KEY_READ | view):
                        pass
                except FileNotFoundError:
                    absent.append(partial)
            with winreg.CreateKeyEx(winreg.HKEY_LOCAL_MACHINE, key_path, 0, winreg.KEY_READ | winreg.KEY_WRITE | view) as key:
                try:
                    previous = winreg.QueryValueEx(key, value_name)
                except FileNotFoundError:
                    previous = None
                snapshots.append((view, previous, absent))
                if view_name == active_view:
                    winreg.SetValueEx(key, value_name, 0, winreg.REG_SZ, str(game))
                else:
                    try:
                        winreg.DeleteValue(key, value_name)
                    except FileNotFoundError:
                        pass
        yield {'key': key_path, 'value': value_name, 'view': active_view}
    finally:
        for view, previous, absent in reversed(snapshots):
            with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, key_path, 0, winreg.KEY_WRITE | view) as key:
                if previous is None:
                    try:
                        winreg.DeleteValue(key, value_name)
                    except FileNotFoundError:
                        pass
                else:
                    winreg.SetValueEx(key, value_name, 0, previous[1], previous[0])
            for partial in reversed(absent):
                winreg.DeleteKeyEx(winreg.HKEY_LOCAL_MACHINE, partial, view, 0)


def package_and_deploy(product, mode, game, expected, work, out, source_commit):
    packages = out / 'packages'
    packages.mkdir(exist_ok=True)
    archive = packages / ('NewModCompiler-Win32-' + mode + '.zip')
    files = [(name, (product / 'bin' / name).read_bytes()) for name in RUNTIME]
    with zipfile.ZipFile(archive, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as target:
        for name, data in files:
            target.writestr(name, data)
    deployment = work / ('zip-' + mode)
    deployment.mkdir()
    runtime, sdk, mod = deployment / 'runtime', deployment / 'sdk', deployment / 'NeutralWbProbe'
    with zipfile.ZipFile(archive) as source:
        need(source.namelist() == list(RUNTIME), 'Independent ZIP runtime layout differs')
        for name, data in files:
            need(source.read(name) == data, 'Independent ZIP runtime bytes differ')
        source.extractall(runtime)
    copy_sdk(sdk)
    shutil.copytree(ROOT / 'recovery/fixtures/neutral-worldbuilder', mod)
    destination = deployment / 'out'
    case = out / mode / 'zip-deployment'
    run([runtime / 'NewModCompiler.exe', 'build', '--mod', mod, '--sdk-root', sdk, '--game-root', game,
         '--out', destination, '--mod-version', '1.0'], runtime, case)
    report = json.loads((destination / 'build-report.json').read_text(encoding='utf-8-sig'))
    need(report['exitCode'] == 0 and not report['externalTools'], 'Independent ZIP did not compile in process')
    artifacts = []
    for entry in report['artifacts']:
        file = Path(entry['Path'])
        need(file.read_bytes() == expected[file.suffix], 'Independent ZIP game-BIG build differs from accepted raw package')
        artifacts.append(identity(file))
    need(len(artifacts) == 2, 'Independent ZIP did not publish BIG/SKUDEF')
    shutil.copyfile(destination / 'build-report.json', case / 'build-report.json')
    result = {**identity(archive), 'configuration': mode, 'sourceCommit': source_commit,
              'runtimeFiles': [{'name': name, 'bytes': len(data), 'sha256': hashlib.sha256(data).hexdigest()} for name, data in files],
              'independentZipDeployment': {'succeeded': True, 'coldGameExtraction': True,
                  'exactAcceptedRawOutputs': True, 'artifacts': artifacts}}
    shutil.rmtree(deployment)
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--product-root', type=Path)
    parser.add_argument('--output', type=Path)
    parser.add_argument('--preflight', action='store_true')
    args = parser.parse_args()
    preflight()
    if args.preflight:
        return
    need(os.name == 'nt', 'Actual registry/game extraction validation requires Windows')
    need(args.product_root and args.output, '--product-root and --output are required')
    out = args.output.resolve()
    out.mkdir(parents=True, exist_ok=False)
    product_root = args.product_root.resolve()
    commit = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip()
    state = {'schemaVersion': 1, 'succeeded': False, 'sourceCommit': commit,
             'runId': os.environ.get('GITHUB_RUN_ID'), 'runAttempt': os.environ.get('GITHUB_RUN_ATTEMPT'),
             'modes': [], 'packages': [],
             'scope': 'Debug/Release actual two-asset MOD; game-layout official BIG extraction, CLI/project/appSettings precedence, both original registry views, extraction cold/hot/failed retry, unchanged inputs, original HashFix/Resolver/LoD/MakeBig raw differential and independent three-file ZIP cold game extraction. No full MOD or in-game acceptance.'}
    work = Path(tempfile.mkdtemp(prefix='gwb-', dir=Path(os.environ['RUNNER_TEMP']).resolve().anchor))
    try:
        game, game_identity = create_game(work)
        state['gameInput'] = game_identity
        big = game / 'Data/WBData_12.big'
        for mode in ('Debug', 'Release'):
            product = product_root / mode
            receipt = json.loads((product / 'build.json').read_text(encoding='utf-8-sig'))
            need(receipt['sourceCommit'] == commit and receipt['configuration'] == mode and
                 digest(product / 'bin/NewModCompiler.exe') == receipt['exeSha256'], 'Product source/binary identity differs')
            mode_work = work / mode
            mode_work.mkdir()
            sdk, mod, runtime = mode_work / 'sdk', mode_work / 'NeutralWbProbe', mode_work / 'runtime'
            copy_sdk(sdk)
            shutil.copytree(ROOT / 'recovery/fixtures/neutral-worldbuilder', mod)
            runtime.mkdir()
            for name in RUNTIME:
                shutil.copy2(product / 'bin' / name, runtime / name)
            sdk_before, data_before = input_snapshot(sdk), input_snapshot(mod / 'Data')
            row = {'configuration': mode, 'passed': False, 'exeSha256': receipt['exeSha256'], 'cases': []}
            state['modes'].append(row)

            def cli(name, root_source='commandLine', expected_exit=0, loose=False, destination=None):
                preset = {'package-content': 'base', 'package-format': 'loose' if loose else 'big',
                          'include-additional': False, 'fix-neutral-assets': not loose}
                bad = work / 'missing-game'
                if root_source in ('commandLine', 'project'):
                    preset['game-root'] = str(game if root_source == 'project' else bad)
                (mod / 'newmodcompiler.json').write_text(json.dumps(preset, indent=2) + '\n')
                settings(runtime / 'NewModCompiler.exe.config',
                         game if root_source == 'applicationConfig' else bad if root_source in ('commandLine', 'project') else None)
                destination = destination or mode_work / name
                command = [runtime / 'NewModCompiler.exe', 'build', '--mod', mod, '--sdk-root', sdk, '--out', destination]
                if not loose:
                    command += ['--mod-version', '1.0']
                if root_source == 'commandLine':
                    command += ['--game-root', game]
                case = out / mode / name
                run(command, runtime, case, expected_exit)
                report = json.loads((destination / 'build-report.json').read_text(encoding='utf-8-sig'))
                need(report['exitCode'] == expected_exit and not report['externalTools'], 'Actual game extraction CLI report differs')
                if root_source != 'registry':
                    option = report['effectiveOptions']['game-root']
                    need(option['Source'] == root_source and Path(option['Value']) == game,
                         'Effective game-root precedence differs: ' + name)
                archived = case / 'artifacts'
                archived.mkdir()
                files = []
                for entry in report['artifacts']:
                    source = Path(entry['Path'])
                    target = archived / source.relative_to(destination / 'published')
                    target.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copyfile(source, target)
                    files.append(target)
                shutil.copyfile(destination / 'build-report.json', case / 'build-report.json')
                row['cases'].append({'name': name, 'passed': True, 'expectedExit': expected_exit,
                                     'rootSource': root_source, 'cache': report.get('cache')})
                return files, report

            loose, _ = cli('loose-source', loose=True)
            source = next(p for p in loose if p.suffix == '.manifest')
            original = oracle(source, ROOT / 'SDK', out / mode / 'original-chain')
            row['originalChain'] = {k: v for k, v in original.items() if k != 'expected'}
            need(original['hashFixChangedCount'] == 1 and original['resolverInsertedAssets'] == 5,
                 'Real official neutral fixture repair scope changed')
            cold, _ = cli('explicit-cold')
            cold_big = next(p for p in cold if p.suffix == '.big')
            validate(cold_big, original['expected'])
            expected_packages = {p.suffix: p.read_bytes() for p in cold}
            cache = next((sdk / 'SuperCompilerCache').glob('*/neutral-worldbuilder'))
            cache_before = metadata_snapshot(cache)
            need(len(list(cache.glob('Worldbuilder.*'))) == 4 and (cache / 'source.json').is_file(),
                 'First game extraction did not create exactly four cached streams and source metadata')
            cached_source = json.loads((cache / 'source.json').read_text(encoding='utf-8-sig'))
            need(Path(cached_source['sourcePath']) == big and cached_source['sourceBytes'] == big.stat().st_size,
                 'Extraction cache is not bound to the actual game BIG')
            for name, root_source in (('explicit-hot', 'commandLine'), ('project-over-app', 'project'),
                                      ('application-config', 'applicationConfig')):
                files, _ = cli(name, root_source)
                need(all(p.read_bytes() == expected_packages[p.suffix] for p in files), 'Hot/config raw package differs')
                need(metadata_snapshot(cache) == cache_before, 'Extraction hot hit rewrote cached files/receipt')
            for view in (32, 64):
                with registry_game(game, view) as registry:
                    files, _ = cli('registry-' + str(view), 'registry')
                    need(all(p.read_bytes() == expected_packages[p.suffix] for p in files), 'Registry auto-discovery raw package differs')
                    row['cases'][-1]['registry'] = registry
                row['cases'][-1]['registryRestored'] = True
            held = big.with_name(big.name + '.held')
            big.rename(held)
            retry = mode_work / 'retry'
            try:
                files, report = cli('missing-game-big', expected_exit=3, destination=retry)
                need(not files and 'PACKAGE_NEUTRAL_REFERENCE_MISSING' in json.dumps(report),
                     'Missing game BIG did not retain the established failure diagnostic')
                need(metadata_snapshot(cache) == cache_before, 'Missing game BIG changed the last valid extraction cache')
            finally:
                held.rename(big)
            files, _ = cli('restored-game-retry', destination=retry)
            need(all(p.read_bytes() == expected_packages[p.suffix] for p in files), 'Restored game BIG retry raw package differs')
            need(metadata_snapshot(cache) == cache_before, 'Unchanged restored game BIG rewrote extraction cache')
            for extension, expected_stream in game_identity['officialStreams'].items():
                cached = cache / ('Worldbuilder' + extension)
                need(cached.stat().st_size == expected_stream['bytes'] and digest(cached) == expected_stream['sha256'],
                     'Extracted game stream differs from unchanged official source bytes')
            need(input_snapshot(sdk) == sdk_before and input_snapshot(mod / 'Data') == data_before,
                 'Production changed SDK or MOD source inputs')
            row['inputsUnchanged'] = row['hotExtractionMetadataUnchanged'] = row['exactOriginalChainRawMembers'] = True
            row['passed'] = True
            shutil.rmtree(mode_work)
            package = package_and_deploy(product, mode, game, expected_packages, work, out, commit)
            package['neutralAcceptance'] = {'hashFixChangedCount': 1, 'resolverInsertedAssets': 5,
                                           'caseCount': len(row['cases']), 'rawOriginalMembers': True}
            state['packages'].append(package)
        need(big.stat().st_mtime_ns == game_identity['mtimeNs'] and digest(big) == game_identity['sha256'],
             'Game BIG bytes or mtime changed')
        state['gameInputUnchanged'] = state['succeeded'] = True
        (out / 'packages/manifest.json').write_text(json.dumps({k: state[k] for k in
            ('schemaVersion', 'sourceCommit', 'runId', 'runAttempt', 'scope', 'packages')}, indent=2) + '\n')
    except Exception as error:
        state['error'] = str(error)
        (out / 'error.log').write_text(traceback.format_exc(), encoding='utf-8')
        raise
    finally:
        (out / 'validation.json').write_text(json.dumps(state, indent=2) + '\n')
        for mode in ('Debug', 'Release'):
            (out / mode / 'original-chain/SDK/BuiltMods/Worldbuilder.bin').unlink(missing_ok=True)
        shutil.rmtree(work)


if __name__ == '__main__':
    main()
