# Exact Texture CustomData allowlist from the linked Win32 manifest. This reads
# names and handles only; no produced byte or path is normalized for comparison.
function Get-CompilerCustomDataOutputs([string]$Manifest,[string[]]$ExpectedTextures){
    $bytes=[IO.File]::ReadAllBytes($Manifest)
    if($bytes.Length -lt 48 -or $bytes[0] -ne 0 -or $bytes[1] -ne 1 -or $bytes[2] -ne 6 -or $bytes[3] -ne 0){throw 'Expected linked Win32 manifest v6'}
    $count=[BitConverter]::ToUInt32($bytes,12)
    $nameSize=[BitConverter]::ToUInt32($bytes,40)
    $nameStart=48L+48L*$count+[BitConverter]::ToUInt32($bytes,32)+[BitConverter]::ToUInt32($bytes,36)
    if($nameStart+$nameSize+[BitConverter]::ToUInt32($bytes,44) -ne $bytes.Length){throw 'Manifest table bounds differ'}
    $expected=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach($texture in $ExpectedTextures){if(!$texture.StartsWith('Texture:',[StringComparison]::Ordinal) -or !$expected.Add($texture)){throw 'Expected Texture names must be unique qualified identities'}}
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $result=@()
    for($i=0L;$i -lt $count;$i++){
        $entry=48L+48L*$i;$offset=[BitConverter]::ToUInt32($bytes,[int]($entry+24))
        if($offset -ge $nameSize){throw 'Asset name offset escaped name table'}
        $start=$nameStart+$offset;$end=$start
        while($end -lt $nameStart+$nameSize -and $bytes[$end] -ne 0){$end++}
        if($end -eq $nameStart+$nameSize){throw 'Unterminated manifest asset name'}
        $name=[Text.Encoding]::ASCII.GetString($bytes,[int]$start,[int]($end-$start))
        if(!$seen.Add($name)){throw 'Duplicate manifest asset name'}
        if($name.StartsWith('OnDemandTexture:',[StringComparison]::Ordinal) -or $name.StartsWith('TerrainTextureAtlas:',[StringComparison]::Ordinal)){throw 'Unexpected CustomData-producing texture route'}
        if(!$name.StartsWith('Texture:',[StringComparison]::Ordinal)){continue}
        if(!$expected.Remove($name)){throw "Unexpected manifest Texture identity: $name"}
        $type=[BitConverter]::ToUInt32($bytes,[int]$entry)
        if($type -ne 2777716390u){throw 'Texture manifest name/type ID mismatch'}
        $file=('{0:x8}.{1:x8}.{2:x8}.{3:x8}.cdata' -f $type,[BitConverter]::ToUInt32($bytes,[int]($entry+8)),[BitConverter]::ToUInt32($bytes,[int]($entry+4)),[BitConverter]::ToUInt32($bytes,[int]($entry+12)))
        $result+=,@{qualifiedName=$name;typeId=$type;relativePath=('cdata/'+$file);path=(Join-Path ($Manifest.Substring(0,$Manifest.Length-9)) ('cdata/'+$file))}
    }
    if($expected.Count -ne 0){throw 'Expected Texture asset missing from manifest'}
    return $result
}
