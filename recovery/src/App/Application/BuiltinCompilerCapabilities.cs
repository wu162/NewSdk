#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NewModCompiler.Application
{
    // An immutable policy declaration, not a measured plugin resource bound,
    // an exact input closure or a replacement for legacy processing/type hashes.
    public sealed class BuiltinCompilerCapabilities
    {
        public uint TypeId { get; }
        public string TypeName { get; }
        public string Family { get; }
        public string ThreadPolicy { get; }
        public string InputTracking { get; }
        public string ResultContractVersion { get; }
        // Eligibility retains the existing whole-root legacy validation gates.
        // It does not enable a cache or override a route's UseBuildCache=false.
        public bool LegacyBuildCacheAllowed { get; }
        public bool LegacySessionCacheAllowed { get; }
        public bool StrongBundleAllowed { get; }
        public bool ParallelAllowed { get; }
        public string MemoryClass { get; }
        // The known minimum host only. Additional native/plugin requirements
        // have not been inventoried; this is never an environment closure key.
        public string RequiredEnvironment { get; }
        public bool RequiredEnvironmentComplete { get; }
        public string PhysicalInputBypassReason { get; }

        private BuiltinCompilerCapabilities(uint typeId, string typeName, string family, bool known, bool texture)
        {
            TypeId = typeId; TypeName = known ? typeName : null; Family = known ? family : "unknown";
            ThreadPolicy = "OwnerThreadSerial";
            InputTracking = known && !texture ? "ConservativeCoreBoundary" : "Unknown";
            ResultContractVersion = !known ? "unknown" : texture ? "legacy-mixed-texture-v1" : "legacy-core-whole-root-v1";
            LegacyBuildCacheAllowed = known && !texture;
            LegacySessionCacheAllowed = known && !texture;
            StrongBundleAllowed = false; ParallelAllowed = false;
            MemoryClass = "Unknown";
            RequiredEnvironment = known ? "Win32/.NETFramework4.8" : "Unknown";
            RequiredEnvironmentComplete = false;
            PhysicalInputBypassReason = texture ? "opaque-texture-closure" : known ? null : "unknown-compiler-closure";
        }
        internal static BuiltinCompilerCapabilities ForRoute(BuiltinCompilerRoute route)
        {
            switch (route.Family)
            {
                case "gameobject": case "weapon": case "armor": case "xml": case "w3x": case "audio":
                    return new BuiltinCompilerCapabilities(route.TypeId, route.TypeName, route.Family, true, false);
                case "texture":
                    return new BuiltinCompilerCapabilities(route.TypeId, route.TypeName, route.Family, true, true);
                default:
                    throw new InvalidOperationException("No built-in compiler capability policy for family: " + route.Family);
            }
        }
        internal static BuiltinCompilerCapabilities Unknown(uint typeId)
        {
            return new BuiltinCompilerCapabilities(typeId, null, "unknown", false, false);
        }
    }

    // Owns an independent dictionary of registered scalar policies. Neither
    // mutable ExtendedTypeInformation nor plugin/native objects enter this map.
    public sealed class BuiltinCompilerCapabilityCatalog
    {
        private readonly ReadOnlyDictionary<uint, BuiltinCompilerCapabilities> entries;
        public IReadOnlyDictionary<uint, BuiltinCompilerCapabilities> Entries { get { return entries; } }
        internal BuiltinCompilerCapabilityCatalog(IEnumerable<BuiltinCompilerRoute> routes)
        {
            var values = new Dictionary<uint, BuiltinCompilerCapabilities>();
            foreach (BuiltinCompilerRoute route in routes)
            {
                if (route == null || String.IsNullOrEmpty(route.TypeName) || values.ContainsKey(route.TypeId))
                    throw new InvalidOperationException("Invalid or duplicate compiler capability route.");
                values.Add(route.TypeId, BuiltinCompilerCapabilities.ForRoute(route));
            }
            entries = new ReadOnlyDictionary<uint, BuiltinCompilerCapabilities>(values);
        }
        public BuiltinCompilerCapabilities GetCapabilities(uint typeId)
        {
            BuiltinCompilerCapabilities result;
            // Unknown requests do not grow a long-lived memoization table.
            return entries.TryGetValue(typeId, out result) ? result : BuiltinCompilerCapabilities.Unknown(typeId);
        }
    }
}
#endif
