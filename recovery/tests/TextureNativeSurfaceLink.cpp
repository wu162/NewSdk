#include "../../src/TextureCompiler/Native/TextureNativeSurfaceApi.h"
#include <Windows.h>
#include <array>
#include <iostream>
int main(){
 using namespace Recovered::TextureCompiler::NativeSurfaceAbi;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 1;
 std::array<std::uint32_t,16> p{};p[0]=3;p[1]=1;p[7]=1<<22;p[8]=0x10000006;p[9]=3|(3<<13);p[12]=0x20000200;
 std::array<std::uint32_t,7> layout{};void* address=nullptr;
 TextureSurfaceGetLayout(p.data(),0,0,&layout[0],&layout[1],&layout[2],&layout[3],&layout[4],&layout[5],&layout[6],&address);
 LockedRect out{};
 if(TextureSurfaceLockRect(p.data(),0,&out,nullptr,0)!=0||out.bits!=address||static_cast<std::uint32_t>(out.pitch)!=layout[0])return 2;
 if(GetModuleHandleA("BinaryAssetBuilder.TextureCompiler.dll"))return 3;
 std::cout<<"PASS native PC surface import-library consumer: stdcall layout/LockRect and correct DLL identity; no original compiler\n";return 0;
}
