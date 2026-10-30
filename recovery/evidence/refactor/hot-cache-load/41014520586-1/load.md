# Hot-cache workload duration floor

Status: failed

Each hot sample is one fresh compiler process with real retained caches. No sleep or inner repetition.
Summary profiling; loose base stream. Wall time includes startup/build/publication, excludes byte comparison.
Fixture growth, preparation, warmups, comparisons and all hot samples share the total load budget.

| Documents | Assets | Phase | Process seconds | XML parses | Compiles | Reused documents |
| ---: | ---: | --- | ---: | ---: | ---: | ---: |
| 21505 | 86016 | warmup | 59.652 |  |  |  |

AssertionError: 00-warmup build failed
