$ErrorActionPreference='Stop'
& "$PSScriptRoot/InspectTexturePublicBc.ps1" -RequireMatch -EvidenceGroup native-bc-files
if($LASTEXITCODE -ne 0){throw 'Public BC file consumer final matrix failed'}
