using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string AreaXML(int g,string attrs="",string child="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AreaTypes[g]+"' id='Area' "+attrs+">"+child+"</ObjectSMCHelper></Behaviors>";}
    static string[] AreaSamples(Field f) {
        if(f.Kind=="ObjectStatus")return WeaponSetStatusValues;
        if(f.Name=="DamageType")return AreaDamageValues;
        if(f.Name=="DeathType")return AttachDeathEnumSamples;
        if(f.Name=="DamageFXType")return AreaFXValues;
        if(f.Name=="Options")return AreaStrafeFlags;
        if(f.Name=="OverflowOptions")return AreaChooserFlags;
        if(f.Name=="Flags")return AreaCircularFlags;
        return AIUpdateSamples(f.Kind=="NullableReference"?new Field(f.Name,"Reference"):f);
    }
    static void AreaTokens(byte[] b,int g,int at,bool big) {
        int t=MotionTable(b,big);AIUpdateSentinel(b,t,AreaKeys[g].Length);
        for(int j=0;j<AreaKeys[g].Length;++j) {
            int token=t+12*j,off=AreaOffsets[g][j];if(Word(b,token,big)!=AreaKeys[g][j] || Word(b,token+4,big)!=AreaFieldTypes[g][j])throw new Exception("Independent area original Token keys/types");
            if(j==0)continue;
            if(Array.IndexOf(AreaChildren[g],off)>=0) {
                int ct=(int)Word(b,token+8,big),value=(g==2 && off!=20) || g==4?at+off:(int)Word(b,at+off,big);
                if((value==0)!=(ct==0) || (ct!=0 && Word(b,ct+8,big)!=value))throw new Exception("Independent area inline/nullable child Token");
                if(ct!=0 && ((g==0 && off==84) || (g==2 && off==20))) {AIUpdateSentinel(b,ct,2);if(Word(b,ct+20,big)!=value+28)throw new Exception("Independent area nullable validation56 Tokens");}
                if(ct!=0 && g==2 && off!=20) {AIUpdateSentinel(b,ct,2);if(Word(b,ct+20,big)!=value+4)throw new Exception("Independent area inline range8 Tokens");}
            } else {
                uint value=g==1 && off==12?Word(b,at+12,big):(uint)(at+off);if(Word(b,token+8,big)!=value)throw new Exception("Independent area scalar/nullable reference Token offset");
            }
        }
    }
    static void AreaFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        for(int g=0;g<6;++g) {
            var def=Check(original,p,"",AreaXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big);
            if(b.Length!=at+AreaSizes[g] || Word(b,at,big)!=uint.Parse(AreaTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent complete area size/type/imports");
            for(int i=8;i<AreaSizes[g];++i) {
                int pos=i-i%4;uint word=0;
                if(g==0)word=pos==24?14u:pos==28?1u:pos==32?112u:pos==64?0x41200000u:0u;
                if(g==1 && pos==48)word=1;if(g==2 && pos==16)word=1;if(g==3 && pos==12)word=0xffffffffu;
                if(g==4)word=pos==12?0x42c80000u:pos==16?0x41200000u:pos==28?0x3e800000u:0u;
                if(b[at+i]!=(byte)(word>>(8*(big?3-i%4:i%4))))throw new Exception("Independent area complete defaults/padding group "+g+" byte "+i);
            }
            AreaTokens(b,g,at,big);
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+AreaTypes[g]+"' id='A'"));
            foreach(Field f in AreaFields[g])foreach(string value in AreaSamples(f))Check(original,p,"",AreaXML(g,f.Name+"='"+Escape(value)+"'"));
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","SpecialPowerTemplate='P'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AreaXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<CustomAnimAndDuration/>","<ToggleState/>"})Reject(p,"",AreaXML(g,"",bad));
            if(g!=1 && g!=2) {
                Check(original,p,"",AreaXML(g,"","<ObjectFilter/>"));
                foreach(string value in WeaponSetStatusValues)Check(original,p,"",AreaXML(g,"","<ObjectFilter Include='ALL' StatusBitFlags='"+Escape(value)+"'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></ObjectFilter>"));
                foreach(string value in KindNames)Check(original,p,"",AreaXML(g,"","<ObjectFilter Include='"+Escape(value)+"'/>"));
            } else Reject(p,"",AreaXML(g,"","<ObjectFilter/>"));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string nodes="";for(int i=0;i<count;++i)nodes+="<ObjectSMCHelper TypeId='"+AreaTypes[g]+"' id='A"+i+"'/>";Check(original,p,"Browser='a bb'","<Behaviors>"+nodes+"<ObjectSMCHelper TypeId='3925677257' id='Parent'/></Behaviors>");
                if(g==1 || g==2)continue;
                string ids="";for(int i=0;i<count;++i)ids+="<IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing>";
                string filter="<ObjectFilter Include='ALL' StatusBitFlags='ALL'>"+ids+"</ObjectFilter>",validation=(g==0 || g==5)?"<ObjectStatusValidation ForbiddenStatus='ALL' RequiredStatus='UNKNOWN'/>":"";
                var x=Check(original,p,"",AreaXML(g,"",validation+filter));b=x.InstanceData;at=LifecycleAt(b,big);int cursor=at+AreaSizes[g],offset=g==0?80:g==3?20:g==4?32:40;
                if(g!=4){if(Word(b,at+offset,big)!=cursor)throw new Exception("Independent area Filter112 allocation");cursor+=112;}
                cursor+=28+8*count;
                if(g==0 || g==5){if(Word(b,at+(g==0?84:24),big)!=cursor)throw new Exception("Independent own filter then validation/parent allocation");cursor+=56;}
                if(b.Length!=cursor)throw new Exception("Independent complete area filter/status/list/validation allocation");AreaTokens(b,g,at,big);
                var y=Check(original,p,"",AreaXML(g,"",filter+validation));Equal(x.InstanceData,y.InstanceData,"Area schema allocation order",false);Equal(x.RelocationData,y.RelocationData,"Area relocation order",false);Equal(x.ImportsData,y.ImportsData,"Area imports order",false);
            }
            foreach(string node in g==0 || g==5?new[]{"ObjectFilter","ObjectStatusValidation"}:g==2?new[]{"ObjectStatusValidation","TimeDelayRange","RadiusSpreadRange"}:g==3 || g==4?new[]{"ObjectFilter"}:new string[0])foreach(string bad in new[]{"<"+node+"/><"+node+"/>","<"+node+" Unknown='1'/>","<"+node+" xmlns='foreign'/>","<"+node+" xmlns:f='foreign' f:TypeId='1'/>","<"+node+">text</"+node+">","<"+node+"><Unknown/></"+node+">"})Reject(p,"",AreaXML(g,"",bad));
        }
        foreach(int g in new[]{0,2,5})foreach(string name in new[]{"RequiredStatus","ForbiddenStatus"})foreach(string value in WeaponSetStatusValues)Check(original,p,"",AreaXML(g,"","<ObjectStatusValidation "+name+"='"+Escape(value)+"'/>"));
        foreach(string name in new[]{"TimeDelayRange","RadiusSpreadRange"}) {
            Check(original,p,"",AreaXML(2,"","<"+name+"/>"));foreach(string field in new[]{"Low","High"})foreach(string value in AIUpdateSamples(new Field(field,name=="TimeDelayRange"?"Time":"Float")))Check(original,p,"",AreaXML(2,"","<"+name+" "+field+"='"+Escape(value)+"'/>"));
        }
        {string a="<TimeDelayRange Low='0.5s' High='100ms'/>",c="<RadiusSpreadRange Low='-17' High='23'/>",v="<ObjectStatusValidation RequiredStatus='ALL'/>";var x=Check(original,p,"",AreaXML(2,"UsePlayerStore='true' IncomingAngle='90d'",c+a+v));byte[] b=x.InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+20,big)!=at+44 || b.Length!=at+100 || Word(b,at+24,big)!=0x3f000000u || Word(b,at+32,big)!=0xc1880000u || Word(b,at+36,big)!=0x41b80000u || b[at+40]!=1)throw new Exception("Independent Bombard inline range layout and validation56 allocation");AreaTokens(b,2,at,big);var y=Check(original,p,"",AreaXML(2,"UsePlayerStore='true' IncomingAngle='90d'",v+a+c));Equal(x.InstanceData,y.InstanceData,"Bombard schema order",false);Equal(x.RelocationData,y.RelocationData,"Bombard relocation order",false);}
        foreach(string value in new[]{"","W","W\\0","W\\23"}) {byte[] b=Check(original,p,"",AreaXML(1,"SecondaryWeaponName='"+Escape(value)+"'")).InstanceData;int at=LifecycleAt(b,big),pointer=(int)Word(b,at+12,big);if(pointer!=at+56 || b.Length!=at+60 || Word(b,pointer,big)!=(value=="W\\0"?1u:value=="W\\23"?24u:0u))throw new Exception("Independent Strafe nullable weapon reference4");AreaTokens(b,1,at,big);}
        // DamageField reuses the complete parent, including structured nugget16/nullable Vector3 and validation56.
        foreach(Field f in new[]{new Field("FireInterval","Time"),new Field("AliveOnly","Boolean"),new Field("ActiveWhenDisabled","Bits")})foreach(string value in f.Name=="ActiveWhenDisabled"?SpecialPowerEnumSamples[1]:AIUpdateSamples(f))Check(original,p,"",AreaXML(5,f.Name+"='"+Escape(value)+"'"));
        foreach(string name in new[]{"WeaponName","FireDelay","OneShot"})foreach(string value in AIUpdateSamples(new Field(name,name=="WeaponName"?"Reference":name=="OneShot"?"Boolean":"Time")))Check(original,p,"",AreaXML(5,"","<FireWeaponNugget "+name+"='"+Escape(value)+"'/>"));
        foreach(string name in new[]{"x","y","z"})foreach(string value in AIUpdateSamples(new Field(name,"Float")))Check(original,p,"",AreaXML(5,"","<FireWeaponNugget><Offset "+name+"='"+Escape(value)+"'/></FireWeaponNugget>"));
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string rows="";for(int i=0;i<count;++i)rows+="<FireWeaponNugget WeaponName='W\\23' FireDelay='100ms' OneShot='true'><Offset x='1.25' y='-17' z='23'/></FireWeaponNugget>";
            string filter="<ObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing></ObjectFilter>",validation="<ObjectStatusValidation RequiredStatus='ALL'/>";var x=Check(original,p,"",AreaXML(5,"",validation+rows+filter));byte[] b=x.InstanceData;int at=LifecycleAt(b,big),cursor=at+44+112+28+4;
            if(Word(b,at+40,big)!=at+44 || Word(b,at+16,big)!=count || Word(b,at+20,big)!=(count==0?0:cursor))throw new Exception("Independent DamageField own filter before parent list");int rowBase=cursor;cursor+=count*16;
            for(int i=0;i<count;++i){if(Word(b,rowBase+16*i,big)!=24 || Word(b,rowBase+16*i+8,big)!=cursor || b[rowBase+16*i+12]!=1)throw new Exception("Independent complete FireWeapon nugget16/vector allocation");cursor+=12;}
            if(Word(b,at+24,big)!=cursor || b.Length!=cursor+56)throw new Exception("Independent DamageField parent validation last");AreaTokens(b,5,at,big);var y=Check(original,p,"",AreaXML(5,"",filter+rows+validation));Equal(x.InstanceData,y.InstanceData,"DamageField schema order",false);Equal(x.RelocationData,y.RelocationData,"DamageField relocation order",false);
        }
        foreach(string bad in new[]{"<FireWeaponNugget Unknown='1'/>","<FireWeaponNugget xmlns='foreign'/>","<FireWeaponNugget><Offset/><Offset/></FireWeaponNugget>","<FireWeaponNugget><Offset Unknown='1'/></FireWeaponNugget>","<FireWeaponNugget><Unknown/></FireWeaponNugget>","<FireWeaponNugget>text</FireWeaponNugget>"})Reject(p,"",AreaXML(5,"",bad));
        {byte[] b=Check(original,p,"",AreaXML(0,"DamageType='UNKNOWN' DeathType='UNKNOWN' DamageFXType='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+24,big)!=38 || Word(b,at+28,big)!=33 || Word(b,at+32,big)!=35)throw new Exception("Independent area exact single-enum fallbacks");}
        foreach(int g in new[]{1,3,4}) {byte[] b=Check(original,p,"",AreaXML(g,(g==1?"Options":g==3?"OverflowOptions":"Flags")+"='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+(g==1?52:g==3?16:8),big)!=(g==1?8u:g==3?2u:1u))throw new Exception("Independent area exact bit-enum fallbacks");}
    }
    static readonly string[] AreaTypes={"14531459","163451004","1230044432","3599948941","2191489142","4088922837"};
    static readonly int[] AreaSizes={88,56,44,24,144,44};
    static readonly Field[][] AreaFields={new[]{new Field("NodeLifetime","Time"),new Field("DamageInterval","Time"),new Field("Distance","Float"),new Field("Damage","Float"),new Field("DamageType","Enum"),new Field("DeathType","Enum"),new Field("DamageFXType","Enum"),new Field("ObjectStatusForbiddenForDamage","ObjectStatus"),new Field("MinDistanceTolerance","Float"),new Field("MaxValidHeightOffGround","Float"),new Field("FXParticleSystemTemplate","Reference"),new Field("NodeFXList","Reference")},new[]{new Field("WeaponName","Reference"),new Field("SecondaryWeaponName","NullableReference"),new Field("Radius","Float"),new Field("MinRadius","Float"),new Field("PreattackDistance","Float"),new Field("SweepFrequency","Float"),new Field("SweepAmplitude","Float"),new Field("DivingFloor","Float"),new Field("InitialSweepPhase","Float"),new Field("TargetJitterOffset","Float"),new Field("ShotsToFirePerFrame","Unsigned"),new Field("Options","Bits")},new[]{new Field("UsePlayerStore","Boolean"),new Field("IncomingAngle","Angle"),new Field("WeaponName","Reference"),new Field("MaxShots","Signed")},new[]{new Field("Radius","Float"),new Field("MaxAttacksPerTarget","Signed"),new Field("OverflowOptions","Bits")},new[]{new Field("Flags","Bits"),new Field("MaxRadius","Float"),new Field("ArcDistance","Float"),new Field("SweepFXList","Reference"),new Field("SweepWeapon","Reference"),new Field("FXTimeout","Time")},new[]{new Field("Radius","Signed"),new Field("RequiredUpgrade","Reference")}};
    static readonly uint[][] AreaKeys={new uint[]{1u,4262558380u,1244447716u,32598927u,2083775721u,1179152369u,3322473782u,799690338u,1803003293u,638618229u,3276709033u,686292351u,1469138589u,3832456114u,3933465695u},new uint[]{1u,271546182u,2996458196u,1328662213u,3963169641u,4273549416u,4126236977u,2459979192u,1929535936u,2647194418u,4288273647u,3202518795u,3202187060u},new uint[]{1u,1083408591u,1869509643u,271546182u,3166299499u,3933465695u,3554337147u,2514943143u},new uint[]{1u,1328662213u,1443257331u,2935781223u,3832456114u},new uint[]{1u,4081386890u,3259461365u,1112161480u,732717553u,1093366695u,2326671296u,3832456114u},new uint[]{1u,1328662213u,2138150697u,3832456114u}};
    static readonly uint[][] AreaFieldTypes={new uint[]{4043318533u,2575722984u,2575722984u,1683514611u,1683514611u,1179152369u,3322473782u,799690338u,2814214430u,1683514611u,1683514611u,1139920438u,219975299u,3832456114u,1049596812u},new uint[]{4043318533u,3494305058u,3494305058u,1683514611u,1683514611u,1683514611u,1683514611u,1683514611u,1683514611u,1683514611u,1683514611u,1982953380u,1635817665u},new uint[]{4043318533u,2011428087u,1310124621u,3494305058u,602296504u,1049596812u,1266366031u,1581808561u},new uint[]{2769121946u,1683514611u,602296504u,713683839u,3832456114u},new uint[]{4043318533u,637034980u,1683514611u,1683514611u,219975299u,3494305058u,2575722984u,3832456114u},new uint[]{2995535186u,602296504u,1253711657u,3832456114u}};
    static readonly int[][] AreaOffsets={new[]{0,8,12,16,20,24,28,32,36,64,68,72,76,80,84},new[]{0,8,12,16,20,24,28,32,36,40,44,48,52},new[]{0,40,8,12,16,20,24,32},new[]{0,8,12,16,20},new[]{0,8,12,16,20,24,28,32},new[]{0,32,36,40}};
    static readonly int[][] AreaChildren={new int[]{80,84},new int[]{},new int[]{20,24,32},new int[]{20},new int[]{32},new int[]{40}};
    static readonly string[] AreaDamageValues={"ALL","AUTO_CANNON","BLAST","CANNON","CONCUSSIVE","CRUSH","EXPLOSIVE","FALLING","FIRE","FLAK","FLAME","FORCE","GRENADE","GUN","HEALING","IMPACT","KI","LASER","LOGICAL_FIRE","MAGIC","MELEE","MINE","PASSENGER","PENALTY","PIERCE","PLASMA","POISON","PRISM","RADIATION","REFLECTED","ROCKET","SLASH","SNIPER","STORM_SHIELD","TESLA","TIBERIUM","UNDEFINED","UNRESISTABLE","WATER","","UNKNOWN","unknown","ALL","all","-1","0","1"," UNDEFINED","UNDEFINED ","UNDEFINED FIRE","+","-"};
    static readonly string[] AreaFXValues={"ALIEN_BOMB","ALIEN_BUZZER","ALIEN_CANNON","ALIEN_DISINTEGRATOR","ALIEN_GRENADE","ALIEN_ION","ALIEN_LASER","ALIEN_MACHINEGUN","ALIEN_MINE","ALIEN_PLASMADISK","ALIEN_RAILGUN","ALIEN_SONIC","ALIEN_TIOXIN","ALLIED_ATHENA","ALLIED_BOMB","ALLIED_BOMBLVL1","ALLIED_BOMBLVL2","ALLIED_BOMBLVL3","ALLIED_CANNON","ALLIED_CHAINGUN","ALLIED_DOG","ALLIED_FIGHTER","ALLIED_INFANTRYMISSILE","ALLIED_MISSILE","ALLIED_PAINTER","ALLIED_PARTICLE","ALLIED_PHALANX","ALLIED_PRESISIONBOMB","ALLIED_PRIMAGECANNON","ALLIED_PRIMAGECANNONVET1","ALLIED_PRIMAGECANNONVET2","ALLIED_PRISM","ALLIED_SAM","ALLIED_SCRAMBLER","ALLIED_SHOTGUN","ALLIED_SONIC","ALLIED_TANYA","CLUBBING","DEFAULT","GDI_BOMB","GDI_CANNON","GDI_FLAMETHROWER","GDI_GRENADE","GDI_ION","GDI_LASER","GDI_MACHINEGUN","GDI_MINE","GDI_MISSILE","GDI_RAILGUN","GDI_RIFLE","GDI_SHOCKWAVE","GDI_SONIC","GENERIC_FIRE","JAPAN_AIRCRAFT","JAPAN_AUTOCANNON","JAPAN_BEAM","JAPAN_BOMB","JAPAN_BONZAI","JAPAN_CANNON","JAPAN_FORCE","JAPAN_GUN","JAPAN_KAMIKAZE","JAPAN_MECHA","JAPAN_MECHAMISSILE","JAPAN_MECHARUSH","JAPAN_MISSILE","JAPAN_PLASMA","JAPAN_PSIONIC","JAPAN_ROCKET","JAPAN_RUSH","JAPAN_SHIRUKEN","JAPAN_TORPEDO","NEUTRAL_CANNON","NOD_BOMB","NOD_CANNON","NOD_FLAMETHROWER","NOD_GRENADE","NOD_HALLUCINE","NOD_ION","NOD_LASER","NOD_MACHINEGUN","NOD_MINE","NOD_MISSILE","NOD_RAILGUN","NOD_RIFLE","NOD_SEED","NOD_SONIC","POISON","RADIATION","REFLECTED","SOVIET_AK47","SOVIET_BEAR","SOVIET_BIGBOMB","SOVIET_BOMB","SOVIET_CANNON","SOVIET_DEBRIS","SOVIET_FLAK","SOVIET_FLAKVET","SOVIET_HARPOON","SOVIET_IRONCURTAIN","SOVIET_LASER","SOVIET_LEECH","SOVIET_MACHINEGUN","SOVIET_MAGNETICBOMB","SOVIET_MISSILE","SOVIET_MOLOTOV","SOVIET_ORBITAL","SOVIET_ORBITAL2","SOVIET_ORBITAL3","SOVIET_REPAIR","SOVIET_SNIPERRIFLE","SOVIET_TESLA","SOVIET_TORPEDO","UNDEFINED","","UNKNOWN","unknown","ALL","all","-1","0","1"," UNDEFINED","UNDEFINED ","UNDEFINED FIRE","+","-"};
    static readonly string[] AreaStrafeFlags={"CANNOT_IDLE","DO_NOT_AVOID_TALLBUILDING","FLEE_STRAIGHT_AHEAD","USE_TARGET_CHOOSER","","UNKNOWN","unknown","ALL","all","-1","0","1"," UNDEFINED","UNDEFINED ","UNDEFINED FIRE","+","-","ALL -ALL -ALL","UNKNOWN -UNKNOWN -UNKNOWN","ALL -UNKNOWN","+ALL","-ALL","CANNOT_IDLE -CANNOT_IDLE -CANNOT_IDLE","DO_NOT_AVOID_TALLBUILDING -DO_NOT_AVOID_TALLBUILDING -DO_NOT_AVOID_TALLBUILDING","FLEE_STRAIGHT_AHEAD -FLEE_STRAIGHT_AHEAD -FLEE_STRAIGHT_AHEAD","USE_TARGET_CHOOSER -USE_TARGET_CHOOSER -USE_TARGET_CHOOSER"};
    static readonly string[] AreaChooserFlags={"BREAK_OFF_ATTACK","CHOOSE_RANDOM_POSITION","","UNKNOWN","unknown","ALL","all","-1","0","1"," UNDEFINED","UNDEFINED ","UNDEFINED FIRE","+","-","ALL -ALL -ALL","UNKNOWN -UNKNOWN -UNKNOWN","ALL -UNKNOWN","+ALL","-ALL","BREAK_OFF_ATTACK -BREAK_OFF_ATTACK -BREAK_OFF_ATTACK","CHOOSE_RANDOM_POSITION -CHOOSE_RANDOM_POSITION -CHOOSE_RANDOM_POSITION"};
    static readonly string[] AreaCircularFlags={"ONLY_ON_WATER","","UNKNOWN","unknown","ALL","all","-1","0","1"," UNDEFINED","UNDEFINED ","UNDEFINED FIRE","+","-","ALL -ALL -ALL","UNKNOWN -UNKNOWN -UNKNOWN","ALL -UNKNOWN","+ALL","-ALL","ONLY_ON_WATER -ONLY_ON_WATER -ONLY_ON_WATER"};
}
