param([switch]$ProbeOnly)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/bmp/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-bmp-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$receipt=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;probeOnly=[bool]$ProbeOnly;configurations=@();matrixVersion=1}
$receipt.originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()
$receipt.scope='BMP POT BI_RGB decoding versus native LoadImageA; production dispatch and SDK not yet validated'
$receipt.sourceFiles=@('src/TextureCompiler/Managed/TextureBmpDecoder.cs','src/TextureCompiler/Managed/TexturePacker.cs','recovery/tests/TextureBmpOracle.cs','recovery/scripts/TestTextureBmp.ps1','SDK/tools/BinaryAssetBuilder.TextureCompiler.dll','SDK/tools/BinaryAssetBuilder.Core.dll','SDK/tools/BinaryAssetBuilder.Utility.dll') | ForEach-Object {@{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant()}}
function Run([string]$label,[scriptblock]$action){& $action *> "$out/$label.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$label.log" -Tail 35;throw "$label failed"};Get-Content "$out/$label.log" | Where-Object {$_ -match '^(PASS|PROBE Texture)'}}
try{
    Copy-Item SDK/tools/*.dll $work
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    foreach($mode in @('Debug','Release')){
        Run "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
        Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
        $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
        $optimize=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
        Run "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $optimize /r:$dll /out:"$work/TextureBmpOracle.exe" (Resolve-Path recovery/tests/TextureBmpOracle.cs).Path}
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/TextureBmpOracle.exe.config"
        $probeFlag=if($ProbeOnly){'--probe'}else{'--strict'}
        Run "$mode-oracle" {& "$work/TextureBmpOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-transcript.txt" "$work/fixtures-$mode" $probeFlag}
        $summary = @(Get-Content "$out/$mode-oracle.log" | Where-Object {$_ -match '^(PASS|PROBE Texture)'})
        $counts=@{}
        foreach($metric in @('fixtures','decodedMatches','originalFailures','rejectionMatches','unsupportedCount','mismatches','assertions')){
            $m=[regex]::Match(($summary -join ' '),$metric+'=(\d+)')
            if($m.Success){$counts[$metric]=[int]$m.Groups[1].Value}
        }
        $receipt.configurations+=@{configuration=$mode;passed=(!$ProbeOnly);probeCompleted=[bool]$ProbeOnly;summary=$summary;counts=$counts;transcriptSha256=(Get-FileHash "$out/$mode-transcript.txt").Hash.ToLowerInvariant()}
    }
    Compress-Archive -Path "$out/*-transcript.txt" -DestinationPath "$out/transcripts.zip"
    Remove-Item "$out/*-transcript.txt"
    $receipt.status=if($ProbeOnly){'probe-only'}else{'passed'}
}finally{$receipt | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json"}

