#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerCliOutputs.ps1"
if(Test-Path -LiteralPath $OutputDirectory){throw 'Refusing to overwrite output-oracle tests'}
New-Item -ItemType Directory -Path $OutputDirectory|Out-Null
$path=Join-Path $OutputDirectory 'fixture.manifest';$binPath=Join-Path $OutputDirectory 'fixture.bin'
function Fixture([string]$Name='Texture:Expected',[uint32]$Type=568797146u){
    $names=[Text.Encoding]::ASCII.GetBytes($Name+[char]0)
    $data=[byte[]]::new(96+$names.Length);$data[1]=1;$data[2]=6
    [BitConverter]::GetBytes([uint32]1).CopyTo($data,12)
    [BitConverter]::GetBytes([uint32]140).CopyTo($data,16)
    [BitConverter]::GetBytes([uint32]$names.Length).CopyTo($data,40)
    [BitConverter]::GetBytes($Type).CopyTo($data,48)
    [BitConverter]::GetBytes([uint32]0x01234567).CopyTo($data,52)
    [BitConverter]::GetBytes([uint32]2673866970).CopyTo($data,56)
    [BitConverter]::GetBytes([uint32]0x13579bdf).CopyTo($data,60)
    [BitConverter]::GetBytes([uint32]140).CopyTo($data,80)
    $names.CopyTo($data,96);return ,$data
}
$passed=@()
try {
    $bin=[byte[]]::new(144);$bin[8]=12;$bin[12]=128;$bin[16]=68;$bin[17]=68;$bin[18]=83;$bin[19]=32;$bin[20]=124
    [IO.File]::WriteAllBytes($binPath,$bin);[IO.File]::WriteAllBytes($path,(Fixture))
    $result=@(Test-CompilerInlineTexturePayload $path @('Texture:Expected'))
    if($result.Count -ne 1 -or $result[0].qualifiedName -cne 'Texture:Expected' -or $result[0].typeId -ne 568797146u -or $result[0].typeHash -ne 2673866970u -or $result[0].instanceOffset -ne 4 -or $result[0].instanceDataSize -ne 140 -or !$result[0].inlineDdsVerified -or $result[0].hasCustomData -ne $false -or @(Get-CompilerCustomDataOutputs $path @('Texture:Expected')).Count -ne 0){throw 'Independent original Texture metadata/inline DDS assertion failed'}
    $passed+='original-texture-inline-dds-zero-custom'
    $cases=@(
        @{name='unexpected-texture';data=(Fixture 'Texture:Other');expected=@('Texture:Expected')},
        @{name='missing-texture';data=(Fixture 'ArmorTemplate:Expected' 1);expected=@('Texture:Expected')},
        @{name='wrong-route-id';data=(Fixture 'Texture:Expected' 1);expected=@('Texture:Expected')},
        @{name='unexpected-custom-route';data=(Fixture 'OnDemandTexture:Expected');expected=@()},
        @{name='duplicate-expectation';data=(Fixture);expected=@('Texture:Expected','Texture:Expected')},
        @{name='invalid-qualified-expectation';data=(Fixture);expected=@('Expected')}
    )
    $badVersion=Fixture;$badVersion[0]=1;$cases+=@{name='big-endian-refused';data=$badVersion;expected=@('Texture:Expected')}
    $badBounds=Fixture;$badBounds[40]=255;$cases+=@{name='table-bounds';data=$badBounds;expected=@('Texture:Expected')}
    $badOffset=Fixture;$badOffset[72]=255;$cases+=@{name='name-offset';data=$badOffset;expected=@('Texture:Expected')}
    $badTerminator=Fixture;$badTerminator[-1]=65;$cases+=@{name='unterminated-name';data=$badTerminator;expected=@('Texture:Expected')}
    $badHash=Fixture;$badHash[56]=0;$cases+=@{name='wrong-original-type-hash';data=$badHash;expected=@('Texture:Expected')}
    foreach($case in $cases){
        [IO.File]::WriteAllBytes($path,$case.data);$rejected=$false
        try {$null=Get-CompilerTextureAssets $path $case.expected}catch{$rejected=$true}
        if(!$rejected){throw "Output oracle accepted invalid contract: $($case.name)"};$passed+=$case.name
    }
    [IO.File]::WriteAllBytes($path,(Fixture));$bin[16]=0;[IO.File]::WriteAllBytes($binPath,$bin);$rejected=$false
    try {$null=Test-CompilerInlineTexturePayload $path @('Texture:Expected')}catch{$rejected=$true}
    if(!$rejected){throw 'Output oracle accepted missing inline DDS bytes'};$passed+='missing-inline-dds-refused'
    [IO.File]::WriteAllBytes($path,(Fixture 'ArmorTemplate:Expected' 1))
    if(@(Get-CompilerCustomDataOutputs $path @()).Count -ne 0 -or @(Get-CompilerTextureAssets $path @()).Count -ne 0){throw 'Nontexture manifest gained Texture/CustomData'};$passed+='no-texture-zero-custom'
    @{succeeded=$true;cases=$passed;count=$passed.Count}|ConvertTo-Json -Depth 4|Set-Content (Join-Path $OutputDirectory 'validation.json')
    Write-Host "PASS CompilerCliOutputs: $($passed.Count) original-metadata/inline DDS contracts";$global:LASTEXITCODE=0
} finally {Remove-Item -LiteralPath $path,$binPath -Force -ErrorAction SilentlyContinue}
