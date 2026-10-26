# Shared fixture transport. No compilation or assembly probing takes place here.
function Initialize-CompilerValidationRun([string]$ProductBuildDirectory,[string]$CleanDirectory,[string]$Configuration,[string]$SourceCommit) {
    $product=Get-Content "$ProductBuildDirectory/build.json" -Raw|ConvertFrom-Json
    $validation=Get-Content "$ProductBuildDirectory/validation/build.json" -Raw|ConvertFrom-Json
    $recipe=Get-Content "$ProductBuildDirectory/link-inputs.json" -Raw|ConvertFrom-Json
    if($product.sourceCommit -cne $SourceCommit -or $validation.sourceCommit -cne $SourceCommit -or $recipe.sourceCommit -cne $SourceCommit -or
        $product.configuration -cne $Configuration -or $validation.configuration -cne $Configuration -or $recipe.configuration -cne $Configuration -or
        !$validation.sameProductionObjects -or !$validation.productionExeUnchanged -or $validation.productExeSha256 -cne $product.exeSha256 -or
        $validation.entryPoint -cne 'NewModCompiler.Validation.ValidationEntry.Main'){throw 'Validation and production receipt identities differ'}
    $expected=@($recipe.inputs|Where-Object{$_.kind -cne 'entry'}|ForEach-Object{$_.kind+'|'+$_.path+'|'+$_.sha256})
    $actual=@($validation.inputs|ForEach-Object{$_.kind+'|'+$_.path+'|'+$_.sha256})
    if(($expected -join "`n") -cne ($actual -join "`n") -or
        (Get-FileHash "$ProductBuildDirectory/link-inputs.json" -Algorithm SHA256).Hash.ToLowerInvariant() -cne $validation.linkRecipeSha256){throw 'Validation input object/module/resource inventory differs from production recipe'}
    if((Get-FileHash "$ProductBuildDirectory/bin/NewModCompiler.exe" -Algorithm SHA256).Hash.ToLowerInvariant() -cne $product.exeSha256){throw 'Production executable changed'}
    New-Item -ItemType Directory -Force $CleanDirectory|Out-Null
    foreach($name in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){Copy-Item "$ProductBuildDirectory/validation/bin/$name" $CleanDirectory}
    $exe=Join-Path $CleanDirectory 'NewModCompiler.exe'
    if((Get-FileHash $exe -Algorithm SHA256).Hash.ToLowerInvariant() -cne $validation.exeSha256){throw 'Validation executable changed'}
    return @{exe=$exe;productExeSha256=$product.exeSha256;validationHostSha256=$validation.exeSha256;productDirectory=$ProductBuildDirectory}
}
function Complete-CompilerValidationRun($Run) {
    if((Get-FileHash $Run.exe -Algorithm SHA256).Hash.ToLowerInvariant() -cne $Run.validationHostSha256 -or
        (Get-FileHash "$($Run.productDirectory)/bin/NewModCompiler.exe" -Algorithm SHA256).Hash.ToLowerInvariant() -cne $Run.productExeSha256){throw 'Production or validation executable changed during fixture execution'}
}
