using BinaryAssetBuilder.Core;

namespace Recovered.W3x
{
    // Original Plugin methods 0x06000148-0x0600014A; original DLL SHA-256:
    // bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1.
    // Animation's processing seed is deliberately different from its TypeHash.
    public static class W3xCatalog
    {
        public const uint MeshTypeId = 3266421346u;
        public const uint AnimationTypeId = 608742960u;
        public const uint AllTypesHash = 1424942948u;

        public static uint Version(TargetPlatform platform)
        {
            switch (platform)
            {
                case TargetPlatform.Win32: return 8u;
                case TargetPlatform.Xbox360: return 9u;
                case TargetPlatform.PlayStation3: return 11u;
                default: return 0u;
            }
        }

        public static ExtendedTypeInformation TypeInformation(uint typeId, uint version)
        {
            var result = new ExtendedTypeInformation();
            result.TypeId = typeId;
            result.HasCustomData = false;
            switch (typeId)
            {
                case MeshTypeId:
                    result.TypeName = "W3DMesh";
                    result.TypeHash = 3448375452u;
                    result.ProcessingHash = version ^ 0xCD8A089Cu;
                    break;
                case AnimationTypeId:
                    result.TypeName = "W3DAnimation";
                    result.TypeHash = 1430609896u;
                    result.ProcessingHash = version ^ 0xD6505CE1u;
                    break;
            }
            return result;
        }
    }
}
