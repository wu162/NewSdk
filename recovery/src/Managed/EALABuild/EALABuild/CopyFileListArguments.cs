namespace EALABuild;

public class CopyFileListArguments : CopyFileArguments
{
	private string[] files;

	public string[] FileList
	{
		get
		{
			return files;
		}
		set
		{
			files = value;
		}
	}

	public CopyFileListArguments(string[] files, string targetFolder)
		: base(null, targetFolder)
	{
		FileList = files;
	}
}
