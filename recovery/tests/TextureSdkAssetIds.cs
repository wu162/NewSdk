using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using BinaryAssetBuilder.Core;

// Run beside the ORIGINAL SDK Core/EALA.Hash assemblies. Resolve collisions in
// the global INSTANCEID string bin, including names belonging to different roots.
internal static class TextureSdkAssetIds
{
    static uint Hash(string name) { return HashProvider.GetCaseInsensitiveSymbolHash(name); }
    static int Main(string[] args)
    {
        try
        {
            string path=Path.GetFullPath(args[0]);
            int expected=int.Parse(args[2],CultureInfo.InvariantCulture);
            if(Hash("BmpHeader_12000")!=0x7c4181ecU || Hash("BmpHeader_11012")!=0x7c4181ecU)
                throw new InvalidDataException("Original SDK hash no longer reproduces the recorded collision.");
            var document=new XmlDocument();document.Load(path);
            var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashes=new Dictionary<uint,string>();
            int count=0,collisions=0;
            using(var map=new StreamWriter(args[1]))
            {
                map.WriteLine("ordinal\ttype\toriginalName\tinstanceName\tinstanceHash\tattempts");
                foreach(XmlNode node in document.DocumentElement.ChildNodes)
                {
                    var element=node as XmlElement;
                    if(element==null || !element.HasAttribute("id"))continue;
                    string original=element.GetAttribute("id");
                    if(original.Length==0 || !names.Add(original))throw new InvalidDataException("Duplicate input asset name: "+original);
                    string candidate=original;int attempt=0;
                    while(hashes.ContainsKey(Hash(candidate)))
                    {
                        if(++attempt>100000)throw new InvalidDataException("Cannot allocate a unique SDK asset hash.");
                        candidate=original+"_h"+attempt.ToString("x",CultureInfo.InvariantCulture);
                    }
                    uint hash=Hash(candidate);hashes.Add(hash,candidate);
                    if(attempt!=0)collisions++;
                    element.SetAttribute("id",candidate);
                    map.WriteLine(count+"\t"+element.LocalName+"\t"+original+"\t"+candidate+"\t"+hash.ToString("x8",CultureInfo.InvariantCulture)+"\t"+attempt);
                    count++;
                }
            }
            if(count!=expected || hashes.Count!=expected)throw new InvalidDataException("Incomplete SDK ID manifest.");
            // Re-read every final name through Core, independently of the loop's
            // cached keys; a suffix may also be an existing asset's input name.
            var finalNames=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var finalHashes=new HashSet<uint>();
            foreach(XmlNode node in document.DocumentElement.ChildNodes)
            {
                var element=node as XmlElement;
                if(element==null || !element.HasAttribute("id"))continue;
                string name=element.GetAttribute("id");
                if(!finalNames.Add(name) || !finalHashes.Add(Hash(name)))throw new InvalidDataException("Final SDK ID collision.");
            }
            document.Save(path);
            Console.WriteLine("PASS original SDK INSTANCEID hash preflight assets="+count+" resolvedCollisions="+collisions+" uniqueNames="+finalNames.Count+" uniqueHashes="+finalHashes.Count);
            return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
