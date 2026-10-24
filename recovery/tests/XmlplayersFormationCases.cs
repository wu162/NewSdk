using System;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
partial class XmlplayersOracle {
    static string FormationGrid(int rows,int cells,int definitions){var b=new StringBuilder();for(int r=0;r<rows;++r){b.Append("<Row>");for(int c=0;c<cells;++c){b.Append("<Cell>");for(int d=0;d<definitions;++d)b.Append("<Definition>U\\").Append(d%7).Append("</Definition>");b.Append("</Cell>");}b.Append("</Row>");}return b.ToString();}
    static void FormationMatrix(IAssetBuilderPlugin o,TargetPlatform p){
        foreach(string type in new[]{"FormationAssistantUnitDefinition","FormationAssistantTemplate","FormationSelectionTemplate","FormationAssistant"}){
            var b=Test(o,p,"formation-default-"+type,Root(type,"",""));Assert(b.InstanceData.Length==RootSize(type),"independent formation default root");
            Reject(Root(type,"Bad='1'",""),p);Reject(Root(type,"xmlns:q='urn:other' q:TypeId='1'",""),p);Reject(Root(type,"","<Unknown/>"),p);Reject(Root(type,"","text"),p);
        }
        foreach(string name in new[]{"DefaultPreviewObject","FacingArrowTemplateHead","FacingArrowTemplateBody","FacingArrowTemplateBase"})foreach(string value in Strings)Test(o,p,"formation-hash-"+name+"-"+value,Root("FormationAssistant",A(name,value),""));
        foreach(string name in new[]{"RowPadding","ColumnPadding","ActivateDragDistance","ActivateTime"})foreach(string value in Numbers)Test(o,p,"formation-scalar-"+name+"-"+value,Root("FormationAssistant",A(name,value),""));
        foreach(string value in Refs)Test(o,p,"selection-ref-"+value,Root("FormationSelectionTemplate",A("FormationAssistantTemplate",value),""));
        foreach(string name in new[]{"MaxDragLength","MaxUnitsSelected"})foreach(string value in Numbers)Test(o,p,"selection-number-"+name+"-"+value,Root("FormationSelectionTemplate",A(name,value),""));
        foreach(string value in Strings)Test(o,p,"formation-preview-"+value,Root("FormationAssistantUnitDefinition","","<PreviewObject>"+E(value)+"</PreviewObject>"));
        foreach(string name in new[]{"IncludeObjectFilter","ExcludeObjectFilter","ObjectFilter"})foreach(string attr in new[]{"Rule","Relationship","Alignment","Include","Exclude","StatusBitFlags","StatusBitFlagsExclude"})foreach(string value in new[]{"","ANY","ALL","unknown","INFANTRY VEHICLE","STEALTHED UNDER_CONSTRUCTION","ALLIES SAME_PLAYER","GOOD EVIL","ALL -ALL"}){
            string type=name=="ObjectFilter"?"FormationAssistant":"FormationAssistantUnitDefinition";
            Test(o,p,"formation-filter-"+name+"-"+attr+"-"+value,Root(type,"","<"+name+" "+A(attr,value)+">"+FilterBody+"</"+name+">"));
        }
        foreach(int count in new[]{0,1,2,3,27,28,29,64,257}){
            var filters=new StringBuilder();for(int i=0;i<count;++i)filters.Append("<IncludeObjectFilter StatusBitFlags='STEALTHED'>").Append(FilterBody).Append("</IncludeObjectFilter><ExcludeObjectFilter Include='STRUCTURE'>").Append(FilterBody).Append("</ExcludeObjectFilter>");
            Test(o,p,"formation-filter-lists-"+count,Root("FormationAssistantUnitDefinition","",filters.ToString()+"<PreviewObject>Preview</PreviewObject>"));
            Test(o,p,"formation-rows-"+count,Root("FormationAssistantTemplate","",FormationGrid(count,2,3)));
            Test(o,p,"formation-cells-"+count,Root("FormationAssistantTemplate","",FormationGrid(2,count,3)));
            Test(o,p,"formation-defs-"+count,Root("FormationAssistantTemplate","",FormationGrid(2,3,count)));
        }
        bool big=p!=TargetPlatform.Win32;
        var u=Test(o,p,"ABI-FormationUnit",Root("FormationAssistantUnitDefinition","","<IncludeObjectFilter/><IncludeObjectFilter/><ExcludeObjectFilter/><PreviewObject>Preview</PreviewObject>"));
        Assert(u.InstanceData.Length==364&&Word(u.InstanceData,4,big)==2&&Word(u.InstanceData,8,big)==24&&Word(u.InstanceData,12,big)==1&&Word(u.InstanceData,16,big)==248&&Word(u.InstanceData,20,big)==360,"filter contiguous list blocks precede preview pointer");
        var t=Test(o,p,"ABI-FormationTemplate",Root("FormationAssistantTemplate","","<Row><Cell><Definition>U\\1</Definition></Cell><Cell><Definition>U\\2</Definition><Definition>U\\3</Definition></Cell></Row>"));
        Assert(t.InstanceData.Length==48&&Word(t.InstanceData,4,big)==1&&Word(t.InstanceData,8,big)==12&&Word(t.InstanceData,12,big)==2&&Word(t.InstanceData,16,big)==20&&Word(t.InstanceData,24,big)==36&&Word(t.InstanceData,32,big)==40,"row/cell/definition nested allocation order");Assert(t.ImportsData.Length==16&&Word(t.ImportsData,0,big)==36&&Word(t.ImportsData,4,big)==40&&Word(t.ImportsData,8,big)==44,"formation definition imports exact offsets");
        var a=Test(o,p,"ABI-FormationAssistant",Root("FormationAssistant","RowPadding='2.5' ColumnPadding='-3.25' ActivateDragDistance='10' ActivateTime='4'","<ObjectFilter/>"));Assert(a.InstanceData.Length==152&&Word(a.InstanceData,20,big)==0x40200000u&&Word(a.InstanceData,36,big)==40,"FormationAssistant float/filter ABI");
        var select=Test(o,p,"ABI-FormationSelection",Root("FormationSelectionTemplate","FormationAssistantTemplate='F\\7' MaxDragLength='2.5' MaxUnitsSelected='0xffffffff'",""));Assert(select.InstanceData.Length==16&&Word(select.InstanceData,8,big)==0x40200000u&&Word(select.InstanceData,12,big)==UInt32.MaxValue&&select.ImportsData.Length==8&&Word(select.ImportsData,0,big)==4,"Selection reference/scalars ABI");
        foreach(string xml in new[]{Root("FormationAssistantUnitDefinition","","<PreviewObject/><PreviewObject/>"),Root("FormationAssistantUnitDefinition","","<PreviewObject Bad='1'/>"),Root("FormationAssistantUnitDefinition","","<IncludeObjectFilter><IncludeThing><Nested/></IncludeThing></IncludeObjectFilter>"),Root("FormationAssistant","","<ObjectFilter/><ObjectFilter/>"),Root("FormationAssistantTemplate","","<Row Bad='1'/>"),Root("FormationAssistantTemplate","","<Row><Cell Bad='1'/></Row>"),Root("FormationAssistantTemplate","","<Row><Cell><Definition Bad='1'>U</Definition></Cell></Row>"),Root("FormationAssistantTemplate","","<Row><Cell><Unknown/></Cell></Row>")})Reject(xml,p);
        foreach(string source in new[]{Root("FormationAssistantTemplate","",FormationGrid(2,2,2)),Root("FormationAssistantUnitDefinition","","<IncludeObjectFilter/>"),Root("FormationAssistant","RowPadding='2'","<ObjectFilter/>"),Root("FormationSelectionTemplate","MaxUnitsSelected='3'","")})foreach(string value in new[]{"0","4294967295","metadata",""}){var d=Parse(source);foreach(XmlElement n in d.SelectNodes("//*"))n.SetAttribute("TypeId",value);TestDoc(o,p,"formation-BAB-metadata",d);}
    }
}
