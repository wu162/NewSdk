
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

100115fe <.text+0x105fe>:
100115fe:	56                   	push   esi
100115ff:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
10011603:	c7 46 3c 01 00 00 00 	mov    DWORD PTR [esi+0x3c],0x1
1001160a:	a1 08 83 0b 10       	mov    eax,ds:0x100b8308
1001160f:	89 86 80 2a 00 00    	mov    DWORD PTR [esi+0x2a80],eax
10011615:	a1 0c 83 0b 10       	mov    eax,ds:0x100b830c
1001161a:	89 86 84 2a 00 00    	mov    DWORD PTR [esi+0x2a84],eax
10011620:	ff 15 98 70 09 10    	call   DWORD PTR ds:0x10097098
10011626:	83 a6 8c 2a 00 00 00 	and    DWORD PTR [esi+0x2a8c],0x0
1001162d:	6a 04                	push   0x4
1001162f:	68 00 10 00 00       	push   0x1000
10011634:	68 c0 12 00 00       	push   0x12c0
10011639:	6a 00                	push   0x0
1001163b:	89 86 88 2a 00 00    	mov    DWORD PTR [esi+0x2a88],eax
10011641:	ff 15 24 70 09 10    	call   DWORD PTR ds:0x10097024
10011647:	33 c9                	xor    ecx,ecx
10011649:	85 c0                	test   eax,eax
1001164b:	0f 95 c1             	setne  cl
1001164e:	89 86 64 38 00 00    	mov    DWORD PTR [esi+0x3864],eax
10011654:	5e                   	pop    esi
10011655:	8b c1                	mov    eax,ecx
10011657:	c2 04 00             	ret    0x4
