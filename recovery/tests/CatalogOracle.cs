using System;
using System.Linq;
using System.Reflection;
using BinaryAssetBuilder.Core;

class CatalogOracle
{
    static int Main(string[] args)
    {
        int count = 0;
        foreach (bool tokenizer in new[] { false, true })
        {
            string name = tokenizer ? "BinaryAssetBuilder.Tokenizer" : "BinaryAssetBuilder.XmlCompiler";
            var assembly = Assembly.LoadFrom(System.IO.Path.Combine(args[0], name + ".dll"));
            var original = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType(name + ".Plugin", true));
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 })
            {
                original.Initialize(null, platform);
                if (original.GetVersionNumber() != Recovered.CompilerCatalog.Version(tokenizer, platform)) throw new Exception(name + " version");
                if (original.GetAllTypesHash() != Recovered.CompilerCatalog.AllTypesHash) throw new Exception(name + " types hash");
                foreach (uint id in Recovered.CompilerCatalog.TypeIds(tokenizer).Concat(new uint[] { 0u, 0xFFFFFFFFu }))
                {
                    var expected = original.GetExtendedTypeInformation(id);
                    var actual = Recovered.CompilerCatalog.TypeInformation(tokenizer, platform, id);
                    foreach (var field in typeof(ExtendedTypeInformation).GetFields())
                    {
                        if (!object.Equals(field.GetValue(expected), field.GetValue(actual)))
                            throw new Exception(name + " " + platform + " " + id + " " + field.Name + ": original=" + field.GetValue(expected) + " recovered=" + field.GetValue(actual));
                    }
                    count++;
                }
                Console.WriteLine("PASS: {0} platform {1}", name, platform);
            }
        }
        Console.WriteLine("PASS: " + count + " original/recovered type records, all public fields");
        return 0;
    }
}
