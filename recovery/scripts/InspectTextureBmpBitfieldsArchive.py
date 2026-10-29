#!/usr/bin/env python3
"""Audit original BMP native/loaded snapshots and cross-context observations."""
import argparse
from collections import Counter, defaultdict
import csv
import hashlib
import io
import json
from pathlib import Path
import struct
import zipfile


def digest(data):
    return hashlib.sha256(data).hexdigest()


def boolean(value):
    assert value in ('True', 'False'), value
    return value == 'True'


def expected_corpus():
    result = {}
    for control in (True, False):
        for context in ((0,) if control else (0, 1)):
            for dib in ((40,) if control else (40, 52, 56, 108, 124)):
                for depth in (16, 24, 32):
                    for width, height in (((16, 8),) if control else ((1, 1), (16, 8), (7, 5))):
                        for top in (False, True):
                            for pattern in ((0,) if control else (0, 1, 2)):
                                for profile in ((0,) if control else range(8)):
                                    kind = 'control' if control else 'bitfields'
                                    label = f'{"control" if control else "bf"}-h{dib}-d{depth}-{width}x{height}-top{top}-p{pattern}-m{profile}-ctx{context}'
                                    result[label] = dict(kind=kind, context=str(context), dib=str(dib),
                                                         depth=str(depth), sourceWidth=str(width),
                                                         sourceHeight=str(height), topDown=str(top),
                                                         pattern=str(pattern), maskProfile=str(profile),
                                                         compression='0' if control else '3')
    assert len(result) == 4326
    return result


def storage_bpp(native_format):
    return {0x28000002: 1, 0x0800000a: 2, 0x28000018: 2,
            0x28280003: 2, 0x18280003: 2, 0x18280006: 4,
            0x28280006: 4, 0x1a20001a: 8}.get(native_format & 0xfffffe3f, 0)


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/bmp-bitfields-load' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['sourceCommit'] == source and str(receipt['runId']) == run
    assert receipt['status'] == 'probe-only' and receipt['probeOnly'] is True
    assert receipt['evidenceOnly'] is True and receipt['productionAccepted'] is False and receipt['sdkValidated'] is False
    assert receipt['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    assert receipt['crossProcessSourceIdentityVerified'] is True
    assert receipt['pairedInputCount'] == 4320 and receipt['pairedControlInputCount'] == 6
    assert len(receipt['processes']) == 2
    for key, expected in {'fixtureCount': 8640, 'controlCount': 12, 'fixtureOriginalCalls': 17304,
                          'contextOriginalCalls': 8640, 'initializationControlCalls': 2,
                          'originalCalls': 25946, 'nativeSnapshots': 17304}.items():
        assert receipt[key] == expected, key
    archive = (directory / 'samples.zip').read_bytes()
    assert digest(archive) == receipt['samplesSha256']
    totals, formats, by_dib_depth = Counter(), Counter(), defaultdict(Counter)
    observations = defaultdict(list)
    corpus = expected_corpus()
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = [i.filename for i in files.infolist() if not i.is_dir()]
        assert len(names) == len(set(names))
        expected_names = set()
        labels_by_process = []
        for process in (0, 1):
            prefix = 'process-' + str(process) + '/'
            expected_names.update({prefix + 'transcript.tsv', prefix + 'context-known-birgb.bmp', prefix + 'progress.tsv'})
            context = files.read(prefix + 'context-known-birgb.bmp')
            assert context[:2] == b'BM'
            progress = list(csv.DictReader(io.StringIO(files.read(prefix + 'progress.tsv').decode('utf-8-sig')), delimiter='\t'))
            assert progress == [{'label': 'all', 'phase': 'completed'}]
            rows = list(csv.DictReader(io.StringIO(files.read(prefix + 'transcript.tsv').decode('utf-8-sig')), delimiter='\t'))
            external = list(csv.DictReader(io.StringIO((directory / ('process-' + str(process) + '.tsv')).read_text(encoding='utf-8-sig')), delimiter='\t'))
            assert rows == external and len(rows) == 4326
            labels = {r['label']: r for r in rows}
            assert len(labels) == len(rows)
            assert set(labels) == set(corpus), 'Incomplete or unexpected fixture axes'
            labels_by_process.append(labels)
            process_totals = Counter()
            for row in rows:
                assert len(row) == 56 and row['process'] == str(process)
                assert row['kind'] in ('control', 'bitfields')
                assert row['context'] in ('0', '1')
                for key, value in corpus[row['label']].items():
                    assert row[key] == value, (row['label'], key)
                label = prefix + row['label']
                expected_names.add(label + '.bmp')
                image = files.read(label + '.bmp')
                assert image[:2] == b'BM' and digest(image) == row['inputSha256']
                assert struct.unpack_from('<I', image, 14)[0] == int(row['dib'])
                assert struct.unpack_from('<i', image, 18)[0] == int(row['sourceWidth'])
                assert struct.unpack_from('<i', image, 22)[0] == int(row['sourceHeight']) * (-1 if boolean(row['topDown']) else 1)
                assert struct.unpack_from('<H', image, 28)[0] == int(row['depth'])
                assert struct.unpack_from('<I', image, 30)[0] == int(row['compression'])
                if row['compression'] == '3':
                    for channel, field in enumerate(('maskR', 'maskG', 'maskB')):
                        assert struct.unpack_from('<I', image, 54 + channel * 4)[0] == int(row[field], 16)
                    if int(row['dib']) >= 56:
                        assert struct.unpack_from('<I', image, 66)[0] == int(row['maskA'], 16)
                    else:
                        assert int(row['maskA'], 16) == 0
                native, loaded = [], []
                for index in (1, 2):
                    np, lp = 'native' + str(index), 'loaded' + str(index)
                    boolean(row[lp + 'HasAlpha'])
                    descriptor = storage = rgba = None
                    if row[np + 'DescriptorSha256']:
                        path = label + '.call' + str(index) + '.descriptor.bin'
                        expected_names.add(path)
                        descriptor = files.read(path)
                        assert len(descriptor) == 60 and digest(descriptor) == row[np + 'DescriptorSha256']
                        assert struct.unpack_from('<i', descriptor, 4)[0] == int(row[np + 'Width'])
                        assert struct.unpack_from('<i', descriptor, 8)[0] == int(row[np + 'Height'])
                        assert struct.unpack_from('<I', descriptor, 16)[0] == int(row[np + 'Format'], 16)
                    if row[np + 'PrefixSha256']:
                        path = label + '.call' + str(index) + '.locked-prefix.bin'
                        expected_names.add(path)
                        storage = files.read(path)
                        width, height, pitch = (int(row[np + s]) for s in ('Width', 'Height', 'Pitch'))
                        assert 0 < width <= 64 and 0 < height <= 64 and 0 < pitch <= 4096
                        bpp = int(row[np + 'StorageBytesPerPixel'])
                        assert bpp == storage_bpp(int(row[np + 'Format'], 16)) and bpp in (1, 2, 4, 8)
                        assert int(row[np + 'Status']) >= 0 and not row[np + 'Error'] and descriptor is not None
                        assert len(storage) == int(row[np + 'PrefixBytes']) == int(row[np + 'FirstRowLogicalBytes']) == width * bpp
                        assert len(storage) <= pitch and not row[np + 'PrefixAbsentReason']
                        assert digest(storage) == row[np + 'PrefixSha256']
                        totals['lockedFirstRowPrefixBytes'] += len(storage)
                    else:
                        assert int(row[np + 'PrefixBytes']) == 0
                        assert int(row[np + 'Status']) < 0 or row[np + 'Error'] or row[np + 'PrefixAbsentReason']
                        if row[np + 'PrefixAbsentReason'] == 'unknown-native-format':
                            assert descriptor is not None and storage_bpp(int(row[np + 'Format'], 16)) == 0
                    if row[lp + 'Sha256']:
                        path = label + '.call' + str(index) + '.rgba'
                        expected_names.add(path)
                        rgba = files.read(path)
                        assert not row[lp + 'Error']
                        width, height = int(row[lp + 'Width']), int(row[lp + 'Height'])
                        assert width > 0 and height > 0 and width * height < 16777216
                        assert len(rgba) == int(row[lp + 'Bytes']) == 4 * width * height
                        assert digest(rgba) == row[lp + 'Sha256']
                        totals['loadedRgbaBytes'] += len(rgba)
                    else:
                        assert row[lp + 'Error'] and int(row[lp + 'Bytes']) == 0
                    native.append((row[np + 'Status'], row[np + 'Error'], descriptor,
                                   tuple(row[np + s] for s in ('Width', 'Height', 'Pitch', 'Format', 'PrefixAbsentReason')), storage))
                    loaded.append((tuple(row[lp + s] for s in ('Error', 'Width', 'Height', 'HasAlpha')), rgba))
                stable = loaded[0] == loaded[1]
                descriptor_stable = native[0][:3] == native[1][:3]
                storage_stable = native[0][:2] == native[1][:2] and native[0][3:] == native[1][3:]
                assert boolean(row['originalStable']) == stable
                assert boolean(row['descriptorStable']) == descriptor_stable
                assert boolean(row['prefixStable']) == storage_stable
                if row['kind'] == 'control':
                    assert row['compression'] == '0' and stable and not row['loaded1Error']
                    for index in (1, 2):
                        np, lp = 'native' + str(index), 'loaded' + str(index)
                        assert int(row[np + 'Status']) >= 0 and not row[np + 'Error']
                        assert row[np + 'Width'] == '16' and row[np + 'Height'] == '8'
                        assert row[np + 'DescriptorSha256'] and row[np + 'PrefixSha256']
                        assert row[np + 'PrefixAbsentReason'] == ''
                        assert row[lp + 'Width'] == '16' and row[lp + 'Height'] == '8'
                        assert row[lp + 'Bytes'] == '512' and row[lp + 'Sha256'] and not row[lp + 'Error']
                    process_totals['controlCount'] += 1
                    continue
                assert row['compression'] == '3'
                process_totals['fixtureCount'] += 1
                outcome = 'rejectedCount' if row['loaded1Error'] else 'decodedCount'
                process_totals[outcome] += 1
                process_totals['unstableOriginals'] += not stable
                process_totals['descriptorDifferences'] += not descriptor_stable
                process_totals['prefixDifferences'] += not storage_stable
                process_totals['firstPrefixAbsent'] += bool(row['native1PrefixAbsentReason'])
                process_totals['repeatPrefixAbsent'] += bool(row['native2PrefixAbsentReason'])
                formats[row['dib'] + '/' + row['depth'] + '/' + row['native1Format']] += 1
                by_dib_depth[row['dib'] + '/' + row['depth']][outcome] += 1
                by_dib_depth[row['dib'] + '/' + row['depth']]['repeatDifferences'] += not stable
                # Same source bytes, regardless of duplicate equivalent mask profiles.
                observations[row['inputSha256']].extend(loaded)
            reported = receipt['processes'][process]
            assert reported['process'] == process
            for key, expected in {'fixtureLoadCalls': 8652, 'contextLoadCalls': 4320,
                                  'initializationControlLoadCalls': 1, 'nativeSnapshots': 8652}.items():
                assert reported[key] == expected, (process, key)
            for key, count in process_totals.items():
                assert reported[key] == count, (process, key)
                totals[key] += count
        assert set(labels_by_process[0]) == set(labels_by_process[1])
        for label, row in labels_by_process[0].items():
            assert row['inputSha256'] == labels_by_process[1][label]['inputSha256']
        assert set(names) == expected_names, 'Missing or unexpected archived files'
    changed_sources = sum(any(value != values[0] for value in values[1:]) for values in observations.values())
    return {
        'scope': 'independent original snapshot archive audit; probe-only, not recovery acceptance',
        'sourceCommit': source, 'runId': run, 'archiveSha256': receipt['samplesSha256'],
        'totals': dict(totals), 'uniqueBitfieldsInputs': len(observations),
        'inputsWithAnyLoadedDifferenceAcrossRepeatsContextsProcesses': changed_sources,
        'nativeFormats': dict(sorted(formats.items())),
        'byDibDepth': {key: dict(value) for key, value in sorted(by_dib_depth.items())},
        'limits': 'Native snapshots and LoadImageA use separate texture creations. Only a known-format logical first-row prefix is read, never pitch*height or trailing padding. Unknown storage formats are not dereferenced. Prefix/descriptor equality is recorded, not treated as determinism or recovery gold. All extended bitfields remain unrecovered until separate complete acceptance.'
    }


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve(), args.run_id, args.source_commit)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result['totals'], sort_keys=True))
