/* Original DLL evidence only; RVA 0x5c310; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */

undefined4 __thiscall
FUN_1005c310(void *this,int *param_1,int *param_2,undefined4 param_3,int param_4,int param_5,
            int param_6)

{
  int iVar1;
  int iVar2;
  undefined4 *puVar3;
  float *pfVar4;
  int iVar5;
  uint uVar6;
  uint uVar7;
  int iVar8;
  bool bVar9;
  float10 fVar10;
  int local_185c;
  uint local_1858;
  int local_1854;
  int local_1850;
  float local_1838 [4];
  float local_1828;
  float afStack_1824 [512];
  undefined4 local_1024;
  undefined4 local_1020;
  int local_101c;
  int local_1018;
  int local_1014;
  undefined4 local_1010;
  uint auStack_100c [513];
  float local_808 [511];
  undefined4 uStack_c;
  
  uStack_c = 0x1005c320;
  pfVar4 = local_808;
  for (iVar5 = 0x200; iVar5 != 0; iVar5 = iVar5 + -1) {
    *pfVar4 = -NAN;
    pfVar4 = pfVar4 + 1;
  }
  iVar5 = 0;
  local_1010 = 0x47;
  local_185c = 0;
  if (*(char *)(*(int *)((int)this + 0x18) + 0x1bc) != '\0') {
    iVar5 = *(int *)(*(int *)((int)this + 0x18) + 0x1c0);
  }
  uVar6 = -iVar5;
  local_1828 = 0.0;
  iVar1 = iVar5 + 4;
  if ((int)uVar6 < iVar1) {
    local_1854 = (uVar6 + param_5) * 4;
    local_1858 = uVar6;
    do {
      if ((int)uVar6 < iVar1) {
        iVar8 = (uVar6 + param_4) * 0x10;
        local_1850 = iVar1 + iVar5;
        uVar7 = uVar6;
        do {
          bVar9 = false;
          if ((int)(local_1858 + param_5) < 0) {
            bVar9 = true;
          }
          else if ((int)(uVar7 + param_4) < 0) {
            bVar9 = true;
          }
          else {
            iVar2 = *param_1;
            if ((int)(local_1858 + param_5) < *(int *)(iVar2 + 8)) {
              if ((int)(uVar7 + param_4) < *(int *)(iVar2 + 0xc)) {
                iVar2 = *(int *)(**(int **)(iVar2 + 0x24) + local_1854);
                pfVar4 = (float *)(iVar2 + iVar8);
                local_1838[3] = (float)*(undefined4 *)(iVar2 + 0xc + iVar8);
                local_1838[0] = *pfVar4;
                local_1838[1] = pfVar4[1];
                local_1838[2] = pfVar4[2];
              }
              else {
                bVar9 = true;
              }
            }
            else {
              bVar9 = true;
            }
          }
          if (*(char *)(*(int *)((int)this + 0x18) + 0x661) == '\0') {
            fVar10 = (float10)local_1838[param_6];
          }
          else {
            fVar10 = FUN_1005c140(local_1838);
          }
          if (bVar9) {
            if ((local_1858 < 4) && (uVar7 < 4)) {
              local_808[local_185c] = -NAN;
              local_185c = local_185c + 1;
            }
          }
          else {
            if ((local_1858 < 4) && (uVar7 < 4)) {
              local_808[local_185c] = local_1828;
              local_185c = local_185c + 1;
            }
            afStack_1824[(int)local_1828] = (float)fVar10;
            local_1828 = (float)((int)local_1828 + 1);
          }
          iVar8 = iVar8 + 0x10;
          uVar7 = uVar7 + 1;
          local_1850 = local_1850 + -1;
        } while (local_1850 != 0);
      }
      local_1854 = local_1854 + 4;
      local_1858 = local_1858 + 1;
    } while ((int)local_1858 < iVar1);
  }
  local_1024 = 0;
  local_1020 = 0;
  if (local_1828 == 0.0) {
    local_1014 = 0;
    local_1018 = 0;
LAB_1005c523:
    iVar5 = 0;
    local_1014 = local_1014 + 1;
    if (0 < (int)local_1828) {
      do {
        auStack_100c[iVar5] = 0;
        iVar5 = iVar5 + 1;
      } while (iVar5 < (int)local_1828);
    }
    local_101c = 6;
    iVar5 = local_1018;
  }
  else {
    local_1010 = **(undefined4 **)((int)this + 0x18);
    FUN_1007e8f0(this,&local_1828);
    iVar5 = local_1018;
    if (local_1018 == local_1014) goto LAB_1005c523;
  }
  iVar1 = local_1014;
  bVar9 = local_1014 < iVar5 == (local_101c == 6);
  if (bVar9) {
    local_1018 = local_1014;
    local_1014 = iVar5;
    iVar5 = iVar1;
  }
  if (iVar5 < 0x100) {
    if (iVar5 < 0) {
      iVar5 = 0;
      goto LAB_1005c5b4;
    }
  }
  else {
    iVar5 = 0xff;
LAB_1005c5b4:
    local_1018 = iVar5;
  }
  *(char *)*param_2 = (char)iVar5;
  *param_2 = *param_2 + 1;
  if (local_1014 < 0x100) {
    if (-1 < local_1014) goto LAB_1005c5e9;
    local_1014 = 0;
  }
  else {
    local_1014 = 0xff;
  }
LAB_1005c5e9:
  *(char *)*param_2 = (char)local_1014;
  *param_2 = *param_2 + 1;
  puVar3 = (undefined4 *)*param_2;
  uVar6 = 0;
  *puVar3 = 0;
  *(undefined2 *)(puVar3 + 1) = 0;
  local_1858 = 0;
  pfVar4 = local_808 + 1;
  do {
    uVar6 = uVar6 >> 3;
    if (-1 < (int)pfVar4[-1]) {
      uVar7 = auStack_100c[(int)pfVar4[-1]];
      if (bVar9) {
        if (local_101c == 8) {
          if ((int)uVar7 < 2) {
LAB_1005c64d:
            uVar7 = (uint)(uVar7 == 0);
          }
          else {
            uVar7 = 9 - uVar7;
          }
        }
        else if ((local_101c == 6) && ((int)uVar7 < 6)) {
          if ((int)uVar7 < 2) goto LAB_1005c64d;
          uVar7 = 7 - uVar7;
        }
      }
      uVar6 = uVar6 | uVar7 << 0x15;
    }
    uVar6 = uVar6 >> 3;
    if (-1 < (int)*pfVar4) {
      uVar7 = auStack_100c[(int)*pfVar4];
      if (bVar9) {
        if (local_101c == 8) {
          if ((int)uVar7 < 2) {
LAB_1005c69d:
            uVar7 = (uint)(uVar7 == 0);
          }
          else {
            uVar7 = 9 - uVar7;
          }
        }
        else if ((local_101c == 6) && ((int)uVar7 < 6)) {
          if ((int)uVar7 < 2) goto LAB_1005c69d;
          uVar7 = 7 - uVar7;
        }
      }
      uVar6 = uVar6 | uVar7 << 0x15;
    }
    uVar6 = uVar6 >> 3;
    if (-1 < (int)pfVar4[1]) {
      uVar7 = auStack_100c[(int)pfVar4[1]];
      if (bVar9) {
        if (local_101c == 8) {
          if ((int)uVar7 < 2) {
LAB_1005c6ee:
            uVar7 = (uint)(uVar7 == 0);
          }
          else {
            uVar7 = 9 - uVar7;
          }
        }
        else if ((local_101c == 6) && ((int)uVar7 < 6)) {
          if ((int)uVar7 < 2) goto LAB_1005c6ee;
          uVar7 = 7 - uVar7;
        }
      }
      uVar6 = uVar6 | uVar7 << 0x15;
    }
    uVar6 = uVar6 >> 3;
    if (-1 < (int)pfVar4[2]) {
      uVar7 = auStack_100c[(int)pfVar4[2]];
      if (bVar9) {
        if (local_101c == 8) {
          if ((int)uVar7 < 2) {
LAB_1005c73f:
            uVar7 = (uint)(uVar7 == 0);
          }
          else {
            uVar7 = 9 - uVar7;
          }
        }
        else if ((local_101c == 6) && ((int)uVar7 < 6)) {
          if ((int)uVar7 < 2) goto LAB_1005c73f;
          uVar7 = 7 - uVar7;
        }
      }
      uVar6 = uVar6 | uVar7 << 0x15;
    }
    pfVar4 = pfVar4 + 4;
    if ((local_1858 & 1) != 0) {
      *(uint *)*param_2 = *(uint *)*param_2 | uVar6;
      *param_2 = *param_2 + 3;
      uVar6 = 0;
    }
    local_1858 = local_1858 + 1;
    if (3 < (int)local_1858) {
      return 0;
    }
  } while( true );
}

