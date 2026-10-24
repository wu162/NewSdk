    // Nested Draw list fields: original list Marshal preserves contiguous item blocks before strings.
    if(input.typeId==3036920908u) {
        U32 count=static_cast<U32>(input.lightningSets.size()); Word(target,28,count);
        if(count) {
            unsigned char* items=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(target+32),72,count));
            for(U32 i=0;i<count;++i) {
                for(U32 j=0;j<6;++j) Random(items+72*i,12*j,input.lightningSets[i].random[j]);
                if(bigEndian) for(U32 j=0;j<72;j+=4) Swap(items+72*i,j);
            }
            tracker.Pop();
        }
        if(bigEndian) Swap(target,28);
    } else if(input.typeId==3408220251u) {
        U32 count=static_cast<U32>(input.objectSets.size()); Word(target,8,count);
        if(count) {
            unsigned char* items=static_cast<unsigned char*>(tracker.Push(reinterpret_cast<void**>(target+12),16,count));
            for(U32 i=0;i<count;++i) {
                unsigned char* item=items+16*i; const auto& source=input.objectSets[i];
                if(source.hasRenderGroup) MarshalString(tracker,item,source.renderGroup);
                Word(item,8,static_cast<U32>(ParseSigned(source.numObjects)));
                Float(item,12,ParsePercentage(source.percent));
                if(bigEndian) { Swap(item,0); Swap(item,8); Swap(item,12); }
            }
            tracker.Pop();
        }
        if(bigEndian) Swap(target,8);
    }
