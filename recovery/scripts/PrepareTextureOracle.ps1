# Test-only original codec oracle. Dot-source and call before saving the isolated
# original BAB configuration. Repository SDK files are never modified.
function Enable-TextureOracleIdentity {
    param(
        [Parameter(Mandatory=$true)][xml]$Configuration,
        [Parameter(Mandatory=$true)][string]$OracleDirectory,
        [Parameter(Mandatory=$true)][string]$EvidenceDirectory
    )
    $oracleRoot=(Resolve-Path "$PSScriptRoot/../..").Path
    $wrapper=Join-Path $oracleRoot 'recovery/tests/TextureOriginalIdentityPlugin.cs'
    $catalog=Join-Path $oracleRoot 'recovery/src/Plugin/Texture/TextureCatalog.cs'
    $texture=Join-Path $OracleDirectory 'BinaryAssetBuilder.TextureCompiler.dll'
    $textureSha256=(Get-FileHash -LiteralPath $texture -Algorithm SHA256).Hash.ToLowerInvariant()
    if($textureSha256 -cne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Texture oracle requires the pinned original DLL'}
    $routes=@($Configuration.SelectNodes('/configuration/assetbuilder/plugins/plugin')|Where-Object{
        $_.GetAttribute('targetType') -ceq 'BinaryAssetBuilder.TextureCompiler.Plugin, BinaryAssetBuilder.TextureCompiler'
    })
    if($routes.Count -ne 1 -or (($routes[0].GetAttribute('assetTypes').Split(',')|Sort-Object) -join ',') -cne 'OnDemandTexture,TerrainTextureAtlas,Texture'){
        throw 'Texture oracle requires the exact original three-route descriptor'
    }
    New-Item -ItemType Directory -Force -Path $EvidenceDirectory|Out-Null
    $assembly=Join-Path $OracleDirectory 'TextureOriginalIdentity.dll'
    $log=Join-Path $EvidenceDirectory 'identity-adapter-build.log'
    $compiler=Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
    & $compiler /nologo /target:library /platform:x86 /optimize+ ("/out:"+$assembly) ("/r:"+(Join-Path $OracleDirectory 'BinaryAssetBuilder.Core.dll')) ("/r:"+(Join-Path $OracleDirectory 'BinaryAssetBuilder.Utility.dll')) $wrapper $catalog *> $log
    if($LASTEXITCODE -ne 0){Get-Content -LiteralPath $log -Tail 60|Out-Host;throw 'Original texture identity adapter compilation failed'}
    $routes[0].SetAttribute('targetType','NewSdk.TextureOracle.Plugin, TextureOriginalIdentity')
    $identity=[ordered]@{
        schemaVersion=1;implementation='original-codec-with-source-processing-identity';originalTextureSha256=$textureSha256
        overrides=@('GetVersionNumber','ExtendedTypeInformation.ProcessingHash')
        unchanged=@('ProcessInstance','GetAllTypesHash','TypeName','TypeId','TypeHash','HasCustomData','Tokenized','UseBuildCache','Initialize','ReInitialize','Dispose')
        sources=@(@{path='recovery/tests/TextureOriginalIdentityPlugin.cs';sha256=(Get-FileHash -LiteralPath $wrapper -Algorithm SHA256).Hash.ToLowerInvariant()},@{path='recovery/src/Plugin/Texture/TextureCatalog.cs';sha256=(Get-FileHash -LiteralPath $catalog -Algorithm SHA256).Hash.ToLowerInvariant()})
        assemblySha256=(Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash.ToLowerInvariant()
        originalCoreSha256=(Get-FileHash -LiteralPath (Join-Path $OracleDirectory 'BinaryAssetBuilder.Core.dll') -Algorithm SHA256).Hash.ToLowerInvariant()
        scope='Test-only original BAB route; original codec and AssetBuffer bytes, source version/processing hashes for intentional cache invalidation; raw four-stream and CDATA comparisons remain strict'
    }
    Copy-Item -LiteralPath $assembly -Destination (Join-Path $EvidenceDirectory 'TextureOriginalIdentity.dll')
    $identity|ConvertTo-Json -Depth 7|Set-Content -LiteralPath (Join-Path $EvidenceDirectory 'identity-adapter.json')
    return $identity
}
