#include "WeaponSlotOrder.h"
#include <algorithm>
#include <climits>
#include <stdexcept>
namespace Recovered { namespace Native {
namespace {
// Original blob 6e40a9ed6c9f961d294d49768d911e6a6f354619:
// quick_sort/impl, median, partition, insertion_sort/simple, heap helpers.
class Sort {
    const std::vector<int>& keys;
    std::vector<std::uint32_t> order;
    bool Less(std::uint32_t a,std::uint32_t b) const {return keys[a]<keys[b];}
    std::uint32_t Median(int a,int b,int c) const {
        if(Less(order[a],order[b])) {if(Less(order[b],order[c])) return order[b];if(Less(order[a],order[c])) return order[c];return order[a];}
        if(Less(order[a],order[c])) return order[a];
        if(Less(order[b],order[c])) return order[c];
        return order[b];
    }
    int Partition(int first,int last,std::uint32_t pivot) {
        for(;;) {while(Less(order[first],pivot)) ++first;do {--last;}while(Less(pivot,order[last]));if(first>=last) return first;std::swap(order[first],order[last]);++first;}
    }
    void AdjustHeap(int first,int top,int size,int position,std::uint32_t value) {
        int child=position*2+2;
        while(child<size) {if(Less(order[first+child],order[first+child-1])) --child;order[first+position]=order[first+child];position=child;child=child*2+2;}
        if(child==size) {order[first+position]=order[first+child-1];position=child-1;}
        while(position>top) {int parent=(position-1)/2;if(!Less(order[first+parent],value)) break;order[first+position]=order[first+parent];position=parent;}
        order[first+position]=value;
    }
    void HeapSort(int first,int last) {
        int size=last-first;if(size<2) return;
        for(int parent=(size-2)/2;;--parent) {AdjustHeap(first,parent,size,parent,order[first+parent]);if(parent==0) break;}
        for(int end=last-1;end>first;--end) {auto value=order[end];order[end]=order[first];AdjustHeap(first,0,end-first,0,value);}
    }
    void Quick(int first,int last,int depth) {
        if(last-first>28) while(depth>0) {int split=Partition(first,last,Median(first,first+(last-first)/2,last-1));--depth;Quick(split,last,depth);last=split;if(last-first<=28) break;}
        if(depth==0) HeapSort(first,last);
    }
    void Insertion(int first,int last) {
        for(int at=first+1;at<last;++at) {auto value=order[at];int position=at;while(position>first && Less(value,order[position-1])) {order[position]=order[position-1];--position;}order[position]=value;}
    }
    void InsertionSimple(int first,int last) {
        for(int at=first;at<last;++at) {auto value=order[at];int position=at;while(Less(value,order[position-1])) {order[position]=order[position-1];--position;}order[position]=value;}
    }
public:
    explicit Sort(const std::vector<int>& input):keys(input),order(input.size()) {for(std::uint32_t i=0;i<order.size();++i) order[i]=i;}
    std::vector<std::uint32_t> Run() {
        int size=static_cast<int>(order.size()),bits=0;for(unsigned n=static_cast<unsigned>(size);n;n>>=1) ++bits;
        Quick(0,size,bits*2-2);if(size>28) {Insertion(0,28);InsertionSimple(28,size);}else Insertion(0,size);return order;
    }
};
}
std::vector<std::uint32_t> WeaponSlotOrder(const std::vector<int>& ordering) {
    if(ordering.size()>INT_MAX) throw std::length_error("WeaponSlot ordering exceeds x86 signed element range");
    return Sort(ordering).Run();
}
}}
