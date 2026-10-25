$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/png-ranges/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('png-ranges-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$false;sdkValidated=$false;configurations=@()}
function Run([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 20;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS Texture'}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 foreach($mode in @('Debug','Release')){
  Run "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $optimize=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Run "$mode-compile" {& $csc /nologo /unsafe /platform:x86 $optimize /main:TexturePngRangeOracle /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TexturePngRangeOracle.exe" (Resolve-Path recovery/tests/TexturePngOracle.cs).Path (Resolve-Path recovery/tests/TexturePngRangeOracle.cs).Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePngRangeOracle.exe.config"
  Run "$mode-range" {& "$work/TexturePngRangeOracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/images"}
  $r.configurations+=@{configuration=$mode;passed=$true;summary=@(Get-Content "$out/$mode-range.log"|Where-Object{$_ -match '^PASS Texture'})}
 }
 $r.status='passed'
}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
