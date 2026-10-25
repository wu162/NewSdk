param([string]$Plan='recovery/texture-plan/plan.json')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path;Set-Location $root
$planData=Get-Content $Plan -Raw|ConvertFrom-Json
if($planData.sourceCommit -ne (git rev-parse HEAD).Trim()){throw 'Selection plan/source commit mismatch'}
$catalog=(Get-Content recovery/catalog/TextureTestGroups.json -Raw|ConvertFrom-Json).groups
$out="recovery/evidence/texture/batch/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
New-Item -ItemType Directory -Force $out|Out-Null
Copy-Item $Plan "$out/selection.json"
$r=@{sourceCommit=$planData.sourceCommit;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';selected=@($planData.selected);reused=@($planData.reused);groups=@()}
$failures=@()
try{
 foreach($name in $planData.selected){
  $spec=$catalog.PSObject.Properties[$name].Value
  if($null -eq $spec){throw "Unknown selected group $name"}
  $parameters=@{};foreach($property in $spec.arguments.PSObject.Properties){$parameters[$property.Name]=$property.Value}
  Write-Output "::group::Texture $name"
  try{
   $global:LASTEXITCODE=0
   & (Join-Path $root $spec.script) @parameters
   if($LASTEXITCODE -ne 0){throw "Native process exit $LASTEXITCODE"}
   $r.groups+=@{group=$name;passed=$true;evidenceOnly=($spec.evidence_only -eq $true)}
  }catch{
   $r.groups+=@{group=$name;passed=$false;evidenceOnly=($spec.evidence_only -eq $true);error=$_.Exception.Message};$failures+=$name
   Write-Output "::error::Texture $name failed: $($_.Exception.Message)"
  }finally{Write-Output '::endgroup::'}
 }
 $r.status=if($failures.Count -eq 0){'passed'}else{'failed'}
 if($failures.Count -ne 0){throw "Failed texture groups: $($failures -join ', ')"}
}finally{$r|ConvertTo-Json -Depth 15|Set-Content "$out/validation.json"}
