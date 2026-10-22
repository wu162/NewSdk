using System;
using System.Windows.Forms;

namespace EALAModStudio;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		try
		{
			Application.Run(new ModStudioUI(args));
		}
		catch (Exception ex)
		{
			MessageBox.Show("Unhandled exception caught: " + ex.Message);
		}
	}
}
