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

## Validation

`TestAudioXmaRawPacket.ps1` prepares original real-CPU versus isolated
Debug/Release comparison of1106 cases:1098 synthetic states (960 bit splices,
24 headers,60 next-packet transitions,30 padding cases,12 set/clear cases,
and12 complete two-packet construction sequences), plus8 hash-verified accepted
raw codec payload inputs. The real payload inputs do not establish actual
encoder-state capture. It compares the complete
`4050`-byte codec state, three full stream slots, raw packet buffer, all public
scalar results, input immutability and guard bytes. Fifteen candidate safety
refusals remain separate from defined native equivalence.

`actualEncoderStates=0` is intentional: these are comprehensive constructed
states, not captures of the original numerical encoder at packet append time.
The metadata real-state evidence does not expand this packet-writer claim.
Component sourceCommit/runId must be taken from the Windows validation receipt
when it completes; local instruction-emulator checks do not substitute for it.

Component acceptance: source
`d3f96009` (use the full sourceCommit in its receipt), run
[40236037500](https://github.com/wu162/NewSdk/actions/runs/40236037500),
attempt1: original CPU plus Debug/Release1106 cases and15 safety rejections
passed. Eight inputs are accepted real raw payload bytes; that run correctly
keeps `actualEncoderStates=0`. `TestAudioXmaRawPacketState.ps1` is a separate
pending two-long-case entry/return capture, reusing the accepted65536-frame
seeded stereo corpus (40233712043). It must preserve all three original
allocations and replay only captured header/next/padding states; it does not
repeat the1106 component cases or claim actual bitsplice-state coverage.
