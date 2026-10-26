# Next AIFF/AIFC NONE4/6 capability group (preparation only)

`AudioMultichannelAiffCases.cs` is an independent, uncompiled matrix generator.
It is not included in the accepted WAVE PR and does not enable production input.
No component or SDK success is claimed. APIs: Run(root, Action<string,int,string,bool>)
and WriteSdkCases(root), each enforcing1728 unique asset IDs when fully consumed.

The108 fixture files cover AIFF and canonical AIFC/NONE (FVER, COMM with empty
Pascal compression name), channels4/6, integral8/16/24, frames127/128/129/4096.
Each short length uses no-loop/start0;4096 also uses intro129. Every fixture
gets4 target rates ×2 codecs ×2 stream modes:1728 total. MARK IDs7/11 define
sustain start and exclusive end=source frames; INST is forward sustain with
no release loop. SSND offset/blockSize are zero. Per-channel composite samples
remain in source order; source PCM24 deliberately varies the discarded low byte.
This baseline group does not claim all malformed metadata/offset permutations.

Original-only prerequisite is available: source0d744a716df7b487816dc0fadf4f12dc82c5b66c,
run40251368891 attempt1, receipt commit5f2574fc7f60b5b63db4100f7b86e9e832efe15d.
192 cases/384 processes succeeded and repeated identically. Independent NONE
sample decoding confirms AIFF/AIFC4→[0,1,2,3],6→[0,1,2,3,4,5]. That bounded
129-frame/no-loop original observation is not validation of this larger matrix.

Minimum next-round wiring, only on a separate preparation branch:

1. Add the generator to plugin/support host compile inputs. Add an explicit
   multichannel-aiff group calling Run and require1728 full successful original
   and recovered three-buffer/CustomData records per Debug/Release configuration.
   Preserve WAVE entry points and prior fixture groups. Include.aifc when copying
   evidence fixtures: current TestAudioPlugin copies only.wav/.aiff.
2. In AudioPcmInputReader, admit4/6 for integral AIFF/AIFC NONE as well as WAVE,
   but explicitly reject ADP4 above2 before decoder invocation/allocation. Keep
   WAVE-only RemapWaveChannels; AIFF/AIFC NONE must remain source order. Do not
   broaden the independent Pcm16Encoder.ReadWave into an AIFF reader. PCM/XAS,
   loop and resampling channel guards already admit4/6 after the accepted WAVE
   group; no second remap or codec/layout rewrite is needed by this source audit.
3. Update refusal tests intentionally: existing AudioMultichannelBoundaryOracle
   labels AIFF/AIFC NONE4/6 as unrecovered and must no longer assert their rejection
   after this feature is implemented. Keep3/5/8 and ADP4>2 refusal checks, unsafe
   single-frame imports, partial marker and out-of-SSND safety boundaries.
   Retain complete affected mono/stereo AIFC/ADP4/loop/COMM/SSND/offset regression.
4. Add a separate explicit AIFF multichannel SDK profile selecting only this
   generator's IDs and requiring1728 assets/schema success before BAB. Reuse
   original-first gates, identical pinned inputs/commands, non-target DLL
   fingerprints, original DLL absence,1728 unique actual callbacks, three buffers,
   CustomData and four products. Never use the WAVE1728 receipt as this profile's
   acceptance. Preserve the mono-only full Upheaval receipt's original scope.
5. Only after final component and dedicated SDK terminal success update support
   declarations. Keep all original source/run anchors immutable. No AIFF4/6
   implementation is part of the current accepted WAVE PR.

Static checks only:1728 unique names,108 fixture names, loop ranges and chunk
length arithmetic. C# compilation, full original matrix, candidate differential,
schema and real SDK execution remain pending the next round.

Remote checkpoint branch: `preparation/audio-aiff-multichannel`. This branch has no enabled AIFF4/6 production change or new CI result. For implementation, start from the latest reviewed target and import these preparation files; preserve concurrent source/navigation changes. Register the explicit new branch/marker before running a new workflow. All merges require a PR and explicit user approval.

Additional integration checks found during final review:

- AudioPluginOracle currently recognizes WAVE4 with the literal `MultichannelC4B`. The proposed AIFF labels use `MultichannelAiffAIFFC4B` / `MultichannelAiffAIFCC4B`; update this test-only classifier deliberately when wiring the new group, while preserving the already accepted WAVE metadata assertions. Otherwise every new4-channel case would be mistaken for6.
- The1728 generator is the canonical baseline, not the entire COMM/SSND boundary inventory. Add4/6-channel cases around COMM declaration filtering/mismatch and channel-width-dependent SSND windows: frame sizes now extend to18 bytes. Preserve valid in-payload offset remainders, exact-bound/one-past refusals, blockSize behavior and existing partial-marker/single-frame refusals. Reuse audited native semantics; do not equate old1/2 evidence with new4/6 execution. Retain the1728 baseline intact if adding boundary assets, derive the exact larger count and register that exact SDK profile/count instead of a count range.
- The full-Upheaval gate now distinguishes accepted `create-marker-range`/1820 and `create-multichannel`/1728 receipts and binds source tree, source/run/attempt and Debug/Release build. Register the new AIFF profile explicitly with its own exact count and successful receipts; never relabel the WAVE1728 profile as AIFF acceptance.
