param([Parameter(Mandatory=$true)][string]$ReceiptPath)
$ErrorActionPreference='Stop'
$r=Get-Content $ReceiptPath -Raw|ConvertFrom-Json
if($r.status -ne 'installed'){throw 'An installed package receipt is required'}
$tools=Join-Path (Resolve-Path $r.sdkDirectory).Path 'tools'
$backup=(Resolve-Path $r.backupDirectory).Path
$allowed=@('BinaryAssetBuilder.exe.config','Recovered.BinaryAssetBuilder.TextureCompiler.dll','Recovered.TextureDxtAlpha.dll','Recovered.TextureDxtColour.dll','Recovered.TextureJpeg.dll','Recovered.TexturePpm.dll','Recovered.TexturePublicS3.dll')
if($r.files.Count -ne $allowed.Count -or @((Compare-Object ($allowed|Sort-Object) @($r.files.file|Sort-Object))).Count -ne 0){throw 'Unexpected restore file set'}
# Preflight every file before touching any of them. A later user edit must not
# be silently overwritten by a package rollback.
foreach($file in $r.files){
 $path=Join-Path $tools $file.file
 if(-not(Test-Path $path) -or (Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.installedSha256){throw "Installed file changed after deployment: $($file.file)"}
 if($file.existed -and (Get-FileHash "$backup/$($file.file)").Hash.ToLowerInvariant() -ne $file.beforeSha256){throw "Backup fingerprint mismatch: $($file.file)"}
}
foreach($file in $r.files){
 $path=Join-Path $tools $file.file
 if($file.existed){Copy-Item "$backup/$($file.file)" $path -Force}else{Remove-Item $path -Force}
}
foreach($file in $r.files){
 $path=Join-Path $tools $file.file
 if($file.existed){if((Get-FileHash $path).Hash.ToLowerInvariant() -ne $file.beforeSha256){throw "Restore fingerprint mismatch: $($file.file)"}}
 elseif(Test-Path $path){throw "New runtime file survived rollback: $($file.file)"}
}
$r.status='restored';$r|ConvertTo-Json -Depth 8|Set-Content "$backup/restoration.json"
Write-Output "PASS TextureCompiler package restore: all $($allowed.Count) prior file states restored; SDK=$($r.sdkDirectory)"
