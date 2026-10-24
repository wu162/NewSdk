using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlmusicscriptOracle {
 static readonly object[][] FieldCases={
 new object[]{1,"TimeBetweenConditionChecks","Time",4},
 new object[]{2,"CountDeadPlayersAsObservers","Bool",4},
 new object[]{3,"TimeBetweenConditionChecks","Time",4},
 new object[]{3,"MinUnitsToPass","Int",8},
 new object[]{3,"MinDistanceFromBase","Real",12},
 new object[]{3,"Relationship","Relationship",16},
 new object[]{4,"Timeout","Time",4},
 new object[]{5,"Track","LowerHash",4},
 new object[]{5,"Count","Int",8},
 new object[]{6,"TrackType","RawHashKind",4},
 new object[]{8,"TimeBetweenConditionChecks","Time",4},
 new object[]{8,"Count","Int",8},
 new object[]{8,"RequiredModelConditions","Model",12},
 new object[]{8,"ExcludedModelConditions","Model",72},
 new object[]{9,"EvaEvent","Ref",4},
 new object[]{9,"Timeout","Time",8},
 new object[]{10,"TimeBetweenConditionChecks","Time",4},
 new object[]{10,"EvaEvent","Ref",8},
 new object[]{10,"Count","Int",12},
 new object[]{10,"Distance","Real",16},
 new object[]{11,"Count","Int",4},
 new object[]{11,"Time","Time",8},
 new object[]{11,"CountOnlyKillsAgainstTheLocalPlayer","Bool",16},
 new object[]{11,"CountOnlyKillsByTheLocalPlayer","Bool",17},
 new object[]{11,"Filter","PtrRef",12},
 new object[]{15,"TrackedState","LowerHash",4},
 new object[]{15,"TrackedStateChange","Change",8},
 new object[]{15,"TimeSinceLastActive","Time",12},
 new object[]{15,"TimeSicneLastActiveOp","Compare",16},
 new object[]{16,"TrackedState","LowerHash",4},
 new object[]{17,"TimeBetweenConditionChecks","Time",4},
 new object[]{17,"Count","Int",8},
 new object[]{17,"Time","Time",12},
 new object[]{17,"CountOnlyEngagementsAgainstTheLocalPlayer","Bool",24},
 new object[]{17,"CountOnlyEngagementsByTheLocalPlayer","Bool",25},
 new object[]{17,"RequiredScreenStatus","Screen",16},
 new object[]{17,"TargetFilter","PtrRef",20},
 new object[]{18,"TimeBetweenConditionChecks","Time",4},
 new object[]{18,"Time","Time",8},
 new object[]{18,"Ratio","Real",12},
 new object[]{18,"RatioOp","Compare",16},
 new object[]{19,"TrackedWeapon","LowerHash",4},
 new object[]{19,"Time","Time",8},
 new object[]{19,"Count","Int",12},
 new object[]{19,"CountOnlyHitsAgainstTheLocalPlayer","Bool",20},
 new object[]{19,"TargetFilter","PtrRef",16},
 new object[]{20,"TrackTypeKey","RawHashKind",4},
 new object[]{20,"Level","Int",8},
 new object[]{20,"Priority","Int",12},
 new object[]{20,"Condition","PtrRef",16},
 new object[]{20,"ConditionsAreLatch","Bool",32},
 new object[]{20,"Track","Ref",20},
 new object[]{20,"FadeInTrack","Bool",33},
 new object[]{20,"FadeOutLowerLevelTrack","Bool",34}
 };
 static readonly string[] ComparisonOperator={"EQUAL","GREATER_THAN","GREATER_THAN_OR_EQUAL","LESS_THAN","LESS_THAN_OR_EQUAL"};
 static readonly string[] StateChangeType={"ENTER","EXIT"};
 static readonly string[] ScreenStatus={"OFF_SCREEN","ON_OR_OFF_SCREEN","ON_SCREEN"};
 static readonly string[] ObjectFilterRelationship={"ALLIES","ENEMIES","NEUTRAL","SAME_PLAYER"};

 static readonly string[] Numbers={"","0","-0","1","-1","+17","077","08","0x12345678","2147483647","2147483648","-2147483648","-2147483649","4294967295","-4294967296","999999999999999999999","1.5","-2.75","1e-5","2D3","NaN","inf","  -17tail","1,5","foo"};
 static readonly string[] Times={"","0","1","1s","100ms","-25ms","1.25ms","1.5s",".1s","3MS","ms5","s5","2d3ms","1e-3s","2D3ms","junk"};
 static readonly string[] Refs={"","none","Type:A\\0","Type:A\\1","Type:A\\3tail","Type:A\\-1","Type:A\\2147483648","Type:A\\99999999999","Type:A\\1\\2","A & B\\+17","é\\2","東京\\3"};
 static string Attr(string name,string value){return name+"='"+Escape(value)+"'";}
 static string Node(string name,string attrs,string children=""){return "<"+name+" "+attrs+">"+children+"</"+name+">";}
 static void DomAttr(IAssetBuilderPlugin original,int root,string name,string value,TargetPlatform platform){var d=new XmlDocument();d.LoadXml(Xml(Roots[root],"",""));d.DocumentElement.SetAttribute(name,value);TestDocument(original,root,"DOM/"+name,d,platform);}
 static string Sample(string kind){return kind=="Int"?"7":kind=="Time"?"125ms":kind=="Bool"?"true":kind=="Real"?"1.25":kind=="Compare"?"GREATER_THAN":kind=="Change"?"EXIT":kind=="Screen"?"ON_OR_OFF_SCREEN":kind=="Relationship"?"ALLIES ENEMIES":kind=="Model"?"DAMAGED MOVING":kind=="Ref"||kind=="PtrRef"?"Type:Asset\\1":"AbC_Name";}
 static void Cases(IAssetBuilderPlugin original,TargetPlatform platform){bool big=platform!=TargetPlatform.Win32;
 foreach(var field in FieldCases){int root=(int)field[0];string name=(string)field[1],kind=(string)field[2];string[] values=kind=="Time"?Times:kind=="Compare"?ComparisonOperator:kind=="Change"?StateChangeType:kind=="Screen"?ScreenStatus:kind=="Relationship"?new[]{"","ALLIES","ENEMIES","NEUTRAL","SAME_PLAYER","unknown","-unknown unknown","ALL -ENEMIES","+","-"}:kind=="Model"?new[]{"","DAMAGED","MOVING","ALL","-ALL MOVING","unknown","-DAMAGED -DAMAGED","+","-"}:kind=="Ref"||kind=="PtrRef"?Refs:kind=="Bool"?new[]{"","true","false","TRUE","True","1","0","truth","yes","2","-1"}:Numbers;
 foreach(string value in values)Test(original,root,"field/"+name+"/"+value,Attr(name,value),"",platform);
 foreach(string value in new[]{"unknown","\0tail","ABC\0tail","\0ABC","é","東京","17\0tail","\u0085","\u00A0","١٢"})DomAttr(original,root,name,value,platform);
 if(kind=="Compare"||kind=="Change"||kind=="Screen"){foreach(string value in values)foreach(string n in new[]{value+"\0tail","\0"+value,"unknown\0"+value})DomAttr(original,root,name,n,platform);}
 if(kind=="Time")foreach(string n in new[]{"2\0ms","2s\0ms","2ms\0s","\0ms2","2\0d3ms"})DomAttr(original,root,name,n,platform);
 if(kind=="PtrRef"){var b=Test(original,root,"independent empty nullable ref/"+name,Attr(name,""),"",platform);if(b.InstanceData.Length!=Sizes[root]+4||Word(b.InstanceData,(int)field[3],big)!=(uint)Sizes[root]||b.ImportsData.Length!=0)throw new Exception("Nullable ref allocation");++layouts;}
 }
 foreach(int root in new[]{12,13,14})foreach(string value in Refs)Test(original,root,"condition strong reference","",Node("Condition","",Escape(value)),platform);
 foreach(string value in Numbers)foreach(string attr in new[]{"Weight","Duration"})Test(original,20,"timeout/"+attr+"/"+value,"",Node("Timeout",Attr(attr,value)),platform);
 foreach(string value in Times)Test(original,20,"timeout time/"+value,"",Node("Timeout",Attr("Duration",value)),platform);
 var timeout=Test(original,20,"independent empty timeout","",Node("Timeout",""),platform);if(timeout.InstanceData.Length!=44||Word(timeout.InstanceData,24,big)!=1||Word(timeout.InstanceData,28,big)!=36||Word(timeout.InstanceData,36,big)!=1000||Word(timeout.InstanceData,40,big)!=0||timeout.InstanceData[34]!=1)throw new Exception("Timeout stride/default ABI");++layouts;
 var rawA=Test(original,6,"raw uppercase","TrackType='ABC'","",platform);var rawB=Test(original,6,"raw lowercase","TrackType='abc'","",platform);if(Word(rawA.InstanceData,4,big)==Word(rawB.InstanceData,4,big))throw new Exception("Raw StringHash must preserve case");++layouts;
 var lowerA=Test(original,5,"weak uppercase","Track='ABC'","",platform);var lowerB=Test(original,5,"weak lowercase","Track='abc'","",platform);Equal("TypedAssetId case-fold",lowerA.InstanceData,lowerB.InstanceData);++layouts;
 foreach(int count in new[]{0,1,2,3,7,28,29,30,31,32,65,129}){var refs=new StringBuilder();var timeouts=new StringBuilder();for(int i=0;i<count;++i){refs.Append(Node("Condition","","Condition:A\\"+(i%7)));timeouts.Append(Node("Timeout","Weight='"+(1000+i)+"' Duration='"+(i+1)+"ms'"));}foreach(int root in new[]{13,14}){var b=Test(original,root,"long condition list/"+count,"",refs.ToString(),platform);if(Word(b.InstanceData,4,big)!=count)throw new Exception("Condition count");++layouts;}var t=Test(original,20,"long timeout list/"+count,"Condition='Condition:A\\1' Track='AudioEvent:T\\2'",timeouts.ToString(),platform);if(Word(t.InstanceData,24,big)!=count)throw new Exception("Timeout count");++layouts;}
 for(int root=0;root<Roots.Length;++root){var attrs=new StringBuilder();foreach(var field in FieldCases)if((int)field[0]==root)attrs.Append(Attr((string)field[1],Sample((string)field[2]))+" ");string children=root==8||root==10?FullFilter():root==12||root==13||root==14?"<Condition>Condition:A\\1</Condition>"+(root==12?"":"<Condition>Condition:B\\2</Condition>"):root==20?"<Timeout Weight='7' Duration='125ms'/><Timeout Weight='11' Duration='1.5s'/>":"";for(int i=0;i<128;++i)Test(original,root,"complete root/"+i,attrs.ToString(),children,platform);}
 foreach(int root in new[]{12,13,14}){Reject(root,Xml(Roots[root],"","<Condition Unknown='1'/>"));Reject(root,Xml(Roots[root],"","<Condition><Unknown/></Condition>"));}Reject(12,Xml(Roots[12],"","<Condition/><Condition/>"));Reject(20,Xml(Roots[20],"","<Timeout Unknown='1'/>"));Reject(20,Xml(Roots[20],"","<Timeout><Unknown/></Timeout>"));Reject(20,Xml(Roots[20],"","<Timeout>text</Timeout>"));
 FilterCases(original,platform);
 }
}

