#!/usr/bin/env python3
"""Compile original TNW 7.1 media in all 15 streams, excluding only Joypad XML."""
import argparse
import collections
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import traceback

from BenchmarkCompilerCache import inventory
from BenchmarkCompilerSdkComparison import measure, root_arguments, validate_shared_report
from MeasureCompilerOriginalMod import discover_tasks, full_inventory, stream_times
from UpheavalEditWorkload import xml_tags


FX_NAME = '166b084d.53c81e47.8192c84b.8d81c6ff.cdata'
FX_SHA256 = '892b3c9b00929a2adb28d88d8c455401ebac72d82313b7cb878c54851a75fd67'


def need(value, message):
    if not value:
        raise ValueError(message)


def write(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')


def digest(data):
    return hashlib.sha256(data).hexdigest()


def remove_joypad(mod, backup, changes, originals):
    opening = re.compile(rb'<(?:[A-Za-z_][\w.-]*:)?JoypadCommandBarButtonTemplate(?=[\s/>])')
    for path in sorted(p for folder in ('Data', 'Art') for p in (mod / folder).rglob('*') if p.suffix.lower() == '.xml'):
        data = path.read_bytes()
        visible = re.sub(rb'<!--.*?-->|<!\[CDATA\[.*?\]\]>|<\?.*?\?>', b'', data, flags=re.S)
        if not opening.search(visible):
            continue
        spans = [tag for tag in xml_tags(data) if 'elementEnd' in tag and
                 tag['topType'] == 'JoypadCommandBarButtonTemplate' and tag['topId']]
        if not spans:
            continue
        relative = path.relative_to(mod)
        target = backup / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(data)
        originals[path] = data
        changed = data
        for tag in reversed(spans):
            changed = changed[:tag['start']] + changed[tag['elementEnd']:]
        path.write_bytes(changed)
        changes.append(dict(path=relative.as_posix(), originalSha256=digest(data),
                            modifiedSha256=digest(changed),
                            removed=[dict(type=tag['topType'], id=tag['topId']) for tag in spans]))
    need(sum(len(change['removed']) for change in changes) == 89,
         'Expected exactly 89 unsupported Joypad declarations; input is not the original 7.1 fixture')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    for name in ('repo', 'exe', 'mod', 'work', 'output', 'reference'):
        parser.add_argument('--' + name, type=Path, required=True)
    parser.add_argument('--timeout-seconds', type=float, default=1800)
    args = parser.parse_args()
    need(os.name == 'nt', 'Real production compilation requires Windows')
    repo, exe, mod, work, out, reference_path = (getattr(args, name).resolve()
        for name in ('repo', 'exe', 'mod', 'work', 'output', 'reference'))
    need(exe.name == 'NewModCompiler.exe' and exe.is_file(), 'Expected the actual production NewModCompiler.exe')
    need((mod / 'Data/Mod.xml').is_file() and (mod / 'xsd/CnC3Types.xsd').is_file(), 'Original complete MOD inputs are absent')
    need(not work.is_relative_to(mod) and not out.is_relative_to(mod), 'Evidence/work directories must be outside the source MOD')
    work.mkdir(parents=True, exist_ok=False)
    out.mkdir(parents=True, exist_ok=False)
    sdk = repo / 'SDK'
    schema = sdk / 'Schemas/xsd'
    original_xml, changes = {}, []
    schema_backup = None
    before, schema_before = None, None
    reference = json.loads(reference_path.read_text(encoding='utf-8-sig'))
    state = dict(status='started', sourceCommit=subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD'], text=True).strip(),
        runId=os.environ.get('GITHUB_RUN_ID'), runAttempt=os.environ.get('GITHUB_RUN_ATTEMPT'),
        exe=str(exe), exeSha256=digest(exe.read_bytes()), modRoot=str(mod),
        reference=dict(runId=reference['runId'], artifactSha256=reference['artifactSha256'], scope=reference['scope'],
                       fileSha256=digest(reference_path.read_bytes())),
        scope='Original media; exactly 89 Joypad declarations temporarily excluded. All 1 base +13 APT +1 global loose streams; cold only, no BIG, no full base cross-input byte-equivalence claim.',
        sourceAdjustments=changes, restorationErrors=[])
    try:
        before, schema_before = inventory(mod), inventory(schema)
        write(out / 'source-inventory.json', before)
        state['sourceInventorySha256'] = digest((out / 'source-inventory.json').read_bytes())
        state['sourceFileCount'] = len(before)
        remove_joypad(mod, work / 'original-xml', changes, original_xml)
        write(out / 'source-adjustments.json', dict(removedAssetCount=89, xmlChanges=changes,
            rawMediaModified=False, tgaHeaderNormalizations=[], excludedTypes=['JoypadCommandBarButtonTemplate']))
        backup = work / 'original-schema'
        shutil.copytree(schema, backup)
        schema_backup = backup
        overlays = []
        for source in sorted(p for p in (mod / 'xsd').rglob('*') if p.is_file()):
            relative = source.relative_to(mod / 'xsd')
            destination = schema / relative
            data = source.read_bytes()
            if not destination.is_file() or destination.read_bytes() != data:
                overlays.append(dict(path=relative.as_posix(), sha256=digest(data)))
            destination.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, destination)
        state['schemaOverlay'] = overlays
        tasks = discover_tasks(mod, 'base,apt,global-metadata')
        need(collections.Counter(task['content'] for task in tasks) == {'base': 1, 'apt': 13, 'maps': 1},
             'Complete 1+13+1 stream discovery differs')
        state['streamIds'] = [task['id'] for task in tasks]
        roots = dict(dataRoot=str(sdk), dataPaths=[str(mod / 'Data'), str(sdk), str(sdk / 'Mods'), str(sdk / 'SAGEXML')],
                     artPaths=[str(mod / 'Art'), str(sdk / 'Art')], audioPaths=[str(mod / 'Audio'), str(sdk / 'Audio')])
        state['inputRoots'] = roots
        product = work / 'product'
        command = [str(exe), 'build', '--sdk-root', str(sdk), '--mod', str(mod), '--out', str(product),
            '--package-content', 'base,apt,global-metadata', '--package-format', 'loose', '--jobs', '4', '--performance', 'off',
            '/bc:true', '/sc:true', '/csc:true', '/psc:true', '/fsc:false', '/atc:false', '/ss:true', '/pc:true', '/slowclean:true',
            '/tp:Win32', '/ls:true', '/tl:3', '/el:0', '/iod:' + str(work / 'intermediate'),
            '/scd:' + str(work / 'session'), '/bcp:' + str(work / 'assets')] + root_arguments(roots)
        environment = os.environ.copy()
        for name in list(environment):
            if name.startswith('RA3_RECOVERED_') or name == 'RECOVERED_W3X_TRACE_DIRECTORY':
                del environment[name]
        environment['PATH'] = os.pathsep.join(p for p in environment.get('PATH', '').split(os.pathsep)
            if '/sdk/tools' not in p.replace('\\', '/').lower())
        state['process'] = measure(command, sdk, out / 'process', args.timeout_seconds, environment)
        report = json.loads((product / 'build-report.json').read_text(encoding='utf-8-sig'))
        need(report['status'] == 'succeeded' and report['exitCode'] == 0 and report['diagnosticCounts']['error'] == 0,
             'Production compiler did not complete with zero errors')
        validate_shared_report(report, dict(tasks=tasks, effectiveInputRoots=roots), sdk)
        state['partWallMs'] = stream_times(product, tasks)
        state['sharedOverheadMs'] = state['process']['wallMs'] - sum(state['partWallMs'].values())
        state['cache'] = report['cache']
        need(report['cache']['instanceCompile'] > 0 and report['cache']['sessionPackageHits'] == 0,
             'Expected a fresh cold compiler namespace')
        files, paths = full_inventory(product / 'published', tasks)
        write(out / 'output-inventory.json', files)
        state['outputFileCount'] = len(files)
        need(len(files) == 4058, 'Expected exactly 4058 output files with original media and only Joypad removed')
        fx = [name for name in files if name.startswith('base/') and name.endswith('/' + FX_NAME)]
        need(len(fx) == 1 and files[fx[0]] == dict(bytes=397648, sha256=FX_SHA256),
             'Original 8-channel FX_kanade_spell CDATA identity, length or raw hash differs')
        state['eightChannelAudioOutput'] = dict(path=fx[0], **files[fx[0]])
        actual = {key: value for key, value in files.items() if key.startswith(('apt/', 'maps/'))}
        expected = {key: value for key, value in reference['files'].items() if key.startswith(('apt/', 'maps/'))}
        need(expected and any(key.startswith('apt/') for key in expected) and any(key.startswith('maps/') for key in expected),
             'Original reference does not contain both complete APT/global inventories')
        differences = sorted(key for key in expected.keys() | actual.keys() if expected.get(key) != actual.get(key))
        state['referenceComparison'] = dict(comparedFileCount=len(actual), lengthAndSha256Equal=not differences,
                                            differentMembers=differences, baseStreamsCompared=False)
        need(not differences, 'Original-media APT/global raw output length/SHA256 differs')
        current = inventory(mod)
        changed_xml = {change['path'] for change in changes}
        state['originalMediaUnchanged'] = ({key: value for key, value in before.items() if key not in changed_xml} ==
                                           {key: value for key, value in current.items() if key not in changed_xml})
        need(state['originalMediaUnchanged'], 'Original media or unselected source files changed')
        state['status'] = 'passed'
    except Exception as error:
        state.update(status='failed', error=type(error).__name__ + ': ' + str(error), traceback=traceback.format_exc())
    finally:
        for name in ('build-report.json', 'build.events.jsonl', 'build.log'):
            generated = work / 'product' / name
            if generated.is_file():
                shutil.copy2(generated, out / name)
        for path, data in original_xml.items():
            try:
                path.write_bytes(data)
            except Exception as error:
                state['restorationErrors'].append(str(path) + ': ' + str(error))
        if schema_backup is not None:
            try:
                shutil.rmtree(schema)
                shutil.copytree(schema_backup, schema)
            except Exception as error:
                state['restorationErrors'].append('SDK Schema: ' + str(error))
        try:
            state['modRestored'] = before is not None and inventory(mod) == before
            state['schemaRestored'] = schema_before is not None and inventory(schema) == schema_before
            if not state['modRestored'] or not state['schemaRestored']:
                state['restorationErrors'].append('Original MOD or Schema inventory was not restored')
        except Exception as error:
            state['restorationErrors'].append('Restoration verification: ' + str(error))
        if state['restorationErrors']:
            state['status'] = 'failed'
        write(out / 'validation.json', state)
    print(json.dumps({key: state.get(key) for key in ('status', 'outputFileCount', 'referenceComparison', 'error', 'modRestored', 'schemaRestored')}, ensure_ascii=False))
    return 0 if state['status'] == 'passed' else 1


if __name__ == '__main__':
    raise SystemExit(main())
