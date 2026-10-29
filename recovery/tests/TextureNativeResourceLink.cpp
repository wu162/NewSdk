#include "../../src/TextureCompiler/Native/TextureNativeResourceApi.h"
#include <Windows.h>
#include <cstring>
#include <iostream>
int main(){
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 auto resource=GlobalAlloc(GMEM_ZEROINIT,64);if(!resource)return 2;
 const std::uint32_t header=5,refs=1;std::memcpy(resource,&header,4);std::memcpy(static_cast<unsigned char*>(resource)+4,&refs,4);
 if(TextureResourceAddRef(resource)!=2||TextureResourceRelease(resource)!=1||TextureResourceRelease(resource)!=0)return 3;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 4;
 std::cout<<"PASS native PC resource import-library consumer: stdcall refcount and correct DLL identity; no original compiler\n";
 return 0;
}
