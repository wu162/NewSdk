using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace BinaryAssetBuilder;

public class SerializedSectionHandler : IConfigurationSectionHandler
{
	public object Create(object parent, object configContext, XmlNode section)
	{
		XmlAttribute xmlAttribute = section.Attributes["serializedType"];
		if (xmlAttribute == null)
		{
			throw new ConfigurationErrorsException("Serialized type not specified.", section);
		}
		string value = xmlAttribute.Value;
		#if NEWSdk_MONOLITHIC
        Type type = ResolveBuiltInType(value);
#else
        Type type = Type.GetType(value, throwOnError: false);
#endif
		if (type == null)
		{
			throw new ConfigurationErrorsException("Serialized type not found or can not be instantiated");
		}
		XmlElement xmlElement = section.OwnerDocument.CreateElement(type.Name);
		while (section.FirstChild != null)
		{
			xmlElement.AppendChild(section.FirstChild);
		}
		while (section.Attributes.Count != 0)
		{
			xmlElement.Attributes.Append(section.Attributes[0]);
		}
		section.AppendChild(xmlElement);
		object obj = null;
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(type);
			XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlElement);
			obj = xmlSerializer.Deserialize(xmlNodeReader);
			xmlNodeReader.Close();
			return obj;
		}
		catch (XmlException inner)
		{
			throw new ConfigurationErrorsException("XML parsing error", inner);
		}
	}
#if NEWSdk_MONOLITHIC
    private static Type ResolveBuiltInType(string value)
    {
        // Existing SDK settings may name the former Core identity. Resolve this
        // one known settings contract locally; never call Type.GetType(oldName).
        var separator=value.IndexOf(',');
        var typeName=(separator<0?value:value.Substring(0,separator)).Trim();
        if(typeName != typeof(Settings).FullName) return null;
        if(separator>=0) {
            System.Reflection.AssemblyName identity;
            try { identity=new System.Reflection.AssemblyName(value.Substring(separator+1).Trim()); }
            catch(ArgumentException) { return null; }
            catch(System.IO.FileLoadException) { return null; }
            var name=identity.Name;
            if(name != "BinaryAssetBuilder.Core" && name != typeof(Settings).Assembly.GetName().Name) return null;
        }
        return typeof(Settings);
    }
#endif
}
