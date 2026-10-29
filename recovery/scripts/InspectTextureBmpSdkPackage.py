#!/usr/bin/env python3
"""Audit saved PC package/ID/input/SDK bytes; never run a compiler or codec."""
import argparse
from collections import Counter
import csv
import hashlib
import io
import json
from pathlib import Path, PureWindowsPath
import struct
import subprocess
import xml.etree.ElementTree as ET
import zipfile

from InspectTextureSdkArchive import audit as audit_sdk


def sha(data):
    return hashlib.sha256(data).hexdigest()


def original_file(root, source, name):
    # Actions checks out text with Windows CRLF. Apply the same Git filters;
    # callers still compare exact package bytes and recorded SHA256 values.
    return subprocess.run(['git', '-C', str(root), '-c', 'core.autocrlf=true', 'cat-file', '--filters', source + ':' + name], check=True, capture_output=True).stdout


def tsv(data):
    return list(csv.DictReader(io.StringIO(data.decode('utf-8-sig')), delimiter='\t'))


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/sdk-bmp-headers' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['sourceCommit'] == source and str(receipt['runId']) == run
    assert receipt['status'] == 'passed' and receipt['sdkValidated'] is True
    assert receipt['packageRestoreValidated'] is True and receipt['expectedAssets'] == 22670
    sdk = audit_sdk(directory, source)  # Every full product and complete trace member.
    package_bytes = (directory / 'pc-plugin-package.zip').read_bytes()
    assert sha(package_bytes) == receipt['packageArchiveSha256']
    runtime = {'Recovered.BinaryAssetBuilder.TextureCompiler.dll', 'Recovered.TextureDxtAlpha.dll',
               'Recovered.TextureDxtColour.dll', 'Recovered.TextureJpeg.dll', 'Recovered.TexturePublicS3.dll'}
    with zipfile.ZipFile(io.BytesIO(package_bytes)) as package:
        members = [n for n in package.namelist() if not n.endswith('/')]
        assert len(members) == len(set(members))
        assert all(not n.startswith('/') and '..' not in n.split('/') for n in members)
        assert {n for n in members if n.lower().endswith('.dll')} == runtime
        manifest_bytes = package.read('manifest.json')
        assert manifest_bytes == (directory / 'package-manifest.json').read_bytes()
        manifest = json.loads(manifest_bytes.decode('utf-8-sig'))
        assert manifest['status'] == 'built' and manifest['sourceCommit'] == source and manifest['dirtySource'] is False
        assert manifest['targetPlatform'] == 'Win32' and manifest['originalTextureCompilerDistributed'] is False
        assert len(manifest['runtimeFiles']) == 5 and {f['file'] for f in manifest['runtimeFiles']} == runtime
        runtime_hashes = {}
        for item in manifest['runtimeFiles']:
            name = item['file']; data = package.read(name)
            assert len(data) == item['bytes'] and sha(data) == item['sha256']
            pe = struct.unpack_from('<I', data, 0x3c)[0]
            assert data[pe:pe+4] == b'PE\0\0' and struct.unpack_from('<H', data, pe+4)[0] == 0x14c
            if name in {'Recovered.TextureDxtAlpha.dll', 'Recovered.TextureDxtColour.dll'}:
                assert struct.unpack_from('<I', data, pe+40)[0] == struct.unpack_from('<I', data, pe+128)[0] == 0
            runtime_hashes[name] = sha(data)
        for item in manifest['sdkDependencies']:
            assert item['file'] in {'BinaryAssetBuilder.Core.dll', 'BinaryAssetBuilder.Utility.dll'}
            data = original_file(root, source, 'SDK/tools/' + item['file'])
            assert sha(data) == item['sha256'] and len(data) == item['bytes']
        assert len(manifest['sdkDependencies']) == 2
        source_files = manifest['sourceFiles']
        assert len(source_files) == len({f['file'] for f in source_files})
        for item in source_files:
            data = original_file(root, source, item['file'])
            assert len(data) == item['bytes'] and sha(data) == item['sha256'], item['file']
        for name in ('InstallTextureCompiler.ps1', 'RestoreTextureCompiler.ps1'):
            assert package.read(name) == original_file(root, source, 'recovery/scripts/' + name)
        for name in ('README', 'NOTICE.md', 'PROVENANCE.json'):
            assert package.read('upstream/IJG6b/' + name) == original_file(root, source, 'recovery/reference/ijg6b/' + name)
        upstream = 'upstream/PublicS3/'
        for name in [n for n in members if n.startswith(upstream)]:
            assert package.read(name) == original_file(root, source, 'src/TextureCompiler/PublicS3/' + name[len(upstream):])
    for mode in ('Recovered', 'NoOriginal'):
        install = json.loads((directory / (mode + '-installation.json')).read_text(encoding='utf-8-sig'))
        assert install['status'] == 'installed' and install['packageSourceCommit'] == source
        assert install['packageManifestSha256'] == sha(manifest_bytes)
        assert len(install['files']) == 6 and {f['file'] for f in install['files']} == runtime | {'BinaryAssetBuilder.exe.config'}
        for item in install['files']:
            if item['file'] in runtime:
                assert item['installedSha256'] == runtime_hashes[item['file']]
    xml_bytes = (directory / 'input-Mod.xml').read_bytes()
    assert sha(xml_bytes) == receipt['inputModSha256']
    namespace = '{uri:ea.com:eala:asset}'
    nodes = [node for node in ET.fromstring(xml_bytes) if 'id' in node.attrib]
    assert len(nodes) == 22670 and Counter(n.tag.removeprefix(namespace) for n in nodes) == {
        'Texture': 11264, 'OnDemandTexture': 11264, 'TerrainTextureAtlas': 142}
    id_bytes = (directory / 'Original-asset-ids.tsv').read_bytes()
    assert sha(id_bytes) == receipt['assetIdManifestSha256']
    ids = tsv(id_bytes)
    assert len(ids) == len({r['instanceHash'] for r in ids}) == len({r['instanceName'].lower() for r in ids}) == 22670
    for index, (row, node) in enumerate(zip(ids, nodes)):
        root_name = node.tag.removeprefix(namespace)
        assert int(row['ordinal']) == index and row['type'] == root_name and row['instanceName'] == node.attrib['id']
        assert row['originalName'] == ('BmpHeaderAtlas_' if root_name == 'TerrainTextureAtlas' else 'BmpHeader_') + str(index)
        attempt = int(row['attempts'])
        assert row['instanceName'] == row['originalName'] + ('_h' + format(attempt, 'x') if attempt else '')
    fixtures = {f['name']: f for f in receipt['fixtureFiles']}
    assert len(fixtures) == len(receipt['fixtureFiles']) == 707
    families = Counter('atlas' if n.startswith('atlas-') else 'transparent' if n.startswith('transparent-') else 'os2' if n.startswith('os2-') else 'immutable' for n in fixtures)
    assert families == {'immutable': 356, 'os2': 189, 'transparent': 91, 'atlas': 71}
    seen = set()
    by_id = {}
    for row, node in zip(ids, nodes):
        attrs = node.attrib
        if row['type'] == 'TerrainTextureAtlas':
            tile = node.find(namespace + 'Tile')
            assert tile is not None
            for field in ('BaseTexture', 'NormalTexture'):
                assert 'tga' in tile.attrib[field].lower()
                name = PureWindowsPath(tile.attrib[field]).name
                if name != 'missing-normal.tga':
                    assert name in fixtures; seen.add(name)
            input_hash = ''  # Trace wrapper has no File attribute on atlas.
        else:
            name = PureWindowsPath(attrs['File']).name
            assert name in fixtures; seen.add(name); input_hash = fixtures[name]['sha256']
        by_id[row['instanceName']] = (row['instanceHash'], row['type'], input_hash)
    assert seen == fixtures.keys()
    with zipfile.ZipFile(directory / 'traces.zip') as traces:
        for mode in ('Original', 'Recovered', 'NoOriginal'):
            assert (directory / (mode + '-asset-ids.tsv')).read_bytes() == id_bytes
            rows = tsv(traces.read(mode + '-trace/texture.calls.tsv'))
            successes = [row for row in rows if row['Stage'] == 'success']
            assert len(successes) == 22670
            assert {row['InstanceName'] for row in successes} == by_id.keys()
            for row in successes:
                assert (row['InstanceId'], row['TypeName'], row['FileSHA256']) == by_id[row['InstanceName']]
    return dict(scope='independent saved full SDK/product/trace/package/source/ID audit; no compiler/codec execution',
                sourceCommit=source, runId=run, packageArchiveSha256=sha(package_bytes),
                packageManifestSha256=sha(manifest_bytes), runtimeHashes=runtime_hashes,
                sourceFingerprintsVerified=len(source_files), sourceImageFamilies=dict(families),
                actualAssetsPerMode=22670, positiveAtlasPerMode=142,
                globallyUniqueInstanceHashes=len(ids), resolvedIdCollisions=sum(int(r['attempts']) > 0 for r in ids),
                sdk=sdk)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--source-commit', required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    result = audit(args.root.resolve(), args.run_id, args.source_commit)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited package/source/IDs and all SDK bytes: assets=22670 positiveAtlas=142 per mode')
