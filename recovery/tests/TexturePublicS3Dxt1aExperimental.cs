using System;
using System.Runtime.InteropServices;

// Experimental API used only by differential hosts. No production plugin route.
// PC Production=71 and the original default alpha threshold (0.5) only.
internal static class TexturePublicS3Dxt1aExperimental
{
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr LoadLibrary(string path);
    [DllImport("kernel32.dll",CharSet=CharSet.Ansi)] static extern IntPtr GetProcAddress(IntPtr module,string name);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] unsafe delegate int Colour(IntPtr state,int length,int mode);
    static Colour colour;
    internal static void Initialize(string path){
        IntPtr module=LoadLibrary(System.IO.Path.GetFullPath(path));
        if(module==IntPtr.Zero)throw new InvalidOperationException("Public S3 native load failed.");
        IntPtr entry=GetProcAddress(module,"TexturePublicS3ThreeColour");
        if(entry==IntPtr.Zero)throw new InvalidOperationException("Public S3 three-colour export missing.");
        colour=(Colour)Marshal.GetDelegateForFunctionPointer(entry,typeof(Colour));
    }
    internal static byte[] EncodeFloatBlock(float[] rgba,int width,int height){
        if(rgba==null)throw new ArgumentNullException("rgba");
        if((width!=1&&width!=2&&width!=4)||(height!=1&&height!=2&&height!=4))throw new NotSupportedException("A 1/2/4 power-of-two block is required.");
        if(rgba.Length!=width*height*4)throw new ArgumentException("RGBA storage must match dimensions.","rgba");
        foreach(float value in rgba)if(Single.IsNaN(value)||value<0||value>1)throw new ArgumentException("Finite normalized RGBA samples are required.","rgba");
        int[] sourceMap=new int[width*height];int count=0;bool transparent=false;
        byte[] state=new byte[0x3850];Put(state,0,1);Put(state,4,0);
        for(int i=0;i<sourceMap.Length;i++){
            if(rgba[i*4+3]<=0.5f){sourceMap[i]=-1;transparent=true;continue;}
            sourceMap[i]=count;for(int c=0;c<3;c++)PutDouble(state,0x10+(count*3+c)*8,rgba[i*4+c]);count++;
        }
        if(!transparent)throw new NotSupportedException("Opaque DXT1A retains the original uninitialized-state boundary.");
        Put(state,8,count);PutDouble(state,0x3010,16);PutDouble(state,0x3018,(double)0.3086f);PutDouble(state,0x3020,(double)0.6094f);PutDouble(state,0x3028,(double)0.082f);Put(state,0x3030,71);
        int[] indices=new int[count];ushort first=0,second=0;int levels=3;
        if(count>0){
            if(colour==null)throw new InvalidOperationException("Experimental native encoder has not been initialized.");
            int status;unsafe{fixed(byte* p=state)status=colour(new IntPtr(p),state.Length,0);}
            if(status!=0)throw new InvalidOperationException("Public S3 state rejected: "+status);
            levels=BitConverter.ToInt32(state,0x3034);if(levels!=3&&levels!=4)throw new InvalidOperationException("Invalid colour level count.");
            first=Pack(state,0x3038);second=Pack(state,0x3044);
            for(int i=0;i<count;i++)indices[i]=BitConverter.ToInt32(state,0x3050+i*4);
        }
        // Original DXT1A equality rule forces level3 and increments endpoint1.
        // Otherwise retain actual level4 single-colour results from the core.
        if(first==second){second=unchecked((ushort)(second+1));levels=3;Array.Clear(indices,0,count);}
        bool swap=(second<first)==(levels==3);if(swap){ushort tmp=first;first=second;second=tmp;}
        byte[] result=new byte[8];result[0]=(byte)first;result[1]=(byte)(first>>8);result[2]=(byte)second;result[3]=(byte)(second>>8);
        int[] map=ColourMap(width,height);uint bits=0;
        for(int i=0;i<16;i++){
            int compact=map[i]<0?-1:sourceMap[map[i]],index=compact<0?3:indices[compact];
            if(index<0||index>3)throw new InvalidOperationException("Invalid colour index.");
            if(compact>=0&&swap&&(levels==4||index<2))index^=1;
            bits|=(uint)index<<(i*2);
        }
        for(int i=0;i<4;i++)result[4+i]=(byte)(bits>>(8*i));return result;
    }
    static ushort Pack(byte[] bytes,int offset){int r=BitConverter.ToInt32(bytes,offset),g=BitConverter.ToInt32(bytes,offset+4),b=BitConverter.ToInt32(bytes,offset+8);if(r<0||r>31||g<0||g>63||b<0||b>31)throw new InvalidOperationException("Invalid RGB565 endpoint.");return(ushort)((r<<11)|(g<<5)|b);}
    static void Put(byte[] b,int o,int v){Buffer.BlockCopy(BitConverter.GetBytes(v),0,b,o,4);}
    static void PutDouble(byte[] b,int o,double v){Buffer.BlockCopy(BitConverter.GetBytes(v),0,b,o,8);}
        private static int[] ColourMap(int width,int height)
        {
            int[] m=new int[16];for(int i=0;i<16;i++)m[i]=-1;
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)m[y*4+x]=y*width+x;
            // Literal default PC dispatch mapping at original RVA 0x5d3c5..0x5d4de.
            switch((height<<4)|width){
                case 0x11:m[1]=m[0];goto L3f7;
                case 0x12:m[12]=m[8]=m[4]=m[0];goto L3f7;
                case 0x14:for(int y=1;y<4;y++)for(int x=0;x<4;x++)m[y*4+x]=m[x];return m;
                case 0x21:m[1]=m[2]=m[3]=m[0];m[8]=m[12]=m[4];m[1]=m[4];goto L405;
                case 0x22:m[2]=m[3]=m[1];m[6]=m[7]=m[10]=m[11]=m[15]=m[14]=m[5];m[8]=m[12]=m[4];return m;
                case 0x24:for(int y=2;y<4;y++)for(int x=0;x<4;x++)m[y*4+x]=m[4+x];return m;
                case 0x41:m[1]=m[2]=m[3]=m[0];m[5]=m[6]=m[7]=m[4];m[9]=m[10]=m[11]=m[8];m[1]=m[12];goto L42f;
                case 0x42:m[2]=m[3]=m[1];m[6]=m[7]=m[5];m[10]=m[11]=m[9];goto L436;
                default:return m;
            }
        L3f7:m[3]=m[2]=m[1];
        L405:m[5]=m[6]=m[7]=m[9]=m[10]=m[11]=m[1];
        L42f:m[13]=m[1];
        L436:m[14]=m[15]=m[13];m[1]=m[2]=m[3];m[5]=m[6]=m[7];m[9]=m[10]=m[11];return m;
        }
}
