
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003cdc4 <.text+0x3bdc4>:
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
