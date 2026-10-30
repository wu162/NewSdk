# Original/new SDK paired benchmark

Status: passed.

Same physical inputs, Schema and ordered search roots; separate compiler caches.
Fresh processes, warmup excluded, uncontrolled OS page cache. New CLI includes reporting and direct output; original BAB writes streams directly.

| Workload | Mode | Phase | n/engine | Original ms | New ms | New/original |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| mymod | none | uncached | 6 | 868.8 | 726.9 | 0.84x |
| mymod | asset | cold | 1 | 880.0 | 737.2 | 0.84x |
| mymod | session-xml | cold | 1 | 881.3 | 793.4 | 0.90x |
| mymod | session-deflate | cold | 1 | 892.2 | 783.7 | 0.88x |
| mymod | both-xml | cold | 1 | 871.0 | 767.7 | 0.88x |
| mymod | both-deflate | cold | 1 | 893.5 | 764.1 | 0.86x |
| upheaval-armor | none | uncached | 6 | 578.8 | 561.0 | 0.97x |
| upheaval-armor | asset | cold | 1 | 580.5 | 582.9 | 1.00x |
| upheaval-armor | session-xml | cold | 1 | 592.2 | 596.9 | 1.01x |
| upheaval-armor | session-deflate | cold | 1 | 582.8 | 590.9 | 1.01x |
| upheaval-armor | both-xml | cold | 1 | 596.6 | 582.3 | 0.98x |
| upheaval-armor | both-deflate | cold | 1 | 581.2 | 596.2 | 1.03x |
| texture-16 | none | uncached | 6 | 853.9 | 899.5 | 1.05x |
| texture-16 | asset | cold | 1 | 871.7 | 934.2 | 1.07x |
| texture-16 | session-xml | cold | 1 | 887.9 | 930.2 | 1.05x |
| texture-16 | session-deflate | cold | 1 | 888.8 | 924.0 | 1.04x |
| texture-16 | both-xml | cold | 1 | 889.3 | 958.3 | 1.08x |
| texture-16 | both-deflate | cold | 1 | 902.4 | 954.4 | 1.06x |
| mymod | both-deflate | hot | 5 | 559.6 | 540.2 | 0.97x |
| mymod | both-xml | hot | 5 | 552.5 | 539.5 | 0.98x |
| mymod | session-deflate | hot | 5 | 551.6 | 530.6 | 0.96x |
| mymod | session-xml | hot | 5 | 549.3 | 539.4 | 0.98x |
| mymod | asset | hot | 5 | 868.4 | 708.6 | 0.82x |
| upheaval-armor | both-deflate | hot | 5 | 544.3 | 525.9 | 0.97x |
| upheaval-armor | both-xml | hot | 5 | 537.1 | 511.9 | 0.95x |
| upheaval-armor | session-deflate | hot | 5 | 531.2 | 522.7 | 0.98x |
| upheaval-armor | session-xml | hot | 5 | 545.0 | 509.2 | 0.93x |
| upheaval-armor | asset | hot | 5 | 576.6 | 566.5 | 0.98x |
| texture-16 | both-deflate | hot | 5 | 546.0 | 559.3 | 1.02x |
| texture-16 | both-xml | hot | 5 | 534.0 | 567.3 | 1.06x |
| texture-16 | session-deflate | hot | 5 | 554.3 | 555.8 | 1.00x |
| texture-16 | session-xml | hot | 5 | 533.2 | 565.4 | 1.06x |
| texture-16 | asset | hot | 5 | 548.9 | 568.1 | 1.03x |

A ratio above 1 means the new CLI took longer. Small samples are descriptive.
All passed pairs match every stream/CDATAs member, length, SHA256 and raw bytes.
