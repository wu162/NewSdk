$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/png-rgb16/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT";$work=Join-Path $env:RUNNER_TEMP ('png-rgb16-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;sdkValidated=$false}
function Run([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 45;throw "$name failed"}}
try{
 Copy-Item SDK/tools/*.dll $work
 Run build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 Run compile {& $csc /nologo /unsafe /platform:x86 /main:TexturePngRgb16Probe /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TexturePngRgb16Probe.exe" (Resolve-Path recovery/tests/TexturePngOracle.cs).Path (Resolve-Path recovery/tests/TexturePngRgb16Probe.cs).Path}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TexturePngRgb16Probe.exe.config"
 Run probe {& "$work/TexturePngRgb16Probe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$out/images"}
 $r.status='probe-only'
}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
