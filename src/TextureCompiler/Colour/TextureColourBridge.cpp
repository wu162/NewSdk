// Bounded Win32 adapter for the restored Production=71 colour state machine.
#pragma comment(linker,"/NOENTRY")
#pragma comment(linker,"/EXPORT:TextureColorState=_TextureColorState@12")
extern "C" int _fltused=0;
extern "C" void ColorCoreAsm();
extern "C" void ColorNoModeAsm();
extern "C" void ColorErrorAsm();
extern "C" int __stdcall TextureColorState(unsigned char* block,int bytes,int mode)
{
    if(!block || bytes!=0x3850 || (mode!=0 && mode!=3 && mode!=4))return 1;
    int* header=(int*)block;
    if(header[0]<0 || header[0]>3 || header[1]<0 || header[1]>2 || header[2]<1 || header[2]>512)return 2;
    if(*(int*)(block+0x3030)!=71)return 3;
    double* samples=(double*)(block+0x10);
    for(int i=0;i<header[2]*3;i++) if(samples[i]!=samples[i] || samples[i]<0 || samples[i]>1)return 4;
    double* weights=(double*)(block+0x3018);
    for(int i=0;i<3;i++) if(weights[i]!=weights[i] || weights[i]<=0 || weights[i]>1)return 5;
    unsigned char owner[32]={0};
    unsigned char options[1644]={0};
    *(int*)options=71;
    *(void**)(owner+0x18)=options;
    __asm {
        lea ecx,owner
        mov eax,block
        mov edx,mode
        test edx,edx
        jnz fixedMode
        push eax
        call ColorNoModeAsm
        jmp complete
    fixedMode:
        push edx
        push eax
        call ColorCoreAsm
    complete:
    }
    return 0;
}
