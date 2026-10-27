/* Original DLL evidence only; RVA 0x5c790; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 __thiscall
FUN_1005c790(void *this,int param_1,int param_2,uint *param_3,int param_4,undefined4 param_5)

{
  float fVar1;
  int iVar2;
  undefined4 *puVar3;
  double dVar4;
  double dVar5;
  double dVar6;
  ushort uVar7;
  ushort uVar8;
  undefined4 uVar9;
  float *pfVar10;
  uint *puVar11;
  uint uVar12;
  ushort uVar13;
  int iVar14;
  int iVar15;
  int iVar16;
  double *pdVar17;
  uint uVar18;
  int *piVar19;
  uint extraout_EDX;
  void *pvVar20;
  bool bVar21;
  float10 fVar22;
  float10 fVar23;
  float10 fVar24;
  ulonglong uVar25;
  int local_40cc;
  uint local_40c8;
  uint *local_40c4;
  int local_40c0;
  int local_40bc;
  uint local_40b8;
  uint local_40b4;
  int local_40b0;
  void *local_40ac;
  int local_40a8;
  int local_40a4;
  float local_40a0;
  float local_409c;
  float local_4098;
  undefined4 local_4094;
  float local_4090;
  uint local_408c;
  float local_4088;
  float local_4084;
  float local_4080;
  float local_407c;
  float local_4078;
  float local_4074;
  float local_4070;
  undefined4 local_406c;
  float local_4068;
  float local_4064;
  float local_4060;
  undefined4 local_405c;
  int local_4058 [4];
  int local_4048;
  int local_4044;
  int local_4040;
  int local_403c;
  int local_4038;
  int local_4034;
  int local_4030;
  int local_402c;
  int local_4028 [500];
  int local_3858 [4];
  double local_3848 [1536];
  undefined8 local_848;
  double local_840;
  double local_838;
  double local_830;
  undefined4 local_828;
  int local_824;
  int local_820;
  uint local_81c [2];
  int local_814;
  uint local_810;
  undefined4 local_80c;
  uint auStack_808 [511];
  undefined4 uStack_c;
  
  uStack_c = 0x1005c7a0;
  iVar14 = *(int *)((int)this + 0x18);
  local_40a8 = 0;
  local_40ac = this;
  if (*(char *)(iVar14 + 0x662) != '\0') {
    uVar9 = FUN_1008a2a0(this,param_1,param_2,param_3,param_4,param_5);
    return uVar9;
  }
  if (*(char *)(iVar14 + 0x1bc) != '\0') {
    local_40a8 = *(int *)(iVar14 + 0x1c0);
  }
  *(undefined4 *)(param_2 + 4) = 0x81806;
  *(undefined4 *)(param_2 + 0xc) = *(undefined4 *)(param_1 + 0xc);
  *(undefined4 *)(param_2 + 8) = *(undefined4 *)(param_1 + 8);
  *(undefined4 *)(param_2 + 0x10) = param_5;
  *(uint **)(param_2 + 0x24) = param_3;
  *(undefined4 *)(param_2 + 0x48) = 0x20;
  *(uint *)(param_2 + 0x4c) = *(uint *)(param_1 + 0x4c) & 1 | 4;
  *(undefined4 *)(param_2 + 0x54) = 0;
  *(undefined4 *)(param_2 + 0x50) = 0x12345678;
  local_4090 = *(float *)(*(int *)((int)this + 0x18) + 0x74);
  local_40c4 = param_3;
  local_40c8 = 0;
  if (0 < *(int *)(param_1 + 8)) {
    do {
      local_40c0 = 0;
      if (0 < *(int *)(param_1 + 0xc)) {
        do {
          pvVar20 = local_40ac;
          iVar16 = local_40c0;
          iVar14 = *(int *)((int)local_40ac + 0x18);
          local_848 = _DAT_10098338;
          if (*(char *)(iVar14 + 0x1bc) != '\0') {
            local_848 = _DAT_100a3980;
          }
          local_3858[0] = param_4;
          local_840 = (double)*(float *)(iVar14 + 0xc);
          local_838 = (double)*(float *)(iVar14 + 0x10);
          fVar1 = *(float *)(iVar14 + 0x14);
          piVar19 = local_4058;
          for (iVar14 = 0x200; iVar14 != 0; iVar14 = iVar14 + -1) {
            *piVar19 = -1;
            piVar19 = piVar19 + 1;
          }
          local_830 = (double)fVar1;
          local_408c = 0;
          local_3858[2] = 0;
          switch(param_4) {
          case 0:
          case 2:
          case 3:
          case 0x18:
            local_40b8 = -local_40a8;
            local_40cc = 0;
            local_40b4 = local_40b8;
            if ((int)local_40b8 < local_40a8 + 4) {
              local_40bc = (local_40b8 + local_40c8) * 4;
              do {
                if ((int)local_40b8 < local_40a8 + 4) {
                  local_40a4 = local_40b4 + local_40c8;
                  iVar16 = (local_40b8 + local_40c0) * 0x10;
                  local_40b0 = (local_40a8 + 4) - local_40b8;
                  iVar14 = param_1;
                  uVar12 = local_40b8;
                  do {
                    bVar21 = false;
                    if (local_40a4 < 0) {
                      bVar21 = true;
                    }
                    else if ((int)(local_40c0 + uVar12) < 0) {
                      bVar21 = true;
                    }
                    else if (local_40a4 < *(int *)(iVar14 + 8)) {
                      if ((int)(local_40c0 + uVar12) < *(int *)(iVar14 + 0xc)) {
                        iVar15 = *(int *)(**(int **)(iVar14 + 0x24) + local_40bc);
                        local_4094 = *(undefined4 *)(iVar15 + 0xc + iVar16);
                        pfVar10 = (float *)(iVar15 + iVar16);
                        local_40a0 = *pfVar10;
                        local_409c = pfVar10[1];
                        local_4098 = pfVar10[2];
                      }
                      else {
                        bVar21 = true;
                      }
                    }
                    else {
                      bVar21 = true;
                    }
                    if (*(char *)(*(int *)((int)local_40ac + 0x18) + 0x661) != '\0') {
                      FUN_1005bf90(&local_4068,&local_40a0);
                      local_4094 = local_405c;
                      local_40a0 = local_4068;
                      local_409c = local_4064;
                      local_4098 = local_4060;
                      iVar14 = param_1;
                    }
                    iVar15 = local_3858[2];
                    if (bVar21) {
                      if ((local_40b4 < 4) && (uVar12 < 4)) {
                        local_4058[local_40cc] = -1;
                        local_40cc = local_40cc + 1;
                      }
                    }
                    else {
                      if ((local_40b4 < 4) && (uVar12 < 4)) {
                        local_4058[local_40cc] = local_3858[2];
                        local_40cc = local_40cc + 1;
                      }
                      iVar2 = local_3858[2] * 3;
                      local_3858[2] = local_3858[2] + 1;
                      if (param_4 == 0x18) {
                        local_3848[iVar2] = (double)local_409c;
                        local_3848[iVar15 * 3 + 1] = (double)local_409c;
                        local_3848[iVar15 * 3 + 2] = (double)local_409c;
                      }
                      else {
                        local_3848[iVar2] = (double)local_40a0;
                        local_3848[iVar15 * 3 + 1] = (double)local_409c;
                        local_3848[iVar15 * 3 + 2] = (double)local_4098;
                      }
                    }
                    iVar16 = iVar16 + 0x10;
                    uVar12 = uVar12 + 1;
                    local_40b0 = local_40b0 + -1;
                  } while (local_40b0 != 0);
                }
                local_40bc = local_40bc + 4;
                local_40b4 = local_40b4 + 1;
              } while ((int)local_40b4 < local_40a8 + 4);
            }
            break;
          case 1:
            local_40b8 = -local_40a8;
            local_40cc = 0;
            if ((int)local_40b8 < local_40a8 + 4) {
              local_40bc = (local_40c8 + local_40b8) * 4;
              uVar12 = local_40b8;
              do {
                if ((int)local_40b8 < local_40a8 + 4) {
                  iVar14 = (local_40b8 + local_40c0) * 0x10;
                  local_40b0 = local_40a8 + 4 + local_40a8;
                  uVar18 = local_40b8;
                  do {
                    iVar16 = local_3858[2];
                    if (((((int)(uVar12 + local_40c8) < 0) || ((int)(local_40c0 + uVar18) < 0)) ||
                        (*(int *)(param_1 + 8) <= (int)(uVar12 + local_40c8))) ||
                       (*(int *)(param_1 + 0xc) <= (int)(local_40c0 + uVar18))) {
LAB_1005cc3c:
                      if ((uVar12 < 4) && (uVar18 < 4)) {
                        local_4058[local_40cc] = -1;
                        local_40cc = local_40cc + 1;
                      }
                    }
                    else {
                      iVar15 = *(int *)(**(int **)(param_1 + 0x24) + local_40bc);
                      local_407c = *(float *)(iVar15 + 0xc + iVar14);
                      pfVar10 = (float *)(iVar15 + iVar14);
                      local_4088 = *pfVar10;
                      local_4084 = pfVar10[1];
                      local_4080 = pfVar10[2];
                      if (local_407c <= local_4090) {
                        local_3858[1] = 0;
                        goto LAB_1005cc3c;
                      }
                      if ((uVar12 < 4) && (uVar18 < 4)) {
                        local_4058[local_40cc] = local_3858[2];
                        local_40cc = local_40cc + 1;
                      }
                      iVar15 = local_3858[2] * 3;
                      local_3858[2] = local_3858[2] + 1;
                      local_3848[iVar15] = (double)local_4088;
                      local_3848[iVar16 * 3 + 1] = (double)local_4084;
                      local_3848[iVar16 * 3 + 2] = (double)local_4080;
                    }
                    iVar14 = iVar14 + 0x10;
                    uVar18 = uVar18 + 1;
                    local_40b0 = local_40b0 + -1;
                  } while (local_40b0 != 0);
                  local_40b0 = 0;
                }
                local_40bc = local_40bc + 4;
                uVar12 = uVar12 + 1;
              } while ((int)uVar12 < local_40a8 + 4);
            }
            break;
          case 0x1d:
            FUN_1005c310(local_40ac,&param_1,(int *)&local_40c4,0x1d,local_40c0,local_40c8,1);
            FUN_1005c310(pvVar20,&param_1,(int *)&local_40c4,0x1d,iVar16,local_40c8,0);
          }
          pvVar20 = local_40ac;
          iVar14 = param_4;
          switch(param_4) {
          case 2:
            local_40a4 = local_40c0 << 4;
            local_40b4 = local_40c8 * 4;
            local_40bc = 0;
            do {
              iVar15 = local_40bc + local_40c8;
              local_40b8 = 0;
              local_40b0 = 4;
              uVar12 = local_40c8;
              iVar14 = local_40c0;
              iVar16 = local_40a4;
              do {
                bVar21 = false;
                if (iVar15 < 0) {
                  bVar21 = true;
                }
                else if (iVar14 < 0) {
                  bVar21 = true;
                }
                else if (iVar15 < *(int *)(param_1 + 8)) {
                  if (iVar14 < *(int *)(param_1 + 0xc)) {
                    iVar2 = *(int *)(**(int **)(param_1 + 0x24) + local_40b4);
                    local_406c = *(undefined4 *)(iVar2 + 0xc + iVar16);
                    pfVar10 = (float *)(iVar2 + iVar16);
                    local_4078 = *pfVar10;
                    local_4074 = pfVar10[1];
                    local_4070 = pfVar10[2];
                    uVar12 = local_40b4;
                  }
                  else {
                    bVar21 = true;
                  }
                }
                else {
                  bVar21 = true;
                }
                if (*(char *)(*(int *)((int)local_40ac + 0x18) + 0x661) == '\0') {
                  uVar25 = FUN_1004e830(*(int *)((int)local_40ac + 0x18),uVar12);
                  uVar12 = (uint)(uVar25 >> 0x20);
                  uVar18 = (uint)uVar25;
                  if ((int)uVar18 < 0x10) {
                    if ((int)uVar18 < 0) {
                      uVar18 = 0;
                    }
                  }
                  else {
                    uVar18 = 0xf;
                  }
                }
                else {
                  uVar18 = FUN_1005bee0(&local_4078);
                  uVar18 = uVar18 & 0xff;
                  uVar12 = extraout_EDX;
                }
                if (bVar21) {
                  local_40b8 = CONCAT22(local_40b8._2_2_,(ushort)local_40b8 >> 4);
                }
                else {
                  uVar12 = (uint)(ushort)((ushort)(uVar18 << 0xc) | (ushort)local_40b8 >> 4);
                  local_40b8 = uVar12;
                }
                iVar16 = iVar16 + 0x10;
                iVar14 = iVar14 + 1;
                local_40b0 = local_40b0 + -1;
                *(ushort *)local_40c4 = (ushort)local_40b8;
              } while (local_40b0 != 0);
              local_40c4 = (uint *)((int)local_40c4 + 2);
              local_40b4 = local_40b4 + 4;
              local_40bc = local_40bc + 1;
            } while (local_40bc < 4);
            local_40b0 = 0;
          default:
            iVar14 = param_4;
            pvVar20 = local_40ac;
            break;
          case 3:
            FUN_1005c310(local_40ac,&param_1,(int *)&local_40c4,param_4,local_40c0,local_40c8,3);
            break;
          case 0x18:
            FUN_1005c310(local_40ac,&param_1,(int *)&local_40c4,param_4,local_40c0,local_40c8,0);
            break;
          case 0x1d:
            goto switchD_1005cc9e_caseD_1d;
          }
          if ((((iVar14 == 2) || (iVar14 == 3)) || (iVar14 == 0x18)) ||
             (*(char *)(*(int *)((int)pvVar20 + 0x18) + 0x660) != '\0')) {
            local_3858[1] = 1;
          }
          if (local_3858[2] == 0) {
            puVar11 = local_81c;
            iVar16 = 2;
            do {
              puVar11[1] = 0;
              *puVar11 = 0;
              puVar11[-1] = 0;
              puVar11 = puVar11 + 3;
              iVar16 = iVar16 + -1;
            } while (iVar16 != 0);
          }
          else {
            puVar3 = *(undefined4 **)((int)pvVar20 + 0x18);
            if (puVar3[2] == 3) {
              local_840 = 0.0;
              iVar16 = 0;
              local_838 = local_840;
              local_830 = local_840;
              if (0 < local_3858[2]) {
                pdVar17 = local_3848 + 2;
                dVar4 = local_840;
                dVar5 = local_840;
                dVar6 = local_840;
                do {
                  if (local_4058[iVar16] != -1) {
                    local_840 = (pdVar17[-2] * _DAT_10098100 - 1.0) + dVar5;
                    local_838 = (pdVar17[-1] * _DAT_10098100 - 1.0) + dVar6;
                    local_830 = (*pdVar17 * _DAT_10098100 - 1.0) + dVar4;
                    dVar5 = local_840;
                    dVar4 = local_830;
                    dVar6 = local_838;
                  }
                  iVar16 = iVar16 + 1;
                  pdVar17 = pdVar17 + 3;
                } while (iVar16 < local_3858[2]);
              }
              fVar22 = (float10)_CIsqrt();
              fVar23 = ABS((float10)local_840 / fVar22);
              fVar24 = ABS((float10)local_838 / fVar22);
              fVar22 = ABS((float10)local_830 / fVar22);
              if ((fVar23 < fVar24) || (fVar23 < fVar22)) {
                if ((fVar23 < fVar24 == (fVar23 == fVar24)) || (fVar24 < fVar22)) {
                  local_830 = _DAT_10098098;
                  if (fVar23 < fVar24) {
                    local_838 = _DAT_100a3978;
                    goto LAB_1005d072;
                  }
                  local_840 = _DAT_100a3978;
                  local_838 = _DAT_100a3970;
                }
                else {
                  local_838 = _DAT_10098098;
                  if (fVar22 < fVar23 == (fVar22 == fVar23)) {
                    local_830 = _DAT_100a3978;
LAB_1005d072:
                    local_840 = _DAT_100a3970;
                  }
                  else {
                    local_840 = _DAT_100a3978;
                    local_830 = _DAT_100a3970;
                  }
                }
              }
              else {
                local_840 = _DAT_10098098;
                if (fVar22 < fVar24 == (fVar22 == fVar24)) {
                  local_830 = _DAT_100a3978;
                  local_838 = _DAT_100a3970;
                }
                else {
                  local_838 = _DAT_100a3978;
                  local_830 = _DAT_100a3970;
                }
              }
            }
            local_828 = *puVar3;
            FUN_10083cd0(pvVar20,(double *)local_3858);
          }
          uVar7 = (ushort)((local_820 << 6 | local_81c[0]) << 5) | (ushort)local_81c[1];
          uVar13 = (ushort)((local_814 << 6 | local_810) << 5) | (ushort)local_80c;
          bVar21 = uVar13 < uVar7;
          if (uVar13 == uVar7) {
            iVar16 = 0;
            if (iVar14 == 1) {
              uVar13 = uVar13 + 1;
              if (0 < local_3858[2]) {
                do {
                  auStack_808[iVar16] = 0;
                  iVar16 = iVar16 + 1;
                } while (iVar16 < local_3858[2]);
              }
              local_824 = 3;
            }
            else {
              uVar7 = uVar7 + 1;
              if (0 < local_3858[2]) {
                do {
                  auStack_808[iVar16] = 1;
                  iVar16 = iVar16 + 1;
                } while (iVar16 < local_3858[2]);
              }
            }
            bVar21 = uVar13 < uVar7;
          }
          bVar21 = bVar21 == (local_824 == 3);
          uVar8 = uVar7;
          if (bVar21) {
            uVar8 = uVar13;
            uVar13 = uVar7;
          }
          *(ushort *)local_40c4 = uVar8;
          *(ushort *)((int)local_40c4 + 2) = uVar13;
          uVar12 = *(uint *)(param_1 + 8);
          iVar14 = 4;
          if ((3 < uVar12) && (3 < *(uint *)(param_1 + 0xc))) goto switchD_1005d1bf_caseD_13;
          uVar18 = *(uint *)(param_1 + 0xc);
          if (4 < (int)uVar12) {
            uVar12 = 4;
          }
          if (4 < (int)uVar18) {
            uVar18 = 4;
          }
          switch(uVar12 << 4 | uVar18) {
          case 0x11:
            local_4058[1] = local_4058[0];
            goto LAB_1005d3f7;
          case 0x12:
            local_4028[0] = local_4058[0];
            local_4038 = local_4058[0];
            local_4048 = local_4058[0];
LAB_1005d3f7:
            local_4058[3] = local_4058[1];
            local_4058[2] = local_4058[1];
LAB_1005d405:
            local_4044 = local_4058[1];
            local_4040 = local_4058[1];
            local_403c = local_4058[1];
            local_4034 = local_4058[1];
            local_4030 = local_4058[1];
            local_402c = local_4058[1];
LAB_1005d42f:
            local_4028[1] = local_4058[1];
LAB_1005d436:
            local_4028[2] = local_4028[1];
            local_4028[3] = local_4028[1];
            local_4058[1] = local_4058[3];
            local_4058[2] = local_4058[3];
            local_4044 = local_403c;
            local_4040 = local_403c;
            local_4034 = local_402c;
            local_4030 = local_402c;
            break;
          default:
            goto switchD_1005d1bf_caseD_13;
          case 0x14:
            local_4048 = local_4058[0];
            local_4038 = local_4058[0];
            local_4028[0] = local_4058[0];
            local_4044 = local_4058[1];
            local_4034 = local_4058[1];
            local_4028[1] = local_4058[1];
            local_4040 = local_4058[2];
            local_4030 = local_4058[2];
            local_4028[2] = local_4058[2];
            local_403c = local_4058[3];
            local_402c = local_4058[3];
            local_4028[3] = local_4058[3];
            break;
          case 0x21:
            local_4058[1] = local_4058[0];
            local_4058[2] = local_4058[0];
            local_4058[3] = local_4058[0];
            local_4038 = local_4048;
            local_4028[0] = local_4048;
            local_4058[1] = local_4048;
            goto LAB_1005d405;
          case 0x22:
            local_4058[2] = local_4058[1];
            local_4058[3] = local_4058[1];
            local_4040 = local_4044;
            local_403c = local_4044;
            local_4030 = local_4044;
            local_402c = local_4044;
            local_4028[3] = local_4044;
            local_4028[2] = local_4044;
            local_4038 = local_4048;
            local_4028[0] = local_4048;
            goto switchD_1005d1bf_caseD_13;
          case 0x24:
            local_4038 = local_4048;
            local_4028[0] = local_4048;
            local_4034 = local_4044;
            local_4028[1] = local_4044;
            local_4030 = local_4040;
            local_4028[2] = local_4040;
            local_402c = local_403c;
            local_4028[3] = local_403c;
            break;
          case 0x41:
            local_4058[1] = local_4058[0];
            local_4058[2] = local_4058[0];
            local_4058[3] = local_4058[0];
            local_4044 = local_4048;
            local_4040 = local_4048;
            local_403c = local_4048;
            local_4034 = local_4038;
            local_4030 = local_4038;
            local_402c = local_4038;
            local_4058[1] = local_4028[0];
            goto LAB_1005d42f;
          case 0x42:
            local_4058[2] = local_4058[1];
            local_4058[3] = local_4058[1];
            local_4040 = local_4044;
            local_403c = local_4044;
            local_4030 = local_4034;
            local_402c = local_4034;
            goto LAB_1005d436;
          }
switchD_1005d1bf_caseD_13:
          piVar19 = local_4028 + 1;
          do {
            if (piVar19[2] < 0) {
              uVar12 = local_408c * 4 | 3;
            }
            else {
              uVar12 = local_408c * 4 | auStack_808[piVar19[2]];
              if (bVar21) {
                if (local_824 == 4) {
                  uVar12 = uVar12 ^ 1;
                }
                else {
                  uVar12 = uVar12 ^ ~(uVar12 >> 1) & 1;
                }
              }
            }
            if (piVar19[1] < 0) {
              uVar12 = uVar12 * 4 | 3;
            }
            else {
              uVar12 = uVar12 * 4 | auStack_808[piVar19[1]];
              if (bVar21) {
                if (local_824 == 4) {
                  uVar12 = uVar12 ^ 1;
                }
                else {
                  uVar12 = uVar12 ^ ~(uVar12 >> 1) & 1;
                }
              }
            }
            if (*piVar19 < 0) {
              uVar12 = uVar12 * 4 | 3;
            }
            else {
              uVar12 = uVar12 * 4 | auStack_808[*piVar19];
              if (bVar21) {
                if (local_824 == 4) {
                  uVar12 = uVar12 ^ 1;
                }
                else {
                  uVar12 = uVar12 ^ ~(uVar12 >> 1) & 1;
                }
              }
            }
            if (piVar19[-1] < 0) {
              local_408c = uVar12 * 4 | 3;
            }
            else {
              local_408c = uVar12 * 4 | auStack_808[piVar19[-1]];
              if (bVar21) {
                if (local_824 == 4) {
                  local_408c = local_408c ^ 1;
                }
                else {
                  local_408c = local_408c ^ ~(local_408c >> 1) & 1;
                }
              }
            }
            piVar19 = piVar19 + -4;
            iVar14 = iVar14 + -1;
          } while (iVar14 != 0);
          local_40c4[1] = local_408c;
          local_40c4 = local_40c4 + 2;
switchD_1005cc9e_caseD_1d:
          local_40c0 = local_40c0 + 4;
        } while (local_40c0 < *(int *)(param_1 + 0xc));
      }
      local_40c8 = local_40c8 + 4;
    } while ((int)local_40c8 < *(int *)(param_1 + 8));
  }
  return 0;
}

