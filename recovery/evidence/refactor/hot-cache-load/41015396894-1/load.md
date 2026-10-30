# Hot-cache workload duration floor

Status: passed

Each hot sample is one fresh compiler process with real retained caches. No sleep or inner repetition.
Summary profiling; independent metadata streams in one CLI build, loose outputs. Wall includes startup/build/publication, excludes byte comparison.
Fixture growth, preparation, warmups, comparisons and all hot samples share the total load budget.

| Documents | Assets | Phase | Process seconds | XML parses | Compiles | Reused documents |
| ---: | ---: | --- | ---: | ---: | ---: | ---: |
| 18435 | 73728 | warmup | 73.689 | 18435 | 73728 | 0 |
| 18435 | 73728 | hot-calibration | 9.373 | 0 | 0 | 18435 |
| 18435 | 73728 | hot-confirmation | 9.264 | 0 | 0 | 18435 |
