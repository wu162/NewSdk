$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/raw-dds-probe/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-raw-dds-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='original nvDDS RGBA uncompressed-format outputs only; no recovered encoder acceptance';sdkValidated=$false;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 & $csc /nologo /unsafe /platform:x86 /out:"$work/TextureRawDdsOracle.exe" (Resolve-Path recovery/tests/TextureRawDdsOracle.cs).Path *> "$out/build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/build.log";throw 'Original raw DDS oracle build failed'}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/TextureRawDdsOracle.exe.config"
 & "$work/TextureRawDdsOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/original.txt" *> "$out/oracle.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/oracle.log" -Tail 50;throw 'Original raw DDS oracle failed'}
 Get-Content "$out/oracle.log"
 $r.records=@(Get-Content "$out/original.txt").Count
 $r.transcriptSha256=(Get-FileHash "$out/original.txt").Hash.ToLowerInvariant()
 Compress-Archive -Path "$out/original.txt" -DestinationPath "$out/transcript.zip"
 Remove-Item "$out/original.txt"
 $r.status='original-evidence-only'
}finally{$r | ConvertTo-Json -Depth 8 | Set-Content "$out/extraction.json"}
