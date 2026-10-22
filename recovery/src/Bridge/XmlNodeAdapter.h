#pragma once
#using <System.Xml.dll>
using namespace System;
using namespace System::Xml;
using namespace System::Xml::XPath;

namespace Recovered {
// Owned managed navigators replace the original 16-byte gcroot Node wrapper.
// Preserve namespace-qualified child selection and unqualified attributes.
ref class XmlNodeAdapter sealed {
    XPathNavigator^ navigator_;
    XmlNamespaceManager^ namespaces_;
    String^ default_;
public:
    XmlNodeAdapter(XPathNavigator^ navigator, XmlNamespaceManager^ namespaces)
        : navigator_(navigator), namespaces_(namespaces) {}
    XmlNodeAdapter(String^ value) : default_(value) {}
    String^ Attribute(String^ name, String^ fallback) {
        if (navigator_ != nullptr) {
            XPathNavigator^ cursor = navigator_->Clone();
            if (cursor->MoveToAttribute(name, String::Empty)) return cursor->Value;
        }
        return fallback;
    }
    XPathNodeIterator^ Children(String^ name) {
        if (navigator_ == nullptr) return nullptr;
        if (name == nullptr) return navigator_->SelectChildren(XPathNodeType::Element);
        return navigator_->Select("ea:" + name, namespaces_);
    }
    XmlNodeAdapter^ Child(String^ name, String^ fallback) {
        if (navigator_ != nullptr) {
            XPathNavigator^ child = navigator_->SelectSingleNode("ea:" + name, namespaces_);
            if (child != nullptr) return gcnew XmlNodeAdapter(child, namespaces_);
        }
        return fallback == nullptr ? nullptr : gcnew XmlNodeAdapter(fallback);
    }
    String^ Value() { return navigator_ == nullptr ? default_ : navigator_->Value; }
};
}
