param([ValidateRange(1,1800)][int]$TimeoutSeconds=300)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/bmp-indexed-header-load/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('texture-bmp-indexed-headers-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;productionAccepted=$false;sdkValidated=$false;processTimeoutSeconds=$TimeoutSeconds;scope='original PC BMP DIB/core/OS2 indexed 1/4/8-bit BI_RGB: proper bounded 3/4-byte palettes, 0/255 reserved palette alpha, legal gaps, POT/NPOT/partial and orientation; repeated full original RGBA/errors; success/rejection/stability not preclassified; no recovered/plugin/SDK acceptance'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 40;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PROBE '}}
try{
 Copy-Item SDK/tools/*.dll $work
 $r.originalSha256=(Get-FileHash "$work/BinaryAssetBuilder.TextureCompiler.dll").Hash.ToLowerInvariant()
 if($r.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original TextureCompiler changed; inspect original binder contracts'}
 Checked support-build {dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c Release -o "$work/build" --nologo}
 Copy-Item "$work/build/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $work
 $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
 $sources=@('recovery/tests/TextureBmpIndexedHeaderProbe.cs','recovery/tests/TextureBmpOracle.cs','recovery/tests/TexturePngOracle.cs')|ForEach-Object{(Resolve-Path $_).Path}
 Checked build {& $csc /nologo /unsafe /platform:x86 /optimize+ /main:TextureBmpIndexedHeaderProbe /r:System.Drawing.dll /r:"$work/Recovered.BinaryAssetBuilder.TextureCompiler.dll" /out:"$work/TextureBmpIndexedHeaderProbe.exe" $sources}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/TextureBmpIndexedHeaderProbe.exe.config"
 $arguments=@(('"{0}"' -f "$work/BinaryAssetBuilder.TextureCompiler.dll"),('"{0}"' -f "$work/samples"))
 $probe=Start-Process -FilePath "$work/TextureBmpIndexedHeaderProbe.exe" -ArgumentList $arguments -RedirectStandardOutput "$out/oracle.stdout.log" -RedirectStandardError "$out/oracle.stderr.log" -NoNewWindow -PassThru
 try{
  if(-not $probe.WaitForExit($TimeoutSeconds*1000)){
   $r.processStatus='timeout';Stop-Process -Id $probe.Id -Force -ErrorAction SilentlyContinue
   if(-not $probe.WaitForExit(10000)){throw 'Original indexed-header process did not terminate after timeout'}
   throw "Original indexed-header process exceeded $TimeoutSeconds seconds"
  }
  $probe.WaitForExit();$r.exitCode=$probe.ExitCode;$r.processStatus='exited'
  Get-Content "$out/oracle.stdout.log","$out/oracle.stderr.log"|Set-Content "$out/oracle.log"
  if($probe.ExitCode -ne 0){Get-Content "$out/oracle.log" -Tail 40;throw "Original indexed-header process exit=$($probe.ExitCode)"}
  Get-Content "$out/oracle.stdout.log"|Where-Object{$_ -match '^PROBE '}
 }finally{
  if(-not $probe.HasExited){Stop-Process -Id $probe.Id -Force -ErrorAction SilentlyContinue}
  $probe.Dispose()
 }
 $progress=@(Import-Csv "$work/samples/progress.tsv" -Delimiter "`t")
 if($progress.Count -ne 1 -or $progress[0].label -ne 'all' -or $progress[0].phase -ne 'completed'){throw 'Missing completed indexed-header phase'}
 Copy-Item "$work/samples/transcript.tsv" "$out/transcript.tsv"
 $rows=@(Import-Csv "$out/transcript.tsv" -Delimiter "`t");$indexed=@($rows|Where-Object kind -eq 'indexed');$controls=@($rows|Where-Object kind -eq 'control')
 if($rows.Count -ne 2121 -or $indexed.Count -ne 2025 -or $controls.Count -ne 96){throw 'Incomplete original indexed-header corpus'}
 $labels=@{}
 foreach($row in $rows){
  if([string]::IsNullOrEmpty($row.label) -or $labels.ContainsKey($row.label)){throw 'Missing or duplicate original indexed-header label'}
  if($row.inputSha256 -notmatch '^[0-9a-f]{64}$' -or $row.originalStable -notin @('True','False')){throw 'Missing original indexed-header identity/stability classification'}
  $labels[$row.label]=$row
 }
 $expected=@{}
 foreach($dib in 12,40,52,56,64,108,124){foreach($depth in 1,4,8){foreach($size in '16x8','7x5','1x1'){
  $directions=if($dib -eq 12){@('False')}else{@('False','True')};$alphas=if($dib -eq 12){@(0)}else{@(0,255)}
  foreach($top in $directions){foreach($pattern in 0,1,2){foreach($gap in 0,3,17){foreach($alpha in $alphas){
   $label="index-h$dib-d$depth-$size-top$top-p$pattern-g$gap-a$alpha"
   if(-not $labels.ContainsKey($label) -or $labels[$label].kind -ne 'indexed'){throw "Missing indexed-header input $label"};$expected[$label]=$true
  }}}}
 }}}
 foreach($size in '16x8','7x5','1x1'){
  foreach($dib in 12,40,52,56,64,108,124){
   $depths=if($dib -eq 12 -or $dib -eq 64){@(24)}else{@(16,24,32)};$directions=if($dib -eq 12 -or $dib -eq 64){@('False')}else{@('False','True')}
   foreach($depth in $depths){foreach($top in $directions){
    $label="control-h$dib-d$depth-$size-top$top-p0-g0-a0"
    if(-not $labels.ContainsKey($label)){throw "Missing true-color indexed-header control $label"};$expected[$label]=$true;$row=$labels[$label]
    if($row.kind -ne 'control' -or $row.error -ne '' -or $row.repeatError -ne '' -or $row.originalStable -ne 'True' -or [int]$row.bytes -le 0 -or $row.bytes -ne $row.repeatBytes -or $row.sha256 -ne $row.repeatSha256 -or $row.sha256 -notmatch '^[0-9a-f]{64}$'){throw "Incomplete true-color indexed-header control $label"}
   }}
  }
 }
 if($expected.Count -ne $rows.Count){throw 'Unexpected indexed-header label set'}
 $r.fixtureCount=$indexed.Count;$r.controlCount=$controls.Count;$r.sampleCount=$rows.Count
 $r.decodedCount=@($indexed|Where-Object error -eq '').Count;$r.rejectedCount=@($indexed|Where-Object error -ne '').Count
 $r.stableOriginals=@($indexed|Where-Object originalStable -eq 'True').Count;$r.unstableOriginals=@($indexed|Where-Object originalStable -eq 'False').Count
 if($r.decodedCount+$r.rejectedCount -ne 2025 -or $r.stableOriginals+$r.unstableOriginals -ne 2025){throw 'Incomplete original indexed-header result classification'}
 $r.fixtureOriginalCalls=$rows.Count*2;$r.initializationControlCalls=1;$r.originalCalls=$r.fixtureOriginalCalls+1
 Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/samples.zip"
 $r.samplesSha256=(Get-FileHash "$out/samples.zip").Hash.ToLowerInvariant();$r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{
 if($r.status -ne 'probe-only' -and (Test-Path "$work/samples")){Compress-Archive -Path "$work/samples/*" -DestinationPath "$out/partial-samples.zip" -Force}
 $r|ConvertTo-Json -Depth 14|Set-Content "$out/validation.json"
}
