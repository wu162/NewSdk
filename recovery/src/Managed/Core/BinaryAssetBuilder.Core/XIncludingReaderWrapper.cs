using System.Xml;
using Mvp.Xml.XInclude;

namespace BinaryAssetBuilder.Core;

// Retained for existing callers. Mvp.Xml is now a compile-time source dependency.
public class XIncludingReaderWrapper
{
    public static void LoadAssembly()
    {
        // Compatibility entry point; no dynamic assembly discovery is needed.
    }

    public static XmlReader GetReader(XmlReader reader, FileNameXmlResolver resolver)
    {
        return new XIncludingReader(reader) { XmlResolver = resolver };
    }
}
