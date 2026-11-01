# Temporary Windows validation harness; not part of the implementation branch.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$SourceCommit,
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
$state=@{baselineCommit=$BaselineCommit;sourceCommit=$SourceCommit;validationCommit=$env:GITHUB_SHA;runId=$env:GITHUB_RUN_ID;succeeded=$false;cases=@();comparisons=@();scope='Release App contracts, one production Release build, baseline App recompiled/relinked against identical native/Foundation inputs, same-input small Armor base/APT/metadata fixture; no benchmark or full MOD claim'}
try {
    # Sharing baseline native/Foundation inputs is valid only if their complete source/build/SDK closure is unchanged.
    $changed=@(git diff --name-only $BaselineCommit $SourceCommit -- recovery/src recovery/build recovery/scripts SDK)
    Need ($LASTEXITCODE -eq 0) 'Cannot compare production input closure'
    $unexpected=@($changed|Where-Object{$_ -notlike 'recovery/src/App/*'})
    Need ($unexpected.Count -eq 0) ('Baseline non-App production inputs differ: '+($unexpected -join ', '))
    $state.changedProductionInputs=$changed

    $app=Join-Path $out 'app'
    New-Item -ItemType Directory -Force $app|Out-Null
    try {& "$repo/recovery/scripts/TestApp.ps1" -Configuration Release -OutputDirectory $app *> "$app/test-app.log";Need ($LASTEXITCODE -eq 0) 'Release App contract tests failed'}
    catch {Get-Content "$app/test-app.log" -Tail 80|Out-Host;throw}
    Get-Content "$app/test-app.log" -Tail 12|Out-Host
    $state.appContractsPassed=$true

    $product=Join-Path $out 'product'
    & "$repo/recovery/scripts/BuildCompiler.ps1" -Configuration Release -OutputDirectory $product
    Need ($LASTEXITCODE -eq 0) 'Release production build failed'
    $build=Get-Content "$product/build.json" -Raw|ConvertFrom-Json
    $recipe=Get-Content "$product/link-inputs.json" -Raw|ConvertFrom-Json
    $candidateExe=Join-Path $product 'bin/NewModCompiler.exe'
    Need ((Hash $candidateExe) -ceq $build.exeSha256) 'Candidate executable differs from build receipt'
    $state.candidateExeSha256=Hash $candidateExe

    # Recompile only baseline App sources. Preserve native/Foundation/bridge/resource input bytes and the real entry point.
    $baseline=Join-Path $out 'baseline'
    $baselineSource=Join-Path $work 'baseline-source'
    $baselineObj=Join-Path $baseline 'obj';$baselineBin=Join-Path $baseline 'bin'
    New-Item -ItemType Directory -Force $baselineSource,$baselineObj,$baselineBin|Out-Null
    $archive=Join-Path $work 'baseline-app.zip'
    & git archive --format=zip "--output=$archive" $BaselineCommit recovery/src/App
    Need ($LASTEXITCODE -eq 0) 'Baseline App source archive failed'
    Expand-Archive -LiteralPath $archive -DestinationPath $baselineSource
    $baselineApp=Join-Path $baselineSource 'recovery/src/App'
    $appModule=Join-Path $baselineObj 'Managed.Application.netmodule'
    $appSourceRoot=[IO.Path]::GetFullPath((Join-Path $repo 'recovery/src/App'))+[IO.Path]::DirectorySeparatorChar
    $arguments=@(foreach($line in [IO.File]::ReadAllLines("$product/obj/Application.rsp")){
        if($line.StartsWith('/out:',[StringComparison]::OrdinalIgnoreCase)){'/out:"'+$appModule+'"'}
        elseif(!$line.Trim('"').StartsWith($appSourceRoot,[StringComparison]::OrdinalIgnoreCase)){$line}
    })
    $arguments+=@(Get-ChildItem -LiteralPath $baselineApp -Recurse -Filter '*.cs' -File|Sort-Object FullName|ForEach-Object{'"'+$_.FullName+'"'})
    $rsp=Join-Path $baselineObj 'Application.rsp'
    [IO.File]::WriteAllLines($rsp,[string[]]$arguments,[Text.UTF8Encoding]::new($false))
    $packageLine=(& dotnet nuget locals global-packages --list)
    Need ($LASTEXITCODE -eq 0) 'Cannot locate fixed Roslyn compiler'
    $packageRoot=($packageLine -replace '^global-packages:\s*','').Trim()
    $csc=Join-Path $packageRoot 'microsoft.net.compilers.toolset/4.0.1/tasks/net472/csc.exe'
    & $csc ('@'+$rsp) *> "$baseline/application-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$baseline/application-build.log" -Tail 80|Out-Host;throw 'Baseline App compilation failed'}
    $candidateModule=@($recipe.inputs|Where-Object{$_.kind -ceq 'application'})[0].path
    $baselineExe=Join-Path $baselineBin 'NewModCompiler.exe'
    $linkArguments=@(foreach($argument in $recipe.arguments){
        if($argument -ceq $candidateModule){$appModule}
        elseif($argument.StartsWith('/OUT:',[StringComparison]::OrdinalIgnoreCase)){'/OUT:'+$baselineExe}
        elseif($argument.StartsWith('/IMPLIB:',[StringComparison]::OrdinalIgnoreCase)){'/IMPLIB:'+(Join-Path $baselineObj 'EaLayer3Exports.lib')}
        else{$argument}
    })
    foreach($sharedInput in @($recipe.inputs|Where-Object{$_.kind -cne 'application'})){Need ((Hash $sharedInput.path) -ceq $sharedInput.sha256) ('Shared link input changed: '+$sharedInput.path)}
    $linkRsp=Join-Path $baselineObj 'Link.rsp'
    [IO.File]::WriteAllLines($linkRsp,[string[]]@($linkArguments|ForEach-Object{'"'+$_+'"'}),[Text.UTF8Encoding]::new($false))
    & link.exe ('@'+$linkRsp) *> "$baseline/link.log"
    if($LASTEXITCODE -ne 0){Get-Content "$baseline/link.log" -Tail 80|Out-Host;throw 'Baseline production relink failed'}
    Copy-Item -LiteralPath "$baselineApp/App.config" -Destination ($baselineExe+'.config')
    Copy-Item -LiteralPath "$repo/SDK/tools/antlr.runtime.dll" -Destination $baselineBin
    $state.baselineExeSha256=Hash $baselineExe
    @{baselineCommit=$BaselineCommit;candidateSourceCommit=$SourceCommit;appModuleSha256=(Hash $appModule);exeSha256=$state.baselineExeSha256;sharedLinkInputs=@($recipe.inputs|Where-Object{$_.kind -cne 'application'});arguments=$linkArguments}|ConvertTo-Json -Depth 10|Set-Content "$baseline/build.json"

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
    foreach($format in @('default','loose')){
        [string[]]$extra=@();if($format -ceq 'loose'){$extra=@('--package-format','loose')}
        $before=RunCli ('baseline-'+$format) $baselineExe $extra
        $after=RunCli ('candidate-'+$format) $candidateExe $extra
        Same $before.inventory $after.inventory $format
        Need (@($after.report.plan.Tasks).Count -eq 3) 'Automatic content discovery changed'
        if($format -ceq 'default'){
            Need ($after.inventory.Count -eq 14 -and @($after.inventory.Keys|Where-Object{$_ -like '*.big'}).Count -eq 1 -and @($after.inventory.Keys|Where-Object{$_ -like '*.skudef'}).Count -eq 1) 'Default did not retain twelve stream files plus BIG + SKUDEF'
            Need (@($after.report.artifacts).Count -eq 2 -and @($after.report.artifacts|Where-Object{$_.Kind -in @('package.big','package.skudef')}).Count -eq 2) 'Default report package artifacts changed'
            $bigRelative=@($after.inventory.Keys|Where-Object{$_ -like '*.big'})[0]
            $bigPath=Join-Path (Join-Path $after.case 'published') $bigRelative
            Need (([Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($bigPath))).Contains('identical Additional input')) 'Default BIG omitted Additional'
            foreach($key in @('ss','pc','slowclean','sc','csc','psc')){Need ($after.report.effectiveOptions.$key.Value -ceq 'true') "Default $key changed"}
            Need ($after.report.effectiveOptions.jobs.Value -ceq 'auto' -and $after.report.effectiveOptions.performance.Value -ceq 'off') 'Jobs/performance defaults changed'
        }else{Need ($after.inventory.Count -eq 12) 'Loose did not publish three four-file streams'}
        $state.comparisons+=,@{format=$format;rawPublishedBytesIdentical=$true;files=$after.inventory}
    }
    # Explicit false must not consult missing Worldbuilder references.
    $falseNeutral=RunCli 'neutral-false' $candidateExe @('--fix-neutral-assets=false')
    Same $falseNeutral.inventory $state.comparisons[0].files 'Explicit neutral false'
    $trueNeutral=RunCli 'neutral-reference-missing' $candidateExe @('--fix-neutral-assets=true') 3
    Need ($null -ne $trueNeutral.report -and @($trueNeutral.report.diagnostics|Where-Object{$_.code -ceq 'PACKAGE_NEUTRAL_REFERENCE_MISSING'}).Count -eq 1) 'Missing neutral reference did not retain exit3/report diagnostic'
    Need (@($trueNeutral.report.artifacts).Count -eq 0) 'Neutral reference failure accepted artifacts'

    $flags=@('/ss:false','/pc:false','/slowclean:false','/sc:false','/bc:false','/csc:false','/psc:false','/fsc:false','/atc:false')
    $advanced=RunCli 'advanced-false' $candidateExe (@('--package-format','loose')+$flags)
    foreach($key in @('ss','pc','slowclean','sc','bc','csc','psc','fsc','atc')){Need ($advanced.report.effectiveOptions.$key.Value -ceq 'false' -and $advanced.report.effectiveOptions.$key.Source -ceq 'commandLine') "Advanced $key false did not reach request"}
    Need ($advanced.report.cache.status -ceq 'Disabled' -and !$advanced.report.cache.sessionCache -and !$advanced.report.cache.compressedSessionCache -and !$advanced.report.cache.partialSessionCache -and !$advanced.report.cache.freezeSessionCache -and !$advanced.report.cache.alwaysTouchCache) 'Advanced cache false flags did not reach Core'

    $configPath=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config';$configBytes=[IO.File]::ReadAllBytes($configPath)
    try {
        [xml]$config=[IO.File]::ReadAllText($configPath);$section=$config.SelectSingleNode('/configuration/assetbuilder')
        $configuredAssets=Join-Path $work 'configured-assets';$section.SetAttribute('buildCache','false');$section.SetAttribute('buildCacheRoot',$configuredAssets);$config.Save($configPath)
        $configured=RunCli 'sdk-buildcache-false' $candidateExe @('--package-format','loose')
        Need ($configured.report.effectiveOptions.bc.Value -ceq 'false' -and $configured.report.effectiveOptions.bc.Source -ceq 'sdkConfig' -and $configured.report.cache.hits -eq 0 -and $configured.report.cache.misses -eq 0) 'SDK buildCache did not override default bc'
        $override=RunCli 'sdk-buildcache-cli-true' $candidateExe @('--package-format','loose','/bc:true')
        Need ($override.report.effectiveOptions.bc.Value -ceq 'true' -and $override.report.effectiveOptions.bc.Source -ceq 'commandLine' -and @(Get-ChildItem -LiteralPath $configuredAssets -Filter '*.asset' -File -Recurse).Count -gt 0) 'Explicit bc true did not override SDK config/use configured cache root'
        Same $configured.inventory $override.inventory 'SDK cache flags'
    }finally{[IO.File]::WriteAllBytes($configPath,$configBytes)}
    Same $state.fixtureInputInventory (Inventory $mod) 'Fixture input'
    Same $state.sdkInputInventory (Inventory $sdk) 'SDK input'
    $state.succeeded=$true
    Write-Host ('PASS CLI simplification: '+$state.cases.Count+' production cases; default BIG/SKUDEF + loose raw bytes identical')
} catch {$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 30|Set-Content (Join-Path $out 'validation.json')}
