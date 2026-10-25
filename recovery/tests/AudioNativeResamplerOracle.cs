// Compile together with AudioResamplerKernelOracle.cs, selecting this entrypoint.
internal static class AudioNativeResamplerOracle
{
    private static int Main(string[] args)
    { return AudioResamplerKernelOracle.Run("original", args); }
}
