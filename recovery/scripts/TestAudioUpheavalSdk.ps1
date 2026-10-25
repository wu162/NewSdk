param([Parameter(Mandatory=$true)][string]$ModSourcePath,[int]$TimeoutSeconds=1200)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path; Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP=[IO.Path]::GetTempPath() }
if (!$env:GITHUB_RUN_ID -or !$env:GITHUB_RUN_ATTEMPT) { throw 'Upheaval milestone requires an identified Actions run' }
$source=(Resolve-Path $ModSourcePath).Path
$expectedModCommit='068643219fded09a05fb7b75c6507b769c430316'
$actualModCommit=(& git -C $source rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $actualModCommit -ne $expectedModCommit) { throw 'Upheaval source is not the required pinned checkout' }
$sourceChanges=@(& git -C $source status --porcelain --untracked-files=all -- .)
if ($LASTEXITCODE -ne 0 -or $sourceChanges.Count -ne 0) { throw 'Pinned Upheaval source has local modifications or untracked inputs' }
$out="recovery/evidence/audio/upheaval-sdk-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if (Test-Path $out) { throw 'Refusing to overwrite Upheaval SDK evidence' }
New-Item -ItemType Directory $out | Out-Null; $out=(Resolve-Path $out).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-upheaval-'+[Guid]::NewGuid().ToString('N'))
$sdk=Join-Path $work 'SDK'; $build=Join-Path $work 'build'
New-Item -ItemType Directory $sdk,$build -Force | Out-Null
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';succeeded=$false;sdkStarted=$false;originalBaselinePassed=$false;modSourceCommit=$actualModCommit;expectedModSourceCommit=$expectedModCommit;scope='Complete pinned Upheaval/PublicCases input; original BAB case3/4/5; only AudioCompiler replaced; other compiler plugins use SDK baseline';fullCombinedRecoveredCompilerValidated=$false;finalExeIntegrated=$false;invocations=@();outputs=@();runs=@()}
$savedSide=$env:AUDIO_SDK_IMPLEMENTATION; $savedTrace=$env:AUDIO_SDK_TRACE
try {
    Get-ChildItem SDK -Force | Where-Object Name -notin @('Mods','builtmods') | Copy-Item -Destination $sdk -Recurse
    New-Item -ItemType Directory "$sdk/Mods" | Out-Null
    Copy-Item $source "$sdk/Mods/Upheaval" -Recurse
    if (!(Test-Path "$sdk/Mods/Upheaval/Data/Mod/PublicCases/manifest.json")) { throw 'Pinned complete Upheaval/PublicCases input absent' }
    $state.inputFiles=@(Get-ChildItem "$sdk/Mods/Upheaval" -Recurse -File).Count
    $modInputs=@(Get-ChildItem "$sdk/Mods/Upheaval" -Recurse -File | Sort-Object FullName | ForEach-Object {
        @{path=$_.FullName.Substring($sdk.Length+1).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant();bytes=$_.Length}
    })
    $modInputs | ConvertTo-Json -Depth 5 | Set-Content "$out/inputs.json"
    $state.pinnedAssetInputFiles=@($modInputs | Where-Object { $_.path.StartsWith('Mods/Upheaval/Assets/',[StringComparison]::OrdinalIgnoreCase) -and $_.path.EndsWith('.asset',[StringComparison]::OrdinalIgnoreCase) }).Count
    if ($state.pinnedAssetInputFiles -ne 194) { throw 'Pinned complete Upheaval must retain its194 precompiled asset inputs' }
    $originalHash=(Get-FileHash "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll").Hash.ToLowerInvariant()
    if ($originalHash -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139') { throw 'Original AudioCompiler fingerprint changed' }
    $state.originalSha256=$originalHash
    $state.defaultScriptSha256=(Get-FileHash SDK/defaultscript.cs).Hash.ToLowerInvariant()
    Copy-Item SDK/defaultscript.cs "$out/defaultscript.cs"
    # These thirteen declarations are unconditionally reachable through
    # Mod.xml -> Mod/Specific.xml -> Specific/AudioFile.xml in this pinned MOD.
    [xml]$specificAudio=Get-Content "$sdk/Mods/Upheaval/Data/Mod/Specific/AudioFile.xml" -Raw
    $knownAudioIds=@($specificAudio.SelectNodes("/*[local-name()='AssetDeclaration']/*[local-name()='AudioFile']") | ForEach-Object { $_.GetAttribute('id') })
    if ($knownAudioIds.Count -ne 13 -or $knownAudioIds -notcontains 'JUPearl_ambiLoopa') { throw 'Pinned Upheaval reachable audio declarations changed' }
    $state.expectedSpecificAudioIds=$knownAudioIds
    dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release -o $build -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/library-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/library-build.log" -Tail 60; throw 'Recovered audio build failed' }
    Copy-Item "$build/Recovered.AudioCompiler.dll" "$sdk/tools"
    $state.recoveredSha256=(Get-FileHash "$sdk/tools/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant()
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    & $csc /nologo /target:library /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$sdk/tools/BinaryAssetBuilder.Core.dll" /out:"$sdk/tools/AudioSdkProbe.dll" (Resolve-Path recovery/tests/AudioSdkTracingPlugin.cs).Path *> "$out/tracer-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/tracer-build.log"; throw 'Audio tracing adapter build failed' }
    $cfg="$sdk/tools/BinaryAssetBuilder.exe.config"; [xml]$config=Get-Content $cfg -Raw
    $routes=@($config.configuration.assetbuilder.plugins.plugin | Where-Object { $_.assetTypes.Split(',') -contains 'AudioFile' })
    if ($routes.Count -ne 1 -or $routes[0].assetTypes -ne 'AudioFile,AudioFileMP3Passthrough,PathMusicMap,PathMusicTrack,PathMusicEvent') { throw 'Unexpected audio route' }
    $routes[0].SetAttribute('targetType','AudioSdkTracingPlugin, AudioSdkProbe'); $routes[0].SetAttribute('enabled','true'); $routes[0].SetAttribute('useBuildCache','false')
    if ($config.configuration.startup) { $config.configuration.RemoveChild($config.configuration.startup) | Out-Null }
    $startup=$config.CreateElement('startup'); $startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
    $runtime=$config.CreateElement('supportedRuntime'); $runtime.SetAttribute('version','v4.0'); $startup.AppendChild($runtime) | Out-Null
    $config.configuration.AppendChild($startup) | Out-Null; $config.Save($cfg); Copy-Item $cfg "$out/sdk.config"
    $unchangedDlls=@(Get-ChildItem "$sdk/tools" -File -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | Sort-Object Name | ForEach-Object {
        @{name=$_.Name;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}
    })
    $unchangedDlls | ConvertTo-Json -Depth 4 | Set-Content "$out/unchanged-sdk-dlls.json"
    $state.onlyAudioCompilerReplaced=$true
    $targets=@(@{step=3;folder='AptUI';filter='*.xml'},@{step=4;folder='AdditionalMaps';filter='mapmetadata_*.xml'},@{step=5;folder='';filter='Mod.xml'})
    foreach ($side in @('Original','Recovered')) {
        if ($side -eq 'Recovered') {
            if (!$state.originalBaselinePassed) { throw 'Original baseline gate not satisfied' }
            Remove-Item "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll" -Force
            if (Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll") { throw 'Original target remains in recovered SDK' }
            # Restore the same complete pinned input at the same absolute path;
            # generated source-local files from the original run cannot leak.
            Remove-Item "$sdk/Mods/Upheaval" -Recurse -Force
            Copy-Item $source "$sdk/Mods/Upheaval" -Recurse
        }
        if (@(Get-ChildItem "$sdk/Mods/Upheaval" -Recurse -File).Count -ne $modInputs.Count) { throw 'Pinned MOD input file count changed' }
        foreach ($modInput in $modInputs) {
            $inputPath=Join-Path $sdk $modInput.path
            if (!(Test-Path $inputPath) -or (Get-FileHash $inputPath).Hash.ToLowerInvariant() -ne $modInput.sha256) { throw "Pinned MOD input changed: $($modInput.path)" }
        }
        foreach ($dll in $unchangedDlls) {
            if (!(Test-Path "$sdk/tools/$($dll.name)") -or (Get-FileHash "$sdk/tools/$($dll.name)").Hash.ToLowerInvariant() -ne $dll.sha256) { throw "Unrelated SDK DLL changed: $($dll.name)" }
        }
        Remove-Item "$sdk/builtmods" -Recurse -Force -ErrorAction SilentlyContinue
        $modPrefix=[IO.Path]::GetFullPath("$sdk/Mods/Upheaval")+[IO.Path]::DirectorySeparatorChar
        Get-ChildItem $sdk -Recurse -Directory | Where-Object { $_.Name -in @('cache','buildcache','sessioncache') -and !$_.FullName.StartsWith($modPrefix,[StringComparison]::OrdinalIgnoreCase) } | Remove-Item -Recurse -Force
        # Studio case2 removes generated caches, not versioned Mods/Upheaval/Assets.
        Get-ChildItem $sdk -File | Where-Object { $_.Name -like 'binaryassetbuilder.sessioncache.xml*' -or $_.Name -eq 'stringhashes.xml' } | Remove-Item -Force
        $env:AUDIO_SDK_IMPLEMENTATION=$side; $env:AUDIO_SDK_TRACE="$out/$side-calls.tsv"
        $built="$sdk/builtmods"
        foreach ($target in $targets) {
            $files=@(Get-ChildItem "$sdk/Mods/Upheaval/Data/$($target.folder)" -Filter $target.filter -File | Sort-Object Name)
            if (!$files.Count) { throw "Missing Upheaval step $($target.step) inputs" }
            foreach ($file in $files) {
                $label="$side-$($target.step)-$($file.BaseName)"
                $arguments=@(('"'+$file.FullName+'"'),('/od:"'+$built+'"'),('/iod:"'+$built+'"'),'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\Upheaval\Art;.\Mods;.\Art"','/audio:".;.\Mods\Upheaval\Audio;.\Mods;.\Audio"','/data:".;.\Mods\Upheaval\Data;.\Mods;.\SageXml"')
                @{mode=$side;step=$target.step;input=$file.Name;executable="$sdk/tools/BinaryAssetBuilder.exe";arguments=$arguments;originalTargetPresent=(Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll");source='SDK/defaultscript.cs case3/4/5'} | ConvertTo-Json -Depth 6 | Set-Content "$out/$label-command.json"
                $state.sdkStarted=$true
                $p=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$label-stdout.log" -RedirectStandardError "$out/$label-stderr.log"
                if (!$p.WaitForExit($TimeoutSeconds*1000)) { Stop-Process -Id $p.Id -Force; throw "$label timed out" }; $p.WaitForExit(); $p.Refresh()
                $log=(Get-Content "$out/$label-stdout.log","$out/$label-stderr.log" -Raw -ErrorAction SilentlyContinue) -join [Environment]::NewLine
                $row=@{mode=$side;step=$target.step;input=$file.Name;exitCode=$p.ExitCode;succeeded=($p.ExitCode -eq 0 -and $log -notmatch '(?im)^\s*(Critical|Error):')}
                $state.invocations+=$row
                if (!$row.succeeded) { Get-Content "$out/$label-stdout.log" -Tail 50; Get-Content "$out/$label-stderr.log"; throw "$label failed" }
                foreach ($ext in @('bin','manifest','imp','relo')) {
                    $path="$built/Mods/Upheaval/Data/$($target.folder)/$($file.BaseName).$ext"
                    if (!(Test-Path $path)) { throw "$label missing $ext" }
                    Copy-Item $path "$out/$label.$ext"
                    $state.outputs+=@{mode=$side;step=$target.step;input=$file.Name;extension=$ext;sha256=(Get-FileHash $path).Hash.ToLowerInvariant();bytes=(Get-Item $path).Length}
                }
                if ($target.step -eq 3) { [IO.File]::WriteAllText("$built/Mods/Upheaval/Data/AptUI/$($file.BaseName).version",[string][char]13+[char]10) }
                Write-Host "PASS Upheaval $label"
            }
        }
        $custom=@(Get-ChildItem $built -Recurse -Filter '*.cdata' | Sort-Object FullName | ForEach-Object { "$($_.FullName.Substring($built.Length+1))`t$((Get-FileHash $_.FullName).Hash.ToLowerInvariant())" })
        [IO.File]::WriteAllLines("$out/$side-custom.tsv",[string[]]$custom)
        if (!(Test-Path "$out/$side-calls.tsv")) { [IO.File]::WriteAllText("$out/$side-calls.tsv",'') }
        $state.runs+=@{mode=$side;audioCalls=@(Get-Content "$out/$side-calls.tsv").Count;customFiles=$custom.Count;originalTargetPresent=(Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll")}
        if ($side -eq 'Original') { $state.originalBaselinePassed=$true }
    }
    $state.comparisons=@()
    foreach ($expected in @($state.outputs | Where-Object mode -eq Original)) {
        $actual=@($state.outputs | Where-Object { $_.mode -eq 'Recovered' -and $_.step -eq $expected.step -and $_.input -eq $expected.input -and $_.extension -eq $expected.extension })
        $equal=($actual.Count -eq 1 -and $actual[0].sha256 -eq $expected.sha256)
        $state.comparisons+=@{step=$expected.step;input=$expected.input;extension=$expected.extension;identical=$equal}
        if (!$equal) { throw "Upheaval product differs: $($expected.step)/$($expected.input).$($expected.extension)" }
    }
    if ((Get-FileHash "$out/Original-custom.tsv").Hash -ne (Get-FileHash "$out/Recovered-custom.tsv").Hash) { throw 'Upheaval custom data differs' }
    $originalCalls=@(Get-Content "$out/Original-calls.tsv" | Sort-Object)
    $recoveredCalls=@(Get-Content "$out/Recovered-calls.tsv" | Sort-Object)
    if ($originalCalls.Count -ne $recoveredCalls.Count) { throw 'Upheaval actual audio handler count differs' }
    for ($i=0; $i -lt $originalCalls.Count; ++$i) { if ($originalCalls[$i] -cne $recoveredCalls[$i]) { throw "Upheaval actual audio buffers/custom data differ at call $i" } }
    if ($state.invocations.Count -ne 10 -or $state.comparisons.Count -ne 20) { throw 'Pinned complete Upheaval case3/4/5 coverage changed' }
    $state.audioPluginExercised=($originalCalls.Count -gt 0)
    $state.audioCalls=$originalCalls.Count
    $observedAudioKeys=@{}
    foreach ($record in $originalCalls) {
        $fields=$record -split "`t"
        if ($fields.Count -ne 6 -or $fields[0] -notin @('AudioFile','AudioFileMP3Passthrough','PathMusicMap','PathMusicTrack','PathMusicEvent') -or !$fields[1]) { throw 'Malformed actual Upheaval audio trace' }
        $observedAudioKeys[$fields[0]+"`t"+$fields[1]]=$true
        foreach ($field in $fields[2..5]) {
            if ($field -ne '<null>' -and $field -ne '<absent>') { [void][Convert]::FromBase64String($field) }
        }
    }
    $state.specificAudioCoverage=@()
    foreach ($id in $knownAudioIds) {
        $called=$observedAudioKeys.ContainsKey("AudioFile`t"+$id)
        $state.specificAudioCoverage+=@{type='AudioFile';id=$id;actualHandlerObserved=$called}
        if (!$called) { throw "Pinned reachable AudioFile never reached actual audio handler: $id; audio replacement is not validated" }
    }
    if (!$state.audioPluginExercised) { throw 'audioCalls=0: full MOD output compatibility cannot establish actual audio plugin coverage' }
    $state.succeeded=$true; $state.status='passed'
    Write-Host "PASS complete pinned Upheaval audio replacement:10 BAB invocations,20 identical products,custom data identical,audio calls=$($originalCalls.Count)"
} catch { $state.status='failed'; $state.error=$_.Exception.ToString(); throw }
finally { $env:AUDIO_SDK_IMPLEMENTATION=$savedSide; $env:AUDIO_SDK_TRACE=$savedTrace; $state | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json" }
