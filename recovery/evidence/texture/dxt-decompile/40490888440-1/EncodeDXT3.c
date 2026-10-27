/* Original DLL evidence only; RVA 0x57f00; not recovered business source. */

void __thiscall FUN_10057f00(void *this,int param_1,uint *param_2,undefined4 *param_3)

{
  uint uVar1;
  uint *puVar2;
  undefined4 local_f8;
  undefined4 local_f4;
  undefined4 local_f0;
  int local_ec;
  int local_e8;
  int local_d4;
  undefined4 local_ac;
  undefined4 local_a4;
  undefined4 local_a0;
  undefined4 local_9c;
  undefined4 local_98;
  undefined4 local_94;
  undefined4 local_7c [31];
  
  memset(&local_f8,0,0x7c);
  local_ec = *(int *)(param_1 + 8);
  local_f0 = *(undefined4 *)(param_1 + 4);
  local_e8 = local_ec << 4;
  local_f8 = 0x7c;
  local_d4 = param_1;
  local_f4 = 0x180e;
  local_ac = 0x41;
  local_a4 = 0x20;
  local_a0 = 0xff0000;
  local_9c = 0xff00;
  local_98 = 0xff;
  local_94 = 0xff000000;
  memset(local_7c,0,0x7c);
  local_7c[0] = 0x7c;
  uVar1 = FUN_1005c290((int)&local_f8,2);
  *param_2 = uVar1;
  puVar2 = (uint *)new__(uVar1);
  *param_3 = puVar2;
  FUN_1005c790(this,(int)&local_f8,(int)local_7c,puVar2,2,*param_2);
  return;
}

