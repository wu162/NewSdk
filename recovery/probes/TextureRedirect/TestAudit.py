"""Exercise audit rejection with a protected native-byte mutation on a temp copy."""
import importlib.util
import json
import pathlib
import tempfile
import dnfile

root = pathlib.Path(__file__).resolve().parents[3]
spec = importlib.util.spec_from_file_location('audit', pathlib.Path(__file__).with_name('AuditPe.py'))
audit = importlib.util.module_from_spec(spec)
spec.loader.exec_module(audit)
original = root / 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll'
assert audit.sha(original.read_bytes()) == '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
result = audit.audit(original, original, 'roundtrip')
assert result['status'] == 'passed', result['checks']
# A redirect is required to change exactly the one intended AssemblyRef.
assert not audit.audit(original, original, 'redirect')['checks']['AssemblyRef-change-allowlist']
pe = dnfile.dnPE(str(original))
row = next(r for r in pe.net.mdtables.MethodDef if r.Rva and r.ImplFlags.miNative)
with tempfile.TemporaryDirectory() as temporary:
    changed = pathlib.Path(temporary) / 'mutated.dll'
    data = bytearray(original.read_bytes())
    data[pe.get_offset_from_rva(row.Rva)] ^= 1
    changed.write_bytes(data)
    rejection = audit.audit(original, changed, 'roundtrip')
    assert rejection['status'] == 'failed'
    assert not rejection['checks']['native-MethodDef-ranges']
print('PASS PE audit: identical original accepted; missing redirect and native-byte corruption rejected')
