#!/usr/bin/env python3
"""Audit complete saved native ABI records against independent fixture models; no DLL calls."""
import hashlib
import json
from pathlib import Path
import struct

ROOT = Path(__file__).resolve().parents[2]


def sha(b):
    return hashlib.sha256(b).hexdigest()


def statuses(data):
    rows = json.loads((ROOT / 'recovery/evidence/texture/native-errors-static/20261029/mapping.json').read_text())
    mapping = {r['code']: (r['text'] + '\0').encode('ascii') for r in rows['mapping']}
    codes = list(range(-4096, 4097))
    signed = lambda x: x if x < 0x80000000 else x - 0x100000000
    for i in range(32):
        mask = 1 << i
        codes += [signed(mask), signed(mask ^ 0xffffffff)]
    codes += [signed(n) for n in (0x7fffffff, 0x80000000, 0x80000064, 0xffffff9c)]
    state = 0x426cab18
    for _ in range(4096):
        state = (state * 1664525 + 1013904223) & 0xffffffff
        codes.append(signed(state))
    assert len(codes) == 12357 and data[:8] == b'TXERR001'
    assert struct.unpack_from('<I', data, 8)[0] == len(codes)
    offset = 12
    for code in codes:
        actual, length = struct.unpack_from('<iI', data, offset)
        offset += 8
        expected = mapping.get(code, (rows['defaultText'] + '\0').encode('ascii'))
        assert actual == code and length == len(expected) and data[offset:offset+length] == expected
        offset += length
    assert offset == len(data)
    return dict(records=12357, nativeStatusTableEntries=101, fullStringAndNulBytes=len(data) - 12 - 8 * len(codes))


def memory(data):
    expected = bytearray()
    def buffer(payload, capacity):
        expected.extend(struct.pack('<II', len(payload), capacity))
        expected.extend(payload)
    for initial in (1, 2, 3, 7, 16, 31, 64, 255, 1024, 65536):
        payload, capacity = bytearray(), initial
        buffer(payload, capacity)
        for round_number in range(4):
            for count in (0, 1, 2, 3, 7, 15, 31, 64, 128, 513, 4097, 0, 17):
                payload.extend((i * 13 + count * 17 + round_number * 19) & 255 for i in range(count))
                while capacity < len(payload):
                    capacity *= 2
                buffer(payload, capacity)
            payload.clear()
            buffer(payload, capacity)
        buffer(b'', 0)
    for flags in range(8):
        buffer(bytes((i * 29) & 255 for i in range(64)), 64)
        expected.extend(struct.pack('<I', flags))
    for n in range(65536):
        for value in (n, n << 16, (n << 8) ^ 0xa50000ff, (n << 16) | n):
            expected.extend(value.to_bytes(4, 'big'))
    expected.extend(struct.pack('<IIIII', 1, 1, 1, 1, 1))
    assert data == expected, 'Complete native memory trace differs from independent fixture model'
    return dict(bufferSequences=10, appendOperations=520, deletingDestructorFlags=8, endianValues=262144, fullTraceBytes=len(data))


def audit(group, run, source, model):
    directory = ROOT / 'recovery/evidence/texture' / group / (run + '-1')
    r = json.loads((directory / 'validation.json').read_text(encoding='utf-8-sig'))
    assert r['sourceCommit'] == source and str(r['runId']) == run and r['status'] == 'passed' and not r['sdkValidated']
    assert r['originalSha256'] == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
    assert len(r['configurations']) == 2 and {m['configuration'] for m in r['configurations']} == {'Debug', 'Release'}
    digest, result = None, None
    for m in r['configurations']:
        assert m['passed'] and m['withoutOriginalTextureCompiler']
        b = (directory / (m['configuration'] + '.capture')).read_bytes()
        assert sha(b) == m['captureSha256']
        assert digest is None or digest == sha(b)
        digest = sha(b)
        result = model(b)
        dlls = list(directory.glob(m['configuration'] + '-Recovered.*.dll'))
        assert len(dlls) == 1 and sha(dlls[0].read_bytes()) == m['nativeSha256']
    return dict(group=group, sourceCommit=source, runId=run, completeDebugReleaseCapturesIdentical=True, captureSha256=digest, **result)


if __name__ == '__main__':
    results = [audit('native-errors', '40889408253', '2093ea3d6aac8c8a89c83670a41570d2d5c00f86', statuses),
               audit('native-memory', '40890508256', 'e4c67dd3597c8405d0a0a2211383d9764681f180', memory)]
    out = ROOT / 'recovery/evidence/texture/handoffs/20261029-native-abi/independent-archive-audit.json'
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(dict(scope='independent complete saved native captures/fixtures/hashes; no DLL calls or full native ABI claim', groups=results), indent=2) + '\n')
    print(json.dumps(results))
