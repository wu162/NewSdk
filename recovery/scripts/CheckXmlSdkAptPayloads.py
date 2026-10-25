#!/usr/bin/env python3
"""Prove file-backed APT SDK payloads from retained bytes; never compile or replay.

calls.tsv columns are root type, asset id, unique trace stem, platform integer.
Run on a retained XmlCompilerRecoveryApt profile directory after its successful SDK
four-output/original-replay/isolated-replay evidence has already been preserved.
"""
import argparse
import hashlib
import json
import ntpath
from pathlib import Path
import re
import struct
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
APT_TYPES = ('AptAptData', 'AptConstData', 'AptDatData', 'AptGeometryData')
EXPECTED_FILES = {'defaults': ('fixture-empty.bin', 0), 'rich': ('fixture.bin', 46), 'repeat': ('fixture-odd.bin', 13)}
ASSET_NS = 'uri:ea.com:eala:asset'


def require(condition, message):
    if not condition:
        raise ValueError(message)


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def read_calls(trace):
    rows, stems = {}, set()
    for number, line in enumerate((trace / 'calls.tsv').read_text(encoding='utf-8-sig').splitlines(), 1):
        fields = line.split('\t')
        require(len(fields) == 4, f'calls.tsv:{number}: expected exactly four columns')
        type_name, asset_id, stem, platform = fields
        require(type_name and asset_id, f'calls.tsv:{number}: empty asset identity')
        require(re.fullmatch(r'[A-Za-z0-9_-]+', stem) is not None,
                f'calls.tsv:{number}: invalid trace stem')
        require(stem not in stems, f'calls.tsv:{number}: duplicate trace stem')
        stems.add(stem)
        require(re.fullmatch(r'-?\d+', platform) is not None,
                f'calls.tsv:{number}: invalid platform')
        key = (type_name, asset_id)
        require(key not in rows, f'calls.tsv:{number}: duplicate asset identity')
        rows[key] = (stem, int(platform))
    require(rows, 'Empty SDK call manifest')
    return rows


def check(profile, mod_source, asset_prefix='XmlRecoveryApt'):
    profile, mod_source = Path(profile), Path(mod_source)
    trace = profile / 'trace'
    rows = read_calls(trace)
    final_bin = (profile / 'Recovered-mod.bin').read_bytes()
    require(final_bin, 'Recovered-mod.bin is empty')
    output = {'succeeded': False, 'scope': 'Retained APT file-byte proof only; no compilation or replay',
              'profile': profile.name, 'platform': 0,
              'callsManifestSha256': sha256((trace / 'calls.tsv').read_bytes()),
              'modBinSha256': sha256(final_bin), 'checks': []}
    for type_name in APT_TYPES:
        root_size = 16 if type_name == 'AptGeometryData' else 12
        for variant, (filename, expected_length) in EXPECTED_FILES.items():
            asset_id = f'{asset_prefix}_{type_name}_{variant}'
            key = (type_name, asset_id)
            require(key in rows, f'Missing real SDK call: {type_name}/{asset_id}')
            stem, platform = rows[key]
            require(platform == 0, f'{asset_id}: expected the actual Win32 SDK platform0 trace')
            expected = (mod_source / 'Data' / filename).read_bytes()
            require(len(expected) == expected_length,
                    f'{asset_id}: source fixture changed; expected {expected_length} bytes')
            xml_bytes = (trace / (stem + '.xml')).read_bytes()
            node = ET.fromstring(xml_bytes)
            require(node.tag == '{' + ASSET_NS + '}' + type_name and node.get('id') == asset_id,
                    f'{asset_id}: trace XML identity mismatch')
            file_value = node.get('File')
            require(file_value is not None and file_value.replace('\\', '/').split('/')[-1] == filename,
                    f'{asset_id}: trace XML File does not identify {filename}')
            require(ntpath.isabs(file_value), f'{asset_id}: BAB did not normalize File to an absolute path')
            instance = (trace / (stem + '.bin')).read_bytes()
            require(len(instance) >= root_size, f'{asset_id}: truncated APT root')
            reserved, pointer, length = struct.unpack_from('<III', instance)
            require(reserved == 0, f'{asset_id}: unexpected reserved root word')
            require(pointer == root_size, f'{asset_id}: file payload must follow the complete root')
            require(length == expected_length, f'{asset_id}: payload count does not match actual source file')
            padded = (expected_length + 3) & ~3
            require(len(instance) == root_size + padded, f'{asset_id}: unexpected root/payload allocation size')
            actual = instance[pointer:pointer + length]
            require(actual == expected, f'{asset_id}: payload bytes differ from source fixture')
            require(instance[pointer + length:] == b'\0' * (padded - length),
                    f'{asset_id}: nonzero file-payload padding')
            relocation = (trace / (stem + '.relo')).read_bytes()
            require(relocation == struct.pack('<II', 4, 0xffffffff),
                    f'{asset_id}: expected the sole blob pointer relocation at root+4')
            require((trace / (stem + '.imp')).read_bytes() == b'',
                    f'{asset_id}: raw file blob unexpectedly imports an asset')
            position = final_bin.find(instance)
            require(position >= 0, f'{asset_id}: complete traced instance absent from final SDK mod.bin')
            output['checks'].append({'type': type_name, 'id': asset_id, 'variant': variant, 'stem': stem,
                                     'normalizedAbsoluteFile': file_value,
                                     'file': 'Data/' + filename, 'payloadBytes': length,
                                     'sourceSha256': sha256(expected), 'payloadSha256': sha256(actual),
                                     'traceInstanceSha256': sha256(instance), 'traceXmlSha256': sha256(xml_bytes),
                                     'rootBytes': root_size, 'payloadOffset': pointer,
                                     'instanceBytes': len(instance), 'paddingBytes': padded - length,
                                     'finalModInstanceOffset': position})
    require(len(output['checks']) == 12, 'Incomplete four-types/three-files proof')
    output['checkedCalls'] = len(output['checks'])
    output['nonemptyFileCalls'] = sum(row['payloadBytes'] > 0 for row in output['checks'])
    output['emptyFileCalls'] = sum(row['payloadBytes'] == 0 for row in output['checks'])
    output['types'] = list(APT_TYPES)
    output['payloadBytesCompared'] = sum(row['payloadBytes'] for row in output['checks'])
    output['succeeded'] = True
    return output


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--profile', type=Path, required=True,
                        help='Retained profile folder containing trace/ and Recovered-mod.bin')
    parser.add_argument('--mod-source', type=Path, default=ROOT / 'SDK/Mods/XmlCompilerRecoveryApt')
    parser.add_argument('--asset-prefix', default='XmlRecoveryApt', help='Asset id prefix; old broad Mod A can be checked with XmlRecoveryA')
    parser.add_argument('--output', type=Path, help='Optional byte-proof JSON; existing evidence files are not modified')
    args = parser.parse_args()
    try:
        result = check(args.profile, args.mod_source, args.asset_prefix)
    except (OSError, ValueError, ET.ParseError, struct.error) as error:
        result = {'succeeded': False, 'scope': 'Retained APT file-byte proof only; no compilation or replay',
                  'error': str(error)}
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(result, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    print(json.dumps({key: value for key, value in result.items() if key != 'checks'}, ensure_ascii=False))
    return 0 if result['succeeded'] else 1


if __name__ == '__main__':
    sys.exit(main())
