#include "../../src/TextureCompiler/Native/TextureNativeCopyApi.h"
#include <Windows.h>
#include <cstring>
#include <iostream>
int main(){
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 unsigned char source[4]={0,0,128,63},destination[4]={};
 if(TextureCopySurface(destination,2,1,1,0x1a20001b,nullptr,source,4,0x1a200024,nullptr,0,1)!=0||destination[0]!=0||destination[1]!=60||destination[2]!=0||destination[3]!=0)return 2;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 3;
 std::cout<<"PASS native float conversion import-library consumer: stdcall row copy and correct DLL identity; no original compiler\n";return 0;
}
