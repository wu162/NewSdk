using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] AIUpdateTypes={"148619278","1149685576","1440184067","2595752269","3197949694","3444991387","3625168682","3996180436"};
    static string[] AIUpdateNames={"AIUpdate","JetAIUpdate","HordeAIUpdate","SupplyTruckAIUpdate","TransportAIUpdate","AnimalAIUpdate","WanderAIUpdate","DeployStyleAIUpdate"};
    static Field[][] AIUpdateFields={
        new Field[]{new Field("AttackPriority","String"),new Field("AutoAcquireEnemiesWhenIdle","Bits"),new Field("StopChaseDistance","Float"),new Field("StandGround","Boolean"),new Field("MinCowerTime","Time"),new Field("MaxCowerTime","Time"),new Field("BurningDeathTime","Time"),new Field("ComboLocoAttackDistance","Float"),new Field("SpecialContactPoints","StringList"),new Field("FadeOnPortals","Boolean"),new Field("IdleTargetTime","Time"),new Field("MaxCombineChildren","Unsigned"),new Field("SpawnOffsetRadius","Float"),new Field("StateMachine","Enum"),new Field("DefaultLocomotorSet","Enum")},
        new Field[]{new Field("OutOfAmmoDamagePerSecond","Percent"),new Field("OutOfAmmoDamageDelay","Time"),new Field("NeedsRunway","Boolean"),new Field("ReturnToBaseWhenVictimDead","Boolean"),new Field("KeepsParkingSpaceWhenAirborne","Boolean"),new Field("ParkingOffset","Float"),new Field("CirclesForAttack","Boolean"),new Field("UseForOutOfAmmoCheck","Reference"),new Field("OutOfAmmoEvaEvent","Reference")},
        new Field[]{},
        new Field[]{new Field("MaxBoxes","Unsigned"),new Field("SupplyCenterActionDelay","Time"),new Field("SupplyWarehouseActionDelay","Time"),new Field("SupplyWarehouseScanDistance","Float"),new Field("DepletedAnnounceDistance","Float"),new Field("HarvestTrees","Boolean"),new Field("HarvestTiberium","Boolean"),new Field("HarvestActivationRange","Float"),new Field("HarvestPreparationTime","Time"),new Field("HarvestActionTime","Time"),new Field("HarvestsFromRearEnd","Boolean"),new Field("CanPathThroughUnitsWhileDocking","Boolean"),new Field("CanPathThroughUnitsWhileHarvesting","Boolean")},
        new Field[]{},
        new Field[]{new Field("FleeRadius","Signed"),new Field("FleeDistance","Signed"),new Field("WanderPercentage","Signed"),new Field("MinDistance","Signed"),new Field("MaxDistance","Signed"),new Field("Radius","Signed"),new Field("FleeUpdateTimer","Time"),new Field("WanderUpdateTimer","Time"),new Field("AfraidOfCastles","Boolean"),new Field("InitialFleeBlindlyRadius","Float")},
        new Field[]{new Field("AttackAll","Boolean"),new Field("EntryCondition","ModelCondition"),new Field("Selectable","Boolean"),new Field("WanderDistance","Unsigned")},
        new Field[]{new Field("UnpackTime","Time"),new Field("PackTime","Time"),new Field("ResetTurretBeforePacking","Boolean"),new Field("TurretsFunctionOnlyWhenDeployed","Boolean"),new Field("TurretsMustCenterBeforePacking","Boolean"),new Field("MustDeployToAttack","Boolean"),new Field("DeployedAttributeModifierName","Reference")},
    };
    static string[] AIUpdateStateMachineTypeValues={"AIStateMachine","CombinedHelicopterAIStateMachine","DisguiseWhenIdleAIStateMachine","DiveBomberJetAIStateMachine","HelicopterAIStateMachine","HordeAIStateMachine","JetAIStateMachine","OffensiveStructureAIStateMachine","OreMinerOffensiveStateMachine","OreMinerStateMachine","OreNodeStateMachine","OreRefineryStateMachine","StrafingRunAIStateMachine","StructureAIStateMachine","TransportHelicopterAIStateMachine","UnitAIStateMachine","","UNKNOWN","all","ALL","-ALL","+ALL","0","-1"," NORMAL","NORMAL ","normal","YES NO","YES -YES","YES UNKNOWN"};
    static string[] AIUpdateLocomotorSetTypeValues={"ATTACKING","BURNINGDEATH","CHANGING_FIRINGARC","COMBO","CONTAINED","ENRAGED","EXITING_PRODUCTION_STRUCTURE","FREEFALL","INVALID","MOUNTED","MOVING_TO_TRANSPORT","NORMAL","NORMAL_UPGRADED","PANIC","SCARED","SUPERSONIC","TAXIING","WALL_SCALING","WANDER","","UNKNOWN","all","ALL","-ALL","+ALL","0","-1"," NORMAL","NORMAL ","normal","YES NO","YES -YES","YES UNKNOWN"};
    static string[] AIUpdateAutoAcquireEnemiesTypeValues={"ATTACK_BUILDINGS","NO","NOTWHILEATTACKING","STEALTHED","YES","","UNKNOWN","all","ALL","-ALL","+ALL","0","-1"," NORMAL","NORMAL ","normal","YES NO","YES -YES","YES UNKNOWN"};
    static string AIUpdateXML(int group,string attrs="",string children="") {
        return "<Behaviors><ObjectSMCHelper TypeId='"+AIUpdateTypes[group]+"' id='AIUpdate' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";
    }
    static string[] AIUpdateSamples(Field f) {
        if(f.Name=="StateMachine")return AIUpdateStateMachineTypeValues;
        if(f.Name=="DefaultLocomotorSet")return AIUpdateLocomotorSetTypeValues;
        if(f.Name=="AutoAcquireEnemiesWhenIdle")return AIUpdateAutoAcquireEnemiesTypeValues;
        if(f.Kind=="Bits")return WeaponSetMaskValues;
        if(f.Kind=="ModelCondition")return HealingModelValues;
        if(f.Kind=="Boolean")return new[]{"","true","false","TRUE","1","0","yes"};
        if(f.Kind=="Reference")return new[]{"","Unresolved","A\\0","B\\1","C\\23","D\\-1","E\\2147483647","F\\2147483648","G\\-2147483648","H\\-2147483649","I\\+17","J\\23tail","K\\0x10","L\\ 23","M\\text"};
        if(f.Kind=="String" || f.Kind=="StringList")return new[]{"","a","ab","abc","abcd","abcde","a bb ccc dddd","A A A","A&#9;B&#10;C","  a  bb  ","中文测试","é ÿ","A&amp;B"};
        return new[]{"","0","-0","1.25","-17","0x1234","010","08","23tail","text","25%","1e-50","nan","inf","-inf","1e39","-1e39","2147483647","2147483648","-2147483648","-2147483649","4294967295","4294967296","-4294967296","0s","0.5s","100ms","  +23tail","33.333333","66.666666"};
    }
    static void AIUpdateSentinel(byte[] b,int table,int count) {for(int i=0;i<12;++i)if(b[table+12*count+i]!=0)throw new Exception("Independent AI Update Token sentinel");}
    static void AIUpdateFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;int[] sizes={76,100,76,116,76,116,144,92},tokens={17,10,1,14,1,11,5,8};
        string chooser="<UnitAITargetChooserData TargetingCompareList='T\\23' CheckVisionRange='true' CanPickDynamicTargetType='ALL'><ObjectVisionRange id='Vision_Metadata' Range='25%'><Filter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></Filter></ObjectVisionRange><ObjectVisionRange id='Vision_Metadata' Range='75'><Filter/></ObjectVisionRange></UnitAITargetChooserData>";
        for(int group=0;group<8;++group) {
            byte[] b=Check(original,p,"",AIUpdateXML(group)).InstanceData;int at=LifecycleAt(b,big),table=MotionTable(b,big);
            if(b.Length!=at+sizes[group] || Word(b,at,big)!=uint.Parse(AIUpdateTypes[group]))throw new Exception("Independent AI Update native type/size");
            AIUpdateSentinel(b,table,tokens[group]);
            if(group!=0) {if(Word(b,table,big)!=1 || Word(b,table+4,big)!=148619278u)throw new Exception("Independent derived AI parent layer");table=(int)Word(b,table+8,big);AIUpdateSentinel(b,table,17);}
            if(Word(b,at+20,big)!=0x43fa0000u || Word(b,at+36,big)!=0x42a00000u || Word(b,at+52,big)!=1 || Word(b,at+68,big)!=0 || b[at+72]!=0 || b[at+73]!=0)throw new Exception("Independent AI Update defaults/nullable chooser");
            if(group==1 && (Word(b,at+92,big)!=0 || Word(b,at+96,big)!=0))throw new Exception("Independent Jet empty Eva/default booleans");
            if(group==3 && (Word(b,at+92,big)!=0x44fa0000u || b[at+111]!=1 || b[at+112]!=1))throw new Exception("Independent Supply defaults");
            if(group==5 && (Word(b,at+76,big)!=20 || Word(b,at+80,big)!=100 || Word(b,at+84,big)!=35 || Word(b,at+88,big)!=15 || Word(b,at+92,big)!=50 || Word(b,at+96,big)!=25 || Word(b,at+100,big)!=0x3f4ccccdu || Word(b,at+104,big)!=0x40000000u || Word(b,at+108,big)!=0x42480000u || b[at+112]!=1))throw new Exception("Independent Animal integer/time/float defaults");
            Check(original,p,"","<AI><"+AIUpdateNames[group]+" TypeId='"+AIUpdateTypes[group]+"' id='AIUpdate' AttackPriority='a bb' SpecialContactPoints='One Two'>"+chooser+"</"+AIUpdateNames[group]+"></AI>");
            foreach(var container in ModuleContainers)Check(original,p,"",ModuleXML(container[0],container[1],"TypeId='"+AIUpdateTypes[group]+"' id='Ai'"));
            foreach(var fields in new[]{AIUpdateFields[group],group==0?new Field[0]:AIUpdateFields[0]})foreach(var f in fields)foreach(string value in AIUpdateSamples(f))Check(original,p,"",AIUpdateXML(group,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AIUpdateXML(group,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<UnitAITargetChooserData/><UnitAITargetChooserData/>"})Reject(p,"",AIUpdateXML(group,"",bad));
            b=Check(original,p,"",AIUpdateXML(group,"StandGround='true' FadeOnPortals='true'",chooser)).InstanceData;at=LifecycleAt(b,big);int ch=(int)Word(b,at+68,big),ranges=(int)Word(b,ch+24,big);
            if(ch!=at+sizes[group] || Word(b,ch+36,big)!=24 || Word(b,ch+20,big)!=2 || ranges!=ch+40 || b[at+72]!=1 || b[at+73]!=1 || Word(b,ranges+4,big)!=0x3e800000u)throw new Exception("Independent UnitChooser40/reference/AI bool/list allocation");
            int aiTable=MotionTable(b,big);if(group!=0)aiTable=(int)Word(b,aiTable+8,big);int ct=(int)Word(b,Token(b,aiTable,607907585u,big)+8,big);AIUpdateSentinel(b,ct,2);
            if(Word(b,ct,big)!=1 || Word(b,ct+4,big)!=2856512412u)throw new Exception("Independent UnitChooser BaseChooser token parent");
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string body="";for(int i=0;i<count;++i)body+="<ObjectVisionRange id='Vision_Metadata' Range='25%'><Filter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A"+i+"</IncludeThing></Filter></ObjectVisionRange>";
                Check(original,p,"Browser='a bb'",AIUpdateXML(group,"AttackPriority='a bb' SpecialContactPoints='one two three'","<UnitAITargetChooserData>"+body+"</UnitAITargetChooserData>"));
                string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+AIUpdateTypes[group]+"' id='A"+i+"' AttackPriority='a bb' SpecialContactPoints='One Two'>"+chooser+"</ObjectSMCHelper>";
                Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<FireWeaponUpdate TypeId='2995535186'><FireWeaponNugget WeaponName='W\\23'><Offset/></FireWeaponNugget></FireWeaponUpdate></Behaviors>");
            }
            foreach(var f in WeaponSetTurretFields)if(f.Name.StartsWith("chooser:") && f.Name!="chooser:ActiveWhenPerformingSpecialAbility" && f.Name!="chooser:CanAcquireDynamicIfAssignedOutOfRange" && f.Name!="chooser:CanPickTargetsOutOfTurretAngle") {
                string field=f.Name.Substring(8);foreach(string value in AIUpdateSamples(f))Check(original,p,"",AIUpdateXML(group,"","<UnitAITargetChooserData "+field+"='"+Escape(value)+"'/>"));
            }
            foreach(string value in AIUpdateSamples(new Field("Range","Percent")))Check(original,p,"",AIUpdateXML(group,"","<UnitAITargetChooserData><ObjectVisionRange id='Vision_Metadata' Range='"+Escape(value)+"'/></UnitAITargetChooserData>"));
            foreach(string path in new[]{"UnitAITargetChooserData","UnitAITargetChooserData/ObjectVisionRange","UnitAITargetChooserData/ObjectVisionRange/Filter"}) {
                string[] parts=path.Split('/');string before="",after="";for(int i=0;i<parts.Length-1;++i){before+="<"+parts[i]+">";after="</"+parts[i]+">"+after;}
                string name=parts[parts.Length-1];Check(original,p,"",AIUpdateXML(group,"",before+"<"+name+"/>"+after));
                foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AIUpdateXML(group,"",before+"<"+name+" "+bad+"/>"+after));
                foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",AIUpdateXML(group,"",before+"<"+name+">"+bad+"</"+name+">"+after));
            }
            Reject(p,"",AIUpdateXML(group,"","<UnitAITargetChooserData ActiveWhenPerformingSpecialAbility='true'/>"));
            Reject(p,"",AIUpdateXML(group,"","<UnitAITargetChooserData><ObjectVisionRange><Filter/><Filter/></ObjectVisionRange></UnitAITargetChooserData>"));
            string first="AttackPriority='a bb' SpecialContactPoints='One Two' StateMachine='UnitAIStateMachine' DefaultLocomotorSet='TAXIING' StandGround='true'",second="StandGround='true' DefaultLocomotorSet='TAXIING' StateMachine='UnitAIStateMachine' SpecialContactPoints='One Two' AttackPriority='a bb'";
            var a=Check(original,p,"",AIUpdateXML(group,first,chooser));var c=Check(original,p,"",AIUpdateXML(group,second,chooser));Equal(a.InstanceData,c.InstanceData,"AI attribute allocation order",false);Equal(a.RelocationData,c.RelocationData,"AI attribute relocation order",false);Equal(a.ImportsData,c.ImportsData,"AI attribute import order",false);
        }
        {byte[] b=Check(original,p,"",AIUpdateXML(0,"AttackPriority='abc' SpecialContactPoints='one two' AutoAcquireEnemiesWhenIdle='UNKNOWN' StateMachine='UNKNOWN' DefaultLocomotorSet='UNKNOWN'","<UnitAITargetChooserData/>")).InstanceData;int at=LifecycleAt(b,big),list=(int)Word(b,at+44,big),ch=(int)Word(b,at+68,big);if(Word(b,at+8,big)!=3 || Word(b,at+12,big)!=at+76 || Word(b,at+16,big)!=16 || Word(b,at+40,big)!=2 || list!=at+80 || ch!=at+104 || Word(b,at+60,big)!=0 || Word(b,at+64,big)!=12)throw new Exception("Independent AI string/list allocation and enum fallbacks");}
        {byte[] b=Check(original,p,"",AIUpdateXML(1,"UseForOutOfAmmoCheck='W\\23' OutOfAmmoEvaEvent='E\\7' AttackPriority='abc' OutOfAmmoDamagePerSecond='25' ParkingOffset='12.5'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+88,big)!=24 || Word(b,at+92,big)!=8 || Word(b,at+12,big)!=at+100 || Word(b,at+76,big)!=0x3e800000u || Word(b,at+84,big)!=0x41480000u)throw new Exception("Independent Jet reference slots, percent and base string order");}
        {byte[] b=Check(original,p,"",AIUpdateXML(5,"FleeRadius='-17' WanderPercentage='-23'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+76,big)!=unchecked((uint)-17) || Word(b,at+84,big)!=unchecked((uint)-23))throw new Exception("Independent Animal signed integers");}
        {byte[] b=Check(original,p,"",AIUpdateXML(6,"EntryCondition='ALL' WanderDistance='0xffffffff' AttackAll='true' Selectable='true'")).InstanceData;int at=LifecycleAt(b,big);for(int i=0;i<15;++i)if(Word(b,at+76+4*i,big)!=0xffffffffu)throw new Exception("Independent Wander full model mask");if(Word(b,at+136,big)!=0xffffffffu || b[at+140]!=1 || b[at+141]!=1 || b[at+142]!=0 || b[at+143]!=0)throw new Exception("Independent Wander unsigned/bool/padding");}
        {byte[] b=Check(original,p,"",AIUpdateXML(7,"PackTime='2s' UnpackTime='500ms' DeployedAttributeModifierName='M\\23' ResetTurretBeforePacking='true' TurretsFunctionOnlyWhenDeployed='true' TurretsMustCenterBeforePacking='true' MustDeployToAttack='true'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+76,big)!=0x3f000000u || Word(b,at+80,big)!=0x40000000u || Word(b,at+84,big)!=24 || b[at+88]!=1 || b[at+89]!=1 || b[at+90]!=1 || b[at+91]!=1)throw new Exception("Independent Deploy time/reference/bool offsets");}
    }
}
