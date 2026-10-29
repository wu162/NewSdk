#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#include <Windows.h>
#include <cstring>
#include <iostream>
int main(){
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 unsigned char source[4]={1,2,3,4},destination[4]={};
 if(TextureCopySurface(destination,4,1,1,0x1a200006,nullptr,source,4,0x1a200032,nullptr,0,1)!=0||std::memcmp(source,destination,4)!=0)return 2;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 3;
 std::cout<<"PASS native conversion import-library consumer: stdcall row copy and correct DLL identity; no original compiler\n";return 0;
}
