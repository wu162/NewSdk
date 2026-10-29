
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d18d <.text+0xc18d>:
1000d18d:	55                   	push   ebp
1000d18e:	8b ec                	mov    ebp,esp
1000d190:	83 ec 0c             	sub    esp,0xc
1000d193:	56                   	push   esi
1000d194:	57                   	push   edi
1000d195:	8b 7d 08             	mov    edi,DWORD PTR [ebp+0x8]
1000d198:	8d 45 fc             	lea    eax,[ebp-0x4]
1000d19b:	50                   	push   eax
1000d19c:	8d 45 f8             	lea    eax,[ebp-0x8]
1000d19f:	50                   	push   eax
1000d1a0:	8d 45 f4             	lea    eax,[ebp-0xc]
1000d1a3:	50                   	push   eax
1000d1a4:	8d 45 08             	lea    eax,[ebp+0x8]
1000d1a7:	50                   	push   eax
1000d1a8:	8d 45 0c             	lea    eax,[ebp+0xc]
1000d1ab:	50                   	push   eax
1000d1ac:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d1af:	57                   	push   edi
1000d1b0:	e8 d2 fc ff ff       	call   0x1000ce87
1000d1b5:	8d 47 1c             	lea    eax,[edi+0x1c]
1000d1b8:	50                   	push   eax
1000d1b9:	e8 7f f6 ff ff       	call   0x1000c83d
1000d1be:	8b 75 10             	mov    esi,DWORD PTR [ebp+0x10]
1000d1c1:	57                   	push   edi
1000d1c2:	89 06                	mov    DWORD PTR [esi],eax
1000d1c4:	e8 bf f1 ff ff       	call   0x1000c388
1000d1c9:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d1cc:	89 46 04             	mov    DWORD PTR [esi+0x4],eax
1000d1cf:	33 c0                	xor    eax,eax
1000d1d1:	89 4e 18             	mov    DWORD PTR [esi+0x18],ecx
1000d1d4:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1000d1d7:	5f                   	pop    edi
1000d1d8:	89 46 08             	mov    DWORD PTR [esi+0x8],eax
1000d1db:	89 46 0c             	mov    DWORD PTR [esi+0xc],eax
1000d1de:	89 46 14             	mov    DWORD PTR [esi+0x14],eax
1000d1e1:	89 4e 1c             	mov    DWORD PTR [esi+0x1c],ecx
1000d1e4:	89 46 10             	mov    DWORD PTR [esi+0x10],eax
1000d1e7:	5e                   	pop    esi
1000d1e8:	c9                   	leave
1000d1e9:	c2 0c 00             	ret    0xc
