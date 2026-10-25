using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] AbilityStateTypes={"72607304","1067163377","2603081264","2431669529","3455002493","2434933121","2000137321"};
    static int[] AbilityStateSizes={252,260,260,256,264,260,260};
    static string AbilityStateXML(int g,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AbilityStateTypes[g]+"' id='AS' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static string[] AbilityStateReplaceSamples={"CHECK_BUILD_ASSISTANT","CLEAR_LOCATION","COPY_TARGET_OBJECT","COPY_UPGRADES","DISABLE_DURING_REPLACE","DISABLE_NEW_OBJECT_DURING_UNPACK","IGNORE_TERRAIN_RESTRICTIONS","","ALL","all","+ALL","-ALL","-COPY_UPGRADES","COPY_UPGRADES -COPY_UPGRADES -COPY_UPGRADES","UNKNOWN","unknown","copy_upgrades","COPY_UPGRADES ALL -COPY_UPGRADES","ALL -COPY_UPGRADES","+","-","COPY_UPGRADES,UNKNOWN"," COPY_UPGRADES  CLEAR_LOCATION ","-UNKNOWN -UNKNOWN"};
    static Field[][] AbilityStateFields={
        new[]{new Field("CommandButton","String"),new Field("UpgradeToGive","Reference")},
        new[]{new Field("UpdateModuleId","AssetId"),new Field("TimeStepsBack","Time"),new Field("LeavePositionFX","Reference"),new Field("ReturnPositionFX","Reference"),new Field("PostReconstituteAttributeModifier","Reference")},
        new[]{new Field("DominateRadius","Float"),new Field("TriggerFX","Reference"),new Field("DominatedFX","Reference"),new Field("PermanentlyConvert","Boolean")},
        new[]{new Field("Experience","Signed"),new Field("RadiusEffect","Float"),new Field("LevelFX","Reference")},
        new[]{new Field("MinDestinationRadius","Float"),new Field("MaxDestinationRadius","Float"),new Field("TriggerAttributeModifierOnClones","Reference"),new Field("TriggerFX","Reference"),new Field("TargetFX","Reference"),new Field("ClonedObjectLifetime","Time")},
        new[]{new Field("NewObjectUnpackTime","Time"),new Field("ReplaceOptions","Bits"),new Field("ClearTriggerDistance","Float")},
        new[]{new Field("NewObjectAssemblyTime","Time"),new Field("BusyDuringAssembly","Boolean"),new Field("ModelConditionsOnAssembly","Enum")}
    };
    static void AbilityStateFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;byte[] parent=Check(original,p,"",SpecialAbilityXML(0)).InstanceData;int parentAt=LifecycleAt(parent,big);
        for(int g=0;g<7;++g) {
            var def=Check(original,p,"",AbilityStateXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big),t=MotionTable(b,big),size=AbilityStateSizes[g];
            if(b.Length!=at+size || Word(b,at,big)!=uint.Parse(AbilityStateTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent ability state native size/type/public imports");
            for(int i=8;i<240;++i)if(b[at+i]!=parent[parentAt+i])throw new Exception("Independent ability state complete parent240 defaults");
            byte[] golden=new byte[size-240];Action<int,uint> word=(offset,value)=>{byte[] bytes=BitConverter.GetBytes(value);if(big)Array.Reverse(bytes);Array.Copy(bytes,0,golden,offset-240,4);};
            if(g==5){word(244,8);word(248,0x42f00000u);}if(g==6)word(244,0xffffffffu);
            for(int i=240;i<size;++i)if(b[at+i]!=golden[i-240])throw new Exception("Independent state own defaults/padding "+g+"/"+i);
            AIUpdateSentinel(b,t,g==0?3:g==1?6:g==2?6:g==3?5:g==4?7:5);if(Word(b,t,big)!=1 || Word(b,t+4,big)!=2891550594u)throw new Exception("Independent state SpecialAbility parent Token");int pt=(int)Word(b,t+8,big);AIUpdateSentinel(b,pt,47);if(Word(b,pt+4,big)!=4043318533u)throw new Exception("Independent state Update grandparent Token");
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+AbilityStateTypes[g]+"' id='AS'"));
            foreach(Field f in AbilityStateFields[g])foreach(string v in f.Name=="ReplaceOptions"?AbilityStateReplaceSamples:f.Name=="ModelConditionsOnAssembly"?SpecialPowerSingleModelNames:AbilityWeaponSamples(f))Check(original,p,"",AbilityStateXML(g,f.Name+"='"+Escape(v)+"'"));
            foreach(Field f in SpecialAbilityFields)foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityStateXML(g,f.Name+"='"+Escape(v)+"'"));
            foreach(string f in new[]{"AnimState","Frames","Trigger"})foreach(string v in f=="AnimState"?SpecialPowerSingleModelNames:AIUpdateSamples(new Field(f,"Unsigned")))Check(original,p,"",AbilityStateXML(g,"","<CustomAnimAndDuration "+f+"='"+Escape(v)+"'/>"));
            foreach(string bad in new[]{"Unknown='1'","DisabledTypesToIgnore='ALL'","StartsPaused='true'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AbilityStateXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<InitiateSound/>","<CustomAnimAndDuration/><CustomAnimAndDuration/>","<CustomAnimAndDuration Unknown='1'/>","<CustomAnimAndDuration xmlns='foreign'/>","<CustomAnimAndDuration>text</CustomAnimAndDuration>","<CustomAnimAndDuration><Unknown/></CustomAnimAndDuration>"})Reject(p,"",AbilityStateXML(g,"",bad));
            foreach(int other in new[]{0,1,2,3,4,5,6})if(other!=g)Reject(p,"",AbilityStateXML(g,AbilityStateFields[other][0].Name+"='1'"));
            string own=g==0?"CommandButton='Bone' UpgradeToGive='U\\23'":g==1?"UpdateModuleId='MiXeD' TimeStepsBack='2s' LeavePositionFX='L\\23' ReturnPositionFX='R\\1' PostReconstituteAttributeModifier='A\\2'":g==2?"DominateRadius='-2.5' TriggerFX='F\\23' DominatedFX='D\\1' PermanentlyConvert='true'":g==3?"Experience='-1' RadiusEffect='-2.5' LevelFX='F\\23'":g==4?"MinDestinationRadius='-2.5' MaxDestinationRadius='3' TriggerAttributeModifierOnClones='A\\23' TriggerFX='F\\1' TargetFX='T\\2' ClonedObjectLifetime='4s'":g==5?"NewObjectUnpackTime='2s' ReplaceOptions='ALL' ClearTriggerDistance='-2.5'":"NewObjectAssemblyTime='2s' BusyDuringAssembly='true' ModelConditionsOnAssembly='UNKNOWN'";
            string node=g==2?"<AttributeModifierAffects/>":g==3?"<AcceptanceFilter/>":g==5?"<ReplacementTemplate>MiXeD</ReplacementTemplate><ReplacementTemplate>mixed</ReplacementTemplate>":g==6?"<StructureTemplate>MiXeD</StructureTemplate><StructureTemplate>mixed</StructureTemplate>":"";
            string attrs=own+" SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH' SpecialPowerTemplate='P\\3' Options='ALL' RequiredConditions='ALL' SetObjectStatusOnTrigger='ALL' ClearObjectStatusOnExit='ALL'",anim="<CustomAnimAndDuration AnimState='INVALID' Frames='16' Trigger='2'/>";
            var combined=Check(original,p,"",AbilityStateXML(g,attrs,anim+node));b=combined.InstanceData;at=LifecycleAt(b,big);int first=at+size;
            if(g==0){if(Word(b,at+240,big)!=4 || Word(b,at+244,big)!=first || Word(b,at+248,big)!=24)throw new Exception("Independent Give string/strong Upgrade order");first+=8;}
            if(g==1 && (Word(b,at+244,big)!=0x40000000u || Word(b,at+248,big)!=24 || Word(b,at+252,big)!=2 || Word(b,at+256,big)!=3))throw new Exception("Independent Reconstitute time/ref order");
            if(g==2 && (Word(b,at+240,big)!=0xc0200000u || Word(b,at+244,big)!=24 || Word(b,at+248,big)!=2 || b[at+256]!=1))throw new Exception("Independent Dominate float/ref/boolean");
            if(g==3 && (Word(b,at+240,big)!=0xffffffffu || Word(b,at+244,big)!=0xc0200000u || Word(b,at+248,big)!=24))throw new Exception("Independent Level signed/float/ref");
            if(g==4 && (Word(b,at+240,big)!=0xc0200000u || Word(b,at+244,big)!=0x40400000u || Word(b,at+248,big)!=24 || Word(b,at+252,big)!=2 || Word(b,at+256,big)!=3 || Word(b,at+260,big)!=0x40800000u))throw new Exception("Independent Clone scalar/ref/time order");
            if(g==2 || g==3){if(Word(b,at+252,big)!=first)throw new Exception("Independent nullable filter allocation order");first+=112;}
            if(g==5 || g==6){int off=g==5?252:248;if(Word(b,at+off,big)!=2 || Word(b,at+off+4,big)!=first || Word(b,first,big)!=Word(b,first+4,big))throw new Exception("Independent typed GameObject list and case-folded IDs");int list=(int)Word(b,Token(b,MotionTable(b,big),g==5?1160307140u:948596956u,big)+8,big);if(Word(b,list,big)!=1820487795u || Word(b,list+8,big)!=at+off || Word(b,list+12,big)!=2974916843u || Word(b,list+16,big)!=0 || Word(b,list+20,big)!=first || Word(b,list+32,big)!=first+4)throw new Exception("Independent typed list direct-item Tokens");first+=8;}
            if(g==5 && (Word(b,at+240,big)!=0x40000000u || Word(b,at+244,big)!=0xffffffffu || Word(b,at+248,big)!=0xc0200000u))throw new Exception("Independent Replace options full-word ALL/time");
            if(g==6 && (Word(b,at+240,big)!=0x40000000u || Word(b,at+244,big)!=233 || b[at+256]!=1 || b[at+257]!=0 || b[at+258]!=0 || b[at+259]!=0))throw new Exception("Independent Assemble single model enum and padding");
            if(Word(b,at+68,big)!=first || Word(b,at+200,big)!=first+4 || Word(b,at+232,big)!=first+12 || b.Length!=first+24)throw new Exception("Independent state own/filter/list then parent allocation order");
            var reverse=Check(original,p,"",AbilityStateXML(g,attrs.Replace("SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'","ContactPointOverride='EFGH' SpecialObjectAttachToBone='abc'"),node+anim.Replace("AnimState='INVALID' Frames='16' Trigger='2'","Trigger='2' Frames='16' AnimState='INVALID'")));Equal(combined.InstanceData,reverse.InstanceData,"State schema allocation order",false);Equal(combined.RelocationData,reverse.RelocationData,"State relocation order",false);Equal(combined.ImportsData,reverse.ImportsData,"State reference order",false);
            foreach(int count in new[]{0,1,2,27,28,29,65}){string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+AbilityStateTypes[g]+"' id='AS"+i+"' "+attrs+">"+anim+node+"</ObjectSMCHelper>";Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<ObjectSMCHelper TypeId='2891550594' id='Parent'/></Behaviors>");}
        }
        foreach(int g in new[]{2,3}) {
            string filter=g==2?"AttributeModifierAffects":"AcceptanceFilter";
            foreach(string attrs in new[]{"","id='MiXeD'","Rule='ANY' Relationship='ALL' Alignment='EVIL' Include='ALL' Exclude='AIRCRAFT' StatusBitFlags='ALL' StatusBitFlagsExclude='DAMAGED'","Rule='UNKNOWN' Relationship='UNKNOWN' Alignment='UNKNOWN' Include='' Exclude='' StatusBitFlags='' StatusBitFlagsExclude=''"})Check(original,p,"",AbilityStateXML(g,"","<"+filter+" "+attrs+"><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></"+filter+">"));
            foreach(string bad in new[]{"<"+filter+"/><"+filter+"/>","<"+filter+" Unknown='1'/>","<"+filter+" xmlns='foreign'/>","<"+filter+">text</"+filter+">","<"+filter+"><Unknown/></"+filter+">","<"+filter+"><IncludeThing Unknown='1'/></"+filter+">","<"+filter+"><IncludeThing><Unknown/></IncludeThing></"+filter+">"})Reject(p,"",AbilityStateXML(g,"",bad));
        }
        foreach(int g in new[]{5,6}) {
            string name=g==5?"ReplacementTemplate":"StructureTemplate";
            foreach(int count in new[]{0,1,2,27,28,29,65}){string children="";for(int i=0;i<count;++i)children+="<"+name+">"+(i%2==0?"MiXeD":"Other")+"</"+name+">";Check(original,p,"",AbilityStateXML(g,"SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'",children+"<CustomAnimAndDuration/>"));}
            foreach(string v in new[]{"","MiXeD","mixed","with spaces","中文","é","A\\23"})Check(original,p,"",AbilityStateXML(g,"","<"+name+">"+Escape(v)+"</"+name+">"));
            foreach(string bad in new[]{"<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+"><Unknown/></"+name+">"})Reject(p,"",AbilityStateXML(g,"",bad));
        }
        foreach(string v in AbilityStateReplaceSamples){byte[] b=Check(original,p,"",AbilityStateXML(5,"ReplaceOptions='"+Escape(v)+"'" )).InstanceData;uint bits=Word(b,LifecycleAt(b,big)+244,big);if(v=="UNKNOWN" && bits!=16)throw new Exception("Independent ReplaceOptions fallback bit4");if((v=="ALL" || v=="ALL -COPY_UPGRADES") && bits!=0xffffffffu)throw new Exception("Independent ReplaceOptions ALL full-word/early exit");if(v=="COPY_UPGRADES -COPY_UPGRADES -COPY_UPGRADES" && bits!=4)throw new Exception("Independent ReplaceOptions XOR");}
        foreach(string v in new[]{"","a","AB","abc","abcd","abcde","MiXeD","中文","é"})Check(original,p,"",AbilityStateXML(0,"CommandButton='"+Escape(v)+"' SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'","<CustomAnimAndDuration/>"));
        var upper=Check(original,p,"",AbilityStateXML(1,"UpdateModuleId='MiXeD'"));var lower=Check(original,p,"",AbilityStateXML(1,"UpdateModuleId='mixed'"));Equal(upper.InstanceData,lower.InstanceData,"Module typed ID ASCII case-folding",false);
    }
}
