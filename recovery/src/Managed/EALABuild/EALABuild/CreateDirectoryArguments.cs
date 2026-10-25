namespace EALABuild;

public class CreateDirectoryArguments
{
	protected string target;

	public string Path => target;

	public CreateDirectoryArguments(string targetDirectory)
	{
		target = targetDirectory;
	}
}
