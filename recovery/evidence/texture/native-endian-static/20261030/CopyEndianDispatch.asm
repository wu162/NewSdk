
SDK/tools/BinaryAssetBuilder.TextureCompiler.dll:     file format pei-i386


Disassembly of section .text:

1003cef7 <.text+0x3bef7>:
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
