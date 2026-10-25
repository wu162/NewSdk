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
void Rejects() {
    ConverterContext c;
    for (int k=0;k<8;++k) ConvertMeshStream(static_cast<MeshConversion>(k),0,0,0,0,0,c);
    bool threw=false;try { ConvertMeshStream(MeshConversion::CopyTexCoord,-1,0,0,0,0,c); }catch(const std::invalid_argument&){threw=true;}Check(threw);
    threw=false;c.bigEndian=true;try { ConvertMeshStream(MeshConversion::CopyVector3,0,0,0,0,0,c); }catch(const std::domain_error&){threw=true;}Check(threw);
    c.bigEndian=false;float f[4]={std::numeric_limits<float>::quiet_NaN(),0,0,0};unsigned char data[16]={};
    const MeshConversion kinds[]={MeshConversion::Vector4ToUByte4N,MeshConversion::ConvertBlendWeight,MeshConversion::CompressNormalizedDirection};
    for (int i=0;i<3;++i) {threw=false;try { ConvertMeshStream(kinds[i],1,f,16,data,16,c); }catch(const std::domain_error&){threw=true;}Check(threw);}
    std::uint16_t ix=0;threw=false;try {ConvertMeshStream(MeshConversion::ConvertBlendIndex,1,&ix,2,data,1,c);}catch(const std::out_of_range&){threw=true;}Check(threw);
}
}
int main() { Golden();StridesAndEndian();Rejects();std::cout << "W3X mesh converters PASS " << checks << " assertions (local golden/stride checks; not original DLL differential)\n"; }
