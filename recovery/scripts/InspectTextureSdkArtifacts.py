#!/usr/bin/env python3
"""Verify archived SDK outputs and complete actual-call traces, without rerunning BAB."""
import argparse
import csv
import hashlib
import io
import json
from pathlib import Path
import zipfile

MODES = ('Original', 'Recovered', 'NoOriginal')
EXTENSIONS = ('bin', 'manifest', 'imp', 'relo')

def file_hash(path):
    h = hashlib.sha256()
    with path.open('rb') as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()

def zip_hash(archive, name):
    h = hashlib.sha256()
    with archive.open(name) as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()

def inspect(receipt_path):
    row = json.loads(receipt_path.read_text(encoding='utf-8-sig'))
    if row.get('status') != 'passed' or row.get('sdkValidated') is not True:
        raise ValueError('Receipt is not a completed SDK acceptance')
    modes = {mode['mode']: mode for mode in row['modes']}
    if set(modes) != set(MODES) or any(modes[m].get('passed') is not True for m in MODES):
        raise ValueError('Incomplete three-mode SDK receipt')
    if modes['NoOriginal'].get('withoutOriginalTextureCompiler') is not True or modes['NoOriginal'].get('assertedOriginalNotLoadedDuringEveryCall') is not True:
        raise ValueError('Missing per-call isolation receipt')
    directory = receipt_path.parent
    outputs = []
    for ext in EXTENSIONS:
        actual = []
        for mode in MODES:
            entries = [item for item in modes[mode]['files'] if item['file'] == 'mod.' + ext]
            if len(entries) != 1:
                raise ValueError('Missing or duplicate stream receipt')
            path = directory / f'{mode}-mod.{ext}'
            digest = file_hash(path)
            if digest != entries[0]['sha256'] or path.stat().st_size != entries[0]['bytes']:
                raise ValueError('Stream bytes/hash differ from immutable receipt')
            actual.append((path.stat().st_size, digest))
        if len(set(actual)) != 1:
            raise ValueError('Original/recovered/isolated stream bytes differ')
        outputs.append({'file': 'mod.' + ext, 'bytes': actual[0][0], 'sha256': actual[0][1]})
    trace_path = directory / 'traces.zip'
    trace_hash = file_hash(trace_path)
    if trace_hash != row['traceArchiveSha256']:
        raise ValueError('Trace archive differs from immutable receipt')
    counts = []
    with zipfile.ZipFile(trace_path) as archive:
        files = [name for name in archive.namelist() if not name.endswith('/')]
        hashes = {}
        for mode in MODES:
            prefix = mode + '-trace/'
            hashes[mode] = {name[len(prefix):]: zip_hash(archive, name) for name in files if name.startswith(prefix)}
            if not hashes[mode] or 'texture.calls.tsv' not in hashes[mode]:
                raise ValueError('Incomplete actual-call trace directory')
            with archive.open(prefix + 'texture.calls.tsv') as stream:
                calls = list(csv.DictReader(io.TextIOWrapper(stream, encoding='utf-8-sig'), delimiter='\t'))
            success = [item for item in calls if item['Stage'] == 'success']
            failures = [item for item in calls if item['Stage'] == 'failure']
            unique = {(item['TypeId'], item['InstanceId']) for item in success}
            if failures or len(success) != modes[mode]['actualCalls'] or len(unique) != modes[mode]['uniqueAssets'] or not unique:
                raise ValueError('Actual Core calls differ from receipt or contain failures')
            if any(not item['FileSHA256'] for item in success if item['TypeName'] in ('Texture', 'OnDemandTexture')):
                raise ValueError('Actual-call source fingerprint is missing')
            formats = sorted({item['OutputFormat'] for item in success})
            roots = sorted({item['TypeName'] for item in success})
            kinds = sorted({item['Type'] for item in success})
            counts.append({'mode': mode, 'actualCalls': len(success), 'uniqueAssets': len(unique),
                           'traceFiles': len(hashes[mode]), 'formats': formats, 'rootNames': roots, 'kinds': kinds})
        if hashes['Original'] != hashes['Recovered'] or hashes['Original'] != hashes['NoOriginal']:
            raise ValueError('Complete trace file sets or actual bytes differ between modes')
        unexpected = [name for name in files if not any(name.startswith(mode + '-trace/') for mode in MODES)]
        if unexpected:
            raise ValueError('Unexpected files in complete trace archive')
    return {'status': 'offline-sdk-artifact-inspection', 'scope': 'All four actual outputs and every complete trace file; no new SDK execution',
            'sourceCommit': row['sourceCommit'], 'runId': str(row['runId']), 'runAttempt': str(row['runAttempt']),
            'profile': row.get('profile'), 'streams': outputs, 'traceArchiveSha256': trace_hash,
            'modes': counts, 'perCallIsolationRuntimeReceipt': True}

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('receipt', type=Path)
    parser.add_argument('--out', type=Path)
    args = parser.parse_args()
    text = json.dumps(inspect(args.receipt), indent=2) + '\n'
    if args.out:
        args.out.write_text(text)
    else:
        print(text, end='')
