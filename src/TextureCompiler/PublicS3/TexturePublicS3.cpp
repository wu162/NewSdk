// Win32/PC ABI for the adapted public S3 encoder; no recovered encoder callbacks.
#include "S3_quant.h"
#pragma comment(linker,"/EXPORT:TexturePublicS3Colour=_TexturePublicS3Colour@12")
#pragma comment(linker,"/EXPORT:TexturePublicS3Alpha=_TexturePublicS3Alpha@20")
void CodeRGBBlockPC(RGBBlock* block);
extern "C" __declspec(dllexport) int __stdcall TexturePublicS3Colour(unsigned char* state,int bytes,int mode) {
    if(!state||bytes!=0x3850||mode!=0)return 1;
    const int* header=(const int*)state;
    if((header[0]!=2&&header[0]!=3)||header[1]!=1||header[2]<1||header[2]>16||*(int*)(state+0x3030)!=71)return 2;
    RGBBlock block={};block.n=header[2];block.inLevel=4;
    const double* values=(const double*)(state+0x10);const double* weights=(const double*)(state+0x3018);
    for(int c=0;c<3;c++){if(weights[c]!=weights[c]||weights[c]<=0||weights[c]>1)return 3;block.weight[c]=weights[c];}
    for(int i=0;i<block.n;i++)for(int c=0;c<3;c++){double value=values[i*3+c];if(value!=value||value<0||value>1)return 4;block.colorChannel[i][c]=value;}
    CodeRGBBlockPC(&block);*(int*)(state+0x3034)=block.outLevel;
    for(int i=0;i<6;i++)((int*)(state+0x3038))[i]=block.endPoint[i/3][i%3];
    for(int i=0;i<block.n;i++)((int*)(state+0x3050))[i]=block.index[i];
    return 0;
}
extern "C" __declspec(dllexport) int __stdcall TexturePublicS3Alpha(const float* rgba,int count,int* endpoints,int* indices,int* levels) {
    if(!rgba||!endpoints||!indices||!levels||count<1||count>16)return 1;
    AlphaBlock block={};block.n=count;
    for(int i=0;i<count;i++){float value=rgba[i*4+3];if(value!=value||value<0||value>1)return 2;block.alpha[i]=value;}
    CodeAlphaBlock(&block);*levels=block.outLevel;endpoints[0]=block.endPoint[0];endpoints[1]=block.endPoint[1];
    for(int i=0;i<count;i++)indices[i]=block.index[i];return 0;
}

// Experimental bounded ABI: explicitly initialized force-three state. No route
// from production Texture/OnDemandTexture consumes this export.
void CodeRGBBlockPC3(RGBBlock* block,int mode,bool opaqueBlack);
#pragma comment(linker,"/EXPORT:TexturePublicS3ThreeColour=_TexturePublicS3ThreeColour@12")
extern "C" __declspec(dllexport) int __stdcall TexturePublicS3ThreeColour(unsigned char* state,int bytes,int mode) {
    if(!state||bytes!=0x3850||(mode!=0&&mode!=3))return 1;
    const int* header=(const int*)state;
    if((header[0]!=0&&header[0]!=1)||header[1]!=0||header[2]<1||header[2]>16||*(int*)(state+0x3030)!=71)return 2;
    RGBBlock block={};block.n=header[2];block.inLevel=3;
    const double* values=(const double*)(state+0x10);const double* weights=(const double*)(state+0x3018);
    for(int c=0;c<3;c++){if(weights[c]!=weights[c]||weights[c]<=0||weights[c]>1)return 3;block.weight[c]=weights[c];}
    for(int i=0;i<block.n;i++)for(int c=0;c<3;c++){double value=values[i*3+c];if(value!=value||value<0||value>1)return 4;block.colorChannel[i][c]=value;}
    CodeRGBBlockPC3(&block,mode,header[0]==0);*(int*)(state+0x3034)=block.outLevel;
    for(int i=0;i<6;i++)((int*)(state+0x3038))[i]=block.endPoint[i/3][i%3];
    for(int i=0;i<block.n;i++)((int*)(state+0x3050))[i]=block.index[i];return 0;
}
