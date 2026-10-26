# Fixed SDK/tools/BinaryAssetBuilder.TextureCompiler.dll SHA256:
# 4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e
# Original Plugin.GetExtendedTypeInformation (RVA 0x1cb0)
# reports Texture typeId 568797146 / typeHash 0x9f5ff8da / HasCustomData=false.
# Texture embeds DDS in InstanceData; OnDemandTexture is a different cdata route.
# Raw evidence: evidence/refactor/core-lifecycle-texture/<run>/original.txt:
# Texture success InstanceData=262284 bytes, uint32@4=12, uint32@8=262272,
# DDS magic@12/headerSize=124, CUSTOM:absent (both successful calls).
# OutputManager.LinkStream writes a 4-byte checksum then each complete instance;
# Utility AssetEntry fields +32=InstanceDataSize and ManifestHeader +16=total.
# AssetEntry v6 has no HasCustomData bit: never infer cdata from a Texture name.
function Get-CompilerTextureAssets([string]$Manifest,[string[]]$ExpectedTextures){
    $bytes=[IO.File]::ReadAllBytes($Manifest)
    if($bytes.Length -lt 48 -or $bytes[0] -ne 0 -or $bytes[1] -ne 1 -or $bytes[2] -ne 6 -or $bytes[3] -ne 0){throw 'Expected linked Win32 manifest v6'}
    $count=[BitConverter]::ToUInt32($bytes,12)
    $nameSize=[BitConverter]::ToUInt32($bytes,40)
    $nameStart=48L+48L*$count+[BitConverter]::ToUInt32($bytes,32)+[BitConverter]::ToUInt32($bytes,36)
    if($nameStart+$nameSize+[BitConverter]::ToUInt32($bytes,44) -ne $bytes.Length){throw 'Manifest table bounds differ'}
    $expected=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach($texture in $ExpectedTextures){if(!$texture.StartsWith('Texture:',[StringComparison]::Ordinal) -or !$expected.Add($texture)){throw 'Expected Texture names must be unique qualified identities'}}
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $result=@();$binOffset=4L
    for($i=0L;$i -lt $count;$i++){
        $entry=48L+48L*$i;$offset=[BitConverter]::ToUInt32($bytes,[int]($entry+24))
        if($offset -ge $nameSize){throw 'Asset name offset escaped name table'}
        $start=$nameStart+$offset;$end=$start
        while($end -lt $nameStart+$nameSize -and $bytes[$end] -ne 0){$end++}
        if($end -eq $nameStart+$nameSize){throw 'Unterminated manifest asset name'}
        $name=[Text.Encoding]::ASCII.GetString($bytes,[int]$start,[int]($end-$start))
        if(!$seen.Add($name)){throw 'Duplicate manifest asset name'}
        $size=[BitConverter]::ToUInt32($bytes,[int]($entry+32));$assetOffset=$binOffset;$binOffset+=$size
        if($name.StartsWith('OnDemandTexture:',[StringComparison]::Ordinal) -or $name.StartsWith('TerrainTextureAtlas:',[StringComparison]::Ordinal)){throw 'Unexpected texture route outside this fixed UI scope'}
        if(!$name.StartsWith('Texture:',[StringComparison]::Ordinal)){continue}
        if(!$expected.Remove($name)){throw "Unexpected manifest Texture identity: $name"}
        $type=[BitConverter]::ToUInt32($bytes,[int]$entry);$typeHash=[BitConverter]::ToUInt32($bytes,[int]($entry+8))
        if($type -ne 568797146u -or $typeHash -ne 2673866970u){throw 'Texture manifest name/type ID/type hash differs from fixed original plugin metadata'}
        $result+=,@{qualifiedName=$name;typeId=$type;typeHash=$typeHash;instanceId=[BitConverter]::ToUInt32($bytes,[int]($entry+4));instanceHash=[BitConverter]::ToUInt32($bytes,[int]($entry+12));instanceOffset=$assetOffset;instanceDataSize=$size;hasCustomData=$false}
    }
    if($expected.Count -ne 0){throw 'Expected Texture asset missing from manifest'}
    if($binOffset-4 -ne [BitConverter]::ToUInt32($bytes,16)){throw 'Manifest total InstanceData size differs from entries'}
    return $result
}
function Get-CompilerCustomDataOutputs([string]$Manifest,[string[]]$ExpectedTextures){
    $null=Get-CompilerTextureAssets $Manifest $ExpectedTextures
    # This fixed UI scope has Texture, not OnDemandTexture. The fixed original
    # metadata and actual original ProcessInstance transcript both require no
    # cdata. Any extra file is rejected by the caller's exact four-stream set.
    return @()
}
function Test-CompilerInlineTexturePayload([string]$Manifest,[string[]]$ExpectedTextures){
    $assets=@(Get-CompilerTextureAssets $Manifest $ExpectedTextures)
    $binPath=$Manifest.Substring(0,$Manifest.Length-9)+'.bin'
    $bin=[IO.File]::ReadAllBytes($binPath);$header=[IO.File]::ReadAllBytes($Manifest)
    if($bin.Length -ne 4L+[BitConverter]::ToUInt32($header,16) -or [BitConverter]::ToUInt32($bin,0) -ne [BitConverter]::ToUInt32($header,4)){throw 'Linked bin length/checksum differs from manifest'}
    foreach($asset in $assets){
        $offset=[int]$asset.instanceOffset;$size=$asset.instanceDataSize
        if($size -lt 140 -or $offset+$size -gt $bin.Length){throw 'Inline Texture payload bounds differ'}
        $ddsOffset=[BitConverter]::ToUInt32($bin,$offset+4);$ddsSize=[BitConverter]::ToUInt32($bin,$offset+8)
        if($ddsOffset -ne 12 -or 12L+$ddsSize -ne $size -or $bin[$offset+12] -ne 68 -or $bin[$offset+13] -ne 68 -or $bin[$offset+14] -ne 83 -or $bin[$offset+15] -ne 32 -or [BitConverter]::ToUInt32($bin,$offset+16) -ne 124){throw 'Texture InstanceData does not contain the complete expected inline DDS'}
        $asset.ddsOffset=$ddsOffset;$asset.ddsBytes=$ddsSize;$asset.inlineDdsVerified=$true
    }
    return $assets
}
