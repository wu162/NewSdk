/* Original DLL evidence only; RVA 0x7d500; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Type propagation algorithm not settling */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

float10 FUN_1007d500(int param_1,int param_2,double *param_3,int *param_4,undefined4 *param_5)

{
  int iVar1;
  double dVar2;
  double dVar3;
  double dVar4;
  double dVar5;
  double dVar6;
  double dVar7;
  double dVar8;
  double dVar9;
  double dVar10;
  double dVar11;
  double dVar12;
  bool bVar13;
  double *pdVar14;
  int iVar15;
  int iVar16;
  int *piVar17;
  undefined4 extraout_ECX;
  int iVar18;
  undefined4 extraout_EDX;
  int iVar19;
  int iVar20;
  float10 fVar21;
  float10 fVar22;
  float10 fVar23;
  ulonglong uVar24;
  int local_11dc;
  int local_11d8;
  int local_11b4;
  double local_1198;
  undefined8 local_1190;
  int local_1188 [3];
  int local_117c;
  int local_1178 [4];
  double local_1168;
  double local_1160;
  double local_1158;
  double local_1150;
  double local_1148 [4];
  double local_1128;
  double local_1120;
  double local_1118;
  double local_1110;
  int local_1108 [16];
  int aiStack_10c8 [8];
  double local_10a8;
  double local_10a0;
  double local_1098;
  double local_1090;
  double local_1088;
  double local_1080 [8];
  double local_1040 [511];
  undefined4 uStack_44;
  
  uStack_44 = 0x1007d510;
  memset(local_1080 + 1,0,0x38);
  memset(local_1080 + 8,0,0x1000);
  local_1148[0] = 0.0;
  local_1148[1] = 0.0;
  local_1108[0] = 0;
  local_1108[1] = 0;
  local_1108[2] = 0;
  local_1108[3] = 0;
  local_1108[4] = 0;
  local_1108[5] = 0;
  local_1108[6] = 0;
  local_1108[7] = 0;
  memset(local_1108 + 8,0,0x40);
  local_1188[0] = 0;
  local_1188[1] = 0;
  local_1188[2] = 0;
  local_117c = 0;
  local_1178[0] = 0;
  local_1178[1] = 0;
  local_1178[2] = 0;
  local_1178[3] = 0;
  local_11b4 = 0x40;
  if (param_2 == 0) {
    return (float10)0;
  }
  local_1148[3] = (double)param_2;
  iVar18 = 1;
  local_1110 = 1.0 / (double)param_2;
  dVar2 = *param_3;
  *param_5 = 0;
  local_1128 = dVar2;
  dVar3 = dVar2;
  if (3 < param_2 + -1) {
    iVar19 = (param_2 - 5U >> 2) + 1;
    pdVar14 = param_3 + 3;
    iVar18 = iVar19 * 4 + 1;
    do {
      if (pdVar14[-2] < dVar3) {
        dVar3 = pdVar14[-2];
      }
      if (dVar2 < pdVar14[-2]) {
        dVar2 = pdVar14[-2];
      }
      dVar4 = pdVar14[-2];
      *param_5 = 0;
      if (pdVar14[-1] < dVar3) {
        dVar3 = pdVar14[-1];
      }
      if (dVar2 < pdVar14[-1]) {
        dVar2 = pdVar14[-1];
      }
      dVar5 = pdVar14[-1];
      *param_5 = 0;
      if (*pdVar14 < dVar3) {
        dVar3 = *pdVar14;
      }
      if (dVar2 < *pdVar14) {
        dVar2 = *pdVar14;
      }
      dVar6 = *pdVar14;
      *param_5 = 0;
      if (pdVar14[1] < dVar3) {
        dVar3 = pdVar14[1];
      }
      if (dVar2 < pdVar14[1]) {
        dVar2 = pdVar14[1];
      }
      local_1128 = local_1128 + dVar4 + dVar5 + dVar6 + pdVar14[1];
      pdVar14 = pdVar14 + 4;
      iVar19 = iVar19 + -1;
      *param_5 = 0;
    } while (iVar19 != 0);
  }
  for (; iVar18 < param_2; iVar18 = iVar18 + 1) {
    if (param_3[iVar18] < dVar3) {
      dVar3 = param_3[iVar18];
    }
    if (dVar2 < param_3[iVar18]) {
      dVar2 = param_3[iVar18];
    }
    dVar4 = param_3[iVar18];
    *param_5 = 0;
    local_1128 = local_1128 + dVar4;
  }
  local_1128 = local_1128 * local_1110;
  iVar18 = 0;
  if (3 < param_2) {
    pdVar14 = param_3 + 2;
    iVar19 = iVar18;
    do {
      iVar18 = iVar19 + 4;
      local_1080[iVar19 + 8] = pdVar14[-2] - local_1128;
      local_1080[iVar19 + 9] = pdVar14[-1] - local_1128;
      *(double *)((int)pdVar14 + (int)local_1080 + (0x40U - (int)param_3)) = *pdVar14 - local_1128;
      *(double *)((int)pdVar14 + (int)local_1080 + (0x48U - (int)param_3)) = pdVar14[1] - local_1128
      ;
      pdVar14 = pdVar14 + 4;
      iVar19 = iVar18;
    } while (iVar18 < param_2 + -3);
  }
  for (; iVar18 < param_2; iVar18 = iVar18 + 1) {
    local_1080[iVar18 + 8] = param_3[iVar18] - local_1128;
  }
  iVar18 = param_1 + -1;
  local_1148[0] = dVar3 - local_1128;
  local_1148[1] = dVar2 - local_1128;
  dVar3 = (double)param_2;
LAB_1007d814:
  do {
    dVar2 = 0.0;
    bVar13 = true;
    local_11d8 = 0;
    if (0 < iVar18) {
      piVar17 = local_1108;
      for (iVar19 = iVar18; iVar19 != 0; iVar19 = iVar19 + -1) {
        *piVar17 = 0;
        piVar17 = piVar17 + 1;
      }
      do {
        dVar4 = (double)local_11d8;
        local_11d8 = local_11d8 + 1;
        local_1080[local_11d8] =
             (dVar4 + _DAT_10098098) * ((local_1148[1] - local_1148[0]) / (double)iVar18) +
             local_1148[0];
      } while (local_11d8 < iVar18);
    }
    local_1108[param_1 + -1] = 0;
    if (0 < param_2) {
      pdVar14 = local_1080 + 8;
      iVar19 = 0;
      do {
        iVar16 = 0;
        if (3 < iVar18) {
          do {
            if (*pdVar14 < local_1080[iVar16 + 1]) goto LAB_1007d909;
            if (*pdVar14 < local_1080[iVar16 + 2]) {
              iVar16 = iVar16 + 1;
              goto LAB_1007d909;
            }
            if (*pdVar14 < local_1080[iVar16 + 3]) {
              iVar16 = iVar16 + 2;
              goto LAB_1007d909;
            }
            if (*pdVar14 < local_1080[iVar16 + 4]) {
              iVar16 = iVar16 + 3;
              goto LAB_1007d909;
            }
            iVar16 = iVar16 + 4;
          } while (iVar16 < param_1 + -4);
        }
        for (; (iVar16 < iVar18 && (local_1080[iVar16 + 1] <= *pdVar14)); iVar16 = iVar16 + 1) {
        }
LAB_1007d909:
        local_1108[iVar16] = local_1108[iVar16] + 1;
        iVar20 = iVar19 + 1;
        pdVar14 = pdVar14 + 1;
        bVar13 = (bool)(bVar13 & param_5[iVar19] == iVar16);
        param_5[iVar19] = iVar16;
        iVar19 = iVar20;
      } while (iVar20 < param_2);
    }
    iVar16 = 0;
    iVar19 = 0;
    local_11d8 = 0;
    if (0 < param_1) {
      do {
        piVar17 = local_1108 + iVar19;
        iVar19 = iVar19 + 1;
        iVar16 = iVar16 + (uint)(*piVar17 != 0);
      } while (iVar19 < param_1);
      local_11d8 = iVar16;
      if (iVar16 == 1) goto LAB_1007e1ae;
    }
    iVar19 = 0;
    local_1198 = 0.0;
    local_1088 = dVar2;
    if (3 < param_2) {
      iVar16 = iVar19;
      piVar17 = param_5 + 2;
      local_1198 = dVar2;
      do {
        dVar7 = (double)piVar17[-2];
        iVar19 = iVar16 + 4;
        dVar6 = (double)piVar17[-1];
        dVar4 = (double)*piVar17;
        dVar5 = (double)piVar17[1];
        local_1198 = (double)*piVar17 * local_1080[iVar16 + 10] +
                     local_1080[iVar16 + 9] * dVar6 + local_1080[iVar16 + 8] * dVar7 + local_1198 +
                     local_1080[iVar16 + 0xb] * dVar5;
        local_1088 = dVar7 + local_1088 + dVar6 + dVar4 + dVar5;
        dVar2 = dVar4 * dVar4 + dVar6 * dVar6 + dVar7 * dVar7 + dVar2 + dVar5 * dVar5;
        iVar16 = iVar19;
        piVar17 = piVar17 + 4;
      } while (iVar19 < param_2 + -3);
    }
    for (; iVar19 < param_2; iVar19 = iVar19 + 1) {
      dVar4 = (double)(int)param_5[iVar19];
      local_1198 = local_1080[iVar19 + 8] * dVar4 + local_1198;
      dVar2 = dVar4 * dVar4 + dVar2;
      local_1088 = dVar4 + local_1088;
    }
    local_1088 = local_1110 * local_1088;
    dVar2 = dVar2 - dVar3 * local_1088 * local_1088;
    dVar11 = -local_1088;
    dVar8 = (double)param_1 - _DAT_10098090;
    local_10a8 = dVar8;
    local_1118 = dVar8 - local_1088;
    local_1158 = dVar11 * dVar11 * dVar3 + dVar2;
    local_1168 = local_1118 * dVar11 * dVar3 + dVar2;
    dVar5 = local_1118 * dVar3 * local_1118 + dVar2;
    dVar7 = dVar5 * local_1158;
    local_1160 = dVar7 * dVar2 * dVar7 * dVar8 * dVar8;
    local_1090 = 1.0 - local_1128;
    dVar6 = (1.0 - local_1128) * dVar2;
    local_1148[2] = dVar11 * local_1198;
    local_1120 = local_1118 * local_1198;
    dVar9 = -(dVar2 * local_1128) - local_1148[2];
    dVar10 = dVar6 - local_1148[2];
    dVar4 = (-(dVar2 * local_1128) - local_1120) * local_1158;
    local_1190 = (dVar6 - local_1120) * local_1158;
    iVar19 = local_11b4 + -1;
    dVar6 = ((dVar10 - ABS(dVar10 - 0.0)) + ABS(dVar9 - 0.0) + dVar9) * _DAT_10098098 * local_1168;
    dVar6 = (ABS(dVar4 - dVar6) + dVar4 + (local_1190 - ABS(local_1190 - dVar6))) * _DAT_10098098;
    dVar4 = local_1168 * dVar6;
    dVar6 = dVar6 * dVar5;
    local_1150 = (ABS(dVar9 * dVar7 - dVar4) + dVar9 * dVar7 +
                 (dVar10 * dVar7 - ABS(dVar10 * dVar7 - dVar4))) * _DAT_10098098;
    dVar4 = dVar6 * dVar11 - local_1150 * local_1118;
    local_1080[0] =
         ((local_1150 - dVar6) * dVar2 * (local_1150 - dVar6) + dVar4 * dVar3 * dVar4) -
         local_1198 * local_1198 * local_1160;
    local_10a0 = local_1160 * dVar2;
    local_1148[0] = (local_1150 / dVar7 + local_1148[2]) / dVar2;
    local_1148[1] = (dVar6 / dVar7 + local_1120) / dVar2;
    if (local_11b4 == 0) goto LAB_1007e1ae;
    local_11b4 = iVar19;
  } while (!bVar13);
  if (0 < param_1) {
    piVar17 = local_1108 + 8;
    for (iVar19 = param_1; iVar19 != 0; iVar19 = iVar19 + -1) {
      *piVar17 = -1;
      piVar17 = piVar17 + 1;
    }
  }
  iVar19 = 0;
  if (3 < param_2) {
    iVar16 = 2;
    piVar17 = param_5 + 2;
    do {
      iVar20 = piVar17[-2];
      if (local_1108[iVar20 + 8] == -1) {
        aiStack_10c8[iVar20] = iVar19;
        local_1108[iVar20 + 8] = iVar19;
      }
      else {
        if (local_1080[iVar19 + 8] < local_1080[local_1108[iVar20 + 8] + 8]) {
          local_1108[iVar20 + 8] = iVar19;
        }
        if (local_1080[aiStack_10c8[iVar20] + 8] < local_1080[iVar19 + 8]) {
          aiStack_10c8[iVar20] = iVar19;
        }
      }
      iVar20 = piVar17[-1];
      if (local_1108[iVar20 + 8] == -1) {
        aiStack_10c8[iVar20] = iVar16 + -1;
        local_1108[iVar20 + 8] = iVar16 + -1;
      }
      else {
        if (local_1080[iVar19 + 9] < local_1080[local_1108[iVar20 + 8] + 8]) {
          local_1108[iVar20 + 8] = iVar16 + -1;
        }
        if (local_1080[aiStack_10c8[iVar20] + 8] < local_1080[iVar19 + 9]) {
          aiStack_10c8[iVar20] = iVar16 + -1;
        }
      }
      iVar20 = *piVar17;
      if (local_1108[iVar20 + 8] == -1) {
        aiStack_10c8[iVar20] = iVar16;
        local_1108[iVar20 + 8] = iVar16;
      }
      else {
        if (local_1080[iVar19 + 10] < local_1080[local_1108[iVar20 + 8] + 8]) {
          local_1108[iVar20 + 8] = iVar16;
        }
        if (local_1080[aiStack_10c8[iVar20] + 8] < local_1080[iVar19 + 10]) {
          aiStack_10c8[iVar20] = iVar16;
        }
      }
      iVar20 = piVar17[1];
      if (local_1108[iVar20 + 8] == -1) {
        aiStack_10c8[iVar20] = iVar16 + 1;
        local_1108[iVar20 + 8] = iVar16 + 1;
      }
      else {
        if (local_1080[iVar19 + 0xb] < local_1080[local_1108[iVar20 + 8] + 8]) {
          local_1108[iVar20 + 8] = iVar16 + 1;
        }
        if (local_1080[aiStack_10c8[iVar20] + 8] < local_1080[iVar19 + 0xb]) {
          aiStack_10c8[iVar20] = iVar16 + 1;
        }
      }
      iVar19 = iVar19 + 4;
      piVar17 = piVar17 + 4;
      iVar16 = iVar16 + 4;
    } while (iVar19 < param_2 + -3);
  }
  for (; iVar19 < param_2; iVar19 = iVar19 + 1) {
    iVar16 = param_5[iVar19];
    if (local_1108[iVar16 + 8] == -1) {
      aiStack_10c8[iVar16] = iVar19;
      local_1108[iVar16 + 8] = iVar19;
    }
    else {
      if (local_1080[iVar19 + 8] < local_1080[local_1108[iVar16 + 8] + 8]) {
        local_1108[iVar16 + 8] = iVar19;
      }
      if (local_1080[aiStack_10c8[iVar16] + 8] < local_1080[iVar19 + 8]) {
        aiStack_10c8[iVar16] = iVar19;
      }
    }
  }
  iVar19 = 0;
  local_1120 = dVar2 + _DAT_10098090;
  do {
    local_1190 = _DAT_100993f0;
    if (iVar19 == 0) {
      local_1190 = 1.0;
    }
    local_1098 = local_1110 * local_1190 + local_1088;
    local_1150 = local_1190 + local_1190;
    local_1148[2] = ((dVar2 + _DAT_10098090) - local_1150 * local_1088) - local_1110;
    for (iVar16 = iVar19; iVar16 < iVar18 + iVar19; iVar16 = iVar16 + 1) {
      if ((local_1108[iVar16] != 0) && ((1 < local_1108[iVar16] || (2 < local_11d8)))) {
        dVar10 = local_1080[aiStack_10c8[iVar16 + iVar19 * -8] + 8] * local_1190 + local_1198;
        dVar9 = local_1150 * (double)iVar16 + local_1148[2];
        dVar11 = -local_1098;
        dVar5 = dVar8 - local_1098;
        dVar4 = dVar11 * dVar3 * dVar11 + dVar9;
        local_1158 = dVar11 * dVar3 * dVar5 + dVar9;
        dVar12 = dVar5 * dVar3 * dVar5 + dVar9;
        dVar6 = dVar12 * dVar4;
        local_1118 = dVar6 * dVar9 * dVar6 * dVar8 * dVar8;
        dVar7 = -(local_1128 * dVar9) - dVar11 * dVar10;
        dVar8 = dVar9 * local_1090 - dVar11 * dVar10;
        local_1168 = (-(local_1128 * dVar9) - dVar5 * dVar10) * dVar4;
        dVar4 = (dVar9 * local_1090 - dVar5 * dVar10) * dVar4;
        dVar3 = ((dVar8 - ABS(dVar8 - 0.0)) + ABS(dVar7 - 0.0) + dVar7) * _DAT_10098098 * local_1158
        ;
        dVar4 = (ABS(local_1168 - dVar3) + local_1168 + (dVar4 - ABS(dVar4 - dVar3))) *
                _DAT_10098098;
        dVar3 = local_1158 * dVar4;
        dVar4 = dVar4 * dVar12;
        local_1160 = (ABS(dVar7 * dVar6 - dVar3) + dVar7 * dVar6 +
                     (dVar8 * dVar6 - ABS(dVar8 * dVar6 - dVar3))) * _DAT_10098098;
        dVar7 = dVar4 * dVar11 - local_1160 * dVar5;
        dVar3 = local_1148[3];
        dVar8 = local_10a8;
        if ((((local_1160 - dVar4) * dVar9 * (local_1160 - dVar4) + dVar7 * (double)param_2 * dVar7)
            - dVar10 * dVar10 * local_1118) * local_10a0 < local_1118 * dVar9 * local_1080[0]) {
          local_1148[0] = (dVar11 * dVar10 + local_1160 / dVar6) / dVar9;
          local_1148[1] = (dVar4 / dVar6 + dVar5 * dVar10) / dVar9;
          goto LAB_1007d814;
        }
      }
    }
    iVar19 = iVar19 + 1;
  } while (iVar19 < 2);
LAB_1007e1ae:
  iVar19 = 0;
  do {
    floor((local_1148[iVar19] + local_1128) * _DAT_10098000 + _DAT_10098098);
    uVar24 = FUN_1004e830(extraout_ECX,extraout_EDX);
    iVar16 = (int)uVar24;
    local_1188[iVar19] = iVar16;
    if (iVar16 < 0) {
      iVar16 = 0;
    }
    else if (0xff < iVar16) {
      iVar16 = 0xff;
    }
    local_1188[iVar19] = iVar16;
    iVar19 = iVar19 + 1;
  } while (iVar19 < 2);
  iVar19 = 2;
  if (2 < param_1) {
    iVar16 = param_1 + -2;
    iVar20 = iVar16 / 2;
    do {
      iVar1 = iVar19 + -1;
      iVar15 = local_1188[0] * iVar16;
      iVar19 = iVar19 + 1;
      iVar16 = iVar16 + -1;
      *(int *)((int)&local_1190 + iVar19 * 4 + 4) =
           (iVar15 + iVar1 * local_1188[1] + iVar20) / iVar18;
    } while (iVar19 < param_1);
  }
  fVar23 = (float10)0;
  *param_4 = local_1188[0];
  param_4[1] = local_1188[1];
  local_11d8 = 0;
  if (0 < param_2) {
    fVar21 = (float10)_DAT_10098000;
    do {
      iVar19 = 0;
      iVar16 = 1;
      if (3 < iVar18) {
        fVar22 = (float10)param_3[local_11d8] * fVar21;
        iVar20 = 0;
        local_11dc = 8;
        do {
          piVar17 = local_1188 + iVar16 + 1;
          if (ABS((float10)local_1188[iVar16] - fVar22) <
              ABS((float10)*(int *)((int)local_1188 + iVar20) - fVar22)) {
            iVar20 = iVar16 * 4;
            iVar19 = iVar16;
          }
          if (ABS((float10)*piVar17 - fVar22) <
              ABS((float10)*(int *)((int)local_1188 + iVar20) - fVar22)) {
            iVar19 = iVar16 + 1;
            iVar20 = local_11dc;
          }
          if (ABS((float10)local_1188[iVar16 + 2] - fVar22) <
              ABS((float10)*(int *)((int)local_1188 + iVar20) - fVar22)) {
            iVar19 = iVar16 + 2;
            iVar20 = (int)piVar17 + (0xc - (int)(local_1188 + 2));
          }
          if (ABS((float10)local_1188[iVar16 + 3] - fVar22) <
              ABS((float10)*(int *)((int)local_1188 + iVar20) - fVar22)) {
            iVar19 = iVar16 + 3;
            iVar20 = (0x10 - (int)(local_1188 + 2)) + (int)piVar17;
          }
          local_11dc = local_11dc + 0x10;
          iVar16 = iVar16 + 4;
        } while (iVar16 < param_1 + -3);
      }
      if (iVar16 < param_1) {
        iVar20 = iVar19 * 4;
        do {
          if (ABS((float10)local_1188[iVar16] - (float10)param_3[local_11d8] * fVar21) <
              ABS((float10)*(int *)((int)local_1188 + iVar20) -
                  (float10)param_3[local_11d8] * fVar21)) {
            iVar20 = iVar16 * 4;
            iVar19 = iVar16;
          }
          iVar16 = iVar16 + 1;
        } while (iVar16 < param_1);
      }
      iVar16 = local_1188[iVar19];
      param_5[local_11d8] = iVar19;
      pdVar14 = param_3 + local_11d8;
      local_11d8 = local_11d8 + 1;
      fVar22 = (float10)iVar16 - (float10)*pdVar14 * fVar21;
      fVar23 = fVar22 * fVar22 + fVar23;
    } while (local_11d8 < param_2);
  }
  return fVar23;
}

