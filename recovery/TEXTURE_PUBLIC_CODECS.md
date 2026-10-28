# Public texture codec replacement experiment

Test branch: `validation/texture-public-codecs`; merge destination: `recovery/texturecompiler-source`. Only Windows GitHub Actions builds/tests; no force pushes. Upstream source is copied, not rewritten as a guessed replacement algorithm. `PROVENANCE.json` pins repository commits, original Git blob IDs and SHA256 for all30 source/notice files. Notices and byte content are retained.

Candidates:

- libsquish1.15 from `castano/nvidia-texture-tools@aeddd65f81d36d8cb7b169b469ef25156666077e`: range/cluster/iterative fits, uniform/original PC channel weights, with/without alpha weighting (12 variants), complete DXT3/5.
- `stb_dxt.h` from `nothings/stb@003501f6d7a8668ce8b6d875e34dd97ad0d5d5b5`: normal/high-quality DXT5 and DXT3 composed from its color encoder and upstream squish explicit alpha (2 variants).
- NVTT `icbc.h` from the same pinned NVTT commit: fast/default/max quality × D3D10/NVIDIA/AMD decoder metrics × uniform/original PC channel weights (18 variants). Its RGB encoder is combined with upstream squish DXT3/DXT5 alpha.
- `stb_image.h` from the same STB commit: complete JPEG original corpus, including POT/NPOT/gray/progressive. Existing validated source-format NPOT filtering is kept as the common downstream adapter.

The one comparison job builds Debug/Release, checks x86 and that the original TextureCompiler DLL is absent/unloaded, then replays10,240 immutable original DXT3/5 blocks saved at `b0278705/40500898688` against every candidate. Counts distinguish full16-byte equality, RGB8-byte equality and alpha8-byte equality. Full source RGBA, original/candidate blocks and their identities are saved in each configuration's dxt-blocks.zip; TSV saves counts and initial counterexamples. Mismatches are results, not a successful acceptance of the candidate. Both configurations must have5120/5120 complete matches for a format before any consumer/SDK integration is attempted.

Sixteen DXT1/1A saved snapshots are compared separately and are explicitly probe-only: the legacy default state has an uninitialized mode boundary, so matching these snapshots cannot establish complete DXT1 support. STB/ICBC adapters report unsupported BC1-alpha rather than inventing it.

The JPEG comparison reuses1284 original records at `3f2351f2/40625499544`, preserving original rejections and complete pixels. The IJG6b production candidate already on the texture branch remains a separate acceptance line, with first component run40626161229 to collect; no STB success is inferred from IJG success.

Merge gate: complete byte equality in both configurations, full affected DDS/mip/volume/atlas/plugin regression and actual Original/Recovered/NoOriginal dedicated SDK outputs/traces. Failed/partial candidates remain on the test branch; no codec is enabled on the texture branch merely because it decodes visually or has a small error. Production SDK DLL/config are not changed by this experiment.
