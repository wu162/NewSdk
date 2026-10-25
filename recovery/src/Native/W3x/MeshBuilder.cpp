#include "MeshBuilder.h"
#include <algorithm>
#include <cstdio>
#include <limits>
#include <set>
#include <stdexcept>

namespace Recovered { namespace W3x {
namespace {
struct Handler { char name; const char* format; std::uint32_t size; MeshConversion converter; };
const Handler win[] = {
    {'p',"3f32",12,MeshConversion::CopyVector3},
    {'n',"3f32",12,MeshConversion::CopyVector3},
    {'g',"3f32",12,MeshConversion::CopyVector3},
    {'b',"3f32",12,MeshConversion::CopyVector3},
    {'t',"2f32",8,MeshConversion::CopyTexCoord},
    {'c',"4u8n",4,MeshConversion::Vector4ToUByte4N},
    {'w',"4u8n",1,MeshConversion::ConvertBlendWeight},
    {'i',"4u8",1,MeshConversion::ConvertBlendIndex}
};
const Handler console[] = {
    {'p',"4f16",8,MeshConversion::Vector3ToHalf4},
    {'n',"3s11n",4,MeshConversion::CompressNormalizedDirection},
    {'g',"3s11n",4,MeshConversion::CompressNormalizedDirection},
    {'b',"3s11n",4,MeshConversion::CompressNormalizedDirection},
    {'t',"2f16",4,MeshConversion::CompressTexCoord},
    {'c',"4u8n",4,MeshConversion::Vector4ToUByte4N},
    {'w',"4u8n",1,MeshConversion::ConvertBlendWeight},
    {'i',"4u8",1,MeshConversion::ConvertBlendIndex}
};
const Handler* Handlers(int platform) {
    if (platform < 0 || platform > 2) throw std::invalid_argument("unknown W3X mesh platform");
    return platform == 0 ? win : console;
}
const MeshVector4 defaultColor = {{1.0f, 1.0f, 1.0f, 1.0f}};
void Add(std::vector<MeshDataMapping>& result, std::uint32_t usage,
         std::size_t index, const void* source, std::uint32_t pitch) {
    if (index > std::numeric_limits<std::uint32_t>::max()) throw std::length_error("mesh usage index");
    MeshDataMapping m = {usage, static_cast<std::uint32_t>(index), source, pitch, 0};
    result.push_back(m);
}
void Validate(const MeshPipelineData& p) {
    if (p.vertices.empty()) throw std::invalid_argument("mesh requires a vertex stream");
    const std::size_t count = p.vertices.front().size();
    if (count > static_cast<std::size_t>(std::numeric_limits<int>::max()))
        throw std::length_error("mesh vertex count");
    // Original reads vertexCount items without checking list lengths. Reject
    // truncated typed streams to avoid undefined reads; do not silently pad.
    for (std::size_t i=0; i<p.vertices.size(); ++i)
        if (p.vertices[i].size()<count) throw std::invalid_argument("short vertex stream");
    for (std::size_t i=0; i<p.normals.size(); ++i)
        if (p.normals[i].size()<count) throw std::invalid_argument("short normal stream");
    if (!p.tangents.empty() && !p.binormals.empty() &&
        (p.tangents.size()<count || p.binormals.size()<count))
        throw std::invalid_argument("short tangent/binormal stream");
    if (!p.colors.empty() && p.colors.size()<count) throw std::invalid_argument("short color stream");
    for (std::size_t i=0; i<p.texCoords.size(); ++i)
        if (p.texCoords[i].size()<count) throw std::invalid_argument("short texture stream");
    for (std::size_t i=0; i<std::min(p.vertices.size(),p.boneInfluences.size()); ++i)
        if (p.boneInfluences[i].size()<count) throw std::invalid_argument("short bone stream");
}
}

std::vector<MeshDataMapping> DetermineVertexDataContents(const MeshPipelineData& p) {
    Validate(p);
    static_assert(sizeof(MeshVector3)==12,"Vector3 layout");
    static_assert(sizeof(MeshVector4)==16,"Vector4 layout");
    static_assert(sizeof(MeshBoneInfluence)==8,"BoneInfluence layout");
    std::vector<MeshDataMapping> m;
    for (std::size_t i=0; i<p.vertices.size(); ++i) Add(m,0,i,p.vertices[i].data(),12);
    for (std::size_t i=0; i<p.normals.size(); ++i) Add(m,1,i,p.normals[i].data(),12);
    Add(m,5,0,p.colors.empty() ? &defaultColor : p.colors.data(),p.colors.empty()?0:16);
    // Both must be present: a lone Tangents or Binormals list is ignored.
    if (!p.tangents.empty() && !p.binormals.empty()) {
        Add(m,2,0,p.tangents.data(),12);
        Add(m,3,0,p.binormals.data(),12);
    }
    for (std::size_t i=0; i<p.texCoords.size(); ++i) Add(m,4,i,p.texCoords[i].data(),8);
    const std::size_t n = std::min(p.vertices.size(),p.boneInfluences.size());
    for (std::size_t i=0; i<n; ++i) {
        const unsigned char* data = reinterpret_cast<const unsigned char*>(p.boneInfluences[i].data());
        Add(m,7,i,p.boneInfluences[i].empty()?0:data+4,8);
    }
    if (n>1) for (std::size_t i=0; i<n; ++i) Add(m,6,i,p.boneInfluences[i].data(),8);
    return m;
}

std::uint32_t DetermineVertexDataLayout(std::vector<MeshDataMapping>& m,int platform) {
    const Handler* handlers = Handlers(platform);
    std::uint64_t stride = 0;
    for (std::size_t i=0; i<m.size(); ++i) {
        if (m[i].usage>=8) throw std::invalid_argument("unknown mesh usage");
        if (stride>std::numeric_limits<std::int32_t>::max()-7u) throw std::length_error("mesh stride");
        m[i].destinationOffset=static_cast<std::uint32_t>(stride);
        if ((m[i].usage==6 || m[i].usage==7) && platform==1) {
            // Original uses stride - 2*usageIndex + 3. Preserve the reverse
            // byte order inside each four-byte Xenon index/weight element.
            const std::int64_t offset = static_cast<std::int64_t>(stride)-2LL*m[i].usageIndex+3;
            if (offset<0 || offset>std::numeric_limits<std::int32_t>::max())
                throw std::invalid_argument("invalid Xbox mesh usage index");
            m[i].destinationOffset=static_cast<std::uint32_t>(offset);
        }
        stride+=handlers[m[i].usage].size;
        if (i+1==m.size() || m[i+1].usage!=m[i].usage) stride=(stride+3)&~std::uint64_t(3);
    }
    return static_cast<std::uint32_t>(stride);
}

void ComputeBoneMappingTable(const MeshPipelineData& p, std::vector<std::uint16_t>& forward,
                             std::vector<std::uint16_t>& inverse) {
    std::set<std::uint16_t> used;
    // All streams contribute, including streams not emitted because the
    // source position-stream count limits DetermineVertexDataContents.
    for (std::size_t i=0; i<p.boneInfluences.size(); ++i)
        for (std::size_t j=0; j<p.boneInfluences[i].size(); ++j)
            used.insert(p.boneInfluences[i][j].bone);
    forward.assign(used.begin(),used.end());
    inverse.clear();
    if (used.empty()) return;
    inverse.assign(static_cast<std::size_t>(*used.rbegin())+1,0);
    for (std::size_t i=0; i<forward.size(); ++i) inverse[forward[i]]=static_cast<std::uint16_t>(i);
}

std::string BuildVertexDeclaration(const std::vector<MeshDataMapping>& m,int platform) {
    const Handler* handlers=Handlers(platform);
    std::string declaration;
    for (std::size_t i=0; i<m.size(); ++i) {
        const MeshDataMapping& first=m[i];
        if (first.usage>=8) throw std::invalid_argument("unknown mesh usage");
        std::uint32_t offset=first.destinationOffset;
        if (first.usage==6 || first.usage==7) {
            std::size_t end=i+1;
            while (end<m.size() && m[end].usage==first.usage) ++end;
            if (end-i>4) throw std::invalid_argument("more than four packed mesh influences");
            if (platform==1) offset-=offset%4;
            i=end-1;
        }
        char field[64];
        const Handler& h=handlers[first.usage];
        const int n=std::snprintf(field,sizeof(field),"%c%X:%02X:%s",h.name,first.usageIndex,offset,h.format);
        if (n<0 || static_cast<std::size_t>(n)>=sizeof(field)) throw std::length_error("mesh declaration field");
        if (!declaration.empty()) declaration+=' ';
        declaration+=field;
        // Original formats into a 512-byte local buffer. Do not reproduce
        // stack corruption for an excessive count of input texture streams.
        if (declaration.size()>=512) throw std::length_error("mesh declaration exceeds original buffer");
    }
    return declaration;
}

MeshVertexData BuildMeshVertexData(const MeshPipelineData& p,int platform) {
    Handlers(platform);
    std::vector<MeshDataMapping> mappings=DetermineVertexDataContents(p);
    MeshVertexData out;
    out.vertexCount=static_cast<std::uint32_t>(p.vertices.front().size());
    out.vertexStride=DetermineVertexDataLayout(mappings,platform);
    std::vector<std::uint16_t> inverse;
    ComputeBoneMappingTable(p,out.boneMapping,inverse);
    const std::uint64_t size=std::uint64_t(out.vertexCount)*out.vertexStride;
    if (size>std::numeric_limits<std::uint32_t>::max()) throw std::length_error("mesh vertex buffer");
    out.vertices.assign(static_cast<std::size_t>(size),0);
    ConverterContext context;
    context.bigEndian=platform!=0;
    context.inverseBoneMapping=inverse.empty()?0:inverse.data();
    context.inverseBoneMappingCount=inverse.size();
    const Handler* handlers=Handlers(platform);
    for (std::size_t i=0; i<mappings.size(); ++i) {
        const MeshDataMapping& m=mappings[i];
        if (m.destinationOffset+handlers[m.usage].size>out.vertexStride)
            throw std::invalid_argument("mesh mapping exceeds stride");
        if (out.vertexCount) ConvertMeshStream(handlers[m.usage].converter,
            static_cast<int>(out.vertexCount),m.source,m.sourcePitch,
            out.vertices.data()+m.destinationOffset,out.vertexStride,context);
    }
    for (std::size_t i=0; i<mappings.size(); ++i) {
        if (mappings[i].usage!=6) continue;
        std::size_t end=i+1;
        while (end<mappings.size() && mappings[end].usage==6) ++end;
        if (end-i!=2) throw std::invalid_argument("original mesh weight group requires two mappings");
        for (std::size_t v=0; v<out.vertexCount; ++v)
            out.vertices[v*out.vertexStride+mappings[i+1].destinationOffset]=
                static_cast<unsigned char>(255-out.vertices[v*out.vertexStride+mappings[i].destinationOffset]);
        break;
    }
    out.declaration=BuildVertexDeclaration(mappings,platform);
    return out;
}
} }
