"""Verify the published EXE works with only that EXE and SWF input present."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import struct
import subprocess
import tempfile
import zlib

parser = argparse.ArgumentParser()
parser.add_argument('exe', type=Path)
parser.add_argument('fixture', type=Path)
parser.add_argument('report', type=Path)
args = parser.parse_args()
exe = args.exe.resolve()
image = exe.read_bytes()
pe = struct.unpack_from('<I', image, 0x3C)[0]
optional = pe + 24
assert image[pe:pe + 4] == b'PE\0\0'
assert struct.unpack_from('<H', image, optional)[0] == 0x20B, 'Expected native x64 PE'
# No CLR directory: this is a native AOT image, not an IL assembly.
assert struct.unpack_from('<II', image, optional + 112 + 14 * 8) == (0, 0)
assert not list(exe.parent.glob('*.dll')), 'Publish directory contains runtime DLLs'
assert not list(exe.parent.glob('*.runtimeconfig.json'))
assert not list(exe.parent.glob('*.deps.json'))
fixture = args.fixture.read_bytes()
assert hashlib.sha256(fixture).hexdigest() == 'b661542cf2c8b52ef846c348f817f523f5fba63ee4baa53c365aabde30af9fa7'
with tempfile.TemporaryDirectory(prefix='newsdk-single-exe-') as temp:
    root = Path(temp)
    single = root / 'SwfCompiler.exe'
    shutil.copyfile(exe, single)
    source = root / 'CoronaUIMain.swf'
    source.write_bytes(fixture)
    env = {k: v for k, v in os.environ.items() if not k.startswith('DOTNET_')}
    env['PATH'] = str(Path(os.environ['SystemRoot']) / 'System32')
    env['DOTNET_MULTILEVEL_LOOKUP'] = '0'

    def run(arguments, code=0):
        p = subprocess.run([str(single), *arguments], cwd=root, env=env,
                           capture_output=True, text=True, timeout=20)
        assert p.returncode == code, (arguments, p.returncode, p.stdout, p.stderr)
        return p

    assert 'framework' in run(['--help']).stdout
    assert 'native ABI=1; native features=0' in run(['--version']).stdout
    info = json.loads(run(['inspect', str(source)]).stdout)
    assert (info['signature'], info['version'], info['uncompressedLength'],
            info['frameRate'], info['frameCount'], info['spriteCount']) == ('CWS', 7, 17830, 30, 1, 4)
    assert info['tagCounts'] == {'9': 1, '12': 1, '39': 4, '0': 5, '56': 4, '59': 4, '1': 1}
    source.write_bytes(b'F' + fixture[1:8] + zlib.decompress(fixture[8:]))
    plain = json.loads(run(['inspect', str(source)]).stdout)
    assert plain == dict(info, signature='FWS')
    # A skeleton must not create bogus APT outputs or return compilation success.
    assert 'has not been migrated' in run(['compile', str(source), '--ra3', '--optimize'], 4).stderr
    assert run(['inspect', str(source), '--unknown'], 2).stderr
    source.write_bytes(b'FWS')
    assert run(['inspect', str(source)], 3).stderr
    assert {p.name for p in root.iterdir()} == {'SwfCompiler.exe', 'CoronaUIMain.swf'}
evidence = {
    'sourceCommit': os.getenv('GITHUB_SHA'), 'runId': os.getenv('GITHUB_RUN_ID'),
    'exeSha256': hashlib.sha256(image).hexdigest(), 'exeBytes': len(image),
    'nativeAot': True, 'runtimeFilesRequired': ['SwfCompiler.exe'],
    'nativeAbi': 1, 'nativeFeatures': 0, 'standaloneInspectionPassed': True,
    'cwsAndFwsPassed': True, 'compileExplicitlyUnavailable': True,
    'conversionImplemented': False, 'sdkIntegration': False,
}
args.report.parent.mkdir(parents=True, exist_ok=True)
args.report.write_text(json.dumps(evidence, indent=2) + '\n')
print(json.dumps(evidence, indent=2))
