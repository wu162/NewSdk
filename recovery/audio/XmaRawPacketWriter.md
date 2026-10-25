# Raw XMA packet lifecycle and bit splice

This group constructs raw XMA packets from already coded frame bits. It is
separate from the EA `ee00` output rewrite and the previously accepted
`e890/ea80/eae0/ef60` packet-count/scanner helpers. It does not encode PCM,
recover quantization/Huffman decisions, or enable AudioFile representation28.

Original DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs.

| Original helper | Recovered contract |
|---|---|
| `5c270..5c382` | Independent-buffer MSB bit splice; StdCall5 args `(source,destination,sourceBit,destinationBit,count)`, returns count |
| `5c390..5c3ae` / `5c3b0..5c3d0` | Set/clear one MSB-addressed bit, StdCall2 args |
| `5dc70..5dce8` | Finalize current raw packet header, ThisCall `(state,streamIndex)` |
| `5dcf0..5dda4` | Zero/start next2048-byte packet, link previous packet skip byte and update state, ThisCall `(state,streamIndex,firstFrameOffset)` |
| `5ddb0..5ddf0` | Pad alignment bits and remaining packet bytes with ones, StdCall `(packet,bitsUsed)` |

## Actual call relationships

`5e8f0` appends already coded frame data and calls bit splice at
`5ea06/5ea98/5eb5a/5ec06/5ec47`; it uses next-packet at `5eb0f/5ec21`,
header finalization at `5eb78/5ec14`, padding at `5ebdc` and continuation
clear/set at `5eb9b/5ec6d`. The complete appender is not recovered by this
helper group: its frame/header orchestration and additional dependencies must
be restored separately.

The actual final raw-block path `5ec90` calls header finalization at `5ecfa`,
clears the last continuation bit at `5ed1e`, aligns with set-bit at `5ed34`
and fills the remaining bytes with ff before appending the raw block. These
observed relationships establish the group as the raw output construction
layer; it is not confused with `60136 -> 728e3`, which primarily consumes the
codec output FIFO and additionally depends on buffer compaction and rate-clock
accounting (`70fd6/710a2`).

## State and finite-domain rules

Native codec state uses `+402c` for the stream-slot array, `+4030` for the raw
packet buffer, `+403c` for the next global packet index. Each stream slot has
stride`818`, with `+8` frame count, `+c` current packet index, `+10` bits used.
`StartNextPacket` exposes exactly those changing scalar fields through ref/out
parameters; all other native state/slot bytes remain untouched.

- FinalizeHeader ORs `(frameCount<<26)` and bit8 into the existing BE32 header.
  It does not clear previous frame-count bits. Zero frame count writes the
  first-frame-offset sentinel`7fff`.
- StartNextPacket clears the new packet, masks its first offset to15 bits,
  writes previous skip as uint8 `(new-old-1)`, advances the global index, and
  resets selected stream count/bits to0/32. If old and new are the same packet,
  the final skip write makes its byte3 ff; that native alias-of-indices behavior
  is explicitly tested.
- Native bit splice reads one additional source byte for every nonaligned
  source chunk, even for a tiny final count. A short final chunk can clear
  destination bits beyond the requested logical range. The implementation
  preserves these writes rather than substituting generic bit-preserving copy.
- Positive bit-copy counts require independent arrays, full native source
  lookahead and complete destination capacity. Counts<=0 return without
  dereferencing either array, as the original does. Source/destination aliasing
  is explicitly refused; no generic memmove claim is made.
- Packet indices must address complete2048-byte regions. Padding is bounded
  to0..16384 bits. Invalid indices, lookahead and capacity fail before modifying
  packet bytes. Native out-of-bounds behavior is not copied.

## Current validation scope

| Group | Exact sourceCommit / run | Accepted scope |
|---|---|---|
| Component matrix | `d3f96009e64b44c4d9482421cc5922b3309320bc` / [40236037500](https://github.com/wu162/NewSdk/actions/runs/40236037500), attempt1 | Original CPU and Debug/Release1106 cases:1098 synthetic states plus8 accepted real raw payload inputs;15 safety rejections. Historical receipt keeps actualEncoderStates=0. |
| Actual entry/return states | `93ba818ef53b6eb23a94a2f847409aadb31f4c44` / [40236621021](https://github.com/wu162/NewSdk/actions/runs/40236621021), attempt1 | **92 observations:63 header,29 next-packet,0 padding**; original and Debug/Release replay agree. Two65536-frame seeded stereo encodes preserve all three accepted original allocations and input fingerprints. |

The1106-case component matrix includes960 bit splices,24 headers,60 next-packet
transitions,30 padding cases,12 set/clear cases,12 complete two-packet sequences
and8 hash-verified accepted raw payload inputs. It checks the complete4050-byte
synthetic codec state, three complete stream slots, raw buffers, scalar results,
source immutability and guards. Safety refusals are not native equivalence cases.

The92 actual observations prove only the exercised header and next-packet
transitions. **No actual padding or bitsplice state was captured**; padding
retains its30 synthetic-case proof and bitsplice its component/payload proof.
Do not broaden this into complete lifecycle actual-state coverage or XMA audio
encoding support. The successful1106 group was not repeated for this capture.

The actual-state tool patches helper prologues with native-only entry stubs and
uses a different native return continuation per record. It temporarily changes
the helper's return slot, preserves integer registers/flags and performs no FP
operation, CLR callback or VEH. It copies exactly the old/new2048-byte packet
regions and used scalar fields; returns are paired per record. All entries are
restored and checked before producing acceptance. The original codec output
comparison is a required gate, not an assumption about instrumentation.

Source capacity is independently checked against state+4028:5f44f writes block
bytes, and5f464..5f485 allocate floor(blockBytes/2048)*2048 at state+4030.
Capture bounds are never relaxed to force a successful observation. The baseline
is the immutable long corpus source579c95661d90f067ab414c4133f033c2571b96e1 /
run40233712043-1; case metadata and file mappings must match its pinned manifest.
