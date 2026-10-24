using System;
using System.Text;
using BinaryAssetBuilder.Core;
partial class XmlsinglesoundOracle {
 static readonly object[][] Fields={
 new object[]{"Volume","Percent"},
 new object[]{"VolumeShift","Percent"},
 new object[]{"PerFileVolumeShift","Percent"},
 new object[]{"MinVolume","Percent"},
 new object[]{"ShrunkenPitchModifier","Real"},
 new object[]{"ShrunkenVolumeModifier","Percent"},
 new object[]{"PlayPercent","Percent"},
 new object[]{"Limit","Int"},
 new object[]{"Priority","Priority"},
 new object[]{"Type","TypeBits"},
 new object[]{"Control","ControlBits"},
 new object[]{"MinRange","Real"},
 new object[]{"MaxRange","Real"},
 new object[]{"LowPassCutoff","Percent"},
 new object[]{"ZoomedInOffscreenVolumePercent","Percent"},
 new object[]{"ZoomedInOffscreenMinVolumePercent","Percent"},
 new object[]{"ZoomedInOffscreenOcclusionPercent","Percent"},
 new object[]{"ReverbEffectLevel","Percent"},
 new object[]{"DryLevel","Percent"}
 };
 static readonly string[] Numbers={"","0","-0","+1","-1","17","077","08","0x12345678","2147483647","2147483648","-2147483648","-2147483649","4294967295","-4294967296","999999999999999999999","1.5","-2.75","1e-5","2D+3","0x1p2","inf","NaN","  -15tail","1,5","foo","--2"};
 static readonly string[] Times={"","0","1","1s","100ms","-25ms","1.25ms","1.5s",".1s","3MS","ms5","s5","2d3ms","1e-3s","2D3ms","junk","inf"};
 static readonly string[] References={"","none","AudioFile:Name\\0","AudioFile:Name\\1","AudioFile:Name\\3tail","AudioFile:Name\\-1","AudioFile:Name\\2147483648","AudioFile:Name\\99999999999","AudioFile:Name\\1\\2","A & B\\+17","é\\2","東京\\3"};
 static readonly string[] Ranges={"PitchShift","PerFilePitchShift","Delay","InitialDelay","MinRangeShift","MaxRangeShift","NonInterruptibleTime"};
 static string Attr(string key,string value){return key+"='"+Escape(value)+"'";}
 static string Node(string name,string attrs,string children=""){return "<"+name+" "+attrs+">"+children+"</"+name+">";}
 static string BitsValues(string kind,int at){return kind=="TypeBits"?new[]{"","UI WORLD VOICE","ALL","-ALL WORLD","+UI -UI -UI","unknown","-unknown unknown","ALL -UI","all WORLD","aLl -WORLD","+","-","UI\tWORLD"}[at%13]:new[]{"","LOOP SEQUENTIAL SMART_LIMITING","ALL","-ALL LOOP","+LOOP -LOOP -LOOP","unknown","-unknown unknown","ALL -LOOP","all SEQUENTIAL","aLl -LOOP","+","-","LOOP\tSEQUENTIAL"}[at%13];}
 static string CoreChildren(int i){return "<PitchShift Low='0.75' High='1.25'/><PerFilePitchShift Low='0.25' High='1.75'/><Delay Low='1' High='3'/><InitialDelay Low='2' High='4'/><VolumeSliderMultiplier Slider='VOICE' Multiplier='0.25'/><VolumeSliderMultiplier Slider='MUSIC' Multiplier='1.25'/><MinRangeShift Low='-25.5' High='50.5'/><MaxRangeShift Low='10.5' High='75.5'/><LimitGroup>AudioEventLimitGroup:One\\1</LimitGroup><LimitGroup>AudioEventLimitGroup:Two\\2</LimitGroup><NonInterruptibleTime Low='125ms' High='1.5s'/>";}
 static string Extra(int root,int i){if(root==2)return "<Attack Weight='17' Volume='25.5'>AudioFile:A\\1</Attack><Sound Weight='23' Volume='50.5'>AudioFile:B\\2</Sound><Decay Weight='29' Volume='75.5'>AudioFile:C\\3</Decay>";if(root>=3&&root<=8)return "<Filename>AudioFile:F\\"+(i%7)+"</Filename>";return "";}
 static void Cases(IAssetBuilderPlugin original,TargetPlatform platform){bool big=platform!=TargetPlatform.Win32;
 for(int root=1;root<=8;++root){foreach(var row in Fields){string name=(string)row[0],kind=(string)row[1];string[] values=kind=="Priority"?new[]{"LOWEST","LOW","NORMAL","HIGH","HIGHEST","CRITICAL","low","unknown","0",""}:kind=="TypeBits"||kind=="ControlBits"?new[]{BitsValues(kind,0),BitsValues(kind,1),BitsValues(kind,2),BitsValues(kind,3),BitsValues(kind,4),BitsValues(kind,5),BitsValues(kind,6),BitsValues(kind,7),BitsValues(kind,8),BitsValues(kind,9),BitsValues(kind,10),BitsValues(kind,11),BitsValues(kind,12)}:Numbers;foreach(string value in values)Test(original,root,"root scalar/"+name+"/"+value,Attr(name,value),"",platform);}
 foreach(string value in new[]{"SOUNDFX","VOICE","MUSIC","AMBIENT","MOVIE","NONE","unknown","1",""})Test(original,root,"nullable submix/"+value,Attr("SubmixSlider",value),"",platform);
 foreach(string range in Ranges)foreach(string axis in new[]{"Low","High"})foreach(string value in range=="NonInterruptibleTime"?Times:Numbers)Test(original,root,"range/"+range+"/"+axis+"/"+value,"",Node(range,Attr(axis,value)),platform);
 foreach(string range in Ranges){var b=Test(original,root,"present empty range/"+range,"",Node(range,""),platform);int offset=range=="PitchShift"?84:range=="PerFilePitchShift"?88:range=="Delay"?92:range=="InitialDelay"?96:range=="MinRangeShift"?108:range=="MaxRangeShift"?112:124;if(b.InstanceData.Length!=Sizes[root]+8||Word(b.InstanceData,offset,big)!=(uint)Sizes[root])throw new Exception("Independent range allocation");++layouts;}
 foreach(string value in new[]{"SOUNDFX","VOICE","MUSIC","AMBIENT","MOVIE","NONE","unknown","0",""})Test(original,root,"multiplier slider/"+value,"",Node("VolumeSliderMultiplier",Attr("Slider",value)),platform);
 foreach(string value in Numbers)Test(original,root,"multiplier real/"+value,"",Node("VolumeSliderMultiplier",Attr("Multiplier",value)),platform);
 foreach(string value in References)Test(original,root,"limit group reference/"+value,"",Node("LimitGroup","",Escape(value)),platform);
 Test(original,root,"present empty list members","",Node("VolumeSliderMultiplier","")+Node("LimitGroup",""),platform);
 for(int i=0;i<128;++i){string attrs="Volume='75.25' VolumeShift='12.5' PerFileVolumeShift='3.5' MinVolume='2.5' ShrunkenPitchModifier='1.125' ShrunkenVolumeModifier='80.25' PlayPercent='67.5' Limit='7' Priority='HIGH' Type='UI WORLD VOICE' Control='LOOP RANDOMSTART FADE_ON_START' MinRange='128.25' MaxRange='777.5' LowPassCutoff='12.5' ZoomedInOffscreenVolumePercent='37.5' ZoomedInOffscreenMinVolumePercent='87.5' ZoomedInOffscreenOcclusionPercent='17.5' ReverbEffectLevel='7.5' DryLevel='93.5' SubmixSlider='MOVIE'";Test(original,root,"complete graph/"+i,attrs,CoreChildren(i)+Extra(root,i),platform);}
 foreach(int count in new[]{0,1,2,3,7,28,29,30,31,32,65}){var sliders=new StringBuilder();var refs=new StringBuilder();for(int i=0;i<count;++i){sliders.Append(Node("VolumeSliderMultiplier","Slider='VOICE' Multiplier='1.25'"));refs.Append(Node("LimitGroup","","AudioEventLimitGroup:G\\"+(i%7)));}var b=Test(original,root,"core long lists/"+count,"",sliders.ToString()+refs.ToString(),platform);if(Word(b.InstanceData,100,big)!=count||Word(b.InstanceData,116,big)!=count)throw new Exception("Independent core list counts");++layouts;}
 }
 foreach(string list in new[]{"Attack","Sound","Decay"}){foreach(string value in References)Test(original,2,"weighted reference/"+list+"/"+value,"",Node(list,"",Escape(value)),platform);foreach(string key in new[]{"Weight","Volume"})foreach(string value in Numbers)Test(original,2,"weighted scalar/"+list+"/"+key+"/"+value,"",Node(list,Attr(key,value),"AudioFile:A\\1"),platform);Test(original,2,"weighted present-empty/"+list,"",Node(list,""),platform);}
 for(int root=3;root<=8;++root){foreach(string value in References)Test(original,root,"filename/"+value,"",Node("Filename","",Escape(value)),platform);var empty=Test(original,root,"empty filename","",Node("Filename",""),platform);if(empty.InstanceData.Length!=136||Word(empty.InstanceData,128,big)!=132||empty.RelocationData.Length!=8)throw new Exception("Independent nullable Filename ref");++layouts;}
 foreach(string value in new[]{"","LOOP","PLAY_ONE","LOOP PLAY_ONE","unknown","-unknown unknown","-LOOP -LOOP","+PLAY_ONE -PLAY_ONE","ALL","all LOOP","-ALL PLAY_ONE","+","-"})Test(original,9,"multi control/"+value,Attr("Control",value),"",platform);
 foreach(string value in References)Test(original,9,"subsound reference/"+value,"",Node("Subsound","",Escape(value)),platform);
 foreach(string key in new[]{"Weight","PitchShiftLow","PitchShiftHigh","Volume","PlayPercent","VolumeShift"})foreach(string value in Numbers)Test(original,9,"subsound scalar/"+key+"/"+value,"",Node("Subsound",Attr(key,value),"AudioEvent:A\\1"),platform);
 for(int i=0;i<128;++i)Test(original,9,"complete multisound/"+i,"Control='LOOP PLAY_ONE'","<Subsound Weight='17' PitchShiftLow='0.75' PitchShiftHigh='1.25' Volume='25.5' PlayPercent='75.5' VolumeShift='12.5'>AudioEvent:A\\1</Subsound><Subsound Weight='23' PitchShiftLow='0.25' PitchShiftHigh='1.75' Volume='50.5' PlayPercent='87.5' VolumeShift='3.5'>MusicTrack:B\\2</Subsound>",platform);
 foreach(int count in new[]{0,1,2,3,7,28,29,30,31,32,65}){var weighted=new StringBuilder();var subs=new StringBuilder();for(int i=0;i<count;++i){weighted.Append(Node("Attack","Weight='17' Volume='25.5'","AudioFile:A\\"+(i%7)));weighted.Append(Node("Sound","Weight='23' Volume='50.5'","AudioFile:B\\"+(i%7)));weighted.Append(Node("Decay","Weight='29' Volume='75.5'","AudioFile:C\\"+(i%7)));subs.Append(Node("Subsound","Weight='17' PitchShiftLow='0.75' PitchShiftHigh='1.25' Volume='25.5' PlayPercent='75.5' VolumeShift='12.5'","AudioEvent:A\\"+(i%7)));}Test(original,2,"weighted long lists/"+count,"",weighted.ToString(),platform);var m=Test(original,9,"subsound long list/"+count,"",subs.ToString(),platform);if(Word(m.InstanceData,8,big)!=count)throw new Exception("Independent subsound count");++layouts;}
 BoundaryCases(original,platform);
 }
}

