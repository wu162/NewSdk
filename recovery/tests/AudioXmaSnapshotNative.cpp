// Test-only x86 integer capture helper. No codec implementation or product DLL.
#include <stdint.h>
#include <string.h>
#include <intrin.h>
#pragma intrinsic(_InterlockedIncrement)
static const unsigned Capacity = 256, MaxSource = 65536;
#pragma pack(push,4)
struct Snapshot {
    uint32_t begin;
    uint32_t arguments[32];
    int32_t blockOffset;
    uint32_t newerBytes, olderBytes, status, caseId, outputPointer;
    unsigned char newer[MaxSource], older[MaxSource];
    uint32_t end;
};
#pragma pack(pop)
static Snapshot records[Capacity];
static volatile long count, overflow;
static uint32_t originalReturn, activeCase;
static bool maximumSpan(int64_t& maximum, int64_t offset, int countFrames, int step, int bytes) {
    if(countFrames <= 0) return true;
    if(offset < 0) return false;
    int64_t last = offset + int64_t(countFrames - 1) * step + bytes;
    if(last > maximum) maximum = last;
    return last <= MaxSource;
}
static void __cdecl Capture(const uint32_t* stack) {
    long index = _InterlockedIncrement(&count) - 1;
    if(index < 0 || unsigned(index) >= Capacity) { overflow = 1; return; }
    Snapshot& r = records[index]; r.begin = 0xa55a1234; r.end = 0x5aa54321; r.caseId = activeCase; r.status = 1;
    memcpy(r.arguments, stack, 128);
    const uint32_t* a = r.arguments;
    int full = int(a[9]), width = int(a[10]), window = int(a[11]), bytes = int(a[12]), bits = int(a[13]), stride = int(a[14]);
    if(full < 2 || full > 16384 || window < 2 || window > full || width != 2*window ||
       (full&1) || (window&1) || int(a[4]) != full/2 || int(a[5]) != window/2 ||
       (bits != 16 && bits != 20 && bits != 24) || bytes != (bits == 16 ? 2 : 3) || stride < 1 || stride > 2 ||
       a[6] == 0 || a[7] > 65535 || a[3] == 0 || a[29] != unsigned(bits == 16 ? 61 : bits == 20 ? 78 : 94)) { r.status = 2; return; }
    int boundaries[4]; for(int i=0;i<4;i++) { boundaries[i]=int(a[15+i]); if(boundaries[i]<0 || boundaries[i]>width) {r.status=3;return;} }
    const short* offsets = reinterpret_cast<const short*>(a[6]); r.blockOffset = offsets[a[7]];
    int64_t initial = int64_t(r.blockOffset) - int(a[5]) + int(a[4]);
    if(initial < 0 || initial > 2*int64_t(full)) {r.status=4;return;}
    int available = initial < full ? int((initial+width<full?initial+width:full)-initial) : 0;
    bool old = available > 0; int step = bytes*stride;
    int64_t offset = (old?initial:initial-full)*step + int64_t(boundaries[0])*step;
    int position = boundaries[0]; int ends[6] = {int(a[5]),boundaries[1],window,boundaries[2],3*int(a[5]),boundaries[3]};
    int64_t oldSpan=0,newSpan=0;
    for(int phase=0;phase<6;phase++) {
        if(ends[phase] > position) {
            int n=ends[phase]-position;
            if(!maximumSpan(old?oldSpan:newSpan,offset,n,step,bytes)) {r.status=5;return;}
            offset+=int64_t(n)*step; position=ends[phase];
        }
        if((phase==0 || phase==2 || phase==4) && old && position>=available) {old=false;offset=0;}
    }
    if(oldSpan > int64_t(full)*step || newSpan > int64_t(full)*step || (oldSpan && !a[2]) || (newSpan && !a[1])) {r.status=6;return;}
    r.outputPointer = *reinterpret_cast<const uint32_t*>(a[3]+0x48);
    r.olderBytes=uint32_t(oldSpan);r.newerBytes=uint32_t(newSpan);
    if(oldSpan)memcpy(r.older,reinterpret_cast<const void*>(a[2]),size_t(oldSpan));
    if(newSpan)memcpy(r.newer,reinterpret_cast<const void*>(a[1]),size_t(newSpan));
    r.status=0;
}
__declspec(naked) static void Hook() {
    __asm {
        pushfd
        pushad
        mov ebp,esp
        sub esp,544
        and esp,-16
        fxsave [esp]
        lea eax,[ebp+36]
        push eax
        call Capture
        add esp,4
        fxrstor [esp]
        mov esp,ebp
        popad
        popfd
        push dword ptr [originalReturn]
        jmp dword ptr [esi+18e08h]
    }
}
extern "C" __declspec(dllexport) void __cdecl SnapshotConfigure(uint32_t next) { originalReturn=next; }
extern "C" __declspec(dllexport) void* __cdecl SnapshotHook() { return reinterpret_cast<void*>(&Hook); }
extern "C" __declspec(dllexport) void __cdecl SnapshotSetCase(uint32_t value) { activeCase=value; }
extern "C" __declspec(dllexport) int __cdecl SnapshotCount() { return overflow ? -1 : count; }
extern "C" __declspec(dllexport) const Snapshot* __cdecl SnapshotRecord(int index) { return index>=0 && unsigned(index)<Capacity && index<count ? &records[index] : 0; }
extern "C" __declspec(dllexport) int __cdecl SnapshotSize() { return sizeof(Snapshot); }
