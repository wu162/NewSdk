#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using BinaryAssetBuilder.Core;

namespace NewModCompiler.Compatibility
{
    // The only dynamic business boundary: a pinned, build-time-patched mixed DLL.
    // This wrapper never patches an input, probes search paths, or loads old Core.
    public sealed class LegacyTextureAdapter : IAssetBuilderPlugin, IDisposable
    {
        const string FileName = "BinaryAssetBuilder.TextureCompiler.dll";
        const string OriginalHash = "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e";
        static readonly string[] Routes = { "Texture", "OnDemandTexture", "TerrainTextureAtlas" };
        static readonly string[] Forbidden = {
            "BinaryAssetBuilder.Core", "BinaryAssetBuilder.Utility", "BinaryAssetBuilder.Tokenizer",
            "BinaryAssetBuilder.XmlCompiler", "BinaryAssetBuilder.W3XCompiler",
            "BinaryAssetBuilder.AudioCompiler", "EALA.Hash", "EALA.Metrics"
        };
        readonly IAssetBuilderPlugin plugin;
        readonly IDisposable lifetime;
        bool initialized;
        bool disposed;

        LegacyTextureAdapter(IAssetBuilderPlugin plugin, IDisposable lifetime)
        {
            this.plugin = plugin;
            this.lifetime = lifetime;
        }

        static string Hash(string path)
        {
            using (var input = File.OpenRead(path))
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
        }
        static string Text(IDictionary<string, object> values, string key)
        {
            object value;
            if (!values.TryGetValue(key, out value) || !(value is string))
                throw new InvalidDataException("Texture patch manifest lacks " + key);
            return (string)value;
        }
        static void Equal(string actual, string expected, string description)
        {
            if (!String.Equals(actual, expected, StringComparison.Ordinal))
                throw new InvalidDataException("Texture deployment mismatch: " + description);
        }
        static void CheckNoOldAssemblies(string directory)
        {
            foreach (string name in Forbidden)
                if (File.Exists(Path.Combine(directory, name + ".dll")))
                    throw new InvalidDataException("Forbidden business DLL in compiler directory: " + name);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (Forbidden.Contains(assembly.GetName().Name, StringComparer.OrdinalIgnoreCase) ||
                    assembly.GetName().Name.StartsWith("Recovered.", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Forbidden business assembly already loaded: " + assembly.FullName);
        }

        public static LegacyTextureAdapter Load()
        {
            if (IntPtr.Size != 4) throw new PlatformNotSupportedException("Legacy texture requires Windows x86.");
            Assembly owner = typeof(IAssetBuilderPlugin).Assembly;
            Assembly entry = Assembly.GetEntryAssembly();
            if (entry == null || owner != entry || owner.GetModules().Length != 1)
                throw new InvalidOperationException("Texture Core contracts must belong to the single-module entry EXE.");
            string directory = Path.GetDirectoryName(entry.Location);
            string path = Path.Combine(directory, FileName);
            CheckNoOldAssemblies(directory);
            var manifest = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(path + ".patch.json"));
            if (manifest == null) throw new InvalidDataException("Empty texture patch manifest.");
            Equal(Text(manifest, "mode"), "redirect", "patch mode");
            Equal(Text(manifest, "originalSha256"), OriginalHash, "original input");
            Equal(Text(manifest, "assemblyRefToken"), "0x23000005", "reference token");
            Equal(Text(manifest, "targetIdentity"), owner.FullName, "EXE identity");
            Equal(Text(manifest, "targetExeSha256"), Hash(entry.Location), "EXE bytes");
            Equal(Text(manifest, "patchedSha256"), Hash(path), "texture bytes");
            object auditValue;
            var audit = manifest.TryGetValue("protectedPeAudit", out auditValue) ? auditValue as Dictionary<string, object> : null;
            if (audit == null) throw new InvalidDataException("Texture deployment has no protected PE audit.");
            Equal(Text(audit, "status"), "passed", "PE audit status");
            Equal(Text(audit, "mode"), "redirect", "PE audit mode");
            Equal(Text(audit, "originalSha256"), OriginalHash, "PE audit original");
            Equal(Text(audit, "outputSha256"), Hash(path), "PE audit output");
            // A previously loaded same-identity DLL can bypass LoadFrom's path.
            Assembly assembly = Assembly.LoadFrom(path);
            Equal(Path.GetFullPath(assembly.Location).ToUpperInvariant(), Path.GetFullPath(path).ToUpperInvariant(), "loaded texture location");
            var references = assembly.GetReferencedAssemblies();
            if (references.Count(a => a.FullName == owner.FullName) != 1 ||
                references.Any(a => a.Name == "BinaryAssetBuilder.Core"))
                throw new InvalidDataException("Texture does not reference this EXE exclusively for Core types.");
            // Other pinned legacy AssemblyRefs are inert in the supported PC route.
            // They are not resolved to replacement stubs or copied business DLLs.
            Type type = assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            if (!typeof(IAssetBuilderPlugin).IsAssignableFrom(type) || !typeof(IDisposable).IsAssignableFrom(type))
                throw new InvalidDataException("Texture plugin has incompatible contracts.");
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
