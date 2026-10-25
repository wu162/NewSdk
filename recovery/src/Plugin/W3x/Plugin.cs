using System;
using BinaryAssetBuilder.Core;
using Recovered.W3x;

namespace Recovered.Compiler.W3x
{
    // The managed plugin shell is recovered; native animation/mesh handlers are
    // still pending. Do not route the SDK here until those handlers are accepted.
    public class Plugin : IAssetBuilderPlugin, IDisposable
    {
        private uint m_HashCode;
        private TargetPlatform m_Platform = TargetPlatform.Win32;
        private readonly IW3xAssetCompiler compiler;

        public Plugin() : this(null) { }
        public Plugin(IW3xAssetCompiler compiler) { this.compiler = compiler; }

        // Original 0x06000142. The per-module hash naturally changes for a new
        // assembly/type identity; preserve the algorithm rather than hardcode it.
        public virtual void Initialize(object configSection, TargetPlatform platform)
        {
            m_HashCode = HashProvider.GetTypeHash(GetType());
            m_Platform = platform;
        }

        // Original 0x06000143 uses virtual dispatch, including subclass overrides.
        public virtual void ReInitialize(object configSection, TargetPlatform platform)
        {
            Initialize(configSection, platform);
        }

        public virtual uint GetAllTypesHash() { return W3xCatalog.AllTypesHash; }
        public virtual uint GetVersionNumber() { return W3xCatalog.Version(m_Platform); }
        public virtual ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            // Unknown IDs do not call the virtual version getter in the original.
            uint version = typeId == W3xCatalog.MeshTypeId || typeId == W3xCatalog.AnimationTypeId
                ? GetVersionNumber() : 0u;
            return W3xCatalog.TypeInformation(typeId, version);
        }

        // Original 0x06000147: use Handle.TypeId, not the XML tag or type name.
        public virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
        {
            switch (declaration.Handle.TypeId)
            {
                case W3xCatalog.MeshTypeId: return HandleMesh(declaration);
                case W3xCatalog.AnimationTypeId: return HandleAnimation(declaration);
                default:
                    Console.WriteLine("Warning: Couldn't process {0}. No matching handler found.", declaration);
                    return null;
            }
        }

        // These adapters are not reconstructions of original 0x06000145/146.
        // Their business algorithms will be provided by the recovered backend.
        public AssetBuffer HandleAnimation(InstanceDeclaration declaration)
        {
            if (compiler == null) throw new NotSupportedException("W3DAnimation compilation has not been recovered.");
            return compiler.CompileAnimation(declaration, m_Platform);
        }
        public AssetBuffer HandleMesh(InstanceDeclaration declaration)
        {
            if (compiler == null) throw new NotSupportedException("W3DMesh compilation has not been recovered.");
            return compiler.CompileMesh(declaration, m_Platform);
        }

        // Original cleanup bodies are empty. There is no disposed-state gate:
        // Dispose does not invalidate metadata methods or subsequent Initialize.
        protected virtual void Dispose(bool disposing) { }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        ~Plugin() { Dispose(false); }
    }
}
