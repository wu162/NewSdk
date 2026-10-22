using System;
using System.Collections.Generic;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

unsafe partial class WeaponOracle
{
    static readonly string[] NuggetNames={"DamageNugget","DOTNugget","GrapplingHookNugget","LeechPercentMaxHealthDamageNugget","LeechHealthDamageNugget","LineDamageNugget","DamageAndSpawnNugget","FireOnObjectsNugget","DamageContainedNugget"};
    static readonly uint[] NuggetIds={3237297817u,4022030886u,1093138400u,921679887u,4012276286u,4021248832u,3266020104u,2014632688u,2459653016u};
    static string NuggetXml(string content,string attributes) {
        return "<WeaponTemplate xmlns='"+AssetNamespace+"' id='Nuggets' "+attributes+"><Nuggets>"+content+"</Nuggets></WeaponTemplate>";
    }
    static string Nugget(int kind,string attrs,string children) {
        return "<"+NuggetNames[kind]+" TypeId='"+NuggetIds[kind]+"' "+attrs+">"+children+"</"+NuggetNames[kind]+">";
    }
    static void NuggetFixture(IAssetBuilderPlugin plugin,TargetPlatform platform,string label,string content,string attributes) {
        Test(plugin,"nugget/"+label,NuggetXml(content,attributes),platform,-1);++fixtures;
    }
    static void DamageLayout(IAssetBuilderPlugin plugin,TargetPlatform platform,int count) {
        string content="";for(int i=0;i<count;++i) content+=Nugget(0,"Damage='"+i+"'","");
        string text=NuggetXml(content,"");NuggetFixture(plugin,platform,"layout-"+count,content,"");
        var xml=new XmlDocument();xml.LoadXml(text);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int tokens=1296+516*count,array=tokens+328;
        if(result.InstanceData.Length!=1624+672*count || Word(result.InstanceData,tokens+256,big)!=count ||
            Word(result.InstanceData,tokens+260,big)!=(count==0?0:array) || result.RelocationData.Length!=(count==0?0:4*(count+2)) || result.ImportsData.Length!=0)
            throw new Exception("Nugget independent size/count/relocation length: "+count);
        if(count>0 && (Word(result.RelocationData,0,big)!=tokens+260 || Word(result.RelocationData,4*(count+1),big)!=uint.MaxValue)) throw new Exception("Nugget relocation endpoints");
        for(int i=0;i<count;++i) {
            int data=array+4*count+152*i;
            if(Word(result.InstanceData,array+4*i,big)!=data || Word(result.InstanceData,data,big)!=3237297817u ||
                Word(result.InstanceData,data+40,big)!=BitConverter.ToUInt32(BitConverter.GetBytes((float)i),0) ||
                Word(result.InstanceData,data+44,big)!=0xbf800000u || Word(result.InstanceData,data+56,big)!=0xbf800000u ||
                Word(result.InstanceData,data+60,big)!=0xbf800000u || Word(result.InstanceData,data+68,big)!=0x3f800000u ||
                Word(result.InstanceData,data+76,big)!=14u || Word(result.InstanceData,data+80,big)!=1u ||
                Word(result.InstanceData,data+84,big)!=112u || Word(result.InstanceData,data+88,big)!=0u || result.InstanceData[data+145]!=1)
                throw new Exception("Nugget independent defaults/order: "+i);
        }
    }
    static void DerivedNuggetLayout(TargetPlatform platform,int kind) {
        var xml=new XmlDocument();xml.LoadXml(NuggetXml(Nugget(kind,"",""),""));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",AssetNamespace);
        var result=WeaponCompiler.ProcessInstance(Declaration(xml,ns),platform);bool big=platform!=TargetPlatform.Win32;
        int[] extras={0,60,24,36,36,96,60,72,216},sizes={152,164,152,156,156,176,220,164,272};
        int tokens=1812+extras[kind],data=tokens+332;
        if(result.InstanceData.Length!=tokens+332+sizes[kind] || Word(result.InstanceData,tokens+328,big)!=data ||
            Word(result.InstanceData,data,big)!=NuggetIds[kind] || Word(result.InstanceData,1284,big)!=2 || Word(result.InstanceData,1288,big)!=NuggetIds[kind])
            throw new Exception("Independent derived nugget size/tag: "+kind);
        if((kind==3 || kind==4) && Word(result.InstanceData,data+152,big)!=0x3f800000u) throw new Exception("Leech percentage default");
        if(kind==1 && (Word(result.InstanceData,data+152,big)!=0 || Word(result.InstanceData,data+156,big)!=0 || result.InstanceData[data+160]!=1)) throw new Exception("DOT default extra fields");
        if(kind==6 && Word(result.InstanceData,data+152,big)!=0x3f800000u) throw new Exception("Spawn probability default");
        if(kind==7 && (Word(result.InstanceData,data+152,big)!=0 || Word(result.InstanceData,data+156,big)!=0 || result.InstanceData[data+160]!=0 || result.InstanceData[data+161]!=1)) throw new Exception("FireOnObjects defaults");
        if(kind==6) {
            for(int at=156;at<=216;at+=4) if(Word(result.InstanceData,data+at,big)!=0) throw new Exception("Spawn default mask/typed ID");
            if(Word(result.InstanceData,1304,big)!=1356 || Word(result.InstanceData,1316,big)!=data+152 || Word(result.InstanceData,1328,big)!=data+156 || Word(result.InstanceData,1340,big)!=data+216) throw new Exception("Spawn token member offsets");
        }
        if(kind==7 && (Word(result.InstanceData,1304,big)!=1368 || Word(result.InstanceData,1316,big)!=data+152 || Word(result.InstanceData,1328,big)!=data+160 || Word(result.InstanceData,1340,big)!=data+161 || Word(result.InstanceData,1352,big)!=0)) throw new Exception("Fire token member offsets");
        if(kind==8 && (Word(result.InstanceData,data+152,big)!=100 || Word(result.InstanceData,data+156,big)!=0 || Word(result.InstanceData,1340,big)!=1848)) throw new Exception("Contained defaults/inline filter token");
        if(kind==5) for(int at=152;at<176;at+=4) if(Word(result.InstanceData,data+at,big)!=0) throw new Exception("Line default fields/padding");
    }
    static void Nuggets(IAssetBuilderPlugin plugin,TargetPlatform platform) {
        DamageLayout(plugin,platform,0);
        foreach(int count in new[] {1,2,3,7,16,17,31,32,33,64,127,128,129,255,256,257}) DamageLayout(plugin,platform,count);
        foreach(int kind in new[] {0,1,2,3,4,5,6,7,8}) {
            DerivedNuggetLayout(platform,kind);
            NuggetFixture(plugin,platform,"defaults-"+kind,Nugget(kind,"",""),"");
            foreach(string field in new[] {"Damage","DamageTaperOff","MinRadius","DamageMaxHeight","DamageMaxHeightAboveTerrain","FlankingBonus","FlankedScalar","DrainLifeMultiplier","DamageSpeed","Radius"})
                foreach(string value in new[] {"","-0","-23.75","2d1","1e-40","1e40","0x1p2","NaN","garbage","25.5%"})
                    NuggetFixture(plugin,platform,kind+"/"+field+"/"+value,Nugget(kind,Attribute(field,value),""),"");
            foreach(string field in new[] {"DamageArc","DelayTimeSeconds"}) foreach(string value in new[] {"","-0","-90d","360d","0.5r","1500ms","2d1s","1e-7s","NaN","bad"})
                NuggetFixture(plugin,platform,kind+"/"+field+"/"+value,Nugget(kind,Attribute(field,value),""),"");
            foreach(string field in new[] {"DamageArcInverted","AcceptDamageAdd","OnlyKillOwnerWhenTriggered","DrainLife","NotifyOwnerOnVictimDeath","NotifyObserversOnPreDamageEffectPosition","ForceFXPositionToVictim","RadiusAffectsBridges"})
                foreach(string value in new[] {"","false","true","TRUE","1","0","t","yes"," true"}) NuggetFixture(plugin,platform,kind+"/"+field+"/"+value,Nugget(kind,Attribute(field,value),""),"");
            foreach(string field in new[] {"UnderAttackOverrideEvaEvent","VictimShroudRevealer"}) foreach(string value in new[] {"","event","e\\0","e\\7","e\\-1","e\\2147483648","e\\-2147483649","e\\ 12tail"})
                NuggetFixture(plugin,platform,kind+"/"+field+"/"+value,Nugget(kind,Attribute(field,value),""),"");
            foreach(string field in new[] {"ForbiddenTargetObjectStatus","ForbiddenTargetModelCondition","InvalidTargetStatus"}) foreach(string value in new[] {"","INVALID","NONE","unknown","ALL","-all TOPPLED","AFLAME","TOPPLED","TOPPLED -TOPPLED","INSIDE_GARRISON UNATTACKABLE UNDER_IRON_CURTAIN"})
                NuggetFixture(plugin,platform,kind+"/"+field+"/"+value,Nugget(kind,Attribute(field,value),""),"");
            foreach(string value in new[] {"CENTER_2D","CENTER_3D","CIRCLE","EXTENTS_3D","SPHERE","","unknown","NONE","normal"," UNDEFINED"}) NuggetFixture(plugin,platform,"PartitionFilterTestType-"+kind+"-"+value,Nugget(kind,Attribute("PartitionFilterTestType",value),""),"");
            foreach(string value in new[] {"ALL","AUTO_CANNON","BLAST","CANNON","CONCUSSIVE","CRUSH","EXPLOSIVE","FALLING","FIRE","FLAK","FLAME","FORCE","GRENADE","GUN","HEALING","IMPACT","KI","LASER","LOGICAL_FIRE","MAGIC","MELEE","MINE","PASSENGER","PENALTY","PIERCE","PLASMA","POISON","PRISM","RADIATION","REFLECTED","ROCKET","SLASH","SNIPER","STORM_SHIELD","TESLA","TIBERIUM","UNDEFINED","UNRESISTABLE","WATER","","unknown","NONE","normal"," UNDEFINED"}) NuggetFixture(plugin,platform,"DamageType-"+kind+"-"+value,Nugget(kind,Attribute("DamageType",value),""),"");
            foreach(string value in new[] {"ALL","BITTEN","BURNED","CATALYST","CRUSHED","DETONATED","ELECTROCUTED","EXPLODED","EXTRA_1","EXTRA_2","EXTRA_3","EXTRA_4","EXTRA_5","EXTRA_6","EXTRA_7","FADED","FLOODED","INTERNAL_DESTRUCTION","IRRADIATED","KNOCKBACK","LASERED","MAULED","NONE","NORMAL","POISONED","POISONED_BETA","PRODUCTION_CANCELLED","SHATTERED","SLAUGHTERED","SPLATTED","STABBED","SUICIDED","SUPERNATURAL","TOPPLED","","unknown","NONE","normal"," UNDEFINED"}) NuggetFixture(plugin,platform,"DeathType-"+kind+"-"+value,Nugget(kind,Attribute("DeathType",value),""),"");
            foreach(string value in new[] {"ALIEN_BOMB","ALIEN_BUZZER","ALIEN_CANNON","ALIEN_DISINTEGRATOR","ALIEN_GRENADE","ALIEN_ION","ALIEN_LASER","ALIEN_MACHINEGUN","ALIEN_MINE","ALIEN_PLASMADISK","ALIEN_RAILGUN","ALIEN_SONIC","ALIEN_TIOXIN","ALLIED_ATHENA","ALLIED_BOMB","ALLIED_BOMBLVL1","ALLIED_BOMBLVL2","ALLIED_BOMBLVL3","ALLIED_CANNON","ALLIED_CHAINGUN","ALLIED_DOG","ALLIED_FIGHTER","ALLIED_INFANTRYMISSILE","ALLIED_MISSILE","ALLIED_PAINTER","ALLIED_PARTICLE","ALLIED_PHALANX","ALLIED_PRESISIONBOMB","ALLIED_PRIMAGECANNON","ALLIED_PRIMAGECANNONVET1","ALLIED_PRIMAGECANNONVET2","ALLIED_PRISM","ALLIED_SAM","ALLIED_SCRAMBLER","ALLIED_SHOTGUN","ALLIED_SONIC","ALLIED_TANYA","CLUBBING","DEFAULT","GDI_BOMB","GDI_CANNON","GDI_FLAMETHROWER","GDI_GRENADE","GDI_ION","GDI_LASER","GDI_MACHINEGUN","GDI_MINE","GDI_MISSILE","GDI_RAILGUN","GDI_RIFLE","GDI_SHOCKWAVE","GDI_SONIC","GENERIC_FIRE","JAPAN_AIRCRAFT","JAPAN_AUTOCANNON","JAPAN_BEAM","JAPAN_BOMB","JAPAN_BONZAI","JAPAN_CANNON","JAPAN_FORCE","JAPAN_GUN","JAPAN_KAMIKAZE","JAPAN_MECHA","JAPAN_MECHAMISSILE","JAPAN_MECHARUSH","JAPAN_MISSILE","JAPAN_PLASMA","JAPAN_PSIONIC","JAPAN_ROCKET","JAPAN_RUSH","JAPAN_SHIRUKEN","JAPAN_TORPEDO","NEUTRAL_CANNON","NOD_BOMB","NOD_CANNON","NOD_FLAMETHROWER","NOD_GRENADE","NOD_HALLUCINE","NOD_ION","NOD_LASER","NOD_MACHINEGUN","NOD_MINE","NOD_MISSILE","NOD_RAILGUN","NOD_RIFLE","NOD_SEED","NOD_SONIC","POISON","RADIATION","REFLECTED","SOVIET_AK47","SOVIET_BEAR","SOVIET_BIGBOMB","SOVIET_BOMB","SOVIET_CANNON","SOVIET_DEBRIS","SOVIET_FLAK","SOVIET_FLAKVET","SOVIET_HARPOON","SOVIET_IRONCURTAIN","SOVIET_LASER","SOVIET_LEECH","SOVIET_MACHINEGUN","SOVIET_MAGNETICBOMB","SOVIET_MISSILE","SOVIET_MOLOTOV","SOVIET_ORBITAL","SOVIET_ORBITAL2","SOVIET_ORBITAL3","SOVIET_REPAIR","SOVIET_SNIPERRIFLE","SOVIET_TESLA","SOVIET_TORPEDO","UNDEFINED","","unknown","NONE","normal"," UNDEFINED"}) NuggetFixture(plugin,platform,"DamageFXType-"+kind+"-"+value,Nugget(kind,Attribute("DamageFXType",value),""),"");
            foreach(string value in new[] {"BECOME_UNDEAD","NORMAL","SELF","UNDEFINED","","unknown","NONE","normal"," UNDEFINED"}) NuggetFixture(plugin,platform,"DamageSubType-"+kind+"-"+value,Nugget(kind,Attribute("DamageSubType",value),""),"");
            foreach(int mask in new[] {0,1,2,3,4,5,6,7}) {
                string attrs=((mask&1)!=0?"ForbiddenTargetObjectStatus='ALL' ":"")+((mask&2)!=0?"ForbiddenTargetModelCondition='ALL'":"");
                string child=(mask&4)!=0?"<SpecialObjectFilter/>":"";
                NuggetFixture(plugin,platform,"base-pointers-"+kind+"-"+mask,Nugget(kind,attrs,child),"");
            }
            foreach(int count in new[] {0,1,2,7,17,33,129,257}) {
                string child="<SpecialObjectFilter Rule='ANY' Include='AIRCRAFT' Exclude='ALL' StatusBitFlags='AFLAME' StatusBitFlagsExclude='ALL'><IncludeThing>Obj</IncludeThing><ExcludeThing>Other</ExcludeThing></SpecialObjectFilter>";
                for(int i=0;i<count;++i) child+="<RequiredUpgrade>Up_"+(i%7)+"</RequiredUpgrade><ForbiddenUpgrade>Other_"+i+"</ForbiddenUpgrade>";
                NuggetFixture(plugin,platform,"upgrades-"+kind+"-"+count,Nugget(kind,"ForbiddenTargetObjectStatus='' ForbiddenTargetModelCondition='SUCKED_UP_HIGH'",child),"");
            }
            foreach(int count in new[] {0,1,2,7,17,33,129,257}) {
                string child="<RequiredUpgrade>CommonUpgrade</RequiredUpgrade>";
                for(int i=0;i<count;++i) child+="<DamageScalarDetails Scalar='"+(i%2==0?"12.5%":"250")+"'><Filter Rule='ANY' Include='AIRCRAFT' StatusBitFlags='ALL'><IncludeThing>Obj</IncludeThing><ExcludeThing>Other</ExcludeThing></Filter><RequiredUpgrade>ScalarUpgrade</RequiredUpgrade></DamageScalarDetails>";
                NuggetFixture(plugin,platform,"scalar-list-"+kind+"-"+count,Nugget(kind,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'",child),"");
            }
            foreach(string scalar in new[] {"","-25.5%","33.3333333","2d1","1e40","garbage"})
                NuggetFixture(plugin,platform,"scalar-"+kind+"-"+scalar,Nugget(kind,"","<DamageScalarDetails "+Attribute("Scalar",scalar)+"/><DamageScalarDetails><Filter/></DamageScalarDetails>"),"");
            NuggetFixture(plugin,platform,"duplicates-"+kind,Nugget(kind,"","<SpecialObjectFilter Rule='ANY'/><SpecialObjectFilter Rule='NONE'/><DamageScalarDetails><Filter Rule='ANY'/><Filter Rule='NONE'/></DamageScalarDetails>"),"");
        }
        foreach(string field in new[] {"DamageInterval","DamageDuration"}) foreach(string value in new[] {"","-2s","1500ms","2d1s","1e-7s","bad"})
            NuggetFixture(plugin,platform,"dot-"+field+"-"+value,Nugget(1,Attribute(field,value),""),"");
        foreach(string value in new[] {"","false","true","1","0","TRUE","yes"}) NuggetFixture(plugin,platform,"dot-bool-"+value,Nugget(1,Attribute("RemoveIfHealed",value),""),"");
        foreach(int kind in new[] {3,4}) foreach(string value in new[] {"", "100%", "-25.5%", "33.3333333333", "2d1", "1e40", "garbage"})
            NuggetFixture(plugin,platform,"leech-percent-"+kind+"-"+value,Nugget(kind,Attribute(kind==3?"PercentMaxHealthToTake":"PercentDamageToHeal",value),""),"");
        foreach(string field in new[] {"OffsetAngle","LineWidth","LineLengthLeadIn","LineLengthLeadOut","OverShootDistance","UseDynamicLineLength"})
            foreach(string value in new[] {"", "0", "-17.5", "90d", "0.5r", "2d1", "1e40", "true", "false", "garbage"})
                NuggetFixture(plugin,platform,"line-"+field+"-"+value,Nugget(5,Attribute(field,value),""),"");
        foreach(string value in new[] {"", "-0", "-25.5", "2d1", "1e40", "garbage"}) NuggetFixture(plugin,platform,"spawn-probability-"+value,Nugget(6,Attribute("SpawnProbability",value),"<SpawnTemplate>mytank</SpawnTemplate>"),"");
        foreach(string value in new[] {"", "INVALID", "ALL", "-ALL TOPPLED", "TOPPLED -TOPPLED", "unknown", "SUCKED_UP_HIGH"}) NuggetFixture(plugin,platform,"spawn-model-"+value,Nugget(6,Attribute("SpawnedModelConditionFlags",value),"<SpawnTemplate>mytank</SpawnTemplate>"),"");
        foreach(string value in new[] {"", "mytank", "MYTANK", "Obj & <test>", "aé", "中文", new string('x',257)}) NuggetFixture(plugin,platform,"spawn-object-"+value,Nugget(6,"","<SpawnTemplate>"+System.Security.SecurityElement.Escape(value)+"</SpawnTemplate>"),"");
        NuggetFixture(plugin,platform,"spawn-first-child",Nugget(6,"","<SpawnTemplate>first</SpawnTemplate><SpawnTemplate>second</SpawnTemplate>"),"");
        foreach(string field in new[] {"Weapon", "FireOnVictimObject", "VictimMustBeAlive"}) foreach(string value in new[] {"", "false", "true", "1", "0", "w", "w\\0", "w\\7", "w\\-1", "w\\2147483648", "w\\-2147483649"}) NuggetFixture(plugin,platform,"fire-"+field+"-"+value,Nugget(7,Attribute(field,value),"<Filter/><SpecialObjectFilter/>"),"");
        foreach(int count in new[] {0,1,2,7,17,33,129,257}) {
            string child="<SpecialObjectFilter StatusBitFlags='ALL'/><DamageScalarDetails><Filter StatusBitFlags='ALL'/><RequiredUpgrade>Up</RequiredUpgrade></DamageScalarDetails><Filter StatusBitFlags='ALL' StatusBitFlagsExclude='ALL'>";
            for(int i=0;i<count;++i) child+="<IncludeThing>Obj_"+i+"</IncludeThing><ExcludeThing>Other_"+i+"</ExcludeThing>";
            child+="</Filter>";NuggetFixture(plugin,platform,"fire-filter-lists-"+count,Nugget(7,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL' Weapon='w\\9'",child),"");
        }
        NuggetFixture(plugin,platform,"fire-first-filter",Nugget(7,"","<Filter Rule='ANY'/><Filter Rule='NONE'/>"),"");
        foreach(string field in new[] {"MaxUnitsToDamage","WindowBlastFX"}) foreach(string value in new[] {"", "100", "-1", "010", "0x7f", "2147483648", "-2147483649", "garbage", "fx\\0", "fx\\-1", "fx\\2147483648"}) NuggetFixture(plugin,platform,"contained-"+field+"-"+value,Nugget(8,Attribute(field,value),"<DamageObjectFilter/>"),"");
        foreach(int count in new[] {0,1,2,7,33,129,257}) {
            string child="<SpecialObjectFilter StatusBitFlags='ALL'/><DamageScalarDetails><Filter StatusBitFlags='ALL'/></DamageScalarDetails><DamageObjectFilter StatusBitFlags='ALL' StatusBitFlagsExclude='ALL'>";
            for(int i=0;i<count;++i) child+="<IncludeThing>Obj_"+i+"</IncludeThing><ExcludeThing>Other_"+i+"</ExcludeThing>";
            child+="</DamageObjectFilter>";NuggetFixture(plugin,platform,"contained-filter-lists-"+count,Nugget(8,"ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL' WindowBlastFX='fx\\9'",child),"");
        }
        NuggetFixture(plugin,platform,"contained-first-filter",Nugget(8,"","<DamageObjectFilter Rule='ANY'/><DamageObjectFilter Rule='NONE'/>"),"");
        Reject(NuggetXml(Nugget(0,"","<DamageObjectFilter/>"),""));
        Reject(NuggetXml(Nugget(8,"","<Filter/>"),""));
        foreach(string id in new[] {"3237297817","0xc0f54699","+3237297817","3237297817tail"}) {
            // Hex fixture is supplied from the exact decimal constant below.
            string value=id=="0xc0f54699"?"0x"+NuggetIds[0].ToString("x"):id;
            NuggetFixture(plugin,platform,"typeid-"+id,"<AnyElement "+Attribute("TypeId",value)+" Damage='17'/>","");
        }
        for(int mask=0;mask<128;++mask) {
            string[] blocks={"OverrideVoiceAttackSound Sound='s\\2'","OverrideVoiceEnterStateAttackSound EvaEvent='e\\3'","PreAttackDelay MinSeconds='1s'","FiringDuration MaxSeconds='2s'","CoolDownDelayBetweenShots MinSeconds='3s'","ClipReloadTime MaxSeconds='4s'","WeaponAiHintInfo MaxSpeedOfTarget='5'"};
            string xml="<WeaponTemplate xmlns='"+AssetNamespace+"' id='MixedNuggets' FireSound='s\\0' RequiredFiringObjectStatus='ALL' PreferredTargetBone='bone'>";
            for(int i=6;i>=0;--i) if((mask&(1<<i))!=0) xml+="<"+blocks[i]+"/>";
            xml+="<ScatterRadiusVsType Radius='3' RequiredModelConditions='ALL'><Filter StatusBitFlags='AFLAME'><IncludeThing>Obj</IncludeThing></Filter></ScatterRadiusVsType><HitStoredObjectFilter StatusBitFlags='ALL'/><Nuggets>";
            for(int kind=0;kind<9;++kind) xml+=Nugget(kind,"Damage='27.5' ForbiddenTargetObjectStatus='ALL' ForbiddenTargetModelCondition='ALL'","<SpecialObjectFilter Include='AIRCRAFT'/><RequiredUpgrade>BaseUp</RequiredUpgrade><ForbiddenUpgrade>OtherUp</ForbiddenUpgrade><DamageScalarDetails Scalar='25%'><Filter StatusBitFlags='ALL'/><RequiredUpgrade>ScalarUp</RequiredUpgrade></DamageScalarDetails>");
            xml+="</Nuggets><IncompatibleAttributeModifier>m\\1</IncompatibleAttributeModifier></WeaponTemplate>";
            Test(plugin,"nugget-mixed-"+mask,xml,platform,-1);++fixtures;
        }
        Reject(NuggetXml(Nugget(0,"","<Filter/>"),""));
        Reject(NuggetXml(Nugget(0,"","<SpawnTemplate>Obj</SpawnTemplate>"),""));
        foreach(uint id in new uint[] {576040660u,169470683u,609705483u,978137461u,1542018985u,1115743736u,1622083673u,1757028425u,1675169590u,2010867179u,2159708756u,2360033115u,2475108259u,2940373499u,2769610658u,3111574559u,3665090361u,3520092115u,3694898303u,3915396258u,3821195611u,4104564135u,4286253161u}) Reject(NuggetXml("<Unrecovered TypeId='"+id+"'/>",""));
        foreach(string content in new[] {"<DamageNugget/>","<DamageNugget TypeId='0'/>","<DamageNugget TypeId='576040660'/>","<DamageNugget TypeId='3237297817' Unknown='1'/>","<DamageNugget TypeId='3237297817'><Unknown/></DamageNugget>","<DamageNugget TypeId='3237297817'><DamageScalarDetails Unknown='1'/></DamageNugget>","<DamageNugget TypeId='3237297817'><RequiredUpgrade Unknown='1'/></DamageNugget>","<DamageNugget TypeId='3237297817'><DamageScalarDetails><Filter Unknown='1'/></DamageScalarDetails></DamageNugget>"}) Reject(NuggetXml(content,""));
    }
}

