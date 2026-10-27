#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.Threading;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

namespace NewModCompiler.Application
{
    public sealed class BuiltinCompilerRoute
    {
        public uint TypeId { get; private set; }
        public string TypeName { get; private set; }
        public uint TypeHash { get; private set; }
        public string Family { get; private set; }
        public bool SchemaGlobal { get; private set; }
        public BuiltinCompilerRoute(uint typeId, string typeName, uint typeHash, string family, bool schemaGlobal)
        {
            TypeId = typeId; TypeName = typeName; TypeHash = typeHash;
            Family = family; SchemaGlobal = schemaGlobal;
        }
    }

    // Owns one instance of each implementation, not one instance per asset type.
    // Core execution is deliberately separate: creating this set does not build a MOD.
    public sealed class BuiltinCompilers : IDisposable
    {
        private static readonly IList<BuiltinCompilerRoute> expected = MakeExpectedRoutes();
        private readonly List<IAssetBuilderPlugin> plugins = new List<IAssetBuilderPlugin>();
        private readonly List<PluginDescriptor> descriptors = new List<PluginDescriptor>();
        private readonly List<BuiltinCompilerRoute> routes = new List<BuiltinCompilerRoute>();
        private readonly Thread ownerThread = Thread.CurrentThread;
        private PluginRegistry registry;
        private bool disposed;
        public static IList<BuiltinCompilerRoute> ExpectedRoutes { get { return expected; } }
        public IList<BuiltinCompilerRoute> Routes { get { return routes.AsReadOnly(); } }
        public IList<IAssetBuilderPlugin> Plugins { get { return plugins.AsReadOnly(); } }
        public PluginRegistry Registry
        {
            get { ThrowIfDisposed(); return registry; }
        }
        private BuiltinCompilers() { }

        public static BuiltinCompilers Create(TargetPlatform platform)
        {
            return CreateCore(platform, false);
        }

        // The compatibility boundary is opt-in and requires its verified deployment.
        // Missing or invalid texture files are fatal; no SDK search path is used.
        public static BuiltinCompilers CreateWithTexture(TargetPlatform platform)
        {
            return CreateCore(platform, true);
        }

        private static BuiltinCompilers CreateCore(TargetPlatform platform, bool includeTexture)
        {
            if (platform != TargetPlatform.Win32)
                throw new NotSupportedException("The production compiler supports only Win32.");
            BuiltinCompilers result = new BuiltinCompilers();
            try
            {
                ValidateRoutes(expected);
                result.Add("gameobject", false, () => new Recovered.GameObject.Plugin());
                result.Add("weapon", false, () => new Recovered.Weapon.Plugin());
                result.Add("armor", false, () => new Recovered.Compiler.Plugin());
                result.Add("xml", false, () => new Recovered.XmlPriority.Plugin());
                result.Add("w3x", true, () => new Recovered.Compiler.W3x.NativePlugin());
                result.Add("audio", true, () => new BinaryAssetBuilder.AudioCompiler.Plugin());
                if (includeTexture)
                {
                    result.descriptors.Add(PluginDescriptor.CreateBuiltin(
                        "Texture,OnDemandTexture,TerrainTextureAtlas", true, () =>
                        {
                            var adapter = NewModCompiler.Compatibility.LegacyTextureAdapter.Load();
                            result.plugins.Add(adapter);
                            return adapter;
                        }));
                }
                result.registry = new PluginRegistry(result.descriptors.ToArray(), platform);
                result.registry.DefaultPlugin = new UnsupportedPlugin();
                foreach (BuiltinCompilerRoute route in expected)
                {
                    IAssetBuilderPlugin plugin = result.registry.GetPlugin(route.TypeId);
                    if (plugin.GetType().Assembly != typeof(BuiltinCompilers).Assembly)
                        throw new InvalidOperationException("Recovered implementation is outside the production EXE: " + route.TypeName);
                    CheckMetadata(route, plugin.GetExtendedTypeInformation(route.TypeId));
                    result.routes.Add(route);
                }
                if (includeTexture)
                {
                    foreach (string name in new[] { "Texture", "OnDemandTexture", "TerrainTextureAtlas" })
                    {
                        uint id = InstanceHandle.GetTypeId(name);
                        var info = result.registry.GetPlugin(id).GetExtendedTypeInformation(id);
                        if (info == null || info.TypeId != id || info.TypeName != name)
                            throw new InvalidOperationException("Missing texture route: " + name);
                        result.routes.Add(new BuiltinCompilerRoute(id, name, info.TypeHash, "texture", true));
                    }
                }
                return result;
            }
            catch (Exception failure)
            {
                try { result.Dispose(); }
                catch (Exception cleanup) { throw new AggregateException("Compiler initialization and cleanup failed.", failure, cleanup); }
                throw;
            }
        }

        private void Add(string family, bool useBuildCache, Func<IAssetBuilderPlugin> factory)
        {
            List<string> names = new List<string>();
            foreach (BuiltinCompilerRoute route in expected)
                if (route.Family == family) names.Add(route.TypeName);
            if (names.Count == 0) throw new InvalidOperationException("Missing built-in family: " + family);
            descriptors.Add(PluginDescriptor.CreateBuiltin(String.Join(",", names), useBuildCache, () =>
            {
                IAssetBuilderPlugin plugin = factory();
                if (plugin == null) throw new InvalidOperationException("Built-in factory returned null: " + family);
                plugins.Add(plugin);
                return plugin;
            }));
        }

        // Validate both the IDs and names before Core initializes any factories.
        // An omitted, duplicate, renamed or reclassified route is never silently accepted.
        public static void ValidateRoutes(IEnumerable<BuiltinCompilerRoute> candidate)
        {
            if (candidate == null) throw new ArgumentNullException("candidate");
            Dictionary<uint, BuiltinCompilerRoute> required = new Dictionary<uint, BuiltinCompilerRoute>();
            foreach (BuiltinCompilerRoute route in expected) required.Add(route.TypeId, route);
            HashSet<uint> ids = new HashSet<uint>();
            HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
            foreach (BuiltinCompilerRoute route in candidate)
            {
                if (route == null || String.IsNullOrEmpty(route.TypeName) || String.IsNullOrEmpty(route.Family))
                    throw new InvalidOperationException("Invalid built-in route.");
                if (!ids.Add(route.TypeId) || !names.Add(route.TypeName))
                    throw new InvalidOperationException("Duplicate built-in route: " + route.TypeName);
                BuiltinCompilerRoute original;
                if (!required.TryGetValue(route.TypeId, out original) ||
                    route.TypeName != original.TypeName || route.TypeHash != original.TypeHash ||
                    route.Family != original.Family || route.SchemaGlobal != original.SchemaGlobal ||
                    InstanceHandle.GetTypeId(route.TypeName) != route.TypeId)
                    throw new InvalidOperationException("Unexpected built-in route: " + route.TypeName);
            }
            if (ids.Count != required.Count)
                throw new InvalidOperationException("Missing built-in routes.");
        }

        private static void CheckMetadata(BuiltinCompilerRoute route, ExtendedTypeInformation info)
        {
            if (info == null || info.TypeId != route.TypeId ||
                info.TypeName != route.TypeName || info.TypeHash != route.TypeHash)
                throw new InvalidOperationException("Built-in metadata mismatch: " + route.TypeName);
        }

        public void ReInitialize(TargetPlatform platform)
        {
            ThrowIfDisposed();
            if (platform != TargetPlatform.Win32)
                throw new NotSupportedException("The production compiler supports only Win32.");
            registry.ReInitialize(descriptors.ToArray(), platform);
            foreach (BuiltinCompilerRoute route in routes)
                CheckMetadata(route, registry.GetPlugin(route.TypeId).GetExtendedTypeInformation(route.TypeId));
        }

        private void ThrowIfDisposed()
        {
            if (disposed) throw new ObjectDisposedException("BuiltinCompilers");
            CheckOwner();
        }

        private void CheckOwner()
        {
            if (!Object.ReferenceEquals(ownerThread, Thread.CurrentThread))
                throw new InvalidOperationException("The built-in compilers must be used by their owning thread.");
        }

        public void Dispose()
        {
            if (disposed) return;
            // Reject before committing the aggregate's disposed state: an
            // owner-thread plugin must remain closable by its actual owner.
            CheckOwner();
            disposed = true;
            List<Exception> failures = new List<Exception>();
            for (int i = plugins.Count - 1; i >= 0; --i)
            {
                IDisposable owned = plugins[i] as IDisposable;
                if (owned == null) continue;
                try { owned.Dispose(); }
                catch (Exception failure) { failures.Add(failure); }
            }
            if (failures.Count != 0) throw new AggregateException("Compiler cleanup failed.", failures);
        }

        private static IList<BuiltinCompilerRoute> MakeExpectedRoutes()
        {
            List<BuiltinCompilerRoute> entries = new List<BuiltinCompilerRoute>
            {
                new BuiltinCompilerRoute(2486173485u, "GameObject", 3415017856u, "gameobject", true),
                new BuiltinCompilerRoute(2496977262u, "WeaponTemplate", 216405646u, "weapon", true),
                new BuiltinCompilerRoute(980180622u, "ArmorTemplate", 1679479598u, "armor", true),
                new BuiltinCompilerRoute(608742960u, "W3DAnimation", 1430609896u, "w3x", true),
                new BuiltinCompilerRoute(3266421346u, "W3DMesh", 3448375452u, "w3x", true),
                new BuiltinCompilerRoute(376113229u, "AudioFile", 1405623879u, "audio", true),
                new BuiltinCompilerRoute(364756738u, "AudioFileMP3Passthrough", 2457571466u, "audio", true),
                new BuiltinCompilerRoute(2516466693u, "PathMusicMap", 3122351307u, "audio", true),
                new BuiltinCompilerRoute(2802565590u, "PathMusicTrack", 2668810035u, "audio", true),
                new BuiltinCompilerRoute(2590317961u, "PathMusicEvent", 1993395951u, "audio", true)
            };
            entries.AddRange(BuiltinXmlRoutes.Entries);
            return entries.AsReadOnly();
        }

        private sealed class UnsupportedPlugin : IAssetBuilderPlugin
        {
            public void Initialize(object configuration, TargetPlatform platform) { }
            public void ReInitialize(object configuration, TargetPlatform platform) { }
            public uint GetAllTypesHash() { return 0; }
            public uint GetVersionNumber() { return 0; }
            public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
            {
                throw new NotSupportedException("No built-in compiler for type ID " + typeId + ".");
            }
            public AssetBuffer ProcessInstance(InstanceDeclaration instance)
            {
                throw new NotSupportedException("No built-in compiler for the requested asset.");
            }
        }
    }
}
#endif
