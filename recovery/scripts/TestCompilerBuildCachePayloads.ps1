#requires -Version 7.0
[CmdletBinding()]
param(
    [string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180,
    [switch]$Preflight
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$Preflight){$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path}
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite build-cache payload evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('p'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $directory=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$path=[IO.Path]::GetFullPath($Path)
    $path.StartsWith($directory+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function EqualBytes([string]$Left,[string]$Right){
    # Preserve strict byte equality while avoiding one PowerShell function call
    # per byte on the multi-MiB real APT payload. No hash-only acceptance.
    if(!('PayloadRawBytes' -as [type])){
        Add-Type -TypeDefinition @'
using System;
using System.IO;
public static class PayloadRawBytes {
    public static bool Equal(string a, string b) {
        using (var left = File.OpenRead(a)) using (var right = File.OpenRead(b)) {
            if (left.Length != right.Length) return false;
            byte[] first = new byte[65536], second = new byte[65536];
            int count;
            while ((count = left.Read(first, 0, first.Length)) != 0) {
                int read = 0;
                while (read < count) { int n = right.Read(second, read, count - read); if (n == 0) return false; read += n; }
                for (int i = 0; i < count; i++) if (first[i] != second[i]) return false;
            }
            return right.ReadByte() == -1;
        }
    }
}
'@
    }
    Need ([PayloadRawBytes]::Equal($Left,$Right)) 'Serial/parallel/cache raw artifact bytes differ'
}
function SameOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'No-cache/cache artifact counts differ'
    foreach($key in $Left.files.Keys){Need ($Right.files.ContainsKey($key)) "Cache output omitted $key";EqualBytes $Left.files[$key] $Right.files[$key]}
}
function DifferentOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'Changed fixture output layout differs'
    $changed=$false
    # CDATA names carry the instance hash and may legitimately change with the
    # input. The unchanged-versus-cache comparison still requires exact names.
    foreach($key in $Left.files.Keys){if(!$Right.files.ContainsKey($key)){$changed=$true}elseif((Hash $Left.files[$key]) -cne (Hash $Right.files[$key])){$changed=$true}}
    Need $changed 'Input mutation did not exercise any observable compiled output change'
}
function Cache($Case,[string]$Expected){
    $statistics=$Case.report.cache
    Need ($null -ne $statistics) 'Durable report omitted actual asset-cache statistics'
    Need ($statistics.executionEngine -ceq 'AssetTasks.v1') 'Payload build did not use the shared asset executor'
    if($Expected -eq 'disabled'){Need ($statistics.cacheMode -ceq 'Disabled' -and $statistics.hits -eq 0 -and $statistics.misses -eq 0 -and $statistics.actionHits -eq 0) 'Disabled cache recorded reuse';return}
    Need ($statistics.status -ceq 'ActionCache' -and $statistics.cacheMode -ceq 'ActionCache') 'Build cache was accepted without the actual action-cache implementation'
    if($Expected -eq 'cold'){Need ($statistics.hits -eq 0 -and $statistics.misses -gt 0) 'Cold asset cache did not record real misses'}
    elseif($Expected -eq 'hot'){Need ($statistics.hits -gt 0 -and $statistics.misses -eq 0) 'Hot asset cache did not reuse all fixture assets'}
    elseif($Expected -eq 'partial'){Need ($statistics.misses -eq 1 -and $statistics.hits -gt 0) 'Single-action change did not retain unrelated certified hits'}
    else{throw "Unknown cache expectation: $Expected"}
}
function SameLengthMutation([string]$Path,[string]$From,[string]$To){
    $before=[IO.File]::ReadAllBytes($Path);$stamp=[IO.File]::GetLastWriteTimeUtc($Path)
    # Match raw UTF-8 bytes to retain BOM, line endings and all other bytes.
    $find=[Text.Encoding]::UTF8.GetBytes($From);$replace=[Text.Encoding]::UTF8.GetBytes($To)
    Need ($find.Length -eq $replace.Length -and $find.Length -gt 0) 'Mutation tokens must have identical UTF-8 lengths'
    $offset=-1
    for($i=0;$i -le $before.Length-$find.Length;$i++){
        $match=$true;for($j=0;$j -lt $find.Length;$j++){if($before[$i+$j] -ne $find[$j]){$match=$false;break}}
        if($match){$offset=$i;break}
    }
    Need ($offset -ge 0) "Mutation token absent: $Path"
    [Array]::Copy($replace,0,$before,$offset,$replace.Length)
    [IO.File]::WriteAllBytes($Path,$before)
    [IO.File]::SetLastWriteTimeUtc($Path,$stamp)
    Need (([IO.File]::ReadAllBytes($Path)).Length -eq $before.Length -and [IO.File]::GetLastWriteTimeUtc($Path) -eq $stamp) 'Mutation did not preserve exact input length and timestamp'
}
function RunCli([string]$Name,[string]$Mod,[string]$Destination,[bool]$Enabled,[int]$Expected=0,[bool]$Cancel=$false,[string]$HoldReadFile=$null,[string[]]$Extra=@()){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    New-Item -ItemType Directory -Force -Path $Destination|Out-Null
    $arguments=@('build','--mod',$Mod,'--sdk-root',$script:sdk,'--out',$Destination,'--package-format','loose','--package-content','base',('--bc='+$Enabled.ToString().ToLowerInvariant()))+$Extra
    $events=Join-Path $Destination 'build.events.jsonl';$report=Join-Path $Destination 'build-report.json'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    if($HoldReadFile){$request.holdReadFile=$HoldReadFile}
    $request|ConvertTo-Json -Depth 6|Set-Content "$caseOut/request.json"
    & $script:control "$caseOut/request.json" "$caseOut/control.json" *> "$caseOut/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$caseOut/control.json" -Raw|ConvertFrom-Json
    $entry=@{name=$Name;arguments=$arguments;expectedExit=$Expected;passed=$false;controller=$audit};$script:modeState.cases+=,$entry
    try{
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "Build-cache payload CLI failed: $Name"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Asset cache introduced a product child process'
        if($Cancel){Need ($audit.signalSent -and $audit.freshEventsPathObserved -and $audit.streamStartedObserved -and $Expected -eq 6) 'Cache cancellation did not receive actual CtrlBreak'}
        $body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json
        Need ($body.exitCode -eq $Expected) 'Cache process/report exit differs'
        $rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        Need ($rows[-1].eventType -ceq 'BuildFinished' -and @($rows|Where-Object{$_.eventType -ceq 'BuildStarted'}).Count -eq 1 -and @($rows|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1) 'Cache CLI terminal protocol differs'
        $sequence=0;foreach($row in $rows){$sequence++;Need ($row.buildId -ceq $body.buildId -and $row.sequence -eq $sequence) 'Cache CLI build identity/sequence differs'}
        Need ($rows[0].payload.processId -eq $audit.processId -and @($body.externalTools).Count -eq 0) 'Cache CLI production identity/external tools differs'
        if($Cancel){Need ($audit.observedBuildStarted.buildId -ceq $body.buildId -and $audit.observedBuildStarted.payload.processId -eq $audit.processId -and $audit.observedStreamStarted.buildId -ceq $body.buildId) 'CtrlBreak observed a stale cached build generation'}
        $files=@{};$archived=Join-Path $caseOut 'artifacts';New-Item -ItemType Directory -Path $archived|Out-Null
        if($Expected -eq 0){
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and @($body.artifacts).Count -eq 4+$script:expectedCustomData) 'Cache fixture did not publish the exact four-file stream plus allowed CustomData'
            Need (@($body.artifacts|Where-Object{$_.Kind -ceq 'custom-data' -and [IO.Path]::GetExtension($_.Path) -ceq '.cdata'}).Count -eq $script:expectedCustomData) 'Payload fixture CustomData count differs'
            $published=Join-Path $Destination 'published'
            foreach($artifact in $body.artifacts){
                Need ((Within $published $artifact.Path) -and (Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'Cached artifact identity/containment differs'
                $relative=[IO.Path]::GetRelativePath($published,$artifact.Path);$copy=Join-Path $archived $relative
                New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($copy))|Out-Null
                Copy-Item -LiteralPath $artifact.Path -Destination $copy;$files[$relative]=$copy
            }
        }else{Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled cache build accepted artifacts'}
        Need (!(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$body.buildId)))) 'Cache build retained owned staging'
        Need (@(Get-ChildItem -LiteralPath $Destination -Directory|Where-Object{$_.Name -like '.cache-candidate-*' -or $_.Name -like '.action-cache-candidate-*'}).Count -eq 0) 'Asset-cache transaction retained its candidate directory'
        $entry.passed=$true;$entry.cache=$body.cache;$entry.buildId=$body.buildId
        return @{name=$Name;report=$body;files=$files;mod=$Mod;caseOut=$caseOut;destination=$Destination}
    }catch{
        foreach($name in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $caseOut $name) -Tail 30 -ErrorAction SilentlyContinue|Out-Host}
        throw
    }finally{
        if(Test-Path -LiteralPath $report){Copy-Item -LiteralPath $report -Destination "$caseOut/build-report.json"}
        if(Test-Path -LiteralPath $events){Copy-Item -LiteralPath $events -Destination "$caseOut/build.events.jsonl"}
    }
}
function Put([string]$Path,[string]$Text){New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($Path))|Out-Null;[IO.File]::WriteAllText($Path,$Text)}
function MakeWave([string]$Path,[int]$Frames=4096){
    # Same known integral PCM source shape as AudioSdkSupport.Wave; no codec,
    # compression library, external encoder or imprecise floating conversion.
    $stream=[IO.File]::Create($Path);$writer=[IO.BinaryWriter]::new($stream)
    try{
        $frames=$Frames;$channels=2;$dataBytes=$frames*$channels*2
        $writer.Write([Text.Encoding]::ASCII.GetBytes('RIFF'));$writer.Write([int](36+$dataBytes));$writer.Write([Text.Encoding]::ASCII.GetBytes('WAVEfmt '))
        $writer.Write([int]16);$writer.Write([int16]1);$writer.Write([int16]$channels);$writer.Write([int]22050);$writer.Write([int](22050*$channels*2));$writer.Write([int16]($channels*2));$writer.Write([int16]16)
        $writer.Write([Text.Encoding]::ASCII.GetBytes('data'));$writer.Write([int]$dataBytes)
        $values=[int16[]](0,1,-1,32767,-32768,0x1234)
        # Build one periodic source block, then write whole blocks. This keeps the
        # fixture setup linear without millions of PowerShell method calls.
        $block=[byte[]]::new(4096*2*2)
        for($i=0;$i -lt 4096*2;$i++){
            $sample=[BitConverter]::GetBytes([int16]$values[$i%$values.Length])
            $block[2*$i]=$sample[0];$block[2*$i+1]=$sample[1]
        }
        for($left=$dataBytes;$left -gt 0;$left-=$block.Length){$writer.Write($block,0,[Math]::Min($left,$block.Length))}
    }finally{$writer.Dispose();$stream.Dispose()}
    Need ((Get-Item -LiteralPath $Path).Length -eq 44+$Frames*2*2) 'PCM fixture length differs from its RIFF data size'
}
function AudioMod([string]$Path){
    Put "$Path/Data/Mod.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioFile id="BuildCachePcmWave" File="../Audio/probe.wav" PCCompression="NONE" IsStreamedOnPC="false"/><AudioFile id="BuildCacheXasWave" File="../Audio/stable.wav" PCCompression="XAS" IsStreamedOnPC="false"/></AssetDeclaration>'
    New-Item -ItemType Directory -Path "$Path/Audio"|Out-Null;MakeWave "$Path/Audio/probe.wav";MakeWave "$Path/Audio/stable.wav"
}
function W3xMod([string]$Path){
    # Reuse the existing source-XSD-valid recovered W3X integration fixtures,
    # retaining the exact animation node and its one hierarchy dependency.
    [xml]$animations=[IO.File]::ReadAllText("$root/SDK/Mods/W3xRecovery/Data/Animations.xml")
    [xml]$dependencies=[IO.File]::ReadAllText("$root/SDK/Mods/W3xRecovery/Data/Dependencies.xml")
    $ns=[Xml.XmlNamespaceManager]::new($animations.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
    $animation=$animations.SelectSingleNode('/ea:AssetDeclaration/ea:W3DAnimation[1]',$ns)
    $dns=[Xml.XmlNamespaceManager]::new($dependencies.NameTable);$dns.AddNamespace('ea','uri:ea.com:eala:asset')
    $hierarchy=$dependencies.SelectSingleNode('/ea:AssetDeclaration/ea:W3DHierarchy[1]',$dns)
    Need ($null -ne $animation -and $null -ne $hierarchy -and $animation.GetAttribute('Hierarchy') -ceq $hierarchy.GetAttribute('id')) 'W3X source fixture hierarchy closure changed'
    [xml]$meshes=[IO.File]::ReadAllText("$root/SDK/Mods/W3xRecovery/Data/Meshes.xml")
    $mns=[Xml.XmlNamespaceManager]::new($meshes.NameTable);$mns.AddNamespace('ea','uri:ea.com:eala:asset')
    $mesh=$meshes.SelectSingleNode('/ea:AssetDeclaration/ea:W3DMesh[1]',$mns)
    Need ($null -ne $mesh) 'W3X mesh fixture is missing'
    Put "$Path/Data/Mod.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="W3x.xml"/><Include type="all" source="W3xAnimation.xml"/><Include type="all" source="W3xMesh.xml"/></Includes></AssetDeclaration>'
    Put "$Path/Data/W3x.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$hierarchy.OuterXml+'</AssetDeclaration>')
    Put "$Path/Data/W3xAnimation.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$animation.OuterXml+'</AssetDeclaration>')
    Put "$Path/Data/W3xMesh.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$mesh.OuterXml+'</AssetDeclaration>')
}
function ParallelW3xMod([string]$Path){
    W3xMod $Path
    [xml]$document=[IO.File]::ReadAllText("$Path/Data/W3xAnimation.xml")
    $ns=[Xml.XmlNamespaceManager]::new($document.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
    $source=$document.SelectSingleNode('/ea:AssetDeclaration/ea:W3DAnimation',$ns)
    $includes='<Include type="all" source="W3x.xml"/>'
    # Sparse source channels are legal and expand to NumFrames inside the real
    # backend. Small prepared XML plus substantial native work makes overlapping
    # independent calls observable without a test sleep or scheduling hook.
    # The existing Timecoded reducer removes vector entries repeatedly; use
    # thousands of expanded frames without turning Debug acceptance into a
    # quadratic-size stress benchmark.
    foreach($item in @(@{name='ParallelLong';frames=4096},@{name='ParallelShort';frames=3072})){
        $node=$source.CloneNode($true);$node.SetAttribute('id',$item.name);$node.SetAttribute('NumFrames',[string]$item.frames)
        Put "$Path/Data/$($item.name).xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$node.OuterXml+'</AssetDeclaration>')
        $includes+='<Include type="all" source="'+$item.name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function ParallelAudioMod([string]$Path){
    # Streamed XAS bounds each pending buffer; unstreamed giant fixtures would
    # exercise the encoder's quadratic RemoveRange path rather than concurrency.
    $includes=''
    New-Item -ItemType Directory -Path "$Path/Audio" -Force|Out-Null
    foreach($item in @(@{name='AudioLong';frames=196608},@{name='AudioShort';frames=131072})){
        MakeWave "$Path/Audio/$($item.name).wav" $item.frames
        Put "$Path/Data/$($item.name).xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioFile id="'+$item.name+'" File="../Audio/'+$item.name+'.wav" PCCompression="XAS" IsStreamedOnPC="true"/></AssetDeclaration>')
        $includes+='<Include type="all" source="'+$item.name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function Mp3Mod([string]$Path){
    $includes='';$source=[IO.File]::ReadAllText("$root/recovery/tests/AudioMp3Fixtures.cs")
    New-Item -ItemType Directory -Force -Path "$Path/Audio"|Out-Null
    foreach($name in @('Mono','Stereo')){
        $match=[regex]::Match($source,('public const string '+$name+' = "([^"]+)"'))
        Need $match.Success "Existing MP3 fixture is missing: $name"
        [IO.File]::WriteAllBytes("$Path/Audio/$name.mp3",[Convert]::FromBase64String($match.Groups[1].Value))
        Put "$Path/Data/$name.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioFileMP3Passthrough id="Mp3'+$name+'" File="../Audio/'+$name+'.mp3" IsStreamed="true"/></AssetDeclaration>')
        $includes+='<Include type="all" source="'+$name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function ParallelXmlMod([string]$Path){
    $includes=''
    foreach($item in @(@{name='AptLong';bytes=12*1024*1024},@{name='AptShort';bytes=256*1024})){
        $input="$Path/Data/$($item.name).bin"
        New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($input))|Out-Null
        $stream=[IO.File]::Create($input)
        try{
            $block=[byte[]]::new(65536)
            for($i=0;$i -lt $block.Length;$i++){$block[$i]=[byte]($i%251)}
            for($left=$item.bytes;$left -gt 0;$left-=$block.Length){$stream.Write($block,0,[Math]::Min($left,$block.Length))}
        }finally{$stream.Dispose()}
        Put "$Path/Data/$($item.name).xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AptAptData id="'+$item.name+'" File="'+$item.name+'.bin"/></AssetDeclaration>')
        $includes+='<Include type="all" source="'+$item.name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function CertifiedParallelEvidence($Case,[string]$Type){
    $cache=$Case.report.cache
    Need ($cache.requestedJobs -eq 2 -and $cache.cacheMode -ceq 'Disabled' -and $cache.actionHits -eq 0 -and $cache.backendCalls -eq 2 -and $cache.maxBackendConcurrency -eq 2) "Real bounded $Type parallel calls were not observed"
    $intervals=@($cache.backendIntervals|Where-Object{$_.typeName -ceq $Type})
    Need ($intervals.Count -eq 2) "Both real $Type backend intervals must be recorded"
    $left=$intervals[0];$right=$intervals[1]
    Need ($left.sourcePath -cne $right.sourcePath -and ![string]::IsNullOrWhiteSpace($left.sourcePath) -and ![string]::IsNullOrWhiteSpace($right.sourcePath)) 'Parallel calls must belong to independent source documents'
    Need ($left.threadId -ne $right.threadId -and $left.startTicks -lt $left.endTicks -and $right.startTicks -lt $right.endTicks -and $left.startTicks -lt $right.endTicks -and $right.startTicks -lt $left.endTicks) "Actual $Type backend invocation intervals did not overlap"
    return @{passed=$true;requestedJobs=2;maxBackendConcurrency=$cache.maxBackendConcurrency;backendCalls=$cache.backendCalls;intervals=$intervals;scope=('Independent '+$Type+' source documents, real backend overlap with persistent cache disabled, strict serial/parallel raw output comparison')}
}
function ParallelEvidence($Case){
    $cache=$Case.report.cache
    Need ($cache.requestedJobs -eq 2 -and $cache.cacheMode -ceq 'Disabled' -and $cache.actionHits -eq 0 -and $cache.maxBackendConcurrency -ge 2 -and $cache.maxBackendConcurrency -le 2) 'Cold no-cache build did not execute bounded real backend concurrency'
    $intervals=@($cache.backendIntervals|Where-Object{$_.typeName -ceq 'W3DAnimation'})
    Need ($intervals.Count -eq 2 -and $cache.backendCalls -ge 3) 'Parallel fixture did not execute both real W3X animations and hierarchy'
    $left=$intervals[0];$right=$intervals[1]
    Need ($left.sourcePath -cne $right.sourcePath -and ![string]::IsNullOrWhiteSpace($left.sourcePath) -and ![string]::IsNullOrWhiteSpace($right.sourcePath)) 'Parallel calls did not originate in independent documents of the same stream'
    Need ($left.startTicks -lt $left.endTicks -and $right.startTicks -lt $right.endTicks -and $left.startTicks -lt $right.endTicks -and $right.startTicks -lt $left.endTicks) 'Actual cross-document backend call intervals did not overlap'
    return @{passed=$true;requestedJobs=2;maxBackendConcurrency=$cache.maxBackendConcurrency;backendCalls=$cache.backendCalls;intervals=$intervals;scope='One stream, independent W3DAnimation source documents, actual native invocation overlap with persistent cache disabled'}
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Production action-cache PCM/XAS AudioFile complete CDATA and W3X animation/hierarchy/mesh; precise one-action edits, Additional zero compilation, corrupt result and missing CDATA repair, exact four-stream/CDATA no-cache equality, bounded real cross-document native, leased APT XML and streamed XAS AudioFile overlap with jobs2; not a new original-DLL/SDK gate'}
try{
    Need ($state.sourceCommit -cmatch '^[0-9a-f]{40}$') 'Cannot identify source commit'
    if($Preflight){
        AudioMod (Join-Path $short 'a');W3xMod (Join-Path $short 'w');ParallelW3xMod (Join-Path $short 'p');ParallelAudioMod (Join-Path $short 'pa');ParallelXmlMod (Join-Path $short 'px');Mp3Mod (Join-Path $short 'mp3')
        $check=@'
import pathlib, runpy, sys
from lxml import etree as E
scripts=pathlib.Path(sys.argv[1]); work=pathlib.Path(sys.argv[2])
module=runpy.run_path(str(scripts/'GenerateXmlSdkModA.py'), run_name='action_payload_schema')
schema=module['aggregate_schema'](); ns={'ea':'uri:ea.com:eala:asset'}
for path in sorted(work.glob('*/Data/*.xml')):
    schema.assertValid(E.parse(str(path)))
audio=E.parse(str(work/'a/Data/Mod.xml'))
assert sorted(audio.xpath('/ea:AssetDeclaration/ea:AudioFile/@PCCompression',namespaces=ns))==['NONE','XAS']
assert len((work/'a/Audio/probe.wav').read_bytes())==44+4096*2*2
assert (work/'a/Audio/probe.wav').read_bytes()==(work/'a/Audio/stable.wav').read_bytes()
assert b'FrameRate="30"' in (work/'w/Data/W3xAnimation.xml').read_bytes()
animations=[]
for name in ('ParallelLong','ParallelShort'):
    node=E.parse(str(work/('p/Data/'+name+'.xml'))).getroot()[0]
    frames=int(node.get('NumFrames')); channels=node.find('ea:Channels',ns)
    assert 2*1024*1024 <= frames*len(channels)*256 <= 64*1024*1024
    assert max(len(channel) for channel in channels)<frames
    animations.append(node.get('id'))
assert len(set(animations))==2
for name,frames in (('AudioLong',196608),('AudioShort',131072)):
    node=E.parse(str(work/('pa/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}AudioFile'
    assert node.get('PCCompression')=='XAS' and node.get('IsStreamedOnPC')=='true'
    data=(work/('pa/Audio/'+name+'.wav')).read_bytes()
    assert len(data)==44+frames*2*2 and len(data)*64 < 64*1024*1024
    assert int.from_bytes(data[40:44],'little')==len(data)-44
for name,size in (('AptLong',12*1024*1024),('AptShort',256*1024)):
    node=E.parse(str(work/('px/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}AptAptData' and node.get('File')==name+'.bin'
    assert (work/('px/Data/'+name+'.bin')).stat().st_size==size
    assert size*4 < 64*1024*1024
for name in ('Mono','Stereo'):
    assert len((work/('mp3/Audio/'+name+'.mp3')).read_bytes())>100
    node=E.parse(str(work/('mp3/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}AudioFileMP3Passthrough'
print('PASS payload fixture XSD, exact PCM/XAS and APT reads, CDATA sources, bounded sparse W3X expansion and streamed Audio/APT cross-document workload budgets')
'@
        & python -c $check $PSScriptRoot $short *> "$out/fixture-preflight.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/fixture-preflight.log" -Tail 30|Out-Host}
        Need ($LASTEXITCODE -eq 0) 'Action-cache payload fixture preflight failed'
        $state.preflight=$true;$state.scope='Source-XSD, exact source-byte and parallel resource-estimate fixture preflight only; no production execution';$state.succeeded=$true;$global:LASTEXITCODE=0
        return
    }
    Need (!(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'reuse.json'))) 'Payload cache acceptance requires current production EXE bytes'

    $state.testSourceCommit=$state.sourceCommit;$state.binarySourceCommit=$state.sourceCommit
    $replayFiles=@{};$replayPath=Join-Path $CompilerDirectory 'candidate-replay.json'
    if(Test-Path -LiteralPath $replayPath){
        $replay=Get-Content -LiteralPath $replayPath -Raw|ConvertFrom-Json
        $fixedSource='fb6f67a0aaf5889bcd950485c41a883bc5807523'
        Need ($replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.sourceCommit -ceq $fixedSource -and $replay.binarySourceCommit -ceq $fixedSource -and $replay.candidateCommit -ceq $state.sourceCommit -and $replay.testSourceCommit -ceq $state.sourceCommit -and [string]$replay.runId -ceq '40360344360' -and [string]$replay.runAttempt -ceq '1' -and $replay.archiveSha256 -ceq 'd1308d63759466789eb657ac3389a78f2274c52d261b0e078144923af435d8de') 'Candidate replay identity or preparation contract differs'
        # The fixed archive checker verifies API status, successful scopes and
        # raw receipts. Independently re-evaluate unchanged production inputs.
        $closureCode='import sys;sys.path.insert(0,sys.argv[1]);from PrepareCompilerReplay import closure;closure(sys.argv[2],sys.argv[3],sys.argv[4])'
        & python -c $closureCode $PSScriptRoot $root $fixedSource $state.sourceCommit *> "$out/replay-closure.log"
        Need ($LASTEXITCODE -eq 0) 'Candidate replay production-source closure rejected'
        $expectedFiles=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach($configuration in @('Debug','Release')){foreach($fileName in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){$null=$expectedFiles.Add($configuration+'/bin/'+$fileName)}}
        Need (@($replay.files).Count -eq 6) 'Candidate replay requires exactly six runtime identities'
        foreach($identity in $replay.files){
            Need ($expectedFiles.Remove([string]$identity.path) -and $identity.sha256 -cmatch '^[0-9a-f]{64}$') 'Duplicate or unexpected candidate replay runtime identity'
            Need ((Hash (Join-Path $CompilerDirectory $identity.path)) -ceq $identity.sha256) 'Candidate replay runtime bytes differ from validated archive receipt'
            $replayFiles[[string]$identity.path]=[string]$identity.sha256
        }
        Need ($expectedFiles.Count -eq 0) 'Candidate replay runtime set is incomplete'
        $state.binarySourceCommit=$fixedSource;$state.candidateReplay=$replay
        $state.scope+='; fixed binarySourceCommit replay with separately recorded current testSourceCommit and unchanged production inputs; this is not a current production rebuild'
    }
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    $state.scriptSha256=Hash $PSCommandPath;$state.controllerSha256=Hash $control
    $state.sourceInputs=@('recovery/tests/AudioSdkSupport.cs','recovery/tests/AudioMp3Fixtures.cs','SDK/Mods/W3xRecovery/Data/Animations.xml','SDK/Mods/W3xRecovery/Data/Dependencies.xml','SDK/Mods/W3xRecovery/Data/Meshes.xml')|ForEach-Object{@{path=$_;sha256=(Hash (Join-Path $root $_))}}
    foreach($mode in @('Debug','Release')){
        $script:modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $modeWork=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName
        $script:runtime=(New-Item -ItemType Directory -Path (Join-Path $modeWork 'r')).FullName
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){
            Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime
            if($replayFiles.Count -gt 0){Need ((Hash (Join-Path $runtime $file)) -ceq $replayFiles[$mode+'/bin/'+$file]) 'Copied candidate replay runtime differs from validated identity'}
        }
        $script:exe=Join-Path $runtime 'NewModCompiler.exe'
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$modeOut/prepare-texture.log";Need ($LASTEXITCODE -eq 0) 'Payload runtime Texture deployment failed'
        $script:modeState=@{configuration=$mode;passed=$false;exeSha256=(Hash $exe);cases=@();fixtures=@()};$state.modes+=,$modeState
        $script:sdk=(New-Item -ItemType Directory -Path (Join-Path $modeWork 's')).FullName
        New-Item -ItemType Directory -Path "$sdk/tools","$sdk/Schemas"|Out-Null
        Copy-Item -LiteralPath "$root/SDK/Schemas/xsd" -Destination "$sdk/Schemas" -Recurse
        Copy-Item -LiteralPath "$root/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$sdk/tools"
        foreach($family in @('audio','w3x')){
            $mod=Join-Path $modeWork $family;$cachedOut=Join-Path $modeWork ($family+'-cache');$plainOut=Join-Path $modeWork ($family+'-plain')
            if($family -eq 'audio'){AudioMod $mod;$script:expectedCustomData=2}else{W3xMod $mod;$script:expectedCustomData=0}
            $before=RunCli ($family+'-disabled') $mod $plainOut $false;Cache $before 'disabled'
            $cold=RunCli ($family+'-cold') $mod $cachedOut $true -Extra @('--jobs=2');Cache $cold 'cold';SameOutputs $before $cold
            $hot=RunCli ($family+'-hot') $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $before $hot
            $hotJobs=RunCli ($family+'-hot-jobs2') $mod $cachedOut $true -Extra @('--jobs=2');Cache $hotJobs 'hot';SameOutputs $before $hotJobs
            Need ($hotJobs.report.cache.requestedJobs -eq 2 -and $hotJobs.report.cache.backendCalls -eq 0 -and $hotJobs.report.cache.maxBackendConcurrency -eq 0) 'Changing jobs invalidated action identities or reported cache hits as actual backend calls'
            $initialActionKeys=@(Get-ChildItem -LiteralPath (Join-Path $cachedOut '.cache/action-cache-v1') -Directory|Where-Object{$_.Name -cmatch '^[0-9a-f]{64}$'}|ForEach-Object{$_.Name})
            if($family -eq 'audio'){
                $input="$mod/Audio/probe.wav";$stamp=[IO.File]::GetLastWriteTimeUtc($input);$bytes=[IO.File]::ReadAllBytes($input);$length=$bytes.Length
                # Change one PCM sample, not RIFF metadata; force payload change.
                $bytes[44]=0x34;$bytes[45]=0x12
                [IO.File]::WriteAllBytes($input,$bytes);[IO.File]::SetLastWriteTimeUtc($input,$stamp)
                Need ((Get-Item -LiteralPath $input).Length -eq $length -and [IO.File]::GetLastWriteTimeUtc($input) -eq $stamp) 'Audio mutation did not preserve input length/mtime'
            }else{
                $input="$mod/Data/W3xAnimation.xml";$stamp=[IO.File]::GetLastWriteTimeUtc($input)
                SameLengthMutation $input 'FrameRate="30"' 'FrameRate="31"'
            }
            $changed=RunCli ($family+'-content-invalidated') $mod $cachedOut $true -Extra @('--jobs=2');Cache $changed 'partial';DifferentOutputs $hot $changed
            $changedPlain=RunCli ($family+'-changed-disabled') $mod $plainOut $false;Cache $changedPlain 'disabled';SameOutputs $changedPlain $changed
            $changedHot=RunCli ($family+'-changed-hot') $mod $cachedOut $true -Extra @('--jobs=2');Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot
            if($family -eq 'audio'){
                $customKeys=@($hot.files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
                $changedKeys=@($changed.files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
                $unchangedCustom=@($customKeys|Where-Object{$changed.files.ContainsKey($_) -and (Hash $hot.files[$_]) -ceq (Hash $changed.files[$_])})
                Need ($customKeys.Count -eq 2 -and $changedKeys.Count -eq 2 -and $unchangedCustom.Count -eq 1) 'One WAV sample mutation must change its complete PCM CDATA while preserving the independent XAS result'
            }
            New-Item -ItemType Directory -Force -Path "$mod/Additional"|Out-Null
            [IO.File]::WriteAllText("$mod/Additional/only-package.txt",'not read by any compiler action')
            $additional=RunCli ($family+'-additional-only') $mod $cachedOut $true -Extra @('--jobs=2');Cache $additional 'hot';SameOutputs $changedPlain $additional
            Need ($additional.report.cache.backendCalls -eq 0) 'Unread Additional member triggered backend compilation in an entirely certified stream'
            $store=Join-Path $cachedOut '.cache/action-cache-v1'
            $entries=@(Get-ChildItem -LiteralPath $store -Directory|Where-Object{$_.Name -cmatch '^[0-9a-f]{64}$'})
            Need ($entries.Count -gt 0) 'Action cache did not persist independently addressed entries'
            $mutatedEntries=@($entries|Where-Object{$initialActionKeys -cnotcontains $_.Name})
            Need ($mutatedEntries.Count -eq 1) 'A single payload edit installed more than one new action identity'
            $entry=$mutatedEntries[0]
            if($family -eq 'audio'){
                $members=@(Get-ChildItem -LiteralPath (Join-Path $entry.FullName 'files') -File|Sort-Object Name)
                Need ($members.Count -eq 2) 'Audio cache entry did not preserve metadata/buffers plus complete CDATA'
                # Result metadata/buffers sort before the complete CDATA member.
                Remove-Item -LiteralPath $members[-1].FullName
            }else{
                $member=@(Get-ChildItem -LiteralPath (Join-Path $entry.FullName 'files') -File)[0]
                $bytes=[IO.File]::ReadAllBytes($member.FullName);Need ($bytes.Length -gt 0) 'Expected a nonempty result contract'
                $bytes[0]=$bytes[0] -bxor 0x7f;[IO.File]::WriteAllBytes($member.FullName,$bytes)
            }
            $repaired=RunCli ($family+'-damaged-result-repaired') $mod $cachedOut $true -Extra @('--jobs=2');Cache $repaired 'partial';SameOutputs $changedPlain $repaired
            $repairedHot=RunCli ($family+'-repaired-hot') $mod $cachedOut $true -Extra @('--jobs=2');Cache $repairedHot 'hot';SameOutputs $changedPlain $repairedHot
            $modeState.fixtures+=,@{family=$family;input=$input;mutatedInputSha256=(Hash $input);sameLengthSameTimestamp=$true;hotAssetHits=$hot.report.cache.hits;changedHotAssetHits=$changedHot.report.cache.hits;completeCustomDataFiles=$script:expectedCustomData;allRawOutputsEqualToIndependentNoCache=$true}
        }
        $parallelMod=Join-Path $modeWork 'parallel';ParallelW3xMod $parallelMod;$script:expectedCustomData=0
        $serial=RunCli 'w3x-parallel-control' $parallelMod (Join-Path $modeWork 'parallel-serial') $false -Extra @('--jobs=1');Cache $serial 'disabled'
        $parallel=RunCli 'w3x-cross-document-parallel' $parallelMod (Join-Path $modeWork 'parallel-jobs2') $false -Extra @('--jobs=2');Cache $parallel 'disabled';SameOutputs $serial $parallel
        $modeState.parallel=ParallelEvidence $parallel
        $modeState.certifiedParallel=@()
        foreach($family in @('audio','xml')){
            $fixture=Join-Path $modeWork ('parallel-'+$family)
            if($family -eq 'audio'){ParallelAudioMod $fixture;$script:expectedCustomData=2;$type='AudioFile'}
            else{ParallelXmlMod $fixture;$script:expectedCustomData=0;$type='AptAptData'}
            $controlCase=RunCli ($family+'-parallel-control') $fixture (Join-Path $modeWork ($family+'-parallel-serial')) $false -Extra @('--jobs=1');Cache $controlCase 'disabled'
            Need ($controlCase.report.cache.backendCalls -eq 2 -and $controlCase.report.cache.maxBackendConcurrency -eq 1) 'Serial control did not execute exactly two serial real backend calls'
            $parallelCase=RunCli ($family+'-cross-document-parallel') $fixture (Join-Path $modeWork ($family+'-parallel-jobs2')) $false -Extra @('--jobs=2');Cache $parallelCase 'disabled';SameOutputs $controlCase $parallelCase
            $modeState.certifiedParallel+=,(CertifiedParallelEvidence $parallelCase $type)
            $cachedDestination=Join-Path $modeWork ($family+'-parallel-cached')
            $parallelCold=RunCli ($family+'-parallel-cold') $fixture $cachedDestination $true -Extra @('--jobs=2');Cache $parallelCold 'cold';SameOutputs $controlCase $parallelCold
            $parallelHot=RunCli ($family+'-parallel-hot') $fixture $cachedDestination $true -Extra @('--jobs=2');Cache $parallelHot 'hot';SameOutputs $controlCase $parallelHot
            Need ($parallelHot.report.cache.backendCalls -eq 0 -and $parallelHot.report.cache.maxBackendConcurrency -eq 0) 'Parallel hot hits were reported as backend calls'
            $changedInput=if($family -eq 'audio'){"$fixture/Audio/AudioLong.wav"}else{"$fixture/Data/AptLong.bin"}
            $stamp=[IO.File]::GetLastWriteTimeUtc($changedInput);$bytes=[IO.File]::ReadAllBytes($changedInput);$length=$bytes.Length
            $offset=if($family -eq 'audio'){44}else{0}
            $bytes[$offset]=$bytes[$offset] -bxor 0x34;[IO.File]::WriteAllBytes($changedInput,$bytes);[IO.File]::SetLastWriteTimeUtc($changedInput,$stamp)
            Need ((Get-Item -LiteralPath $changedInput).Length -eq $length -and [IO.File]::GetLastWriteTimeUtc($changedInput) -eq $stamp) 'Parallel payload mutation must retain exact length/mtime'
            $partial=RunCli ($family+'-parallel-single-edit') $fixture $cachedDestination $true -Extra @('--jobs=2');Cache $partial 'partial';DifferentOutputs $parallelHot $partial
            $changedControl=RunCli ($family+'-parallel-edited-control') $fixture (Join-Path $modeWork ($family+'-parallel-serial')) $false -Extra @('--jobs=1');Cache $changedControl 'disabled';SameOutputs $changedControl $partial
            Need ($partial.report.cache.backendCalls -eq 1) 'One changed parallel payload must execute exactly one backend'

        }
        $mp3=Join-Path $modeWork 'mp3';Mp3Mod $mp3;$script:expectedCustomData=2
        $mp3Serial=RunCli 'mp3-serial-control' $mp3 (Join-Path $modeWork 'mp3-serial') $false -Extra @('--jobs=1');Cache $mp3Serial 'disabled'
        $mp3Parallel=RunCli 'mp3-parallel' $mp3 (Join-Path $modeWork 'mp3-jobs2') $false -Extra @('--jobs=2');Cache $mp3Parallel 'disabled';SameOutputs $mp3Serial $mp3Parallel
        Need ($mp3Parallel.report.cache.backendCalls -eq 2 -and $mp3Parallel.report.cache.backendCallsByType.AudioFileMP3Passthrough -eq 2) 'MP3 parallel mode did not execute both real passthrough calls'
        $mp3Cold=RunCli 'mp3-parallel-cold' $mp3 (Join-Path $modeWork 'mp3-cache') $true -Extra @('--jobs=2');Cache $mp3Cold 'cold';SameOutputs $mp3Serial $mp3Cold
        $mp3Hot=RunCli 'mp3-parallel-hot' $mp3 (Join-Path $modeWork 'mp3-cache') $true -Extra @('--jobs=2');Cache $mp3Hot 'hot';SameOutputs $mp3Serial $mp3Hot
        foreach($case in @($mp3Serial,$mp3Parallel,$mp3Cold,$mp3Hot)){
            $payloads=@($case.files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
            Need ($payloads.Count -eq 2) 'MP3 passthrough omitted complete CDATA'
            foreach($name in @('Mono','Stereo')){Need (@($payloads|Where-Object{[PayloadRawBytes]::Equal($case.files[$_],"$mp3/Audio/$name.mp3")}).Count -eq 1) 'Passthrough CDATA must equal exact original MP3 bytes'}
        }
        $modeState.passed=$true
    }
    foreach($file in $state.sourceInputs){Need ((Hash (Join-Path $root $file.path)) -ceq $file.sha256) 'Original payload fixture source changed'}
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{
    $state|ConvertTo-Json -Depth 18|Set-Content "$out/validation.json"
    try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "Payload cache work cleanup failed: $_"}
}
