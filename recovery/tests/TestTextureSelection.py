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
        old={'groups':{'a':self.group.copy()}}
        self.write('recovery/catalog/TextureTestGroups.json',json.dumps(old));self.anchor=self.commit();self.receipt()
        self.group['dependencies']=[]
        self.write('src/TextureCompiler/Managed/A.cs','changed');self.commit();self.assertEqual(self.result()['selected'],['a'])
    def test_foreign_commit_rejected(self):
        self.receipt(sourceCommit='f'*40);self.assertEqual(self.result()['selected'],['a'])
    def test_unrelated_doc_change_reuses(self):
        self.write('README.md','notes');self.commit();self.assertEqual(self.result()['reused'],['a'])

if __name__=='__main__':unittest.main()
