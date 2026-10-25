// Test-only x86 original whole signal-wrapper I/O capture. No product codec implementation.
#include <stdint.h>
#include <string.h>
#include <intrin.h>
#pragma intrinsic(_InterlockedIncrement)
static const unsigned Capacity=256;
#pragma pack(push,4)
struct SignalSnapshot {uint32_t begin,arguments[3],fields[24];unsigned char window[16384],spectrum[8192],weights[1024];uint32_t end;};
#pragma pack(pop)
static SignalSnapshot records[Capacity];static volatile long count,overflow;static int active=-1;static uint32_t resumeAddress,caseId,signalTarget;
static uint32_t word(uint32_t pointer,unsigned offset){return *reinterpret_cast<const uint32_t*>(pointer+offset);}
static void Fields(uint32_t* f,const uint32_t* a){
    f[0]=a[0];f[1]=a[1];f[2]=word(a[1],0);f[3]=word(a[1],0x48);f[4]=word(a[1],0x50);f[5]=a[2];f[9]=caseId;
    f[10]=uint32_t(int32_t(*reinterpret_cast<const int16_t*>(f[2]+0x7c)));f[11]=uint32_t(int32_t(*reinterpret_cast<const int16_t*>(f[2]+0x7e)));
    uint32_t backend=word(a[0],0);f[12]=word(backend,0x3c);f[13]=word(backend,0x10c);f[14]=word(backend,0x110);f[15]=word(backend,0x1f0);f[16]=word(backend,0x204);
    f[17]=word(a[0],0x75d0);f[18]=word(a[0],0x18c64);f[19]=word(a[0],0x75d4);f[20]=word(a[0],0x18e24);f[21]=word(a[0],0x18e30);f[22]=uint32_t(int32_t(*reinterpret_cast<const int16_t*>(f[2]+0x78)));f[23]=word(f[2],0x94);
}
static void __cdecl Before(const uint32_t* arguments){
    if(active!=-1){overflow=1;return;}long index=_InterlockedIncrement(&count)-1;if(index<0||unsigned(index)>=Capacity){overflow=1;return;}
    active=index;SignalSnapshot& r=records[index];r.begin=0xa55a1234;r.end=0x5aa54321;r.fields[8]=1;memcpy(r.arguments,arguments,12);
    if(!arguments[0]||!arguments[1]){r.fields[8]=2;return;}Fields(r.fields,r.arguments);uint32_t n=r.fields[10];
    if(!r.fields[3]||!r.fields[4]||n<64||n>2048||(n&(n-1))||r.fields[11]!=n||(r.fields[20]&&!r.fields[21])){r.fields[8]=2;return;}
    memcpy(r.window,reinterpret_cast<const void*>(r.fields[3]),n*8);if(r.fields[20])memcpy(r.weights,reinterpret_cast<const void*>(r.fields[21]),1024);r.fields[8]=3;
}
static void __cdecl After(uint32_t hresult){
    if(active<0||unsigned(active)>=Capacity){overflow=1;return;}SignalSnapshot& r=records[active];r.fields[7]=hresult;
    if(r.fields[8]==3){uint32_t f[24]={0};Fields(f,r.arguments);bool same=true;for(unsigned i=0;i<24;i++)if(i!=6&&i!=7&&i!=8&&f[i]!=r.fields[i])same=false;
        if(memcmp(r.window,reinterpret_cast<const void*>(r.fields[3]),r.fields[10]*8)||(r.fields[20]&&memcmp(r.weights,reinterpret_cast<const void*>(r.fields[21]),1024)))same=false;
        if(same){memcpy(r.spectrum,reinterpret_cast<const void*>(r.fields[4]),r.fields[10]*4);if(r.fields[5])r.fields[6]=word(r.fields[5],0);r.fields[8]=0;}else r.fields[8]=4;
    }active=-1;
}
__declspec(naked) static void ReturnHook() {
    __asm {
        pushfd
        pushad
        mov ebp,esp
        sub esp,544
        and esp,-16
        fxsave [esp]
        push dword ptr [ebp+28]
        call After
        add esp,4
        fxrstor [esp]
        mov esp,ebp
        popad
        popfd
        jmp dword ptr [resumeAddress]
    }
}
__declspec(naked) static void EntryHook() {
    __asm {
        pushfd
        pushad
        mov ebp,esp
        sub esp,544
        and esp,-16
        fxsave [esp]
        lea edx,[ebp+36]
        push edx
        call Before
        add esp,4
        fxrstor [esp]
        mov esp,ebp
        popad
        popfd
        push offset ReturnHook
        jmp dword ptr [signalTarget]
    }
}
extern "C" __declspec(dllexport) void __cdecl SignalConfigure(uint32_t next,uint32_t target){resumeAddress=next;signalTarget=target;}
extern "C" __declspec(dllexport) void* __cdecl SignalHook(){return reinterpret_cast<void*>(&EntryHook);}
extern "C" __declspec(dllexport) void __cdecl SignalSetCase(uint32_t n){caseId=n;}
extern "C" __declspec(dllexport) int __cdecl SignalCount(){return overflow||active!=-1?-1:count;}
extern "C" __declspec(dllexport) const SignalSnapshot* __cdecl SignalRecord(int index){return index>=0&&unsigned(index)<Capacity&&index<count?&records[index]:0;}
extern "C" __declspec(dllexport) int __cdecl SignalSize(){return sizeof(SignalSnapshot);}
