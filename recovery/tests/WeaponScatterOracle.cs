using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    // Ordinals are read from the original PE enum map, including the ALL slot.
    static readonly string[] ModelNames = {
        "TOPPLED",
        "FRONTCRUSHED",
        "BACKCRUSHED",
        "DAMAGED",
        "REALLYDAMAGED",
        "RUBBLE",
        "POST_RUBBLE",
        "POST_COLLAPSE",
        "DECAY",
        "DESTROYED_WHILST_BEING_CONSTRUCTED",
        "COLLAPSING",
        "SOLD",
        "STRUCTURE_UNPACKING",
        "SPECIAL_DAMAGED",
        "NIGHT",
        "SNOW",
        "FREEFALL",
        "PARACHUTING",
        "PARACHUTE_LAND",
        "LAUNCHED",
        "GARRISONED",
        "INSIDE_GARRISON",
        "WEAPONSET_VETERAN",
        "WEAPONSET_ELITE",
        "WEAPONSET_HERO",
        "WEAPONSET_PASSENGER_TYPE_ONE",
        "WEAPONSET_PASSENGER_TYPE_TWO",
        "WEAPONSET_PLAYER_UPGRADE",
        "WEAPONSET_TOGGLE_1",
        "WEAPONSET_TOGGLE_2",
        "WEAPONSET_TOGGLE_3",
        "WEAPONSET_SPECIAL_UPGRADE",
        "WEAPONSET_GARRISONED",
        "WEAPONSET_CLOSE_RANGE",
        "WEAPONSET_CONTESTING_BUILDING",
        "WEAPONSET_RIDER1",
        "WEAPONSET_RIDER2",
        "WEAPONSET_RIDER3",
        "WEAPONSET_RIDER4",
        "WEAPONSET_SPECIAL_ONE",
        "WEAPONSET_SPECIAL_TWO",
        "WEAPONSET_CONTAINED",
        "WEAPONSET_MOUNTED",
        "SWAPPING_TO_WEAPONSET_1",
        "SWAPPING_TO_WEAPONSET_2",
        "SWAPPING_TO_WEAPONSET_3",
        "WEAPONSTATE_ONE",
        "WEAPONSTATE_TWO",
        "WEAPONSTATE_THREE",
        "WEAPONSTATE_CLOSE_RANGE",
        "WEAPONSTATE_CONTAINED",
        "WEAPONSLOTID_01",
        "WEAPONSLOTID_02",
        "WEAPONSLOTID_03",
        "WEAPONSLOTID_04",
        "WEAPONSLOTID_05",
        "SPECIAL_WEAPON_ONE",
        "SPECIAL_WEAPON_TWO",
        "SPECIAL_WEAPON_THREE",
        "SPECIAL_WEAPON_FOUR",
        "SPECIAL_WEAPON_FIVE",
        "SPECIAL_WEAPON_SIX",
        "DESTROYED_WEAPON",
        "WEAPON_TOGGLING",
        "ATTACKING",
        "ATTACKING_STRUCTURE",
        "ATTACKING_POSITION",
        "ENGAGED",
        "PREATTACK_A",
        "PREATTACK_B",
        "PREATTACK_C",
        "PREATTACK_D",
        "PREATTACK_E",
        "FIRING_OR_PREATTACK_A",
        "FIRING_OR_PREATTACK_B",
        "FIRING_OR_PREATTACK_C",
        "FIRING_OR_PREATTACK_D",
        "FIRING_OR_PREATTACK_E",
        "FIRING_A",
        "FIRING_B",
        "FIRING_C",
        "FIRING_D",
        "FIRING_E",
        "FIRING_OR_RELOADING_A",
        "FIRING_OR_RELOADING_B",
        "FIRING_OR_RELOADING_C",
        "FIRING_OR_RELOADING_D",
        "FIRING_OR_RELOADING_E",
        "RELOADING_A",
        "RELOADING_B",
        "RELOADING_C",
        "RELOADING_D",
        "RELOADING_E",
        "BETWEEN_FIRING_SHOTS_A",
        "BETWEEN_FIRING_SHOTS_B",
        "BETWEEN_FIRING_SHOTS_C",
        "BETWEEN_FIRING_SHOTS_D",
        "BETWEEN_FIRING_SHOTS_E",
        "USING_WEAPON_A",
        "USING_WEAPON_B",
        "USING_WEAPON_C",
        "USING_WEAPON_D",
        "USING_WEAPON_E",
        "DOOR_1_OPENING",
        "DOOR_1_CLOSING",
        "DOOR_1_WAITING_OPEN",
        "DOOR_1_WAITING_TO_CLOSE",
        "DOOR_2_OPENING",
        "DOOR_2_CLOSING",
        "DOOR_2_WAITING_OPEN",
        "DOOR_2_WAITING_TO_CLOSE",
        "DOOR_3_OPENING",
        "DOOR_3_CLOSING",
        "DOOR_3_WAITING_OPEN",
        "DOOR_3_WAITING_TO_CLOSE",
        "DOOR_4_OPENING",
        "DOOR_4_CLOSING",
        "DOOR_4_WAITING_OPEN",
        "DOOR_4_WAITING_TO_CLOSE",
        "DOOR_HELIPAD_OPENING",
        "DOOR_HELIPAD_CLOSING",
        "DOOR_HELIPAD_WAITING_OPEN",
        "DOOR_HELIPAD_WAITING_TO_CLOSE",
        "PARKINGPLACE_1_DOOR_OPENING",
        "PARKINGPLACE_1_DOOR_OPEN",
        "PARKINGPLACE_1_DOOR_CLOSING",
        "PARKINGPLACE_1_DOOR_CLOSED",
        "PARKINGPLACE_2_DOOR_OPENING",
        "PARKINGPLACE_2_DOOR_OPEN",
        "PARKINGPLACE_2_DOOR_CLOSING",
        "PARKINGPLACE_2_DOOR_CLOSED",
        "PARKINGPLACE_3_DOOR_OPENING",
        "PARKINGPLACE_3_DOOR_OPEN",
        "PARKINGPLACE_3_DOOR_CLOSING",
        "PARKINGPLACE_3_DOOR_CLOSED",
        "PARKINGPLACE_4_DOOR_OPENING",
        "PARKINGPLACE_4_DOOR_OPEN",
        "PARKINGPLACE_4_DOOR_CLOSING",
        "PARKINGPLACE_4_DOOR_CLOSED",
        "PARKINGPLACE_HELIPAD_DOOR_OPENING",
        "PARKINGPLACE_HELIPAD_DOOR_OPEN",
        "PARKINGPLACE_HELIPAD_DOOR_CLOSING",
        "PARKINGPLACE_HELIPAD_DOOR_CLOSED",
        "MOVING",
        "DYING",
        "DYING_WASMOVING",
        "DYING_WASATTACKING",
        "WANDER",
        "WALKING",
        "CHARGING",
        "MOVING_OUT_OF_THE_WAY",
        "RUNNING_OFF_MAP",
        "COMING_OUT_OF_FACTORY",
        "ATTACK_MOVING",
        "DIVING",
        "SWOOPING",
        "MARCHING",
        "BACKING_UP",
        "CLIMBING",
        "RAPPELLING",
        "WADING",
        "SWIMMING",
        "SCALING_WALL",
        "SCALING_WALL_HORDE",
        "FLYING",
        "TAKING_OFF",
        "LANDING",
        "AWAITING_CONSTRUCTION",
        "PARTIALLY_CONSTRUCTED",
        "ACTIVELY_BEING_CONSTRUCTED",
        "UNIT_ACTIVELY_BEING_CONSTRUCTED",
        "ACTIVELY_CONSTRUCTING",
        "RADAR_EXTENDING",
        "RADAR_UPGRADED",
        "PANICKING",
        "AFLAME",
        "SMOLDERING",
        "BURNED",
        "BURNT_MODEL",
        "BURNT_TEXTURE",
        "BURNINGDEATH",
        "DOCKING",
        "DOCKING_BEGINNING",
        "DOCKING_ACTIVE",
        "DOCKING_RETURN",
        "DOCKING_MOVEBACK",
        "DOCKING_FILL",
        "DOCKING_EXTRACT",
        "DOCKING_ENDING",
        "DOCKING_PRE_DOCK",
        "HARVEST_PREPARATION",
        "HARVEST_ACTION",
        "CARRYING",
        "PASSENGER",
        "TRANSPORT_MOVING",
        "TRANSPORT_STOPPED",
        "SIEGE_CONTAIN",
        "JETAFTERBURNER",
        "JETEXHAUST",
        "PACKING",
        "PREPARING",
        "UNPACKING",
        "PACKING_TYPE_1",
        "PACKING_TYPE_2",
        "PACKING_TYPE_3",
        "PACKING_TYPE_4",
        "PACKING_TYPE_5",
        "PACKING_TYPE_6",
        "OVER_WATER",
        "POWER_PLANT_UPGRADED",
        "POWER_PLANT_UPGRADING",
        "BUILD_PLACEMENT_CURSOR",
        "PHANTOM_STRUCTURE",
        "FORMATION_PREVIEW",
        "WORLD_BUILDER",
        "DEBUG",
        "START_CAPTURE",
        "CANCEL_CAPTURE",
        "CAPTURING",
        "CAPTURED",
        "RAISING_FLAG",
        "CONTINUOUS_FIRE_SLOW",
        "CONTINUOUS_FIRE_MEAN",
        "CONTINUOUS_FIRE_FAST",
        "PREORDER",
        "SPECIAL_CHEERING",
        "IMPENDING_DOOM",
        "EATING",
        "STUNNED_FLAILING",
        "STUNNED",
        "STUNNED_STANDING_UP",
        "SPLATTED",
        "THROWN_PROJECTILE",
        "ABOUT_TO_HIT",
        "EXPLODED_FLAILING",
        "EXPLODED_BOUNCING",
        "ACCELERATE",
        "DECELERATE",
        "TURN_LEFT",
        "TURN_RIGHT",
        "TURN_LEFT_HIGH_SPEED",
        "TURN_RIGHT_HIGH_SPEED",
        "DESTROYED_FRONT",
        "DESTROYED_RIGHT",
        "DESTROYED_BACK",
        "DESTROYED_LEFT",
        "WEAPONLOCK_PRIMARY",
        "WEAPONLOCK_SECONDARY",
        "WEAPONLOCK_TERTIARY",
        "WEAPONLOCK_QUATERNARY",
        "WEAPONLOCK_QUINARY",
        "DEATH_1",
        "DEATH_2",
        "DEATH_3",
        "DEATH_4",
        "DEATH_5",
        "SELECTED",
        "GUARDING",
        "LEVELED",
        "MOUNTED",
        "RESURRECTED",
        "ATTACHED",
        "DRAFTED",
        "FLOODED",
        "LOADED",
        "DEPLOYED",
        "JUST_BUILT",
        "BASE_BUILD",
        "REACT_1",
        "REACT_2",
        "REACT_3",
        "REACT_4",
        "REACT_5",
        "REACT_6",
        "HIT_REACTION",
        "HIT_LEVEL_1",
        "HIT_LEVEL_2",
        "HIT_LEVEL_3",
        "AIM_HIGH",
        "AIM_STRAIGHT",
        "AIM_LOW",
        "AIM_NEAR",
        "AIM_FAR",
        "USER_1",
        "USER_2",
        "USER_3",
        "USER_4",
        "USER_5",
        "USER_6",
        "USER_7",
        "USER_8",
        "USER_9",
        "USER_10",
        "PASSENGER_VARIATION_1",
        "PASSENGER_VARIATION_2",
        "PASSENGER_VARIATION_3",
        "PASSENGER_VARIATION_4",
        "PASSENGER_VARIATION_5",
        "SPECIAL_POWER_1",
        "SPECIAL_POWER_2",
        "SPECIAL_POWER_3",
        "SPECIALPOWER1_READY",
        "SPECIALPOWER2_READY",
        "SPECIALPOWER3_READY",
        "SPECIALPOWER4_READY",
        "SPECIALPOWER5_READY",
        "SPECIALPOWER6_READY",
        "SPECIALPOWER7_READY",
        "SPECIALPOWER8_READY",
        "SPECIALPOWER9_READY",
        "USING_SPECIAL_ABILITY",
        "DEFLECT_SPECIAL_POWER",
        "RIDER1",
        "RIDER2",
        "RIDER3",
        "RIDER4",
        "RIDER5",
        "RIDER6",
        "RIDER7",
        "RIDER8",
        "RIDERLESS",
        "HORDE_EMPTY",
        "PRIMARY_FORMATION",
        "ALTERNATE_FORMATION",
        "CAPTURE_100",
        "CAPTURE_75",
        "CAPTURE_50",
        "CAPTURE_25",
        "CAPTURE_0",
        "DISGUISED",
        "HIDDEN",
        "INVISIBLE_STEALTH",
        "INVISIBLE_CAMOUFLAGE",
        "PRONE",
        "COVER",
        "SUPPRESSED",
        "TURRET_ANGLE_0",
        "TURRET_ANGLE_90",
        "TURRET_ANGLE_180",
        "TURRET_ANGLE_270",
        "TURRET_ROTATE",
        "ARMORSET_VETERAN",
        "ARMORSET_ELITE",
        "ARMORSET_HERO",
        "ARMORSET_WEAK_VERSUS_BASEDEFENSES",
        "ARMORSET_ALTERNATE_FORMATION",
        "ARMORSET_MOUNTED",
        "ARMORSET_PLAYER_UPGRADE",
        "ARMORSET_PLAYER_UPGRADE_2",
        "ARMORSET_PLAYER_UPGRADE_3",
        "ARMORSET_UNBESIEGEABLE",
        "UPGRADED_ARMOR",
        "EMOTION_TAUNTING",
        "EMOTION_DOOM",
        "EMOTION_POINTING",
        "EMOTION_GUNG_HO",
        "EMOTION_LOOK_TO_SKY",
        "EMOTION_CELEBRATING",
        "EMOTION_AMUSED",
        "EMOTION_MORALE_HIGH",
        "EMOTION_MORALE_LOW",
        "EMOTION_COWER",
        "EMOTION_DISSIDENT",
        "EMOTION_UNCONTROLLABLY_AFRAID",
        "EMOTION_BRACE_FOR_BEING_CRUSHED",
        "EMOTION_CHEER_FOR_ABOUT_TO_CRUSH",
        "EMOTION_ALERT",
        "EMOTION_AFRAID",
        "EMOTION_TERROR",
        "EMOTION_PANIC",
        "INVULNERABLE",
        "SPECIAL_ENEMY_NEAR",
        "UNCONTROLLABLE",
        "LEASHED_RETURNING",
        "SAIL_FLAPPING",
        "SAIL_BLOWN_RIGHT",
        "SAIL_BLOWN_LEFT",
        "BUILD_VARIATION_ONE",
        "BUILD_VARIATION_TWO",
        "PARALYZED",
        "AFFECTED_BY_EMP",
        "UNDERPOWERED",
        "TIBERIUM_CRYSTAL_TYPE1",
        "TIBERIUM_CRYSTAL_TYPE2",
        "TIBERIUM_CRYSTAL_TYPE3",
        "TIBERIUM_CRYSTAL_TYPE4",
        "TIBERIUM_CRYSTAL_TYPE5",
        "TIBERIUM_GROWING",
        "MARKED_FOR_HUNT_TACTIC",
        "MARKED_FOR_NO_SKIRMISH_RECRUIT",
        "LOCOMOTOR_NORMAL_UPGRADED",
        "LOCOMOTOR_FREEFALL",
        "LOCOMOTOR_WANDER",
        "LOCOMOTOR_PANIC",
        "LOCOMOTOR_TAXIING",
        "LOCOMOTOR_SUPERSONIC",
        "LOCOMOTOR_MOUNTED",
        "LOCOMOTOR_ENRAGED",
        "LOCOMOTOR_SCARED",
        "LOCOMOTOR_CONTAINED",
        "LOCOMOTOR_COMBO",
        "LOCOMOTOR_COMBO2",
        "LOCOMOTOR_COMBO3",
        "LOCOMOTOR_WALL_SCALING",
        "LOCOMOTOR_CHANGING_FIRINGARC",
        "LOCOMOTOR_BURNINGDEATH",
        "USING_COMBO_LOCOMOTOR",
        "MONEY_STORED_AMOUNT_1",
        "MONEY_STORED_AMOUNT_2",
        "MONEY_STORED_AMOUNT_3",
        "MONEY_STORED_AMOUNT_4",
        "VEHICLE_CRUSH_LEFT",
        "VEHICLE_CRUSH_RIGHT",
        "VEHICLE_CRUSH_CENTER",
        "HEALTH_PERCENT_0",
        "HEALTH_PERCENT_25",
        "HEALTH_PERCENT_50",
        "HEALTH_PERCENT_75",
        "HEALTH_PERCENT_100",
        "COMBINED_PARENT",
        "COMBINED_CHILD",
        "REPAIRING_DISABLED",
        "WALL_SEGMENT",
        "SHRINK_EFFECT",
        "HIGH_TECH_EFFECT",
        "AIR_POWER_EFFECT",
        "PLAYER_POWER_1",
        "BEING_MAULED",
        "FROZEN",
        "IRONCURTAIN",
        "CHRONORIFT",
        "INFILTRATED_DISABLE",
        "INFILTRATED_RADAR_FREEZE",
        "INFILTRATED_STEAL_MONEY",
        "INFILTRATED_VISION",
        "INFILTRATED_ENERGY",
        "INFILTRATED_RESET",
        "INFILTRATED_VISION_UNITS",
        "WEAPONSLOTID_06",
        "WEAPONSLOTID_07",
        "WEAPONSLOTID_08",
        "WEAPONSLOTID_09",
        "WEAPONSLOTID_10",
        "BUILD_PLACEMENT_HINT",
        "DEATH_6",
        "DEATH_7",
        "DEATH_8",
        "DEATH_9",
        "DEATH_10",
        "DEATH_11",
        "DEATH_12",
        "DEATH_13",
        "DEATH_14",
        "DEATH_15",
        "ALL",
        "SUCKED_UP_HIGH",
    };
    static uint[] ModelBit(int bit) { var result = new uint[15]; result[bit/32] = 1u << (bit%32); return result; }
    static uint[] ModelAll() { var result = new uint[15]; for (int i=0;i<15;++i) result[i]=uint.MaxValue; return result; }
    static int directModelComparisons;
    static void DirectModel(Assembly original)
    {
#if !WEAPON_TRANSCRIPT_PRODUCT
        MethodInfo parser = null;
        foreach (var method in original.GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            if (method.Name == "Marshaler.Marshal<enum ModelConditionFlagType,456>") parser = method;
        if (parser == null) throw new Exception("Original ModelCondition inline parser missing");
#endif
        var values = new List<string>(ModelNames);
        foreach (string name in ModelNames) { values.Add("-"+name); values.Add(name+" -"+name); }
        values.AddRange(new[] { "INVALID", "-INVALID", "+INVALID", "invalid", "", " ", "\v\f\t\r\n", "TOPPLED\vDAMAGED\fSUCKED_UP_HIGH", "TOPPLED\0DAMAGED", "\0ALL", "ALL", "all -TOPPLED", "-ALL unknown", "TOPPLED -ALL -DAMAGED", "+", "-", "unknown", "unknown -unknown", "TOPPLED,DAMAGED", "++TOPPLED", "--TOPPLED", "ＡＬＬ", "é", "状态", "TOPPLED\u00a0DAMAGED" });
        var pattern = new uint[15]; for(int i=0;i<15;++i) pattern[i]=unchecked(0x9e3779b9u*(uint)(i+1));
        foreach (var seed in new[] { new uint[15], pattern, ModelAll() }) foreach (string text in values) {
#if WEAPON_TRANSCRIPT_ORACLE || WEAPON_TRANSCRIPT_PRODUCT
            var expected = WeaponTranscript.ExpectedMask(original, "Marshaler.Marshal<enum ModelConditionFlagType,456>", text, seed);
#else
            var expected=(uint[])seed.Clone(); IntPtr ansi=Marshal.StringToHGlobalAnsi(text);
            try { fixed(uint* target=expected) {
                var types=parser.GetParameters();
                parser.Invoke(null,new object[] { Pointer.Box(ansi.ToPointer(),types[0].ParameterType),Pointer.Box(target,types[1].ParameterType),Pointer.Box(null,types[2].ParameterType) });
            } } finally { Marshal.FreeHGlobal(ansi); }
#endif
            var actual=WeaponCompiler.MarshalModelCondition(text,seed);
            for(int i=0;i<15;++i) if(expected[i]!=actual[i]) throw new Exception("Direct ModelCondition mismatch: "+text+" word="+i);
            ++directModelComparisons;
        }
        Console.WriteLine("PASS direct ModelCondition parsing: "+directModelComparisons+" fifteen-word comparisons");
    }
    static void ScatterSingle(IAssetBuilderPlugin plugin, TargetPlatform platform, string label,
        string text, uint[] expected, bool filter)
    {
        string xmlText="<WeaponTemplate xmlns='"+AssetNamespace+"' id='Scatter'><ScatterRadiusVsType Radius='1.25' "+Attribute("RequiredModelConditions",text)+">"+(filter?"<Filter/>":"")+"</ScatterRadiusVsType></WeaponTemplate>";
        Test(plugin,"scatter/"+label,xmlText,platform,-1); ++fixtures;
        var xml=new XmlDocument();xml.LoadXml(xmlText);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var buffer=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        const int tokens=1512,payload=1840,table=1296;
        if(buffer.InstanceData.Length!=2016 || buffer.RelocationData.Length!=8 || buffer.ImportsData.Length!=0 ||
            Word(buffer.InstanceData,tokens+276,big)!=1 || Word(buffer.InstanceData,tokens+280,big)!=payload ||
            Word(buffer.InstanceData,102*12+8,big)!=1272 || Word(buffer.InstanceData,1280,big)!=tokens+276 ||
            Word(buffer.InstanceData,1284,big)!=2974916843u || Word(buffer.InstanceData,1292,big)!=table ||
            Word(buffer.InstanceData,table+8,big)!=payload || Word(buffer.InstanceData,table+20,big)!=payload+4 ||
            Word(buffer.InstanceData,table+32,big)!=1344 || Word(buffer.InstanceData,1352,big)!=payload+64 ||
            Word(buffer.InstanceData,payload,big)!=0x3fa00000u || Word(buffer.InstanceData,payload+68,big)!=(filter?3u:0u) ||
            Word(buffer.RelocationData,0,big)!=tokens+280 || Word(buffer.RelocationData,4,big)!=uint.MaxValue)
            throw new Exception("Independent scatter layout: "+label);
        for(int i=0;i<15;++i) if(Word(buffer.InstanceData,payload+4+4*i,big)!=expected[i]) throw new Exception("Independent scatter mask: "+label+" word="+i);
        for(int at=64;at<176;at+=4) if(at!=68 && Word(buffer.InstanceData,payload+at,big)!=0) throw new Exception("Inline filter zero/default distinction: "+label);
    }
    static void Scatter(IAssetBuilderPlugin plugin, TargetPlatform platform)
    {
        ScatterSingle(plugin,platform,"empty","",new uint[15],false);
        ScatterSingle(plugin,platform,"empty-filter","",new uint[15],true);
        for(int bit=0;bit<ModelNames.Length;++bit) {
            string name=ModelNames[bit];bool all=name=="ALL";var expected=all?ModelAll():ModelBit(bit);
            ScatterSingle(plugin,platform,"name-"+bit,name,expected,false);
            ScatterSingle(plugin,platform,"minus-"+bit,"-"+name,expected,true);
            ScatterSingle(plugin,platform,"toggle-"+bit,name+" -"+name,all?ModelAll():new uint[15],false);
            ScatterSingle(plugin,platform,"lower-"+bit,name.ToLowerInvariant(),all?ModelAll():ModelBit(233),true);
        }
        foreach(string text in new[] {"ALL -TOPPLED","-all unknown","TOPPLED ALL -DAMAGED","+ALL"}) ScatterSingle(plugin,platform,text,text,ModelAll(),false);
        foreach(string text in new[] {"unknown","NONE","+","-","++TOPPLED","TOPPLED,DAMAGED"}) ScatterSingle(plugin,platform,text,text,ModelBit(233),true);
        foreach(string text in new[] {"INVALID","+INVALID","-INVALID"}) ScatterSingle(plugin,platform,text,text,new uint[15],false);
        string boundary="";var words=new uint[15];
        foreach(int bit in new[] {0,31,32,63,64,95,96,127,128,159,160,191,192,223,224,255,256,287,288,319,320,351,352,383,384,415,416,447,448,455}) { boundary+=" "+ModelNames[bit];words[bit/32]|=1u<<(bit%32); }
        ScatterSingle(plugin,platform,"boundaries",boundary,words,true);
        string allNames="";var allNamed=new uint[15];
        for(int bit=0;bit<ModelNames.Length;++bit) if(ModelNames[bit]!="ALL") { allNames+=" "+ModelNames[bit];allNamed[bit/32]|=1u<<(bit%32); }
        ScatterSingle(plugin,platform,"all-names-without-special-ALL",allNames,allNamed,false);
        foreach(string radius in new[] {"", "-0", "-23.75", "2d1", "1e-40", "1e40", "0x1p2", "NaN", "garbage"}) {
            Test(plugin,"scatter-radius-"+radius,WithChild("ScatterRadiusVsType",Attribute("Radius",radius)),platform,-1);++fixtures;
        }
        foreach(int count in new[] {0,1,2,3,7,16,17,31,32,33,64,127,128,129,255,256,257}) {
            string xmlText="<WeaponTemplate xmlns='"+AssetNamespace+"' id='List'>";
            for(int i=0;i<count;++i) xmlText+="<ScatterRadiusVsType Radius='"+i+"' RequiredModelConditions='"+ModelNames[(i*31)%456]+"'>"+(i%2==0?"<Filter/>":"")+"</ScatterRadiusVsType>";
            xmlText+="</WeaponTemplate>";Test(plugin,"scatter-list-"+count,xmlText,platform,-1);++fixtures;
            var xml=new XmlDocument();xml.LoadXml(xmlText);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
            var buffer=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;int tokens=1296+216*count,payload=tokens+328;
            if(buffer.InstanceData.Length!=1624+392*count || Word(buffer.InstanceData,tokens+276,big)!=count ||
                Word(buffer.InstanceData,tokens+280,big)!=(count==0?0:payload) || buffer.RelocationData.Length!=(count==0?0:8)) throw new Exception("Scatter list independent size/count/pointer");
            for(int i=0;i<count;++i) if(Word(buffer.InstanceData,payload+176*i+68,big)!=(i%2==0?3u:0u) || Word(buffer.InstanceData,payload+176*i,big)!=BitConverter.ToUInt32(BitConverter.GetBytes((float)i),0)) throw new Exception("Scatter list order/defaults");
        }
        // The first inline Filter wins; all scatter list entries remain in source order.
        Test(plugin,"scatter-duplicate-filter", "<WeaponTemplate xmlns='"+AssetNamespace+"' id='Dup'><ScatterRadiusVsType><Filter Rule='ANY' Include='AIRCRAFT'/><Filter Rule='NONE' Exclude='ALL'/></ScatterRadiusVsType></WeaponTemplate>",platform,-1);++fixtures;
        foreach(int count in new[] {1,2,17,33,129,257}) {
            string xml="<WeaponTemplate xmlns='"+AssetNamespace+"' id='Nested'>";
            for(int i=0;i<count;++i) xml+="<ScatterRadiusVsType Radius='-2.75' RequiredModelConditions='ALL'><Filter id='filter' Rule='ANY' Include='AIRCRAFT' Exclude='ALL' StatusBitFlags='AFLAME' StatusBitFlagsExclude='ALL'><IncludeThing>Obj_"+i+"</IncludeThing><ExcludeThing>Other_"+i+"</ExcludeThing></Filter></ScatterRadiusVsType>";
            xml+="</WeaponTemplate>";Test(plugin,"scatter-nested-lists-"+count,xml,platform,-1);++fixtures;
        }
        for(int mask=0;mask<128;++mask) {
            string[] blocks={"OverrideVoiceAttackSound Sound='s\\2'","OverrideVoiceEnterStateAttackSound EvaEvent='e\\3'","PreAttackDelay MinSeconds='1s'","FiringDuration MaxSeconds='2s'","CoolDownDelayBetweenShots MinSeconds='3s'","ClipReloadTime MaxSeconds='4s'","WeaponAiHintInfo MaxSpeedOfTarget='5'"};
            string xml="<WeaponTemplate xmlns='"+AssetNamespace+"' id='MixedScatter' RequiredFiringObjectStatus='ALL' FireSound='s\\0' PreferredTargetBone='bone'><IncompatibleAttributeModifier>m\\1</IncompatibleAttributeModifier>";
            for(int i=6;i>=0;--i) if((mask&(1<<i))!=0) xml+="<"+blocks[i]+"/>";
            foreach(string name in FilterNames) xml+="<"+name+" Include='AIRCRAFT' StatusBitFlags='AFLAME'><IncludeThing>Obj</IncludeThing></"+name+">";
            xml+="<ScatterRadiusVsType Radius='3' RequiredModelConditions='TOPPLED'><Filter Rule='ANY' StatusBitFlags='ALL'><IncludeThing>ScatterObj</IncludeThing><ExcludeThing>Other</ExcludeThing></Filter></ScatterRadiusVsType><ScatterRadiusVsType Radius='7'/><IncompatibleAttributeModifier>m\\2</IncompatibleAttributeModifier></WeaponTemplate>";
            Test(plugin,"scatter-mixed-"+mask,xml,platform,-1);++fixtures;
        }
    }
}

