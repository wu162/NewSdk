#include "MeshConverters.h"
#include <cmath>
#include <cstring>
#include <stdexcept>
#include <limits>

namespace Recovered { namespace W3x {
namespace {
static_assert(sizeof(float) == 4 && std::numeric_limits<float>::is_iec559,
    "W3X requires IEEE binary32");
template<class T> T Load(const unsigned char* p) { T v; std::memcpy(&v,p,sizeof(v)); return v; }
float R4(double v) { volatile float f = static_cast<float>(v); return f; }
void Store16(unsigned char* p, std::uint16_t v, bool be) {
    p[be ? 1 : 0] = static_cast<unsigned char>(v);
    p[be ? 0 : 1] = static_cast<unsigned char>(v >> 8);
}
void Store32(unsigned char* p, std::uint32_t v, bool be) {
    for (int i=0;i<4;++i) p[be ? 3-i : i] = static_cast<unsigned char>(v >> (i*8));
}
std::uint32_t QuietNaNBits(std::uint32_t bits) {
    if ((bits & 0x7f800000U) == 0x7f800000U && (bits & 0x007fffffU) != 0)
        bits |= 0x00400000U;
    return bits;
}
void StoreFloat(unsigned char* p, float v, bool be) {
    std::uint32_t bits; std::memcpy(&bits,&v,4);
    // CopyTexCoord stores the endian-swapped bits back into a float local,
    // then executes ldloc/stind.r4. CLR4 x86 quiets a signaling NaN at that
    // float store, even when byte swapping a perfectly finite source value
    // happened to manufacture the signaling NaN. A raw byte store differs.
    bits=QuietNaNBits(bits);
    if (be) bits=(bits>>24) | ((bits>>8)&0x0000ff00U) |
        ((bits<<8)&0x00ff0000U) | (bits<<24);
    Store32(p,QuietNaNBits(bits),false);
}
// Original CLR4 x86 conv.u4 uses a truncating signed-64 conversion and
// keeps its low 32 bits. Invalid conversion returns integer indefinite
// (0x8000000000000000), whose low32 are zero. This also defines NaN/Inf and
// finite float extrema after the original double-precision arithmetic.
// Explicit bounds avoid C++ FP-to-integer undefined behavior.
std::uint32_t ConvU4(double v) {
    if (!std::isfinite(v) || v < -9223372036854775808.0 || v >= 9223372036854775808.0)
        return 0;
    return static_cast<std::uint32_t>(static_cast<std::int64_t>(v));
}
double R8(double v) { volatile double d=v; return d; }
float Clamp(float v,float lo,float hi) { return v < lo ? lo : (v > hi ? hi : v); }
std::uint32_t QuantizeColor(float v) {
    v=Clamp(v,0.0f,1.0f);
    const float scaled=R4(static_cast<double>(v)*255.0);
    const float rounded=R4(static_cast<double>(scaled)+(scaled < 0 ? -0.5 : 0.5));
    // NaN survives both original unordered clamp comparisons. conv.i4
    // produces INT_MIN; conv.r4/conv.u4 then yield 0x80000000. The component
    // stores only the low byte, which is zero for either NaN sign/payload.
    if (std::isnan(rounded)) return 0;
    // Original conv.i4 -> conv.r4 -> conv.u4, all exact on [0,255].
    return static_cast<std::uint32_t>(static_cast<std::int32_t>(rounded));
}
}
std::uint16_t MeshFloatToHalf(float value) {
    std::uint32_t bits; std::memcpy(&bits,&value,4);
    std::uint32_t magnitude=bits & 0x7fffffffU;
    const std::uint32_t sign=(bits >> 16) & 0x8000U;
    if (magnitude > 1207955455U) return static_cast<std::uint16_t>(sign | 0x7fffU);
    if (magnitude >= 947912704U) magnitude-=939524096U;
    else magnitude=((magnitude & 0x7fffffU) | 0x800000U) >> ((113U-(magnitude >> 23)) & 31U);
    return static_cast<std::uint16_t>(((magnitude + ((magnitude >> 13) & 1U) + 4095U) >> 13) | sign);
}
void ConvertMeshStream(MeshConversion kind, int count,
    const void* source,std::ptrdiff_t srcPitch,void* destination,std::ptrdiff_t dstPitch,
    const ConverterContext& c) {
    if (kind == MeshConversion::CopyVector3 && c.bigEndian)
        throw std::domain_error("Assert failed: !converterData.m_endian.IsBigEndian()");
    if (count < 0) throw std::invalid_argument("Negative W3X element count");
    if (!count) return;
    if (!source || !destination) throw std::invalid_argument("Null W3X converter buffer");
    const unsigned char* src=static_cast<const unsigned char*>(source);
    unsigned char* dst=static_cast<unsigned char*>(destination);
    for (int element=0;element<count;++element) {
        switch (kind) {
        case MeshConversion::CopyVector3:
            std::memcpy(dst,src,12); break;
        case MeshConversion::CopyTexCoord: {
            const float u=Load<float>(src),v=R4(1.0-static_cast<double>(Load<float>(src+4)));
            StoreFloat(dst,u,c.bigEndian); StoreFloat(dst+4,v,c.bigEndian); break;
        }
        case MeshConversion::CompressTexCoord: {
            const float u=Load<float>(src),v=R4(1.0-static_cast<double>(Load<float>(src+4)));
            Store16(dst,MeshFloatToHalf(u),c.bigEndian); Store16(dst+2,MeshFloatToHalf(v),c.bigEndian); break;
        }
        case MeshConversion::Vector3ToHalf4:
            for (int i=0;i<3;++i) Store16(dst+i*2,MeshFloatToHalf(Load<float>(src+i*4)),c.bigEndian);
            Store16(dst+6,MeshFloatToHalf(0.0f),c.bigEndian); break;
        case MeshConversion::Vector4ToUByte4N: {
            std::uint32_t packed=0;
            for (int i=0;i<4;++i) packed|=QuantizeColor(Load<float>(src+i*4)) << (i*8);
            Store32(dst,packed,c.bigEndian); break;
        }
        case MeshConversion::ConvertBlendIndex: {
            const std::uint16_t index=Load<std::uint16_t>(src);
            if (!c.inverseBoneMapping || index >= c.inverseBoneMappingCount)
                throw std::out_of_range("Assert failed: srcIndex < converterData.m_inverseBoneMapping.size()");
            *dst=static_cast<unsigned char>(c.inverseBoneMapping[index]); break;
        }
        case MeshConversion::ConvertBlendWeight:
            *dst=static_cast<unsigned char>(ConvU4(R8(R8(static_cast<double>(Load<float>(src))*255.0)+0.5)) & 255U); break;
        case MeshConversion::CompressNormalizedDirection: {
            const float x=R4(static_cast<double>(Clamp(Load<float>(src),-1.0f,1.0f))*1023.0);
            const float y=R4(static_cast<double>(Clamp(Load<float>(src+4),-1.0f,1.0f))*1023.0);
            const float z=R4(static_cast<double>(Clamp(Load<float>(src+8),-1.0f,1.0f))*511.0);
            const std::uint32_t packed=(((ConvU4(y)&0x7ffU) | (ConvU4(z)<<11))<<11) | (ConvU4(x)&0x7ffU);
            Store32(dst,packed,c.bigEndian); break;
        }
        default: throw std::invalid_argument("Unknown W3X mesh converter");
        }
        // Avoid manufacturing an out-of-bounds pointer after the final element.
        if (element+1<count) { src+=srcPitch; dst+=dstPitch; }
    }
}
}}
