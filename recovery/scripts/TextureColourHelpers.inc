; Owned Cdecl runtime helpers. No CRT startup, imports, or original DLL calls.
Q_1004E746:
_memcpy LABEL NEAR
    push edi
    push esi
    mov edi,[esp+12]
    mov esi,[esp+16]
    mov ecx,[esp+20]
    mov eax,edi
    cld
    rep movsb
    pop esi
    pop edi
    ret
Q_1004E80A:
_memset LABEL NEAR
    push edi
    mov edi,[esp+8]
    mov eax,[esp+12]
    mov ecx,[esp+16]
    cld
    rep stosb
    mov eax,[esp+8]
    pop edi
    ret
Q_1004E81C:
    sub esp,16
    fnstcw word ptr [esp]
    mov ax,word ptr [esp]
    and ax,0F3FFh
    or ax,0400h
    mov word ptr [esp+2],ax
    fldcw word ptr [esp+2]
    fld qword ptr [esp+20]
    frndint
    fldcw word ptr [esp]
    add esp,16
    ret
ColorCeil:
    sub esp,16
    fnstcw word ptr [esp]
    mov ax,word ptr [esp]
    and ax,0F3FFh
    or ax,0800h
    mov word ptr [esp+2],ax
    fldcw word ptr [esp+2]
    fld qword ptr [esp+20]
    frndint
    fldcw word ptr [esp]
    add esp,16
    ret
Q_1004ED94:
    fsqrt
    ret
; Win32 SSE2 integer conversion, retaining EDX/ECX as in the original fast helper.
Q_1004E830:
    push ebp
    mov ebp,esp
    sub esp,8
    and esp,0FFFFFFF8h
    fstp qword ptr [esp]
    cvttsd2si eax,qword ptr [esp]
    leave
    ret
; The safe exported bridge rejects all non-71 quality requests before dispatch.
; Reaching this target means a violated internal invariant, never a fake result.
Q_100833F0:
    ud2
