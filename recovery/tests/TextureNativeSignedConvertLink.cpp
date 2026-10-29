#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#include <Windows.h>
#include <cstring>
#include <iostream>
int main(){
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 unsigned char source[4]={128,0,127,255},destination[4]={};
 if(TextureCopySurface(destination,4,1,1,0x1a200006,nullptr,source,4,0x1a20aa06,nullptr,0,1)!=0||destination[0]!=0||destination[1]!=0||destination[2]!=255||destination[3]!=0)return 2;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 3;
 std::cout<<"PASS native signed conversion import-library consumer: stdcall row copy and correct DLL identity; no original compiler\n";return 0;
}
