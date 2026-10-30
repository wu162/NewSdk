
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

10016bff <.text+0x15bff>:
10016bff:	55                   	push   ebp
10016c00:	8d 6c 24 90          	lea    ebp,[esp-0x70]
10016c04:	81 ec a8 00 00 00    	sub    esp,0xa8
10016c0a:	a1 60 92 0b 10       	mov    eax,ds:0x100b9260
10016c0f:	33 c5                	xor    eax,ebp
10016c11:	89 45 6c             	mov    DWORD PTR [ebp+0x6c],eax
10016c14:	53                   	push   ebx
10016c15:	8b 5d 78             	mov    ebx,DWORD PTR [ebp+0x78]
10016c18:	56                   	push   esi
10016c19:	33 f6                	xor    esi,esi
10016c1b:	3b de                	cmp    ebx,esi
10016c1d:	57                   	push   edi
10016c1e:	8b f9                	mov    edi,ecx
10016c20:	89 5d d4             	mov    DWORD PTR [ebp-0x2c],ebx
10016c23:	75 0a                	jne    0x10016c2f
10016c25:	b8 6c 08 76 88       	mov    eax,0x8876086c
10016c2a:	e9 e4 00 00 00       	jmp    0x10016d13
10016c2f:	39 75 7c             	cmp    DWORD PTR [ebp+0x7c],esi
10016c32:	74 72                	je     0x10016ca6
10016c34:	8d 45 d8             	lea    eax,[ebp-0x28]
10016c37:	50                   	push   eax
10016c38:	c7 45 d8 94 00 00 00 	mov    DWORD PTR [ebp-0x28],0x94
10016c3f:	ff 15 68 70 09 10    	call   DWORD PTR ds:0x10097068
10016c45:	83 7d e8 02          	cmp    DWORD PTR [ebp-0x18],0x2
10016c49:	74 3d                	je     0x10016c88
10016c4b:	56                   	push   esi
10016c4c:	56                   	push   esi
10016c4d:	56                   	push   esi
10016c4e:	56                   	push   esi
10016c4f:	6a ff                	push   0xffffffff
10016c51:	53                   	push   ebx
10016c52:	8b 1d 6c 70 09 10    	mov    ebx,DWORD PTR ds:0x1009706c
10016c58:	56                   	push   esi
10016c59:	68 e9 fd 00 00       	push   0xfde9
10016c5e:	ff d3                	call   ebx
10016c60:	89 45 cc             	mov    DWORD PTR [ebp-0x34],eax
10016c63:	e8 68 81 03 00       	call   0x1004edd0
10016c68:	89 65 d0             	mov    DWORD PTR [ebp-0x30],esp
10016c6b:	56                   	push   esi
10016c6c:	56                   	push   esi
10016c6d:	ff 75 cc             	push   DWORD PTR [ebp-0x34]
10016c70:	ff 75 d0             	push   DWORD PTR [ebp-0x30]
10016c73:	6a ff                	push   0xffffffff
10016c75:	ff 75 d4             	push   DWORD PTR [ebp-0x2c]
10016c78:	56                   	push   esi
10016c79:	68 e9 fd 00 00       	push   0xfde9
10016c7e:	ff d3                	call   ebx
10016c80:	8b 45 d0             	mov    eax,DWORD PTR [ebp-0x30]
10016c83:	89 75 7c             	mov    DWORD PTR [ebp+0x7c],esi
10016c86:	8b d8                	mov    ebx,eax
10016c88:	39 75 7c             	cmp    DWORD PTR [ebp+0x7c],esi
10016c8b:	74 19                	je     0x10016ca6
10016c8d:	56                   	push   esi
10016c8e:	68 00 00 00 10       	push   0x10000000
10016c93:	6a 03                	push   0x3
10016c95:	56                   	push   esi
10016c96:	6a 01                	push   0x1
10016c98:	68 00 00 00 80       	push   0x80000000
10016c9d:	53                   	push   ebx
10016c9e:	ff 15 70 70 09 10    	call   DWORD PTR ds:0x10097070
10016ca4:	eb 17                	jmp    0x10016cbd
10016ca6:	56                   	push   esi
10016ca7:	68 00 00 00 10       	push   0x10000000
10016cac:	6a 03                	push   0x3
10016cae:	56                   	push   esi
10016caf:	6a 01                	push   0x1
10016cb1:	68 00 00 00 80       	push   0x80000000
10016cb6:	53                   	push   ebx
10016cb7:	ff 15 14 70 09 10    	call   DWORD PTR ds:0x10097014
10016cbd:	83 f8 ff             	cmp    eax,0xffffffff
10016cc0:	89 07                	mov    DWORD PTR [edi],eax
10016cc2:	74 40                	je     0x10016d04
10016cc4:	56                   	push   esi
10016cc5:	50                   	push   eax
10016cc6:	ff 15 10 70 09 10    	call   DWORD PTR ds:0x10097010
10016ccc:	83 f8 ff             	cmp    eax,0xffffffff
10016ccf:	89 47 08             	mov    DWORD PTR [edi+0x8],eax
10016cd2:	74 30                	je     0x10016d04
10016cd4:	3b c6                	cmp    eax,esi
10016cd6:	74 0f                	je     0x10016ce7
10016cd8:	50                   	push   eax
10016cd9:	e8 96 b3 ff ff       	call   0x10012074
10016cde:	3b c6                	cmp    eax,esi
10016ce0:	89 47 04             	mov    DWORD PTR [edi+0x4],eax
10016ce3:	74 1f                	je     0x10016d04
10016ce5:	eb 03                	jmp    0x10016cea
10016ce7:	89 77 04             	mov    DWORD PTR [edi+0x4],esi
10016cea:	56                   	push   esi
10016ceb:	8d 45 c8             	lea    eax,[ebp-0x38]
10016cee:	50                   	push   eax
10016cef:	ff 77 08             	push   DWORD PTR [edi+0x8]
10016cf2:	89 75 c8             	mov    DWORD PTR [ebp-0x38],esi
10016cf5:	ff 77 04             	push   DWORD PTR [edi+0x4]
10016cf8:	ff 37                	push   DWORD PTR [edi]
10016cfa:	ff 15 1c 70 09 10    	call   DWORD PTR ds:0x1009701c
10016d00:	85 c0                	test   eax,eax
10016d02:	75 0d                	jne    0x10016d11
10016d04:	ff 15 74 70 09 10    	call   DWORD PTR ds:0x10097074
10016d0a:	b8 59 0b 76 88       	mov    eax,0x88760b59
10016d0f:	eb 02                	jmp    0x10016d13
10016d11:	33 c0                	xor    eax,eax
10016d13:	8d 65 bc             	lea    esp,[ebp-0x44]
10016d16:	5f                   	pop    edi
10016d17:	5e                   	pop    esi
10016d18:	5b                   	pop    ebx
10016d19:	8b 4d 6c             	mov    ecx,DWORD PTR [ebp+0x6c]
10016d1c:	33 cd                	xor    ecx,ebp
10016d1e:	e8 5c 80 03 00       	call   0x1004ed7f
10016d23:	83 c5 70             	add    ebp,0x70
10016d26:	c9                   	leave
10016d27:	c2 08 00             	ret    0x8
