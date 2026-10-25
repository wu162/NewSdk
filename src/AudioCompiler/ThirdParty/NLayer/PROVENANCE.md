# NLayer source dependency

Upstream: https://github.com/naudio/NLayer

Pinned commit: `046c7ce422970f8f0f0bc205d6c6341bcb7debf1`.

License: MIT, retained verbatim in `LICENSE`.

All 18 upstream `NLayer/**/*.cs` source files are compiled into the recovered AudioCompiler assembly. No NLayer DLL is shipped. The upstream project/build packaging files are not imported.

Local changes: converted two expression-bodied private methods in MpegFrameDecoder.cs and one null-conditional invocation in Decoder/MpegStreamReader.cs into equivalent C# 5 statements so the Windows .NET Framework compiler can consume the sources. NET8_0_OR_GREATER-only Span overloads remain conditionally excluded for the net472 target.

NLayer supplies MPEG decoding validation. It is an openly licensed replacement backend, not reconstructed RenderWare decoder source. The original DLL's seven-frame MPEG identification and runtime serialization are recovered separately. Differences in the decoders' treatment of corrupt/unsupported MPEG bitstreams remain a compatibility boundary and must not be claimed as universal original-DLL equivalence.
