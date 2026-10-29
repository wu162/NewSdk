#!/usr/bin/env python3
"""Audit every complete saved original PPM plugin replay, without executing codecs."""
import argparse
from collections import Counter
import hashlib
import json
from pathlib import Path
import struct
import zipfile

from InspectTextureDibArchive import Replay


def audit(root, run, source):
    directory = root / 'recovery/evidence/texture/ppm-images' / (run + '-1')
    receipt = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert receipt['status'] == 'passed' and receipt['sourceCommit'] == source and str(receipt['runId']) == run
    modes = receipt['configurations']
    assert len(modes) == 2 and {m['configuration'] for m in modes} == {'Debug', 'Release'}
    assert all(m['passed'] is True and m['withoutOriginalTextureCompiler'] is True for m in modes)
    path = directory / 'corpus.zip'
    digest = lambda data: hashlib.sha256(data).hexdigest()
    assert digest(path.read_bytes()) == receipt['corpusSha256']
    outcomes, roots, formats, signatures = (Counter() for _ in range(4))
    sources, compared = {}, 0
    with zipfile.ZipFile(path) as archive:
        assert archive.namelist() == ['Debug-pipeline.replay', 'Release-pipeline.replay']
        with archive.open('Debug-pipeline.replay') as debug, archive.open('Release-pipeline.replay') as release:
            while True:
                part = debug.read(65536)
                assert part == release.read(65536), 'Original Debug/Release replay differs'
                if not part:
                    break
        reader = Replay(archive.open('Debug-pipeline.replay'))
        assert reader.integer() == 41120
        for ordinal in range(41120):
            ext, image, root_name, fmt = reader.string(), reader.blob(), reader.string(), reader.string()
            stale, mip, kind = reader.boolean(), reader.boolean(), reader.string()
            error = reader.string()
            instance, relo, imports, cdata = [reader.blob() for _ in range(4)]
            assert image and image[:2] in (b'P2', b'P3', b'P5', b'P6')
            assert ext == ('tga' if root_name == 'TerrainTextureAtlas' else 'ppm')
            assert kind in ('StandardTexture', 'VolumeTexture')
            sources[(ext, digest(image))] = len(image)
            roots[root_name] += 1
            formats[fmt] += 1
            signatures[image[:2].decode()] += 1
            if error:
                assert instance is relo is imports is None and root_name != 'TerrainTextureAtlas'
                if error.startswith('System.NotSupportedException/'):
                    assert fmt in ('DXT1', 'DXT1A')
                    outcomes['explicitRefusal'] += 1
                else:
                    assert error.startswith('BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/')
                    outcomes['originalError'] += 1
                assert (cdata is not None) == stale
            else:
                outcomes['success'] += 1
                assert instance is not None and relo is not None and imports == b''
                if root_name == 'Texture':
                    assert struct.unpack_from('<I', instance, 4)[0] == 12 and instance[12:16] == b'DDS '
                    assert (cdata is not None) == stale
                elif root_name == 'OnDemandTexture':
                    assert len(instance) == 8 and cdata is not None and cdata[40:44] == b'DDS '
                else:
                    assert root_name == 'TerrainTextureAtlas' and not mip
                    assert struct.unpack_from('<I', instance, 4)[0] == 1 and instance[28:32] == b'DDS '
                    assert fmt in ('normal', 'base-only')
                    assert (struct.unpack_from('<I', instance, 24)[0] == 0) == (fmt == 'base-only')
            compared += sum(len(b) for b in (instance, relo, imports, cdata) if b is not None)
        assert reader.stream.read(1) == b''
        reader.stream.close()
    assert outcomes == {'success': 31424, 'originalError': 9664, 'explicitRefusal': 32}
    assert roots == {'Texture': 20528, 'OnDemandTexture': 20528, 'TerrainTextureAtlas': 64}
    return dict(scope='independent full saved original source/buffer/layout audit; not SDK acceptance',
                sourceCommit=source, runId=run, corpusSha256=receipt['corpusSha256'],
                fixturesPerConfiguration=41120, originalConfigurationsByteIdentical=True,
                outcomes=dict(outcomes), roots=dict(roots), formats=dict(formats), signatures=dict(signatures),
                completeExpectedBufferBytesPerConfiguration=compared,
                sourceFiles=[dict(extension=key[0], sha256=key[1], bytes=size) for key, size in sorted(sources.items())])


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
    print('Audited complete PPM plugin source/buffers: fixtures=41120 positiveAtlas=64 per configuration')
