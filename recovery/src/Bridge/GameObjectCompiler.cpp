#include <msclr/marshal_cppstd.h>
#include <stdexcept>
#include "XmlNodeAdapter.h"
#include "../Native/GameObject.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;
namespace Recovered {
public ref class GameObjectCompiler abstract sealed {
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        auto result=gcnew array<Byte>(static_cast<int>(source.size()));
        if(result->Length) System::Runtime::InteropServices::Marshal::Copy(IntPtr(const_cast<unsigned char*>(source.data())),result,0,result->Length);
        return result;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node,XmlNamespaceManager^ namespaces,bool bigEndian) {
        if(node==nullptr) throw gcnew ArgumentNullException("node");
        if(node->LocalName!="GameObject" || node->NamespaceURI!="uri:ea.com:eala:asset") throw gcnew NotSupportedException("Expected GameObject asset element.");
        auto nav=node->CreateNavigator();auto attrs=nav->Clone();std::map<std::string,std::string> input;
        if(attrs->MoveToFirstAttribute()) do {
            if(attrs->NamespaceURI=="http://www.w3.org/2000/xmlns/") continue;
            if(attrs->NamespaceURI->Length!=0) throw gcnew NotSupportedException("Namespaced GameObject attribute.");
            if(attrs->LocalName=="id" || attrs->LocalName=="TypeId") continue;
            input[marshal_as<std::string>(attrs->LocalName)]=marshal_as<std::string>(attrs->Value);
        } while(attrs->MoveToNextAttribute());
        if(nav->SelectChildren(XPathNodeType::Element)->MoveNext()) throw gcnew NotSupportedException("GameObject child blocks are not reconstructed.");
        try {
            auto compiled=Native::CompileGameObject(input,bigEndian);auto buffer=gcnew AssetBuffer();
            buffer->InstanceData=Copy(compiled.instance);buffer->RelocationData=Copy(compiled.relocations);buffer->ImportsData=Copy(compiled.imports);return buffer;
        } catch(const std::exception& error) {throw gcnew NotSupportedException(gcnew String(error.what()));}
    }
    static AssetBuffer^ ProcessInstance(InstanceDeclaration^ declaration,TargetPlatform platform) {
        if(declaration==nullptr) throw gcnew ArgumentNullException("declaration");
        if(declaration->Handle->TypeId!=2486173485u) throw gcnew NotSupportedException("Expected GameObject type.");
        return CompileXml(declaration->Node,declaration->Document->NamespaceManager,platform!=TargetPlatform::Win32);
    }
};
}
