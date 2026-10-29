#!/usr/bin/env python3
"""Audit complete saved original DIB plugin replay bytes; no codec execution."""
import argparse
from collections import Counter
import hashlib
import io
import json
from pathlib import Path
import struct
import zipfile


def sha(data):
    return hashlib.sha256(data).hexdigest()


class Replay:
    def __init__(self, data):
        self.stream = io.BytesIO(data)

    def read(self, count):
        assert 0 <= count <= 16777216
        data = self.stream.read(count)
        assert len(data) == count, 'Truncated replay field'
        return data

    def integer(self):
        return struct.unpack('<i', self.read(4))[0]

    def string(self):
        length = 0
        for shift in range(0, 35, 7):
            value = self.read(1)[0]
            length |= (value & 127) << shift
            if value < 128:
                return self.read(length).decode('utf-8')
        raise AssertionError('Invalid .NET string length')

    def blob(self):
        length = self.integer()
        return None if length == -1 else self.read(length)

    def boolean(self):
        value = self.read(1)[0]
        assert value in (0, 1)
        return bool(value)


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/dib-images' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['status'] == 'passed' and receipt['sourceCommit'] == source and str(receipt['runId']) == run
    assert not receipt.get('probeOnly') and not receipt.get('sdkValidated')
    modes = receipt['configurations']
    assert len(modes) == 2 and {m['configuration'] for m in modes} == {'Debug', 'Release'}
    assert all(m['passed'] is True and m['withoutOriginalTextureCompiler'] is True for m in modes)
    archive = (directory / 'corpus.zip').read_bytes()
    assert sha(archive) == receipt['corpusSha256']
    with zipfile.ZipFile(io.BytesIO(archive)) as files:
        names = [n for n in files.namelist() if not n.endswith('/')]
        assert len(names) == len(set(names)) == 2
        assert set(names) == {'Debug-pipeline.replay', 'Release-pipeline.replay'}
        debug = files.read('Debug-pipeline.replay')
        assert debug == files.read('Release-pipeline.replay'), 'Full original D/R corpus differs'
    reader = Replay(debug)
    assert reader.integer() == 50304
    outcomes, extensions, roots, depths, tables = (Counter() for _ in range(5))
    input_files, payloads = {}, set()
    compared = 0
    for ordinal in range(50304):
        ext, image, kind, fmt = reader.string(), reader.blob(), reader.string(), reader.string()
        stale, mip, texture_type = reader.boolean(), reader.boolean(), reader.string()
        error = reader.string()
        instance, relocation, imports, cdata = [reader.blob() for _ in range(4)]
        assert ext in {'bmp', 'dib', 'tga'} and image
        assert kind in {'Texture', 'OnDemandTexture', 'TerrainTextureAtlas'}
        assert texture_type in {'StandardTexture', 'VolumeTexture'}
        if image.startswith(b'BM'):
            dib = image[14:]
        else:
            dib = image
        header = struct.unpack_from('<I', dib)[0]
        assert header in {12, 40, 52, 56, 64, 108, 124}
        depth = struct.unpack_from('<H', dib, 10 if header == 12 else 14)[0]
        table = 0 if header == 12 else struct.unpack_from('<I', dib, 32)[0]
        depths[depth] += 1
        if depth > 8:
            tables[table] += 1
        payloads.add(sha(dib))
        input_files[(ext, sha(image))] = len(image)
        extensions[ext] += 1
        roots[kind] += 1
        if error:
            assert instance is relocation is imports is None
            if error.startswith('System.NotSupportedException/'):
                outcomes['explicitRefusal'] += 1
            else:
                assert error.startswith('BinaryAssetBuilder.BinaryAssetBuilderException/')
                outcomes['originalError'] += 1
            if kind == 'TerrainTextureAtlas':
                raise AssertionError('Atlas must actually succeed')
        else:
            outcomes['success'] += 1
            assert relocation is not None and imports == b''
            assert instance is not None
            if kind == 'Texture':
                assert struct.unpack_from('<I', instance, 4)[0] == 12 and instance[12:16] == b'DDS '
                assert (cdata is not None) == stale
            elif kind == 'OnDemandTexture':
                assert len(instance) == 8 and cdata is not None and cdata[40:44] == b'DDS '
            else:
                assert struct.unpack_from('<I', instance, 4)[0] == 1 and instance[28:32] == b'DDS '
                assert fmt in {'normal', 'base-only'} and not mip
                assert (struct.unpack_from('<I', instance, 24)[0] == 0) == (fmt == 'base-only')
        compared += sum(len(b) for b in (instance, relocation, imports, cdata) if b is not None)
    assert reader.stream.read(1) == b'', 'Unaccounted replay bytes'
    assert outcomes == {'success': 46256, 'originalError': 3952, 'explicitRefusal': 96}
    assert roots['TerrainTextureAtlas'] == extensions['tga'] == 432
    assert set(depths) == {1, 4, 8, 16, 24, 32} and set(tables) == {0, 1, 3, 17}
    return dict(scope='independent complete recorded original replay/source/layout audit; no compiler/codec execution; not SDK acceptance',
                sourceCommit=source, runId=run, corpusSha256=sha(archive),
                originalConfigurationsByteIdentical=True, originalReplayBytes=len(debug), originalReplaySha256=sha(debug),
                fixturesPerConfiguration=50304, outcomes=dict(outcomes), roots=dict(roots), extensions=dict(extensions),
                sourceDepthCases=dict(depths), sourceTableCases=dict(tables), distinctSourceFiles=len(input_files),
                distinctDibPayloads=len(payloads), completeExpectedBufferBytesPerConfiguration=compared,
                sourceFiles=[dict(extension=k[0], sha256=k[1], bytes=v) for k, v in sorted(input_files.items())])


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
    print('Audited complete original DIB plugin replays: fixtures=50304 positiveAtlas=432 per configuration')
