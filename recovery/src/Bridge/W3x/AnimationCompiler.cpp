#include <msclr/marshal_cppstd.h>
#include <cstdlib>
#include <cstring>
#include <exception>
#include "../XmlNodeAdapter.h"
#include "../../Native/ScalarParsing.h"
#include "../../Native/W3x/AnimationPipeline.h"
#using <BinaryAssetBuilder.Core.dll>
using namespace BinaryAssetBuilder::Core;
using namespace msclr::interop;

namespace Recovered { namespace W3x {
// Component bridge for already-preprocessed BAB input. The production plugin
// continues to reject animation until the full component gate has passed.
public ref class AnimationCompiler abstract sealed {
    static std::string Text(String^ value) {
        return marshal_as<std::string>(value == nullptr ? String::Empty : value);
    }
    static std::uint32_t Unsigned(XmlNodeAdapter^ node, String^ name) {
        String^ value = node->Attribute(name, nullptr);
        return value == nullptr ? 0u : Native::ParseUnsigned(Text(value));
    }
    static float Float(XmlNodeAdapter^ node, String^ name, float fallback) {
        String^ value = node->Attribute(name, nullptr);
        return value == nullptr ? fallback : Native::ParseFloat(Text(value));
    }
    static bool Boolean(XmlNodeAdapter^ node, String^ name, bool fallback) {
        String^ value = node->Attribute(name, nullptr);
        return value == nullptr ? fallback : Native::ParseBoolean(Text(value));
    }
    static std::uint32_t ChannelType(XmlNodeAdapter^ node) {
        String^ value = node->Attribute("Type", nullptr);
        if (value == nullptr) return 0;
        // Case-sensitive source enum lookup maps present unknown names to Orientation.
        array<String^>^ names = {"XTranslation", "YTranslation", "ZTranslation", "Orientation", "Visibility"};
        for (int i = 0; i != names->Length; ++i) if (value == names[i]) return i;
        return 3;
    }
    static array<Byte>^ Copy(const std::vector<unsigned char>& source) {
        array<Byte>^ output = gcnew array<Byte>(static_cast<int>(source.size()));
        if (output->Length) System::Runtime::InteropServices::Marshal::Copy(
            IntPtr(const_cast<unsigned char*>(source.data())), output, 0, output->Length);
        return output;
    }
public:
    static AssetBuffer^ CompileXml(XmlNode^ node, XmlNamespaceManager^ namespaces, TargetPlatform platform) {
        if (node == nullptr) throw gcnew ArgumentNullException("node");
        XmlNodeAdapter^ root = gcnew XmlNodeAdapter(node->CreateNavigator(), namespaces);
        AnimationInput input;
        input.numFrames = Unsigned(root, "NumFrames");
        input.frameRate = Unsigned(root, "FrameRate");
        // BAB resolves references to a name followed by a backslash and a
        // decimal import index before entering the original marshaler.
        std::string hierarchy = Text(root->Attribute("Hierarchy", nullptr));
        const char* separator = std::strchr(hierarchy.c_str(), '\\');
        if (separator) input.hierarchy = static_cast<std::uint32_t>(std::strtol(separator + 1, nullptr, 10));
        XmlNodeAdapter^ settings = root->Child("CompressionSettings", nullptr);
        input.hasCompressionSettings = settings != nullptr;
        if (settings != nullptr) {
            input.settings.maxTranslationError = Float(settings, "MaxTranslationError", .001f);
            input.settings.maxRotationError = Float(settings, "MaxRotationError", .01f);
            input.settings.maxVisibilityError = Float(settings, "MaxVisibilityError", .01f);
            input.settings.maxAdaptiveDeltaError = Float(settings, "MaxAdaptiveDeltaError", .03f);
            input.settings.forceReductionRate = Float(settings, "ForceReductionRate", 1.f);
            input.settings.allowTimecoded = Boolean(settings, "AllowTimeCoded", true);
            input.settings.allowAdaptiveDelta = Boolean(settings, "AllowAdaptiveDelta", true);
        }
        XmlNodeAdapter^ channels = root->Child("Channels", nullptr);
        if (channels != nullptr) {
            XPathNodeIterator^ children = channels->Children(nullptr);
            while (children->MoveNext()) {
                XmlNodeAdapter^ channel = gcnew XmlNodeAdapter(children->Current->Clone(), namespaces);
                AnimationInputChannel out;
                // Do not infer TypeId from the element name: preprocessing is
                // part of BAB's caller contract, not this marshaler's job.
                out.typeId = Unsigned(channel, "TypeId");
                if (out.typeId != 1355976437u && out.typeId != 4131577120u)
                    throw gcnew NotSupportedException("Unsupported or missing preprocessed animation channel TypeId.");
                out.type = ChannelType(channel);
                out.pivot = static_cast<std::uint16_t>(Unsigned(channel, "Pivot"));
                out.firstFrame = Unsigned(channel, "FirstFrame");
                XPathNodeIterator^ frames = channel->Children("Frame");
                while (frames->MoveNext()) {
                    XmlNodeAdapter^ frame = gcnew XmlNodeAdapter(frames->Current->Clone(), namespaces);
                    AnimationInputFrame value;
                    value.binaryMove = Boolean(frame, "BinaryMove", false);
                    if (out.typeId == 1355976437u) value.values.push_back(Native::ParseFloat(Text(frame->Value())));
                    else {
                        value.values.push_back(Float(frame, "X", 0.f));
                        value.values.push_back(Float(frame, "Y", 0.f));
                        value.values.push_back(Float(frame, "Z", 0.f));
                        value.values.push_back(Float(frame, "W", 0.f));
                    }
                    out.frames.push_back(value);
                }
                input.channels.push_back(out);
            }
        }
        try {
            Relo::Chunk result = CompileAnimation(input, static_cast<int>(platform));
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
