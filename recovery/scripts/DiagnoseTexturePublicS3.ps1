$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/public-codecs/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('s3-stages-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';diagnosticOnly=$true;productionAccepted=$false;sdkValidated=$false;scope='27 remaining S3 RGB differences plus one matching control; existing recovered state-machine diagnostic, checked against immutable original full-block gold; no replacement callback'}
function Checked([string]$name,[scriptblock]$code){& $code *> "$out/$name.log";if($LASTEXITCODE -ne 0){Get-Content "$out/$name.log" -Tail 45;throw "$name failed"};Get-Content "$out/$name.log"|Where-Object{$_ -match '^PASS '}}
try{
 $manifest=Get-Content recovery/reference/public-codecs/PROVENANCE.json -Raw|ConvertFrom-Json
 foreach($file in $manifest.files){if((Get-FileHash $file.path).Hash.ToLowerInvariant() -ne $file.sha256 -or (git hash-object $file.path).Trim() -ne $file.blobSha){throw "Source fingerprint mismatch $($file.path)"}}
 $r.upstreamSources=$manifest
 $receipt=Get-Content recovery/evidence/texture/colour-state/40500150864-1/validation.json -Raw|ConvertFrom-Json
 if($receipt.status -ne 'passed' -or $receipt.sourceCommit -ne '2cee34462377beca4b16926eaf25d093daa02c2a'){throw 'Wrong colour state evidence'}
 foreach($f in $receipt.sourceFiles){if((Get-FileHash $f.path).Hash.ToLowerInvariant() -ne $f.sha256){throw 'Changed accepted diagnostic reference'}}
 $r.referenceEvidence=@{sourceCommit=$receipt.sourceCommit;runId=$receipt.runId}
 Expand-Archive recovery/evidence/texture/dxt-blocks/40500898688-1/transcripts.zip "$work/blocks"
 if((Get-FileHash "$work/blocks/Debug-transcript.txt").Hash.ToLowerInvariant() -ne '7150e0ce4b01c58e319aca053e87e60b00040344211b621c3fa10883716a245c'){throw 'Wrong original gold'}
 $asm=Get-Content src/TextureCompiler/Colour/TextureColour.asm -Raw
 $asm=$asm.Replace('.code',"EXTERN _S3LegacyTrace:PROC`n.code")
 function Tap([int]$stage){return @"
    pushfd
    pushad
    sub esp, 108
    fnsave [esp]
    lea eax, [esp + 144]
    push eax
    push $stage
    call _S3LegacyTrace
    add esp, 8
    frstor [esp]
    add esp, 108
    popad
    popfd
"@}
 $asm=$asm.Replace("Q_10061460:",("Q_10061460:`n"+(Tap 0))).Replace("Q_1007BAD8:",("Q_1007BAD8:`n"+(Tap 1)))
 $asm|Set-Content "$work/colour-trace.asm"
 $bridge=Get-Content src/TextureCompiler/Colour/TextureColourBridge.cpp -Raw
 $bridge=$bridge.Replace('#pragma comment(linker,"/NOENTRY")','').Replace('extern "C" int _fltused=0;','')
 $bridge|Set-Content "$work/colour-bridge.cpp"
 $diag=(Resolve-Path recovery/tests/TexturePublicS3Diagnostics.cpp).Path
 $squish=(Resolve-Path recovery/reference/public-codecs/squish).Path;$s3=(Resolve-Path recovery/reference/public-codecs/s3).Path
 $native=@((Resolve-Path recovery/tests/TexturePublicS3Bridge.cpp).Path,(Resolve-Path recovery/reference/public-codecs/s3/s3_quant.cpp).Path,(Resolve-Path recovery/tests/TexturePublicCodecBridge.cpp).Path,(Resolve-Path recovery/tests/TexturePublicAmdBridge.cpp).Path,(Resolve-Path recovery/reference/public-codecs/amd/compressonatorxcodec.cpp).Path,$diag)+@(Get-ChildItem "$squish/*.cpp"|ForEach-Object FullName)
 $includes=@('squish','stb','nvtt','amd','s3')|ForEach-Object{'/I'+(Resolve-Path "recovery/reference/public-codecs/$_").Path};$includes+=('/I'+(Resolve-Path recovery/tests/public-amd-compat).Path)
 Checked asm {& ml /nologo /c /coff /safeseh /Fo"$work/colour.obj" "$work/colour-trace.asm"}
 Checked reference {Push-Location $work;try{& cl /nologo /LD /MT /fp:strict /GS- /Od "$work/colour-bridge.cpp" $diag "$work/colour.obj" /link /OUT:"$work/TextureS3LegacyDiagnostic.dll" /MACHINE:X86}finally{Pop-Location}}
 Checked source {Push-Location $work;try{& cl /nologo /LD /MT /EHsc /std:c++17 /arch:SSE2 /fp:strict /Od /DS3_STAGE_TRACE @includes @native /link /OUT:"$work/TexturePublicCodecs.dll" /MACHINE:X86}finally{Pop-Location}}
 Get-ChildItem SDK/tools/*.dll|Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll'|Copy-Item -Destination $work
 $sources=@('recovery/tests/TexturePublicS3Diagnostics.cs','recovery/tests/TexturePublicCodecsReplay.cs','src/TextureCompiler/Managed/TexturePacker.cs','src/TextureCompiler/Managed/TextureNpotRgba8Resampler.cs','src/TextureCompiler/Managed/TextureNpotStorageResampler.cs')|ForEach-Object{(Resolve-Path $_).Path}
 Checked host {& "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe" /nologo /platform:x86 /main:TexturePublicS3Diagnostics /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.Utility.dll" /out:"$work/S3Diagnostics.exe" $sources}
 '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$work/S3Diagnostics.exe.config"
 Checked diagnose {& "$work/S3Diagnostics.exe" "$work/blocks/Debug-transcript.txt" "$out/stages"}
 $r.status='diagnostic-completed'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 16|Set-Content "$out/validation.json"}
