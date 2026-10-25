param([ValidateSet('CubicResize')][string]$Group='CubicResize',[switch]$ProbeOnly)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out="recovery/evidence/texture/stage2/$Group/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-stage2-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work | Out-Null
$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;group=$Group;experimental=$true;productionRouteEnabled=$false;probeOnly=[bool]$ProbeOnly;status='started';sdkValidated=$false;configurations=@();originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()}
$r.sourceFiles=@(Get-ChildItem src/TextureCompiler/Managed -File | Where-Object {$_.Extension -in '.cs','.csproj'} | Sort-Object FullName | ForEach-Object {@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})
$r.experimentalSource=@{path='src/TextureCompiler/Experiments/TextureCubicResizeEncoder.cs';sha256=(Get-FileHash src/TextureCompiler/Experiments/TextureCubicResizeEncoder.cs -Algorithm SHA256).Hash.ToLowerInvariant()}
$r.oracleSource=@{path='recovery/tests/TextureCubicResizeDifferential.cs';sha256=(Get-FileHash recovery/tests/TextureCubicResizeDifferential.cs -Algorithm SHA256).Hash.ToLowerInvariant()}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 35;throw "$name failed"};Get-Content "$out/$name.log" | Where-Object {$_ -match '^(PASS|PROBE) '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $oracle=switch($Group){'Volume'{'TextureVolumeOracle'}'Formats'{'TextureTgaFormatsOracle'}'CubicResize'{'TextureCubicResizeDifferential'}}
 foreach($mode in @('Debug','Release')){
  Checked "$mode-build" {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo}
  Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work -Force
  $dll="$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
  $experimental="$work/Experimental.Texture.Cubic.dll"
  Checked "$mode-experimental-build" {& $csc /nologo /target:library /platform:x86 /r:$dll /out:$experimental (Resolve-Path src/TextureCompiler/Experiments/TextureCubicResizeEncoder.cs).Path}
  $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
  Checked "$mode-oracle-build" {& $csc /nologo /unsafe /platform:x86 $opt /r:$dll /out:"$work/$oracle.exe" (Resolve-Path "recovery/tests/$oracle.cs").Path}
  '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/$oracle.exe.config"
  switch($Group){
   'Volume'{Checked "$mode-oracle" {& "$work/$oracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll"}}
   'Formats'{$flag=if($ProbeOnly){'--probe'}else{'--strict'};Checked "$mode-oracle" {& "$work/$oracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-transcript.txt" "$work/fixtures-$mode" $flag}}
   'CubicResize'{Checked "$mode-oracle" {& "$work/$oracle.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-transcript.txt" $experimental}}
  }
  $r.configurations+=@{configuration=$mode;passed=(!$ProbeOnly);summary=@(Get-Content "$out/$mode-oracle.log" | Where-Object {$_ -match '^(PASS|PROBE) '})}
 }
 $files=@(Get-ChildItem "$out/*-transcript.txt")
 if($files.Count -gt 0){Compress-Archive -Path $files.FullName -DestinationPath "$out/transcripts.zip";$files|Remove-Item}
 $r.status=if($ProbeOnly){'probe-only'}else{'passed'}
}finally{$r | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"}


