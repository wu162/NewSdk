
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003ccc0 <.text+0x3bcc0>:
1003ccc0:	55                   	push   ebp
1003ccc1:	8d 6c 24 b8          	lea    ebp,[esp-0x48]
1003ccc5:	81 ec 54 02 00 00    	sub    esp,0x254
1003cccb:	33 d2                	xor    edx,edx
1003cccd:	53                   	push   ebx
1003ccce:	8b 5d 74             	mov    ebx,DWORD PTR [ebp+0x74]
1003ccd1:	3b da                	cmp    ebx,edx
1003ccd3:	56                   	push   esi
1003ccd4:	57                   	push   edi
1003ccd5:	89 55 08             	mov    DWORD PTR [ebp+0x8],edx
1003ccd8:	89 55 0c             	mov    DWORD PTR [ebp+0xc],edx
1003ccdb:	89 55 bc             	mov    DWORD PTR [ebp-0x44],edx
1003ccde:	89 55 f4             	mov    DWORD PTR [ebp-0xc],edx
1003cce1:	c7 45 f8 01 00 02 00 	mov    DWORD PTR [ebp-0x8],0x20001
1003cce8:	c7 45 fc 01 00 04 00 	mov    DWORD PTR [ebp-0x4],0x40001
1003ccef:	c7 45 00 02 00 04 00 	mov    DWORD PTR [ebp+0x0],0x40002
1003ccf6:	75 24                	jne    0x1003cd1c
1003ccf8:	8b 45 58             	mov    eax,DWORD PTR [ebp+0x58]
1003ccfb:	89 85 3c ff ff ff    	mov    DWORD PTR [ebp-0xc4],eax
1003cd01:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003cd04:	89 95 34 ff ff ff    	mov    DWORD PTR [ebp-0xcc],edx
1003cd0a:	89 95 38 ff ff ff    	mov    DWORD PTR [ebp-0xc8],edx
1003cd10:	89 85 40 ff ff ff    	mov    DWORD PTR [ebp-0xc0],eax
1003cd16:	8d 9d 34 ff ff ff    	lea    ebx,[ebp-0xcc]
1003cd1c:	8b 43 08             	mov    eax,DWORD PTR [ebx+0x8]
1003cd1f:	2b 03                	sub    eax,DWORD PTR [ebx]
1003cd21:	89 45 40             	mov    DWORD PTR [ebp+0x40],eax
1003cd24:	8b 43 0c             	mov    eax,DWORD PTR [ebx+0xc]
1003cd27:	2b 43 04             	sub    eax,DWORD PTR [ebx+0x4]
1003cd2a:	39 55 64             	cmp    DWORD PTR [ebp+0x64],edx
1003cd2d:	89 45 24             	mov    DWORD PTR [ebp+0x24],eax
1003cd30:	75 15                	jne    0x1003cd47
1003cd32:	8d 85 54 ff ff ff    	lea    eax,[ebp-0xac]
1003cd38:	89 95 54 ff ff ff    	mov    DWORD PTR [ebp-0xac],edx
1003cd3e:	89 95 58 ff ff ff    	mov    DWORD PTR [ebp-0xa8],edx
1003cd44:	89 45 64             	mov    DWORD PTR [ebp+0x64],eax
1003cd47:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
1003cd4a:	8b 4d 70             	mov    ecx,DWORD PTR [ebp+0x70]
1003cd4d:	83 e0 3f             	and    eax,0x3f
1003cd50:	83 e1 3f             	and    ecx,0x3f
1003cd53:	89 45 34             	mov    DWORD PTR [ebp+0x34],eax
1003cd56:	89 4d 30             	mov    DWORD PTR [ebp+0x30],ecx
1003cd59:	0f 84 fb 0b 00 00    	je     0x1003d95a
1003cd5f:	83 f9 01             	cmp    ecx,0x1
1003cd62:	0f 84 f2 0b 00 00    	je     0x1003d95a
1003cd68:	3b c2                	cmp    eax,edx
1003cd6a:	0f 84 ea 0b 00 00    	je     0x1003d95a
1003cd70:	83 f8 01             	cmp    eax,0x1
1003cd73:	0f 84 e1 0b 00 00    	je     0x1003d95a
1003cd79:	8b 75 34             	mov    esi,DWORD PTR [ebp+0x34]
1003cd7c:	8d 45 cc             	lea    eax,[ebp-0x34]
1003cd7f:	50                   	push   eax
1003cd80:	8d 45 c8             	lea    eax,[ebp-0x38]
1003cd83:	50                   	push   eax
1003cd84:	56                   	push   esi
1003cd85:	e8 8b 18 00 00       	call   0x1003e615
1003cd8a:	8b 7d 30             	mov    edi,DWORD PTR [ebp+0x30]
1003cd8d:	8d 45 74             	lea    eax,[ebp+0x74]
1003cd90:	50                   	push   eax
1003cd91:	8d 45 38             	lea    eax,[ebp+0x38]
1003cd94:	50                   	push   eax
1003cd95:	57                   	push   edi
1003cd96:	e8 7a 18 00 00       	call   0x1003e615
1003cd9b:	0f b6 86 48 77 09 10 	movzx  eax,BYTE PTR [esi+0x10097748]
1003cda2:	0f b6 b7 48 77 09 10 	movzx  esi,BYTE PTR [edi+0x10097748]
1003cda9:	0f af 45 c8          	imul   eax,DWORD PTR [ebp-0x38]
1003cdad:	8b 7d 38             	mov    edi,DWORD PTR [ebp+0x38]
1003cdb0:	0f af 45 cc          	imul   eax,DWORD PTR [ebp-0x34]
1003cdb4:	8b 4d 70             	mov    ecx,DWORD PTR [ebp+0x70]
1003cdb7:	0f af f7             	imul   esi,edi
1003cdba:	0f af 75 74          	imul   esi,DWORD PTR [ebp+0x74]
1003cdbe:	c1 e8 03             	shr    eax,0x3
1003cdc1:	c1 ee 03             	shr    esi,0x3
1003cdc4:	39 4d 60             	cmp    DWORD PTR [ebp+0x60],ecx
1003cdc7:	89 45 28             	mov    DWORD PTR [ebp+0x28],eax
1003cdca:	89 75 20             	mov    DWORD PTR [ebp+0x20],esi
1003cdcd:	0f 85 24 01 00 00    	jne    0x1003cef7
1003cdd3:	8b 4d 78             	mov    ecx,DWORD PTR [ebp+0x78]
1003cdd6:	8b d1                	mov    edx,ecx
1003cdd8:	d1 ea                	shr    edx,1
1003cdda:	33 d1                	xor    edx,ecx
1003cddc:	f6 c2 01             	test   dl,0x1
1003cddf:	0f 85 12 01 00 00    	jne    0x1003cef7
1003cde5:	8b d1                	mov    edx,ecx
1003cde7:	d1 ea                	shr    edx,1
1003cde9:	33 d1                	xor    edx,ecx
1003cdeb:	f6 c2 04             	test   dl,0x4
1003cdee:	0f 85 03 01 00 00    	jne    0x1003cef7
1003cdf4:	8b 4d 64             	mov    ecx,DWORD PTR [ebp+0x64]
1003cdf7:	8b 55 c8             	mov    edx,DWORD PTR [ebp-0x38]
1003cdfa:	8b 09                	mov    ecx,DWORD PTR [ecx]
1003cdfc:	4a                   	dec    edx
1003cdfd:	85 d1                	test   ecx,edx
1003cdff:	89 4d 1c             	mov    DWORD PTR [ebp+0x1c],ecx
1003ce02:	0f 85 ef 00 00 00    	jne    0x1003cef7
1003ce08:	8b 4d 64             	mov    ecx,DWORD PTR [ebp+0x64]
1003ce0b:	8b 55 cc             	mov    edx,DWORD PTR [ebp-0x34]
1003ce0e:	8b 49 04             	mov    ecx,DWORD PTR [ecx+0x4]
1003ce11:	4a                   	dec    edx
1003ce12:	85 d1                	test   ecx,edx
1003ce14:	89 4d 3c             	mov    DWORD PTR [ebp+0x3c],ecx
1003ce17:	0f 85 da 00 00 00    	jne    0x1003cef7
1003ce1d:	8b 0b                	mov    ecx,DWORD PTR [ebx]
1003ce1f:	8d 57 ff             	lea    edx,[edi-0x1]
1003ce22:	85 d1                	test   ecx,edx
1003ce24:	89 4d 2c             	mov    DWORD PTR [ebp+0x2c],ecx
1003ce27:	0f 85 ca 00 00 00    	jne    0x1003cef7
1003ce2d:	8b 55 74             	mov    edx,DWORD PTR [ebp+0x74]
1003ce30:	8b 4b 04             	mov    ecx,DWORD PTR [ebx+0x4]
1003ce33:	4a                   	dec    edx
1003ce34:	85 d1                	test   ecx,edx
1003ce36:	89 4d 18             	mov    DWORD PTR [ebp+0x18],ecx
1003ce39:	0f 85 b8 00 00 00    	jne    0x1003cef7
1003ce3f:	8b 4b 08             	mov    ecx,DWORD PTR [ebx+0x8]
1003ce42:	8d 57 ff             	lea    edx,[edi-0x1]
1003ce45:	85 d1                	test   ecx,edx
1003ce47:	74 09                	je     0x1003ce52
1003ce49:	3b 4d 58             	cmp    ecx,DWORD PTR [ebp+0x58]
1003ce4c:	0f 85 a5 00 00 00    	jne    0x1003cef7
1003ce52:	8b 55 74             	mov    edx,DWORD PTR [ebp+0x74]
1003ce55:	8b 4b 0c             	mov    ecx,DWORD PTR [ebx+0xc]
1003ce58:	4a                   	dec    edx
1003ce59:	85 d1                	test   ecx,edx
1003ce5b:	74 09                	je     0x1003ce66
1003ce5d:	3b 4d 5c             	cmp    ecx,DWORD PTR [ebp+0x5c]
1003ce60:	0f 85 91 00 00 00    	jne    0x1003cef7
1003ce66:	8b 45 40             	mov    eax,DWORD PTR [ebp+0x40]
1003ce69:	8d 44 07 ff          	lea    eax,[edi+eax*1-0x1]
1003ce6d:	33 d2                	xor    edx,edx
1003ce6f:	f7 f7                	div    edi
1003ce71:	8b 4d 74             	mov    ecx,DWORD PTR [ebp+0x74]
1003ce74:	33 d2                	xor    edx,edx
1003ce76:	89 45 70             	mov    DWORD PTR [ebp+0x70],eax
1003ce79:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1003ce7c:	8d 44 01 ff          	lea    eax,[ecx+eax*1-0x1]
1003ce80:	f7 f1                	div    ecx
1003ce82:	33 d2                	xor    edx,edx
1003ce84:	8b d8                	mov    ebx,eax
1003ce86:	8b 45 1c             	mov    eax,DWORD PTR [ebp+0x1c]
1003ce89:	f7 75 c8             	div    DWORD PTR [ebp-0x38]
1003ce8c:	33 d2                	xor    edx,edx
1003ce8e:	0f af 45 28          	imul   eax,DWORD PTR [ebp+0x28]
1003ce92:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ce95:	8b 45 3c             	mov    eax,DWORD PTR [ebp+0x3c]
1003ce98:	f7 75 cc             	div    DWORD PTR [ebp-0x34]
1003ce9b:	33 d2                	xor    edx,edx
1003ce9d:	8b f0                	mov    esi,eax
1003ce9f:	0f af 75 54          	imul   esi,DWORD PTR [ebp+0x54]
1003cea3:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
1003cea6:	03 45 50             	add    eax,DWORD PTR [ebp+0x50]
1003cea9:	03 f0                	add    esi,eax
1003ceab:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1003ceae:	f7 f7                	div    edi
1003ceb0:	33 d2                	xor    edx,edx
1003ceb2:	0f af 45 20          	imul   eax,DWORD PTR [ebp+0x20]
1003ceb6:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ceb9:	8b 45 18             	mov    eax,DWORD PTR [ebp+0x18]
1003cebc:	f7 f1                	div    ecx
1003cebe:	8b f8                	mov    edi,eax
1003cec0:	0f af 7d 6c          	imul   edi,DWORD PTR [ebp+0x6c]
1003cec4:	8b 45 60             	mov    eax,DWORD PTR [ebp+0x60]
1003cec7:	03 45 68             	add    eax,DWORD PTR [ebp+0x68]
1003ceca:	03 f8                	add    edi,eax
1003cecc:	85 db                	test   ebx,ebx
1003cece:	76 20                	jbe    0x1003cef0
1003ced0:	8b 45 70             	mov    eax,DWORD PTR [ebp+0x70]
1003ced3:	0f af 45 20          	imul   eax,DWORD PTR [ebp+0x20]
1003ced7:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003ceda:	ff 75 60             	push   DWORD PTR [ebp+0x60]
1003cedd:	57                   	push   edi
1003cede:	56                   	push   esi
1003cedf:	e8 62 18 01 00       	call   0x1004e746
1003cee4:	03 75 54             	add    esi,DWORD PTR [ebp+0x54]
1003cee7:	03 7d 6c             	add    edi,DWORD PTR [ebp+0x6c]
1003ceea:	83 c4 0c             	add    esp,0xc
1003ceed:	4b                   	dec    ebx
1003ceee:	75 ea                	jne    0x1003ceda
1003cef0:	33 c0                	xor    eax,eax
1003cef2:	e9 68 0a 00 00       	jmp    0x1003d95f
1003cef7:	8b 4d 60             	mov    ecx,DWORD PTR [ebp+0x60]
1003cefa:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003cefd:	c1 f9 11             	sar    ecx,0x11
1003cf00:	83 e1 01             	and    ecx,0x1
1003cf03:	89 4d 3c             	mov    DWORD PTR [ebp+0x3c],ecx
1003cf06:	8b ca                	mov    ecx,edx
1003cf08:	c1 fa 06             	sar    edx,0x6
1003cf0b:	83 e2 03             	and    edx,0x3
1003cf0e:	89 55 1c             	mov    DWORD PTR [ebp+0x1c],edx
1003cf11:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
1003cf14:	c1 fa 09             	sar    edx,0x9
1003cf17:	83 e2 03             	and    edx,0x3
1003cf1a:	89 55 e4             	mov    DWORD PTR [ebp-0x1c],edx
1003cf1d:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
1003cf20:	c1 fa 0b             	sar    edx,0xb
1003cf23:	83 e2 03             	and    edx,0x3
1003cf26:	89 55 e8             	mov    DWORD PTR [ebp-0x18],edx
1003cf29:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
1003cf2c:	c1 fa 0d             	sar    edx,0xd
1003cf2f:	83 e2 03             	and    edx,0x3
1003cf32:	89 55 ec             	mov    DWORD PTR [ebp-0x14],edx
1003cf35:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
1003cf38:	c1 fa 0f             	sar    edx,0xf
1003cf3b:	83 e2 03             	and    edx,0x3
1003cf3e:	89 55 f0             	mov    DWORD PTR [ebp-0x10],edx
1003cf41:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003cf44:	c1 fa 09             	sar    edx,0x9
1003cf47:	83 e2 03             	and    edx,0x3
1003cf4a:	89 55 d4             	mov    DWORD PTR [ebp-0x2c],edx
1003cf4d:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003cf50:	c1 fa 0b             	sar    edx,0xb
1003cf53:	83 e2 03             	and    edx,0x3
1003cf56:	89 55 d8             	mov    DWORD PTR [ebp-0x28],edx
1003cf59:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003cf5c:	c1 f9 11             	sar    ecx,0x11
1003cf5f:	c1 fa 0d             	sar    edx,0xd
1003cf62:	83 e1 01             	and    ecx,0x1
1003cf65:	83 e2 03             	and    edx,0x3
1003cf68:	89 4d 2c             	mov    DWORD PTR [ebp+0x2c],ecx
1003cf6b:	8b 4d 60             	mov    ecx,DWORD PTR [ebp+0x60]
1003cf6e:	89 55 dc             	mov    DWORD PTR [ebp-0x24],edx
1003cf71:	8b 55 70             	mov    edx,DWORD PTR [ebp+0x70]
1003cf74:	c1 f9 06             	sar    ecx,0x6
1003cf77:	c1 fa 0f             	sar    edx,0xf
1003cf7a:	83 e1 03             	and    ecx,0x3
1003cf7d:	83 e2 03             	and    edx,0x3
1003cf80:	85 c9                	test   ecx,ecx
1003cf82:	89 55 e0             	mov    DWORD PTR [ebp-0x20],edx
1003cf85:	74 06                	je     0x1003cf8d
1003cf87:	8b 44 8d f4          	mov    eax,DWORD PTR [ebp+ecx*4-0xc]
1003cf8b:	eb 03                	jmp    0x1003cf90
1003cf8d:	c1 e0 10             	shl    eax,0x10
1003cf90:	89 45 44             	mov    DWORD PTR [ebp+0x44],eax
1003cf93:	8b 45 1c             	mov    eax,DWORD PTR [ebp+0x1c]
1003cf96:	85 c0                	test   eax,eax
1003cf98:	74 09                	je     0x1003cfa3
1003cf9a:	8b 44 85 f4          	mov    eax,DWORD PTR [ebp+eax*4-0xc]
1003cf9e:	89 45 14             	mov    DWORD PTR [ebp+0x14],eax
1003cfa1:	eb 06                	jmp    0x1003cfa9
1003cfa3:	c1 e6 10             	shl    esi,0x10
1003cfa6:	89 75 14             	mov    DWORD PTR [ebp+0x14],esi
1003cfa9:	8b 45 34             	mov    eax,DWORD PTR [ebp+0x34]
1003cfac:	6b c0 3c             	imul   eax,eax,0x3c
1003cfaf:	b9 90 03 0a 10       	mov    ecx,0x100a0390
1003cfb4:	03 c1                	add    eax,ecx
1003cfb6:	8b d0                	mov    edx,eax
1003cfb8:	8b 45 30             	mov    eax,DWORD PTR [ebp+0x30]
1003cfbb:	6b c0 3c             	imul   eax,eax,0x3c
1003cfbe:	03 c1                	add    eax,ecx
1003cfc0:	83 3a 02             	cmp    DWORD PTR [edx],0x2
1003cfc3:	89 55 10             	mov    DWORD PTR [ebp+0x10],edx
1003cfc6:	89 45 34             	mov    DWORD PTR [ebp+0x34],eax
1003cfc9:	0f 85 c1 00 00 00    	jne    0x1003d090
1003cfcf:	8b 45 58             	mov    eax,DWORD PTR [ebp+0x58]
1003cfd2:	8b 4d 74             	mov    ecx,DWORD PTR [ebp+0x74]
1003cfd5:	8d 74 07 ff          	lea    esi,[edi+eax*1-0x1]
1003cfd9:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003cfdc:	4f                   	dec    edi
1003cfdd:	8d 44 01 ff          	lea    eax,[ecx+eax*1-0x1]
1003cfe1:	f7 d7                	not    edi
1003cfe3:	23 f7                	and    esi,edi
1003cfe5:	49                   	dec    ecx
1003cfe6:	f7 d1                	not    ecx
1003cfe8:	23 c1                	and    eax,ecx
1003cfea:	0f af c6             	imul   eax,esi
1003cfed:	c1 e0 04             	shl    eax,0x4
1003cff0:	50                   	push   eax
1003cff1:	e8 7e 50 fd ff       	call   0x10012074
1003cff6:	8b f8                	mov    edi,eax
1003cff8:	85 ff                	test   edi,edi
1003cffa:	75 0a                	jne    0x1003d006
1003cffc:	b8 0e 00 07 80       	mov    eax,0x8007000e
1003d001:	e9 59 09 00 00       	jmp    0x1003d95f
1003d006:	d9 45 7c             	fld    DWORD PTR [ebp+0x7c]
1003d009:	8b 03                	mov    eax,DWORD PTR [ebx]
1003d00b:	51                   	push   ecx
1003d00c:	d9 1c 24             	fstp   DWORD PTR [esp]
1003d00f:	ff 75 78             	push   DWORD PTR [ebp+0x78]
1003d012:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1003d015:	8b 43 04             	mov    eax,DWORD PTR [ebx+0x4]
1003d018:	53                   	push   ebx
1003d019:	ff 75 70             	push   DWORD PTR [ebp+0x70]
1003d01c:	89 45 00             	mov    DWORD PTR [ebp+0x0],eax
1003d01f:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
1003d022:	8d 45 fc             	lea    eax,[ebp-0x4]
1003d025:	ff 75 68             	push   DWORD PTR [ebp+0x68]
1003d028:	89 75 44             	mov    DWORD PTR [ebp+0x44],esi
1003d02b:	c1 65 44 04          	shl    DWORD PTR [ebp+0x44],0x4
1003d02f:	50                   	push   eax
1003d030:	be a6 aa 22 1a       	mov    esi,0x1a22aaa6
1003d035:	56                   	push   esi
1003d036:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d039:	ff 75 58             	push   DWORD PTR [ebp+0x58]
1003d03c:	ff 75 44             	push   DWORD PTR [ebp+0x44]
1003d03f:	57                   	push   edi
1003d040:	e8 7b fc ff ff       	call   0x1003ccc0
1003d045:	85 c0                	test   eax,eax
1003d047:	89 45 70             	mov    DWORD PTR [ebp+0x70],eax
1003d04a:	7d 0e                	jge    0x1003d05a
1003d04c:	57                   	push   edi
1003d04d:	e8 38 50 fd ff       	call   0x1001208a
1003d052:	8b 45 70             	mov    eax,DWORD PTR [ebp+0x70]
1003d055:	e9 05 09 00 00       	jmp    0x1003d95f
1003d05a:	d9 45 7c             	fld    DWORD PTR [ebp+0x7c]
1003d05d:	51                   	push   ecx
1003d05e:	d9 1c 24             	fstp   DWORD PTR [esp]
1003d061:	ff 75 78             	push   DWORD PTR [ebp+0x78]
1003d064:	53                   	push   ebx
1003d065:	56                   	push   esi
1003d066:	ff 75 44             	push   DWORD PTR [ebp+0x44]
1003d069:	57                   	push   edi
1003d06a:	ff 75 64             	push   DWORD PTR [ebp+0x64]
1003d06d:	ff 75 60             	push   DWORD PTR [ebp+0x60]
1003d070:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d073:	ff 75 58             	push   DWORD PTR [ebp+0x58]
1003d076:	ff 75 54             	push   DWORD PTR [ebp+0x54]
1003d079:	ff 75 50             	push   DWORD PTR [ebp+0x50]
1003d07c:	e8 84 2f 00 00       	call   0x10040005
1003d081:	57                   	push   edi
1003d082:	8b f0                	mov    esi,eax
1003d084:	e8 01 50 fd ff       	call   0x1001208a
1003d089:	8b c6                	mov    eax,esi
1003d08b:	e9 cf 08 00 00       	jmp    0x1003d95f
1003d090:	33 c0                	xor    eax,eax
1003d092:	83 7c 05 e4 03       	cmp    DWORD PTR [ebp+eax*1-0x1c],0x3
1003d097:	75 0b                	jne    0x1003d0a4
1003d099:	33 c9                	xor    ecx,ecx
1003d09b:	39 4d 3c             	cmp    DWORD PTR [ebp+0x3c],ecx
1003d09e:	0f 94 c1             	sete   cl
1003d0a1:	89 4d 0c             	mov    DWORD PTR [ebp+0xc],ecx
1003d0a4:	83 7c 05 d4 03       	cmp    DWORD PTR [ebp+eax*1-0x2c],0x3
1003d0a9:	75 0b                	jne    0x1003d0b6
1003d0ab:	33 c9                	xor    ecx,ecx
1003d0ad:	39 4d 2c             	cmp    DWORD PTR [ebp+0x2c],ecx
1003d0b0:	0f 94 c1             	sete   cl
1003d0b3:	89 4d 08             	mov    DWORD PTR [ebp+0x8],ecx
1003d0b6:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1003d0b9:	33 4d 08             	xor    ecx,DWORD PTR [ebp+0x8]
1003d0bc:	74 07                	je     0x1003d0c5
1003d0be:	c7 45 bc 01 00 00 00 	mov    DWORD PTR [ebp-0x44],0x1
1003d0c5:	83 c0 04             	add    eax,0x4
1003d0c8:	83 f8 10             	cmp    eax,0x10
1003d0cb:	72 c5                	jb     0x1003d092
1003d0cd:	ff 75 60             	push   DWORD PTR [ebp+0x60]
1003d0d0:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d0d3:	50                   	push   eax
1003d0d4:	e8 a5 d1 ff ff       	call   0x1003a27e
1003d0d9:	ff 75 70             	push   DWORD PTR [ebp+0x70]
1003d0dc:	8b f0                	mov    esi,eax
1003d0de:	8d bd b4 fe ff ff    	lea    edi,[ebp-0x14c]
1003d0e4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0e5:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0e6:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0e7:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d0ea:	50                   	push   eax
1003d0eb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0ec:	e8 3c d1 ff ff       	call   0x1003a22d
1003d0f1:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003d0f4:	8b f0                	mov    esi,eax
1003d0f6:	8d bd a4 fe ff ff    	lea    edi,[ebp-0x15c]
1003d0fc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0fd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0fe:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d0ff:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d100:	e8 7b e1 ff ff       	call   0x1003b280
1003d105:	89 45 1c             	mov    DWORD PTR [ebp+0x1c],eax
1003d108:	8d 85 94 fe ff ff    	lea    eax,[ebp-0x16c]
1003d10e:	50                   	push   eax
1003d10f:	8d 85 c4 fe ff ff    	lea    eax,[ebp-0x13c]
1003d115:	50                   	push   eax
1003d116:	8d 85 74 fe ff ff    	lea    eax,[ebp-0x18c]
1003d11c:	50                   	push   eax
1003d11d:	8d 45 f4             	lea    eax,[ebp-0xc]
1003d120:	50                   	push   eax
1003d121:	8d 85 84 fe ff ff    	lea    eax,[ebp-0x17c]
1003d127:	50                   	push   eax
1003d128:	8d 85 e4 fe ff ff    	lea    eax,[ebp-0x11c]
1003d12e:	50                   	push   eax
1003d12f:	8d 45 e4             	lea    eax,[ebp-0x1c]
1003d132:	50                   	push   eax
1003d133:	ff 75 3c             	push   DWORD PTR [ebp+0x3c]
1003d136:	8b 45 10             	mov    eax,DWORD PTR [ebp+0x10]
1003d139:	e8 02 e4 ff ff       	call   0x1003b540
1003d13e:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003d141:	d9 45 7c             	fld    DWORD PTR [ebp+0x7c]
1003d144:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d147:	51                   	push   ecx
1003d148:	d9 1c 24             	fstp   DWORD PTR [esp]
1003d14b:	50                   	push   eax
1003d14c:	e8 d5 f9 ff ff       	call   0x1003cb26
1003d151:	d9 45 f4             	fld    DWORD PTR [ebp-0xc]
1003d154:	8b f0                	mov    esi,eax
1003d156:	8d 7d ac             	lea    edi,[ebp-0x54]
1003d159:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d15a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d15b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d15c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d15d:	d8 45 ac             	fadd   DWORD PTR [ebp-0x54]
1003d160:	8d 45 e4             	lea    eax,[ebp-0x1c]
1003d163:	d9 9d 7c ff ff ff    	fstp   DWORD PTR [ebp-0x84]
1003d169:	50                   	push   eax
1003d16a:	d9 45 f8             	fld    DWORD PTR [ebp-0x8]
1003d16d:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d170:	d8 45 b0             	fadd   DWORD PTR [ebp-0x50]
1003d173:	50                   	push   eax
1003d174:	d9 5d 80             	fstp   DWORD PTR [ebp-0x80]
1003d177:	d9 45 fc             	fld    DWORD PTR [ebp-0x4]
1003d17a:	d8 45 b4             	fadd   DWORD PTR [ebp-0x4c]
1003d17d:	d9 5d 84             	fstp   DWORD PTR [ebp-0x7c]
1003d180:	d9 45 00             	fld    DWORD PTR [ebp+0x0]
1003d183:	d8 45 b8             	fadd   DWORD PTR [ebp-0x48]
1003d186:	d9 5d 88             	fstp   DWORD PTR [ebp-0x78]
1003d189:	e8 5b d1 ff ff       	call   0x1003a2e9
1003d18e:	8b f0                	mov    esi,eax
1003d190:	8d bd d4 fe ff ff    	lea    edi,[ebp-0x12c]
1003d196:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d197:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d198:	8d 45 d4             	lea    eax,[ebp-0x2c]
1003d19b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d19c:	50                   	push   eax
1003d19d:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d1a0:	50                   	push   eax
1003d1a1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d1a2:	e8 42 d1 ff ff       	call   0x1003a2e9
1003d1a7:	8b f0                	mov    esi,eax
1003d1a9:	8b 45 64             	mov    eax,DWORD PTR [ebp+0x64]
1003d1ac:	8b 48 04             	mov    ecx,DWORD PTR [eax+0x4]
1003d1af:	8b 00                	mov    eax,DWORD PTR [eax]
1003d1b1:	0f af 4d 54          	imul   ecx,DWORD PTR [ebp+0x54]
1003d1b5:	0f af 45 28          	imul   eax,DWORD PTR [ebp+0x28]
1003d1b9:	03 4d 50             	add    ecx,DWORD PTR [ebp+0x50]
1003d1bc:	33 d2                	xor    edx,edx
1003d1be:	03 c1                	add    eax,ecx
1003d1c0:	89 45 50             	mov    DWORD PTR [ebp+0x50],eax
1003d1c3:	8b 03                	mov    eax,DWORD PTR [ebx]
1003d1c5:	f7 75 38             	div    DWORD PTR [ebp+0x38]
1003d1c8:	8b 4d 74             	mov    ecx,DWORD PTR [ebp+0x74]
1003d1cb:	8d bd 64 fe ff ff    	lea    edi,[ebp-0x19c]
1003d1d1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d1d2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d1d3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d1d4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d1d5:	33 d2                	xor    edx,edx
1003d1d7:	8b f0                	mov    esi,eax
1003d1d9:	8b 43 04             	mov    eax,DWORD PTR [ebx+0x4]
1003d1dc:	0f af 75 20          	imul   esi,DWORD PTR [ebp+0x20]
1003d1e0:	f7 f1                	div    ecx
1003d1e2:	0f af 4d 54          	imul   ecx,DWORD PTR [ebp+0x54]
1003d1e6:	03 75 68             	add    esi,DWORD PTR [ebp+0x68]
1003d1e9:	51                   	push   ecx
1003d1ea:	0f af 45 6c          	imul   eax,DWORD PTR [ebp+0x6c]
1003d1ee:	03 c6                	add    eax,esi
1003d1f0:	89 45 58             	mov    DWORD PTR [ebp+0x58],eax
1003d1f3:	e8 7c 4e fd ff       	call   0x10012074
1003d1f8:	85 c0                	test   eax,eax
1003d1fa:	89 45 5c             	mov    DWORD PTR [ebp+0x5c],eax
1003d1fd:	0f 84 f9 fd ff ff    	je     0x1003cffc
1003d203:	ff 75 6c             	push   DWORD PTR [ebp+0x6c]
1003d206:	e8 69 4e fd ff       	call   0x10012074
1003d20b:	33 f6                	xor    esi,esi
1003d20d:	3b c6                	cmp    eax,esi
1003d20f:	89 45 68             	mov    DWORD PTR [ebp+0x68],eax
1003d212:	75 0d                	jne    0x1003d221
1003d214:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d217:	e8 6e 4e fd ff       	call   0x1001208a
1003d21c:	e9 db fd ff ff       	jmp    0x1003cffc
1003d221:	8b 7d 34             	mov    edi,DWORD PTR [ebp+0x34]
1003d224:	83 3f 02             	cmp    DWORD PTR [edi],0x2
1003d227:	0f 85 e9 03 00 00    	jne    0x1003d616
1003d22d:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
1003d230:	0f af 45 74          	imul   eax,DWORD PTR [ebp+0x74]
1003d234:	c1 e0 04             	shl    eax,0x4
1003d237:	50                   	push   eax
1003d238:	e8 37 4e fd ff       	call   0x10012074
1003d23d:	3b c6                	cmp    eax,esi
1003d23f:	89 45 34             	mov    DWORD PTR [ebp+0x34],eax
1003d242:	75 0d                	jne    0x1003d251
1003d244:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d247:	e8 3e 4e fd ff       	call   0x1001208a
1003d24c:	ff 75 68             	push   DWORD PTR [ebp+0x68]
1003d24f:	eb c6                	jmp    0x1003d217
1003d251:	ff 75 78             	push   DWORD PTR [ebp+0x78]
1003d254:	ff 75 30             	push   DWORD PTR [ebp+0x30]
1003d257:	e8 1f f9 ff ff       	call   0x1003cb7b
1003d25c:	8b 7d 74             	mov    edi,DWORD PTR [ebp+0x74]
1003d25f:	4f                   	dec    edi
1003d260:	23 7b 04             	and    edi,DWORD PTR [ebx+0x4]
1003d263:	39 75 24             	cmp    DWORD PTR [ebp+0x24],esi
1003d266:	89 45 c4             	mov    DWORD PTR [ebp-0x3c],eax
1003d269:	89 7d 3c             	mov    DWORD PTR [ebp+0x3c],edi
1003d26c:	89 75 7c             	mov    DWORD PTR [ebp+0x7c],esi
1003d26f:	0f 86 94 03 00 00    	jbe    0x1003d609
1003d275:	8b 45 14             	mov    eax,DWORD PTR [ebp+0x14]
1003d278:	c1 e8 10             	shr    eax,0x10
1003d27b:	89 45 30             	mov    DWORD PTR [ebp+0x30],eax
1003d27e:	8b 4d 38             	mov    ecx,DWORD PTR [ebp+0x38]
1003d281:	8b 45 40             	mov    eax,DWORD PTR [ebp+0x40]
1003d284:	8d 44 01 ff          	lea    eax,[ecx+eax*1-0x1]
1003d288:	33 d2                	xor    edx,edx
1003d28a:	f7 f1                	div    ecx
1003d28c:	33 d2                	xor    edx,edx
1003d28e:	0f af 45 20          	imul   eax,DWORD PTR [ebp+0x20]
1003d292:	f7 75 30             	div    DWORD PTR [ebp+0x30]
1003d295:	50                   	push   eax
1003d296:	ff 75 30             	push   DWORD PTR [ebp+0x30]
1003d299:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1003d29c:	ff 75 58             	push   DWORD PTR [ebp+0x58]
1003d29f:	ff 75 68             	push   DWORD PTR [ebp+0x68]
1003d2a2:	e8 e5 1b 00 00       	call   0x1003ee8c
1003d2a7:	8b 75 38             	mov    esi,DWORD PTR [ebp+0x38]
1003d2aa:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003d2ad:	83 65 04 00          	and    DWORD PTR [ebp+0x4],0x0
1003d2b1:	4e                   	dec    esi
1003d2b2:	23 33                	and    esi,DWORD PTR [ebx]
1003d2b4:	83 7d 40 00          	cmp    DWORD PTR [ebp+0x40],0x0
1003d2b8:	89 45 70             	mov    DWORD PTR [ebp+0x70],eax
1003d2bb:	8b 45 68             	mov    eax,DWORD PTR [ebp+0x68]
1003d2be:	89 75 18             	mov    DWORD PTR [ebp+0x18],esi
1003d2c1:	89 45 64             	mov    DWORD PTR [ebp+0x64],eax
1003d2c4:	0f 86 be 02 00 00    	jbe    0x1003d588
1003d2ca:	ff 75 34             	push   DWORD PTR [ebp+0x34]
1003d2cd:	ff 75 64             	push   DWORD PTR [ebp+0x64]
1003d2d0:	ff 55 c4             	call   DWORD PTR [ebp-0x3c]
1003d2d3:	3b 7d 74             	cmp    edi,DWORD PTR [ebp+0x74]
1003d2d6:	8b cf                	mov    ecx,edi
1003d2d8:	89 4d 60             	mov    DWORD PTR [ebp+0x60],ecx
1003d2db:	0f 83 7c 02 00 00    	jae    0x1003d55d
1003d2e1:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
1003d2e4:	83 65 c0 00          	and    DWORD PTR [ebp-0x40],0x0
1003d2e8:	2b c7                	sub    eax,edi
1003d2ea:	89 45 2c             	mov    DWORD PTR [ebp+0x2c],eax
1003d2ed:	8d 14 08             	lea    edx,[eax+ecx*1]
1003d2f0:	3b 55 24             	cmp    edx,DWORD PTR [ebp+0x24]
1003d2f3:	0f 83 64 02 00 00    	jae    0x1003d55d
1003d2f9:	3b 75 38             	cmp    esi,DWORD PTR [ebp+0x38]
1003d2fc:	89 75 78             	mov    DWORD PTR [ebp+0x78],esi
1003d2ff:	0f 83 45 02 00 00    	jae    0x1003d54a
1003d305:	8b 45 04             	mov    eax,DWORD PTR [ebp+0x4]
1003d308:	83 65 d0 00          	and    DWORD PTR [ebp-0x30],0x0
1003d30c:	2b c6                	sub    eax,esi
1003d30e:	89 45 00             	mov    DWORD PTR [ebp+0x0],eax
1003d311:	eb 03                	jmp    0x1003d316
1003d313:	8b 45 00             	mov    eax,DWORD PTR [ebp+0x0]
1003d316:	8b 4d 78             	mov    ecx,DWORD PTR [ebp+0x78]
1003d319:	03 c1                	add    eax,ecx
1003d31b:	3b 45 40             	cmp    eax,DWORD PTR [ebp+0x40]
1003d31e:	0f 83 20 02 00 00    	jae    0x1003d544
1003d324:	8d b5 a4 fe ff ff    	lea    esi,[ebp-0x15c]
1003d32a:	8d bd 6c ff ff ff    	lea    edi,[ebp-0x94]
1003d330:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d331:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d332:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d333:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d334:	be d8 13 0a 10       	mov    esi,0x100a13d8
1003d339:	8d bd 24 ff ff ff    	lea    edi,[ebp-0xdc]
1003d33f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d340:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d341:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d342:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d343:	8b 75 60             	mov    esi,DWORD PTR [ebp+0x60]
1003d346:	0f af 75 38          	imul   esi,DWORD PTR [ebp+0x38]
1003d34a:	03 f1                	add    esi,ecx
1003d34c:	c1 e6 04             	shl    esi,0x4
1003d34f:	03 75 34             	add    esi,DWORD PTR [ebp+0x34]
1003d352:	8d bd 04 ff ff ff    	lea    edi,[ebp-0xfc]
1003d358:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d359:	8d 85 04 ff ff ff    	lea    eax,[ebp-0xfc]
1003d35f:	89 45 ac             	mov    DWORD PTR [ebp-0x54],eax
1003d362:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d363:	8d 85 24 ff ff ff    	lea    eax,[ebp-0xdc]
1003d369:	89 45 b0             	mov    DWORD PTR [ebp-0x50],eax
1003d36c:	8d 85 6c ff ff ff    	lea    eax,[ebp-0x94]
1003d372:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d373:	89 45 b4             	mov    DWORD PTR [ebp-0x4c],eax
1003d376:	8d 45 8c             	lea    eax,[ebp-0x74]
1003d379:	89 45 b8             	mov    DWORD PTR [ebp-0x48],eax
1003d37c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d37d:	33 c0                	xor    eax,eax
1003d37f:	0f b6 8c 05 6c ff ff 	movzx  ecx,BYTE PTR [ebp+eax*1-0x94]
1003d386:	ff 
1003d387:	8b d1                	mov    edx,ecx
1003d389:	c1 ea 04             	shr    edx,0x4
1003d38c:	81 e1 0f 00 00 80    	and    ecx,0x8000000f
1003d392:	79 05                	jns    0x1003d399
1003d394:	49                   	dec    ecx
1003d395:	83 c9 f0             	or     ecx,0xfffffff0
1003d398:	41                   	inc    ecx
1003d399:	8b 54 95 ac          	mov    edx,DWORD PTR [ebp+edx*4-0x54]
1003d39d:	83 f1 03             	xor    ecx,0x3
1003d3a0:	8a 0c 11             	mov    cl,BYTE PTR [ecx+edx*1]
1003d3a3:	88 4c 05 8c          	mov    BYTE PTR [ebp+eax*1-0x74],cl
1003d3a7:	40                   	inc    eax
1003d3a8:	83 f8 10             	cmp    eax,0x10
1003d3ab:	72 d2                	jb     0x1003d37f
1003d3ad:	83 7d bc 00          	cmp    DWORD PTR [ebp-0x44],0x0
1003d3b1:	8d 75 8c             	lea    esi,[ebp-0x74]
1003d3b4:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d3b7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3b8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3b9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3ba:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3bb:	74 69                	je     0x1003d426
1003d3bd:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
1003d3c1:	74 2d                	je     0x1003d3f0
1003d3c3:	83 ec 10             	sub    esp,0x10
1003d3c6:	8b fc                	mov    edi,esp
1003d3c8:	8d b5 64 fe ff ff    	lea    esi,[ebp-0x19c]
1003d3ce:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3cf:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3d0:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3d1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3d2:	83 ec 10             	sub    esp,0x10
1003d3d5:	8b fc                	mov    edi,esp
1003d3d7:	8d 75 8c             	lea    esi,[ebp-0x74]
1003d3da:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3db:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3dc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3dd:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d3e0:	50                   	push   eax
1003d3e1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3e2:	e8 84 e3 ff ff       	call   0x1003b76b
1003d3e7:	8b f0                	mov    esi,eax
1003d3e9:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d3ec:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3ed:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3ee:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3ef:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d3f0:	83 7d 0c 00          	cmp    DWORD PTR [ebp+0xc],0x0
1003d3f4:	74 30                	je     0x1003d426
1003d3f6:	83 ec 10             	sub    esp,0x10
1003d3f9:	8b fc                	mov    edi,esp
1003d3fb:	8d b5 d4 fe ff ff    	lea    esi,[ebp-0x12c]
1003d401:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d402:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d403:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d404:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d405:	83 ec 10             	sub    esp,0x10
1003d408:	8b fc                	mov    edi,esp
1003d40a:	8d 75 e4             	lea    esi,[ebp-0x1c]
1003d40d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d40e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d40f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d410:	8d 85 54 fe ff ff    	lea    eax,[ebp-0x1ac]
1003d416:	50                   	push   eax
1003d417:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d418:	e8 1f f8 ff ff       	call   0x1003cc3c
1003d41d:	8b f0                	mov    esi,eax
1003d41f:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d422:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d423:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d424:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d425:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d426:	8d b5 b4 fe ff ff    	lea    esi,[ebp-0x14c]
1003d42c:	8d bd 5c ff ff ff    	lea    edi,[ebp-0xa4]
1003d432:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d433:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d434:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d435:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d436:	be d8 13 0a 10       	mov    esi,0x100a13d8
1003d43b:	8d bd f4 fe ff ff    	lea    edi,[ebp-0x10c]
1003d441:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d442:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d443:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d444:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d445:	8d 75 e4             	lea    esi,[ebp-0x1c]
1003d448:	8d bd 14 ff ff ff    	lea    edi,[ebp-0xec]
1003d44e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d44f:	8d 85 14 ff ff ff    	lea    eax,[ebp-0xec]
1003d455:	89 45 d4             	mov    DWORD PTR [ebp-0x2c],eax
1003d458:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d459:	8d 85 f4 fe ff ff    	lea    eax,[ebp-0x10c]
1003d45f:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
1003d462:	8d 85 5c ff ff ff    	lea    eax,[ebp-0xa4]
1003d468:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d469:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1003d46c:	8d 85 44 ff ff ff    	lea    eax,[ebp-0xbc]
1003d472:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1003d475:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d476:	33 c0                	xor    eax,eax
1003d478:	0f b6 8c 05 5c ff ff 	movzx  ecx,BYTE PTR [ebp+eax*1-0xa4]
1003d47f:	ff 
1003d480:	8b d1                	mov    edx,ecx
1003d482:	c1 ea 04             	shr    edx,0x4
1003d485:	81 e1 0f 00 00 80    	and    ecx,0x8000000f
1003d48b:	79 05                	jns    0x1003d492
1003d48d:	49                   	dec    ecx
1003d48e:	83 c9 f0             	or     ecx,0xfffffff0
1003d491:	41                   	inc    ecx
1003d492:	8b 54 95 d4          	mov    edx,DWORD PTR [ebp+edx*4-0x2c]
1003d496:	83 f1 03             	xor    ecx,0x3
1003d499:	8a 0c 11             	mov    cl,BYTE PTR [ecx+edx*1]
1003d49c:	88 8c 05 44 ff ff ff 	mov    BYTE PTR [ebp+eax*1-0xbc],cl
1003d4a3:	40                   	inc    eax
1003d4a4:	83 f8 10             	cmp    eax,0x10
1003d4a7:	72 cf                	jb     0x1003d478
1003d4a9:	83 ec 10             	sub    esp,0x10
1003d4ac:	8b fc                	mov    edi,esp
1003d4ae:	8d b5 94 fe ff ff    	lea    esi,[ebp-0x16c]
1003d4b4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4b5:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4b6:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4b7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4b8:	83 ec 10             	sub    esp,0x10
1003d4bb:	8b fc                	mov    edi,esp
1003d4bd:	8d b5 c4 fe ff ff    	lea    esi,[ebp-0x13c]
1003d4c3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4c4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4c5:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4c6:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4c7:	83 ec 10             	sub    esp,0x10
1003d4ca:	8b fc                	mov    edi,esp
1003d4cc:	8d b5 74 fe ff ff    	lea    esi,[ebp-0x18c]
1003d4d2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4d3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4d4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4d5:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4d6:	83 ec 10             	sub    esp,0x10
1003d4d9:	8b fc                	mov    edi,esp
1003d4db:	8d b5 7c ff ff ff    	lea    esi,[ebp-0x84]
1003d4e1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4e2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4e3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4e4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4e5:	83 ec 10             	sub    esp,0x10
1003d4e8:	8b fc                	mov    edi,esp
1003d4ea:	8d b5 84 fe ff ff    	lea    esi,[ebp-0x17c]
1003d4f0:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4f1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4f2:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4f3:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d4f4:	83 ec 10             	sub    esp,0x10
1003d4f7:	8b 4d c0             	mov    ecx,DWORD PTR [ebp-0x40]
1003d4fa:	8b fc                	mov    edi,esp
1003d4fc:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1003d4ff:	8b 45 70             	mov    eax,DWORD PTR [ebp+0x70]
1003d502:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003d505:	8d b5 e4 fe ff ff    	lea    esi,[ebp-0x11c]
1003d50b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d50c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d50d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d50e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d50f:	83 ec 10             	sub    esp,0x10
1003d512:	8b fc                	mov    edi,esp
1003d514:	8d b5 44 ff ff ff    	lea    esi,[ebp-0xbc]
1003d51a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d51b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d51c:	03 c1                	add    eax,ecx
1003d51e:	03 45 d0             	add    eax,DWORD PTR [ebp-0x30]
1003d521:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d522:	50                   	push   eax
1003d523:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d524:	e8 32 f3 ff ff       	call   0x1003c85b
1003d529:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1003d52c:	ff 45 78             	inc    DWORD PTR [ebp+0x78]
1003d52f:	01 45 d0             	add    DWORD PTR [ebp-0x30],eax
1003d532:	8b 45 78             	mov    eax,DWORD PTR [ebp+0x78]
1003d535:	3b 45 38             	cmp    eax,DWORD PTR [ebp+0x38]
1003d538:	8b 7d 3c             	mov    edi,DWORD PTR [ebp+0x3c]
1003d53b:	8b 75 18             	mov    esi,DWORD PTR [ebp+0x18]
1003d53e:	0f 82 cf fd ff ff    	jb     0x1003d313
1003d544:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1003d547:	8b 4d 60             	mov    ecx,DWORD PTR [ebp+0x60]
1003d54a:	8b 55 54             	mov    edx,DWORD PTR [ebp+0x54]
1003d54d:	01 55 c0             	add    DWORD PTR [ebp-0x40],edx
1003d550:	41                   	inc    ecx
1003d551:	3b 4d 74             	cmp    ecx,DWORD PTR [ebp+0x74]
1003d554:	89 4d 60             	mov    DWORD PTR [ebp+0x60],ecx
1003d557:	0f 82 90 fd ff ff    	jb     0x1003d2ed
1003d55d:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
1003d560:	2b c6                	sub    eax,esi
1003d562:	01 45 04             	add    DWORD PTR [ebp+0x4],eax
1003d565:	8b 45 38             	mov    eax,DWORD PTR [ebp+0x38]
1003d568:	2b c6                	sub    eax,esi
1003d56a:	0f af 45 28          	imul   eax,DWORD PTR [ebp+0x28]
1003d56e:	01 45 70             	add    DWORD PTR [ebp+0x70],eax
1003d571:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1003d574:	01 45 64             	add    DWORD PTR [ebp+0x64],eax
1003d577:	8b 45 04             	mov    eax,DWORD PTR [ebp+0x4]
1003d57a:	33 f6                	xor    esi,esi
1003d57c:	3b 45 40             	cmp    eax,DWORD PTR [ebp+0x40]
1003d57f:	89 75 18             	mov    DWORD PTR [ebp+0x18],esi
1003d582:	0f 82 42 fd ff ff    	jb     0x1003d2ca
1003d588:	8b 4d 74             	mov    ecx,DWORD PTR [ebp+0x74]
1003d58b:	3b f9                	cmp    edi,ecx
1003d58d:	89 7d 60             	mov    DWORD PTR [ebp+0x60],edi
1003d590:	73 50                	jae    0x1003d5e2
1003d592:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
1003d595:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003d598:	2b c6                	sub    eax,esi
1003d59a:	89 45 70             	mov    DWORD PTR [ebp+0x70],eax
1003d59d:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
1003d5a0:	2b c7                	sub    eax,edi
1003d5a2:	89 45 2c             	mov    DWORD PTR [ebp+0x2c],eax
1003d5a5:	8b 45 2c             	mov    eax,DWORD PTR [ebp+0x2c]
1003d5a8:	8b 55 60             	mov    edx,DWORD PTR [ebp+0x60]
1003d5ab:	03 c2                	add    eax,edx
1003d5ad:	3b 45 24             	cmp    eax,DWORD PTR [ebp+0x24]
1003d5b0:	73 30                	jae    0x1003d5e2
1003d5b2:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1003d5b5:	0f af 45 40          	imul   eax,DWORD PTR [ebp+0x40]
1003d5b9:	8b 4d 44             	mov    ecx,DWORD PTR [ebp+0x44]
1003d5bc:	c1 e9 10             	shr    ecx,0x10
1003d5bf:	33 d2                	xor    edx,edx
1003d5c1:	f7 f1                	div    ecx
1003d5c3:	50                   	push   eax
1003d5c4:	8b 45 70             	mov    eax,DWORD PTR [ebp+0x70]
1003d5c7:	51                   	push   ecx
1003d5c8:	ff 75 44             	push   DWORD PTR [ebp+0x44]
1003d5cb:	03 c6                	add    eax,esi
1003d5cd:	50                   	push   eax
1003d5ce:	56                   	push   esi
1003d5cf:	e8 b8 18 00 00       	call   0x1003ee8c
1003d5d4:	ff 45 60             	inc    DWORD PTR [ebp+0x60]
1003d5d7:	8b 4d 74             	mov    ecx,DWORD PTR [ebp+0x74]
1003d5da:	03 75 54             	add    esi,DWORD PTR [ebp+0x54]
1003d5dd:	39 4d 60             	cmp    DWORD PTR [ebp+0x60],ecx
1003d5e0:	72 c3                	jb     0x1003d5a5
1003d5e2:	8b c1                	mov    eax,ecx
1003d5e4:	2b c7                	sub    eax,edi
1003d5e6:	0f af 45 54          	imul   eax,DWORD PTR [ebp+0x54]
1003d5ea:	01 45 50             	add    DWORD PTR [ebp+0x50],eax
1003d5ed:	8b 45 6c             	mov    eax,DWORD PTR [ebp+0x6c]
1003d5f0:	01 45 58             	add    DWORD PTR [ebp+0x58],eax
1003d5f3:	2b cf                	sub    ecx,edi
1003d5f5:	01 4d 7c             	add    DWORD PTR [ebp+0x7c],ecx
1003d5f8:	8b 45 7c             	mov    eax,DWORD PTR [ebp+0x7c]
1003d5fb:	33 ff                	xor    edi,edi
1003d5fd:	3b 45 24             	cmp    eax,DWORD PTR [ebp+0x24]
1003d600:	89 7d 3c             	mov    DWORD PTR [ebp+0x3c],edi
1003d603:	0f 82 75 fc ff ff    	jb     0x1003d27e
1003d609:	ff 75 34             	push   DWORD PTR [ebp+0x34]
1003d60c:	e8 79 4a fd ff       	call   0x1001208a
1003d611:	e9 2f 03 00 00       	jmp    0x1003d945
1003d616:	57                   	push   edi
1003d617:	e8 7e db ff ff       	call   0x1003b19a
1003d61c:	89 45 00             	mov    DWORD PTR [ebp+0x0],eax
1003d61f:	8d 85 f4 fe ff ff    	lea    eax,[ebp-0x10c]
1003d625:	50                   	push   eax
1003d626:	8d 85 14 ff ff ff    	lea    eax,[ebp-0xec]
1003d62c:	50                   	push   eax
1003d62d:	8d 85 24 ff ff ff    	lea    eax,[ebp-0xdc]
1003d633:	50                   	push   eax
1003d634:	8d 85 04 ff ff ff    	lea    eax,[ebp-0xfc]
1003d63a:	50                   	push   eax
1003d63b:	8d 85 44 fe ff ff    	lea    eax,[ebp-0x1bc]
1003d641:	50                   	push   eax
1003d642:	8d 45 d4             	lea    eax,[ebp-0x2c]
1003d645:	50                   	push   eax
1003d646:	ff 75 2c             	push   DWORD PTR [ebp+0x2c]
1003d649:	8b c7                	mov    eax,edi
1003d64b:	e8 16 dd ff ff       	call   0x1003b366
1003d650:	39 75 24             	cmp    DWORD PTR [ebp+0x24],esi
1003d653:	0f 86 ec 02 00 00    	jbe    0x1003d945
1003d659:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1003d65c:	0f af 45 40          	imul   eax,DWORD PTR [ebp+0x40]
1003d660:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1003d663:	c1 e9 10             	shr    ecx,0x10
1003d666:	33 d2                	xor    edx,edx
1003d668:	f7 f1                	div    ecx
1003d66a:	89 4d 30             	mov    DWORD PTR [ebp+0x30],ecx
1003d66d:	8b 4d 44             	mov    ecx,DWORD PTR [ebp+0x44]
1003d670:	c1 e9 10             	shr    ecx,0x10
1003d673:	33 d2                	xor    edx,edx
1003d675:	89 4d 3c             	mov    DWORD PTR [ebp+0x3c],ecx
1003d678:	89 45 7c             	mov    DWORD PTR [ebp+0x7c],eax
1003d67b:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1003d67e:	0f af 45 40          	imul   eax,DWORD PTR [ebp+0x40]
1003d682:	f7 f1                	div    ecx
1003d684:	89 45 c4             	mov    DWORD PTR [ebp-0x3c],eax
1003d687:	8b 45 24             	mov    eax,DWORD PTR [ebp+0x24]
1003d68a:	89 45 78             	mov    DWORD PTR [ebp+0x78],eax
1003d68d:	ff 75 7c             	push   DWORD PTR [ebp+0x7c]
1003d690:	8b 75 68             	mov    esi,DWORD PTR [ebp+0x68]
1003d693:	ff 75 30             	push   DWORD PTR [ebp+0x30]
1003d696:	ff 75 14             	push   DWORD PTR [ebp+0x14]
1003d699:	ff 75 58             	push   DWORD PTR [ebp+0x58]
1003d69c:	56                   	push   esi
1003d69d:	e8 ea 17 00 00       	call   0x1003ee8c
1003d6a2:	83 7d 40 00          	cmp    DWORD PTR [ebp+0x40],0x0
1003d6a6:	8b 45 5c             	mov    eax,DWORD PTR [ebp+0x5c]
1003d6a9:	89 45 70             	mov    DWORD PTR [ebp+0x70],eax
1003d6ac:	89 75 64             	mov    DWORD PTR [ebp+0x64],esi
1003d6af:	0f 86 66 02 00 00    	jbe    0x1003d91b
1003d6b5:	8d 85 24 fe ff ff    	lea    eax,[ebp-0x1dc]
1003d6bb:	89 45 ac             	mov    DWORD PTR [ebp-0x54],eax
1003d6be:	8d 85 34 fe ff ff    	lea    eax,[ebp-0x1cc]
1003d6c4:	89 45 b0             	mov    DWORD PTR [ebp-0x50],eax
1003d6c7:	8d 85 44 ff ff ff    	lea    eax,[ebp-0xbc]
1003d6cd:	89 45 b4             	mov    DWORD PTR [ebp-0x4c],eax
1003d6d0:	8d 45 8c             	lea    eax,[ebp-0x74]
1003d6d3:	89 45 b8             	mov    DWORD PTR [ebp-0x48],eax
1003d6d6:	8d 45 9c             	lea    eax,[ebp-0x64]
1003d6d9:	89 45 d4             	mov    DWORD PTR [ebp-0x2c],eax
1003d6dc:	8d 85 54 fe ff ff    	lea    eax,[ebp-0x1ac]
1003d6e2:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
1003d6e5:	8d 85 5c ff ff ff    	lea    eax,[ebp-0xa4]
1003d6eb:	89 45 dc             	mov    DWORD PTR [ebp-0x24],eax
1003d6ee:	8d 85 6c ff ff ff    	lea    eax,[ebp-0x94]
1003d6f4:	89 45 e0             	mov    DWORD PTR [ebp-0x20],eax
1003d6f7:	8b 45 40             	mov    eax,DWORD PTR [ebp+0x40]
1003d6fa:	89 45 60             	mov    DWORD PTR [ebp+0x60],eax
1003d6fd:	bb d8 13 0a 10       	mov    ebx,0x100a13d8
1003d702:	83 ec 10             	sub    esp,0x10
1003d705:	8b fc                	mov    edi,esp
1003d707:	8d b5 f4 fe ff ff    	lea    esi,[ebp-0x10c]
1003d70d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d70e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d70f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d710:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d711:	83 ec 10             	sub    esp,0x10
1003d714:	8b fc                	mov    edi,esp
1003d716:	8d b5 14 ff ff ff    	lea    esi,[ebp-0xec]
1003d71c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d71d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d71e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d71f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d720:	83 ec 10             	sub    esp,0x10
1003d723:	8b fc                	mov    edi,esp
1003d725:	8d b5 24 ff ff ff    	lea    esi,[ebp-0xdc]
1003d72b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d72c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d72d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d72e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d72f:	83 ec 10             	sub    esp,0x10
1003d732:	8b fc                	mov    edi,esp
1003d734:	8d b5 04 ff ff ff    	lea    esi,[ebp-0xfc]
1003d73a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d73b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d73c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d73d:	83 ec 10             	sub    esp,0x10
1003d740:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d741:	8b fc                	mov    edi,esp
1003d743:	ff 75 00             	push   DWORD PTR [ebp+0x0]
1003d746:	8d b5 44 fe ff ff    	lea    esi,[ebp-0x1bc]
1003d74c:	ff 75 34             	push   DWORD PTR [ebp+0x34]
1003d74f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d750:	ff 75 64             	push   DWORD PTR [ebp+0x64]
1003d753:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d754:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d755:	8d 85 14 fe ff ff    	lea    eax,[ebp-0x1ec]
1003d75b:	50                   	push   eax
1003d75c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d75d:	e8 38 d2 ff ff       	call   0x1003a99a
1003d762:	8d b5 a4 fe ff ff    	lea    esi,[ebp-0x15c]
1003d768:	8d bd 44 ff ff ff    	lea    edi,[ebp-0xbc]
1003d76e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d76f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d770:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d771:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d772:	8b f3                	mov    esi,ebx
1003d774:	8d bd 34 fe ff ff    	lea    edi,[ebp-0x1cc]
1003d77a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d77b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d77c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d77d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d77e:	8b f0                	mov    esi,eax
1003d780:	8d bd 24 fe ff ff    	lea    edi,[ebp-0x1dc]
1003d786:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d787:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d788:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d789:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d78a:	33 c0                	xor    eax,eax
1003d78c:	0f b6 8c 05 44 ff ff 	movzx  ecx,BYTE PTR [ebp+eax*1-0xbc]
1003d793:	ff 
1003d794:	8b d1                	mov    edx,ecx
1003d796:	c1 ea 04             	shr    edx,0x4
1003d799:	81 e1 0f 00 00 80    	and    ecx,0x8000000f
1003d79f:	79 05                	jns    0x1003d7a6
1003d7a1:	49                   	dec    ecx
1003d7a2:	83 c9 f0             	or     ecx,0xfffffff0
1003d7a5:	41                   	inc    ecx
1003d7a6:	8b 54 95 ac          	mov    edx,DWORD PTR [ebp+edx*4-0x54]
1003d7aa:	83 f1 03             	xor    ecx,0x3
1003d7ad:	8a 0c 11             	mov    cl,BYTE PTR [ecx+edx*1]
1003d7b0:	88 4c 05 8c          	mov    BYTE PTR [ebp+eax*1-0x74],cl
1003d7b4:	40                   	inc    eax
1003d7b5:	83 f8 10             	cmp    eax,0x10
1003d7b8:	72 d2                	jb     0x1003d78c
1003d7ba:	83 7d bc 00          	cmp    DWORD PTR [ebp-0x44],0x0
1003d7be:	8d 75 8c             	lea    esi,[ebp-0x74]
1003d7c1:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d7c4:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7c5:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7c6:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7c7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7c8:	74 6c                	je     0x1003d836
1003d7ca:	83 7d 08 00          	cmp    DWORD PTR [ebp+0x8],0x0
1003d7ce:	74 30                	je     0x1003d800
1003d7d0:	83 ec 10             	sub    esp,0x10
1003d7d3:	8b fc                	mov    edi,esp
1003d7d5:	8d b5 64 fe ff ff    	lea    esi,[ebp-0x19c]
1003d7db:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7dc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7dd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7de:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7df:	83 ec 10             	sub    esp,0x10
1003d7e2:	8b fc                	mov    edi,esp
1003d7e4:	8d 75 8c             	lea    esi,[ebp-0x74]
1003d7e7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7e8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7e9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7ea:	8d 85 04 fe ff ff    	lea    eax,[ebp-0x1fc]
1003d7f0:	50                   	push   eax
1003d7f1:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7f2:	e8 74 df ff ff       	call   0x1003b76b
1003d7f7:	8b f0                	mov    esi,eax
1003d7f9:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d7fc:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7fd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7fe:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d7ff:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d800:	83 7d 0c 00          	cmp    DWORD PTR [ebp+0xc],0x0
1003d804:	74 30                	je     0x1003d836
1003d806:	83 ec 10             	sub    esp,0x10
1003d809:	8b fc                	mov    edi,esp
1003d80b:	8d b5 d4 fe ff ff    	lea    esi,[ebp-0x12c]
1003d811:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d812:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d813:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d814:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d815:	83 ec 10             	sub    esp,0x10
1003d818:	8b fc                	mov    edi,esp
1003d81a:	8d 75 e4             	lea    esi,[ebp-0x1c]
1003d81d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d81e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d81f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d820:	8d 85 f4 fd ff ff    	lea    eax,[ebp-0x20c]
1003d826:	50                   	push   eax
1003d827:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d828:	e8 0f f4 ff ff       	call   0x1003cc3c
1003d82d:	8b f0                	mov    esi,eax
1003d82f:	8d 7d e4             	lea    edi,[ebp-0x1c]
1003d832:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d833:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d834:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d835:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d836:	8d b5 b4 fe ff ff    	lea    esi,[ebp-0x14c]
1003d83c:	8d bd 5c ff ff ff    	lea    edi,[ebp-0xa4]
1003d842:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d843:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d844:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d845:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d846:	8b f3                	mov    esi,ebx
1003d848:	8d bd 54 fe ff ff    	lea    edi,[ebp-0x1ac]
1003d84e:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d84f:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d850:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d851:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d852:	8d 75 e4             	lea    esi,[ebp-0x1c]
1003d855:	8d 7d 9c             	lea    edi,[ebp-0x64]
1003d858:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d859:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d85a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d85b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d85c:	33 c0                	xor    eax,eax
1003d85e:	0f b6 8c 05 5c ff ff 	movzx  ecx,BYTE PTR [ebp+eax*1-0xa4]
1003d865:	ff 
1003d866:	8b d1                	mov    edx,ecx
1003d868:	c1 ea 04             	shr    edx,0x4
1003d86b:	81 e1 0f 00 00 80    	and    ecx,0x8000000f
1003d871:	79 05                	jns    0x1003d878
1003d873:	49                   	dec    ecx
1003d874:	83 c9 f0             	or     ecx,0xfffffff0
1003d877:	41                   	inc    ecx
1003d878:	8b 54 95 d4          	mov    edx,DWORD PTR [ebp+edx*4-0x2c]
1003d87c:	83 f1 03             	xor    ecx,0x3
1003d87f:	8a 0c 11             	mov    cl,BYTE PTR [ecx+edx*1]
1003d882:	88 8c 05 6c ff ff ff 	mov    BYTE PTR [ebp+eax*1-0x94],cl
1003d889:	40                   	inc    eax
1003d88a:	83 f8 10             	cmp    eax,0x10
1003d88d:	72 cf                	jb     0x1003d85e
1003d88f:	83 ec 10             	sub    esp,0x10
1003d892:	8b fc                	mov    edi,esp
1003d894:	8d b5 94 fe ff ff    	lea    esi,[ebp-0x16c]
1003d89a:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d89b:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d89c:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d89d:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d89e:	83 ec 10             	sub    esp,0x10
1003d8a1:	8b fc                	mov    edi,esp
1003d8a3:	8d b5 c4 fe ff ff    	lea    esi,[ebp-0x13c]
1003d8a9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8aa:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ab:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ac:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ad:	83 ec 10             	sub    esp,0x10
1003d8b0:	8b fc                	mov    edi,esp
1003d8b2:	8d b5 74 fe ff ff    	lea    esi,[ebp-0x18c]
1003d8b8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8b9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ba:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8bb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8bc:	83 ec 10             	sub    esp,0x10
1003d8bf:	8b fc                	mov    edi,esp
1003d8c1:	8d b5 7c ff ff ff    	lea    esi,[ebp-0x84]
1003d8c7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8c8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8c9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ca:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8cb:	83 ec 10             	sub    esp,0x10
1003d8ce:	8b fc                	mov    edi,esp
1003d8d0:	8d b5 84 fe ff ff    	lea    esi,[ebp-0x17c]
1003d8d6:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8d7:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8d8:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8d9:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8da:	83 ec 10             	sub    esp,0x10
1003d8dd:	8b fc                	mov    edi,esp
1003d8df:	ff 75 1c             	push   DWORD PTR [ebp+0x1c]
1003d8e2:	8d b5 e4 fe ff ff    	lea    esi,[ebp-0x11c]
1003d8e8:	ff 75 10             	push   DWORD PTR [ebp+0x10]
1003d8eb:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ec:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ed:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ee:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ef:	83 ec 10             	sub    esp,0x10
1003d8f2:	8b fc                	mov    edi,esp
1003d8f4:	ff 75 70             	push   DWORD PTR [ebp+0x70]
1003d8f7:	8d b5 6c ff ff ff    	lea    esi,[ebp-0x94]
1003d8fd:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8fe:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d8ff:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d900:	a5                   	movs   DWORD PTR es:[edi],DWORD PTR ds:[esi]
1003d901:	e8 55 ef ff ff       	call   0x1003c85b
1003d906:	8b 45 28             	mov    eax,DWORD PTR [ebp+0x28]
1003d909:	01 45 70             	add    DWORD PTR [ebp+0x70],eax
1003d90c:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1003d90f:	01 45 64             	add    DWORD PTR [ebp+0x64],eax
1003d912:	ff 4d 60             	dec    DWORD PTR [ebp+0x60]
1003d915:	0f 85 e7 fd ff ff    	jne    0x1003d702
1003d91b:	ff 75 c4             	push   DWORD PTR [ebp-0x3c]
1003d91e:	8b 75 50             	mov    esi,DWORD PTR [ebp+0x50]
1003d921:	ff 75 3c             	push   DWORD PTR [ebp+0x3c]
1003d924:	ff 75 44             	push   DWORD PTR [ebp+0x44]
1003d927:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d92a:	56                   	push   esi
1003d92b:	e8 5c 15 00 00       	call   0x1003ee8c
1003d930:	03 75 54             	add    esi,DWORD PTR [ebp+0x54]
1003d933:	8b 45 6c             	mov    eax,DWORD PTR [ebp+0x6c]
1003d936:	01 45 58             	add    DWORD PTR [ebp+0x58],eax
1003d939:	ff 4d 78             	dec    DWORD PTR [ebp+0x78]
1003d93c:	89 75 50             	mov    DWORD PTR [ebp+0x50],esi
1003d93f:	0f 85 48 fd ff ff    	jne    0x1003d68d
1003d945:	ff 75 5c             	push   DWORD PTR [ebp+0x5c]
1003d948:	e8 3d 47 fd ff       	call   0x1001208a
1003d94d:	ff 75 68             	push   DWORD PTR [ebp+0x68]
1003d950:	e8 35 47 fd ff       	call   0x1001208a
1003d955:	e9 96 f5 ff ff       	jmp    0x1003cef0
1003d95a:	b8 01 40 00 80       	mov    eax,0x80004001
1003d95f:	5f                   	pop    edi
1003d960:	5e                   	pop    esi
1003d961:	5b                   	pop    ebx
1003d962:	83 c5 48             	add    ebp,0x48
1003d965:	c9                   	leave
1003d966:	c2 30 00             	ret    0x30
