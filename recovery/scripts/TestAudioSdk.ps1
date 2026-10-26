param([string]$OutputDirectory,[int]$TimeoutSeconds=300,[string[]]$RealAudioFiles=@(),[switch]$AiffLoopsOnly,[switch]$ResamplingOnly,[switch]$ResamplingRegression,[switch]$InputMetadataOnly,[switch]$AifcOnly,[switch]$PcInputsOnly,[switch]$MarkerRangeOnly,[switch]$MultichannelOnly,[switch]$MultichannelAiffOnly)
$ErrorActionPreference='Stop'
if($RealAudioFiles.Count -eq 0){$RealAudioFiles=@('recovery/fixtures/audio/main/real-0.wav','recovery/fixtures/audio/main/real-1.wav')}
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
if(!$OutputDirectory){$OutputDirectory="recovery/evidence/audio/sdk-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite SDK evidence: $OutputDirectory"}
New-Item -ItemType Directory $OutputDirectory -Force|Out-Null;$out=(Resolve-Path $OutputDirectory).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-sdk-'+[Guid]::NewGuid().ToString('N'));$sdk=Join-Path $work 'SDK';$build=Join-Path $work 'build'
New-Item -ItemType Directory $build -Force|Out-Null
Copy-Item SDK $sdk -Recurse
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe";$core=Join-Path $sdk 'tools/BinaryAssetBuilder.Core.dll'
$sources=@(Get-ChildItem src/AudioCompiler -Recurse -Filter '*.cs'|Where-Object{$_.FullName -notmatch '[\\/](bin|obj)[\\/]'}|ForEach-Object FullName)
dotnet build src/AudioCompiler/BinaryAssetBuilder.AudioCompiler.csproj -c Release -o $build -p:AssemblyName=Recovered.AudioCompiler --nologo *> "$out/library-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/library-build.log" -Tail 60;throw 'Recovered audio SDK library build failed'}
& $csc /nologo /target:library /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$core" /out:"$sdk/tools/AudioSdkProbe.dll" (Resolve-Path recovery/tests/AudioSdkTracingPlugin.cs).Path *> "$out/tracer-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/tracer-build.log";throw 'SDK tracing adapter build failed'}
& $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /out:"$build/AudioSdkSupport.exe" (Resolve-Path recovery/tests/AudioSdkSupport.cs).Path (Resolve-Path recovery/tests/AudioMp3Fixtures.cs).Path (Resolve-Path recovery/tests/AudioInputFormatCases.cs).Path (Resolve-Path recovery/tests/AudioAifcInputCases.cs).Path (Resolve-Path recovery/tests/AudioAdp4InputCases.cs).Path (Resolve-Path recovery/tests/AudioAiffSsndCases.cs).Path (Resolve-Path recovery/tests/AudioPcInputSdkCases.cs).Path (Resolve-Path recovery/tests/AudioPcBoundarySdkCases.cs).Path (Resolve-Path recovery/tests/AudioPcMarkerRangeSdkCases.cs).Path (Resolve-Path recovery/tests/AudioAiffMarkerRangeCases.cs).Path (Resolve-Path recovery/tests/AudioLoopMetadataCases.cs).Path (Resolve-Path recovery/tests/AudioLoopMetadataOptionalCases.cs).Path (Resolve-Path recovery/tests/AudioAiffOffsetCases.cs).Path (Resolve-Path recovery/tests/AudioLoopProbeCases.cs).Path (Resolve-Path recovery/tests/AudioAiffLoopCases.cs).Path (Resolve-Path recovery/tests/AudioUnrecoveredProbeCases.cs).Path (Resolve-Path recovery/tests/AudioResampleCases.cs).Path (Resolve-Path recovery/tests/AudioMultichannelCases.cs).Path (Resolve-Path recovery/tests/AudioMultichannelAiffCases.cs).Path *> "$out/support-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/support-build.log";throw 'SDK support host build failed'}
$createMode=if($MultichannelAiffOnly){'create-multichannel-aiff'}elseif($MultichannelOnly){'create-multichannel'}elseif($MarkerRangeOnly){'create-marker-range'}elseif($PcInputsOnly){'create-pc-inputs'}elseif($AifcOnly){'create-aifc'}elseif($InputMetadataOnly){'create-input-metadata'}elseif($ResamplingRegression){'create-resample-regression'}elseif($ResamplingOnly){'create-resample'}elseif($AiffLoopsOnly){'create-aiff-loops'}else{'create'}
& "$build/AudioSdkSupport.exe" $createMode $sdk "$out/expected-assets.tsv" @RealAudioFiles *> "$out/schema.log"
if($LASTEXITCODE -ne 0){Get-Content "$out/schema.log";throw 'Full original CnC3Types schema validation failed; BAB not started'}
Copy-Item "$build/Recovered.AudioCompiler.dll" "$sdk/tools"
Copy-Item "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll" "$build/original-audio.dll"
$cfg=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config';[xml]$config=Get-Content $cfg -Raw
$routes=@($config.configuration.assetbuilder.plugins.plugin|Where-Object{$_.assetTypes.Split(',') -contains 'AudioFile'})
if($routes.Count -ne 1 -or $routes[0].assetTypes -ne 'AudioFile,AudioFileMP3Passthrough,PathMusicMap,PathMusicTrack,PathMusicEvent'){throw 'Unexpected AudioCompiler SDK route'}
$routes[0].SetAttribute('targetType','AudioSdkTracingPlugin, AudioSdkProbe');$routes[0].SetAttribute('enabled','true');$routes[0].SetAttribute('useBuildCache','false')
if($config.configuration.startup){$config.configuration.RemoveChild($config.configuration.startup)|Out-Null}
$startup=$config.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true');$runtime=$config.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0');$startup.AppendChild($runtime)|Out-Null;$config.configuration.AppendChild($startup)|Out-Null;$config.Save($cfg)
Copy-Item $cfg "$out/sdk.config"
$inputs=@(Get-ChildItem "$sdk/Mods/AudioRecoveryProbe" -Recurse -File|ForEach-Object{@{path=$_.FullName.Substring($sdk.Length+1);sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant();bytes=$_.Length}})
$inputs|ConvertTo-Json -Depth 6|Set-Content "$out/inputs.json"
$realInputs=@($RealAudioFiles | ForEach-Object {@{path=(Resolve-Path $_).Path;sha256=(Get-FileHash $_).Hash.ToLowerInvariant()}})
$realInputs|ConvertTo-Json -Depth 4|Set-Content "$out/real-inputs.json"
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';sdkStarted=$false;succeeded=$false;scope='Fresh original and original-free recovered BAB case5 basic-data build; same temporary SDK root';profile=$createMode;expectedAssets=@(Get-Content "$out/expected-assets.tsv").Count;runs=@()}
if($MultichannelOnly -or $MultichannelAiffOnly){
 $state.originalSha256=(Get-FileHash "$build/original-audio.dll").Hash.ToLowerInvariant()
 if($state.originalSha256 -ne 'a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139'){throw 'Original AudioCompiler fingerprint changed'}
 $state.recoveredSha256=(Get-FileHash "$sdk/tools/Recovered.AudioCompiler.dll").Hash.ToLowerInvariant()
 $state.babSha256=(Get-FileHash "$sdk/tools/BinaryAssetBuilder.exe").Hash.ToLowerInvariant()
 $state.configSha256=(Get-FileHash $cfg).Hash.ToLowerInvariant()
 $unchangedDlls=@(Get-ChildItem "$sdk/tools" -File -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.AudioCompiler.dll' | ForEach-Object {@{name=$_.Name;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
 $unchangedDlls | ConvertTo-Json -Depth 4 | Set-Content "$out/unchanged-sdk-dlls.json"
}
$savedSide=$env:AUDIO_SDK_IMPLEMENTATION;$savedTrace=$env:AUDIO_SDK_TRACE
try{
 foreach($side in @('Original','Recovered')){
  if($MultichannelOnly -or $MultichannelAiffOnly){
   foreach($inputRecord in $inputs){$inputFile=Join-Path $sdk $inputRecord.path;if(!(Test-Path $inputFile) -or (Get-FileHash $inputFile).Hash.ToLowerInvariant() -ne $inputRecord.sha256){throw "Multichannel input changed between builds: $($inputRecord.path)"}}
   foreach($dll in $unchangedDlls){if((Get-FileHash "$sdk/tools/$($dll.name)").Hash.ToLowerInvariant() -ne $dll.sha256){throw "Non-target SDK DLL changed: $($dll.name)"}}
   if((Get-FileHash "$sdk/tools/BinaryAssetBuilder.exe").Hash.ToLowerInvariant() -ne $state.babSha256 -or (Get-FileHash $cfg).Hash.ToLowerInvariant() -ne $state.configSha256){throw 'SDK executable/config changed between builds'}
  }
  # Reuse one disposable SDK root because PathMusicTrack serializes resolved absolute file names.
  Remove-Item "$sdk/builtmods/Mods" -Recurse -Force -ErrorAction SilentlyContinue
  Get-ChildItem $sdk -Recurse -Directory|Where-Object{$_.Name -in @('cache','buildcache','sessioncache')}|Remove-Item -Recurse -Force
  Get-ChildItem $sdk -Recurse -File|Where-Object{$_.Extension -eq '.asset' -or $_.Name -like 'binaryassetbuilder.sessioncache.xml*'}|Remove-Item -Force
  if($side -eq 'Recovered'){Remove-Item "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll" -Force;if(Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll"){throw 'Original target DLL still present'}}
  $env:AUDIO_SDK_IMPLEMENTATION=$side;$env:AUDIO_SDK_TRACE="$out/$side-calls.tsv"
  $output=Join-Path $sdk 'builtmods';$mod=Join-Path $sdk 'Mods/AudioRecoveryProbe/Data/Mod.xml'
  $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"",'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true','/art:".;.\Mods\AudioRecoveryProbe\Art;.\Mods;.\Art"','/audio:".;.\Mods\AudioRecoveryProbe\Audio;.\Mods;.\Audio"','/data:".;.\Mods\AudioRecoveryProbe\Data;.\Mods;.\SageXml"')
  @{mode=$side;executable="$sdk/tools/BinaryAssetBuilder.exe";arguments=$arguments;source='SDK/defaultscript.cs executeModBuildStep case5';originalTargetPresent=(Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll")}|ConvertTo-Json -Depth 6|Set-Content "$out/$side-command.json"
  $state.sdkStarted=$true
  $process=Start-Process "$sdk/tools/BinaryAssetBuilder.exe" -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$out/$side-stdout.log" -RedirectStandardError "$out/$side-stderr.log"
  if(!$process.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $process.Id -Force;throw "$side audio SDK timed out"};$process.Refresh()
  if($process.ExitCode -ne 0){Get-Content "$out/$side-stdout.log" -Tail 60;Get-Content "$out/$side-stderr.log";throw "$side BAB failed: $($process.ExitCode)"}
  $files=@()
  foreach($ext in @('bin','manifest','imp','relo')){
   $file=Join-Path $output "Mods/AudioRecoveryProbe/Data/Mod.$ext"
   if(!(Test-Path $file)){throw "$side missing mod.$ext"}
   Copy-Item $file "$out/$side-mod.$ext";$files+=@{name="mod.$ext";bytes=(Get-Item $file).Length;sha256=(Get-FileHash $file).Hash.ToLowerInvariant()}
  }
  $custom=@(Get-ChildItem $output -Recurse -Filter '*.cdata'|Sort-Object FullName|ForEach-Object{"$($_.FullName.Substring($output.Length+1))`t$((Get-FileHash $_.FullName).Hash.ToLowerInvariant())"})
  $expectedCustom = @(Get-Content "$out/expected-assets.tsv" | Where-Object { $_ -notmatch '^PathMusic(Map|Event)\s' }).Count
  if($custom.Count -ne $expectedCustom){throw "$side emitted $($custom.Count) custom-data files; expected $expectedCustom"}
  $custom|Set-Content "$out/$side-custom.tsv"
  $calls=@(Get-Content "$out/$side-calls.tsv").Count
  if($MultichannelAiffOnly -and $calls -ne 6336){throw "Expected6336 actual AIFF multichannel handler calls, got$calls"}
  if($MultichannelOnly -and $calls -ne 1728){throw "Expected1728 actual multichannel handler calls, got$calls"}
  $state.runs+=@{mode=$side;exitCode=$process.ExitCode;outputs=$files;customFiles=$custom.Count;audioCalls=$calls;originalTargetPresent=(Test-Path "$sdk/tools/BinaryAssetBuilder.AudioCompiler.dll")}
 }
 foreach($ext in @('bin','manifest','imp','relo')){if((Get-FileHash "$out/Original-mod.$ext").Hash -ne (Get-FileHash "$out/Recovered-mod.$ext").Hash){throw "Audio SDK output differs: mod.$ext"}}
 if((Get-FileHash "$out/Original-custom.tsv").Hash -ne (Get-FileHash "$out/Recovered-custom.tsv").Hash){throw 'Audio SDK custom-data files differ'}
 & "$build/AudioSdkSupport.exe" compare "$out/Original-calls.tsv" "$out/Recovered-calls.tsv" "$out/expected-assets.tsv" *> "$out/calls-validation.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/calls-validation.log";throw 'Actual SDK handler coverage or three-buffer/custom-data comparison failed'}
 Get-Content "$out/calls-validation.log";$state.succeeded=$true;$state.status='passed'
 Write-Host 'PASS Audio SDK: original/recovered fresh builds, four products/custom files/actual handler buffers identical; recovered original target DLL absent'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$env:AUDIO_SDK_IMPLEMENTATION=$savedSide;$env:AUDIO_SDK_TRACE=$savedTrace;$state|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
