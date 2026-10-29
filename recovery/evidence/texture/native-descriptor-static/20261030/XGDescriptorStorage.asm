
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003da87 <.text+0x3ca87>:
1003da87:	55                   	push   ebp
1003da88:	8b ec                	mov    ebp,esp
1003da8a:	83 ec 2c             	sub    esp,0x2c
1003da8d:	53                   	push   ebx
1003da8e:	56                   	push   esi
1003da8f:	57                   	push   edi
1003da90:	8b f8                	mov    edi,eax
1003da92:	8b 47 30             	mov    eax,DWORD PTR [edi+0x30]
1003da95:	8b 5f 20             	mov    ebx,DWORD PTR [edi+0x20]
1003da98:	8b 77 28             	mov    esi,DWORD PTR [edi+0x28]
1003da9b:	c1 e8 09             	shr    eax,0x9
1003da9e:	83 e0 03             	and    eax,0x3
1003daa1:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1003daa4:	83 e3 3f             	and    ebx,0x3f
1003daa7:	0f b6 04 5d 30 82 09 	movzx  eax,BYTE PTR [ebx*2+0x10098230]
1003daae:	10 
1003daaf:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
1003dab2:	8b 47 1c             	mov    eax,DWORD PTR [edi+0x1c]
1003dab5:	c1 e8 1f             	shr    eax,0x1f
1003dab8:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1003dabb:	c1 ee 1f             	shr    esi,0x1f
1003dabe:	8d 04 36             	lea    eax,[esi+esi*1]
1003dac1:	89 45 e4             	mov    DWORD PTR [ebp-0x1c],eax
1003dac4:	8d 45 d4             	lea    eax,[ebp-0x2c]
1003dac7:	50                   	push   eax
1003dac8:	8d 45 ec             	lea    eax,[ebp-0x14]
1003dacb:	50                   	push   eax
1003dacc:	8d 45 e8             	lea    eax,[ebp-0x18]
1003dacf:	50                   	push   eax
1003dad0:	57                   	push   edi
1003dad1:	e8 52 eb fc ff       	call   0x1000c628
1003dad6:	8b 45 e8             	mov    eax,DWORD PTR [ebp-0x18]
1003dad9:	2b 45 e4             	sub    eax,DWORD PTR [ebp-0x1c]
1003dadc:	48                   	dec    eax
1003dadd:	50                   	push   eax
1003dade:	e8 cb ec fc ff       	call   0x1000c7ae
1003dae3:	8b c8                	mov    ecx,eax
1003dae5:	8b c6                	mov    eax,esi
1003dae7:	2b c1                	sub    eax,ecx
1003dae9:	83 c0 20             	add    eax,0x20
1003daec:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1003daef:	8b 45 ec             	mov    eax,DWORD PTR [ebp-0x14]
1003daf2:	2b 45 e4             	sub    eax,DWORD PTR [ebp-0x1c]
1003daf5:	48                   	dec    eax
1003daf6:	50                   	push   eax
1003daf7:	e8 b2 ec fc ff       	call   0x1000c7ae
1003dafc:	8b c8                	mov    ecx,eax
1003dafe:	8b c6                	mov    eax,esi
1003db00:	2b c1                	sub    eax,ecx
1003db02:	83 c0 20             	add    eax,0x20
1003db05:	83 7d 18 00          	cmp    DWORD PTR [ebp+0x18],0x0
1003db09:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1003db0c:	75 73                	jne    0x1003db81
1003db0e:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1003db11:	33 f6                	xor    esi,esi
1003db13:	46                   	inc    esi
1003db14:	8b d6                	mov    edx,esi
1003db16:	d3 e2                	shl    edx,cl
1003db18:	8b c8                	mov    ecx,eax
1003db1a:	d3 e6                	shl    esi,cl
1003db1c:	33 c0                	xor    eax,eax
1003db1e:	40                   	inc    eax
1003db1f:	83 fa 10             	cmp    edx,0x10
1003db22:	89 55 fc             	mov    DWORD PTR [ebp-0x4],edx
1003db25:	89 75 f8             	mov    DWORD PTR [ebp-0x8],esi
1003db28:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1003db2b:	76 05                	jbe    0x1003db32
1003db2d:	83 fe 10             	cmp    esi,0x10
1003db30:	77 0d                	ja     0x1003db3f
1003db32:	66 f7 47 30 00 08    	test   WORD PTR [edi+0x30],0x800
1003db38:	74 05                	je     0x1003db3f
1003db3a:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1003db3d:	eb 13                	jmp    0x1003db52
1003db3f:	8b 4d e8             	mov    ecx,DWORD PTR [ebp-0x18]
1003db42:	83 65 f0 00          	and    DWORD PTR [ebp-0x10],0x0
1003db46:	89 4d fc             	mov    DWORD PTR [ebp-0x4],ecx
1003db49:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1003db4c:	89 4d f8             	mov    DWORD PTR [ebp-0x8],ecx
1003db4f:	89 45 f4             	mov    DWORD PTR [ebp-0xc],eax
1003db52:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1003db55:	8d 45 f4             	lea    eax,[ebp-0xc]
1003db58:	ff 75 e0             	push   DWORD PTR [ebp-0x20]
1003db5b:	53                   	push   ebx
1003db5c:	ff 75 d8             	push   DWORD PTR [ebp-0x28]
1003db5f:	50                   	push   eax
1003db60:	8d 45 f8             	lea    eax,[ebp-0x8]
1003db63:	50                   	push   eax
1003db64:	8d 45 fc             	lea    eax,[ebp-0x4]
1003db67:	50                   	push   eax
1003db68:	e8 b0 ed fc ff       	call   0x1000c91d
1003db6d:	8b 7f 1c             	mov    edi,DWORD PTR [edi+0x1c]
1003db70:	c1 ef 16             	shr    edi,0x16
1003db73:	81 e7 ff 01 00 00    	and    edi,0x1ff
1003db79:	c1 e7 05             	shl    edi,0x5
1003db7c:	e9 9c 00 00 00       	jmp    0x1003dc1d
1003db81:	56                   	push   esi
1003db82:	ff 75 ec             	push   DWORD PTR [ebp-0x14]
1003db85:	ff 75 e8             	push   DWORD PTR [ebp-0x18]
1003db88:	e8 4b ed fc ff       	call   0x1000c8d8
1003db8d:	39 45 18             	cmp    DWORD PTR [ebp+0x18],eax
1003db90:	72 2c                	jb     0x1003dbbe
1003db92:	66 f7 47 30 00 08    	test   WORD PTR [edi+0x30],0x800
1003db98:	74 24                	je     0x1003dbbe
1003db9a:	c7 45 f0 01 00 00 00 	mov    DWORD PTR [ebp-0x10],0x1
1003dba1:	89 45 18             	mov    DWORD PTR [ebp+0x18],eax
1003dba4:	83 7d e0 02          	cmp    DWORD PTR [ebp-0x20],0x2
1003dba8:	75 1a                	jne    0x1003dbc4
1003dbaa:	8b 45 d4             	mov    eax,DWORD PTR [ebp-0x2c]
1003dbad:	2b 45 e4             	sub    eax,DWORD PTR [ebp-0x1c]
1003dbb0:	48                   	dec    eax
1003dbb1:	50                   	push   eax
1003dbb2:	e8 f7 eb fc ff       	call   0x1000c7ae
1003dbb7:	2b f0                	sub    esi,eax
1003dbb9:	83 c6 20             	add    esi,0x20
1003dbbc:	eb 08                	jmp    0x1003dbc6
1003dbbe:	83 65 f0 00          	and    DWORD PTR [ebp-0x10],0x0
1003dbc2:	eb e0                	jmp    0x1003dba4
1003dbc4:	33 f6                	xor    esi,esi
1003dbc6:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1003dbc9:	2b 4d 18             	sub    ecx,DWORD PTR [ebp+0x18]
1003dbcc:	79 02                	jns    0x1003dbd0
1003dbce:	33 c9                	xor    ecx,ecx
1003dbd0:	8b 55 f8             	mov    edx,DWORD PTR [ebp-0x8]
1003dbd3:	2b 55 18             	sub    edx,DWORD PTR [ebp+0x18]
1003dbd6:	79 02                	jns    0x1003dbda
1003dbd8:	33 d2                	xor    edx,edx
1003dbda:	8b c6                	mov    eax,esi
1003dbdc:	2b 45 18             	sub    eax,DWORD PTR [ebp+0x18]
1003dbdf:	79 02                	jns    0x1003dbe3
1003dbe1:	33 c0                	xor    eax,eax
1003dbe3:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1003dbe6:	33 f6                	xor    esi,esi
1003dbe8:	ff 75 e0             	push   DWORD PTR [ebp-0x20]
1003dbeb:	46                   	inc    esi
1003dbec:	d3 e6                	shl    esi,cl
1003dbee:	8b ca                	mov    ecx,edx
1003dbf0:	53                   	push   ebx
1003dbf1:	ff 75 d8             	push   DWORD PTR [ebp-0x28]
1003dbf4:	89 75 fc             	mov    DWORD PTR [ebp-0x4],esi
1003dbf7:	33 f6                	xor    esi,esi
1003dbf9:	46                   	inc    esi
1003dbfa:	d3 e6                	shl    esi,cl
1003dbfc:	8b c8                	mov    ecx,eax
1003dbfe:	33 d2                	xor    edx,edx
1003dc00:	8d 45 f4             	lea    eax,[ebp-0xc]
1003dc03:	50                   	push   eax
1003dc04:	42                   	inc    edx
1003dc05:	8d 45 f8             	lea    eax,[ebp-0x8]
1003dc08:	d3 e2                	shl    edx,cl
1003dc0a:	50                   	push   eax
1003dc0b:	8d 45 fc             	lea    eax,[ebp-0x4]
1003dc0e:	50                   	push   eax
1003dc0f:	89 75 f8             	mov    DWORD PTR [ebp-0x8],esi
1003dc12:	89 55 f4             	mov    DWORD PTR [ebp-0xc],edx
1003dc15:	e8 03 ed fc ff       	call   0x1000c91d
1003dc1a:	8b 7d fc             	mov    edi,DWORD PTR [ebp-0x4]
1003dc1d:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1003dc20:	85 c0                	test   eax,eax
1003dc22:	74 02                	je     0x1003dc26
1003dc24:	89 38                	mov    DWORD PTR [eax],edi
1003dc26:	8b 45 0c             	mov    eax,DWORD PTR [ebp+0xc]
1003dc29:	85 c0                	test   eax,eax
1003dc2b:	5f                   	pop    edi
1003dc2c:	5e                   	pop    esi
1003dc2d:	5b                   	pop    ebx
1003dc2e:	74 05                	je     0x1003dc35
1003dc30:	8b 4d f8             	mov    ecx,DWORD PTR [ebp-0x8]
1003dc33:	89 08                	mov    DWORD PTR [eax],ecx
1003dc35:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1003dc38:	85 c0                	test   eax,eax
1003dc3a:	74 05                	je     0x1003dc41
1003dc3c:	8b 4d dc             	mov    ecx,DWORD PTR [ebp-0x24]
1003dc3f:	89 08                	mov    DWORD PTR [eax],ecx
1003dc41:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1003dc44:	85 c0                	test   eax,eax
1003dc46:	74 05                	je     0x1003dc4d
1003dc48:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1003dc4b:	89 08                	mov    DWORD PTR [eax],ecx
1003dc4d:	c9                   	leave
1003dc4e:	c2 14 00             	ret    0x14
