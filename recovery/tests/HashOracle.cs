using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using EALA.Hash;

class HashOracle
{
    static List<string> rows = new List<string>();
    static void Add(string id, Func<uint> action)
    {
        try { rows.Add(id + ":" + action().ToString("x8", CultureInfo.InvariantCulture)); }
        catch (Exception e) { rows.Add(id + ":!" + e.GetType().FullName); }
    }
    static int Main(string[] args)
    {
        // Public identity excludes compiler-generated CRT/module scaffolding.
        Type t = typeof(FastHash);
        if (!t.IsPublic || t.IsSealed || t.IsAbstract || t.GetConstructor(Type.EmptyTypes) == null)
            throw new Exception("Public type contract changed");
        new FastHash();
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        if (methods.Length != 6) throw new Exception("Expected six static overloads");
        List<string> api = new List<string>();
        foreach (MethodInfo m in methods) api.Add(m.ToString());
        api.Sort(StringComparer.Ordinal); rows.AddRange(api);
        uint[] seeds = { 0, 1, 0x7fffffff, 0x80000000, 0xffffffff, 0x12345678 };
        foreach (string culture in new[] { "en-US", "de-DE", "tr-TR" })
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            string[] texts = { null, "", "a", "ab", "abc", "abcd", "abcde", "GameObject", "gameobject",
                "INSTANCEID", "A\0B", "\0", "éÿ", "你好", "İıI", "😀", "\ud800", new string('x',4097) };
            for (int i=0;i<texts.Length;i++)
            {
                string text=texts[i]; string id=culture+"/s/"+i;
                Add(id, delegate { return FastHash.GetHashCode(text); });
                foreach (uint seed in seeds)
                { uint h=seed; Add(id+"/"+h, delegate { return FastHash.GetHashCode(h,text); }); }
            }
        }
        Random random = new Random(137);
        int[] sizes = {1,2,3,4,5,6,7,8,15,16,17,31,32,63,64,255,256,257,1023,4097,65536};
        foreach (int size in sizes)
        for (int pattern=0;pattern<6;pattern++)
        {
            byte[] data = new byte[size]; random.NextBytes(data);
            for (int i=0;i<size;i++)
                if (pattern<5) data[i] = pattern==0 ? (byte)0 : pattern==1 ? (byte)255 : pattern==2 ? (byte)128 : pattern==3 ? (byte)127 : (byte)i;
            string id="b/"+size+"/"+pattern;
            Add(id, delegate { return FastHash.GetHashCode(data); });
            List<int> lengths = new List<int>();
            for (int n=0;n<=Math.Min(size,20);n++) lengths.Add(n);
            if(size>20) {lengths.Add(size-3);lengths.Add(size-2);lengths.Add(size-1);lengths.Add(size);}
            foreach(int n in lengths)
            {
                int count=n;
                Add(id+"/n/"+n,delegate { return FastHash.GetHashCode(data,count); });
                foreach(uint seed in seeds)
                { uint h=seed; Add(id+"/n/"+n+"/h/"+h, delegate {return FastHash.GetHashCode(h,data,count);}); }
            }
            foreach(uint seed in seeds)
            {uint h=seed;Add(id+"/h/"+h,delegate{return FastHash.GetHashCode(h,data);});}
        }
        // Exhaustive signed-byte tails, including every two-byte value.
        for(int n=0;n<65536;n++)
        {
            byte[] data={(byte)n,(byte)(n>>8)};
            Add("pair/"+n,delegate{return FastHash.GetHashCode(data);});
        }
        for(int n=0;n<256;n++)
        {
            byte[] one={(byte)n}; byte[] three={0x81,0xfe,(byte)n};
            foreach(uint seed in seeds)
            { uint h=seed; Add("tail1/"+n+"/"+h,delegate{return FastHash.GetHashCode(h,one);});
              Add("tail3/"+n+"/"+h,delegate{return FastHash.GetHashCode(h,three);}); }
        }
        foreach(byte[] data in new byte[][] {null,new byte[0]})
        {
            string id=data==null ? "null" : "empty";
            Add(id,delegate{return FastHash.GetHashCode(data);});
            Add(id+"/h",delegate{return FastHash.GetHashCode(123u,data);});
            foreach(int n in new[]{-1,0,1})
            {int count=n;Add(id+"/n/"+n,delegate{return FastHash.GetHashCode(data,count);});
             Add(id+"/hn/"+n,delegate{return FastHash.GetHashCode(123u,data,count);});}
        }
        // Do not execute undefined native pointer reads in the original.
        if (args.Length > 1 && args[1] == "recovered")
        {
            foreach(int n in new[]{-1,2,int.MinValue,int.MaxValue})
            {
                try { FastHash.GetHashCode(0,new byte[1],n); throw new Exception("Unsafe read accepted"); }
                catch(ArgumentOutOfRangeException) { }
                try { FastHash.GetHashCode(new byte[1],n); throw new Exception("Unsafe read accepted"); }
                catch(ArgumentOutOfRangeException) { }
            }
        }
        File.WriteAllText(args[0],new JavaScriptSerializer { MaxJsonLength=int.MaxValue }.Serialize(rows));
        Console.WriteLine("PASS "+rows.Count+" differential rows");
        return 0;
    }
}
