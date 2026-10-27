/* Original DLL evidence only; RVA 0x7b3f0; not recovered business source. */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

float10 __fastcall
FUN_1007b3f0(undefined4 param_1,undefined4 param_2,int param_3,int *param_4,int param_5,int param_6,
            undefined4 *param_7,int *param_8)

{
  int iVar1;
  double dVar2;
  double dVar3;
  char cVar4;
  undefined *puVar5;
  byte bVar6;
  undefined4 *puVar7;
  uint uVar8;
  char *pcVar9;
  double *pdVar10;
  byte bVar11;
  undefined4 extraout_ECX;
  undefined4 extraout_ECX_00;
  undefined4 extraout_ECX_01;
  int iVar12;
  undefined4 extraout_ECX_02;
  double *pdVar13;
  uint uVar14;
  uint uVar15;
  undefined4 extraout_EDX;
  int iVar16;
  int iVar17;
  uint uVar18;
  bool bVar19;
  float10 extraout_ST0;
  float10 extraout_ST0_00;
  float10 fVar20;
  float10 fVar21;
  float10 fVar22;
  float10 fVar23;
  float10 extraout_ST1;
  float10 extraout_ST1_00;
  ulonglong uVar24;
  ulonglong uVar25;
  int local_e8;
  uint *local_e4;
  int local_e0;
  double *local_dc;
  double *local_d8;
  int local_d4;
  uint *local_d0;
  uint local_c8;
  int local_c4;
  undefined8 local_b0;
  undefined4 local_a8;
  undefined4 uStack_a4;
  undefined4 local_a0;
  undefined4 uStack_9c;
  uint local_94 [2];
  uint local_8c [3];
  double *local_80;
  int local_7c;
  double local_78 [2];
  uint local_64 [24];
  
  iVar17 = 0;
  if (0 < *(int *)(param_3 + 8)) {
    do {
      uVar24 = FUN_1004e830(param_1,param_2);
      uVar25 = FUN_1004e830(extraout_ECX,(int)(uVar24 >> 0x20));
      fVar22 = extraout_ST0;
      fVar21 = extraout_ST1;
      if ((int)uVar25 != (int)uVar24) {
LAB_1007b4cb:
        local_b0._0_4_ = 0;
        local_b0._4_4_ = 0;
        local_a8 = 0;
        uStack_a4 = 0;
        local_a0 = 0;
        uStack_9c = 0;
        local_8c[0] = 5;
        local_8c[1] = 6;
        local_8c[2] = 5;
        if (DAT_100c1c60 == 0) {
          DAT_100c1c60 = 1;
          local_d4 = 0;
          do {
            bVar6 = (byte)local_8c[local_d4];
            iVar16 = (1 << (bVar6 & 0x1f)) + -1;
            for (iVar17 = iVar16; -1 < iVar17; iVar17 = iVar17 + -1) {
              if (-1 < iVar16) {
                bVar11 = 8 - bVar6;
                uVar8 = iVar17 >> (bVar6 * '\x02' - 8 & 0x1f) | iVar17 << (bVar11 & 0x1f);
                iVar12 = DAT_100bd608 * uVar8;
                local_e8 = iVar16;
                do {
                  uVar18 = local_e8 >> (bVar6 * '\x02' - 8 & 0x1f) | local_e8 << (bVar11 & 0x1f);
                  iVar1 = local_d4 +
                          ((int)(DAT_100bd60c * uVar18 + iVar12 + _DAT_100bd610) / DAT_100bd614) * 3
                  ;
                  local_e0._0_1_ = (char)iVar17;
                  if (((&DAT_100c0160)[iVar1 * 9] == '\0') ||
                     (uVar14 = (int)((uint)(byte)(&DAT_100c0162)[iVar1 * 9] -
                                    (uint)(byte)(&DAT_100c0161)[iVar1 * 9]) >> 0x1f,
                     uVar15 = (int)(uVar18 - uVar8) >> 0x1f,
                     (int)((uVar18 - uVar8 ^ uVar15) - uVar15) <
                     (int)(((uint)(byte)(&DAT_100c0162)[iVar1 * 9] -
                            (uint)(byte)(&DAT_100c0161)[iVar1 * 9] ^ uVar14) - uVar14))) {
                    (&DAT_100c0160)[iVar1 * 9] = 1;
                    (&DAT_100c0161)[iVar1 * 9] = (char)local_e0 << (bVar11 & 0x1f);
                    (&DAT_100c0162)[iVar1 * 9] = (char)local_e8 << (bVar11 & 0x1f);
                  }
                  iVar1 = local_d4 +
                          ((int)(DAT_100bd618 * uVar8 + DAT_100bd61c * uVar18 + _DAT_100bd620) /
                          DAT_100bd624) * 3;
                  if (((&DAT_100c0163)[iVar1 * 9] == '\0') ||
                     (uVar14 = (int)((uint)(byte)(&DAT_100c0165)[iVar1 * 9] -
                                    (uint)(byte)(&DAT_100c0164)[iVar1 * 9]) >> 0x1f,
                     uVar15 = (int)(uVar18 - uVar8) >> 0x1f,
                     (int)((uVar18 - uVar8 ^ uVar15) - uVar15) <
                     (int)(((uint)(byte)(&DAT_100c0165)[iVar1 * 9] -
                            (uint)(byte)(&DAT_100c0164)[iVar1 * 9] ^ uVar14) - uVar14))) {
                    (&DAT_100c0163)[iVar1 * 9] = 1;
                    (&DAT_100c0164)[iVar1 * 9] = (char)local_e0 << (bVar11 & 0x1f);
                    (&DAT_100c0165)[iVar1 * 9] = (char)local_e8 << (bVar11 & 0x1f);
                  }
                  iVar1 = local_d4 +
                          ((int)(DAT_100bd628 * uVar8 + DAT_100bd62c * uVar18 + _DAT_100bd630) /
                          DAT_100bd634) * 3;
                  if (((&DAT_100c0166)[iVar1 * 9] == '\0') ||
                     (uVar14 = (int)((uint)(byte)(&DAT_100c0168)[iVar1 * 9] -
                                    (uint)(byte)(&DAT_100c0167)[iVar1 * 9]) >> 0x1f,
                     uVar15 = (int)(uVar18 - uVar8) >> 0x1f,
                     (int)((uVar18 - uVar8 ^ uVar15) - uVar15) <
                     (int)(((uint)(byte)(&DAT_100c0168)[iVar1 * 9] -
                            (uint)(byte)(&DAT_100c0167)[iVar1 * 9] ^ uVar14) - uVar14))) {
                    (&DAT_100c0166)[iVar1 * 9] = 1;
                    (&DAT_100c0167)[iVar1 * 9] = (char)local_e0 << (bVar11 & 0x1f);
                    (&DAT_100c0168)[iVar1 * 9] = (char)local_e8 << (bVar11 & 0x1f);
                  }
                  local_e8 = local_e8 + -1;
                } while (-1 < local_e8);
              }
            }
            local_d4 = local_d4 + 1;
          } while (local_d4 < 3);
        }
        iVar17 = *param_4;
        local_e8 = 0;
        if (iVar17 != 1 && -1 < iVar17 + -1) {
          local_80 = (double *)(param_3 + 0x3018);
          local_7c = *param_4 + -1;
          local_e4 = local_64;
          local_d8 = (double *)&local_b0;
          while( true ) {
            local_dc = local_80;
            *local_d8 = 0.0;
            local_e0 = 0;
            local_d0 = local_e4;
            while( true ) {
              fVar20 = ((float10)*(double *)(param_6 + local_e0 * 8) * fVar21) / (float10)*local_dc;
              floor((double)(fVar20 + fVar22));
              uVar24 = FUN_1004e830(extraout_ECX_02,extraout_EDX);
              local_c8 = (uint)uVar24;
              if ((int)local_c8 < 0) {
                local_c8 = 0;
              }
              else if (0xff < (int)local_c8) {
                local_c8 = 0xff;
              }
              fVar21 = (float10)_DAT_10098000;
              local_d4 = 0;
              cVar4 = (&DAT_100c0160)[local_e8 * 3 + (local_e0 * 3 + local_c8 * 9) * 3];
              local_c4 = 1;
              pdVar13 = local_78;
              do {
                if ((cVar4 == '\0') ||
                   (uVar8 = local_c8,
                   0.0 < ((double)fVar20 - (double)(int)local_c8) * (double)local_c4)) {
                  uVar8 = local_c8 + local_c4;
                  if ((int)uVar8 < 0) {
                    uVar8 = 0;
                  }
                  else if (0xff < (int)uVar8) {
                    uVar8 = 0xff;
                  }
                  iVar16 = local_e8 + (local_e0 + uVar8 * 3) * 3;
                  pcVar9 = &DAT_100c0160 + iVar16 * 3;
                  if ((&DAT_100c0160)[iVar16 * 3] == '\0') {
                    do {
                      pcVar9 = pcVar9 + local_c4 * 0x1b;
                      uVar8 = uVar8 + local_c4;
                    } while (*pcVar9 == '\0');
                  }
                }
                iVar16 = 0;
                *pdVar13 = 0.0;
                if (3 < param_5) {
                  fVar22 = (float10)(int)uVar8 * (float10)*local_dc;
                  iVar12 = (param_5 - 4U >> 2) + 1;
                  iVar16 = iVar12 * 4;
                  pdVar10 = (double *)(param_6 + 0x30 + local_e0 * 8);
                  do {
                    iVar12 = iVar12 + -1;
                    fVar23 = fVar22 - (float10)pdVar10[-6] * fVar21;
                    *pdVar13 = (double)(fVar23 * fVar23 + (float10)*pdVar13);
                    fVar23 = fVar22 - (float10)pdVar10[-3] * fVar21;
                    *pdVar13 = (double)(fVar23 * fVar23 + (float10)*pdVar13);
                    fVar23 = fVar22 - (float10)*pdVar10 * fVar21;
                    *pdVar13 = (double)(fVar23 * fVar23 + (float10)*pdVar13);
                    fVar23 = fVar22 - (float10)pdVar10[3] * fVar21;
                    *pdVar13 = (double)(fVar23 * fVar23 + (float10)*pdVar13);
                    pdVar10 = pdVar10 + 0xc;
                  } while (iVar12 != 0);
                }
                if (iVar16 < param_5) {
                  dVar2 = *local_dc;
                  pdVar10 = (double *)(param_6 + (local_e0 + iVar16 * 3) * 8);
                  iVar16 = param_5 - iVar16;
                  do {
                    dVar3 = *pdVar10;
                    pdVar10 = pdVar10 + 3;
                    iVar16 = iVar16 + -1;
                    fVar22 = (float10)(int)uVar8 * (float10)dVar2 - (float10)dVar3 * fVar21;
                    *pdVar13 = (double)(fVar22 * fVar22 + (float10)*pdVar13);
                  } while (iVar16 != 0);
                }
                local_94[local_d4] = uVar8;
                uVar8 = local_94[0];
                local_d4 = local_d4 + 1;
                local_c4 = -local_c4;
                pdVar13 = pdVar13 + 1;
              } while (local_d4 < 2);
              if (local_78[1] <= local_78[0]) {
                bVar19 = local_78[1] < local_78[0];
                *local_d8 = local_78[1] + *local_d8;
                if ((bVar19) || (uVar8 = local_94[0], (local_c8 & 1) == 0)) {
                  uVar8 = local_94[1];
                }
              }
              else {
                *local_d8 = local_78[0] + *local_d8;
              }
              local_dc = local_dc + 1;
              *local_d0 = uVar8;
              local_e0 = local_e0 + 1;
              if (2 < local_e0) break;
              fVar22 = (float10)_DAT_10098098;
              local_d0 = local_d0 + 3;
            }
            local_e4 = local_e4 + 1;
            local_e8 = local_e8 + 1;
            local_d8 = local_d8 + 1;
            if (local_7c <= local_e8) break;
            fVar22 = (float10)_DAT_10098098;
          }
        }
        if (iVar17 == 4) {
          if (*(int *)(param_3 + 4) == 1) {
            if ((double)CONCAT44(local_b0._4_4_,(undefined4)local_b0) <=
                (double)CONCAT44(uStack_9c,local_a0)) {
              local_e8 = 0;
              goto LAB_1007bad8;
            }
          }
          else {
            dVar3 = (double)CONCAT44(uStack_a4,local_a8);
            dVar2 = (double)CONCAT44(local_b0._4_4_,(undefined4)local_b0);
            if (dVar2 < dVar3 == (dVar2 == dVar3)) {
              if (dVar3 < (double)CONCAT44(uStack_9c,local_a0) !=
                  (dVar3 == (double)CONCAT44(uStack_9c,local_a0))) {
                local_e8 = 1;
                goto LAB_1007bad8;
              }
            }
            else if (dVar2 < (double)CONCAT44(uStack_9c,local_a0) !=
                     (dVar2 == (double)CONCAT44(uStack_9c,local_a0))) {
              local_e8 = 0;
              goto LAB_1007bad8;
            }
          }
          local_e8 = 2;
        }
        else if ((iVar17 == 3) &&
                (local_e8 = 0,
                (double)CONCAT44(uStack_a4,local_a8) <
                (double)CONCAT44(local_b0._4_4_,(undefined4)local_b0))) {
          local_e8 = 1;
        }
LAB_1007bad8:
        iVar17 = 0;
        local_e4 = local_64 + local_e8;
        do {
          bVar6 = 8 - (char)local_8c[iVar17];
          iVar16 = local_e8 + (iVar17 + *local_e4 * 3) * 3;
          iVar12 = iVar17 * 4 - (int)local_8c;
          iVar17 = iVar17 + 1;
          *(uint *)((int)param_7 + (int)local_8c + iVar12) =
               (uint)((byte)(&DAT_100c0161)[iVar16 * 3] >> (bVar6 & 0x1f));
          *(uint *)((int)param_7 + (int)&local_80 + iVar12) =
               (uint)((byte)(&DAT_100c0162)[iVar16 * 3] >> (bVar6 & 0x1f));
          local_e4 = local_e4 + 3;
        } while (iVar17 < 3);
        if (0 < param_5) {
          for (; param_5 != 0; param_5 = param_5 + -1) {
            *param_8 = local_e8 + 1;
            param_8 = param_8 + 1;
          }
        }
        if (local_e8 != 0) {
          *param_4 = local_e8 + 2;
        }
        return (float10)(double)(&local_b0)[local_e8];
      }
      uVar25 = FUN_1004e830(extraout_ECX_00,(int)(uVar25 >> 0x20));
      param_2 = (undefined4)(uVar25 >> 0x20);
      fVar22 = extraout_ST0_00;
      fVar21 = extraout_ST1_00;
      if ((int)uVar24 != (int)uVar25) goto LAB_1007b4cb;
      iVar17 = iVar17 + 1;
      param_1 = extraout_ECX_01;
    } while (iVar17 < *(int *)(param_3 + 8));
  }
  uVar24 = FUN_1004e830(param_1,param_2);
  if (*(int *)(param_3 + 4) == 1) {
    puVar5 = &DAT_100bb608;
  }
  else {
    puVar5 = &DAT_100b9608;
  }
  puVar7 = (undefined4 *)(puVar5 + (int)uVar24 * 0x20);
  *param_4 = puVar7[6];
  *param_7 = *puVar7;
  param_7[1] = puVar7[1];
  param_7[2] = puVar7[2];
  param_7[3] = puVar7[3];
  param_7[4] = puVar7[4];
  param_7[5] = puVar7[5];
  iVar17 = 0;
  if (0 < *(int *)(param_3 + 8)) {
    do {
      param_8[iVar17] = puVar7[7];
      iVar17 = iVar17 + 1;
    } while (iVar17 < *(int *)(param_3 + 8));
  }
  return (float10)0;
}

