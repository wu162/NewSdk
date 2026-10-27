/* Original DLL evidence only; RVA 0x83cd0; not recovered business source. */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_10083cd0(void *this,double *param_1)

{
  uint uVar1;
  void *this_00;
  float10 fVar2;
  float10 fVar3;
  float10 fVar4;
  float10 fVar5;
  float10 fVar6;
  float10 fVar7;
  float local_e1a4;
  double local_e1a0;
  double local_e198;
  undefined1 local_e180 [14416];
  undefined1 local_a930 [14416];
  undefined1 local_70e0 [14416];
  undefined1 local_3890 [14412];
  undefined4 uStack_44;
  
  uStack_44 = 0x10083ce0;
  if (*(int *)(param_1 + 0x606) == 0x44) {
    switch(*(undefined4 *)param_1) {
    case 0:
    case 2:
    case 3:
    case 0x18:
      FUN_1008c8a0(this,param_1,4);
      break;
    case 1:
      FUN_1008c1f0(this,param_1);
    }
    if (*(int *)((int)param_1 + 4) == 1) {
      return;
    }
    memcpy(local_e180,param_1,0x3850);
    fVar2 = FUN_1007be60((int *)param_1);
    local_e1a0 = (double)fVar2;
    switch(*(undefined4 *)param_1) {
    case 0:
    case 2:
    case 3:
    case 0x18:
      FUN_1008c8a0(this_00,param_1,3);
      break;
    case 1:
      FUN_1008baa0(this_00,param_1);
    }
  }
  else {
    if (*(int *)((int)param_1 + 4) != 1) {
      if (*(int *)((int)param_1 + 4) != 0) {
        FUN_1007c420(this,(int)param_1,4);
        memcpy(local_e180,param_1,0x3850);
        fVar2 = FUN_1007be60((int *)param_1);
        FUN_1007c420(this,(int)param_1,3);
        memcpy(local_70e0,param_1,0x3850);
        fVar3 = FUN_1007be60((int *)param_1);
        fVar4 = (float10)_DAT_100a3a48;
        local_e198 = _DAT_100a3a48;
        local_e1a0 = _DAT_100a3a48;
        if ((*(int *)(param_1 + 0x606) == 0x48) ||
           (fVar5 = fVar4, fVar6 = fVar3, *(int *)(param_1 + 0x606) == 0x47)) {
          FUN_1007bdf0((undefined4 *)param_1,3);
          memcpy(local_3890,param_1,0x3850);
          fVar4 = FUN_1007be60((int *)param_1);
          local_e198 = (double)fVar4;
          FUN_1007bdf0((undefined4 *)param_1,4);
          memcpy(local_a930,param_1,0x3850);
          fVar4 = FUN_1007be60((int *)param_1);
          local_e1a0 = (double)fVar4;
          fVar5 = (float10)local_e198;
          fVar6 = (float10)(double)fVar3;
        }
        local_e1a4 = _DAT_100a3a80;
        fVar7 = (float10)(double)fVar2;
        if (((fVar7 < fVar4 == (fVar7 == fVar4)) || (fVar7 < fVar6 == (fVar7 == fVar6))) ||
           (fVar7 < fVar5 == (fVar7 == fVar5))) {
          if (((fVar6 < fVar4 == (fVar6 == fVar4)) || (fVar6 < fVar7 == (fVar6 == fVar7))) ||
             (fVar6 < fVar5 == (fVar6 == fVar5))) {
            if (((fVar6 < fVar4) || (fVar4 < fVar7 == (fVar4 == fVar7))) ||
               (fVar4 < fVar5 == (fVar4 == fVar5))) {
              if (((fVar5 <= fVar6) && (fVar5 <= fVar7)) && (fVar5 < fVar4 != (fVar5 == fVar4))) {
                memcpy(param_1,local_3890,0x3850);
                local_e1a4 = (float)local_e198;
              }
            }
            else {
              memcpy(param_1,local_a930,0x3850);
              local_e1a4 = (float)local_e1a0;
            }
          }
          else {
            memcpy(param_1,local_70e0,0x3850);
            local_e1a4 = (float)fVar3;
          }
        }
        else {
          memcpy(param_1,local_e180,0x3850);
          local_e1a4 = (float)fVar2;
        }
        if (*(int *)(param_1 + 0x606) != 0x48) {
          return;
        }
        if (local_e1a4 <= *(float *)(*(int *)((int)this + 0x18) + 4)) {
          return;
        }
        FUN_100833f0(param_1,(double)local_e1a4);
        return;
      }
      FUN_1007c420(this,(int)param_1,3);
      if (*(int *)(param_1 + 0x606) == 0x45) {
        return;
      }
      fVar2 = FUN_1007be60((int *)param_1);
      memcpy(local_e180,param_1,0x3850);
      FUN_1007bdf0((undefined4 *)param_1,3);
      fVar3 = FUN_1007be60((int *)param_1);
      if (fVar3 < (float10)(double)fVar2) {
        return;
      }
      memcpy(param_1,local_e180,0x3850);
      return;
    }
    uVar1 = *(uint *)param_1;
    if (uVar1 < 0x19) {
      switch(uVar1) {
      case 0:
      case 2:
      case 3:
      case 0x18:
        FUN_1008c8a0((void *)(uint)(&switchD_10083dbb::switchdataD_1008411c)[uVar1],param_1,4);
        break;
      case 1:
        FUN_1008c1f0((void *)(uint)(&switchD_10083dbb::switchdataD_1008411c)[uVar1],param_1);
      }
    }
    if (*(int *)(param_1 + 0x606) == 0x45) {
      return;
    }
    memcpy(local_e180,param_1,0x3850);
    fVar2 = FUN_1007be60((int *)param_1);
    local_e1a0 = (double)fVar2;
    FUN_1007c420(this,(int)param_1,4);
  }
  fVar2 = FUN_1007be60((int *)param_1);
  if (fVar2 < (float10)local_e1a0 != (fVar2 == (float10)local_e1a0)) {
    return;
  }
  memcpy(param_1,local_e180,0x3850);
  return;
}

