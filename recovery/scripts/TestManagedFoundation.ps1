param([string]$OutputDirectory,[string]$FoundationDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$OutputDirectory){$OutputDirectory="$root/recovery/evidence/refactor/managed/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite validation $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
if(!$FoundationDirectory){$FoundationDirectory="$root/recovery/artifacts/managed-foundation-tests/$([Guid]::NewGuid().ToString('N'))"}
New-Item -ItemType Directory -Force $FoundationDirectory|Out-Null
$FoundationDirectory=(Resolve-Path $FoundationDirectory).Path
$state=@{foundationDirectory=$FoundationDirectory;sourceCommit=(git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='P3 managed source netmodule, not final linked product';singleExeValidated=$false;modes=@()}
try {
    foreach($configuration in @('Debug','Release')) {
        $work=Join-Path $FoundationDirectory $configuration
        New-Item -ItemType Directory -Force $work|Out-Null
        & "$PSScriptRoot/BuildManagedFoundation.ps1" -Configuration $configuration -OutputDirectory $work | Out-Host
        if($LASTEXITCODE -ne 0){throw "Foundation $configuration build failed"}
        $packageLine=(& dotnet nuget locals global-packages --list)
        $packageRoot=($packageLine -replace '^global-packages:\s*','').Trim()
        $csc=Join-Path $packageRoot 'microsoft.net.compilers.toolset/4.0.1/tasks/net472/csc.exe'
        $framework=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
        $hostPath="$work/ManagedFoundationTest.exe"
        # C# addmodule is used only by this test fixture to exercise IL. G1 separately
        # proves the MSVC link consumes the physical modules into one executable.
        $main="$work/Main.cs"
        'class Program { static int Main() { NewSdk.BuildValidation.ManagedFoundationValidation.Run(false); return 0; } }'|Set-Content $main
        $compilerArguments=@('/nologo','/noconfig','/target:exe','/platform:x86','/langversion:10.0',('/out:'+$hostPath),('/addmodule:'+$work+'/Managed.Foundation.netmodule'),('/reference:'+$framework+'/System.Core.dll'),('/reference:'+$framework+'/System.Xml.dll'),('/reference:'+$framework+'/System.Configuration.dll'))
        foreach($resource in (Get-Content "$work/resources.json" -Raw|ConvertFrom-Json)){$compilerArguments+='/resource:'+$resource.path+','+$resource.logicalName}
        $compilerArguments+="$root/recovery/build/ManagedValidation/ManagedFoundationValidation.cs"
        $compilerArguments+=$main
        & $csc $compilerArguments *> "$out/$configuration-host-build.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/$configuration-host-build.log";throw 'Managed validation host failed to compile'}
        $oldBusiness=@('BinaryAssetBuilder.Core','BinaryAssetBuilder.Utility','EALA.Hash','EALA.Metrics','Mvp.Xml','BinaryAssetBuilder.ExpressionEval','BinaryAssetBuilder.GameDataVerifier','BinaryAssetBuilder.AudioCompiler')
        foreach($name in $oldBusiness){if(Test-Path (Join-Path $work ($name+'.dll'))){throw "Old business DLL contaminates managed fixture: $name"}}
        Copy-Item "$root/SDK/tools/antlr.runtime.dll" $work -Force
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/></startup></configuration>'|Set-Content "$hostPath.config"
        & $hostPath *> "$out/$configuration-runtime.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/$configuration-runtime.log";throw 'Managed validation host failed'}
        Get-Content "$out/$configuration-runtime.log"
        Copy-Item "$work/build.json" "$out/$configuration-build.json"
        Copy-Item "$work/metadata.log" "$out/$configuration-metadata.log"
        $state.modes+=@{configuration=$configuration;passed=$true;buildDirectory=$work;buildReceipt="$configuration-build.json";moduleSha256=(Get-FileHash "$work/Managed.Foundation.netmodule").Hash.ToLowerInvariant();oldBusinessDllsAbsent=$true}
    }
    $state.status='passed'
} catch {$state.status='failed';$state.error=$_.Exception.Message;throw}
finally {
    foreach($configuration in @('Debug','Release')) {
        $build=Join-Path $FoundationDirectory $configuration
        if(Test-Path $build){
            Get-ChildItem $build -Filter *.log -File|ForEach-Object {Copy-Item $_.FullName (Join-Path $out ($configuration+'-'+$_.Name)) -Force}
            foreach($file in @('build.json','resources.json','Foundation.types.txt')){
                $path=Join-Path $build $file
                if(Test-Path $path){Copy-Item $path (Join-Path $out ($configuration+'-'+$file)) -Force}
            }
        }
    }
    $state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json" -Encoding utf8
}
