
/workspace/NewSdk/SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1000ce87 <.text+0xbe87>:
1000ce87:	55                   	push   ebp
1000ce88:	8b ec                	mov    ebp,esp
1000ce8a:	83 ec 34             	sub    esp,0x34
1000ce8d:	53                   	push   ebx
1000ce8e:	56                   	push   esi
1000ce8f:	57                   	push   edi
1000ce90:	8b 7d 08             	mov    edi,DWORD PTR [ebp+0x8]
1000ce93:	8b 5f 20             	mov    ebx,DWORD PTR [edi+0x20]
1000ce96:	83 e3 3f             	and    ebx,0x3f
1000ce99:	0f b6 04 5d 30 82 09 	movzx  eax,BYTE PTR [ebx*2+0x10098230]
1000cea0:	10 
1000cea1:	57                   	push   edi
1000cea2:	89 5d dc             	mov    DWORD PTR [ebp-0x24],ebx
1000cea5:	89 45 e8             	mov    DWORD PTR [ebp-0x18],eax
1000cea8:	e8 db f4 ff ff       	call   0x1000c388
1000cead:	89 45 d8             	mov    DWORD PTR [ebp-0x28],eax
1000ceb0:	83 f8 04             	cmp    eax,0x4
1000ceb3:	8d 45 f0             	lea    eax,[ebp-0x10]
1000ceb6:	50                   	push   eax
1000ceb7:	8d 45 f4             	lea    eax,[ebp-0xc]
1000ceba:	50                   	push   eax
1000cebb:	8d 45 ec             	lea    eax,[ebp-0x14]
1000cebe:	50                   	push   eax
1000cebf:	57                   	push   edi
1000cec0:	75 57                	jne    0x1000cf19
1000cec2:	e8 61 f7 ff ff       	call   0x1000c628
1000cec7:	8b 45 ec             	mov    eax,DWORD PTR [ebp-0x14]
1000ceca:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000cecd:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1000ced0:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000ced3:	8b 45 f0             	mov    eax,DWORD PTR [ebp-0x10]
1000ced6:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000ced9:	8b 47 1c             	mov    eax,DWORD PTR [edi+0x1c]
1000cedc:	8b f0                	mov    esi,eax
1000cede:	c1 e8 1f             	shr    eax,0x1f
1000cee1:	50                   	push   eax
1000cee2:	8b 47 30             	mov    eax,DWORD PTR [edi+0x30]
1000cee5:	c1 e8 09             	shr    eax,0x9
1000cee8:	c1 ee 16             	shr    esi,0x16
1000ceeb:	83 e0 03             	and    eax,0x3
1000ceee:	50                   	push   eax
1000ceef:	81 e6 ff 01 00 00    	and    esi,0x1ff
1000cef5:	0f af 75 e8          	imul   esi,DWORD PTR [ebp-0x18]
1000cef9:	53                   	push   ebx
1000cefa:	ff 75 e8             	push   DWORD PTR [ebp-0x18]
1000cefd:	8d 45 f8             	lea    eax,[ebp-0x8]
1000cf00:	50                   	push   eax
1000cf01:	8d 45 08             	lea    eax,[ebp+0x8]
1000cf04:	50                   	push   eax
1000cf05:	8d 45 fc             	lea    eax,[ebp-0x4]
1000cf08:	c1 e6 05             	shl    esi,0x5
1000cf0b:	50                   	push   eax
1000cf0c:	c1 ee 03             	shr    esi,0x3
1000cf0f:	e8 09 fa ff ff       	call   0x1000c91d
1000cf14:	e9 0b 02 00 00       	jmp    0x1000d124
1000cf19:	8b 5f 28             	mov    ebx,DWORD PTR [edi+0x28]
1000cf1c:	c1 eb 1f             	shr    ebx,0x1f
1000cf1f:	8d 34 1b             	lea    esi,[ebx+ebx*1]
1000cf22:	e8 01 f7 ff ff       	call   0x1000c628
1000cf27:	8b 45 ec             	mov    eax,DWORD PTR [ebp-0x14]
1000cf2a:	2b c6                	sub    eax,esi
1000cf2c:	89 45 e4             	mov    DWORD PTR [ebp-0x1c],eax
1000cf2f:	48                   	dec    eax
1000cf30:	50                   	push   eax
1000cf31:	e8 78 f8 ff ff       	call   0x1000c7ae
1000cf36:	8b cb                	mov    ecx,ebx
1000cf38:	2b c8                	sub    ecx,eax
1000cf3a:	33 c0                	xor    eax,eax
1000cf3c:	40                   	inc    eax
1000cf3d:	83 c1 20             	add    ecx,0x20
1000cf40:	d3 e0                	shl    eax,cl
1000cf42:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000cf45:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1000cf48:	2b c6                	sub    eax,esi
1000cf4a:	89 45 d4             	mov    DWORD PTR [ebp-0x2c],eax
1000cf4d:	48                   	dec    eax
1000cf4e:	50                   	push   eax
1000cf4f:	e8 5a f8 ff ff       	call   0x1000c7ae
1000cf54:	8b cb                	mov    ecx,ebx
1000cf56:	2b c8                	sub    ecx,eax
1000cf58:	33 c0                	xor    eax,eax
1000cf5a:	83 c1 20             	add    ecx,0x20
1000cf5d:	40                   	inc    eax
1000cf5e:	8b d0                	mov    edx,eax
1000cf60:	d3 e2                	shl    edx,cl
1000cf62:	8b 4f 30             	mov    ecx,DWORD PTR [edi+0x30]
1000cf65:	c1 e9 09             	shr    ecx,0x9
1000cf68:	83 e1 03             	and    ecx,0x3
1000cf6b:	83 f9 02             	cmp    ecx,0x2
1000cf6e:	89 55 08             	mov    DWORD PTR [ebp+0x8],edx
1000cf71:	89 4d e0             	mov    DWORD PTR [ebp-0x20],ecx
1000cf74:	75 26                	jne    0x1000cf9c
1000cf76:	8b 45 f0             	mov    eax,DWORD PTR [ebp-0x10]
1000cf79:	2b c6                	sub    eax,esi
1000cf7b:	48                   	dec    eax
1000cf7c:	50                   	push   eax
1000cf7d:	e8 2c f8 ff ff       	call   0x1000c7ae
1000cf82:	8b 55 08             	mov    edx,DWORD PTR [ebp+0x8]
1000cf85:	8b cb                	mov    ecx,ebx
1000cf87:	2b c8                	sub    ecx,eax
1000cf89:	33 c0                	xor    eax,eax
1000cf8b:	83 c1 20             	add    ecx,0x20
1000cf8e:	40                   	inc    eax
1000cf8f:	d3 e0                	shl    eax,cl
1000cf91:	8b 4d e0             	mov    ecx,DWORD PTR [ebp-0x20]
1000cf94:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000cf97:	33 c0                	xor    eax,eax
1000cf99:	40                   	inc    eax
1000cf9a:	eb 03                	jmp    0x1000cf9f
1000cf9c:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000cf9f:	83 7d 0c 00          	cmp    DWORD PTR [ebp+0xc],0x0
1000cfa3:	0f 85 b0 00 00 00    	jne    0x1000d059
1000cfa9:	83 7d fc 10          	cmp    DWORD PTR [ebp-0x4],0x10
1000cfad:	76 05                	jbe    0x1000cfb4
1000cfaf:	83 fa 10             	cmp    edx,0x10
1000cfb2:	77 08                	ja     0x1000cfbc
1000cfb4:	66 f7 47 30 00 08    	test   WORD PTR [edi+0x30],0x800
1000cfba:	75 12                	jne    0x1000cfce
1000cfbc:	8b 45 ec             	mov    eax,DWORD PTR [ebp-0x14]
1000cfbf:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000cfc2:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1000cfc5:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000cfc8:	8b 45 f0             	mov    eax,DWORD PTR [ebp-0x10]
1000cfcb:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000cfce:	8b 77 1c             	mov    esi,DWORD PTR [edi+0x1c]
1000cfd1:	89 75 e4             	mov    DWORD PTR [ebp-0x1c],esi
1000cfd4:	c1 6d e4 1f          	shr    DWORD PTR [ebp-0x1c],0x1f
1000cfd8:	ff 75 e4             	push   DWORD PTR [ebp-0x1c]
1000cfdb:	8d 45 f8             	lea    eax,[ebp-0x8]
1000cfde:	51                   	push   ecx
1000cfdf:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1000cfe2:	ff 75 e8             	push   DWORD PTR [ebp-0x18]
1000cfe5:	50                   	push   eax
1000cfe6:	8d 45 08             	lea    eax,[ebp+0x8]
1000cfe9:	50                   	push   eax
1000cfea:	8d 45 fc             	lea    eax,[ebp-0x4]
1000cfed:	50                   	push   eax
1000cfee:	e8 2a f9 ff ff       	call   0x1000c91d
1000cff3:	83 7d e4 00          	cmp    DWORD PTR [ebp-0x1c],0x0
1000cff7:	75 45                	jne    0x1000d03e
1000cff9:	8b 47 20             	mov    eax,DWORD PTR [edi+0x20]
1000cffc:	66 a9 00 04          	test   ax,0x400
1000d000:	75 3c                	jne    0x1000d03e
1000d002:	66 f7 47 30 00 08    	test   WORD PTR [edi+0x30],0x800
1000d008:	75 34                	jne    0x1000d03e
1000d00a:	83 7d e0 01          	cmp    DWORD PTR [ebp-0x20],0x1
1000d00e:	75 2e                	jne    0x1000d03e
1000d010:	66 f7 47 2c c0 03    	test   WORD PTR [edi+0x2c],0x3c0
1000d016:	75 26                	jne    0x1000d03e
1000d018:	85 db                	test   ebx,ebx
1000d01a:	75 22                	jne    0x1000d03e
1000d01c:	8d 4d 08             	lea    ecx,[ebp+0x8]
1000d01f:	51                   	push   ecx
1000d020:	8d 4d cc             	lea    ecx,[ebp-0x34]
1000d023:	51                   	push   ecx
1000d024:	83 e0 3f             	and    eax,0x3f
1000d027:	50                   	push   eax
1000d028:	e8 97 f7 ff ff       	call   0x1000c7c4
1000d02d:	8b 4d 08             	mov    ecx,DWORD PTR [ebp+0x8]
1000d030:	8b 45 f4             	mov    eax,DWORD PTR [ebp-0xc]
1000d033:	8d 44 01 ff          	lea    eax,[ecx+eax*1-0x1]
1000d037:	49                   	dec    ecx
1000d038:	f7 d1                	not    ecx
1000d03a:	23 c1                	and    eax,ecx
1000d03c:	eb 03                	jmp    0x1000d041
1000d03e:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d041:	c1 ee 16             	shr    esi,0x16
1000d044:	81 e6 ff 01 00 00    	and    esi,0x1ff
1000d04a:	0f af 75 e8          	imul   esi,DWORD PTR [ebp-0x18]
1000d04e:	c1 e6 05             	shl    esi,0x5
1000d051:	c1 ee 03             	shr    esi,0x3
1000d054:	e9 ce 00 00 00       	jmp    0x1000d127
1000d059:	8b 4d ec             	mov    ecx,DWORD PTR [ebp-0x14]
1000d05c:	8b 55 e4             	mov    edx,DWORD PTR [ebp-0x1c]
1000d05f:	89 4d cc             	mov    DWORD PTR [ebp-0x34],ecx
1000d062:	8b 4d f4             	mov    ecx,DWORD PTR [ebp-0xc]
1000d065:	89 4d d0             	mov    DWORD PTR [ebp-0x30],ecx
1000d068:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d06b:	d3 ea                	shr    edx,cl
1000d06d:	3b d0                	cmp    edx,eax
1000d06f:	77 02                	ja     0x1000d073
1000d071:	8b d0                	mov    edx,eax
1000d073:	03 d6                	add    edx,esi
1000d075:	89 55 ec             	mov    DWORD PTR [ebp-0x14],edx
1000d078:	8b 55 d4             	mov    edx,DWORD PTR [ebp-0x2c]
1000d07b:	d3 ea                	shr    edx,cl
1000d07d:	3b d0                	cmp    edx,eax
1000d07f:	77 02                	ja     0x1000d083
1000d081:	8b d0                	mov    edx,eax
1000d083:	03 d6                	add    edx,esi
1000d085:	83 7d e0 02          	cmp    DWORD PTR [ebp-0x20],0x2
1000d089:	89 55 f4             	mov    DWORD PTR [ebp-0xc],edx
1000d08c:	75 19                	jne    0x1000d0a7
1000d08e:	8b 4d f0             	mov    ecx,DWORD PTR [ebp-0x10]
1000d091:	2b ce                	sub    ecx,esi
1000d093:	8b d1                	mov    edx,ecx
1000d095:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d098:	d3 ea                	shr    edx,cl
1000d09a:	3b d0                	cmp    edx,eax
1000d09c:	77 02                	ja     0x1000d0a0
1000d09e:	8b d0                	mov    edx,eax
1000d0a0:	03 d6                	add    edx,esi
1000d0a2:	89 55 f0             	mov    DWORD PTR [ebp-0x10],edx
1000d0a5:	eb 03                	jmp    0x1000d0aa
1000d0a7:	89 45 f0             	mov    DWORD PTR [ebp-0x10],eax
1000d0aa:	66 f7 47 30 00 08    	test   WORD PTR [edi+0x30],0x800
1000d0b0:	74 17                	je     0x1000d0c9
1000d0b2:	53                   	push   ebx
1000d0b3:	ff 75 d0             	push   DWORD PTR [ebp-0x30]
1000d0b6:	ff 75 cc             	push   DWORD PTR [ebp-0x34]
1000d0b9:	e8 1a f8 ff ff       	call   0x1000c8d8
1000d0be:	39 45 0c             	cmp    DWORD PTR [ebp+0xc],eax
1000d0c1:	72 03                	jb     0x1000d0c6
1000d0c3:	89 45 0c             	mov    DWORD PTR [ebp+0xc],eax
1000d0c6:	33 c0                	xor    eax,eax
1000d0c8:	40                   	inc    eax
1000d0c9:	8b 55 fc             	mov    edx,DWORD PTR [ebp-0x4]
1000d0cc:	8b 4d 0c             	mov    ecx,DWORD PTR [ebp+0xc]
1000d0cf:	d3 ea                	shr    edx,cl
1000d0d1:	3b d0                	cmp    edx,eax
1000d0d3:	89 55 fc             	mov    DWORD PTR [ebp-0x4],edx
1000d0d6:	77 03                	ja     0x1000d0db
1000d0d8:	89 45 fc             	mov    DWORD PTR [ebp-0x4],eax
1000d0db:	8b 55 08             	mov    edx,DWORD PTR [ebp+0x8]
1000d0de:	d3 ea                	shr    edx,cl
1000d0e0:	3b d0                	cmp    edx,eax
1000d0e2:	89 55 08             	mov    DWORD PTR [ebp+0x8],edx
1000d0e5:	77 03                	ja     0x1000d0ea
1000d0e7:	89 45 08             	mov    DWORD PTR [ebp+0x8],eax
1000d0ea:	8b 55 f8             	mov    edx,DWORD PTR [ebp-0x8]
1000d0ed:	d3 ea                	shr    edx,cl
1000d0ef:	3b d0                	cmp    edx,eax
1000d0f1:	89 55 f8             	mov    DWORD PTR [ebp-0x8],edx
1000d0f4:	77 03                	ja     0x1000d0f9
1000d0f6:	89 45 f8             	mov    DWORD PTR [ebp-0x8],eax
1000d0f9:	8b 47 1c             	mov    eax,DWORD PTR [edi+0x1c]
1000d0fc:	c1 e8 1f             	shr    eax,0x1f
1000d0ff:	50                   	push   eax
1000d100:	ff 75 e0             	push   DWORD PTR [ebp-0x20]
1000d103:	8d 45 f8             	lea    eax,[ebp-0x8]
1000d106:	ff 75 dc             	push   DWORD PTR [ebp-0x24]
1000d109:	ff 75 e8             	push   DWORD PTR [ebp-0x18]
1000d10c:	50                   	push   eax
1000d10d:	8d 45 08             	lea    eax,[ebp+0x8]
1000d110:	50                   	push   eax
1000d111:	8d 45 fc             	lea    eax,[ebp-0x4]
1000d114:	50                   	push   eax
1000d115:	e8 03 f8 ff ff       	call   0x1000c91d
1000d11a:	8b 75 fc             	mov    esi,DWORD PTR [ebp-0x4]
1000d11d:	0f af 75 e8          	imul   esi,DWORD PTR [ebp-0x18]
1000d121:	c1 ee 03             	shr    esi,0x3
1000d124:	8b 45 08             	mov    eax,DWORD PTR [ebp+0x8]
1000d127:	8b 4d 10             	mov    ecx,DWORD PTR [ebp+0x10]
1000d12a:	8b 55 ec             	mov    edx,DWORD PTR [ebp-0x14]
1000d12d:	89 11                	mov    DWORD PTR [ecx],edx
1000d12f:	8b 4d 14             	mov    ecx,DWORD PTR [ebp+0x14]
1000d132:	8b 55 f4             	mov    edx,DWORD PTR [ebp-0xc]
1000d135:	89 11                	mov    DWORD PTR [ecx],edx
1000d137:	8b 4d 18             	mov    ecx,DWORD PTR [ebp+0x18]
1000d13a:	8b 55 f0             	mov    edx,DWORD PTR [ebp-0x10]
1000d13d:	89 11                	mov    DWORD PTR [ecx],edx
1000d13f:	8b 4d 1c             	mov    ecx,DWORD PTR [ebp+0x1c]
1000d142:	89 31                	mov    DWORD PTR [ecx],esi
1000d144:	0f af f0             	imul   esi,eax
1000d147:	8b 45 20             	mov    eax,DWORD PTR [ebp+0x20]
1000d14a:	89 30                	mov    DWORD PTR [eax],esi
1000d14c:	f7 47 1c 00 00 00 80 	test   DWORD PTR [edi+0x1c],0x80000000
1000d153:	75 12                	jne    0x1000d167
1000d155:	83 7d 0c 01          	cmp    DWORD PTR [ebp+0xc],0x1
1000d159:	77 0c                	ja     0x1000d167
1000d15b:	83 7d d8 03          	cmp    DWORD PTR [ebp-0x28],0x3
1000d15f:	74 25                	je     0x1000d186
1000d161:	83 7d d8 14          	cmp    DWORD PTR [ebp-0x28],0x14
1000d165:	74 1f                	je     0x1000d186
1000d167:	8b 4f 30             	mov    ecx,DWORD PTR [edi+0x30]
1000d16a:	81 e1 00 06 00 00    	and    ecx,0x600
1000d170:	81 f9 00 04 00 00    	cmp    ecx,0x400
1000d176:	74 0e                	je     0x1000d186
1000d178:	81 c6 ff 0f 00 00    	add    esi,0xfff
1000d17e:	81 e6 00 f0 ff ff    	and    esi,0xfffff000
1000d184:	89 30                	mov    DWORD PTR [eax],esi
1000d186:	5f                   	pop    edi
1000d187:	5e                   	pop    esi
1000d188:	5b                   	pop    ebx
1000d189:	c9                   	leave
1000d18a:	c2 1c 00             	ret    0x1c
