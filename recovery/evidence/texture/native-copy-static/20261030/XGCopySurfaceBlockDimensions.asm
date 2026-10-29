
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003e615 <.text+0x3d615>:
1003e615:	55                   	push   ebp
1003e616:	8b ec                	mov    ebp,esp
1003e618:	51                   	push   ecx
1003e619:	8d 45 fc             	lea    eax,[ebp-0x4]
1003e61c:	50                   	push   eax
1003e61d:	8d 45 08             	lea    eax,[ebp+0x8]
1003e620:	50                   	push   eax
1003e621:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1003e624:	e8 9b e1 fc ff       	call   0x1000c7c4
1003e629:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003e62c:	85 c0                	test   eax,eax
1003e62e:	74 05                	je     0x1003e635
1003e630:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1003e633:	89 08                	mov    DWORD PTR [eax],ecx
1003e635:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1003e638:	85 c0                	test   eax,eax
1003e63a:	74 05                	je     0x1003e641
1003e63c:	8b 4d fc             	mov    ecx,DWORD PTR [ebp-0x4]
1003e63f:	89 08                	mov    DWORD PTR [eax],ecx
1003e641:	c9                   	leave
1003e642:	c2 0c 00             	ret    0xc
