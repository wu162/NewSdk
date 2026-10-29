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
        internal long VolumeSerial, FileId, ChangeTime, LastWriteTime, Size;

        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        static extern SafeFileHandle CreateFile(string path, uint access, uint sharing, IntPtr security,
            uint disposition, uint flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError=true)]
        static extern bool GetFileInformationByHandleEx(SafeFileHandle file, int informationClass,
            IntPtr information, uint size);

        // Schema closures share a few directories. Read each directory once per
        // validation pass rather than opening every XSD for its identity.
        internal static IDictionary<string, string> SchemaIdentities(IEnumerable<string> paths)
        {
            var directories = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in paths)
            {
                string parent = System.IO.Path.GetDirectoryName(path);
                Dictionary<string, string> files;
                if (!directories.TryGetValue(parent, out files))
                {
                    files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    try { foreach (var file in ReadDirectory(parent, false)) files.Add(file.Path, FileHashCache.Key(file, false)); }
                    catch (DirectoryNotFoundException) { }
                    directories.Add(parent, files);
                }
                string identity;
                files.TryGetValue(path, out identity);
                result.Add(path, identity);
            }
            return result;
        }

        // Query one known file/directory. Used by generation receipts so a hot
        // publication does not recursively enumerate published or read bodies.
        internal static string Stamp(string path)
        {
            using (var handle = CreateFile(path, 0x80, 7, IntPtr.Zero, 3, 0x02200000, IntPtr.Zero))
            {
                if (handle.IsInvalid) return null;
                IntPtr buffer = Marshal.AllocHGlobal(40);
                try
                {
                    if (!GetFileInformationByHandleEx(handle, 0, buffer, 40)) throw new Win32Exception(Marshal.GetLastWin32Error(), path);
                    if (((FileAttributes)Marshal.ReadInt32(buffer, 32) & FileAttributes.ReparsePoint) != 0) throw new IOException("Cache receipt cannot use reparse points: " + path);
                    long change = Marshal.ReadInt64(buffer, 24), write = Marshal.ReadInt64(buffer, 16);
                    if (!GetFileInformationByHandleEx(handle, 1, buffer, 24)) throw new Win32Exception(Marshal.GetLastWin32Error(), path);
                    long size = Marshal.ReadInt64(buffer, 8);
                    if (!GetFileInformationByHandleEx(handle, 18, buffer, 24)) throw new Win32Exception(Marshal.GetLastWin32Error(), path);
                    return Marshal.ReadInt64(buffer, 0).ToString("x16") + Marshal.ReadInt64(buffer, 8).ToString("x16") +
                        Marshal.ReadInt64(buffer, 16).ToString("x16") + change.ToString("x16") + write.ToString("x16") + size.ToString("x16");
                }
                finally { Marshal.FreeHGlobal(buffer); }
            }
        }

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
                    using(var handle=CreateFile(directory,0x81,7,IntPtr.Zero,3,0x02000000,IntPtr.Zero))
                    {
                        if(handle.IsInvalid)
                        {
                            int error=Marshal.GetLastWin32Error();
                            if(error==2 || error==3)throw new DirectoryNotFoundException(directory);
                            throw new Win32Exception(error,directory);
                        }
                        if(!GetFileInformationByHandleEx(handle,18,buffer,bufferSize))throw new Win32Exception(Marshal.GetLastWin32Error(),directory);
                        long volume=Marshal.ReadInt64(buffer,0);
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
                                    else result.Add(new FileIdentity {Path=path,VolumeSerial=volume,FileId=Marshal.ReadInt64(entry,96),
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
