# AudioCompiler merge and SDK review

Reviewed on 2026-10-26. Scope: PC Audio recovery, original/restored audio SDK
acceptance, the4/6-channel capability gap, and merge into compiler-source.
Production Audio now accepts WAVE1/2/4/6; AIFF/AIFC remains mono/stereo. No production code is changed by this
review.

## Accepted audio SDK scopes

| Evidence | Source | Scope |
|---|---|---|
| run40250750964 attempt1 | ed1a7bcbec4a7a89d08369b9668129c8c554f8ec | Complete WAVE4/6 group plus affected components,1728-asset real SDK |
| run40243817293 attempt1 | bb4a225fc5cc8e070044a01a24f87e425c612ccb | Dedicated4488-asset PC audio MOD |
| run40244825458 attempt1 | 2d64684742f9b19808d8fd721fbe31a549907c37 | Complete affected component group, whole source builds and dedicated1820-asset audio MOD |
| run40248486242 attempt1 | c078bce6e5e28866dfd3c25acabda731203b2556 | Full pinned Upheaval/PublicCases, only AudioCompiler replaced |

All four runs retain their own immutable source/input scopes. The1820/full-Upheaval
rows retain tree`ee6074a096795aa66b682fcb363e360feabe4d2f`; the4488 row
retains tree`4928109f17b3545cb0273f05def9198505371b33`;
current WAVE4/6 source is ed1a7bcb/treea8e47a with run40250750964.

Full Upheaval receipt:
`recovery/evidence/audio/upheaval-sdk-validation/40248486242-1/validation.json`
at `ef87339d3f76848687b714a394a9e1ff00ad1923`:

- Input pin `068643219fded09a05fb7b75c6507b769c430316`,2738 files,
  all194 precompiled.asset inputs preserved.
- Ten Original/Recovered BAB case3/4/5 invocations all exit0; twenty output
  products identical.
- Each side has1293 CustomData files and1289 Audio calls;1281 required
  declarations match1281 actual unique Audio assets.
- The original side loads original AudioCompiler; the recovered side replaces
  that plugin and has the original AudioCompiler DLL absent. Other plugins
  remain original SDK components on both sides.
- The measured corpus is mono WAV. It does not certify4/6-channel support or
  replace the dedicated AIFF/AIFC boundary SDK evidence.

## Fixed merge heads and shared tree identity

| Branch | Reviewed head | Meaning |
|---|---|---|
| recovery/compiler-source | b122a54abe4405516b021e425b8fb9235d914a0e | Shared compiler merge target |
| recovery/audio-compiler-source | 7545797e543147e4eb59b5d258e47281479f4ef8 | Earlier Audio handoff head |
| validation/audio-upheaval-win32 | ef87339d3f76848687b714a394a9e1ff00ad1923 | Accepted full audio SDK evidence head |

The reviewed compiler target is an ancestor of the accepted test branch.
Their complete SDK trees match:
`1f2d71a20ce8bf7d9343c7a2246c10275abe28f9`.
SDK/tools remains `10a9b868c74db96976c9c306dc587ec5de62965f`;
SDK/Schemas remains `62e1c5684ee0ea4407430f0a343a37431be6c03c`.
Complete shared recovery/src trees match:
`c13c492885fc64bfac0f53562360d706252b6489`.
Original SDK tools/configuration are not replaced in the repository. Recheck
actual remote heads immediately before merging and preserve concurrent work.
Unchanged shared compilers do not require component reruns merely because
Audio source/docs/evidence are merged.

## Safe Audio merge

1. Keep exact source/run/receipt links above. Mark only the measured audio
   scopes complete; preserve original-versus-restored module distinctions.
2. Read current compiler-source head and recheck tree identity if it moved.
   Preserve all concurrent additions and never force push.
3. Preserve compiler-source's complete generic RESUME navigation, then append
   a short Win32 Audio section linking AUDIO_COMPILER.md and acceptance-current.
   Do not overwrite shared GameObject/XML/W3X/Managed navigation with the
   Audio-only resume. Reviewed generic RESUME blob is
   `979fd0795cdd092953b50cf48128b2524510a0fa`.
4. Current WAVE4/6 source treea8e47a is accepted by its own run40250750964.
   Keep the full-Upheaval mono receipt at its old tree/source. Do not relabel it.
5. PR#10 is prepared for review. Merge only after explicit user approval.

## Remaining Audio recovery

AIFF/AIFC NONE4/6 original ordering and stable outcomes are established by
run40251368891; implementation, complete differences and real SDK acceptance
remain next. ADP4>2 is an original unsupported format. Keep undefined native
reads and explicit malformed-input boundaries. No source outside Audio or
repository SDK tool/configuration is replaced.
