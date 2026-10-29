using System;
namespace BinaryAssetBuilder.Core;
// All callbacks execute on the semantic owner. The commit callback performs
// BinaryAsset.Commit at the supplied known location, inside its document context.
public interface IAssetExecution
{
    bool HasPending { get; }
    void Enqueue(BinaryAsset asset, InstanceDeclaration declaration, IAssetBuilderPlugin plugin,
        Action compileCompatibility, Action verify, Action<AssetLocation> commit);
    AssetBuffer ExecuteCompatibility(IAssetBuilderPlugin plugin, InstanceDeclaration declaration);
    void EnqueueCached(BinaryAsset asset, Action commit);
    void FlushAsset(BinaryAsset asset);
    void Flush();
    void DeferCompletion(Action complete);
}
