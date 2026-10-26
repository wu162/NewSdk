using System;
using System.IO;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.AudioCompiler;

internal class AutoCleanUpTempFiles(string baseTempFileName) : IDisposable
{
	private string m_baseTempFileName = baseTempFileName;

	private void _007EAutoCleanUpTempFiles()
	{
		Tracer tracer = Tracer.GetTracer("AudioCompiler", "Provides Audio processing functionality");
		string directoryName = Path.GetDirectoryName(baseTempFileName);
		string fileName = Path.GetFileName(baseTempFileName);
		fileName += "*";
		string[] files = Directory.GetFiles(directoryName, fileName);
		for (int i = 0; i < (nint)files.LongLength; i++)
		{
			string text = files[i];
			tracer.TraceNote("Cleaning up stale temporary file {0}", text);
			try
			{
				File.Delete(text);
			}
			catch (SystemException ex)
			{
				tracer.TraceWarning("Could not delete stale temporary file {0}: {1}", text, ex);
			}
		}
	}

	protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
	{
		if (P_0)
		{
			_007EAutoCleanUpTempFiles();
		}
		else
		{
			Finalize();
		}
	}

	public virtual sealed void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
