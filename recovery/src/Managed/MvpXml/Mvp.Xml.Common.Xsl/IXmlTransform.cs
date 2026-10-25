using System.Xml;
using System.Xml.Xsl;

namespace Mvp.Xml.Common.Xsl;

public interface IXmlTransform
{
	XmlWriterSettings OutputSettings { get; }

	void Transform(XmlInput defaulDocument, XsltArgumentList args, XmlOutput output);
}
