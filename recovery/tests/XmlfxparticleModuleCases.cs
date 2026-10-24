using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlfxparticleOracle {
    static int ParticleAlign(int value) { return (value+3)&~3; }
    static int ModuleCompletionLayout(string label,XmlNode module,uint id,int start,AssetBuffer b,bool big,List<int> locations,List<int> targets,List<int> imports) {
        int end=start+ModuleSize(id);
        if(id==2248017887u) {
            string shader=module.Attributes["Shader"]==null?"GPUParticle.fx":module.Attributes["Shader"].Value;
            // Adapter performs the same native narrow-string conversion as the original node adapter.
            int bytes=Encoding.UTF8.GetByteCount(shader);
            Check(Word(b.InstanceData,start+4,big)==bytes,label+" gpu shader count");
            locations.Add(start+8); targets.Add(end); end=ParticleAlign(end+bytes+1);
            if(module.Attributes["DetailTexture"]!=null&&Imported(module.Attributes["DetailTexture"].Value)) imports.Add(start+28);
            Check(b.InstanceData[start+58]==0&&b.InstanceData[start+59]==0,label+" gpu padding");
        } else if(id==3036920908u) {
            int count=0; foreach(XmlNode child in module.ChildNodes)if(child.NodeType==XmlNodeType.Element&&child.LocalName=="RandomSet")++count;
            Check(Word(b.InstanceData,start+28,big)==count,label+" lightning count");
            if(count>0) { locations.Add(start+32); targets.Add(end);end+=72*count; }
        } else if(id==3408220251u) {
            List<XmlNode> children=new List<XmlNode>();
            foreach(XmlNode child in module.ChildNodes)if(child.NodeType==XmlNodeType.Element&&child.LocalName=="ObjectSet")children.Add(child);
            Check(Word(b.InstanceData,start+8,big)==children.Count,label+" object count");
            Check(b.InstanceData[start+18]==0&&b.InstanceData[start+19]==0,label+" object padding");
            if(children.Count>0) {
                locations.Add(start+12);targets.Add(end);int list=end;end+=16*children.Count;
                for(int i=0;i<children.Count;++i) {
                    XmlAttribute group=children[i].Attributes["RenderGroup"];
                    if(group!=null) {
                        int bytes=Encoding.UTF8.GetByteCount(group.Value);
                        Check(Word(b.InstanceData,list+16*i,big)==bytes,label+" render group count");
                        locations.Add(list+16*i+4);targets.Add(end);end=ParticleAlign(end+bytes+1);
                    } else Check(Word(b.InstanceData,list+16*i,big)==0&&Word(b.InstanceData,list+16*i+4,big)==0,label+" absent render group");
                }
            }
        }
        return end;
    }
    static string ModuleRandomNode(string name,string type,string low,string high) {
        return "<"+name+" "+Attr("Type",type)+Attr("Low",low)+Attr("High",high)+"/>";
    }
    static void ModuleCompletionMatrix(IAssetBuilderPlugin original,TargetPlatform p) {
        uint[] ids={2933826200u,234581578u,2248017887u,3036920908u,3408220251u};
        foreach(uint id in ids) foreach(string wrapper in new string[]{"Physics","Draw","Volume","Velocity","Update"})
            Test(original,p,"completion-default-"+id+"-"+wrapper,Xml("",Wrap(wrapper,id,"","")));
        foreach(uint id in new uint[]{2933826200u,234581578u})
            foreach(string value in new string[]{"INVALID_ROTATION","ROTATION_OFF","ROTATE_AROUND_VELOCITY","ROTATE_IN_WORLD_SPACE","","unknown","rotation_off","1"})
                Test(original,p,"rotation-"+id+"-"+value,Xml("",Wrap("Update",id,Attr("Rotation",value),"")));
        string[] simple={"SizeRate","SizeRateDamping","AngleZ","AngularRateZ","AngularDamping","AngleXY","AngularRateXY","AngularDampingXY"};
        string[] rendered={"AngleX","AngularRateX","AngleY","AngularRateY","AngleZ","AngularRateZ","AngularDamping"};
        foreach(uint id in new uint[]{2933826200u,234581578u}) {
            string[] names=id==2933826200u?simple:rendered;
            foreach(string name in names) {
                foreach(string value in Numbers)
                    Test(original,p,"update-random-"+id+"-"+name+"-"+value,Xml("",Wrap("Update",id,"",ModuleRandomNode(name,"UNIFORM",value,value))));
                foreach(string type in new string[]{"CONSTANT","UNIFORM","GAUSSIAN","TRIANGULAR","LOW_BIAS","HIGH_BIAS","unknown","","uniform"})
                    Test(original,p,"update-dist-"+id+"-"+name+"-"+type,Xml("",Wrap("Draw",id,"",ModuleRandomNode(name,type,"-3.25","9.5"))));
                foreach(string attrs in new string[]{"","Low='2.5'","High='-3'","Type='CONSTANT'","Low='' High=''","Low='4' Type='GAUSSIAN'"})
                    Test(original,p,"update-partial-"+id+"-"+name+"-"+attrs,Xml("",Wrap("Update",id,"","<"+name+" "+attrs+"/>")));
                Reject(Xml("",Wrap("Update",id,"","<"+name+"/><"+name+"/>")),p);
                Reject(Xml("",Wrap("Update",id,"","<"+name+" Bad='1'/>")),p);
            }
        }
        foreach(string name in new string[]{"StartSize","SizeRate","SizeDamping"}) {
            Test(original,p,"coord-empty-"+name,Xml("",Wrap("Update",234581578u,"","<"+name+"/>")));
            foreach(string axis in new string[]{"x","y","z"}) foreach(string value in Numbers)
                Test(original,p,"coord-"+name+"-"+axis+"-"+value,Xml("",Wrap("Update",234581578u,"","<"+name+">"+ModuleRandomNode(axis,"GAUSSIAN",value,value)+"</"+name+">")));
            Test(original,p,"coord-all-"+name,Xml("",Wrap("Velocity",234581578u,"","<"+name+"><x Low='1' High='2'/><y Low='3' High='4'/><z Low='5' High='6'/></"+name+">")));
            Reject(Xml("",Wrap("Update",234581578u,"","<"+name+"><x/><x/></"+name+">")),p);
            Reject(Xml("",Wrap("Update",234581578u,"","<"+name+"><q/></"+name+">")),p);
        }
        foreach(string field in new string[]{"FramesPerRow","TotalFrames","SingleRow","RandTex","SpeedMultiplier","MiscValue1","MiscValue2","MiscValue3","MiscValue4"})
            foreach(string value in Numbers)Test(original,p,"gpu-"+field+"-"+value,Xml("",Wrap("Draw",2248017887u,Attr(field,value),"")));
        foreach(string value in new string[]{"","a","abc","abcd","abcde","GPUParticle.fx","A&B<shader>","shader\\7"})
            Test(original,p,"gpu-string-"+value,Xml("",Wrap("Draw",2248017887u,Attr("Shader",value),"")));
        foreach(string value in Refs)Test(original,p,"gpu-reference-"+value,Xml("",Wrap("Draw",2248017887u,Attr("DetailTexture",value),"")));
        foreach(string field in new string[]{"SortParticles","EnableSoftParticles"})
            foreach(string value in Bools)Test(original,p,"gpu-bool-"+field+"-"+value,Xml("",Wrap("Draw",2248017887u,Attr(field,value),"")));
        foreach(string value in new string[]{"INVALID","SIMPLE_QUAD","CENTERED_QUAD","TWO_CONCENTRIC_QUADS","TWO_QUADS","unknown","","simple_quad"})
            Test(original,p,"gpu-geometry-"+value,Xml("",Wrap("Draw",2248017887u,Attr("GeometryType",value),"")));
        foreach(string name in new string[]{"StartPoint","EndPoint"})foreach(string axis in new string[]{"x","y","z"})foreach(string value in Numbers)
            Test(original,p,"lightning-vector-"+name+"-"+axis+"-"+value,Xml("",Wrap("Draw",3036920908u,"","<"+name+" "+Attr(axis,value)+"/>")));
        string[] lightning={"StartAmplitude","EndAmplitudeChange","StartFrequency","EndFrequencyChange","StartPhase","EndPhaseChange"};
        foreach(string name in lightning) {
            foreach(string value in Numbers)Test(original,p,"lightning-random-"+name+"-"+value,Xml("",Wrap("Draw",3036920908u,"","<RandomSet>"+ModuleRandomNode(name,"HIGH_BIAS",value,value)+"</RandomSet>")));
            foreach(string type in new string[]{"CONSTANT","UNIFORM","GAUSSIAN","TRIANGULAR","LOW_BIAS","HIGH_BIAS","unknown"})
                Test(original,p,"lightning-dist-"+name+"-"+type,Xml("",Wrap("Draw",3036920908u,"","<RandomSet>"+ModuleRandomNode(name,type,"-5","7")+"</RandomSet>")));
        }
        foreach(int count in new int[]{0,1,2,3,27,28,29,64,257}) {
            StringBuilder random=new StringBuilder(),objects=new StringBuilder();
            for(int i=0;i<count;++i) {
                random.Append("<RandomSet>");foreach(string name in lightning)random.Append(ModuleRandomNode(name,"UNIFORM",i.ToString(CultureInfo.InvariantCulture),"2.75"));random.Append("</RandomSet>");
                objects.Append("<ObjectSet "+Attr("RenderGroup",i%3==0?"":("Group"+i))+Attr("NumObjects",i.ToString(CultureInfo.InvariantCulture))+Attr("Percent","-12.5%")+"/>");
            }
            Test(original,p,"lightning-list-"+count,Xml("",Wrap("Draw",3036920908u,"",random.ToString())));
            Test(original,p,"object-list-"+count,Xml("",Wrap("Draw",3408220251u,"",objects.ToString())));
        }
        foreach(string field in new string[]{"SinkRate"})foreach(string value in Numbers)Test(original,p,"render-"+field+"-"+value,Xml("",Wrap("Draw",3408220251u,Attr(field,value),"")));
        foreach(string field in new string[]{"MultiRenderObjects","SinkOnTerrainCollision"})foreach(string value in Bools)Test(original,p,"render-bool-"+field+"-"+value,Xml("",Wrap("Draw",3408220251u,Attr(field,value),"")));
        foreach(string field in new string[]{"NumObjects","Percent"})foreach(string value in Numbers)Test(original,p,"object-"+field+"-"+value,Xml("",Wrap("Draw",3408220251u,"","<ObjectSet "+Attr(field,value)+"/>")));
        foreach(string value in new string[]{"","a","abc","abcd","abcde","Model\\7","A&B<model>"})Test(original,p,"object-string-"+value,Xml("",Wrap("Draw",3408220251u,"","<ObjectSet "+Attr("RenderGroup",value)+"/><ObjectSet/>")));
        foreach(uint id in ids) {
            Reject(Xml("",Wrap("Draw",id,"Bad='1'","")),p);
            Reject(Xml("",Wrap("Draw",id,"","<Unknown/>")),p);
            Reject(Xml("",Wrap("Draw",id,"","<x xmlns='foreign'/>")),p);
        }
        Reject(Xml("",Wrap("Draw",3036920908u,"","<StartPoint/><StartPoint/>")),p);
        Reject(Xml("",Wrap("Draw",3036920908u,"","<RandomSet><StartPhase/><StartPhase/></RandomSet>")),p);
        Reject(Xml("",Wrap("Draw",3408220251u,"","<ObjectSet><Unknown/></ObjectSet>")),p);
        Reject(Xml("",Wrap("Draw",3408220251u,"","<ObjectSet Bad='1'/>")),p);
    }
}
