// SPDX-License-Identifier: GPL-3.0-only
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NewSdk.Ui;

[Flags]
internal enum NativeFeatures : uint { None = 0, Optimizer = 1, Triangulation = 2, Jpeg = 4 }

internal static partial class NativeBridge
{
    [LibraryImport("swfc_native", EntryPoint = "swfc_get_abi_version")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial uint GetAbiVersion();

    [LibraryImport("swfc_native", EntryPoint = "swfc_get_features")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial NativeFeatures GetFeatures();

    internal static void CheckAbi()
    {
        uint actual = GetAbiVersion();
        if (actual != 1) throw new InvalidOperationException($"Unsupported native ABI: {actual}");
    }
}
