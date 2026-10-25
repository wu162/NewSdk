#!/usr/bin/env python3
"""Static W3X MOD validation using unchanged reachable SDK XSD definitions.

This is a portable focused check. Windows TestW3xSdk must still compile the
complete unmodified CnC3Types.xsd with XmlSchemaSet before invoking BAB.
"""
from copy import deepcopy
from pathlib import Path
import hashlib
import json
from lxml import etree as E

ROOT = Path(__file__).resolve().parents[2]
SDK = ROOT / 'SDK'
MOD = SDK / 'Mods/W3xRecovery'
XS = 'http://www.w3.org/2001/XMLSchema'
NS = 'uri:ea.com:eala:asset'
X = '{' + XS + '}'
ASSETS = {'W3DAnimation', 'W3DMesh', 'W3DHierarchy', 'Texture'}


def schema():
    definitions = {}
    source = {}
    for path in sorted((SDK / 'Schemas/xsd').rglob('*.xsd')):
        doc = E.parse(str(path))
        if doc.getroot().get('targetNamespace') != NS:
            continue
        for node in doc.getroot():
            if not isinstance(node.tag, str) or not node.get('name'):
                continue
            if node.tag not in {X + t for t in ('complexType', 'simpleType', 'element', 'attribute', 'group', 'attributeGroup')}:
                continue
            key = (E.QName(node).localname, node.get('name'))
            definitions.setdefault(key, node)
            source.setdefault(key, str(path.relative_to(ROOT)))
    original = E.parse(str(SDK / 'Schemas/xsd/CnC3Types.xsd'))
    declaration = deepcopy(original.find(X + "element[@name='AssetDeclaration']"))
    # Keep the exact AssetDeclaration sequence/Includes/Tags/Defines and the
    # exact four element declarations present in this MOD. Remove unrelated
    # asset branches so their unrelated, non-portable schemas are not loaded.
    for choice in declaration.findall('.//' + X + 'choice'):
        for node in list(choice):
            if node.tag == X + 'element' and node.get('name') not in ASSETS:
                choice.remove(node)
    document = E.Element(X + 'schema', nsmap=original.getroot().nsmap,
                         targetNamespace=NS, elementFormDefault='qualified')
    pending = [declaration]
    included = set()
    used = []
    while pending:
        current = pending.pop()
        document.append(current)
        for node in current.iter():
            if not isinstance(node.tag, str) or not node.tag.startswith(X):
                continue
            for attr in ('type', 'base', 'itemType', 'memberTypes', 'ref'):
                for name in node.get(attr, '').split():
                    prefix, local = name.split(':', 1) if ':' in name else ('', name)
                    namespace = node.nsmap.get(prefix or None, NS)
                    if namespace != NS:
                        continue
                    kind = E.QName(node).localname if attr == 'ref' else 'complexType'
                    key = (kind, local)
                    if attr != 'ref' and key not in definitions:
                        key = ('simpleType', local)
                    if key in included:
                        continue
                    if key not in definitions:
                        raise RuntimeError('Missing original SDK XSD definition: ' + str(key))
                    included.add(key)
                    pending.append(deepcopy(definitions[key]))
                    used.append({'kind': key[0], 'name': key[1], 'source': source[key]})
    return E.XMLSchema(document), used


def main():
    compiled, used = schema()
    validated = []
    for path in sorted((MOD / 'Data').glob('*.xml')):
        doc = E.parse(str(path))
        compiled.assertValid(doc)
        validated.append({'path': str(path.relative_to(MOD)),
                          'sha256': hashlib.sha256(path.read_bytes()).hexdigest()})
    # Ensure the schema still rejects malformed real type structures. These
    # mutations are in memory only and never become SDK input files.
    meshes = E.parse(str(MOD / 'Data/Meshes.xml'))
    meshes.find('.//{' + NS + '}V').attrib.pop('X')
    if compiled.validate(meshes):
        raise RuntimeError('Schema unexpectedly accepted a position without X')
    animations = E.parse(str(MOD / 'Data/Animations.xml'))
    animations.find('.//{' + NS + '}ChannelQuaternion/{' + NS + '}Frame').attrib.pop('W')
    if compiled.validate(animations):
        raise RuntimeError('Schema unexpectedly accepted quaternion frame without W')
    report = {'scope': 'unchanged reachable SDK definitions; unrelated root choice branches omitted',
              'fullWindowsCnC3TypesValidation': 'required before SDK execution',
              'documents': validated, 'definitions': used, 'negativeControls': 2,
              'sdkExecuted': False}
    (MOD / 'static-xsd-validation.json').write_text(json.dumps(report, indent=2) + '\n')
    coverage = json.loads((MOD / 'coverage.json').read_text())
    coverage['validation']['staticXsd'] = 'PASS reachable original SDK type definitions; 4 XML documents; 2 negative controls'
    (MOD / 'coverage.json').write_text(json.dumps(coverage, indent=2) + '\n')
    print('PASS W3xRecovery focused original XSD: %d XML documents, %d type definitions, 2 negative controls; SDK not run' % (len(validated), len(used)))


if __name__ == '__main__':
    main()
