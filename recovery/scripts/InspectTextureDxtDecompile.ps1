$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-decompile/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('dxt-decompile-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='Original native decompilation evidence only; not recovery acceptance'}
try {
 $r.originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll).Hash.ToLowerInvariant()
 if($r.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original DLL mismatch'}
 $r.ghidraVersion='11.4.2';$r.toolSha256='795a02076af16257bd6f3f4736c4fc152ce9ff1f95df35cd47e2adc086e037a6'
 $zip=Join-Path $work ghidra.zip
 Invoke-WebRequest 'https://github.com/NationalSecurityAgency/ghidra/releases/download/Ghidra_11.4.2_build/ghidra_11.4.2_PUBLIC_20250826.zip' -OutFile $zip
 if((Get-FileHash $zip).Hash.ToLowerInvariant() -ne $r.toolSha256){throw 'Ghidra checksum mismatch'}
 Expand-Archive $zip -DestinationPath $work
 if(!$env:JAVA_HOME_21_X64){throw 'JDK21 missing'}
 $env:JAVA_HOME=$env:JAVA_HOME_21_X64
 New-Item -ItemType Directory -Force "$work/project"|Out-Null
 $headless=Join-Path $work 'ghidra_11.4.2_PUBLIC/support/analyzeHeadless.bat'
 & $headless "$work/project" Texture -import "$root/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll" -processor 'x86:LE:32:default' -analysisTimeoutPerFile 240 -scriptPath "$root/recovery/scripts/ghidra" -postScript ExportTextureDxt.java "$out" -deleteProject *> "$out/ghidra.log"
 if($LASTEXITCODE -ne 0){Get-Content "$out/ghidra.log" -Tail 40;throw 'Ghidra export failed'}
 $files=@(Get-ChildItem "$out/*.c")
 if($files.Count -ne 18){Get-Content "$out/ghidra.log" -Tail 40;throw 'Missing native function exports'}
 $r.exports=@($files|ForEach-Object{@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}})
 $r.status='probe-only'
}catch{$r.status='failed';$r.error=$_.Exception.Message;throw}finally{$r|ConvertTo-Json -Depth 10|Set-Content "$out/validation.json"}
