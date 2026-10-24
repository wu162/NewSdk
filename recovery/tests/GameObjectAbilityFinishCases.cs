using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static readonly string[] AbilityFinishTypes={"769868147","1442440941","1813100279","2784675807"};
    static readonly int[] AbilityFinishSizes={256,260,264,264};
    static readonly uint[][] FinishKeys={new uint[]{1996389071u,559101512u,22217949u,185361499u},new uint[]{2683200202u,1872955543u,3097292883u},new uint[]{3800915515u,2534493354u,1992436293u,1328103125u,2384108347u,561127447u,3963816762u,3468811896u,1454433568u,718672663u,2660289546u,3202187060u},new uint[]{2997621972u,2874932308u,4219948537u,1913325433u}};
    static readonly int[][] FinishOffsets={new[]{240,244,248,252},new[]{240,244,256},new[]{8,12,16,24,28,32,36,64,260,124,132,136},new[]{260,240,244,248}};
    static string AbilityFinishXML(int group,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AbilityFinishTypes[group]+"' id='AF' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static void FinishTokens(byte[] b,int g,int at,bool big) {
        int t=MotionTable(b,big);AIUpdateSentinel(b,t,g==2?16:g==3?6:5);
        if(Word(b,t,big)!=1 || Word(b,t+4,big)!=(g==2?4043318533u:2891550594u))throw new Exception("Independent finish exact parent Token");
        for(int j=0;j<FinishKeys[g].Length;++j)if(Word(b,t+12*(j+1),big)!=FinishKeys[g][j] || Word(b,t+12*(j+1)+8,big)!=at+FinishOffsets[g][j])throw new Exception("Independent finish complete scalar Token offsets");
        if(g==1 || g==3) {
            int offset=g==1?248:252,stride=g==1?12:8,count=(int)Word(b,at+offset,big),rows=(int)Word(b,at+offset+4,big),lt=(int)Word(b,Token(b,t,g==1?2950708851u:1413499044u,big)+8,big);
            if(Word(b,lt,big)!=1820487795u || Word(b,lt+4,big)!=1564774360u || Word(b,lt+8,big)!=at+offset)throw new Exception("Independent finish structured list Token header");
            for(int i=0;i<count;++i) {
                int row=rows+stride*i,it=(int)Word(b,lt+20+12*i,big);AIUpdateSentinel(b,it,2);
                if(Word(b,lt+12+12*i,big)!=2974916843u || Word(b,lt+16+12*i,big)!=0)throw new Exception("Independent finish per-item Token");
                if(g==3) {if(Word(b,it,big)!=3777418260u || Word(b,it+8,big)!=row || Word(b,it+12,big)!=846703367u || Word(b,it+20,big)!=row+4)throw new Exception("Independent TerrainData8 Token");}
                else {
                    int fp=(int)Word(b,row,big),ft=(int)Word(b,it+8,big),wc=(int)Word(b,row+4,big),wp=(int)Word(b,row+8,big),wt=(int)Word(b,it+20,big);
                    if(Word(b,it,big)!=376819750u || Word(b,it+12,big)!=3053726141u || (fp==0)!=(ft==0) || Word(b,wt+8,big)!=row+4)throw new Exception("Independent ReplaceObject12 nullable filter/weak list Token");
                    for(int k=0;k<wc;++k)if(Word(b,wt+12+12*k,big)!=2974916843u || Word(b,wt+16+12*k,big)!=0 || Word(b,wt+20+12*k,big)!=wp+4*k)throw new Exception("Independent replacement weak ID value Token");
                }
            }
        }
        if(g==2) {
            if(Word(b,Token(b,t,4208789467u,big)+8,big)!=Word(b,at+140,big))throw new Exception("Independent shield vector direct value Token");
            int ft=(int)Word(b,Token(b,t,3832456114u,big)+8,big);if(Word(b,ft+8,big)!=at+144)throw new Exception("Independent shield inline filter Token");
            if((Word(b,at+256,big)==0)!=(Word(b,Token(b,t,632492410u,big)+8,big)==0))throw new Exception("Independent shield nullable filter Token");
        }
    }
    static void AbilityFinishFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;byte[] parent=Check(original,p,"",SpecialAbilityXML(0)).InstanceData;int pa=LifecycleAt(parent,big);
        for(int g=0;g<4;++g) {
            var def=Check(original,p,"",AbilityFinishXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big);
            if(b.Length!=at+AbilityFinishSizes[g] || Word(b,at,big)!=uint.Parse(AbilityFinishTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent finish native size/type/public imports");
            if(g!=2)for(int i=8;i<240;++i)if(b[at+i]!=parent[pa+i])throw new Exception("Independent finish complete parent240");
            for(int i=g==2?8:240;i<AbilityFinishSizes[g];++i){uint word=0;int pos=i-i%4;if(g==0 && pos==248)word=BitConverter.ToUInt32(BitConverter.GetBytes(0.025f),0);if(g==2){if(pos==8)word=BitConverter.ToUInt32(BitConverter.GetBytes(40f),0);if(pos==12 || pos==28)word=BitConverter.ToUInt32(BitConverter.GetBytes(10f),0);if(pos==24 || pos==132)word=BitConverter.ToUInt32(BitConverter.GetBytes(1f),0);}byte value=(byte)(word>>(8*(big?3-i%4:i%4)));if(b[at+i]!=value)throw new Exception("Independent finish complete defaults/padding");}
            FinishTokens(b,g,at,big);
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+AbilityFinishTypes[g]+"' id='AF'"));
            foreach(Field f in AbilityFinishFields[g])foreach(string v in FinishSamples(f))Check(original,p,"",AbilityFinishXML(g,f.Name+"='"+Escape(v)+"'"));
            if(g!=2) {
                foreach(Field f in SpecialAbilityFields)foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityFinishXML(g,f.Name+"='"+Escape(v)+"'"));
                foreach(string f in new[]{"AnimState","Frames","Trigger"})foreach(string v in f=="AnimState"?SpecialPowerSingleModelNames:AIUpdateSamples(new Field(f,"Unsigned")))Check(original,p,"",AbilityFinishXML(g,"","<CustomAnimAndDuration "+f+"='"+Escape(v)+"'/>"));
            } else {
                foreach(string bad in new[]{"SpecialObjectAttachToBone='x'","SpecialPowerTemplate='P'","DisabledTypesToProcess='ALL'"})Reject(p,"",AbilityFinishXML(g,bad));Reject(p,"",AbilityFinishXML(g,"","<CustomAnimAndDuration/>"));
            }
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AbilityFinishXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<SkirmishAiInfo/>","<ToggleState/>"})Reject(p,"",AbilityFinishXML(g,"",bad));
            foreach(int count in new[]{0,1,2,27,28,29,65}){string nodes="";for(int i=0;i<count;++i)nodes+="<ObjectSMCHelper TypeId='"+AbilityFinishTypes[g]+"' id='AF"+i+"'/>";Check(original,p,"Browser='a bb'","<Behaviors>"+nodes+"<ObjectSMCHelper TypeId='2891550594' id='Parent'/></Behaviors>");}
        }
        foreach(int g in new[]{1,3}) {
            string name=g==1?"ReplaceObjectNugget":"TerrainData";int offset=g==1?248:252,stride=g==1?12:8;
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string rows="";for(int i=0;i<count;++i)rows+="<"+name+"/>";byte[] b=Check(original,p,"",AbilityFinishXML(g,"",rows)).InstanceData;int at=LifecycleAt(b,big),rp=(int)Word(b,at+offset+4,big);
                if(Word(b,at+offset,big)!=count || rp!=(count==0?0:at+AbilityFinishSizes[g]) || b.Length!=at+AbilityFinishSizes[g]+stride*count)throw new Exception("Independent finish structured list stride/count");for(int i=0;i<stride*count;++i)if(b[rp+i]!=0)throw new Exception("Independent finish list zero defaults");FinishTokens(b,g,at,big);
            }
            foreach(string bad in new[]{"Unknown='1'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AbilityFinishXML(g,"","<"+name+" "+bad+"/>"));foreach(string bad in new[]{"text","<Unknown/>"})Reject(p,"",AbilityFinishXML(g,"","<"+name+">"+bad+"</"+name+">"));
        }
        foreach(string terrain in FinishTerrainSamples)foreach(string id in new[]{"","A","MiXeD","中文","Name:Test","G\\23"})Check(original,p,"",AbilityFinishXML(3,"","<TerrainData TerrainClass='"+Escape(terrain)+"' ObjectTemplate='"+Escape(id)+"'/>"));
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string ids="";for(int i=0;i<count;++i)ids+="<ReplacementObject>MiXeD</ReplacementObject>";
            string filter="<TargetObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></TargetObjectFilter>",row="<ReplaceObjectNugget>"+filter+ids+"</ReplaceObjectNugget>",anim="<CustomAnimAndDuration Frames='17'/>",attrs="SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'";
            var x=Check(original,p,"",AbilityFinishXML(1,attrs,anim+row+row));byte[] b=x.InstanceData;int at=LifecycleAt(b,big),cursor=at+260+24;
            for(int i=0;i<2;++i){int r=at+260+12*i;if(Word(b,r,big)!=cursor)throw new Exception("Independent replacement filter allocation");cursor+=112; // IncludeThing and ExcludeThing weak-ID lists follow the filter.
                cursor+=8;if(Word(b,r+4,big)!=count || Word(b,r+8,big)!=(count==0?0:cursor))throw new Exception("Independent replacement weak list allocation");cursor+=count*4;}
            if(Word(b,at+68,big)!=cursor || Word(b,at+200,big)!=cursor+4 || Word(b,at+232,big)!=cursor+12 || b.Length!=cursor+24)throw new Exception("Independent replacement full list then parent allocation");FinishTokens(b,1,at,big);
            string reversed="<ReplaceObjectNugget>"+ids+filter+"</ReplaceObjectNugget>";var y=Check(original,p,"",AbilityFinishXML(1,attrs,reversed+reversed+anim));Equal(x.InstanceData,y.InstanceData,"Replacement schema allocation order",false);Equal(x.RelocationData,y.RelocationData,"Replacement schema relocation order",false);Equal(x.ImportsData,y.ImportsData,"Replacement schema imports order",false);
            string terrains="";for(int i=0;i<count;++i)terrains+="<TerrainData TerrainClass='Snow' ObjectTemplate='MiXeD'/>";
            b=Check(original,p,"",AbilityFinishXML(3,attrs,anim+terrains)).InstanceData;at=LifecycleAt(b,big);cursor=at+264+count*8;if(Word(b,at+68,big)!=cursor || Word(b,at+200,big)!=cursor+4 || Word(b,at+232,big)!=cursor+12 || b.Length!=cursor+24)throw new Exception("Independent terrain list then parent allocation");for(int i=0;i<count;++i)if(Word(b,at+264+8*i,big)!=10)throw new Exception("Independent terrain PE Snow=10");FinishTokens(b,3,at,big);
        }
        foreach(string bad in new[]{"<ReplaceObjectNugget TargetObjectFilter='x'/>","<ReplaceObjectNugget><TargetObjectFilter/><TargetObjectFilter/></ReplaceObjectNugget>","<ReplaceObjectNugget><ReplacementObject Unknown='1'/></ReplaceObjectNugget>","<ReplaceObjectNugget><ReplacementObject><Unknown/></ReplacementObject></ReplaceObjectNugget>","<ReplaceObjectNugget><ReplacementObject xmlns='foreign'/></ReplaceObjectNugget>"})Reject(p,"",AbilityFinishXML(1,"",bad));
        foreach(string field in new[]{"x","y","z"})foreach(string value in AIUpdateSamples(new Field(field,"Float")))Check(original,p,"",AbilityFinishXML(2,"","<PositionOffset "+field+"='"+Escape(value)+"'/>"));
        foreach(string node in new[]{"ObjectFilter","IgnoreInsideToInsideCheck"}) {
            Check(original,p,"",AbilityFinishXML(2,"","<"+node+"/>"));
            foreach(string value in WeaponSetStatusValues)Check(original,p,"",AbilityFinishXML(2,"","<"+node+" Include='ALL' StatusBitFlags='"+Escape(value)+"'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></"+node+">"));
            foreach(string value in KindNames)Check(original,p,"",AbilityFinishXML(2,"","<"+node+" Include='"+Escape(value)+"'/>"));
            foreach(int count in new[]{0,1,2,27,28,29,65}){string ids="";for(int i=0;i<count;++i)ids+="<IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing>";Check(original,p,"",AbilityFinishXML(2,"","<"+node+">"+ids+"</"+node+">"));}
        }
        {
            string vector="<PositionOffset x='1.25' y='-17' z='23'/>",filter="<ObjectFilter Include='ALL'><IncludeThing>A</IncludeThing></ObjectFilter>",ignore="<IgnoreInsideToInsideCheck><ExcludeThing>B</ExcludeThing></IgnoreInsideToInsideCheck>",attrs="ShieldBoneName='abcd' DamageTypesNotToAbsorb='ALL' Options='ALL' ObjectStatus='ALL' ModelCondition='ALL' InitiallyActive='true'";
            var x=Check(original,p,"",AbilityFinishXML(2,attrs,ignore+filter+vector));byte[] b=x.InstanceData;int at=LifecycleAt(b,big);
            if(Word(b,at+124,big)!=4 || Word(b,at+128,big)!=at+264 || Word(b,at+140,big)!=at+272 || Word(b,at+256,big)!=at+288 || b.Length!=at+404 || Word(b,at+16,big)!=0xffffffffu || Word(b,at+20,big)!=0xffffffffu || Word(b,at+136,big)!=0xffffffffu || b[at+260]!=1)throw new Exception("Independent shield own string/vector/inline-filter/nullable-filter allocation and ALL masks");FinishTokens(b,2,at,big);
            var y=Check(original,p,"",AbilityFinishXML(2,attrs,vector+filter+ignore));Equal(x.InstanceData,y.InstanceData,"Shield schema allocation order",false);Equal(x.RelocationData,y.RelocationData,"Shield schema relocation order",false);
        }
        foreach(string node in new[]{"PositionOffset","ObjectFilter","IgnoreInsideToInsideCheck"})foreach(string bad in new[]{"<"+node+"/><"+node+"/>","<"+node+" Unknown='1'/>","<"+node+" xmlns='foreign'/>","<"+node+" xmlns:f='foreign' f:TypeId='1'/>","<"+node+">text</"+node+">","<"+node+"><Unknown/></"+node+">"})Reject(p,"",AbilityFinishXML(2,"",bad));
        {byte[] b=Check(original,p,"",AbilityFinishXML(2,"DamageTypesNotToAbsorb='UNKNOWN' Options='UNKNOWN'")).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+16,big)!=0 || Word(b,at+20,big)!=64 || Word(b,at+136,big)!=1)throw new Exception("Independent shield damage bit38 and option bit0 fallbacks");}
        {byte[] b=Check(original,p,"",AbilityFinishXML(3,"","<TerrainData TerrainClass='UNKNOWN'/>" )).InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+264,big)!=3)throw new Exception("Independent terrain single unknown3");}
    }
    static readonly Field[][] AbilityFinishFields={
        new[]{new Field("GiveUpgradeEffect","Reference"),new Field("SpawnOutFX","Reference"),new Field("FadeOutSpeed","Float"),new Field("DeliverUpgrade","Boolean")},
        new[]{new Field("ReplaceRadius","Float"),new Field("ReplaceFX","Reference"),new Field("Scatter","Boolean")},
        new[]{new Field("RadiusMax","Float"),new Field("RadiusMin","Float"),new Field("DamageTypesNotToAbsorb","Bits"),new Field("ScanFrequency","Time"),new Field("Duration","Time"),new Field("MaxDamage","Float"),new Field("ObjectStatus","ObjectStatus"),new Field("ModelCondition","ModelCondition"),new Field("InitiallyActive","Boolean"),new Field("ShieldBoneName","String"),new Field("ShieldSizeMultiplier","Float"),new Field("Options","Bits")},
        new[]{new Field("TriggerInstantlyOnCreate","Boolean"),new Field("OpacityTarget","Float"),new Field("DisguiseFX","Reference"),new Field("ScanRadiusForPropDisguise","Float")}
    };
    static readonly string[] FinishTerrainSamples=new[]{"Cliff","Dirt","Grass","Misc","Mud","Road","Rock","Sand","Shrub","Snow","UNSPECIFIED","","UNKNOWN","unknown","ALL","0","-1","Snow Grass"," Snow","Snow "};
    static string[] FinishSamples(Field f) {
        if(f.Name=="DamageTypesNotToAbsorb")return new[]{"ALL","AUTO_CANNON","BLAST","CANNON","CONCUSSIVE","CRUSH","EXPLOSIVE","FALLING","FIRE","FLAK","FLAME","FORCE","GRENADE","GUN","HEALING","IMPACT","KI","LASER","LOGICAL_FIRE","MAGIC","MELEE","MINE","PASSENGER","PENALTY","PIERCE","PLASMA","POISON","PRISM","RADIATION","REFLECTED","ROCKET","SLASH","SNIPER","STORM_SHIELD","TESLA","TIBERIUM","UNDEFINED","UNRESISTABLE","WATER","","ALL","all","-ALL","+ALL","UNKNOWN","unknown","-UNKNOWN -UNKNOWN","ALL -UNKNOWN","UNKNOWN ALL UNKNOWN","+","-","FIRE EXPLOSION","ALL -ALL -ALL","AUTO_CANNON -AUTO_CANNON -AUTO_CANNON","BLAST -BLAST -BLAST","CANNON -CANNON -CANNON","CONCUSSIVE -CONCUSSIVE -CONCUSSIVE","CRUSH -CRUSH -CRUSH","EXPLOSIVE -EXPLOSIVE -EXPLOSIVE","FALLING -FALLING -FALLING","FIRE -FIRE -FIRE","FLAK -FLAK -FLAK","FLAME -FLAME -FLAME","FORCE -FORCE -FORCE","GRENADE -GRENADE -GRENADE","GUN -GUN -GUN","HEALING -HEALING -HEALING","IMPACT -IMPACT -IMPACT","KI -KI -KI","LASER -LASER -LASER","LOGICAL_FIRE -LOGICAL_FIRE -LOGICAL_FIRE","MAGIC -MAGIC -MAGIC","MELEE -MELEE -MELEE","MINE -MINE -MINE","PASSENGER -PASSENGER -PASSENGER","PENALTY -PENALTY -PENALTY","PIERCE -PIERCE -PIERCE","PLASMA -PLASMA -PLASMA","POISON -POISON -POISON","PRISM -PRISM -PRISM","RADIATION -RADIATION -RADIATION","REFLECTED -REFLECTED -REFLECTED","ROCKET -ROCKET -ROCKET","SLASH -SLASH -SLASH","SNIPER -SNIPER -SNIPER","STORM_SHIELD -STORM_SHIELD -STORM_SHIELD","TESLA -TESLA -TESLA","TIBERIUM -TIBERIUM -TIBERIUM","UNDEFINED -UNDEFINED -UNDEFINED","UNRESISTABLE -UNRESISTABLE -UNRESISTABLE","WATER -WATER -WATER"};
        if(f.Name=="Options")return new[]{"ALLOW_ALLIES_PROJECTTILE_GOTHROUGH","","UNKNOWN","unknown","ALL","all","-ALL","+ALL","-UNKNOWN -UNKNOWN","ALL -UNKNOWN","ALLOW_ALLIES_PROJECTTILE_GOTHROUGH -ALLOW_ALLIES_PROJECTTILE_GOTHROUGH -ALLOW_ALLIES_PROJECTTILE_GOTHROUGH","allow_allies_projecttile_gothrough","0","-1"," ALLOW_ALLIES_PROJECTTILE_GOTHROUGH","ALLOW_ALLIES_PROJECTTILE_GOTHROUGH "};
        if(f.Kind=="ObjectStatus")return WeaponSetStatusValues;
        return AIUpdateSamples(f);
    }
}
