/* Original DLL evidence only; RVA 0x603d0; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

float10 FUN_100603d0(int *param_1,int param_2,int param_3,int param_4,int param_5,double *param_6,
                    double *param_7)

{
  double dVar1;
  double dVar2;
  double dVar3;
  double dVar4;
  double dVar5;
  double dVar6;
  double dVar7;
  int iVar8;
  int iVar9;
  int iVar10;
  double *pdVar11;
  char cVar12;
  int iVar13;
  int iVar14;
  double *pdVar15;
  double *pdVar16;
  float10 fVar17;
  float10 fVar18;
  float10 fVar19;
  float10 fVar20;
  float10 fVar21;
  float10 fVar22;
  float10 fVar23;
  int local_40b0;
  double *local_40ac;
  int local_40a8 [4];
  double local_4098;
  double local_4090;
  double local_4088;
  double local_4080;
  double local_4070;
  double local_4068;
  double local_4060;
  double local_4058;
  double local_4050;
  double local_4048;
  double local_4028 [4];
  double local_4008 [512];
  undefined1 local_3008 [8];
  double local_3000 [1534];
  undefined4 uStack_c;
  
  uStack_c = 0x100603e0;
  local_4098 = 0.0;
  local_4090 = 1.0;
  memset(local_3008,0,0x3000);
  memset(local_4028 + 4,0,0x1000);
  if (0 < param_2) {
    iVar13 = 0;
    pdVar11 = local_3000;
    do {
      iVar14 = *(int *)(param_5 + iVar13 * 4);
      iVar10 = iVar13 + 1;
      pdVar11[-1] = *(double *)(param_3 + iVar14 * 0x18);
      *pdVar11 = *(double *)(param_3 + 8 + iVar14 * 0x18);
      pdVar11[1] = *(double *)(param_3 + 0x10 + iVar14 * 0x18);
      local_4028[iVar13 + 4] = *(double *)(param_4 + iVar14 * 8);
      iVar13 = iVar10;
      pdVar11 = pdVar11 + 3;
    } while (iVar10 < param_2);
  }
  local_4088 = 0.0;
  pdVar11 = local_3000 + param_2 * 3;
  iVar13 = param_2;
  iVar14 = param_2;
  pdVar15 = pdVar11;
  pdVar16 = pdVar11;
  iVar10 = param_2;
  local_40ac = pdVar11;
  local_4080 = local_4088;
  local_4070 = local_4088;
  local_4068 = local_4088;
  local_4060 = local_4088;
  local_4058 = local_4088;
  local_4050 = local_4088;
  local_4048 = local_4088;
  local_4028[0] = local_4088;
  local_4028[1] = local_4088;
  dVar1 = local_4088;
  dVar2 = local_4088;
  dVar3 = local_4088;
  dVar4 = local_4088;
  dVar5 = local_4088;
  while( true ) {
    while( true ) {
      while( true ) {
        dVar6 = (-(dVar5 * dVar5) - dVar1 * dVar1) - dVar2 * dVar2;
        dVar7 = dVar6 * local_4090;
        if (dVar7 < dVar3 * local_4098 != (dVar7 == dVar3 * local_4098)) {
          local_40a8[0] = iVar10;
          local_40a8[1] = iVar14;
          local_40a8[2] = iVar13;
          local_4098 = dVar6;
          local_4090 = dVar3;
        }
        if ((iVar13 + -1 < iVar14) || (*param_1 < 3)) break;
        dVar5 = pdVar16[-4] + dVar5;
        dVar1 = pdVar16[-3] + dVar1;
        dVar2 = pdVar16[-2] + dVar2;
        dVar6 = dVar4 + dVar4;
        iVar8 = iVar13 + 3;
        iVar9 = iVar13 + 3;
        dVar4 = dVar4 + local_4028[iVar13 + 3];
        iVar13 = iVar13 + -1;
        pdVar16 = pdVar16 + -3;
        dVar3 = ((_DAT_10099ff8 - dVar6) - local_4028[iVar8]) * local_4028[iVar9] + dVar3;
      }
      if (iVar14 + -1 < iVar10) break;
      local_4058 = pdVar15[-4] + local_4058;
      local_4050 = pdVar15[-3] + local_4050;
      local_4048 = pdVar15[-2] + local_4048;
      local_4080 = ((_DAT_10099c40 - (local_4028[1] + local_4028[1])) - local_4028[iVar14 + 3]) *
                   local_4028[iVar14 + 3] + local_4080;
      local_4028[1] = local_4028[iVar14 + 3] + local_4028[1];
      iVar13 = param_2;
      iVar14 = iVar14 + -1;
      pdVar15 = pdVar15 + -3;
      pdVar16 = pdVar11;
      dVar1 = local_4050;
      dVar2 = local_4048;
      dVar3 = local_4080;
      dVar4 = local_4028[1];
      dVar5 = local_4058;
    }
    if (iVar10 + -1 < 1) break;
    local_4070 = local_40ac[-4] + local_4070;
    local_4068 = local_4068 + local_40ac[-3];
    local_4060 = local_40ac[-2] + local_4060;
    local_4088 = ((1.0 - (local_4028[0] + local_4028[0])) - local_4028[iVar10 + 3]) *
                 local_4028[iVar10 + 3] + local_4088;
    local_4028[0] = local_4028[0] + local_4028[iVar10 + 3];
    iVar13 = param_2;
    iVar14 = param_2;
    pdVar15 = pdVar11;
    pdVar16 = pdVar11;
    iVar10 = iVar10 + -1;
    local_40ac = local_40ac + -3;
    local_4080 = local_4088;
    local_4058 = local_4070;
    local_4050 = local_4068;
    local_4048 = local_4060;
    local_4028[1] = local_4028[0];
    dVar1 = local_4068;
    dVar2 = local_4060;
    dVar3 = local_4088;
    dVar4 = local_4028[0];
    dVar5 = local_4070;
  }
  local_40a8[3] = param_2;
  cVar12 = (local_40a8[1] != local_40a8[2]) + (local_40a8[0] != local_40a8[1]) +
           (local_40a8[2] != param_2);
  if ((cVar12 == '\x01') && (local_40a8[2] != param_2)) {
    if (local_40a8[1] == local_40a8[2]) {
      local_40a8[2] = param_2;
    }
    else if (local_40a8[0] == local_40a8[1]) {
      local_40a8[3] = local_40a8[0];
      local_40a8[2] = local_40a8[0];
    }
  }
  fVar19 = (float10)0;
  iVar13 = 0;
  local_40ac = (double *)0x0;
  local_40b0 = 0;
  fVar20 = fVar19;
  fVar21 = fVar19;
  fVar22 = fVar19;
  fVar23 = fVar19;
  do {
    iVar14 = local_40a8[(int)local_40ac];
    if (3 < iVar14 - iVar13) {
      fVar17 = (float10)(int)local_40ac;
      fVar18 = fVar17 * fVar17;
      pdVar11 = local_3000 + iVar13 * 3;
      iVar10 = iVar13;
      do {
        iVar13 = iVar10 + 4;
        fVar21 = (float10)local_4028[iVar10 + 7] * fVar17 +
                 fVar17 * (float10)local_4028[iVar10 + 6] +
                 fVar17 * (float10)local_4028[iVar10 + 5] +
                 fVar17 * (float10)local_4028[iVar13] + fVar21;
        fVar22 = fVar18 * (float10)local_4028[iVar10 + 7] +
                 fVar18 * (float10)local_4028[iVar10 + 6] +
                 fVar18 * (float10)local_4028[iVar10 + 5] +
                 fVar18 * (float10)local_4028[iVar13] + fVar22;
        fVar23 = (float10)pdVar11[8] * fVar17 +
                 (float10)pdVar11[5] * fVar17 +
                 (float10)pdVar11[2] * fVar17 + (float10)pdVar11[-1] * fVar17 + fVar23;
        fVar19 = (float10)pdVar11[9] * fVar17 +
                 (float10)pdVar11[6] * fVar17 +
                 (float10)pdVar11[3] * fVar17 + (float10)*pdVar11 * fVar17 + fVar19;
        fVar20 = (float10)pdVar11[10] * fVar17 +
                 (float10)pdVar11[7] * fVar17 +
                 (float10)pdVar11[4] * fVar17 + (float10)pdVar11[1] * fVar17 + fVar20;
        pdVar11 = pdVar11 + 0xc;
        iVar10 = iVar13;
      } while (iVar13 < iVar14 + -3);
    }
    if (iVar13 < iVar14) {
      fVar17 = (float10)(int)local_40ac;
      pdVar11 = local_3000 + iVar13 * 3;
      iVar10 = iVar13;
      do {
        iVar13 = iVar10 + 1;
        fVar21 = (float10)local_4028[iVar10 + 4] * fVar17 + fVar21;
        fVar22 = (float10)local_4028[iVar10 + 4] * fVar17 * fVar17 + fVar22;
        fVar23 = (float10)pdVar11[-1] * fVar17 + fVar23;
        fVar19 = fVar17 * (float10)*pdVar11 + fVar19;
        fVar20 = (float10)pdVar11[1] * fVar17 + fVar20;
        pdVar11 = pdVar11 + 3;
        iVar10 = iVar13;
      } while (iVar13 < iVar14);
    }
    local_40ac = (double *)((int)local_40ac + 1);
    local_40b0 = local_40b0 + (uint)(iVar14 != param_2);
  } while ((int)local_40ac < 4);
  fVar22 = fVar22 - fVar21 * fVar21;
  fVar17 = -fVar21;
  fVar21 = (float10)local_40b0 - fVar21;
  *param_6 = (double)((fVar17 * fVar23) / fVar22);
  param_6[3] = (double)((fVar21 * fVar23) / fVar22);
  *param_7 = (double)fVar23;
  param_6[1] = (double)((fVar17 * fVar19) / fVar22);
  param_6[4] = (double)((fVar21 * fVar19) / fVar22);
  param_7[1] = (double)fVar19;
  param_6[2] = (double)((fVar17 * fVar20) / fVar22);
  param_6[5] = (double)((fVar21 * fVar20) / fVar22);
  param_7[2] = (double)fVar20;
  iVar13 = 1;
  if (cVar12 != '\x01') {
    iVar13 = local_40b0;
  }
  *param_1 = iVar13;
  return ((-(fVar23 * fVar23) - fVar19 * fVar19) - fVar20 * fVar20) / fVar22;
}

