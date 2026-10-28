#requires -Version 7.0
[CmdletBinding()]
param(
    [string]$CompilerDirectory,
    [string]$ReplayReceipt,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180,
    [switch]$Preflight,
    [switch]$RepositoryModMetrics
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
    Need (($Expected -eq 'cold' -and $statistics.restoredBytes -eq 0) -or ($Expected -ne 'cold' -and $statistics.restoredBytes -gt 0)) 'Restored-byte accounting must retain complete result and CDATA consumption after private file moves'
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
function RepositoryInputManifest([string]$Directory){
    Get-ChildItem -LiteralPath $Directory -Recurse -File -Force|Sort-Object FullName|ForEach-Object{
        @{path=[IO.Path]::GetRelativePath($Directory,$_.FullName).Replace('\','/');bytes=$_.Length;sha256=(Hash $_.FullName)}
    }
}
function RepositoryModMetrics([string]$ModeWork){
    # This is the repository's small, untrimmed MOD fixture. The OS file cache
    # is uncontrolled; only the compiler's action-cache state is controlled.
    $source=(Resolve-Path -LiteralPath "$root/SDK/Mods/mymod").Path
    $mod=Join-Path $ModeWork 'repository-mod'
    $inputs=@(RepositoryInputManifest $source)
    Need ($inputs.Count -gt 0) 'Repository MOD metrics input is empty'
    Copy-Item -LiteralPath $source -Destination $mod -Recurse -Force
    Need (@(Get-ChildItem -LiteralPath $mod -Recurse -File -Force).Count -eq $inputs.Count) 'Repository MOD copy trimmed or added inputs'
    foreach($inputFile in $inputs){Need ((Hash (Join-Path $mod $inputFile.path)) -ceq $inputFile.sha256) 'Repository MOD copy changed an input byte'}
    $savedSdk=$script:sdk
    try{
        $script:sdk=(Resolve-Path -LiteralPath "$root/SDK").Path
        if(!$state.ContainsKey('repositorySdkInputs')){
            # Capture actual SDK bytes, including the original mymod below it.
            # This setup/verification cost is outside both reported CLI timers.
            $state.repositorySdkInputs=@(RepositoryInputManifest $sdk)
            $state.repositorySdkInputBytes=[long](($state.repositorySdkInputs|Measure-Object -Property bytes -Sum).Sum)
        }
        # Keep the exact defaultscript-style source order used by TestCompilerSdk.
        $data=$sdk+';'+(Join-Path $mod 'Data')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'SageXml')
        $art=$sdk+';'+(Join-Path $mod 'Art')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Art')
        $audio=$sdk+';'+(Join-Path $mod 'Audio')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Audio')
        $fixed=@('--profile','development','--jobs=1','/sc:false','--include-additional=false',('/data:'+$data),('/art:'+$art),('/audio:'+$audio))
        $script:expectedCustomData=0
        $runtimeInputs=@(RepositoryInputManifest $script:runtime)
        Need ($runtimeInputs.Count -eq 5) 'Repository metrics require the deployed five-file runtime'
        $measurement=@{passed=$false;scope='Complete unchanged repository SDK/Mods/mymod copy against the real SDK, development/loose/base/jobs1. Initial no-cache control and empty-store write followed by three alternating disabled/hot pairs on this runner. Small repository fixture, not a large MOD benchmark; sampled private memory is not an OS continuous peak; no speed threshold or speedup claim. Additional changes do not enter loose stream artifacts and are not a packaging-byte comparison.';osFileCache=@{control='not-flushed-or-isolated';pairedSampleCondition='after SDK input hashing, initial no-cache control and cold cache write; OS cache may remain warm across invocations';runner=$env:RUNNER_NAME;operatingSystem=$env:RUNNER_OS;machine=[Environment]::MachineName;crossRunnerComparison=$false};sourceGitTree=(& git -C $root rev-parse 'HEAD:SDK/Mods/mymod').Trim();sdkGitTree=(& git -C $root rev-parse 'HEAD:SDK').Trim();sourceRoot=$source;modRoot=$mod;sdkRoot=$sdk;inputFiles=$inputs;inputFileCount=$inputs.Count;inputBytes=[long](($inputs|Measure-Object -Property bytes -Sum).Sum);runtimeFiles=$runtimeInputs;fixedArguments=(@('--package-format','loose','--package-content','base')+$fixed);rows=@()}
        $script:modeState.repositoryModMetrics=$measurement
        $controlCase=RunCli 'repository-mod-no-cache' $mod (Join-Path $ModeWork 'repository-plain') $false -Extra $fixed;Cache $controlCase 'disabled'
        $measurement.rows+=,(RepositoryMetricRow $controlCase 'disabled' 'unchanged')
        $cached=Join-Path $ModeWork 'repository-cache'
        Need (!(Test-Path -LiteralPath (Join-Path $cached '.cache/action-cache-v1'))) 'Repository action cache must start empty'
        $cold=RunCli 'repository-mod-empty-action-cache' $mod $cached $true -Extra $fixed;Cache $cold 'cold';SameOutputs $controlCase $cold
        $measurement.rows+=,(RepositoryMetricRow $cold 'empty-action-cache' 'unchanged')
        $hot=$null
        foreach($repeat in 1..3){
            # Alternate in one process order, reuse the exact same runtime,
            # roots/options/jobs and both fixed output layouts. Setup/cold rows
            # remain separate from the comparable three-pair summary.
            $plain=RunCli ('repository-mod-pair-'+$repeat+'-no-cache') $mod (Join-Path $ModeWork 'repository-plain') $false -Extra $fixed;Cache $plain 'disabled';SameOutputs $controlCase $plain
            $measurement.rows+=,(RepositoryMetricRow $plain 'disabled' 'unchanged' $repeat)
            $next=RunCli ('repository-mod-pair-'+$repeat+'-hot') $mod $cached $true -Extra $fixed;Cache $next 'hot';SameOutputs $plain $next
            if($null -ne $hot){Need ($next.report.cache.backendCalls -eq $hot.report.cache.backendCalls -and $next.report.cache.actionHits -eq $hot.report.cache.actionHits) 'Repeated repository hot calls or certified hit count changed'}
            $hot=$next;$measurement.rows+=,(RepositoryMetricRow $hot 'hot-action-cache' 'unchanged' $repeat)
        }
        $measurement.comparison=RepositoryPairedSummary $measurement.rows
        $additional=Join-Path $mod 'Additional/repository-cache-metrics.txt'
        Need (!(Test-Path -LiteralPath $additional)) 'Repository metrics Additional marker would overwrite source content'
        Put $additional 'Repository metrics: unread Additional-only change; loose stream bytes stay identical.'
        $additionalCase=RunCli 'repository-mod-additional-only' $mod $cached $true -Extra $fixed;Cache $additionalCase 'hot';SameOutputs $controlCase $additionalCase
        Need ($additionalCase.report.cache.backendCalls -eq $hot.report.cache.backendCalls -and $additionalCase.report.cache.actionHits -eq $hot.report.cache.actionHits) 'Unread Additional-only edit added a backend call or changed certified hits'
        $measurement.rows+=,(RepositoryMetricRow $additionalCase 'hot-action-cache' 'additional-only')
        foreach($inputFile in $inputs){
            Need ((Hash (Join-Path $source $inputFile.path)) -ceq $inputFile.sha256 -and (Hash (Join-Path $mod $inputFile.path)) -ceq $inputFile.sha256) 'Repository metrics changed an original MOD input'
        }
        Need (@(Get-ChildItem -LiteralPath $source -Recurse -File -Force).Count -eq $inputs.Count -and @(Get-ChildItem -LiteralPath $mod -Recurse -File -Force).Count -eq $inputs.Count+1) 'Repository metrics added files outside its one private Additional marker'
        foreach($runtimeFile in $runtimeInputs){Need ((Hash (Join-Path $script:runtime $runtimeFile.path)) -ceq $runtimeFile.sha256) 'Repository metrics changed a deployed runtime file'}
        Need (@(Get-ChildItem -LiteralPath $script:runtime -File -Recurse -Force).Count -eq 5) 'Repository metrics changed the deployed runtime inventory'
        $measurement.additionalChange=@{path='Additional/repository-cache-metrics.txt';sha256=(Hash $additional);streamBytesEqual=$true;certifiedMisses=0;backendCalls=$additionalCase.report.cache.backendCalls}
        $measurement.passed=$true
    }finally{$script:sdk=$savedSdk}
}
function RepositoryCacheStorage([string]$Destination){
    $layers=@{}
    foreach($name in @('actionResults','readClosures','other')){
        $layers[$name]=@{fileCount=0;fileLengthBytes=0L;indexFileCount=0;indexFileLengthBytes=0L;payloadFileCount=0;payloadFileLengthBytes=0L}
    }
    $cache=Join-Path $Destination '.cache'
    if(Test-Path -LiteralPath $cache){
        foreach($file in @(Get-ChildItem -LiteralPath $cache -File -Recurse)){
            $relative=[IO.Path]::GetRelativePath($cache,$file.FullName).Replace('\','/')
            $name=if($relative.StartsWith('action-cache-v1/',[StringComparison]::Ordinal)){'actionResults'}elseif($relative.StartsWith('read-closure-v1/',[StringComparison]::Ordinal)){'readClosures'}else{'other'}
            $layer=$layers[$name];$layer.fileCount++;$layer.fileLengthBytes+=[long]$file.Length
            if($file.Name -ceq 'index.bin'){$layer.indexFileCount++;$layer.indexFileLengthBytes+=[long]$file.Length}
            elseif($relative.Contains('/files/')){$layer.payloadFileCount++;$layer.payloadFileLengthBytes+=[long]$file.Length}
        }
    }
    $files=0;$bytes=0L
    foreach($layer in $layers.Values){$files+=$layer.fileCount;$bytes+=$layer.fileLengthBytes}
    return @{format='NewModCompiler.CacheStorage.v1';sizeBasis='sum-of-logical-file-lengths-not-allocated-disk-space';snapshotScope='after compiler process exit; enumeration cost excluded from both CLI timers';fileCount=$files;fileLengthBytes=$bytes;layers=$layers;documentState='not-implemented';streamResults='not-implemented';compressionComparison='not-run';transactionPeakStatus='not-measured'}
}
function RepositoryMetricRange([double[]]$Values){
    Need ($Values.Count -gt 0) 'Repository metric summary has no comparable samples'
    $sorted=@($Values|Sort-Object);$middle=[int][Math]::Floor($sorted.Count/2)
    $median=if($sorted.Count%2 -eq 1){[double]$sorted[$middle]}else{([double]$sorted[$middle-1]+[double]$sorted[$middle])/2.0}
    return @{samples=@($Values);median=$median;minimum=[double]$sorted[0];maximum=[double]$sorted[-1];range=([double]$sorted[-1]-[double]$sorted[0])}
}
function RepositoryPairedSummary($Rows){
    $plain=@($Rows|Where-Object{$_.pairIndex -gt 0 -and $_.actionCacheState -ceq 'disabled'})
    $hot=@($Rows|Where-Object{$_.pairIndex -gt 0 -and $_.actionCacheState -ceq 'hot-action-cache'})
    Need ($plain.Count -eq 3 -and $hot.Count -eq 3) 'Repository comparison requires exactly three disabled/hot alternating pairs'
    $pairs=@();$reportDelta=@();$controllerDelta=@()
    foreach($repeat in 1..3){
        $left=@($plain|Where-Object{$_.pairIndex -eq $repeat});$right=@($hot|Where-Object{$_.pairIndex -eq $repeat})
        Need ($left.Count -eq 1 -and $right.Count -eq 1) 'Repository comparison lost its paired invocation identity'
        $report=[double]$right[0].reportElapsedMs-[double]$left[0].reportElapsedMs
        $controller=[double]$right[0].controllerElapsedMs-[double]$left[0].controllerElapsedMs
        $pairs+=,@{pairIndex=$repeat;order='disabled-then-hot';disabled=$left[0].name;hot=$right[0].name;reportHotMinusDisabledMs=$report;controllerHotMinusDisabledMs=$controller;allFourRawOutputsEqual=$true}
        $reportDelta+=$report;$controllerDelta+=$controller
    }
    return @{
        pairCount=3;requestedJobs=1;order='disabled,hot,disabled,hot,disabled,hot';pairs=$pairs
        reportElapsedMs=@{disabled=(RepositoryMetricRange @($plain|ForEach-Object{[double]$_.reportElapsedMs}));hot=(RepositoryMetricRange @($hot|ForEach-Object{[double]$_.reportElapsedMs}));pairedHotMinusDisabled=(RepositoryMetricRange $reportDelta)}
        controllerElapsedMs=@{disabled=(RepositoryMetricRange @($plain|ForEach-Object{[double]$_.controllerElapsedMs}));hot=(RepositoryMetricRange @($hot|ForEach-Object{[double]$_.controllerElapsedMs}));pairedHotMinusDisabled=(RepositoryMetricRange $controllerDelta)}
        interpretation='same-runner paired measurements only; positive hot-minus-disabled means overhead; no automatic speedup claim or threshold'
    }
}
function RepositoryMetricRow($Case,[string]$CacheState,[string]$InputVariant,[int]$PairIndex=0){
    $controller=Get-Content -LiteralPath (Join-Path $Case.caseOut 'control.json') -Raw|ConvertFrom-Json
    $entries=@($script:modeState.cases|Where-Object{$_.name -ceq $Case.name})
    Need ($entries.Count -eq 1) 'Repository measurement must identify one exact controlled invocation'
    Need ($null -ne $Case.report.cache.PSObject.Properties['inputHashReusedBytes']) 'Repository metrics runtime must report actual hash reuse'
    Need ($Case.report.cache.requestedJobs -eq 1 -and $Case.report.cache.effectiveJobs -eq 1) 'Repository paired timings require the same requested and effective jobs1 configuration'
    $observed=$Case.report.cache.dependencyObservation
    Need ($null -ne $observed -and $observed.format -ceq 'NewModCompiler.ReadClosure.v1' -and $observed.frequency -gt 0 -and $observed.semantics -ceq 'inclusive-overlapping-ticks') 'Repository measurements require the versioned actual consumed-read timing contract'
    $observationMs=@{}
    foreach($field in @('readTicks','validationTicks','serializationTicks')){
        Need ($observed.$field -ge 0) "Repository consumed-read timing is negative: $field"
        $observationMs[$field.Substring(0,$field.Length-5)+'Ms']=1000.0*[double]$observed.$field/[double]$observed.frequency
    }
    $samples=@()
    if($CacheState -ceq 'empty-action-cache'){
        $keys=@($observed.manifestKeys)
        Need ($keys.Count -gt 0 -and $keys.Count -eq $observed.streams) 'Cold repository sample omitted its observed stream manifest keys'
        $archive=Join-Path $Case.caseOut 'read-closures';New-Item -ItemType Directory -Path $archive|Out-Null
        $sampleBytes=0L
        foreach($key in $keys){
            Need ($key -cmatch '^[0-9a-f]{64}$') 'Cold repository manifest key is not a complete SHA256 content identity'
            $entry=Join-Path $Case.destination ('.cache/read-closure-v1/'+$key)
            $members=@(Get-ChildItem -LiteralPath (Join-Path $entry 'files') -File)
            Need ($members.Count -eq 1 -and (Hash $members[0].FullName) -ceq $key) 'Cold repository sample does not identify exactly one complete keyed binary manifest'
            $target=Join-Path $archive ($key+'.bin');Copy-Item -LiteralPath $members[0].FullName -Destination $target
            $length=[long]$members[0].Length
            Need ((Get-Item -LiteralPath $target).Length -eq $length -and (Hash $target) -ceq $key) 'Archived repository manifest bytes differ from the verified cold sample'
            $samples+=,@{artifactName=[IO.Path]::GetRelativePath($out,$target).Replace('\','/');relativeTo='payload validation.json directory';bytes=$length;sha256=$key;scope='inner read-closure binary manifest from this cold small mymod invocation; no recompression or product format change'}
            $sampleBytes+=$length
        }
        Need ($sampleBytes -eq $observed.manifestBytes) 'Cold repository sample lengths differ from actual manifest-byte accounting'
    }
    return @{name=$Case.name;arguments=$entries[0].arguments;pairIndex=$PairIndex;actionCacheState=$CacheState;inputVariant=$InputVariant;reportElapsedMs=$Case.report.elapsedMs;controllerElapsedMs=$controller.elapsedMs;cache=$Case.report.cache;dependencyObservationMs=$observationMs;observationTimingSemantics='inclusive-overlapping; do not sum nested phases';cacheStorage=(RepositoryCacheStorage $Case.destination);readClosureSamples=$samples;peakSampledPrivateBytes=$Case.report.cache.peakSampledPrivateBytes;memoryScope='executor sampling, not continuous OS peak';artifactCount=$Case.files.Count;allRawStreamBytesEqualToNoCache=$true}
}
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
function ParallelMeshMod([string]$Path){
    # Bounded asymmetric meshes: 8192 vertices exceeded the conservative
    # prepared-task estimate and correctly stayed on the owner. Use 4096 while
    # keeping the actual two-worker interval assertion unchanged.
    # The long real bridge conversion is still
    # in flight while the next tiny independent document is prepared. No sleep,
    # dispatch hook or quadratic animation compression is involved.
    [xml]$source=[IO.File]::ReadAllText("$root/SDK/Mods/W3xRecovery/Data/Meshes.xml")
    $ns=[Xml.XmlNamespaceManager]::new($source.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
    $mesh=$source.SelectSingleNode('/ea:AssetDeclaration/ea:W3DMesh[1]',$ns)
    Need ($null -ne $mesh) 'Parallel mesh source is missing'
    $vertices=$mesh.SelectSingleNode('ea:Vertices',$ns).InnerXml
    $includes=''
    foreach($item in @(@{name='MeshLong';repeat=1024},@{name='MeshShort';repeat=1})){
        $node=$mesh.CloneNode($true);$node.SetAttribute('id',$item.name)
        $node.SelectSingleNode('ea:Vertices',$ns).InnerXml=($vertices*$item.repeat -join '')
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
function FourWorkerAudioMod([string]$Path){
    $includes=''
    New-Item -ItemType Directory -Path "$Path/Audio" -Force|Out-Null
    foreach($name in @('AudioOne','AudioTwo','AudioThree','AudioFour')){
        MakeWave "$Path/Audio/$name.wav" 196608
        Put "$Path/Data/$name.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioFile id="'+$name+'" File="../Audio/'+$name+'.wav" PCCompression="XAS" IsStreamedOnPC="true"/></AssetDeclaration>')
        $includes+='<Include type="all" source="'+$name+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function MachineParallelEvidence($Case){
    $cache=$Case.report.cache
    Need ($cache.requestedJobs -eq 4 -and $cache.logicalProcessorCount -ge 1 -and $cache.effectiveJobs -ge 1 -and $cache.effectiveJobs -le [Math]::Min(4,$cache.logicalProcessorCount)) 'Effective worker capacity did not respect machine CPU and requested jobs'
    Need ($cache.minimumEffectiveJobs -ge 1 -and $cache.minimumEffectiveJobs -le $cache.effectiveJobs -and $cache.minimumTaskMemoryBudgetBytes -le $cache.taskMemoryBudgetBytes) 'Resource-pressure minimum observations differ'
    Need ($cache.perTaskMemoryLimitBytes -eq [Math]::Floor($cache.minimumTaskMemoryBudgetBytes/2) -and $cache.backendCalls -eq 4) 'Per-task memory/report or actual backend count differs'
    $intervals=@($cache.backendIntervals|Where-Object{$_.typeName -ceq 'AudioFile'})
    Need ($intervals.Count -eq 4 -and @($intervals.sourcePath|Sort-Object -Unique).Count -eq 4) 'Machine-capacity fixture must execute four independent real source documents'
    $edges=@();foreach($interval in $intervals){Need ($interval.startTicks -lt $interval.endTicks) 'A backend interval did not complete';$edges+=,@{tick=$interval.startTicks;delta=1};$edges+=,@{tick=$interval.endTicks;delta=-1}}
    $active=0;$peak=0
    foreach($edge in @($edges|Sort-Object @{Expression={$_.tick}},@{Expression={$_.delta}})){$active+=$edge.delta;$peak=[Math]::Max($peak,$active)}
    Need ($active -eq 0 -and $peak -eq $cache.maxBackendConcurrency -and $peak -le $cache.effectiveJobs) 'Reported concurrency does not match bounded actual backend interval overlap'
    # Each source is <1MiB and the conservative backend estimate is <52MiB.
    # On a host/budget admitting three tasks this must prove >2 real calls;
    # one/two-CPU or genuinely memory-constrained runners retain their limits.
    $canAdmitThree=$cache.minimumEffectiveJobs -ge 3 -and $cache.minimumTaskMemoryBudgetBytes -ge 3*52*1024*1024
    if($canAdmitThree){Need ($peak -gt 2) 'A machine admitting at least three tasks remained hard-capped at two actual backend calls'}
    else{Need ($peak -ge 1) 'CPU/memory-constrained execution omitted real backend work'}
    return @{passed=$true;logicalProcessorCount=$cache.logicalProcessorCount;effectiveJobs=$cache.effectiveJobs;minimumEffectiveJobs=$cache.minimumEffectiveJobs;taskMemoryBudgetBytes=$cache.taskMemoryBudgetBytes;minimumTaskMemoryBudgetBytes=$cache.minimumTaskMemoryBudgetBytes;resourceMemoryQuerySucceeded=$cache.resourceMemoryQuerySucceeded;actualMaximumConcurrency=$peak;moreThanTwoApplicable=$canAdmitThree;intervals=$intervals}
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
$environmentTypes=@('Weather','WaterTransparency','WaterSetting','Environment','LightingSettings','WaterTextureList','TerrainParticleMapping','BounceKickTerrainMapping','ShadowMap','OrdersManagerSettings','TerrainVisualSettings','CameraSettings')
function EnvironmentMod([string]$Path){
    # A real sparse W3X task provides a natural in-flight peer while each newly
    # certified metadata-only backend executes. No sleep or dispatch hook is used.
    ParallelW3xMod $Path
    Remove-Item -LiteralPath "$Path/Data/ParallelShort.xml"
    [xml]$probe=[IO.File]::ReadAllText("$root/recovery/tests/XmlenvironmentProbe.xml")
    $ns=[Xml.XmlNamespaceManager]::new($probe.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
    $includes='<Include type="all" source="W3x.xml"/><Include type="all" source="ParallelLong.xml"/>'
    foreach($type in $script:environmentTypes){
        $node=$probe.SelectSingleNode(('/ea:AssetDeclaration/ea:'+$type+'[1]'),$ns).CloneNode($true)
        $node.SetAttribute('id',('Parallel'+$type))
        if($type -ceq 'ShadowMap'){$node.SetAttribute('MapSize','1024')}
        Put "$Path/Data/$type.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$node.OuterXml+'</AssetDeclaration>')
        $includes+='<Include type="all" source="'+$type+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
$targetingTypes=@('TargetingDistanceCompare','TargetingCombatChainCompare','TargetingInTurretArcCompare','TargetingCompareList')
$visibilityTypes=@('InvisibilityTemplate','InvisibilityManagerSettings')
$radarTypes=@('RadarTerrainImage','UIRadarObjectsLayer')
$conditionsTypes=@('GameplayConditionData')
$objectsetsTypes=@('ObjectFilterAsset','RegisteredObjectSet')
$renderdeviceTypes=@('W3DDeviceSettings','ScorchMarkSettings')
$uiimagesTypes=@('ImageSequence','UnitTypeIcon','DrawGroupInfo')
$healthbarsTypes=@('HealthBarTemplate','HealthBarSettings')
$unitoverlayTypes=@('UnitOverlayIconSettings')
function TargetingMod([string]$Path){MetadataMod $Path 'Xmltargeting' $script:targetingTypes}
function VisibilityMod([string]$Path){MetadataMod $Path 'Xmlvisibility' $script:visibilityTypes}
function RadarMod([string]$Path){MetadataMod $Path 'Xmlradar' $script:radarTypes}
function ConditionsMod([string]$Path){MetadataMod $Path 'Xmlconditions' $script:conditionsTypes}
function ObjectSetsMod([string]$Path){MetadataMod $Path 'Xmlobjectsets' $script:objectsetsTypes}
function RenderDeviceMod([string]$Path){MetadataMod $Path 'Xmlrenderdevice' $script:renderdeviceTypes}
function UiImagesMod([string]$Path){MetadataMod $Path 'Xmluiimages' $script:uiimagesTypes}
function HealthBarsMod([string]$Path){MetadataMod $Path 'Xmlhealthbars' $script:healthbarsTypes}
function UnitOverlayMod([string]$Path){MetadataMod $Path 'Xmlunitoverlay' $script:unitoverlayTypes}
function MetadataMod([string]$Path,[string]$Family,[string[]]$Types){
    ParallelW3xMod $Path
    Remove-Item -LiteralPath "$Path/Data/ParallelShort.xml"
    [xml]$probe=[IO.File]::ReadAllText("$root/recovery/tests/$($Family)Probe.xml")
    $ns=[Xml.XmlNamespaceManager]::new($probe.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
    $includes=''
    if($Family -ceq 'Xmlradar' -or $Family -ceq 'Xmluiimages' -or $Family -ceq 'Xmlunitoverlay'){
        # Real strong references are resolved through their complete production
        # texture/image closure before the native peer and metadata workers.
        Put "$Path/Data/RadarDependencies.xml" '<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Texture id="ParallelRadarTexture" File="cache.tga" Type="StandardTexture" GenerateMipMaps="false" OutputFormat="A8R8G8B8" AllowAutomaticResize="false"/><PackedTextureImage id="ParallelRadarImage" Texture="ParallelRadarTexture" Rotated="false"><Dimensions x="32" y="32"/><Coords x="0" y="0"/><TextureDimensions x="256" y="256"/></PackedTextureImage></AssetDeclaration>'
        if($Family -ceq 'Xmlunitoverlay'){
            [xml]$images=[IO.File]::ReadAllText("$root/recovery/tests/XmluiimagesProbe.xml")
            $imageNs=[Xml.XmlNamespaceManager]::new($images.NameTable);$imageNs.AddNamespace('ea','uri:ea.com:eala:asset')
            $sequence=$images.SelectSingleNode('/ea:AssetDeclaration/ea:ImageSequence',$imageNs)
            Need ($null -ne $sequence) 'Overlay source ImageSequence dependency is missing'
            $deps=[IO.File]::ReadAllText("$Path/Data/RadarDependencies.xml")
            Put "$Path/Data/RadarDependencies.xml" ($deps.Replace('</AssetDeclaration>',($sequence.OuterXml+'</AssetDeclaration>')))
        }
        & python "$root/recovery/probes/TextureRedirect/MakeFixture.py" "$Path/Data/cache.tga"
        Need ($LASTEXITCODE -eq 0) 'Radar real Texture fixture generation failed'
        $includes+='<Include type="all" source="RadarDependencies.xml"/>'
    }
    $includes+='<Include type="all" source="W3x.xml"/><Include type="all" source="ParallelLong.xml"/>'
    foreach($type in $Types){
        $node=$probe.SelectSingleNode(('/ea:AssetDeclaration/ea:'+$type+'[1]'),$ns)
        Need ($null -ne $node -and ![string]::IsNullOrEmpty($node.GetAttribute('id'))) "Metadata source fixture identity differs: $type"
        Put "$Path/Data/$type.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$node.OuterXml+'</AssetDeclaration>')
        $includes+='<Include type="all" source="'+$type+'.xml"/>'
    }
    Put "$Path/Data/Mod.xml" ('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
}
function EnvironmentParallelEvidence($Case){MetadataParallelEvidence $Case $script:environmentTypes 'Xmlenvironment'}
function TargetingParallelEvidence($Case){MetadataParallelEvidence $Case $script:targetingTypes 'Xmltargeting'}
function MetadataParallelEvidence($Case,[string[]]$Types,[string]$Family,[int]$DependencyCalls=0){
    $cache=$Case.report.cache
    Need ($cache.requestedJobs -eq 2 -and $cache.cacheMode -ceq 'Disabled' -and $cache.backendCalls -eq ($Types.Count+2+$DependencyCalls) -and $cache.maxBackendConcurrency -eq 2) "$Family fixture must execute every public root plus real W3X dependencies with two actual calls overlapping"
    $intervals=@($cache.backendIntervals)
    $native=@($intervals|Where-Object{$_.typeName -ceq 'W3DAnimation'})
    $owner=@($intervals|Where-Object{$_.typeName -ceq 'W3DHierarchy'})
    Need ($native.Count -eq 1 -and $owner.Count -eq 1 -and $cache.backendOwnerThreadId -gt 0) 'Metadata peer/hierarchy calls and explicit owner identity must be present'
    $overlapping=@()
    foreach($type in $Types){
        $calls=@($intervals|Where-Object{$_.typeName -ceq $type})
        Need ($calls.Count -eq 1 -and $cache.backendCallsByType.$type -eq 1) "Missing actual $Family backend invocation: $type"
        $call=$calls[0]
        Need ($call.startTicks -lt $call.endTicks -and $call.threadId -ne $cache.backendOwnerThreadId) "$Family backend must complete on a worker distinct from the owning frontend thread: $type"
        if($call.threadId -ne $native[0].threadId -and $call.sourcePath -cne $native[0].sourcePath -and $call.startTicks -lt $native[0].endTicks -and $native[0].startTicks -lt $call.endTicks){$overlapping+=,$type}
    }
    Need ($overlapping.Count -gt 0) "No certified $Family backend actually overlapped the in-flight native peer"
    return @{passed=$true;family=$Family;certifiedTypes=$Types;observedOverlappingTypes=$overlapping;maxBackendConcurrency=$cache.maxBackendConcurrency;intervals=$intervals;scope='All public roots of this family through production jobs2, real cross-family backend overlap against sparse W3X, all raw stream bytes equal to jobs1; observed subset is recorded explicitly'}
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
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Production action-cache PCM/XAS AudioFile complete CDATA and W3X animation/hierarchy/mesh; precise one-action edits, Additional zero compilation, corrupt result and missing CDATA repair, exact four-stream/CDATA no-cache equality, bounded real cross-document native, leased APT XML and streamed XAS AudioFile overlap with jobs2, twelve Xmlenvironment and four public Xmltargeting and complete Radar/Visibility and all public Conditions/Objectsets/Renderdevice/UIimages/Healthbars/Unitoverlay roots with cold/hot/same-mtime single edit and actual cross-family overlap, independent dual Mesh native overlap; not a new original-DLL/SDK gate'}
try{
    Need ($state.sourceCommit -cmatch '^[0-9a-f]{40}$') 'Cannot identify source commit'
    $state.repositoryModMetricsRequested=[bool]$RepositoryModMetrics
    if($RepositoryModMetrics -and !$Preflight){Need ([bool]$ReplayReceipt) 'Repository MOD metrics require a verified successful archived runtime and ReplayReceipt'}
    if($Preflight){
        AudioMod (Join-Path $short 'a');W3xMod (Join-Path $short 'w');ParallelW3xMod (Join-Path $short 'p');ParallelAudioMod (Join-Path $short 'pa');ParallelXmlMod (Join-Path $short 'px');Mp3Mod (Join-Path $short 'mp3');FourWorkerAudioMod (Join-Path $short 'four');ParallelMeshMod (Join-Path $short 'mesh');EnvironmentMod (Join-Path $short 'env');TargetingMod (Join-Path $short 'target');VisibilityMod (Join-Path $short 'visibility');RadarMod (Join-Path $short 'radar');ConditionsMod (Join-Path $short 'conditions');ObjectSetsMod (Join-Path $short 'objectsets');RenderDeviceMod (Join-Path $short 'renderdevice');UiImagesMod (Join-Path $short 'uiimages');HealthBarsMod (Join-Path $short 'healthbars');UnitOverlayMod (Join-Path $short 'unitoverlay')
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
for name,vertices in (('MeshLong',4096),('MeshShort',4)):
    node=E.parse(str(work/('mesh/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}W3DMesh' and node.get('SortLevel')=='0'
    assert len(node.find('ea:Vertices',ns))==vertices
    assert len(node.find('ea:Triangles',ns))==2
    for triangle in node.findall('ea:Triangles/ea:T',ns):
        assert all(0<=int(v.text)<vertices for v in triangle.findall('ea:V',ns))
assert len(b'SortLevel="0"')==len(b'SortLevel="1"')
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
environment_roots={'Weather','WaterTransparency','WaterSetting','Environment','LightingSettings','WaterTextureList','TerrainParticleMapping','BounceKickTerrainMapping','ShadowMap','OrdersManagerSettings','TerrainVisualSettings','CameraSettings'}
for name in environment_roots:
    node=E.parse(str(work/('env/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}'+name
shadow=(work/'env/Data/ShadowMap.xml').read_bytes()
assert shadow.count(b'MapSize="1024"')==1
assert len(b'MapSize="1024"')==len(b'MapSize="1025"')
assert len(E.parse(str(work/'env/Data/Mod.xml')).getroot()[0])==14
targeting_roots={'TargetingDistanceCompare','TargetingCombatChainCompare','TargetingInTurretArcCompare','TargetingCompareList'}
for name in targeting_roots:
    node=E.parse(str(work/('target/Data/'+name+'.xml'))).getroot()[0]
    assert node.tag=='{uri:ea.com:eala:asset}'+name and node.get('id')
assert len(E.parse(str(work/'target/Data/Mod.xml')).getroot()[0])==6
compare=E.parse(str(work/'target/Data/TargetingCompareList.xml')).getroot()[0]
assert len(compare)==4
assert all(ref.text.split(':')[0] in targeting_roots for ref in compare)
list_bytes=(work/'target/Data/TargetingCompareList.xml').read_bytes()
first_value=compare[0].text.encode(); second_value=compare[1].text.encode()
first=list_bytes.index(first_value); second=list_bytes.index(second_value)
assert first<second
span=list_bytes[first:second+len(second_value)]
between=list_bytes[first+len(first_value):second]
reordered=list_bytes.replace(span,second_value+between+first_value,1)
assert len(reordered)==len(list_bytes) and reordered!=list_bytes
schema.assertValid(E.fromstring(reordered))
for folder,types in (('visibility',('InvisibilityTemplate','InvisibilityManagerSettings')),('radar',('RadarTerrainImage','UIRadarObjectsLayer')),('conditions',('GameplayConditionData',)),('objectsets',('ObjectFilterAsset','RegisteredObjectSet')),('renderdevice',('W3DDeviceSettings','ScorchMarkSettings')),('uiimages',('ImageSequence','UnitTypeIcon','DrawGroupInfo')),('healthbars',('HealthBarTemplate','HealthBarSettings')),('unitoverlay',('UnitOverlayIconSettings',))):
    for name in types:
        node=E.parse(str(work/(folder+'/Data/'+name+'.xml'))).getroot()[0]
        assert node.tag=='{uri:ea.com:eala:asset}'+name and node.get('id')
assert (work/'healthbars/Data/HealthBarSettings.xml').read_bytes().count(b'Rule="ANY"')==1
assert b'AmmoPipRange="-17"' in (work/'unitoverlay/Data/UnitOverlayIconSettings.xml').read_bytes()
overlay_deps=E.parse(str(work/'unitoverlay/Data/RadarDependencies.xml')).getroot()
assert {n.tag.split('}')[1] for n in overlay_deps}=={'Texture','PackedTextureImage','ImageSequence'}
assert b'MaxTerrainTracks="117"' in (work/'renderdevice/Data/W3DDeviceSettings.xml').read_bytes()
assert b'FontSize="17"' in (work/'uiimages/Data/DrawGroupInfo.xml').read_bytes()
assert b'CamouflageLevel="17"' in (work/'visibility/Data/InvisibilityTemplate.xml').read_bytes()
assert b'TimeReq="250ms"' in (work/'conditions/Data/GameplayConditionData.xml').read_bytes()
assert b'Type="PLAYER"' in (work/'objectsets/Data/RegisteredObjectSet.xml').read_bytes()
assert len(b'Type="PLAYER"')==len(b'Type="GLOBAL"')
assert b'MinUnitDotSize="7"' in (work/'radar/Data/UIRadarObjectsLayer.xml').read_bytes()
radar=E.parse(str(work/'radar/Data/RadarDependencies.xml')).getroot()
assert {n.tag.split('}')[1] for n in radar}=={'Texture','PackedTextureImage'}
assert (work/'radar/Data/cache.tga').stat().st_size==18+256*256*4
for path in (work/'four/Audio').glob('*.wav'):
    assert len(path.read_bytes())==44+196608*2*2
assert len(list((work/'four/Data').glob('Audio*.xml')))==4
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
    $successfulReplay=Join-Path $CompilerDirectory 'cache-payload-replay.json'
    Need ($ReplayReceipt -or !(Test-Path -LiteralPath $successfulReplay)) 'Successful archive reuse requires explicit ReplayReceipt'
    if($ReplayReceipt){
        Need (!(Test-Path -LiteralPath $replayPath)) 'Cannot combine fixed failed-test replay and successful archive replay'
        $ReplayReceipt=(Resolve-Path -LiteralPath $ReplayReceipt).Path
        Need ($ReplayReceipt -ceq $successfulReplay) 'ReplayReceipt must belong to this prepared runtime directory'
        & python "$PSScriptRoot/PrepareCompilerCacheReplay.py" --repo $root --candidate-commit $state.sourceCommit --verify-prepared $ReplayReceipt *> "$out/replay-verification.log"
        Need ($LASTEXITCODE -eq 0) 'Successful archive API, closure, receipts or runtime verification rejected'
        $replay=Get-Content -LiteralPath $ReplayReceipt -Raw|ConvertFrom-Json
        Need ($replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.testSourceCommit -ceq $state.sourceCommit -and $replay.testScriptSha256 -ceq (Hash $PSCommandPath)) 'Successful replay source or current test script identity differs'
        $expectedFiles=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach($configuration in @('Debug','Release')){foreach($fileName in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){$null=$expectedFiles.Add($configuration+'/bin/'+$fileName)}}
        Need (@($replay.files).Count -eq 6) 'Successful replay requires exactly six runtime identities'
        foreach($identity in $replay.files){
            Need ($expectedFiles.Remove([string]$identity.path) -and $identity.sha256 -cmatch '^[0-9a-f]{64}$' -and (Hash (Join-Path $CompilerDirectory $identity.path)) -ceq $identity.sha256) 'Successful replay runtime identity or bytes differ'
            $replayFiles[[string]$identity.path]=[string]$identity.sha256
        }
        Need ($expectedFiles.Count -eq 0) 'Successful replay runtime set is incomplete'
        $state.binarySourceCommit=$replay.binarySourceCommit;$state.successfulArchiveReplay=$replay
        $state.scope+='; successful archived binarySourceCommit with current testSourceCommit; production inputs unchanged and no production rebuild'
    }
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
    $state.sourceInputs=@('recovery/tests/AudioSdkSupport.cs','recovery/tests/AudioMp3Fixtures.cs','recovery/tests/XmltargetingProbe.xml','recovery/tests/XmlvisibilityProbe.xml','recovery/tests/XmlradarProbe.xml','recovery/tests/XmlconditionsProbe.xml','recovery/tests/XmlobjectsetsProbe.xml','recovery/tests/XmlrenderdeviceProbe.xml','recovery/tests/XmluiimagesProbe.xml','recovery/tests/XmlhealthbarsProbe.xml','recovery/tests/XmlunitoverlayProbe.xml','SDK/Mods/W3xRecovery/Data/Animations.xml','SDK/Mods/W3xRecovery/Data/Dependencies.xml','SDK/Mods/W3xRecovery/Data/Meshes.xml')|ForEach-Object{@{path=$_;sha256=(Hash (Join-Path $root $_))}}
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
        $environment=Join-Path $modeWork 'environment';EnvironmentMod $environment;$script:expectedCustomData=0
        $environmentSerial=RunCli 'environment-serial' $environment (Join-Path $modeWork 'environment-jobs1') $false -Extra @('--jobs=1');Cache $environmentSerial 'disabled'
        $environmentParallel=RunCli 'environment-parallel' $environment (Join-Path $modeWork 'environment-jobs2') $false -Extra @('--jobs=2');Cache $environmentParallel 'disabled';SameOutputs $environmentSerial $environmentParallel
        $modeState.environmentParallel=EnvironmentParallelEvidence $environmentParallel
        $environmentCache=Join-Path $modeWork 'environment-cache'
        $environmentCold=RunCli 'environment-cold' $environment $environmentCache $true -Extra @('--jobs=2');Cache $environmentCold 'cold';SameOutputs $environmentSerial $environmentCold
        $environmentHot=RunCli 'environment-hot' $environment $environmentCache $true -Extra @('--jobs=2');Cache $environmentHot 'hot';SameOutputs $environmentSerial $environmentHot
        Need ($environmentHot.report.cache.backendCalls -eq 0) 'Hot environment fixture must eliminate every backend call'
        SameLengthMutation "$environment/Data/ShadowMap.xml" 'MapSize="1024"' 'MapSize="1025"'
        $environmentPartial=RunCli 'environment-single-edit' $environment $environmentCache $true -Extra @('--jobs=2');Cache $environmentPartial 'partial';DifferentOutputs $environmentHot $environmentPartial
        Need ($environmentPartial.report.cache.backendCalls -eq 1 -and $environmentPartial.report.cache.backendCallsByType.ShadowMap -eq 1) 'One same-length/mtime environment edit must compile only ShadowMap'
        $environmentChanged=RunCli 'environment-edited-serial' $environment (Join-Path $modeWork 'environment-changed-jobs1') $false -Extra @('--jobs=1');Cache $environmentChanged 'disabled';SameOutputs $environmentChanged $environmentPartial
        $targeting=Join-Path $modeWork 'targeting';TargetingMod $targeting;$script:expectedCustomData=0
        $targetingSerial=RunCli 'targeting-serial' $targeting (Join-Path $modeWork 'targeting-jobs1') $false -Extra @('--jobs=1');Cache $targetingSerial 'disabled'
        $targetingParallel=RunCli 'targeting-parallel' $targeting (Join-Path $modeWork 'targeting-jobs2') $false -Extra @('--jobs=2');Cache $targetingParallel 'disabled';SameOutputs $targetingSerial $targetingParallel
        $modeState.targetingParallel=TargetingParallelEvidence $targetingParallel
        $targetingCache=Join-Path $modeWork 'targeting-cache'
        $targetingCold=RunCli 'targeting-cold' $targeting $targetingCache $true -Extra @('--jobs=2');Cache $targetingCold 'cold';SameOutputs $targetingSerial $targetingCold
        $targetingHot=RunCli 'targeting-hot' $targeting $targetingCache $true -Extra @('--jobs=2');Cache $targetingHot 'hot';SameOutputs $targetingSerial $targetingHot
        Need ($targetingHot.report.cache.backendCalls -eq 0) 'Hot targeting fixture must eliminate every backend call'
        # Reorder existing references without modifying any referenced asset.
        # Swapping whole adjacent elements retains total source bytes and mtime.
        $listPath="$targeting/Data/TargetingCompareList.xml"
        [xml]$listXml=[IO.File]::ReadAllText($listPath)
        $ns=[Xml.XmlNamespaceManager]::new($listXml.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
        $refs=$listXml.SelectNodes('/ea:AssetDeclaration/ea:TargetingCompareList/ea:Compare',$ns)
        $before=$refs[0].InnerText;$after=$refs[1].InnerText
        $text=[IO.File]::ReadAllText($listPath)
        $first=$text.IndexOf($before,[StringComparison]::Ordinal);$second=$text.IndexOf($after,[StringComparison]::Ordinal)
        Need ($first -ge 0 -and $second -gt $first) 'Targeting reference-swap tokens are missing or out of order'
        $span=$text.Substring($first,$second+$after.Length-$first)
        $between=$text.Substring($first+$before.Length,$second-$first-$before.Length)
        SameLengthMutation $listPath $span ($after+$between+$before)
        $targetingPartial=RunCli 'targeting-single-edit' $targeting $targetingCache $true -Extra @('--jobs=2');Cache $targetingPartial 'partial';DifferentOutputs $targetingHot $targetingPartial
        Need ($targetingPartial.report.cache.backendCalls -eq 1 -and $targetingPartial.report.cache.backendCallsByType.TargetingCompareList -eq 1) 'Reference reorder must compile only the changed targeting list'
        $targetingChanged=RunCli 'targeting-edited-serial' $targeting (Join-Path $modeWork 'targeting-changed-jobs1') $false -Extra @('--jobs=1');Cache $targetingChanged 'disabled';SameOutputs $targetingChanged $targetingPartial
        foreach($family in @('visibility','radar','conditions','objectsets','renderdevice','uiimages','healthbars','unitoverlay')){
            $metadata=Join-Path $modeWork $family;$script:expectedCustomData=0
            if($family -eq 'visibility'){VisibilityMod $metadata;$types=$script:visibilityTypes;$dependencies=0;$ownerCalls=0;$changedType='InvisibilityTemplate'}
            elseif($family -eq 'radar'){RadarMod $metadata;$types=$script:radarTypes;$dependencies=2;$ownerCalls=1;$changedType='UIRadarObjectsLayer'}
            elseif($family -eq 'conditions'){ConditionsMod $metadata;$types=$script:conditionsTypes;$dependencies=0;$ownerCalls=0;$changedType='GameplayConditionData'}
            elseif($family -eq 'objectsets'){ObjectSetsMod $metadata;$types=$script:objectsetsTypes;$dependencies=0;$ownerCalls=0;$changedType='RegisteredObjectSet'}
            elseif($family -eq 'renderdevice'){RenderDeviceMod $metadata;$types=$script:renderdeviceTypes;$dependencies=0;$ownerCalls=0;$changedType='W3DDeviceSettings'}
            elseif($family -eq 'uiimages'){UiImagesMod $metadata;$types=$script:uiimagesTypes;$dependencies=2;$ownerCalls=1;$changedType='DrawGroupInfo'}
            elseif($family -eq 'healthbars'){HealthBarsMod $metadata;$types=$script:healthbarsTypes;$dependencies=0;$ownerCalls=0;$changedType='HealthBarSettings'}
            else{UnitOverlayMod $metadata;$types=$script:unitoverlayTypes;$dependencies=3;$ownerCalls=1;$changedType='UnitOverlayIconSettings'}
            $metadataSerial=RunCli ($family+'-serial') $metadata (Join-Path $modeWork ($family+'-jobs1')) $false -Extra @('--jobs=1');Cache $metadataSerial 'disabled'
            $metadataParallel=RunCli ($family+'-parallel') $metadata (Join-Path $modeWork ($family+'-jobs2')) $false -Extra @('--jobs=2');Cache $metadataParallel 'disabled';SameOutputs $metadataSerial $metadataParallel
            $modeState[($family+'Parallel')]=MetadataParallelEvidence $metadataParallel $types $family $dependencies
            $metadataCache=Join-Path $modeWork ($family+'-cache')
            $metadataCold=RunCli ($family+'-cold') $metadata $metadataCache $true -Extra @('--jobs=2');Cache $metadataCold 'cold';SameOutputs $metadataSerial $metadataCold
            $metadataHot=RunCli ($family+'-hot') $metadata $metadataCache $true -Extra @('--jobs=2');Cache $metadataHot 'hot';SameOutputs $metadataSerial $metadataHot
            Need ($metadataHot.report.cache.backendCalls -eq $ownerCalls) "Hot $family must retain exactly its unprepared Texture owner calls"
            if($family -eq 'visibility'){SameLengthMutation "$metadata/Data/InvisibilityTemplate.xml" 'CamouflageLevel="17"' 'CamouflageLevel="18"'}
            elseif($family -eq 'radar'){SameLengthMutation "$metadata/Data/UIRadarObjectsLayer.xml" 'MinUnitDotSize="7"' 'MinUnitDotSize="8"'}
            elseif($family -eq 'conditions'){SameLengthMutation "$metadata/Data/GameplayConditionData.xml" 'TimeReq="250ms"' 'TimeReq="350ms"'}
            elseif($family -eq 'objectsets'){SameLengthMutation "$metadata/Data/RegisteredObjectSet.xml" 'Type="PLAYER"' 'Type="GLOBAL"'}
            elseif($family -eq 'renderdevice'){SameLengthMutation "$metadata/Data/W3DDeviceSettings.xml" 'MaxTerrainTracks="117"' 'MaxTerrainTracks="118"'}
            elseif($family -eq 'uiimages'){SameLengthMutation "$metadata/Data/DrawGroupInfo.xml" 'FontSize="17"' 'FontSize="18"'}
            elseif($family -eq 'healthbars'){SameLengthMutation "$metadata/Data/HealthBarSettings.xml" 'Rule="ANY"' 'Rule="ALL"'}
            else{SameLengthMutation "$metadata/Data/UnitOverlayIconSettings.xml" 'AmmoPipRange="-17"' 'AmmoPipRange="-18"'}
            $metadataPartial=RunCli ($family+'-single-edit') $metadata $metadataCache $true -Extra @('--jobs=2');Cache $metadataPartial 'partial';DifferentOutputs $metadataHot $metadataPartial
            Need ($metadataPartial.report.cache.backendCalls -eq (1+$ownerCalls) -and $metadataPartial.report.cache.backendCallsByType.$changedType -eq 1) "Single $family edit must compile only the changed metadata and uncertified owner dependencies"
            $metadataChanged=RunCli ($family+'-edited-serial') $metadata (Join-Path $modeWork ($family+'-changed-jobs1')) $false -Extra @('--jobs=1');Cache $metadataChanged 'disabled';SameOutputs $metadataChanged $metadataPartial
        }
        $modeState.certifiedParallel=@()
        foreach($family in @('audio','xml','mesh')){
            $fixture=Join-Path $modeWork ('parallel-'+$family)
            if($family -eq 'audio'){ParallelAudioMod $fixture;$script:expectedCustomData=2;$type='AudioFile'}
            elseif($family -eq 'xml'){ParallelXmlMod $fixture;$script:expectedCustomData=0;$type='AptAptData'}
            else{ParallelMeshMod $fixture;$script:expectedCustomData=0;$type='W3DMesh'}
            $controlCase=RunCli ($family+'-parallel-control') $fixture (Join-Path $modeWork ($family+'-parallel-serial')) $false -Extra @('--jobs=1');Cache $controlCase 'disabled'
            Need ($controlCase.report.cache.backendCalls -eq 2 -and $controlCase.report.cache.maxBackendConcurrency -eq 1) 'Serial control did not execute exactly two serial real backend calls'
            $parallelCase=RunCli ($family+'-cross-document-parallel') $fixture (Join-Path $modeWork ($family+'-parallel-jobs2')) $false -Extra @('--jobs=2');Cache $parallelCase 'disabled';SameOutputs $controlCase $parallelCase
            $modeState.certifiedParallel+=,(CertifiedParallelEvidence $parallelCase $type)
            $cachedDestination=Join-Path $modeWork ($family+'-parallel-cached')
            $parallelCold=RunCli ($family+'-parallel-cold') $fixture $cachedDestination $true -Extra @('--jobs=2');Cache $parallelCold 'cold';SameOutputs $controlCase $parallelCold
            $parallelHot=RunCli ($family+'-parallel-hot') $fixture $cachedDestination $true -Extra @('--jobs=2');Cache $parallelHot 'hot';SameOutputs $controlCase $parallelHot
            Need ($parallelHot.report.cache.backendCalls -eq 0 -and $parallelHot.report.cache.maxBackendConcurrency -eq 0) 'Parallel hot hits were reported as backend calls'
            if($family -eq 'mesh'){
                SameLengthMutation "$fixture/Data/MeshLong.xml" 'SortLevel="0"' 'SortLevel="1"'
            }else{
                $changedInput=if($family -eq 'audio'){"$fixture/Audio/AudioLong.wav"}else{"$fixture/Data/AptLong.bin"}
                $stamp=[IO.File]::GetLastWriteTimeUtc($changedInput);$bytes=[IO.File]::ReadAllBytes($changedInput);$length=$bytes.Length
                $offset=if($family -eq 'audio'){44}else{0}
                $bytes[$offset]=$bytes[$offset] -bxor 0x34;[IO.File]::WriteAllBytes($changedInput,$bytes);[IO.File]::SetLastWriteTimeUtc($changedInput,$stamp)
                Need ((Get-Item -LiteralPath $changedInput).Length -eq $length -and [IO.File]::GetLastWriteTimeUtc($changedInput) -eq $stamp) 'Parallel payload mutation must retain exact length/mtime'
            }
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
        $four=Join-Path $modeWork 'four';FourWorkerAudioMod $four;$script:expectedCustomData=4
        $fourSerial=RunCli 'four-audio-serial-control' $four (Join-Path $modeWork 'four-serial') $false -Extra @('--jobs=1');Cache $fourSerial 'disabled'
        $fourParallel=RunCli 'four-audio-machine-budget-parallel' $four (Join-Path $modeWork 'four-jobs4') $false -Extra @('--jobs=4');Cache $fourParallel 'disabled';SameOutputs $fourSerial $fourParallel
        $modeState.machineParallel=MachineParallelEvidence $fourParallel
        if($RepositoryModMetrics){RepositoryModMetrics $modeWork}
        $modeState.passed=$true
    }
    foreach($file in $state.sourceInputs){Need ((Hash (Join-Path $root $file.path)) -ceq $file.sha256) 'Original payload fixture source changed'}
    if($RepositoryModMetrics){
        foreach($inputFile in $state.repositorySdkInputs){Need ((Hash (Join-Path "$root/SDK" $inputFile.path)) -ceq $inputFile.sha256) 'Repository metrics changed an original SDK input'}
        Need (@(Get-ChildItem -LiteralPath "$root/SDK" -Recurse -File -Force).Count -eq $state.repositorySdkInputs.Count) 'Repository metrics changed the SDK file inventory'
    }
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{
    $state|ConvertTo-Json -Depth 18|Set-Content "$out/validation.json"
    try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "Payload cache work cleanup failed: $_"}
}
