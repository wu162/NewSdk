#!/usr/bin/env python3
"""Check the original six BuildQueue load targets in a new Tactical project."""
import argparse
import json
from pathlib import Path
import re

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('project', type=Path)
parser.add_argument('old_converted', type=Path)
parser.add_argument('output', type=Path)
args = parser.parse_args()
project = json.loads(args.project.read_text(encoding='utf-8'))
source_root = (args.project.parent / project['sourceRoot']).resolve()
movie = next(m for m in project['movies'] if m['name'] == 'TacticalHUDMainCommandBar')
old_folder = args.old_converted / movie['name']
pattern = re.compile(r'\b([A-Za-z_$][\w$]*)\.loadMovie\(\s*["\']TacticalHUDBuildQueuePage\.swf["\']\s*\)')


def uses(files):
    result = []
    for path in sorted(files):
        text = path.read_text(encoding='utf-8-sig')
        for match in pattern.finditer(text):
            result.append({'target': match[1], 'file': str(path),
                           'line': text.count('\n', 0, match.start()) + 1})
    return result


old = uses(old_folder.rglob('*.as'))
current = uses(source_root / relative for relative in movie['sources'])
expected = ['mainStructure', 'otherStructure', 'infantry', 'vehicles', 'aircraft', 'navy']
passed = [row['target'] for row in old] == expected == [row['target'] for row in current]
report = {
    'movie': movie['name'], 'loadedMovie': 'TacticalHUDBuildQueuePage.swf',
    'expectedDistinctTargets': expected, 'oldUses': old, 'projectUses': current,
    'passed': passed,
    'scope': 'Static original/new AS loadMovie target identity; runtime state independence additionally relies on unchanged component output and has not been checked in-game.',
}
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(json.dumps({'movie': movie['name'], 'distinctTargets': len(set(row['target'] for row in current)), 'passed': passed}))
raise SystemExit(0 if passed else 1)
