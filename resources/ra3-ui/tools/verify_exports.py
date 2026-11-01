#!/usr/bin/env python3
"""Verify committed FLA/AS files against the conversion report, without downloads."""
import hashlib
import argparse
import json
from pathlib import Path


def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('resource_root', nargs='?', type=Path, default=Path(__file__).resolve().parents[1])
    root = parser.parse_args().resource_root.resolve()
    report = json.loads((root / 'conversion-report.json').read_text())
    movies = 0
    scripts = 0
    for row in report['movies']:
        if row['status'] != 'exported':
            raise ValueError('Unexported movie: ' + row['source'])
        fla = root / row['fla']
        with fla.open('rb') as stream:
            if stream.read(8) != bytes.fromhex('d0cf11e0a1b11ae1'):
                raise ValueError('Invalid binary FLA: ' + row['fla'])
        if digest(fla) != row['flaSha256']:
            raise ValueError('FLA checksum mismatch: ' + row['fla'])
        movies += 1
        for script in row['scripts']:
            if digest(root / script['path']) != script['sha256']:
                raise ValueError('AS checksum mismatch: ' + script['path'])
            scripts += 1
    if movies != report['totalMovies']:
        raise ValueError('Movie inventory mismatch')
    print(json.dumps({'verifiedFlas': movies, 'verifiedAsFiles': scripts,
                      'scope': 'File identity only; compiler results are in validation/compiler-report.json'}))


if __name__ == '__main__':
    main()
