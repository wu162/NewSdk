"""Preserve decoded audio and resolve one-frame SWF export-name rebinding."""
import hashlib
from pathlib import Path
import re
import struct
import wave
import xml.etree.ElementTree as ET
import zlib


def export_bindings(swf):
    data = Path(swf).read_bytes()
    data = zlib.decompress(data[8:]) if data[:3] == b'CWS' else data[8:]
    pos = (5 + 4 * (data[0] >> 3) + 7) // 8 + 4
    frame = 0
    bindings = {}
    while pos < len(data):
        header = struct.unpack_from('<H', data, pos)[0]
        pos += 2
        length, code = header & 63, header >> 6
        if length == 63:
            length = struct.unpack_from('<I', data, pos)[0]
            pos += 4
        if code == 56:
            at = pos + 2
            for _ in range(struct.unpack_from('<H', data, pos)[0]):
                identifier = struct.unpack_from('<H', data, at)[0]
                at += 2
                end = data.index(0, at)
                name = data[at:end].decode('utf-8')
                at = end + 1
                bindings.setdefault(name, []).append({'id': identifier, 'frame': frame})
        if code == 1:
            frame += 1
        pos += length
        if code == 0:
            break
    return bindings


def normalize_xfl(root, swf):
    root = Path(root)
    report = {'soundSampleCountCorrections': [], 'shadowedExports': []}
    bindings = export_bindings(swf)
    # SWF permits repeated ExportAssets entries. Its last binding wins; FLA
    # requires one unique linkage name. Keep every shape and display reference.
    for path in sorted((root / 'LIBRARY').rglob('*.xml')):
        text = path.read_text(encoding='utf-8-sig')
        element = ET.fromstring(text)
        name = element.get('linkageIdentifier')
        item = re.fullmatch(r'Symbol (\d+)', element.get('name', ''))
        entries = bindings.get(name, [])
        if item and entries and len({e['id'] for e in entries}) > 1:
            if any(e['frame'] != 0 for e in entries):
                raise ValueError('Export rebinding across frames requires manual review: ' + name)
            if int(item[1]) != entries[-1]['id']:
                opening, remainder = text.split('>', 1)
                opening = re.sub(r'\s+linkage(?:Identifier|ExportForAS)="[^"]*"', '', opening)
                path.write_text(opening + '>' + remainder, encoding='utf-8')
                report['shadowedExports'].append({'identifier': name, 'keptCharacterId': entries[-1]['id'],
                                                   'unexportedCharacterId': int(item[1]), 'xml': path.relative_to(root).as_posix()})
    document = root / 'DOMDocument.xml'
    text = document.read_text(encoding='utf-8-sig')
    def sound(match):
        tag = match.group(0)
        element = ET.fromstring(tag if tag.endswith('/>') else tag[:-1] + '/>')
        href = element.get('href', '')
        audio = root / 'LIBRARY' / href
        if audio.suffix.lower() != '.wav' or not audio.is_file():
            return tag
        with wave.open(str(audio), 'rb') as stream:
            frames = stream.getnframes()
            frame_bytes = stream.getnchannels() * stream.getsampwidth()
        declared = int(element.get('sampleCount', frames))
        if declared == frames:
            return tag
        report['soundSampleCountCorrections'].append({'audio': audio.relative_to(root).as_posix(),
            'originalDeclaredSamples': declared, 'decodedPcmSamples': frames,
            'previouslyUnaccountedPcmBytes': (frames - declared) * frame_bytes,
            'wavSha256': hashlib.sha256(audio.read_bytes()).hexdigest(), 'pcmPreserved': True})
        return re.sub(r'sampleCount="[^"]*"', f'sampleCount="{frames}"', tag)
    changed = re.sub(r'<DOMSoundItem\b[^>]*>', sound, text)
    if changed != text:
        ET.fromstring(changed)
        document.write_text(changed, encoding='utf-8')
    return report
