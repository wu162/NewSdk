using System;
using System.IO;
using System.Runtime.InteropServices;
namespace BinaryAssetBuilder.Utility;
public abstract unsafe class StructWrapper_003CAssetStream_003A_003AAssetHeader_003E : IDisposable
{
    protected AssetStream.AssetHeader* m_Data;
    protected abstract void Swap();
    public StructWrapper_003CAssetStream_003A_003AAssetHeader_003E() { m_Data = (AssetStream.AssetHeader*)Marshal.AllocHGlobal(32);
    }
    public virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        // Original cpblk reads exactly 32 bytes; zero length still indexes buffer[0].
        fixed(byte* source = &buffer[0]) { for(int i=0;i<32;i++) ((byte*)m_Data)[i]=source[i]; }
        if(isBigEndian) Swap();
    }
    public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    { byte[] buffer=new byte[32]; input.Read(buffer,0,32); LoadFromBuffer(buffer,isBigEndian); }
    public virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        if(isBigEndian) Swap();
        byte[] buffer=new byte[32]; Marshal.Copy((IntPtr)m_Data,buffer,0,32); output.Write(buffer,0,32);
        if(isBigEndian) Swap();
    }
    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool disposing)
    { if(disposing && m_Data!=null) { Marshal.FreeHGlobal((IntPtr)m_Data); m_Data=null; } }
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
}
public abstract unsafe class StructWrapper_003CAssetStream_003A_003AManifestHeader_003E : IDisposable
{
    protected AssetStream.ManifestHeader* m_Data;
    protected abstract void Swap();
    public StructWrapper_003CAssetStream_003A_003AManifestHeader_003E() { m_Data = (AssetStream.ManifestHeader*)Marshal.AllocHGlobal(48);
        for (int i=0;i<48;i++) ((byte*)m_Data)[i]=0;
    }
    public virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        // Original cpblk reads exactly 48 bytes; zero length still indexes buffer[0].
        fixed(byte* source = &buffer[0]) { for(int i=0;i<48;i++) ((byte*)m_Data)[i]=source[i]; }
        if(isBigEndian) Swap();
    }
    public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    { byte[] buffer=new byte[48]; input.Read(buffer,0,48); LoadFromBuffer(buffer,isBigEndian); }
    public virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        if(isBigEndian) Swap();
        byte[] buffer=new byte[48]; Marshal.Copy((IntPtr)m_Data,buffer,0,48); output.Write(buffer,0,48);
        if(isBigEndian) Swap();
    }
    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool disposing)
    { if(disposing && m_Data!=null) { Marshal.FreeHGlobal((IntPtr)m_Data); m_Data=null; } }
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
}
public abstract unsafe class StructWrapper_003CAssetStream_003A_003AAssetEntry_003E : IDisposable
{
    protected AssetStream.AssetEntry* m_Data;
    protected abstract void Swap();
    public StructWrapper_003CAssetStream_003A_003AAssetEntry_003E() { m_Data = (AssetStream.AssetEntry*)Marshal.AllocHGlobal(48);
    }
    public virtual void LoadFromBuffer(byte[] buffer, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        // Original cpblk reads exactly 48 bytes; zero length still indexes buffer[0].
        fixed(byte* source = &buffer[0]) { for(int i=0;i<48;i++) ((byte*)m_Data)[i]=source[i]; }
        if(isBigEndian) Swap();
    }
    public virtual void LoadFromStream(Stream input, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    { byte[] buffer=new byte[48]; input.Read(buffer,0,48); LoadFromBuffer(buffer,isBigEndian); }
    public virtual void SaveToStream(Stream output, [MarshalAs(UnmanagedType.U1)] bool isBigEndian)
    {
        if(isBigEndian) Swap();
        byte[] buffer=new byte[48]; Marshal.Copy((IntPtr)m_Data,buffer,0,48); output.Write(buffer,0,48);
        if(isBigEndian) Swap();
    }
    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool disposing)
    { if(disposing && m_Data!=null) { Marshal.FreeHGlobal((IntPtr)m_Data); m_Data=null; } }
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
}
