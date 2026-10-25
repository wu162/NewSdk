#include "../src/Native/W3x/AdaptiveDelta.h"
#include <cmath>
#include <cstdlib>
#include <cstring>
#include <iostream>
#include <limits>
#include <stdexcept>
using namespace Recovered::W3x;
namespace {
unsigned checks=0;
void Check(bool good,const char* text) { ++checks; if (!good) throw std::runtime_error(text); }
std::uint32_t U32(const unsigned char* p,bool big) {
    std::uint32_t value=0;
    for (unsigned i=0;i<4;++i) value|=std::uint32_t(p[i])<<((big?3-i:i)*8);
    return value;
}
std::uint32_t Bits(float f) { std::uint32_t n; std::memcpy(&n,&f,4); return n; }
template<class F> void Throws(F f) { bool threw=false; try { f(); } catch(const std::exception&) { threw=true; } Check(threw,"Missing rejection"); }
void VerifyReconstruction(const AdaptiveDeltaChannel& c,const std::vector<std::vector<float> >& samples,float tolerance) {
    std::vector<float> previous=c.initial;
    const std::size_t width=previous.size();
    for (std::size_t block=0;block<c.frames.size()/width;++block) {
        for (std::size_t j=0;j<width;++j) {
            const AdaptiveDeltaFrame& f=c.frames[block*width+j];
            const float unit=(AdaptiveDeltaFilter(f.filter)*c.scale)*(c.bits==8?0.0625f:1.0f);
            for(unsigned i=0;i<16;++i) {
                const double value=double(f.delta[i])*double(unit)+double(previous[j]);
                const std::size_t src=1+block*16+i;
                if(src<samples.size()) Check(std::fabs(float(value-double(samples[src][j])))<=tolerance,"Reconstruction exceeds maxError");
                previous[j]=float(value);
            }
        }
    }
}
}
int main() {
    try {
        Check(Bits(AdaptiveDeltaFilter(0))==0x322bcc77u,"First filter must retain PE bits");
        Check(AdaptiveDeltaFilter(8)==1.0f && AdaptiveDeltaFilter(9)==10.0f,"Decimal filters");
        Check(AdaptiveDeltaFilter(16)==1.0f,"Sin table first");
        for(unsigned i=17;i<256;++i) Check(AdaptiveDeltaFilter(i)<AdaptiveDeltaFilter(i-1),"Sin table monotonic");
        Throws([] { AdaptiveDeltaFilter(256); });
        std::array<float,17> ramp;
        for(unsigned i=0;i<17;++i) ramp[i]=float(i);
        AdaptiveDeltaFrame frame;
        bool excess=false;
        float previous=-1;
        Check(CompressAdaptiveDeltaFrame(frame,8,ramp,1.0f,4,&previous,&excess,0)==0.0f,"Exact integer ramp");
        Check(previous==16.0f && !excess && frame.filter==8,"Ramp state");
        for(unsigned i=0;i<16;++i) Check(frame.delta[i]==1,"Ramp delta");
        for(unsigned i=0;i<17;++i) ramp[i]=-float(i);
        Check(CompressAdaptiveDeltaFrame(frame,8,ramp,1.0f,4,0,0,0)==0.0f,"Exact descending ramp");
        for(unsigned i=0;i<16;++i) Check(frame.delta[i]==-1,"Negative ramp delta");
        // Frame saturation and persistent error flag are independent of whole
        // channel filter search. First positive step 100 is capped at +7.
        ramp.fill(100.0f); ramp[0]=0.0f;
        excess=false;
        CompressAdaptiveDeltaFrame(frame,8,ramp,1.0f,4,0,&excess,0.1f);
        Check(frame.delta[0]==7 && excess,"Positive signed nibble cap");
        ramp.fill(-100.0f); ramp[0]=0.0f;
        CompressAdaptiveDeltaFrame(frame,8,ramp,1.0f,4,0,0,0);
        Check(frame.delta[0]==-8,"Negative signed nibble cap");
        // Scale zero exercises x86 conv.i4(NaN)'s low byte, without C++ UB.
        ramp.fill(2.0f);
        Check(CompressAdaptiveDeltaFrame(frame,0,ramp,0.0f,4,&previous,0,0)==0,"Zero scale error");
        Check(previous==2.0f,"Zero scale previous");
        for(unsigned i=0;i<16;++i) Check(frame.delta[i]==0,"Zero scale byte");
        AdaptiveDeltaChannel sentinel;
        sentinel.type=123;
        Check(!CompressAdaptiveDelta(1,2,{},0,sentinel) && sentinel.type==123,"Null result preserves output");
        for(unsigned count=2;count<=34;++count) {
            for(unsigned dim=1;dim<=4;++dim) {
                std::vector<std::vector<float> > samples(count,std::vector<float>(dim,2.0f));
                AdaptiveDeltaChannel c;
                Check(CompressAdaptiveDelta(0x10203040u,0x1234u,samples,0,c),"Constant compression");
                Check(c.bits==4 && c.scale==0 && c.frames.size()==((count+15)/16)*dim,"Block count uses source sample count");
                Check(EstimateAdaptiveDeltaSize(c)==dim*4u+((dim*4u+7u)>>3)*c.frames.size(),"Original estimate formula");
                for(unsigned endian=0;endian<2;++endian) {
                    AdaptiveDeltaEncoded b=EncodeAdaptiveDelta(c,endian!=0);
                    Check(U32(b.root.data(),endian)!=0 && U32(b.root.data(),endian)==2012836613u,"Delta type id");
                    Check(U32(b.root.data()+4,endian)==0x10203040u,"Channel type");
                    Check(U32(b.root.data()+8,endian)==((count+15)/16)*16,"Padded runtime count");
                    Check(U32(b.root.data()+12,endian)==dim && U32(b.root.data()+24,endian)==dim,"Dimensions");
                    Check(b.root[18]==0 && b.root[19]==0,"Root padding");
                    Check(U32(b.root.data()+28,endian)==0 && U32(b.root.data()+40,endian)==0,"Unbound relocation slots");
                    Check(b.initial.size()==dim*4 && U32(b.initial.data(),endian)==0x40000000u,"Initial endian floats");
                    Check(b.data.size()==c.frames.size()*9 && U32(b.root.data()+36,endian)==b.data.size(),"Packed payload bytes");
                    for(std::size_t i=0;i<b.data.size();++i) Check(b.data[i]==0,"Constant compressed payload");
                }
            }
        }
        // Direct wire fixture checks nibble order and the endian-invariant byte
        // stream, not an encoder/decoder round trip.
        AdaptiveDeltaChannel wire;
        wire.type=7; wire.pivot=9; wire.scale=1; wire.bits=4; wire.initial.push_back(0);
        AdaptiveDeltaFrame f; f.filter=0xfe;
        for(unsigned i=0;i<16;++i) f.delta[i]=std::int8_t(int(i)-8);
        wire.frames.push_back(f);
        AdaptiveDeltaEncoded encoded=EncodeAdaptiveDelta(wire,false);
        Check(encoded.data[0]==0xfe && encoded.data[1]==0x98 && encoded.data[4]==0xfe && encoded.data[5]==0x10 && encoded.data[8]==0x76,"Signed nibble wire fixture");
        Check(EncodeAdaptiveDelta(wire,true).data==encoded.data,"Packed bytes not endian swapped");
        wire.bits=8; encoded=EncodeAdaptiveDelta(wire,false);
        Check(encoded.data.size()==17 && encoded.data[1]==0xf8 && encoded.data[16]==7,"Eight bit wire fixture");
        unsigned four=0,eight=0,rejected=0;
        std::uint32_t random=123456789;
        for(unsigned fixture=0;fixture<40;++fixture) {
            const unsigned dim=fixture%4+1, count=fixture%31+2;
            std::vector<std::vector<float> > samples(count,std::vector<float>(dim));
            for(unsigned i=0;i<count;++i) for(unsigned j=0;j<dim;++j) {
                random=random*1664525u+1013904223u;
                samples[i][j]=float(int((random>>16)%1000)-500)/100.0f;
            }
            for(unsigned level=0;level<3;++level) {
                const float tolerance=level==0?1.0f:(level==1?0.03f:0.0f);
                AdaptiveDeltaChannel c;
                if(CompressAdaptiveDelta(1,2,samples,tolerance,c)) {
                    c.bits==4?++four:++eight;
                    VerifyReconstruction(c,samples,tolerance);
                } else ++rejected;
            }
        }
        Check(four && eight && rejected,"Must exercise 4 bit, 8 bit and null paths");
        Throws([&] { CompressAdaptiveDelta(0,0,{{1},{2,3}},0,sentinel); });
        Throws([&] { CompressAdaptiveDelta(0,0,{{1},{std::numeric_limits<float>::infinity()}},0,sentinel); });
        Throws([&] { CompressAdaptiveDelta(0,0,{{1},{2}},std::numeric_limits<float>::quiet_NaN(),sentinel); });
        Throws([&] { CompressAdaptiveDelta(0,0,{{},{}} ,0,sentinel); });
        std::cout << "PASS AdaptiveDelta " << checks << " checks, 4bit=" << four << " 8bit=" << eight << " null=" << rejected << "\n";
    } catch(const std::exception& e) { std::cerr << "FAIL " << e.what() << "\n"; return 1; }
}
