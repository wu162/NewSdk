#!/usr/bin/env python3
"""Three-way full Upheaval comparison: SDK, original new compiler, optimized new compiler."""
import argparse
import itertools
import json
import os
from pathlib import Path
import statistics
import traceback

import benchmark as B
import expanded as E

VARIANTS = ('sdk', 'before', 'after')
SCENARIOS = ('no-cache', 'bc-cold', 'bc-hot')

def order(index, replica):
    # Six balanced permutations, with a different start on the second VM.
    return list(itertools.permutations(VARIANTS))[(index + 3*(replica-1)) % 6]

def comparisons(triplets):
    result = {}
    for scenario, rows in triplets.items():
        if len(rows) < 3:
            continue
        result[scenario] = {}
        for left, right in (('sdk','after'), ('before','after'), ('sdk','before')):
            pairs = [dict(old=row[left], new=row[right]) for row in rows]
            result[scenario][left+'/'+right] = B.summarize(pairs)
    return result

class Experiment(E.Experiment):
    def __init__(self, args):
        self.active = 'after'
        super().__init__(args)
        self.runtimes = {v:Path(args.runtime_root).resolve()/v/'bin' for v in ('before','after')}

    def destination(self, engine, mode):
        return self.work/((self.active if engine == 'new' else 'sdk')+'-'+mode)

    def persist(self):
        B.write(self.out/'state.json', self.state)

    def sample(self, variant, label, mode, bc, expected):
        self.active = variant
        engine = 'old' if variant == 'sdk' else 'new'
        if engine == 'new':
            self.runtime = self.runtimes[variant]
        result = self.build(engine, label+'-'+variant, mode=mode, bc=bc)
        row, products = result
        row['variant'] = variant
        if expected is None:
            accepted = json.loads((Path(self.c['repo'])/'recovery/docs/UPHEAVAL_BENCHMARK_RESULT.json').read_text())
            if E.fingerprint(products) != accepted['outputGate']['inventorySha256']:
                raise ValueError('Full workload differs from accepted exact-byte baseline')
            expected = products
        self.accept(result, expected)
        if variant != 'sdk':
            stats = row['cacheStatistics']
            if not bc:
                if stats['hits'] != 0 or stats['misses'] != 0:
                    raise ValueError('Disabled cache recorded hits/misses')
            elif mode == 'plain':
                raise ValueError('Wrong cache workload')
            elif label.endswith('cold'):
                if stats['hits'] != 0 or stats['misses'] != 1617:
                    raise ValueError('Cold run did not compile the expected supported assets')
            else:
                if stats['hits'] != 1617 or stats['misses'] != 0:
                    raise ValueError('Hot run did not exercise the confirmed namespace')
                if row['cacheBefore'] != row['cacheAfter']:
                    raise ValueError('Hot run rewrote the confirmed cache')
            if variant == 'after' and stats['inputInventoryPasses'] != (2 if bc else 0):
                raise ValueError('Optimized compiler did not use exactly one initial and one final inventory')
            if variant == 'after' and bc and label.endswith('hot') and stats['buildCacheStages'] != 4:
                raise ValueError('Full-hit base stream unexpectedly regenerated a cache candidate')
        elif bc and label.endswith('hot') and row['cacheCopyMessages'] != 1734:
            raise ValueError('SDK hot run did not exercise the expected cached assets')
        self.persist()
        return row, expected

    def measure(self):
        self.state['triplets'] = {s:[] for s in SCENARIOS}
        self.state['warmups'] = []
        self.state['methodology'] = dict(runsPerVM=self.c['runs'], warmupsPerVM=1,
            engineOrder='Six balanced engine permutations, opposite starting half on second VM',
            cacheOrder='Each engine cold immediately followed by hot; no-cache before/after alternates by block',
            clocks='Fresh processes, creation through exit; SDK sum of five sequential BAB processes',
            filesystemCache='OS page cache uncontrolled/warm, never claimed to be cold OS caches',
            pairing='Same VM and block for all three variants, independent native cache/output roots',
            outliers='No removed samples; every accepted build must match full linked/CDATAs byte inventory',
            confidence='VM-stratified paired bootstrap performed by aggregate_cache_speed.py')
        expected = None
        # Warm-up block is excluded in full, including initial cache generation.
        for index in range(self.c['runs']+1):
            measured = index > 0
            block = {s:dict(index=index, order=list(order(index, self.c['replica']))) for s in SCENARIOS}
            prefix = 'measured' if measured else 'warmup'
            for variant in order(index, self.c['replica']):
                self.active = variant
                engine = 'old' if variant == 'sdk' else 'new'
                self.reset(engine, 'plain'); self.reset(engine, 'bc')
                if index % 2 == 0:
                    block['no-cache'][variant], expected = self.sample(variant, prefix+'-no-cache', 'plain', False, expected)
                block['bc-cold'][variant], expected = self.sample(variant, prefix+'-bc-cold', 'bc', True, expected)
                block['bc-hot'][variant], expected = self.sample(variant, prefix+'-bc-hot', 'bc', True, expected)
                if index % 2:
                    block['no-cache'][variant], expected = self.sample(variant, prefix+'-no-cache', 'plain', False, expected)
            for scenario in SCENARIOS:
                if measured:
                    self.state['triplets'][scenario].append(block[scenario])
                else:
                    self.state['warmups'].append(dict(scenario=scenario, **block[scenario]))
            self.persist()
        checks = {name:B.snapshot(path)==json.loads((self.out/(name+'-inputs.json')).read_text())
                  for name,path in [('sdk',self.sdk),('mod',self.mod),('runtime',self.runtimes['after']),('original-runtime',self.tools)]}
        before = json.loads((self.runtimes['before'].parent/'reuse.json').read_text())
        checks['beforeRuntime'] = all(B.digest(self.runtimes['before']/name)==digest for name,digest in before['files'].items())
        self.state['comparisons'] = comparisons(self.state['triplets'])
        self.state['inputsUnchanged'] = checks
        self.state['succeeded'] = all(checks.values()) and all(row['byteIdentical'] for row in self.state['samples'])
        self.persist()
        if not self.state['succeeded']:
            raise ValueError('Input/output verification failed')

def main():
    p = argparse.ArgumentParser()
    p.add_argument('--repo', default='.')
    p.add_argument('--runtime-root', default='recovery/artifacts/cache-speed-runtime')
    p.add_argument('--output', default='recovery/artifacts/cache-speed')
    p.add_argument('--runs', type=int, choices=range(3,11), default=5)
    p.add_argument('--replica', type=int, choices=(1,2), required=True)
    p.add_argument('--timeout', type=int, default=1200)
    args = p.parse_args()
    args.runtime = str(Path(args.runtime_root)/'after/bin')
    args.changes=3; args.only_package=False; args.followup_group=None
    E.initialize(args)
    experiment=Experiment(args)
    provenance=json.loads((Path(args.runtime_root)/'provenance.json').read_text())
    experiment.state['runtimeProvenance']=provenance
    try:
        experiment.measure()
    except BaseException:
        experiment.state['error']=traceback.format_exc(); experiment.state['succeeded']=False
        raise
    finally:
        experiment.persist()

if __name__=='__main__':
    main()
