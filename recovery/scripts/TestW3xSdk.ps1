param(
    [Parameter(Mandatory=$true)][string]$ComponentDirectory,
    [Parameter(Mandatory=$true)][string]$PluginValidationDirectory,
    [string]$GateFile='recovery/w3x/SDK_READY.json',
    [string]$ModName='W3xRecovery',
    [string]$OutputDirectory,
    [int]$TimeoutSeconds=300
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$head=(git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP=[IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix=if($env:GITHUB_RUN_ID){"$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}else{'local-'+[Guid]::NewGuid().ToString('N')}
    $OutputDirectory="recovery/evidence/w3x/sdk-runs/$suffix/sdk"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite SDK evidence: $OutputDirectory" }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$report=(Resolve-Path $OutputDirectory).Path
$state=@{succeeded=$false;sourceCommit=$head;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;sdkStarted=$false;scope='W3DAnimation/W3DMesh test MOD; Win32 SDK only';status='checking closure and component/plugin gates'}
$state | ConvertTo-Json -Depth 6 | Set-Content "$report/validation.json"
& "$PSScriptRoot/VerifyW3xSdkGate.ps1" -GateFile $GateFile -ReportFile "$report/closure-gate.json"
$component=Get-Content (Join-Path $ComponentDirectory 'validation.json') -Raw | ConvertFrom-Json
foreach($field in @('succeeded','completeAnimationBuffersValidated','completeMeshBuffersValidated','componentWithoutOriginalReplay')) {
    if($component.$field -ne $true){throw "SDK blocked: component $field did not pass"}
}
if($component.sourceCommit -ne $head){throw 'SDK blocked: component result belongs to a different source commit'}
$plugin=Get-Content (Join-Path $PluginValidationDirectory 'plugin-validation.json') -Raw | ConvertFrom-Json
foreach($field in @('succeeded','processInstanceOriginalDifferential','withoutOriginalReplay')) {
    if($plugin.$field -ne $true){throw "SDK blocked: formal plugin $field did not pass"}
}
if($plugin.sourceCommit -ne $head){throw 'SDK blocked: plugin result belongs to a different source commit'}
$build=(Resolve-Path (Join-Path $PluginValidationDirectory 'O2')).Path
foreach($name in @('Recovered.W3x.Native.dll','Recovered.W3x.Mesh.dll','Recovered.W3x.Plugin.dll')) {
    if(!(Test-Path (Join-Path $build $name))){throw "Missing validated plugin artifact $name"}
}
$o2=@($plugin.modes | Where-Object mode -eq 'O2')
if($o2.Count -ne 1 -or !$o2[0].succeeded -or $o2[0].pluginSha256 -ne (Get-FileHash "$build/Recovered.W3x.Plugin.dll" -Algorithm SHA256).Hash.ToLowerInvariant()){throw 'SDK blocked: tested O2 plugin binary changed'}
$built=Get-Content "$build/plugin-build.json" -Raw | ConvertFrom-Json
if($built.sourceCommit -ne $head -or $built.mode -ne 'O2' -or $built.bridgeAnimationSha256 -ne (Get-FileHash "$build/Recovered.W3x.Native.dll" -Algorithm SHA256).Hash.ToLowerInvariant() -or $built.bridgeMeshSha256 -ne (Get-FileHash "$build/Recovered.W3x.Mesh.dll" -Algorithm SHA256).Hash.ToLowerInvariant()){throw 'SDK blocked: tested O2 bridge binaries changed'}
$work=Join-Path $env:RUNNER_TEMP ('w3x-sdk-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $work | Out-Null
$host=Join-Path $work 'host'
New-Item -ItemType Directory $host | Out-Null
Copy-Item SDK/tools/*.dll $host
$csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /optimize+ /r:System.Core.dll /r:System.Xml.dll /r:"$host/BinaryAssetBuilder.Core.dll" /out:"$host/W3xSdkOracle.exe" (Resolve-Path 'recovery/tests/W3xSdkOracle.cs').Path *> "$report/sdk-oracle-build.log"
if($LASTEXITCODE -ne 0){Get-Content "$report/sdk-oracle-build.log";throw 'SDK oracle host build failed'}
$runtime='<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'
$runtime | Set-Content "$host/W3xSdkOracle.exe.config"
$expected=Join-Path $report 'expected-assets.txt'
& "$host/W3xSdkOracle.exe" schema (Join-Path $root 'SDK') $ModName $expected *> "$report/schema.log"
if($LASTEXITCODE -ne 0){Get-Content "$report/schema.log";throw 'Full original CnC3Types XSD validation failed; BAB not started'}
$inputs=@()
foreach($file in Get-ChildItem (Join-Path $root "SDK/Mods/$ModName") -Recurse -File) {
    $inputs+=@{path=$file.FullName.Substring($root.Length+1).Replace('\','/');bytes=$file.Length;sha256=(Get-FileHash $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}
}
$inputs | ConvertTo-Json -Depth 5 | Set-Content "$report/inputs.json"
$tools=@()
foreach($file in @(Get-ChildItem SDK/tools -File | Where-Object Extension -in @('.dll','.exe','.config'))+@(Get-ChildItem $build -Filter 'Recovered.W3x*.dll')) {
    $tools+=@{name=$file.Name;path=$file.FullName;bytes=$file.Length;sha256=(Get-FileHash $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}
}
$tools | ConvertTo-Json -Depth 5 | Set-Content "$report/tools.json"
$runs=@();$outputs=@()
$savedTrace=$env:RECOVERED_W3X_TRACE_DIRECTORY
try {
    foreach($mode in @('Original','Recovered','RecoveredWithoutOriginal')) {
        $sdk=Join-Path $work $mode
        Copy-Item (Join-Path $root 'SDK') $sdk -Recurse
        # EALAModStudio/defaultscript.cs steps 2/5: fresh derived caches/output.
        # Shipped reference manifests remain identical in all disposable copies.
        Remove-Item (Join-Path $sdk 'builtmods/Mods') -Recurse -Force -ErrorAction SilentlyContinue
        Get-ChildItem $sdk -Recurse -Directory | Where-Object Name -in @('cache','buildcache','sessioncache') | Remove-Item -Recurse -Force
        Get-ChildItem $sdk -Recurse -File | Where-Object { $_.Extension -eq '.asset' -or $_.Name -like 'binaryassetbuilder.sessioncache.xml*' } | Remove-Item -Force
        $output=Join-Path $sdk 'builtmods'
        New-Item -ItemType Directory -Force $output | Out-Null
        $cfg=Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'
        [xml]$config=Get-Content $cfg -Raw
        $route=@($config.configuration.assetbuilder.plugins.plugin | Where-Object { $_.assetTypes.Split(',') -contains 'W3DAnimation' -or $_.assetTypes.Split(',') -contains 'W3DMesh' })
        if($route.Count -ne 1 -or $route[0].assetTypes -ne 'W3DAnimation,W3DMesh'){throw 'W3X SDK routing is not the expected isolated two-type route'}
        $originalRoutes=@($config.configuration.assetbuilder.plugins.plugin | ForEach-Object { "$($_.assetTypes)=$($_.targetType)" })
        $route[0].SetAttribute('useBuildCache','false')
        $route[0].SetAttribute('enabled','true')
        if($mode -ne 'Original') {
            $route[0].SetAttribute('targetType','Recovered.Compiler.W3x.NativePlugin, Recovered.W3x.Plugin')
            Copy-Item "$build/Recovered.W3x*.dll" (Join-Path $sdk 'tools')
        }
        if($mode -eq 'RecoveredWithoutOriginal') {
            Remove-Item (Join-Path $sdk 'tools/BinaryAssetBuilder.W3XCompiler.dll') -Force
            if(Test-Path (Join-Path $sdk 'tools/BinaryAssetBuilder.W3XCompiler.dll')){throw 'Original W3X DLL was not removed'}
        }
        $otherRoutes=@($config.configuration.assetbuilder.plugins.plugin | Where-Object { $_ -ne $route[0] } | ForEach-Object { "$($_.assetTypes)=$($_.targetType)" })
        foreach($line in $otherRoutes){if($originalRoutes -cnotcontains $line){throw "Non-W3X plugin routing changed: $line"}}
        if($config.configuration.startup){$config.configuration.RemoveChild($config.configuration.startup)|Out-Null}
        $startup=$config.CreateElement('startup');$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true')
        $supported=$config.CreateElement('supportedRuntime');$supported.SetAttribute('version','v4.0');$startup.AppendChild($supported)|Out-Null
        $config.configuration.AppendChild($startup)|Out-Null;$config.Save($cfg)
        Copy-Item $cfg "$report/$mode.config"
        $trace=Join-Path $report "$mode-trace"
        $env:RECOVERED_W3X_TRACE_DIRECTORY=if($mode -eq 'Original'){$null}else{$trace}
        if($mode -ne 'Original'){New-Item -ItemType Directory $trace | Out-Null}
        $mod=Join-Path $sdk "Mods/$ModName/Data/Mod.xml"
        if(!(Test-Path $mod)){throw "Missing SDK test MOD $mod"}
        # Exact BAB stage from EALAModStudio's defaultscript.cs case 5; no GUI.
        $arguments=@("`"$mod`"","/od:`"$output`"","/iod:`"$output`"",'/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true',"/art:`".;.\Mods\$ModName\Art;.\Mods;.\Art`"","/audio:`".;.\Mods\$ModName\Audio;.\Mods;.\Audio`"","/data:`".;.\Mods\$ModName\Data;.\Mods;.\SageXml`"")
        $command=@{mode=$mode;executable=(Join-Path $sdk 'tools/BinaryAssetBuilder.exe');arguments=$arguments;workingDirectory=$sdk;source='SDK/defaultscript.cs executeModBuildStep case 5';configSha256=(Get-FileHash $cfg -Algorithm SHA256).Hash.ToLowerInvariant();originalW3xPresent=(Test-Path (Join-Path $sdk 'tools/BinaryAssetBuilder.W3XCompiler.dll'))}
        $command | ConvertTo-Json -Depth 6 | Set-Content "$report/$mode-command.json"
        # Recheck the explicit closure immediately before starting the SDK.
        & "$PSScriptRoot/VerifyW3xSdkGate.ps1" -GateFile $GateFile
        $state.sdkStarted=$true;$state.status="running $mode";$state | ConvertTo-Json -Depth 6 | Set-Content "$report/validation.json"
        $process=Start-Process $command.executable -ArgumentList ($arguments -join ' ') -WorkingDirectory $sdk -PassThru -RedirectStandardOutput "$report/$mode-stdout.log" -RedirectStandardError "$report/$mode-stderr.log"
        if(!$process.WaitForExit($TimeoutSeconds*1000)){Stop-Process -Id $process.Id -Force;throw "$mode SDK timed out"}
        $process.Refresh();$command.exitCode=$process.ExitCode;$runs+=$command
        if($process.ExitCode -ne 0){Get-Content "$report/$mode-stdout.log" | Select-Object -Last 80;Get-Content "$report/$mode-stderr.log";throw "$mode SDK failed: $($process.ExitCode)"}
        foreach($ext in @('bin','manifest','imp','relo')) {
            $file=Join-Path $output "mods/$ModName/data/mod.$ext"
            if(!(Test-Path $file)){throw "$mode SDK did not emit mod.$ext"}
            Copy-Item $file "$report/$mode-mod.$ext"
            $outputs+=@{mode=$mode;file="mod.$ext";bytes=(Get-Item $file).Length;sha256=(Get-FileHash $file -Algorithm SHA256).Hash.ToLowerInvariant()}
        }
        if($mode -ne 'Original') {
            $env:RECOVERED_W3X_TRACE_DIRECTORY=$null
            Copy-Item "$host/W3xSdkOracle.exe" (Join-Path $sdk 'tools')
            Copy-Item "$host/W3xSdkOracle.exe.config" (Join-Path $sdk 'tools')
            $pluginFile=if($mode -eq 'Recovered'){Join-Path $host 'BinaryAssetBuilder.W3XCompiler.dll'}else{Join-Path $sdk 'tools/Recovered.W3x.Plugin.dll'}
            $verb=if($mode -eq 'Recovered'){'buffers'}else{'isolated'}
            $oracle=if($mode -eq 'Recovered'){Join-Path $host 'W3xSdkOracle.exe'}else{Join-Path $sdk 'tools/W3xSdkOracle.exe'}
            & $oracle $verb $pluginFile $trace "$report/$mode-mod.bin" $expected "$report/$mode-calls.tsv" *> "$report/$mode-buffer-oracle.log"
            if($LASTEXITCODE -ne 0){Get-Content "$report/$mode-buffer-oracle.log";throw "$mode actual SDK buffer replay/coverage failed"}
            Get-Content "$report/$mode-buffer-oracle.log"
        }
    }
    foreach($mode in @('Recovered','RecoveredWithoutOriginal'))foreach($ext in @('bin','manifest','imp','relo')) {
        if((Get-FileHash "$report/Original-mod.$ext" -Algorithm SHA256).Hash -ne (Get-FileHash "$report/$mode-mod.$ext" -Algorithm SHA256).Hash){throw "$mode SDK output differs: mod.$ext"}
    }
    $state.succeeded=$true;$state.status='Original/recovered/original-free fresh SDK builds; four exact outputs and actual plugin three-buffer/asset coverage verified';$state.componentSourceCommit=$component.sourceCommit;$state.pluginSourceCommit=$plugin.sourceCommit
} catch {
    $state.status='failed';$state.error=$_.Exception.Message
    throw
} finally {
    $env:RECOVERED_W3X_TRACE_DIRECTORY=$savedTrace
    $runs | ConvertTo-Json -Depth 8 | Set-Content "$report/commands.json"
    $outputs | ConvertTo-Json -Depth 6 | Set-Content "$report/outputs.json"
    $state | ConvertTo-Json -Depth 8 | Set-Content "$report/validation.json"
}
