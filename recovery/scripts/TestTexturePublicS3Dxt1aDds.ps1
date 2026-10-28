$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/public-s3-dxt1a-dds/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-s3-dxt1a-dds-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;productionAccepted=$false;sdkValidated=$false;scope='Experimental full transparent-block DXT1A 2D/volume no-mip and float mip DDS; each encoded block must contain an actual transparent sample, no production/plugin/SDK acceptance';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 50;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^COMPARISON '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($configuration in @('Debug','Release')){
  Checked "$configuration-native" {& "$PSScriptRoot/BuildTexturePublicS3.ps1" -Destination "$work/$configuration" -Configuration $configuration}
  Checked "$configuration-managed" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $configuration -o "$work/build-$configuration" --nologo}
  Copy-Item "$work/build-$configuration/*.dll" $work -Force
  $r.nativeBinaries=@($r.nativeBinaries|Where-Object{$_})+@((Get-Content "$work/$configuration/PublicS3-native.json" -Raw|ConvertFrom-Json))
  $opt=if($configuration -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$configuration-build" {& $csc /nologo /unsafe /platform:x86 $opt /out:"$work/TexturePublicS3Dxt1aDdsComparison.exe" (Resolve-Path recovery/tests/TexturePublicS3Dxt1aDdsComparison.cs).Path (Resolve-Path recovery/tests/TexturePublicS3Dxt1aExperimental.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePublicS3Dxt1aDdsComparison.exe.config"
  Checked "$configuration-original" {& "$work/TexturePublicS3Dxt1aDdsComparison.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$configuration-transcript.tsv" "$work/$configuration/Recovered.TexturePublicS3.dll" "$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"}
  $isolated="$work/isolated-$configuration";New-Item -ItemType Directory -Force $isolated|Out-Null
  Get-ChildItem "$work/*.dll"|Where-Object{$_.Name -ne "BinaryAssetBuilder.TextureCompiler.dll"}|Copy-Item -Destination $isolated
  Copy-Item "$work/TexturePublicS3Dxt1aDdsComparison.exe","$work/$configuration/Recovered.TexturePublicS3.dll" $isolated
  Checked "$configuration-isolated" {& "$isolated/TexturePublicS3Dxt1aDdsComparison.exe" --replay "$out/$configuration-transcript.tsv.records" "$isolated/Recovered.TexturePublicS3.dll" "$isolated/Recovered.BinaryAssetBuilder.TextureCompiler.dll"}
  $r.configurations+=@{configuration=$configuration;comparisonCompleted=$true;withoutOriginalTextureCompiler=$true;summary=@(Get-Content "$out/$configuration-original.log","$out/$configuration-isolated.log"|Where-Object{$_ -match '^COMPARISON '})}
 }
 $files=@(Get-ChildItem $out -File|Where-Object {$_.Name -match '-transcript[.]tsv([.]records|[.]options)?$'})
 Compress-Archive -Path $files.FullName -DestinationPath "$out/comparison.zip"
 $r.archiveSha256=(Get-FileHash "$out/comparison.zip").Hash.ToLowerInvariant();$files|Remove-Item
 $r.status='comparison-completed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"}


