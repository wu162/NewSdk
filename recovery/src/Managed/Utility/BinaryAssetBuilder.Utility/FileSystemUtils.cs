using System;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility;

// The original C++/CLI methods call these same Unicode Win32 functions.
public class FileSystemUtils
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern unsafe uint FormatMessageW(uint flags, void* source, uint messageId,
        uint languageId, char* buffer, uint size, void* arguments);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern unsafe IntPtr CreateFileW(char* name, uint access, uint sharing,
        void* security, uint disposition, uint flags, IntPtr template);
    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern int FlushFileBuffers(IntPtr handle);
    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern int CloseHandle(IntPtr handle);

    public unsafe static string GetErrorString(uint code)
    {
        char* buffer = stackalloc char[256];
        // Original initializes only the first 80 bytes; FormatMessage writes the
        // returned characters. Match the observed text and preserve trailing CR/LF.
        for (int i = 0; i < 40; i++) buffer[i] = '\0';
        uint length = FormatMessageW(4096u, null, code, 0u, buffer, 256u, null);
        return string.Concat(code + " ", new string(buffer, 0, (int)length));
    }

    public unsafe static void FlushVolume([MarshalAs(UnmanagedType.U2)] char drive)
    {
        string text = string.Concat("\\\\.\\" + drive, ":\0");
        char* buffer = stackalloc char[7];
        for (int i = 0; i < 7; i++) buffer[i] = text[i];
        IntPtr handle = CreateFileW(buffer, 1073741824u, 2u, null, 3u, 0u, IntPtr.Zero);
        if (handle == new IntPtr(-1))
        {
            uint error = unchecked((uint)Marshal.GetLastWin32Error());
            throw new Exception("Could not open handle to " + text + ", error: " + GetErrorString(error));
        }
        // Preserve the original failure order: a failed flush throws before CloseHandle.
        if (FlushFileBuffers(handle) == 0)
        {
            uint error = unchecked((uint)Marshal.GetLastWin32Error());
            throw new Exception("Could not flush " + text + ", error: " + GetErrorString(error));
        }
        if (CloseHandle(handle) == 0)
        {
            uint error = unchecked((uint)Marshal.GetLastWin32Error());
            throw new Exception("Could not close " + text + ", error: " + GetErrorString(error));
        }
    }
}
