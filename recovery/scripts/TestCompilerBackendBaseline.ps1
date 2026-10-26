#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$FixtureResult,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180
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
$state=@{schemaVersion=1;succeeded=$false;status='started';sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;fixtureResultSha256=(Hash $FixtureResult);cases=@();scope='Small independent original BAB process oracle for two Armor MOD inputs; exact four-stream comparison; not full SDK, production CLI or complete G3 acceptance'}
try {
    $source=(& git -C $root rev-parse HEAD).Trim()
    if($LASTEXITCODE -ne 0){throw 'Cannot determine current source commit'}
    $fixture=Get-Content -LiteralPath $FixtureResult -Raw|ConvertFrom-Json
    if(!$fixture.succeeded -or $fixture.sourceCommit -cne $source -or $fixture.runId -cne $state.runId){throw 'Backend fixture success/source/run identity mismatch'}
    if(@($fixture.baselineCases).Count -ne 2){throw 'Expected the two successful MOD A/B backend oracle cases'}
    $state.sourceCommit=$source
    $state.scriptSha256=Hash $PSCommandPath
    $state.backendSourceSha256=Hash (Join-Path $root 'recovery/src/App/Application/CoreStreamCompiler.cs')
    $state.fixtureSourceSha256=Hash (Join-Path $root 'recovery/tests/CompilerBackendLifecycle.cs')
    $names=@{}
    foreach($case in $fixture.baselineCases){
        if($case.name -notin @('mod-a','mod-b') -or $names.ContainsKey($case.name)){throw 'Duplicate or unexpected independent baseline case'}
        $names[$case.name]=$true
        $caseOut=(New-Item -ItemType Directory -Path (Join-Path $out $case.name)).FullName
        $sdk=(Resolve-Path -LiteralPath $case.sdkRoot).Path
        $mod=(Resolve-Path -LiteralPath $case.modRoot).Path
        $input=(Resolve-Path -LiteralPath $case.inputPath).Path
        if(!(Contains $mod $input) -or (Contains $mod $caseOut) -or (Contains $sdk $caseOut)){throw 'Baseline source/output containment is invalid'}
        $tools=(Resolve-Path -LiteralPath (Join-Path $sdk 'tools')).Path
        $schemas=(Resolve-Path -LiteralPath (Join-Path $sdk 'Schemas/xsd')).Path
        $inputInventory=@(Inventory $mod);$toolInventory=@(Inventory $tools);$schemaInventory=@(Inventory $schemas)
        $caseState=@{name=$case.name;passed=$false;inputPath=$input;modRoot=$mod;sdkRoot=$sdk;inputs=$inputInventory;originalTools=$toolInventory;schemaInputs=$schemaInventory;outputs=@()}
        $state.cases+=,$caseState
        # Copy only original tools. Read the same original schema and exact same
        # fixture MOD paths as the backend; no full SDK/MOD baseline is rerun.
        $oracle=(New-Item -ItemType Directory -Path (Join-Path $caseOut 'original')).FullName
        Copy-Item -Path (Join-Path $tools '*') -Destination $oracle -Recurse
        $exe=Join-Path $oracle 'BinaryAssetBuilder.exe';$configPath="$exe.config"
        if((Hash $exe) -cne (Hash (Join-Path $tools 'BinaryAssetBuilder.exe'))){throw 'Original BAB copy differs from fixed SDK input'}
        [xml]$configuration=[IO.File]::ReadAllText($configPath)
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
        $streams=Join-Path $caseOut 'streams';$intermediate=Join-Path $caseOut 'intermediate';$hashes=Join-Path $caseOut 'hashes';$cache=Join-Path $caseOut 'cache'
        New-Item -ItemType Directory -Path $streams,$intermediate,$hashes,$cache|Out-Null
        # Match CoreStreamCompiler.StreamSettings. No resident server, precompiled
        # inputs, persistent caches, reports, GUI, postprocessing or extra streams.
        $arguments=@($input,"/sp:$schema","/dr:$sdk","/od:$streams","/iod:$intermediate","/scd:$hashes","/bcp:$cache",
            '/tp:Win32','/sc:false','/bc:false','/psc:false','/fsc:false','/csc:false','/atc:false','/sh:false','/pc:false',
            '/res:false','/gui:false','/poe:false','/mr:false','/oof:false','/sf:false','/ls:true','/ss:false','/oix:false','/vf:false',
            '/osh:false','/oar:false','/slowclean:false','/tl:3','/el:0',"/art:$art","/audio:$audio","/data:$data")
        $caseState.arguments=$arguments;$caseState.workingDirectory=$sdk
        $caseState.originalExeSha256=Hash $exe;$caseState.originalConfigSha256=Hash (Join-Path $tools 'BinaryAssetBuilder.exe.config')
        $caseState.effectiveConfigSha256=Hash $configPath
        $caseState.runtimeInputs=@(Inventory $oracle)
        $caseState.effectiveSettings=@{schema=$schema;dataRoot=$sdk;defaultDataPaths=$data;defaultArtPaths=$art;defaultAudioPaths=$audio;targetPlatform='Win32';sessionCache=$false;buildCache=$false;precompiled=$false;resident=$false;linked=$true;stableSort=$false;outputStringHashes=$false;outputAssetReport=$false;scope='Same normalized Core settings for this selected Armor stream; original built-in plugin routes retained'}
        $caseState.process=InvokeOriginal $exe $arguments $sdk $caseOut
        $actual=@(Get-ChildItem -LiteralPath $streams -File -Recurse)
        if($actual.Count -ne 4){throw 'Original BAB did not emit exactly the four selected linked stream files'}
        foreach($extension in @('manifest','bin','imp','relo')){
            $original=@($actual|Where-Object{$_.Extension -ceq ".$extension"})
            if($original.Count -ne 1){throw "Original BAB emitted ambiguous/missing .$extension stream"}
            $backend=[string]$case.outputs.$extension
            if(!$backend -or !(Test-Path -LiteralPath $backend) -or !(Contains $case.stagingDirectory $backend)){throw "Backend oracle output missing/outside staging: $extension"}
            CompareBytes $original[0].FullName $backend
            $caseState.outputs+=,@{extension=$extension;original=$original[0].FullName;backend=$backend;bytes=$original[0].Length;originalSha256=(Hash $original[0].FullName);backendSha256=(Hash $backend);byteIdentical=$true}
        }
        SameInventory $mod $inputInventory;SameInventory $tools $toolInventory;SameInventory $schemas $schemaInventory
        SameInventory $oracle $caseState.runtimeInputs
        $caseState.stdoutSha256=Hash (Join-Path $caseOut 'stdout.log');$caseState.stderrSha256=Hash (Join-Path $caseOut 'stderr.log')
        $caseState.passed=$true
        Write-Host "PASS IndependentBackendBaseline $($case.name): original BAB process, manifest/bin/imp/relo byte-identical"
    }
    if((Hash $FixtureResult) -cne $state.fixtureResultSha256){throw 'Backend fixture result changed during independent baseline'}
    $state.succeeded=$true;$state.status='passed two original BAB independent-process Armor baselines; all four files byte-identical in each case; full SDK/G3/production CLI remain separate'
    $global:LASTEXITCODE=0
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 14|Set-Content (Join-Path $out 'validation.json')}
