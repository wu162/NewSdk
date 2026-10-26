#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(30,600)][int]$FixtureTimeoutSeconds=180
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite Unicode replay evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$work=$null;$oldPath=$env:PATH
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function InvokeFixture([string]$Executable,[string[]]$Arguments,[string]$Directory,[string]$Log){
    $info=[Diagnostics.ProcessStartInfo]::new($Executable);$info.WorkingDirectory=$Directory;$info.UseShellExecute=$false
    $info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
    foreach($argument in $Arguments){$info.ArgumentList.Add($argument)}
    $process=[Diagnostics.Process]::new();$process.StartInfo=$info;$started=$false
    try {
        $started=$process.Start();Need $started 'Unable to start archived production-object validation host'
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        $finished=$process.WaitForExit($FixtureTimeoutSeconds*1000)
        if(!$finished){$process.Kill($true);Need ($process.WaitForExit(10000)) 'Timed-out validation host did not terminate'}
        Need ([Threading.Tasks.Task]::WaitAll([Threading.Tasks.Task[]]@($stdout,$stderr),10000)) 'Validation fixture log readers did not close'
        [IO.File]::WriteAllText($Log,($stdout.GetAwaiter().GetResult()+$stderr.GetAwaiter().GetResult()))
        if(!$finished -or $process.ExitCode -ne 0){Get-Content -LiteralPath $Log -Tail 65|Out-Host;throw "Fixture failed: timeout=$(!$finished) exit=$($process.ExitCode)"}
        return @{processId=$process.Id;exitCode=$process.ExitCode;timedOut=$false}
    } finally {if($started -and !$process.HasExited){try{$process.Kill($true);$null=$process.WaitForExit(5000)}catch{}};$process.Dispose()}
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=$null;testSourceCommit=$null;binarySourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Corrected UnicodeSettings and unchanged BackendLifecycle fixtures on exact archived production-object hosts, plus original BAB two-stream baselines. Other nine source Core scopes retain their original independent successful receipts. No Native linking, production rebuild, full Core rerun, or rewrite of the original failed run is claimed.'}
try {
    Need (!!$env:RUNNER_TEMP) 'A Windows runner temporary directory is required'
    $state.sourceCommit=(git -C $root rev-parse HEAD).Trim();Need ($LASTEXITCODE -eq 0) 'Cannot identify current test source'
    $state.testSourceCommit=$state.sourceCommit;$state.scriptSha256=Hash $PSCommandPath
    $guard=@'
import hashlib, json, pathlib, sys
root, candidate, product = sys.argv[1:]
sys.path.insert(0, str(pathlib.Path(root) / 'recovery/scripts'))
import PrepareCompilerSessionReplay as replay
changed = replay.closure(root, replay.SOURCE, candidate)
receipt = json.loads((pathlib.Path(product) / 'session-replay.json').read_text(encoding='utf-8-sig'))
assert receipt['prepared'] is True and receipt['candidateValidated'] is False
assert receipt['productionSourcesUnchanged'] is True and receipt['sourceRunConclusion'] == 'failure'
assert receipt['binarySourceCommit'] == replay.SOURCE and receipt['testSourceCommit'] == candidate
assert receipt['sourceCommit'] == replay.SOURCE and receipt['candidateCommit'] == candidate
assert str(receipt['runId']) == str(replay.RUN) and str(receipt['runAttempt']) == str(replay.ATTEMPT)
assert receipt['archiveSha256'] == replay.SHA256 and receipt['artifactId'] == replay.ARTIFACT
rows = receipt['validationFiles']
seen = set()
for row in rows:
    name = replay.guard.safe_name(row['path'])
    assert name == row['path'] and name not in seen
    seen.add(name)
    assert hashlib.sha256((pathlib.Path(product) / name).read_bytes()).hexdigest() == row['sha256']
for mode in ('Debug', 'Release'):
    for name in ('build.json','link-inputs.json','validation/build.json','validation/bin/NewModCompiler.exe','validation/bin/NewModCompiler.exe.config','validation/bin/antlr.runtime.dll'):
        assert mode + '/' + name in seen
print(json.dumps({'changedPaths': changed, 'receipt': receipt}))
'@
    $verified=& python -c $guard $root $state.sourceCommit $CompilerDirectory
    Need ($LASTEXITCODE -eq 0) 'Archived validation runtime or fixed source/archive closure rejected'
    $proof=$verified|ConvertFrom-Json;$state.archiveProof=$proof;$state.binarySourceCommit=$proof.receipt.binarySourceCommit
    $state.originalSourceRun=@{runId=$proof.receipt.runId;runAttempt=$proof.receipt.runAttempt;conclusion=$proof.receipt.sourceRunConclusion;failedSteps=$proof.receipt.sourceFailedSteps}
    $sdk=(Resolve-Path -LiteralPath (Join-Path $root 'SDK')).Path
    $state.fixtureInputs=@(foreach($name in @('tools/BinaryAssetBuilder.exe.config','Mods/mymod/Data/RecoveryArmorProbe.xml')){@{path=$name;sha256=(Hash (Join-Path $sdk $name))}})
    $schemaRoot=Join-Path $sdk 'Schemas/xsd'
    $state.schemaInputs=@(Get-ChildItem -LiteralPath $schemaRoot -File -Recurse|Sort-Object FullName|ForEach-Object{@{path=[IO.Path]::GetRelativePath($sdk,$_.FullName).Replace('\','/');sha256=(Hash $_.FullName)}})
    Need ($state.schemaInputs.Count -gt 0) 'SDK schema inventory is empty'
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    Need ($LASTEXITCODE -eq 0 -and !!$vs) 'Cannot find VS Roslyn compiler'
    $csc=Join-Path $vs 'MSBuild/Current/Bin/Roslyn/csc.exe'
    $refs=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
    $manifest=Get-Content (Join-Path $root 'recovery/build/Managed.Foundation.json') -Raw|ConvertFrom-Json
    $frameworkReferences=@(@($manifest.frameworkReferences)+@('System.Web.Extensions')|Select-Object -Unique)
    $state.compiler=@{path=$csc;sha256=(Hash $csc);version=([Diagnostics.FileVersionInfo]::GetVersionInfo($csc).FileVersion);languageVersion='7.3';target='net48/x86'}
    $state.frameworkReferences=@($frameworkReferences|ForEach-Object{@{name=$_;sha256=(Hash (Join-Path $refs ($_+'.dll')))}})
    $work=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('u'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
    $env:PATH=(@($oldPath -split ';'|Where-Object{$_ -notmatch '[\\/]SDK[\\/]tools'}) -join ';')
    foreach($mode in @('Debug','Release')){
        $modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $clean=Join-Path $work $mode.Substring(0,1)
        $run=Initialize-CompilerValidationRun (Join-Path $CompilerDirectory $mode) $clean $mode $state.binarySourceCommit
        $modeState=@{configuration=$mode;passed=$false;productExeSha256=$run.productExeSha256;validationHostSha256=$run.validationHostSha256;fixtures=@();originalBaselinePassed=$false};$state.modes+=,$modeState
        foreach($name in @('build.json','link-inputs.json','validation/build.json')){Copy-Item -LiteralPath (Join-Path $CompilerDirectory ($mode+'/'+$name)) -Destination (Join-Path $modeOut ($name.Replace('/','-')))}
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $run.exe *> (Join-Path $modeOut 'prepare-texture.log')
        Need ($LASTEXITCODE -eq 0) 'Archived validation-host Texture registration preparation failed'
        $texture=Join-Path $clean 'BinaryAssetBuilder.TextureCompiler.dll';$patchPath=$texture+'.patch.json';$patch=Get-Content -LiteralPath $patchPath -Raw|ConvertFrom-Json
        Need ($patch.targetExeSha256 -ceq $run.validationHostSha256 -and $patch.patchedSha256 -ceq (Hash $texture) -and $patch.originalSha256 -ceq '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -and $patch.protectedPeAudit.status -ceq 'passed' -and $patch.protectedPeAudit.mode -ceq 'redirect' -and $patch.protectedPeAudit.originalSha256 -ceq $patch.originalSha256 -and $patch.protectedPeAudit.outputSha256 -ceq $patch.patchedSha256) 'Validation Texture is not bound to the verified archived host'
        Copy-Item -LiteralPath $patchPath -Destination (Join-Path $modeOut 'texture.patch.json');$textureHash=Hash $texture;$patchHash=Hash $patchPath
        $modeState.textureDeployment=$patch
        foreach($fixture in @(@{name='CompilerUnicodeSettings';marker='PASS UnicodeSettings:'},@{name='CompilerBackendLifecycle';marker='PASS BackendLifecycle:'})){
            $fixtureOut=(New-Item -ItemType Directory -Path (Join-Path $modeOut $fixture.name)).FullName
            $fixtureWork=Join-Path $work ($mode.Substring(0,1)+'-'+$fixture.name)
            $source=(Resolve-Path -LiteralPath (Join-Path $root ('recovery/tests/'+$fixture.name+'.cs'))).Path;$sourceHash=Hash $source
            $fixtureExe=Join-Path $clean ($fixture.name+'.exe')
            $arguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86',('/out:'+$fixtureExe),('/r:'+$run.exe),('/r:'+(Join-Path $clean 'antlr.runtime.dll')))
            $arguments+=$(if($mode -ceq 'Release'){'/optimize+'}else{'/debug:full'})
            foreach($name in $frameworkReferences){$arguments+=('/r:'+(Join-Path $refs ($name+'.dll')))};$arguments+=$source
            $fixtureState=@{name=$fixture.name;passed=$false;testSourceCommit=$state.testSourceCommit;binarySourceCommit=$state.binarySourceCommit;sourceSha256=$sourceHash};$modeState.fixtures+=,$fixtureState
            try {
                & $csc @arguments *> (Join-Path $fixtureOut 'fixture-build.log')
                if($LASTEXITCODE -ne 0){Get-Content -LiteralPath (Join-Path $fixtureOut 'fixture-build.log') -Tail 65|Out-Host;throw 'Replay fixture compilation failed'}
                Copy-Item -LiteralPath ($run.exe+'.config') -Destination ($fixtureExe+'.config')
                Copy-Item -LiteralPath $fixtureExe -Destination (Join-Path $fixtureOut 'fixture.exe');Copy-Item -LiteralPath ($fixtureExe+'.config') -Destination (Join-Path $fixtureOut 'fixture.exe.config')
                $fixtureHash=Hash $fixtureExe;$resultPath=Join-Path $fixtureOut 'result.json';$logPath=Join-Path $fixtureOut 'runtime.log'
                $fixtureState.process=InvokeFixture $run.exe @($fixtureExe,$fixtureWork,$resultPath,$state.testSourceCommit,$state.runId,$sdk) $clean $logPath
                Need (Select-String -LiteralPath $logPath -SimpleMatch $fixture.marker -Quiet) 'Replay fixture success marker missing'
                $result=Get-Content -LiteralPath $resultPath -Raw|ConvertFrom-Json
                Need ($result.succeeded -and $result.sourceCommit -ceq $state.testSourceCommit -and $result.runId -ceq $state.runId -and $result.checks -gt 0 -and @($result.tests).Count -gt 0) 'Replay fixture result identity/coverage differs'
                Need ((Hash $fixtureExe) -ceq $fixtureHash -and (Hash $source) -ceq $sourceHash -and (Hash $texture) -ceq $textureHash -and (Hash $patchPath) -ceq $patchHash) 'Replay fixture/runtime input changed'
                Complete-CompilerValidationRun $run
                $fixtureState.fixtureExeSha256=$fixtureHash;$fixtureState.resultSha256=Hash $resultPath;$fixtureState.runtimeLogSha256=Hash $logPath;$fixtureState.passed=$true
                if($fixture.name -ceq 'CompilerBackendLifecycle'){
                    & "$PSScriptRoot/TestCompilerBackendBaseline.ps1" -FixtureResult $resultPath -OutputDirectory (Join-Path $modeOut 'original-baseline') -TimeoutSeconds $FixtureTimeoutSeconds
                    Need ($LASTEXITCODE -eq 0) 'Replayed backend original BAB raw differential failed'
                    $baseline=Get-Content (Join-Path $modeOut 'original-baseline/validation.json') -Raw|ConvertFrom-Json
                    Need ($baseline.succeeded -and @($baseline.cases).Count -eq 2 -and @($baseline.cases|Where-Object{$_.passed -ne $true}).Count -eq 0) 'Original BAB baseline result incomplete'
                    $modeState.originalBaselinePassed=$true
                }
            } catch {$fixtureState.error=$_.Exception.ToString();throw}
            finally {if(Test-Path -LiteralPath $fixtureWork){Copy-Item -LiteralPath $fixtureWork -Destination (Join-Path $fixtureOut 'work') -Recurse}}
        }
        Complete-CompilerValidationRun $run;$modeState.passed=$true
    }
    foreach($inputFile in @($state.fixtureInputs)+@($state.schemaInputs)){Need ((Hash (Join-Path $sdk $inputFile.path)) -ceq $inputFile.sha256) 'SDK input changed during Unicode/backend replay'}
    $state.succeeded=$true;$global:LASTEXITCODE=0
} catch {$state.error=$_.Exception.ToString();throw}
finally {
    $env:PATH=$oldPath
    if($work -and (Test-Path -LiteralPath $work)){try{Remove-Item -LiteralPath $work -Recurse -Force;$state.temporaryRootRemoved=$true}catch{$state.temporaryRootRemoved=$false;$state.cleanupError=$_.Exception.ToString()}}
    $state|ConvertTo-Json -Depth 20|Set-Content (Join-Path $out 'validation.json')
}
