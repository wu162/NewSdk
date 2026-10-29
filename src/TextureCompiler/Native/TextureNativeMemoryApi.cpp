#include "TextureSupport.h"
#include <cstdlib>
using Recovered::TextureCompiler::SimpleBuffer;
static_assert(sizeof(SimpleBuffer)==12,"Native memory ABI is x86 only");
extern "C" __declspec(noinline) SimpleBuffer* BufferConstructImpl(SimpleBuffer* self,std::uint32_t capacity){return Recovered::TextureCompiler::ConstructBuffer(self,capacity);}
extern "C" __declspec(noinline) void BufferResetImpl(SimpleBuffer* self){Recovered::TextureCompiler::ResetBuffer(self);}
extern "C" __declspec(noinline) void BufferAppendImpl(SimpleBuffer* self,const void* data,std::uint32_t count){Recovered::TextureCompiler::AppendBuffer(self,data,count);}
extern "C" __declspec(noinline) SimpleBuffer* BufferDeleteImpl(SimpleBuffer* self,std::uint32_t flags){Recovered::TextureCompiler::DestroyBuffer(self);if(flags&1u)std::free(self);return self;}
// Preserve ECX=this and callee stack cleanup of the original member entries.
extern "C" __declspec(naked) void* TextureBufferConstruct(){
    __asm {
        push dword ptr[esp+4]
        push ecx
        call BufferConstructImpl
        add esp,8
        ret 4
    }
}
extern "C" __declspec(naked) void TextureBufferReset(){
    __asm {
        push ecx
        call BufferResetImpl
        add esp,4
        ret
    }
}
extern "C" __declspec(naked) void TextureBufferAppend(){
    __asm {
        push dword ptr[esp+8]
        push dword ptr[esp+8]
        push ecx
        call BufferAppendImpl
        add esp,12
        ret 8
    }
}
extern "C" __declspec(naked) void* TextureBufferDelete(){
    __asm {
        push dword ptr[esp+4]
        push ecx
        call BufferDeleteImpl
        add esp,8
        ret 4
    }
}
extern "C" void* __cdecl TextureAllocate(std::uint32_t count){return std::malloc(count);}
extern "C" void __cdecl TextureFree(void* p){std::free(p);}
extern "C" std::uint32_t __cdecl TextureReverseEndianness(std::uint32_t value){return Recovered::TextureCompiler::ReverseEndianness(value);}
extern "C" void* __stdcall TextureDirect3DCreate9(std::uint32_t version){return Recovered::TextureCompiler::Direct3DCreate9(version);}
extern "C" std::uint32_t __cdecl TextureDirect3DRelease(){return Recovered::TextureCompiler::Direct3DRelease();}
