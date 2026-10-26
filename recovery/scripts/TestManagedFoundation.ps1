param([string]$OutputDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$OutputDirectory){$OutputDirectory="$root/recovery/evidence/refactor/managed/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"}
if(Test-Path $OutputDirectory){throw "Refusing to overwrite validation $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$state=@{sourceCommit=(git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';scope='P3 managed source netmodule, not final linked product';singleExeValidated=$false;modes=@()}
try {
    foreach($configuration in @('Debug','Release')) {
        $work="$root/recovery/artifacts/managed-foundation-tests/$configuration-$([Guid]::NewGuid().ToString('N'))"
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
        $args=@('/nologo','/noconfig','/target:exe','/platform:x86','/langversion:10.0','/out:'+$hostPath,'/addmodule:'+$work+'/Managed.Foundation.netmodule','/reference:'+$framework+'/System.Core.dll','/reference:'+$framework+'/System.Xml.dll','/reference:'+$framework+'/System.Configuration.dll')
        foreach($resource in (Get-Content "$work/resources.json" -Raw|ConvertFrom-Json)){$args+='/resource:'+$resource.path+','+$resource.logicalName}
        $args+="$root/recovery/build/ManagedValidation/ManagedFoundationValidation.cs"
        $args+=$main
        & $csc $args *> "$out/$configuration-host-build.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/$configuration-host-build.log";throw 'Managed validation host failed to compile'}
        Copy-Item "$root/SDK/tools/antlr.runtime.dll" $work -Force
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/></startup></configuration>'|Set-Content "$hostPath.config"
        & $hostPath *> "$out/$configuration-runtime.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/$configuration-runtime.log";throw 'Managed validation host failed'}
        Get-Content "$out/$configuration-runtime.log"
        Copy-Item "$work/build.json" "$out/$configuration-build.json"
        Copy-Item "$work/metadata.log" "$out/$configuration-metadata.log"
        $state.modes+=@{configuration=$configuration;passed=$true;moduleSha256=(Get-FileHash "$work/Managed.Foundation.netmodule").Hash.ToLowerInvariant();oldBusinessDllsAbsent=$true}
    }
    $state.status='passed'
} catch {$state.status='failed';$state.error=$_.Exception.Message;throw}
finally {$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json" -Encoding utf8}
