using System;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlprogressionOracle {
 static void UiCases(IAssetBuilderPlugin original,TargetPlatform platform){bool big=platform!=TargetPlatform.Win32;
 string[] factions={"Allies","Soviet","Japan","Neutral","RANDOM","unknown","","allies","Allies "," Japan","ALL"};
 foreach(string f in factions){var b=Test(original,8,"UI-faction:"+f,"","<FactionSettings Faction='"+f+"'/>",platform);uint expected=f=="Soviet"?1u:f=="Japan"?2u:f=="Neutral"?3u:f=="RANDOM"?UInt32.MaxValue:0u;if(Word(b.InstanceData,4,big)!=1||Word(b.InstanceData,8,big)!=12||Word(b.InstanceData,12,big)!=expected)throw new Exception("UI faction/count/pointer independent ABI");}
 foreach(string v in new[]{"","0","1","-1","0x17","077","08","2147483647","2147483648","4294967295","4294967296","999999999999999999999","-99999999999999999999","1.25","nan","unknown","Image\\0","Image\\17","Image\\-1","Image\\4294967295","Image\\7\\8","前缀\\23"}){
 string a=Escape(v);Test(original,8,"UI-reference:"+v,"","<FactionSettings LockedImage='"+a+"'/>",platform);foreach(string channel in new[]{"R","G","B","A"})Test(original,8,"UI-channel:"+channel+":"+v,"","<FactionSettings><ImageTintColor "+channel+"='"+a+"'/></FactionSettings>",platform);
 Test(original,8,"UI-color:"+v,"","<FactionSettings><ImageTintColor R='"+a+"' G='"+a+"' B='"+a+"' A='"+a+"'/></FactionSettings>",platform);
 }
 var layout=Test(original,8,"UI-independent-color-reference","TypeId='134745024'","<FactionSettings TypeId='123' Faction='Japan' LockedImage='Image\\17'><ImageTintColor TypeId='456' R='17' G='34' B='51' A='68'/></FactionSettings>",platform);
 if(layout.InstanceData.Length!=24||Word(layout.InstanceData,4,big)!=1||Word(layout.InstanceData,8,big)!=12||Word(layout.InstanceData,12,big)!=2||Word(layout.InstanceData,16,big)!=18||Word(layout.InstanceData,20,big)!=0x44112233u||layout.RelocationData.Length!=8||Word(layout.RelocationData,0,big)!=8||layout.ImportsData.Length!=8||Word(layout.ImportsData,0,big)!=16)throw new Exception("UI packed ARGB/import independent ABI");
 var empty=Test(original,8,"UI-empty-color","","<FactionSettings><ImageTintColor/></FactionSettings>",platform);if(Word(empty.InstanceData,20,big)!=0xff000000u)throw new Exception("UI empty color alpha default");
 foreach(int count in new[]{0,1,2,3,5,27,28,29,64,257}){string children="";for(int i=0;i<count;++i){children+="<FactionSettings Faction='"+factions[i%5]+"' LockedImage='Image\\"+(count-i)+"'><ImageTintColor R='"+i+"' G='"+(i+1)+"' B='"+(i+2)+"' A='"+(i+3)+"'/></FactionSettings>";}var b=Test(original,8,"UI-list:"+count,"",children,platform);if(Word(b.InstanceData,4,big)!=count||b.InstanceData.Length!=12+12*count)throw new Exception("UI repeated list stride");}
 foreach(string child in new[]{"<FactionSettings Bad='1'/>","<FactionSettings><ImageTintColor/><ImageTintColor/></FactionSettings>","<FactionSettings><ImageTintColor bad='1'/></FactionSettings>","<FactionSettings><ImageTintColor><Nested/></ImageTintColor></FactionSettings>","<FactionSettings>text</FactionSettings>","<FactionSettings><Other/></FactionSettings>","<FactionSettings xmlns='other'/>","<FactionSettings xmlns:x='other' x:TypeId='1'/>","<FactionSettings><ImageTintColor r='1'/></FactionSettings>"})Reject(8,Xml("UICommandButton","",child));
 }
}
