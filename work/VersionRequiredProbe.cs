using System;
using System.IO;

namespace NewModCompiler.Tests
{
    public static class VersionRequiredProbe
    {
        public static int Main(string[] args)
        {
            try {
                int checks = BuildDefaultsTests.RunVersionRequirement(Path.GetFullPath(args[0]));
                Console.WriteLine("PASS required BIG mod-version assertions=" + checks);
                return 0;
            } catch (Exception error) {
                Console.Error.WriteLine(error);
                return 1;
            }
        }
    }
}
