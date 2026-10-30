# Full Upheaval backend parallel A/B

Status: passed

| Sample | Wall ms | CPU ms | Compile calls | Max backend overlap |
| --- | ---: | ---: | ---: | ---: |
| nocache-cold-1 | 52087.4 | 52062.5 | 3354 | off |
| nocache-cold-4 | 34087.8 | 58718.8 | 3354 | off |
| nocache-cold-auto | 34158.6 | 58218.8 | 3354 | off |
| defaults-cold-4 | 36253.0 | 61109.4 | 3354 | off |
| defaults-hot-4 | 2357.0 | 2343.8 | 0 | off |
| defaults-hot-repeat-4 | 2288.4 | 2250.0 | 0 | off |
| defaults-hot-diagnostic-4 | 2333.8 | 2281.2 | 0 | 0 |
| defaults-cold-1 | 54710.1 | 54203.1 | 3354 | off |
| defaults-hot-1 | 2260.1 | 2203.1 | 0 | off |
| defaults-hot-repeat-1 | 2245.9 | 2234.4 | 0 | off |
| defaults-hot-diagnostic-1 | 2278.1 | 2187.5 | 0 | 0 |
| defaults-cold-auto | 36283.2 | 60750.0 | 3354 | off |
| defaults-hot-auto | 2323.9 | 2296.9 | 0 | off |
| defaults-hot-repeat-auto | 2273.2 | 2203.1 | 0 | off |
| defaults-hot-diagnostic-auto | 2280.7 | 2234.4 | 0 | 0 |
| diagnostic-cold-1 | 52235.3 | 52109.4 | 3354 | 1 |
| diagnostic-cold-4 | 34951.0 | 58984.4 | 3354 | 4 |
| diagnostic-cold-auto | 34676.5 | 58781.2 | 3354 | 4 |
| modified-independent-control | 21124.2 | 21203.1 | 1 | off |
| defaults-modified-1 | 2628.3 | 2562.5 | 1 | off |
| defaults-modified-4 | 2626.9 | 2562.5 | 1 | off |
| defaults-modified-auto | 2611.5 | 2531.2 | 1 | off |

Off = throughput comparison. Detailed = diagnostic attribution; durations overlap and cannot be added.

