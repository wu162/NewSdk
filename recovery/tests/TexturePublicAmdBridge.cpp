// Adapter to unmodified ATI/AMD CompRGBBlock/CompBlock1X. Legacy CPU branch, float ABI.
#include "common.h"
#include "codec.h"
#include "compressonatorxcodec.h"
#include "alpha.h"
#include <algorithm>
int EncodePublicAmdImpl(int variant,int format,const unsigned char* rgba,unsigned char* output,bool patched)
{
    if(variant<0||variant>=12||format<2||format>3)return 0;
    static_assert(sizeof(CMP_DWORD)==4,"Win32 DWORD required");
    CMP_DWORD pixels[16],packed[2]={0,0};CMP_BYTE ends[3][2],indices[16];
    float weights[3]={0.3086f,0.6094f,0.082f};
    for(int i=0;i<16;i++)pixels[i]=((CMP_DWORD)rgba[i*4+3]<<24)|((CMP_DWORD)rgba[i*4]<<16)|((CMP_DWORD)rgba[i*4+1]<<8)|rgba[i*4+2];
    float error;
    if(patched){
        float normalized[64];for(int i=0;i<64;i++)normalized[i]=(float)(rgba[i]/255.0);
        error=CompRGBBlock(normalized,packed,16,5,6,5,ends,indices,4,false,(variant/3)%2!=0,(CMP_BYTE)(variant%3),variant/6?weights:0,false,0.5f);
    }else error=CompRGBBlock(pixels,packed,16,5,6,5,ends,indices,4,false,(variant/3)%2!=0,(CMP_BYTE)(variant%3),variant/6?weights:0,false,128);
    if(error==FLT_MAX)return 0;
    unsigned int c0=((ends[2][0]>>3)<<11)|((ends[1][0]>>2)<<5)|(ends[0][0]>>3);
    unsigned int c1=((ends[2][1]>>3)<<11)|((ends[1][1]>>2)<<5)|(ends[0][1]>>3);
    bool degenerate=patched&&c0==c1;
    if(degenerate)c0=(c0+1)&65535;
    unsigned int constantIndex=1;
    if(c0<=c1){std::swap(c0,c1);constantIndex=0;} // upstream Clstr orders its indices by this same rule.
    output[8]=(unsigned char)c0;output[9]=(unsigned char)(c0>>8);output[10]=(unsigned char)c1;output[11]=(unsigned char)(c1>>8);
    unsigned int bits=0;for(int i=0;i<16;i++)bits|=(unsigned int)(degenerate?constantIndex:indices[i])<<(i*2);
    for(int i=0;i<4;i++)output[12+i]=(unsigned char)(bits>>(i*8));
    if(format==2)squish::CompressAlphaDxt3(rgba,65535,output);
    else if(patched){CMP_BYTE alpha[16];for(int i=0;i<16;i++)alpha[i]=rgba[i*4+3];CompBlockLegacyPC(alpha,output);}
    else{
        CMP_BYTE alpha[16],aEnds[2][2],aIndices[2][16];for(int i=0;i<16;i++)alpha[i]=rgba[i*4+3];
        float e8=CompBlock1X(alpha,16,aEnds[0],aIndices[0],8,false,false,8,0,true);
        float e6=e8==0?FLT_MAX:CompBlock1X(alpha,16,aEnds[1],aIndices[1],6,true,false,8,0,true);
        int chosen=e8<=e6?0:1;output[0]=aEnds[chosen][0];output[1]=aEnds[chosen][1];
        unsigned __int64 mask=0;for(int i=0;i<16;i++)mask|=(unsigned __int64)aIndices[chosen][i]<<(i*3);
        for(int i=0;i<6;i++)output[2+i]=(unsigned char)(mask>>(i*8));
    }
    return 16;
}

int EncodePublicAmd(int variant,int format,const unsigned char* rgba,unsigned char* output){return EncodePublicAmdImpl(variant,format,rgba,output,false);}
int EncodePublicAmdPatched(int variant,int format,const unsigned char* rgba,unsigned char* output){return EncodePublicAmdImpl(variant,format,rgba,output,true);}
