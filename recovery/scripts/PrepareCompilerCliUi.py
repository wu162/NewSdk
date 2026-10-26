#!/usr/bin/env python3
"""Export original Upheaval APT UI inputs from a pinned ancestor; never compile.

Every exported file preserves its Git blob bytes. The command-list-only subset
has no textures/custom-data outputs; the other two entries include TGA textures.
"""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import posixpath
import runpy
import subprocess
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
SOURCE = '068643219fded09a05fb7b75c6507b769c430316'
PREFIX = 'SDK/Mods/Upheaval/Data/AptUI/'
ENTRIES = ('TacticalHUDBuildQueuePage', 'TacticalHUDCommandListBox', 'TacticalHUDMainCommandBar')


def git(*args):
    return subprocess.check_output(['git', '-C', str(ROOT), *args])


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('destination', type=Path)
    parser.add_argument('--entry', choices=('all', *ENTRIES), default='all')
    args = parser.parse_args()
    destination = args.destination.resolve()
    if destination.exists():
        raise ValueError('Refusing to overwrite UI fixture: ' + str(destination))
    subprocess.run(['git', '-C', str(ROOT), 'merge-base', '--is-ancestor', SOURCE, 'HEAD'], check=True)
    files = {}
    for row in git('ls-tree', '-r', '-z', SOURCE, '--', PREFIX).split(b'\0'):
        if not row:
            continue
        metadata, raw_path = row.split(b'\t', 1)
        mode, kind, blob = metadata.decode('ascii').split()
        path = raw_path.decode('utf-8')
        relative = path[len(PREFIX):]
        if mode != '100644' or kind != 'blob' or '..' in PurePosixPath(relative).parts:
            raise ValueError('UI input must be an ordinary contained file: ' + path)
        if args.entry != 'all' and relative != args.entry + '.xml' and not relative.startswith(args.entry + '/'):
            continue
        data = git('cat-file', 'blob', blob)
        actual_blob = hashlib.sha1(b'blob ' + str(len(data)).encode() + b'\0' + data).hexdigest()
        if actual_blob != blob or relative.lower() in files:
            raise ValueError('UI blob identity/case collision: ' + path)
        files[relative.lower()] = {'path': relative, 'sourcePath': path, 'blob': blob,
                                    'sha256': hashlib.sha256(data).hexdigest(), 'data': data}
    expected = 106 if args.entry == 'all' else (5 if args.entry == ENTRIES[1] else None)
    if not files or (expected is not None and len(files) != expected):
        raise ValueError('Pinned UI closure file count differs')
    dependencies, roots, textures = [], {}, []
    for key, item in files.items():
        if not key.endswith('.xml'):
            continue
        document = ET.fromstring(item['data'])
        counts = {}
        for node in document:
            name = node.tag.split('}')[-1]
            if name != 'Includes':
                counts[name] = counts.get(name, 0) + 1
            if name == 'Texture':
                textures.append({'entry': item['path'].split('/')[0], 'qualifiedName': 'Texture:' + node.attrib['id'], 'declaration': item['path']})
        roots[item['path']] = counts
        for node in document.iter():
            attribute = 'source' if node.tag.split('}')[-1] == 'Include' else 'File'
            target = node.get(attribute)
            if target is None:
                continue
            if ':' in target or target.startswith(('/', '\\')):
                raise ValueError('UI dependency is not declaring-XML-relative: ' + target)
            resolved = posixpath.normpath(posixpath.join(posixpath.dirname(item['path']), target.replace('\\', '/')))
            dependency = files.get(resolved.lower())
            if dependency is None:
                raise ValueError('UI dependency missing from pinned closure: ' + resolved)
            dependencies.append({'declaration': item['path'], 'attribute': attribute, 'literal': target,
                                 'target': dependency['path'], 'blob': dependency['blob']})
    # The same original schema graph used by existing SDK fixture preparation.
    from lxml import etree as E
    schema = runpy.run_path(str(ROOT / 'recovery/scripts/GenerateXmlSdkModA.py'),
                           run_name='cli_ui_schema')['aggregate_schema']()
    for item in files.values():
        if item['path'].lower().endswith('.xml'):
            if not schema.validate(E.fromstring(item['data'])):
                raise ValueError(item['path'] + ': ' + str(schema.error_log))
    for item in files.values():
        path = destination / 'Data/AptUI' / item['path']
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(item['data'])
    base_source = ROOT / 'SDK/Mods/mymod/Data/RecoveryArmorProbe.xml'
    if not schema.validate(E.fromstring(base_source.read_bytes())):
        raise ValueError('Support Armor base is not valid original XSD: ' + str(schema.error_log))
    base_target = destination / 'Data/Mod.xml'
    base_target.write_bytes(base_source.read_bytes())
    source_inputs = [Path(__file__).resolve(), ROOT / 'recovery/scripts/GenerateXmlSdkModA.py', base_source]
    receipt = {'schemaVersion': 1, 'prepared': True, 'compiled': False, 'xsdValidated': True,
               'supportFiles': [{'path': 'Data/Mod.xml', 'sourcePath': base_source.relative_to(ROOT).as_posix(), 'sha256': hashlib.sha256(base_target.read_bytes()).hexdigest()}],
               'sourceInputs': [{'path': p.relative_to(ROOT).as_posix(), 'sha256': hashlib.sha256(p.read_bytes()).hexdigest()} for p in source_inputs],
               'sourceCommit': SOURCE, 'sourceIsAncestor': True, 'entry': args.entry,
               'scope': 'Original APT/CONST/DAT/geometry/texture bytes and wrappers only; no compiler execution, output comparison, runtime UI or G3 acceptance',
               'files': [{k: v for k, v in item.items() if k != 'data'} for item in files.values()],
               'dependencies': dependencies, 'assetRoots': roots, 'textures': textures,
               'textureInputs': sum(name.endswith('.tga') for name in files),
               'expectedCustomDataBoundary': 'CommandListBox has no Texture; QueuePage/MainCommandBar each have one Texture and may emit cdata in addition to the four stream files'}
    (destination / 'fixture-manifest.json').write_text(json.dumps(receipt, indent=2) + '\n')
    print(json.dumps({'prepared': True, 'compiled': False, 'files': len(files),
                      'dependencies': len(dependencies), 'xsdValidated': True}))


if __name__ == '__main__':
    main()
