namespace EALABuild;

public class WriteFileArguments
{
	protected string target;

	protected string content;

	public string Path => target;

	public string Content
	{
		get
		{
			return content;
		}
		set
		{
			content = value;
		}
	}

	public WriteFileArguments(string targetFile)
	{
		target = targetFile;
	}
}
