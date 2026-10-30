#!/usr/bin/env python3
"""Losslessly split oversized current-run capture ZIPs; never rewrite receipts."""
import argparse
import hashlib
import json
from pathlib import Path


ARCHIVES = ('captures.zip', 'failure-captures.zip')
MANIFEST = 'captures-storage.json'
CHUNK_BYTES = 64 * 1024 * 1024
PART_DIRECTORY = 'captures-parts'
ATTRIBUTES = b'* -text\n'
RUN_ATTRIBUTES = (b'validation.json -text\ncaptures-storage.json -text\n'
                  b'captures-parts/** -text\n.gitattributes -text\n')


def fingerprint(path):
    digest = hashlib.sha256()
    size = 0
    with Path(path).open('rb') as source:
        for data in iter(lambda: source.read(1024 * 1024), b''):
            size += len(data)
            digest.update(data)
    return dict(bytes=size, sha256=digest.hexdigest())


def _manifest(directory):
    result = json.loads((directory / MANIFEST).read_text(encoding='utf-8'))
    if result.get('version') != 1 or not isinstance(result.get('archives'), list):
        raise ValueError('Unsupported capture storage manifest')
    receipt_hash = fingerprint(directory / 'validation.json')['sha256']
    if result.get('validationSha256') != receipt_hash:
        raise ValueError('Validation receipt changed after capture storage')
    run_attributes = directory / '.gitattributes'
    if not run_attributes.is_file() or not run_attributes.read_bytes().endswith(RUN_ATTRIBUTES):
        raise ValueError('Capture receipts/manifests require explicit binary Git attributes')
    attributes = directory / PART_DIRECTORY / '.gitattributes'
    if not attributes.is_file() or attributes.read_bytes() != ATTRIBUTES:
        raise ValueError('Capture parts require explicit immutable binary Git attributes')
    names = set()
    for record in result['archives']:
        name = record.get('file')
        if name not in ARCHIVES or name in names or not record.get('parts'):
            raise ValueError('Invalid or duplicate capture archive')
        names.add(name)
        if not isinstance(record.get('bytes'), int) or record['bytes'] <= 0:
            raise ValueError('Invalid original capture length')
        for index, part in enumerate(record['parts'], 1):
            if part.get('file') != PART_DIRECTORY + '/' + name + '.part' + str(index).zfill(4):
                raise ValueError('Invalid capture part order/path')
            if not isinstance(part.get('bytes'), int) or not 0 < part['bytes'] <= CHUNK_BYTES:
                raise ValueError('Invalid capture part length')
    return result


def _chunks(directory, record):
    """Yield all verified raw ZIP bytes; exhausting the iterator checks EOF/SHA."""
    total = 0
    digest = hashlib.sha256()
    for part in record['parts']:
        path = directory / part['file']
        if not path.is_file() or fingerprint(path) != {key: part[key] for key in ('bytes', 'sha256')}:
            raise ValueError('Missing or damaged capture part: ' + str(path))
        # Verify again while consuming, including changes between the two reads.
        part_digest = hashlib.sha256()
        part_size = 0
        with path.open('rb') as source:
            for data in iter(lambda: source.read(1024 * 1024), b''):
                part_size += len(data)
                total += len(data)
                part_digest.update(data)
                digest.update(data)
                yield data
        if part_size != part['bytes'] or part_digest.hexdigest() != part['sha256']:
            raise ValueError('Capture part changed during reading: ' + str(path))
    if total != record['bytes'] or digest.hexdigest() != record['sha256']:
        raise ValueError('Reconstructed capture ZIP length/SHA mismatch')


def read_archive(directory, name='captures.zip'):
    """Read exact original ZIP bytes from raw storage or verified split storage."""
    directory = Path(directory)
    if name not in ARCHIVES:
        raise ValueError('Not a capture archive')
    if (directory / MANIFEST).exists():
        manifest = _manifest(directory)
        for record in manifest['archives']:
            if record['file'] == name:
                return b''.join(_chunks(directory, record))
    return (directory / name).read_bytes()


def reconstruct(directory, destination, name='captures.zip'):
    """Write an exact ZIP to a new destination, deleting incomplete output on error."""
    directory, destination = Path(directory), Path(destination)
    manifest = _manifest(directory)
    records = [record for record in manifest['archives'] if record['file'] == name]
    if len(records) != 1:
        raise ValueError('Capture archive not present in split storage: ' + name)
    created = False
    try:
        with destination.open('xb') as target:
            created = True
            for data in _chunks(directory, records[0]):
                target.write(data)
        return fingerprint(destination)
    except BaseException:
        if created:
            destination.unlink()
        raise


def pack(directory, threshold=90 * 1024 * 1024, chunk_size=CHUNK_BYTES):
    directory = Path(directory)
    if threshold < 0 or not 0 < chunk_size <= CHUNK_BYTES:
        raise ValueError('Invalid capture storage threshold/chunk size')
    manifest_path = directory / MANIFEST
    receipt_path = directory / 'validation.json'
    existing = _manifest(directory) if manifest_path.exists() else None
    records = list(existing['archives']) if existing else []
    for record in records:
        for _ in _chunks(directory, record):
            pass
        raw = directory / record['file']
        if raw.exists() and fingerprint(raw) != {key: record[key] for key in ('bytes', 'sha256')}:
            raise ValueError('Existing raw capture conflicts with split storage')
    known = {record['file'] for record in records}
    sources = [directory / name for name in ARCHIVES if name not in known and
               (directory / name).is_file() and (directory / name).stat().st_size > threshold]
    if not sources:
        # An interrupted deletion is safe to finish only after all parts and
        # every remaining raw ZIP have been checked against the same manifest.
        for record in records:
            (directory / record['file']).unlink(missing_ok=True)
        return existing
    receipt_bytes = receipt_path.read_bytes()
    receipt = json.loads(receipt_bytes.decode('utf-8-sig'))
    new_parts = []
    temporary = directory / (MANIFEST + '.tmp')
    temporary_created = False
    part_directory = directory / PART_DIRECTORY
    attributes = part_directory / '.gitattributes'
    directory_created = not part_directory.exists()
    attributes_created = False
    run_attributes = directory / '.gitattributes'
    previous_run_attributes = run_attributes.read_bytes() if run_attributes.exists() else None
    run_attributes_changed = False
    committed = False
    try:
        old_attributes = previous_run_attributes or b''
        if not old_attributes.endswith(RUN_ATTRIBUTES):
            separator = b'\n' if old_attributes and not old_attributes.endswith(b'\n') else b''
            run_attributes.write_bytes(old_attributes + separator + RUN_ATTRIBUTES)
            run_attributes_changed = True
        part_directory.mkdir(exist_ok=True)
        if attributes.exists():
            if attributes.read_bytes() != ATTRIBUTES:
                raise ValueError('Refusing to change existing capture Git attributes')
        else:
            with attributes.open('xb') as output:
                attributes_created = True
                output.write(ATTRIBUTES)
        for source in sources:
            original = fingerprint(source)
            expected = receipt.get('captureArchiveSha256') if source.name == 'captures.zip' else None
            if expected is not None and original['sha256'] != expected:
                raise ValueError('Raw capture ZIP differs from original validation receipt')
            record = dict(file=source.name, **original, chunkBytes=chunk_size, parts=[])
            with source.open('rb') as stream:
                for index, data in enumerate(iter(lambda: stream.read(chunk_size), b''), 1):
                    part = part_directory / (source.name + '.part' + str(index).zfill(4))
                    with part.open('xb') as target:
                        new_parts.append(part)
                        target.write(data)
                    record['parts'].append(dict(file=part.relative_to(directory).as_posix(), bytes=len(data),
                                                sha256=hashlib.sha256(data).hexdigest()))
            for _ in _chunks(directory, record):
                pass
            if fingerprint(source) != original:
                raise ValueError('Raw capture changed while splitting')
            records.append(record)
        if receipt_path.read_bytes() != receipt_bytes:
            raise ValueError('Validation receipt changed while splitting captures')
        result = dict(version=1, scope='lossless binary storage only; original ZIP and validation receipt bytes unchanged',
                      validationSha256=hashlib.sha256(receipt_bytes).hexdigest(), archives=records)
        with temporary.open('x', encoding='utf-8', newline='\n') as output:
            temporary_created = True
            output.write(json.dumps(result, indent=2) + '\n')
        temporary.replace(manifest_path)
        committed = True
        # The durable manifest is readable and every reconstructed ZIP has the
        # original SHA before any original ZIP is removed.
        saved = _manifest(directory)
        for record in saved['archives']:
            for _ in _chunks(directory, record):
                pass
        if receipt_path.read_bytes() != receipt_bytes:
            raise ValueError('Validation receipt changed before raw capture deletion')
        for record in saved['archives']:
            (directory / record['file']).unlink(missing_ok=True)
        return saved
    finally:
        if not committed:
            for part in new_parts:
                part.unlink(missing_ok=True)
            if temporary_created:
                temporary.unlink(missing_ok=True)
            if attributes_created:
                attributes.unlink(missing_ok=True)
            if directory_created and part_directory.exists() and not any(part_directory.iterdir()):
                part_directory.rmdir()
            if run_attributes_changed:
                if previous_run_attributes is None:
                    run_attributes.unlink(missing_ok=True)
                else:
                    run_attributes.write_bytes(previous_run_attributes)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('directory', type=Path)
    args = parser.parse_args()
    result = pack(args.directory)
    print('Capture storage: ' + ('unchanged' if result is None else
          'verified lossless parts; archives=' + str(len(result['archives']))))
