#include "../src/Native/WeaponSlotOrder.h"
#include <algorithm>
#include <fstream>
#include <sstream>
#include <iostream>
#include <stdexcept>
#include <climits>
#include <set>
using Recovered::Native::WeaponSlotOrder;
std::vector<std::string> Split(const std::string& line,char delimiter) {std::vector<std::string> result;std::stringstream stream(line);std::string part;while(std::getline(stream,part,delimiter)) result.push_back(part);return result;}
int main(int argc,char** argv) {
    try {
        if(argc!=2) throw std::runtime_error("Expected original permutations.tsv");
        std::ifstream input(argv[1]);if(!input) throw std::runtime_error("Cannot open original evidence");std::string line;std::getline(input,line);unsigned fixtures=0;std::set<std::string> combinations;
        while(std::getline(input,line)) {auto fields=Split(line,'\t');if(fields.size()!=12) throw std::runtime_error("Malformed original evidence row");int count=std::stoi(fields[2]),pattern=std::stoi(fields[3]);
            const std::set<std::string> platforms={"0","1","2","99"},slots={"WeaponSlotHardpoint","WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"};const std::set<int> counts={0,1,2,27,28,29,65,129,257};
            if(!platforms.count(fields[0]) || !slots.count(fields[1]) || !counts.count(count) || pattern<0 || pattern>=5 || !combinations.insert(fields[0]+"/"+fields[1]+"/"+fields[2]+"/"+fields[3]).second) throw std::runtime_error("Unexpected or duplicate original case");
            std::vector<int> keys(count);std::uint32_t random=1234567;
            for(int i=0;i<count;++i) {random=1664525u*random+1013904223u;keys[i]=pattern==0?i*6/std::max(1,count):pattern==1?5-i*6/std::max(1,count):pattern==2?2:pattern==3?static_cast<int>(random%6):((count-1-i)/3)%6;}
            auto order=WeaponSlotOrder(keys);auto expected=Split(fields[5],',');if(order.size()!=expected.size()) throw std::runtime_error("Permutation count mismatch");for(unsigned i=0;i<order.size();++i) if(order[i]!=static_cast<unsigned>(std::stoul(expected[i]))) throw std::runtime_error("Original permutation mismatch at "+fields[0]+"/"+fields[1]+"/"+fields[2]+"/"+fields[3]+" index "+std::to_string(i));++fixtures;
        }
        if(fixtures!=720) throw std::runtime_error("Incomplete original matrix");
        // Additional arbitrary signed-key cases check that the complete heap and
        // partition implementation returns a sorted permutation without aliasing.
        std::uint32_t random=17;for(int count=0;count<1024;++count) {std::vector<int> keys(count);for(auto& key:keys) {random=1664525u*random+1013904223u;key=static_cast<int>(random);}if(count>2) {keys[0]=INT_MIN;keys[1]=INT_MAX;}auto order=WeaponSlotOrder(keys);std::vector<bool> seen(count);for(int i=0;i<count;++i) {if(order[i]>=keys.size() || seen[order[i]] || (i && keys[order[i-1]]>keys[order[i]])) throw std::runtime_error("Invalid signed-key permutation");seen[order[i]]=true;}}
        std::cout<<"PASS WeaponSlot ordering: 720 original permutations plus 1024 signed-key property cases; recovered WeaponSetUpdate remains refused\n";return 0;
    }catch(const std::exception& error) {std::cerr<<error.what()<<'\n';return 1;}
}
