/* Original DLL evidence only; RVA 0x833f0; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

float10 FUN_100833f0(double *param_1,double param_2)

{
  double *pdVar1;
  float10 fVar2;
  double *pdVar3;
  uint uVar4;
  uint uVar5;
  uint extraout_ECX;
  undefined4 extraout_ECX_00;
  undefined4 extraout_ECX_01;
  uint extraout_ECX_02;
  undefined4 extraout_ECX_03;
  undefined4 extraout_ECX_04;
  uint extraout_ECX_05;
  uint uVar6;
  undefined4 extraout_ECX_06;
  undefined4 extraout_ECX_07;
  int iVar7;
  undefined4 extraout_EDX;
  undefined4 extraout_EDX_00;
  float10 fVar8;
  float10 fVar9;
  float10 fVar10;
  float10 extraout_ST0;
  float10 extraout_ST0_00;
  float10 fVar11;
  float10 extraout_ST0_01;
  float10 extraout_ST1;
  float10 extraout_ST1_00;
  ulonglong uVar12;
  ulonglong uVar13;
  ulonglong uVar14;
  ulonglong uVar15;
  int local_38c8;
  uint local_38c0;
  uint local_38bc;
  int local_38b8;
  int local_38b4;
  int local_389c;
  double local_3898;
  double local_3890;
  double local_3888;
  double local_3880;
  double local_3878 [3];
  void *local_385c;
  undefined1 local_3858 [14412];
  undefined4 uStack_c;
  
  uStack_c = 0x10083400;
  local_385c = new__(-(uint)((int)((ulonglong)*(uint *)(param_1 + 1) * 0x18 >> 0x20) != 0) |
                     (uint)((ulonglong)*(uint *)(param_1 + 1) * 0x18));
  iVar7 = 0;
  if (0 < *(int *)(param_1 + 1)) {
    pdVar3 = (double *)((int)local_385c + 0x10);
    pdVar1 = param_1;
    do {
      iVar7 = iVar7 + 1;
      pdVar3[-2] = pdVar1[2] * param_1[0x603];
      pdVar3[-1] = pdVar1[3] * param_1[0x604];
      *pdVar3 = pdVar1[4] * param_1[0x605];
      pdVar3 = pdVar3 + 3;
      pdVar1 = pdVar1 + 3;
    } while (iVar7 < *(int *)(param_1 + 1));
  }
  FUN_10082bd0((int)local_385c,*(int *)(param_1 + 1),local_3878,&local_3898);
  memcpy(local_3858,param_1,0x3850);
  local_3880 = param_2;
  fVar11 = (float10)0;
  fVar8 = (float10)local_3898;
  uVar6 = extraout_ECX;
  if (fVar8 == fVar11) {
    fVar9 = (float10)local_3890;
    fVar10 = (float10)local_3888;
  }
  else {
    if (fVar11 <= fVar8) {
      fVar10 = (float10)local_3888;
      fVar9 = (float10)local_3890;
      local_38c0 = 0;
    }
    else {
      fVar8 = -fVar8;
      local_38c0 = 0;
      fVar9 = -(float10)local_3890;
      fVar10 = -(float10)local_3888;
      local_3898 = (double)fVar8;
      local_3890 = (double)fVar9;
      local_3888 = (double)fVar10;
    }
    do {
      if ((float10)local_3880 <= fVar11) break;
      uVar12 = FUN_1004e830(local_38c0 >> 2 | local_38c0 * 8,local_38c0 * 8);
      uVar13 = FUN_1004e830(extraout_ECX_00,(int)(uVar12 >> 0x20));
      uVar5 = local_38c0 + 1;
      uVar6 = uVar5;
      fVar2 = extraout_ST0;
      fVar8 = extraout_ST1;
      fVar10 = fVar9;
      for (uVar4 = uVar5; fVar9 = fVar11, fVar11 = fVar2, uVar4 < 0x20; uVar4 = uVar4 + 1) {
        local_389c = (int)uVar13;
        uVar6 = uVar4;
        if ((float10)local_3880 <= fVar11) break;
        uVar14 = FUN_1004e830(uVar4 * 8,(int)(uVar13 >> 0x20));
        uVar15 = FUN_1004e830(extraout_ECX_01,(int)(uVar14 >> 0x20));
        local_38c8 = -1;
        do {
          local_38b8 = -1;
          do {
            local_38bc = -1;
            do {
              iVar7 = -1;
              do {
                FUN_1007e6f0((int *)param_1,local_3858,&local_3880,local_38c0,
                             local_38c8 + (int)uVar12,local_38b8 + local_389c,uVar4,
                             local_38bc + (int)uVar14,iVar7 + (int)uVar15);
                uVar13 = CONCAT44(extraout_EDX,local_389c);
                iVar7 = iVar7 + 1;
              } while (iVar7 < 2);
              local_38bc = local_38bc + 1;
            } while ((int)local_38bc < 2);
            local_38b8 = local_38b8 + 1;
          } while (local_38b8 < 2);
          local_38c8 = local_38c8 + 1;
        } while (local_38c8 < 2);
        fVar10 = (float10)local_3888;
        fVar8 = (float10)local_3898;
        uVar6 = extraout_ECX_02;
        fVar2 = (float10)0;
        fVar11 = (float10)local_3890;
      }
      local_38c0 = uVar5;
    } while (uVar5 < 0x1f);
  }
  if (fVar11 != fVar9) {
    if (fVar9 < fVar11) {
      fVar8 = -fVar8;
      fVar9 = -fVar9;
      fVar10 = -fVar10;
      local_3898 = (double)fVar8;
      local_3890 = (double)fVar9;
      local_3888 = (double)fVar10;
    }
    local_38bc = 0;
    do {
      if ((float10)local_3880 <= fVar11) break;
      uVar13 = FUN_1004e830(uVar6,local_38bc >> 4 | local_38bc * 4);
      uVar12 = FUN_1004e830(extraout_ECX_03,(int)(uVar13 >> 0x20));
      uVar5 = local_38bc + 1;
      uVar4 = uVar5;
      uVar6 = uVar5;
      fVar2 = extraout_ST0_00;
      fVar8 = extraout_ST1_00;
      fVar10 = fVar9;
      while ((fVar9 = fVar11, fVar11 = fVar2, uVar4 < 0x40 &&
             (uVar6 = uVar4, fVar11 < (float10)local_3880))) {
        uVar14 = FUN_1004e830(uVar4,uVar4 >> 4 | uVar4 * 4);
        uVar15 = FUN_1004e830(extraout_ECX_04,(int)(uVar14 >> 0x20));
        local_38b8 = -1;
        do {
          local_38c8 = -1;
          do {
            local_38c0 = -1;
            do {
              iVar7 = -1;
              do {
                FUN_1007e6f0((int *)param_1,local_3858,&local_3880,local_38b8 + (int)uVar13,
                             local_38bc,local_38c8 + (int)uVar12,local_38c0 + (int)uVar14,uVar4,
                             iVar7 + (int)uVar15);
                iVar7 = iVar7 + 1;
              } while (iVar7 < 2);
              local_38c0 = local_38c0 + 1;
            } while ((int)local_38c0 < 2);
            local_38c8 = local_38c8 + 1;
          } while (local_38c8 < 2);
          local_38b8 = local_38b8 + 1;
        } while (local_38b8 < 2);
        uVar4 = uVar4 + 1;
        fVar10 = (float10)local_3888;
        fVar8 = (float10)local_3898;
        uVar6 = extraout_ECX_05;
        fVar11 = (float10)local_3890;
        fVar2 = (float10)0;
      }
      local_38bc = uVar5;
    } while (uVar5 < 0x1f);
  }
  if (fVar11 != fVar10) {
    if (fVar10 < fVar11) {
      local_3898 = (double)-fVar8;
      local_3890 = (double)-fVar9;
      local_3888 = (double)-fVar10;
    }
    local_38bc = 0;
    do {
      if ((float10)local_3880 <= fVar11) break;
      uVar12 = FUN_1004e830(local_38bc >> 2 | local_38bc * 8,local_38bc * 8);
      uVar13 = FUN_1004e830(extraout_ECX_06,(int)(uVar12 >> 0x20));
      uVar6 = local_38bc + 1;
      fVar11 = extraout_ST0_01;
      for (uVar5 = uVar6; uVar5 < 0x20; uVar5 = uVar5 + 1) {
        local_38b4 = (int)uVar13;
        if ((float10)local_3880 <= fVar11) break;
        uVar14 = FUN_1004e830(uVar5 * 8,(int)(uVar13 >> 0x20));
        uVar15 = FUN_1004e830(extraout_ECX_07,(int)(uVar14 >> 0x20));
        local_38b8 = -1;
        do {
          local_38c8 = -1;
          do {
            local_38c0 = -1;
            do {
              iVar7 = -1;
              do {
                FUN_1007e6f0((int *)param_1,local_3858,&local_3880,local_38b8 + local_38b4,
                             local_38c8 + (int)uVar12,local_38bc,local_38c0 + (int)uVar15,
                             iVar7 + (int)uVar14,uVar5);
                uVar13 = CONCAT44(extraout_EDX_00,local_38b4);
                iVar7 = iVar7 + 1;
              } while (iVar7 < 2);
              local_38c0 = local_38c0 + 1;
            } while ((int)local_38c0 < 2);
            local_38c8 = local_38c8 + 1;
          } while (local_38c8 < 2);
          local_38b8 = local_38b8 + 1;
        } while (local_38b8 < 2);
        fVar11 = (float10)0;
      }
      local_38bc = uVar6;
    } while (uVar6 < 0x1f);
  }
  memcpy(param_1,local_3858,0x3850);
  delete__(local_385c);
  return (float10)local_3880;
}

