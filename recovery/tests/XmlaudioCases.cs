using System;
using System.Collections.Generic;
using System.Text;
using BinaryAssetBuilder.Core;
partial class XmlaudioOracle {
 static readonly object[][] Fields={
 new object[]{2,"","StreamBufferSizePerChannel","Int"},
 new object[]{2,"","MaxRequestsPerStream","Int"},
 new object[]{2,"","ForceResetTime","Time"},
 new object[]{2,"","EmergencyResetTime","Time"},
 new object[]{2,"","SuppressOcclusion","Bool"},
 new object[]{2,"","MinOcclusion","Percent"},
 new object[]{2,"","DelayPriorToPlayingToReadSoundFile","Time"},
 new object[]{2,"","ReadAheadTime","Time"},
 new object[]{2,"","QueueAheadTime","Time"},
 new object[]{2,"","LongTime","Time"},
 new object[]{2,"","AudioFootprintInBytes","UInt"},
 new object[]{2,"","DontLoadThirdVariationWhenFileReadQueueIsLongerThan","UInt"},
 new object[]{2,"","DontLoadSecondVariationWhenFileReadQueueIsLongerThan","UInt"},
 new object[]{2,"","DontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan","UInt"},
 new object[]{2,"","WhatIsLowPriorityForDontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan","Priority"},
 new object[]{2,"","VolumeToGainConversionFactor","Real"},
 new object[]{2,"","DefaultSoundVolume","Percent"},
 new object[]{2,"","DefaultVoiceVolume","Percent"},
 new object[]{2,"","DefaultMusicVolume","Percent"},
 new object[]{2,"","DefaultMovieVolume","Percent"},
 new object[]{2,"","DefaultAmbientVolume","Percent"},
 new object[]{2,"","AutomaticSubtitleDuration","Time"},
 new object[]{2,"","AutomaticSubtitleLines","Int"},
 new object[]{2,"","PanRadiusScaleValue","Real"},
 new object[]{2,"","PanSize","Real"},
 new object[]{2,"","MinSampleVolume","Percent"},
 new object[]{2,"","PositionDeltaForReverbCheck","Real"},
 new object[]{2,"","TimeToFadeAudio","Time"},
 new object[]{2,"","AmbientStreamHysteresisVolume","Int"},
 new object[]{2,"","VolumeMultiplierFor2DSounds","Percent"},
 new object[]{2,"","VolumeMultiplierFor3DSounds","Percent"},
 new object[]{2,"","VoiceMoveToCampMaxCampnessAtStartPoint","UInt"},
 new object[]{2,"","VoiceMoveToCampMinCampnessAtEndPoint","UInt"},
 new object[]{2,"","MinDelayBetweenEnterStateVoice","Time"},
 new object[]{2,"","TimeSinceLastAttackForVoiceRetreatToCastle","Time"},
 new object[]{2,"","DistanceToLookForEnemiesForVoiceRetreatToCastle","Real"},
 new object[]{2,"","PostPostGameMusicWait","Time"},
 new object[]{2,"","PlayEnemySightedEventsOnlyFromNearbyUnits","Bool"},
 new object[]{2,"","CapturedUnitsVoicePriorityOffset","Int"},
 new object[]{2,"","UISelectionVoicePriorityOffset","Int"},
 new object[]{2,"","AddToSelectionVoiceTimeout","Time"},
 new object[]{2,"","MinVolumeBeforeFade","Percent"},
 new object[]{2,"","MaxVolumeBeforeFade","Percent"},
 new object[]{2,"","PathMusicBufferTime","Time"},
 new object[]{2,"","PathMusicLatencyMilisec","Int"},
 new object[]{5,"","MicrophonePreferredFractionCameraToGround","Percent"},
 new object[]{5,"","VolumeMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{5,"","PanningMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{5,"","MicrophoneMinDistanceToCamera","Real"},
 new object[]{5,"","MicrophoneMaxDistanceToCamera","Real"},
 new object[]{5,"","ZoomMinDistance","Real"},
 new object[]{5,"","ZoomMaxDistance","Real"},
 new object[]{5,"","ZoomSoundVolumePercentageAmount","Percent"},
 new object[]{5,"","ZoomFadeDistanceForMaxEffect","Real"},
 new object[]{5,"","ZoomFadeZeroEffectEdgeLength","Real"},
 new object[]{5,"","ZoomFadeFullEffectEdgeLength","Real"},
 new object[]{2,"TacticalMicSettings","MicrophonePreferredFractionCameraToGround","Percent"},
 new object[]{2,"TacticalMicSettings","VolumeMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"TacticalMicSettings","PanningMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"TacticalMicSettings","MicrophoneMinDistanceToCamera","Real"},
 new object[]{2,"TacticalMicSettings","MicrophoneMaxDistanceToCamera","Real"},
 new object[]{2,"TacticalMicSettings","ZoomMinDistance","Real"},
 new object[]{2,"TacticalMicSettings","ZoomMaxDistance","Real"},
 new object[]{2,"TacticalMicSettings","ZoomSoundVolumePercentageAmount","Percent"},
 new object[]{2,"TacticalMicSettings","ZoomFadeDistanceForMaxEffect","Real"},
 new object[]{2,"TacticalMicSettings","ZoomFadeZeroEffectEdgeLength","Real"},
 new object[]{2,"TacticalMicSettings","ZoomFadeFullEffectEdgeLength","Real"},
 new object[]{2,"LivingWorldMicSettings","MicrophonePreferredFractionCameraToGround","Percent"},
 new object[]{2,"LivingWorldMicSettings","VolumeMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"LivingWorldMicSettings","PanningMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"LivingWorldMicSettings","MicrophoneMinDistanceToCamera","Real"},
 new object[]{2,"LivingWorldMicSettings","MicrophoneMaxDistanceToCamera","Real"},
 new object[]{2,"LivingWorldMicSettings","ZoomMinDistance","Real"},
 new object[]{2,"LivingWorldMicSettings","ZoomMaxDistance","Real"},
 new object[]{2,"LivingWorldMicSettings","ZoomSoundVolumePercentageAmount","Percent"},
 new object[]{2,"LivingWorldMicSettings","ZoomFadeDistanceForMaxEffect","Real"},
 new object[]{2,"LivingWorldMicSettings","ZoomFadeZeroEffectEdgeLength","Real"},
 new object[]{2,"LivingWorldMicSettings","ZoomFadeFullEffectEdgeLength","Real"},
 new object[]{2,"ShellMicSettings","MicrophonePreferredFractionCameraToGround","Percent"},
 new object[]{2,"ShellMicSettings","VolumeMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"ShellMicSettings","PanningMicrophonePullTowardsTerrainLookAtPoint","Percent"},
 new object[]{2,"ShellMicSettings","MicrophoneMinDistanceToCamera","Real"},
 new object[]{2,"ShellMicSettings","MicrophoneMaxDistanceToCamera","Real"},
 new object[]{2,"ShellMicSettings","ZoomMinDistance","Real"},
 new object[]{2,"ShellMicSettings","ZoomMaxDistance","Real"},
 new object[]{2,"ShellMicSettings","ZoomSoundVolumePercentageAmount","Percent"},
 new object[]{2,"ShellMicSettings","ZoomFadeDistanceForMaxEffect","Real"},
 new object[]{2,"ShellMicSettings","ZoomFadeZeroEffectEdgeLength","Real"},
 new object[]{2,"ShellMicSettings","ZoomFadeFullEffectEdgeLength","Real"},
 new object[]{2,"VolumeCompressorSettings","Threshold","Real"},
 new object[]{2,"VolumeCompressorSettings","Ratio","Real"},
 new object[]{2,"VolumeCompressorSettings","AttackTime","Time"},
 new object[]{2,"VolumeCompressorSettings","ReleaseTime","Time"},
 new object[]{2,"VolumeCompressorSettings","AppliesEquallyToAllChannels","Bool"},
 new object[]{1,"","Level","Lod"},
 new object[]{1,"","AllowReverb","Bool"},
 new object[]{1,"","MaximumAmbientStreams","Int"},
 new object[]{1,"","AllowCPULimiter","Bool"},
 new object[]{1,"","CPULimiterLevel","Percent"},
 new object[]{1,"","CPULimiterLevelDebug","Percent"},
 new object[]{1,"","MaxVoices","Int"},
 new object[]{1,"","OutputBitRate","Int"},
 new object[]{1,"","AllowVolumeCompressor","Bool"},
 new object[]{1,"","MinimumLogicalProcessors","UInt"},
 new object[]{0,"","Limit","Int"},
 new object[]{3,"","Volume","Percent"}
 };

 static string Attr(string name,string value){return name+"='"+Escape(value)+"'";}
 static string Node(string name,string attrs,string children=""){return "<"+name+" "+attrs+">"+children+"</"+name+">";}
 static string Fade(string name,int seed){return Node(name,Attr("FrameCount",(seed-3).ToString()),"<StartColor r='0.25' g='-0.75' b='2.5'/><EndColor r='1' g='2' b='3'/>");}
 static string TextNode(int seed){return Node("TextInstance","TextId='A  B&#x9;C  A' Font='Arial &amp; Sans' PointSize='"+(seed+11)+"' Justification='"+new[]{"LEFT","RIGHT","CENTER","unknown"}[seed%4]+"' XOffset='-1.25' YOffset='0.1'","<TextColor r='.25' g='.5' b='1.0'/>");}
 static string Movie(int seed){return Node("Movie","TypeId='3074724042' MovieAsset='Video:Oracle\\1'",(seed%2==0?Fade("FadeIn",seed):"")+Fade("FadeOut",seed));}
 static string Title(int seed){return Node("TitleCard","TypeId='866596483' DisplayTime='"+seed+"ms'",TextNode(seed)+TextNode(seed+1)+Fade("FadeIn",seed)+Fade("FadeOut",seed));}
 static string AudioSettingsChildren(int seed){return "<AutomaticSubtitleWindowColor R='.1' G='0.2' B='0.3' A='0.4'/><AutomaticSubtitleTextColor R='1' G='2' B='3' A='4'/>"+Node("VolumeSliderMultiplierForMovie","Slider='VOICE' Multiplier='1.25'")+Node("VolumeSliderMultiplierForMovie","Slider='invalid' Multiplier='-3.5'")+Node("TacticalMicSettings","MicrophonePreferredFractionCameraToGround='25' ZoomMinDistance='12.5'")+Node("LivingWorldMicSettings","ZoomMaxDistance='-1.5' ZoomSoundVolumePercentageAmount='50%'")+Node("ShellMicSettings","VolumeMicrophonePullTowardsTerrainLookAtPoint='77'")+Node("VolumeCompressorSettings","Threshold='25.125' Ratio='2.5' AttackTime='251ms' ReleaseTime='7s' AppliesEquallyToAllChannels='false'");}
 static void Cases(IAssetBuilderPlugin original,TargetPlatform platform){bool big=platform!=TargetPlatform.Win32;
 string[] ints={"","0","-0","+1","-1","17","077","0x12345678","-2147483648","2147483647","2147483648","4294967295","-4294967296","999999999999999999999","  -15tail","08","foo","--2"," + 3"};
 string[] floats={"","0","-0","+1.5","-2.75","0.1","100%","1.25junk","1e-5","2D+3","0x1p2","inf","NaN","  -15tail","1,5","foo","999999999999999999999"};
 string[] times={"","0","1","1s","100ms","-25ms","1.25ms","1.5s",".1s","3MS","ms5","s5","2d3ms","1e-3s","2D3ms","junk","inf"};
 foreach(var f in Fields){int root=(int)f[0];string wrapper=(string)f[1],name=(string)f[2],kind=(string)f[3];string[] values=kind=="Int"||kind=="UInt"?ints:kind=="Bool"?new[]{"","true","false","True","TRUE","1","0","truth","yes","-1","2"}:kind=="Priority"?new[]{"LOWEST","LOW","NORMAL","HIGH","HIGHEST","CRITICAL","low","unknown","1",""}:kind=="Lod"?new[]{"UNKNOWN","LOW","HIGH","high","unknown","1",""}:kind=="Time"?times:floats;
 foreach(string value in values){string a=Attr(name,value);Test(original,root,"field/"+wrapper+"/"+name+"/"+value,wrapper==""?a:"",wrapper==""?"":Node(wrapper,a),platform);}
 }
 foreach(string v in new[]{"","a","1234","A &amp; B","Movies/foo.vp6"," a  b ","é","long long string"}){
 string escaped=v=="A &amp; B"?v:Escape(v);Test(original,3,"filename", "Filename='"+escaped+"'","",platform);Test(original,3,"comment","Comment='"+escaped+"'","",platform);}
 foreach(string s in new[]{"DUCK","NO_DUCK","duck","unknown","0",""})Test(original,3,"override",Attr("SoundOverride",s),"",platform);
 foreach(string s in new[]{"UNKNOWN","PENTIUM_4","CORE_DUO","ATHLON","ATHLON_64","unknown","0",""})Test(original,1,"processor", "",Node("Processor",Attr("Type",s)+" MinMHz='1234'"),platform);
 foreach(string s in new[]{"SOUNDFX","VOICE","MUSIC","AMBIENT","MOVIE","NONE","unknown","1",""})Test(original,2,"slider","",Node("VolumeSliderMultiplierForMovie",Attr("Slider",s)+" Multiplier='1.25'"),platform);
 foreach(string s in new[]{"LEFT","RIGHT","CENTER","unknown","left","2",""}){string text=Node("TextInstance",Attr("Justification",s)+" TextId='Alpha Beta' Font='Arial' PointSize='17'","<TextColor r='1' g='.5' b='0'/>");Test(original,4,"justification","",Node("EventList","",Node("TitleCard","TypeId='866596483' DisplayTime='1s'",text)),platform);}
 foreach(string s in new[]{"","A","A B"," A  B  A ","A&#x9;B","&amp; &lt; &quot;","é"}){
 var b=Test(original,4,"text-ids","",Node("EventList","",Node("TitleCard","TypeId='866596483' DisplayTime='100ms'",Node("TextInstance","TextId='"+s+"' Font='' PointSize='-2'"))),platform);
 if(s=="A B"){int list=(int)Word(b.InstanceData,8,big),title=(int)Word(b.InstanceData,list,big),texts=(int)Word(b.InstanceData,title+20,big);if(Word(b.InstanceData,title+16,big)!=1||Word(b.InstanceData,texts,big)!=2)throw new Exception("Independent title text counts");++layouts;}}
 foreach(string reference in new[]{"","none","Video:Name\\0","Video:Name\\1","Video:Name\\3tail","Video:Name\\-1","Video:Name\\2147483648","Video:Name\\99999999999","Video:Name\\1\\2"})Test(original,4,"movie reference","",Node("EventList","",Node("Movie","TypeId='3074724042' "+Attr("MovieAsset",reference))),platform);
 foreach(string id in new[]{"0","99","0xB74490CA","3074724042","866596483","-1","nonsense","0x33A73683"}){bool movie=id=="3074724042"||id=="0xB74490CA",title=id=="866596483"||id=="0x33A73683";Test(original,4,"event dispatch/"+id,"",Node("EventList","",Node("ArbitraryName","TypeId='"+id+"' "+(movie?"MovieAsset='Video:Name\\1'":title?"DisplayTime='125ms'":""),Fade("FadeIn",3)+(title?TextNode(4):""))),platform);}
 foreach(int count in new[]{0,1,2,3,7,28,29,30,31,32,65}){
 var processors=new StringBuilder();var sliders=new StringBuilder();var events=new StringBuilder();for(int i=0;i<count;++i){processors.Append(Node("Processor","Type='CORE_DUO' MinMHz='"+(1000+i)+"'"));sliders.Append(Node("VolumeSliderMultiplierForMovie","Slider='MOVIE' Multiplier='"+i+".5'"));events.Append(i%3==0?Movie(i):i%3==1?Title(i):Node("UnknownEvent","TypeId='999'",Fade("FadeOut",i)));}
 var p=Test(original,1,"processor length/"+count,"Level='HIGH' AllowReverb='true'",processors.ToString(),platform);if(Word(p.InstanceData,32,big)!=count)throw new Exception("Processor list count");
 Test(original,2,"slider length/"+count,"",sliders.ToString(),platform);var e=Test(original,4,"event length/"+count,"",Node("EventList","",events.ToString()),platform);if(Word(e.InstanceData,4,big)!=count)throw new Exception("Event list count");layouts+=2;}
 for(int i=0;i<256;++i){Test(original,0,"limit mixed/"+i,"Limit='"+(i%2==0?-i:i).ToString()+"'","",platform);Test(original,1,"lod mixed/"+i,"Level='HIGH' MaximumAmbientStreams='"+i+"' MaxVoices='"+(i*3)+"' OutputBitRate='44100' MinimumLogicalProcessors='2' AllowReverb='true' AllowCPULimiter='1' AllowVolumeCompressor='false' CPULimiterLevel='75.1%' CPULimiterLevelDebug='25.5'",Node("Processor","Type='CORE_DUO' MinMHz='3200'")+Node("Processor","Type='ATHLON_64' MinMHz='2100'"),platform);Test(original,2,"settings mixed/"+i,"StreamBufferSizePerChannel='32768' MaxRequestsPerStream='7' ReadAheadTime='125ms' DefaultVoiceVolume='25.25' MinVolumeBeforeFade='7' MaxVolumeBeforeFade='41' SuppressOcclusion='true' PlayEnemySightedEventsOnlyFromNearbyUnits='true'",AudioSettingsChildren(i),platform);Test(original,3,"video mixed/"+i,"Filename='movies/intro"+i+".vp6' Comment='Oracle &amp; Test' Volume='45.125' SoundOverride='NO_DUCK'","",platform);Test(original,4,"sequence mixed/"+i,"",Node("EventList","",Movie(i)+Title(i)),platform);}
 // Reject unsupported inputs at every structural level while retaining the original numeric unknown-event base fallback.
 Reject(4,Xml("VideoEventList","","<EventList/><EventList/>"));Reject(4,Xml("VideoEventList","","<EventList Unknown='1'/>"));Reject(4,Xml("VideoEventList","","<EventList><Movie TypeId='3074724042' Bogus='1'/></EventList>"));Reject(4,Xml("VideoEventList","","<EventList><Unknown TypeId='9' MovieAsset='x'/></EventList>"));Reject(4,Xml("VideoEventList","","<EventList><TitleCard TypeId='866596483'><TextInstance Unknown='1'/></TitleCard></EventList>"));Reject(4,Xml("VideoEventList","","<EventList><TitleCard TypeId='866596483'><TextInstance><TextColor R='1'/></TextInstance></TitleCard></EventList>"));
 foreach(string child in new[]{"TacticalMicSettings","LivingWorldMicSettings","ShellMicSettings","VolumeCompressorSettings","AutomaticSubtitleWindowColor","AutomaticSubtitleTextColor"}){Reject(2,Xml("AudioSettings","",Node(child,"Unknown='x'")));Reject(2,Xml("AudioSettings","",Node(child,"")+Node(child,"")));Reject(2,Xml("AudioSettings","",Node(child,"","<Unknown/>")));}
 Reject(2,Xml("AudioSettings","","<VolumeSliderMultiplierForMovie Unknown='1'/>"));Reject(1,Xml("AudioLOD","","<Processor Unknown='1'/>"));Reject(1,Xml("AudioLOD","","<Processor><Unknown/></Processor>"));Reject(4,Xml("VideoEventList","","<EventList><Movie TypeId='3074724042'><FadeIn/><FadeIn/></Movie></EventList>"));Reject(4,Xml("VideoEventList","","<EventList><Movie TypeId='3074724042'><FadeIn><StartColor/><StartColor/></FadeIn></Movie></EventList>"));
 }
}

