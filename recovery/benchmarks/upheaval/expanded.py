#!/usr/bin/env python3
"""Paired cache, incremental, recovery and packaging experiments on real Upheaval."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import struct
import subprocess
import tarfile
import tempfile
import time
import traceback
import xml.etree.ElementTree as ET

import benchmark as B


def fingerprint(value):
    return hashlib.sha256(json.dumps(value, sort_keys=True, separators=(',', ':')).encode()).hexdigest()


def mutate(path, kind, original):
    """Change actual reachable content, retaining length and timestamp."""
    data = bytearray(original)
    if kind == 'xml':
        token = b'Volume = "60"'
        if data.count(token) != 1:
            raise ValueError('Expected one reachable AmbientStream volume')
        data = data.replace(token, b'Volume = "61"', 1)
    elif kind == 'texture':
        if data[:4] != b'DDS ' or data[84:88] != b'DXT1' or len(data) != 640:
            raise ValueError('Expected original 32x32 DXT1 DDS')
        data[128:] = struct.pack('<HHI', 0xf800, 0x001f, 0) * 64
        if data == original:
            data[128:] = struct.pack('<HHI', 0x07e0, 0x001f, 0) * 64
    elif kind == 'audio':
        if data[:4] != b'RIFF' or data[8:12] != b'WAVE':
            raise ValueError('Expected original WAV')
        position = 12
        while position + 8 <= len(data):
            size = struct.unpack_from('<I', data, position + 4)[0]
            if data[position:position+4] == b'data':
                if size < 128 or position + 8 + size > len(data):
                    raise ValueError('Invalid WAV data bounds')
                for offset in range(position + 8, position + 8 + size, 17):
                    data[offset] ^= 0x7f
                break
            position += 8 + size + size % 2
        else:
            raise ValueError('WAV has no PCM payload')
    else:
        raise ValueError(kind)
    if len(data) != len(original) or data == original:
        raise ValueError('Mutation must change content at the same length')
    return bytes(data)


def execute(exe, arguments, cwd, logs, timeout, environment):
    logs.mkdir(parents=True, exist_ok=False)
    B.write(logs / 'command.json', dict(exe=str(exe), arguments=arguments, cwd=str(cwd)))
    with (logs/'stdout.log').open('wb') as stdout, (logs/'stderr.log').open('wb') as stderr:
        start = time.perf_counter()
        process = subprocess.Popen([str(exe), *arguments], cwd=cwd, env=environment,
                                   stdout=stdout, stderr=stderr)
        timed_out = False
        try:
            code = process.wait(timeout=timeout)
        except subprocess.TimeoutExpired:
            timed_out = True
            subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'],
                           stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            code = process.wait()
        elapsed = time.perf_counter() - start
        row = dict(wallSeconds=elapsed, exitCode=code, timedOut=timed_out, **B.metrics(process))
    text = '\n'.join((logs/n).read_text(encoding='utf-8-sig', errors='replace')
                     for n in ('stdout.log', 'stderr.log'))
    row['compilerErrorLogged'] = bool(re.search(r'^\s*(Critical|Error):', text, re.M | re.I))
    row['legacyCacheCopyMessages'] = len(re.findall('copied from cache:', text, re.I))
    row['succeeded'] = not timed_out and code == 0 and not row['compilerErrorLogged']
    B.write(logs/'process.json', row)
    if not row['succeeded']:
        print(f'PROCESS_FAILURE {logs}: exit={code}, timeout={timed_out}\n' + text[-4000:], flush=True)
    return row


def initialize(args):
    repo, out = Path(args.repo).resolve(), Path(args.output).resolve()
    out.mkdir(parents=True, exist_ok=False)
    work = Path(tempfile.mkdtemp(prefix='ux', dir=os.environ.get('RUNNER_TEMP')))
    runtime = Path(args.runtime).resolve()
    sdk = repo/'SDK'
    historical = work/'h'
    historical.mkdir()
    archive = work/'mod.tar'
    tree = subprocess.check_output(['git', '-C', str(repo), 'rev-parse', B.MOD_SOURCE+':SDK/Mods/Upheaval']).decode().strip()
    if tree != B.MOD_TREE:
        raise ValueError('Historical workload identity differs')
    with archive.open('xb') as stream:
        subprocess.run(['git', '-C', str(repo), 'archive', '--format=tar', B.MOD_SOURCE,
                        'SDK/Mods/Upheaval'], check=True, stdout=stream)
    with tarfile.open(archive) as tar:
        for member in tar.getmembers():
            if (not (historical/member.name).resolve().is_relative_to(historical.resolve())
                    or not (member.isfile() or member.isdir())):
                raise ValueError('Unsafe MOD archive')
        tar.extractall(historical, filter='data')
    archive.unlink()
    mod = historical/'SDK/Mods/Upheaval'
    mod_inputs = B.snapshot(mod)
    if len(mod_inputs) != 2723:
        raise ValueError('Full workload file count differs')
    B.write(out/'mod-inputs.json', mod_inputs)
    B.write(out/'sdk-inputs.json', B.snapshot(sdk))
    B.write(out/'runtime-inputs.json', B.snapshot(runtime))
    tools = work/'t'
    shutil.copytree(sdk/'tools', tools)
    paths = {kind: ';'.join(map(str, (sdk, mod/leaf, sdk/'Mods', sdk/shared)))
             for kind, leaf, shared in [('data','Data','SageXml'),('art','Art','Art'),('audio','Audio','Audio')]}
    config = ET.parse(tools/'BinaryAssetBuilder.exe.config')
    section = config.getroot().find('assetbuilder')
    section.set('schema', str(sdk/'Schemas/xsd/CnC3Types.xsd'))
    section.set('dataRoot', str(sdk))
    for kind, value in paths.items():
        section.set('default'+kind.title()+'Paths', value)
    startup = config.getroot().find('startup')
    if startup is None:
        startup = ET.SubElement(config.getroot(), 'startup')
    startup.clear()
    startup.set('useLegacyV2RuntimeActivationPolicy', 'true')
    ET.SubElement(startup, 'supportedRuntime', version='v4.0')
    config.write(tools/'BinaryAssetBuilder.exe.config', encoding='utf-8', xml_declaration=True)
    (tools/'MakeBig.exe.config').write_text('<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0"/></startup></configuration>')
    B.write(out/'original-runtime-inputs.json', B.snapshot(tools))
    source = subprocess.check_output(['git', '-C', str(repo), 'rev-parse', 'HEAD']).decode().strip()
    reuse = json.loads((runtime.parent/'reuse.json').read_text())
    if reuse['testSourceCommit'] != source:
        raise ValueError('Runtime not verified against this source')
    config = dict(repo=str(repo), work=str(work), runtime=str(runtime), sdk=str(sdk), mod=str(mod),
                  tools=str(tools), paths=paths, runs=args.runs, changes=args.changes, replica=args.replica,
                  timeout=args.timeout, sourceCommit=source, onlyPackaging=args.only_package)
    B.write(out/'context.json', config)
    state = dict(schemaVersion=1, sourceCommit=source, binarySourceCommit=reuse['binarySourceCommit'],
                 runId=os.getenv('GITHUB_RUN_ID'), runAttempt=os.getenv('GITHUB_RUN_ATTEMPT'),
                 replica=args.replica, machine=platform.node(), runner=os.getenv('RUNNER_NAME'),
                 environment=dict(platform=platform.platform(), processor=platform.processor(),
                                  logicalCpus=os.cpu_count(), python=platform.python_version()),
                 reuse=reuse, modSourceCommit=B.MOD_SOURCE, modGitTree=tree, phases={}, samples=[],
                 pairs={}, observations=[], succeeded=False, methodology=dict(runs=args.runs,
                 changeRuns=args.changes, order='Alternating engine blocks; opposite first engine on the second VM',
                 clocks='perf_counter: process creation through exit; full pipeline wall time for BIG builds',
                 caches='BC and SC independent; native cache stores retained separately, never shared between engines',
                 mutations='Reachable XML/DDS/WAV content changed at same length and timestamp; unmodified full MOD otherwise',
                 packagingLayout='Old BAB writes directly into package/input/data; no artificial loose-output collection copy',
                 outliers='No removed samples; no ratio for failed or unsupported operations'))
    B.write(out/'state.json', state)
    print('INITIALIZED '+json.dumps(dict(replica=args.replica,machine=state['machine'],modFiles=len(mod_inputs))),flush=True)


class Experiment:
    def __init__(self, args):
        self.out = Path(args.output).resolve()
        self.c = json.loads((self.out/'context.json').read_text())
        self.state = json.loads((self.out/'state.json').read_text())
        self.work, self.sdk, self.mod, self.tools, self.runtime = [Path(self.c[k]) for k in ('work','sdk','mod','tools','runtime')]
        self.env = {k:v for k,v in os.environ.items() if not k.startswith('RA3_RECOVERED_') and k != 'RECOVERED_W3X_TRACE_DIRECTORY'}
        self.env['PATH'] = ';'.join(p for p in self.env['PATH'].split(';') if not re.search(r'[\\/]SDK[\\/]tools', p, re.I))
        self.tasks = [('base',self.mod/'Data/Mod.xml')]
        self.tasks += [('apt',p) for p in sorted((self.mod/'Data/AptUI').glob('*.xml'))]
        self.tasks += [('maps',p) for p in sorted((self.mod/'Data/AdditionalMaps').glob('MapMetaData_*.xml'))]
        self.serial = len(self.state['samples'])

    def persist(self):
        self.state['statistics'] = {name: B.summarize(rows) for name,rows in self.state['pairs'].items() if len(rows)>=3}
        B.write(self.out/'state.json', self.state)
        summary = '# Upheaval expanded benchmark\n\n'
        summary += f'Replica {self.c["replica"]}, machine {self.state["machine"]}; phases: '+json.dumps(self.state['phases'])+'\n\n'
        summary += '| Scenario | n | Old mean s | New mean s | Old/new | 95% CI |\n| --- | ---: | ---: | ---: | ---: | --- |\n'
        for name,stats in self.state['statistics'].items():
            summary += f'| {name} | {stats["old"]["n"]} | {stats["old"]["mean"]:.3f} | {stats["new"]["mean"]:.3f} | {stats["speedup"]:.3f} | {stats["speedup95CI"]} |\n'
        summary += '\nCapability/recovery observations: '+str(len(self.state['observations']))+'; see state.json.\n'
        (self.out/'summary.md').write_text(summary,encoding='utf-8')

    def order(self, index):
        return ['old','new'] if (index+self.c['replica'])%2==0 else ['new','old']

    def destination(self, engine, mode):
        return self.work/(engine[0]+'-'+mode)

    def cache_root(self, engine, mode='bc'):
        return self.destination(engine,mode)/('caches' if engine=='old' else '.cache')

    def reset(self, engine, mode):
        destination=self.destination(engine,mode)
        if destination.exists(): shutil.rmtree(destination)
        destination.mkdir()

    def backup(self, engine):
        source=self.cache_root(engine)
        if not any(p.is_file() for p in source.rglob('*')): raise ValueError('Cache was not populated: '+engine)
        target=self.work/('backup-'+engine)
        if target.exists(): shutil.rmtree(target)
        shutil.copytree(source,target)

    def restore_cache(self, engine):
        target=self.cache_root(engine)
        if target.exists(): shutil.rmtree(target)
        shutil.copytree(self.work/('backup-'+engine),target)

    def build(self, engine, label, mode='bc', bc=True, sc=False, selection=None, package=False):
        self.serial+=1
        case=self.out/'cases'/f'{self.serial:04}-{label}-{engine}'
        destination=self.destination(engine,mode)
        destination.mkdir(parents=True,exist_ok=True)
        tasks=self.tasks if selection is None else [t for t in self.tasks if t[0] in selection]
        if not tasks: raise ValueError('Empty workload selection')
        cache_before=B.snapshot(self.cache_root(engine,mode)) if self.cache_root(engine,mode).exists() else {}
        rows=[]; commands=[]; products={}; report=None; package_paths=None
        if engine=='old':
            if package:
                package_dir=destination/'package'
                if package_dir.exists():shutil.rmtree(package_dir)
                (package_dir/'input/data').mkdir(parents=True)
            # BC-only experiments deliberately discard intermediate/link outputs;
            # SC experiments retain its required intermediate/session state.
            for number,(content,entry) in enumerate(tasks):
                task_dir=destination/str(number)
                for leaf in ('s','i','h'):
                    target=task_dir/leaf
                    if not sc and target.exists(): shutil.rmtree(target)
                    target.mkdir(parents=True,exist_ok=True)
                cache=destination/'caches'/str(number);cache.mkdir(parents=True,exist_ok=True)
                streams=destination/'package/input/data' if package else task_dir/'s'
                arguments=[str(entry),f'/sp:{self.sdk/"Schemas/xsd/CnC3Types.xsd"}',f'/dr:{self.sdk}',
                           f'/od:{streams}',f'/iod:{task_dir/"i"}',f'/scd:{task_dir/"h"}',f'/bcp:{cache}',
                           '/tp:Win32',f'/sc:{str(sc).lower()}',f'/bc:{str(bc).lower()}',
                           '/psc:false','/fsc:false','/csc:false','/atc:false','/sh:false','/pc:true',
                           '/res:true','/gui:false','/poe:false','/mr:false','/oof:false','/sf:false',
                           '/ls:true','/ss:true','/oix:false','/vf:false','/osh:false','/oar:false',
                           '/slowclean:true','/tl:3','/el:0']+[f'/{k}:{v}' for k,v in self.c['paths'].items()]
                commands.append((task_dir,arguments))
        else:
            arguments=['build','--mod',str(self.mod),'--sdk-root',str(self.sdk),'--out',str(destination),
                       '--package-content',','.join(selection or ['base','apt','maps']),
                       '--package-format','big' if package else 'loose',f'/bc:{str(bc).lower()}',
                       f'/sc:{str(sc).lower()}','/pc:true','/ss:true','/slowclean:true','/tl:3']
            if package: arguments += ['--include-additional=true']
            arguments += [f'/{k}:{v}' for k,v in self.c['paths'].items()]
        started=time.perf_counter()
        if engine=='old':
            for number,(task_dir,arguments) in enumerate(commands):
                row=execute(self.tools/'BinaryAssetBuilder.exe',arguments,self.sdk,case/str(number),self.c['timeout'],self.env)
                rows.append(row)
                if not row['succeeded']: break
            if len(rows)==len(tasks) and all(r['succeeded'] for r in rows) and package:
                package_paths,extra=self.package_old(destination,tasks,case)
                rows+=extra
        else:
            rows.append(execute(self.runtime/'NewModCompiler.exe',arguments,self.runtime,case,self.c['timeout'],self.env))
        elapsed=time.perf_counter()-started
        row=dict(id=self.serial,label=label,engine=engine,mode=mode,bc=bc,sc=sc,package=package,
                 selection=selection or ['base','apt','maps'],succeeded=all(r['succeeded'] for r in rows),
                 wallSeconds=elapsed if package else sum(r['wallSeconds'] for r in rows),
                 cpuSeconds=sum(r['cpuSeconds'] for r in rows),peakWorkingSetBytes=max(r['peakWorkingSetBytes'] for r in rows),
                 processes=rows,byteIdentical=False,case=str(case.relative_to(self.out)),
                 cacheBefore=dict(files=len(cache_before),bytes=sum(v['bytes'] for v in cache_before.values()),sha256=fingerprint(cache_before)))
        self.state['samples'].append(row)
        try:
            if engine=='new':
                report_path=destination/'build-report.json'
                if report_path.exists():
                    report=json.loads(report_path.read_text(encoding='utf-8-sig'))
                    row['cacheStatistics']=report.get('cache')
                    row['diagnostics']=report.get('diagnostics')
                    row['reportExitCode']=report.get('exitCode')
                    for name in ('build-report.json','build.events.jsonl','build.log'):
                        if (destination/name).exists(): shutil.copy2(destination/name,case/name)
                if row['succeeded']:
                    if not report or not(report['status']=='succeeded' and report['exitCode']==0 and report['executionMode']=='InProcess' and not report['externalTools'] and report['publicationStatus']=='PublishedDirectory'):
                        raise ValueError('CLI report did not confirm in-process publication')
                    if {(t['Content'],t['InputPath'].casefold()) for t in report['plan']['Tasks']} != {(c,str(p).casefold()) for c,p in tasks}:
                        raise ValueError('CLI omitted or changed workload')
            if row['succeeded']:
                if package:
                    if engine=='new':
                        package_paths=(next(Path(a['Path']) for a in report['artifacts'] if a['Kind']=='package.big'),
                                       next(Path(a['Path']) for a in report['artifacts'] if a['Kind']=='package.skudef'))
                    import sys
                    sys.path.insert(0,str(Path(self.c['repo'])/'recovery/scripts'))
                    from ValidateCompilerBig import read_archive
                    archive=read_archive(package_paths[0])
                    products={a['name']:dict(bytes=a['bytes'],sha256=a['sha256']) for a in archive['members']}
                    products['__skudef__']=dict(bytes=package_paths[1].stat().st_size,sha256=B.digest(package_paths[1]))
                    row['bigSha256']=B.digest(package_paths[0]);row['bigBytes']=package_paths[0].stat().st_size
                    row['bigMembers']=len(archive['members'])
                else:
                    for number,(content,entry) in enumerate(tasks):
                        stream=destination/str(number)/'s' if engine=='old' else destination/'published'/content/entry.stem/'streams'
                        products[content+'/'+entry.stem]=B.inventory(stream)
                    if engine=='new' and sum(len(v) for v in products.values())!=len(report['artifacts']):
                        raise ValueError('CLI artifact allowlist differs from linked outputs')
                B.write(case/'outputs.json',products)
                row['outputsSha256']=fingerprint(products)
                if package and label=='big-warmup':
                    raw=case/'raw';raw.mkdir()
                    for path in package_paths:shutil.copy2(path,raw/path.name)
                elif engine=='old' and label.endswith('-no-cache-oracle'):
                    for number in range(len(tasks)):
                        shutil.copytree(destination/str(number)/'s',case/'raw'/str(number))
            cache_after=B.snapshot(self.cache_root(engine,mode)) if self.cache_root(engine,mode).exists() else {}
            row['cacheAfter']=dict(files=len(cache_after),bytes=sum(v['bytes'] for v in cache_after.values()),sha256=fingerprint(cache_after))
            row['cacheCopyMessages']=sum(r['legacyCacheCopyMessages'] for r in rows)
            print(f'SAMPLE {label} {engine}: {row["wallSeconds"]:.6f}s, success={row["succeeded"]}, cacheFiles={len(cache_after)}',flush=True)
            return row,products
        finally:
            self.persist()

    def package_old(self,destination,tasks,case):
        package=destination/'package'
        inputs=package/'input'
        for content,entry in tasks:
            if content=='apt': (inputs/'data/aptui'/(entry.stem.lower()+'.version')).write_bytes(b' \r\n')
        rows=[execute(self.tools/'LoDStreamBuilder.exe',[str(inputs/'data/mod.manifest')],self.tools,case/'lod',self.c['timeout'],self.env)]
        if not rows[0]['succeeded']: return (package/'failed.big',package/'failed.skudef'),rows
        for root in (self.sdk/'Additional',self.mod/'Additional'):
            if root.exists():
                for source in root.rglob('*'):
                    if source.is_file():
                        target=inputs/source.relative_to(root);target.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(source,target)
        big=package/'Upheaval_1.0.big'
        rows.append(execute(self.tools/'MakeBig.exe',['-f',str(inputs),'-x:*.asset','-o:'+str(big)],self.tools,case/'makebig',self.c['timeout'],self.env))
        sku=package/'Upheaval_1.0.skudef';sku.write_bytes(b'mod-game 1.12\r\nadd-big Upheaval_1.0.big\r\n')
        return (big,sku),rows

    def accept(self,result,expected):
        row,products=result
        if not row['succeeded'] or products!=expected:
            row['gateError']='Compile failure or exact output mismatch'
            B.write(self.out/'rejected-outputs.json',dict(sample=row,actual=products,expected=expected))
            raise ValueError(row['gateError']+': '+row['label']+'/'+row['engine'])
        row['byteIdentical']=True
        return row

    def pair(self,name,index,expected,**kwargs):
        pair=dict(index=index,order=self.order(index))
        for engine in pair['order']:
            pair[engine]=self.accept(self.build(engine,name,**kwargs),expected)
        self.state['pairs'].setdefault(name,[]).append(pair)
        self.persist()
        return pair

    def baseline(self):
        expected=None
        accepted=json.loads((Path(self.c['repo'])/'recovery/docs/UPHEAVAL_BENCHMARK_RESULT.json').read_text())
        for index in range(1):
            pair=dict(index=index,order=self.order(index))
            for engine in pair['order']:
                self.reset(engine,'plain')
                result=self.build(engine,'no-cache-warmup' if index==0 else 'no-cache',mode='plain',bc=False)
                if expected is None:
                    if fingerprint(result[1])!=accepted['outputGate']['inventorySha256']:
                        raise ValueError('Current full MOD differs from accepted baseline inventory')
                    expected=result[1];B.write(self.out/'baseline.json',expected)
                pair[engine]=self.accept(result,expected)
            if index: self.state['pairs'].setdefault('no-cache',[]).append(pair)
            self.persist()

    def cache(self):
        expected=json.loads((self.out/'baseline.json').read_text())
        for index in range(self.c['runs']):
            cold=dict(index=index,order=self.order(index));hot=dict(index=index,order=self.order(index));plain=dict(index=index,order=self.order(index))
            for engine in cold['order']:
                self.reset(engine,'bc')
                self.reset(engine,'plain')
                if index%2==0:
                    plain[engine]=self.accept(self.build(engine,'no-cache',mode='plain',bc=False),expected)
                cold[engine]=self.accept(self.build(engine,'bc-cold'),expected)
                hot[engine]=self.accept(self.build(engine,'bc-hot'),expected)
                if index%2!=0:
                    plain[engine]=self.accept(self.build(engine,'no-cache',mode='plain',bc=False),expected)
                if hot[engine]['cacheBefore']['files']==0: raise ValueError('Hot profile had no populated generated cache')
                if engine=='new':
                    if cold[engine]['cacheStatistics']['hits']!=0 or cold[engine]['cacheStatistics']['misses']<=0:
                        raise ValueError('No actual cold cache miss evidence')
                    if hot[engine]['cacheStatistics']['hits']<=0: raise ValueError('No actual new cache hits')
            self.state['pairs'].setdefault('bc-cold',[]).append(cold)
            self.state['pairs'].setdefault('bc-hot',[]).append(hot)
            self.state['pairs'].setdefault('no-cache',[]).append(plain)
            self.persist()
        for engine in ('old','new'): self.backup(engine)

    def incremental(self,family=None):
        baseline=json.loads((self.out/'baseline.json').read_text())
        inputs={'xml':self.mod/'Data/Mod/Specific/AmbientStream.xml',
                'texture':self.mod/'Art/Specific/LURomania_vol.dds',
                'audio':self.mod/'Audio/Specific/JUPearl_ambiLoopa.wav'}
        if family:inputs={family:inputs[family]}
        for kind,path in inputs.items():
            original=path.read_bytes();stamp=path.stat();changed=mutate(path,kind,original)
            def put(data):
                path.write_bytes(data);os.utime(path,ns=(stamp.st_atime_ns,stamp.st_mtime_ns))
            try:
                put(changed)
                oracle=None
                for engine in self.order(0):
                    self.reset(engine,'oracle')
                    result=self.build(engine,kind+'-no-cache-oracle',mode='oracle',bc=False)
                    if oracle is None:
                        oracle=result[1]
                        if not result[0]['succeeded'] or oracle==baseline: raise ValueError('Mutation did not change compiled bytes: '+kind)
                    self.accept(result,oracle)
                B.write(self.out/(kind+'-oracle.json'),oracle)
                self.state['observations'].append(dict(kind='mutation',family=kind,path=str(path.relative_to(self.mod)),
                     beforeSha256=hashlib.sha256(original).hexdigest(),afterSha256=hashlib.sha256(changed).hexdigest(),
                     bytes=len(changed),sameLengthSameTimestamp=True,changedCompiledOutputs=True))
                for index in range(self.c['changes']):
                    for engine in ('old','new'): self.restore_cache(engine)
                    self.pair(kind+'-changed',index,oracle)
                    self.pair(kind+'-changed-hot',index,oracle)
            finally:
                put(original)
        for engine in ('old','new'): self.restore_cache(engine)

    def recovery(self):
        baseline=json.loads((self.out/'baseline.json').read_text())
        for damage in ('missing','corrupt'):
            for engine in self.order(0):
                self.restore_cache(engine)
                candidates=[p for p in self.cache_root(engine).rglob('*') if p.is_file() and p.stat().st_size>16
                            and (p.suffix=='.asset' if engine=='old' else p.parent.name=='files')]
                if not candidates: raise ValueError('No real cache payload to damage')
                victim=max(candidates,key=lambda p:p.stat().st_size)
                before=B.digest(victim)
                if damage=='missing': victim.unlink()
                else:
                    data=bytearray(victim.read_bytes());data[-1]^=0xff;victim.write_bytes(data)
                result=self.build(engine,'cache-'+damage)
                correct=result[0]['succeeded'] and result[1]==baseline
                if correct: result[0]['byteIdentical']=True
                self.state['observations'].append(dict(kind='cache-'+damage,engine=engine,sampleId=result[0]['id'],
                      victim=str(victim.relative_to(self.cache_root(engine))),beforeSha256=before,
                      automaticRecovery=correct,failedClosed=not result[0]['succeeded'],
                      wrongOutputAccepted=result[0]['succeeded'] and not correct))
                if result[0]['succeeded'] and not correct:
                    destination=self.destination(engine,'bc')
                    raw=self.out/result[0]['case']/'wrong-output'
                    if engine=='new':shutil.copytree(destination/'published',raw)
                    else:
                        for number in range(len(self.tasks)):shutil.copytree(destination/str(number)/'s',raw/str(number))
                self.restore_cache(engine)
                self.accept(self.build(engine,'retry-after-cache-'+damage),baseline)
        root=self.mod/'Data/Mod.xml';original=root.read_bytes();stamp=root.stat()
        for engine in self.order(1):
            self.restore_cache(engine)
            cache_before=B.snapshot(self.cache_root(engine))
            published=self.destination(engine,'bc')/'published'
            before=B.snapshot(published) if engine=='new' else None
            try:
                root.write_bytes(original+b'\n<')
                row,products=self.build(engine,'invalid-xml')
                if row['succeeded']: raise ValueError('Invalid XML was accepted')
                unchanged=B.snapshot(self.cache_root(engine))==cache_before
                publication_unchanged=B.snapshot(published)==before if engine=='new' else None
                self.state['observations'].append(dict(kind='failed-build',engine=engine,sampleId=row['id'],
                      failureObserved=True,confirmedCacheUnchanged=unchanged,publishedUnchanged=publication_unchanged))
                if engine=='new' and (not unchanged or not publication_unchanged):
                    raise ValueError('Failed build changed confirmed cache or publication')
            finally:
                root.write_bytes(original);os.utime(root,ns=(stamp.st_atime_ns,stamp.st_mtime_ns))
            self.accept(self.build(engine,'retry-after-invalid-xml'),baseline)

    def session(self):
        baseline=json.loads((self.out/'baseline.json').read_text())
        self.reset('new','sc-full')
        row,products=self.build('new','full-session-capability',mode='sc-full',sc=True,bc=False)
        codes=[d.get('code') for d in row.get('diagnostics') or []]
        if row['succeeded'] or 'CORE_SESSION_TEXTURE_UNSUPPORTED' not in codes:
            raise ValueError('Expected explicit full-MOD Texture/session boundary')
        self.state['observations'].append(dict(kind='full-session-cache',engine='new',supported=False,
               sampleId=row['id'],diagnostic='CORE_SESSION_TEXTURE_UNSUPPORTED',notIncludedInSpeedRatios=True))
        self.reset('old','sc-full')
        old_cold=self.accept(self.build('old','full-session-cold',mode='sc-full',sc=True,bc=False),baseline)
        old_hot=self.accept(self.build('old','full-session-hot',mode='sc-full',sc=True,bc=False),baseline)
        self.state['observations'].append(dict(kind='full-session-cache',engine='old',supported=True,
               coldSampleId=old_cold['id'],hotSampleId=old_hot['id'],notIncludedInSpeedRatios=True))
        expected={k:v for k,v in baseline.items() if k.startswith('maps/')}
        for index in range(self.c['runs']):
            cold=dict(index=index,order=self.order(index));hot=dict(index=index,order=self.order(index))
            for engine in cold['order']:
                self.reset(engine,'sc-maps')
                cold[engine]=self.accept(self.build(engine,'maps-session-cold',mode='sc-maps',sc=True,bc=False,selection=['maps']),expected)
                hot[engine]=self.accept(self.build(engine,'maps-session-hot',mode='sc-maps',sc=True,bc=False,selection=['maps']),expected)
                if engine=='new' and hot[engine]['cacheStatistics'].get('sessionPackageHits',0)<=0:
                    raise ValueError('Metadata SC hot path had no actual package reuse')
            self.state['pairs'].setdefault('maps-session-cold',[]).append(cold)
            self.state['pairs'].setdefault('maps-session-hot',[]).append(hot)
            self.persist()

    def packaging(self):
        expected=None
        for index in range(4):
            pair=dict(index=index,order=self.order(index))
            for engine in pair['order']:
                self.reset(engine,'big')
                result=self.build(engine,'big-warmup' if index==0 else 'big-end-to-end',mode='big',bc=False,package=True)
                if expected is None:
                    if not result[0]['succeeded']: raise ValueError('Original BIG baseline failed')
                    expected=result[1];B.write(self.out/'big-baseline.json',expected)
                pair[engine]=self.accept(result,expected)
            if index: self.state['pairs'].setdefault('big-end-to-end',[]).append(pair)
            self.persist()

    def finalize(self):
        checks={name:B.snapshot(root)==json.loads((self.out/(name+'-inputs.json')).read_text())
                for name,root in [('sdk',self.sdk),('mod',self.mod),('runtime',self.runtime),('original-runtime',self.tools)]}
        self.state['inputsUnchanged']=checks
        failed=[name for name,phase in self.state['phases'].items() if phase.get('status')=='failed']
        wrong=[x for x in self.state['observations'] if x.get('wrongOutputAccepted')]
        self.state['succeeded']=not failed and not wrong and all(checks.values())
        self.persist()
        print((self.out/'summary.md').read_text(),flush=True)
        if os.getenv('GITHUB_STEP_SUMMARY'):
            with open(os.environ['GITHUB_STEP_SUMMARY'],'a',encoding='utf-8') as stream: stream.write((self.out/'summary.md').read_text())
        if not self.state['succeeded']: raise ValueError('Expanded suite has failed gates; see independent phase results')


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--phase',choices=['init','baseline','cache','incremental','incremental-xml','incremental-texture','incremental-audio','recovery','session','packaging','finalize'],required=True)
    parser.add_argument('--repo',default='.')
    parser.add_argument('--runtime',default='recovery/artifacts/upheaval-runtime/bin')
    parser.add_argument('--output',default='recovery/artifacts/upheaval-expanded')
    parser.add_argument('--runs',type=int,choices=range(3,21),default=5)
    parser.add_argument('--changes',type=int,choices=range(3,11),default=3)
    parser.add_argument('--replica',type=int,default=1)
    parser.add_argument('--timeout',type=int,default=1200)
    parser.add_argument('--only-package',action='store_true')
    args=parser.parse_args()
    if os.name!='nt': raise ValueError('Real experiments require Windows')
    if args.phase=='init': return initialize(args)
    experiment=Experiment(args)
    experiment.state['phases'][args.phase]=dict(status='running')
    try:
        if args.phase.startswith('incremental-'):experiment.incremental(args.phase.split('-',1)[1])
        else:getattr(experiment,args.phase)()
        experiment.state['phases'][args.phase]=dict(status='passed')
    except BaseException:
        experiment.state['phases'][args.phase]=dict(status='failed',error=traceback.format_exc())
        raise
    finally:
        experiment.persist()


if __name__=='__main__':
    main()
