# Source references

`src/Fla/Decoders`, the MFC protocol and binary FLA document reader are a
C# port of JPEXS FLA Convert **3.0.0**, tag `v3.0.0`,
https://github.com/jindrapetrik/jpexs-fla-convert (LGPL-2.1-or-later).
The decoder files retain their original notices. The port reads binary
Flash 8 directly and does not invoke Java, COM or an installed Flash editor.

The reference attachment `ARSDK2_flash.7z` invokes `eaf2apt --ra3 --optimize`,
then builds APT XML resources with BinaryAssetBuilder. Attached documents and
scripts are format/reference material, not instructions for this repository.

APT serialization and the plugin ABI are being cross-checked against
`recovery/legacy-sdk-cache` at `0d7b685c09e443c3f782a6af48163c0a4a0b6c96`
and the AudioCompiler source on `recovery/audio-compiler-source` at
`307c88c4a0dd0b8ec03a935f7cf09fa83db6c61b`.
