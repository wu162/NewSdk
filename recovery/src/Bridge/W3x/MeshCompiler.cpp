#include <msclr/marshal_cppstd.h>
#include <exception>
#include "../XmlNodeAdapter.h"
#include "../../Native/ScalarParsing.h"
#include "../../Native/W3x/MeshAsset.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;

namespace Recovered { namespace W3x {
// Component adapter for BAB's preprocessed XML. Production routing is separate.
public ref class MeshCompiler abstract sealed {
    static std::string Text(String^ value) {
        String^ normalized = value == nullptr ? String::Empty : value;
        return marshal_as<std::string>(normalized);
    }
    static std::uint32_t Unsigned(XmlNodeAdapter^ node, String^ name) {
        if (node == nullptr) return 0;
        String^ value = node->Attribute(name, nullptr);
        return value == nullptr ? 0u : Native::ParseUnsigned(Text(value));
    }
    static float Float(XmlNodeAdapter^ node, String^ name) {
        if (node == nullptr) return 0.f;
        String^ value = node->Attribute(name, nullptr);
        return value == nullptr ? 0.f : Native::ParseFloat(Text(value));
    }
    static bool Boolean(XmlNodeAdapter^ node, String^ name) {
        return node != nullptr && Native::ParseBoolean(Text(node->Attribute(name, nullptr)));
    }
    static float FloatValue(XmlNodeAdapter^ node) {
        return node == nullptr ? 0.f : Native::ParseFloat(Text(node->Value()));
    }
    static MeshVector3 Vector3(XmlNodeAdapter^ node) {
        MeshVector3 value = {{Float(node,"X"),Float(node,"Y"),Float(node,"Z")}};
        return value;
    }
    static std::vector<MeshVector3> Vectors3(XmlNodeAdapter^ parent, String^ child, XmlNamespaceManager^ ns) {
        std::vector<MeshVector3> result;
        if (parent == nullptr) return result;
        XPathNodeIterator^ list = parent->Children(child);
        while (list->MoveNext()) result.push_back(Vector3(gcnew XmlNodeAdapter(list->Current->Clone(),ns)));
        return result;
    }
    static void VectorStreams3(XmlNodeAdapter^ parent, String^ stream, String^ child,
                               XmlNamespaceManager^ ns, std::vector<std::vector<MeshVector3> >& output) {
        XPathNodeIterator^ streams = parent->Children(stream);
        while (streams->MoveNext())
            output.push_back(Vectors3(gcnew XmlNodeAdapter(streams->Current->Clone(),ns),child,ns));
    }
    static void Pipeline(XmlNodeAdapter^ root, XmlNamespaceManager^ ns, MeshPipelineData& data) {
        VectorStreams3(root,"Vertices","V",ns,data.vertices);
        VectorStreams3(root,"Normals","N",ns,data.normals);
        data.tangents = Vectors3(root->Child("Tangents",nullptr),"T",ns);
        data.binormals = Vectors3(root->Child("Binormals",nullptr),"B",ns);
        XmlNodeAdapter^ colors = root->Child("VertexColors",nullptr);
        if (colors != nullptr) {
            XPathNodeIterator^ list = colors->Children("C");
            while (list->MoveNext()) {
                XmlNodeAdapter^ color = gcnew XmlNodeAdapter(list->Current->Clone(),ns);
                MeshVector4 value = {{Float(color,"R"),Float(color,"G"),Float(color,"B"),Float(color,"A")}};
                data.colors.push_back(value);
            }
        }
        XPathNodeIterator^ textureStreams = root->Children("TexCoords");
        while (textureStreams->MoveNext()) {
            std::vector<MeshVector2> values;
            XmlNodeAdapter^ stream = gcnew XmlNodeAdapter(textureStreams->Current->Clone(),ns);
            XPathNodeIterator^ list = stream->Children("T");
            while (list->MoveNext()) {
                XmlNodeAdapter^ uv = gcnew XmlNodeAdapter(list->Current->Clone(),ns);
                MeshVector2 value = {{Float(uv,"X"),Float(uv,"Y")}};
                values.push_back(value);
            }
            data.texCoords.push_back(values);
        }
        XPathNodeIterator^ influenceStreams = root->Children("BoneInfluences");
        while (influenceStreams->MoveNext()) {
            std::vector<MeshBoneInfluence> values;
            XmlNodeAdapter^ stream = gcnew XmlNodeAdapter(influenceStreams->Current->Clone(),ns);
            XPathNodeIterator^ list = stream->Children("I");
            while (list->MoveNext()) {
                XmlNodeAdapter^ influence = gcnew XmlNodeAdapter(list->Current->Clone(),ns);
                MeshBoneInfluence value;
                value.weight = Float(influence,"Weight");
                value.bone = static_cast<std::uint16_t>(Unsigned(influence,"Bone"));
                value.padding = 0;
                values.push_back(value);
            }
            data.boneInfluences.push_back(values);
        }
        // ShadeIndices is parsed into the original temporary tracker but never
        // consumed by DetermineVertexDataContents/BuildVertexData.
    }
    static std::uint32_t Geometry(XmlNodeAdapter^ root) {
        String^ name = root->Attribute("GeometryType",nullptr);
        if (name == nullptr || name == "Normal") return 0;
        if (name == "Skin") return 1;
        if (name == "CameraAligned") return 2;
        if (name == "CameraOriented") return 3;
        return 2; // Original enum map fallback, not the XSD's ordinal position.
    }
    static void Triangles(XmlNodeAdapter^ root, XmlNamespaceManager^ ns, MeshAssetInput& input) {
        XmlNodeAdapter^ parent = root->Child("Triangles",nullptr);
        if (parent == nullptr) return;
        XPathNodeIterator^ list = parent->Children("T");
        while (list->MoveNext()) {
            XmlNodeAdapter^ node = gcnew XmlNodeAdapter(list->Current->Clone(),ns);
            MeshTriangle triangle;
            XPathNodeIterator^ indices = node->Children("V");
            while (indices->MoveNext())
                triangle.vertices.push_back(Native::ParseUnsigned(Text(indices->Current->Value)));
            triangle.normal = Vector3(node->Child("Nrm",nullptr));
            triangle.distance = FloatValue(node->Child("Dist",nullptr));
            input.triangles.push_back(triangle);
        }
    }
    static void Tree(XmlNodeAdapter^ root, XmlNamespaceManager^ ns, MeshAssetInput& input) {
        XmlNodeAdapter^ tree = root->Child("AABTree",nullptr);
        input.hasAabTree = tree != nullptr;
        if (tree == nullptr) return;
        XmlNodeAdapter^ polys = tree->Child("PolyIndices",nullptr);
        if (polys != nullptr) {
            XPathNodeIterator^ values = polys->Children("P");
            while (values->MoveNext())
                input.aabTree.polyIndices.push_back(Native::ParseUnsigned(Text(values->Current->Value)));
        }
        XPathNodeIterator^ nodes = tree->Children("Node");
        while (nodes->MoveNext()) {
            XmlNodeAdapter^ node = gcnew XmlNodeAdapter(nodes->Current->Clone(),ns);
            MeshAabNode result;
            result.min = Vector3(node->Child("Min",nullptr));
            result.max = Vector3(node->Child("Max",nullptr));
            XmlNodeAdapter^ children = node->Child("Children",nullptr);
            result.hasChildren = children != nullptr;
            result.front = Unsigned(children,"Front"); result.back = Unsigned(children,"Back");
            XmlNodeAdapter^ indices = node->Child("Polys",nullptr);
            result.hasPolys = indices != nullptr;
            result.polyBegin = Unsigned(indices,"Begin"); result.polyCount = Unsigned(indices,"Count");
            input.aabTree.nodes.push_back(result);
        }
    }
    static void Shader(XmlNodeAdapter^ root, XmlNamespaceManager^ ns, MeshAssetInput& input) {
        XmlNodeAdapter^ shader = root->Child("FXShader",nullptr);
        input.hasShader = shader != nullptr;
        if (shader == nullptr) return;
        String^ shaderName = shader->Attribute("ShaderName",nullptr);
        input.shader.hasShaderName = shaderName != nullptr;
        input.shader.shaderName = Text(shaderName);
        input.shader.techniqueName = Text(shader->Attribute("TechniqueName",String::Empty));
        input.shader.techniqueIndex = static_cast<std::uint8_t>(Unsigned(shader,"TechniqueIndex"));
        XmlNodeAdapter^ constants = shader->Child("Constants",nullptr);
        if (constants == nullptr) return;
        XPathNodeIterator^ list = constants->Children(nullptr);
        while (list->MoveNext()) {
            XmlNodeAdapter^ node = gcnew XmlNodeAdapter(list->Current->Clone(),ns);
            ShaderConstantInput constant;
            constant.typeId = Unsigned(node,"TypeId");
            String^ name = node->Attribute("Name",nullptr);
            constant.hasName = name != nullptr; constant.name = Text(name);
            XmlNodeAdapter^ value = node->Child("Value",nullptr);
            switch (constant.typeId) {
            case 2750958653u: constant.booleanValue = value != nullptr && Native::ParseBoolean(Text(value->Value())); break;
            case 2300057986u: constant.integerValue = value == nullptr ? 0 : Native::ParseSigned(Text(value->Value())); break;
            case 2777716390u: constant.textureValue = value == nullptr ? std::string() : Text(value->Value()); break;
            case 3741900581u: {
                XPathNodeIterator^ values = node->Children("Value");
                while (values->MoveNext()) constant.floatValues.push_back(Native::ParseFloat(Text(values->Current->Value)));
                break;
            }
            default: break; // Original unknown-polymorphic base preserves Name.
            }
            input.shader.constants.push_back(constant);
        }
    }
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ result = gcnew array<Byte>(static_cast<int>(source.size()));
        if (result->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())),result,0,result->Length);
        return result;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, TargetPlatform platform) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        XmlNodeAdapter^ root = gcnew XmlNodeAdapter(node->CreateNavigator(),namespaces);
        try {
            MeshAssetInput input;
            Pipeline(root,namespaces,input.pipeline);
            input.geometryType = Geometry(root);
            input.hidden = Boolean(root,"Hidden"); input.castShadow = Boolean(root,"CastShadow");
            input.sortLevel = static_cast<std::uint8_t>(Unsigned(root,"SortLevel"));
            XmlNodeAdapter^ box = root->Child("BoundingBox",nullptr);
            if (box != nullptr) {
                input.boundingMin = Vector3(box->Child("Min",nullptr));
                input.boundingMax = Vector3(box->Child("Max",nullptr));
            }
            XmlNodeAdapter^ sphere = root->Child("BoundingSphere",nullptr);
            if (sphere != nullptr) {
                input.sphereRadius = Float(sphere,"Radius");
                input.sphereCenter = Vector3(sphere->Child("Center",nullptr));
            }
            Triangles(root,namespaces,input);
            Shader(root,namespaces,input);
            Tree(root,namespaces,input);
            Relo::Chunk result = CompileMeshAsset(input,static_cast<int>(platform));
            AssetBuffer^ buffer = gcnew AssetBuffer();
            buffer->InstanceData = Copy(result.instance);
            buffer->RelocationData = Copy(result.relocations);
            buffer->ImportsData = Copy(result.imports);
            return buffer;
        } catch (const std::exception& error) {
            throw gcnew NotSupportedException(gcnew String(error.what()));
        }
    }
};
}}
