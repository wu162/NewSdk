#!/usr/bin/env python3
"""Independently audit saved SDK/complete-MOD/package/deployment bytes; no codec execution."""
import argparse
import csv
import hashlib
import io
import json
from pathlib import Path
import subprocess
import zipfile

from InspectTextureSdkArchive import audit as audit_sdk
from InspectTexturePpmSdkPackage import audit as audit_ppm, original_file


def sha(data):
    return hashlib.sha256(data).hexdigest()


def load(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def package(root, directory, source):
    receipt = load(directory / 'validation.json')
    data = (directory / 'pc-plugin-package.zip').read_bytes()
    assert sha(data) == receipt['packageArchiveSha256']
    with zipfile.ZipFile(io.BytesIO(data)) as z:
        manifest_bytes = z.read('manifest.json')
        assert manifest_bytes == original_file(root, 'HEAD', (directory / 'package-manifest.json').relative_to(root).as_posix())
        m = json.loads(manifest_bytes.decode('utf-8-sig'))
        assert m['sourceCommit'] == source and m['status'] == 'built' and not m['dirtySource']
        names = {f['file'] for f in m['runtimeFiles']}
        assert len(names) == len(m['runtimeFiles']) == 6
        assert names == {n for n in z.namelist() if n.lower().endswith('.dll')}
        assert not m['originalTextureCompilerDistributed'] and m['targetPlatform'] == 'Win32'
        for item in m['runtimeFiles']:
            b = z.read(item['file'])
            assert len(b) == item['bytes'] and sha(b) == item['sha256']
        for item in m['sourceFiles']:
            b = original_file(root, source, item['file'])
            assert len(b) == item['bytes'] and sha(b) == item['sha256'], item['file']
        for item in m['sdkDependencies']:
            b = original_file(root, source, 'SDK/tools/' + item['file'])
            assert sha(b) == item['sha256'] and len(b) == item['bytes']
        for item in directory.glob('*installation.json'):
            r = load(item)
            assert r['status'] == 'installed' and r['packageSourceCommit'] == source
            assert r['packageManifestSha256'] == sha(manifest_bytes)
            assert len(r['files']) == 7 and {f['file'] for f in r['files']} == names | {'BinaryAssetBuilder.exe.config'}
            hashes = {f['file']: f['sha256'] for f in m['runtimeFiles']}
            for f in r['files']:
                if f['file'] in hashes:
                    assert f['installedSha256'] == hashes[f['file']]
    return dict(packageArchiveSha256=sha(data), sourceFilesVerified=len(m['sourceFiles']), runtimeFiles=m['runtimeFiles'])


def mymod(root, directory, source):
    r = load(directory / 'validation.json')
    assert r['sourceCommit'] == source and r['status'] == 'passed' and r['sdkValidated']
    assert r['packageRestoreValidated'] and r['portableRestoreValidated'] and r['reusedPpmPackage']
    for item in r['inputMymod']:
        path = 'SDK/Mods/mymod/' + item['path'].replace('\\', '/')
        data = original_file(root, source, path)
        assert len(data) == item['bytes'] and sha(data) == item['sha256']
    assert len(r['artInputs']) == 157 and r['ddsInputs'] == 150 and r['tgaInputs'] == 7
    for item in r['artInputs']:
        data = original_file(root, source, item['path'].replace('\\', '/'))
        assert len(data) == item['bytes'] and sha(data) == item['sha256']
    profiles = []
    for profile, field, count in [('Baseline', 'baselineModes', 0), ('ArtOverlay', 'modes', 384)]:
        modes = r[field]
        assert len(modes) == 3 and {m['mode'] for m in modes} == {'Original', 'Recovered', 'NoOriginal'}
        isolated = next(m for m in modes if m['mode'] == 'NoOriginal')
        assert isolated['withoutOriginalTextureCompiler'] and isolated['assertedOriginalNotLoadedDuringEveryCall']
        with zipfile.ZipFile(directory / (profile + '-original-outputs.zip')) as z:
            names = {'mod.bin', 'mod.manifest', 'mod.imp', 'mod.relo'}
            assert len(z.namelist()) == 4 and set(z.namelist()) == names
            products = {name: z.read(name) for name in names}
        for m in modes:
            assert m['passed'] and m['uniqueAssets'] == m['actualCalls'] == count
            assert len(m['files']) == 4 and {f['file'] for f in m['files']} == names
            for f in m['files']:
                assert len(products[f['file']]) == f['bytes'] and sha(products[f['file']]) == f['sha256']
        if count:
            with zipfile.ZipFile(directory / (profile + '-original-traces.zip')) as z:
                assert len(z.namelist()) == len(set(z.namelist()))
                original_index = None
                for m in modes:
                    label = profile + '-' + m['mode']
                    index = load(directory / (label + '-trace-files.json'))
                    assert len(index) == m['traceFiles'] == len(z.namelist())
                    actual_index = sorted((f['path'], f['bytes'], f['sha256']) for f in index)
                    if original_index is None:
                        original_index = actual_index
                    assert actual_index == original_index
                    assert {f['path'] for f in index} == set(z.namelist())
                    for f in index:
                        b = z.read(f['path'])
                        assert len(b) == f['bytes'] and sha(b) == f['sha256']
                    # The archived call log is written with LF by the host.
                    # Compare its exact saved Git blob, without checkout filters.
                    relative = (directory / (label + '-calls.tsv')).relative_to(root).as_posix()
                    saved = subprocess.check_output(['git', '-C', str(root), 'show', 'HEAD:' + relative])
                    assert saved == z.read('texture.calls.tsv')
                calls = list(csv.DictReader(io.StringIO(z.read('texture.calls.tsv').decode('utf-8-sig')), delimiter='\t'))
                assert not any(c['Stage'] == 'failure' for c in calls)
                success = [c for c in calls if c['Stage'] == 'success']
                assert len(success) == len({(c['TypeId'], c['InstanceId']) for c in success}) == 384
        else:
            assert all(m['traceFiles'] == 0 for m in modes)
        profiles.append(dict(profile=profile, actualCallsPerMode=count,
                             products=[dict(file=n, bytes=len(b), sha256=sha(b)) for n, b in sorted(products.items())],
                             fullOriginalTraceBytesVerified=True, allModeTraceManifestsEqual=True))
    return dict(profiles=profiles, package=package(root, directory, source), portableRestoreValidated=True)


def audit(root, run, source, require_deployment):
    receipts = []
    for p in (root / 'recovery/evidence/texture').glob('**/' + run + '-1/validation.json'):
        r = load(p)
        if r.get('sdkValidated'):
            assert r['sourceCommit'] == source and r['status'] == 'passed'
            receipts.append((p.parent, r))
    assert len(receipts) == 17 and len({r['profile'] for _, r in receipts}) == 17
    groups = []
    for directory, r in receipts:
        if r['profile'] != 'PcMymod':
            item = audit_sdk(directory, source)
            item['receipt'] = directory.relative_to(root).as_posix()
            if (directory / 'pc-plugin-package.zip').exists():
                item['package'] = package(root, directory, source)
            groups.append(item)
    ppm = audit_ppm(root, run, source)
    mod = mymod(root, root / 'recovery/evidence/texture/sdk-mymod' / (run + '-1'), source)
    assert mod['package']['packageArchiveSha256'] == ppm['packageArchiveSha256']
    result = dict(scope='independent full saved SDK/complete-MOD/package and deployment audit; no codec execution', sourceCommit=source, runId=run, sdkProfiles=groups, ppm=ppm, completeMymod=mod, productionInstalled=False)
    deployed = root / 'SDK/RecoveryTexture/deployment.json'
    if require_deployment:
        d = load(deployed)
        assert d['status'] == 'installed' and d['sourceCommit'] == source and str(d['runId']) == run
        assert d['packageArchiveSha256'] == ppm['packageArchiveSha256']
        assert d['fullMymodValidated'] and d['ppmSdkValidated'] and d['portableRestoreValidated']
        for f in d['runtimeFiles']:
            b = (root / 'SDK/tools' / f['file']).read_bytes()
            assert len(b) == f['bytes'] and sha(b) == f['sha256']
        assert sha((root / 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll').read_bytes()) == d['originalCompilerSha256']
        assert sha((root / 'SDK/tools/BinaryAssetBuilder.exe.config').read_bytes()) == d['installedConfigSha256']
        receipt = load(root / 'SDK/RecoveryTexture/installation.json')
        assert receipt['sdkDirectory'] == '..' and receipt['backupDirectory'] == 'backup'
        for f in receipt['files']:
            if f['existed']:
                assert sha((root / 'SDK/RecoveryTexture/backup' / f['file']).read_bytes()) == f['beforeSha256']
        assert sha((root / 'SDK/RecoveryTexture/manifest.json').read_bytes()) == receipt['packageManifestSha256']
        assert (root / 'SDK/RecoveryTexture/RestoreTextureCompiler.ps1').read_bytes() == original_file(root, source, 'recovery/scripts/RestoreTextureCompiler.ps1')
        result['productionInstalled'] = True
        result['deployment'] = d
    return result


if __name__ == '__main__':
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    p.add_argument('--run-id', required=True)
    p.add_argument('--source-commit', required=True)
    p.add_argument('--out', type=Path, required=True)
    p.add_argument('--require-deployment', action='store_true')
    a = p.parse_args()
    result = audit(a.root.resolve(), a.run_id, a.source_commit, a.require_deployment)
    a.out.parent.mkdir(parents=True, exist_ok=True)
    a.out.write_text(json.dumps(result, indent=2) + '\n')
    print('Audited 17 complete SDK profiles, full mymod, package identity; productionInstalled=' + str(result['productionInstalled']))
