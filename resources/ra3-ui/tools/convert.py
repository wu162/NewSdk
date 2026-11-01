#!/usr/bin/env python3
"""Offline SWF -> binary MX2004 FLA with editable AS includes; requires Java 21."""
import argparse
import concurrent.futures
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import struct
import subprocess
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape
import zlib
from inline_xfl import inline_xfl
from normalize_xfl import normalize_xfl
from normalize_classes import normalize as normalize_classes
from meaningful_names import rename_scripts


def sha256(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def import_urls(path):
    data = path.read_bytes()
    if data[:3] == b'CWS':
        data = zlib.decompress(data[8:])
    elif data[:3] == b'FWS':
        data = data[8:]
    else:
        raise ValueError('Unsupported SWF signature: ' + str(path))
    pos = (5 + 4 * (data[0] >> 3) + 7) // 8 + 4
    urls = []
    while pos < len(data):
        header = struct.unpack_from('<H', data, pos)[0]
        pos += 2
        length, code = header & 63, header >> 6
        if length == 63:
            length = struct.unpack_from('<I', data, pos)[0]
            pos += 4
        if code in (57, 71):
            urls.append(data[pos:pos + length].split(b'\0')[0].decode('utf-8'))
        pos += length
        if code == 0:
            break
    return urls


def timeline_metadata(path):
    data = path.read_bytes()
    version = data[3]
    data = zlib.decompress(data[8:]) if data[:3] == b'CWS' else data[8:]
    offset = (5 + 4 * (data[0] >> 3) + 7) // 8
    return {'version': version, 'framesPerSecond': struct.unpack_from('<H', data, offset)[0] / 256,
            'mainTimelineFrames': struct.unpack_from('<H', data, offset + 2)[0]}


def run(command, log):
    result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, timeout=600)
    log.write_bytes(result.stdout)
    if result.returncode:
        raise RuntimeError(f'{log.stem} exit code {result.returncode}')
    return result.stdout.decode('utf-8', errors='replace')


def externalize_scripts(xfl, output, destination):
    mapping = []
    for xml in sorted(xfl.rglob('*.xml')):
        ordinal = 0
        def replace(match):
            nonlocal ordinal
            ordinal += 1
            raw = ET.fromstring(match.group(0)).text or ''
            if not raw.strip():
                return match.group(0)
            # FFDec emits this spelling for the AVM1 Construct movieclip event.
            code, normalized = re.subn(r'(?m)^([ \t]*)on\s*\(\s*construct\s*\)(?=\s*\{)',
                                       r'\1onClipEvent(construct)', raw)
            relative = Path('scripts') / xml.relative_to(xfl).with_suffix('') / f'{ordinal:04}.as'
            path = output / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            path.write_text(code, encoding='utf-8')
            mapping.append({'path': path.relative_to(destination).as_posix(),
                            'xflXml': xml.relative_to(xfl).as_posix(), 'scriptOrdinal': ordinal,
                            'sha256': sha256(path), 'originalCodeSha256': hashlib.sha256(raw.encode()).hexdigest(),
                            'normalizedConstructEvents': normalized})
            return '<script>' + escape('#include "' + relative.as_posix() + '"') + '</script>'
        text = xml.read_text(encoding='utf-8-sig')
        if 'tweenType="motion object"' in text or '<MotionObject' in text:
            raise RuntimeError('Motion-object animation would be lost by legacy FLA conversion')
        changed = re.sub(r'<script(?:\s[^>]*)?>[\s\S]*?</script>', replace, text)
        if changed != text:
            ET.fromstring(changed)
            xml.write_text(changed, encoding='utf-8')
    for script in xfl.rglob('*.as'):
        target = output / script.relative_to(xfl)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(script, target)
        mapping.append({'path': target.relative_to(destination).as_posix(), 'sha256': sha256(target),
                        'xflClassFile': script.relative_to(xfl).as_posix()})
    return mapping


def convert(source, root, destination, jar, classes, work, mappings, graph):
    relative = source.relative_to(root)
    movie = relative.parent if relative.parent.name.casefold() == source.stem.casefold() else relative.with_suffix('')
    output = destination / 'converted' / movie
    logs = destination / 'validation' / 'conversion-logs' / movie
    stage = work / 'xfl' / movie
    for folder in (output, logs, stage):
        folder.mkdir(parents=True, exist_ok=True)
    row = {'source': relative.as_posix(), 'sourceSha256': sha256(source), 'sourceSwf': timeline_metadata(source)}
    visited, dependencies, missing = set(), set(), set()
    def visit(path):
        if path in visited:
            return
        visited.add(path)
        for url in graph[path]:
            if url in mappings:
                target = mappings[url]
                dependencies.add(target.relative_to(root).as_posix())
                visit(target)
            else:
                missing.add(url)
    visit(source)
    row.update(sharedDependencies=sorted(dependencies), missingOriginalImports=sorted(missing),
               dependencyClosure='missing-original-libraries' if missing else 'closed')
    try:
        cp = os.pathsep.join([str(classes), str(jar), str(jar.parent / 'lib' / '*')])
        command = ['java', '-Xmx1536m', '-Djava.awt.headless=true', '-cp', cp, 'LegacyXflExport',
                   '-onerror', 'abort', '-importAssets', 'yes,local',
                   '-config', 'skipDetectionOfUninitializedClassFields=false']
        for url, target in mappings.items():
            command += ['-changeImport', url, '../' + target.relative_to(root).as_posix()]
        export_log = run(command + ['-format', 'xfl:mx2004', '-export', 'xfl', str(stage), str(source)], logs / 'xfl.log')
        xfl = stage / source.stem
        if not (xfl / 'DOMDocument.xml').is_file():
            raise RuntimeError('No DOMDocument.xml from legacy XFL export')
        # The resolver maps URLs for loading; XFL retains the original URL text.
        known_urls = set(mappings) | {'../' + p.relative_to(root).as_posix() for p in mappings.values()}
        linkage = inline_xfl(xfl, known_urls)
        (logs / 'linkage.json').write_text(json.dumps(linkage, ensure_ascii=False, indent=2) + '\n')
        normalizations = normalize_xfl(xfl, source)
        (logs / 'normalization.json').write_text(json.dumps(normalizations, ensure_ascii=False, indent=2) + '\n')
        scripts = externalize_scripts(xfl, output, destination)
        needs_classes = any('linkageClassName=' in xml.read_text(encoding='utf-8-sig') for xml in xfl.rglob('*.xml'))
        needs_classes = needs_classes or any('#initclip' in (destination / item['path']).read_text(encoding='utf-8-sig') and
            re.search(r'\b(?:class|interface)\s+[\w.$]+', (destination / item['path']).read_text(encoding='utf-8-sig')) for item in scripts)
        class_report = None
        if needs_classes:
            packages = stage / 'raw-default'
            skipped = stage / 'raw-skipfields'
            run(command + ['-config', 'skipDetectionOfUninitializedClassFields=false', '-export', 'script',
                           str(packages), str(source)], logs / 'classes-default.log')
            run(command + ['-config', 'skipDetectionOfUninitializedClassFields=true', '-export', 'script',
                           str(skipped), str(source)], logs / 'classes-skipfields.log')
            class_report = normalize_classes(output, packages / 'scripts/__Packages', skipped / 'scripts/__Packages')
            (logs / 'classes.json').write_text(json.dumps(class_report, ensure_ascii=False, indent=2) + '\n')
            existing = {item['path'] for item in scripts}
            for script in sorted(output.rglob('*.as')):
                path = script.relative_to(destination).as_posix()
                if path not in existing:
                    scripts.append({'path': path, 'kind': 'recovered-AS2-class-or-interface', 'sha256': sha256(script)})
            for item in scripts:
                item['sha256'] = sha256(destination / item['path'])
        naming = rename_scripts(xfl, output, destination)
        (logs / 'naming.json').write_text(json.dumps(naming, ensure_ascii=False, indent=2) + '\n')
        renamed = {item['oldPath']: item for item in naming}
        for item in scripts:
            if item['path'] in renamed:
                name = renamed[item['path']]
                item.update(path=name['newPath'], semanticName={key: name[key] for key in
                    ('owner', 'ownerBasis', 'frameNumber', 'frameLabel', 'event', 'action', 'actionBasis')})
        fla = output / (source.stem + '.fla')
        convert_log = run(['java', '-Xmx1536m', '-jar', str(jar.parent / 'lib' / 'flacomdoc.jar'), 'convert',
                           '--format', 'MX2004', str(xfl / (source.stem + '.xfl')), str(fla)], logs / 'fla.log')
        if not fla.is_file() or fla.read_bytes()[:8] != bytes.fromhex('d0cf11e0a1b11ae1'):
            raise RuntimeError('Exporter did not produce a binary FLA')
        warnings = [line for line in (export_log + '\n' + convert_log).splitlines()
                    if re.search(r'warning|unsupported|ignored|cannot load|not supported', line, re.I)]
        missing_symbols = []
        for line in warnings:
            match = re.search(r'Imported character from URL (.*?) not found: imported name = (.*?), imported id = (\d+)', line)
            if match:
                missing_symbols.append({'url': match[1], 'name': match[2], 'id': int(match[3])})
        row.update(status='exported', fla=fla.relative_to(destination).as_posix(), flaSha256=sha256(fla),
                   flaBytes=fla.stat().st_size, inlinedLibraryItems=linkage['localizedCount'],
                   unresolvedLibraryItems=linkage['unresolvedCount'], scriptCount=len(scripts), scripts=scripts, warnings=warnings,
                   unresolvedImportedSymbols=missing_symbols,
                   dependencyClosure='unresolved-source-dependencies' if missing or missing_symbols else 'closed',
                   recoveredClassCount=class_report['classCount'] if class_report else 0)
        (output / 'script-map.json').write_text(json.dumps(scripts, ensure_ascii=False, indent=2) + '\n')
    except Exception as error:
        row.update(status='failed', error=str(error))
    return row


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--source', type=Path, required=True)
    parser.add_argument('--destination', type=Path, required=True)
    parser.add_argument('--ffdec', type=Path, required=True, help='Local ffdec.jar; no downloads')
    parser.add_argument('--work', type=Path, required=True, help='Intermediate XFL and compiled wrapper')
    parser.add_argument('--workers', type=int, default=4)
    args = parser.parse_args()
    root, destination, jar, work = (p.resolve() for p in (args.source, args.destination, args.ffdec, args.work))
    files = sorted(p for p in root.rglob('*') if p.is_file() and p.suffix.casefold() == '.swf')
    if not files or not jar.is_file():
        parser.error('Source SWFs and a local FFDec JAR are required')
    names = {p.name.casefold(): p for p in files}
    if len(names) != len(files):
        parser.error('Duplicate movie basenames need an explicit dependency map')
    graph = {p: import_urls(p) for p in files}
    mappings = {url: names[url.replace('\\', '/').rsplit('/', 1)[-1].casefold()]
                for urls in graph.values() for url in urls
                if url.replace('\\', '/').rsplit('/', 1)[-1].casefold() in names}
    classes = work / 'classes'
    classes.mkdir(parents=True, exist_ok=True)
    cp = os.pathsep.join([str(jar), str(jar.parent / 'lib' / '*')])
    run(['java', '-m', 'jdk.compiler/com.sun.tools.javac.Main', '-cp', cp, '-d', str(classes),
         str(Path(__file__).with_name('LegacyXflExport.java'))], work / 'javac.log')
    results = []
    with concurrent.futures.ThreadPoolExecutor(max_workers=args.workers) as executor:
        jobs = [executor.submit(convert, p, root, destination, jar, classes, work, mappings, graph) for p in files]
        for job in concurrent.futures.as_completed(jobs):
            row = job.result()
            results.append(row)
            print(f'{len(results)}/{len(files)} {row["status"]}: {row["source"]}' +
                  (' ' + row['error'] if row['status'] == 'failed' else ''), flush=True)
    report = {'format': 'binary Flash MX 2004 FLA (CFB, Contents 0x38)', 'ffdecVersion': '26.4.0',
              'totalMovies': len(files), 'exported': sum(r['status'] == 'exported' for r in results),
              'failed': sum(r['status'] == 'failed' for r in results), 'movies': sorted(results, key=lambda r: r['source'])}
    report.update(actionScriptFiles=sum(r.get('scriptCount', 0) for r in results),
                  dependencyClosedMovies=sum(r['dependencyClosure'] == 'closed' for r in results),
                  sourceDependencyIncompleteMovies=sum(r['dependencyClosure'] != 'closed' for r in results))
    (destination / 'conversion-report.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({k: v for k, v in report.items() if k != 'movies'}))
    return int(bool(report['failed']))


if __name__ == '__main__':
    raise SystemExit(main())
