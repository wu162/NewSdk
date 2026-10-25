// Test-only x86 original DCT I/O capture. No product codec implementation.
#include <stdint.h>
#include <string.h>
#include <intrin.h>
#pragma intrinsic(_InterlockedIncrement)
static const unsigned Capacity=256,MaxData=8192;
#pragma pack(push,4)
struct DctSnapshot {
    uint32_t begin,arguments[9],dataPointer,n,scaleBits,target,fftCallback,optionalPointer,optionalAfter,hresult,status,caseId;
    unsigned char input[MaxData],output[MaxData];
    uint32_t end;
};
#pragma pack(pop)
static DctSnapshot records[Capacity];
static volatile long count,overflow;
static int active=-1;
static uint32_t resumeAddress,caseId;
// The accepted original raw oracle invokes codecs serially; native a81db does
// not recurse. Keep an explicit single-active-call guard instead of silently
// pairing input/output from different invocations.
static void __cdecl Before(const uint32_t* arguments,uint32_t target) {
    if(active!=-1){overflow=1;return;}
    long index=_InterlockedIncrement(&count)-1;
    if(index<0 || unsigned(index)>=Capacity){overflow=1;return;}
    active=index;DctSnapshot& r=records[index];r.begin=0xa55a1234;r.end=0x5aa54321;r.status=1;r.caseId=caseId;
    memcpy(r.arguments,arguments,36);r.dataPointer=arguments[0];r.scaleBits=arguments[1];r.optionalPointer=arguments[2];r.n=arguments[3];r.target=target;r.fftCallback=arguments[4];
    if(!r.dataPointer || r.n<64 || r.n>2048 || (r.n&(r.n-1))){r.status=2;return;}
    memcpy(r.input,reinterpret_cast<const void*>(r.dataPointer),r.n*4);r.status=3;
}
static void __cdecl After(uint32_t hresult) {
    if(active<0 || unsigned(active)>=Capacity){overflow=1;return;}
    DctSnapshot& r=records[active];r.hresult=hresult;
    if(r.status==3){memcpy(r.output,reinterpret_cast<const void*>(r.dataPointer),r.n*4);if(r.optionalPointer)r.optionalAfter=*reinterpret_cast<const uint32_t*>(r.optionalPointer);r.status=0;}
    active=-1;
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
        mov eax,[ebp+28]
        mov eax,[eax+1f0h]
        push eax
        push edx
        call Before
        add esp,8
        fxrstor [esp]
        mov esp,ebp
        popad
        popfd
        push offset ReturnHook
        jmp dword ptr [eax+1f0h]
    }
}
extern "C" __declspec(dllexport) void __cdecl DctConfigure(uint32_t next){resumeAddress=next;}
extern "C" __declspec(dllexport) void* __cdecl DctHook(){return reinterpret_cast<void*>(&EntryHook);}
extern "C" __declspec(dllexport) void __cdecl DctSetCase(uint32_t n){caseId=n;}
extern "C" __declspec(dllexport) int __cdecl DctCount(){return overflow||active!=-1?-1:count;}
extern "C" __declspec(dllexport) const DctSnapshot* __cdecl DctRecord(int index){return index>=0&&unsigned(index)<Capacity&&index<count?&records[index]:0;}
extern "C" __declspec(dllexport) int __cdecl DctSize(){return sizeof(DctSnapshot);}
