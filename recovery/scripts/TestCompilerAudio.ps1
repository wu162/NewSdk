param([Parameter(Mandatory=$true)][string]$CompilerDirectory,[Parameter(Mandatory=$true)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite Audio product evidence $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path ([IO.Path]::GetTempPath()) ('compiler-audio-'+[Guid]::NewGuid().ToString('N'))
$oracle=Join-Path $work 'original';$fixtures=Join-Path $work 'fixtures'
New-Item -ItemType Directory -Force $oracle,$fixtures|Out-Null
$oldPath=$env:PATH;$oldRealDirectory=$env:AUDIO_TEST_REAL_DIRECTORY
# Full existing AudioPlugin differential groups. This is not all Audio subsystem
# acceptance: specialized standalone kernel/reader suites retain their evidence.
$groups=@('business','pcm','xas','defaults','rates','main','pcm8','formats','loops','aiffloops','resample-full','multichannel','multichannel-aiff')
$pluginSources=@('AudioPluginOracle.cs','AudioMp3Fixtures.cs','AudioInputFormatCases.cs','AudioLoopProbeCases.cs','AudioAiffLoopCases.cs','AudioUnrecoveredProbeCases.cs','AudioResampleCases.cs','AudioMultichannelCases.cs','AudioMultichannelAiffCases.cs','AudioAiffMarkerRangeCases.cs','AudioAiffOffsetCases.cs','AudioAifcInputCases.cs','AudioAdp4InputCases.cs')
$boundarySources=@{
    AudioExcludedPlatformOracle=@('AudioExcludedPlatformOracle.cs')
    AudioInputReaderOracle=@('AudioInputReaderOracle.cs','AudioInputFormatCases.cs')
    AudioLoopReaderOracle=@('AudioLoopReaderOracle.cs','AudioLoopProbeCases.cs')
    AudioAiffLoopReaderOracle=@('AudioAiffLoopReaderOracle.cs','AudioAiffLoopCases.cs','AudioInputFormatCases.cs')
}
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Full existing AudioPlugin PC differential groups and existing input/loop/excluded-platform contracts in the linked production combination; not SDK/MOD or all standalone Audio kernel suites';groups=$groups;configurations=@();payloadRoutes=@('AudioFile','AudioFileMP3Passthrough','PathMusicMap','PathMusicTrack','PathMusicEvent');sdkValidated=$false}
function Build-AudioFixture([string]$Directory,[string]$Name,[string[]]$Sources,[string]$Core,[switch]$Product){
    $files=@($Sources|ForEach-Object{(Resolve-Path "recovery/tests/$_").Path})
    $flags=@('/nologo','/unsafe','/codepage:65001','/platform:x86','/r:System.Core.dll','/r:System.Xml.dll',"/r:$Core","/out:$Directory/$Name.exe")
    if($Product){$flags+='/define:AUDIO_PRODUCT'}
    & $script:csc @flags @files *> "$out/$Name-$(Split-Path $Directory -Leaf)-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$out/$Name-$(Split-Path $Directory -Leaf)-build.log" -Tail 60|Out-Host;throw "Audio fixture build failed: $Name"}
    $script:config|Set-Content "$Directory/$Name.exe.config"
}
function Invoke-AudioFixture([string]$Executable,[string[]]$Arguments,[string]$Directory,[string]$Log){
    Push-Location $Directory
    try{& $Executable @Arguments *> $Log;$code=$LASTEXITCODE}finally{Pop-Location}
    if($code -ne 0){Get-Content $Log -Tail 60|Out-Host;throw "Audio fixture failed: $Executable (exit $code)"}
}
function Compare-AudioTranscript([string]$Expected,[string]$Actual,[string]$Label){
    $expectedRows=@(Get-Content $Expected);$actualRows=@(Get-Content $Actual)
    if($expectedRows.Count -eq 0 -or $expectedRows.Count -ne $actualRows.Count -or
       (Get-FileHash $Expected -Algorithm SHA256).Hash -cne (Get-FileHash $Actual -Algorithm SHA256).Hash){
        Compare-Object $expectedRows $actualRows|Out-String -Width 2000|Set-Content "$out/$Label-difference.txt"
        throw "Audio transcript differs: $Label"
    }
    return $actualRows.Count
}
Push-Location $root
try{
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    # Keep the original plugin fixture source manifest in sync without silently
    # omitting a fixture when the established standalone matrix changes.
    $existingScript=Get-Content "$PSScriptRoot/TestAudioPlugin.ps1" -Raw
    $existingSources=@([regex]::Matches(($existingScript -split "`n"|Where-Object{$_ -match '/out:\"\$hostExe\"'}),"Resolve-Path 'recovery/tests/([^']+\.cs)'")|ForEach-Object{$_.Groups[1].Value})
    if(($existingSources -join "`n") -cne ($pluginSources -join "`n")){throw 'AudioPlugin fixture source manifest changed; update the complete product matrix explicitly'}
    $allSources=@($pluginSources)+@('AudioMetadataOracle.cs')+@($boundarySources.Values|ForEach-Object{$_})
    $state.fixtureInputs=@($allSources|Sort-Object -Unique|ForEach-Object{@{path="recovery/tests/$_";sha256=(Get-FileHash "recovery/tests/$_" -Algorithm SHA256).Hash.ToLowerInvariant()}})
    $state.realMainInputs=@(Get-ChildItem recovery/fixtures/audio/main -File|Sort-Object Name|ForEach-Object{@{name=$_.Name;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})
    Copy-Item SDK/tools/*.dll $oracle
    $state.originalInputs=@(Get-ChildItem $oracle -Filter *.dll -File|Sort-Object Name|ForEach-Object{@{name=$_.Name;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})
    $script:csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $script:config='<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/></startup></configuration>'
    Build-AudioFixture $oracle AudioPluginOracle $pluginSources "$oracle/BinaryAssetBuilder.Core.dll"
    Build-AudioFixture $oracle AudioMetadataOracle @('AudioMetadataOracle.cs') "$oracle/BinaryAssetBuilder.Core.dll"
    $env:AUDIO_TEST_REAL_DIRECTORY=(Resolve-Path recovery/fixtures/audio/main).Path
    Invoke-AudioFixture "$oracle/AudioMetadataOracle.exe" @("$oracle/BinaryAssetBuilder.AudioCompiler.dll",'original',"$out/metadata-original.txt") $oracle "$out/metadata-original.log"
    foreach($group in $groups){
        Invoke-AudioFixture "$oracle/AudioPluginOracle.exe" @("$oracle/BinaryAssetBuilder.AudioCompiler.dll",'original',$fixtures,"$out/$group-original.txt",$group) $oracle "$out/$group-original.log"
    }
    if(@(Get-Content "$out/multichannel-original.txt").Count -ne 1728 -or @(Get-Content "$out/multichannel-aiff-original.txt").Count -ne 6336){throw 'Incomplete original multichannel matrix'}
    foreach($mode in @('Debug','Release')){
        $clean=Join-Path $work $mode
        $validationRun=Initialize-CompilerValidationRun (Join-Path $CompilerDirectory $mode) $clean $mode $state.sourceCommit
        $exe=Join-Path $clean 'NewModCompiler.exe'
        Build-AudioFixture $clean AudioPluginOracle $pluginSources $exe -Product
        Build-AudioFixture $clean AudioMetadataOracle @('AudioMetadataOracle.cs') $exe -Product
        foreach($name in @($boundarySources.Keys|Sort-Object)){Build-AudioFixture $clean $name $boundarySources[$name] $exe -Product}
        $allowed=@('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')+@(@('AudioPluginOracle','AudioMetadataOracle')+@($boundarySources.Keys)|ForEach-Object{"$_.exe";"$_.exe.config"})
        if(@(Get-ChildItem $clean -Recurse|Where-Object{$_.PSIsContainer -or $_.Name -notin $allowed}).Count -ne 0){throw 'Unexpected file in isolated Audio product directory'}
        $modeResult=@{configuration=$mode;exeSha256=$validationRun.productExeSha256;validationHostSha256=$validationRun.validationHostSha256;groups=@();boundaryChecks=@();originalBusinessDllsAbsent=$true}
        $env:PATH="$env:WINDIR/System32;$env:WINDIR;$env:WINDIR/Microsoft.NET/Framework/v4.0.30319"
        try{
            Invoke-AudioFixture $validationRun.exe @("$clean/AudioMetadataOracle.exe",$exe,'catalog',"$out/$mode-metadata.txt") $clean "$out/$mode-metadata.log"
            $modeResult.metadataRecords=Compare-AudioTranscript "$out/metadata-original.txt" "$out/$mode-metadata.txt" "$mode-metadata"
            foreach($name in @($boundarySources.Keys|Sort-Object)){
                $fixtureArgs=@("$clean/$name.exe",$exe)
                if($name -eq 'AudioInputReaderOracle'){$fixtureArgs+=$fixtures}
                Invoke-AudioFixture $validationRun.exe $fixtureArgs $clean "$out/$mode-$name.log"
                $modeResult.boundaryChecks+=@{fixture=$name;passed=$true;summary=@(Select-String -LiteralPath "$out/$mode-$name.log" -SimpleMatch 'PASS '|ForEach-Object{$_.Line})}
            }
            foreach($group in $groups){
                $expected="$out/$group-original.txt";$actual="$out/$mode-$group.txt"
                Invoke-AudioFixture $validationRun.exe @("$clean/AudioPluginOracle.exe",$exe,'recovered',$fixtures,$actual,$group) $clean "$out/$mode-$group.log"
                $safetyRejections=0
                if($group -eq 'resample-full'){
                    # Preserve exactly the established undefined-import exception:
                    # eight original malloc-dependent records are evidence only;
                    # candidate must reject each with the exact oracle contract.
                    $pattern='^pcm-resample-400-96000-1-[12]-signal0-(NONE|XAS)-(False|True):0:dispatch:'
                    $originalRows=@(Get-Content $expected);$candidateRows=@(Get-Content $actual)
                    if($originalRows.Count -ne 432 -or $candidateRows.Count -ne 432){throw 'Expected all 432 registered resampling records'}
                    $originalUndefined=@($originalRows|Where-Object{$_ -match $pattern});$candidateUndefined=@($candidateRows|Where-Object{$_ -match $pattern})
                    if($originalUndefined.Count -ne 8 -or $candidateUndefined.Count -ne 8 -or
                       @($originalUndefined|Where-Object{$_ -notmatch ':BUFFER:'}).Count -ne 0 -or
                       @($candidateUndefined|Where-Object{$_ -notmatch ':ERROR:System.NotSupportedException:' -or $_ -notmatch ':CUSTOM:<absent>$'}).Count -ne 0){throw 'Invalid undefined-import evidence or safety rejection matrix'}
                    $originalUndefined|Set-Content "$out/$mode-original-single-frame-evidence.txt"
                    $candidateUndefined|Set-Content "$out/$mode-single-frame-safety.txt"
                    $expected="$out/$mode-original-resample-defined.txt";$actual="$out/$mode-resample-defined.txt"
                    @($originalRows|Where-Object{$_ -notmatch $pattern})|Set-Content $expected
                    @($candidateRows|Where-Object{$_ -notmatch $pattern})|Set-Content $actual
                    $safetyRejections=8
                }
                $count=Compare-AudioTranscript $expected $actual "$mode-$group"
                $modeResult.groups+=@{group=$group;passed=$true;records=$count;candidateSafetyRejections=$safetyRejections;originalUndefinedEvidenceRecords=$safetyRejections;transcriptSha256=(Get-FileHash $actual -Algorithm SHA256).Hash.ToLowerInvariant()}
                Write-Output "PASS product Audio $mode $group`: $count identical three-buffer/custom-data records; safetyRejections=$safetyRejections"
            }
        }finally{$env:PATH=$oldPath}
        Complete-CompilerValidationRun $validationRun
        $modeResult.passed=$true;$state.configurations+=$modeResult
    }
    $state.status='passed';$global:LASTEXITCODE=0
}catch{$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally{$env:PATH=$oldPath;$env:AUDIO_TEST_REAL_DIRECTORY=$oldRealDirectory;$state|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json";Pop-Location}
