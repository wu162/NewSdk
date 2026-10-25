namespace EALABuild;

public class CopyFileArguments
{
	protected string target;

	protected string destination;

	protected bool raiseErrors;

	protected bool recursive;

	protected string pattern = string.Empty;

	protected bool overwrite = true;

	public string TargetFiles
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public string DestinationPath
	{
		get
		{
			return destination;
		}
		set
		{
			destination = value;
		}
	}

	public bool RaiseErrors
	{
		get
		{
			return raiseErrors;
		}
		set
		{
			raiseErrors = value;
		}
	}

	public bool Recursive
	{
		get
		{
			return recursive;
		}
		set
		{
			recursive = value;
		}
	}

	public bool Overwrite
	{
		get
		{
			return overwrite;
		}
		set
		{
			overwrite = value;
		}
	}

	public string Filter
	{
		get
		{
			return pattern;
		}
		set
		{
			pattern = value;
		}
	}

	public CopyFileArguments(string targetFiles, string destinationPath)
	{
		target = targetFiles;
		destination = destinationPath;
	}
}
