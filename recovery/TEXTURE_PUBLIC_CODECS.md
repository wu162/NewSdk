# Public texture codec replacement experiment

Test branch: `validation/texture-public-codecs`; merge destination: `recovery/texturecompiler-source`. Only Windows GitHub Actions builds/tests; no force pushes. Upstream code is copied as a base and then modified to match legacy output, as the user explicitly requested. `PROVENANCE.json` pins repository commits and SHA256/Git blob IDs for all34 current files. For modified files it separately records original upstream identities and the compatibility patch; PATCHES.md explains changes. Notices are retained.

Candidates:

- libsquish1.15 from `castano/nvidia-texture-tools@aeddd65f81d36d8cb7b169b469ef25156666077e`: range/cluster/iterative fits, uniform/original PC channel weights, with/without alpha weighting (12 variants), complete DXT3/5.
- `stb_dxt.h` from `nothings/stb@003501f6d7a8668ce8b6d875e34dd97ad0d5d5b5`: normal/high-quality DXT5 and DXT3 composed from its color encoder and upstream squish explicit alpha (2 variants).
- NVTT `icbc.h` from the same pinned NVTT commit: fast/default/max quality × D3D10/NVIDIA/AMD decoder metrics × uniform/original PC channel weights (18 variants). Its RGB encoder is combined with upstream squish DXT3/DXT5 alpha.
- `stb_image.h` from the same STB commit: complete JPEG original corpus, including POT/NPOT/gray/progressive. Existing validated source-format NPOT filtering is kept as the common downstream adapter.

The one comparison job builds Debug/Release, checks x86 and that the original TextureCompiler DLL is absent/unloaded, then replays10,240 immutable original DXT3/5 blocks saved at `b0278705/40500898688` against every candidate. Counts distinguish full16-byte equality, RGB8-byte equality and alpha8-byte equality. Full source RGBA, original/candidate blocks and their identities are saved in each configuration's dxt-blocks.zip; TSV saves counts and initial counterexamples. Mismatches are results, not a successful acceptance of the candidate. Both configurations must have5120/5120 complete matches for a format before any consumer/SDK integration is attempted.

Sixteen DXT1/1A saved snapshots are compared separately and are explicitly probe-only: the legacy default state has an uninitialized mode boundary, so matching these snapshots cannot establish complete DXT1 support. STB/ICBC adapters report unsupported BC1-alpha rather than inventing it.

The JPEG comparison reuses1284 original records at `3f2351f2/40625499544`, preserving original rejections and complete pixels. The IJG6b production candidate already on the texture branch remains a separate acceptance line, with first component run40626161229 to collect; no STB success is inferred from IJG success.

Merge gate: complete byte equality in both configurations, full affected DDS/mip/volume/atlas/plugin regression and actual Original/Recovered/NoOriginal dedicated SDK outputs/traces. Failed/partial candidates remain on the test branch; no codec is enabled on the texture branch merely because it decodes visually or has a small error. Production SDK DLL/config are not changed by this experiment.

## Additional ATI/AMD legacy CPU experiment

After the initial32 configurations failed complete equality, add the retained legacyCPU CompRGBBlock/CompBlock1X implementation from GPUOpen-Tools/compressonator@095a143ad8cfbf9ed5afa4ea63c735bfd0fd04dd. Its original source/header identities are pinned; later compatibility changes retain the upstream hashes separately. Standalone compatibility headers provide only the original float/Win32 integer typedefs and standard library includes, choosing the upstream retained legacyCPU branch instead of the larger UI/GPU framework; no compression search is implemented in the shims. The adapter packs upstream endpoint/index results and uses the upstream8-vs6 alpha selection; DXT3 keeps the already-matching squish explicit-alpha primitive.

Twelve configurations: refinement steps0/1/2, three-dimensional refinement off/on, uniform/original PC channel weights. Explicit [public-codecs-amd] executes only these new candidates in the same Debug/Release10240-block matrix (122880 blocks/config); it does not repeat the initial32 or STB JPEG. All full input/expected/candidate artifacts are retained, and full5120/5120 per format in both configurations is still required before consumer/SDK integration. No new source is activated on the texture branch.


## Compatibility changes are the primary follow-up

Do not discard a candidate merely because stock bytes differ. Modify the copied
source against the immutable complete payloads, then repeat the full affected
matrix. Do not add fixture-ID dispatch, expected-payload tables or calls into the
original/recovered native encoder and present them as a public-source replacement.
Keep candidate changes and unchanged-source baselines separate.

- e1ecde44 / Actions40630771403 completed the first source patch comparison:
  libsquish equal565/constant-alpha canonicalization; ICBC equal565;
  AMD five alpha classes/low-first ties and normalized-float RGB input.
  D/R identical. AMD alpha improved2400→2771/5120; best complete DXT3=374,
  DXT5=311 (baseline369/308). ICBC DXT5 complete2→258/5120. No full acceptance.
- 1ce890cf / Actions40631302274 adds a readable constrained-fit/local-move
  scalar replacement within the copied AMD source. At this checkpoint it is
  still running; scalar/x87 tie limitations are explicitly unaccepted.
- 36637170 / Actions40631487946 adds two verbatim scalar source files from
  Team-Resurgent/RXDK-Libs@b7c9f2b3a24ece1aec850dca52155a4f8633686c,
  libs/libxgraphics/dxtc/{s3_quant.cpp,S3_quant.h}. Only CPU RGB/alpha routines
  are consumed on Win32; no platform API or platform product route is added.
  Four explicit variants preserve float/double input and channel-weight
  precision. DXT3 alpha remains the already exact public squish primitive;
  DXT5 uses copied CodeAlphaBlock. Complete results are still pending.

The copied S3 code exposes allSame/search43Mult/mapAndRoundMult/quantizeAlpha,
so it is a stronger algorithm-family candidate than changing an unrelated fit's
quality flags. A shared name is a research lead, never proof of byte equality.
