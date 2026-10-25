#!/usr/bin/env python3
"""Selector regression tests use synthetic evidence and never run compiler matrices."""
import hashlib
import importlib.util
import json
import subprocess
import tempfile
import unittest
from pathlib import Path

SELECTOR = Path(__file__).resolve().parents[1] / 'scripts' / 'SelectXmlComponentGroups.py'


class RepositoryFixture:
    def __init__(self, root):
        self.root = Path(root)
        self.git('init', '-q')
        self.git('config', 'user.name', 'Selector Test')
        self.git('config', 'user.email', 'selector@example.invalid')
        self.write('SDK/tools/BinaryAssetBuilder.XmlCompiler.dll', b'fixed original fixture')
        self.write('SDK/tools/BinaryAssetBuilder.Core.dll', b'core fixture')
        self.write('recovery/src/Native/Relocation.cpp', '// shared relocation\n')
        self.write('recovery/src/Native/Relocation.h', '// shared relocation header\n')
        self.write('recovery/src/Native/SharedDetail.inc', '// included detail\n')
        self.write('recovery/src/Bridge/XmlNodeAdapter.h', '// adapter\n')
        for number, group in enumerate(('alpha', 'beta'), 1):
            self.group(group, number)
        self.write('recovery/scripts/TestXmlPriorityLink.ps1', "recovery/src/Native/Relocation.cpp\n")
        self.write('recovery/tests/XmlPriorityLinkOracle.cs', '// combined oracle\n')
        self.source_commit = self.commit('initial source')
        for group in ('alpha', 'beta'):
            self.evidence(group)
        self.evidence_commit = self.commit('record valid evidence')

    def git(self, *args):
        return subprocess.check_output(['git', '-C', str(self.root), *args], text=True).strip()

    def write(self, path, content):
        path = self.root / path
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(content if isinstance(content, bytes) else content.encode())

    def read(self, path):
        return (self.root / path).read_text()

    def commit(self, message):
        self.git('add', '.')
        self.git('commit', '-qm', message)
        return self.git('rev-parse', 'HEAD')

    def group(self, group, number):
        self.write(f'recovery/src/Native/Xml{group}.cpp', f'#include "Xml{group}.h"\n// native {group}\n')
        self.write(f'recovery/src/Native/Xml{group}.h', f'#include "SharedDetail.inc"\n// header {group}\n')
        self.write(f'recovery/src/Bridge/Xml{group}Compiler.cpp', f'#include "../Native/Xml{group}.h"\n#include "XmlNodeAdapter.h"\n')
        self.write(f'recovery/tests/Xml{group}Oracle.cs', f'// test {group}\n')
        script = "\n".join([
            "$sources = @(",
            "'recovery/src/Native/Relocation.cpp',",
            f"'recovery/src/Bridge/Xml{group}Compiler.cpp'",
            ")",
            f"$sources += @(Get-ChildItem 'recovery/src/Native/Xml{group}*.cpp' -File)",
            f"$oracleSources = @((Resolve-Path 'recovery/tests/Xml{group}Oracle.cs').Path)",
            "Copy-Item SDK/tools/*.dll $out",
        ])
        self.write(f'recovery/scripts/TestXml{group}.ps1', script)
        self.write(f'recovery/catalog/Xml{group}Types.json', json.dumps({
            'componentOnly': True, 'sdkIntegrated': False,
            'roots': [{'name': group.title() + 'Root', 'typeId': number, 'typeHash': number + 10}]
        }))

    def evidence(self, group, **changes):
        row = {'succeeded': True, 'sourceCommit': self.source_commit, 'runId': '12345',
               'platforms': [0, 1, 2, 99], 'modes': []}
        original_hash = hashlib.sha256(b'fixed original fixture').hexdigest()
        for mode in ('/Od', '/O2'):
            result = {'succeeded': True, 'originalDllSha256': original_hash,
                      'platforms': [{'platform': p, 'fixtures': 2, 'arrayComparisons': 12,
                                     'rejections': 1, 'layoutAssertions': 2} for p in (0, 1, 2, 99)]}
            row['modes'].append({'mode': mode, 'result': result})
        row.update(changes)
        self.write(f'recovery/evidence/xml-{group}/validation.json', json.dumps(row))

class SelectXmlGroupsTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        spec = importlib.util.spec_from_file_location('xml_group_selector_tests', SELECTOR)
        cls.selector = importlib.util.module_from_spec(spec)
        import sys
        sys.modules[spec.name] = cls.selector
        spec.loader.exec_module(cls.selector)

    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix='xml-selector-tests-')
        self.addCleanup(self.temporary.cleanup)
        self.repo = RepositoryFixture(self.temporary.name)
        self.repo.evidence('priority-link')
        self.repo.commit('record accepted combined link')

    def select(self):
        return self.selector.select(self.repo.root)

    def changed(self, path, content='// changed\n'):
        self.repo.write(path, content)
        self.repo.commit('change ' + path)
        return self.select()

    def assert_groups(self, expected, result):
        self.assertEqual(sorted(expected), sorted(result['groups']), result)

    def test_unchanged_verified_groups_are_reused(self):
        result = self.select()
        self.assert_groups([], result)
        self.assertEqual({'alpha', 'beta'}, {r['group'] for r in result['reused']})
        for row in result['reused']:
            self.assertEqual(self.repo.source_commit, row['sourceCommit'])
            self.assertEqual('12345', str(row['runId']))
        self.assertFalse(result['link_ready'], result)
        self.assertEqual([], result['link_roots'])

    def test_new_group_without_evidence_runs_only_new_group(self):
        self.repo.group('gamma', 3)
        self.repo.commit('add gamma')
        result = self.select()
        self.assert_groups(['gamma'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual(['GammaRoot'], result['link_roots'])

    def test_failed_evidence_is_not_reused(self):
        self.repo.evidence('alpha', succeeded=False)
        self.repo.commit('failed alpha evidence')
        result = self.select()
        self.assert_groups(['alpha'], result)
        self.assertIn('alpha', result['reasons'])

    def test_missing_evidence_is_not_reused(self):
        (self.repo.root / 'recovery/evidence/xml-alpha/validation.json').unlink()
        self.repo.commit('remove alpha evidence')
        self.assert_groups(['alpha'], self.select())

    def test_unknown_source_commit_fails_closed(self):
        self.repo.evidence('alpha', sourceCommit='f' * 40)
        self.repo.commit('unknown evidence anchor')
        self.assert_groups(['alpha'], self.select())

    def test_missing_source_commit_fails_closed(self):
        path = 'recovery/evidence/xml-alpha/validation.json'
        document = json.loads(self.repo.read(path))
        del document['sourceCommit']
        self.repo.write(path, json.dumps(document))
        self.repo.commit('missing evidence anchor')
        self.assert_groups(['alpha'], self.select())

    def test_single_mode_acceptance_fails_closed(self):
        path = 'recovery/evidence/xml-alpha/validation.json'
        document = json.loads(self.repo.read(path))
        document['modes'] = document['modes'][:1]
        self.repo.write(path, json.dumps(document))
        self.repo.commit('incomplete optimization modes')
        self.assert_groups(['alpha'], self.select())

    def test_missing_mode_success_flags_fail_closed(self):
        path = 'recovery/evidence/xml-alpha/validation.json'
        document = json.loads(self.repo.read(path))
        document['modes'] = [{'mode': '/Od'}, {'mode': '/O2'}]
        self.repo.write(path, json.dumps(document))
        self.repo.commit('unproven optimization modes')
        self.assert_groups(['alpha'], self.select())

    def test_nonancestor_source_commit_fails_closed(self):
        tree = self.repo.git('rev-parse', self.repo.source_commit + '^{tree}')
        detached = self.repo.git('commit-tree', tree, '-m', 'unrelated source anchor')
        self.repo.evidence('alpha', sourceCommit=detached)
        self.repo.commit('record nonancestor evidence anchor')
        self.assert_groups(['alpha'], self.select())

    def test_failed_mode_acceptance_fails_closed(self):
        path = 'recovery/evidence/xml-alpha/validation.json'
        document = json.loads(self.repo.read(path))
        document['modes'][1]['result']['succeeded'] = False
        self.repo.write(path, json.dumps(document))
        self.repo.commit('failed optimization mode')
        self.assert_groups(['alpha'], self.select())

    def test_group_native_change_is_selective_and_links_affected_root(self):
        result = self.changed('recovery/src/Native/Xmlalpha.cpp', '#include "Xmlalpha.h"\n// alpha update\n')
        self.assert_groups(['alpha'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual(['AlphaRoot'], result['link_roots'])

    def test_shared_native_helper_change_selects_all_affected_groups(self):
        result = self.changed('recovery/src/Native/Relocation.cpp')
        self.assert_groups(['alpha', 'beta'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual({'AlphaRoot', 'BetaRoot'}, set(result['link_roots']))

    def test_transitive_external_include_selects_only_actual_consumer(self):
        self.repo.write('recovery/src/Native/ExternalInner.inc', '// external original\n')
        self.repo.write('recovery/src/Native/ExternalOuter.h', '#include "ExternalInner.inc"\n')
        self.repo.write('recovery/src/Native/Xmlalpha.h', '#include "ExternalOuter.h"\n')
        source = self.repo.commit('introduce transitive alpha dependency')
        self.repo.source_commit = source
        self.repo.evidence('alpha')
        self.repo.evidence('beta')
        self.repo.evidence('priority-link')
        self.repo.commit('accept dependency source')
        result = self.changed('recovery/src/Native/ExternalInner.inc')
        self.assert_groups(['alpha'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual(['AlphaRoot'], result['link_roots'])

    def test_external_header_companion_cpp_invalidates_consumer(self):
        self.repo.write('recovery/src/Native/ExternalLibrary.h', '// declaration\n')
        self.repo.write('recovery/src/Native/ExternalLibrary.cpp', '// original implementation\n')
        self.repo.write('recovery/src/Native/Xmlalpha.h', '#include "ExternalLibrary.h"\n')
        self.repo.source_commit = self.repo.commit('add linked external implementation')
        self.repo.evidence('alpha')
        self.repo.evidence('beta')
        self.repo.evidence('priority-link')
        self.repo.commit('accept linked external implementation')
        result = self.changed('recovery/src/Native/ExternalLibrary.cpp')
        self.assert_groups(['alpha'], result)
        self.assertEqual(['AlphaRoot'], result['link_roots'])

    def test_test_only_change_runs_group_without_relink(self):
        result = self.changed('recovery/tests/XmlalphaOracle.cs')
        self.assert_groups(['alpha'], result)
        self.assertFalse(result['link_ready'], result)
        self.assertEqual([], result['link_roots'])

    def test_script_only_change_runs_group_without_relink(self):
        path = 'recovery/scripts/TestXmlalpha.ps1'
        result = self.changed(path, self.repo.read(path) + '\n# test runner update\n')
        self.assert_groups(['alpha'], result)
        self.assertFalse(result['link_ready'], result)
        self.assertEqual([], result['link_roots'])

    def test_test_and_link_harness_change_runs_only_selected_root(self):
        self.repo.write('recovery/tests/XmlalphaOracle.cs', '// changed alpha test\n')
        self.repo.write('recovery/tests/XmlPriorityLinkOracle.cs', '// changed link harness\n')
        self.repo.commit('change alpha test and combined harness')
        result = self.select()
        self.assert_groups(['alpha'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual(['AlphaRoot'], result['link_roots'])

    def test_dll_change_invalidates_all_component_acceptance(self):
        result = self.changed('SDK/tools/BinaryAssetBuilder.XmlCompiler.dll', b'changed original dll')
        self.assert_groups(['alpha', 'beta'], result)
        self.assertTrue(result['link_ready'], result)

    def test_unrelated_document_change_reuses_both_groups(self):
        result = self.changed('recovery/NOTES.md', 'A documentation-only update.\n')
        self.assert_groups([], result)
        self.assertFalse(result['link_ready'], result)
        self.assertEqual([], result['link_roots'])

    def test_deleted_group_source_is_not_reused(self):
        (self.repo.root / 'recovery/src/Native/Xmlalpha.cpp').unlink()
        self.repo.commit('delete alpha native source')
        self.assert_groups(['alpha'], self.select())

    def test_deleted_external_transitive_include_is_not_reused(self):
        self.repo.write('recovery/src/Native/ExternalInner.inc', '// external original\n')
        self.repo.write('recovery/src/Native/ExternalOuter.h', '#include "ExternalInner.inc"\n')
        self.repo.write('recovery/src/Native/Xmlalpha.h', '#include "ExternalOuter.h"\n')
        self.repo.source_commit = self.repo.commit('introduce accepted external dependency')
        self.repo.evidence('alpha')
        self.repo.evidence('beta')
        self.repo.evidence('priority-link')
        self.repo.commit('accept external dependency')
        (self.repo.root / 'recovery/src/Native/ExternalInner.inc').unlink()
        self.repo.commit('delete external transitive dependency')
        result = self.select()
        self.assert_groups(['alpha'], result)
        self.assertTrue(result['link_ready'], result)
        self.assertEqual(['AlphaRoot'], result['link_roots'])

    def test_missing_include_fails_closed_for_its_group(self):
        (self.repo.root / 'recovery/src/Native/Xmlalpha.h').unlink()
        self.repo.commit('remove alpha include dependency')
        self.assert_groups(['alpha'], self.select())


if __name__ == '__main__':
    unittest.main()
