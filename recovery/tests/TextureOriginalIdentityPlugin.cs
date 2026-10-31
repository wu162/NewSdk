using System;
using System.IO;
using System.Reflection;
using BinaryAssetBuilder.Core;
using Recovered.Compiler.Texture;

// Test-only identity adapter. All texture decoding, compression, layout and
// ProcessInstance output still come directly from the pinned original DLL.
// Aligning the deliberate processing identity lets the original Core produce
// matching InstanceHash/checksum/CDATAnames for strict full-stream comparisons.
namespace NewSdk.TextureOracle
{
    public sealed class Plugin : IAssetBuilderPlugin, IDisposable
    {
        readonly IAssetBuilderPlugin original;
        TargetPlatform platform;

        public Plugin()
        {
            string directory = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
            Assembly assembly = Assembly.LoadFrom(Path.Combine(directory, "BinaryAssetBuilder.TextureCompiler.dll"));
            original = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true));
        }

        public void Initialize(object configObject, TargetPlatform target)
        {
            TextureCatalog.RequireWin32(target);
            platform = target;
            original.Initialize(configObject, target);
        }

        public void ReInitialize(object configObject, TargetPlatform target)
        {
            TextureCatalog.RequireWin32(target);
            platform = target;
            original.ReInitialize(configObject, target);
        }

        public AssetBuffer ProcessInstance(InstanceDeclaration instance)
        {
            return original.ProcessInstance(instance);
        }

        public uint GetAllTypesHash() { return original.GetAllTypesHash(); }
        public uint GetVersionNumber() { return TextureCatalog.Version(platform); }

        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            ExtendedTypeInformation information = original.GetExtendedTypeInformation(typeId);
            if (information == null) return null;
            ExtendedTypeInformation aligned = TextureCatalog.TypeInformation(typeId, platform);
            if (aligned.TypeName == null || information.TypeName != aligned.TypeName ||
                information.TypeId != aligned.TypeId || information.TypeHash != aligned.TypeHash ||
                information.HasCustomData != aligned.HasCustomData ||
                (information.ProcessingHash ^ 0x54580001u) != aligned.ProcessingHash)
                throw new InvalidDataException("Original texture type metadata does not match the source identity adapter.");
            return new ExtendedTypeInformation {
                TypeName = information.TypeName, TypeId = information.TypeId,
                TypeHash = information.TypeHash, ProcessingHash = aligned.ProcessingHash,
                HasCustomData = information.HasCustomData, Tokenized = information.Tokenized,
                UseBuildCache = information.UseBuildCache
            };
        }

        public void Dispose() { ((IDisposable)original).Dispose(); }
    }
}
