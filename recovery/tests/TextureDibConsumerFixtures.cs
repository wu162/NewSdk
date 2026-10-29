using System;
using System.Collections.Generic;

// One shared, explicit source set for full plugin and SDK consumption.
internal static class TextureDibConsumerFixtures
{
    internal sealed class Source
    {
        internal string Label;internal byte[] Bmp;internal bool Transparent,Atlas,Indexed;
        internal Source(string label,byte[] bmp,bool transparent,bool atlas,bool indexed){Label=label;Bmp=bmp;Transparent=transparent;Atlas=atlas;Indexed=indexed;}
    }
    internal static void Put(byte[] b,int at,uint v){for(int i=0;i<4;i++)b[at+i]=(byte)(v>>(8*i));}
    internal static byte[] Bare(byte[] bmp){byte[] b=new byte[bmp.Length-14];Buffer.BlockCopy(bmp,14,b,0,b.Length);return b;}
    internal static byte[] Table(byte[] bmp,int entries)
    {
        if(entries==0)return bmp;
        int dib=(int)BitConverter.ToUInt32(bmp,14),at=14+dib,bytes=entries*4;
        if(dib==12||BitConverter.ToUInt32(bmp,46)!=0)throw new Exception("Direct-colour table fixture required");
        byte[] b=new byte[bmp.Length+bytes];Buffer.BlockCopy(bmp,0,b,0,at);Buffer.BlockCopy(bmp,at,b,at+bytes,bmp.Length-at);
        for(int i=0;i<bytes;i++)b[at+i]=(byte)(139+i*73);
        Put(b,2,(uint)b.Length);Put(b,10,BitConverter.ToUInt32(bmp,10)+(uint)bytes);Put(b,46,(uint)entries);
        if(dib==124&&BitConverter.ToUInt32(bmp,126)!=0)Put(b,126,BitConverter.ToUInt32(bmp,126)+(uint)bytes);
        return b;
    }
    internal static byte[] Create(string icc,int dib,int depth,int width,int height,bool top,string profile)
    {return dib==12||dib==64?TextureBmpOs2Fixtures.Create(width,height,depth,top,0,dib,0,0):TextureBmpHeaderFixtures.Create(icc,width,height,depth,top,0,dib,0,profile,0);}
    internal static IEnumerable<Source> All(string icc)
    {
        foreach(int dib in new[]{12,40,52,56,64,108,124})foreach(string profile in TextureBmpHeaderConsumerFixtures.Profiles(dib))foreach(bool top in dib==12?new[]{false}:new[]{false,true})
        {
            int[][] sizes=profile=="plain"?new[]{new[]{16,8},new[]{7,5},new[]{1,1},new[]{8,16}}:new[]{new[]{16,8},new[]{7,5}};
            int shape=0;
            foreach(int[] size in sizes){
                foreach(int depth in new[]{16,24,32}){
                    int entries=dib==12?0:new[]{0,1,3,17}[(dib+depth/8+shape+(top?1:0))%4];
                    string label="h"+dib+"-d"+depth+"-"+size[0]+"x"+size[1]+"-top"+top+"-table"+entries+"-"+profile;
                    yield return new Source(label,Table(Create(icc,dib,depth,size[0],size[1],top,profile),entries),false,false,false);
                }
                byte[] transparent=Create(icc,dib,32,size[0],size[1],top,profile);
                int start=(int)BitConverter.ToUInt32(transparent,10);
                for(int i=0;i<size[0]*size[1];i++)transparent[start+i*4+3]=63;
                int n=dib==12?0:new[]{0,1,3,17}[(dib+shape+(top?1:0))%4];
                yield return new Source("transparent-h"+dib+"-"+size[0]+"x"+size[1]+"-top"+top+"-table"+n+"-"+profile,Table(transparent,n),true,false,false);
                shape++;
            }
        }
        foreach(int dib in new[]{12,40,52,56,64,108,124})foreach(int depth in new[]{16,24,32})foreach(string profile in TextureBmpHeaderConsumerFixtures.Profiles(dib)){
            int n=dib==12?0:new[]{1,3,17}[depth/8-2];
            yield return new Source("atlas-h"+dib+"-d"+depth+"-table"+n+"-"+profile,Table(Create(icc,dib,depth,256,256,false,profile),n),false,true,false);
        }
        foreach(int dib in new[]{12,40,52,56,64,108,124})foreach(int depth in new[]{1,4,8})foreach(bool top in dib==12?new[]{false}:new[]{false,true})foreach(int[] size in new[]{new[]{16,8},new[]{7,5}})
            yield return new Source("indexed-h"+dib+"-d"+depth+"-top"+top+"-"+size[0]+"x"+size[1],Create(icc,dib,depth,size[0],size[1],top,"plain"),false,false,true);
    }
}
