// Standalone PC adapter to the pinned scalar S3 source. No platform SDK types.
#include "S3_quant.h"
#include "alpha.h"
#include <algorithm>
#include <cstring>
void SetLegacyS3SearchBudget(int policy);
int EncodePublicS3(int variant,int format,const unsigned char* rgba,unsigned char* output)
{
    if(variant<0||variant>15||format<2||format>3)return 0;
    SetLegacyS3SearchBudget(variant/4);
    RGBBlock b={};b.n=16;b.inLevel=4;
    double weights[3]={0.3086,0.6094,0.082};
    for(int c=0;c<3;c++)b.weight[c]=(variant&2)?weights[c]:(double)(float)weights[c];
    for(int i=0;i<16;i++)for(int c=0;c<3;c++)b.colorChannel[i][c]=(variant&1)?rgba[i*4+c]/255.0:(double)(float)(rgba[i*4+c]/255.0);
    CodeRGBBlock(&b);if(b.outLevel!=4)return 0;
    unsigned int c0=(b.endPoint[0][0]<<11)|(b.endPoint[0][1]<<5)|b.endPoint[0][2];
    unsigned int c1=(b.endPoint[1][0]<<11)|(b.endPoint[1][1]<<5)|b.endPoint[1][2];
    if(c0==c1){c0=(c0+1)&65535;for(int i=0;i<16;i++)b.index[i]=1;}
    bool swap=c0<c1;if(swap)std::swap(c0,c1);
    output[8]=(unsigned char)c0;output[9]=(unsigned char)(c0>>8);output[10]=(unsigned char)c1;output[11]=(unsigned char)(c1>>8);
    unsigned int bits=0;for(int i=0;i<16;i++)bits|=(unsigned int)(b.index[i]^(swap?1:0))<<(i*2);
    for(int i=0;i<4;i++)output[12+i]=(unsigned char)(bits>>(8*i));
    if(format==2)squish::CompressAlphaDxt3(rgba,65535,output);
    else{
        AlphaBlock a={};a.n=16;for(int i=0;i<16;i++)a.alpha[i]=(float)(rgba[i*4+3]/255.0);
        CodeAlphaBlock(&a);
        int low=a.endPoint[0],high=a.endPoint[1],levels=a.outLevel;
        if(low==high){high++;levels=6;memset(a.index,0,sizeof(a.index));}
        bool alphaSwap=(low>high)==(levels==6);if(alphaSwap)std::swap(low,high);
        output[0]=(unsigned char)std::max(0,std::min(255,low));output[1]=(unsigned char)std::max(0,std::min(255,high));
        unsigned __int64 mask=0;for(int i=0;i<16;i++){int index=a.index[i];if(alphaSwap){if(levels==8)index=index>1?9-index:1-index;else if(index<6)index=index>1?7-index:1-index;}mask|=(unsigned __int64)index<<(i*3);}
        for(int i=0;i<6;i++)output[i+2]=(unsigned char)(mask>>(8*i));
    }
    return 16;
}
