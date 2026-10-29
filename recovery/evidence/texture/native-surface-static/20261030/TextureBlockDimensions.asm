
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000c7c4 <.text+0xb7c4>:
1000c7c4:	55                   	push   ebp
1000c7c5:	8b ec                	mov    ebp,esp
1000c7c7:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000c7ca:	83 f8 2c             	cmp    eax,0x2c
1000c7cd:	77 3f                	ja     0x1000c80e
1000c7cf:	74 32                	je     0x1000c803
1000c7d1:	83 f8 0b             	cmp    eax,0xb
1000c7d4:	72 51                	jb     0x1000c827
1000c7d6:	83 f8 0c             	cmp    eax,0xc
1000c7d9:	76 14                	jbe    0x1000c7ef
1000c7db:	83 f8 11             	cmp    eax,0x11
1000c7de:	76 47                	jbe    0x1000c827
1000c7e0:	83 f8 14             	cmp    eax,0x14
1000c7e3:	76 47                	jbe    0x1000c82c
1000c7e5:	83 f8 27             	cmp    eax,0x27
1000c7e8:	74 19                	je     0x1000c803
1000c7ea:	83 f8 28             	cmp    eax,0x28
1000c7ed:	75 38                	jne    0x1000c827
1000c7ef:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000c7f2:	c7 00 02 00 00 00    	mov    DWORD PTR [eax],0x2
1000c7f8:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1000c7fb:	c7 00 01 00 00 00    	mov    DWORD PTR [eax],0x1
1000c801:	eb 36                	jmp    0x1000c839
1000c803:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1000c806:	c7 00 04 00 00 00    	mov    DWORD PTR [eax],0x4
1000c80c:	eb ea                	jmp    0x1000c7f8
1000c80e:	83 f8 31             	cmp    eax,0x31
1000c811:	74 19                	je     0x1000c82c
1000c813:	83 f8 32             	cmp    eax,0x32
1000c816:	76 0f                	jbe    0x1000c827
1000c818:	83 f8 35             	cmp    eax,0x35
1000c81b:	76 0f                	jbe    0x1000c82c
1000c81d:	83 f8 39             	cmp    eax,0x39
1000c820:	76 05                	jbe    0x1000c827
1000c822:	83 f8 3d             	cmp    eax,0x3d
1000c825:	76 05                	jbe    0x1000c82c
1000c827:	33 c0                	xor    eax,eax
1000c829:	40                   	inc    eax
1000c82a:	eb 03                	jmp    0x1000c82f
1000c82c:	6a 04                	push   0x4
1000c82e:	58                   	pop    eax
1000c82f:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000c832:	89 01                	mov    DWORD PTR [ecx],eax
1000c834:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1000c837:	89 01                	mov    DWORD PTR [ecx],eax
1000c839:	5d                   	pop    ebp
1000c83a:	c2 0c 00             	ret    0xc
