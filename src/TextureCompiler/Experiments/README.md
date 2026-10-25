# Texture compiler experiments

These files are not included by Managed/Recovered.TextureCompiler.csproj.

TextureCubicResizeEncoder is an unaccepted candidate. Only the dedicated
TestTextureCubicResize.ps1 compiles it into Experimental.Texture.Cubic.dll.
Do not route production Plugin.Images.cs to it until the complete differential
and actual plugin / SDK integration pass. Current numeric mismatch and exact
probe anchors: recovery/TEXTURE_CUBIC_RESIZE.md (repository root).
