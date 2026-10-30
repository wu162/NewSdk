# Hot-cache workload duration floor

Status: failed

Each hot sample is one fresh compiler process with real retained caches. No sleep or inner repetition.
Summary profiling; loose base stream. Wall time includes startup/build/publication, excludes byte comparison.
Fixture growth, preparation, warmups, comparisons and all hot samples share the total load budget.

| Documents | Assets | Phase | Process seconds | XML parses | Compiles | Reused documents |
| ---: | ---: | --- | ---: | ---: | ---: | ---: |
| 8193 | 32768 | warmup | 34.046 | 8193 | 32768 | 0 |
| 8193 | 32768 | hot-calibration | 3.491 | 0 | 0 | 8193 |
| 16385 | 65536 | warmup | 33.412 | 8193 | 32768 | 8192 |
| 16385 | 65536 | hot-calibration | 6.767 | 0 | 0 | 16385 |
| 32769 | 131072 | warmup | 0.000 |  |  |  |

TimeoutExpired: Command '['D:\\a\\_temp\\hot-load\\runtime\\NewModCompiler.exe', 'build', '--sdk-root', 'D:\\a\\NewSdk\\NewSdk\\SDK', '--mod', 'D:\\a\\_temp\\hot-load\\LoadMod', '--out', 'D:\\a\\_temp\\hot-load\\build', '--package-content', 'base', '--package-format', 'loose', '--include-additional=false', '--jobs', '4', '--performance', 'summary']' timed out after 36.6646178 seconds
