"""Linked-PDB source fingerprints plus original/rebuilt CLI output comparison.

Original managed assemblies are extracted unchanged from the uploaded EXE.
Both variants run on the same .NET8 x86 host; original .NET5 native hosting is
not reproduced. Compare ALL generated files at identical logical paths.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def compare_sources(vendor, manifest):
    result = {}
    errors = []
    for name, project in manifest['projects'].items():
        if not project['pdbGuidMatchesBundledAssembly']:
            raise ValueError('PDB is not linked to the uploaded assembly')
        count = 0
        for row in project['sourceFiles']:
            path = vendor / row['path']
            actual = sha(path) if path.is_file() else None
            if actual != row['sha256']:
                errors.append({'path': row['path'], 'expected': row['sha256'], 'actual': actual})
            else:
                count += 1
        result[name] = {'matched': count, 'total': len(project['sourceFiles']),
                        'excludedGeneratedDocuments': project['excludedGeneratedDocuments']}
    if errors:
        raise ValueError('Source mismatch: ' + json.dumps(errors, indent=2))
    return result


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--vendor', type=Path, required=True)
    parser.add_argument('--cli', type=Path)
    parser.add_argument('--original', type=Path)
    parser.add_argument('--report', type=Path, required=True)
    args = parser.parse_args()
    ui = Path(__file__).resolve().parents[1]
    manifest = json.loads((ui / 'reference/uploaded-tool-source-checksums.json').read_text())
    evidence = {'sourceCommit': os.getenv('GITHUB_SHA'), 'runId': os.getenv('GITHUB_RUN_ID'),
                'upstreamCommit': manifest['upstreamCommit'],
                'method': manifest['method'], 'sourceFiles': compare_sources(args.vendor, manifest),
                'originalManagedAssemblyExecuted': False, 'sdkIntegration': False}
    if args.cli:
        previous = json.loads((ui / 'evidence/validation.json').read_text())
        fixture = ui / 'fixtures/CoronaUIMain.swf'
        if sha(fixture) != previous['fixtureSha256']:
            raise ValueError('Fixture fingerprint mismatch')
        cli = args.cli.resolve()
        original = args.original.resolve()
        original_manifest = json.loads((original / 'manifest.json').read_text())
        for name, fingerprint in original_manifest['files'].items():
            if sha(original / name) != fingerprint:
                raise ValueError('Original tool package changed: ' + name)
        original.joinpath('eaf2apt.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {
            'tfm': 'net8.0', 'framework': {'name': 'Microsoft.NETCore.App', 'version': '8.0.0'},
            'rollForward': 'LatestPatch'}}))
        dotnet = os.environ['DOTNET_X86']
        results = []
        with tempfile.TemporaryDirectory(prefix='newsdk-ui-direct-') as temp:
            root = Path(temp)
            for optimize in (False, True):
                case = root / ('optimized' if optimize else 'plain')
                case.mkdir()
                source = case / fixture.name
                shutil.copyfile(fixture, source)
                def run(assembly, original_tool=False):
                    command = [dotnet, str(assembly), str(source), '--ra3']
                    if optimize:
                        command.append('--optimize')
                    env = os.environ.copy()
                    if original_tool:
                        env['PATH'] = str(original) + os.pathsep + env['PATH']
                    completed = subprocess.run(command, cwd=case, env=env, capture_output=True, text=True, timeout=90)
                    if completed.returncode:
                        raise RuntimeError(f'Tool failed ({assembly}): {completed.stdout}\n{completed.stderr}')
                    print(completed.stdout)
                    return {str(p.relative_to(case)).replace('\\', '/'): p.read_bytes()
                            for p in case.rglob('*') if p.is_file() and p != source}
                expected_outputs = run(original / 'eaf2apt.dll', original_tool=True)
                shutil.rmtree(case)
                case.mkdir()
                shutil.copyfile(fixture, source)
                actual_outputs = run(cli)
                if expected_outputs != actual_outputs:
                    differences = sorted(k for k in expected_outputs.keys() | actual_outputs.keys()
                                         if expected_outputs.get(k) != actual_outputs.get(k))
                    raise ValueError('Original/rebuilt output differs: ' + json.dumps(differences))
                files = {}
                for ext, field in [('.apt', 'aptSha256'), ('.const', 'constSha256'), ('.dat', 'datSha256')]:
                    output = source.with_suffix(ext)
                    expected = previous['originalToolOptimizedRa3'][ext]['sha256'] if optimize else previous[field]
                    actual = sha(output)
                    if actual != expected:
                        raise ValueError(f'Output differs from pinned compiled-source baseline: {optimize} {ext}')
                    files[ext] = {'bytes': output.stat().st_size, 'sha256': actual}
                if not source.with_suffix('.xml').is_file():
                    raise ValueError('CLI did not produce the declaration XML')
                results.append({'ra3': True, 'optimize': optimize, 'files': files,
                                'allGeneratedFilesEqual': True, 'comparedFiles': sorted(actual_outputs)})
        evidence['directCliResults'] = results
        evidence['originalManagedAssemblyExecuted'] = True
        evidence['outputBaseline'] = 'unchanged assemblies and flasm extracted from uploaded tool; all generated files compared byte-for-byte at identical paths under same .NET8 x86 runtime'
        evidence['adoptSource'] = True
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(evidence, indent=2) + '\n')
    print(json.dumps(evidence, indent=2))


if __name__ == '__main__':
    main()
