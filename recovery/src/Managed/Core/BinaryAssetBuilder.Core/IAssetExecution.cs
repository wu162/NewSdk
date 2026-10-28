using System;

namespace BinaryAssetBuilder.Core;

// The semantic owner prepares declarations and verifies/commits in discovery
// order. Implementations may execute only explicitly certified private inputs.
public interface IAssetExecution
{
	void Enqueue(BinaryAsset asset, InstanceDeclaration declaration, IAssetBuilderPlugin plugin,
		Action compileCompatibility, Action verify, Action<AssetLocation> committed);
	void Flush();
	void DeferCompletion(Action complete);
}
