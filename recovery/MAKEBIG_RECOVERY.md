# Recovered MakeBig

`SDK/tools/MakeBig.exe` is EA Los Angeles' managed 2007 MakeBig, not jonwil's native cc3tools. Its assembly references only `mscorlib, Version=2.0.0.0` and `System, Version=2.0.0.0`. The recovered source is physically compiled into `NewModCompiler.exe` under `recovery/src/App/Application/MakeBig.cs`; no MakeBig child process, original assembly load or business DLL is used.

The previous `BigArchiveWriter` already implemented an independently validated exact-inventory BIG4 writer. This change retains its production boundary and replaces its format emitter with recovered `Lumpy`/`FixupBinaryWriter`. Production calls `Application.MakeBig.Write` → strict `BigArchiveWriter` inventory/size/atomic validation → recovered `Lumpy.WriteArchive`. It does not discover stale staging, intermediate or cache files. Existing independently accepted writer results remain historical evidence; the recovered emitter and affected production need fresh acceptance.

## Original method evidence

Source was recovered directly from IL of the repository's original executable. Original EXE SHA256: `2eaa2dfa8473ed5a74b5901c65ab133ddea26f70c8cb2747a0d8725c6c5a7cf9`. The primary methods are:

| Original class/method | RVA | Recovered behavior |
| --- | --- | --- |
| FixupBinaryWriter.WriteBookmark / ResolveBookmark | `0x2088` / `0x20f8` | uint bookmarks with endian mode captured at reservation |
| FixupBinaryWriter.Write / WriteBookmarks | `0x2160` / `0x21e4` | uint endian conversion, offset-sorted fixup writes |
| FixupRecord | `0x228a–0x22f8` | key, uint offset/value, original max-uint unset sentinel |
| Lumpy.BuildLumpy / RawDataBuffer | `0x232c` / `0x2670` | fixed BIG4 writer, UTF8 BinaryWriter names and raw file data |
| ParseArguments.WildcardToRegex / parse | `0x270e` / `0x2744` | anchored escaped regex, colon arguments, short/long options |
| Program.Main / GetDirectories | `0x2974` / `0x2b74` | validation/overwrite handling, child-first native enumeration |
| Program.PrintHelp | `0x2c44` | original option descriptions |

There are no original compression, endian, alternate BIG-magic or sorting options. The format is fixed: `BIG4`, little-endian total size, big-endian member count/index length/offsets/sizes, UTF8 lowercased relative paths with backslashes and NUL terminators, `L219` plus four zero bytes, then contiguous uncompressed member data.

Directory traversal recursively consumes each child directory before the current directory's files. Native enumeration order is retained, with no added sorting. Each exclusion is `^` + escaped lowercased wildcard (`*`→`.*`, `?`→`.`) + `$`, matched against the entire lowercased **absolute file path**, not its basename. Multiple excludes combine as OR. Case conversion uses the current culture as in the original.

The parser supports `-h/--help`, `-q/--quiet`, `-v/--verbose`, `-f/--force`, `-x:pattern/--exclude:pattern`, and `-o:path/--outputFile:path`. Unknown switches are ignored. Later directory/output arguments win; exclusions accumulate. A separate following argument is not a value syntax. Original quiet/verbose edge behavior and the two-read uppercase-Y overwrite prompt are retained in the explicit legacy runner. An empty directory produces a 24-byte BIG4 archive; production publication still rejects an empty inventory.

## Embedding boundaries

Original classes live under `NewModCompiler.Recovered.MakeBig`; `Program.Main` is exposed as `Program.Run` with supplied reader/writer channels, avoiding a second executable entry point. A fresh parser is used for every embedded call. No legacy prompt runner is invoked by normal MOD builds.

The shared emitter streams bounded chunks instead of allocating twice the largest input. It validates complete reads, supports cancellation and restores the final stream position after patching. Production retains its prior safe relative ASCII member names, deterministic ordering, no overwrite, case-collision/reparse/overflow checks and atomic cleanup. Unique member bookmark keys preserve production aliases of a single source file; the original directory runner retains original source-path bookmark keys. Directory discovery additionally refuses reparse points to avoid recursive cycles. These are integration adaptations, not claims that the old tool implemented those protections.

Original standalone MakeBig emits UTF8 non-ASCII member names. This recovered component behavior does not establish game/runtime Unicode support, and the normal MOD packaging boundary remains restricted to its previously supported ASCII names.

`recovery/preparation/makebig/TestCompilerMakeBig.py` compares original/recovered runs on the **same input directory**, including full archive bytes and parsed order/payloads. Cases cover recursive mixed-case and UTF8 files, complete absolute-path exclusion, repeated exclusions, unknown options, quiet/verbose, empty trees, force and original prompt/refusal behavior. Independent tests use a standalone control host; production contains no test entry point. App tests cover parser and bookmark boundaries, directory traversal, force/refusal and same-source logical aliases. Fresh Windows and production acceptance is required before marking this recovery complete.
