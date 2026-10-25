namespace EALABuild;

public class DeleteFileArguments
{
	protected string target;

	protected bool raiseErrors;

	protected bool recursive;

	protected string filter;

	public string Path => target;

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

	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			filter = value;
		}
	}

	public DeleteFileArguments(string path)
	{
		target = path;
	}
}
