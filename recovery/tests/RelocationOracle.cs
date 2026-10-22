using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Recovered;

// The original DLL is used only here, as an independent oracle. The rebuilt
// runtime has no original-DLL dependency. Each DLL is tested in its own process.
unsafe class RelocationOracle
{
    sealed class Runtime : IDisposable
    {
        internal RelocationTracker rebuilt;
        internal IntPtr tracker, chunk, endian;
        MethodInfo[] methods;
        internal Runtime(Assembly original)
        {
            if (original == null) { rebuilt = new RelocationTracker(); return; }
            methods = original.GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            tracker = Marshal.AllocHGlobal(88); chunk = Marshal.AllocHGlobal(24); endian = Marshal.AllocHGlobal(1);
            for (int i=0; i<24; ++i) Marshal.WriteByte(chunk,i,0);
            Call("Relo.Tracker.{ctor}",tracker);
        }
        object Call(string name, params object[] args)
        {
            var m = methods.Single(x => x.Name == name && x.GetParameters().Length == args.Length);
            var p = m.GetParameters();
            for (int i=0; i<p.Length; ++i)
                if (p[i].ParameterType.IsPointer) args[i] = Pointer.Box(((IntPtr)args[i]).ToPointer(),p[i].ParameterType);
            return m.Invoke(null,args);
        }
        internal IntPtr Push(IntPtr field, uint size, uint count)
        {
            if (rebuilt != null) return rebuilt.Push(field,size,count);
            var p = Call("Relo.Tracker.Push",tracker,field,size,count);
            return new IntPtr(Pointer.Unbox(p));
        }
        internal IntPtr Root(uint size, uint count)
        {
            IntPtr result = IntPtr.Zero; Push(new IntPtr(&result),size,count); return result;
        }
        internal void Pop()
        {
            if (rebuilt != null) rebuilt.Pop();
            else Marshal.WriteInt32(tracker,8,Marshal.ReadInt32(tracker,8)-4); // original inline Pop
        }
        internal void Reference(IntPtr field, uint value)
        {
            if (rebuilt != null) rebuilt.AddReference(field,value);
            else Call("Relo.Tracker.AddReference",tracker,field,value);
        }
        internal bool Offset(IntPtr member, ref uint offset)
        {
            if (rebuilt != null) return rebuilt.GetMemberOffset(member,ref offset);
            fixed (uint* p = &offset) return (bool)Call("Relo.Tracker.GetMemberOffset",tracker,member,new IntPtr(p));
        }
        internal void Bookmark(IntPtr member, IntPtr field)
        {
            if (rebuilt != null) rebuilt.BookmarkMemberOffset(member,field);
            else Call("Relo.Tracker.BookmarkMemberOffset",tracker,member,field);
        }
        internal bool Exchange(IntPtr a, IntPtr b)
        {
            return rebuilt != null ? rebuilt.ExchangePointers(a,b) : (bool)Call("Relo.Tracker.ExchangePointers",tracker,a,b);
        }
        internal void Make(bool big, uint absolute)
        {
            if (rebuilt != null) rebuilt.MakeRelocatable(big,absolute);
            else { Marshal.WriteByte(endian,big ? (byte)1 : (byte)0); Call("Relo.Tracker.MakeRelocatable",tracker,chunk,endian,absolute); }
        }
        internal void Fix(Runtime owner, bool big)
        {
            if (rebuilt != null) rebuilt.FixupMemberOffsets(owner.rebuilt,big);
            else Call("Relo.Tracker.FixupMemberOffsets",tracker,owner.tracker,chunk,endian);
        }
        internal RelocationChunk Snapshot()
        {
            if (rebuilt != null) return rebuilt.Snapshot();
            var result = new RelocationChunk();
            result.Instance = Bytes(0); result.Relocations = Bytes(8); result.Imports = Bytes(16); return result;
        }
        byte[] Bytes(int offset)
        {
            var result = new byte[Marshal.ReadInt32(chunk,offset+4)];
            if (result.Length != 0) Marshal.Copy(Marshal.ReadIntPtr(chunk,offset),result,0,result.Length);
            return result;
        }
        public void Dispose()
        {
            if (rebuilt != null) { rebuilt.Dispose(); return; }
            Call("Relo.Chunk.{dtor}",chunk); Call("Relo.Tracker.{dtor}",tracker);
            Marshal.FreeHGlobal(tracker); Marshal.FreeHGlobal(chunk); Marshal.FreeHGlobal(endian);
        }
    }
    static IntPtr At(IntPtr p, int offset) { return new IntPtr(p.ToInt64()+offset); }
    static int comparisons;
    static void Equal(string name, byte[] expected, byte[] actual)
    {
        if (!expected.SequenceEqual(actual)) throw new Exception(name+" mismatch\nexpected "+BitConverter.ToString(expected)+"\nactual   "+BitConverter.ToString(actual));
        comparisons++;
    }
    static void Equal(string name, uint expected, uint actual)
    {
        if (expected != actual) throw new Exception(name+" expected "+expected+" actual "+actual); comparisons++;
    }
    static void Compare(string name, RelocationChunk a, RelocationChunk b)
    {
        Equal(name+" instance",a.Instance,b.Instance); Equal(name+" relocations",a.Relocations,b.Relocations); Equal(name+" imports",a.Imports,b.Imports);
        Console.WriteLine("PASS {0}: instance={1} relocations={2} imports={3}",name,a.Instance.Length,a.Relocations.Length,a.Imports.Length);
    }
    static RelocationChunk Graph(Assembly original, int seed, bool big, uint absolute)
    {
        using (var rt = new Runtime(original))
        {
            if (seed == 0) { rt.Make(big,absolute); return rt.Snapshot(); }
            IntPtr root = rt.Root(4,24); // pointer, reference, scalar slots
            Marshal.WriteInt32(root,92,0x12345678);
            var random = new Random(seed);
            for (int i=7; i>=0; --i)
            {
                IntPtr child = rt.Push(At(root,i*4),4,(uint)(random.Next(1,5)));
                rt.Reference(child,(uint)random.Next(0,8));
                rt.Pop();
            }
            for (int i=15; i>=8; --i) rt.Reference(At(root,i*4),(uint)(seed==1 ? 0 : random.Next(0,8)));
            // Allocate with no pointer location: still affects offsets/stack.
            rt.Push(IntPtr.Zero,3,1); rt.Pop();
            rt.Make(big,absolute); return rt.Snapshot();
        }
    }
    static RelocationChunk LargeReferences(Assembly original, int count, bool big)
    {
        using (var rt = new Runtime(original))
        {
            IntPtr root = rt.Root(4,(uint)count);
            for (int i=count-1; i>=0; --i) rt.Reference(At(root,i*4),(uint)(i%5));
            rt.Make(big,0x100); return rt.Snapshot();
        }
    }
    static RelocationChunk MemberTable(Assembly original, bool big, bool exchange)
    {
        using (var data = new Runtime(original))
        using (var table = new Runtime(original))
        {
            IntPtr root = data.Root(4,8);
            IntPtr child = data.Push(root,1,5); data.Pop();
            IntPtr tokens = table.Root(12,3);
            Marshal.WriteInt32(tokens,0,0x11223344);
            Marshal.WriteInt32(tokens,12,0x55667788);
            table.Bookmark(At(child,4),At(tokens,8));
            table.Bookmark(At(root,24),At(tokens,20));
            if (exchange)
            {
                if (!table.Exchange(tokens,At(tokens,12))) throw new Exception("Valid exchange rejected");
                // ExchangePointers only moves bookmarks; the sort caller moves bytes.
                for (int i=0; i<12; ++i) { byte a=Marshal.ReadByte(tokens,i), b=Marshal.ReadByte(tokens,12+i); Marshal.WriteByte(tokens,i,b); Marshal.WriteByte(tokens,12+i,a); }
            }
            table.Make(big,0); table.Fix(data,big);
            return table.Snapshot();
        }
    }
    static RelocationChunk ExchangeReferences(Assembly original, bool big)
    {
        using (var rt = new Runtime(original))
        {
            IntPtr root = rt.Root(8,3);
            rt.Push(root,4,1); rt.Pop();
            rt.Push(At(root,8),4,1); rt.Pop();
            rt.Reference(At(root,4),10); rt.Reference(At(root,12),20);
            if (!rt.Exchange(root,At(root,8))) throw new Exception("Valid exchange rejected");
            for (int i=0; i<8; ++i) { byte a=Marshal.ReadByte(root,i), b=Marshal.ReadByte(root,8+i); Marshal.WriteByte(root,i,b); Marshal.WriteByte(root,8+i,a); }
            if (rt.Exchange(root,IntPtr.Zero)) throw new Exception("Outside exchange accepted");
            rt.Make(big,72); return rt.Snapshot();
        }
    }
    static void Offsets(Assembly original)
    {
        using (var a = new Runtime(original))
        using (var b = new Runtime(null))
        {
            var ra=a.Root(1,5); var rb=b.Root(1,5);
            var ca=a.Push(ra,1,3); var cb=b.Push(rb,1,3); a.Pop(); b.Pop();
            for (int i=0; i<4; ++i)
            {
                uint x=123,y=123;
                Equal("member found",a.Offset(At(ca,i),ref x)?1u:0u,b.Offset(At(cb,i),ref y)?1u:0u);
                Equal("member offset",x,y); Equal("padded member offset",(uint)(8+i),x);
            }
            uint ax=777,bx=777;
            Equal("invalid member found",a.Offset(IntPtr.Zero,ref ax)?1u:0u,b.Offset(IntPtr.Zero,ref bx)?1u:0u);
            Equal("invalid member preserves value",ax,bx); Equal("invalid value",777,ax);
            // Root's alignment padding is considered part of its block too.
            ax=0; bx=0; a.Offset(At(ra,7),ref ax); b.Offset(At(rb,7),ref bx); Equal("root padding",ax,bx);
        }
    }
    static int Main(string[] args)
    {
        if (IntPtr.Size != 4) throw new Exception("Original runtime requires x86");
        var original = Assembly.LoadFrom(args[0]);
        Console.WriteLine(original.FullName);
        bool tokenized = original.GetName().Name == "BinaryAssetBuilder.Tokenizer";
        if (tokenized) Offsets(original);
        else Console.WriteLine("INFO: XmlCompiler has no GetMemberOffset/BookmarkMemberOffset/FixupMemberOffsets methods; token-table-only cases apply to Tokenizer.");
        foreach (bool big in new[] {false,true})
        {
            for (int seed=0; seed<24; ++seed)
                foreach (uint absolute in new uint[] {0,1260})
                    Compare("graph "+seed+" big="+big+" base="+absolute,Graph(original,seed,big,absolute),Graph(null,seed,big,absolute));
            foreach (int count in new[] {1,2,27,28,29,64,257})
                Compare("duplicate reference sort "+count+" big="+big,LargeReferences(original,count,big),LargeReferences(null,count,big));
            if (tokenized) foreach (bool exchange in new[] {false,true})
                Compare("member table exchange="+exchange+" big="+big,MemberTable(original,big,exchange),MemberTable(null,big,exchange));
            Compare("exchange pointer/reference big="+big,ExchangeReferences(original,big),ExchangeReferences(null,big));
        }
        Console.WriteLine("PASS: "+comparisons+" original/recovered comparisons");
        return 0;
    }
}
