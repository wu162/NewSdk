# EA-XMA integral PCM outer window scheduler

2026-10-26 (Beijing). Original DLL SHA256
`a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
**This batch recovers integral window scheduling, not an XMA encoder.**

## Complete method and supported paths

`src/AudioCompiler/Native/XmaPcmWindowScheduler.cs` reconstructs native
`98267..98624` for representations61/78/94 (signed16/signed20/signed24),
using the complete six leaves described in [XmaPcmWindow](XmaPcmWindow.md).
The native method has 32 stack slots and returns with `ret80`.

The real numerical dispatch site is `97d97: call [ESI+18e08]`.
The pinned PE installs `98267` at `673af`; its PCM16 window leaf slot
`[core+18e1c]` is installed with `98010` at `672f2`. Native CPU dispatch
recordings and complete raw output comparison live in the corresponding
numerical-dispatch evidence; this static method note does not substitute for
that run evidence.

Native parameter mapping:

| Slot | Meaning used by this method |
|---|---|
| 1..4 | core, newer source, older source, channel record (output pointer at +48) |
| 5..8 | fullFrames/2, windowFrames/2, int16 offset table, table index |
| 9..12 | unused for integral leaves, fullFrames, width=2*windowFrames, windowFrames |
| 13..15 | bytes/item, precision field, interleaved stride |
| 16..19 | leading zero count, rising end, copy end, falling end |
| 20..24 | rising w/q/previousW/previousQ/factor |
| 25..29 | falling w/q/previousW/previousQ/factor |
| 30..32 | representation, reader field, reader callback |

`98271` reads signed16 `table[index]`; slot8 is an index, not a table count.
The managed public entry takes that selected signed16 value, the two frame
sizes, four boundaries and two five-float coefficient groups directly.

Representations other than61/78/94 use native `9812a`/`98204` and a reader
callback. Those generic routes are **not recovered** by this bounded entry;
it rejects bits outside16/20/24. Nothing here establishes 8-bit or float
codec input handling, the transform or later quantization/entropy stages.

## Exact block and phase semantics

The initial frame position is
`a = selectedBlockOffset - windowFrames/2 + fullFrames/2`.
If a is before the full block end, the method starts from the older block
at a; otherwise it starts from the newer block at a-fullFrames. Older
availability is `min(a+width,fullFrames)-a`, or zero when already newer.
Source byte positions multiply frame index by stride and item size.

It writes leading positive zeros and advances the initial source by the same
number of interleaved items. The subsequent phases are:

1. Rising recurrence up to windowFrames/2; check old/new switch.
2. Continue that recurrence up to risingEnd, then unwindowed copy up to
   windowFrames; check old/new switch.
3. Copy up to copyEnd, reset to the independent falling recurrence, and
   window up to 3*windowFrames/2; check old/new switch.
4. Continue falling recurrence up to fallingEnd, then pad positive zeros to
   width.

There are **three switch points**, not an arbitrary switch at every source
frame. A phase whose end is at or before the current position is a native
no-op; the scheduler preserves that behavior by not calling the managed
forward-only leaf. Each switch resets the active source to the start of the
newer buffer. Final recurrence state is local, as in the original method.
All input buffers, coefficient groups and boundaries remain unchanged.

Managed safety policy requires positive even frame sizes, window no larger
than full frame, nonnegative bounded padding/phase ends, nonnegative initial
source position and initialized source slices. These policies do not claim
original malformed-pointer equivalence.

## Verification

`TestAudioXmaSchedule.ps1` runs the real original CPU first, then isolated
Debug/Release over3,240 complete scheduler cases: three sample formats, two
strides, full/short window geometry, five old/new starting offsets, four
padding/phase plans, three independent old/new source patterns and three
coefficient pairs. It compares every float output/padding bit, guards and
all input immutability. Fifteen managed safety refusals are separate.
Physical x87 CW027f, DLL SHA/load path and actual candidate absence of the
original target DLL are checked; previous physical CW is restored and
verified. Receipt: `evidence/audio/xma-schedule-validation/<run>-<attempt>`.

Local preflight compared360 complete geometries against original machine
code under Unicorn and an independent Python recurrence. This is preliminary
sanity only, not Windows/C# acceptance. The final receipt must provide actual
sourceCommit/runId and Debug/Release outcomes. Real raw-window snapshots are
an additional replay input, retaining their native provenance and distinct
scope; synthetic cases are not labeled real codec input coverage.
