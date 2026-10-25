param([Parameter(Mandatory=$true)][string]$Path,[Parameter(Mandatory=$true)][string]$Message)
$ErrorActionPreference='Stop'
git config user.name 'github-actions[bot]'
git config user.email '41898282+github-actions[bot]@users.noreply.github.com'
git add -- $Path
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
