
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c388 <.text+0xb388>:
1000c388:	8b 54 24 04          	mov    edx,DWORD PTR [esp+0x4]
1000c38c:	8b 0a                	mov    ecx,DWORD PTR [edx]
1000c38e:	8b c1                	mov    eax,ecx
1000c390:	83 e0 0f             	and    eax,0xf
1000c393:	83 f8 03             	cmp    eax,0x3
1000c396:	75 2d                	jne    0x1000c3c5
1000c398:	8b 4a 30             	mov    ecx,DWORD PTR [edx+0x30]
1000c39b:	c1 e9 09             	shr    ecx,0x9
1000c39e:	23 c8                	and    ecx,eax
1000c3a0:	3b c8                	cmp    ecx,eax
1000c3a2:	75 04                	jne    0x1000c3a8
1000c3a4:	6a 12                	push   0x12
1000c3a6:	eb 40                	jmp    0x1000c3e8
1000c3a8:	83 f9 02             	cmp    ecx,0x2
1000c3ab:	75 04                	jne    0x1000c3b1
1000c3ad:	6a 11                	push   0x11
1000c3af:	eb 37                	jmp    0x1000c3e8
1000c3b1:	85 c9                	test   ecx,ecx
1000c3b3:	75 04                	jne    0x1000c3b9
1000c3b5:	6a 14                	push   0x14
1000c3b7:	eb 2f                	jmp    0x1000c3e8
1000c3b9:	66 f7 42 20 00 04    	test   WORD PTR [edx+0x20],0x400
1000c3bf:	74 28                	je     0x1000c3e9
1000c3c1:	6a 13                	push   0x13
1000c3c3:	eb 23                	jmp    0x1000c3e8
1000c3c5:	83 f8 04             	cmp    eax,0x4
1000c3c8:	75 1f                	jne    0x1000c3e9
1000c3ca:	f7 c1 00 00 00 40    	test   ecx,0x40000000
1000c3d0:	74 17                	je     0x1000c3e9
1000c3d2:	8b 4a 18             	mov    ecx,DWORD PTR [edx+0x18]
1000c3d5:	8b 49 30             	mov    ecx,DWORD PTR [ecx+0x30]
1000c3d8:	81 e1 00 06 00 00    	and    ecx,0x600
1000c3de:	81 f9 00 04 00 00    	cmp    ecx,0x400
1000c3e4:	75 03                	jne    0x1000c3e9
1000c3e6:	6a 10                	push   0x10
1000c3e8:	58                   	pop    eax
1000c3e9:	c2 04 00             	ret    0x4
