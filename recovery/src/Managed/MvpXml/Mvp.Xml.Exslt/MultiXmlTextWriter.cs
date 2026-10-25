using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Exslt;

public class MultiXmlTextWriter : XmlTextWriter
{
	protected const string RedirectNamespace = "http://exslt.org/common";

	protected const string RedirectElementName = "document";

	private Stack states;

	private OutputState state;

	private string currentAttributeName;

	private RedirectState redirectState;

	public MultiXmlTextWriter(string fileName, Encoding encoding)
		: base(fileName, encoding)
	{
		DirectoryInfo parent = Directory.GetParent(fileName);
		Directory.SetCurrentDirectory(parent.ToString());
	}

	public MultiXmlTextWriter(TextWriter w)
		: base(w)
	{
	}

	public MultiXmlTextWriter(Stream w, Encoding encoding)
		: base(w, encoding)
	{
	}

	private void CheckContentStart()
	{
		if (redirectState != RedirectState.WritingRedirectElementAttrs)
		{
			return;
		}
		if (state.Href == null)
		{
			throw new ArgumentNullException("'href' attribute of exsl:document element must be specified.");
		}
		foreach (OutputState state in states)
		{
			if (state.Href == this.state.Href)
			{
				throw new ArgumentException("Cannot write to " + this.state.Href + " two documents simultaneously.");
			}
		}
		this.state.InitWriter();
		redirectState = RedirectState.Redirecting;
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
		CheckContentStart();
		if (ns == "http://exslt.org/common" && localName == "document")
		{
			if (states == null)
			{
				states = new Stack();
			}
			if (redirectState == RedirectState.Redirecting)
			{
				states.Push(state);
			}
			state = new OutputState();
			redirectState = RedirectState.WritingRedirectElementAttrs;
		}
		else if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method == OutputMethod.Text)
			{
				state.Depth++;
				return;
			}
			if (state.Depth == 0 && state.SystemDoctype != null)
			{
				if (prefix != string.Empty)
				{
					state.XmlWriter.WriteDocType(prefix + ":" + localName, state.PublicDoctype, state.SystemDoctype, null);
				}
				else
				{
					state.XmlWriter.WriteDocType(localName, state.PublicDoctype, state.SystemDoctype, null);
				}
			}
			state.XmlWriter.WriteStartElement(prefix, localName, ns);
			state.Depth++;
		}
		else
		{
			base.WriteStartElement(prefix, localName, ns);
		}
	}

	internal void FinishRedirecting()
	{
		state.CloseWriter();
		if (states.Count != 0)
		{
			state = (OutputState)states.Pop();
			redirectState = RedirectState.Redirecting;
		}
		else
		{
			state = null;
			redirectState = RedirectState.Relaying;
		}
	}

	public override void WriteEndElement()
	{
		CheckContentStart();
		if (redirectState == RedirectState.Redirecting)
		{
			if (state.Depth-- == 0)
			{
				FinishRedirecting();
			}
			else if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteEndElement();
			}
		}
		else
		{
			base.WriteEndElement();
		}
	}

	public override void WriteFullEndElement()
	{
		CheckContentStart();
		if (redirectState == RedirectState.Redirecting)
		{
			if (state.Depth-- == 0)
			{
				FinishRedirecting();
			}
			else if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteFullEndElement();
			}
		}
		else
		{
			base.WriteFullEndElement();
		}
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
		if (redirectState == RedirectState.WritingRedirectElementAttrs)
		{
			redirectState = RedirectState.WritingRedirectElementAttrValue;
			currentAttributeName = localName;
		}
		else if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteStartAttribute(prefix, localName, ns);
			}
		}
		else
		{
			base.WriteStartAttribute(prefix, localName, ns);
		}
	}

	public override void WriteEndAttribute()
	{
		if (redirectState == RedirectState.WritingRedirectElementAttrValue)
		{
			redirectState = RedirectState.WritingRedirectElementAttrs;
		}
		else if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteEndAttribute();
			}
		}
		else
		{
			base.WriteEndAttribute();
		}
	}

	public override void WriteComment(string text)
	{
		CheckContentStart();
		if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteComment(text);
			}
		}
		else
		{
			base.WriteComment(text);
		}
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
		CheckContentStart();
		if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method != OutputMethod.Text)
			{
				state.XmlWriter.WriteProcessingInstruction(name, text);
			}
		}
		else
		{
			base.WriteProcessingInstruction(name, text);
		}
	}

	public override void WriteString(string text)
	{
		if (redirectState == RedirectState.WritingRedirectElementAttrValue)
		{
			switch (currentAttributeName)
			{
			case "href":
				state.Href = text;
				break;
			case "method":
				if (text == "text")
				{
					state.Method = OutputMethod.Text;
				}
				break;
			case "encoding":
				try
				{
					state.Encoding = Encoding.GetEncoding(text);
					break;
				}
				catch (Exception)
				{
					break;
				}
			case "indent":
				if (text == "yes")
				{
					state.Indent = true;
				}
				break;
			case "doctype-public":
				state.PublicDoctype = text;
				break;
			case "doctype-system":
				state.SystemDoctype = text;
				break;
			case "standalone":
				if (text == "yes")
				{
					state.Standalone = true;
				}
				break;
			case "omit-xml-declaration":
				if (text == "yes")
				{
					state.OmitXmlDeclaration = true;
				}
				break;
			}
			return;
		}
		CheckContentStart();
		if (redirectState == RedirectState.Redirecting)
		{
			if (state.Method == OutputMethod.Text)
			{
				state.TextWriter.Write(text);
			}
			else
			{
				state.XmlWriter.WriteString(text);
			}
		}
		else
		{
			base.WriteString(text);
		}
	}
}
