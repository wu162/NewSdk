param(
    [int]$TimeoutSeconds = 300,
    [string[]]$Profiles = @(),
    [string]$CompilerArtifactDirectory = $env:XML_PRIORITY_COMPILER_ARTIFACT
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$request = Get-Content 'recovery/catalog/XmlSdkValidationRequest.json' -Raw | ConvertFrom-Json
if (!$Profiles.Count) { $Profiles = @($request.profiles) }
if (!$Profiles.Count -or @($Profiles | Select-Object -Unique).Count -ne $Profiles.Count) { throw 'SDK profiles must be nonempty and unique' }
$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$work = Join-Path $tempRoot ('ra3-xml-priority-sdk-' + [Guid]::NewGuid().ToString('N'))
$build = Join-Path $work 'build'
$report = Join-Path $root 'recovery/evidence/xml-priority-sdk'
New-Item -ItemType Directory -Force $build,$report | Out-Null
$sourceCommit = if ($env:GITHUB_SHA) { $env:GITHUB_SHA } else { (& git rev-parse HEAD).Trim() }
$runId = if ($env:GITHUB_RUN_ID) { $env:GITHUB_RUN_ID } else { 'local-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') }
$summary = [ordered]@{
 succeeded=$false;sourceCommit=$sourceCommit;runId=$runId;status='started'
 scope='289 recovered XmlCompiler entries; schema-valid real MODs under SDK/Mods; EALAModStudio defaultscript base-data BAB flow only'
 sdkMode='/O2';sdkPlatform=0;requestedProfiles=@($Profiles);profiles=@();files=@();originalDllUnchanged=$false
 fullMymodGate=$false;profileCount=$Profiles.Count;studioStages=@(1,2,5);realSdkBuilds=0
}
$originalHashes = @{}
foreach ($name in @('BinaryAssetBuilder.Tokenizer.dll','BinaryAssetBuilder.XmlCompiler.dll')) { $originalHashes[$name]=(Get-FileHash "SDK/tools/$name" -Algorithm SHA256).Hash }
$assetNamespace='uri:ea.com:eala:asset'
function Export-ModProbe([string]$ModXml,[string]$ProbePath) {
    $probe = New-Object Xml.XmlDocument
    $declaration=$probe.CreateXmlDeclaration('1.0','utf-8',$null);$probe.AppendChild($declaration)|Out-Null
    $top=$probe.CreateElement('AssetDeclaration',$assetNamespace);$probe.AppendChild($top)|Out-Null
    $visited=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    function Add-ModDocument([string]$Path) {
        $full=(Resolve-Path $Path).Path
        if (!$visited.Add($full)) { return }
        [xml]$document=Get-Content -Raw $full
        if ($document.DocumentElement.LocalName -ne 'AssetDeclaration' -or $document.DocumentElement.NamespaceURI -ne $assetNamespace) { throw "Not an SDK asset source: $full" }
        foreach ($node in $document.DocumentElement.ChildNodes) {
            if ($node.NodeType -ne [Xml.XmlNodeType]::Element) { continue }
            if ($node.LocalName -eq 'Includes') {
                foreach ($include in $node.ChildNodes) {
                    if ($include.NodeType -ne [Xml.XmlNodeType]::Element -or $include.GetAttribute('type') -eq 'reference') { continue }
                    $source=$include.GetAttribute('source')
                    if ($source -match '^[A-Za-z]+:') { throw "Test MOD all-includes must be explicit local sources: $source" }
                    Add-ModDocument (Join-Path ([IO.Path]::GetDirectoryName($full)) $source)
                }
            } elseif ($node.LocalName -ne 'Tags') { $top.AppendChild($probe.ImportNode($node,$true))|Out-Null }
        }
    }
    Add-ModDocument $ModXml
    $probe.Save($ProbePath)
    return @($visited | Sort-Object)
}
try {
    Copy-Item SDK/tools/*.dll $build
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $oracleSources=@((Resolve-Path 'recovery/tests/XmlPrioritySdkOracle.cs').Path)
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll "/r:$build/BinaryAssetBuilder.Core.dll" "/out:$build/XmlPrioritySdkOracle.exe" @oracleSources *> "$report/oracle-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle-build.txt";throw 'SDK oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$build/XmlPrioritySdkOracle.exe.config"
    $reused=$false
    if ($CompilerArtifactDirectory -and (Test-Path $CompilerArtifactDirectory)) {
        & git diff --quiet $request.acceptedCompilerSource HEAD -- recovery/src/Native recovery/src/Bridge
        if ($LASTEXITCODE -eq 0) {
            $link=Get-Content 'recovery/evidence/xml-priority-link/validation.json' -Raw|ConvertFrom-Json
            $accepted=@($link.modes|Where-Object{$_.mode.TrimStart('/') -eq 'O2'})
            if ($link.succeeded -and $link.sourceCommit -eq $request.acceptedCompilerSource -and $accepted.Count -eq 1) {
                $candidates=@(Get-ChildItem $CompilerArtifactDirectory -Filter Recovered.XmlCompilerPriority.dll -File -Recurse|Where-Object{(Get-FileHash $_.FullName -Algorithm SHA256).Hash -eq $accepted[0].linkedAssemblySha256})
                if ($candidates.Count -eq 1) {
                    Copy-Item $candidates[0].FullName "$build/Recovered.XmlCompilerPriority.dll"
                    $reused=$true;$summary['compilerReuse']=[ordered]@{sourceCommit=$request.acceptedCompilerSource;runId=$request.acceptedCompilerRunId;sha256=$accepted[0].linkedAssemblySha256;nativeAndBridgeInputsUnchanged=$true}
                }
            }
        }
    }
    if (!$reused) {
        $native=Join-Path $root 'recovery/src/Native'
        $common=@('Relocation','ScalarParsing','DataInitializers','ResourceMarshaling','EnumBitset','ObjectFilter')
        $sources=@($common|ForEach-Object{Join-Path $native "$_.cpp"})
        $sources+=@(Get-ChildItem "$native/Xml*.cpp" -File|Where-Object{$_.Name -notmatch 'Draft'}|Sort-Object Name|ForEach-Object{$_.FullName})
        $sources+=@(Get-ChildItem 'recovery/src/Bridge/Xml*Compiler.cpp' -File|Where-Object{$_.Name -notmatch 'Draft'}|Sort-Object Name|ForEach-Object{$_.FullName})
        & cl /nologo /clr /LD /MD /EHa /std:c++17 /O2 /fp:precise "/AI$build" "/Fo$build/" "/Fe$build/Recovered.XmlCompilerPriority.dll" @sources *> "$report/build.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$report/build.txt";throw 'Combined SDK compiler build failed' }
        $summary['compilerReuse']=[ordered]@{reused=$false;reason='No matching accepted artifact or native/bridge inputs changed'}
    }
    & $csc /nologo /target:library /platform:x86 "/r:$build/BinaryAssetBuilder.Core.dll" "/r:$build/Recovered.XmlCompilerPriority.dll" "/out:$build/Recovered.XmlPriority.Plugin.dll" (Resolve-Path 'recovery/src/Plugin/XmlPriorityPlugin.cs').Path *> "$report/plugin-build.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/plugin-build.txt";throw 'SDK plugin build failed' }
    & "$build/XmlPrioritySdkOracle.exe" metadata "$build/BinaryAssetBuilder.XmlCompiler.dll" "$build/Recovered.XmlPriority.Plugin.dll" "$report/metadata.json" *> "$report/metadata.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/metadata.txt";throw 'Original/recovered 289-entry metadata differs' }
    $summary['compilerSha256']=(Get-FileHash "$build/Recovered.XmlCompilerPriority.dll" -Algorithm SHA256).Hash
    $summary['pluginSha256']=(Get-FileHash "$build/Recovered.XmlPriority.Plugin.dll" -Algorithm SHA256).Hash
    foreach ($profile in $Profiles) {
        $profileReport=Join-Path $report $profile
        if (Test-Path $profileReport) { Remove-Item $profileReport -Recurse -Force }
        New-Item -ItemType Directory -Force $profileReport|Out-Null
        $profileSummary=[ordered]@{name=$profile;succeeded=$false;schemaValidated=$false;fourOutputsIdentical=$false;originalAndIsolatedReplay=$false;compilerSha256=$summary.compilerSha256;pluginSha256=$summary.pluginSha256}
        $summary.profiles+=,$profileSummary
        $fullMymod=$profile -eq 'FullMymod'
        $modName=if($fullMymod){'mymod'}else{$profile}
        $sourceMod=Join-Path $root "SDK/Mods/$modName/Data/Mod.xml"
        if (!(Test-Path $sourceMod)) { throw "Missing test MOD $sourceMod" }
        $probe=Join-Path $profileReport 'source-probe.xml'
        if ($fullMymod) { Copy-Item 'recovery/sdk/XmlPriorityAllProbe.xml' $probe }
        else { $profileSummary['sourceFiles']=@(Export-ModProbe $sourceMod $probe|ForEach-Object{[ordered]@{path=$_.Substring($root.Length+1).Replace('\','/');sha256=(Get-FileHash $_ -Algorithm SHA256).Hash}}) }
        & "$build/XmlPrioritySdkOracle.exe" schema (Join-Path $root 'SDK') $probe "$profileReport/schema.json" *> "$profileReport/schema.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$profileReport/schema.txt";throw "$profile source MOD is not valid original XSD input" }
        $profileSummary.schemaValidated=$true
        $profileSummary['probeSha256']=(Get-FileHash $probe -Algorithm SHA256).Hash
        $trace=Join-Path $profileReport 'trace'
        if (Test-Path $trace) { Remove-Item $trace -Recurse -Force }
        foreach ($mode in @('Original','Recovered')) {
            $sdk=Join-Path $work ($profile+'-'+$mode)
            Copy-Item (Join-Path $root 'SDK') $sdk -Recurse
            $mod=Join-Path $sdk "Mods/$modName/Data/Mod.xml"
            if ($fullMymod) {
                Copy-Item 'recovery/sdk/XmlPriorityAllProbe.xml' (Join-Path $sdk 'Mods/mymod/Data/RecoveryXmlPriorityProbe.xml')
                [xml]$full=Get-Content $mod -Raw;$ns=New-Object Xml.XmlNamespaceManager($full.NameTable);$ns.AddNamespace('ea',$assetNamespace)
                $includes=$full.SelectSingleNode('/ea:AssetDeclaration/ea:Includes',$ns)
                if ($null -eq $includes) { throw 'Shipped mymod has no Includes' }
                $originalIncludes=@($includes.ChildNodes|Where-Object{$_.NodeType -eq [Xml.XmlNodeType]::Element}|ForEach-Object{$_.OuterXml})
                $append=$full.CreateElement('Include',$assetNamespace);$append.SetAttribute('type','all');$append.SetAttribute('source','RecoveryXmlPriorityProbe.xml');$includes.AppendChild($append)|Out-Null
                $retained=@($includes.ChildNodes|Where-Object{$_.NodeType -eq [Xml.XmlNodeType]::Element})
                if ($retained.Count -ne $originalIncludes.Count+1) { throw 'Shipped mymod include count changed' }
                for($i=0;$i -lt $originalIncludes.Count;++$i){if($retained[$i].OuterXml -ne $originalIncludes[$i]){throw 'Shipped mymod include modified'}}
                $full.Save($mod);$profileSummary['shippedIncludesPreserved']=$originalIncludes.Count
            }
            & "$PSScriptRoot/SetXmlPriorityPlugin.ps1" -SdkRoot $sdk -PluginDirectory $build -Mode $mode
            $cfg=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
            [xml]$configuration=Get-Content -Raw $cfg
            foreach($startup in @($configuration.configuration.SelectNodes('startup'))){$configuration.configuration.RemoveChild($startup)|Out-Null}
            $startup=$configuration.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true');$runtime=$configuration.CreateElement('supportedRuntime');$runtime.SetAttribute('version','v4.0');$startup.AppendChild($runtime)|Out-Null;$configuration.configuration.AppendChild($startup)|Out-Null;$configuration.Save($cfg)
            Copy-Item $cfg "$profileReport/$mode.config";Copy-Item $mod "$profileReport/$mode-Mod.xml"
            $env:RA3_RECOVERED_XML_PRIORITY_TRACE=if($mode -eq 'Recovered'){$trace}else{$null}
            if($mode -eq 'Recovered'){$recoveredSdk=$sdk}
            & "$PSScriptRoot/InvokeEalaModStudioBuild.ps1" -SdkRoot $sdk -ModName $modName -EvidenceDirectory "$profileReport/$mode-studio" -TimeoutSeconds $TimeoutSeconds
            $summary.realSdkBuilds++
            $flow=Get-Content "$profileReport/$mode-studio/studio-flow.json" -Raw|ConvertFrom-Json
            if(!$flow.succeeded -or @($flow.steps|Where-Object{$_.step -eq 5 -and $_.label -eq 'mod' -and $_.exitCode -eq 0}).Count -ne 1){throw 'No successful EALAModStudio base-data BAB invocation'}
            foreach($ext in @('bin','manifest','imp','relo')){
                $file=Join-Path $sdk "builtmods/mods/$modName/data/mod.$ext"
                if(!(Test-Path $file)){throw "$mode missing mod.$ext"}
                Copy-Item $file "$profileReport/$mode-mod.$ext"
                $summary.files+=[ordered]@{profile=$profile;mode=$mode;file="mod.$ext";bytes=(Get-Item $file).Length;sha256=(Get-FileHash $file -Algorithm SHA256).Hash}
            }
        }
        $env:RA3_RECOVERED_XML_PRIORITY_TRACE=$null
        if((Get-FileHash "$profileReport/Original-Mod.xml").Hash -ne (Get-FileHash "$profileReport/Recovered-Mod.xml").Hash){throw "$profile MOD inputs differ"}
        foreach($ext in @('bin','manifest','imp','relo')){if((Get-FileHash "$profileReport/Original-mod.$ext").Hash -ne (Get-FileHash "$profileReport/Recovered-mod.$ext").Hash){throw "$profile SDK output differs: mod.$ext"}}
        if((Get-Item "$profileReport/Recovered-mod.imp").Length -le 4){throw "$profile did not exercise nonempty SDK imports"}
        $profileSummary['nonemptyImports']=$true
        $profileSummary.fourOutputsIdentical=$true
        Push-Location $recoveredSdk
        try {
            & "$build/XmlPrioritySdkOracle.exe" replay "$build/BinaryAssetBuilder.XmlCompiler.dll" $trace "$profileReport/Recovered-mod.bin" $probe "$profileReport/replay.json" 0 $profile *> "$profileReport/replay.txt"
            $replayExit=$LASTEXITCODE
        } finally { Pop-Location }
        if($replayExit -ne 0){Get-Content "$profileReport/replay.txt";throw "$profile original three-buffer replay failed"}
        $isolated=Join-Path $work ($profile+'-isolated');New-Item -ItemType Directory -Force $isolated|Out-Null
        Get-ChildItem $build -File|Where-Object{$_.Extension -in @('.dll','.exe','.config') -and $_.Name -notin @('BinaryAssetBuilder.Tokenizer.dll','BinaryAssetBuilder.XmlCompiler.dll')}|Copy-Item -Destination $isolated
        # Recreate relative file-backed MOD inputs without copying either original compiler DLL.
        Copy-Item (Join-Path $root 'SDK/Mods') (Join-Path $isolated 'Mods') -Recurse
        Push-Location $isolated
        try {
            & "$isolated/XmlPrioritySdkOracle.exe" isolated "$isolated/Recovered.XmlPriority.Plugin.dll" $trace "$profileReport/Recovered-mod.bin" $probe "$profileReport/isolated.json" 0 $profile *> "$profileReport/isolated.txt"
            $isolatedExit=$LASTEXITCODE
        } finally { Pop-Location }
        if($isolatedExit -ne 0){Get-Content "$profileReport/isolated.txt";throw "$profile original-free replay failed"}
        foreach($name in @('schema','replay','isolated')){if(!(Get-Content "$profileReport/$name.json" -Raw|ConvertFrom-Json).succeeded){throw "$profile missing successful $name evidence"}}
        $profileSummary.originalAndIsolatedReplay=$true;$profileSummary.succeeded=$true
        $profileSummary|ConvertTo-Json -Depth 15|Set-Content "$profileReport/validation.json"
    }
    if($summary.profiles.Count -ne $Profiles.Count -or @($summary.profiles|Where-Object{!$_.succeeded}).Count){throw 'Incomplete requested SDK profile matrix'}
    $summary.fullMymodGate=@($summary.profiles|Where-Object{$_.name -eq 'FullMymod' -and $_.succeeded}).Count -eq 1
    $summary.succeeded=$true;$summary.status='Requested original/recovered MOD base-data builds match four outputs; exact metadata, XSD, original replay and original-free recovered replay passed'
} catch {$summary.status='failed';$summary['error']=$_.ToString();throw}
finally {
    $env:RA3_RECOVERED_XML_PRIORITY_TRACE=$null
    $summary.originalDllUnchanged=$true
    foreach($name in $originalHashes.Keys){if((Get-FileHash "SDK/tools/$name" -Algorithm SHA256).Hash -ne $originalHashes[$name]){$summary.originalDllUnchanged=$false;$summary.succeeded=$false}}
    $summary|ConvertTo-Json -Depth 18|Set-Content "$report/validation.json"
}
