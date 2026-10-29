import importlib.util
import json
from pathlib import Path
import subprocess
import tempfile
import unittest

spec=importlib.util.spec_from_file_location('selector',Path(__file__).parents[1]/'scripts/SelectTextureGroups.py')
selector=importlib.util.module_from_spec(spec);spec.loader.exec_module(selector)

class SelectionTests(unittest.TestCase):
    def setUp(self):
        self.temp=tempfile.TemporaryDirectory();self.root=Path(self.temp.name)
        self.git('init','-q');self.git('config','user.name','Test');self.git('config','user.email','test@example.invalid')
        self.write('src/TextureCompiler/Managed/A.cs','class A {}')
        self.write('SDK/tools/Core.dll','same bytes')
        self.anchor=self.commit()
        self.group={'script':'test.ps1','dependencies':['src/TextureCompiler/Managed/A.cs','SDK/tools/*.dll'],'evidence':'evidence/*/validation.json'}
        self.receipt()
    def tearDown(self):self.temp.cleanup()
    def git(self,*args):return subprocess.run(['git','-C',str(self.root),*args],capture_output=True,text=True,check=True).stdout.strip()
    def write(self,path,text):
        target=self.root/path;target.parent.mkdir(parents=True,exist_ok=True);target.write_text(text)
    def commit(self):
        self.git('add','.');self.git('commit','-qm','checkpoint');return self.git('rev-parse','HEAD')
    def receipt(self,**updates):
        row={'sourceCommit':self.anchor,'runId':'1','status':'passed','configurations':[{'configuration':x,'passed':True} for x in ['Debug','Release']]};row.update(updates)
        self.write('evidence/1/validation.json',json.dumps(row));self.commit()
    def result(self,**kwargs):return selector.select(self.root,{'a':self.group},**kwargs)
    def test_reuse_unchanged_complete_success(self):self.assertEqual(self.result()['reused'],['a'])
    def test_changed_dependency_runs(self):
        self.write('src/TextureCompiler/Managed/A.cs','changed');self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_repeated_selection_tracks_new_heads_and_new_acceptance_anchor(self):
        self.assertEqual(self.result()['reused'],['a'])
        self.write('src/TextureCompiler/Managed/A.cs','new implementation');self.commit()
        self.assertEqual(self.result()['selected'],['a'])
        self.anchor=self.git('rev-parse','HEAD');self.receipt(runId='2')
        self.assertEqual(self.result()['reused'],['a'])
        (self.root/'src/TextureCompiler/Managed/A.cs').unlink();self.commit()
        self.assertEqual(self.result()['selected'],['a'])
        self.write('src/TextureCompiler/Managed/A.cs','new implementation');self.commit()
        self.assertEqual(self.result()['reused'],['a'])
    def test_deleted_input_runs(self):
        (self.root/'src/TextureCompiler/Managed/A.cs').unlink();self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_probe_and_incomplete_cannot_be_reused(self):
        self.receipt(probeOnly=True);self.assertEqual(self.result()['selected'],['a'])
        self.receipt(configurations=[{'configuration':'Debug','passed':True}]);self.assertEqual(self.result()['selected'],['a'])
    def test_new_unmapped_helper_fails_safe(self):
        self.write('src/TextureCompiler/Managed/New.cs','class New {}');self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_tool_dll_change_invalidates(self):
        self.write('SDK/tools/Core.dll','new');self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_sdk_is_explicit(self):
        self.group['sdk']=True;self.assertEqual(self.result()['selected'],[]);self.assertEqual(self.result(sdk=True)['selected'],['a'])
    def test_sdk_complete_success_reuses(self):
        self.group['sdk']=True
        self.receipt(sdkValidated=True,modes=[{'mode':x,'passed':True,'uniqueAssets':1,'actualCalls':1,'withoutOriginalTextureCompiler':x=='NoOriginal'} for x in ['Original','Recovered','NoOriginal']])
        self.assertEqual(self.result(sdk=True)['reused'],['a'])
    def test_historical_dependency_removal_does_not_hide_change(self):
        self.write('recovery/tests/FormerHelper.cs','old')
        self.group['dependencies'].append('recovery/tests/FormerHelper.cs')
        old={'groups':{'a':self.group.copy()}}
        self.write('recovery/catalog/TextureTestGroups.json',json.dumps(old));self.anchor=self.commit();self.receipt()
        self.group['dependencies']=[x for x in self.group['dependencies'] if x!='recovery/tests/FormerHelper.cs']
        self.write('recovery/tests/FormerHelper.cs','changed');self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_manual_evidence_is_not_automatic_regression(self):
        self.group['manual_only']=True
        self.assertEqual(self.result()['selected'],[])
        self.assertEqual(self.result(requested=['a'])['selected'],['a'])
    def test_full_mod_is_not_selected_by_general_sdk_request(self):
        catalog=json.loads((Path(__file__).parents[1]/'catalog/TextureTestGroups.json').read_text())['groups']
        self.assertTrue(catalog['sdk-PcMymod'].get('manual_only'))
        self.group.update(sdk=True,manual_only=True)
        self.write('src/TextureCompiler/Managed/A.cs','changed');self.commit()
        self.assertEqual(self.result(sdk=True)['selected'],[])
        self.assertEqual(self.result(sdk=True,requested=['a'])['selected'],['a'])
    def test_explicit_full_mod_does_not_request_other_sdk_profiles(self):
        full_mod=dict(self.group,sdk=True,manual_only=True)
        ordinary_sdk=dict(self.group,sdk=True)
        row=selector.select(self.root,{'full-mod':full_mod,'ordinary-sdk':ordinary_sdk},requested=['full-mod'])
        self.assertEqual(row['selected'],['full-mod'])
        self.assertEqual(row['details']['ordinary-sdk']['reason'],'SDK not requested')
    def test_existing_nonancestor_is_rejected(self):
        foreign=self.git('commit-tree',self.git('rev-parse','HEAD^{tree}'),'-m','foreign')
        self.receipt(sourceCommit=foreign);self.assertEqual(self.result()['selected'],['a'])
    def test_foreign_commit_rejected(self):
        self.receipt(sourceCommit='f'*40);self.assertEqual(self.result()['selected'],['a'])
    def test_unrelated_doc_change_reuses(self):
        self.write('README.md','notes');self.commit();self.assertEqual(self.result()['reused'],['a'])

if __name__=='__main__':unittest.main()
