/* Original DLL evidence only; RVA 0x7c420; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */

void __thiscall FUN_1007c420(void *this,int param_1,int param_2)

{
  double dVar1;
  double dVar2;
  double dVar3;
  double dVar4;
  double dVar5;
  double dVar6;
  double dVar7;
  double dVar8;
  double dVar9;
  double dVar10;
  bool bVar11;
  bool bVar12;
  double *pdVar13;
  undefined3 extraout_var;
  undefined3 extraout_var_00;
  undefined3 extraout_var_01;
  undefined3 extraout_var_02;
  double *pdVar14;
  int iVar15;
  int iVar16;
  double *pdVar17;
  int iVar18;
  int iVar19;
  int iVar20;
  int iVar21;
  undefined4 *puVar22;
  int *piVar23;
  int *piVar24;
  float10 fVar25;
  double *pdStack_1714c;
  int *piStack_17140;
  double *pdStack_1713c;
  int iStack_17138;
  uint uStack_17134;
  uint uStack_17130;
  double *pdStack_1712c;
  uint uStack_17120;
  double *pdStack_1711c;
  int aiStack_17118 [2];
  undefined8 uStack_17110;
  undefined4 uStack_17108;
  undefined4 uStack_17104;
  undefined4 uStack_17100;
  undefined4 uStack_170fc;
  undefined8 uStack_170f8;
  undefined4 uStack_170f0;
  undefined4 uStack_170ec;
  undefined4 uStack_170e8;
  undefined4 uStack_170e4;
  undefined4 uStack_170e0;
  undefined4 uStack_170dc;
  undefined4 uStack_170d8;
  undefined4 uStack_170d4;
  double adStack_170d0 [6];
  double adStack_170a0 [6];
  double adStack_17070 [6];
  double adStack_17040 [512];
  undefined4 auStack_16040 [512];
  double adStack_15840 [1536];
  int aiStack_12840 [512];
  double adStack_12040 [1536];
  double local_f040 [1536];
  double local_c040 [1536];
  int local_9040 [1024];
  int local_8040 [8191];
  undefined4 uStack_44;
  
  uStack_44 = 0x1007c430;
  iVar21 = 0;
  if (0 < *(int *)(param_1 + 8)) {
    pdVar17 = (double *)(param_1 + 0x10);
    do {
      iVar18 = 3;
      do {
        if (0.0 <= *pdVar17) {
          if (1.0 < *pdVar17) {
            *pdVar17 = 1.0;
          }
        }
        else {
          *pdVar17 = 0.0;
        }
        pdVar17 = pdVar17 + 1;
        iVar18 = iVar18 + -1;
      } while (iVar18 != 0);
      iVar21 = iVar21 + 1;
    } while (iVar21 < *(int *)(param_1 + 8));
  }
  if (*(double *)(param_1 + 0x3018) < 0.0) {
    *(undefined8 *)(param_1 + 0x3018) = 0;
  }
  if (1.0 < *(double *)(param_1 + 0x3018)) {
    *(undefined8 *)(param_1 + 0x3018) = 0x3ff0000000000000;
  }
  if (*(double *)(param_1 + 0x3020) < 0.0) {
    *(undefined8 *)(param_1 + 0x3020) = 0;
  }
  if (1.0 < *(double *)(param_1 + 0x3020)) {
    *(undefined8 *)(param_1 + 0x3020) = 0x3ff0000000000000;
  }
  if (*(double *)(param_1 + 0x3028) < 0.0) {
    *(undefined8 *)(param_1 + 0x3028) = 0;
  }
  if (1.0 < *(double *)(param_1 + 0x3028)) {
    *(undefined8 *)(param_1 + 0x3028) = 0x3ff0000000000000;
  }
  pdStack_1712c = (double *)0x0;
  uStack_17130 = 0;
  memset(adStack_15840,0,0x3000);
  memset(adStack_12040,0,0x3000);
  uStack_170f8._0_4_ = 0;
  uStack_170f8._4_4_ = 0;
  uStack_170f0 = 0;
  uStack_170ec = 0;
  uStack_170e8 = 0;
  uStack_170e4 = 0;
  memset(local_c040,0,0x3000);
  memset(adStack_17070 + 6,0,0x1000);
  memset(adStack_170d0,0,0x30);
  uStack_17110._0_4_ = 0;
  uStack_17110._4_4_ = 0;
  uStack_17108 = 0;
  uStack_17104 = 0;
  uStack_17100 = 0;
  uStack_170fc = 0;
  uStack_170e0 = 0;
  uStack_170dc = 0;
  uStack_170d8 = 0;
  uStack_170d4 = 0;
  memset(adStack_170a0,0,0x60);
  memset(aiStack_12840,0,0x800);
  memset(auStack_16040,0,0x800);
  memset(local_8040,0,0x8000);
  memset(local_9040,0,0x1000);
  aiStack_17118[0] = 0;
  aiStack_17118[1] = 0;
  iVar21 = *(int *)(param_1 + 8);
  if (iVar21 != 0) {
    dVar9 = 1.0 / (double)iVar21;
    if (*(int *)(param_1 + 4) == 1) {
      *(undefined4 *)(param_1 + 0x3034) = 4;
    }
    iVar18 = (int)adStack_15840 + (8 - param_1);
    piStack_17140 = (int *)0x0;
    bVar11 = true;
    pdVar13 = (double *)&uStack_170f8;
    pdVar17 = (double *)(param_1 + 0x10);
    do {
      *pdVar13 = 0.0;
      uStack_17120 = 0;
      if (3 < iVar21) {
        pdStack_1713c = (double *)((int)pdVar17 + iVar18);
        iVar19 = (iVar21 - 4U >> 2) + 1;
        uStack_17120 = iVar19 * 4;
        pdVar14 = pdVar17 + 3;
        do {
          dVar1 = *pdVar17;
          dVar2 = pdVar14[-3];
          dVar3 = pdVar17[0x601];
          dVar4 = pdVar14[-3];
          pdStack_1713c[-3] = dVar3 * dVar4;
          dVar10 = dVar3 * dVar4 + *pdVar13;
          *pdVar13 = dVar10;
          dVar3 = *pdVar17;
          dVar4 = *pdVar14;
          dVar5 = *pdVar14;
          dVar6 = pdVar17[0x601];
          *pdStack_1713c = dVar5 * dVar6;
          dVar10 = dVar5 * dVar6 + dVar10;
          *pdVar13 = dVar10;
          dVar5 = *pdVar17;
          dVar6 = pdVar14[3];
          dVar7 = pdVar17[0x601];
          dVar8 = pdVar14[3];
          *(double *)(iVar18 + (int)pdVar14) = dVar7 * dVar8;
          dVar10 = dVar7 * dVar8 + dVar10;
          *pdVar13 = dVar10;
          dVar7 = pdVar17[0x601];
          pdStack_1713c = pdStack_1713c + 0xc;
          dVar8 = pdVar14[6];
          bVar11 = (bool)(bVar11 & dVar1 == dVar2 & dVar3 == dVar4 & dVar5 == dVar6 &
                         *pdVar17 == pdVar14[6]);
          pdVar14 = pdVar14 + 0xc;
          iVar19 = iVar19 + -1;
          *(double *)((int)auStack_16040 + (0x7c0 - param_1) + (int)pdVar14) = dVar7 * dVar8;
          *pdVar13 = dVar7 * dVar8 + dVar10;
        } while (iVar19 != 0);
      }
      if ((int)uStack_17120 < iVar21) {
        iVar19 = (int)piStack_17140 + uStack_17120 * 3;
        pdVar14 = (double *)(param_1 + 0x10 + iVar19 * 8);
        iVar20 = iVar21 - uStack_17120;
        pdStack_1714c = adStack_15840 + iVar19;
        do {
          dVar1 = *pdVar14;
          bVar11 = (bool)(bVar11 & *pdVar17 == *pdVar14);
          dVar2 = pdVar17[0x601];
          pdVar14 = pdVar14 + 3;
          iVar20 = iVar20 + -1;
          *pdStack_1714c = dVar1 * dVar2;
          *pdVar13 = dVar1 * dVar2 + *pdVar13;
          pdStack_1714c = pdStack_1714c + 3;
        } while (iVar20 != 0);
      }
      piStack_17140 = (int *)((int)piStack_17140 + 1);
      pdVar17 = pdVar17 + 1;
      pdVar13 = pdVar13 + 1;
    } while ((int)piStack_17140 < 3);
    if (bVar11) {
      aiStack_17118[0] = param_2;
      FUN_1007b3f0(this,adStack_15840,param_1,aiStack_17118,iVar21,(int)adStack_15840,
                   (undefined4 *)(param_1 + 0x3038),(int *)(param_1 + 0x3050));
      *(int *)(param_1 + 0x3034) = aiStack_17118[0];
      return;
    }
    iVar19 = 0;
    pdStack_1714c = (double *)(param_1 + 0x3018);
    do {
      iVar20 = 0;
      dVar1 = (double)(&uStack_170f8)[iVar19] / (double)iVar21;
      (&uStack_170f8)[iVar19] = dVar1;
      if (3 < iVar21) {
        iVar15 = (iVar21 - 4U >> 2) + 1;
        iVar20 = iVar15 * 4;
        iVar16 = iVar19 * 8;
        do {
          iVar15 = iVar15 + -1;
          *(double *)((int)adStack_12040 + iVar16) =
               *(double *)((int)adStack_15840 + iVar16) - dVar1;
          *(double *)((int)adStack_12040 + iVar16 + 0x18) =
               *(double *)((int)adStack_15840 + iVar16 + 0x18) - dVar1;
          *(double *)((int)adStack_12040 + iVar16 + 0x30) =
               *(double *)((int)adStack_15840 + iVar16 + 0x30) - dVar1;
          *(double *)((int)adStack_12040 + iVar16 + 0x48) =
               *(double *)((int)adStack_15840 + iVar16 + 0x48) - dVar1;
          iVar16 = iVar16 + 0x60;
        } while (iVar15 != 0);
      }
      if (iVar20 < iVar21) {
        iVar16 = iVar21 - iVar20;
        iVar20 = (iVar19 + iVar20 * 3) * 8;
        do {
          iVar16 = iVar16 + -1;
          *(double *)((int)adStack_12040 + iVar20) =
               *(double *)((int)adStack_15840 + iVar20) - (double)(&uStack_170f8)[iVar19];
          iVar20 = iVar20 + 0x18;
        } while (iVar16 != 0);
      }
      iVar20 = iVar19 + 1;
      adStack_170d0[iVar19] = -dVar1;
      adStack_170d0[iVar19 + 3] = *pdStack_1714c - dVar1;
      iVar19 = iVar20;
      pdStack_1714c = pdStack_1714c + 1;
    } while (iVar20 < 3);
    pdVar17 = local_c040;
    for (iVar19 = 0xc00; iVar19 != 0; iVar19 = iVar19 + -1) {
      *(undefined4 *)pdVar17 = 0;
      pdVar17 = (double *)((int)pdVar17 + 4);
    }
    pdVar17 = local_f040;
    for (iVar19 = 0xc00; iVar19 != 0; iVar19 = iVar19 + -1) {
      *(undefined4 *)pdVar17 = 0;
      pdVar17 = (double *)((int)pdVar17 + 4);
    }
    iVar19 = 0;
    iVar20 = 0;
    if (0 < iVar21) {
      iVar16 = 0;
      do {
        iVar15 = 0;
        if (3 < iVar19) {
          pdVar17 = local_c040 + 1;
          do {
            if (((*(double *)((int)adStack_15840 + iVar16) == pdVar17[-1]) &&
                (*(double *)((int)adStack_15840 + iVar16 + 8) == *pdVar17)) &&
               (*(double *)((int)adStack_15840 + iVar16 + 0x10) == pdVar17[1])) goto LAB_1007caee;
            if (((*(double *)((int)adStack_15840 + iVar16) == pdVar17[2]) &&
                (*(double *)((int)adStack_15840 + iVar16 + 8) == pdVar17[3])) &&
               (*(double *)((int)adStack_15840 + iVar16 + 0x10) == pdVar17[4])) {
              iVar15 = iVar15 + 1;
              goto LAB_1007caee;
            }
            if (((*(double *)((int)adStack_15840 + iVar16) == pdVar17[5]) &&
                (*(double *)((int)adStack_15840 + iVar16 + 8) == pdVar17[6])) &&
               (*(double *)((int)adStack_15840 + iVar16 + 0x10) == pdVar17[7])) {
              iVar15 = iVar15 + 2;
              goto LAB_1007caee;
            }
            if (((*(double *)((int)adStack_15840 + iVar16) == pdVar17[8]) &&
                (*(double *)((int)adStack_15840 + iVar16 + 8) == pdVar17[9])) &&
               (*(double *)((int)adStack_15840 + iVar16 + 0x10) == pdVar17[10])) {
              iVar15 = iVar15 + 3;
              goto LAB_1007caee;
            }
            iVar15 = iVar15 + 4;
            pdVar17 = pdVar17 + 0xc;
          } while (iVar15 < iVar19 + -3);
        }
        if (iVar15 < iVar19) {
          pdVar17 = local_c040 + iVar15 * 3 + 1;
          do {
            if (((*(double *)((int)adStack_15840 + iVar16) == pdVar17[-1]) &&
                (*(double *)((int)adStack_15840 + iVar16 + 8) == *pdVar17)) &&
               (*(double *)((int)adStack_15840 + iVar16 + 0x10) == pdVar17[1])) break;
            iVar15 = iVar15 + 1;
            pdVar17 = pdVar17 + 3;
          } while (iVar15 < iVar19);
        }
LAB_1007caee:
        if (iVar15 == iVar19) {
          local_c040[iVar15 * 3] = *(double *)((int)adStack_15840 + iVar16);
          iVar19 = iVar19 + 1;
          local_c040[iVar15 * 3 + 1] = *(double *)((int)adStack_15840 + iVar16 + 8);
          local_c040[iVar15 * 3 + 2] = *(double *)((int)adStack_15840 + iVar16 + 0x10);
          local_f040[iVar15 * 3] = *(double *)((int)adStack_12040 + iVar16);
          local_f040[iVar15 * 3 + 1] = *(double *)((int)adStack_12040 + iVar16 + 8);
          local_f040[iVar15 * 3 + 2] = *(double *)((int)adStack_12040 + iVar16 + 0x10);
          adStack_17070[iVar15 + 6] = 1.0;
        }
        else {
          local_f040[iVar15 * 3] = *(double *)((int)adStack_12040 + iVar16) + local_f040[iVar15 * 3]
          ;
          local_f040[iVar15 * 3 + 1] =
               *(double *)((int)adStack_12040 + iVar16 + 8) + local_f040[iVar15 * 3 + 1];
          local_f040[iVar15 * 3 + 2] =
               *(double *)((int)adStack_12040 + iVar16 + 0x10) + local_f040[iVar15 * 3 + 2];
          adStack_17070[iVar15 + 6] = adStack_17070[iVar15 + 6] + 1.0;
        }
        aiStack_12840[iVar20] = iVar15;
        iVar20 = iVar20 + 1;
        iVar16 = iVar16 + 0x18;
      } while (iVar20 < iVar21);
    }
    iVar20 = 0;
    if (3 < iVar19) {
      pdVar17 = local_f040 + 1;
      iVar16 = iVar20;
      do {
        iVar20 = iVar16 + 4;
        adStack_17070[iVar16 + 6] = adStack_17070[iVar16 + 6] * dVar9;
        pdVar17[-1] = pdVar17[-1] * dVar9;
        *pdVar17 = *pdVar17 * dVar9;
        pdVar17[1] = pdVar17[1] * dVar9;
        adStack_17070[iVar16 + 7] = adStack_17070[iVar16 + 7] * dVar9;
        pdVar17[2] = dVar9 * pdVar17[2];
        pdVar17[3] = pdVar17[3] * dVar9;
        pdVar17[4] = pdVar17[4] * dVar9;
        adStack_17070[iVar16 + 8] = adStack_17070[iVar16 + 8] * dVar9;
        pdVar17[5] = pdVar17[5] * dVar9;
        pdVar17[6] = dVar9 * pdVar17[6];
        pdVar17[7] = pdVar17[7] * dVar9;
        adStack_17070[iVar16 + 9] = adStack_17070[iVar16 + 9] * dVar9;
        pdVar17[8] = pdVar17[8] * dVar9;
        pdVar17[9] = pdVar17[9] * dVar9;
        pdVar17[10] = dVar9 * pdVar17[10];
        pdVar17 = pdVar17 + 0xc;
        iVar16 = iVar20;
      } while (iVar20 < iVar19 + -3);
    }
    if (iVar20 < iVar19) {
      pdVar17 = local_f040 + iVar20 * 3 + 1;
      do {
        iVar16 = iVar20 + 1;
        adStack_17070[iVar20 + 6] = adStack_17070[iVar20 + 6] * dVar9;
        pdVar17[-1] = dVar9 * pdVar17[-1];
        *pdVar17 = *pdVar17 * dVar9;
        pdVar17[1] = pdVar17[1] * dVar9;
        pdVar17 = pdVar17 + 3;
        iVar20 = iVar16;
      } while (iVar16 < iVar19);
    }
    if (iVar19 == 1) {
      pdStack_1712c = adStack_15840 + 3;
      piStack_17140 = (int *)0x0;
      pdVar17 = (double *)(param_1 + 0x3018);
      do {
        iVar19 = 0;
        if (3 < iVar21) {
          iVar20 = (iVar21 - 4U >> 2) + 1;
          iVar19 = iVar20 * 4;
          pdVar13 = pdVar17 + -0x5fe;
          pdVar14 = pdStack_1712c;
          do {
            iVar20 = iVar20 + -1;
            pdVar14[-3] = pdVar13[-3] * *pdVar17;
            *pdVar14 = *pdVar13 * *pdVar17;
            *(double *)((int)pdVar13 + iVar18) = pdVar13[3] * *pdVar17;
            *(double *)((int)pdVar13 + (int)adStack_15840 + (0x20 - param_1)) =
                 pdVar13[6] * *pdVar17;
            pdVar13 = pdVar13 + 0xc;
            pdVar14 = pdVar14 + 0xc;
          } while (iVar20 != 0);
        }
        if (iVar19 < iVar21) {
          iVar20 = (int)piStack_17140 + iVar19 * 3;
          pdVar13 = (double *)(param_1 + 0x10 + iVar20 * 8);
          iVar19 = iVar21 - iVar19;
          pdVar14 = adStack_15840 + iVar20;
          do {
            dVar9 = *pdVar13;
            pdVar13 = pdVar13 + 3;
            iVar19 = iVar19 + -1;
            *pdVar14 = dVar9 * *pdVar17;
            pdVar14 = pdVar14 + 3;
          } while (iVar19 != 0);
        }
        pdStack_1712c = pdStack_1712c + 1;
        piStack_17140 = (int *)((int)piStack_17140 + 1);
        pdVar17 = pdVar17 + 1;
      } while ((int)piStack_17140 < 3);
      aiStack_17118[0] = param_2;
      FUN_1007b3f0(this,aiStack_17118,param_1,aiStack_17118,iVar21,(int)adStack_15840,
                   (undefined4 *)(param_1 + 0x3038),(int *)(param_1 + 0x3050));
      *(int *)(param_1 + 0x3034) = aiStack_17118[0];
      return;
    }
    if (iVar19 != 2) {
      uStack_17134 = 0;
      bVar11 = false;
      FUN_1005f5d0(iVar21,(int)adStack_12040,(int)&uStack_17110);
      FUN_1005ffd0(iVar19,(int)local_c040,(double *)&uStack_17110,(int)local_8040,0);
      pdStack_1714c = (double *)local_9040;
      pdStack_1711c = adStack_170a0;
      piStack_17140 = aiStack_17118;
      uStack_17120 = 0;
LAB_1007cf10:
      pdStack_1713c = (double *)0x10;
LAB_1007cf20:
      *piStack_17140 = param_2 + -1;
      if ((uStack_17120 - 1) + (int)pdStack_1713c == 0x10) {
        FUN_1005fe00(iVar19,(int)local_c040,(double *)&uStack_17110);
      }
      else {
        fVar25 = FUN_100603d0(piStack_17140,iVar19,(int)local_f040,(int)(adStack_17070 + 6),
                              (int)(local_8040 + uStack_17134 * 0x200),pdStack_1711c,
                              (double *)&uStack_17110);
        *(double *)(&uStack_170e0 + uStack_17120 * 2) = (double)fVar25;
        uStack_17130 = uStack_17120;
      }
      uStack_17134 = uStack_17134 + 1 & 0x8000000f;
      if ((int)uStack_17134 < 0) {
        uStack_17134 = (uStack_17134 - 1 | 0xfffffff0) + 1;
      }
      iStack_17138 = 0;
      bVar11 = (bool)(bVar11 | uStack_17134 == 0);
      piVar23 = local_8040 + uStack_17134 * 0x200;
      do {
        FUN_1005ffd0(iVar19,(int)local_c040,(double *)&uStack_17110,(int)piVar23,iStack_17138);
        iVar18 = uStack_17134 - 1;
        if (iVar18 < 0) {
          if (pdStack_1712c == (double *)0x0) goto LAB_1007d03e;
        }
        else {
          piVar24 = local_8040 + iVar18 * 0x200;
          do {
            bVar12 = FUN_10060350(iVar19,(int)piVar24,piVar23);
            pdStack_1712c = (double *)CONCAT31(extraout_var,bVar12);
            if (pdStack_1712c != (double *)0x0) goto LAB_1007d088;
            iVar18 = iVar18 + -1;
            piVar24 = piVar24 + -0x200;
          } while (-1 < iVar18);
LAB_1007d03e:
          if ((!bVar11) || (iVar18 = uStack_17134 + 1, 0xf < iVar18)) goto LAB_1007d0a3;
          piVar24 = local_8040 + iVar18 * 0x200;
          while( true ) {
            bVar12 = FUN_10060350(iVar19,(int)piVar24,piVar23);
            pdStack_1712c = (double *)CONCAT31(extraout_var_00,bVar12);
            if (pdStack_1712c != (double *)0x0) break;
            iVar18 = iVar18 + 1;
            piVar24 = piVar24 + 0x200;
            if (0xf < iVar18) goto LAB_1007d0a3;
          }
        }
LAB_1007d088:
        if ((pdStack_1713c == (double *)0x0) || (iStack_17138 = iStack_17138 + 1, 1 < iStack_17138))
        goto LAB_1007d0a3;
      } while( true );
    }
    piVar23 = (int *)(param_1 + 0x3034);
    if (*(int *)(param_1 + 4) == 1) {
      *piVar23 = 4;
    }
    else {
      *piVar23 = 2;
    }
    FUN_1007bb80(piVar23,param_2,2,local_c040,adStack_17070 + 6,param_1 + 0x3018,local_c040,
                 param_1 + 0x3038,auStack_16040,*(undefined8 *)(param_1 + 0x3010));
    iVar18 = 0;
    if (0 < iVar21) {
      puVar22 = (undefined4 *)(param_1 + 0x3050);
      do {
        *puVar22 = auStack_16040[aiStack_12840[iVar18]];
        iVar18 = iVar18 + 1;
        puVar22 = puVar22 + 1;
      } while (iVar18 < iVar21);
      return;
    }
  }
  return;
LAB_1007d0a3:
  if ((pdStack_1713c == (double *)0x0) ||
     (pdStack_1713c = (double *)((int)pdStack_1713c + -1), pdStack_1712c != (double *)0x0))
  goto LAB_1007d0bf;
  goto LAB_1007cf20;
LAB_1007d0bf:
  if (0 < iVar19) {
    piVar23 = local_8040 + uStack_17134 * 0x200;
    pdVar17 = pdStack_1714c;
    for (iVar18 = iVar19; iVar18 != 0; iVar18 = iVar18 + -1) {
      *(int *)pdVar17 = *piVar23;
      piVar23 = piVar23 + 1;
      pdVar17 = (double *)((int)pdVar17 + 4);
    }
  }
  piStack_17140 = piStack_17140 + 1;
  pdStack_1711c = pdStack_1711c + 6;
  pdStack_1714c = (double *)((int)pdStack_1714c + 0x800);
  uStack_17120 = uStack_17120 + 1;
  if (1 < (int)uStack_17120) {
    if (uStack_17130 == 1) {
      uStack_17130 = (uint)((double)CONCAT44(uStack_170d4,uStack_170d8) <=
                           (double)CONCAT44(uStack_170dc,uStack_170e0));
    }
    bVar11 = false;
    iVar18 = 0;
    do {
      if (((((bVar11) ||
            (iVar20 = iVar18 + uStack_17130 * 2, adStack_170a0[iVar20 * 3] < adStack_170d0[0])) ||
           (adStack_170d0[3] < adStack_170a0[iVar20 * 3])) ||
          ((adStack_170a0[iVar20 * 3 + 1] < adStack_170d0[1] ||
           (adStack_170d0[4] < adStack_170a0[iVar20 * 3 + 1])))) ||
         ((adStack_170a0[iVar20 * 3 + 2] < adStack_170d0[2] ||
          (adStack_170d0[5] < adStack_170a0[iVar20 * 3 + 2])))) {
        bVar11 = true;
      }
      else {
        bVar11 = false;
      }
      iVar18 = iVar18 + 1;
    } while (iVar18 < 2);
    if (bVar11) {
      if (0 < iVar19) {
        piVar23 = local_9040 + uStack_17130 * 0x200;
        piVar24 = local_8040;
        for (iVar18 = iVar19; iVar18 != 0; iVar18 = iVar18 + -1) {
          *piVar24 = *piVar23;
          piVar23 = piVar23 + 1;
          piVar24 = piVar24 + 1;
        }
      }
      uStack_17134 = 0;
      bVar11 = false;
      pdStack_1713c = (double *)0x8;
      do {
        aiStack_17118[uStack_17130] = param_2 + -1;
        fVar25 = FUN_100608b0(aiStack_17118 + uStack_17130,iVar19,(int)local_f040,
                              (int)(adStack_17070 + 6),(int)(local_8040 + uStack_17134 * 0x200),
                              adStack_170d0,(int)(adStack_170a0 + uStack_17130 * 6),
                              (int)&uStack_17110);
        *(double *)(&uStack_170e0 + uStack_17130 * 2) = (double)fVar25;
        uStack_17134 = uStack_17134 + 1 & 0x8000000f;
        if ((int)uStack_17134 < 0) {
          uStack_17134 = (uStack_17134 - 1 | 0xfffffff0) + 1;
        }
        iStack_17138 = 0;
        bVar11 = (bool)(bVar11 | uStack_17134 == 0);
        piVar23 = local_8040 + uStack_17134 * 0x200;
        do {
          FUN_1005ffd0(iVar19,(int)local_c040,(double *)&uStack_17110,(int)piVar23,iStack_17138);
          iVar18 = uStack_17134 - 1;
          if (iVar18 < 0) {
            if (pdStack_1712c == (double *)0x0) goto LAB_1007d35f;
          }
          else {
            pdStack_1714c = (double *)(local_8040 + iVar18 * 0x200);
            do {
              bVar12 = FUN_10060350(iVar19,(int)pdStack_1714c,piVar23);
              pdStack_1712c = (double *)CONCAT31(extraout_var_01,bVar12);
              if (pdStack_1712c != (double *)0x0) goto LAB_1007d3ae;
              pdStack_1714c = (double *)((int)pdStack_1714c + -0x800);
              iVar18 = iVar18 + -1;
            } while (-1 < iVar18);
LAB_1007d35f:
            if ((!bVar11) || (piStack_17140 = (int *)(uStack_17134 + 1), 0xf < (int)piStack_17140))
            break;
            piVar24 = local_8040 + (int)piStack_17140 * 0x200;
            while( true ) {
              bVar12 = FUN_10060350(iVar19,(int)piVar24,piVar23);
              pdStack_1712c = (double *)CONCAT31(extraout_var_02,bVar12);
              if (pdStack_1712c != (double *)0x0) break;
              piStack_17140 = (int *)((int)piStack_17140 + 1);
              piVar24 = piVar24 + 0x200;
              if (0xf < (int)piStack_17140) goto LAB_1007d3c2;
            }
          }
LAB_1007d3ae:
          iStack_17138 = iStack_17138 + 1;
        } while (iStack_17138 < 2);
LAB_1007d3c2:
      } while ((pdStack_1713c != (double *)0x0) &&
              (pdStack_1713c = (double *)((int)pdStack_1713c + -1), pdStack_1712c == (double *)0x0))
      ;
    }
    iVar18 = 2;
    pdVar17 = adStack_17070 + uStack_17130 * -6 + 1;
    pdVar13 = adStack_170a0 + uStack_17130 * 6 + 1;
    do {
      iVar18 = iVar18 + -1;
      pdVar17[-1] = pdVar13[-1] + (double)CONCAT44(uStack_170f8._4_4_,(undefined4)uStack_170f8);
      *pdVar17 = (double)CONCAT44(uStack_170ec,uStack_170f0) + *pdVar13;
      pdVar17[1] = pdVar13[1] + (double)CONCAT44(uStack_170e4,uStack_170e8);
      pdVar17 = pdVar17 + 3;
      pdVar13 = pdVar13 + 3;
    } while (iVar18 != 0);
    piVar23 = (int *)(param_1 + 0x3034);
    *piVar23 = aiStack_17118[uStack_17130] + 1;
    if (*(int *)(param_1 + 4) == 1) {
      *piVar23 = 4;
    }
    FUN_1007bb80(piVar23,param_2,iVar19,local_c040,adStack_17070 + 6,param_1 + 0x3018,
                 adStack_17070 + uStack_17130 * -6,param_1 + 0x3038,auStack_16040,
                 *(undefined8 *)(param_1 + 0x3010));
    if ((*(int *)(param_1 + 4) == 1) && (*piVar23 == 3)) {
      *piVar23 = 4;
    }
    iVar18 = 0;
    if (iVar21 < 1) {
      return;
    }
    puVar22 = (undefined4 *)(param_1 + 0x3050);
    do {
      *puVar22 = auStack_16040[aiStack_12840[iVar18]];
      iVar18 = iVar18 + 1;
      puVar22 = puVar22 + 1;
    } while (iVar18 < iVar21);
    return;
  }
  goto LAB_1007cf10;
}

