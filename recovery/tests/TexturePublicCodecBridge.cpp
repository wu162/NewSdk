// Experimental adapters to pinned public sources; compatibility edits are listed in PATCHES.md.
#include <cstring>
#include "squish.h"
#include "alpha.h"
#define STB_DXT_IMPLEMENTATION
#include "stb_dxt.h"
#define STB_IMAGE_IMPLEMENTATION
#define STBI_NO_STDIO
#include "stb_image.h"
#define ICBC_IMPLEMENTATION
#define ICBC_SIMD 0
#include "icbc.h"

int EncodePublicS3(int variant,int format,const unsigned char* rgba,unsigned char* output);
int EncodePublicAmd(int variant,int format,const unsigned char* rgba,unsigned char* output);
int EncodePublicAmdPatched(int variant,int format,const unsigned char* rgba,unsigned char* output);
extern "C" __declspec(dllexport) int __cdecl PublicDxtEncode(int backend,int variant,int format,const unsigned char* rgba,unsigned char* output)
{
    if(!rgba||!output||format<0||format>3)return 0;
    std::memset(output,0,16);
    float metric[3]={0.3086f,0.6094f,0.082f};
    if(backend==0){
        if(variant<0||variant>=12)return 0;
        const int fits[3]={squish::kColourRangeFit,squish::kColourClusterFit,squish::kColourIterativeClusterFit};
        int flags=(format<2?squish::kDxt1:format==2?squish::kDxt3:squish::kDxt5)|fits[variant%3];
        if(variant/6)flags|=squish::kWeightColourByAlpha;
        squish::CompressMasked(rgba,65535,output,flags,(variant/3)%2?metric:0);
    }else if(backend==1){
        if(variant<0||variant>1||format==1)return 0; // stb has no BC1 punch-through-alpha API.
        int mode=variant?STB_DXT_HIGHQUAL:STB_DXT_NORMAL;
        if(format==2){squish::CompressAlphaDxt3(rgba,65535,output);stb_compress_dxt_block(output+8,rgba,0,mode);}
        else stb_compress_dxt_block(output,rgba,format==3,mode);
    }else if(backend==2){
        if(variant<0||variant>=18||format==1)return 0;
        const icbc::Quality qualities[3]={icbc::Quality_Fast,icbc::Quality_Default,icbc::Quality_Max};
        static int initializedDecoder=-1;int decoder=(variant/3)%3;
        if(initializedDecoder!=decoder){icbc::init_dxt1((icbc::Decoder)decoder);initializedDecoder=decoder;}
        float colors[64],weights[16],uniform[3]={1,1,1};
        for(int i=0;i<16;i++){for(int c=0;c<4;c++)colors[i*4+c]=rgba[i*4+c]/255.0f;weights[i]=1;}
        if(format==2)squish::CompressAlphaDxt3(rgba,65535,output);
        if(format==3)squish::CompressAlphaDxt5(rgba,65535,output);
        icbc::compress_dxt1(qualities[variant%3],colors,weights,variant/9?metric:uniform,format==0,false,output+(format<2?0:8));
    }else if(backend==3)return EncodePublicAmd(variant,format,rgba,output);
    else if(backend==5)return EncodePublicS3(variant,format,rgba,output);
    else if(backend==4)return EncodePublicAmdPatched(variant,format,rgba,output);
    else return 0;
    return format<2?8:16;
}
extern "C" __declspec(dllexport) int __cdecl PublicImageDecode(const unsigned char* input,int bytes,unsigned char* output,int capacity,int* width,int* height,int* channels)
{
    if(!input||bytes<1||!width||!height||!channels)return 0;
    if(!stbi_info_from_memory(input,bytes,width,height,channels))return 0;
    if(*width<=0||*height<=0||*width>8192||*height>8192||*width* *height>=16777216)return 0;
    if(!output)return 1;
    if(capacity!=*width* *height*4)return 0;
    unsigned char* rgba=stbi_load_from_memory(input,bytes,width,height,channels,4);
    if(!rgba)return 0;
    std::memcpy(output,rgba,capacity);stbi_image_free(rgba);return 1;
}
