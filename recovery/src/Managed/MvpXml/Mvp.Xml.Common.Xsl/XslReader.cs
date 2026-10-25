using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Common.Xsl;

public class XslReader : XmlReader
{
	private class QName
	{
		private string local;

		private string nsUri;

		private string prefix;

		public string Local => local;

		public string NsUri => nsUri;

		public string Prefix => prefix;

		public QName(string local, string nsUri, string prefix)
		{
			this.local = local;
			this.nsUri = nsUri;
			this.prefix = prefix;
		}

		public override string ToString()
		{
			if (Prefix == null || Prefix.Length == 0)
			{
				return Local;
			}
			return Prefix + ':' + Local;
		}
	}

	[DebuggerDisplay("{NodeType}: name={Name}, Value={Value}")]
	private struct XmlToken
	{
		public XmlNodeType NodeType;

		public QName Name;

		public string Value;

		public static void Set(ref XmlToken evnt, XmlNodeType nodeType, QName name, string value)
		{
			evnt.NodeType = nodeType;
			evnt.Name = name;
			evnt.Value = value;
		}

		public static void Get(ref XmlToken evnt, out XmlNodeType nodeType, out QName name, out string value)
		{
			nodeType = evnt.NodeType;
			name = evnt.Name;
			value = evnt.Value;
		}
	}

	private class BufferWriter : XmlWriter
	{
		private class QNameTable
		{
			private XmlNameTable nameTable;

			private Dictionary<string, List<QName>> qnames = new Dictionary<string, List<QName>>();

			public QNameTable(XmlNameTable nameTable)
			{
				this.nameTable = nameTable;
			}

			public QName GetQName(string local, string nsUri, string prefix)
			{
				nsUri = nameTable.Add(nsUri);
				prefix = nameTable.Add(prefix);
				if (!qnames.TryGetValue(local, out var value))
				{
					value = new List<QName>();
					qnames.Add(local, value);
				}
				else
				{
					foreach (QName item in value)
					{
						if (RefEquals(item.Prefix, prefix) && RefEquals(item.NsUri, nsUri))
						{
							return item;
						}
					}
				}
				QName qName = new QName(nameTable.Add(local), nsUri, prefix);
				value.Add(qName);
				return qName;
			}

			private static string Atomize(string s, Dictionary<string, string> dic)
			{
				if (dic.TryGetValue(s, out var value))
				{
					return value;
				}
				dic.Add(s, s);
				return s;
			}
		}

		private QNameTable qnameTable;

		private TokenPipe pipe;

		private string firstText;

		private StringBuilder sbuilder;

		private QName curAttribute;

		public QName QNameXmlSpace;

		public QName QNameXmlLang;

		public QName QNameEmpty;

		public QName QNameXmlNs;

		public override WriteState WriteState
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override string XmlLang
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public BufferWriter(TokenPipe pipe, XmlNameTable nameTable)
		{
			this.pipe = pipe;
			qnameTable = new QNameTable(nameTable);
			sbuilder = new StringBuilder();
			QNameXmlSpace = qnameTable.GetQName("space", NsXml, "xml");
			QNameXmlLang = qnameTable.GetQName("lang", NsXml, "xml");
			QNameXmlNs = qnameTable.GetQName("xmlns", NsXmlNs, "");
			QNameEmpty = qnameTable.GetQName("", "", "");
		}

		public void Reset()
		{
			firstText = null;
			sbuilder.Length = 0;
		}

		private void AppendText(string text)
		{
			if (firstText == null)
			{
				firstText = text;
			}
			else if (sbuilder.Length == 0)
			{
				sbuilder.Append(firstText);
			}
			sbuilder.Append(text);
		}

		private string MergeText()
		{
			if (firstText == null)
			{
				return string.Empty;
			}
			if (sbuilder.Length != 0)
			{
				firstText = sbuilder.ToString();
				sbuilder.Length = 0;
			}
			string result = firstText;
			firstText = null;
			return result;
		}

		private void FinishTextNode()
		{
			string text = MergeText();
			if (text.Length != 0)
			{
				pipe.Write(XmlNodeType.Text, QNameEmpty, text);
			}
		}

		public override void WriteComment(string text)
		{
			FinishTextNode();
			pipe.Write(XmlNodeType.Comment, QNameEmpty, text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			FinishTextNode();
			pipe.Write(XmlNodeType.ProcessingInstruction, qnameTable.GetQName(name, string.Empty, string.Empty), text);
		}

		public override void WriteStartElement(string prefix, string name, string ns)
		{
			FinishTextNode();
			pipe.Write(XmlNodeType.Element, qnameTable.GetQName(name, ns, prefix), "");
		}

		public override void WriteEndElement()
		{
			FinishTextNode();
			pipe.Write(XmlNodeType.EndElement, QNameEmpty, "");
		}

		public override void WriteStartAttribute(string prefix, string name, string ns)
		{
			curAttribute = qnameTable.GetQName(name, ns, prefix);
		}

		public override void WriteEndAttribute()
		{
			pipe.Write(XmlNodeType.Attribute, curAttribute, MergeText());
		}

		public override void WriteString(string text)
		{
			AppendText(text);
		}

		public override void WriteFullEndElement()
		{
			WriteEndElement();
		}

		public override void WriteRaw(string data)
		{
			WriteString(data);
		}

		public override void Close()
		{
			FinishTextNode();
			pipe.Close();
		}

		public override void Flush()
		{
		}

		public override void WriteStartDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteStartDocument(bool standalone)
		{
			throw new NotSupportedException();
		}

		public override void WriteEndDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new NotSupportedException();
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotSupportedException();
		}

		public override void WriteCharEntity(char ch)
		{
			throw new NotSupportedException();
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			throw new NotSupportedException();
		}

		public override void WriteWhitespace(string ws)
		{
			throw new NotSupportedException();
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteCData(string text)
		{
			throw new NotSupportedException();
		}

		public override string LookupPrefix(string ns)
		{
			throw new NotSupportedException();
		}
	}

	private class TokenPipe
	{
		protected XmlToken[] buffer;

		protected int writePos;

		protected int readStartPos;

		protected int readEndPos;

		protected int mask;

		public TokenPipe(int bufferSize)
		{
			if (bufferSize < 2)
			{
				bufferSize = defaultBufferSize;
			}
			bufferSize--;
			mask = bufferSize;
			while ((bufferSize >>= 1) != 0)
			{
				mask |= bufferSize;
			}
			buffer = new XmlToken[mask + 1];
		}

		public virtual void Reset()
		{
			readStartPos = (readEndPos = (writePos = 0));
		}

		public virtual void Write(XmlNodeType nodeType, QName name, string value)
		{
			if (writePos == buffer.Length)
			{
				XmlToken[] array = new XmlToken[buffer.Length * 2];
				buffer.CopyTo(array, 0);
				buffer = array;
			}
			XmlToken.Set(ref buffer[writePos], nodeType, name, value);
			writePos++;
		}

		public virtual void WriteException(Exception e)
		{
			throw e;
		}

		public virtual void Read(out XmlNodeType nodeType, out QName name, out string value)
		{
			XmlToken.Get(ref buffer[readEndPos], out nodeType, out name, out value);
			readEndPos++;
		}

		public virtual void FreeTokens(int num)
		{
			readStartPos += num;
			readEndPos = readStartPos;
		}

		public virtual void Close()
		{
			Write(XmlNodeType.None, null, null);
		}

		public virtual void GetToken(int attNum, out QName name, out string value)
		{
			XmlToken.Get(ref buffer[readStartPos + attNum], out var _, out name, out value);
		}
	}

	private class ScopeManager
	{
		private static string atomLang = new string("lang".ToCharArray());

		private static string atomSpace = new string("space".ToCharArray());

		private XmlNameTable nameTable;

		private string stringEmpty;

		private QName[] records = new QName[32];

		private int lastRecord;

		private XmlSpace currentSpace;

		private string currentLang;

		public string Lang => currentLang;

		public XmlSpace Space => currentSpace;

		public QName Name => records[lastRecord - 1];

		public ScopeManager(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
			stringEmpty = nameTable.Add(string.Empty);
			currentLang = stringEmpty;
			currentSpace = XmlSpace.None;
			Reset();
		}

		public void Reset()
		{
			lastRecord = 0;
			records[lastRecord++] = new QName(null, nameTable.Add(NsXml), nameTable.Add("xml"));
			records[lastRecord++] = new QName(null, stringEmpty, stringEmpty);
			records[lastRecord++] = new QName(stringEmpty, stringEmpty, stringEmpty);
		}

		public void PushScope(QName qname)
		{
			AddRecord(qname);
		}

		public void PopScope()
		{
			while (true)
			{
				lastRecord--;
				QName qName = records[lastRecord - 1];
				if (qName.Local != null)
				{
					break;
				}
				if (RefEquals(qName.Prefix, atomLang))
				{
					currentLang = qName.NsUri;
				}
				else if (RefEquals(qName.Prefix, atomSpace))
				{
					currentSpace = Str2Space(qName.NsUri);
				}
			}
		}

		private void AddRecord(QName qname)
		{
			if (lastRecord == records.Length)
			{
				QName[] array = new QName[records.Length * 2];
				records.CopyTo(array, 0);
				records = array;
			}
			records[lastRecord++] = qname;
		}

		public void AddNamespace(string prefix, string uri)
		{
			uri = nameTable.Add(uri);
			AddRecord(new QName(null, uri, prefix));
		}

		public void AddLang(string lang)
		{
			lang = nameTable.Add(lang);
			if (!RefEquals(lang, currentLang))
			{
				AddRecord(new QName(null, currentLang, atomLang));
				currentLang = lang;
			}
		}

		public void AddSpace(string space)
		{
			XmlSpace xmlSpace = Str2Space(space);
			if (xmlSpace == XmlSpace.None)
			{
				throw new Exception("Unexpected value for xml:space attribute");
			}
			if (xmlSpace != currentSpace)
			{
				AddRecord(new QName(null, Space2Str(currentSpace), atomSpace));
				currentSpace = xmlSpace;
			}
		}

		private string Space2Str(XmlSpace space)
		{
			return space switch
			{
				XmlSpace.Preserve => "preserve", 
				XmlSpace.Default => "default", 
				_ => "none", 
			};
		}

		private XmlSpace Str2Space(string space)
		{
			return space switch
			{
				"preserve" => XmlSpace.Preserve, 
				"default" => XmlSpace.Default, 
				_ => XmlSpace.None, 
			};
		}

		public string LookupNamespace(string prefix)
		{
			prefix = nameTable.Get(prefix);
			int num = lastRecord - 2;
			while (0 <= num)
			{
				QName qName = records[num];
				if (qName.Local == null && RefEquals(qName.Prefix, prefix))
				{
					return qName.NsUri;
				}
				num--;
			}
			return null;
		}
	}

	private class TokenPipeMultiThread : TokenPipe
	{
		private Exception exception;

		public TokenPipeMultiThread(int bufferSize)
			: base(bufferSize)
		{
		}

		public override void Reset()
		{
			base.Reset();
			exception = null;
		}

		private void ExpandBuffer()
		{
			int num = (mask << 1) | 1;
			XmlToken[] array = new XmlToken[num + 1];
			for (int i = readStartPos; i < writePos; i++)
			{
				ref XmlToken reference = ref array[i & num];
				reference = buffer[i & mask];
			}
			buffer = array;
			mask = num;
		}

		public override void Write(XmlNodeType nodeType, QName name, string value)
		{
			lock (this)
			{
				if (writePos == readStartPos + buffer.Length)
				{
					if (writePos == readEndPos)
					{
						ExpandBuffer();
					}
					else
					{
						Monitor.Wait(this);
					}
				}
				XmlToken.Set(ref buffer[writePos & mask], nodeType, name, value);
				writePos++;
				if (readStartPos + buffer.Length <= writePos)
				{
					Monitor.Pulse(this);
				}
			}
		}

		public override void WriteException(Exception e)
		{
			lock (this)
			{
				exception = e;
				Monitor.Pulse(this);
			}
		}

		public override void Read(out XmlNodeType nodeType, out QName name, out string value)
		{
			lock (this)
			{
				if (readEndPos == writePos)
				{
					if (readEndPos == readStartPos + buffer.Length)
					{
						ExpandBuffer();
						Monitor.Pulse(this);
					}
					Monitor.Wait(this);
				}
				if (exception != null)
				{
					throw new XsltException("Exception happened during transformation. See inner exception for details:\n", exception);
				}
			}
			XmlToken.Get(ref buffer[readEndPos & mask], out nodeType, out name, out value);
			readEndPos++;
		}

		public override void FreeTokens(int num)
		{
			lock (this)
			{
				readStartPos += num;
				readEndPos = readStartPos;
				Monitor.Pulse(this);
			}
		}

		public override void Close()
		{
			Write(XmlNodeType.None, null, null);
			lock (this)
			{
				Monitor.Pulse(this);
			}
		}

		public override void GetToken(int attNum, out QName name, out string value)
		{
			XmlToken.Get(ref buffer[(readStartPos + attNum) & mask], out var _, out name, out value);
		}
	}

	private static string NsXml;

	private static string NsXmlNs;

	private static int defaultBufferSize;

	private XmlNameTable nameTable;

	private TokenPipe pipe;

	private BufferWriter writer;

	private ScopeManager scope;

	private Thread thread;

	private XslCompiledTransform xslCompiledTransform;

	private bool multiThread;

	private int initialBufferSize;

	private static XmlReaderSettings ReaderSettings;

	private XmlInput defaulDocument;

	private XsltArgumentList args;

	private int attOffset;

	private int attCount;

	private int depth;

	private XmlNodeType nodeType;

	private ReadState readState;

	private QName qname;

	private string value;

	private static char[] qnameSeparator;

	public XslCompiledTransform XslCompiledTransform
	{
		get
		{
			return xslCompiledTransform;
		}
		set
		{
			xslCompiledTransform = value;
		}
	}

	public int InitialBufferSize
	{
		get
		{
			return initialBufferSize;
		}
		set
		{
			initialBufferSize = value;
		}
	}

	public override int AttributeCount => attCount;

	public override string BaseURI => string.Empty;

	public override XmlNameTable NameTable => nameTable;

	public override int Depth => depth;

	public override bool EOF => ReadState == ReadState.EndOfFile;

	public override bool HasValue => 0 != (0x2659C & (1 << (int)nodeType));

	public override XmlNodeType NodeType => nodeType;

	public override bool IsEmptyElement => false;

	public override string LocalName => qname.Local;

	public override string NamespaceURI => qname.NsUri;

	public override string Prefix => qname.Prefix;

	public override string Value => value;

	public override ReadState ReadState => readState;

	public override string XmlLang => scope.Lang;

	public override XmlSpace XmlSpace => scope.Space;

	static XslReader()
	{
		NsXml = "http://www.w3.org/XML/1998/namespace";
		NsXmlNs = "http://www.w3.org/2000/xmlns/";
		defaultBufferSize = 256;
		qnameSeparator = new char[1] { ':' };
		ReaderSettings = new XmlReaderSettings();
		ReaderSettings.ProhibitDtd = true;
	}

	public XslReader(XslCompiledTransform xslTransform, bool multiThread, int initialBufferSize)
	{
		xslCompiledTransform = xslTransform;
		this.multiThread = multiThread;
		this.initialBufferSize = initialBufferSize;
		nameTable = new NameTable();
		pipe = (this.multiThread ? new TokenPipeMultiThread(initialBufferSize) : new TokenPipe(initialBufferSize));
		writer = new BufferWriter(pipe, nameTable);
		scope = new ScopeManager(nameTable);
		SetUndefinedState(ReadState.Initial);
	}

	public XslReader(XslCompiledTransform xslTransform)
		: this(xslTransform, multiThread: true, defaultBufferSize)
	{
	}

	public XmlReader StartTransform(XmlInput input, XsltArgumentList args)
	{
		defaulDocument = input;
		this.args = args;
		Start();
		return this;
	}

	private void Start()
	{
		if (thread != null && thread.IsAlive)
		{
			thread.Abort();
			thread.Join();
		}
		writer.Reset();
		scope.Reset();
		pipe.Reset();
		depth = 0;
		SetUndefinedState(ReadState.Initial);
		if (multiThread)
		{
			thread = new Thread(StartTransform);
			thread.Start();
		}
		else
		{
			StartTransform();
		}
	}

	private void StartTransform()
	{
		try
		{
			if (defaulDocument.source is XmlReader input)
			{
				xslCompiledTransform.Transform(input, args, writer, defaulDocument.resolver);
			}
			else if (defaulDocument.source is IXPathNavigable input2)
			{
				xslCompiledTransform.Transform(input2, args, writer);
			}
			else if (defaulDocument.source is string inputUri)
			{
				using XmlReader input3 = XmlReader.Create(inputUri, ReaderSettings);
				xslCompiledTransform.Transform(input3, args, writer, defaulDocument.resolver);
			}
			else if (defaulDocument.source is Stream input4)
			{
				using XmlReader input5 = XmlReader.Create(input4, ReaderSettings);
				xslCompiledTransform.Transform(input5, args, writer, defaulDocument.resolver);
			}
			else
			{
				if (!(defaulDocument.source is TextReader input6))
				{
					throw new Exception("Unexpected XmlInput");
				}
				using XmlReader input7 = XmlReader.Create(input6, ReaderSettings);
				xslCompiledTransform.Transform(input7, args, writer, defaulDocument.resolver);
			}
			writer.Close();
		}
		catch (Exception e)
		{
			if (multiThread)
			{
				pipe.WriteException(e);
				return;
			}
			throw;
		}
	}

	private void SetUndefinedState(ReadState readState)
	{
		qname = writer.QNameEmpty;
		value = string.Empty;
		nodeType = XmlNodeType.None;
		attCount = 0;
		this.readState = readState;
	}

	private bool IsWhitespace(string s)
	{
		foreach (char c in s)
		{
			if (' ' < c)
			{
				return false;
			}
		}
		return true;
	}

	public override bool Read()
	{
		switch (nodeType)
		{
		case XmlNodeType.None:
			if (readState == ReadState.EndOfFile || readState == ReadState.Closed)
			{
				return false;
			}
			readState = ReadState.Interactive;
			break;
		case XmlNodeType.Attribute:
			attOffset = 0;
			depth--;
			goto case XmlNodeType.Element;
		case XmlNodeType.Element:
			pipe.FreeTokens(1 + attCount);
			depth++;
			break;
		case XmlNodeType.EndElement:
			scope.PopScope();
			pipe.FreeTokens(1);
			break;
		case XmlNodeType.Text:
			if (attOffset != 0)
			{
				attOffset = 0;
				depth -= 2;
				goto case XmlNodeType.Element;
			}
			pipe.FreeTokens(1);
			break;
		case XmlNodeType.ProcessingInstruction:
		case XmlNodeType.Comment:
		case XmlNodeType.Whitespace:
		case XmlNodeType.SignificantWhitespace:
			pipe.FreeTokens(1);
			break;
		default:
			throw new InvalidProgramException("Internal Error: unexpected node type");
		}
		attCount = 0;
		pipe.Read(out nodeType, out qname, out value);
		if (nodeType == XmlNodeType.None)
		{
			SetUndefinedState(ReadState.EndOfFile);
			return false;
		}
		switch (nodeType)
		{
		case XmlNodeType.Element:
			attCount = 0;
			while (true)
			{
				pipe.Read(out var xmlNodeType, out var name, out var text);
				if (xmlNodeType != XmlNodeType.Attribute)
				{
					break;
				}
				if (RefEquals(name.Prefix, "xmlns"))
				{
					scope.AddNamespace(name.Local, text);
				}
				else if (RefEquals(name, writer.QNameXmlNs))
				{
					scope.AddNamespace(name.Prefix, text);
				}
				else if (RefEquals(name, writer.QNameXmlLang))
				{
					scope.AddLang(text);
				}
				else if (RefEquals(name, writer.QNameXmlSpace))
				{
					scope.AddSpace(text);
				}
				attCount++;
			}
			scope.PushScope(qname);
			break;
		case XmlNodeType.EndElement:
			qname = scope.Name;
			depth--;
			break;
		case XmlNodeType.Text:
			if (IsWhitespace(value))
			{
				nodeType = ((XmlSpace == XmlSpace.Preserve) ? XmlNodeType.SignificantWhitespace : XmlNodeType.Whitespace);
			}
			break;
		default:
			throw new InvalidProgramException("Internal Error: unexpected node type");
		case XmlNodeType.ProcessingInstruction:
		case XmlNodeType.Comment:
			break;
		}
		return true;
	}

	public override void Close()
	{
		SetUndefinedState(ReadState.Closed);
	}

	public override string GetAttribute(int i)
	{
		if (IsInsideElement() && 0 <= i && i < attCount)
		{
			pipe.GetToken(i + 1, out var _, out var _);
			return value;
		}
		throw new ArgumentOutOfRangeException("i");
	}

	private int FindAttribute(string name)
	{
		if (IsInsideElement())
		{
			string[] array = name.Split(qnameSeparator, StringSplitOptions.None);
			string text;
			string text2;
			switch (array.Length)
			{
			case 1:
				text = string.Empty;
				text2 = name;
				break;
			case 2:
				if (array[0].Length == 0)
				{
					return 0;
				}
				text = array[0];
				text2 = array[1];
				break;
			default:
				return 0;
			}
			for (int i = 1; i <= attCount; i++)
			{
				pipe.GetToken(i, out var name2, out var _);
				if (name2.Local == text2 && name2.Prefix == text)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public override string GetAttribute(string name)
	{
		int num = FindAttribute(name);
		if (num != 0)
		{
			return GetAttribute(num - 1);
		}
		return null;
	}

	public override string GetAttribute(string name, string ns)
	{
		if (IsInsideElement())
		{
			for (int i = 1; i <= attCount; i++)
			{
				pipe.GetToken(i, out var name2, out var result);
				if (name2.Local == name && name2.NsUri == ns)
				{
					return result;
				}
			}
		}
		return null;
	}

	public override string LookupNamespace(string prefix)
	{
		return scope.LookupNamespace(prefix);
	}

	public override bool MoveToAttribute(string name)
	{
		int num = FindAttribute(name);
		if (num != 0)
		{
			MoveToAttribute(num - 1);
			return true;
		}
		return false;
	}

	public override void MoveToAttribute(int i)
	{
		if (IsInsideElement() && 0 <= i && i < attCount)
		{
			ChangeDepthToElement();
			attOffset = i + 1;
			depth++;
			pipe.GetToken(attOffset, out qname, out value);
			nodeType = XmlNodeType.Attribute;
		}
		throw new ArgumentOutOfRangeException("i");
	}

	public override bool MoveToAttribute(string name, string ns)
	{
		if (IsInsideElement())
		{
			for (int i = 1; i <= attCount; i++)
			{
				pipe.GetToken(i, out var name2, out var text);
				if (name2.Local == name && name2.NsUri == ns)
				{
					ChangeDepthToElement();
					nodeType = XmlNodeType.Attribute;
					attOffset = i;
					qname = name2;
					depth++;
					value = text;
				}
			}
		}
		return false;
	}

	private bool IsInsideElement()
	{
		if (nodeType != XmlNodeType.Element && nodeType != XmlNodeType.Attribute)
		{
			if (nodeType == XmlNodeType.Text)
			{
				return attOffset != 0;
			}
			return false;
		}
		return true;
	}

	private void ChangeDepthToElement()
	{
		switch (nodeType)
		{
		case XmlNodeType.Attribute:
			depth--;
			break;
		case XmlNodeType.Text:
			if (attOffset != 0)
			{
				depth -= 2;
			}
			break;
		}
	}

	public override bool MoveToElement()
	{
		if (nodeType == XmlNodeType.Attribute || (nodeType == XmlNodeType.Text && attOffset != 0))
		{
			ChangeDepthToElement();
			nodeType = XmlNodeType.Element;
			attOffset = 0;
			pipe.GetToken(0, out qname, out value);
			return true;
		}
		return false;
	}

	public override bool MoveToFirstAttribute()
	{
		ChangeDepthToElement();
		attOffset = 0;
		return MoveToNextAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		if (attOffset < attCount)
		{
			ChangeDepthToElement();
			depth++;
			attOffset++;
			pipe.GetToken(attOffset, out qname, out value);
			nodeType = XmlNodeType.Attribute;
			return true;
		}
		return false;
	}

	public override bool ReadAttributeValue()
	{
		if (nodeType == XmlNodeType.Attribute)
		{
			nodeType = XmlNodeType.Text;
			depth++;
			return true;
		}
		return false;
	}

	public override void ResolveEntity()
	{
		throw new InvalidOperationException();
	}

	private static bool RefEquals(string strA, string strB)
	{
		return (object)strA == strB;
	}

	private static bool RefEquals(QName qnA, QName qnB)
	{
		return qnA == qnB;
	}
}
