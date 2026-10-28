#!/usr/bin/env python3
"""Extract and audit the complete original Upheaval MOD, never the SDK tree."""
import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import re
import runpy
import stat
import subprocess
import tarfile
import time
from lxml import etree as E

SOURCE = 'bacfc69fc3e71c8116d1f22b933b8b6173170ecf'
PREFIX = 'SDK/Mods/Upheaval/'
TREE = '5551fc26a6f96b62613443e0d0fa2958c14cbce0'
COUNT, BYTES = 2723, 300900885
NS = 'uri:ea.com:eala:asset'
DEADLINE = None


def remaining(limit=180):
    value = limit if DEADLINE is None else min(limit, DEADLINE - time.monotonic())
    if value <= 0:
        raise TimeoutError('Upheaval input preparation deadline exceeded; not product cancellation')
    return value


def need(value, message):
    if not value:
        raise ValueError(message)


def git(repo, *args):
    return subprocess.check_output(['git', '-C', str(repo), *args], timeout=remaining())


def sha(data):
    return hashlib.sha256(data).hexdigest()


def inventory(root):
    def ordinary(path):
        attributes = path.lstat()
        need(not stat.S_ISLNK(attributes.st_mode) and
             not (getattr(attributes, 'st_file_attributes', 0) & getattr(stat, 'FILE_ATTRIBUTE_REPARSE_POINT', 0x400)),
             'Input reparse point rejected: ' + str(path))
    ordinary(Path(root))
    result = []
    pending = [Path(root)]
    while pending:
        remaining()
        directory = pending.pop()
        ordinary(directory)
        with os.scandir(directory) as entries:
            for entry in entries:
                remaining()
                p = Path(entry.path)
                ordinary(p)  # Reject symlinks/junctions BEFORE descending.
                if entry.is_dir(follow_symlinks=False):
                    pending.append(p)
                elif entry.is_file(follow_symlinks=False):
                    data = p.read_bytes()
                    result.append(dict(path=p.relative_to(root).as_posix(), bytes=len(data), sha256=sha(data)))
                else:
                    raise ValueError('Non-regular input rejected: ' + str(p))
    result.sort(key=lambda row: row['path'])
    return result


def prepare(repo, sdk, mod, output):
    need(git(repo, 'rev-parse', SOURCE + ':SDK/Mods/Upheaval').decode().strip() == TREE, 'Pinned MOD tree mismatch')
    # Object sizes (-l) can hydrate one missing promisor blob at a time. Obtain
    # only tree metadata first, then use Git's standard bulk promisor fetch.
    # No credential value is placed in the command or emitted into the log.
    tree_rows = git(repo, 'ls-tree', '-rz', SOURCE, 'SDK/Mods/Upheaval').split(b'\0')
    blobs = set()
    file_count = 0
    for row in tree_rows:
        if not row:
            continue
        metadata, path = row.split(b'\t', 1)
        mode, kind, blob = metadata.decode().split()
        need(mode == '100644' and kind == 'blob' and path.decode('utf-8').startswith(PREFIX), 'Unsafe pinned tree entry')
        need(re.fullmatch(r'[0-9a-f]{40}', blob), 'Malformed pinned blob identity')
        blobs.add(blob); file_count += 1
    need(file_count == COUNT, 'Pinned tree file count differs before hydration')
    promisor = subprocess.run(['git', '-C', str(repo), 'config', '--get', 'remote.origin.promisor'],
                              check=False, capture_output=True, text=True, timeout=remaining())
    need(promisor.returncode in (0, 1), 'Cannot inspect promisor configuration')
    if promisor.returncode == 0 and promisor.stdout.strip().lower() == 'true':
        print('Prefetching complete pinned Upheaval blob set in one bounded promisor fetch.', flush=True)
        environment = dict(os.environ, GIT_TERMINAL_PROMPT='0', GCM_INTERACTIVE='never')
        fetched = subprocess.run(['git', '-C', str(repo), '-c', 'credential.helper=',
                        '-c', 'credential.helper=!gh auth git-credential',
                        '-c', 'fetch.negotiationAlgorithm=noop', 'fetch', 'origin',
                        '--no-tags', '--no-write-fetch-head', '--recurse-submodules=no',
                        '--filter=blob:none', '--stdin'],
                       input=('\n'.join(sorted(blobs)) + '\n').encode('ascii'),
                       stdout=subprocess.PIPE, stderr=subprocess.PIPE, timeout=remaining(300), check=False, env=environment)
        if fetched.returncode != 0:
            # Emit fixed categories only, never raw remote/helper output that
            # could contain credential material or an authenticated URL.
            error = fetched.stderr.lower()
            categories = [label for text, label in ((b'authentication', 'authentication'), (b'not found', 'not-found'),
                (b'resolve host', 'dns'), (b'timed out', 'network-timeout'), (b'not our ref', 'object-unavailable')) if text in error]
            print('Upheaval hydration failed: exit=' + str(fetched.returncode) + ' categories=' + ','.join(categories) +
                  ' stderrBytes=' + str(len(fetched.stderr)) + ' stderrSha256=' + sha(fetched.stderr), flush=True)
            raise RuntimeError('Bounded Upheaval hydration failed; credential output withheld')
        print('Pinned Upheaval bulk blob hydration completed.', flush=True)
    else:
        print('Repository is not an origin promisor clone; using its existing complete objects.', flush=True)
    rows = git(repo, 'ls-tree', '-rlz', SOURCE, 'SDK/Mods/Upheaval').split(b'\0')
    expected = {}
    for row in rows:
        if not row:
            continue
        meta, path = row.split(b'\t', 1)
        mode, kind, blob, size = meta.decode().split()
        path = path.decode('utf-8')
        need(mode == '100644' and kind == 'blob' and path.startswith(PREFIX), 'Unsafe pinned input')
        relative = path[len(PREFIX):]
        need('..' not in PurePosixPath(relative).parts and not relative.startswith('/'), 'Unsafe relative input')
        expected[relative] = dict(blob=blob, bytes=int(size))
    need(len(expected) == COUNT and sum(x['bytes'] for x in expected.values()) == BYTES, 'Pinned complete input inventory mismatch')
    need(not mod.exists(), 'Refusing to replace extracted MOD')
    mod.mkdir(parents=True)
    process = subprocess.Popen(['git', '-C', str(repo), 'archive', '--format=tar', SOURCE, 'SDK/Mods/Upheaval'], stdout=subprocess.PIPE)
    try:
        with tarfile.open(fileobj=process.stdout, mode='r|') as archive:
            for member in archive:
                remaining()
                if member.isdir():
                    continue
                need(member.isfile() and member.name.startswith(PREFIX), 'Unsafe archive entry')
                relative = member.name[len(PREFIX):]
                need(relative in expected, 'Unexpected archive entry')
                target = mod / relative
                target.parent.mkdir(parents=True, exist_ok=True)
                with target.open('xb') as stream:
                    stream.write(archive.extractfile(member).read())
        # tarfile's streaming reader may stop at the TAR end marker before
        # consuming Git's remaining block padding. On Windows that padding can
        # fill the small stdout pipe and prevent git archive from exiting; wait
        # must follow EOF, not precede draining the producer's remaining bytes.
        print('Upheaval TAR entries extracted; draining git archive stdout to EOF.', flush=True)
        for _ in iter(lambda: process.stdout.read(65536), b''):
            remaining()
        need(process.wait(timeout=remaining()) == 0, 'Pinned git archive failed')
        print('Upheaval git archive exited; checking all extracted input blobs.', flush=True)
    finally:
        if process.poll() is None:
            process.kill()
            process.wait(timeout=15)
        process.stdout.close()
    inputs = inventory(mod)
    need(len(inputs) == COUNT, 'Extracted input count differs')
    for row in inputs:
        remaining()
        data = (mod / row['path']).read_bytes()
        old = expected[row['path']]
        need(row['bytes'] == old['bytes'] and hashlib.sha1(b'blob ' + str(len(data)).encode() + b'\0' + data).hexdigest() == old['blob'], 'Extracted input blob differs: ' + row['path'])
        row['gitBlob'] = old['blob']
    route_sources = [git(repo, 'show', 'HEAD:' + p).decode() for p in ('recovery/src/App/Application/BuiltinCompilers.cs', 'recovery/src/App/Application/BuiltinXmlRoutes.cs')]
    routes = set(re.findall(r'new BuiltinCompilerRoute\([^\n]*?"([^\"]+)"', '\n'.join(route_sources)))
    routes.update(('Texture', 'OnDemandTexture', 'TerrainTextureAtlas'))
    # Reuse the production preflight's aggregate construction from the exact
    # unchanged SDK bytes. Direct lxml file-URI includes fail on Windows paths.
    # A non-__main__ run name loads definitions only, never create() fixtures.
    schema_script = Path(repo) / 'recovery/scripts/GenerateXmlSdkModA.py'
    schema_module = runpy.run_path(str(schema_script), run_name='upheaval_schema_preflight')
    need(Path(schema_module['S']).resolve() == (Path(sdk) / 'Schemas/xsd').resolve(), 'Aggregate schema must use the measured immutable SDK')
    schema = schema_module['aggregate_schema']()
    remaining()
    types, documents, runtime_xml = {}, [], []
    parser = E.XMLParser(resolve_entities=False, no_network=True)
    for input_row in inputs:
        remaining()
        p = mod / input_row['path']
        if p.suffix.lower() not in ('.xml', '.w3x'):
            continue
        doc = E.parse(str(p), parser)
        root = doc.getroot()
        if root.tag != '{' + NS + '}AssetDeclaration':
            runtime_xml.append(p.relative_to(mod).as_posix())
            continue
        # Real SDK schema, without replacing it with a test-specific schema.
        # Validate only actual declaration docs; Misc runtime XML is not a compiler root.
        # Only the five planned entry documents are guaranteed to be consumed;
        # unused source documents are parsed and route-audited, not promoted to
        # mandatory runtime coverage by this static scan.
        if p == mod / 'Data/Mod.xml' or (p.parent == mod / 'Data/AptUI') or (p.parent == mod / 'Data/AdditionalMaps' and p.name.startswith('MapMetaData_')):
            schema.assertValid(doc)
        documents.append(p.relative_to(mod).as_posix())
        for child in root:
            if not isinstance(child.tag, str):
                continue
            kind = E.QName(child).localname
            if kind in ('Includes', 'Defines', 'Tags'):
                continue
            need(kind in routes, 'No current production route for ' + kind)
            types[kind] = types.get(kind, 0) + 1
    entries = ['Data/Mod.xml']
    entries += sorted(p.relative_to(mod).as_posix() for p in (mod / 'Data/AptUI').glob('*.xml'))
    entries += sorted(p.relative_to(mod).as_posix() for p in (mod / 'Data/AdditionalMaps').glob('MapMetaData_*.xml'))
    need(len(entries) == 5 and len([x for x in entries if '/AptUI/' in x]) == 3, 'Pinned five-entry plan input contract differs')
    result = dict(sourceCommit=SOURCE, modTree=TREE, files=inputs, fileCount=COUNT, bytes=BYTES, entries=entries,
                  schemaProvider=dict(path='recovery/scripts/GenerateXmlSdkModA.py', sha256=sha(schema_script.read_bytes())),
                  declarationDocuments=documents, runtimeXml=runtime_xml, staticTypes=types,
                  actualResolverClosureValidated=False, scope='Complete original input blob/byte/SHA audit, real SDK schema and current top-level route preflight. Actual resolver and compilation still require the CLI baseline.')
    output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')


def main():
    global DEADLINE
    DEADLINE = time.monotonic() + 600
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--repo', type=Path)
    ap.add_argument('--sdk', type=Path)
    ap.add_argument('--mod', type=Path, required=True)
    ap.add_argument('--output', type=Path, required=True)
    ap.add_argument('--verify', type=Path)
    args = ap.parse_args()
    if args.verify:
        before = json.loads(args.verify.read_text(encoding='utf-8'))
        actual = inventory(args.mod)
        expected = [{k: v for k, v in row.items() if k != 'gitBlob'} for row in before['files']]
        need(actual == expected, 'Complete MOD input inventory changed')
        args.output.write_text(json.dumps(dict(unchanged=True, fileCount=len(actual), bytes=sum(x['bytes'] for x in actual)), indent=2) + '\n', encoding='utf-8')
    else:
        need(args.repo and args.sdk, 'Prepare requires repository and SDK')
        prepare(args.repo, args.sdk, args.mod, args.output)


if __name__ == '__main__':
    main()
