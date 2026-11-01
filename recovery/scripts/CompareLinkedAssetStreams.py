#!/usr/bin/env python3
"""Read-only raw v6 stream comparison. Never rewrites or normalizes compiler output."""
import argparse
from collections import Counter
from contextlib import ExitStack
import hashlib
from itertools import islice
import json
import mmap
from pathlib import Path
import struct

HEADER = struct.Struct('<BBH11I')
ENTRY = struct.Struct('<12I')
ENTRY_FIELDS = ('typeId', 'instanceId', 'typeHash', 'instanceHash', 'assetReferenceOffset',
                'assetReferenceCount', 'nameOffset', 'sourceOffset', 'instanceBytes',
                'relocationBytes', 'importBytes', 'tokenized')


def need(condition, text):
    if not condition:
        raise ValueError(text)


def parse_manifest(path):
    data = path.read_bytes()
    need(len(data) >= 48, f'{path}: truncated header')
    header = HEADER.unpack_from(data)
    need(header[:3] == (0, 1, 6), f'{path}: requires linked little-endian v6 manifest')
    checksum, all_types, count, total, max_instance, max_relo, max_import, refs, streams, names, sources = header[3:]
    name_start = 48 + 48 * count + refs + streams
    source_start = name_start + names
    need(source_start + sources == len(data), f'{path}: invalid manifest tables')

    def string(start, length, offset):
        need(offset < length, f'{path}: string offset outside table')
        end = data.find(b'\0', start + offset, start + length)
        need(end >= 0, f'{path}: unterminated string')
        return data[start + offset:end].decode('utf-8', errors='replace')

    assets = {}
    offsets = {'.bin': 4, '.relo': 4, '.imp': 4}
    for i in range(count):
        values = ENTRY.unpack_from(data, 48 + 48 * i)
        row = dict(zip(ENTRY_FIELDS, values))
        row.update(name=string(name_start, names, row['nameOffset']),
                   source=string(source_start, sources, row['sourceOffset']), index=i,
                   offsets=dict(offsets))
        key = (row['typeId'], row['instanceId'])
        need(key not in assets, f'{path}: duplicate asset identity {key}')
        assets[key] = row
        for ext, field in (('.bin', 'instanceBytes'), ('.relo', 'relocationBytes'), ('.imp', 'importBytes')):
            offsets[ext] += row[field]
    need(offsets['.bin'] == total + 4, f'{path}: instance size sum differs from header')
    return dict(data=data, checksum=checksum, allTypesHash=all_types, count=count,
                assets=assets, lengths=offsets)


def file_pair(old, new):
    a, b = hashlib.sha256(), hashlib.sha256()
    equal = True
    old_bytes = new_bytes = 0
    with old.open('rb') as left, new.open('rb') as right:
        while True:
            x, y = left.read(4 << 20), right.read(4 << 20)
            if not x and not y:
                break
            a.update(x); b.update(y)
            old_bytes += len(x); new_bytes += len(y)
            equal = equal and x == y
    return dict(rawBytesEqual=equal, oldBytes=old_bytes, newBytes=new_bytes,
                oldSha256=a.hexdigest(), newSha256=b.hexdigest())


def chunk_pair(left, right):
    if left == right:
        return None
    different = sum(x != y for x, y in zip(left, right)) + abs(len(left) - len(right))
    first = list(islice((i for i, (x, y) in enumerate(zip(left, right)) if x != y), 25))
    return dict(oldBytes=len(left), newBytes=len(right), differentBytes=different,
                firstOffsets=first, oldSha256=hashlib.sha256(left).hexdigest(),
                newSha256=hashlib.sha256(right).hexdigest())


def dds_info(data):
    if len(data) < 140:
        return None
    offset, size = struct.unpack_from('<II', data, 4)
    if offset != 12 or size + offset != len(data) or data[offset:offset+4] != b'DDS ':
        return None
    if struct.unpack_from('<I', data, offset + 4)[0] != 124:
        return None
    return dict(offset=offset, bytes=size,
                format=data[offset+84:offset+88].decode('ascii', errors='replace'),
                width=struct.unpack_from('<I', data, offset+16)[0],
                height=struct.unpack_from('<I', data, offset+12)[0],
                mipLevels=struct.unpack_from('<I', data, offset+28)[0])


def texture_detail(left, right, first_offsets):
    a, b = dds_info(left), dds_info(right)
    if a is None or b is None:
        return dict(classification='texture-instance-layout-or-non-dds', oldDds=a, newDds=b)
    prefix = a['offset'] + 128
    same_root = left[:a['offset']] == right[:b['offset']]
    same_header = left[a['offset']:prefix] == right[b['offset']:b['offset']+128]
    payload_only = len(left) == len(right) and same_root and same_header
    format_name = a['format'] if a['format'] == b['format'] else a['format'] + '->' + b['format']
    detail = dict(oldDds=a, newDds=b, format=format_name, rootEqual=same_root,
                  headerEqual=same_header, onlyCompressedPayloadDiffers=payload_only and a['format'] in ('DXT1', 'DXT3', 'DXT5'),
                  classification=('texture-dds-' + format_name + '-payload-only') if payload_only else 'texture-root-or-dds-header-change')
    if payload_only and a['format'] == 'DXT1' and first_offsets and (len(left)-prefix) % 8 == 0:
        block = max(0, (first_offsets[0] - prefix) // 8)
        at = prefix + block * 8
        decode = lambda data: dict(zip(('endpoint0', 'endpoint1', 'indices'), struct.unpack_from('<HHI', data, at)))
        detail['firstBlockDifference'] = dict(blockIndex=block, instanceOffset=at, old=decode(left), new=decode(right))
    return detail


def describe(row):
    return {key: value for key, value in row.items() if key != 'offsets'}


def analyze_stream(old_path, new_path):
    left, right = parse_manifest(old_path), parse_manifest(new_path)
    result = dict(oldManifest=str(old_path), newManifest=str(new_path),
                  oldAssetCount=left['count'], newAssetCount=right['count'],
                  manifestEqual=left['data'] == right['data'], streamChecksumEqual=left['checksum'] == right['checksum'],
                  allTypesHashEqual=left['allTypesHash'] == right['allTypesHash'], assets=[],
                  onlyOld=[describe(left['assets'][k]) for k in sorted(left['assets'].keys()-right['assets'].keys())],
                  onlyNew=[describe(right['assets'][k]) for k in sorted(right['assets'].keys()-left['assets'].keys())],
                  rawFiles={})
    with ExitStack() as stack:
        maps = [{}, {}]
        for ext in ('.bin', '.imp', '.relo'):
            for index, (manifest, path) in enumerate(((left, old_path), (right, new_path))):
                target = path.with_suffix(ext)
                need(target.stat().st_size == manifest['lengths'][ext], f'{target}: chunks do not consume stream')
                stream = stack.enter_context(target.open('rb'))
                view = stack.enter_context(mmap.mmap(stream.fileno(), 0, access=mmap.ACCESS_READ))
                need(struct.unpack_from('<I', view)[0] == manifest['checksum'], f'{target}: checksum differs from own manifest')
                maps[index][ext] = view
            result['rawFiles'][ext] = file_pair(old_path.with_suffix(ext), new_path.with_suffix(ext))
        for key in sorted(left['assets'].keys() & right['assets'].keys()):
            a, b = left['assets'][key], right['assets'][key]
            changes = {}
            # Name/metadata/order changes stay visible even if payloads match.
            meta = [field for field in ENTRY_FIELDS + ('name', 'source', 'index') if a[field] != b[field]]
            for ext, field in (('.bin', 'instanceBytes'), ('.relo', 'relocationBytes'), ('.imp', 'importBytes')):
                x = maps[0][ext][a['offsets'][ext]:a['offsets'][ext]+a[field]]
                y = maps[1][ext][b['offsets'][ext]:b['offsets'][ext]+b[field]]
                difference = chunk_pair(x, y)
                if difference:
                    difference.update(oldOffset=a['offsets'][ext], newOffset=b['offsets'][ext])
                    if ext == '.bin':
                        difference['classification'] = 'non-texture-instance-change'
                        if a['typeId'] == b['typeId'] == 568797146 and a['name'].startswith('Texture:') and b['name'].startswith('Texture:'):
                            difference.update(texture_detail(x, y, difference['firstOffsets']))
                    changes[ext] = difference
            if changes or meta:
                result['assets'].append(dict(name=a['name'], newName=b['name'], typeId=key[0], instanceId=key[1], metadataFields=meta, chunks=changes))
    result['differentPayloadAssetCount'] = sum(bool(row['chunks']) for row in result['assets'])
    result['instanceDifferenceBytes'] = sum(row['chunks'].get('.bin', {}).get('differentBytes', 0) for row in result['assets'])
    result['classificationCounts'] = dict(Counter(row['chunks']['.bin']['classification'] for row in result['assets'] if '.bin' in row['chunks']))
    return result


def historical_comparison(streams, baseline_path):
    baseline = json.loads(baseline_path.read_text(encoding='utf-8-sig'))
    old = {row['name']: row for row in baseline['assets']}
    # Compare only the original baseline base stream, not same-name APT assets.
    candidates = [(name, stream) for name, stream in streams.items() if name.casefold() == 'base/mod/mod.manifest']
    need(len(candidates) == 1, 'Historical comparison requires base/mod/mod.manifest')
    stream = candidates[0][1]
    if 'analysisError' in stream:
        return dict(baselinePath=str(baseline_path), unavailable=stream['analysisError'])
    current = {row['name']: row['chunks']['.bin'] for row in stream['assets'] if '.bin' in row['chunks']}
    common = old.keys() & current.keys()
    changed = [name for name in sorted(common) if any(old[name].get(k) != current[name].get(k) for k in ('oldSha256', 'newSha256', 'differentBytes')) or old[name].get('bytes') != current[name]['oldBytes'] or old[name].get('bytes') != current[name]['newBytes']]
    return dict(baselinePath=str(baseline_path), baselineDifferentAssets=len(old),
                baselineDifferentBytes=sum(row['differentBytes'] for row in old.values()),
                sameDifferingAssetNames=old.keys() == current.keys(),
                addedDifferingAssets=sorted(current.keys()-old.keys()), removedDifferingAssets=sorted(old.keys()-current.keys()),
                changedHashesSizesOrDifferenceCounts=changed,
                exactHistoricalDifferingChunks=old.keys() == current.keys() and not changed,
                currentBaseManifestEqual=stream['manifestEqual'],
                currentBaseAddedAssets=len(stream['onlyNew']), currentBaseRemovedAssets=len(stream['onlyOld']),
                currentBaseAssetsWithMetadataChanges=sum(bool(row['metadataFields']) for row in stream['assets']),
                currentBaseAssetsWithImportOrRelocationChanges=[row['name'] for row in stream['assets'] if '.imp' in row['chunks'] or '.relo' in row['chunks']],
                scope='Historical input removed 89 Joypad declarations; this compares differing asset identities/chunks only, never equates complete inputs or outputs.')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--old-root', required=True, type=Path)
    parser.add_argument('--new-root', required=True, type=Path)
    parser.add_argument('--output', required=True, type=Path)
    parser.add_argument('--baseline', type=Path)
    args = parser.parse_args()
    roots = [args.old_root.resolve(), args.new_root.resolve()]
    files = [{p.relative_to(root).as_posix(): p for p in root.rglob('*') if p.is_file()} for root in roots]
    shared = files[0].keys() & files[1].keys()
    report = dict(scope='Raw archived files and independently manifest-bound chunks; no byte normalization. Omitted archive members cannot be classified.',
                  oldRoot=str(roots[0]), newRoot=str(roots[1]), onlyOldFiles=sorted(files[0].keys()-files[1].keys()),
                  onlyNewFiles=sorted(files[1].keys()-files[0].keys()), fileCountOld=len(files[0]), fileCountNew=len(files[1]),
                  archivedCdataFilesOld=sum(n.endswith('.cdata') for n in files[0]), archivedCdataFilesNew=sum(n.endswith('.cdata') for n in files[1]),
                  differentFiles={}, streams={}, streamAnalysisErrors={})
    for name in sorted(shared):
        comparison = file_pair(files[0][name], files[1][name])
        if not comparison['rawBytesEqual']:
            report['differentFiles'][name] = comparison
        if name.endswith('.manifest'):
            try:
                report['streams'][name] = analyze_stream(files[0][name], files[1][name])
            except (ValueError, FileNotFoundError) as error:
                message = type(error).__name__ + ': ' + str(error)
                report['streams'][name] = dict(analysisError=message)
                report['streamAnalysisErrors'][name] = message
    if args.baseline:
        report['historicalComparison'] = historical_comparison(report['streams'], args.baseline)
    report['allSharedStreamsStructurallyValid'] = not report['streamAnalysisErrors']
    report['rawAllArchivedFilesEqual'] = not report['onlyOldFiles'] and not report['onlyNewFiles'] and not report['differentFiles']
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
    print(json.dumps({key: report[key] for key in ('fileCountOld', 'fileCountNew', 'rawAllArchivedFilesEqual', 'archivedCdataFilesOld', 'archivedCdataFilesNew')}, ensure_ascii=False))
    print(json.dumps(dict(streamCount=len(report['streams']), differentFiles=list(report['differentFiles']), historicalComparison=report.get('historicalComparison')), ensure_ascii=False))

if __name__ == '__main__':
    main()
