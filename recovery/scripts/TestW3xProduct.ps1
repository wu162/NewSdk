[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CaptureDirectory,
    [Parameter(Mandatory=$true)][string]$PluginValidation,
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$captures=(Resolve-Path $CaptureDirectory).Path
$pluginReceiptPath=(Resolve-Path $PluginValidation).Path
$compiler=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite W3X product evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory $OutputDirectory).FullName
$work=Join-Path ([IO.Path]::GetTempPath()) ('w3x-product-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work|Out-Null
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';configurations=@();scope='complete captured W3X ProcessInstance matrix replay against product EXE; SDK not run'}
function Invoke-Checked([string]$Command,[string[]]$Arguments,[string]$Log){
    & $Command @Arguments *> $Log
    if($LASTEXITCODE -ne 0){Get-Content $Log -Tail 80|Write-Host;throw "W3X product check failed: $Command"}
}
Push-Location $root
try {
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $component=Get-Content "$captures/validation.json" -Raw|ConvertFrom-Json
    $pluginReceipt=Get-Content $pluginReceiptPath -Raw|ConvertFrom-Json
    if(!$component.succeeded -or !$component.completeAnimationBuffersValidated -or !$component.completeMeshBuffersValidated -or !$component.componentWithoutOriginalReplay){throw 'A successful complete W3X capture receipt is required'}
    if(!$pluginReceipt.succeeded -or !$pluginReceipt.processInstanceOriginalDifferential -or !$pluginReceipt.withoutOriginalReplay -or $pluginReceipt.sourceCommit -cne $component.sourceCommit -or [string]$pluginReceipt.runId -cne [string]$component.runId -or [string]$pluginReceipt.runAttempt -cne [string]$component.runAttempt){throw 'Plugin capture fingerprints require the matching successful original differential receipt'}
    if($component.sourceCommit -notmatch '^[0-9a-f]{40}$' -or [string]$component.runId -notmatch '^\d+$' -or [string]$component.runAttempt -notmatch '^[1-9]\d*$'){throw 'Missing capture source/run identity'}
    & git merge-base --is-ancestor $component.sourceCommit HEAD
    if($LASTEXITCODE -ne 0){throw 'Capture source is not an available ancestor of this checkout'}
    $oracleInputs=@('recovery/tests/W3xAnimationOracle.cs','recovery/tests/W3xMeshAssetOracle.cs','SDK/tools')
    & git diff --quiet $component.sourceCommit HEAD -- @oracleInputs
    if($LASTEXITCODE -ne 0){throw 'Original capture fixture generators or SDK oracle inputs changed; existing capture cannot be reused'}
    $original=(Get-FileHash 'SDK/tools/BinaryAssetBuilder.W3XCompiler.dll').Hash.ToLowerInvariant()
    if($original -cne 'bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1' -or $component.originalSha256 -cne $original -or $pluginReceipt.originalSha256 -cne $original){throw 'W3X original identity mismatch'}
    # Check the union of both revisions' local-include closures, including all
    # shared native implementations. Unrelated XML/Tokenizer additions do not
    # change W3X evidence, but changed/deleted transitive headers still reject it.
    & python "$PSScriptRoot/CheckW3xCaptureInputs.py" $component.sourceCommit $state.sourceCommit *> "$out/reused-inputs.json"
    if($LASTEXITCODE -ne 0){Get-Content "$out/reused-inputs.json" -Tail 80|Write-Host;throw 'W3X implementation/shared headers or independent matrix inputs changed; review component evidence before reusing captures'}
    $state.lowLevelNativeMatrixRerun=$false
    $state.lowLevelEvidenceScope='unchanged implementation and tests; historical component toolchain only, product build mode covered by new captured ProcessInstance replay'
    $state.reusedCaptureSourceCommit=$component.sourceCommit
    $state.reusedCaptureRunId=$component.runId
    $state.reusedCaptureRunAttempt=$component.runAttempt
    $state.reusedInputsSha256=(Get-FileHash "$out/reused-inputs.json").Hash.ToLowerInvariant()
    $state.componentReceiptSha256=(Get-FileHash "$captures/validation.json").Hash.ToLowerInvariant()
    $state.pluginReceiptSha256=(Get-FileHash $pluginReceiptPath).Hash.ToLowerInvariant()
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    foreach($configuration in @('Debug','Release')){
        $mode=if($configuration -eq 'Debug'){'Od'}else{'O2'}
        $modeReceipt=@($pluginReceipt.modes|Where-Object{$_.mode -ceq $mode})
        $componentMode=@($component.modes|Where-Object{$_.mode -ceq $mode})
        if($modeReceipt.Count -ne 1 -or $componentMode.Count -ne 1 -or !$modeReceipt[0].succeeded -or !$modeReceipt[0].processInstanceOriginalDifferential -or !$modeReceipt[0].withoutOriginalReplay -or !$componentMode[0].animationThreeBufferOriginalDifferential -or !$componentMode[0].completeMeshThreeBufferOriginalDifferential -or !$componentMode[0].componentWithoutOriginalReplay){throw "Incomplete $mode capture verification"}
        $animation=(Resolve-Path "$captures/$mode-animation.fixtures").Path
        $mesh=(Resolve-Path "$captures/$mode-mesh.fixtures").Path
        if((Get-FileHash $animation).Hash.ToLowerInvariant() -cne $modeReceipt[0].animationCaptureSha256 -or (Get-FileHash $mesh).Hash.ToLowerInvariant() -cne $modeReceipt[0].meshCaptureSha256 -or $modeReceipt[0].animationFixtures -ne 675 -or $modeReceipt[0].meshFixtures -ne 666){throw 'Captured W3X bytes/counts differ from successful original plugin differential'}
        $build=Join-Path $compiler $configuration
        $buildReceipt=Get-Content "$build/build.json" -Raw|ConvertFrom-Json
        $sourceExe=(Resolve-Path "$build/bin/NewModCompiler.exe").Path
        if($buildReceipt.sourceCommit -cne $state.sourceCommit -or $buildReceipt.configuration -cne $configuration -or $buildReceipt.exeSha256 -cne (Get-FileHash $sourceExe).Hash.ToLowerInvariant()){throw 'Product build does not match requested source/configuration'}
        $bin=(New-Item -ItemType Directory "$work/$configuration").FullName
        $evidence=(New-Item -ItemType Directory "$out/$configuration").FullName
        $validationRun=Initialize-CompilerValidationRun $build $bin $configuration $state.sourceCommit
        # No original/recovered business DLL, build module, SDK directory, or resolver is available.
        $hostExe=Join-Path $bin 'W3xProductReplay.exe'
        $opt=if($configuration -eq 'Release'){'/optimize+'}else{'/optimize-'}
        Invoke-Checked $csc @('/nologo','/codepage:65001','/platform:x86',$opt,'/define:MONOLITHIC_REPLAY','/r:System.Xml.dll',("/r:"+$bin+'/NewModCompiler.exe'),("/out:"+$hostExe),(Join-Path $root 'recovery/tests/W3xPluginOracle.cs')) "$evidence/replay-build.log"
        Copy-Item "$sourceExe.config" "$hostExe.config"
        Invoke-Checked $validationRun.exe @($hostExe,'standalone',$animation,$mesh) "$evidence/replay.log"
        Get-Content "$evidence/replay.log"|Where-Object{$_ -match '^PASS '}|Write-Host
        $metadataExe=Join-Path $bin 'W3xManagedStandalone.exe'
        Invoke-Checked $csc @('/nologo','/codepage:65001','/platform:x86',$opt,'/r:System.Core.dll',("/r:"+$bin+'/NewModCompiler.exe'),("/out:"+$metadataExe),(Join-Path $root 'recovery/tests/W3xManagedStandalone.cs')) "$evidence/metadata-build.log"
        Copy-Item "$sourceExe.config" "$metadataExe.config"
        Invoke-Checked $validationRun.exe @($metadataExe) "$evidence/metadata.log"
        Complete-CompilerValidationRun $validationRun
        $state.configurations+=@{validationHostSha256=$validationRun.validationHostSha256;configuration=$configuration;passed=$true;animationFixtures=675;meshFixtures=666;threeBufferComparisons=4029;exeSha256=$buildReceipt.exeSha256;animationCaptureSha256=$modeReceipt[0].animationCaptureSha256;meshCaptureSha256=$modeReceipt[0].meshCaptureSha256;businessDllsAbsent=$true;productMetadataPlatforms=@(0,1,2,99)}
    }
    $state.status='passed'
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json" -Encoding utf8;Pop-Location}

