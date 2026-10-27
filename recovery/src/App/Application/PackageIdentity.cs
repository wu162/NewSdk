using System;
using System.Linq;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Application
{
    // SDK/defaultscript.cs used modversion/skudefname GUI values and fixed
    // DEFAULT_GAME_VERSION. Package identity never changes linked stream bytes.
    public static class PackageIdentity
    {
        static BuildFailure Bad(string message) { return new BuildFailure(ExitCode.Arguments,"PACKAGE_IDENTITY_INVALID",message); }
        public static void ValidateGameVersion(string value)
        {
            if(string.IsNullOrEmpty(value) || value.Length>32 || value.Split('.').Any(part=>part.Length==0 || part.Any(c=>c<'0' || c>'9')))
                throw Bad("game-version must contain decimal components separated by dots.");
        }
        public static void Validate(BuildRequest request)
        {
            string version=request.ModVersion;
            if(string.IsNullOrEmpty(version) || version.Length>64 || !AlphaNumeric(version[0]) || version.Any(c=>!AlphaNumeric(c) && c!='.' && c!='_' && c!='-') || version.EndsWith(".",StringComparison.Ordinal))
                throw Bad("mod-version must start with an ASCII letter/digit and contain only letters, digits, dots, underscores or hyphens, without a trailing dot (maximum 64 characters).");
            ValidateGameVersion(request.GameVersion);
            string name=request.SkudefName;
            if(name!=null && (string.IsNullOrWhiteSpace(name) || name=="." || name==".." || name!=name.Trim() || name.EndsWith(".",StringComparison.Ordinal) || name.Any(c=>c<32 || c>126 || "\\/:*?\"<>|".IndexOf(c)>=0)))
                throw Bad("skudef-name must be one ASCII file basename without an extension, directories or trailing dot/space.");
            // Development derives loose output without discarding the project's normal
            // package identity. Explicit loose requests retain the existing guard.
            EffectiveOption profile,format;
            bool development = request.Options.TryGetValue("profile",out profile) && profile.Value=="development"
                && request.Options.TryGetValue("package-format",out format) && format.Source=="default";
            if(request.Command!="doctor" && request.PackageFormat!="big" && !development) foreach(string key in new[]{"mod-version","game-version","skudef-name"}) {
                EffectiveOption option;
                if(request.Options.TryGetValue(key,out option) && option.Source!="default")
                    throw new BuildFailure(ExitCode.Arguments,"PACKAGE_IDENTITY_FORMAT","Package version/name options require --package-format big.");
            }
        }
        static bool AlphaNumeric(char c) { return c>='0' && c<='9' || c>='A' && c<='Z' || c>='a' && c<='z'; }
    }
}
