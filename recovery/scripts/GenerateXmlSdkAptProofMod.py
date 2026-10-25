#!/usr/bin/env python3
"""Create a targeted, source-XSD-valid APT DataBlob SDK proof MOD; no compilation.

BAB resolves DataBlob/File relative to the declaring XML file. These basename
literals deliberately avoid the duplicated Mods/.../Data prefix seen in the
first broad Mod A SDK run. Existing Mod A assets/evidence remain untouched.
"""
import hashlib
import json
from pathlib import Path
import runpy
from lxml import etree as E

ROOT = Path(__file__).resolve().parents[2]
NAME = 'XmlCompilerRecoveryApt'
MOD = ROOT / 'SDK/Mods' / NAME
DATA = MOD / 'Data'
NS = 'uri:ea.com:eala:asset'
A = '{' + NS + '}'
TYPES = ('AptAptData', 'AptConstData', 'AptDatData', 'AptGeometryData')
PAYLOADS = {'defaults': ('fixture-empty.bin', b''),
            'rich': ('fixture.bin', bytes(range(32)) + b'XMLRECOVERY-A\0'),
            'repeat': ('fixture-odd.bin', b'padding-check')}


def main():
    DATA.mkdir(parents=True, exist_ok=True)
    document = E.Element(A + 'AssetDeclaration', nsmap={None: NS})
    manifest = {'mod': NAME, 'scope': 'Targeted true file-backed APT SDK proof; not custom APT file-format recovery',
                'assetCount': 12, 'globalRoots': list(TYPES), 'originalModAChanged': False,
                'fileResolution': 'File basenames relative to Data/FixturesApt.xml; BAB supplies absolute path',
                'expectedSdkImports': 'No asset imports; final mod.imp sentinel only', 'cases': []}
    for variant, (filename, payload) in PAYLOADS.items():
        (DATA / filename).write_bytes(payload)
    for type_name in TYPES:
        for variant, (filename, payload) in PAYLOADS.items():
            asset_id = f'XmlRecoveryApt_{type_name}_{variant}'
            node = E.SubElement(document, A + type_name, id=asset_id, File=filename)
            if type_name == 'AptGeometryData':
                node.set('AptID', {'defaults': '0', 'rich': '17', 'repeat': '27'}[variant])
            manifest['cases'].append({'type': type_name, 'id': asset_id, 'variant': variant,
                                      'file': 'Data/' + filename, 'sourceLiteral': filename,
                                      'expectedFileBytes': len(payload),
                                      'sourceSha256': hashlib.sha256(payload).hexdigest()})
    # Reuse the existing read-only case-normalized source-schema graph. It does not
    # call Mod A's generator or change any source schema or existing fixture file.
    schema_support = runpy.run_path(str(ROOT / 'recovery/scripts/GenerateXmlSdkModA.py'),
                                   run_name='apt_proof_schema_support')
    schema = schema_support['aggregate_schema']()
    if not schema.validate(document):
        raise ValueError('APT proof MOD is not valid original XSD: ' + str(schema.error_log))
    manifest['xsdValidated'] = True
    (DATA / 'FixturesApt.xml').write_bytes(E.tostring(document, pretty_print=True, xml_declaration=True, encoding='utf-8'))
    mod = E.Element(A + 'AssetDeclaration', nsmap={None: NS})
    includes = E.SubElement(mod, A + 'Includes')
    for source in ('DATA:static.xml', 'DATA:global.xml', 'DATA:audio.xml'):
        E.SubElement(includes, A + 'Include', type='reference', source=source)
    E.SubElement(includes, A + 'Include', type='all', source='FixturesApt.xml')
    if not schema.validate(mod):
        raise ValueError('APT proof Mod.xml is not valid original XSD: ' + str(schema.error_log))
    (DATA / 'Mod.xml').write_bytes(E.tostring(mod, pretty_print=True, xml_declaration=True, encoding='utf-8'))
    manifest['files'] = ['Data/Mod.xml', 'Data/FixturesApt.xml'] + ['Data/' + v[0] for v in PAYLOADS.values()]
    (MOD / 'fixture-manifest.json').write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'mod': NAME, 'assets': len(document), 'xsdValidated': True,
                      'fileByteLengths': [len(v[1]) for v in PAYLOADS.values()]}))


if __name__ == '__main__':
    main()
