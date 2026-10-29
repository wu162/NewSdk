#include "../../src/TextureCompiler/Native/TextureNativeDeviceApi.h"
#include <Windows.h>
#include <iostream>
int main(){
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 void* device=nullptr;
 if(TextureCreateDevice(0,1,nullptr,0,nullptr,&device)!=0||!device)return 2;
 if(TextureDeviceRelease(device)!=0)return 3;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 4;
 std::cout<<"PASS native PC device import-library consumer: stdcall factory/release and correct DLL identity; no original compiler\n";
 return 0;
}
