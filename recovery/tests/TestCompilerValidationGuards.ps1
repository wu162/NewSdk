# Tests only receipt transport and stale runtime rejection. Native compiler and
# linker commands are mocked; passing this test makes no Windows/ABI claim.
[CmdletBinding()]
param([string]$Repo=(Resolve-Path "$PSScriptRoot/../..").Path)
$ErrorActionPreference='Stop'
. "$Repo/recovery/scripts/CompilerValidation.Common.ps1"
$work=Join-Path ([IO.Path]::GetTempPath()) ('newsdk validation guards '+[guid]::NewGuid().ToString('N'))
try {
    New-Item -ItemType Directory "$work/product/bin","$work/product/obj","$work/product/validation/bin" -Force|Out-Null
    $product=(Resolve-Path "$work/product").Path
    function Hash($Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
    function WriteJson($Object,$Path){$Object|ConvertTo-Json -Depth 8|Set-Content $Path}
    $source=(git -C $Repo rev-parse HEAD).Trim()
    'production'|Set-Content "$product/bin/NewModCompiler.exe"
    'config'|Set-Content "$product/bin/NewModCompiler.exe.config"
    'antlr'|Set-Content "$product/bin/antlr.runtime.dll"
    foreach($name in @('foundation','application','entry','nativeOrBridge','resource')){$name|Set-Content "$product/obj/$name.obj"}
    $inputs=@('foundation','application','entry','nativeOrBridge','resource'|ForEach-Object{@{kind=$_;path=(Resolve-Path "$product/obj/$_.obj").Path;sha256=(Hash "$product/obj/$_.obj")}})
    $productionExe=(Resolve-Path "$product/bin/NewModCompiler.exe").Path
    $arguments=@('/nologo',('/OUT:'+$productionExe),'/ENTRY:NewModCompiler.Hosting.CompilerEntry.Main')+@($inputs|ForEach-Object{$_.path})
    $recipe=@{schemaVersion=1;sourceCommit=$source;configuration='Debug';inputs=$inputs;arguments=$arguments}
    WriteJson $recipe "$product/link-inputs.json"
    WriteJson @{sourceCommit=$source;configuration='Debug';exeSha256=(Hash "$product/bin/NewModCompiler.exe");foundationSha256=$inputs[0].sha256;applicationSha256=$inputs[1].sha256;entryPoint='NewModCompiler.Hosting.CompilerEntry.Main';nativeBuild=@{objects=@($inputs[3].path)}} "$product/build.json"
    # Exercise receipt construction with fake compiler/linker tools, without a native build.
    function cl.exe {param([Parameter(ValueFromRemainingArguments=$true)]$Arguments)
        $path=($Arguments|Where-Object{$_ -like '/Fo*'}).Substring(3)
        'validation entry'|Set-Content $path
        $global:LASTEXITCODE=0
    }
    function link.exe {param([Parameter(ValueFromRemainingArguments=$true)]$Arguments)
        $rsp=$Arguments[0].Substring(1)
        $path=(@(Get-Content $rsp)|Where-Object{$_ -like '"/OUT:*'}).Trim('"').Substring(5)
        'validation'|Set-Content $path
        $global:LASTEXITCODE=0
    }
    Remove-Item "$product/validation" -Recurse
    & "$Repo/recovery/scripts/BuildCompilerValidationHost.ps1" -ProductBuildDirectory $product -OutputDirectory "$product/validation"|Out-Null
    $receipt=Get-Content "$product/validation/build.json" -Raw|ConvertFrom-Json
    if($receipt.schemaVersion -ne 2 -or @($receipt.runtimeFiles).Count -ne 2){throw 'Runtime receipt not generated'}
    $run=Initialize-CompilerValidationRun $product "$work/clean" 'Debug' $source
    Complete-CompilerValidationRun $run
    $count=1
    function ExpectFailure([scriptblock]$Action,[string]$Message){
        $caught=$false
        try{& $Action}catch{$caught=$true;if($_.Exception.Message -notlike "*$Message*"){throw "Unexpected failure: $_"}}
        if(!$caught){throw "Expected rejection: $Message"}
        $script:count++
    }
    'changed'|Set-Content "$work/clean/NewModCompiler.exe.config"
    ExpectFailure {Complete-CompilerValidationRun $run} 'runtime input changed during fixture execution'
    Copy-Item "$product/bin/NewModCompiler.exe.config" "$work/clean/NewModCompiler.exe.config"
    'changed'|Set-Content "$product/bin/antlr.runtime.dll"
    ExpectFailure {Complete-CompilerValidationRun $run} 'runtime input changed during fixture execution'
    'antlr'|Set-Content "$product/bin/antlr.runtime.dll"
    'changed'|Set-Content "$product/validation/bin/NewModCompiler.exe.config"
    ExpectFailure {Initialize-CompilerValidationRun $product "$work/clean2" 'Debug' $source} 'runtime input changed'
    Copy-Item "$product/bin/NewModCompiler.exe.config" "$product/validation/bin/NewModCompiler.exe.config"
    New-Item -ItemType Directory "$work/dirty"|Out-Null
    'old DLL'|Set-Content "$work/dirty/.BinaryAssetBuilder.Core.dll"
    ExpectFailure {Initialize-CompilerValidationRun $product "$work/dirty" 'Debug' $source} 'runtime directory is not empty'
    $legacy=Get-Content "$product/validation/build.json" -Raw
    $receipt.schemaVersion=1
    WriteJson $receipt "$product/validation/build.json"
    ExpectFailure {Initialize-CompilerValidationRun $product "$work/old-schema" 'Debug' $source} 'receipt identities differ'
    $receipt.schemaVersion=2
    $receipt.PSObject.Properties.Remove('runtimeFiles')
    WriteJson $receipt "$product/validation/build.json"
    ExpectFailure {Initialize-CompilerValidationRun $product "$work/legacy" 'Debug' $source} 'runtime input inventory differs'
    $legacy|Set-Content "$product/validation/build.json"
    $empty=New-Item -ItemType Directory "$work/precreated"
    $run2=Initialize-CompilerValidationRun $product $empty.FullName 'Debug' $source
    Complete-CompilerValidationRun $run2
    $count++
    Write-Output "PASS $count validation receipt/clean directory cases (mock native tools; no ABI claim)"

} finally {
    if(Test-Path -LiteralPath $work){Remove-Item -LiteralPath $work -Recurse -Force}
}
