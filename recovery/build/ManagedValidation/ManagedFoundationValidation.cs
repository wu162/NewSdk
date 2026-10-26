using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder;

namespace NewSdk.BuildValidation
{
    // Test-only code, compiled into validation hosts rather than product modules.
    public static class ManagedFoundationValidation
    {
        private static void Require(bool value, string message) { if(!value) throw new Exception(message); }
        public static void Run(bool requireSingleModule)
        {
            Run(requireSingleModule, Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Foundation.types.txt"));
        }
        public static void Run(bool requireSingleModule, string typeInventoryPath)
        {
            var own = Assembly.GetEntryAssembly();
            Require(own != null, "No entry executable");
            string[] types = { "BinaryAssetBuilder.Core.AssetBuffer", "BinaryAssetBuilder.Utility.ManifestHeader", "EALA.Hash.FastHash", "EALA.Metrics.MetricTimer", "Mvp.Xml.XInclude.XIncludingReader", "BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator", "BinaryAssetBuilder.GameDataVerifier.Plugin", "BinaryAssetBuilder.AudioCompiler.Plugin" };
            foreach(var name in types) {
                var type=own.GetType(name, true);
                Require(type.Assembly==own,"Implementation escaped executable: "+name);
            }
            if(requireSingleModule) Require(own.GetModules().Length==1,"Build-time netmodule remains in product");
            Require(File.Exists(typeInventoryPath),"Missing independent Foundation type inventory");
            var expectedTypes=File.ReadAllLines(typeInventoryPath);
            Require(expectedTypes.Length>200,"Incomplete Foundation type inventory");
            foreach(var name in expectedTypes) Require(own.GetType(name,true).Assembly==own,"Foundation type is outside entry EXE: "+name);
            string[] resources = { "BinaryAssetBuilderProject.xsd", "Mvp.Xml.Common.SR.resources", "Mvp.Xml.Common.Serialization.PerfCounterInstaller.resources", "Mvp.Xml.XPointer.SR.resources", "Mvp.Xml.XInclude.SR.resources" };
            var expectedResourceHashes = new string[] { "3ffe9a0f876bc76deaf28d2e0302f32d63b0c842d719f3d9c428dc961b7616c1", "65bc06e64351f384b3f8658d273a36d7dcb2af167e458adc0f9579bb2cf0a1cb", "a3237a994521c1904b0367691fdafc8b4b309371b845157bd149f27b53849d76", "fb2b9ea0745131e0ba26b9c7967a4f74885427d73dbeb6ad68114c8a3bacb1de", "0ef800084cbdf48a5a0b2de121f52737410aaa1356a6325fae779a8b56a77f6c" };
            for(int i=0;i<resources.Length;i++) using(var resource=own.GetManifestResourceStream(resources[i])) {
                Require(resource!=null && resource.Length>0,"Missing resource "+resources[i]);
                using(var hash=System.Security.Cryptography.SHA256.Create())
                    Require(BitConverter.ToString(hash.ComputeHash(resource)).Replace("-","").ToLowerInvariant()==expectedResourceHashes[i],"Embedded resource bytes changed: "+resources[i]);
            }
            using(var schema=own.GetManifestResourceStream("BinaryAssetBuilderProject.xsd")) Require(XmlSchema.Read(schema,null)!=null,"Core schema is unreadable");
            var rm=new ResourceManager("Mvp.Xml.XInclude.SR",own);
            Require(!string.IsNullOrEmpty(rm.GetString("AttributeOrNamespaceInIncludeLocationError",CultureInfo.InvariantCulture)),"XInclude resource lookup failed");
            Require(own.GetTypes().Where(t=>t.Namespace=="Mvp.Xml.Exslt").SelectMany(t=>t.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.Static|BindingFlags.DeclaredOnly)).Count(m=>m.Name.Contains("-"))==51,"EXSLT alias count changed");
            var exsltType=own.GetType("Mvp.Xml.Exslt.ExsltStrings",true);
            var decode=exsltType.GetMethod("decode-uri",new[]{typeof(string)});
            Require(decode!=null && (string)decode.Invoke(Activator.CreateInstance(exsltType),new object[]{"a%20b"})=="a b","Restored EXSLT CLR alias is not callable");
            var includeDirectory=Path.Combine(Path.GetTempPath(),"NewSdk XInclude 中文 "+Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(includeDirectory);
            try {
                File.WriteAllText(Path.Combine(includeDirectory,"included.xml"),"<item>merged</item>");
                var sourcePath=Path.Combine(includeDirectory,"source.xml");
                File.WriteAllText(sourcePath,"<root xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='included.xml'/></root>");
                var merged=new XmlDocument();
                using(var reader=new Mvp.Xml.XInclude.XIncludingReader(sourcePath)) {
                    reader.XmlResolver=new XmlUrlResolver();
                    merged.Load(reader);
                }
                Require(merged.DocumentElement.InnerText=="merged","XInclude stopped resolving relative resource paths");
            } finally {Directory.Delete(includeDirectory,true);}
            Require(own.GetType("BinaryAssetBuilder.Utility.StructWrapper<AssetStream::ManifestHeader>")!=null,"Utility C++ type name lost");
            var before=CultureInfo.CurrentCulture;
            try {
                CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("en-US");
                ExpressionEvaluatorWrapper.LoadAssembly();
                var evaluator=ExpressionEvaluatorWrapper.GetEvaluator(null);
                Require(evaluator!=null && evaluator.Evaluate("=7/2")=="3.5","Direct expression evaluation failed");
            } finally { CultureInfo.CurrentCulture=before; }
            Require(EALA.Hash.FastHash.GetHashCode("ArmorTemplate")==HashProvider.GetTextHash("ArmorTemplate"),"Core hash did not use recovered implementation");
            var audio=new BinaryAssetBuilder.AudioCompiler.Plugin();
            audio.Initialize(null,TargetPlatform.Win32);
            Require(audio.GetVersionNumber()!=0,"Recovered audio plugin initialization failed");
            audio.ReInitialize(null,TargetPlatform.Win32);
            audio.Dispose();
            var verifier=(IAssetBuilderVerifierPlugin)Activator.CreateInstance(own.GetType("BinaryAssetBuilder.GameDataVerifier.Plugin"),true);
            verifier.Initialize(null,TargetPlatform.Win32);
            var wrapper=own.GetType("BinaryAssetBuilder.Utility.ManifestHeader");
            var instance=Activator.CreateInstance(wrapper);
            try {
                var version=wrapper.GetProperty("Version");
                version.SetValue(instance,(ushort)7,null);
                Require((ushort)version.GetValue(instance,null)==7,"Utility native-layout storage did not round trip");
                var size=wrapper.GetMethod("get_TotalInstanceDataSize");
                Require(size!=null,"Utility property contract missing");
            } finally {
                var disposable=instance as IDisposable;
                if(disposable!=null) disposable.Dispose();
            }
            // Factory/serializedType integration is deliberately exercised after source
            // co-compilation, where old assembly identities must never be resolved.
            var audioDescriptor=BuiltinManagedPlugins.Audio();
            var audioRegistry=new PluginRegistry(new[]{audioDescriptor},TargetPlatform.Win32);
            var registeredAudio=audioRegistry.GetPlugin(BinaryAssetBuilder.AudioCompiler.AudioCatalog.AudioFileTypeId);
            Require(registeredAudio.GetType()==typeof(BinaryAssetBuilder.AudioCompiler.Plugin),"Audio factory did not register recovered plugin");
            audioRegistry.ReInitialize(new[]{audioDescriptor},TargetPlatform.Win32);
            Require(object.ReferenceEquals(registeredAudio,audioRegistry.GetPlugin(BinaryAssetBuilder.AudioCompiler.AudioCatalog.AudioFileTypeId)),"Audio factory recreated plugin during ReInitialize");
            ((IDisposable)registeredAudio).Dispose();
            var verifierDescriptor=BuiltinManagedPlugins.Verifier();
            var verifierRegistry=new VerifierPluginRegistry(new[]{verifierDescriptor},TargetPlatform.Win32);
            Require(verifierRegistry.DefaultPlugin.GetType().Assembly==own,"Verifier factory escaped executable");
            var oldDescriptor=new PluginDescriptor {Enabled=true,QualifiedName="BinaryAssetBuilder.AudioCompiler.Plugin, BinaryAssetBuilder.AudioCompiler"};
            bool rejected=false;
            try {oldDescriptor.Initialize(TargetPlatform.Win32);} catch(ApplicationException){rejected=true;}
            Require(rejected,"Unbound old plugin descriptor was silently accepted");
            var settingsDocument=new XmlDocument();
            settingsDocument.LoadXml("<assetbuilder serializedType='BinaryAssetBuilder.Settings,BinaryAssetBuilder.Core'/>");
            Require(new SerializedSectionHandler().Create(null,null,settingsDocument.DocumentElement) is Settings,"Legacy Settings contract was not mapped locally");
            settingsDocument.LoadXml("<assetbuilder serializedType='System.String,mscorlib'/>");
            rejected=false;
            try {new SerializedSectionHandler().Create(null,null,settingsDocument.DocumentElement);} catch(System.Configuration.ConfigurationErrorsException){rejected=true;}
            Require(rejected,"Unsupported serializedType was accepted");
            var forbidden = new[]{"BinaryAssetBuilder.Core","BinaryAssetBuilder.Utility","EALA.Hash","EALA.Metrics","Mvp.Xml","BinaryAssetBuilder.ExpressionEval","BinaryAssetBuilder.GameDataVerifier","BinaryAssetBuilder.AudioCompiler"};
            Require(!own.GetReferencedAssemblies().Any(r=>forbidden.Contains(r.Name)),"Old business assembly reference remains");
            Require(own.GetType("BinaryAssetBuilder.Core.GUIBuildOutput")==null && own.GetType("BinaryAssetBuilder.Core.NoScrollRichTextBox")==null,"Old GUI entered production source set");
            Console.WriteLine("PASS Foundation: EXE type ownership; Core/Mvp resources; 51 EXSLT names; Utility ABI names and storage; expression; hash; audio lifecycle; verifier; no old business AssemblyRef");
        }
    }
}
