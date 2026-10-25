using System;
using System.Text;
using BinaryAssetBuilder.Core;
partial class XmlfxparticleOracle {
    static string EventItem(uint id,string attrs,string children) {
        return "<EventItem "+Attr("TypeId",id.ToString(System.Globalization.CultureInfo.InvariantCulture))+attrs+">"+children+"</EventItem>";
    }
    static void EventCompletionMatrix(IAssetBuilderPlugin original,TargetPlatform p) {
        uint[] ids={2693838876u,1075723135u,3465979181u};
        foreach(uint id in ids) {
            foreach(string wrapper in new string[]{"Physics","Draw","Volume","Velocity","Update"})
                Test(original,p,"event-singleton-"+wrapper+"-"+id,Xml("",Wrap(wrapper,id,"","")));
            Test(original,p,"event-list-"+id,Xml("","<Event>"+EventItem(id,"","")+"</Event>"));
            foreach(string v in Refs) {
                Test(original,p,"event-reference-"+id+"-"+v,Xml("",Wrap("Physics",id,Attr("EventFX",v),"")));
                Test(original,p,"event-list-reference-"+id+"-"+v,Xml("","<Event>"+EventItem(id,Attr("EventFX",v),"")+"</Event>"));
            }
            foreach(string field in new string[]{"PerParticle","KillAfterEvent"})foreach(string v in Bools)
                Test(original,p,"event-bool-"+id+"-"+field+"-"+v,Xml("",Wrap("Draw",id,Attr(field,v),"")));
            if(id==1075723135u)foreach(string v in Bools)
                Test(original,p,"collision-orient-"+v,Xml("",Wrap("Velocity",id,Attr("OrientFXToTerrain",v),"")));
            string child=id==1075723135u?"HeightOffset":id==3465979181u?"EventTime":null;
            if(child!=null) {
                Test(original,p,"event-empty-random-"+id,Xml("",Wrap("Update",id,"","<"+child+"/>")));
                foreach(string v in Numbers) {
                    Test(original,p,"event-random-"+id+"-"+v,Xml("",Wrap("Update",id,"",Range(child,v))));
                    Test(original,p,"event-list-random-"+id+"-"+v,Xml("","<Event>"+EventItem(id,Attr("EventFX","Name\\27"),Range(child,v))+"</Event>"));
                }
                foreach(string type in new string[]{"UNIFORM","CONSTANT","GAUSSIAN","UNKNOWN","uniform",""})
                    Test(original,p,"event-random-type-"+id+"-"+type,Xml("","<Event>"+EventItem(id,"","<"+child+" "+Attr("Type",type)+" Low='-2' High='3'/>")+"</Event>"));
                Test(original,p,"event-partial-random-"+id,Xml("","<Event>"+EventItem(id,"","<"+child+" Low='2.25'/>")+"</Event>"));
                Reject(Xml("",Wrap("Draw",id,"","<"+child+"/><"+child+"/>")),p);
                Reject(Xml("",Wrap("Draw",id,"","<"+child+" Other='1'/>")),p);
            }
            Reject(Xml("","<Event>"+EventItem(id,"Unknown='1'","")+"</Event>"),p);
            Reject(Xml("","<Event>"+EventItem(id,"","<Unknown/>")+"</Event>"),p);
        }
        foreach(int n in new int[]{0,1,2,27,28,29,64,257}) {
            StringBuilder body=new StringBuilder("<Event>");
            for(int i=0;i<n;++i)body.Append(EventItem(ids[i%3],Attr("EventFX","Name\\"+i)+Attr("PerParticle",i%2==0?"true":"false")+Attr("KillAfterEvent",i%3==0?"true":"false"),i%3==1?Range("HeightOffset","-2.75"):i%3==2?Range("EventTime","12.5"):""));
            body.Append("</Event>");
            Test(original,p,"event-list-cardinality-"+n,Xml("ParticleTexture='Name\\7' EmitterSound='Name\\8'",Wrap("Physics",3110487039u,"Gravity='1'","")+Wrap("Draw",2381754661u,"EnvironmentTexture='Name\\9'","")+body+Wrap("Update",3465979181u,"EventFX='Name\\10'",Range("EventTime","4"))));
        }
        foreach(uint id in new uint[]{877888681u,3201797789u,3809157760u,2268785311u,1903056374u,612354849u,3543124331u,1423381183u,1373300932u,1865622437u,2381754661u,3110487039u,1976375475u,3992468329u,1242273524u,1552576278u,795809322u,228810652u,3772792373u,2343798068u,3082112962u,3476018911u,4058790702u,3880943874u,3975650247u,2933826200u,234581578u,2248017887u,3036920908u,3408220251u,0u,99u,4294967295u})
            Test(original,p,"event-cross-category-"+id,Xml("","<Event>"+EventItem(id,"","")+"</Event>"));
        Test(original,p,"event-missing-type",Xml("","<Event><Anything/></Event>"));
        Test(original,p,"event-comments",Xml("","<Event> <!-- comment --> "+EventItem(2693838876u,"","")+" </Event>"));
        foreach(string children in new string[]{"<Event Unknown='1'/>","<Event><Anything xmlns='urn:foreign'/></Event>","<Event>text</Event>","<Event/><Event/>","<Event><A TypeId='2693838876' xmlns:q='urn:other' q:EventFX='x'/></Event>"})Reject(Xml("",children),p);
    }
}
