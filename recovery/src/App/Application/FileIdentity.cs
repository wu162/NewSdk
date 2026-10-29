using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace NewModCompiler.Application
{
    // NTFS directory records include ChangeTime and FileId. Unlike length/mtime,
    // these detect replacement and writes followed by restoring LastWriteTime.
    // One metadata query returns a batch; schema identity never reads file bodies.
    internal sealed class FileIdentity
    {
        internal string Path;
        internal long FileId, ChangeTime, LastWriteTime, Size;

        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        static extern SafeFileHandle CreateFile(string path, uint access, uint sharing, IntPtr security,
            uint disposition, uint flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError=true)]
        static extern bool GetFileInformationByHandleEx(SafeFileHandle file, int informationClass,
            IntPtr information, uint size);

        internal static IList<FileIdentity> ReadDirectory(string root, bool recursive)
        {
            var result=new List<FileIdentity>(); var pending=new Stack<string>(); pending.Push(root);
            const int bufferSize=65536;
            IntPtr buffer=Marshal.AllocHGlobal(bufferSize);
            try
            {
                while(pending.Count!=0)
                {
                    string directory=pending.Pop();
                    using(var handle=CreateFile(directory,1,7,IntPtr.Zero,3,0x02000000,IntPtr.Zero))
                    {
                        if(handle.IsInvalid)throw new Win32Exception(Marshal.GetLastWin32Error(),directory);
                        while(true)
                        {
                            // FILE_ID_BOTH_DIR_INFO (10), directory cursor advances.
                            if(!GetFileInformationByHandleEx(handle,10,buffer,bufferSize))
                            {
                                int error=Marshal.GetLastWin32Error();
                                if(error==18)break; // ERROR_NO_MORE_FILES
                                throw new Win32Exception(error,directory);
                            }
                            int offset=0;
                            while(true)
                            {
                                IntPtr entry=IntPtr.Add(buffer,offset);
                                string name=Marshal.PtrToStringUni(IntPtr.Add(entry,104),Marshal.ReadInt32(entry,60)/2);
                                if(name!="." && name!="..")
                                {
                                    string path=System.IO.Path.Combine(directory,name);
                                    var attributes=(FileAttributes)Marshal.ReadInt32(entry,56);
                                    if((attributes&FileAttributes.Directory)!=0)
                                    {
                                        if(recursive)pending.Push(path);
                                    }
                                    else result.Add(new FileIdentity {Path=path,FileId=Marshal.ReadInt64(entry,96),
                                        ChangeTime=Marshal.ReadInt64(entry,32),LastWriteTime=Marshal.ReadInt64(entry,24),Size=Marshal.ReadInt64(entry,40)});
                                }
                                int next=Marshal.ReadInt32(entry,0);
                                if(next==0)break;
                                offset+=next;
                            }
                        }
                    }
                }
                return result;
            }
            finally {Marshal.FreeHGlobal(buffer);}
        }
    }
}
