#!/usr/bin/env python3
"""Losslessly store oversized SDK products without changing validation receipts."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import zipfile


def sha(data):
    return hashlib.sha256(data).hexdigest()


def pack(directory, threshold=90 * 1024 * 1024):
    products = sorted(p for p in directory.iterdir() if p.is_file() and
                      re.fullmatch(r'(Original|Recovered|NoOriginal)-mod\.(bin|manifest|imp|relo)', p.name))
    if not products or max(p.stat().st_size for p in products) < threshold:
        return None
    target = directory / 'products.zip'
    manifest_path = directory / 'products-storage.json'
    if target.exists() or manifest_path.exists():
        raise ValueError('Refusing to replace existing immutable product storage: ' + str(directory))
    receipt_path = directory / 'validation.json'
    receipt_bytes = receipt_path.read_bytes()
    receipt = json.loads(receipt_bytes.decode('utf-8-sig'))
    expected = {mode['mode'] + '-' + item['file']: item
                for mode in receipt.get('modes', []) for item in mode.get('files', [])}
    records = []
    for path in products:
        data = path.read_bytes()
        item = dict(file=path.name, bytes=len(data), sha256=sha(data))
        if path.name in expected:
            assert item['bytes'] == expected[path.name]['bytes'] and item['sha256'] == expected[path.name]['sha256'], path.name
        records.append(item)
    temporary = directory / 'products.tmp.zip'
    try:
        with zipfile.ZipFile(temporary, 'x', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
            for path in products:
                archive.write(path, path.name)
        assert temporary.stat().st_size < 100 * 1024 * 1024, 'Compressed products still exceed GitHub file limit'
        with zipfile.ZipFile(temporary) as archive:
            assert archive.namelist() == [item['file'] for item in records]
            for item in records:
                data = archive.read(item['file'])
                assert len(data) == item['bytes'] and sha(data) == item['sha256'], 'Archive round-trip mismatch'
                assert data == (directory / item['file']).read_bytes(), 'Full raw product round-trip mismatch'
        assert receipt_path.read_bytes() == receipt_bytes, 'Receipt must remain byte-for-byte unchanged'
        result = dict(scope='lossless storage only; original receipt and every raw product hash unchanged',
                      sourceCommit=receipt['sourceCommit'], runId=receipt['runId'],
                      archive='products.zip', archiveSha256=sha(temporary.read_bytes()), files=records)
        temporary.rename(target)
        manifest_path.write_text(json.dumps(result, indent=2) + '\n')
        for path in products:
            path.unlink()
        return result
    finally:
        if temporary.exists():
            temporary.unlink()


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('directory', type=Path)
    args = parser.parse_args()
    result = pack(args.directory)
    print('SDK product storage: ' + ('unchanged' if result is None else 'lossless archive; products=' + str(len(result['files']))))
