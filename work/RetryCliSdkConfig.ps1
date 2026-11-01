# Resume only the fixture stage with verified, unchanged Windows binaries.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$SourceCommit,
    [Parameter(Mandatory=$true)][string]$CandidateRuntime,
    [Parameter(Mandatory=$true)][string]$BaselineRuntime,
    [Parameter(Mandatory=$true)][string]$CandidateEvidence,
    [string]$BaselineCommit='9fd3557eea3a33479a99f22872b2034b11b74c2b'
)
$ErrorActionPreference='Stop'
$repo=(Get-Location).Path
$out=Join-Path $repo 'recovery/artifacts/cli-simplification'
$work=Join-Path $env:RUNNER_TEMP 'cli-simplification'
New-Item -ItemType Directory -Force $out,$work|Out-Null
function Need([bool]$Condition,[string]$Message){if(!$Condition){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Inventory([string]$Directory){
    $result=@{}
    foreach($file in @(Get-ChildItem -LiteralPath $Directory -Recurse -File)){
        $result[[IO.Path]::GetRelativePath($Directory,$file.FullName).Replace('\','/')]=@{sha256=(Hash $file.FullName);bytes=$file.Length}
    }
    return $result
}
function Same([hashtable]$Before,[hashtable]$After,[string]$Name){
    Need ($Before.Count -eq $After.Count) "$Name file count differs"
    foreach($key in $Before.Keys){Need ($After.ContainsKey($key) -and $Before[$key].sha256 -ceq $After[$key].sha256 -and $Before[$key].bytes -eq $After[$key].bytes) "$Name raw file differs: $key"}
}
$state=@{baselineCommit=$BaselineCommit;sourceCommit=$SourceCommit;validationCommit=$env:GITHUB_SHA;runId=$env:GITHUB_RUN_ID;succeeded=$false;cases=@();comparisons=@();scope='Reuse unchanged binaries, Release App1603 success, and first7 proven fixture cases; recheck archived outputs/reports, then retry only two SDK buildCache cases with sessionCache disabled to observe actual Core buildCache state; no rebuild or benchmark';candidateArtifact=@{id=13069274015;sha256='df837c63493bb0df8bd973d2e502c23c1c2b5ad796cae88f98a4f3c933b46b49'};baselineArtifact=@{id=13069292067;sha256='8eaa2b2b713ce8939623a84020e4112dad4a8873e85351d1ede879f65a70a696'}}
try {
    $candidateBuildCommit='5c79cb57feb93314d388936b84e85c46e02a5599'
    $baselineBuildCommit='1f0cea03083f2219e43fa35f3dd44df73158310e'
    foreach($pair in @(@($candidateBuildCommit,$SourceCommit),@($baselineBuildCommit,$BaselineCommit))){
        & git diff --quiet $pair[0] $pair[1] -- recovery/src recovery/build recovery/scripts recovery/probes SDK
        Need ($LASTEXITCODE -eq 0) ('Runtime production input closure differs: '+$pair[0]+' to '+$pair[1])
    }
    $changed=@(git diff --name-only $BaselineCommit $SourceCommit -- recovery/src recovery/build recovery/scripts recovery/probes SDK)
    Need ($LASTEXITCODE -eq 0) 'Cannot compare baseline/candidate production closure'
    Need (@($changed|Where-Object{$_ -notlike 'recovery/src/App/*'}).Count -eq 0) 'Non-App production inputs differ between baseline and candidate'
    $state.changedProductionInputs=$changed
    $prior=Get-Content (Join-Path $CandidateEvidence 'prior-validation.json') -Raw|ConvertFrom-Json
    Need ($prior.runId -ceq '41271617644' -and $prior.sourceCommit -ceq $SourceCommit -and $prior.validationCommit -ceq $candidateBuildCommit -and $prior.appContractsPassed -and !$prior.succeeded -and $prior.error.Contains('Baseline production relink failed')) 'Prior successful App/build stages do not match known harness failure'
    $appLog=Join-Path $CandidateEvidence 'app/test-app.log'
    Need (([IO.File]::ReadAllText($appLog)).Contains('PASS App contract assertions=1603;')) 'Prior Release App success log is missing'
    $app=Join-Path $out 'app';New-Item -ItemType Directory -Force $app|Out-Null
    Copy-Item -LiteralPath $appLog -Destination "$app/test-app.log"
    Copy-Item -LiteralPath (Join-Path $CandidateEvidence 'prior-validation.json') -Destination (Join-Path $out 'prior-build-validation.json')
    $state.appContractsPassed=$true;$state.appContractAssertions=1603;$state.appContractsRunId='41271617644';$state.appContractsReused=$true
    $product=Join-Path $out 'product';$baseline=Join-Path $out 'baseline'
    Copy-Item -LiteralPath $CandidateRuntime -Destination $product -Recurse
    Copy-Item -LiteralPath $BaselineRuntime -Destination $baseline -Recurse
    $build=Get-Content "$product/build.json" -Raw|ConvertFrom-Json
    $baselineBuild=Get-Content "$baseline/build.json" -Raw|ConvertFrom-Json
    Need ($build.sourceCommit -ceq $candidateBuildCommit -and $build.configuration -ceq 'Release' -and $build.backendIntegrated -and $build.exeSha256 -ceq $prior.candidateExeSha256) 'Candidate build receipt mismatch'
    Need ($baselineBuild.sourceCommit -ceq $baselineBuildCommit -and $baselineBuild.configuration -ceq 'Release' -and $baselineBuild.backendIntegrated) 'Accepted baseline build receipt mismatch'
    $acceptedText=@(git show ($SourceCommit+':recovery/evidence/joypad/41270490335-1/summary.json'))
    Need ($LASTEXITCODE -eq 0) 'Accepted baseline evidence is unavailable in implementation commit'
    $accepted=($acceptedText -join "`n")|ConvertFrom-Json
    Need ($accepted.succeeded -and $accepted.sourceCommit -ceq $baselineBuildCommit -and $accepted.runId -ceq '41270490335' -and $accepted.workflowConclusion -ceq 'success' -and $accepted.artifactSha256 -ceq $state.baselineArtifact.sha256) 'Accepted baseline evidence identity mismatch'
    $baselineIdentity=@($accepted.configurations|Where-Object{$_.configuration -ceq 'Release'})[0]
    foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){
        Need ((Hash (Join-Path "$baseline/bin" $file)) -ceq $baselineIdentity.runtimeFiles.$file) ('Accepted baseline runtime identity differs: '+$file)
    }
    $candidateExe=Join-Path $product 'bin/NewModCompiler.exe';$baselineExe=Join-Path $baseline 'bin/NewModCompiler.exe'
    Need ((Hash $candidateExe) -ceq $build.exeSha256 -and (Hash $candidateExe) -ceq '6120e780cbf3f2d8735d308d184a846815dcc2446bfab60bd60202a6bfa91b0c') 'Candidate EXE identity differs'
    Need ((Hash $baselineExe) -ceq $baselineBuild.exeSha256) 'Baseline EXE differs from accepted receipt'
    foreach($file in @('NewModCompiler.exe.config','antlr.runtime.dll')){
        Need ((Hash (Join-Path "$product/bin" $file)) -ceq $baselineIdentity.runtimeFiles.$file) ('Candidate unchanged runtime dependency differs: '+$file)
    }
    Need (@(Get-ChildItem "$product/bin" -File).Count -eq 3 -and @(Get-ChildItem "$baseline/bin" -File).Count -eq 3) 'Runtime file inventory differs'
    $state.candidateExeSha256=Hash $candidateExe;$state.baselineExeSha256=Hash $baselineExe
    $state.candidateBinarySourceCommit=$candidateBuildCommit;$state.candidateBuildRunId='41271617644';$state.baselineBinarySourceCommit=$baselineBuildCommit;$state.baselineBuildRunId='41270490335';$state.productionBuildReused=$true
    $state.priorValidationFailure='Baseline relink failure in temporary harness only; original App and candidate production-build successes are reused independently.'

    # Reuse TestCompilerCli.ps1's known schema-valid Armor fixture and entry-path layout.
    $sdk=Join-Path $work 'sdk';$mod=Join-Path $work 'fixture'
    New-Item -ItemType Directory -Force "$sdk/tools","$sdk/Schemas","$mod/Data/AptUI","$mod/Data/AdditionalMaps","$mod/Additional"|Out-Null
    Copy-Item -LiteralPath "$repo/SDK/Schemas/xsd" -Destination "$sdk/Schemas" -Recurse
    Copy-Item -LiteralPath "$repo/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$sdk/tools"
    $probe=[IO.File]::ReadAllText("$repo/SDK/Mods/mymod/Data/RecoveryArmorProbe.xml")
    [IO.File]::WriteAllText("$mod/Data/Mod.xml",$probe)
    [IO.File]::WriteAllText("$mod/Data/AptUI/One.xml",$probe.Replace('Default="72.5%"','Default="22.5%"'))
    [IO.File]::WriteAllText("$mod/Data/AdditionalMaps/MapMetaData_One.xml",$probe.Replace('Default="72.5%"','Default="42.5%"'))
    [IO.File]::WriteAllText("$mod/Additional/cli-fixture.ini",'identical Additional input')
    $state.fixtureInputInventory=Inventory $mod
    $state.sdkInputInventory=Inventory $sdk

    function RunCli([string]$Name,[string]$Exe,[string[]]$Extra=@(),[int]$Expected=0,[string]$Command='build'){
        $case=Join-Path $out ('cases/'+$Name);$destination=Join-Path $work ('out-'+$Name)
        New-Item -ItemType Directory -Force $case|Out-Null
        $cli=@($Command,'--mod',$mod,'--sdk-root',$sdk,'--out',$destination);if($Extra){$cli+=$Extra}
        $start=[Diagnostics.ProcessStartInfo]::new($Exe)
        $start.WorkingDirectory=[IO.Path]::GetDirectoryName($Exe);$start.UseShellExecute=$false
        $start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true;$start.CreateNoWindow=$true
        foreach($argument in $cli){$start.ArgumentList.Add($argument)}
        $process=[Diagnostics.Process]::Start($start)
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        $timedOut=!$process.WaitForExit(180000)
        if($timedOut){$process.Kill($true);$process.WaitForExit()}
        [IO.File]::WriteAllText("$case/stdout.log",$stdout.GetAwaiter().GetResult())
        [IO.File]::WriteAllText("$case/stderr.log",$stderr.GetAwaiter().GetResult())
        $exit=$process.ExitCode;$process.Dispose()
        Need (!$timedOut) "$Name exceeded 180 seconds"
        $entry=@{name=$Name;arguments=$cli;exitCode=$exit;expectedExit=$Expected;passed=$false}
        $state.cases+=,$entry
        if($exit -ne $Expected){Get-Content "$case/stderr.log" -Tail 50|Out-Host;throw "$Name expected exit $Expected but received $exit"}
        $report=$null
        if(Test-Path -LiteralPath "$destination/build-report.json"){
            Copy-Item -LiteralPath "$destination/build-report.json","$destination/build.events.jsonl" -Destination $case
            $report=Get-Content "$destination/build-report.json" -Raw|ConvertFrom-Json
            Need ($report.exitCode -eq $Expected) "$Name report/process exit differ"
        }
        $inventory=@{}
        if($Expected -eq 0 -and $Command -ceq 'build'){
            Need ($null -ne $report -and $report.status -ceq 'succeeded' -and $report.publicationStatus -ceq 'PublishedDirectory') "$Name did not publish successfully"
            $inventory=Inventory "$destination/published"
            Need ($inventory.Count -gt 0) "$Name has no published outputs"
            Copy-Item -LiteralPath "$destination/published" -Destination $case -Recurse
        }
        $entry.passed=$true;$entry.inventory=$inventory
        return @{report=$report;inventory=$inventory;case=$case;destination=$destination}
    }
    $fixturePrior=Get-Content (Join-Path $CandidateEvidence 'validation.json') -Raw|ConvertFrom-Json
    Need ($fixturePrior.runId -ceq '41272130186' -and $fixturePrior.sourceCommit -ceq $SourceCommit -and !$fixturePrior.succeeded -and $fixturePrior.error.Contains('configured-assets') -and @($fixturePrior.cases).Count -eq 9 -and @($fixturePrior.cases|Where-Object{!$_.passed -or $_.exitCode -ne $_.expectedExit}).Count -eq 0) 'Prior fixture does not match known final harness assumption failure'
    Need ($fixturePrior.candidateExeSha256 -ceq $state.candidateExeSha256 -and $fixturePrior.baselineExeSha256 -ceq $state.baselineExeSha256) 'Prior fixture runtimes differ'
    Copy-Item -LiteralPath (Join-Path $CandidateEvidence 'validation.json') -Destination (Join-Path $out 'prior-fixture-validation.json')
    $state.priorValidationFailures=@($prior.error,$fixturePrior.error)
    $state.reusedFixtureRunId='41272130186';$state.reusedCases=@()
    function JsonMap($Object){$map=@{};foreach($property in $Object.PSObject.Properties){$map[$property.Name]=$property.Value};return $map}
    Same (JsonMap $fixturePrior.fixtureInputInventory) (Inventory $mod) 'Reconstructed fixture inputs'
    Same (JsonMap $fixturePrior.sdkInputInventory) (Inventory $sdk) 'Reconstructed SDK inputs'
    $names=@('baseline-default','candidate-default','baseline-loose','candidate-loose','neutral-false','neutral-reference-missing','advanced-false')
    $priorReports=@{};$priorInventories=@{}
    New-Item -ItemType Directory -Force (Join-Path $out 'cases')|Out-Null
    foreach($name in $names){
        $entry=@($fixturePrior.cases|Where-Object{$_.name -ceq $name})
        Need ($entry.Count -eq 1) ('Prior fixture case missing/duplicated: '+$name)
        $entry=$entry[0];$case=Join-Path $CandidateEvidence ('cases/'+$name)
        $report=Get-Content (Join-Path $case 'build-report.json') -Raw|ConvertFrom-Json
        Need ($report.exitCode -eq $entry.expectedExit -and $report.status -ceq $(if($name -ceq 'neutral-reference-missing'){'failed'}else{'succeeded'})) ('Prior report failed: '+$name)
        $priorReports[$name]=$report
        $inventory=@{}
        if($name -cne 'neutral-reference-missing'){
            $inventory=Inventory (Join-Path $case 'published')
            Same (JsonMap $entry.inventory) $inventory ('Archived '+$name)
        }
        $priorInventories[$name]=$inventory
        Copy-Item -LiteralPath $case -Destination (Join-Path $out 'cases') -Recurse
        $state.reusedCases+=,@{name=$name;sourceRunId='41272130186';passed=$true;exitCode=$entry.exitCode;expectedExit=$entry.expectedExit;inventory=$inventory}
    }
    Same $priorInventories['baseline-default'] $priorInventories['candidate-default'] 'Prior default published bytes'
    Same $priorInventories['baseline-loose'] $priorInventories['candidate-loose'] 'Prior loose published bytes'
    Same $priorInventories['candidate-default'] $priorInventories['neutral-false'] 'Prior explicit neutral false'
    Need ($priorInventories['candidate-default'].Count -eq 14 -and $priorInventories['candidate-loose'].Count -eq 12) 'Prior stream/package inventory changed'
    $default=$priorReports['candidate-default']
    Need (@($default.plan.Tasks).Count -eq 3 -and @($default.artifacts).Count -eq 2) 'Prior default auto/BIG contract differs'
    foreach($key in @('ss','pc','slowclean','sc','csc','psc')){Need ($default.effectiveOptions.$key.Value -ceq 'true') ('Prior default changed: '+$key)}
    Need ($default.effectiveOptions.jobs.Value -ceq 'auto' -and $default.effectiveOptions.performance.Value -ceq 'off') 'Prior jobs/performance default changed'
    $bigRelative=@($priorInventories['candidate-default'].Keys|Where-Object{$_ -like '*.big'})[0]
    $bigPath=Join-Path (Join-Path $CandidateEvidence 'cases/candidate-default/published') $bigRelative
    $bigText=[Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($bigPath))
    Need ($bigText.Contains('identical Additional input')) 'Prior Additional input absent from BIG'
    Need (@($priorReports['neutral-reference-missing'].diagnostics|Where-Object{$_.code -ceq 'PACKAGE_NEUTRAL_REFERENCE_MISSING'}).Count -eq 1 -and @($priorReports['neutral-reference-missing'].artifacts).Count -eq 0) 'Prior neutral reference rejection differs'
    $advanced=$priorReports['advanced-false']
    foreach($key in @('ss','pc','slowclean','sc','bc','csc','psc','fsc','atc')){Need ($advanced.effectiveOptions.$key.Value -ceq 'false' -and $advanced.effectiveOptions.$key.Source -ceq 'commandLine') ('Prior advanced false changed: '+$key)}
    Need ($advanced.cache.status -ceq 'Disabled' -and !$advanced.cache.sessionCache -and !$advanced.cache.compressedSessionCache -and !$advanced.cache.partialSessionCache -and !$advanced.cache.freezeSessionCache -and !$advanced.cache.alwaysTouchCache) 'Prior advanced settings did not reach Core'
    $state.comparisons=@(@{format='default';rawPublishedBytesIdentical=$true;sourceRunId='41272130186';files=$priorInventories['candidate-default']},@{format='loose';rawPublishedBytesIdentical=$true;sourceRunId='41272130186';files=$priorInventories['candidate-loose']})

    # Armor's compiler has UseBuildCache=false, so no network *.asset is expected.
    # Disable session caching to distinguish actual Core BuildCache states directly.
    $configPath=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config';$configBytes=[IO.File]::ReadAllBytes($configPath)
    try {
        [xml]$config=[IO.File]::ReadAllText($configPath);$section=$config.SelectSingleNode('/configuration/assetbuilder')
        $section.SetAttribute('buildCache','false');$config.Save($configPath)
        $configured=RunCli 'sdk-buildcache-false-no-session' $candidateExe @('--package-format','loose','/sc:false')
        Need ($configured.report.effectiveOptions.bc.Value -ceq 'false' -and $configured.report.effectiveOptions.bc.Source -ceq 'sdkConfig' -and $configured.report.cache.status -ceq 'Disabled' -and !$configured.report.cache.sessionCache -and $configured.report.cache.hits -eq 0 -and $configured.report.cache.misses -eq 0) 'SDK buildCache did not override default bc in actual Core state'
        $override=RunCli 'sdk-buildcache-cli-true-no-session' $candidateExe @('--package-format','loose','/sc:false','/bc:true')
        Need ($override.report.effectiveOptions.bc.Value -ceq 'true' -and $override.report.effectiveOptions.bc.Source -ceq 'commandLine' -and $override.report.cache.status -ceq 'BuildAssetCache' -and !$override.report.cache.sessionCache) 'Explicit bc true did not override SDK config in actual Core state'
        Same $configured.inventory $override.inventory 'SDK buildCache flags'
        Same $priorInventories['candidate-loose'] $override.inventory 'SDK buildCache same input output bytes'
    }finally{[IO.File]::WriteAllBytes($configPath,$configBytes)}
    Same $state.fixtureInputInventory (Inventory $mod) 'Fixture input'
    Same $state.sdkInputInventory (Inventory $sdk) 'SDK input'
    $state.succeeded=$true
    Write-Host ('PASS CLI simplification: reused7 checked fixture cases; retried2 SDK buildCache cases; default BIG/SKUDEF + loose raw bytes identical')
} catch {$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 30|Set-Content (Join-Path $out 'validation.json')}
