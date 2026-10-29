#pragma once
#include <cstdint>
#ifdef RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API_BUILD
#define RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API __declspec(dllexport)
#else
#define RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API __declspec(dllimport)
#endif
namespace Recovered { namespace TextureCompiler { namespace NativeDescriptorAbi {
struct LevelDesc{std::uint32_t format,type,usage,pool,multiSampleType,multiSampleQuality,width,height;};
struct VolumeDesc{std::uint32_t format,type,usage,pool,width,height,depth;};
struct TextureDesc{std::uint32_t type,width,height,depth,format,rowPitch,slicePitch,bitsPerPixel,widthInBlocks,heightInBlocks,depthInBlocks,bytesPerBlock,exponentBias,flags,multiSampleType;};
using LevelFn=void (__stdcall *)(const void*,std::uint32_t,LevelDesc*);
using VolumeFn=void (__stdcall *)(const void*,std::uint32_t,VolumeDesc*);
using TextureFn=void (__stdcall *)(const void*,std::uint32_t,TextureDesc*);
}}}
// Original CPU texture resource type3, including 1D/2D/volume/array/cube headers.
// These are native pointer descriptors, not a mixed-mode CLR identity facade.
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureGetLevelDesc(const void*,std::uint32_t,Recovered::TextureCompiler::NativeDescriptorAbi::LevelDesc*);
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureGetVolumeLevelDesc(const void*,std::uint32_t,Recovered::TextureCompiler::NativeDescriptorAbi::VolumeDesc*);
extern "C" RECOVERED_TEXTURE_NATIVE_DESCRIPTOR_API void __stdcall TextureXGGetTextureDesc(const void*,std::uint32_t,Recovered::TextureCompiler::NativeDescriptorAbi::TextureDesc*);
