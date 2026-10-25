using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using _003CCppImplementationDetails_003E;

namespace BinaryAssetBuilder.Utility;

public class FileSystemUtils
{
	public unsafe static string GetErrorString(uint code)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BAA_0040G _0024ArrayType_0024_0024_0024BY0BAA_0040G);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ref _0024ArrayType_0024_0024_0024BY0BAA_0040G, 0, 80);
		uint length = global::_003CModule_003E.FormatMessageW(4096u, null, code, 0u, (ushort*)(&_0024ArrayType_0024_0024_0024BY0BAA_0040G), 256u, null);
		string text = new string((char*)(&_0024ArrayType_0024_0024_0024BY0BAA_0040G), 0, (int)length);
		return string.Concat(code + " ", text);
	}

	public unsafe static void FlushVolume([MarshalAs(UnmanagedType.U2)] char drive)
	{
		string text = string.Concat("\\\\.\\" + drive, ":\0");
		char[] array = text.ToCharArray();
		int num = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY06G _0024ArrayType_0024_0024_0024BY06G);
		do
		{
			*(char*)((ref *(_003F*)(num * 2)) + (ref *(_003F*)(&_0024ArrayType_0024_0024_0024BY06G))) = array[num];
			num++;
		}
		while (num < 7);
		void* ptr = global::_003CModule_003E.CreateFileW((ushort*)(&_0024ArrayType_0024_0024_0024BY06G), 1073741824u, 2u, null, 3u, 0u, null);
		if (ptr == (void*)(-1))
		{
			uint lastError = global::_003CModule_003E.GetLastError();
			throw new Exception(string.Concat(string.Concat("Could not open handle to " + text, ", error: "), GetErrorString(lastError)));
		}
		if (global::_003CModule_003E.FlushFileBuffers(ptr) == 0)
		{
			uint lastError2 = global::_003CModule_003E.GetLastError();
			throw new Exception(string.Concat(string.Concat("Could not flush " + text, ", error: "), GetErrorString(lastError2)));
		}
		if (global::_003CModule_003E.CloseHandle(ptr) == 0)
		{
			uint lastError3 = global::_003CModule_003E.GetLastError();
			throw new Exception(string.Concat(string.Concat("Could not close " + text, ", error: "), GetErrorString(lastError3)));
		}
	}
}
