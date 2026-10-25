#include "Timecoded.h"
#include <cmath>
#include <cstring>
#include <limits>
#include <stdexcept>

namespace Recovered { namespace W3x {
namespace {
void Validate(const TimecodedChannel& channel) {
    if (channel.frames.size() > 0xffffffffu)
        throw std::invalid_argument("Timecoded frame count overflow");
    if (channel.frames.empty()) return;
    const std::size_t width = channel.frames.front().values.size();
    if (width == 0 || width > 4 || (channel.type == 3 && width != 4))
        throw std::invalid_argument("Timecoded source dimension");
    for (const TimecodedFrame& frame : channel.frames) {
        if (frame.values.size() != width)
            throw std::invalid_argument("Timecoded inconsistent source dimension");
        for (float value : frame.values)
            if (!std::isfinite(value))
                throw std::invalid_argument("Timecoded nonfinite source");
    }
}
std::array<float,4> Slerp(const std::vector<float>& p,
                        const std::vector<float>& q, float alpha) {
    float dot = float(double(p[0])*q[0]+double(p[1])*q[1]+
                      double(p[2])*q[2]+double(p[3])*q[3]);
    const bool flip = dot < 0.f;
    if (flip) dot = 0.f-dot;
    float weight;
    if (1.0-double(dot) < 9.99999905104687E-09) {
        weight = float(1.0-double(alpha));
    } else {
        const float theta = float(std::acos(double(dot)));
        const float sine = float(std::sin(double(theta)));
        const float inverse = float(1.0/double(sine));
        const double angle = double(alpha)*double(theta);
        weight = float(double(float(std::sin(double(float(double(theta)-angle)))))*double(inverse));
        alpha = float(double(float(std::sin(double(float(angle)))))*double(inverse));
    }
    if (flip) alpha = 0.f-alpha;
    std::array<float,4> result;
    for (unsigned j=0;j<4;++j)
        result[j] = float(double(p[j])*double(weight)+double(q[j])*double(alpha));
    return result;
}
float RotationW(const std::vector<float>& p, const std::array<float,4>& q) {
    // W of conjugate(p)*q, retaining the original double expression grouping.
    return float(double(p[3])*double(q[3])-
        (double(0.f-p[0])*double(q[0])+double(q[1])*double(0.f-p[1])+
         double(0.f-p[2])*double(q[2])));
}
int FindScalar(const std::vector<TimecodedFrame>& f, float error) {
    if (f.size()<2) return -1;
    if (f.size()==2) {
        if (f[1].binaryMove) return -1;
        if (error>=0.f) {
            error = float(double(float(std::uint32_t(f[1].index-f[0].index)))*double(error));
            for (float value : f[1].values) {
                const float difference=float(double(value)-double(value));
                if (double(float(std::fabs(double(difference)))) > double(error)) return -1;
            }
        }
        return 1;
    }
    int best=-1;
    float minimum=std::numeric_limits<float>::max();
    for (std::size_t i=0;i+2<f.size();++i) {
        if (f[i].binaryMove || f[i+2].binaryMove) continue;
        const float alpha=float(double(float(std::uint32_t(f[i+1].index-f[i].index)))/
                                double(float(std::uint32_t(f[i+2].index-f[i].index))));
        float maximum=0.f;
        for (std::size_t j=0;j<f[i].values.size();++j) {
            // Original FindUselessPacket compares the interpolated endpoints
            // with the LAST endpoint (not the middle sample).
            const double last=f[i+2].values[j], first=f[i].values[j];
            const float difference=float(std::fabs(double(float((last-first)*double(alpha)+first-last))));
            if (double(difference)>double(maximum)) maximum=difference;
        }
        if (double(maximum)<double(error)) return int(i+1);
        if (double(maximum)<double(minimum)) {best=int(i+1);minimum=maximum;}
    }
    return error<0.f ? best : -1;
}
int FindQuaternion(const std::vector<TimecodedFrame>& f, float error) {
    if (f.size()<2) return -1;
    if (f.size()==2) {
        if (f[1].binaryMove) return -1;
        if (error>=0.f) {
            std::array<float,4> last;
            for (unsigned j=0;j<4;++j) last[j]=f[1].values[j];
            float w=float(std::fabs(double(RotationW(f[0].values,last))));
            if (w<1.f && double(float(std::acos(double(w))))*2.0>double(error)) return -1;
        }
        return 1;
    }
    int best=-1;
    float minimum=std::numeric_limits<float>::max();
    for (std::size_t i=0;i+2<f.size();++i) {
        if (f[i].binaryMove || f[i+2].binaryMove) continue;
        float alpha=float(double(float(std::uint32_t(f[i+1].index-f[i].index)))/
                          double(float(std::uint32_t(f[i+2].index-f[i].index))));
        const std::array<float,4> interpolated=Slerp(f[i].values,f[i+2].values,alpha);
        float w=float(std::fabs(double(RotationW(f[i+1].values,interpolated))));
        if (w>=1.f) return int(i+1);
        const float angle=float(std::acos(double(w)))*2.f;
        if (double(angle)<=double(error)) return int(i+1);
        if (double(angle)<double(minimum)) {best=int(i+1);minimum=angle;}
    }
    return error<0.f ? best : -1;
}
void Put16(unsigned char* p, std::uint16_t value, bool be) {
    p[be?1:0]=static_cast<unsigned char>(value);
    p[be?0:1]=static_cast<unsigned char>(value>>8);
}
void Put32(unsigned char* p, std::uint32_t value, bool be) {
    for (unsigned i=0;i<4;++i) p[be?3-i:i]=static_cast<unsigned char>(value>>(i*8));
}
void PutFloat(unsigned char* p, std::uint32_t bits, bool be) {
    Put32(p,bits,be);
    if (be) {
        // WriteOut calls the original float-returning EndianToPlatform helper.
        // Its swapped word passes through x87 as a float, quieting an sNaN
        // created by the byte swap even when the source float was finite.
        const std::uint32_t swapped=(bits>>24)|((bits>>8)&0x0000ff00u)|
            ((bits<<8)&0x00ff0000u)|(bits<<24);
        if ((swapped&0x7f800000u)==0x7f800000u &&
            (swapped&0x007fffffu)!=0 && (swapped&0x00400000u)==0)
            p[2]|=0x40u;
    }
}
}
TimecodedChannel CompressTimecoded(std::uint32_t type, std::uint16_t pivot,
    const std::vector<TimecodedFrame>& frames, float translationError,
    float rotationError, float visibilityError, float forceRate) {
    TimecodedChannel output={type,pivot,frames};
    Validate(output);
    const double target=double(float(std::uint32_t(frames.size())))*double(forceRate);
    if (!std::isfinite(target) || target<0.0 || target>4294967295.0)
        throw std::invalid_argument("Timecoded force reduction rate outside defined conversion range");
    std::uint32_t minimum=static_cast<std::uint32_t>(target);
    if (minimum<1) minimum=1;
    float error= type==3 ? rotationError : type==4 ? visibilityError : translationError;
    while (output.frames.size()>minimum || error>=0.f) {
        const int index=type==3 ? FindQuaternion(output.frames,error) : FindScalar(output.frames,error);
        if (index<0) {
            if (error<0.f) break;
            error=-1.f;
        } else output.frames.erase(output.frames.begin()+index);
    }
    return output;
}
std::uint32_t EstimateTimecodedSize(const TimecodedChannel& channel) {
    Validate(channel);
    if (channel.frames.empty()) return 0;
    const std::uint64_t size=(channel.frames.front().values.size()*4+2)*std::uint64_t(channel.frames.size());
    return static_cast<std::uint32_t>(size);
}
TimecodedEncoded EncodeTimecoded(const TimecodedChannel& channel, bool bigEndian) {
    Validate(channel);
    if (channel.frames.empty())
        throw std::invalid_argument("Original Timecoded WriteOut dereferences empty source frames");
    TimecodedEncoded encoded={};
    const std::size_t count=channel.frames.size(), width=channel.frames.front().values.size();
    if (count>0xffffffffu/width || count*width>0xffffffffu/4)
        throw std::invalid_argument("Timecoded serialized values overflow");
    Put32(encoded.root.data(),1624821609u,bigEndian);
    Put32(encoded.root.data()+4,channel.type,bigEndian);
    Put32(encoded.root.data()+8,static_cast<std::uint32_t>(count),bigEndian);
    Put32(encoded.root.data()+12,static_cast<std::uint32_t>(width),bigEndian);
    Put16(encoded.root.data()+16,channel.pivot,bigEndian);
    Put32(encoded.root.data()+20,static_cast<std::uint32_t>(count),bigEndian);
    Put32(encoded.root.data()+28,static_cast<std::uint32_t>(count*width),bigEndian);
    encoded.values.resize(count*width*4);
    encoded.codes.resize(count*2);
    for (std::size_t i=0;i<count;++i) {
        std::uint16_t code=static_cast<std::uint16_t>(channel.frames[i].index);
        if (channel.frames[i].binaryMove) code|=0x8000u;
        Put16(encoded.codes.data()+i*2,code,bigEndian);
        for (std::size_t j=0;j<width;++j) {
            std::uint32_t bits;
            std::memcpy(&bits,&channel.frames[i].values[j],4);
            PutFloat(encoded.values.data()+(i*width+j)*4,bits,bigEndian);
        }
    }
    return encoded;
}
}}
