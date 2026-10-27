param([Parameter(Mandatory=$true)][string]$Destination,[Parameter(Mandatory=$true)][string]$Extraction)
$ErrorActionPreference='Stop'
$evidence='recovery/evidence/texture/npot-storage/40592911746-1'
$receipt=Get-Content "$evidence/validation.json" -Raw|ConvertFrom-Json
if($receipt.status -ne 'probe-only' -or $receipt.sourceCommit -ne 'c6459c7f0d2a88d3680e882bb0713bc95cb985a5' -or $receipt.fixtureCount -ne 4986 -or $receipt.decodedCount -ne 4446 -or $receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -or (Get-FileHash "$evidence/samples.zip").Hash.ToLowerInvariant() -ne $receipt.samplesSha256){throw 'NPOT storage corpus mismatch'}
git merge-base --is-ancestor $receipt.sourceCommit HEAD;if($LASTEXITCODE -ne 0){throw 'Storage baseline source is not an ancestor'}
Expand-Archive "$evidence/samples.zip" $Extraction
New-Item -ItemType Directory -Force $Destination|Out-Null
foreach($row in Import-Csv "$Extraction/transcript.tsv" -Delimiter "`t"){
 if($row.error -eq '' -and ("$($row.sourceWidth)x$($row.sourceHeight)" -in @('3x2','7x5')) -and $row.pattern -in @('0','2') -and $row.metadata -eq '0' -and ($row.profile.EndsWith('-i0') -or $row.profile.EndsWith('-rleFalse-dir0') -or $row.profile.EndsWith('-topFalse'))){
  $ext=if($row.profile.StartsWith('tga-')){'tga'}elseif($row.profile.StartsWith('png-')){'png'}else{'bmp'}
  Copy-Item "$Extraction/$($row.label).$ext" $Destination
 }
}
if(@(Get-ChildItem $Destination -File).Count -ne 64){throw 'Expected 64 complete storage-profile inputs'}
