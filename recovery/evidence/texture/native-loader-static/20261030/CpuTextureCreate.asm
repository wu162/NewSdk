
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000dc78 <.text+0xcc78>:
1000dc78:	55                   	push   ebp
1000dc79:	8b ec                	mov    ebp,esp
1000dc7b:	51                   	push   ecx
1000dc7c:	56                   	push   esi
1000dc7d:	57                   	push   edi
1000dc7e:	6a 34                	push   0x34
1000dc80:	33 ff                	xor    edi,edi
1000dc82:	6a 40                	push   0x40
1000dc84:	89 7d fc             	mov    DWORD PTR [ebp-0x4],edi
1000dc87:	ff 15 20 70 09 10    	call   DWORD PTR ds:0x10097020
1000dc8d:	8b f0                	mov    esi,eax
1000dc8f:	3b f7                	cmp    esi,edi
1000dc91:	75 07                	jne    0x1000dc9a
1000dc93:	33 c0                	xor    eax,eax
1000dc95:	e9 a1 00 00 00       	jmp    0x1000dd3b
1000dc9a:	53                   	push   ebx
1000dc9b:	8d 45 18             	lea    eax,[ebp+0x18]
1000dc9e:	50                   	push   eax
1000dc9f:	8d 45 1c             	lea    eax,[ebp+0x1c]
1000dca2:	50                   	push   eax
1000dca3:	56                   	push   esi
1000dca4:	57                   	push   edi
1000dca5:	57                   	push   edi
1000dca6:	57                   	push   edi
1000dca7:	6a 02                	push   0x2
1000dca9:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000dcac:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000dcaf:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1000dcb2:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1000dcb5:	ff 75 0c             	push   DWORD PTR [ebp+0xc]
1000dcb8:	ff 75 08             	push   DWORD PTR [ebp+0x8]
1000dcbb:	ff 75 24             	push   DWORD PTR [ebp+0x24]
1000dcbe:	e8 27 fb ff ff       	call   0x1000d7ea
1000dcc3:	6a 04                	push   0x4
1000dcc5:	bb 00 10 00 00       	mov    ebx,0x1000
1000dcca:	53                   	push   ebx
1000dccb:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000dcce:	57                   	push   edi
1000dccf:	8b 3d 24 70 09 10    	mov    edi,DWORD PTR ds:0x10097024
1000dcd5:	ff d7                	call   edi
1000dcd7:	85 c0                	test   eax,eax
1000dcd9:	89 45 1c             	mov    DWORD PTR [ebp+0x1c],eax
1000dcdc:	75 09                	jne    0x1000dce7
1000dcde:	56                   	push   esi
1000dcdf:	ff 15 28 70 09 10    	call   DWORD PTR ds:0x10097028
1000dce5:	eb 2b                	jmp    0x1000dd12
1000dce7:	83 7d 18 00          	cmp    DWORD PTR [ebp+0x18],0x0
1000dceb:	76 29                	jbe    0x1000dd16
1000dced:	6a 04                	push   0x4
1000dcef:	53                   	push   ebx
1000dcf0:	ff 75 18             	push   DWORD PTR [ebp+0x18]
1000dcf3:	6a 00                	push   0x0
1000dcf5:	ff d7                	call   edi
1000dcf7:	85 c0                	test   eax,eax
1000dcf9:	75 1e                	jne    0x1000dd19
1000dcfb:	56                   	push   esi
1000dcfc:	ff 15 28 70 09 10    	call   DWORD PTR ds:0x10097028
1000dd02:	68 00 80 00 00       	push   0x8000
1000dd07:	6a 00                	push   0x0
1000dd09:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1000dd0c:	ff 15 a8 70 09 10    	call   DWORD PTR ds:0x100970a8
1000dd12:	33 c0                	xor    eax,eax
1000dd14:	eb 24                	jmp    0x1000dd3a
1000dd16:	8b 45 fc             	mov    eax,DWORD PTR [ebp-0x4]
1000dd19:	8b 56 20             	mov    edx,DWORD PTR [esi+0x20]
1000dd1c:	33 55 1c             	xor    edx,DWORD PTR [ebp+0x1c]
1000dd1f:	b9 ff 0f 00 00       	mov    ecx,0xfff
1000dd24:	23 d1                	and    edx,ecx
1000dd26:	33 55 1c             	xor    edx,DWORD PTR [ebp+0x1c]
1000dd29:	89 56 20             	mov    DWORD PTR [esi+0x20],edx
1000dd2c:	8b 56 30             	mov    edx,DWORD PTR [esi+0x30]
1000dd2f:	33 d0                	xor    edx,eax
1000dd31:	23 d1                	and    edx,ecx
1000dd33:	33 d0                	xor    edx,eax
1000dd35:	89 56 30             	mov    DWORD PTR [esi+0x30],edx
1000dd38:	8b c6                	mov    eax,esi
1000dd3a:	5b                   	pop    ebx
1000dd3b:	5f                   	pop    edi
1000dd3c:	5e                   	pop    esi
1000dd3d:	c9                   	leave
1000dd3e:	c2 20 00             	ret    0x20
