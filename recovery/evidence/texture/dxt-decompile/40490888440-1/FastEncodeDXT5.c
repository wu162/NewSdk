/* Original DLL evidence only; RVA 0x8b570; not recovered business source. */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_1008b570(void *this,int *param_1)

{
  float fVar1;
  float fVar2;
  float fVar3;
  uint uVar4;
  int iVar5;
  int *piVar6;
  int iVar7;
  ulonglong uVar8;
  float local_24;
  undefined4 local_20 [8];
  
  local_24 = _DAT_10099c60;
  iVar5 = *param_1;
  local_20[0] = 0;
  local_20[1] = 2;
  local_20[2] = 3;
  local_20[3] = 4;
  local_20[4] = 5;
  local_20[5] = 6;
  local_20[6] = 7;
  local_20[7] = 1;
  fVar1 = _DAT_100a3ca0;
  if (0 < iVar5) {
    this = param_1 + 1;
    iVar7 = iVar5;
    do {
                    /* WARNING: Load size is inaccurate */
      if (*this < local_24) {
                    /* WARNING: Load size is inaccurate */
        local_24 = *this;
      }
                    /* WARNING: Load size is inaccurate */
      if (fVar1 < *this) {
                    /* WARNING: Load size is inaccurate */
        fVar1 = *this;
      }
      this = (void *)((int)this + 4);
      iVar7 = iVar7 + -1;
    } while (iVar7 != 0);
  }
  fVar3 = (float)_DAT_10098000;
  fVar2 = (float)_DAT_100a3c98;
  param_1[0x204] = (uint)(fVar1 * fVar3 + fVar2) & 0xffff;
  uVar4 = (uint)(local_24 * fVar3 + fVar2) & 0xffff;
  param_1[0x205] = uVar4;
  if (param_1[0x204] == uVar4) {
    iVar7 = iVar5 + -1;
    if (-1 < iVar7) {
      piVar6 = param_1 + iVar5 + 0x206;
      do {
        *piVar6 = 0;
        iVar7 = iVar7 + -1;
        piVar6 = piVar6 + -1;
      } while (-1 < iVar7);
      return;
    }
  }
  else {
    iVar7 = iVar5 + -1;
    if (iVar7 < 0) {
      return;
    }
    piVar6 = param_1 + iVar5 + 0x206;
    do {
      uVar8 = FUN_1004e830(this,param_1);
      param_1 = (int *)(uVar8 >> 0x20);
      iVar5 = (int)uVar8;
      if (iVar5 < 0) {
        iVar5 = 0;
      }
      else if (7 < iVar5) {
        iVar5 = 7;
      }
      this = (void *)local_20[iVar5];
      *piVar6 = (int)this;
      iVar7 = iVar7 + -1;
      piVar6 = piVar6 + -1;
    } while (-1 < iVar7);
  }
  return;
}

