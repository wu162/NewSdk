#include "../../src/TextureCompiler/Native/TextureNativeDescriptorApi.h"
#include <Windows.h>
#include <array>
#include <iostream>
int main(){
 using namespace Recovered::TextureCompiler::NativeDescriptorAbi;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 std::array<std::uint32_t,16> p{};p[0]=3;p[1]=1;p[7]=1<<22;p[8]=6;p[9]=3|(3<<13);p[12]=0x200;
 LevelDesc level{};VolumeDesc volume{};TextureDesc texture{};
 TextureGetLevelDesc(p.data(),0,&level);TextureGetVolumeLevelDesc(p.data(),0,&volume);TextureXGGetTextureDesc(p.data(),0,&texture);
 if(level.width!=4||level.height!=4||volume.width!=4||volume.height!=4||volume.depth!=1||texture.width!=4||texture.height!=4||texture.depth!=1)return 2;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 3;
 std::cout<<"PASS native descriptor import-library consumer: all three original-size output structures and correct DLL identity; no original compiler\n";return 0;
}
