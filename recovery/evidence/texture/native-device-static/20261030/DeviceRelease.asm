
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000df8a <.text+0xcf8a>:
1000df8a:	56                   	push   esi
1000df8b:	8b 74 24 08          	mov    esi,DWORD PTR [esp+0x8]
1000df8f:	8b 46 3c             	mov    eax,DWORD PTR [esi+0x3c]
1000df92:	83 f8 01             	cmp    eax,0x1
1000df95:	75 10                	jne    0x1000dfa7
1000df97:	56                   	push   esi
1000df98:	e8 fd 36 00 00       	call   0x1001169a
1000df9d:	56                   	push   esi
1000df9e:	e8 d3 ff ff ff       	call   0x1000df76
1000dfa3:	33 c0                	xor    eax,eax
1000dfa5:	eb 04                	jmp    0x1000dfab
1000dfa7:	48                   	dec    eax
1000dfa8:	89 46 3c             	mov    DWORD PTR [esi+0x3c],eax
1000dfab:	5e                   	pop    esi
1000dfac:	c2 04 00             	ret    0x4
