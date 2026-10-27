[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(30,900)][int]$FixtureTimeoutSeconds=180,
    [string]$ReuseReceipt
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite Core lifecycle evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Force $OutputDirectory).FullName
$work=(New-Item -ItemType Directory (Join-Path ([IO.Path]::GetTempPath()) ('clr-'+[Guid]::NewGuid().ToString('N').Substring(0,12)))).FullName
$oldPath=$env:PATH
$state=[ordered]@{
    schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT
    succeeded=$false;status='started';modes=@();sdkValidated=$false;g3Validated=$false;coldProcessValidated=$false;longPathsValidated=$false
    scope='P5 targeted real Core/backend/application pipeline and resource reuse in production-object validation fixtures using small Armor inputs; not production build CLI, APT business coverage, cold-process equivalence, complete MOD/SDK or G3 acceptance'
}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Invoke-LifecycleFixture([string]$Executable,[string[]]$Arguments,[string]$Directory,[string]$Log,[int]$Timeout) {
    $info=[Diagnostics.ProcessStartInfo]::new($Executable)
    $info.WorkingDirectory=$Directory;$info.UseShellExecute=$false
    $info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
    foreach($arg in $Arguments){$info.ArgumentList.Add($arg)}
    $process=[Diagnostics.Process]::new();$process.StartInfo=$info
    $stdout=$null;$stderr=$null
    try {
        if(!$process.Start()){throw 'Unable to start Core lifecycle fixture'}
        $processId=$process.Id
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        $timedOut=!$process.WaitForExit($Timeout*1000)
        if($timedOut){$process.Kill($true);$process.WaitForExit()}
        [IO.File]::WriteAllText($Log,($stdout.GetAwaiter().GetResult()+$stderr.GetAwaiter().GetResult()))
        if($timedOut -or $process.ExitCode -ne 0){
            Get-Content $Log -Tail 100|Out-Host
            throw "Core lifecycle fixture failed: timeout=$timedOut exit=$($process.ExitCode)"
        }
        return $processId
    } finally {
        if($stdout -and !$process.HasExited){$process.Kill($true);$process.WaitForExit()}
        $process.Dispose()
    }
}
Push-Location $root
try {
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $state.binarySourceCommit=$state.sourceCommit
    if($ReuseReceipt){
        $reuse=Get-Content -LiteralPath $ReuseReceipt -Raw|ConvertFrom-Json
        if($reuse.prepared -ne $true -or $reuse.binarySourceCommit -cne '64bfcc80cec7860e69f0ffbc5cb0e04c69baec87' -or
           $reuse.testSourceCommit -cne $state.sourceCommit -or [string]$reuse.sourceRunId -cne '40524897974'){
            throw 'Targeted runtime reuse receipt identity mismatch'
        }
        $state.binarySourceCommit=$reuse.binarySourceCommit
        $state.sourceRunId=$reuse.sourceRunId
        $state.reuseReceiptSha256=Hash $ReuseReceipt
    }
    $state.workRoot=$work
    if(!$state.runId){$state.runId='local-'+[Guid]::NewGuid().ToString('N')}
    $state.runnerSha256=Hash $PSCommandPath
    $sdk=(Resolve-Path 'SDK').Path
    $schemas=(Resolve-Path 'SDK/Schemas/xsd').Path
    $state.schemaInputs=@(Get-ChildItem -LiteralPath $schemas -Recurse -File|Sort-Object FullName|ForEach-Object{
        @{path=[IO.Path]::GetRelativePath($sdk,$_.FullName).Replace('\','/');sha256=(Hash $_.FullName)}
    })
    if($state.schemaInputs.Count -eq 0){throw 'Real SDK schema input inventory is empty'}
    $fixtureFailures=@()
    $fixtures=@(
        @{name='CompilerInputDigests';marker='PASS InputDigests:'},
        @{name='CompilerHashingWriter';marker='PASS HashingWriter:';timeoutSeconds=600},
        @{name='CompilerPhysicalInputs';marker='PASS PhysicalInputs:'},
        @{name='CompilerManifestInputs';marker='PASS ManifestInputs:'},
        @{name='CompilerStreamBindings';marker='PASS StreamBindings:';timeoutSeconds=600},
        @{name='CompilerTransferBuffers';marker='PASS TransferBuffers:';timeoutSeconds=600},
        @{name='CompilerBinaryMaterialization';marker='PASS BinaryMaterialization:';timeoutSeconds=600},
        @{name='CompilerDeclarationSnapshots';marker='PASS DeclarationSnapshots:';timeoutSeconds=600},
        @{name='CompilerDocumentSnapshots';marker='PASS DocumentSnapshots:';timeoutSeconds=600},
        @{name='CompilerDocumentMetadata';marker='PASS DocumentMetadata:';timeoutSeconds=600},
        @{name='CompilerUpheavalSamples';marker='PASS UpheavalSamples:';timeoutSeconds=600},
        @{name='CompilerStableSortMembership';marker='PASS StableSortMembership:';timeoutSeconds=600},
        @{name='CompilerStableSortStreams';marker='PASS StableSortStreams:';timeoutSeconds=600},
        @{name='CompilerExpressionPrograms';marker='PASS ExpressionPrograms:';timeoutSeconds=600},
        @{name='CompilerDependencyObservations';marker='PASS DependencyObservations:'},
        @{name='CompilerBaseManifestIndex';marker='PASS BaseManifestIndex:'},
        @{name='CompilerNodeJoiner';marker='PASS NodeJoiner:'},
        @{name='CompilerSymbolLookup';marker='PASS SymbolLookup:'},
        @{name='CompilerSchemaDescriptors';marker='PASS SchemaDescriptors:'},
        @{name='CompilerSchemaMetadata';marker='PASS SchemaMetadata:'},
        @{name='CompilerCacheContent';marker='PASS CacheContent:'},
        @{name='CompilerCacheBoundary';marker='PASS CacheBoundary:'},
        @{name='CompilerInputRoots';marker='PASS InputRoots:'},
        @{name='CompilerCorePaths';marker='PASS CorePaths:'},
        @{name='CompilerUnicodeSettings';marker='PASS UnicodeSettings:'},
        @{name='CompilerCoreLifecycle';marker='PASS CoreLifecycle:'},
        @{name='CompilerResourceLifecycle';marker='PASS resource lifecycle'},
        @{name='CompilerStateLifecycle';marker='PASS Hash/Tracer state lifecycle'},
        @{name='CompilerBackendLifecycle';marker='PASS BackendLifecycle:'},
        @{name='CompilerApplicationBuild';marker='PASS ApplicationBuild:';timeoutSeconds=600}
    )
    $state.fixtures=@($fixtures|ForEach-Object{
        @{name=$_.name;path=('recovery/tests/'+$_.name+'.cs');sha256=(Hash ('recovery/tests/'+$_.name+'.cs'))}
    })
    $state.upheavalSampleInputs=@(Get-ChildItem -LiteralPath 'recovery/tests/fixtures/UpheavalSmall' -Recurse -File|Sort-Object FullName|ForEach-Object{
        @{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Hash $_.FullName)}
    })
    $state.sdkFixtureInputs=@('Mods/mymod/Data/RecoveryArmorProbe.xml','tools/BinaryAssetBuilder.exe.config'|ForEach-Object{
        @{path=$_;sha256=(Hash (Join-Path $sdk $_))}
    })
    $originalTexture=Join-Path $sdk 'tools/BinaryAssetBuilder.TextureCompiler.dll'
    $state.originalTextureSha256=Hash $originalTexture
    if($state.originalTextureSha256 -cne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Backend texture input is not the pinned audited original'}
    # Match the App build's Roslyn/NET48 toolchain. The legacy Framework csc
    # silently assumes a much older C# language version and is not this recipe.
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    if(!(Test-Path -LiteralPath $vswhere)){throw 'VS2022 Build Tools with Roslyn are required'}
    $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    if($LASTEXITCODE -ne 0 -or !$vs){throw 'Cannot locate VS Roslyn compiler'}
    $csc=Join-Path $vs 'MSBuild/Current/Bin/Roslyn/csc.exe'
    $refs=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
    if(!(Test-Path -LiteralPath $csc) -or !(Test-Path -LiteralPath (Join-Path $refs 'mscorlib.dll'))){throw 'Roslyn or .NET Framework 4.8 reference assemblies missing'}
    $manifest=Get-Content recovery/build/Managed.Foundation.json -Raw|ConvertFrom-Json
    $frameworkReferences=@(@($manifest.frameworkReferences)+@('System.Web.Extensions')|Select-Object -Unique)
    $state.compiler=@{path=$csc;sha256=(Hash $csc);version=([Diagnostics.FileVersionInfo]::GetVersionInfo($csc).FileVersion);languageVersion='7.3';target='net48/x86'}
    $state.frameworkReferences=@($frameworkReferences|ForEach-Object{@{name=$_;sha256=(Hash (Join-Path $refs ($_+'.dll')))}})
    $env:PATH=(@($oldPath -split ';'|Where-Object{$_ -notmatch '[\\/]SDK[\\/]tools'}) -join ';')
    # Compile the same expression corpus against original SDK assemblies in a
    # separate process/directory; never load originals into the product host.
    $expressionOracleOut=(New-Item -ItemType Directory (Join-Path $out 'expression-original')).FullName
    $expressionOracleRuntime=(New-Item -ItemType Directory (Join-Path $work 'expression-original-runtime')).FullName
    Get-ChildItem (Join-Path $sdk 'tools') -Filter '*.dll' -File|Copy-Item -Destination $expressionOracleRuntime
    $expressionOracleSource=(Resolve-Path 'recovery/tests/CompilerExpressionPrograms.cs').Path
    $expressionOracleExe=Join-Path $expressionOracleRuntime 'CompilerExpressionOriginal.exe'
    $expressionOracleArguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86','/define:EXPRESSION_ORIGINAL',('/out:'+$expressionOracleExe))
    foreach($name in $frameworkReferences){$expressionOracleArguments+=('/r:'+(Join-Path $refs ($name+'.dll')))}
    foreach($name in @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.ExpressionEval.dll','BinaryAssetBuilder.Utility.dll','antlr.runtime.dll')){
        $expressionOracleArguments+=('/r:'+(Join-Path $expressionOracleRuntime $name))
    }
    $expressionOracleArguments+=$expressionOracleSource
    & $csc @expressionOracleArguments *> "$expressionOracleOut/oracle-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$expressionOracleOut/oracle-build.log" -Tail 100|Out-Host;throw 'Original expression corpus compilation failed'}
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$expressionOracleExe.config"
    $expressionOriginalJson=Join-Path $expressionOracleOut 'transcript.json'
    $expressionOracleProcess=Invoke-LifecycleFixture $expressionOracleExe @($expressionOriginalJson) $expressionOracleRuntime "$expressionOracleOut/oracle-runtime.log" 180
    if(!(Select-String -LiteralPath "$expressionOracleOut/oracle-runtime.log" -SimpleMatch 'PASS ExpressionOriginal:' -Quiet)){throw 'Original expression success marker missing'}
    $state.expressionOriginal=@{
        sourceCommit=$state.sourceCommit;runId=$state.runId;runAttempt=$state.runAttempt;sourcePath='recovery/tests/CompilerExpressionPrograms.cs'
        sourceSha256=(Hash $expressionOracleSource);compilerSha256=(Hash $csc);executableSha256=(Hash $expressionOracleExe);configSha256=(Hash "$expressionOracleExe.config")
        processId=$expressionOracleProcess;transcriptSha256=(Hash $expressionOriginalJson);rows=@(Get-Content $expressionOriginalJson -Raw|ConvertFrom-Json).Count
        runtimeInputs=@(Get-ChildItem $expressionOracleRuntime -Filter '*.dll' -File|Sort-Object Name|ForEach-Object{@{name=$_.Name;sha256=(Hash $_.FullName)}})
        scope='Original SDK expression corpus in a separate original-only directory and process; no original Core/ExpressionEval assembly enters a production host'
    }
    # Keep the document ownership oracle in the same original-only runtime,
    # with independent source, process, work tree and evidence identity.
    $documentOracleOut=(New-Item -ItemType Directory (Join-Path $out 'document-original')).FullName
    $documentOracleWork=(New-Item -ItemType Directory (Join-Path $work 'document-original-work')).FullName
    $documentOracleSource=(Resolve-Path 'recovery/tests/CompilerDocumentSnapshotOracle.cs').Path
    $documentOracleExe=Join-Path $expressionOracleRuntime 'CompilerDocumentSnapshotOriginal.exe'
    $documentOracleArguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86',('/out:'+$documentOracleExe))
    foreach($name in $frameworkReferences){$documentOracleArguments+=('/r:'+(Join-Path $refs ($name+'.dll')))}
    foreach($name in @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.ExpressionEval.dll','BinaryAssetBuilder.Utility.dll','antlr.runtime.dll')){
        $documentOracleArguments+=('/r:'+(Join-Path $expressionOracleRuntime $name))
    }
    $documentOracleArguments+=$documentOracleSource
    & $csc @documentOracleArguments *> "$documentOracleOut/oracle-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$documentOracleOut/oracle-build.log" -Tail 100|Out-Host;throw 'Original document ownership oracle compilation failed'}
    Copy-Item "$expressionOracleExe.config" "$documentOracleExe.config"
    Copy-Item $documentOracleExe "$documentOracleOut/oracle.exe"
    Copy-Item "$documentOracleExe.config" "$documentOracleOut/oracle.exe.config"
    $documentOriginalJson=Join-Path $documentOracleOut 'transcript.json'
    $documentOracleProcess=Invoke-LifecycleFixture $documentOracleExe @($documentOracleWork,$documentOriginalJson,$state.sourceCommit,$state.runId,'original',$sdk) $expressionOracleRuntime "$documentOracleOut/oracle-runtime.log" 180
    if(!(Select-String -LiteralPath "$documentOracleOut/oracle-runtime.log" -SimpleMatch 'PASS DocumentSnapshotOracle: original' -Quiet)){throw 'Original document oracle success marker missing'}
    $documentOriginal=Get-Content $documentOriginalJson -Raw|ConvertFrom-Json
    $documentOriginalCore=Join-Path $expressionOracleRuntime 'BinaryAssetBuilder.Core.dll'
    if($documentOriginal.succeeded -ne $true -or $documentOriginal.mode -cne 'original' -or
       $documentOriginal.sourceCommit -cne $state.sourceCommit -or [string]$documentOriginal.runId -cne [string]$state.runId -or
       $documentOriginal.sameDeclaration -ne $true -or $documentOriginal.sourceXmlChanged -ne $true -or
       @($documentOriginal.expressionTraversal).Count -ne 40 -or
       @($documentOriginal.inheritance).Count -ne 9 -or @($documentOriginal.inputReaders).Count -ne 6 -or
       @($documentOriginal.defineLookups).Count -ne 52 -or @($documentOriginal.semanticDomRestores).Count -ne 3 -or
       $documentOriginal.assemblySha256Base64 -cne [Convert]::ToBase64String([Convert]::FromHexString((Hash $documentOriginalCore)))){
        throw 'Original document oracle identity or historical shared-state outcome mismatch'
    }
    $state.documentOriginal=@{
        sourceCommit=$state.sourceCommit;runId=$state.runId;runAttempt=$state.runAttempt;sourcePath='recovery/tests/CompilerDocumentSnapshotOracle.cs'
        sourceSha256=(Hash $documentOracleSource);compilerSha256=(Hash $csc);executableSha256=(Hash $documentOracleExe);configSha256=(Hash "$documentOracleExe.config")
        processId=$documentOracleProcess;transcriptSha256=(Hash $documentOriginalJson);coreSha256=(Hash $documentOriginalCore)
        sameDeclaration=$documentOriginal.sameDeclaration;sourceXmlChanged=$documentOriginal.sourceXmlChanged
        metadata=$documentOriginal.metadata;expressionTraversalRows=@($documentOriginal.expressionTraversal).Count
        inheritanceRows=@($documentOriginal.inheritance).Count;inputReaderRows=@($documentOriginal.inputReaders).Count;defineLookupRows=@($documentOriginal.defineLookups).Count;semanticDomRestoreRows=@($documentOriginal.semanticDomRestores).Count
        runtimeInputs=$state.expressionOriginal.runtimeInputs
        scope='Original SDK public document metadata, expression-DOM traversal, inheritance stages and APM/file-hash boundary corpora in a separate original-only process; direct reproducer does not establish a production CLI fault'
    }
    $hashingOracleOut=(New-Item -ItemType Directory (Join-Path $out 'hashing-original')).FullName
    $hashingOracleSource=(Resolve-Path 'recovery/tests/CompilerHashingWriter.cs').Path
    $hashingOracleExe=Join-Path $expressionOracleRuntime 'CompilerHashingOriginal.exe'
    $hashingOracleArguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86','/define:HASHING_ORIGINAL',('/out:'+$hashingOracleExe))
    foreach($name in $frameworkReferences){$hashingOracleArguments+=('/r:'+(Join-Path $refs ($name+'.dll')))}
    foreach($name in @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','antlr.runtime.dll')){$hashingOracleArguments+=('/r:'+(Join-Path $expressionOracleRuntime $name))}
    $hashingOracleArguments+=$hashingOracleSource
    & $csc @hashingOracleArguments *> "$hashingOracleOut/oracle-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$hashingOracleOut/oracle-build.log" -Tail 100|Out-Host;throw 'Original HashingWriter oracle compilation failed'}
    Copy-Item "$expressionOracleExe.config" "$hashingOracleExe.config"
    Copy-Item $hashingOracleExe "$hashingOracleOut/oracle.exe"
    Copy-Item "$hashingOracleExe.config" "$hashingOracleOut/oracle.exe.config"
    $hashingOriginalJson=Join-Path $hashingOracleOut 'transcript.json'
    $hashingOracleProcess=Invoke-LifecycleFixture $hashingOracleExe @($hashingOriginalJson,$state.sourceCommit,$state.runId) $expressionOracleRuntime "$hashingOracleOut/oracle-runtime.log" 180
    $hashingOriginal=Get-Content $hashingOriginalJson -Raw|ConvertFrom-Json
    if($hashingOriginal.succeeded -ne $true -or $hashingOriginal.sourceCommit -cne $state.sourceCommit -or
       [string]$hashingOriginal.runId -cne [string]$state.runId -or @($hashingOriginal.rows).Count -le 850 -or
       $hashingOriginal.assemblySha256Base64 -cne [Convert]::ToBase64String([Convert]::FromHexString((Hash $documentOriginalCore)))){
        throw 'Original HashingWriter oracle identity/corpus mismatch'
    }
    $state.hashingOriginal=@{
        sourceCommit=$state.sourceCommit;runId=$state.runId;runAttempt=$state.runAttempt;sourcePath='recovery/tests/CompilerHashingWriter.cs'
        sourceSha256=(Hash $hashingOracleSource);compilerSha256=(Hash $csc);executableSha256=(Hash $hashingOracleExe);configSha256=(Hash "$hashingOracleExe.config")
        processId=$hashingOracleProcess;transcriptSha256=(Hash $hashingOriginalJson);coreSha256=(Hash $documentOriginalCore);rows=@($hashingOriginal.rows).Count
        runtimeInputs=$state.expressionOriginal.runtimeInputs
        scope='Exact writer-operation/public XML hash corpus in independent original-only Core process; UTF-16 inputs generated in-process to preserve unpaired surrogate test data'
    }
    foreach($mode in @('Debug','Release')) {
        $modeOut=(New-Item -ItemType Directory (Join-Path $out $mode)).FullName
        $clean=Join-Path $work $mode
        $run=Initialize-CompilerValidationRun (Join-Path $CompilerDirectory $mode) $clean $mode $state.binarySourceCommit
        $productBuild=Join-Path $CompilerDirectory $mode
        Copy-Item "$productBuild/build.json" "$modeOut/product-build.json"
        Copy-Item "$productBuild/validation/build.json" "$modeOut/validation-host-build.json"
        Copy-Item "$productBuild/link-inputs.json" "$modeOut/link-inputs.json"
        $runtimeOut=(New-Item -ItemType Directory (Join-Path $modeOut 'runtime')).FullName
        foreach($file in $run.runtimeFiles){Copy-Item (Join-Path $clean $file.name) $runtimeOut}
        # Backend construction uses the real texture registration. Its patched
        # dependency must bind to this validation EXE, never the production EXE.
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $run.exe *> "$modeOut/prepare-texture.log"
        if($LASTEXITCODE -ne 0){Get-Content "$modeOut/prepare-texture.log" -Tail 80|Out-Host;throw 'Backend validation-host texture preparation failed'}
        $texture=Join-Path $clean 'BinaryAssetBuilder.TextureCompiler.dll'
        $textureManifest="$texture.patch.json"
        $patch=Get-Content $textureManifest -Raw|ConvertFrom-Json
        $textureHash=Hash $texture;$textureManifestHash=Hash $textureManifest
        if($patch.targetExeSha256 -cne $run.validationHostSha256 -or $patch.originalSha256 -cne $state.originalTextureSha256 -or
            $patch.patchedSha256 -cne $textureHash -or $patch.protectedPeAudit.status -cne 'passed' -or $patch.protectedPeAudit.mode -cne 'redirect' -or
            $patch.protectedPeAudit.originalSha256 -cne $state.originalTextureSha256 -or $patch.protectedPeAudit.outputSha256 -cne $textureHash){
            throw 'Backend texture patch is not the audited validation-host deployment'
        }
        Copy-Item $textureManifest (Join-Path $runtimeOut 'texture.patch.json')
        $modeState=@{
            configuration=$mode;passed=$false;productExeSha256=$run.productExeSha256
            validationHostSha256=$run.validationHostSha256;runtimeFiles=$run.runtimeFiles
            linkRecipeSha256=(Hash "$productBuild/link-inputs.json");fixtures=@()
            textureDeployment=@{targetExeSha256=$run.validationHostSha256;patchedSha256=$textureHash;manifestSha256=$textureManifestHash;protectedPeAudit=$patch.protectedPeAudit;scope='Host-bound registration/lifecycle only; texture payload differential remains separate'}
        }
        $state.modes+=,$modeState
        $fixtureIndex=0
        foreach($fixture in $fixtures) {
            $fixtureIndex++
            $fixtureOut=(New-Item -ItemType Directory (Join-Path $modeOut $fixture.name)).FullName
            $fixtureWork=(New-Item -ItemType Directory (Join-Path $work ($mode.Substring(0,1)+'-f'+$fixtureIndex))).FullName
            $source=(Resolve-Path ('recovery/tests/'+$fixture.name+'.cs')).Path
            $fixtureExe=Join-Path $clean ($fixture.name+'.exe')
            $arguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86',('/out:'+$fixtureExe),('/r:'+$run.exe),('/r:'+(Join-Path $clean 'antlr.runtime.dll')))
            $arguments+=$(if($mode -eq 'Release'){'/optimize+'}else{'/debug:full'})
            foreach($name in $frameworkReferences){$arguments+=('/r:'+(Join-Path $refs ($name+'.dll')))}
            $arguments+=$source
            $additionalSourceInputs=@()
            if($fixture.name -in @('CompilerCoreLifecycle','CompilerInputDigests','CompilerExpressionPrograms','CompilerDocumentMetadata')){
                # Run identical inheritance/reader corpora against independent
                # original SDK and this production host; suppress only its Main.
                $arguments+=@('/define:DOCUMENT_ORACLE_SHARED',$documentOracleSource)
                $additionalSourceInputs+=,@{path='recovery/tests/CompilerDocumentSnapshotOracle.cs';sha256=(Hash $documentOracleSource);define='DOCUMENT_ORACLE_SHARED'}
            }
            try {
            & $csc @arguments *> "$fixtureOut/fixture-build.log"
            if($LASTEXITCODE -ne 0){Get-Content "$fixtureOut/fixture-build.log" -Tail 100|Out-Host;throw "$mode $($fixture.name) fixture compilation failed"}
            Copy-Item "$($run.exe).config" "$fixtureExe.config"
            Copy-Item $fixtureExe "$fixtureOut/fixture.exe"
            Copy-Item "$fixtureExe.config" "$fixtureOut/fixture.exe.config"
            $fixtureHash=Hash $fixtureExe
            $timeout=if($fixture.timeoutSeconds){[int]$fixture.timeoutSeconds}else{$FixtureTimeoutSeconds}
            $resultPath=Join-Path $fixtureOut 'result.json'
                $fixtureArguments=@($fixtureExe,$fixtureWork,$resultPath,$state.sourceCommit,$state.runId,$sdk)
                if($fixture.name -eq 'CompilerExpressionPrograms'){$fixtureArguments+=@($expressionOriginalJson,$documentOriginalJson)}
                if($fixture.name -eq 'CompilerHashingWriter'){$fixtureArguments+=$hashingOriginalJson}
                if($fixture.name -in @('CompilerDocumentSnapshots','CompilerDocumentMetadata','CompilerCoreLifecycle','CompilerInputDigests')){$fixtureArguments+=$documentOriginalJson}
                $processId=Invoke-LifecycleFixture $run.exe $fixtureArguments $clean "$fixtureOut/runtime.log" $timeout
                if(!(Select-String -LiteralPath "$fixtureOut/runtime.log" -SimpleMatch $fixture.marker -Quiet)){throw 'Core lifecycle success marker missing'}
                $result=Get-Content -LiteralPath $resultPath -Raw|ConvertFrom-Json
                if(!$result.succeeded -or $result.sourceCommit -cne $state.sourceCommit -or $result.runId -cne $state.runId){throw 'Core lifecycle result identity or success mismatch'}
                if(@($result.checks).Count -eq 0 -or ($result.checks -is [ValueType] -and $result.checks -le 0)){throw 'Lifecycle fixture assertion coverage missing'}
                if($fixture.name -eq 'CompilerCoreLifecycle' -and @($result.tests).Count -lt 4){throw 'Real Core lifecycle case groups missing'}
                if($fixture.name -in @('CompilerBackendLifecycle','CompilerApplicationBuild') -and @($result.tests).Count -eq 0){throw 'Targeted backend/application case groups missing'}
                if((Hash $fixtureExe) -cne $fixtureHash -or (Hash $source) -cne ($state.fixtures|Where-Object{$_.name -ceq $fixture.name}).sha256){throw 'Lifecycle fixture input changed while running'}
                foreach($input in $additionalSourceInputs){if((Hash (Join-Path $root $input.path)) -cne $input.sha256){throw 'Lifecycle shared corpus input changed while running'}}
                if((Hash $texture) -cne $textureHash -or (Hash $textureManifest) -cne $textureManifestHash){throw 'Backend texture deployment changed while running'}
                Complete-CompilerValidationRun $run
                $modeState.fixtures+=,@{
                    name=$fixture.name;passed=$true;processId=$processId;timeoutSeconds=$timeout;workRoot=$fixtureWork
                    sourceSha256=(Hash $source);additionalSourceInputs=$additionalSourceInputs;fixtureExeSha256=(Hash $fixtureExe)
                    resultSha256=(Hash $resultPath);runtimeLogSha256=(Hash "$fixtureOut/runtime.log")
                }
                $state|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
                Write-Host "PASS $mode $($fixture.name): targeted production-object lifecycle fixture"
            } catch {
                $fixtureError=$_.Exception.ToString()
                $fixtureFailures+=($mode+' '+$fixture.name+': '+$fixtureError)
                $modeState.fixtures+=,@{name=$fixture.name;passed=$false;error=$fixtureError;workRoot=$fixtureWork;sourceSha256=(Hash $source)}
                # Each fixture is its own process. Keep collecting independent
                # failures without converting any failed scope to a success.
                Complete-CompilerValidationRun $run
                $state|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
                Write-Host "FAIL $mode $($fixture.name): $fixtureError"
            } finally {
                # Preserve the small real XML/cache/output case tree on failures too.
                if(Test-Path -LiteralPath $fixtureWork){Copy-Item -LiteralPath $fixtureWork -Destination (Join-Path $fixtureOut 'work') -Recurse}
            }
        }
        Complete-CompilerValidationRun $run
        foreach($file in $run.runtimeFiles){if((Hash (Join-Path $runtimeOut $file.name)) -cne $file.sha256){throw 'Archived lifecycle runtime input changed'}}
        if($modeState.fixtures.Count -ne $fixtures.Count){throw 'Lifecycle fixture omitted'}
        $modeState.passed=(@($modeState.fixtures|Where-Object{$_.passed -ne $true}).Count -eq 0)
    }
    foreach($input in $state.upheavalSampleInputs){if((Hash (Join-Path $root $input.path)) -cne $input.sha256){throw 'Upheaval small-group source input changed during validation'}}
    foreach($input in $state.schemaInputs){if((Hash (Join-Path $sdk $input.path)) -cne $input.sha256){throw 'SDK schema input changed during lifecycle validation'}}
    foreach($input in $state.sdkFixtureInputs){if((Hash (Join-Path $sdk $input.path)) -cne $input.sha256){throw 'SDK fixture/config input changed during lifecycle validation'}}
    if((Hash $originalTexture) -cne $state.originalTextureSha256){throw 'Pinned original texture changed during backend lifecycle validation'}
    if((Hash $documentOracleSource) -cne $state.documentOriginal.sourceSha256 -or
       (Hash $documentOriginalJson) -cne $state.documentOriginal.transcriptSha256 -or
       (Hash $documentOriginalCore) -cne $state.documentOriginal.coreSha256){throw 'Original document oracle evidence changed during validation'}
    if((Hash $hashingOracleSource) -cne $state.hashingOriginal.sourceSha256 -or
       (Hash $hashingOriginalJson) -cne $state.hashingOriginal.transcriptSha256){throw 'Original HashingWriter oracle evidence changed during validation'}
    if($fixtureFailures.Count -ne 0){throw ($fixtureFailures -join "`n")}
    $state.succeeded=$true;$state.status='passed targeted Core/backend/application/resource fixtures in Debug and Release; production CLI, APT business coverage, cold-process equivalence, SDK and G3 acceptance remain separate'
    $global:LASTEXITCODE=0
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {
    $env:PATH=$oldPath
    $state|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json"
    Pop-Location
}
