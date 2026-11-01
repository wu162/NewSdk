#!/usr/bin/env python3
"""Reassemble the bundled original archive locally, checking every SHA-256."""
import argparse
import hashlib
import json
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('destination', type=Path, help='Output ra3ui.7z path')
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[1]
    provenance = json.loads((root / 'provenance.json').read_text())
    archive = provenance['archive']
    if args.destination.exists():
        parser.error('Destination already exists')
    combined = hashlib.sha256()
    try:
        with args.destination.open('xb') as target:
            for part in archive['parts']:
                data = (root / part['path']).read_bytes()
                if hashlib.sha256(data).hexdigest() != part['sha256']:
                    raise ValueError('Part checksum mismatch: ' + part['path'])
                target.write(data)
                combined.update(data)
        if combined.hexdigest() != archive['sha256']:
            raise ValueError('Archive checksum mismatch')
    except Exception:
        args.destination.unlink(missing_ok=True)
        raise
    print(f'Verified {args.destination}: {combined.hexdigest()}')


if __name__ == '__main__':
    main()
