#!/usr/bin/env python3
"""Prepare a small XSD-valid CLI business MOD; never compile or invoke SDK tools.

APT coverage is four file-backed DataBlob roots, not APT file-format parsing or
an executable UI. MapMetaData uses the existing source-XSD-valid fixture nodes.
"""
import argparse
import copy
import hashlib
import json
from pathlib import Path
import runpy

from lxml import etree as E

ROOT = Path(__file__).resolve().parents[2]
NS = 'uri:ea.com:eala:asset'
A = '{' + NS + '}'


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('destination', type=Path)
    args = parser.parse_args()
    destination = args.destination.resolve()
    if destination.exists():
        raise ValueError('Refusing to overwrite business fixture: ' + str(destination))
    apt_source = ROOT / 'SDK/Mods/XmlCompilerRecoveryApt/Data'
    map_source = ROOT / 'SDK/Mods/XmlCompilerRecoveryA/Data/FixturesA.xml'
    base_source = ROOT / 'SDK/Mods/mymod/Data/RecoveryArmorProbe.xml'
    apt = E.parse(str(apt_source / 'FixturesApt.xml'))
    maps = E.Element(A + 'AssetDeclaration', nsmap={None: NS})
    for node in E.parse(str(map_source)).getroot().findall(A + 'MapMetaData'):
        maps.append(copy.deepcopy(node))
    if len(apt.getroot()) != 12 or len(maps) != 3:
        raise ValueError('Expected 12 APT DataBlob assets and three MapMetaData fixtures')
    schema = runpy.run_path(str(ROOT / 'recovery/scripts/GenerateXmlSdkModA.py'),
                           run_name='cli_business_schema')['aggregate_schema']()
    documents = {'Data/Mod.xml': E.parse(str(base_source)),
                 'Data/AptUI/FileBacked.xml': apt,
                 'Data/AdditionalMaps/MapMetaData_Business.xml': E.ElementTree(maps)}
    for name, document in documents.items():
        if not schema.validate(document):
            raise ValueError(name + ': ' + str(schema.error_log))
    inputs = [Path(__file__).resolve(), ROOT / 'recovery/scripts/GenerateXmlSdkModA.py',
              base_source, map_source, apt_source / 'FixturesApt.xml']
    payloads = {}
    for node in apt.getroot():
        name = node.get('File')
        if not name or Path(name).name != name:
            raise ValueError('APT proof requires a declaring-XML-relative payload basename')
        payloads[name] = (apt_source / name).read_bytes()
    for name, document in documents.items():
        path = destination / name
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(E.tostring(document, encoding='utf-8', xml_declaration=True, pretty_print=True))
    for name, data in payloads.items():
        path = destination / 'Data/AptUI' / name
        path.write_bytes(data)
        inputs.append(apt_source / name)
    receipt = {'schemaVersion': 1, 'prepared': True, 'executed': False,
               'scope': 'Armor base, 12 file-backed APT assets and three MapMetaData assets; no APT format parsing, full SDK, publication or G3 acceptance',
               'expectedTasks': ['base/Mod', 'apt/FileBacked', 'maps/MapMetaData_Business'],
               'sourceInputs': [{'path': p.relative_to(ROOT).as_posix(), 'sha256': sha(p)} for p in inputs],
               'files': [{'path': p.relative_to(destination).as_posix(), 'sha256': sha(p)}
                         for p in sorted(destination.rglob('*')) if p.is_file()],
               'xsdValidated': True, 'lxmlVersion': '.'.join(map(str, E.LXML_VERSION)), 'assetImports': False,
               'payloadResolution': 'Each File basename remains beside its declaring AptUI XML'}
    (destination / 'fixture-manifest.json').write_text(json.dumps(receipt, indent=2) + '\n')
    print(json.dumps({'prepared': True, 'executed': False, 'tasks': len(documents), 'aptAssets': 12, 'mapAssets': 3}))


if __name__ == '__main__':
    main()
