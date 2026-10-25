$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$out="recovery/evidence/texture/dxt-library/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work=Join-Path $env:RUNNER_TEMP ('nvdxt-symbols-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out,$work|Out-Null;$out=(Resolve-Path $out).Path
$r=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';probeOnly=$true;evidenceOnly=$true;sdkValidated=$false;scope='Reference static-library COFF symbol inventory only; no DXT recovery/equivalence claim'}
try{
 $originalHash=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll).Hash.ToLowerInvariant()
 if($originalHash -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Original DLL changed'}
 $r.originalSha256=$originalHash
 $r.referenceRepository='Stehfyn/Legacy_DDS_Utilities';$r.referenceCommit='f0467589621354150acf2c623cb5cad44c8f35c1'
 $r.referencePath='LIBS/lib/release/nvDXTlibMT.vc8.lib'
 $expectedBlob='48d8c74d9ca04798246242c4be3bf459ddcbbdfc'
 $uri="https://raw.githubusercontent.com/$($r.referenceRepository)/$($r.referenceCommit)/$($r.referencePath)"
 $lib=Join-Path $work 'nvDXTlibMT.vc8.lib'
 Invoke-WebRequest $uri -OutFile $lib
 $blob=(git hash-object -- $lib).Trim()
 if($LASTEXITCODE -ne 0 -or $blob -ne $expectedBlob){throw 'Reference library Git blob mismatch'}
 $r.referenceGitBlob=$blob;$r.referenceBytes=(Get-Item $lib).Length;$r.referenceSha256=(Get-FileHash $lib).Hash.ToLowerInvariant()
 $vswhere=Join-Path ([Environment]::GetFolderPath('ProgramFilesX86')) 'Microsoft Visual Studio/Installer/vswhere.exe'
 $vs=(& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath|Select-Object -First 1)
 if(!$vs){throw 'MSVC tools unavailable'}
 $dump=Get-ChildItem "$vs/VC/Tools/MSVC/*/bin/Hostx64/x86/dumpbin.exe"|Sort-Object FullName -Descending|Select-Object -First 1
 if(!$dump){throw 'dumpbin unavailable'}
 $r.dumpbinVersion=$dump.VersionInfo.FileVersion
 & $dump.FullName /nologo /linkermember:2 $lib *> "$out/linker-members.txt"
 if($LASTEXITCODE -ne 0){throw 'COFF linker-member dump failed'}
 & $dump.FullName /nologo /symbols $lib *> "$out/object-symbols.txt"
 if($LASTEXITCODE -ne 0){throw 'COFF object-symbol dump failed'}
 $selected=@(Get-Content "$out/linker-members.txt"|Where-Object{$_ -match 'nvDXTcompress|dxt_compress|JAMA|Eigen|DXT|Cluster|Compress|Principal|rgba_t|Palette|Optimize|endpoint'})
 if($selected.Count -eq 0){throw 'No compressor reference symbols found'}
 $selected|Set-Content "$out/selected-symbols.txt"
 $r.selectedSymbolLines=$selected.Count
 $methods=Get-Content recovery/evidence/texture/runs/40191971356-1/methods.json -Raw|ConvertFrom-Json
 $r.originalEntrypoints=@($methods|Where-Object{$_.token -in @('0x060000DB','0x060000DC','0x060000FB','0x06000101','0x06000102')}|Select-Object token,name,rva,kind,signature)
 if($r.originalEntrypoints.Count -ne 5){throw 'Original entrypoint inventory missing'}
 Compress-Archive -Path "$out/linker-members.txt","$out/object-symbols.txt" -DestinationPath "$out/coff-symbols.zip"
 $r.symbolArchiveSha256=(Get-FileHash "$out/coff-symbols.zip").Hash.ToLowerInvariant()
 Remove-Item "$out/linker-members.txt","$out/object-symbols.txt"
 $r.status='probe-only'
 Write-Output "PROBE Texture DXT reference symbols=$($selected.Count) bytes=$($r.referenceBytes); no original/reference binary equivalence established"
}finally{$r|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"}
