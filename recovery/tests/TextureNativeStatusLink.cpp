#include "../../src/TextureCompiler/Native/TextureNativeApi.h"
#include <Windows.h>
#include <cstring>
#include <iostream>
int main(){
    if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")!=nullptr)return 1;
    if(std::strcmp(TextureGetErrorString(0),"OK                       ")!=0)return 2;
    if(std::strcmp(TextureGetErrorString(-101),"ERROR NOT DEFINED")!=0)return 3;
    if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll")!=nullptr)return 4;
    std::cout<<"PASS native status import-library consumer: correct runtime DLL name and C ABI; no original compiler\n";
    return 0;
}
