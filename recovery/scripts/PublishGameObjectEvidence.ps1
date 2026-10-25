$ErrorActionPreference = 'Stop'
# Validation output stays untouched in the tested checkout. Publish only the
# immutable run directory from a fresh detached worktree, so concurrent runs
# never rebase competing changes to the shared oracle/validation.json files.
$runKey = "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if ($runKey -notmatch '^\d+-\d+$') { throw 'Invalid workflow run/attempt identity' }
$source = (Resolve-Path 'recovery/evidence/gameobject-root').Path
$publish = Join-Path $env:RUNNER_TEMP "ra3-gameobject-evidence-$runKey"
$relative = "recovery/evidence/gameobject-runs/$runKey"
git config user.name 'github-actions[bot]'
git config user.email '41898282+github-actions[bot]@users.noreply.github.com'
git fetch origin recovery/compiler-source
if ($LASTEXITCODE -ne 0) { throw 'Evidence fetch failed' }
git worktree add --detach $publish origin/recovery/compiler-source
if ($LASTEXITCODE -ne 0) { throw 'Evidence publishing worktree failed' }
$destination = Join-Path $publish $relative
New-Item -ItemType Directory -Force $destination | Out-Null
Copy-Item (Join-Path $source '*') $destination -Recurse -Force
git -C $publish add -- $relative
if ($LASTEXITCODE -ne 0) { throw 'Unable to stage immutable run evidence' }
git -C $publish diff --cached --quiet
if ($LASTEXITCODE -eq 0) { exit 0 }
if ($LASTEXITCODE -ne 1) { throw 'Unable to inspect immutable run evidence' }
git -C $publish commit -m "recovery: archive GameObject validation $runKey for $env:GITHUB_SHA"
if ($LASTEXITCODE -ne 0) { throw 'Immutable evidence commit failed' }
for ($attempt = 1; $attempt -le 3; ++$attempt) {
    # Only a run-specific directory changed, so other runs' archives are disjoint.
    git -C $publish pull --rebase origin recovery/compiler-source
    if ($LASTEXITCODE -ne 0) { throw 'Immutable evidence rebase failed' }
    git -C $publish push origin HEAD:recovery/compiler-source
    if ($LASTEXITCODE -eq 0) { Write-Host "Saved immutable evidence: $relative"; exit 0 }
    if ($attempt -eq 3) { throw 'Immutable evidence push failed after three ordinary attempts' }
}
