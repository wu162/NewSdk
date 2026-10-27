using System;

namespace BinaryAssetBuilder.Core;

// Serial Core work owns its scratch through the processor lifetime. Keeping
// these buffers separate is an ownership step, not a parallel execution permit.
internal sealed class CoreTransferBuffers
{
    private readonly object _Gate = new object();
    private readonly bool _Enabled;
    private byte[] _Copy, _Link;
    private long _CopyRequests, _LinkRequests, _CopyAllocations, _LinkAllocations, _AllocatedBytes, _LegacyCopyRequests, _LegacyLinkRequests;
    internal CoreTransferBuffers()
    {
        bool disabled;
        _Enabled = !(AppContext.TryGetSwitch("NewSdk.Compiler.DisableOwnedTransferBuffers", out disabled) && disabled);
    }
    private byte[] GetBuffer(bool copy, Func<byte[]> legacyFactory)
    {
        lock (_Gate)
        {
            if (copy) _CopyRequests++; else _LinkRequests++;
            if (!_Enabled)
            {
                if (copy) _LegacyCopyRequests++; else _LegacyLinkRequests++;
            }
            else if (copy)
            {
                if (_Copy == null)
                {
                    _Copy = new byte[1048576]; _CopyAllocations++; _AllocatedBytes += _Copy.Length;
                }
                return _Copy;
            }
            else
            {
                if (_Link == null)
                {
                    _Link = new byte[65536]; _LinkAllocations++; _AllocatedBytes += _Link.Length;
                }
                return _Link;
            }
        }
        // Never hold the owner gate while entering the legacy allocation gate.
        // The static array belongs to the legacy consumer, not this owner.
        return legacyFactory();
    }
    internal byte[] GetCopyBuffer(Func<byte[]> legacyFactory) { return GetBuffer(true, legacyFactory); }
    internal byte[] GetLinkBuffer(Func<byte[]> legacyFactory) { return GetBuffer(false, legacyFactory); }
    internal object GetMetrics()
    {
        lock (_Gate)
            return new { enabled = _Enabled, capturedOwnerMode = _Enabled ? "Owned" : "LegacyStatic", scope = "SerialDocumentProcessorTransferBuffers",
                copyRequests = _CopyRequests, linkRequests = _LinkRequests, copyAllocations = _CopyAllocations, linkAllocations = _LinkAllocations,
                allocatedBytes = _AllocatedBytes, legacyCopyRequests = _LegacyCopyRequests, legacyLinkRequests = _LegacyLinkRequests,
                allocationScope = "OwnedBuffersOnly", copyBufferBytes = 1048576, linkBufferBytes = 65536, parallelExecutionEnabled = false };
    }
}
