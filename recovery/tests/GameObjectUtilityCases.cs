using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static readonly string[] UtilityTypes={"783731100","880602396","2018429368","4216826759"};
    static readonly int[] UtilitySizes={12,16,16,16};
    static readonly uint[] UtilityParents={2769121946u,4043318533u,816016269u,4043318533u};
    static readonly uint[][] UtilityKeys={new uint[]{2122048096u},new uint[]{3580059422u,3615748891u},new uint[]{1589100344u},new uint[]{2688997143u,2384108347u}};
    static readonly uint[][] UtilityTypesOfFields={new uint[]{1982953380u},new uint[]{1683514611u,1683514611u},new uint[]{438907818u},new uint[]{1683514611u,2575722984u}};
    static readonly Field[][] UtilityFields={new[]{new Field("MaxPlayerUpgrades","Unsigned")},new[]{new Field("TriggerWidthX","Float"),new Field("TriggerWidthY","Float")},new[]{new Field("DockingBone","String")},new[]{new Field("Opacity","Float"),new Field("Duration","Time")}};
    static string UtilityXML(int g,string attrs="",string child="") {return "<Behaviors><ObjectSMCHelper TypeId='"+UtilityTypes[g]+"' id='Utility' "+attrs+">"+child+"</ObjectSMCHelper></Behaviors>";}
    static void UtilityTokens(byte[] b,int g,int at,bool big) {
        int t=MotionTable(b,big);AIUpdateSentinel(b,t,1+UtilityKeys[g].Length);
        if(Word(b,t,big)!=1 || Word(b,t+4,big)!=UtilityParents[g])throw new Exception("Independent utility exact Behavior/Update/Create Token parent");
        for(int j=0;j<UtilityKeys[g].Length;++j)if(Word(b,t+12*(j+1),big)!=UtilityKeys[g][j] || Word(b,t+12*(j+1)+4,big)!=UtilityTypesOfFields[g][j] || Word(b,t+12*(j+1)+8,big)!=at+8+4*j)throw new Exception("Independent utility complete Token fields");
    }
    static void UtilityFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        for(int g=0;g<4;++g) {
            var result=Check(original,p,"",UtilityXML(g));byte[] b=result.InstanceData;int at=LifecycleAt(b,big);
            if(b.Length!=at+UtilitySizes[g] || Word(b,at,big)!=uint.Parse(UtilityTypes[g]) || result.ImportsData.Length!=0)throw new Exception("Independent utility native size/type/imports");
            uint first=g==0?9999u:g==3?0x3f000000u:0u,last=g==3?0x40a00000u:0u;
            if(Word(b,at+8,big)!=first || (g!=0 && Word(b,at+12,big)!=last))throw new Exception("Independent utility complete defaults");UtilityTokens(b,g,at,big);
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+UtilityTypes[g]+"' id='U'"));
            foreach(Field f in UtilityFields[g])foreach(string value in AIUpdateSamples(f))Check(original,p,"",UtilityXML(g,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","SpecialPowerTemplate='P'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",UtilityXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<ObjectFilter/>","<CustomAnimAndDuration/>","<DockingBone/>","<Duration/>"})Reject(p,"",UtilityXML(g,"",bad));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string rows="";for(int i=0;i<count;++i)rows+="<ObjectSMCHelper TypeId='"+UtilityTypes[g]+"' id='U"+i+"' "+(g==2?"DockingBone='abcd'":"")+"/>";
                Check(original,p,"Browser='a bb'","<Behaviors>"+rows+"<ObjectSMCHelper TypeId='3925677257' id='Parent'/></Behaviors>");
            }
        }
        foreach(int length in new[]{0,1,2,3,4,5,27,28,29,65}) {
            string value=new string('x',length);byte[] b=Check(original,p,"Browser='a bb'",UtilityXML(2,"DockingBone='"+value+"'")).InstanceData;int at=LifecycleAt(b,big);
            if(Word(b,at+8,big)!=length || Word(b,at+12,big)!=at+16 || b.Length!=at+16+((length+4)&~3))throw new Exception("Independent utility String8 length/allocation/padding");
            for(int i=0;i<length;++i)if(b[at+16+i]!=(byte)'x')throw new Exception("Independent utility string bytes");for(int i=length;i<((length+4)&~3);++i)if(b[at+16+i]!=0)throw new Exception("Independent utility string terminator/padding");UtilityTokens(b,2,at,big);
        }
        var a=Check(original,p,"",UtilityXML(1,"TriggerWidthX='17' TriggerWidthY='-23'"));var z=Check(original,p,"",UtilityXML(1,"TriggerWidthY='-23' TriggerWidthX='17'"));Equal(a.InstanceData,z.InstanceData,"Utility attribute order",false);Equal(a.RelocationData,z.RelocationData,"Utility relocation order",false);
    }
}
