namespace EALABuild;

public class RunExecutableArguments
{
	private string fileName;

	private string arguments = "";

	private bool redirectOutput = true;

	private bool createWindow;

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public string Arguments
	{
		get
		{
			return arguments;
		}
		set
		{
			arguments = value;
		}
	}

	public bool RedirectOutput
	{
		get
		{
			return redirectOutput;
		}
		set
		{
			redirectOutput = value;
		}
	}

	public bool CreateWindow
	{
		get
		{
			return createWindow;
		}
		set
		{
			createWindow = value;
		}
	}

	public RunExecutableArguments(string executable)
	{
		fileName = executable;
	}
}
