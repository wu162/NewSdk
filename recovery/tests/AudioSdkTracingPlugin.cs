using System;
using System.IO;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;

// Test-only SDK adapter: both sides pass through the same wrapper and original BAB pipeline.
public sealed class AudioSdkTracingPlugin : IAssetBuilderPlugin, IDisposable
{
    private readonly IAssetBuilderPlugin inner;
    public AudioSdkTracingPlugin()
    {
        string side=Environment.GetEnvironmentVariable("AUDIO_SDK_IMPLEMENTATION");
        if(side!="Original" && side!="Recovered")throw new Exception("SDK audio test implementation was not selected");
        string name=side=="Original"?"BinaryAssetBuilder.AudioCompiler":"Recovered.AudioCompiler";
        string path=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,name+".dll");
        inner=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(path).GetType("BinaryAssetBuilder.AudioCompiler.Plugin",true));
        if(side=="Recovered")foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            if(assembly.GetName().Name=="BinaryAssetBuilder.AudioCompiler")throw new Exception("Original AudioCompiler loaded in recovered SDK");
    }
    public void Initialize(object config,TargetPlatform platform){inner.Initialize(config,platform);}
    public void ReInitialize(object config,TargetPlatform platform){inner.ReInitialize(config,platform);}
    public uint GetAllTypesHash(){return inner.GetAllTypesHash();}
    public uint GetVersionNumber(){return inner.GetVersionNumber();}
    public ExtendedTypeInformation GetExtendedTypeInformation(uint id){return inner.GetExtendedTypeInformation(id);}
    public AssetBuffer ProcessInstance(InstanceDeclaration declaration)
    {
        AssetBuffer output=inner.ProcessInstance(declaration);
        if(output==null)throw new Exception("Required audio SDK fixture returned null: "+declaration.XmlNode.Name);
        Func<byte[],string> encode=data=>data==null?"<null>":Convert.ToBase64String(data);
        string custom=File.Exists(declaration.CustomDataPath)?encode(File.ReadAllBytes(declaration.CustomDataPath)):"<absent>";
        string id=declaration.XmlNode.Attributes["id"].Value;
        string record=declaration.XmlNode.LocalName+"\t"+id+"\t"+encode(output.InstanceData)+"\t"+encode(output.RelocationData)+"\t"+encode(output.ImportsData)+"\t"+custom+Environment.NewLine;
        File.AppendAllText(Environment.GetEnvironmentVariable("AUDIO_SDK_TRACE"),record);
        return output;
    }
    public void Dispose(){((IDisposable)inner).Dispose();}
}
