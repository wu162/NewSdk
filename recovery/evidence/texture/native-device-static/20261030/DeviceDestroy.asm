
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1001169a <.text+0x1069a>:
1001169a:	56                   	push   esi
1001169b:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1001169f:	83 7e 30 00          	cmp    DWORD PTR [esi+0x30],0x0
100116a3:	74 06                	je     0x100116ab
100116a5:	56                   	push   esi
100116a6:	e8 04 c9 ff ff       	call   0x1000dfaf
100116ab:	f6 86 b0 2a 00 00 01 	test   BYTE PTR [esi+0x2ab0],0x1
100116b2:	74 0c                	je     0x100116c0
100116b4:	a1 04 83 0b 10       	mov    eax,ds:0x100b8304
100116b9:	39 30                	cmp    DWORD PTR [eax],esi
100116bb:	75 03                	jne    0x100116c0
100116bd:	83 20 00             	and    DWORD PTR [eax],0x0
100116c0:	8b 86 64 38 00 00    	mov    eax,DWORD PTR [esi+0x3864]
100116c6:	85 c0                	test   eax,eax
100116c8:	74 0e                	je     0x100116d8
100116ca:	68 00 80 00 00       	push   0x8000
100116cf:	6a 00                	push   0x0
100116d1:	50                   	push   eax
100116d2:	ff 15 a8 70 09 10    	call   DWORD PTR ds:0x100970a8
100116d8:	8b b6 4c 4b 00 00    	mov    esi,DWORD PTR [esi+0x4b4c]
100116de:	85 f6                	test   esi,esi
100116e0:	74 06                	je     0x100116e8
100116e2:	56                   	push   esi
100116e3:	e8 e5 fe ff ff       	call   0x100115cd
100116e8:	5e                   	pop    esi
100116e9:	c2 04 00             	ret    0x4
