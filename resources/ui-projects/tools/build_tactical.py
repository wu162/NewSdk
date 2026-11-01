#!/usr/bin/env python3
"""Import independent recovered HUD documents into a separate editable project.

The original resources/ra3-ui tree is read-only. This importer is distinct from
normal project publishing: after editing Tactical.fla, use UiProject.Build.
"""
import argparse
from collections import defaultdict
import copy
import hashlib
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET

NS = 'http://ns.adobe.com/xfl/2008/'
ET.register_namespace('', NS)
ET.register_namespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance')
MOVIES = ['TacticalHUD', 'TacticalHUDMainCommandBar', 'TacticalHUDBuildQueuePage', 'TacticalHUDRadar']
IGNORED = {'itemID', 'lastModified', 'sourceLastImported', 'current', 'isSelected',
           'selected', 'currentFrame', 'sourceExternalFilepath', 'externalFileSize',
           'bitmapDataHRef', 'soundDataHRef'}
INCLUDE = re.compile(r'^\s*#include\s+["\']([^"\']+)["\']\s*;?\s*$')
LINKAGE = {'linkageIdentifier', 'linkageClassName', 'linkageExportForAS', 'linkageExportForRS',
           'linkageImportForRS', 'linkageExportInFirstFrame', 'linkageURL'}


def tag(name): return '{' + NS + '}' + name
def local(node): return node.tag.rsplit('}', 1)[-1]
def sha(data): return hashlib.sha256(data).hexdigest()


def write_xml(element, path):
    # XML parsers normalize literal CR to LF. Re-escape preserved character
    # references in text so authored CR text remains byte-identical at runtime.
    data = ET.tostring(element, encoding='utf-8', xml_declaration=True)
    Path(path).write_bytes(data.replace(b'\r', b'&#13;'))


class Document:
    def __init__(self, movie, xfl_root, assets):
        self.movie = movie
        self.path = xfl_root / movie / movie
        self.assets = assets / movie
        self.root = ET.parse(self.path / 'DOMDocument.xml').getroot()
        self.symbols = {}
        self.symbol_files = {}
        self.media = {}
        self.order = []
        symbol_list = self.root.find(tag('symbols'))
        for include in list(symbol_list) if symbol_list is not None else []:
            node = ET.parse(self.path / 'LIBRARY' / include.get('href')).getroot()
            name = node.get('name')
            self.symbols[name] = node
            self.symbol_files['LIBRARY/' + include.get('href')] = name
            self.order.append(name)
        media_list = self.root.find(tag('media'))
        for node in list(media_list) if media_list is not None else []:
            self.media[node.get('name')] = node
        self.cache = {}
        self.active = set()
        self.aliases = defaultdict(set)
        for owner in [self.root] + list(self.symbols.values()):
            for element in owner.iter():
                if local(element) == 'DOMSymbolInstance' and element.get('name'):
                    self.aliases[element.get('libraryItemName')].add(element.get('name'))

    def readable_name(self, name):
        node = self.node(name)
        label = node.get('linkageIdentifier') or node.get('linkageClassName')
        aliases = self.aliases.get(name, set())
        if not label and len(aliases) == 1: label = next(iter(aliases))
        label = label or name.replace('/', '_')
        return re.sub(r'[^\w.-]+', '_', label).strip('_')[:56]

    def node(self, name): return self.symbols[name] if name in self.symbols else self.media[name]

    def dependencies(self, name):
        result = []
        for element in self.node(name).iter():
            for key in ('libraryItemName', 'bitmapPath'):
                if key in element.attrib:
                    result.append(element.get(key))
        return result

    def fingerprint(self, name):
        if name in self.cache: return self.cache[name]
        if name in self.active: raise ValueError('Recursive source symbol: ' + name)
        self.active.add(name)
        node = self.node(name)
        if name in self.symbols:
            value = self.canonical(node, True)
        else:
            value = [local(node), {k: v for k, v in node.attrib.items() if k not in IGNORED | {'name', 'href'}},
                     sha((self.path / 'LIBRARY' / node.get('href')).read_bytes())]
        result = sha(json.dumps(value, sort_keys=True, separators=(',', ':')).encode())
        self.cache[name] = result
        self.active.remove(name)
        return result

    def canonical(self, node, root=False):
        attributes = {k: v for k, v in node.attrib.items() if k not in IGNORED}
        if root or local(node) == 'DOMTimeline': attributes.pop('name', None)
        if local(node) == 'DOMLayer': attributes.pop('color', None)
        for key in ('libraryItemName', 'bitmapPath'):
            if key in attributes: attributes[key] = 'GRAPH:' + self.fingerprint(attributes[key])
        text = node.text or ''
        if local(node) != 'characters' and not text.strip(): text = ''
        if local(node) == 'script':
            match = INCLUDE.fullmatch(text)
            if match: text = 'AS:' + sha((self.assets / match[1]).read_bytes())
        return [local(node), attributes, text, [self.canonical(child) for child in node]]


class Sharing:
    def __init__(self, documents):
        self.documents = documents
        self.parent = {(d.movie, name): (d.movie, name) for d in documents.values() for name in list(d.symbols) + list(d.media)}
        self.members = {key: {key[0]: key} for key in self.parent}
        self.seeds = defaultdict(set)

    def find(self, key):
        if self.parent[key] != key: self.parent[key] = self.find(self.parent[key])
        return self.parent[key]

    def pairs(self, left, right, seen=None):
        seen = set() if seen is None else seen
        if (left, right) in seen: return []
        seen.add((left, right))
        a, b = self.documents[left[0]], self.documents[right[0]]
        if a.fingerprint(left[1]) != b.fingerprint(right[1]):
            raise ValueError('Shared graph mismatch: ' + repr((left, right)))
        result = [(left, right)]
        aa, bb = a.dependencies(left[1]), b.dependencies(right[1])
        if len(aa) != len(bb): raise ValueError('Dependency graph arity mismatch')
        for x, y in zip(aa, bb): result.extend(self.pairs((left[0], x), (right[0], y), seen))
        return result

    def merge(self, left, right, identifier):
        pairs = self.pairs(left, right)
        # Validate the entire prospective merge before changing any group. Every
        # original document retains injective descriptor identity/multiplicity.
        temporary = {key: dict(members) for key, members in self.members.items() if self.find(key) == key}
        parents = {key: self.find(key) for key in self.parent}
        def find(key):
            while parents[key] != key: key = parents[key]
            return key
        for a, b in pairs:
            a, b = find(a), find(b)
            if a == b: continue
            if set(temporary[a]) & set(temporary[b]):
                raise ValueError('Sharing would merge two descriptors in one original movie: ' + identifier)
            parents[b] = a
            temporary[a].update(temporary[b])
        for a, b in pairs:
            a, b = self.find(a), self.find(b)
            if a != b:
                self.parent[b] = a
                self.members[a].update(self.members[b])
        self.seeds[self.find(left)].add(identifier)


def original_linkage(node, name):
    if not any(key in node.attrib for key in LINKAGE): return None
    return {'itemName': name, 'identifier': node.get('linkageIdentifier', ''),
            'className': node.get('linkageClassName', ''),
            'flags': (1 if node.get('linkageExportForAS') == 'true' else 0) |
                     (2 if node.get('linkageImportForRS') == 'true' else 0)}


def import_project(xfl_root, old_assets, old_logs, destination, ffdec, movies):
    if (destination / 'Tactical.fla').exists():
        raise FileExistsError('Destination already contains an editable Tactical.fla; import into a new directory')
    destination.mkdir(parents=True, exist_ok=True)
    xfl = destination / 'Authoring'
    (xfl / 'LIBRARY').mkdir(parents=True, exist_ok=True)
    (xfl / 'bin').mkdir(exist_ok=True)
    docs = {movie: Document(movie, xfl_root, old_assets) for movie in movies}
    sharing = Sharing(docs)
    imported = defaultdict(set)
    origin_symbols = defaultdict(set)
    for movie in movies:
        log = json.loads((old_logs / movie / 'linkage.json').read_text())
        imported[movie] = {row['identifier'] for row in log['localized']}
        for row in log['localized']:
            name = docs[movie].symbol_files.get(row['file'])
            if name is not None:
                origin = row['url'].replace('\\', '/').rsplit('/', 1)[-1]
                origin_symbols[(origin, row['identifier'])].add((movie, name))
    # Keep publishing-wrapper variants as distinct visual graphs. The same
    # provider export's identical AS can still have one editable source file.
    providers = {movie.casefold(): movie for movie in movies}
    for (origin, identifier), candidates in origin_symbols.items():
        provider = providers.get(Path(origin).stem.casefold())
        if provider:
            for name, node in docs[provider].symbols.items():
                if node.get('linkageIdentifier') == identifier: candidates.add((provider, name))
    shared_script_routes = {}
    shared_script_proof = []
    visual_variants = []
    for (origin, identifier), candidates in sorted(origin_symbols.items()):
        versions = defaultdict(list)
        script_groups = defaultdict(list)
        for movie, name in sorted(candidates, key=lambda key: (movies.index(key[0]), key[1])):
            document = docs[movie]
            versions[document.fingerprint(name)].append({'movie': movie, 'item': name})
            for element in document.symbols[name].iter():
                if local(element) != 'script': continue
                include = INCLUDE.fullmatch(element.text or '')
                if include:
                    code = (document.assets / include[1]).read_bytes()
                    script_groups[(Path(include[1]).name, sha(code))].append((movie, include[1]))
        if len(versions) > 1:
            visual_variants.append({'originLibrary': origin, 'identifier': identifier,
                'reason': 'Original native/export and imported publishing wrappers differ; visual graphs retained separately',
                'variants': [{'fingerprint': fingerprint, 'sources': sources} for fingerprint, sources in versions.items()]})
        for (basename, digest), users in script_groups.items():
            if len({movie for movie, path in users}) < 2: continue
            provider_folder = re.sub(r'[^\w.-]+', '_', Path(origin).stem)
            export_folder = re.sub(r'[^\w.-]+', '_', identifier)
            target = Path('Common/Scripts') / provider_folder / export_folder / basename
            # Separate different recovered implementations even if their editor
            # filenames happen to coincide.
            if any(path == target.as_posix() for path in shared_script_routes.values()):
                target = target.with_name(target.stem + '_' + digest[:8] + target.suffix)
            for source_key in users: shared_script_routes.setdefault(source_key, target.as_posix())
            shared_script_proof.append({'originLibrary': origin, 'identifier': identifier, 'sha256': digest,
                'path': target.as_posix(), 'sources': [{'movie': movie, 'script': path} for movie, path in users]})
    groups = defaultdict(list)
    for movie, doc in docs.items():
        for name, node in doc.symbols.items():
            identifier = node.get('linkageIdentifier')
            if identifier and identifier in imported[movie]:
                groups[(identifier, doc.fingerprint(name))].append((movie, name))
    share_proof = []
    for (identifier, fingerprint), group in groups.items():
        if len({key[0] for key in group}) < 2: continue
        for target in group[1:]: sharing.merge(group[0], target, identifier)
        share_proof.append({'identifier': identifier, 'fingerprint': fingerprint,
                            'movies': [key[0] for key in group]})
    representatives = []
    for movie, doc in docs.items():
        for name in doc.order + list(doc.media):
            rep = sharing.find((movie, name))
            if rep not in representatives: representatives.append(rep)
    all_names = {}
    common_sequence = 0
    for rep in representatives:
        members = sharing.members[rep]
        if len(members) > 1:
            node = docs[rep[0]].node(rep[1])
            common_sequence += 1
            label = sorted(sharing.seeds.get(rep, []))[0] if sharing.seeds.get(rep) else node.get('linkageIdentifier')
            label = label or ('SharedPart_' + str(common_sequence).zfill(2))
            base = 'Common/Common_' + re.sub(r'[^\w.-]+', '_', label).strip('_')
        else:
            short_movie = rep[0][len('Tactical'):] if rep[0].startswith('Tactical') else rep[0]
            leaf = short_movie + '_' + docs[rep[0]].readable_name(rep[1])
            base = 'Components/' + short_movie + '/' + leaf
        candidate, count = base, 1
        while candidate in all_names.values():
            count += 1
            candidate = base + '_part_' + str(count)
        all_names[rep] = candidate
    mapped = {key: all_names[sharing.find(key)] for key in sharing.parent}
    script_targets = {}
    script_bytes = {}
    node_sources = {}
    def transform(node, movie, owner, common=False):
        node = copy.deepcopy(node)
        used = set()
        for element in node.iter():
            for key in ('libraryItemName', 'bitmapPath'):
                if key in element.attrib: element.set(key, mapped[(movie, element.get(key))])
            if local(element) == 'script':
                match = INCLUDE.fullmatch(element.text or '')
                if match:
                    old = match[1]
                    source = (docs[movie].assets / old)
                    data = source.read_bytes()
                    source_key = (movie, old)
                    if source_key in shared_script_routes:
                        target = Path(shared_script_routes[source_key])
                    elif common:
                        target = Path('Common/Scripts') / Path(old).name
                        while target.as_posix() in script_bytes and script_bytes[target.as_posix()] != data:
                            target = Path('Common/Scripts') / (Path(old).stem + '_' + sha(data)[:8] + '.as')
                    else:
                        target = Path('Scripts') / movie / Path(old).relative_to('scripts')
                    target_text = target.as_posix()
                    script_targets[source_key] = target_text
                    script_bytes[target_text] = data
                    used.add(target_text)
                    element.text = '#include "' + target_text + '"'
        return node, used
    landing_movie = 'TacticalHUD' if 'TacticalHUD' in docs else next(iter(docs))
    master = copy.deepcopy(docs[landing_movie].root)
    for child in list(master): master.remove(child)
    folders = ET.SubElement(master, tag('folders'))
    media = ET.SubElement(master, tag('media'))
    symbols = ET.SubElement(master, tag('symbols'))
    timelines = ET.SubElement(master, tag('timelines'))
    all_folder_names = set()
    item_counter = 0
    def item_id():
        nonlocal item_counter
        item_counter += 1
        return '6ae70000-' + format(item_counter, '08x')
    for rep in representatives:
        doc = docs[rep[0]]
        name = all_names[rep]
        if rep[1] not in doc.symbols: continue
        element, sources = transform(doc.node(rep[1]), rep[0], name, len(sharing.members[rep]) > 1)
        element.set('name', name)
        identifier = item_id()
        element.set('itemID', identifier)
        for key in LINKAGE: element.attrib.pop(key, None)
        timeline = element.find(tag('timeline'))
        if timeline is not None and len(timeline): timeline[0].set('name', name)
        target = xfl / 'LIBRARY' / (name + '.xml')
        target.parent.mkdir(parents=True, exist_ok=True)
        write_xml(element, target)
        ET.SubElement(symbols, tag('Include'), {'href': name + '.xml', 'itemID': identifier, 'loadImmediate': 'false'})
        node_sources[name] = sources
    for rep in representatives:
        doc = docs[rep[0]]
        if rep[1] not in doc.media: continue
        name = all_names[rep]
        element = copy.deepcopy(doc.media[rep[1]])
        element.set('name', name)
        element.set('itemID', item_id())
        for key in LINKAGE: element.attrib.pop(key, None)
        href = element.get('href')
        source = doc.path / 'LIBRARY' / href
        target = xfl / 'LIBRARY' / name
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, target)
        element.set('href', name)
        for field in ('bitmapDataHRef', 'soundDataHRef'):
            if element.get(field):
                source = doc.path / 'bin' / element.get(field)
                relative = 'media_' + str(len(media)).zfill(4) + '.dat'
                shutil.copyfile(source, xfl / 'bin' / relative)
                element.set(field, relative)
        media.append(element)
        node_sources[name] = set()
    original_report = Path(__file__).resolve().parents[2] / 'ra3-ui/conversion-report.json'
    original_rows = {Path(row['source']).stem: row for row in json.loads(original_report.read_text())['movies']}
    profiles = []
    for movie, doc in docs.items():
        entry = 'Roots/Root_' + movie
        scene_list = list(doc.root.find(tag('timelines')))
        if len(scene_list) != 1: raise ValueError('Pilot requires one source scene: ' + movie)
        timeline, sources = transform(scene_list[0], movie, entry)
        timeline.set('name', entry)
        identifier = item_id()
        element = ET.Element(tag('DOMSymbolItem'), {'name': entry, 'itemID': identifier})
        ET.SubElement(element, tag('timeline')).append(timeline)
        target = xfl / 'LIBRARY' / (entry + '.xml')
        target.parent.mkdir(parents=True, exist_ok=True)
        write_xml(element, target)
        ET.SubElement(symbols, tag('Include'), {'href': entry + '.xml', 'itemID': identifier, 'loadImmediate': 'false'})
        for name in doc.order + list(doc.media): sources.update(node_sources[mapped[(movie, name)]])
        color = doc.root.get('backgroundColor', '#ffffff').lstrip('#')
        red, green, blue = (int(color[i:i + 2], 16) for i in (0, 2, 4))
        linkages = []
        for name in doc.order + list(doc.media):
            master_name = mapped[(movie, name)]
            link = original_linkage(doc.node(name), master_name if name in doc.symbols else Path(master_name).name)
            if link: linkages.append(link)
        source_issues = original_rows.get(movie, {})
        profiles.append({'name': movie, 'sourceName': movie + '.fla', 'entrySymbol': entry, 'entryPath': entry,
                         'widthTwips': round(float(doc.root.get('width')) * 20),
                         'heightTwips': round(float(doc.root.get('height')) * 20),
                         'frameRate': float(doc.root.get('frameRate')),
                         'backgroundColor': 0xff000000 | red | (green << 8) | (blue << 16),
                         'libraryOrder': [mapped[(movie, name)] for name in doc.order],
                         'mediaOrder': [Path(mapped[(movie, name)]).name for name in sorted(doc.media)],
                         'linkages': linkages, 'sources': sorted(sources),
                         'originalSourceIssues': {'staticLibraryDependencyState': source_issues.get('dependencyClosure', 'unknown'),
                            'missingImports': source_issues.get('missingOriginalImports', []),
                            'missingExports': source_issues.get('unresolvedImportedSymbols', [])}})
    # The main stage is a visual editing landing page. Publishing uses the
    # selected Roots/<movie> timeline directly, never this wrapper instance.
    preview = ET.SubElement(timelines, tag('DOMTimeline'), {'name': 'Tactical editing workspace'})
    layers = ET.SubElement(preview, tag('layers'))
    layer = ET.SubElement(layers, tag('DOMLayer'), {'name': 'Edit Roots in Library; publish project.json'})
    frames = ET.SubElement(layer, tag('frames'))
    frame = ET.SubElement(frames, tag('DOMFrame'), {'index': '0', 'keyMode': '9728'})
    elements = ET.SubElement(frame, tag('elements'))
    landing_profile = next(profile for profile in profiles if profile['name'] == landing_movie)
    instance = ET.SubElement(elements, tag('DOMSymbolInstance'), {'libraryItemName': landing_profile['entryPath']})
    ET.SubElement(ET.SubElement(instance, tag('matrix')), tag('Matrix'))
    for name in list(all_names.values()) + [p['entryPath'] for p in profiles]:
        parent = Path(name).parent
        while parent.as_posix() != '.':
            all_folder_names.add(parent.as_posix())
            parent = parent.parent
    for name in sorted(all_folder_names): ET.SubElement(folders, tag('DOMFolderItem'), {'name': name, 'itemID': item_id(), 'isExpanded': 'false'})
    master.set('currentTimeline', '1')
    write_xml(master, xfl / 'DOMDocument.xml')
    (xfl / 'Tactical.xfl').write_text('PROXY-CS5', encoding='ascii')
    for relative, data in script_bytes.items():
        path = destination / relative
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(data)
    definition = {'formatVersion': 1, 'masterFile': 'Tactical.fla', 'sourceRoot': '.',
                  'editing': {'landingMovie': landing_movie, 'publishFrom': 'Each configured Roots library timeline',
                              'masterIsAuthoritative': True, 'authoringSnapshotIsBuildInput': False}, 'movies': profiles}
    (destination / 'project.json').write_text(json.dumps(definition, indent=2) + '\n')
    common_items = [{'itemName': all_names[rep], 'sources': [{'movie': movie, 'item': key[1]}
        for movie, key in sharing.members[rep].items()], 'scripts': sorted(node_sources.get(all_names[rep], []))}
        for rep in representatives if len(sharing.members[rep]) > 1]
    (destination / 'import-report.json').write_text(json.dumps({'sourceMovies': movies, 'sharedGraphs': share_proof,
        'sourceSymbolCount': sum(len(d.symbols) for d in docs.values()),
        'masterSymbolCount': len(symbols), 'sharedDescriptorGroups': sum(len(sharing.members[r]) > 1 for r in representatives),
        'commonItems': common_items, 'sharedScripts': shared_script_proof, 'retainedVisualVariants': visual_variants,
        'scriptFiles': len(script_bytes), 'originalAssetsModified': False}, indent=2) + '\n')
    with (destination / 'import.log').open('wb') as output:
        subprocess.run(['java', '-Xmx2048m', '-jar', str(ffdec / 'lib/flacomdoc.jar'), 'convert', '--format', 'MX2004',
                        str(xfl / 'Tactical.xfl'), str(destination / 'Tactical.fla')], stdout=output, stderr=subprocess.STDOUT, check=True)
    return definition


def movie_dependencies(assets, initial):
    """Resolve only literal loadMovie targets, preserving dynamic calls as data."""
    available = {p.name.casefold(): p.name for p in assets.iterdir() if p.is_dir()}
    movies = list(initial)
    edges, missing, dynamic = [], [], []
    # AS1/2 string literals are tokenized before punctuation/call recognition;
    # quoted strings or comments containing fake loadMovie text are not calls.
    lexer = re.compile(r'(?P<space>\s+)|(?P<comment>//[^\r\n]*|/\*[\s\S]*?\*/)|'
                       r'(?P<string>"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\')|'
                       r'(?P<identifier>[A-Za-z_$][\w$]*)|(?P<symbol>.)')
    import ast
    for movie in movies:
        for script in sorted((assets / movie).rglob('*.as')):
            code = script.read_text(encoding='utf-8-sig')
            tokens = [(match.lastgroup, match.group(), match.start()) for match in lexer.finditer(code)
                      if match.lastgroup not in {'space', 'comment'}]
            for index in range(len(tokens) - 2):
                if tokens[index][0:2] != ('identifier', 'loadMovie') or tokens[index + 1][1] != '(': continue
                argument = tokens[index + 2]
                location = {'movie': movie, 'script': script.relative_to(assets).as_posix(),
                            'line': code.count('\n', 0, tokens[index][2]) + 1}
                if argument[0] != 'string' or index + 3 >= len(tokens) or tokens[index + 3][1] not in {')', ','}:
                    dynamic.append(location)
                    continue
                literal = ast.literal_eval(argument[1])
                if not literal.lower().endswith('.swf'):
                    missing.append(dict(location, url=literal, reason='Non-SWF literal target'))
                    continue
                basename = literal.replace('\\', '/').rsplit('/', 1)[-1][:-4]
                target = available.get(basename.casefold())
                if target is None:
                    missing.append(dict(location, url=literal, reason='Movie absent from original pack'))
                    continue
                edges.append(dict(location, url=literal, target=target))
                if target not in movies: movies.append(target)
    return movies, {'literalLoads': edges, 'missingLiteralLoads': missing, 'dynamicLoadSites': dynamic}


def create_import_seed(original, ffdec, work, movies):
    """Rebuild selected normalized XFL locally using the pinned existing tools."""
    sys.dont_write_bytecode = True
    sys.path.insert(0, str(original / 'tools'))
    from convert import convert, import_urls, run
    source = original / 'source/ra3ui'
    files = sorted(source.rglob('*.swf'))
    names = {path.name.casefold(): path for path in files}
    graph = {path: import_urls(path) for path in files}
    mappings = {url: names[url.replace('\\', '/').rsplit('/', 1)[-1].casefold()]
        for urls in graph.values() for url in urls if url.replace('\\', '/').rsplit('/', 1)[-1].casefold() in names}
    classes = work / 'classes'
    classes.mkdir(parents=True, exist_ok=True)
    import os
    classpath = os.pathsep.join([str(ffdec / 'ffdec.jar'), str(ffdec / 'lib/*')])
    run(['java', '-m', 'jdk.compiler/com.sun.tools.javac.Main', '-cp', classpath, '-d', str(classes),
         str(original / 'tools/LegacyXflExport.java')], work / 'javac.log')
    from concurrent.futures import ThreadPoolExecutor
    destination = work / 'imported'
    conversion = work / 'conversion'
    def export(movie):
        result = convert(source / movie / (movie + '.swf'), source, destination, ffdec / 'ffdec.jar',
                         classes, conversion, mappings, graph)
        if result['status'] != 'exported': raise ValueError('Could not import ' + movie + ': ' + result['error'])
        return result
    with ThreadPoolExecutor(max_workers=4) as pool: rows = list(pool.map(export, movies))
    (work / 'seed-report.json').write_text(json.dumps(rows, indent=2) + '\n')
    return conversion / 'xfl', destination / 'converted', destination / 'validation/conversion-logs'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    original = Path(__file__).resolve().parents[2] / 'ra3-ui'
    parser.add_argument('--xfl-root', type=Path, help='Reuse a read-only normalized per-movie XFL directory')
    parser.add_argument('--as-root', type=Path, default=original / 'converted')
    parser.add_argument('--logs-root', type=Path, default=original / 'validation/conversion-logs')
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--ffdec', type=Path, required=True)
    parser.add_argument('--movies', nargs='+', default=MOVIES)
    parser.add_argument('--group', choices=['pilot', 'tactical'], default='pilot')
    parser.add_argument('--work', type=Path, default=Path('work/tactical-import'))
    args = parser.parse_args()
    immutable = (Path(__file__).resolve().parents[2] / 'ra3-ui').resolve()
    destination = args.output.resolve()
    if destination == immutable or immutable in destination.parents:
        parser.error('The recovered resource tree is immutable')
    if args.work.resolve() == immutable or immutable in args.work.resolve().parents:
        parser.error('Intermediate files cannot be written into the recovered resource tree')
    if (destination / 'Tactical.fla').exists():
        parser.error('An editable Tactical.fla already exists; import into a new directory')
    movies = args.movies
    dependency_report = None
    if args.group == 'tactical':
        movies = sorted(p.name for p in args.as_root.iterdir() if p.is_dir() and p.name.startswith('Tactical'))
        movies, dependency_report = movie_dependencies(args.as_root, movies)
    xfl_root, as_root, logs_root = args.xfl_root, args.as_root, args.logs_root
    if xfl_root is None:
        xfl_root, as_root, logs_root = create_import_seed(original, args.ffdec.resolve(), args.work.resolve(), movies)
    result = import_project(xfl_root.resolve(), as_root.resolve(), logs_root.resolve(), destination, args.ffdec.resolve(), movies)
    if dependency_report is not None:
        (destination / 'runtime-dependencies.json').write_text(json.dumps(dependency_report, indent=2) + '\n')
    print(json.dumps({'movies': len(result['movies']), 'master': str(destination / 'Tactical.fla')}))


if __name__ == '__main__':
    main()
