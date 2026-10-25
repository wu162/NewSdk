namespace BinaryAssetBuilder.Remote;

public interface IServerCommand
{
	ServerState State { get; }

	void StartBuild(string[] args);

	void EndBuild();

	void RedirectConsoleOutput();
}
