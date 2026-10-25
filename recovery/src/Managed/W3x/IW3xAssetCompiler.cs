using BinaryAssetBuilder.Core;

namespace Recovered.W3x
{
    // Recovered seam, not an original ABI. A future C++/CLI implementation can
    // supply the native handlers without duplicating the managed plugin logic.
    public interface IW3xAssetCompiler
    {
        AssetBuffer CompileAnimation(InstanceDeclaration declaration, TargetPlatform platform);
        AssetBuffer CompileMesh(InstanceDeclaration declaration, TargetPlatform platform);
    }
}
