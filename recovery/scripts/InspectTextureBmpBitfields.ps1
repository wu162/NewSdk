$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/bmp-bitfields-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-bmp-bitfields-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;productionAccepted=$false;sdkValidated=$false;scope='original PC BMP bitfields masks: two independent reversed-order processes, two prior-call contexts, repeated descriptor/nativeFormat/full locked storage and LoadImageA RGBA/errors; no recovered/plugin/SDK acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 Checked support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $sources=@('recovery/tests/TextureBmpBitfieldsProbe.cs','recovery/tests/TextureBmpOracle.cs','recovery/tests/TexturePngOracle.cs')|ForEach-Object{(Resolve-Path $_).Path}
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /main:TextureBmpBitfieldsProbe /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureBmpBitfieldsProbe.exe" $sources}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpBitfieldsProbe.exe.config"
 $processes=@();$processRows=@{}
 foreach($process in 0,1){
  Checked "process-$process" {& "$work/TextureBmpBitfieldsProbe.exe" "$work/BinaryAssetBuilder.TextureCompiler.dll" "$work/samples/process-$process" $process}
  Copy-Item "$work/samples/process-$process/transcript.tsv" "$out/process-$process.tsv"
  $rows=Import-Csv "$out/process-$process.tsv" -Delimiter "`t";$fixtures=@($rows|Where-Object kind -eq 'bitfields');$controls=@($rows|Where-Object kind -eq 'control')
  if($fixtures.Count -ne 4320 -or $controls.Count -ne 6 -or $rows.Count -ne 4326){throw 'Incomplete independent bitfields process'}
  $byLabel=@{}
  foreach($row in $rows){
   if([string]::IsNullOrEmpty($row.label) -or $byLabel.ContainsKey($row.label)){throw "Missing or duplicate bitfields label in process $process"}
   if($row.process -ne [string]$process -or $row.inputSha256 -notmatch '^[0-9a-f]{64}$'){throw "Incomplete source identity in process $process"}
   $byLabel[$row.label]=$row
  }
  foreach($depth in 16,24,32){foreach($top in 'False','True'){
   $label="control-h40-d$depth-16x8-top$top-p0-m0-ctx0"
   if(-not $byLabel.ContainsKey($label)){throw "Missing known BI_RGB control $label"}
   $control=$byLabel[$label]
   if($control.kind -ne 'control' -or $control.dib -ne '40' -or $control.depth -ne [string]$depth -or $control.topDown -ne $top -or $control.sourceWidth -ne '16' -or $control.sourceHeight -ne '8' -or $control.compression -ne '0' -or $control.originalStable -ne 'True'){throw "Incomplete known BI_RGB control $label"}
   foreach($call in 1,2){
    $native="native$call";$loaded="loaded$call"
    if([int]$control.("${native}Status") -lt 0 -or $control.("${native}Error") -ne '' -or $control.("${native}Width") -ne '16' -or $control.("${native}Height") -ne '8' -or [int]$control.("${native}StorageBytes") -le 0 -or $control.("${native}DescriptorSha256") -notmatch '^[0-9a-f]{64}$' -or $control.("${native}StorageSha256") -notmatch '^[0-9a-f]{64}$'){throw "Missing native descriptor/storage control $label call $call"}
    if($control.("${loaded}Error") -ne '' -or $control.("${loaded}Width") -ne '16' -or $control.("${loaded}Height") -ne '8' -or $control.("${loaded}Bytes") -ne '512' -or $control.("${loaded}Sha256") -notmatch '^[0-9a-f]{64}$'){throw "Missing original RGBA control $label call $call"}
   }
  }}
  $processRows[$process]=$byLabel
  $processes+=@{process=$process;fixtureCount=$fixtures.Count;controlCount=$controls.Count;decodedCount=@($fixtures|Where-Object loaded1Error -eq '').Count;rejectedCount=@($fixtures|Where-Object loaded1Error -ne '').Count;unstableOriginals=@($fixtures|Where-Object originalStable -eq 'False').Count;storageDifferences=@($fixtures|Where-Object storageStable -eq 'False').Count;descriptorDifferences=@($fixtures|Where-Object descriptorStable -eq 'False').Count;fixtureLoadCalls=$rows.Count*2;contextLoadCalls=4320;initializationControlLoadCalls=1;nativeSnapshots=$rows.Count*2}
 }
 if($processRows[0].Count -ne $processRows[1].Count){throw 'Independent process label set sizes differ'}
 foreach($label in $processRows[0].Keys){
  if(-not $processRows[1].ContainsKey($label)){throw "Independent process source label missing: $label"}
  if($processRows[0][$label].inputSha256 -ne $processRows[1][$label].inputSha256){throw "Independent process source bytes differ: $label"}
 }
 $r.pairedInputCount=4320;$r.pairedControlInputCount=6;$r.crossProcessSourceIdentityVerified=$true
 $r.processes=$processes;$r.fixtureCount=8640;$r.controlCount=12;$r.fixtureOriginalCalls=17304;$r.contextOriginalCalls=8640;$r.initializationControlCalls=2;$r.originalCalls=25946;$r.nativeSnapshots=17304
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}
