"""Join original source records to observed original buffers on every target."""
import csv
import json
import struct
import sys
from pathlib import Path


def verify(report_file, details):
    report = json.loads(report_file.read_text(encoding='utf-8'))
    tokens = report['tokens']
    rows = list(csv.reader((details / 'root-tokens.tsv').read_text(encoding='utf-8-sig').splitlines(), delimiter='\t'))
    list_names = ['Browser', 'ArmorSet', 'LocomotorSet', 'EquivalentTo', 'AI', 'UpgradeCameo', 'ProjectedBuildabilityInfo', 'DisplayUpgrade']
    layouts = []
    for platform in (0, 1, 2, 99):
        data = (details / f'{platform}-defaults-0.bin').read_bytes()
        fmt = '<III' if platform == 0 else '>III'
        native_root = struct.unpack_from(fmt, data, 0)[2] - 4
        if native_root != 1620 or len(data) != 2212:
            raise ValueError('Unexpected original default root/prefix/size')
        for extension in ('relo', 'imp'):
            if (details / f'{platform}-defaults-0.{extension}').read_bytes():
                raise ValueError('Unexpected original default auxiliary buffer')
        runtime = [row for row in rows if int(row[0]) == platform]
        if len(runtime) != 127:
            raise ValueError('Incomplete runtime root table')
        for token, row in zip(tokens, runtime):
            index, name_hash, type_hash, target = map(int, row[1:5])
            if index != token['index'] or name_hash != token['nameHash'] or type_hash != token['typeHash']:
                raise ValueError('Source/runtime token keys differ')
            name = row[5]
            if index == 126:
                if target or name_hash or type_hash:
                    raise ValueError('Original sentinel differs')
                continue
            if name == '<unresolved>':
                raise ValueError('Unresolved original field name: ' + str(index))
            offset = token['nativeOffset']
            if name in list_names:
                list_target = 1524 + 12 * list_names.index(name)
                if target != list_target or struct.unpack_from(fmt, data, target) != (1820487795, 1564774360, native_root + offset):
                    raise ValueError('Original default list count token: ' + name)
                category = 'empty-list-count-table'
            elif target == 0:
                category = 'null-token-offset'
            elif target == native_root + offset:
                category = 'native-member-bookmark'
            else:
                raise ValueError('Unmapped original default token target: ' + name)
            if token['kind'] == 'null-only-token-stub' and target:
                raise ValueError('Original stub emitted nonzero token offset')
            if platform == 0:
                token.update(name=name, defaultTokenKind=category)
            elif token['name'] != name or token['defaultTokenKind'] != category:
                raise ValueError('Original target classification varies by platform')
        layouts.append({'platform': platform, 'nativeRootOffset': native_root, 'instanceBytes': len(data), 'relocationBytes': 0, 'importsBytes': 0})
    report['defaultLayouts'] = layouts
    report_file.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print('PASS original source/runtime join: 126 named fields and sentinel on four targets; eight empty-list tables; default prefix=1620, instance=2212, empty relocation/imports')


if __name__ == '__main__':
    verify(Path(sys.argv[1]), Path(sys.argv[2]))
