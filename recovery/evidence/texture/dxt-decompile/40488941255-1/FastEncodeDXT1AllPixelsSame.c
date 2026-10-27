/* Original DLL evidence only; RVA 0x8b380; not recovered business source. */

void __fastcall FUN_1008b380(undefined4 param_1,undefined4 param_2,int param_3)

{
  int iVar1;
  undefined4 extraout_ECX;
  undefined4 extraout_ECX_00;
  undefined4 *puVar2;
  ulonglong uVar3;
  ulonglong uVar4;
  ulonglong uVar5;
  
  uVar3 = FUN_1004e830(param_1,param_2);
  *(int *)(param_3 + 0x3038) = (int)uVar3;
  uVar4 = FUN_1004e830(extraout_ECX,(int)(uVar3 >> 0x20));
  *(int *)(param_3 + 0x303c) = (int)uVar4;
  uVar5 = FUN_1004e830(extraout_ECX_00,(int)(uVar4 >> 0x20));
  *(int *)(param_3 + 0x3040) = (int)uVar5;
  *(int *)(param_3 + 0x304c) = (int)uVar5;
  iVar1 = 0;
  *(int *)(param_3 + 0x3044) = (int)uVar3;
  *(int *)(param_3 + 0x3048) = (int)uVar4;
  *(undefined4 *)(param_3 + 0x3034) = 4;
  if (0 < *(int *)(param_3 + 8)) {
    puVar2 = (undefined4 *)(param_3 + 0x3050);
    do {
      *puVar2 = 0;
      iVar1 = iVar1 + 1;
      puVar2 = puVar2 + 1;
    } while (iVar1 < *(int *)(param_3 + 8));
  }
  return;
}

