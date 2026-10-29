#if NEWSdk_MONOLITHIC
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BinaryAssetBuilder.Core;

namespace NewModCompiler.Compatibility
{
    // The only dynamic business boundary: a pinned, build-time-patched mixed DLL.
    // This wrapper never patches an input, probes search paths, or loads old Core.
    public sealed class LegacyTextureAdapter : IAssetBuilderPlugin, IDisposable
    {
        const string FileName = "BinaryAssetBuilder.TextureCompiler.dll";
        static readonly string[] Routes = { "Texture", "OnDemandTexture", "TerrainTextureAtlas" };
        readonly IAssetBuilderPlugin plugin;
        readonly IDisposable lifetime;
        bool initialized;
        bool disposed;

        LegacyTextureAdapter(IAssetBuilderPlugin plugin, IDisposable lifetime)
        {
            this.plugin = plugin;
            this.lifetime = lifetime;
        }

        public static LegacyTextureAdapter Load()
        {
            if (IntPtr.Size != 4) throw new PlatformNotSupportedException("Legacy texture requires Windows x86.");
            // The build prepares the mixed DLL beside the EXE. Loading it is the
            // runtime boundary; deployment manifests and PE audits are build evidence.
            string directory = Path.GetDirectoryName(typeof(IAssetBuilderPlugin).Assembly.Location);
            Assembly assembly = Assembly.LoadFrom(Path.Combine(directory, FileName));
            Type type = assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            object instance = Activator.CreateInstance(type);
            return new LegacyTextureAdapter((IAssetBuilderPlugin)instance, (IDisposable)instance);
        }

        void Alive()
        {
            if (disposed) throw new ObjectDisposedException(nameof(LegacyTextureAdapter));
        }
        static void Platform(TargetPlatform platform)
        {
            if (platform != TargetPlatform.Win32) throw new PlatformNotSupportedException("Only Win32 texture builds are supported.");
        }
        public void Initialize(object configObject, TargetPlatform platform)
        {
            Alive(); Platform(platform);
            if (initialized) throw new InvalidOperationException("Texture plugin is already initialized; use ReInitialize.");
            plugin.Initialize(configObject, platform);
            initialized = true;
        }
        public void ReInitialize(object configObject, TargetPlatform platform)
        {
            Alive(); Platform(platform);
            if (!initialized) throw new InvalidOperationException("Texture plugin must first be initialized.");
            plugin.ReInitialize(configObject, platform);
        }
        public AssetBuffer ProcessInstance(InstanceDeclaration instance)
        {
            Alive();
            if (!initialized) throw new InvalidOperationException("Texture plugin must first be initialized.");
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!Routes.Any(name => InstanceHandle.GetTypeId(name) == instance.Handle.TypeId))
                throw new NotSupportedException("Unsupported texture type: " + instance.Handle.TypeName);
            // Deliberately no catch: preserve original failures and reuse-after-error semantics.
            return plugin.ProcessInstance(instance);
        }
        public uint GetAllTypesHash() { Alive(); return plugin.GetAllTypesHash(); }
        public uint GetVersionNumber() { Alive(); return plugin.GetVersionNumber(); }
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            Alive();
            string name = Routes.FirstOrDefault(route => InstanceHandle.GetTypeId(route) == typeId);
            if (name == null) return null;
            ExtendedTypeInformation information = plugin.GetExtendedTypeInformation(typeId);
            if (information == null || information.TypeName != name || information.TypeId != typeId)
                throw new InvalidDataException("Texture metadata route mismatch: " + name);
            return information;
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            lifetime.Dispose();
        }
    }
}
#endif
