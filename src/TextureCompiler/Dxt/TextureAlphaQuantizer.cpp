// Win32 x87 quantizer compatibility source, original RVA 0x7D500.
// Reconstructed instruction order is intentional: algebraically equivalent
// managed expressions can choose different endpoints on strict error ties.
// All constants and helper bodies below belong to this implementation.
// No original DLL or original function address is called.
#include <cstring>
static const double kHalf=0.5, kOne=1.0, kByteScale=255.0, kMinusOne=-1.0;

// Cdecl floor(double): retain the caller's precision and exception masks.
__declspec(naked) static void AlphaFloor() {
    __asm {
        sub esp,16
        fnstcw word ptr [esp]
        mov ax,word ptr [esp]
        and ax,0xf3ff
        or ax,0x0400
        mov word ptr [esp+2],ax
        fldcw word ptr [esp+2]
        fld qword ptr [esp+20]
        frndint
        fldcw word ptr [esp]
        add esp,16
        ret
    }
}
// Convert ST(0) to signed int64 in EDX:EAX, matching the original ftol helper.
__declspec(naked) static void AlphaTruncate() {
    __asm {
        sub esp,16
        fnstcw word ptr [esp]
        mov ax,word ptr [esp]
        or ax,0x0c00
        mov word ptr [esp+2],ax
        fldcw word ptr [esp+2]
        fistp qword ptr [esp+8]
        fldcw word ptr [esp]
        mov eax,dword ptr [esp+8]
        mov edx,dword ptr [esp+12]
        add esp,16
        ret
    }
}
#pragma comment(linker,"/EXPORT:TextureQuantizeAlpha=_TextureQuantizeAlpha@20")
extern "C" __declspec(naked) double __stdcall TextureQuantizeAlpha(
    int levels,int count,const double* values,int* endpoints,int* indices) {
    __asm {
Q_1007d500:
        push   ebp
Q_1007d501:
        mov    ebp,esp
Q_1007d503:
        and    esp,0xffffffc0
Q_1007d506:
        mov    eax,0x11b4
Q_1007d50b:
        // Probe both pages of the fixed-size aligned local frame.
        sub esp,0x1000
        test dword ptr [esp],eax
        sub esp,0x1b4
        test dword ptr [esp],eax
Q_1007d510:
        push   ebx
Q_1007d511:
        push   esi
Q_1007d512:
        push   edi
Q_1007d513:
        push   0x38
Q_1007d515:
        lea    eax,[esp+0x18c]
Q_1007d51c:
        push   0x0
Q_1007d51e:
        push   eax
Q_1007d51f:
        call memset
Q_1007d524:
        push   0x1000
Q_1007d529:
        lea    ecx,[esp+0x1d0]
Q_1007d530:
        push   0x0
Q_1007d532:
        push   ecx
Q_1007d533:
        call memset
Q_1007d538:
        xor    eax,eax
Q_1007d53a:
        push   0x40
Q_1007d53c:
        push   eax
Q_1007d53d:
        lea    edx,[esp+0x138]
Q_1007d544:
        push   edx
Q_1007d545:
        mov    DWORD PTR [esp+0xdc],eax
Q_1007d54c:
        mov    DWORD PTR [esp+0xe0],eax
Q_1007d553:
        mov    DWORD PTR [esp+0xe4],eax
Q_1007d55a:
        mov    DWORD PTR [esp+0xe8],eax
Q_1007d561:
        mov    DWORD PTR [esp+0x11c],eax
Q_1007d568:
        mov    DWORD PTR [esp+0x120],eax
Q_1007d56f:
        mov    DWORD PTR [esp+0x124],eax
Q_1007d576:
        mov    DWORD PTR [esp+0x128],eax
Q_1007d57d:
        mov    DWORD PTR [esp+0x12c],eax
Q_1007d584:
        mov    DWORD PTR [esp+0x130],eax
Q_1007d58b:
        mov    DWORD PTR [esp+0x134],eax
Q_1007d592:
        mov    DWORD PTR [esp+0x138],eax
Q_1007d599:
        call memset
Q_1007d59e:
        xor    eax,eax
Q_1007d5a0:
        mov    DWORD PTR [esp+0x9c],eax
Q_1007d5a7:
        mov    DWORD PTR [esp+0xa0],eax
Q_1007d5ae:
        mov    DWORD PTR [esp+0xa4],eax
Q_1007d5b5:
        mov    DWORD PTR [esp+0xa8],eax
Q_1007d5bc:
        mov    DWORD PTR [esp+0xac],eax
Q_1007d5c3:
        mov    DWORD PTR [esp+0xb0],eax
Q_1007d5ca:
        mov    DWORD PTR [esp+0xb4],eax
Q_1007d5d1:
        mov    DWORD PTR [esp+0xb8],eax
Q_1007d5d8:
        mov    eax,DWORD PTR [ebp+0xc]
Q_1007d5db:
        add    esp,0x24
Q_1007d5de:
        test   eax,eax
Q_1007d5e0:
        mov    DWORD PTR [esp+0x4c],0x40
Q_1007d5e8:
        jne Q_1007d5f5
Q_1007d5ea:
        fldz
Q_1007d5ec:
        pop    edi
Q_1007d5ed:
        pop    esi
Q_1007d5ee:
        pop    ebx
Q_1007d5ef:
        mov    esp,ebp
Q_1007d5f1:
        pop    ebp
Q_1007d5f2:
        ret    0x14
Q_1007d5f5:
        fild   DWORD PTR [ebp+0xc]
Q_1007d5f8:
        mov    edi,DWORD PTR [ebp+0x10]
Q_1007d5fb:
        mov    esi,DWORD PTR [ebp+0x18]
Q_1007d5fe:
        add    eax,0xffffffff
Q_1007d601:
        cmp    eax,0x4
Q_1007d604:
        fst    QWORD PTR [esp+0xd0]
Q_1007d60b:
        fld1
Q_1007d60d:
        mov    edx,0x1
Q_1007d612:
        fdiv   st(0),st(1)
Q_1007d614:
        fst    QWORD PTR [esp+0xf0]
Q_1007d61b:
        fld    QWORD PTR [edi]
Q_1007d61d:
        fld    st(0)
Q_1007d61f:
        fld    st(1)
Q_1007d621:
        mov    DWORD PTR [esi],0x0
Q_1007d627:
        jl Q_1007d705
Q_1007d62d:
        mov    ebx,DWORD PTR [ebp+0xc]
Q_1007d630:
        add    ebx,0xfffffffb
Q_1007d633:
        shr    ebx,0x2
Q_1007d636:
        add    ebx,edx
Q_1007d638:
        lea    ecx,[edi+0x18]
Q_1007d63b:
        lea    edx,[ebx*4+0x1]
Q_1007d642:
        fld    QWORD PTR [ecx-0x10]
Q_1007d645:
        fcomp  st(2)
Q_1007d647:
        fnstsw ax
Q_1007d649:
        test   ah,0x5
Q_1007d64c:
        jp Q_1007d655
Q_1007d64e:
        fstp   st(1)
Q_1007d650:
        fld    QWORD PTR [ecx-0x10]
Q_1007d653:
        fxch   st(1)
Q_1007d655:
        fld    QWORD PTR [ecx-0x10]
Q_1007d658:
        fcomp  st(3)
Q_1007d65a:
        fnstsw ax
Q_1007d65c:
        test   ah,0x41
Q_1007d65f:
        jne Q_1007d668
Q_1007d661:
        fstp   st(2)
Q_1007d663:
        fld    QWORD PTR [ecx-0x10]
Q_1007d666:
        fxch   st(2)
Q_1007d668:
        fadd   QWORD PTR [ecx-0x10]
Q_1007d66b:
        mov    DWORD PTR [esi],0x0
Q_1007d671:
        fld    QWORD PTR [ecx-0x8]
Q_1007d674:
        fcomp  st(2)
Q_1007d676:
        fnstsw ax
Q_1007d678:
        test   ah,0x5
Q_1007d67b:
        jp Q_1007d684
Q_1007d67d:
        fstp   st(1)
Q_1007d67f:
        fld    QWORD PTR [ecx-0x8]
Q_1007d682:
        fxch   st(1)
Q_1007d684:
        fld    QWORD PTR [ecx-0x8]
Q_1007d687:
        fcomp  st(3)
Q_1007d689:
        fnstsw ax
Q_1007d68b:
        test   ah,0x41
Q_1007d68e:
        jne Q_1007d697
Q_1007d690:
        fstp   st(2)
Q_1007d692:
        fld    QWORD PTR [ecx-0x8]
Q_1007d695:
        fxch   st(2)
Q_1007d697:
        fadd   QWORD PTR [ecx-0x8]
Q_1007d69a:
        mov    DWORD PTR [esi],0x0
Q_1007d6a0:
        fld    QWORD PTR [ecx]
Q_1007d6a2:
        fcomp  st(2)
Q_1007d6a4:
        fnstsw ax
Q_1007d6a6:
        test   ah,0x5
Q_1007d6a9:
        jp Q_1007d6b1
Q_1007d6ab:
        fstp   st(1)
Q_1007d6ad:
        fld    QWORD PTR [ecx]
Q_1007d6af:
        fxch   st(1)
Q_1007d6b1:
        fld    QWORD PTR [ecx]
Q_1007d6b3:
        fcomp  st(3)
Q_1007d6b5:
        fnstsw ax
Q_1007d6b7:
        test   ah,0x41
Q_1007d6ba:
        jne Q_1007d6c2
Q_1007d6bc:
        fstp   st(2)
Q_1007d6be:
        fld    QWORD PTR [ecx]
Q_1007d6c0:
        fxch   st(2)
Q_1007d6c2:
        fadd   QWORD PTR [ecx]
Q_1007d6c4:
        mov    DWORD PTR [esi],0x0
Q_1007d6ca:
        fld    QWORD PTR [ecx+0x8]
Q_1007d6cd:
        fcomp  st(2)
Q_1007d6cf:
        fnstsw ax
Q_1007d6d1:
        test   ah,0x5
Q_1007d6d4:
        jp Q_1007d6dd
Q_1007d6d6:
        fstp   st(1)
Q_1007d6d8:
        fld    QWORD PTR [ecx+0x8]
Q_1007d6db:
        fxch   st(1)
Q_1007d6dd:
        fld    QWORD PTR [ecx+0x8]
Q_1007d6e0:
        fcomp  st(3)
Q_1007d6e2:
        fnstsw ax
Q_1007d6e4:
        test   ah,0x41
Q_1007d6e7:
        jne Q_1007d6f0
Q_1007d6e9:
        fstp   st(2)
Q_1007d6eb:
        fld    QWORD PTR [ecx+0x8]
Q_1007d6ee:
        fxch   st(2)
Q_1007d6f0:
        fadd   QWORD PTR [ecx+0x8]
Q_1007d6f3:
        add    ecx,0x20
Q_1007d6f6:
        sub    ebx,0x1
Q_1007d6f9:
        mov    DWORD PTR [esi],0x0
Q_1007d6ff:
        jne Q_1007d642
Q_1007d705:
        cmp    edx,DWORD PTR [ebp+0xc]
Q_1007d708:
        jge Q_1007d743
Q_1007d70a:
        mov    ecx,DWORD PTR [ebp+0xc]
Q_1007d70d:
        fld    QWORD PTR [edi+edx*8]
Q_1007d710:
        fcomp  st(2)
Q_1007d712:
        fnstsw ax
Q_1007d714:
        test   ah,0x5
Q_1007d717:
        jp Q_1007d720
Q_1007d719:
        fstp   st(1)
Q_1007d71b:
        fld    QWORD PTR [edi+edx*8]
Q_1007d71e:
        fxch   st(1)
Q_1007d720:
        fld    QWORD PTR [edi+edx*8]
Q_1007d723:
        fcomp  st(3)
Q_1007d725:
        fnstsw ax
Q_1007d727:
        test   ah,0x41
Q_1007d72a:
        jne Q_1007d733
Q_1007d72c:
        fstp   st(2)
Q_1007d72e:
        fld    QWORD PTR [edi+edx*8]
Q_1007d731:
        fxch   st(2)
Q_1007d733:
        fadd   QWORD PTR [edi+edx*8]
Q_1007d736:
        add    edx,0x1
Q_1007d739:
        cmp    edx,ecx
Q_1007d73b:
        mov    DWORD PTR [esi],0x0
Q_1007d741:
        jl Q_1007d70d
Q_1007d743:
        fmulp  st(3),st(0)
Q_1007d745:
        xor    ecx,ecx
Q_1007d747:
        cmp    DWORD PTR [ebp+0xc],0x4
Q_1007d74b:
        fxch   st(2)
Q_1007d74d:
        fst    QWORD PTR [esp+0xd8]
Q_1007d754:
        jl Q_1007d7a5
Q_1007d756:
        mov    ebx,DWORD PTR [ebp+0xc]
Q_1007d759:
        lea    edx,[esp+0x1c0]
Q_1007d760:
        lea    esi,[esp+0x1c8]
Q_1007d767:
        add    ebx,0xfffffffd
Q_1007d76a:
        sub    edx,edi
Q_1007d76c:
        lea    eax,[edi+0x10]
Q_1007d76f:
        sub    esi,edi
Q_1007d771:
        fld    QWORD PTR [eax-0x10]
Q_1007d774:
        add    ecx,0x4
Q_1007d777:
        fsub   st(0),st(1)
Q_1007d779:
        add    eax,0x20
Q_1007d77c:
        cmp    ecx,ebx
Q_1007d77e:
        fstp   QWORD PTR [esp+ecx*8+0x1a0]
Q_1007d785:
        fld    QWORD PTR [eax-0x28]
Q_1007d788:
        fsub   st(0),st(1)
Q_1007d78a:
        fstp   QWORD PTR [esp+ecx*8+0x1a8]
Q_1007d791:
        fld    QWORD PTR [eax-0x20]
Q_1007d794:
        fsub   st(0),st(1)
Q_1007d796:
        fstp   QWORD PTR [eax+edx*1-0x20]
Q_1007d79a:
        fld    QWORD PTR [eax-0x18]
Q_1007d79d:
        fsub   st(0),st(1)
Q_1007d79f:
        fstp   QWORD PTR [eax+esi*1-0x20]
Q_1007d7a3:
        jl Q_1007d771
Q_1007d7a5:
        mov    eax,DWORD PTR [ebp+0xc]
Q_1007d7a8:
        cmp    ecx,eax
Q_1007d7aa:
        jge Q_1007d7cf
Q_1007d7ac:
        lea    edx,[esp+0x1c0]
Q_1007d7b3:
        sub    edi,edx
Q_1007d7b5:
        lea    edx,[edi+ecx*8]
Q_1007d7b8:
        fld    QWORD PTR [esp+edx*1+0x1c0]
Q_1007d7bf:
        add    ecx,0x1
Q_1007d7c2:
        cmp    ecx,eax
Q_1007d7c4:
        fsub   st(0),st(1)
Q_1007d7c6:
        fstp   QWORD PTR [esp+ecx*8+0x1b8]
Q_1007d7cd:
        jl Q_1007d7b5
Q_1007d7cf:
        mov    eax,DWORD PTR [ebp+0x8]
Q_1007d7d2:
        fsub   st(2),st(0)
Q_1007d7d4:
        fxch   st(2)
Q_1007d7d6:
        lea    ecx,[esp+eax*4+0xf4]
Q_1007d7dd:
        add    eax,0xffffffff
Q_1007d7e0:
        fst    QWORD PTR [esp+0xb8]
Q_1007d7e7:
        mov    DWORD PTR [esp+0x5c],ecx
Q_1007d7eb:
        fxch   st(1)
Q_1007d7ed:
        mov    DWORD PTR [esp+0x2c],eax
Q_1007d7f1:
        fsubrp st(2),st(0)
Q_1007d7f3:
        mov    ebx,eax
Q_1007d7f5:
        fxch   st(1)
Q_1007d7f7:
        fst    QWORD PTR [esp+0xc0]
Q_1007d7fe:
        fldz
Q_1007d800:
        jmp Q_1007d814
Q_1007d802:
        fstp   st(3)
Q_1007d804:
        fstp   st(3)
Q_1007d806:
        fstp   st(0)
Q_1007d808:
        fldz
Q_1007d80a:
        jmp Q_1007d814
Q_1007d80c:
        fxch   st(1)
Q_1007d80e:
        fxch   st(3)
Q_1007d810:
        fxch   st(2)
Q_1007d812:
        fxch   st(1)
Q_1007d814:
        xor    edx,edx
Q_1007d816:
        test   ebx,ebx
Q_1007d818:
        mov    DWORD PTR [esp+0x24],0x1
Q_1007d820:
        mov    DWORD PTR [esp+0x28],edx
Q_1007d824:
        jle Q_1007d8f3
Q_1007d82a:
        fxch   st(1)
Q_1007d82c:
        mov    ecx,ebx
Q_1007d82e:
        fsub   st(0),st(2)
Q_1007d830:
        xor    eax,eax
Q_1007d832:
        lea    edi,[esp+0xf8]
Q_1007d839:
        rep stosd
Q_1007d83b:
        fidiv  DWORD PTR [esp+0x2c]
Q_1007d83f:
        fld    QWORD PTR [kHalf]
Q_1007d845:
        fild   DWORD PTR [esp+0x28]
Q_1007d849:
        add    edx,0x1
Q_1007d84c:
        cmp    edx,ebx
Q_1007d84e:
        mov    DWORD PTR [esp+0x28],edx
Q_1007d852:
        fadd   st(0),st(1)
Q_1007d854:
        fmul   st(0),st(2)
Q_1007d856:
        fadd   st(0),st(4)
Q_1007d858:
        fstp   QWORD PTR [esp+edx*8+0x180]
Q_1007d85f:
        jl Q_1007d845
Q_1007d861:
        fstp   st(3)
Q_1007d863:
        fstp   st(0)
Q_1007d865:
        fstp   st(1)
Q_1007d867:
        mov    edi,DWORD PTR [ebp+0xc]
Q_1007d86a:
        mov    edx,DWORD PTR [esp+0x5c]
Q_1007d86e:
        xor    esi,esi
Q_1007d870:
        test   edi,edi
Q_1007d872:
        mov    DWORD PTR [edx],0x0
Q_1007d878:
        jle Q_1007d937
Q_1007d87e:
        lea    edx,[esp+0x1c0]
Q_1007d885:
        xor    ecx,ecx
Q_1007d887:
        cmp    ebx,0x4
Q_1007d88a:
        jl Q_1007d8d6
Q_1007d88c:
        fld    QWORD PTR [esp+ecx*8+0x188]
Q_1007d893:
        fcomp  QWORD PTR [edx]
Q_1007d895:
        fnstsw ax
Q_1007d897:
        test   ah,0x41
Q_1007d89a:
        je Q_1007d909
Q_1007d89c:
        fld    QWORD PTR [esp+ecx*8+0x190]
Q_1007d8a3:
        fcomp  QWORD PTR [edx]
Q_1007d8a5:
        fnstsw ax
Q_1007d8a7:
        test   ah,0x41
Q_1007d8aa:
        je Q_1007d8fc
Q_1007d8ac:
        fld    QWORD PTR [esp+ecx*8+0x198]
Q_1007d8b3:
        fcomp  QWORD PTR [edx]
Q_1007d8b5:
        fnstsw ax
Q_1007d8b7:
        test   ah,0x41
Q_1007d8ba:
        je Q_1007d901
Q_1007d8bc:
        fld    QWORD PTR [esp+ecx*8+0x1a0]
Q_1007d8c3:
        fcomp  QWORD PTR [edx]
Q_1007d8c5:
        fnstsw ax
Q_1007d8c7:
        test   ah,0x41
Q_1007d8ca:
        je Q_1007d906
Q_1007d8cc:
        add    ecx,0x4
Q_1007d8cf:
        lea    eax,[ebx-0x3]
Q_1007d8d2:
        cmp    ecx,eax
Q_1007d8d4:
        jl Q_1007d88c
Q_1007d8d6:
        cmp    ecx,ebx
Q_1007d8d8:
        jge Q_1007d909
Q_1007d8da:
        fld    QWORD PTR [esp+ecx*8+0x188]
Q_1007d8e1:
        fcomp  QWORD PTR [edx]
Q_1007d8e3:
        fnstsw ax
Q_1007d8e5:
        test   ah,0x41
Q_1007d8e8:
        je Q_1007d909
Q_1007d8ea:
        add    ecx,0x1
Q_1007d8ed:
        cmp    ecx,ebx
Q_1007d8ef:
        jl Q_1007d8da
Q_1007d8f1:
        jmp Q_1007d909
Q_1007d8f3:
        fstp   st(2)
Q_1007d8f5:
        fstp   st(0)
Q_1007d8f7:
        jmp Q_1007d867
Q_1007d8fc:
        add    ecx,0x1
Q_1007d8ff:
        jmp Q_1007d909
Q_1007d901:
        add    ecx,0x2
Q_1007d904:
        jmp Q_1007d909
Q_1007d906:
        add    ecx,0x3
Q_1007d909:
        mov    edi,DWORD PTR [ebp+0x18]
Q_1007d90c:
        add    DWORD PTR [esp+ecx*4+0xf8],0x1
Q_1007d914:
        xor    eax,eax
Q_1007d916:
        cmp    DWORD PTR [edi+esi*4],ecx
Q_1007d919:
        sete   al
Q_1007d91c:
        add    esi,0x1
Q_1007d91f:
        add    edx,0x8
Q_1007d922:
        and    DWORD PTR [esp+0x24],eax
Q_1007d926:
        mov    eax,edi
Q_1007d928:
        mov    edi,DWORD PTR [ebp+0xc]
Q_1007d92b:
        cmp    esi,edi
Q_1007d92d:
        mov    DWORD PTR [eax+esi*4-0x4],ecx
Q_1007d931:
        jl Q_1007d885
Q_1007d937:
        mov    esi,DWORD PTR [ebp+0x8]
Q_1007d93a:
        xor    ecx,ecx
Q_1007d93c:
        xor    eax,eax
Q_1007d93e:
        test   esi,esi
Q_1007d940:
        mov    DWORD PTR [esp+0x28],ecx
Q_1007d944:
        jle Q_1007d96f
Q_1007d946:
        xor    edx,edx
Q_1007d948:
        cmp    DWORD PTR [esp+eax*4+0xf8],edx
Q_1007d94f:
        setne  dl
Q_1007d952:
        add    eax,0x1
Q_1007d955:
        add    ecx,edx
Q_1007d957:
        cmp    eax,esi
Q_1007d959:
        jl Q_1007d946
Q_1007d95b:
        cmp    ecx,0x1
Q_1007d95e:
        mov    DWORD PTR [esp+0x28],ecx
Q_1007d962:
        je Q_1007e1ac
Q_1007d968:
        mov    ebx,DWORD PTR [esp+0x2c]
Q_1007d96c:
        mov    edi,DWORD PTR [ebp+0xc]
Q_1007d96f:
        mov    esi,DWORD PTR [ebp+0x18]
Q_1007d972:
        fld    st(0)
Q_1007d974:
        fld    st(1)
Q_1007d976:
        xor    eax,eax
Q_1007d978:
        cmp    edi,0x4
Q_1007d97b:
        fxch   st(2)
Q_1007d97d:
        fst    QWORD PTR [esp+0x68]
Q_1007d981:
        jl Q_1007da17
Q_1007d987:
        lea    edx,[edi-0x3]
Q_1007d98a:
        lea    ecx,[esi+0x8]
Q_1007d98d:
        jmp Q_1007d993
Q_1007d98f:
        fxch   st(1)
Q_1007d991:
        fxch   st(2)
Q_1007d993:
        fild   DWORD PTR [ecx-0x8]
Q_1007d996:
        add    eax,0x4
Q_1007d999:
        fild   DWORD PTR [ecx-0x4]
Q_1007d99c:
        add    ecx,0x10
Q_1007d99f:
        cmp    eax,edx
Q_1007d9a1:
        fild   DWORD PTR [ecx-0x10]
Q_1007d9a4:
        fst    QWORD PTR [esp+0x68]
Q_1007d9a8:
        fild   DWORD PTR [ecx-0xc]
Q_1007d9ab:
        fstp   QWORD PTR [esp+0x50]
Q_1007d9af:
        fld    QWORD PTR [esp+eax*8+0x1a0]
Q_1007d9b6:
        fmul   st(0),st(3)
Q_1007d9b8:
        faddp  st(4),st(0)
Q_1007d9ba:
        fld    QWORD PTR [esp+eax*8+0x1a8]
Q_1007d9c1:
        fmul   st(0),st(2)
Q_1007d9c3:
        faddp  st(4),st(0)
Q_1007d9c5:
        fmul   QWORD PTR [esp+eax*8+0x1b0]
Q_1007d9cc:
        faddp  st(3),st(0)
Q_1007d9ce:
        fld    QWORD PTR [esp+eax*8+0x1b8]
Q_1007d9d5:
        fld    QWORD PTR [esp+0x50]
Q_1007d9d9:
        fmul   st(1),st(0)
Q_1007d9db:
        fxch   st(4)
Q_1007d9dd:
        faddp  st(1),st(0)
Q_1007d9df:
        fld    st(2)
Q_1007d9e1:
        faddp  st(6),st(0)
Q_1007d9e3:
        fxch   st(5)
Q_1007d9e5:
        fadd   st(0),st(1)
Q_1007d9e7:
        fadd   QWORD PTR [esp+0x68]
Q_1007d9eb:
        fadd   st(0),st(3)
Q_1007d9ed:
        fld    st(2)
Q_1007d9ef:
        fmulp  st(3),st(0)
Q_1007d9f1:
        fxch   st(2)
Q_1007d9f3:
        faddp  st(4),st(0)
Q_1007d9f5:
        fmul   st(0),st(0)
Q_1007d9f7:
        faddp  st(3),st(0)
Q_1007d9f9:
        fld    QWORD PTR [esp+0x68]
Q_1007d9fd:
        fmul   st(0),st(0)
Q_1007d9ff:
        faddp  st(3),st(0)
Q_1007da01:
        fld    st(1)
Q_1007da03:
        fmulp  st(2),st(0)
Q_1007da05:
        fxch   st(2)
Q_1007da07:
        faddp  st(1),st(0)
Q_1007da09:
        jl Q_1007d98f
Q_1007da0b:
        fxch   st(2)
Q_1007da0d:
        fst    QWORD PTR [esp+0x68]
Q_1007da11:
        fxch   st(1)
Q_1007da13:
        fxch   st(2)
Q_1007da15:
        fxch   st(1)
Q_1007da17:
        cmp    eax,edi
Q_1007da19:
        jge Q_1007da3c
Q_1007da1b:
        fild   DWORD PTR [esi+eax*4]
Q_1007da1e:
        add    eax,0x1
Q_1007da21:
        cmp    eax,edi
Q_1007da23:
        fld    QWORD PTR [esp+eax*8+0x1b8]
Q_1007da2a:
        fmul   st(0),st(1)
Q_1007da2c:
        faddp  st(2),st(0)
Q_1007da2e:
        fld    st(0)
Q_1007da30:
        faddp  st(4),st(0)
Q_1007da32:
        fmul   st(0),st(0)
Q_1007da34:
        faddp  st(2),st(0)
Q_1007da36:
        jl Q_1007da1b
Q_1007da38:
        fst    QWORD PTR [esp+0x68]
Q_1007da3c:
        fld    QWORD PTR [esp+0xf0]
Q_1007da43:
        fmulp  st(3),st(0)
Q_1007da45:
        fxch   st(2)
Q_1007da47:
        fst    QWORD PTR [esp+0x178]
Q_1007da4e:
        fld    st(3)
Q_1007da50:
        fmul   st(0),st(1)
Q_1007da52:
        fmul   st(0),st(1)
Q_1007da54:
        fsubp  st(2),st(0)
Q_1007da56:
        fld    st(0)
Q_1007da58:
        fchs
Q_1007da5a:
        fst    QWORD PTR [esp+0x50]
Q_1007da5e:
        fild   DWORD PTR [ebp+0x8]
Q_1007da61:
        fsub   QWORD PTR [kOne]
Q_1007da67:
        fst    QWORD PTR [esp+0x158]
Q_1007da6e:
        fld    st(0)
Q_1007da70:
        fsubrp st(3),st(0)
Q_1007da72:
        fxch   st(2)
Q_1007da74:
        fst    QWORD PTR [esp+0xe8]
Q_1007da7b:
        fld    st(1)
Q_1007da7d:
        fmul   st(0),st(6)
Q_1007da7f:
        fmul   st(2),st(0)
Q_1007da81:
        fxch   st(2)
Q_1007da83:
        fadd   st(0),st(4)
Q_1007da85:
        fst    QWORD PTR [esp+0xa8]
Q_1007da8c:
        fld    st(1)
Q_1007da8e:
        fmulp  st(3),st(0)
Q_1007da90:
        fxch   st(2)
Q_1007da92:
        fadd   st(0),st(4)
Q_1007da94:
        fstp   QWORD PTR [esp+0x98]
Q_1007da9b:
        fld    st(0)
Q_1007da9d:
        fmul   st(0),st(6)
Q_1007da9f:
        fmul   st(0),st(1)
Q_1007daa1:
        fadd   st(0),st(4)
Q_1007daa3:
        fst    QWORD PTR [esp+0xb0]
Q_1007daaa:
        fmulp  st(2),st(0)
Q_1007daac:
        fld    st(1)
Q_1007daae:
        fmul   st(0),st(4)
Q_1007dab0:
        fmul   st(0),st(2)
Q_1007dab2:
        fmul   st(0),st(3)
Q_1007dab4:
        fmul   st(0),st(3)
Q_1007dab6:
        fstp   QWORD PTR [esp+0xa0]
Q_1007dabd:
        fld    st(3)
Q_1007dabf:
        fld    QWORD PTR [esp+0xd8]
Q_1007dac6:
        fmul   st(1),st(0)
Q_1007dac8:
        fxch   st(1)
Q_1007daca:
        fchs
Q_1007dacc:
        fstp   QWORD PTR [esp+0x40]
Q_1007dad0:
        fld1
Q_1007dad2:
        fsubrp st(1),st(0)
Q_1007dad4:
        fst    QWORD PTR [esp+0x170]
Q_1007dadb:
        fmul   st(0),st(4)
Q_1007dadd:
        fst    QWORD PTR [esp+0x70]
Q_1007dae1:
        fld    QWORD PTR [esp+0x50]
Q_1007dae5:
        fmul   st(0),st(6)
Q_1007dae7:
        fst    QWORD PTR [esp+0xc8]
Q_1007daee:
        fxch   st(2)
Q_1007daf0:
        fmul   st(0),st(6)
Q_1007daf2:
        fst    QWORD PTR [esp+0xe0]
Q_1007daf9:
        fstp   QWORD PTR [esp+0x38]
Q_1007dafd:
        fld    QWORD PTR [esp+0x40]
Q_1007db01:
        fsub   st(0),st(2)
Q_1007db03:
        fstp   QWORD PTR [esp+0x30]
Q_1007db07:
        fsubrp st(1),st(0)
Q_1007db09:
        fst    QWORD PTR [esp+0x60]
Q_1007db0d:
        fld    QWORD PTR [esp+0x40]
Q_1007db11:
        fsub   QWORD PTR [esp+0x38]
Q_1007db15:
        fmul   QWORD PTR [esp+0xa8]
Q_1007db1c:
        fstp   QWORD PTR [esp+0x40]
Q_1007db20:
        fld    QWORD PTR [esp+0x70]
Q_1007db24:
        fsub   QWORD PTR [esp+0x38]
Q_1007db28:
        fmul   QWORD PTR [esp+0xa8]
Q_1007db2f:
        fstp   QWORD PTR [esp+0x70]
Q_1007db33:
        fld    st(0)
Q_1007db35:
        fldz
Q_1007db37:
        fsub   st(1),st(0)
Q_1007db39:
        fxch   st(1)
Q_1007db3b:
        fabs
Q_1007db3d:
        fsubp  st(2),st(0)
Q_1007db3f:
        fsubr  QWORD PTR [esp+0x30]
Q_1007db43:
        fabs
Q_1007db45:
        fld    QWORD PTR [esp+0x30]
Q_1007db49:
        fadd   st(1),st(0)
Q_1007db4b:
        mov    eax,DWORD PTR [esp+0x4c]
Q_1007db4f:
        fxch   st(2)
Q_1007db51:
        mov    ecx,eax
Q_1007db53:
        sub    eax,0x1
Q_1007db56:
        faddp  st(1),st(0)
Q_1007db58:
        fmul   QWORD PTR [kHalf]
Q_1007db5e:
        fmul   QWORD PTR [esp+0x98]
Q_1007db65:
        fxch   st(1)
Q_1007db67:
        fmul   st(0),st(2)
Q_1007db69:
        fstp   QWORD PTR [esp+0x30]
Q_1007db6d:
        fld    QWORD PTR [esp+0x60]
Q_1007db71:
        fmul   st(0),st(2)
Q_1007db73:
        fstp   QWORD PTR [esp+0x60]
Q_1007db77:
        fld    QWORD PTR [esp+0x70]
Q_1007db7b:
        fld    st(0)
Q_1007db7d:
        fsub   st(0),st(2)
Q_1007db7f:
        fabs
Q_1007db81:
        fsubp  st(1),st(0)
Q_1007db83:
        fld    QWORD PTR [esp+0x40]
Q_1007db87:
        fsubrp st(2),st(0)
Q_1007db89:
        fxch   st(1)
Q_1007db8b:
        fabs
Q_1007db8d:
        fadd   QWORD PTR [esp+0x40]
Q_1007db91:
        faddp  st(1),st(0)
Q_1007db93:
        fmul   QWORD PTR [kHalf]
Q_1007db99:
        fld    QWORD PTR [esp+0x98]
Q_1007dba0:
        fmul   st(0),st(1)
Q_1007dba2:
        fxch   st(1)
Q_1007dba4:
        fmul   QWORD PTR [esp+0xb0]
Q_1007dbab:
        fstp   QWORD PTR [esp+0x38]
Q_1007dbaf:
        fld    QWORD PTR [esp+0x60]
Q_1007dbb3:
        fld    st(0)
Q_1007dbb5:
        fsub   st(0),st(2)
Q_1007dbb7:
        fabs
Q_1007dbb9:
        fsubp  st(1),st(0)
Q_1007dbbb:
        fld    QWORD PTR [esp+0x30]
Q_1007dbbf:
        fsubrp st(2),st(0)
Q_1007dbc1:
        fxch   st(1)
Q_1007dbc3:
        fabs
Q_1007dbc5:
        fadd   QWORD PTR [esp+0x30]
Q_1007dbc9:
        faddp  st(1),st(0)
Q_1007dbcb:
        fmul   QWORD PTR [kHalf]
Q_1007dbd1:
        fst    QWORD PTR [esp+0xb0]
Q_1007dbd8:
        fld    st(0)
Q_1007dbda:
        fld    QWORD PTR [esp+0x38]
Q_1007dbde:
        fsub   st(1),st(0)
Q_1007dbe0:
        fxch   st(1)
Q_1007dbe2:
        fstp   QWORD PTR [esp+0x30]
Q_1007dbe6:
        fmul   QWORD PTR [esp+0x50]
Q_1007dbea:
        fxch   st(1)
Q_1007dbec:
        fmul   QWORD PTR [esp+0xe8]
Q_1007dbf3:
        fsubp  st(1),st(0)
Q_1007dbf5:
        fld    st(0)
Q_1007dbf7:
        fmul   st(0),st(6)
Q_1007dbf9:
        fmulp  st(1),st(0)
Q_1007dbfb:
        fld    QWORD PTR [esp+0x30]
Q_1007dbff:
        fld    st(0)
Q_1007dc01:
        fmul   st(0),st(5)
Q_1007dc03:
        fmulp  st(1),st(0)
Q_1007dc05:
        faddp  st(1),st(0)
Q_1007dc07:
        fld    st(4)
Q_1007dc09:
        fmul   st(0),st(5)
Q_1007dc0b:
        fld    QWORD PTR [esp+0xa0]
Q_1007dc12:
        fmul   st(1),st(0)
Q_1007dc14:
        fxch   st(2)
Q_1007dc16:
        fsubrp st(1),st(0)
Q_1007dc18:
        fstp   QWORD PTR [esp+0x180]
Q_1007dc1f:
        fmul   st(0),st(3)
Q_1007dc21:
        fstp   QWORD PTR [esp+0x160]
Q_1007dc28:
        fld    QWORD PTR [esp+0xb0]
Q_1007dc2f:
        fdiv   st(0),st(1)
Q_1007dc31:
        fadd   QWORD PTR [esp+0xc8]
Q_1007dc38:
        fdiv   st(0),st(3)
Q_1007dc3a:
        fst    QWORD PTR [esp+0xb8]
Q_1007dc41:
        fld    QWORD PTR [esp+0x38]
Q_1007dc45:
        fdivrp st(2),st(0)
Q_1007dc47:
        fxch   st(1)
Q_1007dc49:
        fadd   QWORD PTR [esp+0xe0]
Q_1007dc50:
        fdiv   st(0),st(3)
Q_1007dc52:
        fst    QWORD PTR [esp+0xc0]
Q_1007dc59:
        test   ecx,ecx
Q_1007dc5b:
        mov    DWORD PTR [esp+0x4c],eax
Q_1007dc5f:
        je Q_1007e1a0
Q_1007dc65:
        cmp    DWORD PTR [esp+0x24],0x0
Q_1007dc6a:
        je Q_1007d802
Q_1007dc70:
        cmp    DWORD PTR [ebp+0x8],0x0
Q_1007dc74:
        fstp   st(1)
Q_1007dc76:
        fstp   st(0)
Q_1007dc78:
        jle Q_1007dc8c
Q_1007dc7a:
        mov    ecx,DWORD PTR [ebp+0x8]
Q_1007dc7d:
        or     eax,0xffffffff
Q_1007dc80:
        lea    edi,[esp+0x118]
Q_1007dc87:
        rep stosd
Q_1007dc89:
        mov    edi,DWORD PTR [ebp+0xc]
Q_1007dc8c:
        xor    ecx,ecx
Q_1007dc8e:
        cmp    edi,0x4
Q_1007dc91:
        jl Q_1007de3e
Q_1007dc97:
        mov    ebx,DWORD PTR [ebp+0x18]
Q_1007dc9a:
        mov    edi,0x2
Q_1007dc9f:
        add    ebx,0x8
Q_1007dca2:
        mov    edx,DWORD PTR [ebx-0x8]
Q_1007dca5:
        mov    eax,DWORD PTR [esp+edx*4+0x118]
Q_1007dcac:
        cmp    eax,0xffffffff
Q_1007dcaf:
        lea    esi,[esp+edx*4+0x118]
Q_1007dcb6:
        jne Q_1007dcc3
Q_1007dcb8:
        mov    DWORD PTR [esp+edx*4+0x138],ecx
Q_1007dcbf:
        mov    DWORD PTR [esi],ecx
Q_1007dcc1:
        jmp Q_1007dcff
Q_1007dcc3:
        fld    QWORD PTR [esp+ecx*8+0x1c0]
Q_1007dcca:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007dcd1:
        fnstsw ax
Q_1007dcd3:
        test   ah,0x5
Q_1007dcd6:
        jp Q_1007dcda
Q_1007dcd8:
        mov    DWORD PTR [esi],ecx
Q_1007dcda:
        mov    eax,DWORD PTR [esp+edx*4+0x138]
Q_1007dce1:
        fld    QWORD PTR [esp+ecx*8+0x1c0]
Q_1007dce8:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007dcef:
        lea    edx,[esp+edx*4+0x138]
Q_1007dcf6:
        fnstsw ax
Q_1007dcf8:
        test   ah,0x41
Q_1007dcfb:
        jne Q_1007dcff
Q_1007dcfd:
        mov    DWORD PTR [edx],ecx
Q_1007dcff:
        mov    edx,DWORD PTR [ebx-0x4]
Q_1007dd02:
        mov    eax,DWORD PTR [esp+edx*4+0x118]
Q_1007dd09:
        cmp    eax,0xffffffff
Q_1007dd0c:
        lea    esi,[esp+edx*4+0x118]
Q_1007dd13:
        jne Q_1007dd23
Q_1007dd15:
        lea    eax,[edi-0x1]
Q_1007dd18:
        mov    DWORD PTR [esp+edx*4+0x138],eax
Q_1007dd1f:
        mov    DWORD PTR [esi],eax
Q_1007dd21:
        jmp Q_1007dd65
Q_1007dd23:
        fld    QWORD PTR [esp+ecx*8+0x1c8]
Q_1007dd2a:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007dd31:
        fnstsw ax
Q_1007dd33:
        test   ah,0x5
Q_1007dd36:
        jp Q_1007dd3d
Q_1007dd38:
        lea    eax,[edi-0x1]
Q_1007dd3b:
        mov    DWORD PTR [esi],eax
Q_1007dd3d:
        mov    eax,DWORD PTR [esp+edx*4+0x138]
Q_1007dd44:
        fld    QWORD PTR [esp+ecx*8+0x1c8]
Q_1007dd4b:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007dd52:
        lea    edx,[esp+edx*4+0x138]
Q_1007dd59:
        fnstsw ax
Q_1007dd5b:
        test   ah,0x41
Q_1007dd5e:
        jne Q_1007dd65
Q_1007dd60:
        lea    eax,[edi-0x1]
Q_1007dd63:
        mov    DWORD PTR [edx],eax
Q_1007dd65:
        mov    edx,DWORD PTR [ebx]
Q_1007dd67:
        mov    eax,DWORD PTR [esp+edx*4+0x118]
Q_1007dd6e:
        cmp    eax,0xffffffff
Q_1007dd71:
        lea    esi,[esp+edx*4+0x118]
Q_1007dd78:
        jne Q_1007dd85
Q_1007dd7a:
        mov    DWORD PTR [esp+edx*4+0x138],edi
Q_1007dd81:
        mov    DWORD PTR [esi],edi
Q_1007dd83:
        jmp Q_1007ddc1
Q_1007dd85:
        fld    QWORD PTR [esp+ecx*8+0x1d0]
Q_1007dd8c:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007dd93:
        fnstsw ax
Q_1007dd95:
        test   ah,0x5
Q_1007dd98:
        jp Q_1007dd9c
Q_1007dd9a:
        mov    DWORD PTR [esi],edi
Q_1007dd9c:
        mov    eax,DWORD PTR [esp+edx*4+0x138]
Q_1007dda3:
        fld    QWORD PTR [esp+ecx*8+0x1d0]
Q_1007ddaa:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007ddb1:
        lea    edx,[esp+edx*4+0x138]
Q_1007ddb8:
        fnstsw ax
Q_1007ddba:
        test   ah,0x41
Q_1007ddbd:
        jne Q_1007ddc1
Q_1007ddbf:
        mov    DWORD PTR [edx],edi
Q_1007ddc1:
        mov    edx,DWORD PTR [ebx+0x4]
Q_1007ddc4:
        mov    eax,DWORD PTR [esp+edx*4+0x118]
Q_1007ddcb:
        cmp    eax,0xffffffff
Q_1007ddce:
        lea    esi,[esp+edx*4+0x118]
Q_1007ddd5:
        jne Q_1007dde5
Q_1007ddd7:
        lea    eax,[edi+0x1]
Q_1007ddda:
        mov    DWORD PTR [esp+edx*4+0x138],eax
Q_1007dde1:
        mov    DWORD PTR [esi],eax
Q_1007dde3:
        jmp Q_1007de27
Q_1007dde5:
        fld    QWORD PTR [esp+ecx*8+0x1d8]
Q_1007ddec:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007ddf3:
        fnstsw ax
Q_1007ddf5:
        test   ah,0x5
Q_1007ddf8:
        jp Q_1007ddff
Q_1007ddfa:
        lea    eax,[edi+0x1]
Q_1007ddfd:
        mov    DWORD PTR [esi],eax
Q_1007ddff:
        mov    eax,DWORD PTR [esp+edx*4+0x138]
Q_1007de06:
        fld    QWORD PTR [esp+ecx*8+0x1d8]
Q_1007de0d:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007de14:
        lea    edx,[esp+edx*4+0x138]
Q_1007de1b:
        fnstsw ax
Q_1007de1d:
        test   ah,0x41
Q_1007de20:
        jne Q_1007de27
Q_1007de22:
        lea    eax,[edi+0x1]
Q_1007de25:
        mov    DWORD PTR [edx],eax
Q_1007de27:
        mov    eax,DWORD PTR [ebp+0xc]
Q_1007de2a:
        add    ecx,0x4
Q_1007de2d:
        add    eax,0xfffffffd
Q_1007de30:
        add    ebx,0x10
Q_1007de33:
        add    edi,0x4
Q_1007de36:
        cmp    ecx,eax
Q_1007de38:
        jl Q_1007dca2
Q_1007de3e:
        cmp    ecx,DWORD PTR [ebp+0xc]
Q_1007de41:
        jge Q_1007dead
Q_1007de43:
        mov    edi,DWORD PTR [ebp+0x18]
Q_1007de46:
        mov    ebx,DWORD PTR [ebp+0xc]
Q_1007de49:
        mov    edx,DWORD PTR [edi+ecx*4]
Q_1007de4c:
        mov    eax,DWORD PTR [esp+edx*4+0x118]
Q_1007de53:
        cmp    eax,0xffffffff
Q_1007de56:
        lea    esi,[esp+edx*4+0x118]
Q_1007de5d:
        jne Q_1007de6a
Q_1007de5f:
        mov    DWORD PTR [esp+edx*4+0x138],ecx
Q_1007de66:
        mov    DWORD PTR [esi],ecx
Q_1007de68:
        jmp Q_1007dea6
Q_1007de6a:
        fld    QWORD PTR [esp+ecx*8+0x1c0]
Q_1007de71:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007de78:
        fnstsw ax
Q_1007de7a:
        test   ah,0x5
Q_1007de7d:
        jp Q_1007de81
Q_1007de7f:
        mov    DWORD PTR [esi],ecx
Q_1007de81:
        mov    eax,DWORD PTR [esp+edx*4+0x138]
Q_1007de88:
        fld    QWORD PTR [esp+ecx*8+0x1c0]
Q_1007de8f:
        fcomp  QWORD PTR [esp+eax*8+0x1c0]
Q_1007de96:
        lea    edx,[esp+edx*4+0x138]
Q_1007de9d:
        fnstsw ax
Q_1007de9f:
        test   ah,0x41
Q_1007dea2:
        jne Q_1007dea6
Q_1007dea4:
        mov    DWORD PTR [edx],ecx
Q_1007dea6:
        add    ecx,0x1
Q_1007dea9:
        cmp    ecx,ebx
Q_1007deab:
        jl Q_1007de49
Q_1007dead:
        fxch   st(1)
Q_1007deaf:
        xor    edx,edx
Q_1007deb1:
        fadd   QWORD PTR [kOne]
Q_1007deb7:
        fstp   QWORD PTR [esp+0xe0]
Q_1007debe:
        test   edx,edx
Q_1007dec0:
        jne Q_1007dec6
Q_1007dec2:
        fld1
Q_1007dec4:
        jmp Q_1007decc
Q_1007dec6:
        fld    QWORD PTR [kMinusOne]
Q_1007decc:
        fst    QWORD PTR [esp+0x70]
Q_1007ded0:
        mov    eax,DWORD PTR [esp+0x2c]
Q_1007ded4:
        fld    QWORD PTR [esp+0xf0]
Q_1007dedb:
        lea    esi,[eax+edx*1]
Q_1007dede:
        cmp    edx,esi
Q_1007dee0:
        fmul   st(0),st(1)
Q_1007dee2:
        fld    QWORD PTR [esp+0x178]
Q_1007dee9:
        mov    ecx,edx
Q_1007deeb:
        mov    DWORD PTR [esp+0x24],ecx
Q_1007deef:
        fadd   st(1),st(0)
Q_1007def1:
        fxch   st(1)
Q_1007def3:
        fst    QWORD PTR [esp+0x168]
Q_1007defa:
        fld    st(2)
Q_1007defc:
        fadd   st(0),st(0)
Q_1007defe:
        fst    QWORD PTR [esp+0xb0]
Q_1007df05:
        fld    st(0)
Q_1007df07:
        fmulp  st(3),st(0)
Q_1007df09:
        fld    QWORD PTR [esp+0xe0]
Q_1007df10:
        fsubrp st(3),st(0)
Q_1007df12:
        fxch   st(2)
Q_1007df14:
        fsub   QWORD PTR [esp+0xf0]
Q_1007df1b:
        fstp   QWORD PTR [esp+0xc8]
Q_1007df22:
        jge Q_1007e145
Q_1007df28:
        mov    eax,DWORD PTR [esp+ecx*4+0xf8]
Q_1007df2f:
        test   eax,eax
Q_1007df31:
        je Q_1007e136
Q_1007df37:
        cmp    eax,0x1
Q_1007df3a:
        jg Q_1007df47
Q_1007df3c:
        cmp    DWORD PTR [esp+0x28],0x2
Q_1007df41:
        jle Q_1007e136
Q_1007df47:
        lea    eax,[edx*8+0x0]
Q_1007df4e:
        mov    edi,ecx
Q_1007df50:
        sub    edi,eax
Q_1007df52:
        mov    eax,DWORD PTR [esp+edi*4+0x138]
Q_1007df59:
        fld    QWORD PTR [esp+eax*8+0x1c0]
Q_1007df60:
        fmulp  st(3),st(0)
Q_1007df62:
        fxch   st(2)
Q_1007df64:
        faddp  st(4),st(0)
Q_1007df66:
        fimul  DWORD PTR [esp+0x24]
Q_1007df6a:
        fadd   QWORD PTR [esp+0xc8]
Q_1007df71:
        fld    st(1)
Q_1007df73:
        fchs
Q_1007df75:
        fld    st(3)
Q_1007df77:
        fsubrp st(3),st(0)
Q_1007df79:
        fld    st(0)
Q_1007df7b:
        fmul   st(0),st(6)
Q_1007df7d:
        fld    st(0)
Q_1007df7f:
        fmul   st(0),st(2)
Q_1007df81:
        fadd   st(0),st(3)
Q_1007df83:
        fst    QWORD PTR [esp+0x50]
Q_1007df87:
        fxch   st(1)
Q_1007df89:
        fmul   st(0),st(4)
Q_1007df8b:
        fadd   st(0),st(3)
Q_1007df8d:
        fstp   QWORD PTR [esp+0xa8]
Q_1007df94:
        fld    st(3)
Q_1007df96:
        fmulp  st(7),st(0)
Q_1007df98:
        fxch   st(6)
Q_1007df9a:
        fmul   st(0),st(3)
Q_1007df9c:
        fadd   st(0),st(2)
Q_1007df9e:
        fst    QWORD PTR [esp+0xa0]
Q_1007dfa5:
        fmulp  st(6),st(0)
Q_1007dfa7:
        fld    st(5)
Q_1007dfa9:
        fmul   st(0),st(2)
Q_1007dfab:
        fmul   st(0),st(6)
Q_1007dfad:
        fmul   st(0),st(4)
Q_1007dfaf:
        fmulp  st(4),st(0)
Q_1007dfb1:
        fxch   st(3)
Q_1007dfb3:
        fstp   QWORD PTR [esp+0xe8]
Q_1007dfba:
        fld    QWORD PTR [esp+0xd8]
Q_1007dfc1:
        fmul   st(0),st(1)
Q_1007dfc3:
        fchs
Q_1007dfc5:
        fld    st(1)
Q_1007dfc7:
        fmul   QWORD PTR [esp+0x170]
Q_1007dfce:
        fstp   QWORD PTR [esp+0x30]
Q_1007dfd2:
        fld    st(3)
Q_1007dfd4:
        fmul   st(0),st(5)
Q_1007dfd6:
        fld    st(3)
Q_1007dfd8:
        fmul   st(0),st(6)
Q_1007dfda:
        fstp   QWORD PTR [esp+0x40]
Q_1007dfde:
        fld    st(1)
Q_1007dfe0:
        fsub   st(0),st(1)
Q_1007dfe2:
        fstp   QWORD PTR [esp+0x38]
Q_1007dfe6:
        fsubr  QWORD PTR [esp+0x30]
Q_1007dfea:
        fst    QWORD PTR [esp+0x60]
Q_1007dfee:
        fxch   st(1)
Q_1007dff0:
        fsub   QWORD PTR [esp+0x40]
Q_1007dff4:
        fmul   QWORD PTR [esp+0x50]
Q_1007dff8:
        fstp   QWORD PTR [esp+0x98]
Q_1007dfff:
        fld    QWORD PTR [esp+0x30]
Q_1007e003:
        fsub   QWORD PTR [esp+0x40]
Q_1007e007:
        fmul   QWORD PTR [esp+0x50]
Q_1007e00b:
        fstp   QWORD PTR [esp+0x30]
Q_1007e00f:
        fld    st(0)
Q_1007e011:
        fldz
Q_1007e013:
        fsub   st(1),st(0)
Q_1007e015:
        fxch   st(1)
Q_1007e017:
        fabs
Q_1007e019:
        fsubp  st(2),st(0)
Q_1007e01b:
        fsubr  QWORD PTR [esp+0x38]
Q_1007e01f:
        fabs
Q_1007e021:
        fld    QWORD PTR [esp+0x38]
Q_1007e025:
        fadd   st(1),st(0)
Q_1007e027:
        fxch   st(2)
Q_1007e029:
        faddp  st(1),st(0)
Q_1007e02b:
        fmul   QWORD PTR [kHalf]
Q_1007e031:
        fmul   QWORD PTR [esp+0xa8]
Q_1007e038:
        fxch   st(1)
Q_1007e03a:
        fmul   st(0),st(6)
Q_1007e03c:
        fstp   QWORD PTR [esp+0x38]
Q_1007e040:
        fld    QWORD PTR [esp+0x60]
Q_1007e044:
        fmul   st(0),st(6)
Q_1007e046:
        fstp   QWORD PTR [esp+0x60]
Q_1007e04a:
        fld    QWORD PTR [esp+0x30]
Q_1007e04e:
        fld    st(0)
Q_1007e050:
        fsub   st(0),st(2)
Q_1007e052:
        fabs
Q_1007e054:
        fsubp  st(1),st(0)
Q_1007e056:
        fld    QWORD PTR [esp+0x98]
Q_1007e05d:
        fsubrp st(2),st(0)
Q_1007e05f:
        fxch   st(1)
Q_1007e061:
        fabs
Q_1007e063:
        fadd   QWORD PTR [esp+0x98]
Q_1007e06a:
        faddp  st(1),st(0)
Q_1007e06c:
        fmul   QWORD PTR [kHalf]
Q_1007e072:
        fld    QWORD PTR [esp+0xa8]
Q_1007e079:
        fmul   st(0),st(1)
Q_1007e07b:
        fxch   st(1)
Q_1007e07d:
        fmul   QWORD PTR [esp+0xa0]
Q_1007e084:
        fstp   QWORD PTR [esp+0x40]
Q_1007e088:
        fld    QWORD PTR [esp+0x60]
Q_1007e08c:
        fld    st(0)
Q_1007e08e:
        fsub   st(0),st(2)
Q_1007e090:
        fabs
Q_1007e092:
        fsubp  st(1),st(0)
Q_1007e094:
        fld    QWORD PTR [esp+0x38]
Q_1007e098:
        fsubrp st(2),st(0)
Q_1007e09a:
        fxch   st(1)
Q_1007e09c:
        fabs
Q_1007e09e:
        fadd   QWORD PTR [esp+0x38]
Q_1007e0a2:
        faddp  st(1),st(0)
Q_1007e0a4:
        fmul   QWORD PTR [kHalf]
Q_1007e0aa:
        fst    QWORD PTR [esp+0xa0]
Q_1007e0b1:
        fld    st(0)
Q_1007e0b3:
        fld    QWORD PTR [esp+0x40]
Q_1007e0b7:
        fsub   st(1),st(0)
Q_1007e0b9:
        fmul   st(0),st(5)
Q_1007e0bb:
        fxch   st(2)
Q_1007e0bd:
        fmul   st(0),st(4)
Q_1007e0bf:
        fsubp  st(2),st(0)
Q_1007e0c1:
        fld    st(1)
Q_1007e0c3:
        fmul   QWORD PTR [esp+0xd0]
Q_1007e0ca:
        fmulp  st(2),st(0)
Q_1007e0cc:
        fld    st(0)
Q_1007e0ce:
        fmul   st(0),st(3)
Q_1007e0d0:
        fmulp  st(1),st(0)
Q_1007e0d2:
        faddp  st(1),st(0)
Q_1007e0d4:
        fld    st(4)
Q_1007e0d6:
        fmul   st(0),st(5)
Q_1007e0d8:
        fld    QWORD PTR [esp+0xe8]
Q_1007e0df:
        fmul   st(1),st(0)
Q_1007e0e1:
        fxch   st(2)
Q_1007e0e3:
        fsubrp st(1),st(0)
Q_1007e0e5:
        fxch   st(1)
Q_1007e0e7:
        fmul   st(0),st(2)
Q_1007e0e9:
        fmul   QWORD PTR [esp+0x180]
Q_1007e0f0:
        fxch   st(1)
Q_1007e0f2:
        fmul   QWORD PTR [esp+0x160]
Q_1007e0f9:
        fcompp
Q_1007e0fb:
        fnstsw ax
Q_1007e0fd:
        test   ah,0x5
Q_1007e100:
        jnp Q_1007e15d
Q_1007e102:
        fstp   st(0)
Q_1007e104:
        fstp   st(0)
Q_1007e106:
        fstp   st(1)
Q_1007e108:
        fstp   st(1)
Q_1007e10a:
        fstp   st(0)
Q_1007e10c:
        fld    QWORD PTR [esp+0xd0]
Q_1007e113:
        fld    QWORD PTR [esp+0xb0]
Q_1007e11a:
        fld    QWORD PTR [esp+0x158]
Q_1007e121:
        fld    QWORD PTR [esp+0x168]
Q_1007e128:
        fld    QWORD PTR [esp+0x70]
Q_1007e12c:
        fld    QWORD PTR [esp+0x68]
Q_1007e130:
        fxch   st(4)
Q_1007e132:
        fxch   st(1)
Q_1007e134:
        fxch   st(2)
Q_1007e136:
        add    ecx,0x1
Q_1007e139:
        cmp    ecx,esi
Q_1007e13b:
        mov    DWORD PTR [esp+0x24],ecx
Q_1007e13f:
        jl Q_1007df28
Q_1007e145:
        fstp   st(0)
Q_1007e147:
        add    edx,0x1
Q_1007e14a:
        cmp    edx,0x2
Q_1007e14d:
        fstp   st(1)
Q_1007e14f:
        fstp   st(0)
Q_1007e151:
        jl Q_1007debe
Q_1007e157:
        fstp   st(1)
Q_1007e159:
        fstp   st(1)
Q_1007e15b:
        jmp Q_1007e1ae
Q_1007e15d:
        fld    QWORD PTR [esp+0xa0]
Q_1007e164:
        mov    ebx,DWORD PTR [esp+0x2c]
Q_1007e168:
        fdiv   st(0),st(5)
Q_1007e16a:
        fxch   st(3)
Q_1007e16c:
        fmul   st(0),st(4)
Q_1007e16e:
        faddp  st(3),st(0)
Q_1007e170:
        fdiv   st(2),st(0)
Q_1007e172:
        fxch   st(2)
Q_1007e174:
        fst    QWORD PTR [esp+0xb8]
Q_1007e17b:
        fld    QWORD PTR [esp+0x40]
Q_1007e17f:
        fdivrp st(5),st(0)
Q_1007e181:
        fxch   st(1)
Q_1007e183:
        fmulp  st(3),st(0)
Q_1007e185:
        fxch   st(3)
Q_1007e187:
        faddp  st(2),st(0)
Q_1007e189:
        fdivp  st(1),st(0)
Q_1007e18b:
        fst    QWORD PTR [esp+0xc0]
Q_1007e192:
        fld    QWORD PTR [esp+0xd0]
Q_1007e199:
        fldz
Q_1007e19b:
        jmp Q_1007d80c
Q_1007e1a0:
        fstp   st(1)
Q_1007e1a2:
        fstp   st(2)
Q_1007e1a4:
        fstp   st(2)
Q_1007e1a6:
        fstp   st(0)
Q_1007e1a8:
        fstp   st(1)
Q_1007e1aa:
        jmp Q_1007e1ae
Q_1007e1ac:
        fstp   st(0)
Q_1007e1ae:
        fstp   st(0)
Q_1007e1b0:
        xor    esi,esi
Q_1007e1b2:
        fld    QWORD PTR [esp+esi*8+0xb8]
Q_1007e1b9:
        sub    esp,0x8
Q_1007e1bc:
        fadd   QWORD PTR [esp+0xe0]
Q_1007e1c3:
        fmul   QWORD PTR [kByteScale]
Q_1007e1c9:
        fadd   QWORD PTR [kHalf]
Q_1007e1cf:
        fstp   QWORD PTR [esp]
Q_1007e1d2:
        call AlphaFloor
Q_1007e1d7:
        add    esp,0x8
Q_1007e1da:
        call AlphaTruncate
Q_1007e1df:
        test   eax,eax
Q_1007e1e1:
        mov    DWORD PTR [esp+esi*4+0x78],eax
Q_1007e1e5:
        jge Q_1007e1eb
Q_1007e1e7:
        xor    eax,eax
Q_1007e1e9:
        jmp Q_1007e1f7
Q_1007e1eb:
        cmp    eax,0xff
Q_1007e1f0:
        jle Q_1007e1f7
Q_1007e1f2:
        mov    eax,0xff
Q_1007e1f7:
        mov    DWORD PTR [esp+esi*4+0x78],eax
Q_1007e1fb:
        add    esi,0x1
Q_1007e1fe:
        cmp    esi,0x2
Q_1007e201:
        jl Q_1007e1b2
Q_1007e203:
        mov    eax,DWORD PTR [ebp+0x8]
Q_1007e206:
        mov    ecx,0x2
Q_1007e20b:
        cmp    eax,ecx
Q_1007e20d:
        jle Q_1007e247
Q_1007e20f:
        lea    esi,[eax-0x2]
Q_1007e212:
        mov    eax,esi
Q_1007e214:
        cdq
Q_1007e215:
        sub    eax,edx
Q_1007e217:
        mov    edi,eax
Q_1007e219:
        sar    edi,1
Q_1007e21b:
        jmp Q_1007e220
Q_1007e21d:
        lea    ecx,[ecx+0x0]
Q_1007e220:
        mov    eax,DWORD PTR [esp+0x78]
Q_1007e224:
        lea    edx,[ecx-0x1]
Q_1007e227:
        imul   eax,esi
Q_1007e22a:
        imul   edx,DWORD PTR [esp+0x7c]
Q_1007e22f:
        add    edx,edi
Q_1007e231:
        add    eax,edx
Q_1007e233:
        cdq
Q_1007e234:
        idiv   DWORD PTR [esp+0x2c]
Q_1007e238:
        add    ecx,0x1
Q_1007e23b:
        sub    esi,0x1
Q_1007e23e:
        cmp    ecx,DWORD PTR [ebp+0x8]
Q_1007e241:
        mov    DWORD PTR [esp+ecx*4+0x74],eax
Q_1007e245:
        jl Q_1007e220
Q_1007e247:
        cmp    DWORD PTR [ebp+0xc],0x0
Q_1007e24b:
        fldz
Q_1007e24d:
        mov    eax,DWORD PTR [ebp+0x14]
Q_1007e250:
        mov    ecx,DWORD PTR [esp+0x78]
Q_1007e254:
        mov    edx,DWORD PTR [esp+0x7c]
Q_1007e258:
        mov    DWORD PTR [eax],ecx
Q_1007e25a:
        mov    DWORD PTR [eax+0x4],edx
Q_1007e25d:
        mov    DWORD PTR [esp+0x28],0x0
Q_1007e265:
        jle Q_1007e3c7
Q_1007e26b:
        fld    QWORD PTR [kByteScale]
Q_1007e271:
        xor    edi,edi
Q_1007e273:
        cmp    DWORD PTR [esp+0x2c],0x4
Q_1007e278:
        mov    ecx,0x1
Q_1007e27d:
        jl Q_1007e357
Q_1007e283:
        mov    eax,DWORD PTR [ebp+0x10]
Q_1007e286:
        mov    edx,DWORD PTR [esp+0x28]
Q_1007e28a:
        fld    QWORD PTR [eax+edx*8]
Q_1007e28d:
        lea    esi,[esp+0x80]
Q_1007e294:
        mov    eax,0xc
Q_1007e299:
        fmul   st(0),st(1)
Q_1007e29b:
        sub    eax,esi
Q_1007e29d:
        mov    DWORD PTR [esp+0x5c],eax
Q_1007e2a1:
        mov    eax,0x10
Q_1007e2a6:
        xor    edx,edx
Q_1007e2a8:
        sub    eax,esi
Q_1007e2aa:
        mov    DWORD PTR [esp+0x24],0x8
Q_1007e2b2:
        mov    DWORD PTR [esp+0x4c],eax
Q_1007e2b6:
        lea    ebx,[ecx*4+0x0]
Q_1007e2bd:
        fild   DWORD PTR [esp+ebx*1+0x78]
Q_1007e2c1:
        lea    esi,[esp+ebx*1+0x7c]
Q_1007e2c5:
        fsub   st(0),st(1)
Q_1007e2c7:
        fabs
Q_1007e2c9:
        fild   DWORD PTR [esp+edx*1+0x78]
Q_1007e2cd:
        fsub   st(0),st(2)
Q_1007e2cf:
        fabs
Q_1007e2d1:
        fcompp
Q_1007e2d3:
        fnstsw ax
Q_1007e2d5:
        test   ah,0x41
Q_1007e2d8:
        jne Q_1007e2de
Q_1007e2da:
        mov    edi,ecx
Q_1007e2dc:
        mov    edx,ebx
Q_1007e2de:
        fild   DWORD PTR [esi]
Q_1007e2e0:
        fsub   st(0),st(1)
Q_1007e2e2:
        fabs
Q_1007e2e4:
        fild   DWORD PTR [esp+edx*1+0x78]
Q_1007e2e8:
        fsub   st(0),st(2)
Q_1007e2ea:
        fabs
Q_1007e2ec:
        fcompp
Q_1007e2ee:
        fnstsw ax
Q_1007e2f0:
        test   ah,0x41
Q_1007e2f3:
        jne Q_1007e2fc
Q_1007e2f5:
        mov    edx,DWORD PTR [esp+0x24]
Q_1007e2f9:
        lea    edi,[ecx+0x1]
Q_1007e2fc:
        fild   DWORD PTR [esi+0x4]
Q_1007e2ff:
        fsub   st(0),st(1)
Q_1007e301:
        fabs
Q_1007e303:
        fild   DWORD PTR [esp+edx*1+0x78]
Q_1007e307:
        fsub   st(0),st(2)
Q_1007e309:
        fabs
Q_1007e30b:
        fcompp
Q_1007e30d:
        fnstsw ax
Q_1007e30f:
        test   ah,0x41
Q_1007e312:
        jne Q_1007e31e
Q_1007e314:
        mov    eax,DWORD PTR [esp+0x5c]
Q_1007e318:
        lea    edi,[ecx+0x2]
Q_1007e31b:
        lea    edx,[esi+eax*1]
Q_1007e31e:
        fild   DWORD PTR [esi+0x8]
Q_1007e321:
        fsub   st(0),st(1)
Q_1007e323:
        fabs
Q_1007e325:
        fild   DWORD PTR [esp+edx*1+0x78]
Q_1007e329:
        fsub   st(0),st(2)
Q_1007e32b:
        fabs
Q_1007e32d:
        fcompp
Q_1007e32f:
        fnstsw ax
Q_1007e331:
        test   ah,0x41
Q_1007e334:
        jne Q_1007e33f
Q_1007e336:
        mov    edx,DWORD PTR [esp+0x4c]
Q_1007e33a:
        lea    edi,[ecx+0x3]
Q_1007e33d:
        add    edx,esi
Q_1007e33f:
        mov    eax,DWORD PTR [ebp+0x8]
Q_1007e342:
        add    DWORD PTR [esp+0x24],0x10
Q_1007e347:
        add    ecx,0x4
Q_1007e34a:
        add    eax,0xfffffffd
Q_1007e34d:
        cmp    ecx,eax
Q_1007e34f:
        jl Q_1007e2b6
Q_1007e355:
        fstp   st(0)
Q_1007e357:
        cmp    ecx,DWORD PTR [ebp+0x8]
Q_1007e35a:
        mov    ebx,DWORD PTR [esp+0x28]
Q_1007e35e:
        jge Q_1007e39d
Q_1007e360:
        mov    eax,DWORD PTR [ebp+0x10]
Q_1007e363:
        fld    QWORD PTR [eax+ebx*8]
Q_1007e366:
        lea    esi,[edi*4+0x0]
Q_1007e36d:
        fmul   st(0),st(1)
Q_1007e36f:
        lea    edx,[ecx*4+0x0]
Q_1007e376:
        fild   DWORD PTR [esp+edx*1+0x78]
Q_1007e37a:
        fsub   st(0),st(1)
Q_1007e37c:
        fabs
Q_1007e37e:
        fild   DWORD PTR [esp+esi*1+0x78]
Q_1007e382:
        fsub   st(0),st(2)
Q_1007e384:
        fabs
Q_1007e386:
        fcompp
Q_1007e388:
        fnstsw ax
Q_1007e38a:
        test   ah,0x41
Q_1007e38d:
        jne Q_1007e393
Q_1007e38f:
        mov    edi,ecx
Q_1007e391:
        mov    esi,edx
Q_1007e393:
        add    ecx,0x1
Q_1007e396:
        cmp    ecx,DWORD PTR [ebp+0x8]
Q_1007e399:
        jl Q_1007e36f
Q_1007e39b:
        fstp   st(0)
Q_1007e39d:
        mov    ecx,DWORD PTR [ebp+0x18]
Q_1007e3a0:
        fild   DWORD PTR [esp+edi*4+0x78]
Q_1007e3a4:
        mov    edx,DWORD PTR [ebp+0x10]
Q_1007e3a7:
        mov    DWORD PTR [ecx+ebx*4],edi
Q_1007e3aa:
        fld    QWORD PTR [edx+ebx*8]
Q_1007e3ad:
        fmul   st(0),st(2)
Q_1007e3af:
        add    ebx,0x1
Q_1007e3b2:
        cmp    ebx,DWORD PTR [ebp+0xc]
Q_1007e3b5:
        mov    DWORD PTR [esp+0x28],ebx
Q_1007e3b9:
        fsubp  st(1),st(0)
Q_1007e3bb:
        fmul   st(0),st(0)
Q_1007e3bd:
        faddp  st(2),st(0)
Q_1007e3bf:
        jl Q_1007e271
Q_1007e3c5:
        fstp   st(0)
Q_1007e3c7:
        pop    edi
Q_1007e3c8:
        pop    esi
Q_1007e3c9:
        pop    ebx
Q_1007e3ca:
        mov    esp,ebp
Q_1007e3cc:
        pop    ebp
Q_1007e3cd:
        ret    0x14
    }
}
