/* Original DLL evidence only; RVA 0x8c1f0; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_1008c1f0(void *this,double *param_1)

{
  float fVar1;
  float fVar2;
  float *pfVar3;
  undefined4 extraout_ECX;
  undefined4 extraout_ECX_00;
  undefined4 extraout_ECX_01;
  undefined4 uVar4;
  double *pdVar5;
  int iVar6;
  float extraout_EDX;
  undefined4 extraout_EDX_00;
  float fVar7;
  float extraout_EDX_01;
  float fVar8;
  int iVar9;
  float10 fVar10;
  float local_20b8;
  float local_20b0;
  float local_20ac;
  float local_20a8;
  float local_20a4;
  float local_209c [4];
  float local_208c;
  float local_2088;
  float fStack_2084;
  float fStack_2080;
  float local_207c;
  double local_2078;
  float fStack_2070;
  float fStack_206c;
  float fStack_2068;
  float fStack_2060;
  float fStack_205c;
  float fStack_2058;
  double local_2050;
  double local_2048;
  float local_203c [13];
  float local_2008 [2047];
  undefined4 uStack_c;
  
  uStack_c = 0x1008c200;
  iVar9 = *(int *)(param_1 + 1);
  iVar6 = 1;
  *(undefined4 *)((int)param_1 + 0x3034) = 4;
  local_203c[0] = 0.0;
  local_203c[1] = 2.8026e-45;
  local_203c[2] = 4.2039e-45;
  local_203c[3] = 1.4013e-45;
  if (iVar9 < 2) {
LAB_1008c280:
    FUN_1008b380(this,iVar6,(int)param_1);
    return;
  }
  this = param_1 + 2;
                    /* WARNING: Load size is inaccurate */
  while (((*this == *(double *)((int)this + 0x18) &&
          (*(double *)((int)this + 8) == *(double *)((int)this + 0x20))) &&
         (*(double *)((int)this + 0x10) == *(double *)((int)this + 0x28)))) {
    iVar6 = iVar6 + 1;
    this = (void *)((int)this + 0x18);
    if (iVar9 <= iVar6) goto LAB_1008c280;
  }
  if (0 < iVar9) {
    pfVar3 = local_2008 + 1;
    iVar6 = iVar9;
    pdVar5 = param_1;
    do {
      iVar6 = iVar6 + -1;
      pfVar3[-1] = (float)((float10)pdVar5[2] * (float10)param_1[0x603]);
      *pfVar3 = (float)((float10)pdVar5[3] * (float10)param_1[0x604]);
      pfVar3[1] = (float)((float10)pdVar5[4] * (float10)param_1[0x605]);
      pfVar3 = pfVar3 + 4;
      pdVar5 = pdVar5 + 3;
    } while (iVar6 != 0);
  }
  local_20ac = 0.0;
  local_20a8 = 0.0;
  local_20a4 = 0.0;
  if (0 < iVar9) {
    pfVar3 = local_2008 + 1;
    iVar6 = iVar9;
    do {
      iVar6 = iVar6 + -1;
      local_20ac = pfVar3[-1] + local_20ac;
      local_20a8 = local_20a8 + *pfVar3;
      local_20a4 = pfVar3[1] + local_20a4;
      pfVar3 = pfVar3 + 4;
    } while (iVar6 != 0);
  }
  fVar8 = 1.0 / (float)iVar9;
  local_20ac = local_20ac * fVar8;
  local_20a8 = local_20a8 * fVar8;
  local_20a4 = fVar8 * local_20a4;
  if (0 < iVar9) {
    pfVar3 = local_2008 + 1;
    iVar6 = iVar9;
    do {
      iVar6 = iVar6 + -1;
      pfVar3[-1] = pfVar3[-1] - local_20ac;
      *pfVar3 = *pfVar3 - local_20a8;
      pfVar3[1] = pfVar3[1] - local_20a4;
      pfVar3 = pfVar3 + 4;
    } while (iVar6 != 0);
  }
  local_209c[0] = 0.0;
  local_209c[1] = 0.0;
  local_208c = 0.0;
  local_209c[2] = 0.0;
  local_2088 = 0.0;
  local_207c = 0.0;
  if (0 < iVar9) {
    pfVar3 = local_2008 + 1;
    iVar6 = iVar9;
    do {
      fVar8 = pfVar3[-1];
      iVar6 = iVar6 + -1;
      local_209c[0] = fVar8 * fVar8 + local_209c[0];
      fVar1 = *pfVar3;
      local_209c[1] = fVar8 * fVar1 + local_209c[1];
      local_208c = fVar1 * fVar1 + local_208c;
      fVar7 = pfVar3[1];
      local_209c[2] = local_209c[2] + fVar7 * fVar8;
      local_2088 = fVar1 * fVar7 + local_2088;
      local_207c = fVar7 * fVar7 + local_207c;
      pfVar3 = pfVar3 + 4;
    } while (iVar6 != 0);
  }
  iVar6 = 0;
  do {
    FUN_1008b1c0(local_209c,local_203c + 4);
    FUN_1008b1c0(local_203c + 4,local_209c);
    local_2048 = (double)local_208c;
    local_2078 = (double)local_209c[0];
    local_2050 = (double)local_207c;
    uVar4 = extraout_ECX;
    fVar8 = extraout_EDX;
    if (ABS(local_208c + local_209c[0] + local_207c) == 0.0) goto LAB_1008c776;
    fVar10 = (float10)_CIsqrt();
    iVar6 = iVar6 + 1;
    local_207c = (float)_DAT_10099c40 * (1.0 / (float)fVar10) * (1.0 / (float)fVar10);
    local_209c[0] = (float)local_2078 * local_207c;
    local_209c[1] = local_209c[1] * local_207c;
    local_208c = (float)local_2048 * local_207c;
    local_209c[2] = local_209c[2] * local_207c;
    local_2088 = local_2088 * local_207c;
    local_207c = local_207c * (float)local_2050;
  } while (iVar6 < 9);
  local_20b8 = 0.0;
  uVar4 = 0;
  iVar6 = iVar9;
  if (0 < (int)local_209c[0]) {
    iVar6 = 0;
    local_20b8 = local_209c[0];
  }
  fVar8 = local_20b8;
  if ((int)local_20b8 < (int)local_208c) {
    iVar6 = 1;
    local_20b8 = local_208c;
  }
  if ((int)local_20b8 < (int)local_207c) {
    iVar6 = 2;
    local_20b8 = local_207c;
  }
  local_209c[3] = local_209c[1];
  fStack_2084 = local_209c[2];
  fStack_2080 = local_2088;
  if (_DAT_100a3ca4 <= ABS(local_20b8)) {
    fVar10 = (float10)_CIsqrt();
    fStack_2068 = 1.0 / (float)fVar10;
    fStack_2070 = local_209c[iVar6] * fStack_2068;
    fStack_206c = local_209c[iVar6 + 3] * fStack_2068;
    fStack_2068 = fStack_2068 * local_209c[iVar6 + 6];
    fVar8 = fStack_2068 * fStack_2068 + fStack_206c * fStack_206c + fStack_2070 * fStack_2070;
    if (fVar8 == 0.0) {
      FUN_1008b380(extraout_ECX_00,extraout_EDX_00,(int)param_1);
      return;
    }
    local_2078 = (double)CONCAT44(local_2078._4_4_,_DAT_10099c60);
    local_20b0 = _DAT_100a3ca0;
    fVar1 = (float)_DAT_10099d48;
    if (0 < iVar9) {
      iVar9 = *(int *)(param_1 + 1);
      pfVar3 = local_2008 + 1;
      fVar7 = _DAT_10099c60;
      do {
        fVar2 = (pfVar3[1] * fStack_2068 + *pfVar3 * fStack_206c + pfVar3[-1] * fStack_2070) *
                (1.0 / fVar8) + fVar1;
        if ((int)fVar2 < (int)fVar7) {
          fVar7 = fVar2;
        }
        if ((int)local_20b0 < (int)fVar2) {
          local_20b0 = fVar2;
        }
        pfVar3 = pfVar3 + 4;
        iVar9 = iVar9 + -1;
      } while (iVar9 != 0);
      local_2078 = (double)(ulonglong)(uint)fVar7;
    }
    fVar8 = local_2078._0_4_ - fVar1;
    local_20b0 = local_20b0 - fVar1;
    local_2050 = (double)local_20ac;
    fStack_2060 = fVar8 * fStack_2070 + local_20ac;
    local_2048 = (double)local_20a8;
    fStack_205c = fVar8 * fStack_206c + local_20a8;
    local_2078 = (double)local_20a4;
    fStack_2058 = fVar8 * fStack_2068 + local_20a4;
    local_20ac = local_20b0 * fStack_2070 + local_20ac;
    local_20a8 = local_20a8 + local_20b0 * fStack_206c;
    local_20a4 = local_20a4 + local_20b0 * fStack_2068;
    FUN_1008b420((int)param_1,&fStack_2060,&local_20ac);
    FUN_1008b930((uint)param_1,&fStack_2060,&local_20ac);
    fStack_2070 = local_20ac - fStack_2060;
    fStack_206c = local_20a8 - fStack_205c;
    fStack_2068 = local_20a4 - fStack_2058;
    fVar1 = fStack_2068 * fStack_2068 + fStack_206c * fStack_206c + fStack_2070 * fStack_2070;
    uVar4 = extraout_ECX_01;
    fVar8 = extraout_EDX_01;
    if (fVar1 != 0.0) {
      iVar9 = 0;
      if (*(int *)(param_1 + 1) < 1) {
        return;
      }
      pfVar3 = local_2008 + 1;
      fVar8 = (float)_DAT_100a3c98;
      pdVar5 = param_1 + 0x60a;
      do {
        local_20b8 = (pfVar3[1] * fStack_2068 + *pfVar3 * fStack_206c + pfVar3[-1] * fStack_2070 +
                     ((float)local_2078 - fStack_2058) * fStack_2068 +
                     ((float)local_2048 - fStack_205c) * fStack_206c +
                     ((float)local_2050 - fStack_2060) * fStack_2070) * (1.0 / fVar1);
        if ((int)local_20b8 < 0) {
          local_20b8 = 0.0;
        }
        else if (0x3f800000 < (int)local_20b8) {
          local_20b8 = 1.0;
        }
        iVar9 = iVar9 + 1;
        pfVar3 = pfVar3 + 4;
        *(float *)pdVar5 = local_203c[(uint)(local_20b8 * (float)_DAT_10099c40 + fVar8) & 0xffff];
        pdVar5 = (double *)((int)pdVar5 + 4);
      } while (iVar9 < *(int *)(param_1 + 1));
      return;
    }
  }
LAB_1008c776:
  FUN_1008b380(uVar4,fVar8,(int)param_1);
  return;
}

