using System;
using BinaryAssetBuilder.Core;

namespace Recovered.Compiler.Texture
{
    public partial class Plugin : IAssetBuilderPlugin, IDisposable
    {
        private uint _HashCode;
        private TargetPlatform _Platform;

        // Recovered managed state only. Original Win32 Initialize also allocates
        // D3D and a shared 64-MiB native compression buffer. Those
        // native lifecycle side effects remain unrecovered and are not emulated.
        public virtual void Initialize(object configSection, TargetPlatform platform)
        {
            TextureCatalog.RequireWin32(platform);
            _HashCode = HashProvider.GetTypeHash(GetType());
            _Platform = platform;
        }

        // Unlike W3X, TextureCompiler.ReInitialize does NOT call Initialize;
        // preserve its direct state assignment and virtual-dispatch behavior.
        public virtual void ReInitialize(object configSection, TargetPlatform platform)
        {
            TextureCatalog.RequireWin32(platform);
            _HashCode = HashProvider.GetTypeHash(GetType());
            _Platform = platform;
        }

        public virtual uint GetAllTypesHash() { return TextureCatalog.AllTypesHash; }
        public virtual uint GetVersionNumber() { return TextureCatalog.Version(_Platform); }
        public virtual ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            return TextureCatalog.TypeInformation(typeId, _Platform);
        }

        // No native resources are allocated by this managed implementation.
        // This preserves managed dispatch/suppression only, not the original
        // module-global native resource cleanup or its unsafe ownership effects.
        protected virtual void Dispose(bool disposing) { }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        ~Plugin() { Dispose(false); }
    }
}

