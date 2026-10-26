[CmdletBinding()]
param([switch]$OriginalServer)
$ErrorActionPreference='Stop'
# Portable wire self-test, using an intentionally minimal Core interface stub.
# This verifies neither original/recovered Core ABI nor Windows x86 CLR2 loading.
# Invoke in separate pwsh processes with and without -OriginalServer so both
# conditional compilation branches are semantically compiled from fresh types.
$source=Get-Content (Join-Path $PSScriptRoot 'XmlProductOracleTransport.cs') -Raw
$stub=@'
namespace BinaryAssetBuilder.Core {
 public enum TargetPlatform { Win32=0 }
 public interface IAssetBuilderPlugin {
  void Initialize(object context,TargetPlatform platform);
  AssetBuffer ProcessInstance(InstanceDeclaration declaration);
 }
 public class AssetBuffer { public byte[] InstanceData,RelocationData,ImportsData; }
 public class InstanceHandle { public string TypeName;public uint TypeId,InstanceId; }
 public class InstanceDeclaration {
  public InstanceDeclaration(AssetDeclarationDocument doc){}
  public System.Xml.XmlNode XmlNode;
  public InstanceHandle Handle=new InstanceHandle();
 }
 public class AssetDeclarationDocument {
  private object _Current;
  private class CurrentState { public System.Xml.XmlNamespaceManager NamespaceManager; }
 }
}
'@
if($OriginalServer){$source="#define XML_ORIGINAL_SERVER`n"+$source}
# Framework FormatterServices is deliberately used by the real test harness;
# its modern-runtime obsolescence warning does not affect this wire self-test.
Add-Type -TypeDefinition ($source+$stub) -IgnoreWarnings -PassThru -WarningAction SilentlyContinue|Out-Null
$wire=[AppDomain]::CurrentDomain.GetAssemblies()|ForEach-Object{$_.GetType('XmlOracleWire')}|Where-Object{$null -ne $_}|Select-Object -First 1
if($null -eq $wire){throw 'Compiled XML oracle wire type is missing'}
$flags=[Reflection.BindingFlags]'Static,NonPublic'
$wire.GetMethod('SelfTest',$flags).Invoke($null,@())|Out-Null

# Framing must reject truncated or negative-size payloads rather than quietly
# returning partial data to the original oracle or product comparison.
$bytesMethod=$wire.GetMethod('Bytes',$flags,$null,[type[]]@([IO.BinaryReader]),$null)
foreach($payload in @([byte[]](255,255,255,255),[byte[]](4,0,0,0,1,2))) {
    $reader=[IO.BinaryReader]::new([IO.MemoryStream]::new([byte[]]$payload))
    $rejected=$false
    try {$bytesMethod.Invoke($null,@($reader))|Out-Null}
    catch {
        $cause=$_.Exception
        while($null -ne $cause.InnerException){$cause=$cause.InnerException}
        if($cause -isnot [IO.InvalidDataException] -and $cause -isnot [IO.EndOfStreamException]){throw}
        $rejected=$true
    } finally {$reader.Dispose()}
    if(!$rejected){throw 'Invalid XML wire buffer framing was accepted'}
}
$branch=if($OriginalServer){'original server'}else{'product client'}
Write-Host "PASS XML $branch branch: C# semantic compilation, lossless UTF-16 DOM round trip, malformed-frame rejection (Core stub only; not ABI or SDK acceptance)"
