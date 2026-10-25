// x86 CLR4 component oracle: original internal BuildVertexData token 0xCE.
// Compares its real output against recovered code; does not launch SDK/BAB.
#include "../src/Native/W3x/MeshBuilder.h"
#include "../src/Native/W3x/MeshSerialization.h"
#include <cstdint>
#include <cstring>
#include <stdexcept>
#include <vector>
#using <System.dll>
using namespace System;
using namespace System::Reflection;
using namespace Recovered::W3x;

namespace {
int fixtures=0;
long long comparedBytes=0;
struct RawList { int count; const void* data; };
struct RawPipeline {
    RawList vertices,normals;
    const RawList* tangents;
    const RawList* binormals;
    const RawList* colors;
    RawList texCoords,bones;
    const RawList* shadeIndices;
};
static_assert(sizeof(RawList)==8,"oracle requires x86 pointers");
static_assert(sizeof(RawPipeline)==48,"original W3DMeshPipelineVertexData layout");

MethodInfo^ Resolve(Assembly^ a,int token,int parameters) {
    MethodInfo^ m=dynamic_cast<MethodInfo^>(a->ManifestModule->ResolveMethod(token));
    if (!m || m->GetParameters()->Length!=parameters)
        throw gcnew Exception("Original mesh method signature changed");
    return m;
}
Object^ PointerArg(MethodInfo^ m,int index,void* p) {
    return Pointer::Box(p,m->GetParameters()[index]->ParameterType);
}
void Invoke(MethodInfo^ m,array<Object^>^ a) {
    try { m->Invoke(nullptr,a); }
    catch (TargetInvocationException^ e) { throw e->InnerException; }
}
std::uint32_t Read32(const unsigned char* p,bool big) {
    std::uint32_t value=0;
    for(int i=0;i<4;++i) value|=std::uint32_t(p[i])<<((big?3-i:i)*8);
    return value;
}
std::uint16_t Read16(const unsigned char* p,bool big) {
    return static_cast<std::uint16_t>(big?(std::uint16_t(p[0])<<8)|p[1]:(std::uint16_t(p[1])<<8)|p[0]);
}
void* ReadPointer(const unsigned char* p) {void* v;std::memcpy(&v,p,4);return v;}
void Check(bool ok,const char* message) { if(!ok) throw std::runtime_error(message); }
RawList List(const void* p,std::size_t n) { RawList l={static_cast<int>(n),p};return l; }
template<class T> std::vector<RawList> Lists(const std::vector<std::vector<T> >& streams) {
    std::vector<RawList> result;
    for(std::size_t i=0;i<streams.size();++i) result.push_back(List(streams[i].data(),streams[i].size()));
    return result;
}
void Test(Assembly^ a,const MeshPipelineData& p,int platform,int caseNumber) {
    MethodInfo^ push=Resolve(a,0x06000006,4);
    MethodInfo^ build=Resolve(a,0x060000ce,5);
    MethodInfo^ destroy=Resolve(a,0x06000037,1);
    std::vector<RawList> positions=Lists(p.vertices),normals=Lists(p.normals),
                         texCoords=Lists(p.texCoords),bones=Lists(p.boneInfluences);
    RawList tangents=List(p.tangents.data(),p.tangents.size());
    RawList binormals=List(p.binormals.data(),p.binormals.size());
    RawList colors=List(p.colors.data(),p.colors.size());
    RawPipeline pipeline={List(positions.data(),positions.size()),List(normals.data(),normals.size()),
        p.tangents.empty()?0:&tangents,p.binormals.empty()?0:&binormals,p.colors.empty()?0:&colors,
        List(texCoords.data(),texCoords.size()),List(bones.data(),bones.size()),0};
    unsigned char tracker[88];std::memset(tracker,0,sizeof(tracker));
    void* root=0;
    // A zeroed Tracker matches all constructor fields immediately before its
    // first Allocate/stack push (tokens 0x88-0x8B). Allocate a 28-byte root via
    // the original Push rather than embedding it in a 96-byte W3DMesh root.
    array<Object^>^ pushArgs=gcnew array<Object^>(4);
    pushArgs[0]=PointerArg(push,0,tracker);pushArgs[1]=PointerArg(push,1,&root);
    pushArgs[2]=static_cast<UInt32>(28);pushArgs[3]=static_cast<UInt32>(1);
    Invoke(push,pushArgs);
    try {
        Check(root!=0,"original tracker root is null");
        unsigned char* output=static_cast<unsigned char*>(root);
        std::memset(output,0,28);
        bool big=platform!=0;
        array<Object^>^ buildArgs=gcnew array<Object^>(5);
        buildArgs[0]=PointerArg(build,0,&pipeline);buildArgs[1]=PointerArg(build,1,output);
        buildArgs[2]=PointerArg(build,2,tracker);buildArgs[3]=PointerArg(build,3,&big);
        buildArgs[4]=Enum::ToObject(build->GetParameters()[4]->ParameterType,platform);
        Invoke(build,buildArgs);
        MeshVertexData actual=BuildMeshVertexData(p,platform);
        Check(Read32(output,big)==actual.vertexCount,"original mesh vertex count differs");
        Check(Read32(output+4,big)==actual.vertexStride,"original mesh stride differs");
        const unsigned char* expectedVertices=static_cast<const unsigned char*>(ReadPointer(output+8));
        for(std::size_t i=0;i<actual.vertices.size();++i) {
            if(expectedVertices[i]!=actual.vertices[i]) {
                Console::WriteLine("FAIL mesh case={0} platform={1} byte={2}",caseNumber,platform,static_cast<int>(i));
                Console::WriteLine("original={0:X2} recovered={1:X2}",static_cast<int>(expectedVertices[i]),static_cast<int>(actual.vertices[i]));
                throw std::runtime_error("original mesh vertex bytes differ");
            }
            ++comparedBytes;
        }
        // BuildVertexDeclaration writes a native-host length before Tracker
        // finalization; unlike vertexCount/stride/boneCount it is not swapped.
        Check(Read32(output+12,false)==actual.declaration.size(),"original declaration length differs");
        const char* declaration=static_cast<const char*>(ReadPointer(output+16));
        Check(std::memcmp(declaration,actual.declaration.c_str(),actual.declaration.size()+1)==0,
              "original mesh declaration differs");
        comparedBytes+=static_cast<long long>(actual.declaration.size()+1);
        Check(Read32(output+20,big)==actual.boneMapping.size(),"original bone map count differs");
        const unsigned char* expectedBones=static_cast<const unsigned char*>(ReadPointer(output+24));
        for(std::size_t i=0;i<actual.boneMapping.size();++i) {
            Check(Read16(expectedBones+i*2,big)==actual.boneMapping[i],"original bone map differs");
            comparedBytes+=2;
        }
        unsigned char chunk[24];std::memset(chunk,0,sizeof(chunk));
        MethodInfo^ finalize=Resolve(a,0x06000038,4);
        MethodInfo^ destroyChunk=Resolve(a,0x06000035,1);
        // Caller pops the root before finalizing, as Plugin.HandleMesh does.
        std::uint32_t stackEnd;std::memcpy(&stackEnd,tracker+8,4);stackEnd-=4;
        std::memcpy(tracker+8,&stackEnd,4);
        array<Object^>^ finalizeArgs=gcnew array<Object^>(4);
        finalizeArgs[0]=PointerArg(finalize,0,tracker);finalizeArgs[1]=PointerArg(finalize,1,chunk);
        finalizeArgs[2]=PointerArg(finalize,2,&big);finalizeArgs[3]=static_cast<UInt32>(0);
        Invoke(finalize,finalizeArgs);
        try {
            Recovered::Relo::Chunk serialized=CompileMeshVertexData(p,platform);
            const std::vector<unsigned char>* actualBuffers[]={&serialized.instance,&serialized.relocations,&serialized.imports};
            for(int buffer=0;buffer<3;++buffer) {
                const std::uint32_t length=Read32(chunk+buffer*8+4,false);
                Check(length==actualBuffers[buffer]->size(),"original serialized mesh component buffer length differs");
                const unsigned char* expected=static_cast<const unsigned char*>(ReadPointer(chunk+buffer*8));
                for(std::size_t i=0;i<length;++i) {
                    if(expected[i]!=(*actualBuffers[buffer])[i]) {
                        Console::WriteLine("FAIL serialized mesh case={0} platform={1} buffer={2}",caseNumber,platform,buffer);
                        Console::WriteLine("byte={0}",static_cast<int>(i));
                        throw std::runtime_error("original serialized mesh component bytes differ");
                    }
                    ++comparedBytes;
                }
            }
        } finally {
            array<Object^>^ chunkArgs=gcnew array<Object^>(1);chunkArgs[0]=PointerArg(destroyChunk,0,chunk);Invoke(destroyChunk,chunkArgs);
        }
        ++fixtures;
    } finally {
        array<Object^>^ args=gcnew array<Object^>(1);args[0]=PointerArg(destroy,0,tracker);Invoke(destroy,args);
    }
}
MeshPipelineData Fixture(int pos,int normals,int tangentFlags,bool color,int textures,int bones,int seed) {
    const int count=3;
    MeshPipelineData p;
    for(int stream=0;stream<pos;++stream) {
        p.vertices.push_back(std::vector<MeshVector3>());
        for(int i=0;i<count;++i) p.vertices.back().push_back(MeshVector3{{float(i+stream)+0.25f,float(seed)-0.5f,float(-i)}});
    }
    for(int stream=0;stream<normals;++stream) {
        p.normals.push_back(std::vector<MeshVector3>());
        for(int i=0;i<count;++i) p.normals.back().push_back(MeshVector3{{0.25f*float(i-1),0.5f,float((stream+seed)%2)}});
    }
    if(tangentFlags&1) p.tangents.assign(count,MeshVector3{{1,0,0}});
    if(tangentFlags&2) p.binormals.assign(count,MeshVector3{{0,1,0}});
    if(color) p.colors.assign(count,MeshVector4{{0.25f,0.5f,1,-1}});
    for(int stream=0;stream<textures;++stream) p.texCoords.push_back(std::vector<MeshVector2>(count,MeshVector2{{float(stream)*0.5f,0.25f*seed}}));
    const unsigned short boneIds[]={0,9,2,65535,300,9};
    for(int stream=0;stream<bones;++stream) {
        p.boneInfluences.push_back(std::vector<MeshBoneInfluence>());
        for(int i=0;i<count;++i) p.boneInfluences.back().push_back(MeshBoneInfluence{float(i+1)*0.125f,boneIds[(stream*3+i+seed)%6],0});
    }
    return p;
}
}
int main(array<String^>^ args) {
    try {
        if(IntPtr::Size!=4 || args->Length!=1) throw gcnew Exception("Usage: x86 W3xMeshBuilderOracle original.dll");
        Assembly^ original=Assembly::LoadFrom(args[0]);
        int caseNumber=0;
        for(int pos=1;pos<=2;++pos) for(int normal=0;normal<=2;++normal)
        for(int tangent=0;tangent<4;++tangent) for(int color=0;color<2;++color)
        for(int textures=0;textures<3;++textures) for(int bones=0;bones<=2;++bones) {
            const int texCount=textures==2?3:textures;
            MeshPipelineData p=Fixture(pos,normal,tangent,color!=0,texCount,bones,caseNumber%3);
            for(int platform=0;platform<3;++platform) Test(original,p,platform,caseNumber);
            ++caseNumber;
        }
        Console::WriteLine("PASS W3X mesh builder original differential fixtures={0} comparedBytes={1}; original 0xCE fields and isolated 28-byte vertex component three buffers; full HandleMesh and SDK not accepted",fixtures,comparedBytes);
        return 0;
    } catch(Exception^ e) {Console::Error->WriteLine(e);return 1;}
      catch(const std::exception& e) {Console::Error->WriteLine(gcnew String(e.what()));return 1;}
}
