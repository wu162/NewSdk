#!/usr/bin/env python3
"""Extract and audit the complete original Upheaval MOD, never the SDK tree."""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import re
import stat
import subprocess
import tarfile
from lxml import etree as E

SOURCE = 'bacfc69fc3e71c8116d1f22b933b8b6173170ecf'
PREFIX = 'SDK/Mods/Upheaval/'
TREE = '5551fc26a6f96b62613443e0d0fa2958c14cbce0'
COUNT, BYTES = 2723, 300900885
NS = 'uri:ea.com:eala:asset'


def need(value, message):
    if not value:
        raise ValueError(message)


def git(repo, *args):
    return subprocess.check_output(['git', '-C', str(repo), *args])


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
    for p in sorted(Path(root).rglob('*')):
        ordinary(p)
        if p.is_file():
            data = p.read_bytes()
            result.append(dict(path=p.relative_to(root).as_posix(), bytes=len(data), sha256=sha(data)))
    return result


def prepare(repo, sdk, mod, output):
    need(git(repo, 'rev-parse', SOURCE + ':SDK/Mods/Upheaval').decode().strip() == TREE, 'Pinned MOD tree mismatch')
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
            pass
        need(process.wait() == 0, 'Pinned git archive failed')
        print('Upheaval git archive exited; checking all extracted input blobs.', flush=True)
    finally:
        if process.poll() is None:
            process.kill()
            process.wait()
        process.stdout.close()
    inputs = inventory(mod)
    need(len(inputs) == COUNT, 'Extracted input count differs')
    for row in inputs:
        data = (mod / row['path']).read_bytes()
        old = expected[row['path']]
        need(row['bytes'] == old['bytes'] and hashlib.sha1(b'blob ' + str(len(data)).encode() + b'\0' + data).hexdigest() == old['blob'], 'Extracted input blob differs: ' + row['path'])
        row['gitBlob'] = old['blob']
    route_sources = [git(repo, 'show', 'HEAD:' + p).decode() for p in ('recovery/src/App/Application/BuiltinCompilers.cs', 'recovery/src/App/Application/BuiltinXmlRoutes.cs')]
    routes = set(re.findall(r'new BuiltinCompilerRoute\([^\n]*?"([^\"]+)"', '\n'.join(route_sources)))
    routes.update(('Texture', 'OnDemandTexture', 'TerrainTextureAtlas'))
    schema = E.XMLSchema(E.parse(str(sdk / 'Schemas/xsd/CnC3Types.xsd')))
    types, documents, runtime_xml = {}, [], []
    parser = E.XMLParser(resolve_entities=False, no_network=True)
    for p in sorted(mod.rglob('*')):
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
                  declarationDocuments=documents, runtimeXml=runtime_xml, staticTypes=types,
                  actualResolverClosureValidated=False, scope='Complete original input blob/byte/SHA audit, real SDK schema and current top-level route preflight. Actual resolver and compilation still require the CLI baseline.')
    output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')


def main():
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
