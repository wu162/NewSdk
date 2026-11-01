"""Name recovered timeline AS files from their real XFL owner and behavior."""
from collections import defaultdict
import hashlib
from pathlib import Path
import re
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape

# Include empty self-closing nodes in ordinal counting, just as ElementTree does.
# Put this alternative first so a self-closing tag cannot consume the next body.
SCRIPT = re.compile(r'<script(?:\s[^>]*)?/>|<script(?:\s[^>]*)?>[\s\S]*?</script>')
INCLUDE = re.compile(r'^\s*#include\s+["\']([^"\']+)["\']\s*;?\s*$')
GENERIC = re.compile(r'^(?:symbol|scene|instance|layer)[ _-]*\d+$', re.I)


def local(element):
    return element.tag.rsplit('}', 1)[-1]


def readable(value):
    value = (value or '').strip()
    return bool(value and not GENERIC.fullmatch(value) and not value.isdigit()
                and value not in {'_root', '_parent', 'this', 'Script Layer', 'undefined'})


def token(value, limit=48):
    value = re.sub(r'\.(?:png|jpe?g|tga|swf)$', '', value or '', flags=re.I)
    value = re.sub(r'[^\w$]+', '_', value, flags=re.UNICODE).strip('_')
    value = re.sub(r'_+', '_', value)
    value = (value or 'Timeline')[:limit].rstrip('_')
    if value.upper() in {'CON', 'PRN', 'AUX', 'NUL', *(f'COM{i}' for i in range(1, 10)), *(f'LPT{i}' for i in range(1, 10))}:
        value = (value + '_component')[:limit]
    return value


def action_name(code):
    # Retain actual identifiers rather than assigning guessed UI behavior.
    # A debug string such as "playSound()" is not an actual function call.
    plain = re.sub(r'"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|//[^\r\n]*|/\*[\s\S]*?\*/',
                   lambda match: ''.join('\n' if char == '\n' else ' ' for char in match[0]), code)
    event = re.search(r'\bon(?:ClipEvent)?\s*\(\s*([^)]*)\)', plain)
    if event:
        value = token(code[event.start(1):event.end(1)], 48)
        return value, 'event-handler:' + code[event.start():event.end()], value
    imported = re.search(r'\bimport\s+([\w.$]+)\s*;', plain)
    if imported:
        return 'load_' + token(imported[1]), 'class-import:' + imported[1], ''
    functions = re.findall(r'\bfunction\s+([\w$]+)\s*\(', plain)
    if functions:
        return 'define_' + token(functions[0]), 'function-declaration:' + functions[0], ''
    assigned_handler = re.search(r'\b(on[A-Z]\w*)\s*=\s*function\s*\(', plain)
    if assigned_handler:
        return token(assigned_handler[1]), 'assigned-event:' + assigned_handler[1], assigned_handler[1]
    calls = re.findall(r'(?<![\w$])(?:[\w$]+\.)*([A-Za-z_$][\w$]*)\s*\(', plain)
    calls = [c for c in calls if c not in {'if', 'for', 'while', 'switch', 'function', 'catch', 'with'}]
    if calls:
        meaningful = next((c for c in calls if c not in {'trace', 'stop', 'play'}), calls[0])
        return token(meaningful), 'function-call:' + meaningful, ''
    assigned = re.search(r'(?:^|[;\n])\s*(?:this\.)?([A-Za-z_$][\w$]*)\s*=(?!=)', plain)
    if assigned and not re.fullmatch(r'_loc\d+_', assigned[1]):
        return 'set_' + token(assigned[1]), 'property-assignment:' + assigned[1], ''
    return 'timelineActions', 'timeline-actions-without-named-function', ''


def rename_scripts(xfl_dir, movie_output, resource_root):
    """Move included AS files and update XFL includes, preserving AS bytes.

    Returns one metadata row per renamed file. Root AS2 class/interface files keep
    their real qualified names. FLA must be regenerated after changing includes.
    """
    xfl_dir, movie_output, resource_root = map(lambda p: Path(p).resolve(),
                                               (xfl_dir, movie_output, resource_root))
    movie = movie_output.name
    documents = []
    aliases = defaultdict(set)
    owners = {}
    for path in sorted(xfl_dir.rglob('*.xml')):
        text = path.read_text(encoding='utf-8-sig')
        root = ET.fromstring(text)
        parents = {child: parent for parent in root.iter() for child in parent}
        documents.append((path, text, root, parents))
        if local(root) == 'DOMSymbolItem':
            owners[root.get('name', '')] = root
        for element in root.iter():
            if local(element) in {'DOMSymbolInstance', 'DOMBitmapInstance'} and readable(element.get('name')):
                aliases[element.get('libraryItemName', '')].add(element.get('name'))

    def owner(element, root, parents):
        # Instance handlers belong to that exact named instance, when available.
        current = parents.get(element)
        while current is not None:
            if local(current) in {'DOMSymbolInstance', 'DOMBitmapInstance'}:
                if readable(current.get('name')):
                    return current.get('name'), 'instance-name'
                target = owners.get(current.get('libraryItemName', ''))
                if target is not None:
                    for field in ('linkageIdentifier', 'linkageClassName'):
                        if readable(target.get(field)):
                            return target.get(field), 'instance-library-' + field
            current = parents.get(current)
        for field in ('linkageIdentifier', 'linkageClassName'):
            if readable(root.get(field)):
                return root.get(field), field
        for timeline in root.iter():
            if local(timeline) == 'DOMTimeline' and readable(timeline.get('name')):
                return timeline.get('name'), 'timeline-name'
        references = aliases.get(root.get('name', ''), set())
        if references:
            return sorted(references, key=lambda name: (len(name), name.casefold(), name))[0], 'referencing-instance-name'
        return movie, 'movie-name-fallback'

    plans = []
    xml_plans = defaultdict(dict)
    seen_sources = {}
    reserved = {}
    for path, text, root, parents in documents:
        scripts = [element for element in root.iter() if local(element) == 'script']
        for ordinal, script in enumerate(scripts, 1):
            include = INCLUDE.fullmatch(script.text or '')
            if not include:
                continue
            old_relative = Path(include[1].replace('\\', '/'))
            source = (movie_output / old_relative).resolve()
            source.relative_to(movie_output)
            if not source.is_file():
                raise FileNotFoundError('Missing included AS: ' + str(source))
            if source in seen_sources:
                xml_plans[path][ordinal] = seen_sources[source]['relativeNewPath']
                continue
            data = source.read_bytes()
            code = data.decode('utf-8-sig')
            owner_name, owner_basis = owner(script, root, parents)
            frame, timeline, current = None, None, parents.get(script)
            while current is not None:
                if frame is None and local(current) == 'DOMFrame':
                    frame = current
                if local(current) == 'DOMTimeline':
                    timeline = current
                    break
                current = parents.get(current)
            index = int(frame.get('index', '0')) if frame is not None else 0
            labels = [(int(element.get('index', '0')), element.get('name'))
                      for element in (timeline.iter() if timeline is not None else [])
                      if local(element) == 'DOMFrame' and readable(element.get('name'))
                      and int(element.get('index', '0')) <= index]
            label = sorted(labels, key=lambda item: item[0])[-1][1] if labels else ''
            action, action_basis, event = action_name(code)
            owner_token = token(owner_name, 32)
            label_part = ('_label_' + token(label, 32)) if label else ''
            # Put the action ahead of the optional label so truncation preserves
            # behavior. Bound paths for Windows and legacy Flash tooling.
            basename = token(f'{owner_token}_frame_{index + 1:04}_{action}{label_part}', 96)
            relative = Path('scripts') / owner_token / (basename + '.as')
            occurrence = 1
            while relative.as_posix().casefold() in reserved and reserved[relative.as_posix().casefold()] != source:
                occurrence += 1
                relative = Path('scripts') / owner_token / (basename + f'_occurrence_{occurrence:02}.as')
            reserved[relative.as_posix().casefold()] = source
            target = movie_output / relative
            if target.exists() and target.resolve() != source:
                raise FileExistsError('Meaningful AS target already exists: ' + str(target))
            row = {'oldPath': source.relative_to(resource_root).as_posix(),
                   'newPath': target.relative_to(resource_root).as_posix(),
                   'relativeOldPath': old_relative.as_posix(), 'relativeNewPath': relative.as_posix(),
                   'xflXml': path.relative_to(xfl_dir).as_posix(), 'scriptOrdinal': ordinal,
                   'frameNumber': index + 1, 'frameLabel': label, 'owner': owner_name,
                   'ownerBasis': owner_basis, 'event': event, 'action': action,
                   'actionBasis': action_basis, 'occurrence': occurrence,
                   'sha256': hashlib.sha256(data).hexdigest()}
            plans.append((source, target, row))
            seen_sources[source] = row
            xml_plans[path][ordinal] = relative.as_posix()
    for source, target, row in plans:
        if source != target:
            target.parent.mkdir(parents=True, exist_ok=True)
            source.rename(target)
    for path, text, root, parents in documents:
        ordinal = 0
        def replace(match):
            nonlocal ordinal
            ordinal += 1
            if ordinal not in xml_plans[path]:
                return match[0]
            return '<script>' + escape('#include "' + xml_plans[path][ordinal] + '"') + '</script>'
        changed = SCRIPT.sub(replace, text)
        if ordinal != sum(local(element) == 'script' for element in root.iter()):
            raise ValueError('XFL script mapping mismatch: ' + str(path))
        if changed != text:
            ET.fromstring(changed)
            path.write_text(changed, encoding='utf-8')
    for directory in sorted((p for p in (movie_output / 'scripts').rglob('*') if p.is_dir()),
                            key=lambda p: len(p.parts), reverse=True):
        if not any(directory.iterdir()):
            directory.rmdir()
    return [row for source, target, row in plans]


if __name__ == '__main__':
    import argparse
    import json
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('xfl_directory', type=Path)
    parser.add_argument('movie_output', type=Path)
    parser.add_argument('resource_root', type=Path)
    parser.add_argument('--report', type=Path, required=True)
    args = parser.parse_args()
    report = rename_scripts(args.xfl_directory, args.movie_output, args.resource_root)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'renamedScripts': len(report)}))
