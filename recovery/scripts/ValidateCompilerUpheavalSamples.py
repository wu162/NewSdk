#!/usr/bin/env python3
"""Cheap Actions preflight for the bounded Upheaval differential fixture.

This verifies source bytes and the closed XML input set, without compiling or
claiming that raw inheritance/Define XML has already passed Core validation.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import xml.etree.ElementTree as E

ROOT = Path(__file__).resolve().parents[2]
SOURCE = '068643219fded09a05fb7b75c6507b769c430316'
FIXTURE = ROOT / 'recovery/tests/fixtures/UpheavalSmall'
A = '{uri:ea.com:eala:asset}'
ORIGINALS = {
    'Data/ArcherMaidenArmor.xml': 'SDK/Mods/Upheaval/Data/Mod/Units/Archer Maiden/Armor.xml',
    'Data/FutureTankArmor.xml': 'SDK/Mods/Upheaval/Data/Mod/Units/Future Tank X-1/Armor.xml',
    'Data/GrinderArmor.xml': 'SDK/Mods/Upheaval/Data/Mod/Units/Grinder/Armor.xml',
    'Data/GigaFortressUpgrades.xml': 'SDK/Mods/Upheaval/Data/Mod/Units/Giga-Fortress/UpgradeTemplate.xml',
}
ADAPTERS = {'Data/Mod.xml', 'Data/Defines.xml', 'Data/Derived.xml'}


def require(value, message):
    if not value:
        raise ValueError(message)


def validate():
    provenance = json.loads((FIXTURE / 'provenance.json').read_text(encoding='utf-8'))
    require(provenance['sourceCommit'] == SOURCE, 'Original source commit changed')
    originals = provenance['originals']
    require(len(originals) == 4 and {r['path']: r['sourcePath'] for r in originals} == ORIGINALS,
            'Original path inventory changed')
    inventory = {p.relative_to(FIXTURE).as_posix(): p.read_bytes() for p in FIXTURE.rglob('*') if p.is_file()}
    require(set(inventory) == set(ORIGINALS) | ADAPTERS | {'README.md', '.gitattributes', 'provenance.json'},
            'Unexpected fixture inputs; keep the small group closed')
    require(sum(map(len, inventory.values())) <= 16 * 1024, 'Small fixture exceeds its byte budget')
    for row in originals:
        content = inventory[row['path']]
        require(len(content) == row['bytes'] and hashlib.sha256(content).hexdigest() == row['sha256'],
                'Original file size/SHA256 mismatch: ' + row['path'])
        blob = hashlib.sha1(b'blob ' + str(len(content)).encode('ascii') + b'\0' + content).hexdigest()
        require(blob == row['blob'], 'Original Git blob mismatch: ' + row['path'])
        original = subprocess.run(['git', '-C', str(ROOT), 'show', SOURCE + ':' + row['sourcePath']],
                                  check=True, capture_output=True).stdout
        require(content == original, 'Whole original source bytes differ: ' + row['path'])
    assets, definitions, includes, expressions = {}, {}, [], []
    for name in sorted(set(ORIGINALS) | ADAPTERS):
        content = inventory[name]
        if name in ADAPTERS:
            require(b'synthetic' in content.lower(), 'Adapter must identify synthetic input: ' + name)
        document = E.fromstring(content)
        require(document.tag == A + 'AssetDeclaration', 'Unexpected XML root: ' + name)
        for node in document:
            if node.tag == A + 'Includes':
                require(name in ADAPTERS, 'Original unexpectedly needs external includes')
                for item in node:
                    source = item.attrib['source']
                    require(item.tag == A + 'Include' and item.attrib['type'] in ('all', 'instance') and
                            source == Path(source).name and source not in ('.', '..') and ':' not in source,
                            'Include escapes the closed small group: ' + source)
                    target = 'Data/' + source
                    require(target in inventory and target.endswith('.xml'), 'Missing included XML: ' + source)
                    includes.append({'source': name, 'target': target})
            elif node.tag == A + 'Defines':
                require(name in ADAPTERS, 'Unexpected original Define adapter')
                for item in node:
                    key = item.attrib['name']
                    require(item.tag == A + 'Define' and key not in definitions, 'Duplicate/invalid Define')
                    definitions[key] = item.attrib['value']
            else:
                require(node.tag in (A + 'ArmorTemplate', A + 'UpgradeTemplate'), 'Unexpected asset type')
                key = node.attrib['id']
                require(key not in assets, 'Duplicate small-group asset: ' + key)
                assets[key] = {'type': node.tag[len(A):], 'base': node.get('inheritFrom'), 'source': name}
            expressions.extend(value for item in node.iter() for value in item.attrib.values() if value.startswith('='))
    require(len(assets) == 7 and sum(v['type'] == 'ArmorTemplate' for v in assets.values()) == 5,
            'Expected five Armor and two Upgrade assets')
    require(sum(bool(v['base']) for v in assets.values()) == 2 and len(definitions) == 1 and len(expressions) == 5,
            'Expected two inherited adapters, one Define and five expression reads')
    for key, value in assets.items():
        seen = {key}
        while value['base']:
            base = value['base']
            require(base in assets and base not in seen, 'Missing/cyclic inheritance base: ' + base)
            seen.add(base)
            require(assets[base]['type'] == value['type'], 'Inheritance type changed')
            value = assets[base]
    candidate = subprocess.run(['git', '-C', str(ROOT), 'rev-parse', 'HEAD'],
                               check=True, capture_output=True, text=True).stdout.strip()
    return {'succeeded': True, 'compiled': False, 'sourceCommit': candidate, 'originalSourceCommit': SOURCE,
            'runId': os.environ.get('GITHUB_RUN_ID'), 'runAttempt': os.environ.get('GITHUB_RUN_ATTEMPT'), 'assets': assets,
            'includes': includes, 'definitions': definitions, 'expressionReads': len(expressions),
            'inputBytes': sum(map(len, inventory.values())),
            'inputs': [{'path': n, 'sha256': hashlib.sha256(b).hexdigest(), 'bytes': len(b)} for n, b in sorted(inventory.items())],
            'scope': 'Original-byte and closed-input preflight only; Core/XSD, output and original BAB gates remain required'}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    receipt = validate()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(receipt, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'succeeded': True, 'compiled': False, 'assets': len(receipt['assets']), 'inputBytes': receipt['inputBytes']}))


if __name__ == '__main__':
    main()
