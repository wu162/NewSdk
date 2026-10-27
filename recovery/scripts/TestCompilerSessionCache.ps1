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
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite session-cache evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('c'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $directory=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$path=[IO.Path]::GetFullPath($Path)
    $path.StartsWith($directory+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function EqualBytes([string]$Left,[string]$Right){
    $leftBytes=[IO.File]::ReadAllBytes($Left);$rightBytes=[IO.File]::ReadAllBytes($Right)
    Need ($leftBytes.Length -eq $rightBytes.Length) 'No-cache/cache artifact lengths differ'
    for($i=0;$i -lt $leftBytes.Length;$i++){Need ($leftBytes[$i] -eq $rightBytes[$i]) "No-cache/cache raw artifact byte differs at $i"}
}
function SameOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'No-cache/cache artifact counts differ'
    foreach($key in $Left.files.Keys){Need ($Right.files.ContainsKey($key)) "Cache output omitted $key";EqualBytes $Left.files[$key] $Right.files[$key]}
}
function DifferentOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'Changed fixture output layout differs'
    $changed=$false
    foreach($key in $Left.files.Keys){if(!$Right.files.ContainsKey($key)){$changed=$true}elseif((Hash $Left.files[$key]) -cne (Hash $Right.files[$key])){$changed=$true}}
    Need $changed 'Input mutation did not exercise any observable compiled output change'
}
function CacheFiles([string]$Destination){
    $files=@{};$cache=Join-Path $Destination '.cache'
    if(Test-Path -LiteralPath $cache){Get-ChildItem -LiteralPath $cache -File -Recurse|ForEach-Object{$files[$_.FullName.Substring($cache.Length)]=Hash $_.FullName}}
    return $files
}
function CacheUnchanged([string]$Destination,$Before){
    $after=CacheFiles $Destination;Need ($after.Count -eq $Before.Count) 'Failed/cancelled build persisted additional session-cache files'
    foreach($key in $Before.Keys){Need ($after.ContainsKey($key) -and $after[$key] -ceq $Before[$key]) 'Failed/cancelled build changed previously confirmed session-cache bytes'}
}
function Cache($Case,[string]$Expected){
    $statistics=$Case.report.cache
    Need ($null -ne $statistics) 'Durable report omitted cache statistics'
    foreach($name in @('documentReuse','xmlParse','instanceCompile','sessionPackageHits','sessionPackageMisses')){
        Need ($null -ne $statistics.$name -and $statistics.$name -ge 0) "Actual session counter missing: $name"
    }
    Need ($statistics.hits -eq 0 -and $statistics.misses -eq 0) '/sc-only build used /bc statistics'
    if($Expected -eq 'disabled'){
        Need (!$statistics.sessionCache -and $statistics.sessionPackageHits -eq 0 -and $statistics.sessionPackageMisses -eq 0 -and $statistics.documentReuse -eq 0) 'Disabled session cache recorded reuse';return
    }
    Need ($statistics.sessionCache -eq $true -and $statistics.status -ceq 'SessionPackage') 'Session cache was accepted without production enablement'
    if($Expected -eq 'cold'){
        Need ($statistics.sessionPackageHits -eq 0 -and $statistics.sessionPackageMisses -gt 0 -and $statistics.documentReuse -eq 0 -and $statistics.xmlParse -gt 0 -and $statistics.instanceCompile -gt 0) 'Cold session cache did not parse and compile actual assets'
    }elseif($Expected -eq 'hot'){
        Need ($statistics.sessionPackageHits -gt 0 -and $statistics.sessionPackageMisses -eq 0 -and $statistics.documentReuse -gt 0 -and $statistics.xmlParse -lt $script:hotParseCeiling -and $statistics.instanceCompile -eq 0) 'Hot session package did not reduce XML parsing and reuse compiled instances'
    }else{throw "Unknown session expectation: $Expected"}
}
function SessionEntries([string]$Destination){
    $entries=@();$cache=Join-Path $Destination '.cache/session-packages-v1'
    if(!(Test-Path -LiteralPath $cache)){return $entries}
    foreach($index in @(Get-ChildItem -LiteralPath $cache -Filter index.bin -File -Recurse)){
        $reader=[IO.BinaryReader]::new([IO.File]::OpenRead($index.FullName),[Text.Encoding]::UTF8)
        try{
            Need ($reader.ReadString() -ceq 'NewModCompiler.SessionPackage.v1') 'Session cache format identity differs'
            $owner=$reader.ReadString();$count=$reader.ReadInt32();Need ($count -gt 0 -and $count -lt 100000) 'Invalid session index size'
            for($i=0;$i -lt $count;$i++){
                $name=$reader.ReadString();$length=$reader.ReadInt64();$hash=$reader.ReadString()
                $entries+=,@{owner=$owner;name=$name.Replace('\','/');path=(Join-Path $index.DirectoryName ('files/'+$i.ToString('x8')));index=$index.FullName;length=$length;sha256=$hash}
            }
            Need ($reader.BaseStream.Position -eq $reader.BaseStream.Length) 'Session index retained trailing bytes'
        }finally{$reader.Dispose()}
    }
    return $entries
}
function DamagePackage([string]$Destination,[string]$Kind){
    $entries=@(SessionEntries $Destination)
    if($Kind -eq 'xml'){$selected=@($entries|Where-Object{$_.name -ceq 'hashes/session.xml'})}
    elseif($Kind -eq 'intermediate'){$selected=@($entries|Where-Object{$_.name.StartsWith('intermediate/',[StringComparison]::Ordinal)})}
    elseif($Kind -eq 'cdata'){$selected=@($entries|Where-Object{$_.name.StartsWith('streams/',[StringComparison]::Ordinal) -and $_.name.EndsWith('.cdata',[StringComparison]::Ordinal)})}
    else{throw 'Unknown package corruption kind'}
    Need ($selected.Count -gt 0) "Session package omitted required $Kind payload"
    # Damage every stream package: the build must report complete cold misses,
    # never a partially restored document tree. Preserve raw corrupted bytes.
    foreach($entry in $selected){
        Need ((Hash $entry.path) -ceq $entry.sha256 -and (Get-Item -LiteralPath $entry.path).Length -eq $entry.length) 'Pre-damage session payload identity differs'
        if($Kind -eq 'xml'){[IO.File]::WriteAllText($entry.path,'<SessionCache>broken')}else{Remove-Item -LiteralPath $entry.path}
    }
    if($Kind -eq 'xml'){
        # Keep the outer receipt valid to reach SessionPackage's XML validator,
        # independently of the payload-SHA and missing-file checks below.
        foreach($index in @($selected.index|Select-Object -Unique)){
            $items=@($entries|Where-Object{$_.index -ceq $index})
            $writer=[IO.BinaryWriter]::new([IO.File]::Open($index,[IO.FileMode]::Create),[Text.Encoding]::UTF8)
            try{
                $writer.Write('NewModCompiler.SessionPackage.v1');$writer.Write([string]$items[0].owner);$writer.Write([int]$items.Count)
                foreach($item in $items){$writer.Write([string]$item.name);$writer.Write([long](Get-Item -LiteralPath $item.path).Length);$writer.Write([string](Hash $item.path))}
            }finally{$writer.Dispose()}
        }
    }
    return $selected
}
function MakeMod([string]$Path,[string]$Probe){
    Put "$Path/Data/Armor.xml" $Probe
    Put "$Path/Data/Defs.xml" '<Defines xmlns="uri:ea.com:eala:asset"><Define name="SESSION_SCALE" value="83.25"/></Defines>'
    Put "$Path/Data/XArmor.xml" '<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="SessionXIncludeArmor" Default="22.5%"/>'
    Put "$Path/Data/Mod.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xi="http://www.w3.org/2001/XInclude"><Includes><Include type="all" source="Armor.xml"/></Includes><xi:include href="Defs.xml"/><xi:include href="XArmor.xml"/><ArmorTemplate id="SessionExpressionArmor" DamageScalar="=$SESSION_SCALE"/><PathMusicEvent id="CacheEvent" PathfinderEventHeader="events.h" IsXferable="true"/></AssetDeclaration>'
    Put "$Path/Data/events.h" "#define PATH_EVENT_CacheEvent 0x12345678`r`n"
    Put "$Path/Data/AptUI/Payload.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AptAptData id="SessionPayload" File="payload.bin"/></AssetDeclaration>'
    [IO.File]::WriteAllBytes("$Path/Data/AptUI/payload.bin",[byte[]](1,2,3,4,5,6,7))
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
function RunCli([string]$Name,[string]$Mod,[string]$Destination,[bool]$Enabled,[int]$Expected=0,[bool]$Cancel=$false,[string]$HoldReadFile=$null,[string[]]$Extra=@(),[bool]$BuildEnabled=$false){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    New-Item -ItemType Directory -Force -Path $Destination|Out-Null
    $arguments=@('build','--mod',$Mod,'--sdk-root',$script:sdk,'--out',$Destination,'--package-format','loose','--package-content',$script:contents,('--sc='+$Enabled.ToString().ToLowerInvariant()),('--bc='+$BuildEnabled.ToString().ToLowerInvariant()))+$Extra
    $events=Join-Path $Destination 'build.events.jsonl';$report=Join-Path $Destination 'build-report.json'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    if($HoldReadFile){$request.holdReadFile=$HoldReadFile}
    $request|ConvertTo-Json -Depth 6|Set-Content "$caseOut/request.json"
    & $script:control "$caseOut/request.json" "$caseOut/control.json" *> "$caseOut/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$caseOut/control.json" -Raw|ConvertFrom-Json
    $entry=@{name=$Name;arguments=$arguments;expectedExit=$Expected;passed=$false;controller=$audit};$script:modeState.cases+=,$entry
    try{
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "Session-cache CLI failed: $Name"
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
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and @($body.artifacts).Count -eq $script:expectedArtifacts) 'Session fixture did not publish exact stream/CDATAs count'
            $published=Join-Path $Destination 'published'
            foreach($artifact in $body.artifacts){
                Need ((Within $published $artifact.Path) -and (Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'Cached artifact identity/containment differs'
                $relative=[IO.Path]::GetRelativePath($published,$artifact.Path);$copy=Join-Path $archived $relative
                New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($copy))|Out-Null
                Copy-Item -LiteralPath $artifact.Path -Destination $copy;$files[$relative]=$copy
            }
        }else{Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled cache build accepted artifacts'}
        Need (!(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$body.buildId)))) 'Cache build retained owned staging'
        Need (@(Get-ChildItem -LiteralPath $Destination -Directory|Where-Object{$_.Name -like '.cache-candidate-*'}).Count -eq 0) 'Session-cache transaction retained its candidate directory'
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
function OriginalOracle($Case,[string]$Name){
    $baseline=@();$index=0
    foreach($task in $Case.report.plan.Tasks){
        $outputs=@{};$prefix=$task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.InputPath)
        foreach($key in $Case.files.Keys){if($key.Replace('\','/').StartsWith($prefix+'/',[StringComparison]::Ordinal)){$outputs[[IO.Path]::GetExtension($key).Substring(1)]=$Case.files[$key]}}
        Need ($outputs.Count -eq 4) 'Cache original oracle requires exact four files per task'
        $baseline+=,@{name=$(if($index++ -eq 0){'mod-a'}else{'mod-b'});modRoot=$Case.mod;inputPath=$task.InputPath;sdkRoot=$script:sdk;originalToolsRoot=(Join-Path $root 'SDK/tools');stagingDirectory=([IO.Path]::GetDirectoryName($outputs.manifest));outputs=$outputs;expectedTextures=@();scope='Current session-cache fixture, strict original BAB raw four-output comparison; no normalization'}
    }
    $fixture=Join-Path $script:modeOut "$Name.json"
    @{succeeded=$true;sourceCommit=$script:state.sourceCommit;runId=$env:GITHUB_RUN_ID;baselineCases=$baseline;scope='Session-cache Armor/include/define/inheritance and file-backed APT stream; not full SDK'}|ConvertTo-Json -Depth 9|Set-Content $fixture
    & "$PSScriptRoot/TestCompilerBackendBaseline.ps1" -FixtureResult $fixture -OutputDirectory (Join-Path $script:modeOut $Name) -TimeoutSeconds $TimeoutSeconds -ExpectedCaseCount $baseline.Count
    Need ($LASTEXITCODE -eq 0) 'Build-cache independent original BAB oracle failed'
}
function Put([string]$Path,[string]$Text){New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($Path))|Out-Null;[IO.File]::WriteAllText($Path,$Text)}
function MakeWave([string]$Path){
    # Same known integral PCM source shape as AudioSdkSupport.Wave; no codec,
    # compression library, external encoder or imprecise floating conversion.
    $stream=[IO.File]::Create($Path);$writer=[IO.BinaryWriter]::new($stream)
    try{
        $frames=4096;$channels=2;$dataBytes=$frames*$channels*2
        $writer.Write([Text.Encoding]::ASCII.GetBytes('RIFF'));$writer.Write([int](36+$dataBytes));$writer.Write([Text.Encoding]::ASCII.GetBytes('WAVEfmt '))
        $writer.Write([int]16);$writer.Write([int16]1);$writer.Write([int16]$channels);$writer.Write([int]22050);$writer.Write([int](22050*$channels*2));$writer.Write([int16]($channels*2));$writer.Write([int16]16)
        $writer.Write([Text.Encoding]::ASCII.GetBytes('data'));$writer.Write([int]$dataBytes)
        $values=[int16[]](0,1,-1,32767,-32768,0x1234)
        for($i=0;$i -lt $frames*$channels;$i++){$writer.Write([int16]$values[$i%$values.Length])}
    }finally{$writer.Dispose();$stream.Dispose()}
    Need ((Get-Item -LiteralPath $Path).Length -eq 44+4096*2*2) 'PCM fixture length differs from its RIFF data size'
}
function AudioMod([string]$Path){
    Put "$Path/Data/Mod.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioFile id="BuildCachePcmWave" File="../Audio/probe.wav" PCCompression="NONE" IsStreamedOnPC="false"/></AssetDeclaration>'
    New-Item -ItemType Directory -Path "$Path/Audio"|Out-Null;MakeWave "$Path/Audio/probe.wav"
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
    Put "$Path/Data/Mod.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="W3x.xml"/></Includes></AssetDeclaration>'
    Put "$Path/Data/W3x.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$hierarchy.OuterXml+$animation.OuterXml+'</AssetDeclaration>')
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Actual production /sc session-package disabled/cold/hot reuse, Include/XInclude defines/expressions/inheritance, complete intermediate and Audio CDATA/W3X, content invalidation, MOD isolation, corruption, cancellation/failure retries; exact raw streams, selected original BAB oracle; not complete P5/G3/SDK'}
try{
    Need ($state.sourceCommit -cmatch '^[0-9a-f]{40}$') 'Cannot identify source commit'
    if($Preflight){
        $probe=[IO.File]::ReadAllText("$root/SDK/Mods/mymod/Data/RecoveryArmorProbe.xml")
        $preflightMod=Join-Path $short 'm';MakeMod $preflightMod $probe
        AudioMod (Join-Path $short 'a');W3xMod (Join-Path $short 'w')
        $check=@'
import importlib.util, pathlib, sys
from lxml import etree as E
scripts=pathlib.Path(sys.argv[1]); work=pathlib.Path(sys.argv[2])
spec=importlib.util.spec_from_file_location('session_schema', scripts/'GenerateXmlSdkModA.py')
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
schema=module.aggregate_schema(); ns={'ea':'uri:ea.com:eala:asset'}
root=E.parse(str(work/'m/Data/Mod.xml')); root.xinclude()
defines=root.xpath('/ea:AssetDeclaration/ea:Defines/ea:Define', namespaces=ns)
assert len(defines)==1 and defines[0].get('name')=='SESSION_SCALE' and defines[0].get('value')=='83.25'
expression=root.xpath('/ea:AssetDeclaration/ea:ArmorTemplate[@id="SessionExpressionArmor"]', namespaces=ns)[0]
assert expression.get('DamageScalar')=='=$SESSION_SCALE'
# Core evaluates expressions before XSD validation. Check that exact known
# expression result, then source-XSD validation including declaration order.
expression.set('DamageScalar',defines[0].get('value')); schema.assertValid(root)
for name in ('m/Data/Armor.xml','m/Data/AptUI/Payload.xml','a/Data/Mod.xml','w/Data/Mod.xml','w/Data/W3x.xml'):
    schema.assertValid(E.parse(str(work/name)))
armor=E.parse(str(work/'m/Data/Armor.xml'))
assert len(armor.xpath('/ea:AssetDeclaration/ea:ArmorTemplate[@inheritFrom="RecoveryArmorProbe_NonDefault"]',namespaces=ns))==1
assert b'72.5%' in (work/'m/Data/Armor.xml').read_bytes()
assert b'22.5%' in (work/'m/Data/XArmor.xml').read_bytes()
assert b'0x12345678' in (work/'m/Data/events.h').read_bytes()
assert b'FrameRate="30"' in (work/'w/Data/W3x.xml').read_bytes()
assert len((work/'a/Audio/probe.wav').read_bytes())==44+4096*2*2
config=(module.ROOT/'SDK/tools/BinaryAssetBuilder.exe.config').read_bytes()
assert b'monitorPaths="..\\"' in config
assert b'elementFormDefault="qualified"' in (module.S/'AssetTypeTexture.xsd').read_bytes()
print('PASS session fixtures: XInclude declaration order, exact defines/expression, inheritance, source XSD, payload closure and mutation tokens')
'@
        & python -c $check $PSScriptRoot $short *> "$out/fixture-preflight.log"
        Need ($LASTEXITCODE -eq 0) 'Session fixture preflight failed; inspect fixture-preflight.log'
        $state.preflight=$true;$state.scope='Fixture source-XSD/known expression/closure and mutation-token preflight only; no production cache execution';$state.succeeded=$true;$global:LASTEXITCODE=0
        return
    }
    Need (!(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'reuse.json')) -and !(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'candidate-replay.json'))) 'New session-cache acceptance requires current production EXE bytes'
    $state.binarySourceCommit=$state.sourceCommit;$state.testSourceCommit=$state.sourceCommit
    $replayFiles=@{};$replayPath=Join-Path $CompilerDirectory 'session-replay.json'
    if(Test-Path -LiteralPath $replayPath){
        $replay=Get-Content -LiteralPath $replayPath -Raw|ConvertFrom-Json
        $guard=@'
import json, re, sys
sys.path.insert(0,sys.argv[1])
import PrepareCompilerSessionReplay as p
assert p.SOURCE=='f2ba141284469411d604d756f7916b0cf6aecf0a'
assert str(p.RUN)=='40406049617' and str(p.ATTEMPT)=='1'
assert re.fullmatch('[0-9a-f]{64}',p.SHA256) and int(p.ARTIFACT)>0
p.closure(sys.argv[2],p.SOURCE,sys.argv[3])
print(json.dumps({'source':p.SOURCE,'runId':str(p.RUN),'runAttempt':str(p.ATTEMPT),'sha256':p.SHA256,'artifactId':str(p.ARTIFACT)}))
'@
        & python -c $guard $PSScriptRoot $root $state.sourceCommit > "$out/replay-source-closure.json"
        Need ($LASTEXITCODE -eq 0) 'Session replay actual production-source closure rejected'
        $identity=Get-Content "$out/replay-source-closure.json" -Raw|ConvertFrom-Json
        Need ($replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.binarySourceCommit -ceq $identity.source -and $replay.testSourceCommit -ceq $state.sourceCommit -and [string]$replay.runId -ceq $identity.runId -and [string]$replay.runAttempt -ceq $identity.runAttempt -and $replay.archiveSha256 -ceq $identity.sha256 -and [string]$replay.artifactId -ceq $identity.artifactId) 'Session replay fixed identity or preparation contract differs'
        $expectedFiles=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach($configuration in @('Debug','Release')){foreach($fileName in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){$null=$expectedFiles.Add($configuration+'/bin/'+$fileName)}}
        Need (@($replay.files).Count -eq 6) 'Session replay requires exactly six runtime identities'
        foreach($file in $replay.files){
            Need ($expectedFiles.Remove([string]$file.path) -and $file.sha256 -cmatch '^[0-9a-f]{64}$') 'Unexpected or duplicate session replay runtime identity'
            Need ((Hash (Join-Path $CompilerDirectory $file.path)) -ceq $file.sha256) 'Session replay production bytes differ from fixed archive'
            $replayFiles[[string]$file.path]=[string]$file.sha256
        }
        Need ($expectedFiles.Count -eq 0) 'Session replay runtime set is incomplete'
        $state.binarySourceCommit=$identity.source;$state.sessionReplay=$replay
        $state.scope+='; fixed binarySourceCommit with separately recorded current testSourceCommit and checked actual source closure; not a current production rebuild or a change to the original run conclusion'
    }
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    $state.scriptSha256=Hash $PSCommandPath;$state.controllerSha256=Hash $control
    foreach($mode in @('Debug','Release')){
        $script:modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $modeWork=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName
        $script:runtime=(New-Item -ItemType Directory -Path (Join-Path $modeWork 'r')).FullName
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){
            Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime
            if($replayFiles.Count -gt 0){Need ((Hash (Join-Path $runtime $file)) -ceq $replayFiles[$mode+'/bin/'+$file]) 'Copied session replay runtime identity differs'}
        }
        $script:exe=Join-Path $runtime 'NewModCompiler.exe'
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$modeOut/prepare-texture.log";Need ($LASTEXITCODE -eq 0) 'Session runtime Texture deployment failed'
        $script:modeState=@{configuration=$mode;passed=$false;exeSha256=(Hash $exe);cases=@();corruptions=@()};$state.modes+=,$modeState
        $script:sdk=(New-Item -ItemType Directory -Path (Join-Path $modeWork 's')).FullName
        New-Item -ItemType Directory -Path "$sdk/tools","$sdk/Schemas"|Out-Null
        Copy-Item -LiteralPath "$root/SDK/Schemas/xsd" -Destination "$sdk/Schemas" -Recurse
        Copy-Item -LiteralPath "$root/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$sdk/tools"
        $probe=[IO.File]::ReadAllText("$root/SDK/Mods/mymod/Data/RecoveryArmorProbe.xml")
        $mod=Join-Path $modeWork 'm';MakeMod $mod $probe
        $cachedOut=Join-Path $modeWork 'cache';$plainOut=Join-Path $modeWork 'plain'
        $script:contents='base,apt';$script:expectedArtifacts=8
        $plain=RunCli 'disabled' $mod $plainOut $false;Cache $plain 'disabled'
        $cold=RunCli 'cold' $mod $cachedOut $true;Cache $cold 'cold';SameOutputs $plain $cold;$script:hotParseCeiling=$cold.report.cache.xmlParse
        $hot=RunCli 'hot' $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $plain $hot
        OriginalOracle $hot 'original-initial'
        $combinedOut=Join-Path $modeWork 'both'
        $combinedCold=RunCli 'combined-sc-bc-cold' $mod $combinedOut $true -BuildEnabled $true;SameOutputs $plain $combinedCold
        Need ($combinedCold.report.cache.sessionCache -and $combinedCold.report.cache.sessionPackageMisses -gt 0 -and $combinedCold.report.cache.documentReuse -eq 0 -and $combinedCold.report.cache.misses -gt 0 -and $combinedCold.report.cache.hits -eq 0) 'Combined cold session/build cache did not record independent actual misses'
        $combinedHot=RunCli 'combined-sc-bc-hot' $mod $combinedOut $true -BuildEnabled $true;SameOutputs $plain $combinedHot
        Need ($combinedHot.report.cache.sessionPackageHits -gt 0 -and $combinedHot.report.cache.sessionPackageMisses -eq 0 -and $combinedHot.report.cache.documentReuse -gt 0 -and $combinedHot.report.cache.instanceCompile -eq 0 -and $combinedHot.report.cache.hits -eq 0 -and $combinedHot.report.cache.misses -eq 0) 'Combined hot cache conflated session reuse with asset-cache hits'
        foreach($change in @(
            @{name='include-inheritance';path="$mod/Data/Armor.xml";from='72.5%';to='12.5%'},
            @{name='xinclude';path="$mod/Data/XArmor.xml";from='22.5%';to='32.5%'},
            @{name='define-expression';path="$mod/Data/Defs.xml";from='83.25';to='93.25'},
            @{name='file-reference';path="$mod/Data/events.h";from='0x12345678';to='0x23456789'})){
            SameLengthMutation $change.path $change.from $change.to
            $changed=RunCli ($change.name+'-invalidated') $mod $cachedOut $true;Cache $changed 'cold';DifferentOutputs $hot $changed;$script:hotParseCeiling=$changed.report.cache.xmlParse
            $plain=RunCli ($change.name+'-disabled') $mod $plainOut $false;SameOutputs $plain $changed
            $hot=RunCli ($change.name+'-hot') $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $plain $hot
        }
        OriginalOracle $hot 'original-mutated'
        # Neutral same-length/same-mtime schema and configuration edits must
        # miss the content namespace while preserving all compiled bytes.
        foreach($change in @(
            @{name='schema';path="$sdk/Schemas/xsd/AssetTypeTexture.xsd";from='elementFormDefault="qualified"';to="elementFormDefault='qualified'"},
            @{name='sdk-config';path="$sdk/tools/BinaryAssetBuilder.exe.config";from='monitorPaths="..\"';to="monitorPaths='..\'"},
            @{name='runtime-config';path="$runtime/NewModCompiler.exe.config";from='version="v4.0"';to="version='v4.0'"})){
            SameLengthMutation $change.path $change.from $change.to
            $changed=RunCli ($change.name+'-invalidated') $mod $cachedOut $true;Cache $changed 'cold';SameOutputs $plain $changed;$script:hotParseCeiling=$changed.report.cache.xmlParse
            $hot=RunCli ($change.name+'-hot') $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $plain $hot
        }
        $other=Join-Path $modeWork 'b';MakeMod $other ($probe.Replace('72.5%','32.5%'))
        $otherCold=RunCli 'second-mod' $other $cachedOut $true;Cache $otherCold 'cold'
        $otherPlain=RunCli 'second-mod-disabled' $other $plainOut $false;SameOutputs $otherPlain $otherCold
        $returnA=RunCli 'first-mod-return' $mod $cachedOut $true;Cache $returnA 'hot';SameOutputs $plain $returnA
        # Fresh destinations keep one namespace per package; corrupt all package
        # payloads and prove every selected stream returns to actual cold work.
        foreach($kind in @('xml','intermediate')){
            $damagedOut=Join-Path $modeWork ('damage-'+$kind)
            $beforeDamage=RunCli ($kind+'-warm') $mod $damagedOut $true;Cache $beforeDamage 'cold';SameOutputs $plain $beforeDamage
            $modeState.corruptions+=,@{kind=$kind;entries=@(DamagePackage $damagedOut $kind)}
            $afterDamage=RunCli ($kind+'-damaged-cold') $mod $damagedOut $true;Cache $afterDamage 'cold';SameOutputs $plain $afterDamage
            $repairedHot=RunCli ($kind+'-repaired-hot') $mod $damagedOut $true;Cache $repairedHot 'hot';SameOutputs $plain $repairedHot
        }
        $armorPath="$mod/Data/Armor.xml";$valid=[IO.File]::ReadAllBytes($armorPath);$confirmed=CacheFiles $cachedOut
        [IO.File]::WriteAllText($armorPath,'<AssetDeclaration>broken')
        $failed=RunCli 'invalid-include' $mod $cachedOut $true 4;CacheUnchanged $cachedOut $confirmed
        [IO.File]::WriteAllBytes($armorPath,$valid)
        $retry=RunCli 'retry-after-invalid-include' $mod $cachedOut $true;SameOutputs $plain $retry
        $confirmed=CacheFiles $cachedOut
        $cancelled=RunCli 'actual-cancel' $mod $cachedOut $true 6 $true;CacheUnchanged $cachedOut $confirmed
        $afterCancel=RunCli 'retry-after-actual-cancel' $mod $cachedOut $true;SameOutputs $plain $afterCancel
        $held=@($afterCancel.report.artifacts)[0].Path;$confirmed=CacheFiles $cachedOut
        $occupied=RunCli 'publication-fault' $mod $cachedOut $true 7 $false $held;CacheUnchanged $cachedOut $confirmed
        $afterFault=RunCli 'retry-after-publication-fault' $mod $cachedOut $true;SameOutputs $plain $afterFault
        $script:contents='base'
        foreach($family in @('audio','w3x')){
            $payloadMod=Join-Path $modeWork $family;$payloadCache=Join-Path $modeWork ($family+'-cache');$payloadPlain=Join-Path $modeWork ($family+'-plain')
            if($family -eq 'audio'){AudioMod $payloadMod;$script:expectedArtifacts=5}else{W3xMod $payloadMod;$script:expectedArtifacts=4}
            $before=RunCli ($family+'-disabled') $payloadMod $payloadPlain $false;Cache $before 'disabled'
            $payloadCold=RunCli ($family+'-cold') $payloadMod $payloadCache $true;Cache $payloadCold 'cold';SameOutputs $before $payloadCold;$script:hotParseCeiling=$payloadCold.report.cache.xmlParse
            $payloadHot=RunCli ($family+'-hot') $payloadMod $payloadCache $true;Cache $payloadHot 'hot';SameOutputs $before $payloadHot;Need ($payloadHot.report.cache.xmlParse -eq 0) 'Hot Audio/W3X unexpectedly parsed XML'
            if($family -eq 'audio'){
                $modeState.corruptions+=,@{kind='cdata';entries=@(DamagePackage $payloadCache 'cdata')}
                $damaged=RunCli 'audio-missing-cdata-cold' $payloadMod $payloadCache $true;Cache $damaged 'cold';SameOutputs $before $damaged
                $repairedHot=RunCli 'audio-repaired-cdata-hot' $payloadMod $payloadCache $true;Cache $repairedHot 'hot';SameOutputs $before $repairedHot
                $input="$payloadMod/Audio/probe.wav";$stamp=[IO.File]::GetLastWriteTimeUtc($input);$bytes=[IO.File]::ReadAllBytes($input);$length=$bytes.Length
                $bytes[44]=0x34;$bytes[45]=0x12;[IO.File]::WriteAllBytes($input,$bytes);[IO.File]::SetLastWriteTimeUtc($input,$stamp)
                Need ((Get-Item -LiteralPath $input).Length -eq $length -and [IO.File]::GetLastWriteTimeUtc($input) -eq $stamp) 'Audio mutation failed to preserve length/mtime'
            }else{SameLengthMutation "$payloadMod/Data/W3x.xml" 'FrameRate="30"' 'FrameRate="31"'}
            $changed=RunCli ($family+'-invalidated') $payloadMod $payloadCache $true;Cache $changed 'cold';DifferentOutputs $payloadHot $changed;$script:hotParseCeiling=$changed.report.cache.xmlParse
            $changedPlain=RunCli ($family+'-changed-disabled') $payloadMod $payloadPlain $false;SameOutputs $changedPlain $changed
            $changedHot=RunCli ($family+'-changed-hot') $payloadMod $payloadCache $true;Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot;Need ($changedHot.report.cache.xmlParse -eq 0) 'Changed-hot Audio/W3X unexpectedly parsed XML'
            if($family -eq 'w3x'){OriginalOracle $changedHot 'original-w3x'}
        }
        # Exercise each real mixed plugin route far enough to observe the
        # production /sc rejection; no invalid-input surrogate or cache hit.
        foreach($type in @('Texture','OnDemandTexture','TerrainTextureAtlas')){
            $textureMod=Join-Path $modeWork ('t'+[array]::IndexOf(@('Texture','OnDemandTexture','TerrainTextureAtlas'),$type));$textureOut=Join-Path $modeWork ('o'+[array]::IndexOf(@('Texture','OnDemandTexture','TerrainTextureAtlas'),$type))
            New-Item -ItemType Directory -Force -Path "$textureMod/Data"|Out-Null
            & python "$root/recovery/probes/TextureRedirect/MakeFixture.py" "$textureMod/Data/probe.tga"
            Need ($LASTEXITCODE -eq 0) 'Session Texture fixture preparation failed'
            if($type -ceq 'TerrainTextureAtlas'){$asset='<TerrainTextureAtlas id="SessionTexture" AtlasSize="2048" AllowLossyCompression="false"><Tile TextureID="1" BaseTexture="probe.tga" NormalTexture="probe.tga"><Cell IndexX="0" IndexY="0"/></Tile></TerrainTextureAtlas>'}
            else{$asset='<'+$type+' id="SessionTexture" File="probe.tga" Type="StandardTexture" GenerateMipMaps="false" OutputFormat="A8R8G8B8" AllowAutomaticResize="false"/>'}
            Put "$textureMod/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$asset+'</AssetDeclaration>')
            $rejected=RunCli ($type+'-session-rejected') $textureMod $textureOut $true 2
            Need (@($rejected.report.diagnostics|Where-Object{$_.code -ceq 'CORE_SESSION_TEXTURE_UNSUPPORTED'}).Count -eq 1) "Mixed $type session cache was not explicitly rejected"
            Need ((CacheFiles $textureOut).Count -eq 0) 'Rejected mixed Texture persisted a session package'
        }
        $modeState.passed=$true
    }
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{
    $state|ConvertTo-Json -Depth 18|Set-Content "$out/validation.json"
    try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "Session-cache work cleanup failed: $_"}
}
