using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility;

public class ReferencedManifest
{
	private bool m_IsPatch;

	private string m_Path;

	public string Path => m_Path;

	public bool IsPatch
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return m_IsPatch;
		}
	}

	public ReferencedManifest(string path, [MarshalAs(UnmanagedType.U1)] bool isPatch)
	{
		m_Path = path;
		m_IsPatch = isPatch;
	}
}
