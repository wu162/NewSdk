$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/volume-images/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-images-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkValidated=$false;scope='actual ProcessInstance image entry; supported TGA to raw volume DDS (PC, no mips); three buffers/full cdata';configurations=@()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log" | Where-Object {$_ -match '^PASS '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Checked oracle-build {& $csc /nologo /platform:x86 /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/TextureVolumePipelineOracle.exe" (Resolve-Path recovery/tests/TextureVolumePipelineOracle.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/TextureVolumePipelineOracle.exe.config"
 foreach($mode in @('Debug','Release')){
  Checked "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Checked "$mode-oracle" {& "$work/TextureVolumePipelineOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" "$root/recovery/tests/fixtures/texture-main" "$work/fixtures-$mode"}
  $r.configurations+=@{configuration=$mode;passed=$true;summary=@(Get-Content "$out/$mode-oracle.log" | Where-Object {$_ -match '^PASS '})}
 }
 $r.status='passed'
}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}


