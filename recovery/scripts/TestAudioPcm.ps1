param([Parameter(Mandatory=$true)][string]$OriginalEvidenceDirectory, [string]$OutputDirectory, [ValidateSet('PCM','XAS')][string]$Codec='PCM')
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$evidence=(Resolve-Path $OriginalEvidenceDirectory).Path
if (!$env:RUNNER_TEMP) {$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if (!$OutputDirectory) {$OutputDirectory="recovery/evidence/audio/$($Codec.ToLowerInvariant())-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if (Test-Path $OutputDirectory) {throw "Refusing to overwrite evidence: $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-pcm-'+[Guid]::NewGuid().ToString('N')); New-Item -ItemType Directory $work | Out-Null
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /out:"$work/AudioPcmEncoderOracle.exe" (Resolve-Path recovery/tests/AudioPcmEncoderOracle.cs).Path *> "$out/oracle-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log";throw 'PCM oracle build failed'}
& $csc /nologo /platform:x86 /out:"$work/AudioBlockFramesOracle.exe" (Resolve-Path recovery/tests/AudioBlockFramesOracle.cs).Path *> "$out/block-frames-oracle-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/block-frames-oracle-build.log";throw 'Block-size oracle build failed'}
$modes=@()
$evidenceFile = if ($Codec -eq 'XAS') { 'xas-original.txt' } else { 'pcm-original.txt' }
$sources = @((Resolve-Path src/AudioCompiler/Native/Pcm16Encoder.cs).Path)
if ($Codec -eq 'XAS') {
 $sources += (Resolve-Path src/AudioCompiler/Native/Xas1Encoder.cs).Path
 & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$work/AudioXasBlockOracle.exe" (Resolve-Path recovery/tests/AudioXasBlockOracle.cs).Path *> "$out/block-oracle-build.log"
 if ($LASTEXITCODE -ne 0) { Get-Content "$out/block-oracle-build.log"; throw 'XAS block oracle build failed' }
}
foreach($mode in @('Debug','Release')) {
 $opt=if($mode -eq 'Release'){'/optimize+'}else{'/optimize-'}
 $library="$work/Recovered.Audio.Pcm.$mode.dll"
 & $csc /nologo /target:library /platform:x86 $opt /out:"$library" $sources *> "$out/$mode-build.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log";throw "$mode PCM build failed"}
 & "$work/AudioPcmEncoderOracle.exe" $library "$evidence/$evidenceFile" $evidence $Codec *> "$out/$mode.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log";throw "$mode PCM component differs"}
 Get-Content "$out/$mode.log"
 & "$work/AudioBlockFramesOracle.exe" $library (Resolve-Path recovery/evidence/audio/native-block-frames/original.tsv).Path $Codec *> "$out/$mode-block-frames.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-block-frames.log";throw "$mode native block-size differs"}
 Get-Content "$out/$mode-block-frames.log"
 if ($Codec -eq 'XAS') {
  & "$work/AudioXasBlockOracle.exe" $library (Resolve-Path recovery/evidence/audio/native-xas/original-blocks.tsv).Path *> "$out/$mode-blocks.log"
  if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-blocks.log"; throw "$mode XAS block algorithm differs" }
  Get-Content "$out/$mode-blocks.log"
 }
 $modes+=@{configuration=$mode;passed=$true;records=$(if($Codec -eq 'XAS'){12}else{8});recoveredSha256=(Get-FileHash $library).Hash.ToLowerInvariant();blockFrameRecords=504}
}
@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;scope="$Codec encoder component only, original AudioFile HeaderData/CustomData versus static encoder";modes=$modes;originalEvidenceSha256=(Get-FileHash "$evidence/$evidenceFile").Hash.ToLowerInvariant();pluginValidated=$false;sdkValidated=$false}|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"
