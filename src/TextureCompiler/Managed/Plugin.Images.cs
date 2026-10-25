using System;
using System.IO;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

namespace Recovered.Compiler.Texture
{
    public partial class Plugin
    {
        private static readonly Tracer ImageTracer = Tracer.GetTracer("TextureCompiler", "Provides Texture Processing");
        private AssetBuffer CompileTexture(InstanceDeclaration declaration, bool onDemand)
        {
            string file = declaration.XmlNode.Attributes["File"].Value;
            // Original preserves DDS bytes without validating or recompressing them.
            if (file.EndsWith(".dds")) return CompileDds(declaration, onDemand);
            if (!File.Exists(file))
                throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile,
                    "The texture file referenced from the XML does not exist on disk!", new object[0]);
            string format = null;
            switch (declaration.Node.Attributes["OutputFormat"].Value)
            {
                case "A8R8G8B8": format = "D3DFMT_A8R8G8B8"; break;
                case "X8R8G8B8": format = "D3DFMT_X8R8G8B8"; break;
                case "R5G6B5": format = "D3DFMT_R5G6B5"; break;
                case "DXT1": format = "D3DFMT_DXT1"; break;
                case "DXT1A": format = "D3DFMT_DXT1A"; break;
                case "DXT3": format = "D3DFMT_DXT3"; break;
                case "DXT5": format = "D3DFMT_DXT5"; break;
            }
            bool mipmaps = bool.Parse(declaration.Node.Attributes["GenerateMipMaps"].Value);
            string textureType = declaration.Node.Attributes["Type"].Value;
            bool resize = bool.Parse(declaration.Node.Attributes["AllowAutomaticResize"].Value);
            if (_Platform == TargetPlatform.Xbox360)
                throw new NotSupportedException("Xenon image bundling has not been recovered.");
            if (!file.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Only the declared TGA image decoder subset has been recovered.");

            TexturePacker.Image image;
            try { image = TextureImageLoader.Load(file); }
            catch (InvalidDataException)
            {
                throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile,
                    "D3DX cannot load the texture from disk.", new object[0]);
            }
            if ((image.Width & (image.Width - 1)) != 0 || (image.Height & (image.Height - 1)) != 0)
                throw new NotSupportedException("Original D3DX power-of-two image resampling has not been recovered.");
            AssetBuffer result = new AssetBuffer { InstanceData = new byte[onDemand ? 8 : 12],
                RelocationData = new byte[0], ImportsData = new byte[0] };
            bool valid = TextureImageRules.IsValidTexture((uint)image.Width, (uint)image.Height,
                declaration.Handle.InstanceName, delegate(string message) { ImageTracer.TraceWarning(message); });
            if (valid)
            {
                byte[] rgba = ImageBytes(image);
                format = TextureImageRules.GuessProperFormat((uint)image.Width, (uint)image.Height,
                    rgba, image.HasAlpha, format, declaration.Handle.InstanceName,
                    delegate(string message) { ImageTracer.TraceInfo(message); });
                switch (textureType)
                {
                    case "CubeTexture":
                        throw new BinaryAssetBuilderException(null, ErrorCode.InvalidArgument,
                            "CubeTexture is not currently supported.  Create the cube texture as a DDS with the D3D Texture tool in the meantime.", new object[0]);
                    case "VolumeTexture":
                        byte[] volumeDds = mipmaps
                            ? TextureVolumeMipEncoder.EncodeStrip((uint)image.Width, (uint)image.Height,
                                rgba, format, _Platform == TargetPlatform.PlayStation3)
                            : TextureVolumeRawDdsEncoder.EncodeStrip((uint)image.Width,
                                (uint)image.Height, rgba, format, false);
                        result = EmbedDds(volumeDds, !onDemand, _Platform);
                        break;
                    case "StandardTexture":
                        // PS3's automatic resizing modifies dimensions and filtering
                        // for >256 textures before encoding. Do not silently omit it.
                        if (_Platform == TargetPlatform.PlayStation3 && resize && (image.Width > 256 || image.Height > 256))
                        {
                            uint targetWidth = (uint)image.Width, targetHeight = (uint)image.Height;
                            while (targetWidth > 256 || targetHeight > 256)
                            {
                                targetWidth >>= 1; targetHeight >>= 1;
                            }
                            if (targetWidth == 0 || targetHeight == 0)
                                throw new NotSupportedException("PS3 automatic resize producing a zero axis has not been accepted.");
                            byte[] resizedDds = TextureCubicResizeEncoder.Encode((uint)image.Width,
                                (uint)image.Height, targetWidth, targetHeight, rgba, format, mipmaps);
                            result = EmbedDds(resizedDds, !onDemand, _Platform);
                            break;
                        }
                        if (mipmaps && _Platform != TargetPlatform.Win32 && _Platform != TargetPlatform.PlayStation3)
                            throw new NotSupportedException("Raw mip encoding is accepted only for Win32 and PS3 option profiles.");
                        byte[] dds = mipmaps
                            ? (_Platform == TargetPlatform.PlayStation3
                                ? TexturePs3MipEncoder.Encode((uint)image.Width, (uint)image.Height, rgba, format)
                                : TextureTriangleMipEncoder.Encode((uint)image.Width, (uint)image.Height, rgba, format))
                            : TextureRawDdsEncoder.Encode((uint)image.Width, (uint)image.Height, rgba, format, false);
                        result = EmbedDds(dds, !onDemand, _Platform);
                        break;
                    // Original unknown enum text leaves TextureFileData empty.
                }
            }
            return onDemand ? FinishOnDemand(declaration, textureType, result) : result;
        }
        private static byte[] ImageBytes(TexturePacker.Image image)
        {
            byte[] bytes = new byte[checked(image.Pixels.Length * 4)];
            for (int i = 0; i < image.Pixels.Length; i++)
            {
                uint pixel = image.Pixels[i]; int at = i * 4;
                bytes[at] = (byte)pixel; bytes[at + 1] = (byte)(pixel >> 8);
                bytes[at + 2] = (byte)(pixel >> 16); bytes[at + 3] = (byte)(pixel >> 24);
            }
            return bytes;
        }
    }
}

