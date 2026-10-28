#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path
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
    foreach($item in @(@{name='ParallelLong';frames=32768},@{name='ParallelShort';frames=24576})){
        $node=$source.CloneNode($true);$node.SetAttribute('id',$item.name);$node.SetAttribute('NumFrames',[string]$item.frames)
        Put "$Path/Data/$($item.name).xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$node.OuterXml+'</AssetDeclaration>')
        $includes+='<Include type="all" source="'+$item.name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
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
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Production /bc asset-cache replay for AudioFile PCM WAVE including complete CDATA and one real W3DAnimation with hierarchy closure; disabled/cold/hot, same-length same-mtime input changes, exact four-stream and CDATA equality against independent no-cache processes; not a new original-DLL/SDK gate'}
try{
    Need ($state.sourceCommit -cmatch '^[0-9a-f]{40}$') 'Cannot identify source commit'
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
    $state.sourceInputs=@('recovery/tests/AudioSdkSupport.cs','SDK/Mods/W3xRecovery/Data/Animations.xml','SDK/Mods/W3xRecovery/Data/Dependencies.xml','SDK/Mods/W3xRecovery/Data/Meshes.xml')|ForEach-Object{@{path=$_;sha256=(Hash (Join-Path $root $_))}}
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
            $cold=RunCli ($family+'-cold') $mod $cachedOut $true;Cache $cold 'cold';SameOutputs $before $cold
            $hot=RunCli ($family+'-hot') $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $before $hot
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
            $changed=RunCli ($family+'-content-invalidated') $mod $cachedOut $true;Cache $changed 'partial';DifferentOutputs $hot $changed
            $changedPlain=RunCli ($family+'-changed-disabled') $mod $plainOut $false;Cache $changedPlain 'disabled';SameOutputs $changedPlain $changed
            $changedHot=RunCli ($family+'-changed-hot') $mod $cachedOut $true;Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot
            if($family -eq 'audio'){
                $customKeys=@($hot.files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
                $changedKeys=@($changed.files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
                $unchangedCustom=@($customKeys|Where-Object{$changed.files.ContainsKey($_) -and (Hash $hot.files[$_]) -ceq (Hash $changed.files[$_])})
                Need ($customKeys.Count -eq 2 -and $changedKeys.Count -eq 2 -and $unchangedCustom.Count -eq 1) 'One WAV sample mutation must change its complete PCM CDATA while preserving the independent XAS result'
            }
            New-Item -ItemType Directory -Force -Path "$mod/Additional"|Out-Null
            [IO.File]::WriteAllText("$mod/Additional/only-package.txt",'not read by any compiler action')
            $additional=RunCli ($family+'-additional-only') $mod $cachedOut $true;Cache $additional 'hot';SameOutputs $changedPlain $additional
            Need ($additional.report.cache.backendCalls -eq 0) 'Unread Additional member triggered backend compilation in an entirely certified stream'
            $store=Join-Path $cachedOut '.cache/action-cache-v1'
            $entries=@(Get-ChildItem -LiteralPath $store -Directory|Where-Object{$_.Name -cmatch '^[0-9a-f]{64}$'})
            Need ($entries.Count -gt 0) 'Action cache did not persist independently addressed entries'
            if($family -eq 'audio'){
                $entry=@($entries|Where-Object{@(Get-ChildItem -LiteralPath (Join-Path $_.FullName 'files') -File).Count -eq 2})[0]
                Need ($null -ne $entry) 'Audio cache entry did not preserve metadata/buffers plus complete CDATA'
                $members=@(Get-ChildItem -LiteralPath (Join-Path $entry.FullName 'files') -File|Sort-Object Name)
                # Result metadata/buffers sort before the complete CDATA member.
                Remove-Item -LiteralPath $members[-1].FullName
            }else{
                $entry=$entries[0];$member=@(Get-ChildItem -LiteralPath (Join-Path $entry.FullName 'files') -File)[0]
                $bytes=[IO.File]::ReadAllBytes($member.FullName);Need ($bytes.Length -gt 0) 'Expected a nonempty result contract'
                $bytes[0]=$bytes[0] -bxor 0x7f;[IO.File]::WriteAllBytes($member.FullName,$bytes)
            }
            $repaired=RunCli ($family+'-damaged-result-repaired') $mod $cachedOut $true;Cache $repaired 'partial';SameOutputs $changedPlain $repaired
            $repairedHot=RunCli ($family+'-repaired-hot') $mod $cachedOut $true;Cache $repairedHot 'hot';SameOutputs $changedPlain $repairedHot
            $modeState.fixtures+=,@{family=$family;input=$input;mutatedInputSha256=(Hash $input);sameLengthSameTimestamp=$true;hotAssetHits=$hot.report.cache.hits;changedHotAssetHits=$changedHot.report.cache.hits;completeCustomDataFiles=$script:expectedCustomData;allRawOutputsEqualToIndependentNoCache=$true}
        }
        $parallelMod=Join-Path $modeWork 'parallel';ParallelW3xMod $parallelMod;$script:expectedCustomData=0
        $serial=RunCli 'w3x-parallel-control' $parallelMod (Join-Path $modeWork 'parallel-serial') $false -Extra @('--jobs=1');Cache $serial 'disabled'
        $parallel=RunCli 'w3x-cross-document-parallel' $parallelMod (Join-Path $modeWork 'parallel-jobs2') $false -Extra @('--jobs=2');Cache $parallel 'disabled';SameOutputs $serial $parallel
        $modeState.parallel=ParallelEvidence $parallel
        $modeState.passed=$true
    }
    foreach($file in $state.sourceInputs){Need ((Hash (Join-Path $root $file.path)) -ceq $file.sha256) 'Original payload fixture source changed'}
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{
    $state|ConvertTo-Json -Depth 18|Set-Content "$out/validation.json"
    try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "Payload cache work cleanup failed: $_"}
}
