#!/usr/bin/env python3
"""Prepare the user's supported-input benchmark variant without reserializing XML."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

from UpheavalEditWorkload import xml_tags

UNSUPPORTED_INSTANCES = {
    ('AudioFile', 'FX_kanade_spell'): 'Source WAVE extensible input has 8 channels; the accepted reader supports 1/2/4/6.',
    ('AudioEvent', 'FX_kanade_spell'): 'Unused event depends exclusively on the removed 8-channel AudioFile.',
}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def prepare(mod, catalog, backup, output):
    excluded = {row['name'] for row in json.loads(catalog.read_text(encoding='utf-8'))['excluded']}
    previous = json.loads(output.read_text(encoding='utf-8')) if output.exists() else {}
    if previous and previous['modRoot'] != str(mod):
        raise ValueError('Adjustment receipt belongs to a different MOD directory')
    removed = previous.get('removedAssets', [])
    xml_changes = previous.get('xmlChanges', [])
    tga_changes = previous.get('tgaHeaderNormalizations', [])
    candidate_types = excluded | {kind for kind, name in UNSUPPORTED_INSTANCES}
    for path in sorted((mod / 'Data').rglob('*.xml')):
        relative = path.relative_to(mod)
        if relative.parts[1].lower() in ('aptui', 'additionalmaps'):
            continue
        data = path.read_bytes()
        if not any(name.encode('ascii') in data for name in candidate_types):
            continue
        spans = [tag for tag in xml_tags(data)
                 if 'elementEnd' in tag and tag['topId'] and
                 (tag['topType'] in excluded or (tag['topType'], tag['topId']) in UNSUPPORTED_INSTANCES)]
        if not spans:
            continue
        original = backup / relative
        original.parent.mkdir(parents=True, exist_ok=True)
        if not original.exists():
            shutil.copy2(path, original)
        changed = data
        for tag in reversed(spans):
            changed = changed[:tag['start']] + changed[tag['elementEnd']:]
        path.write_bytes(changed)
        xml_changes.append(dict(path=relative.as_posix(), originalSha256=sha(data),
                                modifiedSha256=sha(changed), backup=str(original)))
        removed.extend(dict(path=relative.as_posix(), type=tag['topType'], id=tag['topId'],
            reason=UNSUPPORTED_INSTANCES.get((tag['topType'], tag['topId']), 'Excluded built-in XML compiler type')) for tag in spans)
    for path in sorted((mod / 'Data/AptUI').rglob('*')):
        if path.suffix.lower() != '.tga':
            continue
        with path.open('rb') as stream:
            header = stream.read(18)
        if len(header) < 18 or header[1] != 0 or header[2] not in (2, 10) or not any(header[3:8]):
            continue
        data = path.read_bytes()
        changed = data[:3] + bytes(5) + data[8:]
        if data[8:] != changed[8:] or data[:3] != changed[:3]:
            raise ValueError('TGA normalization touched bytes outside the unused color-map specification')
        path.write_bytes(changed)
        tga_changes.append(dict(path=path.relative_to(mod).as_posix(),
            originalHeader=header.hex(), modifiedHeader=changed[:18].hex(),
            originalSha256=sha(data), modifiedSha256=sha(changed),
            unchangedPixelPayloadSha256=sha(data[18 + header[0]:])))
    result = dict(schemaVersion=1, modRoot=str(mod),
        scope='User-authorized base unsupported declarations removed. All base/APT/global entries remain selected. Unused TGA color-map specification bytes normalized for both engines; pixel/encoded payload bytes unchanged.',
        excludedTypes=sorted(excluded), removedAssets=removed, xmlChanges=xml_changes,
        tgaHeaderNormalizations=tga_changes, removedAssetCount=len(removed),
        normalizedTgaCount=len(tga_changes), originalArchivePreserved=True)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({key: result[key] for key in ('removedAssetCount', 'normalizedTgaCount', 'scope')}, ensure_ascii=False))
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--mod', type=Path, required=True)
    parser.add_argument('--catalog', type=Path, default=Path(__file__).resolve().parents[1] / 'catalog/XmlSdkTypes.json')
    parser.add_argument('--backup', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    prepare(args.mod.resolve(), args.catalog.resolve(), args.backup.resolve(), args.output.resolve())


if __name__ == '__main__':
    main()
