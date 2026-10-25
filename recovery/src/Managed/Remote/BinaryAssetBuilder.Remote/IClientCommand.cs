using System.IO;

namespace BinaryAssetBuilder.Remote;

public interface IClientCommand
{
	TextWriter OutputHandle { get; }

	void NotifyBuildFinished(int code);
}
