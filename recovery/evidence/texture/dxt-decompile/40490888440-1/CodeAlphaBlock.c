/* Original DLL evidence only; RVA 0x7e8f0; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Type propagation algorithm not settling */

void __thiscall FUN_1007e8f0(void *this,float *param_1)

{
  int *piVar1;
  uint *puVar2;
  float fVar3;
  bool bVar4;
  undefined4 uVar5;
  uint uVar6;
  float *pfVar7;
  int iVar8;
  int iVar9;
  int iVar10;
  int iVar11;
  bool bVar12;
  float10 fVar13;
  double *local_78d4;
  float *local_78c8;
  int local_78c4 [10];
  int local_789c;
  undefined8 local_7898;
  undefined8 local_7890;
  undefined4 local_7888;
  undefined4 local_7884;
  undefined8 local_7880;
  undefined4 local_7878;
  undefined4 uStack_7874;
  float fStack_786c;
  int local_7868 [14];
  float local_7830 [5];
  int local_781c [5];
  float local_7808 [2558];
  double adStack_5010 [512];
  double adStack_4010 [512];
  double adStack_3010 [512];
  double adStack_2010 [513];
  double adStack_1008 [511];
  undefined4 uStack_c;
  
  uStack_c = 0x1007e900;
  if (param_1[0x206] == 9.52883e-44) {
    FUN_1008b570(this,(int *)param_1);
    return;
  }
  local_7830[1] = 8.40779e-45;
  local_7830[2] = 8.40779e-45;
  local_7830[3] = 8.40779e-45;
  local_7830[4] = 8.40779e-45;
  iVar10 = 0;
  local_7898._0_4_ = 0;
  local_7898._4_4_ = 0;
  local_7890._0_4_ = 0;
  local_7890._4_4_ = 0;
  local_7888 = 0;
  local_7884 = 0;
  local_7880._0_4_ = 0;
  local_7880._4_4_ = 0;
  local_7878 = 0;
  uStack_7874 = 0;
  local_7830[0] = 1.12104e-44;
  memset(adStack_5010 + 1,0,0x5000);
  memset(local_7808,0,0x2800);
  fStack_786c = 0.0;
  local_7868[0] = 0;
  local_7868[1] = 0;
  local_7868[2] = 0;
  local_7868[3] = 0;
  local_7868[4] = 0;
  local_7868[5] = 0;
  local_7868[6] = 0;
  local_7868[7] = 0;
  local_7868[8] = 0;
  iVar8 = 0;
  local_78c4[4] = 0;
  iVar9 = 0;
  iVar11 = 0;
  local_781c[0] = 0;
  local_781c[1] = 0;
  local_781c[2] = 1;
  local_781c[3] = 0;
  local_781c[4] = 1;
  local_7868[9] = 0;
  local_7868[10] = 0;
  local_7868[0xb] = 0;
  local_7868[0xc] = 1;
  local_7868[0xd] = 1;
  local_78c4[0] = 0;
  local_78c4[1] = 0;
  local_78c4[2] = 0;
  local_78c4[3] = 0;
  if (0 < (int)*param_1) {
    local_78d4 = (double *)*param_1;
    pfVar7 = param_1;
    do {
      pfVar7 = pfVar7 + 1;
      fVar3 = *pfVar7;
      iVar9 = iVar9 + 1;
      adStack_5010[iVar9] = (double)fVar3;
      iVar8 = iVar8 + 1;
      adStack_4010[iVar8] = (double)fVar3;
      if (*pfVar7 != 0.0) {
        iVar11 = iVar11 + 1;
        adStack_3010[iVar11] = (double)*pfVar7;
      }
      if (*pfVar7 != 1.0) {
        iVar10 = iVar10 + 1;
        adStack_2010[iVar10] = (double)*pfVar7;
      }
      if ((*pfVar7 != 0.0) && (*pfVar7 != 1.0)) {
        adStack_1008[local_78c4[4]] = (double)*pfVar7;
        local_78c4[4] = local_78c4[4] + 1;
      }
      local_78d4 = (double *)((int)local_78d4 + -1);
    } while (local_78d4 != (double *)0x0);
    local_78c4[3] = iVar10;
    local_78c4[2] = iVar11;
    local_78c4[1] = iVar8;
    local_78c4[0] = iVar9;
  }
  if ((param_1[0x201] == 0.0) || (local_789c = 1, local_78c4[0] == local_78c4[2])) {
    local_789c = 0;
  }
  if ((param_1[0x202] == 0.0) || (bVar4 = true, local_78c4[0] == local_78c4[3])) {
    bVar4 = false;
  }
  local_78c4[5] = 1;
  local_78c4[6] = 1;
  local_78c4[7] = (uint)(local_78c4[2] != local_78c4[1]);
  local_78c4[8] = (uint)(local_78c4[3] != local_78c4[1]);
  local_78d4 = adStack_5010 + 1;
  local_78c8 = local_7808;
  iVar9 = 0;
  iVar8 = 0;
  local_78c4[9] = (uint)(local_78c4[4] != local_78c4[1]);
  do {
    if (*(int *)((int)local_78c4 + iVar9 + 0x14) != 0) {
      piVar1 = (int *)((int)&fStack_786c + iVar8);
      fVar13 = FUN_1007d500(*(int *)((int)local_7830 + iVar9),*(int *)((int)local_78c4 + iVar9),
                            local_78d4,piVar1,local_78c8);
      *(double *)((int)&local_7898 + iVar8) = (double)fVar13;
      bVar12 = local_789c != 0;
      *(undefined4 *)((int)local_78c4 + iVar9 + 0x14) = 1;
      if ((bVar12) && (*(int *)((int)local_781c + iVar9) == 0)) {
        if ((*piVar1 == 0) || (*(int *)((int)local_7868 + iVar8) == 0)) {
          uVar5 = 1;
        }
        else {
          uVar5 = 0;
        }
        *(undefined4 *)((int)local_78c4 + iVar9 + 0x14) = uVar5;
      }
      if ((bVar4) && (*(int *)((int)local_7868 + iVar9 + 0x24) == 0)) {
        if ((*piVar1 == 1) || (*(int *)((int)local_7868 + iVar8) == 1)) {
          uVar6 = 1;
        }
        else {
          uVar6 = 0;
        }
        puVar2 = (uint *)((int)local_78c4 + iVar9 + 0x14);
        *puVar2 = *puVar2 & uVar6;
      }
    }
    local_78c8 = local_78c8 + 0x200;
    local_78d4 = local_78d4 + 0x200;
    iVar8 = iVar8 + 8;
    iVar9 = iVar9 + 4;
  } while (iVar8 < 0x28);
  iVar8 = 0;
  do {
    if (local_78c4[iVar8 + 5] != 0) break;
    iVar8 = iVar8 + 1;
  } while (iVar8 < 5);
  iVar9 = iVar8 + 1;
  if (3 < 5 - iVar9) {
    iVar10 = iVar8 * 8;
    iVar11 = iVar9 * 8 + 8;
    do {
      if ((local_78c4[iVar9 + 5] != 0) &&
         ((double)(&local_7898)[iVar9] < *(double *)((int)&local_7898 + iVar10))) {
        iVar10 = iVar9 * 8;
        iVar8 = iVar9;
      }
      if ((local_78c4[iVar9 + 6] != 0) &&
         ((double)(&local_7890)[iVar9] < *(double *)((int)&local_7898 + iVar10))) {
        iVar8 = iVar9 + 1;
        iVar10 = iVar11;
      }
      if ((local_78c4[iVar9 + 7] != 0) &&
         (*(double *)(&local_7888 + iVar9 * 2) < *(double *)((int)&local_7898 + iVar10))) {
        iVar8 = iVar9 + 2;
        iVar10 = (int)&local_7880 + (iVar9 * 8 - (int)&local_7890);
      }
      if ((local_78c4[iVar9 + 8] != 0) &&
         ((double)(&local_7880)[iVar9] < *(double *)((int)&local_7898 + iVar10))) {
        iVar8 = iVar9 + 3;
        iVar10 = (int)&local_7878 + (iVar9 * 8 - (int)&local_7890);
      }
      iVar11 = iVar11 + 0x20;
      iVar9 = iVar9 + 4;
    } while (iVar11 < 0x18);
  }
  if (iVar9 < 5) {
    iVar10 = iVar8 * 8;
    do {
      if ((local_78c4[iVar9 + 5] != 0) &&
         ((double)(&local_7898)[iVar9] < *(double *)((int)&local_7898 + iVar10))) {
        iVar8 = iVar9;
        iVar10 = iVar9 * 8;
      }
      iVar9 = iVar9 + 1;
    } while (iVar9 < 5);
  }
  fVar3 = local_7830[iVar8];
  param_1[0x204] = (&fStack_786c)[iVar8 * 2];
  param_1[0x203] = fVar3;
  iVar9 = 0;
  iVar10 = 0;
  param_1[0x205] = (&fStack_786c)[iVar8 * 2 + 1];
  if (0 < (int)*param_1) {
    pfVar7 = param_1 + 0x207;
    do {
      if ((pfVar7[-0x206] == 0.0) && (local_781c[iVar8] != 0)) {
        *pfVar7 = 8.40779e-45;
      }
      else if ((pfVar7[-0x206] == 1.0) && (local_7868[iVar8 + 9] != 0)) {
        *pfVar7 = 9.80909e-45;
      }
      else {
        *pfVar7 = local_7808[iVar8 * 0x200 + iVar10];
        iVar10 = iVar10 + 1;
      }
      iVar9 = iVar9 + 1;
      pfVar7 = pfVar7 + 1;
    } while (iVar9 < (int)*param_1);
  }
  return;
}

