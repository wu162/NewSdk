#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$CompilerDirectory,[Parameter(Mandatory=$true)][string]$OutputDirectory,[ValidateRange(15,600)][int]$TimeoutSeconds=180)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite CLI acceptance evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('q'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
$oldPath=$env:PATH
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Within([string]$Directory,[string]$Path){
    $base=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$full=[IO.Path]::GetFullPath($Path)
    $full.Equals($base,[StringComparison]::OrdinalIgnoreCase) -or $full.StartsWith($base+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function Snapshot([string]$Directory){
    $map=@{};Get-ChildItem -LiteralPath $Directory -File -Recurse|ForEach-Object{$map[$_.FullName.Substring($Directory.Length)]=Hash $_.FullName};$map
}
function Unchanged([string]$Directory,$Expected){
    $actual=Snapshot $Directory;Need ($actual.Count -eq $Expected.Count) "Input tree changed: $Directory"
    foreach($key in $Expected.Keys){Need ($actual.ContainsKey($key) -and $actual[$key] -ceq $Expected[$key]) "Input mutated: $Directory$key"}
}
function MakeMod([string]$Path,[string]$Probe){
    New-Item -ItemType Directory -Force -Path (Join-Path $Path 'Data/AptUI'),(Join-Path $Path 'Data/AdditionalMaps')|Out-Null
    [IO.File]::WriteAllText((Join-Path $Path 'Data/Mod.xml'),$Probe)
    [IO.File]::WriteAllText((Join-Path $Path 'Data/AptUI/One.xml'),$Probe.Replace('Default="72.5%"','Default="22.5%"'))
    [IO.File]::WriteAllText((Join-Path $Path 'Data/AptUI/Two.xml'),$Probe.Replace('Default="72.5%"','Default="32.5%"'))
    [IO.File]::WriteAllText((Join-Path $Path 'Data/AdditionalMaps/MapMetaData_One.xml'),$Probe.Replace('Default="72.5%"','Default="42.5%"'))
}
function RunCli([string]$Name,[string]$Command,[string]$Mod,[string]$Sdk,[string]$Selection,[int]$Expected,[int]$StreamCount,[bool]$Cancel=$false,[string[]]$Extra=@(),[string]$HoldReadFile=$null){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    $destination=Join-Path $script:modeWork ('o'+$script:caseIndex);$script:caseIndex++
    New-Item -ItemType Directory -Path $destination|Out-Null
    $arguments=@($Command,'--sdk-root',$Sdk)
    if($Command -ne 'doctor'){$arguments+=@('--mod',$Mod,'--out',$destination,'--package-content',$Selection)}
    $arguments+=@($Extra)
    $events=Join-Path $destination 'build.events.jsonl';$report=Join-Path $destination 'build-report.json'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    if($HoldReadFile){$request.holdReadFile=$HoldReadFile}
    $requestFile=Join-Path $caseOut 'request.json';$controlFile=Join-Path $caseOut 'control.json'
    $request|ConvertTo-Json -Depth 5|Set-Content $requestFile
    & $script:control $requestFile $controlFile *> (Join-Path $caseOut 'controller.log')
    $controlExit=$LASTEXITCODE
    $audit=Get-Content -LiteralPath $controlFile -Raw|ConvertFrom-Json
    $entry=@{name=$Name;command=$Command;arguments=$arguments;destination=$destination;expectedExit=$Expected;control=$audit;passed=$false}
    $script:modeState.cases+=,$entry
    try {
        Need ($controlExit -eq 0 -and $audit.succeeded) "Real CLI process/control failed: $Name; cancellation is unverified unless actual CTRL_BREAK and exit6 are recorded"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Single production process/no-worker audit failed'
        Need ($audit.processExitCode -eq $Expected) "$Name CLI exit $($audit.processExitCode) differs from $Expected"
        if($Cancel){Need ($audit.signalSent -and $audit.streamStartedObserved -and $audit.processExitCode -eq 6) 'Real CtrlBreak cancellation was not verified'}
        $rows=@();$body=$null
        if(Test-Path -LiteralPath $events){$rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})}
        if(Test-Path -LiteralPath $report){$body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json}
        if($Command -eq 'doctor'){
            Need ((Get-Content (Join-Path $caseOut 'stdout.log') -Raw).Contains('CORE_ENVIRONMENT_OK')) 'Doctor did not verify actual environment'
        } elseif($Name -eq 'log-directory-rejected') {
            Need ($Expected -eq 2 -and (Get-Content (Join-Path $caseOut 'stderr.log') -Raw).Contains('CLI_LOG_PATH')) 'Diagnostic directory must be refused during argument parsing'
            Need ($null -eq $body -and $rows.Count -eq 0) 'Invalid diagnostic path started a build'
        } elseif($Name -eq 'required-log-fault') {
            Need ($audit.readLockHeld -and $audit.readLockPath -ceq $HoldReadFile) 'External controller did not hold the real required-output file against replacement'
            Need ($Expected -eq 7 -and (Get-Content (Join-Path $caseOut 'stderr.log') -Raw).Contains('OUTPUT_FAILED')) 'Required output channel failure must explicitly fail output with exit7'
        } else {
            Need ($null -ne $body -and $rows.Count -gt 0) 'CLI durable report/events missing'
            Need ($body.exitCode -eq $Expected) 'Report/process exit codes disagree'
            Need (@($rows|Where-Object{$_.eventType -eq 'BuildStarted'}).Count -eq 1 -and @($rows|Where-Object{$_.eventType -eq 'BuildFinished'}).Count -eq 1) 'CLI terminal event count changed'
            Need ($rows[-1].eventType -ceq 'BuildFinished') 'Event followed terminal BuildFinished'
            $started=@($rows|Where-Object{$_.eventType -eq 'BuildStarted'})[0]
            Need ($started.payload.processId -eq $audit.processId) 'BuildStarted is not the actual production process PID'
            $sequence=0
            foreach($row in $rows){$sequence++;Need ($row.buildId -ceq $body.buildId -and $row.sequence -eq $sequence) 'Build ID/sequence changed within CLI execution'}
            Need ($rows[-1].payload.exitCode -eq $Expected -and $rows[-1].payload.status -ceq $body.status) 'Terminal event/report status disagree'
            $streamStarts=@($rows|Where-Object{$_.eventType -eq 'StreamStarted'});$streamEnds=@($rows|Where-Object{$_.eventType -eq 'StreamCompleted'})
            if($Cancel){Need ($streamStarts.Count -ge 1 -and $streamStarts.Count -le @($body.plan.Tasks).Count) 'Cancellation did not observe an actual selected stream'}
            else{Need ($streamStarts.Count -eq $StreamCount) 'CLI stream selection count differs'}
            Need ($streamEnds.Count -eq $streamStarts.Count) 'Stream completion count differs from starts'
            foreach($stream in $streamStarts){Need (@($streamEnds|Where-Object{$_.streamId -ceq $stream.streamId}).Count -eq 1) 'Stream terminal event missing or duplicated'}
            Need (@(Get-ChildItem -LiteralPath $destination -File -Recurse -Filter '*.tmp').Count -eq 0) 'Atomic report temporary file leaked'
            Need (@($body.externalTools).Count -eq 0) 'Production claimed external compiler tools'
            if($Expected -eq 0 -and $Command -eq 'build'){
                Need (@($body.artifacts).Count -eq 4*$StreamCount) 'Expected four real linked files per selected task'
                foreach($artifact in $body.artifacts){
                    Need ((Within (Join-Path $destination ('.build/'+$body.buildId)) $artifact.Path) -and (Test-Path -LiteralPath $artifact.Path)) 'Artifact escaped this atomic isolated build staging'
                    Need ((Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size -and (Hash $artifact.Path) -ceq $artifact.Sha256) 'Artifact identity differs from report'
                }
                Need ($body.publicationStatus -ceq 'IsolatedStagingOnly' -and $body.executionMode -ceq 'InProcess') 'CLI falsely claimed publication or external execution'
            } elseif($Command -eq 'plan'){
                Need (@($body.artifacts).Count -eq 0 -and $body.executionMode -ceq 'PlanOnly') 'Plan compiled assets'
            } else {
                Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled CLI accepted artifacts'
                Need (!(Test-Path -LiteralPath (Join-Path $destination ('.build/'+$body.buildId)))) 'Failed/cancelled CLI retained this build staging'
            }
        }
        $entry.passed=$true;$entry.report=$body;$entry.eventCount=$rows.Count
        return @{report=$body;events=$rows;destination=$destination;caseOut=$caseOut;mod=$Mod;sdk=$Sdk;name=$Name}
    } catch {
        # Surface bounded child-process diagnostics in Actions while retaining
        # the full files and original failing assertion in the evidence archive.
        try {
            Write-Host "FAILED real CLI case: $Name"
            if($audit.error){Write-Host ([string]$audit.error)}
            foreach($logName in @('stdout.log','stderr.log','controller.log')){
                $logPath=Join-Path $caseOut $logName
                if(Test-Path -LiteralPath $logPath){
                    Write-Host "$Name / $logName (last 30 lines)"
                    Get-Content -LiteralPath $logPath -Tail 30|Out-Host
                }
            }
        } catch {Write-Host "Could not read CLI failure log tail: $($_.Exception.Message)"}
        throw
    } finally {
        if(Test-Path -LiteralPath $destination){Copy-Item -LiteralPath $destination -Destination (Join-Path $caseOut 'output') -Recurse}
    }
}
function TaskOutputs($Case,$Task){
    $prefix=Join-Path $Case.destination ('.build/'+$Case.report.buildId+'/'+$Task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($Task.InputPath))
    $map=@{};foreach($artifact in $Case.report.artifacts){if(Within $prefix $artifact.Path){$map[$artifact.Path.Substring($prefix.Length)]=$artifact.Sha256}}
    Need ($map.Count -eq 4) 'Selected task does not own exactly four streams';$map
}
# These fixtures have no runtime/manifest references. Decode the real source
# table without normalizing or rewriting any produced byte.
function AssertLogicalSources($Case){
    $records=@()
    foreach($task in $Case.report.plan.Tasks){
        $prefix=Join-Path $Case.destination ('.build/'+$Case.report.buildId+'/'+$task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.InputPath))
        $files=@($Case.report.artifacts|Where-Object{(Within $prefix $_.Path) -and [IO.Path]::GetExtension($_.Path) -ceq '.manifest'})
        Need ($files.Count -eq 1) 'Expected one manifest per source-path semantic case'
        $bytes=[IO.File]::ReadAllBytes($files[0].Path)
        Need ($bytes.Length -ge 48 -and $bytes[0] -eq 0 -and $bytes[1] -eq 1 -and $bytes[2] -eq 6 -and $bytes[3] -eq 0) 'Unexpected linked Win32 manifest header'
        $count=[BitConverter]::ToUInt32($bytes,12);$names=[BitConverter]::ToUInt32($bytes,40);$sources=[BitConverter]::ToUInt32($bytes,44)
        Need ($count -eq 3 -and [BitConverter]::ToUInt32($bytes,32) -eq 0 -and [BitConverter]::ToUInt32($bytes,36) -eq 0) 'Source-path fixture reference layout changed'
        $start=48+48*$count+$names
        Need ($sources -gt 0 -and $start+$sources -eq $bytes.Length) 'Manifest source table bounds invalid'
        $expected='DATA:'+([IO.Path]::GetRelativePath((Join-Path $Case.mod 'Data'),$task.InputPath).Replace('\','/').ToLowerInvariant())
        $expectedBytes=[Text.Encoding]::ASCII.GetBytes($expected+[char]0)
        Need ($sources -eq $expectedBytes.Length) 'Manifest logical source table length differs'
        for($i=0;$i -lt $expectedBytes.Length;$i++){Need ($bytes[$start+$i] -eq $expectedBytes[$i]) 'Manifest logical source table differs from DATA alias'}
        for($i=0;$i -lt $count;$i++){Need ([BitConverter]::ToUInt32($bytes,(48+48*$i+28)) -eq 0) 'Armor source entry does not reference the inspected alias'}
        $records+=,@{taskId=$task.Id;expectedSource=$expected;manifestSha256=(Hash $files[0].Path);byteTableVerified=$true}
    }
    return $records
}
function EqualTask($Combined,$Single,[string]$Group){
    foreach($task in @($Combined.report.plan.Tasks|Where-Object{$_.Content -ceq $Group})){
        $same=@($Single.report.plan.Tasks|Where-Object{$_.Id -ceq $task.Id});Need ($same.Count -eq 1) 'Single/combined selected task identity differs'
        $left=TaskOutputs $Combined $task;$right=TaskOutputs $Single $same[0]
        Need ($left.Count -eq $right.Count) 'Single/combined file count differs'
        foreach($key in $left.Keys){Need ($right.ContainsKey($key) -and $left[$key] -ceq $right[$key]) 'Fresh production CLI single/combined bytes differ'}
    }
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Real clean production EXE processes only: selected multi-stream CLI, independent fresh-process group comparisons and actual CtrlBreak; original BAB oracle comparison remains separately reported'}
try {
    Push-Location $root
    $state.sourceCommit=(git rev-parse HEAD).Trim();Need ($LASTEXITCODE -eq 0) 'Cannot identify source commit'
    $state.scriptSha256=Hash $PSCommandPath;$state.controllerSourceSha256=Hash "$root/recovery/tests/CompilerCliControl.cs"
    $sourceSdk=Join-Path $root 'SDK';$schemaSnapshot=Snapshot (Join-Path $sourceSdk 'Schemas/xsd')
    $configHash=Hash (Join-Path $sourceSdk 'tools/BinaryAssetBuilder.exe.config');$probeFile=Join-Path $sourceSdk 'Mods/mymod/Data/RecoveryArmorProbe.xml';$probeHash=Hash $probeFile;$probe=[IO.File]::ReadAllText($probeFile)
    $state.sdkSchemaInputs=$schemaSnapshot;$state.sdkConfigSha256=$configHash;$state.sdkProbeSha256=$probeHash
    $controllerDirectory=Join-Path $out 'controller'
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory $controllerDirectory
    $controllerBuild=Get-Content (Join-Path $controllerDirectory 'build.json') -Raw|ConvertFrom-Json
    Need ($controllerBuild.succeeded -and $controllerBuild.sourceCommit -ceq $state.sourceCommit -and $controllerBuild.sourceSha256 -ceq $state.controllerSourceSha256) 'CLI controller build identity differs'
    $state.controllerCompiler=$controllerBuild.controllerCompiler;$state.controllerReferences=$controllerBuild.controllerReferences
    $state.controllerSha256=Hash $script:control
    Need ($state.controllerSha256 -ceq $controllerBuild.controllerSha256) 'CLI controller binary identity differs'
    $env:PATH=(@($oldPath -split ';'|Where-Object{$_ -notmatch '[\\/]SDK[\\/]tools'}) -join ';')
    foreach($mode in @('Debug','Release')){
        $script:modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $script:modeWork=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName;$script:caseIndex=0
        $script:runtime=(New-Item -ItemType Directory -Path (Join-Path $modeWork 'r')).FullName
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime}
        $script:exe=Join-Path $runtime 'NewModCompiler.exe';$exeHash=Hash $exe
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$modeOut/prepare-texture.log"
        Need ($LASTEXITCODE -eq 0) 'Production EXE texture preparation failed'
        $patchPath=Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll.patch.json';$patch=Get-Content $patchPath -Raw|ConvertFrom-Json
        Need ($patch.targetExeSha256 -ceq $exeHash -and $patch.protectedPeAudit.status -ceq 'passed' -and $patch.patchedSha256 -ceq (Hash (Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll'))) 'Texture deployment does not bind the actual production EXE'
        Need (@(Get-ChildItem $runtime -File).Count -eq 5) 'Unexpected clean production runtime files'
        $runtimeSnapshot=Snapshot $runtime;Copy-Item $patchPath "$modeOut/texture.patch.json"
        $script:modeState=@{configuration=$mode;fixtureAssetScope='Known schema-valid Armor probes in base/APT/maps entry paths test orchestration; not new APT/map payload coverage';passed=$false;exeSha256=$exeHash;runtimeInputs=$runtimeSnapshot;textureDeployment=$patch;cases=@();baselineCases=@()};$state.modes+=,$modeState
        $sdk=(New-Item -ItemType Directory -Path (Join-Path $modeWork 's')).FullName
        New-Item -ItemType Directory -Path "$sdk/tools","$sdk/Schemas"|Out-Null
        Copy-Item -LiteralPath (Join-Path $sourceSdk 'Schemas/xsd') -Destination "$sdk/Schemas" -Recurse
        Copy-Item -LiteralPath (Join-Path $sourceSdk 'tools/BinaryAssetBuilder.exe.config') -Destination "$sdk/tools"
        $mod=Join-Path $modeWork '外部 MOD';MakeMod $mod $probe
        $inside=Join-Path $sdk 'Mods/内部 MOD';MakeMod $inside $probe
        $asciiMod=Join-Path $modeWork 'ascii-mod';MakeMod $asciiMod $probe
        $modSnapshot=Snapshot $mod;$asciiSnapshot=Snapshot $asciiMod;$sdkSnapshot=Snapshot $sdk
        $doctor=RunCli 'doctor' 'doctor' $null $sdk $null 0 0
        $plan=RunCli 'plan-all' 'plan' $mod $sdk 'base,apt,maps' 0 0
        Need (@($plan.report.plan.Tasks).Count -eq 4) 'Real CLI plan omitted base+two apt+map tasks'
        $combined=RunCli 'combined' 'build' $mod $sdk 'base,apt,maps' 0 4
        # The old ANSI BAB cannot reliably open Unicode input paths. Keep those
        # production cases and their exact DATA source-table checks, and give the
        # independent old-tool oracle an identical ASCII-path companion.
        $ascii=RunCli 'ascii-companion' 'build' $asciiMod $sdk 'base,apt,maps' 0 4
        foreach($group in @('base','apt','maps')){EqualTask $combined $ascii $group}
        $modeState.logicalSourceSemantics=@{unicode=@(AssertLogicalSources $combined);ascii=@(AssertLogicalSources $ascii);allFourStreamsByteIdentical=$true}
        $base=RunCli 'base-only' 'build' $mod $sdk 'base' 0 1;$apt=RunCli 'apt-only' 'build' $mod $sdk 'apt' 0 2;$maps=RunCli 'maps-only' 'build' $mod $sdk 'maps' 0 1
        EqualTask $combined $base 'base';EqualTask $combined $apt 'apt';EqualTask $combined $maps 'maps'
        $internal=RunCli 'sdk-internal' 'build' $inside $sdk 'base' 0 1
        $emptyMod=Join-Path $modeWork '空可选 MOD';New-Item -ItemType Directory -Path "$emptyMod/Data"|Out-Null
        [IO.File]::WriteAllText("$emptyMod/Data/Mod.xml",$probe)
        $empty=RunCli 'empty-optional' 'build' $emptyMod $sdk 'apt,maps' 0 0
        Need (@($empty.report.plan.Skipped).Count -eq 2) 'Empty optional groups were not explicitly skipped'
        $unsupported=RunCli 'cache-rejected' 'build' $mod $sdk 'base' 2 0 $false @('--bc=true')
        Need (@($unsupported.report.diagnostics|Where-Object{$_.code -ceq 'CORE_OPTION_UNSUPPORTED'}).Count -eq 1) 'Unsupported cache was not explicitly refused'
        $badMod=Join-Path $modeWork '坏 XML MOD';New-Item -ItemType Directory -Path "$badMod/Data"|Out-Null
        [IO.File]::WriteAllText("$badMod/Data/Mod.xml",'<AssetDeclaration>broken')
        $bad=RunCli 'bad-xml' 'build' $badMod $sdk 'base' 4 1
        [IO.File]::WriteAllText("$badMod/Data/Mod.xml",$probe)
        $retry=RunCli 'retry-after-bad' 'build' $badMod $sdk 'base' 0 1
        $blockedDirectory=Join-Path $modeWork 'log-directory';New-Item -ItemType Directory -Path $blockedDirectory|Out-Null
        $directoryRejected=RunCli 'log-directory-rejected' 'build' $mod $sdk 'base' 2 0 $false @('--log',$blockedDirectory)
        # A read handle permits read-only parsing/physical-path inspection, but
        # withholds FILE_SHARE_DELETE so the real sink cannot replace its file.
        $fault=Join-Path $modeWork 'blocked-log.txt';[IO.File]::WriteAllText($fault,'held output')
        $logFault=RunCli 'required-log-fault' 'build' $mod $sdk 'base' 7 0 $false @('--log',$fault) $fault
        Need ([IO.File]::ReadAllText($fault) -ceq 'held output') 'Required-log fault truncated or replaced the held file'
        $afterLogFault=RunCli 'retry-after-log-fault' 'build' $mod $sdk 'base' 0 1 $false @('--log',$fault)
        EqualTask $combined $afterLogFault 'base'
        Need ([IO.File]::ReadAllText($fault).Contains('BuildFinished')) 'Retry did not reuse the released required log path'
        $cancelled=RunCli 'ctrl-break' 'build' $mod $sdk 'base,apt,maps' 6 1 $true
        Need ($cancelled.report.status -ceq 'cancelled' -and @($cancelled.report.artifacts).Count -eq 0) 'Actual cancellation report accepted output'
        $afterCancel=RunCli 'retry-after-cancel' 'build' $mod $sdk 'base' 0 1
        foreach($selection in @(@{name='mod-a';task=@($ascii.report.plan.Tasks|Where-Object{$_.Content -ceq 'base'})[0]},@{name='mod-b';task=@($ascii.report.plan.Tasks|Where-Object{$_.Content -ceq 'apt'})[0]})){
            $task=$selection.task;$prefix=Join-Path $ascii.destination ('.build/'+$ascii.report.buildId+'/'+$task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.InputPath));$outputs=@{}
            foreach($artifact in $ascii.report.artifacts){if(Within $prefix $artifact.Path){$outputs[[IO.Path]::GetExtension($artifact.Path).Substring(1)]=$artifact.Path}}
            $modeState.baselineCases+=,@{name=$selection.name;modRoot=$asciiMod;inputPath=$task.InputPath;sdkRoot=$sdk;originalToolsRoot=(Join-Path $sourceSdk 'tools');stagingDirectory=$prefix;outputs=$outputs;scope='Identical ASCII-path companion of Unicode combined CLI base/first-APT stream; strict byte comparison, no output normalization; not the remaining two selected streams'}
        }
        @{succeeded=$true;sourceCommit=$state.sourceCommit;runId=$state.runId;baselineCases=$modeState.baselineCases;scope='Real production CLI base plus first APT task only'}|ConvertTo-Json -Depth 8|Set-Content "$modeOut/cli-result.json"
        # Baseline comparison is an independent original-tool step, never a product child.
        & "$PSScriptRoot/TestCompilerBackendBaseline.ps1" -FixtureResult "$modeOut/cli-result.json" -OutputDirectory "$modeOut/original-baseline" -TimeoutSeconds $TimeoutSeconds
        Need ($LASTEXITCODE -eq 0) 'Independent original BAB CLI task baseline failed'
        # Exercise actual business roots in the same production process, then
        # compare every selected stream against a fresh single-group process.
        $businessMod=Join-Path $modeWork 'business'
        $businessReceipt=@{succeeded=$false;sourceCommit=$state.sourceCommit;runId=$state.runId;configuration=$mode;baselineCases=@();scope='Three CLI tasks: Armor base, 12 file-backed APT DataBlob assets and three MapMetaData assets; strict current-original APT/maps oracle, not executable UI/APT parser, full SDK, cache or publication acceptance'}
        $modeState.business=$businessReceipt
        try {
            & python "$PSScriptRoot/PrepareCompilerCliBusiness.py" $businessMod *> "$modeOut/business-prepare.log"
            Need ($LASTEXITCODE -eq 0) 'Source-XSD-valid CLI business fixture preparation failed'
            $businessManifest=Join-Path $businessMod 'fixture-manifest.json'
            Copy-Item -LiteralPath $businessManifest -Destination "$modeOut/business-fixture.json"
            $prepared=Get-Content -LiteralPath $businessManifest -Raw|ConvertFrom-Json
            Need ($prepared.prepared -and $prepared.xsdValidated -and !$prepared.executed -and @($prepared.expectedTasks).Count -eq 3) 'Business fixture preparation receipt invalid'
            $businessReceipt.fixtureManifestSha256=Hash $businessManifest
            $businessReceipt.generatorSha256=Hash "$PSScriptRoot/PrepareCompilerCliBusiness.py"
            $businessSnapshot=Snapshot $businessMod
            $businessCombined=RunCli 'business-combined' 'build' $businessMod $sdk 'base,apt,maps' 0 3
            Need (@($businessCombined.report.plan.Tasks).Count -eq 3) 'Business plan must contain base, real APT and real map metadata tasks'
            foreach($group in @('base','apt','maps')){
                $businessSingle=RunCli ('business-'+$group+'-only') 'build' $businessMod $sdk $group 0 1
                EqualTask $businessCombined $businessSingle $group
            }
            foreach($selection in @(@{name='mod-a';group='apt';types='AptAptData,AptConstData,AptDatData,AptGeometryData'},@{name='mod-b';group='maps';types='MapMetaData'})){
                $tasks=@($businessCombined.report.plan.Tasks|Where-Object{$_.Content -ceq $selection.group})
                Need ($tasks.Count -eq 1) 'Business original oracle requires exactly one task for its selected group'
                $task=$tasks[0]
                $prefix=Join-Path $businessCombined.destination ('.build/'+$businessCombined.report.buildId+'/'+$task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.InputPath));$outputs=@{}
                foreach($artifact in $businessCombined.report.artifacts){if(Within $prefix $artifact.Path){$outputs[[IO.Path]::GetExtension($artifact.Path).Substring(1)]=$artifact.Path}}
                Need ($outputs.Count -eq 4) 'Business original oracle requires four current CLI outputs'
                $businessReceipt.baselineCases+=,@{name=$selection.name;modRoot=$businessMod;inputPath=$task.InputPath;sdkRoot=$sdk;originalToolsRoot=(Join-Path $sourceSdk 'tools');stagingDirectory=$prefix;outputs=$outputs;scope=('Actual '+$selection.types+' business task, current original BAB strict four-stream comparison; no historical output reuse or byte normalization')}
            }
            # This is a request for the independent oracle, not its success claim.
            @{succeeded=$true;sourceCommit=$state.sourceCommit;runId=$state.runId;baselineCases=$businessReceipt.baselineCases;scope=$businessReceipt.scope}|ConvertTo-Json -Depth 8|Set-Content "$modeOut/business-result.json"
            & "$PSScriptRoot/TestCompilerBackendBaseline.ps1" -FixtureResult "$modeOut/business-result.json" -OutputDirectory "$modeOut/business-original-baseline" -TimeoutSeconds $TimeoutSeconds
            Need ($LASTEXITCODE -eq 0) 'Independent original BAB real APT/MapMetaData baseline failed'
            Unchanged $businessMod $businessSnapshot
            foreach($inputFile in $prepared.sourceInputs){Need ((Hash (Join-Path $root $inputFile.path)) -ceq $inputFile.sha256) 'Business fixture source input changed'}
            $businessReceipt.inputsUnchanged=$true;$businessReceipt.coldGroupOutputsIdentical=$true;$businessReceipt.originalFourOutputsIdentical=$true;$businessReceipt.succeeded=$true
        } finally {$businessReceipt|ConvertTo-Json -Depth 12|Set-Content "$modeOut/business-validation.json"}
        Unchanged $mod $modSnapshot;Unchanged $asciiMod $asciiSnapshot;Unchanged $sdk $sdkSnapshot;Unchanged $runtime $runtimeSnapshot
        $modeState.passed=$true
    }
    Unchanged (Join-Path $sourceSdk 'Schemas/xsd') $schemaSnapshot
    Need ((Hash (Join-Path $sourceSdk 'tools/BinaryAssetBuilder.exe.config')) -ceq $configHash -and (Hash $probeFile) -ceq $probeHash) 'Original SDK inputs changed'
    $state.succeeded=$true;$state.scope='Passed real production EXE CLI Debug/Release: four tasks, single/group fresh-process output equality, two selected ASCII-companion original BAB baselines, exact Unicode/ASCII four-task output equality and DATA source semantics, SDK-internal/external Unicode MODs, explicit cache rejection, failure/retry, required-log exit7 and actual CTRL_BREAK exit6/retry; plus real file-backed APT/MapMetaData business tasks, cold group equality and current-original strict four-stream oracles; full SDK/cache/publishing remain separate'
    $global:LASTEXITCODE=0
} catch {$state.error=$_.Exception.ToString();throw}
finally {
    $env:PATH=$oldPath
    $state|ConvertTo-Json -Depth 20|Set-Content "$out/validation.json"
    try {Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "CLI temporary workspace cleanup failed: $_"}
    Pop-Location
}
