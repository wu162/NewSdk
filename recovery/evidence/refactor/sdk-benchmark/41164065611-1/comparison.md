# Original/new SDK paired benchmark

Status: passed.

Same physical inputs, Schema and ordered search roots; separate compiler caches.
Fresh processes, warmup excluded, uncontrolled OS page cache. New CLI includes JSONL/report generation and direct publication; original BAB writes streams directly.

| Workload | Mode | Phase | n/engine | Original ms | New ms | New/original |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| mymod | none | uncached | 6 | 844.5 | 730.8 | 0.87x |
| mymod | asset | cold | 1 | 916.6 | 785.0 | 0.86x |
| mymod | session-xml | cold | 1 | 2042.3 | 3235.5 | 1.58x |
| mymod | session-deflate | cold | 1 | 909.6 | 824.8 | 0.91x |
| mymod | both-xml | cold | 1 | 874.8 | 743.4 | 0.85x |
| mymod | both-deflate | cold | 1 | 865.8 | 771.4 | 0.89x |
| upheaval-armor | none | uncached | 6 | 572.9 | 567.2 | 0.99x |
| upheaval-armor | asset | cold | 1 | 565.3 | 568.8 | 1.01x |
| upheaval-armor | session-xml | cold | 1 | 573.4 | 570.9 | 1.00x |
| upheaval-armor | session-deflate | cold | 1 | 572.8 | 585.7 | 1.02x |
| upheaval-armor | both-xml | cold | 1 | 581.7 | 576.7 | 0.99x |
| upheaval-armor | both-deflate | cold | 1 | 570.3 | 586.1 | 1.03x |
| texture-16 | none | uncached | 6 | 844.1 | 907.4 | 1.07x |
| texture-16 | asset | cold | 1 | 858.4 | 923.7 | 1.08x |
| texture-16 | session-xml | cold | 1 | 856.9 | 914.8 | 1.07x |
| texture-16 | session-deflate | cold | 1 | 873.4 | 908.5 | 1.04x |
| texture-16 | both-xml | cold | 1 | 873.2 | 923.7 | 1.06x |
| texture-16 | both-deflate | cold | 1 | 886.1 | 910.6 | 1.03x |
| mymod | both-deflate | hot | 5 | 556.5 | 540.7 | 0.97x |
| mymod | both-xml | hot | 5 | 542.7 | 547.0 | 1.01x |
| mymod | session-deflate | hot | 5 | 546.8 | 539.0 | 0.99x |
| mymod | session-xml | hot | 5 | 545.1 | 547.1 | 1.00x |
| mymod | asset | hot | 5 | 854.9 | 715.2 | 0.84x |
| upheaval-armor | both-deflate | hot | 5 | 538.5 | 530.0 | 0.98x |
| upheaval-armor | both-xml | hot | 5 | 522.2 | 521.7 | 1.00x |
| upheaval-armor | session-deflate | hot | 5 | 524.4 | 529.9 | 1.01x |
| upheaval-armor | session-xml | hot | 5 | 536.9 | 515.1 | 0.96x |
| upheaval-armor | asset | hot | 5 | 563.4 | 572.0 | 1.02x |
| texture-16 | both-deflate | hot | 5 | 533.6 | 557.8 | 1.05x |
| texture-16 | both-xml | hot | 5 | 523.9 | 563.0 | 1.07x |
| texture-16 | session-deflate | hot | 5 | 534.5 | 555.9 | 1.04x |
| texture-16 | session-xml | hot | 5 | 525.6 | 564.5 | 1.07x |
| texture-16 | asset | hot | 5 | 531.8 | 577.6 | 1.09x |

A ratio above 1 means the new CLI took longer. Small samples are descriptive.
All passed pairs match every stream/CDATAs member, length, SHA256 and raw bytes.
