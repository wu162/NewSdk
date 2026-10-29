
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000d70f <.text+0xc70f>:
1000d70f:	55                   	push   ebp
1000d710:	8b ec                	mov    ebp,esp
1000d712:	56                   	push   esi
1000d713:	8b 75 08             	mov    esi,DWORD PTR [ebp+0x8]
1000d716:	8d 45 14             	lea    eax,[ebp+0x14]
1000d719:	50                   	push   eax
1000d71a:	8d 45 08             	lea    eax,[ebp+0x8]
1000d71d:	50                   	push   eax
1000d71e:	8d 45 18             	lea    eax,[ebp+0x18]
1000d721:	50                   	push   eax
1000d722:	8d 45 24             	lea    eax,[ebp+0x24]
1000d725:	50                   	push   eax
1000d726:	ff 75 24             	push   DWORD PTR [ebp+0x24]
1000d729:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000d72c:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1000d72f:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000d732:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000d735:	56                   	push   esi
1000d736:	e8 45 ff ff ff       	call   0x1000d680
1000d73b:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1000d73e:	85 c0                	test   eax,eax
1000d740:	8b 55 18             	mov    edx,DWORD PTR [ebp+0x18]
1000d743:	74 21                	je     0x1000d766
1000d745:	8b 4e 20             	mov    ecx,DWORD PTR [esi+0x20]
1000d748:	83 e1 3f             	and    ecx,0x3f
1000d74b:	0f b6 0c 4d 30 82 09 	movzx  ecx,BYTE PTR [ecx*2+0x10098230]
1000d752:	10 
1000d753:	0f af 08             	imul   ecx,DWORD PTR [eax]
1000d756:	8b 40 04             	mov    eax,DWORD PTR [eax+0x4]
1000d759:	0f af c2             	imul   eax,edx
1000d75c:	03 45 24             	add    eax,DWORD PTR [ebp+0x24]
1000d75f:	c1 e9 03             	shr    ecx,0x3
1000d762:	03 c1                	add    eax,ecx
1000d764:	eb 03                	jmp    0x1000d769
1000d766:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1000d769:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1000d76c:	89 11                	mov    DWORD PTR [ecx],edx
1000d76e:	89 41 04             	mov    DWORD PTR [ecx+0x4],eax
1000d771:	5e                   	pop    esi
1000d772:	5d                   	pop    ebp
1000d773:	c2 20 00             	ret    0x20
