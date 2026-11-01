# Original SDK versus current compiler

Status: failed

Complete downloaded The New World source copy; no assets or includes removed.
All entries in selected groups base,apt,global-metadata, loose output; no BIG/SKUDEF packaging.
Same Windows VM, SDK, physical MOD input, Schema, cwd and ordered search roots. Separate caches.
Fresh process per sample, process launch through exit includes CLI/report overhead. Hash checks excluded.
Cold = initially absent compiler cache/output namespace; OS page cache is uncontrolled.
New Release jobs4, performance off; original BAB has its original serial implementation.
First paired cold engine: new; trial/phase order alternates.

| MOD | Mode | Phase | n/engine | Original seconds | New seconds | New time change |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| The New World | both-deflate | cold | 1 | 1933.785013 | 413.702670 | -78.61% |

Original total/parts sum each sequential BAB process launch-to-exit; new total is one complete CLI process.
New parts use Owner Compile intervals when available, otherwise StreamStarted/StreamCompleted events. Parallel group durations overlap and must not be added to obtain total wall time. Shared overhead subtracts the interval union, never the sum of overlapping groups.

| Mode | Phase | Content | n/engine | Original seconds | New stream seconds |
| --- | --- | --- | ---: | ---: | ---: |
| both-deflate | cold | base | 1 | 1884.839936 | 406.151752 |
| both-deflate | cold | apt | 1 | 33.506086 | 3.484374 |
| both-deflate | cold | maps | 1 | 15.438991 | 26.855773 |

All raw samples, CPU/peak working set and output identities are in measurement.json.
All cache-state repeats must match their own engine baseline byte for byte.
Cross-engine differences, if present, are retained below; such results do not establish equivalent output.
Small sample counts describe this VM and workload; no statistical significance or game-runtime claim.

- Output mismatch: The New World trial 0 both-deflate cold: base/mod/mod.bin
- Failed sample: new trial 0 both-deflate hot: ValueError: Process failed: The New World-0-both-deflate-hot-1-new

Shared runtime Schema overlay: {"source": "D:\\a\\NewSdk\\NewSdk\\SDK\\Mods\\The New World\\xsd", "changed": [{"path": "AssetTypeLocomotorTemplate.xsd", "sha256": "2c04dc9b5d4f7724f3a5eb057715d2b988ac7dcec1c3b72508a89ee0d94db601"}], "scope": "MOD-provided runtime Schema shared by both engines; production binaries unchanged"}
