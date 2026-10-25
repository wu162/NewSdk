# EA-XMA: concrete next recovery batch

2026-10-25 (Beijing). Read-only native audit; **no XMA runtime support or
acceptance is added**. Original input is `SDK/tools/BinaryAssetBuilder.AudioCompiler.dll`,
SHA256 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.
All addresses below are RVAs, not preferred-image absolute addresses.
`objdump -d` was used on the original PE; vtable words were read directly from
its PE memory-mapped image with pefile. The existing
[xma-wrapper-disassembly.txt](xma-wrapper-disassembly.txt) preserves the outer
API call. This note extends that evidence into the actual object methods.

## Resolve the indirect call before estimating the codec

`0xf275` calls `0x589d0`. This API uses 11 stack arguments (`ret 0x2c`),
allocates an object through `0x5a6d0`, calls its **vtable slot +4**, then releases
it through `0x58a70`. It is not calling the nearby `0x5a710` method.

`0x5a6d0` allocates **0x18 bytes**, sets its first word to preferred VA
`0x100c5040`, zeroes state fields +4/+0xc/+0x14 and sets byte +0x10 to 1.
The three code pointers at the vtable are:

| Vtable slot | Actual native method | Evidence / ABI |
|---|---|---|
| `0xc5040 + 0` | `0x5a710` | A different operation; enters RIFF/parser helper `0x58a90`. Not the encode call selected by `0x589d0`. |
| `0xc5040 + 4` | **`0x59a60`** | Actual encode dispatch. Fifteen stack slots including explicit object pointer; final `ret 0x3c` at `0x5a6c0`. |
| `0xc5040 + 8` | `0x59840` | Deleting destructor. `0x58a70` calls it with ECX=object and flag 1. |

For comparison, the outer SIMEX object vtable at `0xac870` contains
`0xecc0`, `0xf400`, `0x14250`, `0xeb70`, `0xeba0`, `0xb880`.
Those entries are a separate wrapper object, not the codec object's methods.

The direct ranges below exclude padding and exclude callees. Instruction
counts are decoded instruction rows (continuation-byte rows excluded), not
claims about a complete transitive call graph.

| Range `[start,end)` | Bytes / instructions | What it actually contains |
|---|---:|---|
| `0x589d0..0x58a61` | 145 / 59 | Public 11-argument adapter, object creation, indirect encode call, release. |
| `0x5a6d0..0x5a70f` | 63 / 23 | Small factory; allocation is not an encoder implementation. |
| **`0x59a60..0x5a6c3`** | **3171 / 954** | Parameter validation, quality table, per-stream state/buffers, preparation, encoding, final assembly, cleanup. |
| `0x5d180..0x5d397` | 535 / 171 | Per-stream codec initialization; already enters the substantial linked codec below. |

The encode dispatch alone has **28 distinct direct call targets and three
indirect call sites**. Its direct targets, including CRT/compiler support, are:
`09a9c 09aa8 09aae 09c5a 09e90 5861c 58760 59070 59650 5ae90 5af00
5afd0 5b090 5b160 5b240 5cae0 5cf50 5cf90 5d010 5d040 5d070 5d160
5d180 5e060 5e0e0 5e100 5e270 5f3e0`.
This is a measured first-level inventory, **not the whole codec size**.

A decisive deeper edge is `0x5a141 -> 0x5d180`. Initialization calls
`0x6a064` at `0x5d1d7`, `0x69e95` at `0x5d2a2`, **`0x659b8` at
`0x5d2d1`**, and `0x6978f` at `0x5d2e3`. `0x659b8` requests a
**0x18e50-byte (101,968-byte) codec state**, clears it and calls `0x6592c`.
Thus the 0x18-byte factory is only a facade over much larger persistent state.
The linked numerical encoder's complete transitive boundaries, tables, indirect
callbacks and rounding requirements have not been recovered here.

There is another state object: `0x5b160` installs vtable `0xc5320`, whose four
entries are `5b630 5b300 5b390 5b1e0`. This factory allocates 0x28 bytes and
invokes slot +0xc. Treating every indirect call as a leaf would miss this
additional output/state abstraction.

## Smallest useful complete group: packet/frame structure helpers

Current implementation: `XmaPacketBits.cs` and `XmaPacketScanner.cs` now cover
this closed group, with 1,025 original-machine-code fixtures and a real-CPU
Debug/Release differential prepared. Accepted source `96f72d037b840ad09590688e8d4ec1f998446d8d`,
run [40209977895](https://github.com/wu162/NewSdk/actions/runs/40209977895),
attempt1: real original CPU and Debug/Release each1025 exact fixtures;
6 candidate safety refusals. No encoder route is enabled. Public cross-checks are recorded in [PublicXmaReferences.md](PublicXmaReferences.md).

Start with the integer-only packet structure group, while preparing native
codec oracles in parallel with that implementation work. These four functions
have a closed direct dependency graph and do not call the large encoder,
allocation services, or trigonometric functions:

| Function | Exact range | Instructions | Direct dependencies |
|---|---|---:|---|
| Variable-length count writer | `0xe890..0xe946` | 59 | None |
| Bit-field accumulator | `0xea80..0xeae0` | 40 | None |
| Frame length / continuation reader | `0xeae0..0xeb70` | 53 | `0xea80` |
| Packet/frame scan and count emission | `0xef60..0xf0fa` | 121 | `0xeae0`, `0xe890` |

Total: **832 code bytes / 273 instructions**, before managed bounds policies
and tests. This is one coherent first batch; the count writer alone is too
small to represent useful XMA recovery progress. Next, the outer packet
rewriter `0xee00..0xef54` adds 340 bytes / 102 instructions and no direct calls.
Its input includes the outer SIMEX object state, so establish that state layout
before merging it into the same candidate API.

Important boundaries already visible in this group:

- The count writer changes encodings at **0xc0, 0x30c0, 0xc30c0,
  0x30c30c0**, then uses a five-byte representation beginning with 0xff.
  Probe both sides of each threshold and `uint.MaxValue`; do not substitute a
  generic LEB128/7-bit integer writer.
- `0xef60` works in 0x800-byte packet steps and treats 0x3fe0 as usable bits.
  Its initial frame offset is a 15-bit field, and values at/above 0x3fe0
  become a sentinel. The frame helper has an explicit **0x3fd1** split for
  the length field crossing the packet edge. These are distinct boundaries.
- The original bit accumulator unconditionally reads three source bytes for a
  positive bit count. Unsafe truncated inputs must not be executed in-process
  as if they established a defined rejection contract. Declare a bounded
  candidate domain and record separate malformed-input policy.

## Oracle entry points and the first actual codec milestone

A Windows x86 host can invoke `0xe890`, `0xea80`, `0xeae0` and `0xef60`
directly with Cdecl delegates or a register-preserving thunk. Pin DLL SHA/path,
keep source buffers and output guard bytes, and compare return values plus
all out parameters and written bytes. There are no host service hooks needed
for those four leaf/group routines. Use synthetic valid bitstreams around the
boundaries above **and actual XMA packet bytes from the existing original-only
XMA cases**. The existing plugin transcript buffers are outer EA output;
do not assume they are the raw `0x589d0` output without tracing the rewriter.

For actual codec progress, the next oracle should call **`0x589d0` on the real
CPU**, using the exact 0x3c-byte stream descriptors assembled by `0xf150..0xf1e5`
and output pointers assembled by `0xf234..0xf275`. Capture the raw returned
allocations **before** `0xee00` rewrites them. Save descriptor fields, planar
signed16 source bytes, all 11 arguments, HRESULT, every output length/array and
FPU settings. The wrapper uses the original CRT free IAT at `0xab1a8` for
three returned allocations (`0xf30b..0xf320`); allocator ownership must be
confirmed before freeing independently acquired output buffers.

This top-level native oracle is feasible without first recovering the whole
codec. A general Unicorn codec oracle is **not yet established**: it would
need CRT/compiler thunks, security-cookie/stack-probe handling, global state,
all indirect targets and any host-library numerical operations audited. The
resampler's successful emulator harness cannot simply be reused as proof.

After raw encode outputs and state are reproducible, trace **`0x59a60 ->
0x5d180 -> 0x659b8/0x6978f`** and the encode loop at `0x59070` on one fixed
mono stream, then stereo. Record actual target addresses at the three
indirect dispatch sites `0x5a4cd`, `0x5a529`, `0x5a58d`; do not infer them
solely from nearby vtables. Only then choose the first numerical codec group.
This audit does not establish a bound on the remaining implementation effort,
codec portability, or complete XMA support.
