#!/usr/bin/env python3
"""Select affected TextureCompiler groups; never turn probe/partial evidence into acceptance."""
import argparse
import fnmatch
import json
from functools import lru_cache
from pathlib import Path
import re
import subprocess


def git(root, *args, check=True):
    return subprocess.run(['git', '-C', str(root), *args], check=check, capture_output=True, text=True)


@lru_cache(maxsize=None)
def files(root, ref):
    return frozenset(git(root, 'ls-tree', '-r', '--name-only', ref).stdout.splitlines())


@lru_cache(maxsize=None)
def is_ancestor(root, anchor, head):
    return git(root, 'merge-base', '--is-ancestor', anchor, head, check=False).returncode == 0


@lru_cache(maxsize=None)
def historical_catalog(root, anchor):
    return git(root, 'show', anchor + ':recovery/catalog/TextureTestGroups.json', check=False)


@lru_cache(maxsize=None)
def changed_files(root, anchor, head):
    # select() resolves HEAD before reaching this cache. The repository and both
    # immutable commit IDs are keys; never cache a mutable branch/worktree diff.
    return frozenset(git(root, 'diff', '--name-only', anchor, head).stdout.splitlines())


@lru_cache(maxsize=None)
def dependency_files(root, patterns, ref):
    available = files(root, ref)
    # Match the same case-sensitive fnmatch language, but cross into the regex
    # engine once per file rather than once per pattern. Each translated arm
    # keeps fnmatch's complete-string anchor and DOTALL behaviour.
    matcher = re.compile('|'.join(fnmatch.translate(p) for p in patterns)) if patterns else None
    result = {p for p in available if matcher is not None and matcher.fullmatch(p)}
    # Explicit missing/deleted inputs remain part of the dependency set.
    result.update(p for p in patterns if not any(c in p for c in '*?['))
    return frozenset(result)


def dependencies(root, spec, ref):
    return dependency_files(root, tuple(spec['dependencies']), ref)


def successful(row, sdk=False):
    if row.get('status') != 'passed' or row.get('probeOnly') or not row.get('runId'):
        return False
    if sdk:
        modes = row.get('modes', [])
        return row.get('sdkValidated') is True and len(modes) == 3 and {
            m.get('mode') for m in modes} == {'Original', 'Recovered', 'NoOriginal'} and all(
                m.get('passed') is True and m.get('uniqueAssets',0)>0 and m.get('actualCalls',0)>=m.get('uniqueAssets',0) for m in modes) and next(m for m in modes if m['mode']=='NoOriginal').get('withoutOriginalTextureCompiler') is True
    modes = row.get('configurations', [])
    return len(modes) == 2 and {m.get('configuration') for m in modes} == {
        'Debug', 'Release'} and all(m.get('passed') is True for m in modes)


def select(root, groups, head='HEAD', requested=None, sdk=False):
    head=git(root,'rev-parse',head).stdout.strip()
    selected, reused, details = [], [], {}
    forced = set(requested or [])
    unknown = forced - groups.keys()
    if unknown:
        raise ValueError('Unknown groups: ' + ', '.join(sorted(unknown)))
    known = set()
    for spec in groups.values():
        if not spec.get('sdk'):
            known |= dependencies(root, spec, head)
    for name, spec in groups.items():
        if spec.get('manual_only') and name not in forced:
            details[name] = {'reason': 'evidence collection requires explicit request'}
            continue
        if spec.get('sdk') and not sdk and name not in forced:
            details[name] = {'reason': 'SDK not requested'}
            continue
        if name in forced:
            selected.append(name)
            details[name] = {'reason': 'explicit group request'}
            continue
        valid, reason = None, 'no complete successful ancestor evidence'
        candidates = []
        for path in root.glob(spec['evidence']):
            try:
                row = json.loads(path.read_text(encoding='utf-8-sig'))
                if successful(row, spec.get('sdk', False)):
                    candidates.append((int(row['runId']), path, row))
            except (OSError, ValueError, TypeError):
                continue
        for _, path, row in sorted(candidates, reverse=True):
            anchor = row.get('sourceCommit', '')
            if not re.fullmatch('[0-9a-fA-F]{40}', anchor):
                continue
            if not is_ancestor(root, anchor, head):
                continue
            old_spec=spec
            old_catalog=historical_catalog(root, anchor)
            if old_catalog.returncode==0:
                try:old_spec=json.loads(old_catalog.stdout)['groups'].get(name,spec)
                except (ValueError,KeyError,TypeError):
                    reason='unreadable historical dependency catalog'
                    continue
            inputs = dependencies(root, spec, head) | dependencies(root, old_spec, anchor)
            changed = changed_files(root, anchor, head)
            # New/unmapped implementation files fail safe. Mapping a new helper
            # must also update every actual consumer group, or all groups rerun.
            unmanaged = {p for p in changed if p.startswith(('src/TextureCompiler/Managed/',
                         'src/TextureCompiler/Native/')) and p.endswith(('.cs', '.cpp', '.h')) and p not in known}
            affected = changed & inputs
            tool_changes = {p for p in changed if p.startswith('SDK/tools/') and p.endswith('.dll')}
            if affected or unmanaged or tool_changes:
                reason = 'changed inputs: ' + ', '.join(sorted(affected | unmanaged | tool_changes))
                continue
            valid = {'sourceCommit': anchor, 'runId': str(row['runId']),
                     'receipt': path.relative_to(root).as_posix(), 'reason': 'unchanged complete successful inputs'}
            break
        if valid:
            reused.append(name)
            details[name] = valid
        else:
            selected.append(name)
            details[name] = {'reason': reason}
    return {'sourceCommit': git(root, 'rev-parse', head).stdout.strip(),
            'selected': selected, 'reused': reused, 'details': details}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--groups', default='auto')
    parser.add_argument('--sdk', action='store_true')
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    root = args.root.resolve()
    catalog = json.loads((root/'recovery/catalog/TextureTestGroups.json').read_text())['groups']
    requested = [] if args.groups == 'auto' else [x.strip() for x in args.groups.split(',') if x.strip()]
    result = select(root, catalog, requested=requested, sdk=args.sdk)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2)+'\n')
    print('Texture selected: ' + (', '.join(result['selected']) or '(none)'))
    print('Texture reused: ' + (', '.join(result['reused']) or '(none)'))
    for name in result['selected']:
        print(name + ': ' + result['details'][name]['reason'])


if __name__ == '__main__':
    main()
