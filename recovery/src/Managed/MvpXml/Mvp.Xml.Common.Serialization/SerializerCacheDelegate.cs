using System;
using System.Xml.Serialization;

namespace Mvp.Xml.Common.Serialization;

public delegate void SerializerCacheDelegate(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace);
