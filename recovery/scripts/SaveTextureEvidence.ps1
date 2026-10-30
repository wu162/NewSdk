param([Parameter(Mandatory=$true)][string]$Path,[Parameter(Mandatory=$true)][string]$Message,[string[]]$AdditionalPaths=@())
$ErrorActionPreference='Stop'
# The uploaded Actions artifact retains raw outputs. Before staging only this
# run's directories, round-trip oversized SDK products into a lossless archive;
# never rewrite old gold, receipt hashes or historical checkout attributes.
if($env:GITHUB_RUN_ID -and $env:GITHUB_RUN_ATTEMPT){
 $runDirectory="$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
 foreach($directory in Get-ChildItem $Path -Directory -Recurse|Where-Object Name -eq $runDirectory){
  python "$PSScriptRoot/PackTextureSdkProducts.py" $directory.FullName
  if($LASTEXITCODE -ne 0){throw "SDK evidence product storage failed: $($directory.FullName)"}
  python "$PSScriptRoot/PackTextureCaptures.py" $directory.FullName
  if($LASTEXITCODE -ne 0){throw "Capture evidence storage failed: $($directory.FullName)"}
 }
}
git config user.name 'github-actions[bot]'
git config user.email '41898282+github-actions[bot]@users.noreply.github.com'
git add -- $Path @AdditionalPaths
if ($LASTEXITCODE -ne 0) { throw 'Evidence staging failed' }
git diff --cached --quiet
if ($LASTEXITCODE -eq 0) { exit 0 }
if ($LASTEXITCODE -ne 1) { throw 'Evidence diff failed' }
git commit -m $Message
if ($LASTEXITCODE -ne 0) { throw 'Evidence commit failed' }
# Several independent groups may finish together. Rebase and retry a rejected
# ordinary push; never overwrite the branch or rerun successful comparisons.
for ($attempt=1; $attempt -le 6; $attempt++) {
    git pull --rebase origin $env:GITHUB_REF_NAME
    if ($LASTEXITCODE -ne 0) { throw 'Evidence rebase conflict requires resolution' }
    git push origin "HEAD:$env:GITHUB_REF_NAME"
    if ($LASTEXITCODE -eq 0) { exit 0 }
    if ($attempt -lt 6) { Start-Sleep -Seconds $attempt }
}
throw 'Evidence ordinary push failed after six attempts; artifact remains available'
