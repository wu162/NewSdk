# XMA codec metadata serialization group

The recovered group builds output metadata from the encoder's existing
per-stream 32-byte format records and block frame counts. It does not take PCM,
produce coded audio, or enable AudioFile representation28.

## Original routines and state contract

Original DLL SHA256:
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses are RVAs. Actual flags2 codec finalization calls `5f170`,
`5f000`, then `5eea0` from `5f5b2/5f5ca/5f5e2`.

| Original routine | Behavior |
|---|---|
| `5f170..5f3df` | EA format, 40-byte prefix plus 4 bytes per stream; stores through `5ae90` |
| `5f000..5f16d` | 52-byte XMA2 WAVE format; stores through `5af60` |
| `5eea0..5effc` | BE32 cumulative seek positions: running sum of `(blockCountValue << 9)`; stores through `5b500` |
| `5af00` / `5afd0` | Export fresh copies of the existing EA/WAVE format buffers, not format builders |
| `5b570` / `5b5d0` | Export flat seek data / calculate seek byte length |

Input native state has stream count at `+4`, a format pointer at
`+0x10 + stream*0x38`, loop begin/end at `+0x401c/+0x4020`, play length at
`+0x4024`, block bytes at `+0x4028`, number of blocks at `+0x4038` and a
pointer to uint32 block counts at `+0x4040`. Only the listed state fields and
32-byte source records are needed for serialization. The native state itself
is not changed by this group.

The target output object owns EA pointer/length at `+8/+c`, WAVE pointer/length
at `+10/+14`, and a seek pair pointer at `+24`; the pair contains count/pointer.
The CPU oracle uses the original CRT allocator and free IAT `ab1a8` for original
owned buffers and export copies. Its mocked state/format/count containers are
separately owned by Marshal and are never passed to original free.

## Exact details preserved

- EA layout is version4, byte stream count, loop byte, two adjusted BE32 loop
  positions, native sample rate, promoted native uint16 field at source+4,
  native uint32 at source+c, block bytes, total samples, play length, block
  count, then stream channel/mask records. Unnamed source fields are copied as
  observed rather than assigned speculative codec meanings.
- Loop values use unchecked uint arithmetic: a nonzero begin/end loses64;
  the WAVE loop-length field uses `end-adjustedBegin-64` when end is nonzero.
- The WAVE speaker-mask path ORs **the two bytes** of each native source mask,
  ORs streams, then maps bits4/5 to output bits9/10 and bits6/7 to bits4/5.
  It is not a direct uint16 mask copy. WAVE channel/block-count fields truncate
  to uint16 exactly as the original does.
- Seek positions and total sample counts retain uint32 wrapping. Zero blocks
  yield an empty seek array.

Managed policy requires1..256 streams and exactly32 bytes per format record.
The upper bound follows the original EA writer's fixed `0x428`-byte local
buffer (`40+4*256`), not a claim that the complete encoder accepts256 streams.
Null inputs and malformed record sizes are rejected. Allocation failures and
out-of-bounds native behavior are not emulated.

## Validation scope

`TestAudioXmaCodecMetadata.ps1` runs all384 synthetic-state cases on original
Windows x86 CPU, then isolated Debug/Release builds with no original DLL present
or loaded. All three output buffers, export lengths, fresh allocation semantics,
source immutability and input guards are checked. The matrix spans8 stream
counts,6 block counts and8 patterns covering masks, unsigned wrapping and loop
boundaries;15 candidate safety rejections are separate from equivalence cases.

The original codec's real44-byte EA format and4-byte seek output have already
been captured by the raw codec oracle. This metadata component matrix does not
claim capture/replay of the internal state that generated those exact outputs;
its receipt therefore records `actualWrapperStates=0`. Original real-codec
outputs must not be reverse-derived into inputs and presented as independent
wrapper-state evidence. Exact component sourceCommit/runId must be taken from
its validation receipt after the Windows run completes.
