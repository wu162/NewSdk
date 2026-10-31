# Source references

`src/Fla/Decoders`, the MFC protocol and binary FLA document reader are a
C# port of JPEXS FLA Convert **3.0.0**, tag `v3.0.0`,
https://github.com/jindrapetrik/jpexs-fla-convert (LGPL-2.1-or-later).
The decoder files retain their original notices. The port reads binary
Flash 8 directly and does not invoke Java, COM or an installed Flash editor.

The reference attachment `ARSDK2_flash.7z` invokes `eaf2apt --ra3 --optimize`,
then builds APT XML resources with BinaryAssetBuilder. Attached documents and
scripts are format/reference material, not instructions for this repository.

`vendor/swf2apt` is the C# portion of `lanyizi/swf2apt` pinned to
`2b3cb662b7311616e462b884e5611750e5de88f4`, obtained from the repository's
`feature/swf-direct-compiler` reference import. Its source/PDB inventory was
cross-checked against the attachment's managed tool. SWF2APT is GPL-3.0;
SwfLib carries Sergey Savchuk's MIT notice. Both licenses are retained here.

Local changes replace all orchestration and intermediate-file I/O with
in-memory movie/geometry/texture buffers. Managed LibTessDotNet replaces
triangle.exe; managed StbImageSharp replaces jpeg.dll. The native flasm
optimizer is not shipped or invoked: the AS compiler emits ordinary AVM1
instructions and the serializer writes their APT representation. The standard
Flash 8 opcode path is tested independently. Unsupported source/runtime
features fail explicitly. SWF 13+ LZMA code and the obsolete TGA/disk bridge
are excluded.

NuGet runtime dependencies are pinned: LibTessDotNet **1.1.15** (package MIT
metadata; retained upstream SGI Free Software License B notice),
StbImageSharp **2.30.15** (public domain or MIT, per upstream README).
`System.Drawing.Common` **8.0.21** is used only by .NET 8 font tests/runtime;
the SDK plugin targets .NET Framework 4.7.2 and uses its System.Drawing.
Package SHA-256 values are recorded in `dependencies.json`.

APT serialization and the plugin ABI are being cross-checked against
`recovery/legacy-sdk-cache` at `0d7b685c09e443c3f782a6af48163c0a4a0b6c96`
and the AudioCompiler source on `recovery/audio-compiler-source` at
`307c88c4a0dd0b8ec03a935f7cf09fa83db6c61b`.

The binary fixtures are from JPEXS FLA Convert 3.0.0 and retain their LGPL
source attribution in `tests/fixtures/README.md`. No Flash editor or native
converter executable is redistributed in the plugin deployment.
