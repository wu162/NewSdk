# Texture pure image rules

> **历史记录：自 2026-10-26 起仅以 Win32/PC 为目标。本文非 PC 能力、旧待办和旧平台矩阵不再适用；当前实现与计划以 [TEXTURECOMPILER.md](TEXTURECOMPILER.md) 为准。原 sourceCommit/runId 仅证明原验收版本，不代表本轮删除后的代码已验证。**

Source: original `SDK/tools/BinaryAssetBuilder.TextureCompiler.dll`, SHA256 `4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e`; immutable extraction run `40191971356-1`.

`src/TextureCompiler/Managed/TextureImageRules.cs` reconstructs:

- Module methods `06000018` / `06000019`: binary and constant alpha scans. Pixels use alpha byte at RGBA offset 3; the loops use unsigned, wrapped 32-bit width × height. Empty images return true for both scans.
- Plugin `IsValidTexture`: reject wrapped zero pixel count, then wrapped byte count ≥ 64 MiB, then non-power-of-two dimensions, in exactly that order. The size check is deliberately **not** rewritten as a 64-bit mathematical size check: dimensions whose original uint arithmetic wraps retain their original dimension-only result. Actual image processing needs independently valid storage.
- Plugin `GuessProperFormat`: only exact `D3DFMT_DXT1` plus `hasAlpha=true` triggers inference. Constant opaque stays DXT1; constant nonopaque or variable binary alpha becomes DXT3; other alpha becomes DXT5. Original diagnostic text is preserved through injectable callbacks.
- Plugin `GetNVCompressionOptions`, `nvCompressionOptions` constructor and `SetDefaultOptions` (`06000006`): full 1644-byte layout, field constants, convolution kernel, 16 sharpen passes, palette bytes, and PS3-specific mip filter. Caller-provided seed bytes preserve the original constructor's unwritten padding. DXT1A=1, DXT3=2, DXT5=3, R5G6B5=6, A8R8G8B8=7, X8R8G8B8=25; unknown/case-mismatched/null/default=0. Mipmap mode=30 when enabled, 32 when disabled; enabled PS3 filter=3, otherwise=0.

Managed storage guards explicitly reject null/short arrays and missing first-pixel format inference rather than duplicating native invalid-memory reads. These guards are adapter safety boundaries, not original DLL behavior claims. Garbage padding has no fixed canonical value: differential tests compare original/recovered using identical zero, A5, and pseudorandom seed storage.

`recovery/tests/TextureImageRulesOracle.cs` invokes original private helpers via exact reflected pointer signatures. Its temporary 24-byte nvImage and 4-byte gcroot respect original x86 layouts; it checks gcroot consumption. The original plugin's native device initialization is never called; `_Platform` is set directly for the platform-dependent pure options method. Cases cover every constant alpha byte, every alpha byte paired with 0/255, random pixels, empty/wrapped-zero images, threshold/power-of-two/overflow dimension pairs, eleven format inputs, mipmaps on/off, four platforms including unknown 99, and all 1644 options bytes under three padding seeds. Native compression, image loading, GPU initialization, full ProcessInstance, `.cdata`, and SDK integration remain outside this component test.

Build the x86 CLR4 oracle with `/unsafe /r:System.Xml.dll /r:<SDK tools>/BinaryAssetBuilder.Core.dll`, compile the recovered rules source alongside it, and provide `useLegacyV2RuntimeActivationPolicy=true` in the executable configuration. The original DLL and its unchanged support dependencies must be beside the host or in the original DLL directory. Running requires Windows x86 mixed-mode CLR support. No local runtime validation was available in the Linux implementation environment; use the exact Windows workflow receipt for accepted validation status.

