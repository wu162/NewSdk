using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static readonly string[] UtilityTypes={"783731100","880602396","2018429368","4216826759","344630372","2739727708","3032642002","3231835197","3052093933","1206381257","3040483501","3214000077","3833989627"};
    static readonly int[] UtilitySizes={12,16,16,16,12,12,12,12,20,20,28,40,36};
    static readonly uint[] UtilityParents={2769121946u,4043318533u,816016269u,4043318533u,402003752u,816016269u,2769121946u,4043318533u,1894105347u,2769121946u,4043318533u,4043318533u,4043318533u};
    static readonly uint[][] UtilityKeys={new uint[]{2122048096u},new uint[]{3580059422u,3615748891u},new uint[]{1589100344u},new uint[]{2688997143u,2384108347u},new uint[]{3586538858u},new uint[]{3319822471u},new uint[]{3562606791u},new uint[]{2525284163u},new uint[]{2284035418u,3672504870u,2871119420u,1530631961u,205651807u},new uint[]{384904913u,3246016559u},new uint[]{3218355687u,932610101u,3340962800u,726211492u,3196713972u},new uint[]{2505916104u,939681351u,13888185u,352194316u,2393042570u,2306052161u,4070628995u,2793371144u},new uint[]{271546182u,95434915u,1328662213u,3662769867u,162863551u,1148254404u,3223426280u}};
    static readonly uint[][] UtilityTypesOfFields={new uint[]{1982953380u},new uint[]{1683514611u,1683514611u},new uint[]{438907818u},new uint[]{1683514611u,2575722984u},new uint[]{4239460053u},new uint[]{165531635u},new uint[]{2011428087u},new uint[]{226629883u},new uint[]{2575722984u,2575722984u,2011428087u,2011428087u,2011428087u},new uint[]{729580760u,438907818u},new uint[]{3494305058u,1552515497u,219975299u,1982953380u,1683514611u},new uint[]{2011428087u,1683514611u,1683514611u,2575722984u,2575722984u,1683514611u,3494305058u,3494305058u},new uint[]{3494305058u,3494305058u,1683514611u,602296504u,2575722984u,2575722984u,219975299u}};
    static readonly Field[][] UtilityFields={new[]{new Field("MaxPlayerUpgrades","Unsigned")},new[]{new Field("TriggerWidthX","Float"),new Field("TriggerWidthY","Float")},new[]{new Field("DockingBone","String")},new[]{new Field("Opacity","Float"),new Field("Duration","Time")},new[]{new Field("Model","Reference")},new[]{new Field("AttributeModifier","Reference")},new[]{new Field("ConsumeItem","Boolean")},new[]{new Field("StanceTemplate","Reference")},new[]{new Field("CrushAnimTime","Time"),new Field("CrushKillDelay","Time"),new Field("UseDirectionCheck","Boolean"),new Field("UseVehicleStyleCrush","Boolean"),new Field("AllowAllyCrush","Boolean")},new[]{new Field("RestrictionType","Enum"),new Field("AreaName","String")},new[]{new Field("DetonationWeapon","Reference"),new Field("DetonatedBy","Bits"),new Field("DetonationFX","Reference"),new Field("NumVirtualMines","Unsigned"),new Field("RepeatDetonateMoveThresh","Float")},new[]{new Field("CrusherModifiesVelocity","Boolean"),new Field("LiftVelocity","Float"),new Field("MaxElevationFromGround","Float"),new Field("TimeIncrement","Time"),new Field("MaxTimeLifted","Time"),new Field("MaxStructureShakeVelocity","Float"),new Field("AirplaneCrashWeapon","Reference"),new Field("SoftLandingWeapon","Reference")},new[]{new Field("WeaponName","Reference"),new Field("InitialWeaponName","NullableReference"),new Field("Radius","Float"),new Field("NumBomblets","Signed"),new Field("MinDelay","Time"),new Field("MaxDelay","Time"),new Field("BombletFX","Reference")}};
    static readonly int[][] UtilityOffsets={new[]{8},new[]{8,12},new[]{8},new[]{8,12},new[]{8},new[]{8},new[]{8},new[]{8},new[]{8,12,16,17,18},new[]{8,12},new[]{8,12,16,20,24},new[]{36,8,12,16,20,24,28,32},new[]{8,12,16,20,24,28,32}};
    static string UtilityXML(int g,string attrs="",string child="") {return "<Behaviors><ObjectSMCHelper TypeId='"+UtilityTypes[g]+"' id='Utility' "+attrs+">"+child+"</ObjectSMCHelper></Behaviors>";}
    static void UtilityTokens(byte[] b,int g,int at,bool big) {
        int t=MotionTable(b,big);AIUpdateSentinel(b,t,1+UtilityKeys[g].Length);
        if(Word(b,t,big)!=1 || Word(b,t+4,big)!=UtilityParents[g])throw new Exception("Independent utility exact Behavior/Update/Create Token parent");
        for(int j=0;j<UtilityKeys[g].Length;++j)if(Word(b,t+12*(j+1),big)!=UtilityKeys[g][j] || Word(b,t+12*(j+1)+4,big)!=UtilityTypesOfFields[g][j] || Word(b,t+12*(j+1)+8,big)!=(g==12 && UtilityOffsets[g][j]==12?Word(b,at+12,big):(uint)(at+UtilityOffsets[g][j])))throw new Exception("Independent utility complete Token fields");
    }
    static void UtilityFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        for(int g=0;g<UtilityTypes.Length;++g) {
            var result=Check(original,p,"",UtilityXML(g));byte[] b=result.InstanceData;int at=LifecycleAt(b,big);
            if(b.Length!=at+UtilitySizes[g] || Word(b,at,big)!=uint.Parse(UtilityTypes[g]) || result.ImportsData.Length!=0)throw new Exception("Independent utility native size/type/imports");
            for(int i=8;i<UtilitySizes[g];++i) {
                int pos=i-i%4;uint word=0;
                if(g==0 && pos==8)word=9999u;
                if(g==3)word=pos==8?0x3f000000u:pos==12?0x40a00000u:0u;
                if(g==8)word=pos==8?0x3f800000u:pos==12?0x3f000000u:0u;
                if(g==10 && pos==24)word=0x40a00000u;
                if(g==11 && pos==20)word=0x3f800000u;
                byte value=(byte)(word>>(8*(big?3-i%4:i%4)));
                if((g==6 && i==8) || (g==8 && i==16))value=1;
                if(b[at+i]!=value)throw new Exception("Independent utility complete defaults/padding group "+g+" byte "+i);
            }
            UtilityTokens(b,g,at,big);
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+UtilityTypes[g]+"' id='U'"));
            foreach(Field f in UtilityFields[g])foreach(string value in UtilitySamples(f))Check(original,p,"",UtilityXML(g,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","SpecialPowerTemplate='P'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",UtilityXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<ObjectFilter/>","<CustomAnimAndDuration/>","<DockingBone/>","<Duration/>"})Reject(p,"",UtilityXML(g,"",bad));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string rows="";for(int i=0;i<count;++i)rows+="<ObjectSMCHelper TypeId='"+UtilityTypes[g]+"' id='U"+i+"' "+(g==2?"DockingBone='abcd'":"")+"/>";
                Check(original,p,"Browser='a bb'","<Behaviors>"+rows+"<ObjectSMCHelper TypeId='3925677257' id='Parent'/></Behaviors>");
            }
        }
        foreach(int stringGroup in new[]{2,9})foreach(int length in new[]{0,1,2,3,4,5,27,28,29,65}) {
            string value=new string('x',length);byte[] b=Check(original,p,"Browser='a bb'",UtilityXML(stringGroup,(stringGroup==2?"DockingBone":"AreaName")+"='"+value+"'")).InstanceData;int at=LifecycleAt(b,big),slot=stringGroup==2?8:12,end=at+UtilitySizes[stringGroup];
            if(Word(b,at+slot,big)!=length || Word(b,at+slot+4,big)!=end || b.Length!=end+((length+4)&~3))throw new Exception("Independent utility String8 length/allocation/padding");
            for(int i=0;i<length;++i)if(b[end+i]!=(byte)'x')throw new Exception("Independent utility string bytes");for(int i=length;i<((length+4)&~3);++i)if(b[end+i]!=0)throw new Exception("Independent utility string terminator/padding");UtilityTokens(b,stringGroup,at,big);
        }
        foreach(int g in new[]{4,5,7}) {
            string name=g==4?"Model":g==5?"AttributeModifier":"StanceTemplate";byte[] b=Check(original,p,"",UtilityXML(g,name+"='R\\23'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=24)throw new Exception("Independent utility strong reference index");
        }
        foreach(string value in new[]{"","Name","R\\0","R\\23"}) {
            byte[] b=Check(original,p,"",UtilityXML(12,"InitialWeaponName='"+Escape(value)+"'")).InstanceData;int at=LifecycleAt(b,big),pointer=(int)Word(b,at+12,big);
            if(pointer!=at+36 || b.Length!=at+40 || Word(b,pointer,big)!=(value=="R\\0"?1u:value=="R\\23"?24u:0u))throw new Exception("Independent nullable weapon reference4 presence/allocation/value");UtilityTokens(b,12,at,big);
        }
        {byte[] b=Check(original,p,"",UtilityXML(9,"RestrictionType='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=1)throw new Exception("Independent restriction enum unknown1");}
        {byte[] b=Check(original,p,"",UtilityXML(10,"DetonatedBy='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+12,big)!=1)throw new Exception("Independent relationship bit unknown0");}
        var a=Check(original,p,"",UtilityXML(1,"TriggerWidthX='17' TriggerWidthY='-23'"));var z=Check(original,p,"",UtilityXML(1,"TriggerWidthY='-23' TriggerWidthX='17'"));Equal(a.InstanceData,z.InstanceData,"Utility attribute order",false);Equal(a.RelocationData,z.RelocationData,"Utility relocation order",false);
    }
    static string[] UtilitySamples(Field f) {
        if(f.Name=="RestrictionType")return new[]{"1","2","UNRESTRICTED","","UNKNOWN","unrestricted","0","-1","1 2"," 1","1 "};
        if(f.Name=="DetonatedBy")return new[]{"ALLIES","ENEMIES","NEUTRAL","SAME_PLAYER","","ALL","all","UNKNOWN","unknown","ALL -ALL -ALL","ALLIES -ALLIES -ALLIES","ENEMIES -ENEMIES -ENEMIES","NEUTRAL -NEUTRAL -NEUTRAL","SAME_PLAYER -SAME_PLAYER -SAME_PLAYER","UNKNOWN -UNKNOWN -UNKNOWN","ALL -UNKNOWN","+ALL","-ALL","+","-"};
        return AIUpdateSamples(f.Kind=="NullableReference"?new Field(f.Name,"Reference"):f);
    }
}
