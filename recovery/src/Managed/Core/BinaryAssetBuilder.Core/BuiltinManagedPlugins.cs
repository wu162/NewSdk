#if NEWSdk_MONOLITHIC
namespace BinaryAssetBuilder.Core;

// Native/XML/W3X registration is a later phase. These two managed factories
// already resolve directly into the same production assembly as Core.
public static class BuiltinManagedPlugins
{
    public static PluginDescriptor Audio()
    {
        return PluginDescriptor.CreateBuiltin("AudioFile,AudioFileMP3Passthrough,PathMusicMap,PathMusicTrack,PathMusicEvent", true, () => new BinaryAssetBuilder.AudioCompiler.Plugin());
    }
    public static PluginDescriptor Verifier()
    {
        return PluginDescriptor.CreateBuiltin("#all", false, () => new BinaryAssetBuilder.GameDataVerifier.Plugin());
    }
}
#endif
