using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
// Original-only investigation. This does not enable recovered WeaponSetUpdate.
class WeaponSlotSortProbe {
    static InstanceDeclaration Declaration(string children) {
        var xml=new XmlDocument();xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='SlotSortProbe'>"+children+"</GameObject>");
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");var doc=new AssetDeclarationDocument();var st=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);var state=FormatterServices.GetUninitializedObject(st);st.GetField("NamespaceManager").SetValue(state,ns);typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);var d=new InstanceDeclaration(doc);d.XmlNode=xml.DocumentElement;return d;
    }
    static uint Word(byte[] b,int at,bool big) {return big?((uint)b[at]<<24)|((uint)b[at+1]<<16)|((uint)b[at+2]<<8)|b[at+3]:BitConverter.ToUInt32(b,at);}
    static string Hash(byte[] data) {using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
    static int Main(string[] args) {
        try {
            var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[0]).GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            string[] names={"PRIMARY_WEAPON","SECONDARY_WEAPON","TERTIARY_WEAPON","QUATERNARY_WEAPON","QUINARY_WEAPON","NO_WEAPON"};
            string[] slots={"WeaponSlotHardpoint","WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"};int fixtures=0;var lines=new List<string>();lines.Add("platform\tslot\tcount\tpattern\tkeys\toriginalIndices\tinstanceLength\trelocationLength\timportsLength\tinstanceSha256\trelocationSha256\timportsSha256");
            foreach(TargetPlatform p in new[]{TargetPlatform.Win32,TargetPlatform.Xbox360,TargetPlatform.PlayStation3,(TargetPlatform)99}) {
                original.Initialize(null,p);bool big=p!=TargetPlatform.Win32;
                for(int group=0;group<4;++group)foreach(int count in new[]{0,1,2,27,28,29,65,129,257})for(int pattern=0;pattern<5;++pattern) {
                    int[] keys=new int[count];var xml=new StringBuilder();xml.Append("<Behaviors><ObjectSMCHelper TypeId='1125589673' id='Sort'><"+slots[group]+" ID='23'>");uint random=1234567;
                    for(int i=0;i<count;++i) {random=unchecked(1664525*random+1013904223);int key=pattern==0?i*6/Math.Max(1,count):pattern==1?5-i*6/Math.Max(1,count):pattern==2?2:pattern==3?(int)(random%6):((count-1-i)/3)%6;keys[i]=key;xml.Append("<Weapon Ordering='"+names[key]+"' Template='W\\"+(1000+i)+"' Upgrade='U\\"+(10000+i)+"' ObjectStatus='DAMAGED' IsPlayerUpgradePermanent='"+(i%2==0?"true":"false")+"'/>");}
                    xml.Append("</"+slots[group]+"></ObjectSMCHelper></Behaviors>");var data=original.ProcessInstance(Declaration(xml.ToString()));byte[] b=data.InstanceData;int root=(int)Word(b,8,big)-4,header=(int)Word(b,root+468,big),module=(int)Word(b,(int)Word(b,header+4,big),big);int list=module+8+8*group,slot=(int)Word(b,list+4,big);if(Word(b,list,big)!=1 || Word(b,slot+16,big)!=count)throw new Exception("Slot/list layout");int at=(int)Word(b,slot+20,big),last=-1;var actualKeys=new List<int>();var indices=new List<int>();var seen=new HashSet<int>();
                    for(int i=0;i<count;++i) {int item=at+72*i,key=(int)Word(b,item,big),index=(int)Word(b,item+4,big)-1001;uint upgrade=Word(b,item+8,big);if(index<0 || index>=count || !seen.Add(index) || key!=keys[index] || key<last || upgrade!=10001+index || b[item+68]!=(index%2==0?1:0))throw new Exception("Sorted record/reference permutation platform="+(int)p+" slot="+group+" count="+count+" pattern="+pattern+" i="+i+" key="+key+" index="+index+" upgrade="+upgrade);last=key;actualKeys.Add(key);indices.Add(index);}
                    lines.Add((int)p+"\t"+slots[group]+"\t"+count+"\t"+pattern+"\t"+string.Join(",",actualKeys)+"\t"+string.Join(",",indices)+"\t"+b.Length+"\t"+data.RelocationData.Length+"\t"+data.ImportsData.Length+"\t"+Hash(b)+"\t"+Hash(data.RelocationData)+"\t"+Hash(data.ImportsData));++fixtures;
                }
            }
            File.WriteAllLines(args[1],lines,Encoding.UTF8);Console.WriteLine("PASS original-only WeaponSlot sorted permutation/reference pairing: fixtures="+fixtures+"; four slot types, counts0/1/2/27/28/29/65/129/257, five patterns, platforms0/1/2/99; recovered WeaponSetUpdate remains refused");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
