#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$FixtureResult,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180,
    [ValidateRange(1,2)][int]$ExpectedCaseCount=2
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$FixtureResult=(Resolve-Path -LiteralPath $FixtureResult).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite original backend baseline evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Inventory([string]$Directory){
    @(Get-ChildItem -LiteralPath $Directory -File -Recurse|Sort-Object FullName|ForEach-Object{
        @{path=$_.FullName.Substring($Directory.Length).TrimStart([char[]]'\/');bytes=$_.Length;sha256=(Hash $_.FullName)}
    })
}
function SameInventory([string]$Directory,$Expected){
    $actual=@(Inventory $Directory)
    if($actual.Count -ne @($Expected).Count){throw "Input file count changed: $Directory"}
    $map=@{};foreach($item in $actual){$map[$item.path]=$item}
    foreach($item in $Expected){if(!$map.ContainsKey($item.path) -or $map[$item.path].sha256 -cne $item.sha256){throw "Input changed: $Directory/$($item.path)"}}
}
function Contains([string]$Directory,[string]$Path){
    $directoryFull=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/')
    $pathFull=[IO.Path]::GetFullPath($Path)
    $pathFull.Equals($directoryFull,[StringComparison]::OrdinalIgnoreCase) -or $pathFull.StartsWith($directoryFull+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function CompareBytes([string]$Original,[string]$Backend){
    $left=[IO.File]::ReadAllBytes($Original);$right=[IO.File]::ReadAllBytes($Backend)
    if($left.Length -ne $right.Length){throw "Independent original/backend output size differs: $Original vs $Backend"}
    for($i=0;$i -lt $left.Length;$i++){if($left[$i] -ne $right[$i]){throw "Independent original/backend bytes differ at $i : $Original vs $Backend"}}
}
function InvokeOriginal([string]$Exe,[string[]]$Arguments,[string]$WorkingDirectory,[string]$LogRoot){
    $start=[Diagnostics.ProcessStartInfo]::new()
    $start.FileName=$Exe;$start.WorkingDirectory=$WorkingDirectory
    $start.UseShellExecute=$false;$start.CreateNoWindow=$true
    $start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true
    foreach($argument in $Arguments){$start.ArgumentList.Add($argument)}
    # Test instrumentation must not redirect or replace the original DLL calls.
    foreach($name in @($start.Environment.Keys)){
        if($name -like 'RA3_RECOVERED_*'){$start.Environment.Remove($name)|Out-Null}
    }
    $process=[Diagnostics.Process]::new();$process.StartInfo=$start
    $watch=[Diagnostics.Stopwatch]::StartNew();$started=$false
    try {
        $started=$process.Start()
        if(!$started){throw 'Original BAB process could not start'}
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        $finished=$process.WaitForExit($TimeoutSeconds*1000)
        if(!$finished){
            try{$process.Kill($true)}catch{}
            if(!$process.WaitForExit(10000)){throw 'Timed-out original BAB could not be terminated within 10 seconds'}
        }
        if(![Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]]@($stdout,$stderr),10000)){throw 'Original BAB redirected logs did not close within 10 seconds'}
        [IO.File]::WriteAllText((Join-Path $LogRoot 'stdout.log'),$stdout.GetAwaiter().GetResult())
        [IO.File]::WriteAllText((Join-Path $LogRoot 'stderr.log'),$stderr.GetAwaiter().GetResult())
        $watch.Stop()
        $result=@{processId=$process.Id;exitCode=$process.ExitCode;elapsedMs=$watch.ElapsedMilliseconds;timedOut=(!$finished)}
        $result|ConvertTo-Json|Set-Content (Join-Path $LogRoot 'process.json')
        if(!$finished){throw "Original BAB exceeded $TimeoutSeconds seconds"}
        if($process.ExitCode -ne 0){Get-Content (Join-Path $LogRoot 'stdout.log') -Tail 80|Out-Host;Get-Content (Join-Path $LogRoot 'stderr.log') -Tail 80|Out-Host;throw "Original BAB failed with exit code $($process.ExitCode)"}
        $result
    } finally {
        if($started -and !$process.HasExited){try{$process.Kill($true);$process.WaitForExit(5000)|Out-Null}catch{}}
        $process.Dispose()
    }
}
function ArchiveStreams([string]$Directory,[string]$CaseOut,[string]$Prefix){
    $archived=@()
    if(Test-Path -LiteralPath $Directory){
        foreach($extension in @('manifest','bin','imp','relo')){
            $files=@(Get-ChildItem -LiteralPath $Directory -File -Recurse|Where-Object{$_.Extension -ceq ".$extension"}|Sort-Object FullName)
            for($index=0;$index -lt $files.Count;$index++){
                $name=if($files.Count -eq 1){"$Prefix.$extension"}else{"$Prefix-$index.$extension"}
                $target=Join-Path $CaseOut $name
                Copy-Item -LiteralPath $files[$index].FullName -Destination $target -Force
                $archived+=,@{source=$files[$index].FullName;archive=$target;bytes=$files[$index].Length;sha256=(Hash $target)}
            }
        }
    }
    $archived
}
$temporaryRoot=$null
$state=@{schemaVersion=1;succeeded=$false;status='started';sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;fixtureResultSha256=(Hash $FixtureResult);cases=@();scope="Small independent original BAB process oracle for $ExpectedCaseCount selected fixture stream inputs; exact four-stream comparison; not full SDK, production CLI or complete G3 acceptance"}
try {
    $source=(& git -C $root rev-parse HEAD).Trim()
    if($LASTEXITCODE -ne 0){throw 'Cannot determine current source commit'}
    $fixture=Get-Content -LiteralPath $FixtureResult -Raw|ConvertFrom-Json
    if(!$fixture.succeeded -or $fixture.sourceCommit -cne $source -or $fixture.runId -cne $state.runId){throw 'Backend fixture success/source/run identity mismatch'}
    if(@($fixture.baselineCases).Count -ne $ExpectedCaseCount){throw "Expected $ExpectedCaseCount successful selected fixture stream oracle cases"}
    $state.sourceCommit=$source
    $state.fixtureScope=$fixture.scope;$state.expectedCaseCount=$ExpectedCaseCount
    $state.scriptSha256=Hash $PSCommandPath
    $state.backendSourceSha256=Hash (Join-Path $root 'recovery/src/App/Application/CoreStreamCompiler.cs')
    $state.fixtureSourceSha256=Hash (Join-Path $root 'recovery/tests/CompilerBackendLifecycle.cs')
    $temporaryRoot=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('b'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
    $state.temporaryRoot=$temporaryRoot
    $names=@{};$caseIndex=0
    foreach($case in $fixture.baselineCases){
        if($case.name -notin @('mod-a','mod-b') -or $names.ContainsKey($case.name)){throw 'Duplicate or unexpected independent baseline case'}
        $names[$case.name]=$true
        $caseOut=(New-Item -ItemType Directory -Path (Join-Path $out $case.name)).FullName
        $sdk=(Resolve-Path -LiteralPath $case.sdkRoot).Path
        $mod=(Resolve-Path -LiteralPath $case.modRoot).Path
        $input=(Resolve-Path -LiteralPath $case.inputPath).Path
        if(!(Contains $mod $input) -or (Contains $mod $caseOut) -or (Contains $sdk $caseOut)){throw 'Baseline source/output containment is invalid'}
        $tools=(Resolve-Path -LiteralPath (Join-Path $sdk 'tools')).Path
        if($case.originalToolsRoot){
            $requestedTools=(Resolve-Path -LiteralPath ([string]$case.originalToolsRoot)).Path
            $repositoryTools=(Resolve-Path -LiteralPath (Join-Path $root 'SDK/tools')).Path
            if(![IO.Path]::GetFullPath($requestedTools).TrimEnd([char[]]'\/').Equals([IO.Path]::GetFullPath($repositoryTools).TrimEnd([char[]]'\/'),[StringComparison]::OrdinalIgnoreCase)){
                throw 'Only the repository original SDK/tools may override the independent oracle tool source'
            }
            $tools=$requestedTools
        }
        $schemas=(Resolve-Path -LiteralPath (Join-Path $sdk 'Schemas/xsd')).Path
        $inputInventory=@(Inventory $mod);$toolInventory=@(Inventory $tools);$schemaInventory=@(Inventory $schemas)
        $caseState=@{name=$case.name;scope=$case.scope;passed=$false;inputPath=$input;modRoot=$mod;sdkRoot=$sdk;originalToolsRoot=$tools;inputs=$inputInventory;originalTools=$toolInventory;schemaInputs=$schemaInventory;outputs=@()}
        $state.cases+=,$caseState
        # CLR2/Framework mixed-mode tools still use MAX_PATH internally. Keep all
        # writable native build paths short; retain the exact source MOD paths,
        # including Unicode/spaces, and archive only small outputs and receipts.
        $caseTemp=(New-Item -ItemType Directory -Path (Join-Path $temporaryRoot ([string]$caseIndex))).FullName
        $caseIndex++
        $caseState.temporaryRoot=$caseTemp
        $oracle=(New-Item -ItemType Directory -Path (Join-Path $caseTemp 't')).FullName
        $streams=Join-Path $caseTemp 's';$intermediate=Join-Path $caseTemp 'i';$hashes=Join-Path $caseTemp 'h';$cache=Join-Path $caseTemp 'c'
        try {
            Copy-Item -Path (Join-Path $tools '*') -Destination $oracle -Recurse
            $exe=Join-Path $oracle 'BinaryAssetBuilder.exe';$configPath="$exe.config"
            if((Hash $exe) -cne (Hash (Join-Path $tools 'BinaryAssetBuilder.exe'))){throw 'Original BAB copy differs from fixed SDK input'}
            # Effective SDK settings come from the actual case SDK, even when
            # immutable original EXE/DLL bytes come from repository SDK/tools.
            $caseConfig=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
            $caseState.sdkConfigSha256=Hash $caseConfig
            [xml]$configuration=[IO.File]::ReadAllText($caseConfig)
            $section=$configuration.SelectSingleNode('/configuration/assetbuilder')
            if(!$section){throw 'Original BAB assetbuilder configuration missing'}
            $data=(Join-Path $mod 'Data')+';'+$section.GetAttribute('defaultDataPaths')
            $art=(Join-Path $mod 'Art')+';'+$section.GetAttribute('defaultArtPaths')
            $audio=(Join-Path $mod 'Audio')+';'+$section.GetAttribute('defaultAudioPaths')
            $schema=Join-Path $schemas 'CnC3Types.xsd'
            $section.SetAttribute('schema',$schema);$section.SetAttribute('dataRoot',$sdk)
            $section.SetAttribute('defaultDataPaths',$data);$section.SetAttribute('defaultArtPaths',$art);$section.SetAttribute('defaultAudioPaths',$audio)
            $buildConfigurations=$section.SelectSingleNode('buildConfigurations')
            if($buildConfigurations){$buildConfigurations.RemoveAll()}
            $handlers=@($section.SelectNodes('plugins/plugin')|Where-Object{$_.GetAttribute('assetTypes').Split(',') -contains 'ArmorTemplate'})
            $defaults=@($section.SelectNodes('plugins/plugin')|Where-Object{$_.GetAttribute('assetTypes') -ceq '#all'})
            if($handlers.Count -ne 1 -or $handlers[0].GetAttribute('targetType') -cne 'BinaryAssetBuilder.Tokenizer.Plugin, BinaryAssetBuilder.Tokenizer' -or
                $defaults.Count -ne 1 -or $defaults[0].GetAttribute('targetType') -cne 'BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler'){
                throw 'Independent oracle does not use the original Tokenizer/XML default routes'
            }
            $startup=$configuration.SelectSingleNode('/configuration/startup')
            if(!$startup){$startup=$configuration.CreateElement('startup');$configuration.configuration.AppendChild($startup)|Out-Null}
            $startup.RemoveAll();$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
            $runtime=$configuration.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0');$startup.AppendChild($runtime)|Out-Null
            $configuration.Save($configPath)
            Copy-Item -LiteralPath $configPath -Destination (Join-Path $caseOut 'original-effective.config')
            New-Item -ItemType Directory -Path $streams,$intermediate,$hashes,$cache|Out-Null
            # Match CoreStreamCompiler.StreamSettings. No resident server, precompiled
            # inputs, persistent caches, reports, GUI, postprocessing or extra streams.
            $arguments=@($input,"/sp:$schema","/dr:$sdk","/od:$streams","/iod:$intermediate","/scd:$hashes","/bcp:$cache",
                '/tp:Win32','/sc:false','/bc:false','/psc:false','/fsc:false','/csc:false','/atc:false','/sh:false','/pc:false',
                '/res:false','/gui:false','/poe:false','/mr:false','/oof:false','/sf:false','/ls:true','/ss:false','/oix:false','/vf:false',
                '/osh:false','/oar:false','/slowclean:false','/tl:3','/el:0',"/art:$art","/audio:$audio","/data:$data")
            $caseState.arguments=$arguments;$caseState.workingDirectory=$sdk
            @{exe=$exe;arguments=$arguments;workingDirectory=$sdk}|ConvertTo-Json -Depth 4|Set-Content (Join-Path $caseOut 'invocation.json')
            $caseState.originalExeSha256=Hash $exe;$caseState.originalConfigSha256=Hash (Join-Path $tools 'BinaryAssetBuilder.exe.config')
            $caseState.effectiveConfigSha256=Hash $configPath
            $caseState.runtimeInputs=@(Inventory $oracle)
            $caseState.effectiveSettings=@{schema=$schema;dataRoot=$sdk;defaultDataPaths=$data;defaultArtPaths=$art;defaultAudioPaths=$audio;targetPlatform='Win32';sessionCache=$false;buildCache=$false;precompiled=$false;resident=$false;linked=$true;stableSort=$false;outputStringHashes=$false;outputAssetReport=$false;scope='Same normalized Core settings for this selected fixture stream; original built-in plugin routes retained'}
            $caseState.process=InvokeOriginal $exe $arguments $sdk $caseOut
            $actual=@(Get-ChildItem -LiteralPath $streams -File -Recurse)
            if($actual.Count -ne 4){throw 'Original BAB did not emit exactly the four selected linked stream files'}
            foreach($extension in @('manifest','bin','imp','relo')){
                $original=@($actual|Where-Object{$_.Extension -ceq ".$extension"})
                if($original.Count -ne 1){throw "Original BAB emitted ambiguous/missing .$extension stream"}
                $backend=[string]$case.outputs.$extension
                if(!$backend -or !(Test-Path -LiteralPath $backend) -or !(Contains $case.stagingDirectory $backend)){throw "Backend oracle output missing/outside staging: $extension"}
                $originalArchive=Join-Path $caseOut ("original.$extension")
                $backendArchive=Join-Path $caseOut ("backend.$extension")
                Copy-Item -LiteralPath $original[0].FullName -Destination $originalArchive
                Copy-Item -LiteralPath $backend -Destination $backendArchive
                $pair=@{extension=$extension;original=$original[0].FullName;backend=$backend;originalArchive=$originalArchive;backendArchive=$backendArchive;bytes=$original[0].Length;originalSha256=(Hash $originalArchive);backendSha256=(Hash $backendArchive);byteIdentical=$false}
                $caseState.outputs+=,$pair
                CompareBytes $originalArchive $backendArchive
                $pair.byteIdentical=$true
            }
            $caseState.passed=$true
        } finally {
            # Preserve small final/partial output evidence even when BAB or the
            # comparison failed; original EXE/DLL bytes remain reproducible from
            # the recorded Git inputs and are never copied into the artifact.
            $caseState.archivedOriginalOutputs=@(ArchiveStreams $streams $caseOut 'original')
            $caseState.archivedBackendOutputs=@()
            foreach($extension in @('manifest','bin','imp','relo')){
                $backend=[string]$case.outputs.$extension
                if($backend -and (Test-Path -LiteralPath $backend) -and (Contains $case.stagingDirectory $backend)){
                    $target=Join-Path $caseOut ("backend.$extension")
                    Copy-Item -LiteralPath $backend -Destination $target -Force
                    $caseState.archivedBackendOutputs+=,@{source=$backend;archive=$target;bytes=(Get-Item -LiteralPath $backend).Length;sha256=(Hash $target)}
                }
            }
            foreach($log in @('stdout','stderr')){
                $logPath=Join-Path $caseOut ("$log.log")
                if(Test-Path -LiteralPath $logPath){$caseState[$log+'Sha256']=Hash $logPath}
            }
            try {
                SameInventory $mod $inputInventory;SameInventory $tools $toolInventory;SameInventory $schemas $schemaInventory
                if($caseState.ContainsKey('runtimeInputs')){SameInventory $oracle $caseState.runtimeInputs}
                $caseState.originalInputsUnchanged=$true
            } catch {
                $caseState.originalInputsUnchanged=$false;$caseState.inputVerificationError=$_.Exception.ToString()
                $wasPassed=$caseState.passed;$caseState.passed=$false
                if($wasPassed){throw}
            }
        }
        if($caseState.passed){Write-Host "PASS IndependentBackendBaseline $($case.name): original BAB process, manifest/bin/imp/relo byte-identical"}
    }
    if((Hash $FixtureResult) -cne $state.fixtureResultSha256){throw 'Backend fixture result changed during independent baseline'}
    $state.succeeded=$true;$state.status="passed $ExpectedCaseCount original BAB independent-process selected fixture baselines; all four files byte-identical in each case; full SDK/G3/production CLI remain separate"
    $global:LASTEXITCODE=0
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {
    if($temporaryRoot -and (Test-Path -LiteralPath $temporaryRoot)){
        try {Remove-Item -LiteralPath $temporaryRoot -Recurse -Force;$state.temporaryRootRemoved=$true}
        catch {$state.temporaryRootRemoved=$false;$state.temporaryCleanupError=$_.Exception.ToString()}
    }
    $state|ConvertTo-Json -Depth 14|Set-Content (Join-Path $out 'validation.json')
}
