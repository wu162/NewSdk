using System.IO;
using System.Xml;

namespace Mvp.Xml.Common.Xsl;

public class XmlOutput
{
	internal object destination;

	public XmlOutput(XmlWriter writer)
	{
		destination = writer;
	}

	public XmlOutput(TextWriter writer)
	{
		destination = writer;
	}

	public XmlOutput(Stream stream)
	{
		destination = stream;
	}

	public XmlOutput(string uri)
	{
		destination = uri;
	}
}
