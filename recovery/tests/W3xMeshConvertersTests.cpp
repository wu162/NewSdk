#include "../src/Native/W3x/MeshConverters.h"
#include <cassert>
#include <cmath>
#include <cstdint>
#include <cstring>
#include <iostream>
#include <limits>
#include <stdexcept>
#include <vector>
using namespace Recovered::W3x;
namespace {
std::size_t checks=0;
void Check(bool ok) { ++checks; if (!ok) throw std::runtime_error("W3X converter assertion"); }
float Bits(std::uint32_t b) { float v;std::memcpy(&v,&b,4);return v; }
std::uint32_t LE(const unsigned char* p) { return std::uint32_t(p[0])|(std::uint32_t(p[1])<<8)|(std::uint32_t(p[2])<<16)|(std::uint32_t(p[3])<<24); }
std::vector<unsigned char> Convert(MeshConversion k,const void* src,int count,int sp,int dp,bool be=false) {
    std::vector<unsigned char> data(count*dp+8,0xa5); ConverterContext c;c.bigEndian=be;
    ConvertMeshStream(k,count,src,sp,&data[0],dp,c);return data;
}
void Golden() {
    Check(MeshFloatToHalf(1.0f)==0x3c00);
    Check(MeshFloatToHalf(-2.0f)==0xc000);
    Check(MeshFloatToHalf(65504.0f)==0x7bff);
    Check(MeshFloatToHalf(std::numeric_limits<float>::infinity())==0x7fff);
    Check(MeshFloatToHalf(-std::numeric_limits<float>::infinity())==0xffff);
    Check(MeshFloatToHalf(Bits(0x7fc00001))==0x7fff);
    Check(MeshFloatToHalf(0.0f)==0x0000);
    Check(MeshFloatToHalf(-0.0f)==0x8000);
    Check(MeshFloatToHalf(Bits(1))==0x0000);
    Check(MeshFloatToHalf(Bits(0x08800000))==0x0400); // 2^-110; original shr.un count wraps to zero.
    Check(MeshFloatToHalf(Bits(0x33800000))==0x0001); // 2^-24.
    Check(MeshFloatToHalf(Bits(0x33000000))==0x0000); // tie to even.
    Check(MeshFloatToHalf(Bits(0x3f801000))==0x3c00); // tie to even.
    Check(MeshFloatToHalf(Bits(0x3f803000))==0x3c02);
    float uv[2]={0.25f,0.75f};
    std::vector<unsigned char> out=Convert(MeshConversion::CopyTexCoord,uv,1,8,12);
    Check(LE(&out[0])==0x3e800000);Check(LE(&out[4])==0x3e800000);
    out=Convert(MeshConversion::CompressTexCoord,uv,1,8,12);
    Check(out[0]==0&&out[1]==0x34&&out[2]==0&&out[3]==0x34);
    // Original run40165200024: finite v becomes an sNaN only AFTER byte
    // swapping into a float local, and the CLR float store quiets that sNaN.
    float swappedNaN[2]={Bits(0xa89a8620U),Bits(0x76d882ffU)};
    out=Convert(MeshConversion::CopyTexCoord,swappedNaN,1,8,12,true);
    const unsigned char swappedGolden[]={0xa8,0x9a,0x86,0x20,0xf6,0xd8,0xc2,0xff};
    Check(std::memcmp(&out[0],swappedGolden,8)==0);
    float swappedU[2]={Bits(0x76d882ffU),0.0f};
    out=Convert(MeshConversion::CopyTexCoord,swappedU,1,8,12,true);
    Check(LE(&out[0])==0xffc2d876U);
    float xyz[3]={1.0f,-2.0f,0.0f};
    out=Convert(MeshConversion::Vector3ToHalf4,xyz,1,12,12);
    const unsigned char expected[]={0,0x3c,0,0xc0,0,0,0,0};
    Check(std::memcmp(&out[0],expected,8)==0);
    float rgba[4]={-1.0f,0.5f,1.0f,2.0f};
    out=Convert(MeshConversion::Vector4ToUByte4N,rgba,1,16,8);
    Check(LE(&out[0])==0xffff8000U);
    float weights[]={0.0f,0.5f,1.0f,2.0f,-1.0f};
    out=Convert(MeshConversion::ConvertBlendWeight,weights,5,4,4);
    const unsigned char bytes[]={0,128,255,254,2};
    for (int i=0;i<5;++i) Check(out[i*4]==bytes[i]);
    float normal[3]={1.0f,-1.0f,1.0f};
    out=Convert(MeshConversion::CompressNormalizedDirection,normal,1,12,8);
    Check(LE(&out[0])==0x7fe00bffU);
    // Color clipping and signed normal clipping intentionally differ.
    float infinityColor[4]={-std::numeric_limits<float>::infinity(),std::numeric_limits<float>::infinity(),0,1};
    out=Convert(MeshConversion::Vector4ToUByte4N,infinityColor,1,16,8);
    Check(LE(&out[0])==0xff00ff00U);
}
void StridesAndEndian() {
    float f[4]={0.3f,-0.3f,0.7f,0.9f};
    const MeshConversion kinds[]={MeshConversion::CopyTexCoord,MeshConversion::CompressTexCoord,MeshConversion::Vector4ToUByte4N,
        MeshConversion::Vector3ToHalf4,MeshConversion::ConvertBlendWeight,MeshConversion::CompressNormalizedDirection};
    const int widths[]={8,4,4,8,1,4};
    const int units[]={4,2,4,2,1,4};
    for (int k=0;k<6;++k) {
        std::vector<unsigned char> a=Convert(kinds[k],f,3,0,16,false);
        std::vector<unsigned char> b=Convert(kinds[k],f,3,0,16,true);
        for (int row=0;row<3;++row) {
            for (int i=0;i<widths[k];++i) {
                Check(a[row*16+i]==a[i]);
                const int j=(i/units[k])*units[k]+units[k]-1-(i%units[k]);
                Check(b[row*16+i]==a[row*16+j]);
            }
            for(int i=widths[k];i<16;++i) Check(a[row*16+i]==0xa5&&b[row*16+i]==0xa5);
        }
        for (int i=48;i<56;++i) Check(a[i]==0xa5&&b[i]==0xa5);
    }
    // Negative pitch traverses valid in-allocation source rows backwards.
    float uvRows[3][2]={{0,0},{1,1},{0.25f,0.75f}};
    std::vector<unsigned char> neg=Convert(MeshConversion::CopyTexCoord,uvRows[2],3,-8,8);
    Check(LE(&neg[0])==0x3e800000&&LE(&neg[8])==0x3f800000&&LE(&neg[16])==0);
    ConverterContext c;
    const std::uint16_t map[]={258,0,255};c.inverseBoneMapping=map;c.inverseBoneMappingCount=3;
    std::uint16_t ix[]={0,2,1};unsigned char dst[6]={0,0xa5,0,0xa5,0,0xa5};
    ConvertMeshStream(MeshConversion::ConvertBlendIndex,3,ix,2,dst,2,c);
    Check(dst[0]==2&&dst[2]==255&&dst[4]==0&&dst[1]==0xa5&&dst[3]==0xa5&&dst[5]==0xa5);
    c.bigEndian=true;unsigned char reversed[3]={};
    ConvertMeshStream(MeshConversion::ConvertBlendIndex,3,ix,2,reversed,1,c);
    Check(reversed[0]==2&&reversed[1]==255&&reversed[2]==0);
    // CopyDirect must preserve signed zero, NaN payload, and subnormal bits.
    std::uint32_t src[3]={0x80000000,0x7fc01234,1};std::uint32_t raw[3]={};c.bigEndian=false;
    ConvertMeshStream(MeshConversion::CopyVector3,1,src,12,raw,12,c);
    Check(std::memcmp(src,raw,12)==0);
}
void FullNumericDomain() {
    // Immutable original CLR4 x86 results from 40166287709-1's 548
    // BlendWeight probes, deduplicated by source float bit pattern.
    static const std::uint32_t originalWeights[][2]={
        {0x4c35c678U,0x20U},
        {0x4c6669fcU,0x10U},
        {0x4cb36368U,0xc0U},
        {0x4cceec88U,0xc0U},
        {0x4cf08fe8U,0xc0U},
        {0x4d40062cU,0x40U},
        {0x4dad9c10U,0x00U},
        {0x4e0a6214U,0x00U},
        {0x4e773a1cU,0x00U},
        {0x4ea1a8dcU,0x00U},
        {0x4eb9bef0U,0x00U},
        {0x4ebdab20U,0x00U},
        {0x4f2e583cU,0x00U},
        {0x506b8a64U,0x00U},
        {0x50757550U,0x00U},
        {0x50f852fcU,0x00U},
        {0x511aa4bcU,0x00U},
        {0x51b6ecd8U,0x00U},
        {0x51bc624cU,0x00U},
        {0x521e9d88U,0x00U},
        {0x52d38dbcU,0x00U},
        {0x5349d934U,0x00U},
        {0x53fbac74U,0x00U},
        {0x5447f6ccU,0x00U},
        {0x54c501e0U,0x00U},
        {0x54d864ecU,0x00U},
        {0x554dd0e4U,0x00U},
        {0x56c85890U,0x00U},
        {0x572d2450U,0x00U},
        {0x578ae6c0U,0x00U},
        {0x57c674b0U,0x00U},
        {0x583afc98U,0x00U},
        {0x5957de60U,0x00U},
        {0x5a26b664U,0x00U},
        {0x5a8b9cc0U,0x00U},
        {0x5baeee9cU,0x00U},
        {0x5bee042cU,0x00U},
        {0x5c05e780U,0x00U},
        {0x5cfff520U,0x00U},
        {0x5d5e737cU,0x00U},
        {0x5eb00c70U,0x00U},
        {0x5eeb3ffcU,0x00U},
        {0x5f65374cU,0x00U},
        {0x5fc8a540U,0x00U},
        {0x5fd01554U,0x00U},
        {0x606b9460U,0x00U},
        {0x612a96d0U,0x00U},
        {0x61ddb0acU,0x00U},
        {0x623d052cU,0x00U},
        {0x62687688U,0x00U},
        {0x62a20da4U,0x00U},
        {0x62bc355cU,0x00U},
        {0x62beddd8U,0x00U},
        {0x641c3728U,0x00U},
        {0x651f1a20U,0x00U},
        {0x65fc0c30U,0x00U},
        {0x66e4a9c8U,0x00U},
        {0x66f8afacU,0x00U},
        {0x676336e8U,0x00U},
        {0x67878164U,0x00U},
        {0x679d45f4U,0x00U},
        {0x690b745cU,0x00U},
        {0x692dcdccU,0x00U},
        {0x69cb7390U,0x00U},
        {0x6a24ba04U,0x00U},
        {0x6a701510U,0x00U},
        {0x6aee0c48U,0x00U},
        {0x6af4e7d0U,0x00U},
        {0x6b7f47c8U,0x00U},
        {0x6ba28c8cU,0x00U},
        {0x6bf12054U,0x00U},
        {0x6c42f8b8U,0x00U},
        {0x6c9804e0U,0x00U},
        {0x6d37b960U,0x00U},
        {0x6d4667b4U,0x00U},
        {0x6d4faab0U,0x00U},
        {0x6db677a4U,0x00U},
        {0x6e286d14U,0x00U},
        {0x6e4002c4U,0x00U},
        {0x6f4ddd30U,0x00U},
        {0x6f7b0df8U,0x00U},
        {0x6f817c28U,0x00U},
        {0x6fda3a6cU,0x00U},
        {0x7034d020U,0x00U},
        {0x70805714U,0x00U},
        {0x70e10fecU,0x00U},
        {0x713c69dcU,0x00U},
        {0x7233dd44U,0x00U},
        {0x724d0698U,0x00U},
        {0x724e3b6cU,0x00U},
        {0x738b51a0U,0x00U},
        {0x746d6644U,0x00U},
        {0x74cc0360U,0x00U},
        {0x75f0ef34U,0x00U},
        {0x76089f24U,0x00U},
        {0x76751dd0U,0x00U},
        {0x76b3641cU,0x00U},
        {0x76bac128U,0x00U},
        {0x776ee548U,0x00U},
        {0x77e4043cU,0x00U},
        {0x780792f8U,0x00U},
        {0x780b5380U,0x00U},
        {0x78a507a0U,0x00U},
        {0x78de03f8U,0x00U},
        {0x78e3b9ecU,0x00U},
        {0x793a368cU,0x00U},
        {0x79612e3cU,0x00U},
        {0x79b04d60U,0x00U},
        {0x7b000a68U,0x00U},
        {0x7b1693dcU,0x00U},
        {0x7b49ff54U,0x00U},
        {0x7b940b98U,0x00U},
        {0x7bf35908U,0x00U},
        {0x7c31cfa8U,0x00U},
        {0x7cfdbaecU,0x00U},
        {0x7d2bf584U,0x00U},
        {0x7d89220cU,0x00U},
        {0x7dbd2d9cU,0x00U},
        {0x7ec36f3cU,0x00U},
        {0x7ed8d350U,0x00U},
        {0x7f095a50U,0x00U},
        {0x7f0eb35cU,0x00U},
        {0x7f240b8cU,0x00U},
        {0x7f7fffffU,0x00U},
        {0x7f800000U,0x00U},
        {0x7fc00000U,0x00U},
        {0x7fc00001U,0x00U},
        {0x7fc12345U,0x00U},
        {0x7fed2048U,0x00U},
        {0xcbca7db8U,0x71U},
        {0xcc50de7cU,0xf1U},
        {0xccc02788U,0x41U},
        {0xcccf2328U,0x41U},
        {0xccdc3ae4U,0x21U},
        {0xccece6f4U,0xa1U},
        {0xcd4595e0U,0x01U},
        {0xcdb73e24U,0x81U},
        {0xce0a8100U,0x01U},
        {0xce2ef294U,0x01U},
        {0xce75d8a4U,0x01U},
        {0xcf10f4f0U,0x01U},
        {0xcf5459a8U,0x01U},
        {0xd18d04a4U,0x01U},
        {0xd1a53834U,0x01U},
        {0xd1ecf8bcU,0x01U},
        {0xd22805acU,0x01U},
        {0xd2536434U,0x01U},
        {0xd26242a4U,0x01U},
        {0xd2815938U,0x01U},
        {0xd428e0b0U,0x01U},
        {0xd44a9244U,0x01U},
        {0xd4b81224U,0x01U},
        {0xd4e54d0cU,0x01U},
        {0xd4ead6c4U,0x01U},
        {0xd5a4a2ccU,0x00U},
        {0xd5d6de54U,0x00U},
        {0xd71f3940U,0x00U},
        {0xd793ae30U,0x00U},
        {0xd7be2290U,0x00U},
        {0xd885e604U,0x00U},
        {0xd8923f20U,0x00U},
        {0xd8afff18U,0x00U},
        {0xd8bcca38U,0x00U},
        {0xd93c1eb4U,0x00U},
        {0xd9b840c4U,0x00U},
        {0xda894230U,0x00U},
        {0xdb463384U,0x00U},
        {0xdb8cb7c4U,0x00U},
        {0xdc2afd94U,0x00U},
        {0xdc5ec49cU,0x00U},
        {0xde23d0f4U,0x00U},
        {0xdf7c5564U,0x00U},
        {0xdfaed424U,0x00U},
        {0xdfc288f8U,0x00U},
        {0xe0359050U,0x00U},
        {0xe07afa18U,0x00U},
        {0xe1f2dc94U,0x00U},
        {0xe2253b70U,0x00U},
        {0xe2f36f60U,0x00U},
        {0xe319f518U,0x00U},
        {0xe3d914a8U,0x00U},
        {0xe4bb0894U,0x00U},
        {0xe590f9f8U,0x00U},
        {0xe7590cb8U,0x00U},
        {0xe7fbbdc8U,0x00U},
        {0xe86d4a5cU,0x00U},
        {0xe8a0ee50U,0x00U},
        {0xe8a6b84cU,0x00U},
        {0xe8eebf64U,0x00U},
        {0xe9811e68U,0x00U},
        {0xe98d0a08U,0x00U},
        {0xea7c93b4U,0x00U},
        {0xeacd7ef8U,0x00U},
        {0xeb3f8e1cU,0x00U},
        {0xeb54d670U,0x00U},
        {0xebf6ace8U,0x00U},
        {0xecc62234U,0x00U},
        {0xed39e34cU,0x00U},
        {0xedece56cU,0x00U},
        {0xee874dc4U,0x00U},
        {0xf113e954U,0x00U},
        {0xf1fb52f0U,0x00U},
        {0xf2197324U,0x00U},
        {0xf253df74U,0x00U},
        {0xf28b101cU,0x00U},
        {0xf34e6058U,0x00U},
        {0xf44cbdfcU,0x00U},
        {0xf457497cU,0x00U},
        {0xf69f7edcU,0x00U},
        {0xf6ff6610U,0x00U},
        {0xf744f9d4U,0x00U},
        {0xf780087cU,0x00U},
        {0xf840f70cU,0x00U},
        {0xf85f45a8U,0x00U},
        {0xf8c4b1d0U,0x00U},
        {0xf8d96ea4U,0x00U},
        {0xf9960918U,0x00U},
        {0xfa3091b8U,0x00U},
        {0xfb5f0504U,0x00U},
        {0xfbc19ba0U,0x00U},
        {0xfc0e1ad4U,0x00U},
        {0xfc2770e0U,0x00U},
        {0xfc8dde28U,0x00U},
        {0xfd423778U,0x00U},
        {0xfd844b78U,0x00U},
        {0xfe3193fcU,0x00U},
        {0xff297abcU,0x00U},
        {0xff37c85cU,0x00U},
        {0xff520088U,0x00U},
        {0xff7fffffU,0x00U},
        {0xff800000U,0x00U},
        {0xffc00000U,0x00U},
        {0xffd18074U,0x00U},
        {0xffe6d078U,0x00U},
    };
    ConverterContext context;
    unsigned char output[16];
    for(std::size_t i=0;i<sizeof(originalWeights)/sizeof(originalWeights[0]);++i) {
        float v=Bits(originalWeights[i][0]);
        ConvertMeshStream(MeshConversion::ConvertBlendWeight,1,&v,4,output,1,context);
        Check(output[0]==originalWeights[i][1]);
    }
    const std::uint32_t nanBits[]={0x7f800001U,0xff800001U,0x7fc00000U,0xffc12345U};
    for(unsigned i=0;i<4;++i) {
        float values[4]={Bits(nanBits[i]),Bits(nanBits[i]),Bits(nanBits[i]),Bits(nanBits[i])};
        for(int be=0;be<2;++be) {
            context.bigEndian=be!=0;
            ConvertMeshStream(MeshConversion::Vector4ToUByte4N,1,values,16,output,16,context);
            Check(LE(output)==0);
            ConvertMeshStream(MeshConversion::CompressNormalizedDirection,1,values,16,output,16,context);
            Check(LE(output)==0);
            ConvertMeshStream(MeshConversion::ConvertBlendWeight,1,values,16,output,16,context);
            Check(output[0]==0);
        }
    }
}
void Rejects() {
    ConverterContext c;
    for (int k=0;k<8;++k) ConvertMeshStream(static_cast<MeshConversion>(k),0,0,0,0,0,c);
    bool threw=false;try { ConvertMeshStream(MeshConversion::CopyTexCoord,-1,0,0,0,0,c); }catch(const std::invalid_argument&){threw=true;}Check(threw);
    threw=false;c.bigEndian=true;try { ConvertMeshStream(MeshConversion::CopyVector3,0,0,0,0,0,c); }catch(const std::domain_error&){threw=true;}Check(threw);
    unsigned char data[16]={};
    std::uint16_t ix=0;threw=false;try {ConvertMeshStream(MeshConversion::ConvertBlendIndex,1,&ix,2,data,1,c);}catch(const std::out_of_range&){threw=true;}Check(threw);
}
}
int main() { Golden();StridesAndEndian();FullNumericDomain();Rejects();std::cout << "W3X mesh converters PASS " << checks << " assertions (local golden/stride checks; not original DLL differential)\n"; }
