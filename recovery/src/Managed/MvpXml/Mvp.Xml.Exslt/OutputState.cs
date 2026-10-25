using System.IO;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Exslt;

internal class OutputState
{
	private XmlTextWriter xmlWriter;

	private StreamWriter textWriter;

	private int depth;

	private string href;

	private Encoding encoding;

	private bool indent;

	private string publicDoctype;

	private string systemDoctype;

	private bool standalone;

	private string storedDir;

	private OutputMethod method;

	private bool omitXmlDecl;

	public bool Standalone
	{
		get
		{
			return standalone;
		}
		set
		{
			standalone = value;
		}
	}

	public OutputMethod Method
	{
		get
		{
			return method;
		}
		set
		{
			method = value;
		}
	}

	public string Href
	{
		get
		{
			return href;
		}
		set
		{
			href = value;
		}
	}

	public Encoding Encoding
	{
		get
		{
			return encoding;
		}
		set
		{
			encoding = value;
		}
	}

	public bool Indent
	{
		get
		{
			return indent;
		}
		set
		{
			indent = value;
		}
	}

	public string PublicDoctype
	{
		get
		{
			return publicDoctype;
		}
		set
		{
			publicDoctype = value;
		}
	}

	public string SystemDoctype
	{
		get
		{
			return systemDoctype;
		}
		set
		{
			systemDoctype = value;
		}
	}

	public XmlTextWriter XmlWriter => xmlWriter;

	public StreamWriter TextWriter => textWriter;

	public int Depth
	{
		get
		{
			return depth;
		}
		set
		{
			depth = value;
		}
	}

	public bool OmitXmlDeclaration
	{
		get
		{
			return omitXmlDecl;
		}
		set
		{
			omitXmlDecl = value;
		}
	}

	public OutputState()
	{
		encoding = Encoding.UTF8;
		indent = false;
		standalone = false;
		omitXmlDecl = false;
		method = OutputMethod.Xml;
	}

	public void InitWriter()
	{
		storedDir = Directory.GetCurrentDirectory();
		DirectoryInfo parent = Directory.GetParent(href);
		if (!parent.Exists)
		{
			parent.Create();
		}
		if (method == OutputMethod.Xml)
		{
			xmlWriter = new XmlTextWriter(href, encoding);
			if (indent)
			{
				xmlWriter.Formatting = Formatting.Indented;
			}
			if (!omitXmlDecl)
			{
				if (standalone)
				{
					xmlWriter.WriteStartDocument(standalone: true);
				}
				else
				{
					xmlWriter.WriteStartDocument();
				}
			}
		}
		else
		{
			textWriter = new StreamWriter(href, append: false, encoding);
		}
		Directory.SetCurrentDirectory(parent.ToString());
	}

	public void CloseWriter()
	{
		if (method == OutputMethod.Xml)
		{
			if (!omitXmlDecl)
			{
				xmlWriter.WriteEndDocument();
			}
			xmlWriter.Close();
		}
		else
		{
			textWriter.Close();
		}
		Directory.SetCurrentDirectory(storedDir);
	}
}
