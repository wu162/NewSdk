using System;
namespace BinaryAssetBuilder.Core;
public interface IAssetExecution
{
    bool HasPending { get; }
    void Enqueue(BinaryAsset asset, InstanceDeclaration declaration, IAssetBuilderPlugin plugin,
        Action compileCompatibility, Action verify, Action<AssetLocation> committed);
    void EnqueueCached(Action commit);
    void Flush();
    void DeferCompletion(Action complete);
}
