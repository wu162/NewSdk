import copy
import unittest
from cache_speed import order, VARIANTS
from aggregate_cache_speed import validate, ratio

class CacheSpeedTests(unittest.TestCase):
    def test_balanced_six_engine_orders(self):
        for replica in (1,2):
            rows=[order(i,replica) for i in range(6)]
            self.assertEqual(len(set(rows)),6)
            for variant in VARIANTS:
                self.assertEqual([sum(r[p]==variant for r in rows) for p in range(3)],[2,2,2])
        self.assertNotEqual(order(0,1),order(0,2))

    def test_stratified_pairing_preserves_shared_machine_noise(self):
        groups=[[{'sdk':{'wallSeconds':noise*3},'before':{'wallSeconds':noise*2},'after':{'wallSeconds':noise}}
                 for noise in factors] for factors in ([1,2,3],[10,20,30])]
        result=ratio(groups,'before','after')
        self.assertEqual(result['ratio'],2)
        self.assertEqual(result['ratio95CI'],[2,2])

    def runner_fixture(self):
        states=[]; jobs={'jobs':[]}
        for replica in (1,2):
            row=dict(index=1,order=list(VARIANTS),**{v:dict(variant=v,byteIdentical=True) for v in VARIANTS})
            rows=[dict(row,index=i) for i in (1,2,3)]
            states.append(dict(replica=replica,succeeded=True,inputsUnchanged={'sdk':True},
                samples=[dict(succeeded=True,byteIdentical=True)],runner='cloned-hostname',runId='42',
                sourceCommit='source',modGitTree='mod',runtimeProvenance={'same':'binary'},
                methodology={'runsPerVM':3},triplets={s:copy.deepcopy(rows) for s in ('no-cache','bc-cold','bc-hot')}))
            jobs['jobs'].append(dict(id=replica,name=f'cache-speed ({replica})',run_id=42,
                runner_id=replica,runner_name='cloned-hostname',conclusion='success',started_at='start',completed_at='end'))
        return states,jobs

    def test_verify_runner_id_instead_of_hostname(self):
        states,jobs=self.runner_fixture()
        self.assertEqual(len(validate(states,jobs)),2)
        jobs['jobs'][1]['runner_id']=1
        with self.assertRaisesRegex(ValueError,'same VM'):
            validate(states,jobs)

    def test_reject_mispaired_or_incomplete_samples(self):
        states,jobs=self.runner_fixture()
        states[1]['triplets']['bc-hot'][0]['after']['variant']='before'
        with self.assertRaisesRegex(ValueError,'pairing'):
            validate(states,jobs)

    def test_reject_failed_input_gate_before_aggregation(self):
        states=[dict(replica=i,succeeded=False,inputsUnchanged={'sdk':False}) for i in (1,2)]
        with self.assertRaisesRegex(ValueError,'Failed input'):
            validate(states,{'jobs':[]})

if __name__=='__main__':
    unittest.main()
