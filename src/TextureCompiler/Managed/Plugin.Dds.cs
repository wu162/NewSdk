using System;
using System.IO;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

namespace Recovered.Compiler.Texture
{
    public partial class Plugin
    {
        // Original ProcessInstance switches on Handle.TypeId, never XML element name.
        public virtual AssetBuffer ProcessInstance(InstanceDeclaration declaration)
        {
            switch (declaration.Handle.TypeId)
            {
                case TextureCatalog.TextureTypeId: return CompileDds(declaration, false);
                case TextureCatalog.OnDemandTextureTypeId: return CompileDds(declaration, true);
                case TextureCatalog.TerrainTextureAtlasTypeId:
                    throw new NotSupportedException("Terrain texture atlas packing and compression have not been recovered.");
                default:
                    Console.WriteLine("Warning: Couldn't process {0}. No matching handler found.", declaration);
                    return null;
            }
        }

        private static void Write32(byte[] data, int offset, uint value, bool big)
        {
            for (int i = 0; i < 4; i++) data[offset + i] = (byte)(value >> (8 * (big ? 3 - i : i)));
        }

        private static AssetBuffer EmbedDds(byte[] bytes, bool rootTexture, TargetPlatform platform)
        {
            int root = rootTexture ? 12 : 8;
            int pointer = rootTexture ? 4 : 0;
            // Tracker::Allocate rounds EACH block to four bytes and zero-fills padding.
            byte[] data = new byte[checked(root + ((bytes.Length + 3) & ~3))];
            bool big = platform != TargetPlatform.Win32;
            Write32(data, pointer, (uint)root, big);
            // Original ProcessTexture swaps the byte count only for PS3. Unknown
            // platforms use big-endian relocations but keep this count little-endian.
            Write32(data, pointer + 4, (uint)bytes.Length, platform == TargetPlatform.PlayStation3);
            Buffer.BlockCopy(bytes, 0, data, root, bytes.Length);
            byte[] relocations = new byte[8];
            Write32(relocations, 0, (uint)pointer, big);
            Write32(relocations, 4, uint.MaxValue, big);
            return new AssetBuffer { InstanceData = data, RelocationData = relocations, ImportsData = new byte[0] };
        }

        private AssetBuffer CompileDds(InstanceDeclaration declaration, bool onDemand)
        {
            string file = declaration.XmlNode.Attributes["File"].Value;
            if (!File.Exists(file))
                throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile,
                    "The texture file referenced from the XML does not exist on disk!", new object[0]);
            // XSD validation/default expansion and FileReference preprocessing happen
            // in Core before this entry, just as they do for the original plugin.
            string outputFormat = declaration.Node.Attributes["OutputFormat"].Value;
            bool mipmaps = bool.Parse(declaration.Node.Attributes["GenerateMipMaps"].Value);
            string textureType = declaration.Node.Attributes["Type"].Value;
            bool resize = bool.Parse(declaration.Node.Attributes["AllowAutomaticResize"].Value);
            if (_Platform == TargetPlatform.Xbox360)
                throw new NotSupportedException("Xenon DDS bundling has not been recovered.");
            // Preserve the case-sensitive original suffix test. Core normally lowercases paths.
            if (!file.EndsWith(".dds"))
                throw new NotSupportedException("Image decoding and nvDDS texture compression have not been recovered.");
            byte[] bytes = File.ReadAllBytes(file);
            if (bytes.Length == 0)
                throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile,
                    "Texture file has length 0", new object[0]);
            AssetBuffer result = EmbedDds(bytes, !onDemand, _Platform);
            if (!onDemand) return result;

            bool big = _Platform != TargetPlatform.Win32;
            // OnDemandTexture root has only a zero runtime pointer and an enum.
            // The original writes the enum directly without an endian conversion.
            uint type = textureType == "CubeTexture" ? 0u : textureType == "VolumeTexture" ? 1u : 2u;
            byte[] outer = new byte[8];
            Write32(outer, 4, type, false);
            byte[] header = new byte[32];
            Write32(header, 0, declaration.Handle.TypeId, big);
            Write32(header, 4, declaration.Handle.InstanceId, big);
            Write32(header, 8, declaration.Handle.TypeHash, big);
            Write32(header, 12, declaration.Handle.InstanceHash, big);
            Write32(header, 16, (uint)result.InstanceData.Length, big);
            Write32(header, 20, (uint)result.RelocationData.Length, big);
            Write32(header, 24, (uint)result.ImportsData.Length, big);
            // File.OpenWrite intentionally retains any previous trailing bytes.
            using (FileStream stream = File.OpenWrite(declaration.CustomDataPath))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(result.InstanceData, 0, result.InstanceData.Length);
                stream.Write(result.RelocationData, 0, result.RelocationData.Length);
            }
            return new AssetBuffer { InstanceData = outer, RelocationData = new byte[0], ImportsData = new byte[0] };
        }
    }
}
