#include "../../src/TextureCompiler/Native/TextureSupport.h"
#include <algorithm>
#include <cassert>
#include <cstring>
#include <iostream>
#include <stdexcept>
#include <vector>
using namespace Recovered::TextureCompiler;
int main() {
    const std::uint32_t capacities[] = {1,2,3,7,16,31,256,1024};
    const std::uint32_t counts[] = {0,1,2,5,8,64,0,513,3};
    unsigned comparisons = 0;
    for (std::uint32_t initial : capacities) {
        SimpleBuffer b; assert(ConstructBuffer(&b,initial)==&b);
        assert(b.size==0 && b.capacity==initial && b.data);
        std::vector<unsigned char> model;
        for (int round=0;round<3;++round) {
            for (std::uint32_t count : counts) {
                std::vector<unsigned char> source(count);
                for(std::uint32_t i=0;i<count;++i) source[i]=static_cast<unsigned char>((i*31+count*7+round)&255);
                model.insert(model.end(),source.begin(),source.end());
                AppendBuffer(&b,count ? source.data() : nullptr,count);
                assert(b.size==model.size());
                if(b.size) assert(std::memcmp(b.data,model.data(),b.size)==0);
                assert(b.capacity>=b.size && b.capacity%initial==0);
                assert((b.capacity/initial & (b.capacity/initial-1))==0);
                comparisons += b.size;
            }
            auto* data=b.data; auto capacity=b.capacity;
            ResetBuffer(&b);model.clear();
            assert(!b.size && b.data==data && b.capacity==capacity);
        }
        DestroyBuffer(&b);assert(!b.data && !b.size && !b.capacity);
        DestroyBuffer(&b); // Native destructor body is idempotent (flags=0).
    }
    SimpleBuffer zero;ConstructBuffer(&zero,0);AppendBuffer(&zero,nullptr,0);
    bool rejected=false;unsigned char byte=0;
    try { AppendBuffer(&zero,&byte,1); } catch(const std::length_error&) { rejected=true; }
    assert(rejected);DestroyBuffer(&zero);
    for(std::uint32_t value : {0u,1u,0x12345678u,0xffffffffu,0x80000000u,0x00008000u})
        assert(ReverseEndianness(ReverseEndianness(value))==value);
    assert(ReverseEndianness(0x12345678u)==0x78563412u);
    assert(Direct3DCreate9(0)==reinterpret_cast<void*>(1));
    assert(Direct3DCreate9(32)==reinterpret_cast<void*>(1));
    assert(Direct3DRelease()==1);
    std::cout << "PASS deterministic TextureSupport tests: " << comparisons << " payload bytes; native original differential pending Windows oracle\n";
}
