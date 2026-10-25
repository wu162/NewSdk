#include "../src/Native/W3x/Timecoded.h"
#include <cassert>
#include <iostream>
#include <stdexcept>
#include <cstring>
using namespace Recovered::W3x;
namespace {
TimecodedFrame Scalar(unsigned index,float value,bool binary=false) {
    return TimecodedFrame{index,std::vector<float>(1,value),binary};
}
TimecodedFrame Q(unsigned index,float w,bool binary=false) {
    return TimecodedFrame{index,std::vector<float>{0.f,0.f,0.f,w},binary};
}
TimecodedChannel Compress(unsigned type,const std::vector<TimecodedFrame>& frames,
                          float error=0.f,float rate=1.f) {
    return CompressTimecoded(type,259,frames,error,error,error,rate);
}
}
int main() {
    // Original scalar two-frame comparison subtracts the second value from
    // itself. Preserve this counterintuitive observable behavior.
    TimecodedChannel c=Compress(0,{Scalar(0,1.f),Scalar(1,999.f)});
    assert(c.frames.size()==1 && c.frames[0].values[0]==1.f);
    c=Compress(0,{Scalar(0,1.f),Scalar(1,999.f,true)});
    assert(c.frames.size()==2);
    // A strict '<' means zero tolerance does not remove the middle sample;
    // positive tolerance observes LAST endpoint error, not middle sample error.
    c=Compress(0,{Scalar(0,0.f),Scalar(1,100.f),Scalar(2,0.f,true)});
    assert(c.frames.size()==3);
    c=Compress(0,{Scalar(0,0.f),Scalar(1,100.f,true),Scalar(2,0.f)},.001f);
    assert(c.frames.size()==1);
    c=Compress(0,{Scalar(0,0.f),Scalar(1,5.f),Scalar(2,10.f)},-1.f,.67f);
    assert(c.frames.size()==2 && c.frames[1].index==2);
    c=Compress(0,{Scalar(0,0.f),Scalar(1,5.f),Scalar(2,10.f)},0.f);
    assert(c.frames.size()==3);
    c=Compress(3,{Q(0,1.f),Q(1,-1.f),Q(2,1.f)},0.f);
    assert(c.frames.size()==1);
    c=Compress(3,{Q(0,1.f),Q(1,1.f,true)},.1f);
    assert(c.frames.size()==2);
    c=Compress(0,{Scalar(0x12345,1.f),Scalar(0x23456,-2.f,true)},-1.f);
    assert(EstimateTimecodedSize(c)==12u);
    TimecodedEncoded le=EncodeTimecoded(c,false),be=EncodeTimecoded(c,true);
    assert(le.root[0]==0x69 && le.root[1]==0xcf && le.root[2]==0xd8 && le.root[3]==0x60);
    assert(le.root[16]==3 && le.root[17]==1 && be.root[16]==1 && be.root[17]==3);
    assert(le.root[24]==0 && le.root[32]==0);
    assert(le.codes.size()==4 && le.codes[0]==0x45 && le.codes[1]==0x23);
    assert(le.codes[2]==0x56 && le.codes[3]==0xb4);
    assert(le.values[2]==0x80 && le.values[3]==0x3f);
    assert(be.values[0]==0x3f && be.values[1]==0x80);
    assert(be.codes[0]==0x23 && be.codes[1]==0x45);
    // A finite source whose byte swap is an sNaN: original float-returning
    // EndianToPlatform quiets the swapped representation through x87.
    std::uint32_t specialBits=0xf6d882ffu;
    float special;
    std::memcpy(&special,&specialBits,4);
    c=Compress(0,{Scalar(0,special)},-1.f);
    be=EncodeTimecoded(c,true);
    le=EncodeTimecoded(c,false);
    assert(be.values[0]==0xf6 && be.values[1]==0xd8 && be.values[2]==0xc2 && be.values[3]==0xff);
    assert(le.values[0]==0xff && le.values[1]==0x82 && le.values[2]==0xd8 && le.values[3]==0xf6);
    TimecodedChannel empty=Compress(0,{});
    assert(EstimateTimecodedSize(empty)==0);
    bool rejected=false;
    try { EncodeTimecoded(empty,false); } catch(const std::invalid_argument&) {rejected=true;}
    assert(rejected);
    std::cout << "PASS W3X Timecoded local source-contract checks\n";
}
