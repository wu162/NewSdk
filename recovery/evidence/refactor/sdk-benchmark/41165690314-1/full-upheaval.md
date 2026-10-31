# Full untouched Upheaval original/new SDK

Status: output-differs

All five streams, base/APT/global metadata, loose outputs including CDATA/version files.
Same physical inputs and roots, separate caches. Original runs sequential BAB processes; new uses one process and jobs=auto.
Launch-to-exit costs include reporting; preparation and output verification are outside timing. No BIG/LoD packaging or game-runtime timing.

Cross-engine output differences fail acceptance even when timings complete; timings with different outputs are descriptive only.

| Phase | n/engine | Old ms | New ms | New change |
| --- | ---: | ---: | ---: | ---: |
| uncached | 2 | 91495.27 | 34268.66 | -62.55% |
| cold | 2 | 89352.50 | 35539.42 | -60.23% |
| hot | 3 | 4563.89 | 1954.63 | -57.17% |
