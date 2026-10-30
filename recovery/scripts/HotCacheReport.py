"""Performance-only CSV and Chinese Markdown, with strict sample admission."""
import csv
import json
from pathlib import Path
import statistics

PHASES = ('hot_before', 'modified', 'hot_after')
CACHE_KEYS = ('xmlParse', 'instanceCompile', 'documentReuse', 'hits', 'misses',
              'linkedStreamsRestored', 'fileHashReads', 'fileHashBytesRead')


def summarize(samples):
    groups = {}
    for sample in samples:
        if sample.get('formal') and sample.get('performance') == 'off':
            key = (sample['case'], sample['mode'], sample['jobs'])
            groups.setdefault(key, []).append(sample)
    rows = []
    for (case, mode, jobs), samples in groups.items():
        row = dict(case=case, mode=mode, jobs=jobs)
        for phase in PHASES:
            good = [s for s in samples if s['phase'] == phase and s['status'] == 'qualified']
            values = [s['wallMs'] for s in good]
            row[phase + 'N'] = len(values)
            for suffix, function in (('MedianMs', statistics.median), ('MinMs', min), ('MaxMs', max)):
                row[phase + suffix] = function(values) if values else None
            for key in ('reportElapsedMs', 'cpuMs'):
                row[phase + key[0].upper() + key[1:]] = statistics.median(s[key] for s in good) if good else None
        before, modified, after = (row[p + 'MedianMs'] for p in PHASES)
        row['firstChangeExtraMs'] = modified - before if modified is not None and before is not None else None
        row['reheatedChangeMs'] = after - before if after is not None and before is not None else None
        rows.append(row)
    return rows


def write_csv(path, rows, fields):
    with Path(path).open('w', newline='', encoding='utf-8-sig') as stream:
        writer = csv.DictWriter(stream, fieldnames=fields, extrasaction='ignore')
        writer.writeheader()
        writer.writerows(rows)


def save(output, state):
    output = Path(output)
    state['summary'] = summarize(state['samples'])
    (output / 'benchmark.json').write_text(json.dumps(state, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    measurements, phases = [], []
    for sample in state['samples']:
        row = {k: v for k, v in sample.items() if not isinstance(v, (dict, list))}
        row.update(sample.get('cache', {}))
        row.pop('parallel', None)
        row.update(sample.get('mutation', {}))
        row['streamIds'] = ';'.join(sample.get('streamIds', []))
        parallel = sample.get('cache', {}).get('parallel') or {}
        for key in ('workerWaitMs', 'workerComputeMs', 'ownerCommitMs'):
            row[key] = parallel.get(key)
        measurements.append(row)
        performance = sample.get('reportPerformance') or {}
        for entry in performance.get('entries', []):
            phases.append(dict(sample=sample['label'], case=sample['case'], phase=sample['phase'],
                               performance=sample['performance'], kind='timing', **entry))
        for entry in performance.get('counters', []):
            phases.append(dict(sample=sample['label'], case=sample['case'], phase=sample['phase'],
                               performance=sample['performance'], kind='counter', **entry))
        for key in ('workerWaitMs', 'workerComputeMs', 'ownerCommitMs'):
            if key in parallel:
                phases.append(dict(sample=sample['label'], case=sample['case'], phase=sample['phase'],
                                   performance=sample['performance'], kind='parallel', name='cache.parallel.' + key,
                                   totalMs=parallel[key]))
    fields = ['label', 'case', 'phase', 'repeat', 'formal', 'performance', 'mode', 'jobs', 'status',
              'wallMs', 'reportElapsedMs', 'cpuMs', 'over8s', 'pathExercised', 'exitCode',
              'changedAssets', 'changedDocuments', 'changedMaterials', 'changedCaches', 'changedOutputs',
              'streamIds', 'workerWaitMs', 'workerComputeMs', 'ownerCommitMs', *CACHE_KEYS, 'error']
    write_csv(output / 'measurements.csv', measurements, fields)
    scenario_fields = ['case', 'mode', 'jobs'] + [p + suffix for p in PHASES for suffix in
                      ('N', 'MedianMs', 'MinMs', 'MaxMs', 'ReportElapsedMs', 'CpuMs')]
    write_csv(output / 'scenarios.csv', state['summary'], scenario_fields + ['firstChangeExtraMs', 'reheatedChangeMs'])
    write_csv(output / 'phases.csv', phases, ['sample', 'case', 'phase', 'performance', 'kind', 'streamId',
              'name', 'count', 'totalMs', 'selfMs', 'maxMs', 'value', 'aggregation'])
    def number(value):
        return '—' if value is None else '%.1f' % value
    lines = ['# L8 热缓存性能报告', '',
             '**状态：`%s`；组别：%s；累计 %.2f 分钟。**' %
             (state['status'], state['group'], state.get('elapsedSeconds', 0) / 60), '',
             '测试提交：`%s`；Release 来源：`%s`；EXE SHA256：`%s`。' %
             (state.get('sourceCommit', ''), state.get('binarySourceCommit', ''), state.get('exeSha256', '')), '',
             '正式成绩仅采用 performance=off、正常退出、目标路径已执行，且单次 wallMs 与编译器 elapsedMs 均严格 >8000 的样本。'
             '每次启动独立进程；准备、快照恢复、预热和报告均计入总预算。OS 页缓存未受控，样本数较少。', '',
             'L8 为完整固定版本 Upheaval 加合法扩容分区的混合性能负载，区别于原始未修改 Upheaval。', '',
             '## 场景完成程度', '', '| 场景 | 状态 | 说明 |', '| --- | --- | --- |']
    for case in state.get('cases', []):
        lines.append('| %s | %s | %s |' % (case['id'], case.get('status', 'pending'), case.get('reason', '')))
    lines += ['', '## 三阶段耗时', '',
              '| 场景 / 缓存 / jobs | 热前 n / 中位 ms | 修改 n / 中位 ms | 再热 n / 中位 ms | 首改额外 ms | 再热变化 ms |',
              '| --- | ---: | ---: | ---: | ---: | ---: |']
    for row in state['summary']:
        values = ['%s / %s / %s' % (row['case'], row['mode'], row['jobs'])]
        values += ['%s / %s' % (row[p + 'N'], number(row[p + 'MedianMs'])) for p in PHASES]
        values += [number(row[k]) for k in ('firstChangeExtraMs', 'reheatedChangeMs')]
        lines.append('| ' + ' | '.join(values) + ' |')
    ranked = sorted((r for r in state['summary'] if r['firstChangeExtraMs'] is not None),
                    key=lambda r: r['firstChangeExtraMs'], reverse=True)[:5]
    lines += ['', '## 较大增量来源（本机）', '']
    lines += ['- %s：首次修改额外 %s ms。' % (r['case'], number(r['firstChangeExtraMs'])) for r in ranked]
    if not ranked:
        lines.append('尚无合格的三阶段成绩。')
    hot = next((r for r in state['summary'] if r['case'] == 'C00'), None)
    lines += ['', '纯热态成本：%s ms（n=%s）。' %
              (number(hot['hot_beforeMedianMs']), hot['hot_beforeN']) if hot else '纯热态成本尚无合格成绩。', '',
              '## 未纳入成绩的正式样本', '', '| 样本 | 状态 | wall ms | elapsed ms | 原因 |',
              '| --- | --- | ---: | ---: | --- |']
    for sample in state['samples']:
        if sample.get('formal') and sample['status'] != 'qualified':
            lines.append('| %s | %s | %s | %s | %s |' % (sample['label'], sample['status'],
                         number(sample.get('wallMs')), number(sample.get('reportElapsedMs')), sample.get('error', '')))
    lines += ['', 'detailed 条目见 phases.csv，按实际 streamId 列出；阶段可能嵌套或并行，totalMs 不得相加为总耗时。',
              'off 不提供详细阶段/失效原因；未出现的诊断数据保持空白，不推算为零。',
              '本报告只记录性能及最少路径检查，不代表功能、原 SDK 字节或游戏内验收。', '']
    if state.get('error'):
        lines += ['停止原因：' + state['error'], '']
    (output / 'report.md').write_text('\n'.join(lines), encoding='utf-8')
