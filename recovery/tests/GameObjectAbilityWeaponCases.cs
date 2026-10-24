using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] AbilityWeaponTypes={"507880339","915350037","2695497697","127626925"};
    static int[] AbilityWeaponSizes={272,276,272,296};
    static string AbilityWeaponXML(int g,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AbilityWeaponTypes[g]+"' id='AW' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static Field[][] AbilityWeaponFields={
        new[]{new Field("SpecialWeapon","Reference"),new Field("WhichSpecialWeapon","Signed"),new Field("SkipContinue","Boolean"),new Field("BusyForDuration","Time"),new Field("NeedLivingTargets","Boolean"),new Field("PlayWeaponPreFireFX","Boolean"),new Field("OffsetTargetLocationFromObject","Boolean"),new Field("WeaponSlotID","Signed"),new Field("WeaponSlotType","Enum"),new Field("DestealthClosestObjectMatchingFilter","Boolean"),new Field("ModelConditionsOnUnpack","Enum")},
        new[]{new Field("FireDelay","Time")},
        new[]{new Field("DamagePerSecond","Float"),new Field("JoinWithOtherBeams","Boolean"),new Field("DissapateWhenTargetDead","Boolean"),new Field("LineType","Boolean"),new Field("TargetAttributeModifierAdd","Reference"),new Field("TargetObjectStatusAdd","ObjectStatus"),new Field("SweepFX","Reference"),new Field("ReflectorExtendDistance","Float"),new Field("TargetSamePlayerOnly","Boolean"),new Field("PreferredTargetBone","String")},
        new[]{new Field("Status1","ObjectStatus"),new Field("Status2","ObjectStatus")}
    };
    static string[] AbilityWeaponSamples(Field f) {
        if(f.Name=="WeaponSlotType")return new[]{"NO_WEAPON","PRIMARY_WEAPON","SECONDARY_WEAPON","TERTIARY_WEAPON","QUATERNARY_WEAPON","QUINARY_WEAPON","","UNKNOWN","primary_weapon","0","-1","ALL","PRIMARY_WEAPON SECONDARY_WEAPON"," PRIMARY_WEAPON","PRIMARY_WEAPON "};
        if(f.Name=="ModelConditionsOnUnpack" || f.Name=="ActiveModelCondition")return SpecialPowerSingleModelNames;
        if(f.Kind=="ObjectStatus")return WeaponSetStatusValues;
        if(f.Name=="Options")return SpecialAbilityOptionsSamples;
        if(f.Name=="RequiredConditions" || f.Name=="RejectedConditions")return SpecialAbilityConditionsSamples;
        if(f.Name=="DisabledTypesToProcess" || f.Name=="DisabledTypesToContinueSoundsFor")return SpecialPowerEnumSamples[1];
        return AIUpdateSamples(f);
    }
    static void AbilityWeaponFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        byte[] parent=Check(original,p,"",SpecialAbilityXML(0)).InstanceData;int parentAt=LifecycleAt(parent,big);
        for(int g=0;g<4;++g) {
            var def=Check(original,p,"",AbilityWeaponXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big),t=MotionTable(b,big),size=AbilityWeaponSizes[g];
            if(b.Length!=at+size || Word(b,at,big)!=uint.Parse(AbilityWeaponTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent ability weapon native size/type/public imports");
            for(int i=8;i<240;++i)if(b[at+i]!=parent[parentAt+i])throw new Exception("Independent inherited complete SpecialAbility defaults");
            for(int i=240;i<size;++i){byte expected=0;if(g<2 && i==252+(big?3:0))expected=1;if(g<2 && i>=260 && i<264)expected=255;if(g==2 && i==269)expected=1;if(b[at+i]!=expected)throw new Exception("Independent own defaults/padding "+g+"/"+i);}
            int ownCount=g==0?12:g==1?2:g==2?11:3;AIUpdateSentinel(b,t,ownCount);
            if(Word(b,t,big)!=1 || Word(b,t+4,big)!=(g==1?507880339u:2891550594u))throw new Exception("Independent immediate ability parent Token");
            t=(int)Word(b,t+8,big);if(g==1){AIUpdateSentinel(b,t,12);if(Word(b,t+4,big)!=2891550594u)throw new Exception("Independent delayed WeaponFire/SpecialAbility Token chain");t=(int)Word(b,t+8,big);}
            AIUpdateSentinel(b,t,47);if(Word(b,t+4,big)!=4043318533u)throw new Exception("Independent ability Update parent Token");
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+AbilityWeaponTypes[g]+"' id='AW'"));
            foreach(Field f in AbilityWeaponFields[g])foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityWeaponXML(g,f.Name+"='"+Escape(v)+"'"));
            if(g==1)foreach(Field f in AbilityWeaponFields[0])foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityWeaponXML(g,f.Name+"='"+Escape(v)+"'"));
            foreach(Field f in SpecialAbilityFields)foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityWeaponXML(g,f.Name+"='"+Escape(v)+"'"));
            foreach(string f in new[]{"AnimState","Frames","Trigger"})foreach(string v in f=="AnimState"?SpecialPowerSingleModelNames:AIUpdateSamples(new Field(f,"Unsigned")))Check(original,p,"",AbilityWeaponXML(g,"","<CustomAnimAndDuration "+f+"='"+Escape(v)+"'/>"));
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","DisabledTypesToIgnore='ALL'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AbilityWeaponXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<InitiateSound/>","<DestealthClosestObjectMatchingFilter/>","<TargetObjectStatusAdd/>","<Status1/>","<CustomAnimAndDuration/><CustomAnimAndDuration/>","<CustomAnimAndDuration Unknown='1'/>","<CustomAnimAndDuration xmlns='foreign'/>","<CustomAnimAndDuration>text</CustomAnimAndDuration>","<CustomAnimAndDuration><Unknown/></CustomAnimAndDuration>"})Reject(p,"",AbilityWeaponXML(g,"",bad));
            if(g!=1)Reject(p,"",AbilityWeaponXML(g,"FireDelay='1s'"));if(g!=2)Reject(p,"",AbilityWeaponXML(g,"DamagePerSecond='1'"));if(g!=3)Reject(p,"",AbilityWeaponXML(g,"Status1='ALL'"));if(g>=2)Reject(p,"",AbilityWeaponXML(g,"SpecialWeapon='W'"));
            string attrs="SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH' SpecialPowerTemplate='P\\23' SetObjectStatusOnTrigger='ALL' ClearObjectStatusOnExit='ALL' Options='ALL' RequiredConditions='ALL'",anim="<CustomAnimAndDuration AnimState='INVALID' Frames='16' Trigger='2'/>";
            if(g<2)attrs+=" SpecialWeapon='W\\23' WhichSpecialWeapon='-1' WeaponSlotID='-2' WeaponSlotType='QUINARY_WEAPON' BusyForDuration='2s' SkipContinue='true' NeedLivingTargets='true' PlayWeaponPreFireFX='true' OffsetTargetLocationFromObject='true' DestealthClosestObjectMatchingFilter='true' ModelConditionsOnUnpack='INVALID'"+(g==1?" FireDelay='3s'":"");
            if(g==2)attrs+=" DamagePerSecond='23' TargetAttributeModifierAdd='A\\23' TargetObjectStatusAdd='ALL' SweepFX='F\\23' ReflectorExtendDistance='-2.5' PreferredTargetBone='Bone' JoinWithOtherBeams='true' DissapateWhenTargetDead='false' LineType='true' TargetSamePlayerOnly='true'";
            if(g==3)attrs+=" Status1='ALL' Status2='ALL'";
            var combined=Check(original,p,"",AbilityWeaponXML(g,attrs,anim));b=combined.InstanceData;at=LifecycleAt(b,big);int first=at+size;
            if(g==2){int status=(int)Word(b,at+248,big);if(status!=first || Word(b,at+260,big)!=4 || Word(b,at+264,big)!=first+28 || Word(b,at+240,big)!=0x41b80000u || Word(b,at+256,big)!=0xc0200000u || b[at+268]!=1 || b[at+269]!=0 || b[at+270]!=1 || b[at+271]!=1)throw new Exception("Independent beam scalar/booleans/nullable223/String8");for(int i=0;i<7;++i)if(Word(b,status+4*i,big)!=0xffffffffu)throw new Exception("Independent nullable223 full word padding");int tok=Token(b,MotionTable(b,big),3948109203u,big);if(Word(b,tok+8,big)!=status)throw new Exception("Independent nullable status Token points to value");first+=36;}
            if(Word(b,at+68,big)!=first || Word(b,at+200,big)!=first+4 || Word(b,at+232,big)!=first+12 || b.Length!=first+24)throw new Exception("Independent derived/parent string/Anim12 schema allocation order");
            if(g<2){if(Word(b,at+240,big)!=24 || Word(b,at+244,big)!=0xffffffffu || Word(b,at+252,big)!=0xfffffffeu || Word(b,at+256,big)!=4 || Word(b,at+260,big)!=0xffffffffu)throw new Exception("Independent fire strong ref/signed/single enums");for(int i=264;i<=268;++i)if(b[at+i]!=1)throw new Exception("Independent fire packed booleans");for(int i=269;i<272;++i)if(b[at+i]!=0)throw new Exception("Independent fire padding");}
            if(g==3)for(int i=0;i<14;++i)if(Word(b,at+240+4*i,big)!=0xffffffffu)throw new Exception("Independent two complete inline223 masks");
            var reversed=Check(original,p,"",AbilityWeaponXML(g,attrs.Replace("SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'","ContactPointOverride='EFGH' SpecialObjectAttachToBone='abc'"),anim.Replace("AnimState='INVALID' Frames='16' Trigger='2'","Trigger='2' Frames='16' AnimState='INVALID'")));Equal(combined.InstanceData,reversed.InstanceData,"Ability weapon attribute order",false);Equal(combined.RelocationData,reversed.RelocationData,"Ability weapon relocation order",false);Equal(combined.ImportsData,reversed.ImportsData,"Ability weapon reference order",false);
            foreach(int count in new[]{0,1,2,27,28,29,65}){string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+AbilityWeaponTypes[g]+"' id='AW"+i+"' "+attrs+">"+anim+"</ObjectSMCHelper>";Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<ObjectSMCHelper TypeId='2891550594' id='Parent'/></Behaviors>");}
        }
        foreach(int g in new[]{0,1}){byte[] b=Check(original,p,"",AbilityWeaponXML(g,"WeaponSlotType='UNKNOWN' ModelConditionsOnUnpack='UNKNOWN' DestealthClosestObjectMatchingFilter='true'" )).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+256,big)!=5 || Word(b,at+260,big)!=233 || b[at+268]!=1)throw new Exception("Independent single enum unknowns and misleading Filter boolean");}
        foreach(string value in new[]{"","ALL","UNKNOWN","DAMAGED -DAMAGED -DAMAGED"})Check(original,p,"",AbilityWeaponXML(2,"TargetObjectStatusAdd='"+value+"' PreferredTargetBone='' SpecialObjectAttachToBone='' ContactPointOverride=''","<CustomAnimAndDuration/>"));
    }
}
