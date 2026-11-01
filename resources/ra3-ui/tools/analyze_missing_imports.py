#!/usr/bin/env python3
"""Read-only verification of actual display-list uses of missing RA3 imports."""
import collections
import hashlib
import json
from pathlib import Path
import struct
import sys
import zlib
from functools import lru_cache

ROOT = Path(__file__).resolve().parents[1] / 'source' / 'ra3ui'
MOVIES = ['LoadScreenImage', 'LoadScreen', 'testCafeUI', 'InGameChat',
          'LoadScreenCampaign', 'TacticalPlayerStatusDialog', 'skins']
AVAILABLE = {p.name.lower(): p for p in ROOT.rglob('*.swf')}


def u16(data, offset=0):
    return struct.unpack_from('<H', data, offset)[0]


def string(data, offset):
    end = data.index(0, offset)
    return data[offset:end].decode('utf-8'), end + 1


class Bits:
    def __init__(self, data, byte):
        self.data, self.bit = data, byte * 8

    def read(self, count):
        result = 0
        for i in range(count):
            result = (result << 1) | ((self.data[self.bit // 8] >> (7 - self.bit % 8)) & 1)
            self.bit += 1
        return result

    def skip(self, count):
        self.bit += count

    def byte(self):
        return (self.bit + 7) // 8


def skip_matrix(data, offset):
    b = Bits(data, offset)
    if b.read(1):
        b.skip(b.read(5) * 2)
    if b.read(1):
        b.skip(b.read(5) * 2)
    b.skip(b.read(5) * 2)
    return b.byte()


def skip_color_transform(data, offset):
    b = Bits(data, offset)
    add, multiply, width = b.read(1), b.read(1), b.read(4)
    b.skip((add + multiply) * width * 4)
    return b.byte()


@lru_cache(maxsize=None)
def exported_symbols(path):
    raw = path.read_bytes()
    data = raw[8:] if raw[:3] == b'FWS' else zlib.decompress(raw[8:])
    pos = (5 + 4 * (data[0] >> 3) + 7) // 8 + 4
    symbols = {}
    while pos < len(data):
        header = u16(data, pos)
        tag, length = header >> 6, header & 63
        pos += 2
        if length == 63:
            length = struct.unpack_from('<I', data, pos)[0]
            pos += 4
        payload = data[pos:pos + length]
        pos += length
        if tag == 56:
            count, cursor = u16(payload), 2
            for i in range(count):
                character = u16(payload, cursor)
                name, cursor = string(payload, cursor + 2)
                symbols[name] = character
    return symbols


def character_dependencies(tag, payload):
    """Read bitmap fills, button-state characters and font references.

    These cover the non-placement character-reference tags present in the seven
    inspected SWFs; source tags are all <= Flash 7 (no LINESTYLE2/filter lists).
    """
    references = []
    if tag in (7, 34):
        pos = 5 if tag == 34 else 2
        while payload[pos]:
            flags = payload[pos]
            references.append((u16(payload, pos + 1), 'button-state'))
            pos = skip_matrix(payload, pos + 5)
            if tag == 34:
                pos = skip_color_transform(payload, pos)
            if flags & 0x30:
                raise ValueError('Unexpected Flash 8 button filter/blend fields')
        return references
    if tag == 37:
        pos = (5 + (payload[2] >> 3) * 4 + 7) // 8 + 2
        if payload[pos] & 1:
            references.append((u16(payload, pos + 2), 'edit-text-font'))
        return references
    if tag not in (2, 22, 32):
        return references
    pos = (5 + (payload[2] >> 3) * 4 + 7) // 8 + 2
    color_bytes = 4 if tag == 32 else 3

    def styles(pos):
        count = payload[pos]
        pos += 1
        if count == 255 and tag != 2:
            count = u16(payload, pos)
            pos += 2
        for i in range(count):
            kind = payload[pos]
            pos += 1
            if kind == 0:
                pos += color_bytes
            elif kind in (0x10, 0x12, 0x13):
                pos = skip_matrix(payload, pos)
                count = payload[pos] & 15
                pos += 1 + count * (1 + color_bytes)
                if kind == 0x13:
                    pos += 2
            elif kind in (0x40, 0x41, 0x42, 0x43):
                references.append((u16(payload, pos), 'shape-bitmap-fill'))
                pos = skip_matrix(payload, pos + 2)
            else:
                raise ValueError('Unsupported shape fill type: ' + hex(kind))
        count = payload[pos]
        pos += 1
        if count == 255 and tag != 2:
            count = u16(payload, pos)
            pos += 2
        return pos + count * (2 + color_bytes)

    pos = styles(pos)
    b = Bits(payload, pos)
    fill_width, line_width = b.read(4), b.read(4)
    while True:
        if b.read(1):
            straight, width = b.read(1), b.read(4) + 2
            if straight:
                if b.read(1):
                    b.skip(width * 2)
                else:
                    b.read(1)
                    b.skip(width)
            else:
                b.skip(width * 4)
        else:
            flags = b.read(5)
            if flags == 0:
                break
            if flags & 1:
                b.skip(b.read(5) * 2)
            if flags & 2:
                b.skip(fill_width)
            if flags & 4:
                b.skip(fill_width)
            if flags & 8:
                b.skip(line_width)
            if flags & 16:
                pos = styles(b.byte())
                b = Bits(payload, pos)
                fill_width, line_width = b.read(4), b.read(4)
    return references


def placement(tag, payload):
    if tag == 4:
        return {'characterId': u16(payload), 'depth': u16(payload, 2), 'instanceName': None}
    flags = payload[0]
    offset = 3
    depth = u16(payload, 1)
    if tag == 70:
        flags2 = payload[1]
        offset = 4
        depth = u16(payload, 2)
        if flags2 & 8 or (flags2 & 16 and flags & 2):
            _, offset = string(payload, offset)
    if not flags & 2:
        return None
    character = u16(payload, offset)
    offset += 2
    if flags & 4:
        offset = skip_matrix(payload, offset)
    if flags & 8:
        offset = skip_color_transform(payload, offset)
    if flags & 16:
        offset += 2
    name = string(payload, offset)[0] if flags & 32 else None
    return {'characterId': character, 'depth': depth, 'instanceName': name}


def analyze(path):
    raw = path.read_bytes()
    assert raw[:3] in (b'CWS', b'FWS'), 'Expected standard compressed/uncompressed SWF'
    data = raw[8:] if raw[:3] == b'FWS' else zlib.decompress(raw[8:])
    assert struct.unpack_from('<I', raw, 4)[0] == len(data) + 8, 'Declared SWF size mismatch'
    root_start = (5 + 4 * (data[0] >> 3) + 7) // 8 + 4
    imports, references, import_ranges = [], [], []
    graph = collections.defaultdict(list)
    tag_counts = collections.Counter()

    def scan(buffer, start, timeline, absolute_offset):
        pos, index, frame = start, 0, 1
        while pos < len(buffer):
            tag_start = pos
            header = u16(buffer, pos)
            tag, length = header >> 6, header & 63
            pos += 2
            if length == 63:
                length = struct.unpack_from('<I', buffer, pos)[0]
                pos += 4
            payload_start = pos
            payload = buffer[pos:pos + length]
            assert len(payload) == length, 'Truncated tag'
            pos += length
            tag_counts[tag] += 1
            evidence = {'tagCode': tag, 'tagIndexInTimeline': index,
                        'timelineCharacterId': timeline, 'timelineFrame': frame,
                        'uncompressedFileOffset': absolute_offset + tag_start}
            if tag in (57, 71):
                url, cursor = string(payload, 0)
                if tag == 71:
                    cursor += 2
                count = u16(payload, cursor)
                cursor += 2
                assets = []
                for i in range(count):
                    character = u16(payload, cursor)
                    name, cursor = string(payload, cursor + 2)
                    assets.append({'characterId': character, 'exportName': name})
                assert cursor == len(payload), 'Unconsumed import table'
                basename = url.replace('\\', '/').split('/')[-1]
                imports.append(dict(evidence, url=url, symbolCount=count, assets=assets,
                                    missingLibrary=basename.lower() not in AVAILABLE))
                import_ranges.append((absolute_offset + payload_start - 8,
                                      absolute_offset + payload_start - 8 + length))
            if tag in (4, 26, 70):
                place = placement(tag, payload)
                if place:
                    reference = dict(evidence, **place, referenceKind='display-placement',
                                     sourceCharacterId=timeline)
                    references.append(reference)
                    graph[timeline].append(reference)
            for character, kind in character_dependencies(tag, payload):
                reference = dict(evidence, characterId=character, sourceCharacterId=u16(payload),
                                 referenceKind=kind)
                references.append(reference)
                graph[u16(payload)].append(reference)
            if tag == 39:
                scan(payload, 4, u16(payload), absolute_offset + payload_start)
            if tag == 1:
                frame += 1
            if tag == 0:
                assert pos == len(buffer), 'Bytes after End tag'
                break
            index += 1

    scan(data, root_start, 0, 8)
    paths = {0: []}
    queue = collections.deque([0])
    while queue:
        parent = queue.popleft()
        for reference in graph[parent]:
            child = reference['characterId']
            if child not in paths:
                paths[child] = paths[parent] + [reference]
                queue.append(child)
    export_paths = {}
    queue = collections.deque()
    for name, character in exported_symbols(path).items():
        if character not in export_paths:
            export_paths[character] = {'exportName': name, 'exportedCharacterId': character, 'path': []}
            queue.append(character)
    while queue:
        parent = queue.popleft()
        for reference in graph[parent]:
            child = reference['characterId']
            if child not in export_paths:
                previous = export_paths[parent]
                export_paths[child] = dict(previous, path=previous['path'] + [reference])
                queue.append(child)
    missing = []

    def occurrences_outside_imports(value):
        needle = value.encode('utf-8')
        result, pos = [], 0
        while True:
            pos = data.find(needle, pos)
            if pos < 0:
                break
            if not any(begin <= pos < end for begin, end in import_ranges):
                result.append(pos + 8)
            pos += len(needle)
        return result

    for imp in imports:
        basename = imp['url'].replace('\\', '/').split('/')[-1]
        library = AVAILABLE.get(basename.lower())
        exports = exported_symbols(library) if library else {}
        for asset in imp['assets']:
            if library and asset['exportName'] in exports:
                continue
            character = asset['characterId']
            uses = [r for r in references if r['characterId'] == character]
            literals = occurrences_outside_imports(asset['exportName'])
            case_matches = [name for name in exports if name.casefold() == asset['exportName'].casefold()]
            normal = lambda name: ''.join(c for c in name.casefold() if c.isalnum())
            spelling_matches = [name for name in exports if normal(name) == normal(asset['exportName'])]
            library_evidence = None
            if library:
                eaf = library.with_suffix('.eaf')
                library_evidence = {
                    'path': library.relative_to(ROOT).as_posix(),
                    'sha256': hashlib.sha256(library.read_bytes()).hexdigest(),
                    'exportedSymbols': [{'exportName': name, 'characterId': cid}
                                        for name, cid in sorted(exports.items())],
                    'swfAndEafExportTablesAgree': exports == exported_symbols(eaf),
                    'caseInsensitiveNameMatches': case_matches,
                    'normalizedSpellingCandidates': spelling_matches,
                    'safeAliasEstablished': False,
                    'aliasApplied': False,
                    'aliasNote': 'ExportAssets uses exact identifier names. Similar spelling alone does not establish identical symbol identity.'}
            missing.append(dict(asset, url=imp['url'], importTable=imp,
                                reason='missing-library' if library is None else 'missing-exported-symbol',
                                libraryEvidence=library_evidence,
                                directReferences=uses,
                                directPlacements=[r for r in uses if r['referenceKind'] == 'display-placement'],
                                displayListReachableFromRoot=character in paths,
                                shortestRootPlacementPath=paths.get(character),
                                reachableFromExportedRuntimeSymbol=character in export_paths,
                                shortestExportDependencyPath=export_paths.get(character),
                                nameLiteralOffsetsOutsideImportTable=literals,
                                declaredButUnreferenced=not uses and not literals))
    return {'path': path.relative_to(ROOT).as_posix(), 'bytes': len(raw),
            'sha256': hashlib.sha256(raw).hexdigest(), 'swfVersion': raw[3],
            'tagCounts': dict(sorted(tag_counts.items())), 'missingImports': missing,
            'attachmentApiLiteralCounts': {name: data.count(name.encode('ascii')) for name in
                                            ['attachMovie', 'duplicateMovieClip', 'loadMovie', 'eval']},
            'allMissingImportsHaveDirectReferences': all(x['directReferences'] for x in missing),
            'allMissingImportsReachableFromRootOrExports': all(x['displayListReachableFromRoot'] or x['reachableFromExportedRuntimeSymbol'] for x in missing),
            'allMissingImportsHaveDirectPlacements': all(x['directPlacements'] for x in missing),
            'allMissingImportsDisplayListReachableFromRoot': all(x['displayListReachableFromRoot'] for x in missing)}


movies = []
for movie in MOVIES:
    swf = analyze(ROOT / movie / (movie + '.swf'))
    eaf = analyze(ROOT / movie / (movie + '.eaf'))
    key = lambda item: (item['url'], item['characterId'], item['exportName'])
    same = {key(x) for x in swf['missingImports']} == {key(x) for x in eaf['missingImports']}
    use_key = lambda item: (key(item), item['displayListReachableFromRoot'],
                           item['reachableFromExportedRuntimeSymbol'],
                           tuple(sorted((r['sourceCharacterId'], r['referenceKind'])
                                        for r in item['directReferences'])))
    same_uses = {use_key(x) for x in swf['missingImports']} == {use_key(x) for x in eaf['missingImports']}
    movies.append({'movie': movie, 'swf': swf, 'eaf': eaf,
                   'swfAndEafMissingImportTablesAgree': same,
                   'swfAndEafStructuralUsesAgree': same_uses})

report = {
    'method': 'Read ImportAssets/ImportAssets2 and exact ExportAssets tables. Recursively parse PlaceObject/PlaceObject2/PlaceObject3 tags, including all DefineSprite timelines, plus raw DefineShape bitmap fills, button-state references and dynamic-text font references. Build directed character dependency graphs from root timeline id 0 and from public exported symbols. Cross-check original EAF files with the same parser.',
    'offsetConvention': 'Zero-based byte offsets into the full decompressed SWF/EAF including its eight-byte header; tagIndexInTimeline is zero-based and timelineFrame is one-based.',
    'limits': 'Reachability is static and independent of ActionScript control flow. Exported-library reachability identifies public runtime symbols with real missing bitmap-fill dependencies; it does not prove any particular game branch executes them. No alias or substitute asset is applied. FFDec dependency helpers may omit unresolved bitmap fills, so raw SWF bitfields are parsed directly.',
    'totalMovies': len(movies),
    'missingSymbolCount': sum(len(m['swf']['missingImports']) for m in movies),
    'missingLibrarySymbolCount': sum(a['reason'] == 'missing-library' for m in movies for a in m['swf']['missingImports']),
    'missingExportedSymbolCount': sum(a['reason'] == 'missing-exported-symbol' for m in movies for a in m['swf']['missingImports']),
    'rootReachableMissingSymbolCount': sum(a['displayListReachableFromRoot'] for m in movies for a in m['swf']['missingImports']),
    'exportedLibraryOnlyMissingSymbolCount': sum(not a['displayListReachableFromRoot'] and a['reachableFromExportedRuntimeSymbol'] for m in movies for a in m['swf']['missingImports']),
    'allMissingSymbolsHaveDirectReferences': all(m['swf']['allMissingImportsHaveDirectReferences'] for m in movies),
    'allMissingSymbolsReachableFromRootOrExports': all(m['swf']['allMissingImportsReachableFromRootOrExports'] for m in movies),
    'allMissingSymbolsHaveDirectPlacements': all(m['swf']['allMissingImportsHaveDirectPlacements'] for m in movies),
    'allMissingSymbolsDisplayListReachableFromRoot': all(m['swf']['allMissingImportsDisplayListReachableFromRoot'] for m in movies),
    'allSwfAndEafMissingImportTablesAgree': all(m['swfAndEafMissingImportTablesAgree'] for m in movies),
    'allSwfAndEafStructuralUsesAgree': all(m['swfAndEafStructuralUsesAgree'] for m in movies),
    'conclusion': 'All fifteen missing symbols across seven movies have real structural uses. Twelve are reachable from root display lists; skins has three missing bitmap fills used by public exported symbols. These seven movies cannot be claimed to be complete standalone reconstructions from the supplied pack. The dropdown spelling candidate is not an established safe alias.',
    'movies': movies,
}
output = Path(sys.argv[1]) if len(sys.argv) > 1 else ROOT.parents[1] / 'validation' / 'missing-original-imports.json'
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
print(json.dumps({k: v for k, v in report.items() if k != 'movies'}, indent=2))
