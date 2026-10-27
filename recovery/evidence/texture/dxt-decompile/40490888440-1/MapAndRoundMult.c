/* Original DLL evidence only; RVA 0x7bb80; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */

float10 FUN_1007bb80(int *param_1,int param_2,int param_3,undefined4 param_4,undefined4 param_5,
                    undefined4 param_6,double *param_7,int param_8,undefined4 *param_9,
                    undefined8 param_10)

{
  double dVar1;
  double *pdVar2;
  undefined4 *puVar3;
  double *pdVar4;
  undefined4 *puVar5;
  int iVar6;
  int iVar7;
  float10 fVar8;
  int local_10cc;
  uint local_10c8;
  int local_10c4;
  int local_10c0;
  uint local_10bc [3];
  double local_10b0;
  undefined4 local_10a8;
  undefined4 uStack_10a4;
  undefined1 local_10a0 [4];
  undefined4 auStack_109c [11];
  undefined1 local_1070 [8];
  double local_1068 [5];
  undefined4 local_1040 [1023];
  undefined4 uStack_44;
  
  uStack_44 = 0x1007bb90;
  memset(local_1070,0,0x30);
  local_10b0 = 0.0;
  local_10a8 = 0;
  uStack_10a4 = 0;
  memset(local_10a0,0,0x30);
  local_10bc[0] = 0;
  local_10bc[1] = 0;
  memset(local_1040,0,0x1000);
  local_10bc[0] = (uint)(*param_1 == 4);
  fVar8 = (float10)FUN_10061460(local_10bc[0] + 3,param_3,param_4,param_5,param_6,param_7,local_10a0
                                ,local_1040,param_10);
  local_10b0 = (double)fVar8;
  *param_1 = *param_1 + -2;
  iVar7 = 1;
  local_10c8 = 0;
  if (0 < param_2 + -2) {
    local_10cc = 0;
    local_10c4 = 0;
    do {
      local_10c0 = 0;
      if (0 < *(int *)(&DAT_100bd938 + (*param_1 + local_10cc) * 4)) {
        do {
          iVar6 = 2;
          pdVar2 = (double *)(&DAT_100bd638 + (local_10c0 + (*param_1 + local_10c4) * 6) * 0x20);
          pdVar4 = local_1068;
          do {
            iVar6 = iVar6 + -1;
            pdVar4[-1] = pdVar2[1] * param_7[3] + *param_7 * *pdVar2;
            *pdVar4 = *pdVar2 * param_7[1] + param_7[4] * pdVar2[1];
            pdVar4[1] = pdVar2[1] * param_7[5] + *pdVar2 * param_7[2];
            pdVar2 = pdVar2 + 2;
            pdVar4 = pdVar4 + 3;
          } while (iVar6 != 0);
          local_10bc[iVar7] = local_10c8;
          fVar8 = (float10)FUN_10061460(local_10c8 + 3,param_3,param_4,param_5,param_6,local_1070,
                                        local_10a0 + iVar7 * 0x18,local_1040 + iVar7 * 0x200,
                                        param_10);
          (&local_10b0)[iVar7] = (double)fVar8;
          if (fVar8 < (float10)*(double *)(&local_10a8 + iVar7 * -2)) {
            iVar7 = 1 - iVar7;
          }
          local_10c0 = local_10c0 + 1;
        } while (local_10c0 < *(int *)(&DAT_100bd938 + (*param_1 + local_10cc) * 4));
      }
      local_10c4 = local_10c4 + 2;
      local_10cc = local_10cc + 3;
      local_10c8 = local_10c8 + 1;
    } while ((int)local_10c8 < param_2 + -2);
  }
  iVar7 = 1 - iVar7;
  if (0 < param_3) {
    puVar3 = local_1040 + iVar7 * 0x200;
    for (; param_3 != 0; param_3 = param_3 + -1) {
      *param_9 = *puVar3;
      puVar3 = puVar3 + 1;
      param_9 = param_9 + 1;
    }
  }
  puVar5 = (undefined4 *)(param_8 + 8);
  puVar3 = auStack_109c + iVar7 * 6;
  iVar6 = 2;
  do {
    puVar5[-2] = puVar3[-1];
    puVar5[-1] = *puVar3;
    *puVar5 = puVar3[1];
    puVar3 = puVar3 + 3;
    puVar5 = puVar5 + 3;
    iVar6 = iVar6 + -1;
  } while (iVar6 != 0);
  dVar1 = (&local_10b0)[iVar7];
  *param_1 = local_10bc[iVar7] + 3;
  return (float10)dVar1;
}

