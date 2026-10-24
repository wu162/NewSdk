using System;
using BinaryAssetBuilder.Core;
partial class GameObjectOracle {
    static string[] AbilityRoutingTypes={"3442719982","3660533477"};
    static string[] AbilityRoutingNodes={"SpecialPowerModules","IntersectionFlagsFilter"};
    static string AbilityRoutingXML(int g,string attrs="",string children="") {return "<Behaviors><ObjectSMCHelper TypeId='"+AbilityRoutingTypes[g]+"' id='AR' "+attrs+">"+children+"</ObjectSMCHelper></Behaviors>";}
    static void AbilityRoutingFamily(IAssetBuilderPlugin original,TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;byte[] parent=Check(original,p,"",SpecialAbilityXML(0)).InstanceData;int parentAt=LifecycleAt(parent,big);
        for(int g=0;g<2;++g) {
            int stride=g==0?12:128;string name=AbilityRoutingNodes[g];uint key=g==0?2206287502u:4249583194u;
            var def=Check(original,p,"",AbilityRoutingXML(g));byte[] b=def.InstanceData;int at=LifecycleAt(b,big),t=MotionTable(b,big);
            if(b.Length!=at+248 || Word(b,at,big)!=uint.Parse(AbilityRoutingTypes[g]) || def.ImportsData.Length!=0)throw new Exception("Independent routing native248/type/public imports");
            for(int i=8;i<240;++i)if(b[at+i]!=parent[parentAt+i])throw new Exception("Independent routing complete parent240 defaults");for(int i=240;i<248;++i)if(b[at+i]!=0)throw new Exception("Independent routing empty list");
            AIUpdateSentinel(b,t,2);if(Word(b,t,big)!=1 || Word(b,t+4,big)!=2891550594u)throw new Exception("Independent routing SpecialAbility parent");int pt=(int)Word(b,t+8,big);AIUpdateSentinel(b,pt,47);if(Word(b,pt+4,big)!=4043318533u)throw new Exception("Independent routing Update grandparent");
            int lt=(int)Word(b,Token(b,t,key,big)+8,big);if(Word(b,lt,big)!=1820487795u || Word(b,lt+4,big)!=1564774360u || Word(b,lt+8,big)!=at+240)throw new Exception("Independent empty structured list Token header");
            foreach(var c in ModuleContainers)Check(original,p,"",ModuleXML(c[0],c[1],"TypeId='"+AbilityRoutingTypes[g]+"' id='AR'"));
            foreach(Field f in SpecialAbilityFields)foreach(string v in AbilityWeaponSamples(f))Check(original,p,"",AbilityRoutingXML(g,f.Name+"='"+Escape(v)+"'"));
            foreach(string f in new[]{"AnimState","Frames","Trigger"})foreach(string v in f=="AnimState"?SpecialPowerSingleModelNames:AIUpdateSamples(new Field(f,"Unsigned")))Check(original,p,"",AbilityRoutingXML(g,"","<CustomAnimAndDuration "+f+"='"+Escape(v)+"'/>"));
            foreach(string bad in new[]{"Unknown='1'","StartsPaused='true'","DisabledTypesToIgnore='ALL'","ModuleId='x'","TriggerAtTargetPosition='true'","ObjectStatus='ALL'","xmlns='foreign'","xmlns:f='foreign' f:TypeId='1'"})Reject(p,"",AbilityRoutingXML(g,bad));
            foreach(string bad in new[]{"text","<Unknown/>","<InitiateSound/>","<"+AbilityRoutingNodes[1-g]+"/>","<CustomAnimAndDuration/><CustomAnimAndDuration/>","<CustomAnimAndDuration Unknown='1'/>","<"+name+" Unknown='1'/>","<"+name+" xmlns='foreign'/>","<"+name+" xmlns:f='foreign' f:TypeId='1'/>","<"+name+">text</"+name+">","<"+name+"><Unknown/></"+name+">"})Reject(p,"",AbilityRoutingXML(g,"",bad));
            foreach(int count in new[]{0,1,2,27,28,29,65}) {
                string children="";for(int i=0;i<count;++i)children+="<"+name+"/>";
                b=Check(original,p,"",AbilityRoutingXML(g,"",children)).InstanceData;at=LifecycleAt(b,big);int rows=(int)Word(b,at+244,big);t=MotionTable(b,big);lt=(int)Word(b,Token(b,t,key,big)+8,big);
                if(Word(b,at+240,big)!=(uint)count || b.Length!=at+248+count*stride || rows!=(count==0?0:at+248))throw new Exception("Independent structured list stride/count/allocation");
                for(int i=0;i<count;++i) {
                    int row=rows+stride*i,entry=(int)Word(b,lt+20+12*i,big);AIUpdateSentinel(b,entry,g==0?3:4);
                    if(Word(b,lt+12+12*i,big)!=2974916843u || Word(b,lt+16+12*i,big)!=0 || Word(b,entry+8,big)!=row)throw new Exception("Independent structured list per-item Token");
                    for(int j=0;j<stride;++j){byte value=0;if(g==0 && (j==8 || j==4+(big?3:0)))value=1;if(g==1 && j==124)value=1;if(b[row+j]!=value)throw new Exception("Independent structured item complete defaults/padding");}
                }
            }
            string item=g==0?"ModuleId='MiXeD' TriggerAtTargetPosition='false' TriggerType='TARGET_OBJECT'":"ObjectStatus='ALL' ModelCondition='ALL' KindOf='ALL' UseLocalPlayerFilter='false'";
            string attrs="SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH' SpecialPowerTemplate='P\\23' Options='ALL' RequiredConditions='ALL' SetObjectStatusOnTrigger='ALL' ClearObjectStatusOnExit='ALL'",anim="<CustomAnimAndDuration AnimState='INVALID' Frames='16' Trigger='2'/>",nodes="<"+name+" "+item+"/><"+name+" "+item+"/>";
            var combo=Check(original,p,"",AbilityRoutingXML(g,attrs,anim+nodes));b=combo.InstanceData;at=LifecycleAt(b,big);int first=at+248+2*stride;
            if(Word(b,at+240,big)!=2 || Word(b,at+244,big)!=at+248 || Word(b,at+68,big)!=first || Word(b,at+200,big)!=first+4 || Word(b,at+232,big)!=first+12 || b.Length!=first+24)throw new Exception("Independent structured list then parent allocation order");
            for(int i=0;i<2;++i){int row=at+248+stride*i;if(g==0){if(Word(b,row+4,big)!=2 || b[row+8]!=0 || b[row+9]!=0 || b[row+10]!=0 || b[row+11]!=0)throw new Exception("Independent ModuleInfo12 enum/boolean/padding");}else {for(int j=0;j<31;++j)if(Word(b,row+4*j,big)!=0xffffffffu)throw new Exception("Independent intersection223/456/284 complete-word ALL");for(int j=124;j<128;++j)if(b[row+j]!=0)throw new Exception("Independent intersection boolean/padding");}}
            var reverse=Check(original,p,"",AbilityRoutingXML(g,attrs.Replace("SpecialObjectAttachToBone='abc' ContactPointOverride='EFGH'","ContactPointOverride='EFGH' SpecialObjectAttachToBone='abc'"),nodes+anim));Equal(combo.InstanceData,reverse.InstanceData,"Ability routing schema allocation order",false);Equal(combo.RelocationData,reverse.RelocationData,"Ability routing relocation order",false);Equal(combo.ImportsData,reverse.ImportsData,"Ability routing reference order",false);
            foreach(int count in new[]{0,1,2,27,28,29,65}){string modules="";for(int i=0;i<count;++i)modules+="<ObjectSMCHelper TypeId='"+AbilityRoutingTypes[g]+"' id='AR"+i+"' "+attrs+">"+anim+nodes+"</ObjectSMCHelper>";Check(original,p,"Browser='a bb'","<Behaviors>"+modules+"<ObjectSMCHelper TypeId='2891550594' id='Parent'/></Behaviors>");}
        }
        foreach(string f in new[]{"ModuleId","TriggerAtTargetPosition","TriggerType"})foreach(string v in f=="TriggerType"?new[]{"SELF_POSITION","TARGET_OBJECT","TARGET_POSITION","","UNKNOWN","target_object","0","-1","ALL","TARGET_OBJECT SELF_POSITION"," TARGET_OBJECT","TARGET_OBJECT "}:AIUpdateSamples(new Field(f,f=="ModuleId"?"String":"Boolean")))Check(original,p,"",AbilityRoutingXML(0,"","<SpecialPowerModules "+f+"='"+Escape(v)+"'/>"));
        foreach(string value in WeaponSetStatusValues)Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter ObjectStatus='"+Escape(value)+"'/>"));
        foreach(string value in SpecialPowerSingleModelNames)foreach(string v in new[]{value,"-"+value,value+" -"+value+" -"+value})Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter ModelCondition='"+Escape(v)+"'/>"));
        foreach(string value in KindNames)foreach(string v in new[]{value,"-"+value,value+" -"+value+" -"+value})Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter KindOf='"+Escape(v)+"'/>"));
        foreach(string value in new[]{"","ALL","all","+ALL","-ALL","UNKNOWN","unknown","-UNKNOWN -UNKNOWN","ALL -UNKNOWN","UNKNOWN ALL UNKNOWN","+","-","AIRCRAFT,INFANTRY"})Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter ObjectStatus='"+value+"' ModelCondition='"+value+"' KindOf='"+value+"'/>"));
        foreach(string value in AIUpdateSamples(new Field("UseLocalPlayerFilter","Boolean")))Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter UseLocalPlayerFilter='"+Escape(value)+"'/>"));
        {byte[] b=Check(original,p,"",AbilityRoutingXML(1,"","<IntersectionFlagsFilter ObjectStatus='UNKNOWN' ModelCondition='UNKNOWN' KindOf='UNKNOWN'/>" )).InstanceData;int row=LifecycleAt(b,big)+248;if(Word(b,row+24,big)!=(1u<<6) || Word(b,row+56,big)!=(1u<<9) || Word(b,row+104,big)!=(1u<<21))throw new Exception("Independent intersection unknown bits198/233/149");}
        var a=Check(original,p,"",AbilityRoutingXML(0,"","<SpecialPowerModules ModuleId='MiXeD'/>"));var z=Check(original,p,"",AbilityRoutingXML(0,"","<SpecialPowerModules ModuleId='mixed'/>"));Equal(a.InstanceData,z.InstanceData,"SpecialPowerModules typed ID ASCII case-folding",false);
    }
}
