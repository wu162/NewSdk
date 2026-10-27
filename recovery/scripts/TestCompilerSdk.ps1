#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=600,
    [ValidatePattern('^[0-9a-f]{40}$')][string]$BinarySourceCommit
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite SDK evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=$null;$oldPath=$env:PATH;$removedEnvironment=@{}
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $base=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$full=[IO.Path]::GetFullPath($Path)
    $full.Equals($base,[StringComparison]::OrdinalIgnoreCase) -or $full.StartsWith($base+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function Snapshot([string]$Directory){
    $result=@{};Get-ChildItem -LiteralPath $Directory -File -Recurse|ForEach-Object{$result[[IO.Path]::GetRelativePath($Directory,$_.FullName).Replace('\','/')]=@{sha256=(Hash $_.FullName);bytes=$_.Length}};$result
}
function Unchanged([string]$Directory,$Expected){
    $actual=Snapshot $Directory;Need ($actual.Count -eq $Expected.Count) "Input tree file count changed: $Directory"
    foreach($key in $Expected.Keys){Need ($actual.ContainsKey($key) -and $actual[$key].sha256 -ceq $Expected[$key].sha256 -and $actual[$key].bytes -eq $Expected[$key].bytes) "Input changed: $Directory/$key"}
}
function InvokeControlled([string]$Name,[string]$Exe,[string[]]$Arguments,[string]$Cwd,[string]$Events,[string]$CaseOut){
    New-Item -ItemType Directory -Path $CaseOut|Out-Null
    $request=@{exe=$Exe;cwd=$Cwd;arguments=$Arguments;stdout=(Join-Path $CaseOut 'stdout.log');stderr=(Join-Path $CaseOut 'stderr.log');events=$Events;cancel=$false;timeoutSeconds=$TimeoutSeconds}
    $requestPath=Join-Path $CaseOut 'request.json';$controlPath=Join-Path $CaseOut 'control.json'
    $request|ConvertTo-Json -Depth 5|Set-Content $requestPath
    & $script:control $requestPath $controlPath *> (Join-Path $CaseOut 'controller.log')
    $controlExit=$LASTEXITCODE;$audit=Get-Content -LiteralPath $controlPath -Raw|ConvertFrom-Json
    $row=@{name=$Name;request=$request;control=$audit;passed=$false};$script:state.processes+=,$row
    if($controlExit -ne 0 -or !$audit.succeeded -or $audit.processExitCode -ne 0){
        foreach($log in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $CaseOut $log) -Tail 35|Out-Host}
        throw "SDK process failed: $Name (exit $($audit.processExitCode))"
    }
    Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) "$Name launched a worker/descendant"
    Need (((Get-Content (Join-Path $CaseOut 'stdout.log'),(Join-Path $CaseOut 'stderr.log') -Raw) -join "`n") -notmatch '(?im)^\s*(Critical|Error):') "$Name logged a compiler error"
    $row.passed=$true;return $audit
}
function OutputInventory([string]$Directory){
    Need (Test-Path -LiteralPath $Directory -PathType Container) "Missing linked output root: $Directory"
    $files=@(Get-ChildItem -LiteralPath $Directory -File -Recurse);$accepted=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    $manifests=@($files|Where-Object{$_.Extension -ieq '.manifest'});Need ($manifests.Count -gt 0) 'SDK produced no stream manifest'
    foreach($manifest in $manifests){
        $stem=$manifest.FullName.Substring(0,$manifest.FullName.Length-9)
        foreach($extension in @('.manifest','.bin','.imp','.relo')){Need (Test-Path -LiteralPath ($stem+$extension) -PathType Leaf) "Incomplete linked stream: $stem$extension";$null=$accepted.Add($stem+$extension)}
    }
    $result=@{}
    foreach($file in $files){
        if(!$accepted.Contains($file.FullName)){
            Need ($file.Name -cmatch '^[0-9a-fA-F]{8}\.[0-9a-fA-F]{8}\.[0-9a-fA-F]{8}\.[0-9a-fA-F]{8}\.cdata$' -and $file.Directory.Name -ieq 'cdata' -and $accepted.Contains($file.Directory.Parent.FullName+'.manifest')) "Unlisted linked output: $($file.FullName)"
        }
        $relative=[IO.Path]::GetRelativePath($Directory,$file.FullName).Replace('\','/')
        Need (!$result.ContainsKey($relative)) 'Duplicate SDK output identity'
        $result[$relative]=@{path=$file.FullName;bytes=$file.Length;sha256=(Hash $file.FullName)}
    }
    return $result
}
function CompareOutputs($Original,$Product,[string]$TaskId){
    $rows=@()
    foreach($key in @( @($Original.Keys)+@($Product.Keys) | Sort-Object -Unique)){
        $left=$Original[$key];$right=$Product[$key]
        $identical=$null -ne $left -and $null -ne $right -and $left.bytes -eq $right.bytes -and $left.sha256 -ceq $right.sha256
        $firstDifference=$null
        if($left -and $right){
            $originalBytes=[IO.File]::ReadAllBytes($left.path);$productBytes=[IO.File]::ReadAllBytes($right.path)
            for($offset=0;$offset -lt [Math]::Min($originalBytes.Length,$productBytes.Length);$offset++){
                if($originalBytes[$offset] -ne $productBytes[$offset]){$firstDifference=$offset;break}
            }
            if($null -eq $firstDifference -and $originalBytes.Length -ne $productBytes.Length){$firstDifference=[Math]::Min($originalBytes.Length,$productBytes.Length)}
            $identical=$identical -and $null -eq $firstDifference
        }
        # Preserve every raw difference across both modes, without normalization.
        $rows+=,@{relativePath=$key;original=$left;product=$right;byteIdentical=[bool]$identical;firstDifference=$firstDifference}
    }
    return $rows
}
$script:state=@{schemaVersion=1;succeeded=$false;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;processes=@();modes=@();originalCases=@();originalBaselineSucceeded=$false;scope='Complete unchanged repository SDK/Mods/mymod source, base/apt/maps discovery, one production build process per Debug/Release configuration, original BAB raw four-stream and exact CDATA output comparison. Empty optional groups remain empty; no synthetic APT/map coverage or complete P5/G3 acceptance is inferred.'}
try {
    Need (!!$env:RUNNER_TEMP) 'A Windows runner temporary directory is required'
    $state.sourceCommit=(git -C $root rev-parse HEAD).Trim();Need ($LASTEXITCODE -eq 0) 'Cannot identify SDK source'
    $state.scriptSha256=Hash $PSCommandPath
    $state.testSourceCommit=$state.sourceCommit
    if(!$BinarySourceCommit){$BinarySourceCommit=$state.sourceCommit}
    $state.binarySourceCommit=$BinarySourceCommit
    if($BinarySourceCommit -cne $state.sourceCommit){
        # Reuse the exact replay closure and fixed archive guard; a caller-supplied
        # older commit alone never authorizes historic production runtime bytes.
        $guard=@'
import json, pathlib, sys
root, binary, candidate, product = sys.argv[1:]
sys.path.insert(0, str(pathlib.Path(root) / 'recovery/scripts'))
import PrepareCompilerSessionReplay as replay
assert binary == replay.SOURCE, 'SDK reuse source is not the fixed session build'
changed = replay.closure(root, binary, candidate)
receipt = json.loads((pathlib.Path(product) / 'session-replay.json').read_text(encoding='utf-8-sig'))
assert receipt['prepared'] is True and receipt['candidateValidated'] is False
assert receipt['productionSourcesUnchanged'] is True
assert receipt['binarySourceCommit'] == binary and receipt['testSourceCommit'] == candidate
assert receipt['sourceCommit'] == binary and receipt['candidateCommit'] == candidate
assert str(receipt['runId']) == str(replay.RUN) and str(receipt['runAttempt']) == str(replay.ATTEMPT)
assert receipt['archiveSha256'] == replay.SHA256 and receipt['artifactId'] == replay.ARTIFACT
print(json.dumps({'changedPaths': changed, 'receipt': receipt}))
'@
        $verified=& python -c $guard $root $BinarySourceCommit $state.sourceCommit $CompilerDirectory
        Need ($LASTEXITCODE -eq 0) 'SDK historical binary input/archive closure rejected'
        $state.runtimeReuse=$verified|ConvertFrom-Json
    }

    $sdk=(Resolve-Path -LiteralPath (Join-Path $root 'SDK')).Path;$mod=(Resolve-Path -LiteralPath (Join-Path $sdk 'Mods/mymod')).Path
    $sdkBefore=Snapshot $sdk;$state.sdkInputs=$sdkBefore
    $state.sdkGitTree=(git -C $root rev-parse HEAD:SDK).Trim();$state.modGitTree=(git -C $root rev-parse HEAD:SDK/Mods/mymod).Trim()
    Need ((Test-Path -LiteralPath (Join-Path $mod 'Data/Mod.xml')) -and (Test-Path -LiteralPath (Join-Path $sdk 'SageXml/Static.xml'))) 'Full SDK/MOD source is unavailable'
    $tasks=@(@{id='base/Mod';content='base';inputPath=(Join-Path $mod 'Data/Mod.xml')});$skipped=@()
    foreach($selection in @(@{content='apt';directory='Data/AptUI';filter='*.xml'},@{content='maps';directory='Data/AdditionalMaps';filter='MapMetaData_*.xml'})){
        $directory=Join-Path $mod $selection.directory;$entries=@()
        if(Test-Path -LiteralPath $directory){$entries=@(Get-ChildItem -LiteralPath $directory -File -Filter $selection.filter|Sort-Object FullName)}
        if(!$entries.Count){$skipped+=$selection.content}
        foreach($entry in $entries){$tasks+=@{id=($selection.content+'/'+$entry.BaseName);content=$selection.content;inputPath=$entry.FullName}}
    }
    $state.selectedTasks=$tasks;$state.emptyOptionalGroups=$skipped
    $short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('g'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    $controlReceipt=Get-Content (Join-Path $out 'controller/build.json') -Raw|ConvertFrom-Json
    Need ($controlReceipt.succeeded -and $controlReceipt.sourceCommit -ceq $state.sourceCommit -and $controlReceipt.controllerSha256 -ceq (Hash $script:control)) 'SDK controller identity mismatch'
    $state.controller=$controlReceipt
    $env:PATH=(@($oldPath -split ';'|Where-Object{$_ -notmatch '[\\/]SDK[\\/]tools'}) -join ';')
    foreach($item in @(Get-ChildItem Env:|Where-Object{$_.Name -like 'RA3_RECOVERED_*' -or $_.Name -eq 'RECOVERED_W3X_TRACE_DIRECTORY'})){$removedEnvironment[$item.Name]=$item.Value;[Environment]::SetEnvironmentVariable($item.Name,$null,'Process')}
    # The original oracle runs first with untouched original plugin routes.
    $oracleTools=(New-Item -ItemType Directory -Path (Join-Path $short 't')).FullName
    Copy-Item -Path (Join-Path $sdk 'tools/*') -Destination $oracleTools -Recurse
    $originalExe=Join-Path $oracleTools 'BinaryAssetBuilder.exe';$config=Join-Path $oracleTools 'BinaryAssetBuilder.exe.config'
    [xml]$xml=[IO.File]::ReadAllText((Join-Path $sdk 'tools/BinaryAssetBuilder.exe.config'))
    $section=$xml.SelectSingleNode('/configuration/assetbuilder');Need ($null -ne $section) 'Original SDK configuration has no assetbuilder settings'
    $schema=Join-Path $sdk 'Schemas/xsd/CnC3Types.xsd'
    # defaultscript.cs steps 3/4/5: SDK current directory, MOD, Mods, shared assets.
    $data=$sdk+';'+(Join-Path $mod 'Data')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'SageXml')
    $art=$sdk+';'+(Join-Path $mod 'Art')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Art')
    $audio=$sdk+';'+(Join-Path $mod 'Audio')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Audio')
    $legacyScript=Join-Path $sdk 'defaultscript.cs'
    $legacyCalls=@(Get-Content -LiteralPath $legacyScript|Where-Object{$_ -match '/csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true'})
    Need ($legacyCalls.Count -eq 3) 'Legacy base/APT/maps compiler argument contract changed'
    $state.legacyParameterContract=@{script='SDK/defaultscript.cs';sha256=(Hash $legacyScript);matchingCalls=3;cacheException=@('/bc:false','/sc:false','/psc:false','/csc:false','/fsc:false','/atc:false');enabled=@('/ls:true','/pc:true','/res:true','/slowclean:true','/ss:true');searchPaths=@{data=$data;art=$art;audio=$audio};originalBuildConfigurationsPreserved=$true;outputIsolation='Fresh separate output/intermediate/hash/cache directories; no input SDK changes'}

    $section.SetAttribute('schema',$schema);$section.SetAttribute('dataRoot',$sdk);$section.SetAttribute('defaultDataPaths',$data);$section.SetAttribute('defaultArtPaths',$art);$section.SetAttribute('defaultAudioPaths',$audio)
    # Keep the original LowLOD/MediumLOD configuration table from the SDK script environment.
    $startup=$xml.SelectSingleNode('/configuration/startup');if(!$startup){$startup=$xml.CreateElement('startup');$null=$xml.configuration.AppendChild($startup)}
    $startup.RemoveAll();$startup.SetAttribute('useLegacyV2RuntimeActivationPolicy','true');$version=$xml.CreateElement('supportedRuntime');$version.SetAttribute('version','v4.0');$null=$startup.AppendChild($version);$xml.Save($config)
    $state.originalRuntime=Snapshot $oracleTools;Copy-Item -LiteralPath $config -Destination (Join-Path $out 'original-effective.config')
    $originals=@{};$index=0
    foreach($task in $tasks){
        $caseOut=Join-Path $out ('original-'+$index);$work=Join-Path $short ('o'+$index);$streams=Join-Path $work 's'
        New-Item -ItemType Directory -Path $streams,(Join-Path $work 'i'),(Join-Path $work 'h'),(Join-Path $work 'c')|Out-Null
        $arguments=@($task.inputPath,"/sp:$schema","/dr:$sdk","/od:$streams",("/iod:"+(Join-Path $work 'i')),("/scd:"+(Join-Path $work 'h')),("/bcp:"+(Join-Path $work 'c')),
            '/tp:Win32','/sc:false','/bc:false','/psc:false','/fsc:false','/csc:false','/atc:false','/sh:false','/pc:true','/res:true','/gui:false','/poe:false','/mr:false','/oof:false','/sf:false','/ls:true','/ss:true','/oix:false','/vf:false','/osh:false','/oar:false','/slowclean:true','/tl:3','/el:0',"/art:$art","/audio:$audio","/data:$data")
        $null=InvokeControlled ('original/'+$task.id) $originalExe $arguments $sdk (Join-Path $work 'unused.events.jsonl') $caseOut
        $null=OutputInventory $streams
        Copy-Item -LiteralPath $streams -Destination (Join-Path $caseOut 'streams') -Recurse
        $originals[$task.id]=OutputInventory (Join-Path $caseOut 'streams')
        $state.originalCases+=,@{taskId=$task.id;input=$task.inputPath;outputs=$originals[$task.id]}
        $index++
    }
    $state.originalBaselinePassed=$true;$state.originalBaselineSucceeded=$true
    foreach($mode in @('Debug','Release')){
        $modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName;$runtime=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName
        $buildPath=Join-Path $CompilerDirectory "$mode/build.json";$build=Get-Content -LiteralPath $buildPath -Raw|ConvertFrom-Json
        Need ($build.sourceCommit -ceq $BinarySourceCommit -and $build.configuration -ceq $mode -and !$build.containsProbeOrTestSources -and $build.backendIntegrated) 'SDK acceptance requires current clean production build receipts'
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime}
        $exe=Join-Path $runtime 'NewModCompiler.exe';Need ((Hash $exe) -ceq $build.exeSha256) 'Production EXE differs from current build receipt'
        if($state.runtimeReuse){
            foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){
                $identity=@($state.runtimeReuse.receipt.files|Where-Object{$_.path -ceq ($mode+'/bin/'+$file)})
                Need ($identity.Count -eq 1 -and $identity[0].sha256 -ceq (Hash (Join-Path $runtime $file))) 'SDK runtime differs from verified historical bytes'
            }
        }
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> (Join-Path $modeOut 'prepare-texture.log');Need ($LASTEXITCODE -eq 0) 'SDK texture deployment failed'
        $patch=Get-Content (Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll.patch.json') -Raw|ConvertFrom-Json
        Need ($patch.targetExeSha256 -ceq (Hash $exe) -and $patch.patchedSha256 -ceq (Hash (Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll')) -and $patch.protectedPeAudit.status -ceq 'passed') 'Texture runtime does not bind production EXE'
        Need (@(Get-ChildItem -LiteralPath $runtime -File).Count -eq 5) 'SDK production deployment is not the exact five-file runtime'
        $runtimeBefore=Snapshot $runtime;$destination=Join-Path $short ($mode.Substring(0,1)+'out');New-Item -ItemType Directory -Path $destination|Out-Null
        $arguments=@('build','--mod',$mod,'--sdk-root',$sdk,'--out',$destination,'--package-content','base,apt,maps','--package-format','loose','/sc:false','/bc:false',"/data:$data","/art:$art","/audio:$audio")
        $audit=InvokeControlled $mode $exe $arguments $runtime (Join-Path $destination 'build.events.jsonl') (Join-Path $modeOut 'process')
        $report=Get-Content -LiteralPath (Join-Path $destination 'build-report.json') -Raw|ConvertFrom-Json
        $events=@(Get-Content -LiteralPath (Join-Path $destination 'build.events.jsonl')|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        $modeState=@{configuration=$mode;succeeded=$false;buildReceipt=$build;runtime=$runtimeBefore;control=$audit;comparisons=@();report=$report;eventCount=$events.Count};$state.modes+=,$modeState
        Need ($report.exitCode -eq 0 -and $report.status -ceq 'succeeded' -and $report.publicationStatus -ceq 'PublishedDirectory' -and $report.executionMode -ceq 'InProcess' -and @($report.externalTools).Count -eq 0) 'SDK production report did not confirm in-process publication'
        Need (@($report.plan.Tasks).Count -eq $tasks.Count -and @($events|Where-Object{$_.eventType -eq 'StreamStarted'}).Count -eq $tasks.Count -and @($events|Where-Object{$_.eventType -eq 'StreamCompleted'}).Count -eq $tasks.Count) 'SDK task count changed'
        Need (@($events|Where-Object{$_.eventType -eq 'BuildStarted'}).Count -eq 1 -and @($events|Where-Object{$_.eventType -eq 'BuildFinished'}).Count -eq 1 -and $events[-1].eventType -ceq 'BuildFinished') 'SDK protocol terminal count/order changed'
        Need ($events[0].payload.processId -eq $audit.processId -and $events[-1].payload.exitCode -eq 0 -and $events[-1].payload.status -ceq 'succeeded') 'SDK protocol belongs to another process/outcome'
        Copy-Item -LiteralPath $destination -Destination (Join-Path $modeOut 'output') -Recurse
        for($i=0;$i -lt $events.Count;$i++){Need ($events[$i].sequence -eq $i+1 -and $events[$i].buildId -ceq $report.buildId) 'SDK protocol correlation/sequence changed'}
        foreach($empty in $skipped){Need (@($report.plan.Skipped|Where-Object{$_.Content -ceq $empty}).Count -eq 1) 'SDK empty optional group was not explicitly reported'}
        $accepted=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        foreach($task in $tasks){
            Need (@($report.plan.Tasks|Where-Object{$_.Id -ceq $task.id -and $_.InputPath -ceq $task.inputPath}).Count -eq 1) 'SDK task identity/input differs from original'
            $streams=Join-Path $destination ('published/'+$task.content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.inputPath)+'/streams')
            $product=OutputInventory $streams;$modeState.comparisons+=,@{taskId=$task.id;outputs=@(CompareOutputs $originals[$task.id] $product $task.id)}
            foreach($value in $product.Values){$null=$accepted.Add($value.path)}
        }
        Need (@($report.artifacts).Count -eq $accepted.Count -and @(Get-ChildItem -LiteralPath (Join-Path $destination 'published') -File -Recurse).Count -eq $accepted.Count) 'SDK publication artifact allowlist differs from all raw outputs'
        foreach($artifact in $report.artifacts){Need ($accepted.Remove([string]$artifact.Path) -and (Within (Join-Path $destination 'published') $artifact.Path) -and (Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'SDK reported artifact path/identity differs'}
        Need ($accepted.Count -eq 0 -and !(Test-Path -LiteralPath (Join-Path $destination ('.build/'+$report.buildId)))) 'SDK publication retained staging or omitted output'
        Unchanged $runtime $runtimeBefore
        $modeState.succeeded=@($modeState.comparisons.outputs|Where-Object{!$_.byteIdentical}).Count -eq 0
    }
    Unchanged $oracleTools $state.originalRuntime;Unchanged $sdk $sdkBefore;$state.inputsUnchanged=$true
    Need (@($state.modes).Count -eq 2 -and @($state.modes|Where-Object{!$_.succeeded}).Count -eq 0) 'Complete SDK differential failed; see per-file raw comparisons in validation.json'
    $state.succeeded=$true
    $global:LASTEXITCODE=0
} catch {$state.error=$_.Exception.ToString();throw}
finally {
    if($sdk -and $sdkBefore){
        try{Unchanged $sdk $sdkBefore;$state.inputsUnchanged=$true}
        catch{$state.inputsUnchanged=$false;$state.succeeded=$false;$state.inputVerificationError=$_.Exception.ToString();if(!$state.error){$verificationFailure=$_}}
    }
    $env:PATH=$oldPath;foreach($key in $removedEnvironment.Keys){[Environment]::SetEnvironmentVariable($key,$removedEnvironment[$key],'Process')}
    if($short -and (Test-Path -LiteralPath $short)){try{Remove-Item -LiteralPath $short -Recurse -Force;$state.temporaryRootRemoved=$true}catch{$state.temporaryRootRemoved=$false;$state.temporaryCleanupError=$_.Exception.ToString()}}
    $state|ConvertTo-Json -Depth 20|Set-Content (Join-Path $out 'validation.json')
    if($verificationFailure){throw $verificationFailure}
}
