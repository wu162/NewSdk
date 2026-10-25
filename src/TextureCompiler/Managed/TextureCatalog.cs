using BinaryAssetBuilder.Core;

namespace Recovered.Compiler.Texture
{
    // Source: original Plugin metadata methods; SHA256
    // 4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e.
    public static class TextureCatalog
    {
        public const uint TextureTypeId = 568797146u;
        public const uint OnDemandTextureTypeId = 2467477932u;
        public const uint TerrainTextureAtlasTypeId = 1601459053u;
        public const uint AllTypesHash = 1424942948u;

        // Original GetVersionNumber omits PS3 despite its declared version
        // fields. Keep that observable omission and the unknown-platform zero.
        public static uint Version(TargetPlatform platform)
        {
            switch (platform)
            {
                case TargetPlatform.Win32: return 4202u;
                case TargetPlatform.Xbox360: return 4605u;
                default: return 0u;
            }
        }

        // Metadata uses a separate per-type/platform table, not the virtual
        // GetVersionNumber. Unknown platforms use the PC metadata constants.
        public static ExtendedTypeInformation TypeInformation(uint typeId, TargetPlatform platform)
        {
            ExtendedTypeInformation result = new ExtendedTypeInformation();
            result.TypeId = typeId;
            result.HasCustomData = false;
            switch (typeId)
            {
                case TextureTypeId:
                    result.TypeName = "Texture";
                    result.TypeHash = 2673866970u;
                    result.ProcessingHash = platform == TargetPlatform.Xbox360 ? 2673862951u :
                        platform == TargetPlatform.PlayStation3 ? 2673863356u : 2673862832u;
                    break;
                case OnDemandTextureTypeId:
                    result.TypeName = "OnDemandTexture";
                    result.TypeHash = 4165504734u;
                    result.ProcessingHash = platform == TargetPlatform.Xbox360 ? 4165508899u :
                        platform == TargetPlatform.PlayStation3 ? 4165508280u : 4165508788u;
                    result.HasCustomData = true;
                    break;
                case TerrainTextureAtlasTypeId:
                    result.TypeName = "TerrainTextureAtlas";
                    result.TypeHash = 417928170u;
                    result.ProcessingHash = platform == TargetPlatform.Xbox360 ? 1182597248u :
                        platform == TargetPlatform.PlayStation3 ? 1182597327u : 1182597302u;
                    break;
            }
            return result;
        }
    }
}
