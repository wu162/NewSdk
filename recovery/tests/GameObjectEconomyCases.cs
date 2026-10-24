using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] EconomyTypes={"3651459681","2384596706","2432276463","3270634528","3593042897","642696730","3859925230"};
    static Field[][] EconomyFields={
        new Field[]{new Field("NumberApproachPositions","Signed"),new Field("AllowsPassthrough","Boolean"),new Field("GoToRallyPointAfterDock","Boolean"),new Field("ShouldDockInReverse","Boolean"),new Field("MinDockTime","Time"),new Field("ObjectsInLineWeight","Time")},
        new Field[]{new Field("MaximumGatheredValue","Unsigned"),new Field("DeliveryAmount","Unsigned"),new Field("DeliveryAmountWhenEmpty","Unsigned"),new Field("CashMultiplier","Percent"),new Field("MoveToExtractTime","Time"),new Field("ExtractTime","Time"),new Field("MoveToDeliverTime","Time"),new Field("DeliveryTime","Time"),new Field("ExtractAnimation","Enum"),new Field("DeliveryAnimation","Enum"),new Field("ReturnAnimation","Enum"),new Field("MoveBackAnimation","Enum"),new Field("LowNotificationPercentage","Percent"),new Field("LowNotificationEvaEventRef","Reference"),new Field("DepletedNotificationEvaEventRef","Reference"),new Field("DepletedNotificationAudioEventInfoRef","Reference")},
        new Field[]{new Field("DeliveryCapacity","Unsigned"),new Field("DeliveryTime","Time")},
        new Field[]{new Field("ValueMultiplier","Float"),new Field("DistributedDeposit","Boolean")},
        new Field[]{new Field("StartingBoxes","Signed"),new Field("DeleteWhenEmpty","Boolean")},
        new Field[]{new Field("SlaveTemplate","Reference"),new Field("Radius","Float"),new Field("Amount","Unsigned")},
        new Field[]{new Field("Lifetime","Time"),new Field("Slots","Signed"),new Field("ScanTime","Time"),new Field("HeightOffGround","Float"),new Field("EffectDistance","Float"),new Field("LiftOffGroundDistance","Float"),new Field("GatherAcceleration","Float")},
    };
    static string EconomyXML(int group,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+EconomyTypes[group]+"' id='Economy' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static bool EconomyDock(int group) {return group==0 || group==3 || group==4;}
    static string EconomyPayload(int group) {
        if(EconomyDock(group))return "<ForVoiceRetreatThisIsASafeHarborToObjectFilter Include='ALL' Exclude='AIRCRAFT' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></ForVoiceRetreatThisIsASafeHarborToObjectFilter>";
        if(group==2)return "<ObjectStatusValidation RequiredStatus='ALL' ForbiddenStatus='DAMAGED'/>";
        if(group==6)return "<RotationSpeed x='1' y='2' z='3'/><UnitSpacing x='4' y='5' z='6'/><Filter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></Filter>";
        return "";
    }
    static void EconomyFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;int[] sizes={28,72,20,36,36,20,172},tokens={8,17,4,3,3,4,11};
        for(int group=0;group<7;++group) {
            byte[] b=Check(original,p,"",EconomyXML(group)).InstanceData;int at=LifecycleAt(b,big),table=MotionTable(b,big);
            if(b.Length!=at+sizes[group] || Word(b,at,big)!=uint.Parse(EconomyTypes[group]))throw new Exception("Independent economy native type/size");AIUpdateSentinel(b,table,tokens[group]);
            if(group==3 || group==4) {if(Word(b,table,big)!=1 || Word(b,table+4,big)!=3651459681u)throw new Exception("Independent supply Dock parent layer");AIUpdateSentinel(b,(int)Word(b,table+8,big),8);}
            if(EconomyDock(group) && (Word(b,at+16,big)!=0x41500000u || Word(b,at+20,big)!=0 || Word(b,at+24,big)!=0))throw new Exception("Independent Dock defaults/nullable Filter/packed booleans");
            if(group==1) {if(Word(b,at+8,big)!=20000 || Word(b,at+12,big)!=1000 || Word(b,at+16,big)!=0 || Word(b,at+20,big)!=0x3f800000u)throw new Exception("Independent OreNode defaults");for(int i=24;i<=36;i+=4)if(Word(b,at+i,big)!=0x3f800000u)throw new Exception("Independent OreNode default times");}
            if(group==2 && (Word(b,at+8,big)!=1000 || Word(b,at+12,big)!=0x3f800000u || Word(b,at+16,big)!=0))throw new Exception("Independent OreMiner defaults/nullable status");
            if(group==3 && Word(b,at+28,big)!=0x3f800000u)throw new Exception("Independent SupplyCenter multiplier is plain float");
            if(group==4 && Word(b,at+28,big)!=1)throw new Exception("Independent SupplyWarehouse box default");
            if(group==6 && (Word(b,at+8,big)!=0x41200000u || Word(b,at+12,big)!=5 || Word(b,at+16,big)!=0x3e4ccccdu || Word(b,at+20,big)!=0x42480000u || Word(b,at+24,big)!=0x42fa0000u || Word(b,at+28,big)!=0x42960000u || Word(b,at+32,big)!=0x3c23d70au))throw new Exception("Independent GatherEject time/integer/float defaults");
            foreach(var container in ModuleContainers)Check(original,p,"",ModuleXML(container[0],container[1],"TypeId='"+EconomyTypes[group]+"' id='Ec'"));
            foreach(var fields in new[]{EconomyFields[group],(group==3 || group==4)?EconomyFields[0]:new Field[0]})foreach(var f in fields)foreach(string value in f.Kind=="Enum"?HealingModelValues:AIUpdateSamples(f))Check(original,p,"",EconomyXML(group,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",EconomyXML(group,bad));
            foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",EconomyXML(group,"",bad));
            string payload=EconomyPayload(group);
            b=Check(original,p,"Browser='a bb'",EconomyXML(group,"",payload)).InstanceData;at=LifecycleAt(b,big);
            if(EconomyDock(group) && Word(b,at+20,big)!=at+sizes[group])throw new Exception("Independent Dock nullable Filter native allocation");
            if(group==2) {int status=(int)Word(b,at+16,big);if(status!=at+20 || b.Length!=at+76)throw new Exception("Independent OreMiner status allocation");for(int i=0;i<7;++i)if(Word(b,status+4*i,big)!=0xffffffffu)throw new Exception("Independent OreMiner full required mask");}
            if(group==6 && (Word(b,at+36,big)!=0x3f800000u || Word(b,at+40,big)!=0x40000000u || Word(b,at+44,big)!=0x40400000u || Word(b,at+48,big)!=0x40800000u || Word(b,at+52,big)!=0x40a00000u || Word(b,at+56,big)!=0x40c00000u))throw new Exception("Independent GatherEject inline vectors");
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+EconomyTypes[group]+"' id='E"+i+"'>"+payload+"</ObjectSMCHelper>";
                Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<FireWeaponUpdate TypeId='2995535186'><FireWeaponNugget WeaponName='W\\23'><Offset/></FireWeaponNugget></FireWeaponUpdate></Behaviors>");
            }
            string filter=EconomyDock(group)?"ForVoiceRetreatThisIsASafeHarborToObjectFilter":group==6?"Filter":"";
            if(filter!="") {
                foreach(string attrs in new[]{"","id='MiXeD'","Rule='ANY' Relationship='ALL' Alignment='EVIL' Include='ALL' Exclude='AIRCRAFT' StatusBitFlags='ALL' StatusBitFlagsExclude='DAMAGED'","StatusBitFlags='' StatusBitFlagsExclude=''"})Check(original,p,"",EconomyXML(group,"","<"+filter+" "+attrs+"><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></"+filter+">"));
                foreach(int count in new[]{0,1,2,27,28,29,65}) {
                    string things="";for(int i=0;i<count;++i)things+="<IncludeThing>A"+i+"</IncludeThing><ExcludeThing>B"+i+"</ExcludeThing>";
                    Check(original,p,"",EconomyXML(group,"","<"+filter+" Include='ALL' StatusBitFlags='ALL'>"+things+"</"+filter+">"));
                }
            }
            string[] paths=EconomyDock(group)?new[]{filter}:group==2?new[]{"ObjectStatusValidation"}:group==6?new[]{"RotationSpeed","UnitSpacing","Filter"}:new string[0];
            foreach(string name in paths) {
                Check(original,p,"",EconomyXML(group,"","<"+name+"/>"));
                foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",EconomyXML(group,"","<"+name+" "+bad+"/>"));
                foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",EconomyXML(group,"","<"+name+">"+bad+"</"+name+">"));
                Reject(p,"",EconomyXML(group,"","<"+name+"/><"+name+"/>"));
            }
            if(group==2)foreach(string field in new[]{"RequiredStatus","ForbiddenStatus"})foreach(string value in WeaponSetStatusValues)Check(original,p,"",EconomyXML(group,"","<ObjectStatusValidation "+field+"='"+Escape(value)+"'/>"));
            if(group==6) {
                foreach(string node in new[]{"RotationSpeed","UnitSpacing"})foreach(string field in new[]{"x","y","z"})foreach(string value in AIUpdateSamples(new Field(field,"Float")))Check(original,p,"",EconomyXML(group,"","<"+node+" "+field+"='"+Escape(value)+"'/>"));
                string vectors="<RotationSpeed x='1' y='2' z='3'/><UnitSpacing x='4' y='5' z='6'/>",filters="<Filter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></Filter>";
                var a=Check(original,p,"",EconomyXML(group,"",vectors+filters));var c=Check(original,p,"",EconomyXML(group,"",filters+vectors));Equal(a.InstanceData,c.InstanceData,"Economy schema allocation order",false);Equal(a.RelocationData,c.RelocationData,"Economy schema relocations",false);Equal(a.ImportsData,c.ImportsData,"Economy schema imports",false);
            }
        }
        {byte[] b=Check(original,p,"",EconomyXML(1,"ExtractAnimation='UNKNOWN' DeliveryAnimation='ALL' ReturnAnimation='' MoveBackAnimation='MOVING' LowNotificationPercentage='25' LowNotificationEvaEventRef='A\\23' DepletedNotificationEvaEventRef='B\\7' DepletedNotificationAudioEventInfoRef='C\\1'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+40,big)!=233 || Word(b,at+44,big)!=0xffffffffu || Word(b,at+48,big)!=233 || Word(b,at+56,big)!=0x3e800000u || Word(b,at+60,big)!=24 || Word(b,at+64,big)!=8 || Word(b,at+68,big)!=2)throw new Exception("Independent OreNode enum/fallback/percent/reference offsets");}
        {byte[] b=Check(original,p,"",EconomyXML(0,"NumberApproachPositions='-17' AllowsPassthrough='true' GoToRallyPointAfterDock='true' ShouldDockInReverse='true'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=unchecked((uint)-17) || b[at+24]!=1 || b[at+25]!=1 || b[at+26]!=1 || b[at+27]!=0)throw new Exception("Independent Dock signed/packed boolean/padding");}
        {byte[] b=Check(original,p,"",EconomyXML(5,"SlaveTemplate='A\\23' Radius='12.5' Amount='0xffffffff'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+8,big)!=24 || Word(b,at+12,big)!=0x41480000u || Word(b,at+16,big)!=0xffffffffu)throw new Exception("Independent GatherSlaves strong reference/float/uint");}
        {byte[] b=Check(original,p,"",EconomyXML(4,"StartingBoxes='-17' DeleteWhenEmpty='true'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+28,big)!=unchecked((uint)-17) || b[at+32]!=1)throw new Exception("Independent warehouse signed boxes/boolean");}
    }
}
