param([switch]$ProbeOnly)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/tga/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-tga-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$receipt=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;probeOnly=[bool]$ProbeOnly;configurations=@();realFixtures=(Get-Content recovery/tests/fixtures/texture-main/manifest.json -Raw | ConvertFrom-Json)}
$receipt.originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()
$receipt.scope='TGA explicitly declared decoder subset and rejection boundaries; full D3DX image decoding, codec compression and SDK not validated'
$receipt.sourceFiles=@('src/TextureCompiler/Managed/TextureTgaDecoder.cs','src/TextureCompiler/Managed/TexturePacker.cs','recovery/tests/TextureTgaOracle.cs','recovery/scripts/TestTextureTga.ps1','SDK/tools/BinaryAssetBuilder.TextureCompiler.dll','SDK/tools/BinaryAssetBuilder.Core.dll','SDK/tools/BinaryAssetBuilder.Utility.dll') | ForEach-Object {@{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant()}}
foreach($f in $receipt.realFixtures.files){if((Get-FileHash $f.fixture -Algorithm SHA256).Hash.ToLowerInvariant() -ne $f.sha256){throw 'Real fixture hash mismatch'}}
function Run([string]$label,[scriptblock]$action){& $action *> "$out/$label.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$label.log" -Tail 35;throw "$label failed"};Get-Content "$out/$label.log" | Where-Object {$_ -match '^(PASS|PROBE Texture)'}}
try{
    Copy-Item SDK/tools/*.dll $work
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    foreach($mode in @('Debug','Release')){
        Run "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
        Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
        $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
        $optimize=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
        Run "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $optimize /r:$dll /out:"$work/TextureTgaOracle.exe" (Resolve-Path recovery/tests/TextureTgaOracle.cs).Path}
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/TextureTgaOracle.exe.config"
        $probeFlag=if($ProbeOnly){'--probe'}else{'--strict'}
        Run "$mode-oracle" {& "$work/TextureTgaOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-transcript.txt" "$work/fixtures-$mode" $probeFlag "$root/recovery/tests/fixtures/texture-main"}
        $summary = @(Get-Content "$out/$mode-oracle.log" | Where-Object {$_ -match '^(PASS|PROBE Texture)'})
        $counts=@{}
        foreach($metric in @('fixtures','decodedMatches','rejectionMatches','unsupportedCount','mismatches','assertions')){
            $m=[regex]::Match(($summary -join ' '),$metric+'=(\d+)')
            if($m.Success){$counts[$metric]=[int]$m.Groups[1].Value}
        }
        $receipt.configurations+=@{configuration=$mode;passed=(!$ProbeOnly);probeCompleted=[bool]$ProbeOnly;summary=$summary;counts=$counts;transcriptSha256=(Get-FileHash "$out/$mode-transcript.txt").Hash.ToLowerInvariant()}
    }
    Compress-Archive -Path "$out/*-transcript.txt" -DestinationPath "$out/transcripts.zip"
    Remove-Item "$out/*-transcript.txt"
    $receipt.status=if($ProbeOnly){'probe-only'}else{'passed'}
}finally{$receipt | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json"}
