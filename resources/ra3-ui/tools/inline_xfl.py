#!/usr/bin/env python3
"""Localize only successfully resolved runtime imports in FFDec's XFL output.

The original timelines, media, symbol names and script text are preserved.
Unresolved external links remain external and are explicitly reported.
"""
import argparse
import html
import json
from pathlib import Path
import re
import xml.etree.ElementTree as ET


# Consume complete quoted attributes, including '>' inside an attribute value.
OPEN_TAG = re.compile(r'<(?![!?/])(?:[^>"\']|"[^"]*"|\'[^\']*\')*>')
ATTRIBUTE = re.compile(r'([\w:]+)\s*=\s*(["\'])(.*?)\2', re.DOTALL)


def inline_xfl(root, resolved_urls):
    """Return changed linkage count and unchanged unresolved URL inventory.

    resolved_urls must identify imports the SWF loader actually resolved; merely
    stripping every linkage attribute could hide missing symbols from a pack.
    """
    localized = []
    unresolved = []
    resolved_urls = set(resolved_urls)
    for path in sorted(Path(root).rglob('*.xml')):
        text = path.read_text(encoding='utf-8-sig')
        ET.fromstring(text)  # Refuse malformed generated XML before mutation.

        def replace(match):
            tag = match.group(0)
            attributes = {m.group(1): html.unescape(m.group(3)) for m in ATTRIBUTE.finditer(tag)}
            if attributes.get('linkageImportForRS') != 'true':
                return tag
            url = attributes.get('linkageURL', '')
            row = {'file': path.relative_to(root).as_posix(), 'url': url,
                   'identifier': attributes.get('linkageIdentifier', ''),
                   'name': attributes.get('name', '')}
            if url not in resolved_urls:
                unresolved.append(row)
                return tag
            localized.append(row)
            tag = re.sub(r'\s+linkageImportForRS\s*=\s*(["\']).*?\1', '', tag)
            tag = re.sub(r'\s+linkageURL\s*=\s*(["\']).*?\1', '', tag)
            if 'linkageExportForAS' in attributes:
                tag = re.sub(r'linkageExportForAS\s*=\s*(["\']).*?\1', 'linkageExportForAS="true"', tag)
            else:
                end = -2 if tag.endswith('/>') else -1
                tag = tag[:end] + ' linkageExportForAS="true"' + tag[end:]
            return tag

        changed = OPEN_TAG.sub(replace, text)
        if changed != text:
            ET.fromstring(changed)
            path.write_text(changed, encoding='utf-8')
    return {'localizedCount': len(localized), 'localized': localized,
            'unresolvedCount': len(unresolved), 'unresolved': unresolved}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('xfl_directory', type=Path)
    parser.add_argument('--resolved-url', action='append', default=[])
    parser.add_argument('--report', type=Path)
    args = parser.parse_args()
    result = inline_xfl(args.xfl_directory, args.resolved_url)
    rendered = json.dumps(result, ensure_ascii=False, indent=2) + '\n'
    if args.report:
        args.report.write_text(rendered)
    print(rendered, end='')


if __name__ == '__main__':
    main()
