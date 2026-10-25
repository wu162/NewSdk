using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string SupportXML(int g,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+SupportTypes[g]+"' id='Support' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static string[] SupportSamples(Field f) {
        if(f.Kind=="ObjectStatus")return WeaponSetStatusValues;
        if(f.Name=="Type")return new[]{"INVALID","IN_RANGE_OBJECT_FILTER","","UNKNOWN","invalid","0","1","-1"," INVALID","INVALID "};
        if(f.Name=="RestrictionType")return UtilitySamples(f);
        if(f.Name=="AntiCategoryMask" || f.Name=="AntiCategory")return SupportCategories;
        if(f.Name=="RequiredConditions")return new[]{"MOUNTED","mounted","ALL","all","UNKNOWN","unknown","","-ALL","+ALL","MOUNTED -MOUNTED -MOUNTED","UNKNOWN -UNKNOWN -UNKNOWN"};
        if(f.Name=="DisabledTypesToProcess")return SpecialPowerEnumSamples[1];
        if(f.Name=="Index")return SupportFXIndices;
        if(f.Name=="Direction")return SupportFXDirections;
        return AIUpdateSamples(f);
    }
    static void SupportTokens(byte[] b,int g,int at,bool big) {
        int t=MotionTable(b,big);AIUpdateSentinel(b,t,SupportKeys[g].Length);
        for(int j=0;j<SupportKeys[g].Length;++j) {
            int token=t+12*j,off=SupportOffsets[g][j];if(Word(b,token,big)!=SupportKeys[g][j] || Word(b,token+4,big)!=SupportFieldTypes[g][j])throw new Exception("Independent support original Token keys/types");
            if(j==0)continue;
            int mode=SupportModes[g][j];
            if(mode==0) {if(Word(b,token+8,big)!=at+off)throw new Exception("Independent support inline scalar Token");}
            if(mode==4 || mode==7) {
                int vp=(int)Word(b,at+off,big),ct=(int)Word(b,token+8,big);if((vp==0)!=(ct==0) || (ct!=0 && Word(b,ct+8,big)!=vp))throw new Exception("Independent support nullable filter/validation Token");
                if(mode==4 && ct!=0) {AIUpdateSentinel(b,ct,2);if(Word(b,ct+20,big)!=vp+28)throw new Exception("Independent support nullable56 masks");}
            }
            if(mode==1 || mode==3) {
                int count=(int)Word(b,at+off,big),rows=(int)Word(b,at+off+4,big),lt=(int)Word(b,token+8,big);if(Word(b,lt,big)!=1820487795u || Word(b,lt+4,big)!=1564774360u || Word(b,lt+8,big)!=at+off)throw new Exception("Independent support typed-list Token header");
                for(int i=0;i<count;++i) {
                    int item=lt+12+12*i;if(Word(b,item,big)!=2974916843u || Word(b,item+4,big)!=0)throw new Exception("Independent support list Token item");
                    if(mode==3) {if(Word(b,item+8,big)!=rows+4*i)throw new Exception("Independent support strong-reference/float value Token");}
                    else {
                        int row=rows+(g==5?12:24)*i,rt=(int)Word(b,item+8,big);AIUpdateSentinel(b,rt,g==5?3:7);
                        if(g==5) {if(Word(b,rt,big)!=SupportQueryKeys[0] || Word(b,rt+12,big)!=SupportQueryKeys[1] || Word(b,rt+24,big)!=3832456114u || Word(b,rt+8,big)!=row || Word(b,rt+20,big)!=row+4)throw new Exception("Independent Query12 signed fields/Token");int fp=(int)Word(b,row+8,big),ft=(int)Word(b,rt+32,big);if((fp==0)!=(ft==0) || (ft!=0 && Word(b,ft+8,big)!=fp))throw new Exception("Independent query nullable Filter112 Token");}
                        else for(int k=0;k<7;++k)if(Word(b,rt+12*k,big)!=SupportEventKeys[k] || Word(b,rt+12*k+8,big)!=row+SupportEventOffsets[k])throw new Exception("Independent FXEvent24 complete Token offsets");
                    }
                }
            }
        }
    }
    static void SupportFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        for(int g=0;g<8;++g) {
            var def=Check(original,p,"",SupportXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big);
            if(b.Length!=at+SupportSizes[g] || Word(b,at,big)!=uint.Parse(SupportTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent support complete size/type/imports");
            for(int i=8;i<SupportSizes[g];++i) {
                int pos=i-i%4;uint word=0;
                if(g==1)word=pos==28?0x3f800000u:pos==32?0x40490fdbu:0u;
                if(g==3 && pos==24)word=20;if(g==5 && pos==8)word=0x3f800000u;
                if(g==6)word=pos==16?0x40000000u:pos==24?5u:0u;
                byte value=(byte)(word>>(8*(big?3-i%4:i%4)));
                if(((g==0 || g==4) && i==32) || (g==2 && i==24) || (g==4 && i==96))value=1;
                if(b[at+i]!=value)throw new Exception("Independent support complete defaults/padding group "+g+" byte "+i);
            }
            SupportTokens(b,g,at,big);
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+SupportTypes[g]+"' id='S'"));
            foreach(Field f in SupportFields[g])foreach(string value in SupportSamples(f))Check(original,p,"",SupportXML(g,f.Name+"='"+Escape(value)+"'"));
            if(g==0 || g==4) {
                foreach(string name in new[]{"RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode","WaypointModeTerminal"})foreach(string value in AIUpdateSamples(new Field(name,"Boolean")))Check(original,p,"",SupportXML(g,name+"='"+Escape(value)+"'"));
                foreach(string name in new[]{"AnimState","Frames","Trigger"})foreach(string value in name=="AnimState"?SpecialPowerSingleModelNames:AIUpdateSamples(new Field(name,"Unsigned")))Check(original,p,"",SupportXML(g,"","<CustomAnimAndDuration "+name+"='"+Escape(value)+"'/>"));
            }
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","SpecialPowerTemplate='P'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",SupportXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<ToggleState/>"})Reject(p,"",SupportXML(g,"",bad));
            foreach(string node in SupportFilterNames[g]) {
                Check(original,p,"",SupportXML(g,"","<"+node+"/>"));
                foreach(string value in WeaponSetStatusValues)Check(original,p,"",SupportXML(g,"","<"+node+" Include='ALL' StatusBitFlags='"+Escape(value)+"'><IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing></"+node+">"));
                foreach(string value in KindNames)Check(original,p,"",SupportXML(g,"","<"+node+" Include='"+Escape(value)+"'/>"));
                foreach(int count in new[]{0,1,2,27,28,29,65}) {string ids="";for(int i=0;i<count;++i)ids+="<IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing>";Check(original,p,"",SupportXML(g,"","<"+node+">"+ids+"</"+node+">"));}
                foreach(string bad in new[]{"<"+node+"/><"+node+"/>","<"+node+" Unknown='1'/>","<"+node+" xmlns='foreign'/>","<"+node+" xmlns:f='foreign' f:TypeId='1'/>","<"+node+">text</"+node+">","<"+node+"><Unknown/></"+node+">"})Reject(p,"",SupportXML(g,"",bad));
            }
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string nodes="";for(int i=0;i<count;++i)nodes+="<ObjectSMCHelper TypeId='"+SupportTypes[g]+"' id='S"+i+"'/>";Check(original,p,"Browser='a bb'","<Behaviors>"+nodes+"<ObjectSMCHelper TypeId='3925677257' id='Parent'/></Behaviors>");
            }
        }
        foreach(int g in new[]{2,3,6}) {
            string name=g==2?"DependentObjectRadius":g==3?"Modifier":"AddEmotion";int offset=g==2?16:g==3?48:44;
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string children="";for(int i=0;i<count;++i)children+="<"+name+">"+(g==2?"-17":"Ref\\23")+"</"+name+">";
                byte[] b=Check(original,p,"",SupportXML(g,"",children)).InstanceData;int at=LifecycleAt(b,big);
                if(Word(b,at+offset,big)!=count || Word(b,at+offset+4,big)!=(count==0?0:at+SupportSizes[g]) || b.Length!=at+SupportSizes[g]+4*count)throw new Exception("Independent support repeated float/strong-list count/allocation");for(int i=0;i<count;++i)if(Word(b,at+SupportSizes[g]+4*i,big)!=(g==2?0xc1880000u:24u))throw new Exception("Independent support float/strong reference values");SupportTokens(b,g,at,big);
            }
            foreach(string value in AIUpdateSamples(new Field(name,g==2?"Float":"Reference")))Check(original,p,"",SupportXML(g,"","<"+name+">"+Escape(value)+"</"+name+">"));
            Reject(p,"",SupportXML(g,name+"='X'"));foreach(string bad in new[]{"<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+"><Unknown/></"+name+">"})Reject(p,"",SupportXML(g,"",bad));
        }
        foreach(int g in new[]{5,7}) {
            string node=g==5?"Queries":"Event";int offset=g==5?56:52,stride=g==5?12:24;
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string children="";for(int i=0;i<count;++i)children+="<"+node+"/>";byte[] b=Check(original,p,"",SupportXML(g,"",children)).InstanceData;int at=LifecycleAt(b,big),rows=at+SupportSizes[g];
                if(Word(b,at+offset,big)!=count || Word(b,at+offset+4,big)!=(count==0?0:rows) || b.Length!=rows+stride*count)throw new Exception("Independent Query12/Event24 list layout");
                for(int i=0;i<count;++i) {int row=rows+stride*i;if(g==5) {if(Word(b,row,big)!=6 || Word(b,row+4,big)!=0xffffffffu || Word(b,row+8,big)!=0)throw new Exception("Independent Query12 defaults");}else {if(Word(b,row,big)!=0 || Word(b,row+4,big)!=0 || Word(b,row+8,big)!=0 || Word(b,row+12,big)!=0 || Word(b,row+16,big)!=4 || b[row+20]!=1 || b[row+21]!=0 || b[row+22]!=0 || b[row+23]!=0)throw new Exception("Independent Event24 defaults/padding");}}
                SupportTokens(b,g,at,big);
            }
            foreach(Field f in g==5?new[]{new Field("EMax","Signed"),new Field("Value","Signed")}:SupportEventFields)foreach(string value in SupportSamples(f))Check(original,p,"",SupportXML(g,"","<"+node+" "+f.Name+"='"+Escape(value)+"'/>"));
            foreach(string bad in new[]{"<"+node+" Unknown='1'/>","<"+node+" xmlns='foreign'/>","<"+node+" xmlns:f='foreign' f:TypeId='1'/>","<"+node+">text</"+node+">","<"+node+"><Unknown/></"+node+">"})Reject(p,"",SupportXML(g,"",bad));
        }
        foreach(string value in WeaponSetStatusValues)Check(original,p,"",SupportXML(5,"","<Queries><ObjectFilter Include='ALL' StatusBitFlags='"+Escape(value)+"'/></Queries>"));foreach(string value in KindNames)Check(original,p,"",SupportXML(5,"","<Queries><ObjectFilter Include='"+Escape(value)+"'/></Queries>"));
        foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string ids="";for(int i=0;i<count;++i)ids+="<IncludeThing>A</IncludeThing><ExcludeThing>B</ExcludeThing>";string filter="<ObjectFilter Include='ALL' StatusBitFlags='ALL'>"+ids+"</ObjectFilter>",query="<Queries EMax='-17' Value='23'>"+filter+"</Queries>";
            byte[] b=Check(original,p,"",SupportXML(5,"",query+query)).InstanceData;int at=LifecycleAt(b,big),cursor=at+68+24;
            for(int i=0;i<2;++i) {int row=at+68+12*i;if(Word(b,row,big)!=0xffffffefu || Word(b,row+4,big)!=23 || Word(b,row+8,big)!=cursor)throw new Exception("Independent nested Query allocation");cursor+=112+28+8*count;}
            if(b.Length!=cursor)throw new Exception("Independent nested Query complete filter/list allocation");SupportTokens(b,5,at,big);
        }
        foreach(string bad in new[]{"<Queries><ObjectFilter/><ObjectFilter/></Queries>","<Queries><ObjectFilter Unknown='1'/></Queries>","<Queries><ObjectFilter xmlns='foreign'/></Queries>","<Queries><ObjectFilter>text</ObjectFilter></Queries>"})Reject(p,"",SupportXML(5,"",bad));
        foreach(string name in new[]{"RequiredStatus","ForbiddenStatus"})foreach(string value in WeaponSetStatusValues)Check(original,p,"",SupportXML(3,"","<ObjectStatusValidation "+name+"='"+Escape(value)+"'/>"));
        foreach(string bad in new[]{"<ObjectStatusValidation/><ObjectStatusValidation/>","<ObjectStatusValidation Unknown='1'/>","<ObjectStatusValidation xmlns='foreign'/>","<ObjectStatusValidation>text</ObjectStatusValidation>"})Reject(p,"",SupportXML(3,"",bad));
        foreach(int g in new[]{0,4})foreach(int count in new[]{0,1,2,27,28,29,65}) {
            string ids="";for(int i=0;i<count;++i)ids+="<TriggeredBy>A</TriggeredBy><ConflictsWith>B</ConflictsWith>";string anim="<CustomAnimAndDuration Frames='17'/>",filter=g==4?"<ObjectFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing></ObjectFilter>":"";
            var x=Check(original,p,"",SupportXML(g,"",ids+anim+filter));byte[] b=x.InstanceData;int at=LifecycleAt(b,big),cursor=at+SupportSizes[g];
            if(g==4){if(Word(b,at+92,big)!=cursor)throw new Exception("Independent aura own filter before Upgrade");cursor+=112+28+4;}
            if(Word(b,at+8,big)!=cursor)throw new Exception("Independent complete Upgrade animation after own data");cursor+=12;
            if(Word(b,at+12,big)!=count || Word(b,at+16,big)!=(count==0?0:cursor))throw new Exception("Independent Upgrade TriggeredBy allocation");cursor+=4*count;if(Word(b,at+20,big)!=count || Word(b,at+24,big)!=(count==0?0:cursor) || b.Length!=cursor+4*count)throw new Exception("Independent Upgrade ConflictsWith allocation");SupportTokens(b,g,at,big);var y=Check(original,p,"",SupportXML(g,"",filter+anim+ids));Equal(x.InstanceData,y.InstanceData,"Support Upgrade schema order",false);Equal(x.RelocationData,y.RelocationData,"Support Upgrade relocation order",false);
        }
        // Distinguish all four emotion filters and retain their schema allocation order before strong Emotion references.
        {string[] names={"TauntAndPointExcluded","AfraidOf","AlwaysAfraidOf","PointAt"};string forward="",reverse="";for(int i=0;i<4;++i){string f="<"+names[i]+" Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing></"+names[i]+">";forward+=f;reverse=f+reverse;}string refs="<AddEmotion>E\\23</AddEmotion>";var x=Check(original,p,"",SupportXML(6,"",refs+reverse));byte[] b=x.InstanceData;int at=LifecycleAt(b,big),cursor=at+56;for(int i=0;i<4;++i){if(Word(b,at+28+4*i,big)!=cursor)throw new Exception("Independent emotion filter order");cursor+=144;}if(Word(b,at+48,big)!=cursor || b.Length!=cursor+4 || Word(b,cursor,big)!=24)throw new Exception("Independent emotion strong list last");SupportTokens(b,6,at,big);var y=Check(original,p,"",SupportXML(6,"",forward+refs));Equal(x.InstanceData,y.InstanceData,"Emotion schema order",false);Equal(x.RelocationData,y.RelocationData,"Emotion relocation order",false);}
        {string f="<AllowFilter Include='ALL' StatusBitFlags='ALL'><IncludeThing>A</IncludeThing></AllowFilter>",c="<ContainerFilter Include='ALL' StatusBitFlags='ALL'><ExcludeThing>B</ExcludeThing></ContainerFilter>",v="<ObjectStatusValidation RequiredStatus='ALL'/>",r="<Modifier>M\\23</Modifier>";var x=Check(original,p,"",SupportXML(3,"",v+c+f+r));byte[] b=x.InstanceData;int at=LifecycleAt(b,big);if(Word(b,at+52,big)!=at+76 || Word(b,at+56,big)!=at+80 || Word(b,at+60,big)!=at+224 || Word(b,at+64,big)!=at+368 || b.Length!=at+424)throw new Exception("Independent passive strong list/filter/filter/validation order");SupportTokens(b,3,at,big);var y=Check(original,p,"",SupportXML(3,"",r+f+c+v));Equal(x.InstanceData,y.InstanceData,"Passive schema order",false);Equal(x.RelocationData,y.RelocationData,"Passive relocation order",false);}
        // Original full DieMux contract is retained even when Event24 lists precede it.
        foreach(Field f in new[]{new Field("VeterancyLevels","StringList"),new Field("ExemptStatus","ObjectStatus"),new Field("RequiredStatus","ObjectStatus"),new Field("DamageAmountRequired","Float"),new Field("MinKillerAngle","Angle"),new Field("MaxKillerAngle","Angle"),new Field("DeathTypes","DeathBits"),new Field("DeathTypesForbidden","DeathBits")})foreach(string value in f.Kind=="ObjectStatus"?WeaponSetStatusValues:f.Kind=="DeathBits"?AttachDeathBitSamples:AIUpdateSamples(f))Check(original,p,"",SupportXML(7,"","<DieMuxData "+f.Name+"='"+Escape(value)+"'/><Event/>"));
        foreach(int g in new[]{0,4})foreach(string bad in new[]{"<CustomAnimAndDuration/><CustomAnimAndDuration/>","<CustomAnimAndDuration Unknown='1'/>","<TriggeredBy Unknown='1'/>","<ConflictsWith><Unknown/></ConflictsWith>"})Reject(p,"",SupportXML(g,"",bad));
        foreach(string bad in new[]{"<DieMuxData/><DieMuxData/>","<DieMuxData Unknown='1'/>","<DieMuxData xmlns='foreign'/>","<DieMuxData>text</DieMuxData>"})Reject(p,"",SupportXML(7,"",bad));
    }
    static readonly string[] SupportTypes={"664395762","793065128","2503769947","149050769","1578938331","1285783358","2842040686","2960202212"};
    static readonly int[] SupportSizes={40,40,28,76,104,68,56,60};
    static readonly Field[][] SupportFields={new[]{new Field("KillLocomotorUpgrade","Boolean")},new[]{new Field("Type","Enum"),new Field("LocomotorSpeedToSet","Float"),new Field("LocomotorDeltaPerUpdate","Float"),new Field("OtherScaredScanRangeMultiplier","Float"),new Field("ObjectFilterRange","Float"),new Field("UpdatePeriod","Time"),new Field("VisionArc","Angle")},new[]{new Field("RestrictionType","Enum"),new Field("DependentObject","AssetId"),new Field("ConsiderSpecialPowerRadius","Boolean")},new[]{new Field("EffectRadius","Float"),new Field("MaxBeneficiaries","Unsigned"),new Field("HealingPercentPerSecond","Percent"),new Field("HealingPointsPerSecond","Float"),new Field("SurveyDelayFrames","Unsigned"),new Field("UpgradeRequired","Reference"),new Field("NonStackable","Boolean"),new Field("AntiCategoryMask","Bits"),new Field("AntiFX","Reference"),new Field("HealFX","Reference"),new Field("AffectAttached","Boolean"),new Field("AffectWoundedOnly","Boolean"),new Field("AffectUnderAttack","Boolean"),new Field("AffectContained","Boolean"),new Field("DisabledTypesToProcess","Bits")},new[]{new Field("AttributeModifierName","Reference"),new Field("RefreshDelay","Time"),new Field("Range","Float"),new Field("AllowPowerWhenAttacking","Boolean"),new Field("InitiallyActive","Boolean"),new Field("RequiredConditions","Bits"),new Field("AntiCategory","Bits"),new Field("AntiFX","Reference"),new Field("RunWhileDead","Boolean"),new Field("AllowSelf","Boolean"),new Field("AffectContainedOnly","Boolean"),new Field("AffectHordeMembersOnly","Boolean"),new Field("AffectAttacheeOnly","Boolean"),new Field("MaxActiveRank","Signed"),new Field("RequiredObjectStatusFlags","ObjectStatus"),new Field("ShowDebugCircle","Boolean")},new[]{new Field("MaxScanRange","Float"),new Field("MinScanRange","Float"),new Field("Radius","Float"),new Field("IdleSeconds","Time"),new Field("SpecialAbilityName","Reference"),new Field("ForbiddenStatus","ObjectStatus"),new Field("StartsActive","Boolean"),new Field("UseStartPosition","Boolean"),new Field("AdjustAttackPos","Boolean"),new Field("AllowSelf","Boolean")},new[]{new Field("TauntAndPointDistance","Float"),new Field("HeroScanDistance","Float"),new Field("TauntAndPointUpdateDelay","Time"),new Field("FearScanDistance","Float"),new Field("IgnoreVeterancy","Boolean"),new Field("ImmuneToFearLevel","Signed")},new Field[0]};
    static readonly Field[] SupportEventFields=new[]{new Field("Index","Enum"),new Field("FX","Reference"),new Field("MinThreshold","Float"),new Field("MaxThreshold","Float"),new Field("OrientToObject","Boolean"),new Field("ForceUseDoFXObj","Boolean"),new Field("Direction","Enum")};
    static readonly uint[][] SupportKeys={new uint[]{1u,2908758514u},new uint[]{1u,2219907648u,145327958u,3217246955u,762780181u,3098758058u,3829086399u,1144468699u,3832456114u},new uint[]{1u,384904913u,3318598274u,2723359786u,2587419965u},new uint[]{1u,386882139u,624406373u,139575931u,3741384635u,3822215771u,2084565028u,3807635175u,2546513103u,4240413615u,1937770881u,3270774743u,897405705u,2281253216u,437681369u,943614829u,2698420400u,1290071661u,1885606616u,3933465695u},new uint[]{1u,226106323u,3742013746u,3426209446u,1950954445u,1454433568u,1294012426u,711753708u,4240413615u,1662924473u,3435022180u,2326753257u,4204774495u,1047290712u,1262013020u,2141005593u,2050255657u,3832456114u},new uint[]{1u,1988733171u,1906490224u,1328662213u,3541024273u,3617336977u,824345085u,3455740364u,1815375446u,3068870907u,3435022180u,2314718166u},new uint[]{1u,4285554915u,169727143u,1878100543u,569813769u,2839391183u,3285241533u,2790838710u,1356802037u,607908341u,2035695822u,2736334765u},new uint[]{1u,4208909635u}};
    static readonly uint[][] SupportFieldTypes={new uint[]{2925571736u,2011428087u},new uint[]{4043318533u,2361384232u,1683514611u,1683514611u,1683514611u,1683514611u,2575722984u,1310124621u,3832456114u},new uint[]{2769121946u,729580760u,344905565u,2011428087u,1683514611u},new uint[]{4043318533u,1683514611u,1982953380u,1683514611u,1683514611u,1982953380u,1253711657u,2011428087u,1173445096u,219975299u,219975299u,2011428087u,2011428087u,2011428087u,2011428087u,426078409u,165531635u,3832456114u,3832456114u,1049596812u},new uint[]{2925571736u,165531635u,2575722984u,1683514611u,2011428087u,2011428087u,1031726474u,1173445096u,219975299u,2011428087u,2011428087u,2011428087u,2011428087u,2011428087u,602296504u,2814214430u,2011428087u,3832456114u},new uint[]{4043318533u,1683514611u,1683514611u,1683514611u,2575722984u,2934454408u,2814214430u,2011428087u,2011428087u,2011428087u,2011428087u,405260724u},new uint[]{4043318533u,1683514611u,1683514611u,2575722984u,1683514611u,2011428087u,602296504u,3832456114u,3832456114u,3832456114u,3832456114u,2633945524u},new uint[]{2655988594u,1706983441u}};
    static readonly int[][] SupportOffsets={new[]{0,36},new[]{0,8,12,16,20,24,28,32,36},new[]{0,8,12,24,16},new[]{0,8,12,16,20,24,28,68,32,36,40,69,70,71,72,44,48,56,60,64},new[]{0,36,40,44,96,97,48,52,56,98,99,100,101,102,60,64,103,92},new[]{0,8,12,16,20,24,28,64,65,66,67,56},new[]{0,8,12,16,20,52,24,28,32,36,40,44},new[]{0,52}};
    static readonly int[][] SupportModes={new[]{0,0},new[]{0,0,0,0,0,0,0,0,7},new[]{0,0,0,0,3},new[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,7,7,4},new[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7},new[]{0,0,0,0,0,0,0,0,0,0,0,1},new[]{0,0,0,0,0,0,0,7,7,7,7,3},new[]{0,1}};
    static readonly string[][] SupportFilterNames={new string[0],new[]{"ObjectFilter"},new string[0],new[]{"AllowFilter","ContainerFilter"},new[]{"ObjectFilter"},new string[0],new[]{"TauntAndPointExcluded","AfraidOf","AlwaysAfraidOf","PointAt"},new string[0]};
    static readonly uint[] SupportQueryKeys={1341120459u,3893002744u,3832456114u};
    static readonly uint[] SupportEventKeys={1034518573u,898024161u,1003967157u,1967413098u,3497346223u,3779533864u,908073516u};
    static readonly int[] SupportEventOffsets={0,4,8,12,20,21,16};
    static readonly string[] SupportCategories={"BUFF","DEBUFF","FORMATION","INNATE_ARMOR","INNATE_AUTOHEAL","INNATE_DAMAGEMULT","INNATE_HEALTH","INNATE_VISION","LEADERSHIP","LEVEL","NONE","SPELL","STRUCTURE","STUN","WEAPON","","UNKNOWN","unknown","ALL","all","-1","0","1"," onDeath","onDeath ","onDeath onCreate","+","-","ALL -ALL -ALL","UNKNOWN -UNKNOWN -UNKNOWN","ALL -UNKNOWN","+ALL","-ALL","BUFF -BUFF -BUFF","DEBUFF -DEBUFF -DEBUFF","FORMATION -FORMATION -FORMATION","INNATE_ARMOR -INNATE_ARMOR -INNATE_ARMOR","INNATE_AUTOHEAL -INNATE_AUTOHEAL -INNATE_AUTOHEAL","INNATE_DAMAGEMULT -INNATE_DAMAGEMULT -INNATE_DAMAGEMULT","INNATE_HEALTH -INNATE_HEALTH -INNATE_HEALTH","INNATE_VISION -INNATE_VISION -INNATE_VISION","LEADERSHIP -LEADERSHIP -LEADERSHIP","LEVEL -LEVEL -LEVEL","NONE -NONE -NONE","SPELL -SPELL -SPELL","STRUCTURE -STRUCTURE -STRUCTURE","STUN -STUN -STUN","WEAPON -WEAPON -WEAPON"};
    static readonly string[] SupportFXIndices={"onBecomingTemporaryOwner","onBecomingTemporarySlave","onCreate","onDamaged","onDeath","onEndingTemporaryOwner","onEndingTemporarySlave","onShieldSphereCollision","onSubmerging","onSurfacing","onTerrainCollision","onTransitionToDamaged","onTransitionToReallyDamaged","onTransitionToRubble","","UNKNOWN","unknown","ALL","all","-1","0","1"," onDeath","onDeath ","onDeath onCreate","+","-"};
    static readonly string[] SupportFXDirections={"Default","Front","Rear","Side","Top","","UNKNOWN","unknown","ALL","all","-1","0","1"," onDeath","onDeath ","onDeath onCreate","+","-"};
}
