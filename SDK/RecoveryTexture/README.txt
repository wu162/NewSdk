Recovered TextureCompiler is active for Win32 Texture, OnDemandTexture and TerrainTextureAtlas.
Six runtime DLLs exactly match the package tested by the PPM SDK and complete mymod.
Core/Utility and all other compiler routes retain their existing binaries.
To restore the original configuration and prior runtime file states, run in PowerShell:
  ./SDK/RecoveryTexture/Restore.ps1
The receipt is relative to this directory and works after moving the checkout.
Restore verifies every installed file and backup before changing any file.
Default DXT1, fully opaque DXT1A blocks and uncertain extended BMP bitfields remain rejected.
