using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] AssistTypes={"2328709842","2920212896","3962605070","3358372044"};
    static Field[][] AssistFields={
        new Field[]{new Field("AllowShrouded","Boolean")},
        new Field[]{new Field("PlacementHint","Bits"),new Field("SnappingDistance","Float")},
        new Field[]{new Field("ObjectStatus","ObjectStatus"),new Field("WeaponSlotID","Unsigned"),new Field("WeaponSlotType","Enum")},
        new Field[]{new Field("SpecialPowerTemplate","Reference"),new Field("ObjectStatus","ObjectStatus")},
        new Field[]{new Field("PlayerTemplate","NullableAssetId"),new Field("HintDisplayObject","AssetId"),new Field("HintOpacity","Float"),new Field("HintUsesShaders","Boolean")},
        new Field[]{new Field("FilterRadius","Float")},
    };
    static string[] AssistAssistanceHintTypeValues={"ALLOW_SHROUDED","FACING_AWAY","FACING_TOWARDS","NONE","","ALL","all","-ALL","+ALL","UNKNOWN","none"," NONE","NONE ","NONE -NONE","NONE UNKNOWN","NO_WEAPON PRIMARY_WEAPON"};
    static string[] AssistWeaponSlotTypeValues={"NO_WEAPON","PRIMARY_WEAPON","QUATERNARY_WEAPON","QUINARY_WEAPON","SECONDARY_WEAPON","TERTIARY_WEAPON","","ALL","all","-ALL","+ALL","UNKNOWN","none"," NONE","NONE ","NONE -NONE","NONE UNKNOWN","NO_WEAPON PRIMARY_WEAPON"};
    static string AssistXML(int group,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AssistTypes[group]+"' id='Assist' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static string[] AssistSamples(Field f) {
        if(f.Kind=="Bits")return AssistAssistanceHintTypeValues;
        if(f.Kind=="Enum")return AssistWeaponSlotTypeValues;
        if(f.Kind=="ObjectStatus")return WeaponSetStatusValues;
        if(f.Kind=="AssetId" || f.Kind=="NullableAssetId")return new[]{"","MiXeD","mixed","A/B","A\\B","A.B","with spaces","中文测试","é ÿ","A&amp;B","A\\23"};
        return AIUpdateSamples(f);
    }
    static void AssistFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;int[] sizes={36,20,44,40},tokens={5,4,4,3};
        string hint="<HintObject PlayerTemplate='MiXeD' HintDisplayObject='MiXeD' HintOpacity='0.25' HintUsesShaders='true'><ObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></ObjectFilter></HintObject>";
        string restriction="<PlacementRestriction FilterRadius='12.5'><RestrictedObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing></RestrictedObjectFilter></PlacementRestriction>",offset="<SuggestedOffset x='1' y='2' z='3'/>";
        for(int group=0;group<4;++group) {
            byte[] b=Check(original,p,"",AssistXML(group)).InstanceData;int at=LifecycleAt(b,big),table=MotionTable(b,big);
            if(b.Length!=at+sizes[group] || Word(b,at,big)!=uint.Parse(AssistTypes[group]))throw new Exception("Independent assistance native type/size");AIUpdateSentinel(b,table,tokens[group]);
            if(group==0 && (Word(b,at+8,big)!=0 || Word(b,at+16,big)!=0 || b[at+32]!=1))throw new Exception("Independent Info default list/nullable restriction/boolean");
            if(group==1 && (Word(b,at+8,big)!=1 || Word(b,at+12,big)!=0 || Word(b,at+16,big)!=0))throw new Exception("Independent assistance NONE bit and nullable Filter");
            foreach(var container in ModuleContainers)Check(original,p,"",ModuleXML(container[0],container[1],"TypeId='"+AssistTypes[group]+"' id='As'"));
            foreach(var f in AssistFields[group])foreach(string value in AssistSamples(f))Check(original,p,"",AssistXML(group,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AssistXML(group,bad));
            foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",AssistXML(group,"",bad));
            string payload=group==0?hint+restriction+offset:group==1?"<RequirementObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></RequirementObjectFilter>":"";
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+AssistTypes[group]+"' id='S"+i+"'>"+payload+"</ObjectSMCHelper>";
                Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<FireWeaponUpdate TypeId='2995535186'><FireWeaponNugget WeaponName='W\\23'><Offset/></FireWeaponNugget></FireWeaponUpdate></Behaviors>");
            }
        }
        foreach(var f in AssistFields[4])foreach(string value in AssistSamples(f))Check(original,p,"",AssistXML(0,"","<HintObject "+f.Name+"='"+Escape(value)+"'/>"));
        foreach(string value in AssistSamples(new Field("FilterRadius","Float")))Check(original,p,"",AssistXML(0,"","<PlacementRestriction FilterRadius='"+Escape(value)+"'/>"));
        foreach(string field in new[]{"x","y","z"})foreach(string value in AIUpdateSamples(new Field(field,"Float")))Check(original,p,"",AssistXML(0,"","<SuggestedOffset "+field+"='"+Escape(value)+"'/>"));
        foreach(int count in new[]{0,1,2,27,28,29,65,129}) {string rows="";for(int i=0;i<count;++i)rows+=hint;Check(original,p,"Browser='a bb'",AssistXML(0,"",rows+restriction+offset));}
        {byte[] b=Check(original,p,"",AssistXML(0,"","<HintObject PlayerTemplate='' HintDisplayObject=''/><PlacementRestriction/>"+offset)).InstanceData;int at=LifecycleAt(b,big),rows=(int)Word(b,at+12,big),player=(int)Word(b,rows,big),rest=(int)Word(b,at+16,big);if(rows!=at+36 || player!=at+56 || rest!=at+60 || b.Length!=at+68 || Word(b,player,big)!=0 || Word(b,rows+4,big)!=0 || Word(b,rows+8,big)!=0x3f800000u || Word(b,rest,big)!=0 || Word(b,rest+4,big)!=0 || Word(b,at+20,big)!=0x3f800000u || Word(b,at+24,big)!=0x40000000u || Word(b,at+28,big)!=0x40400000u)throw new Exception("Independent Info list20/nullable typed ID4/restriction8/inline vector allocation");}
        {byte[] b=Check(original,p,"",AssistXML(0,"",hint+restriction+offset)).InstanceData;int at=LifecycleAt(b,big),rows=(int)Word(b,at+12,big),player=(int)Word(b,rows,big),filter=(int)Word(b,rows+12,big),rest=(int)Word(b,at+16,big);if(player!=rows+20 || filter!=player+4 || Word(b,player,big)!=Word(b,rows+4,big) || Word(b,rows+8,big)!=0x3e800000u || b[rows+16]!=1 || rest<=filter+112 || Word(b,rest,big)!=0x41480000u || Word(b,rest+4,big)!=rest+8)throw new Exception("Independent nested Info weak hash/filter/restriction order");}
        var first=Check(original,p,"",AssistXML(0,"",hint+restriction+offset));var second=Check(original,p,"",AssistXML(0,"",offset+restriction+hint));Equal(first.InstanceData,second.InstanceData,"Info child schema allocation order",false);Equal(first.RelocationData,second.RelocationData,"Info child schema relocations",false);Equal(first.ImportsData,second.ImportsData,"Info child schema imports",false);
        var upper=Check(original,p,"",AssistXML(0,"","<HintObject PlayerTemplate='MiXeD' HintDisplayObject='MiXeD'/>"));var lower=Check(original,p,"",AssistXML(0,"","<HintObject PlayerTemplate='mixed' HintDisplayObject='mixed'/>"));Equal(upper.InstanceData,lower.InstanceData,"Info typed-ID case folding",false);Equal(upper.RelocationData,lower.RelocationData,"Info typed-ID relocations",false);Equal(upper.ImportsData,lower.ImportsData,"Info typed-ID imports",false);
        foreach(string path in new[]{"HintObject","PlacementRestriction","SuggestedOffset","HintObject/ObjectFilter","PlacementRestriction/RestrictedObjectFilter","RequirementObjectFilter"}) {
            int group=path=="RequirementObjectFilter"?1:0;string[] parts=path.Split('/');string before="",after="";for(int i=0;i<parts.Length-1;++i){before+="<"+parts[i]+">";after="</"+parts[i]+">"+after;}
            string name=parts[parts.Length-1];Check(original,p,"",AssistXML(group,"",before+"<"+name+"/>"+after));
            foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AssistXML(group,"",before+"<"+name+" "+bad+"/>"+after));
            foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",AssistXML(group,"",before+"<"+name+">"+bad+"</"+name+">"+after));
            if(path!="HintObject")Reject(p,"",AssistXML(group,"",before+"<"+name+"/><"+name+"/>"+after));
            if(name=="ObjectFilter" || name=="RestrictedObjectFilter" || name=="RequirementObjectFilter") {
                foreach(string attrs in new[]{"id='MiXeD'","Rule='ANY' Relationship='ALL' Alignment='EVIL' Include='ALL' Exclude='AIRCRAFT' StatusBitFlags='ALL' StatusBitFlagsExclude='DAMAGED'","StatusBitFlags='' StatusBitFlagsExclude=''"})Check(original,p,"",AssistXML(group,"",before+"<"+name+" "+attrs+"><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></"+name+">"+after));
                foreach(int count in new[]{0,1,2,27,28,29,65}) {string things="";for(int i=0;i<count;++i)things+="<IncludeThing>A"+i+"</IncludeThing><ExcludeThing>B"+i+"</ExcludeThing>";Check(original,p,"",AssistXML(group,"",before+"<"+name+" Include='ALL' StatusBitFlags='ALL'>"+things+"</"+name+">"+after));}
            }
        }
        {byte[] b=Check(original,p,"",AssistXML(1,"PlacementHint='UNKNOWN' SnappingDistance='12.5'","<RequirementObjectFilter/>")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=8 || Word(b,at+12,big)!=0x41480000u || Word(b,at+16,big)!=at+20 || b.Length!=at+132)throw new Exception("Independent placement unknown bit3/float/nullable Filter");}
        {byte[] b=Check(original,p,"",AssistXML(2,"ObjectStatus='ALL' WeaponSlotID='0xffffffff' WeaponSlotType='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);for(int i=0;i<7;++i)if(Word(b,at+8+4*i,big)!=0xffffffffu)throw new Exception("Independent attack assist full status");if(Word(b,at+36,big)!=0xffffffffu || Word(b,at+40,big)!=5)throw new Exception("Independent attack assist uint/enum fallback");}
        {byte[] b=Check(original,p,"",AssistXML(3,"SpecialPowerTemplate='P\\23' ObjectStatus='ALL'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=24)throw new Exception("Independent special-power assist reference");for(int i=0;i<7;++i)if(Word(b,at+12+4*i,big)!=0xffffffffu)throw new Exception("Independent special-power assist full status");}
    }
}
