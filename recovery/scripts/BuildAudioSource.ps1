param()
$ErrorActionPreference='Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if(!$env:RUNNER_TEMP){$env:RUNNER_TEMP=[IO.Path]::GetTempPath()}
$out="recovery/evidence/audio/standalone-build-validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
if(Test-Path $out){throw 'Refusing to overwrite standalone audio build receipt'}
New-Item -ItemType Directory $out -Force|Out-Null
$out=(Resolve-Path $out).Path
$work=Join-Path $env:RUNNER_TEMP ('audio-standalone-build-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$state=@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='Whole recovered AudioCompiler source project builds only';sdkStarted=$false;modCompiled=$false;supportedPlatforms=@('Win32');sourceTree=(git rev-parse HEAD:recovery/src/Managed/Audio).Trim();coreReferenceSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.Core.dll).Hash.ToLowerInvariant();modes=@()}
try{
 foreach($mode in @('Debug','Release')){
  $destination=Join-Path $work $mode
  & dotnet build recovery/src/Managed/Audio/BinaryAssetBuilder.AudioCompiler.csproj --configuration $mode --output $destination --nologo *> "$out/$mode-build.log"
  if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-build.log" -Tail 80;throw "$mode whole source project build failed"}
  $library=Join-Path $destination 'BinaryAssetBuilder.AudioCompiler.dll'
  if(!(Test-Path $library)){throw "$mode output library missing"}
  $state.modes+=@{configuration=$mode;passed=$true;librarySha256=(Get-FileHash $library).Hash.ToLowerInvariant();libraryBytes=(Get-Item $library).Length;log="$mode-build.log"}
  Write-Host "PASS whole AudioCompiler source project $mode build"
 }
 $state.status='passed'
}catch{$state.status='failed';$state.error=$_.Exception.Message;throw}
finally{$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json"}
