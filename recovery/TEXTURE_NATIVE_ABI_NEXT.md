# PC原生接口续接：状态码函数已实现，等待完整差分

用户要求继续完成剩余任务并尽量推进70分钟。只考虑PC，原版未初始化行为仍不伪造成确定输出；完整native/混合模式ABI尚未完成。

## 本轮新增实现

`TextureGetErrorString(int32_t)`以独立x86 C ABI导出，恢复原`getErrorString`@RVA0x508D0的全部-100..0映射、原ASCII/尾部空格/NUL、未知码默认值与静态字符串对象共享。完整静态分支/跳转表/字符串地址与固定原SHA见`evidence/texture/native-errors-static/20261029/mapping.json`。实现不调用原DLL；函数本身就是状态码到固定业务错误文本的映射，不是图像gold输出查表。

`BuildTextureNativeApi.ps1`构建独立`Recovered.TextureNativeApi.dll`，要求x86、零入口/零导入。该DLL不加入已验收的六DLL生产包，现有生产codec输入不变。原始CLR Module方法token和整个原设备/CRT ABI不因此恢复。

`native-errors`完整组含Debug/Release，每配置12357状态输入（完整守卫区、int32边界、位模式与固定随机）、10404原/恢复指针共享关系、完整原字节保存和同量逐次NoOriginal隔离重放；原DLL SHA和函数头均强校验，拒绝缺失/截断/额外记录。当前尚未编译运行，不计验收完成。

## 下一轮具体入口

收集中工作流中新native-errors组终态及完整capture/native DLL，独立核对全部12357记录、D/R原字节一致与证据hash。失败只修该真实失败/变化组，未变成功SDK不重跑。下一原生能力需从完整methods.json与PC生命周期的CreateDevice/资源/LockRect/UnlockRect/XG描述符链按连贯组推进，尚未实现的设备接口不能用常量桩冒充。

## 本轮追加：独立原生内存接口（尚未验收）

`Recovered.TextureNativeMemory.dll`提供12字节x86 SimpleBuffer的原thiscall构造/Reset/Append/带flag的scalar deleting destructor，保留ECX和callee stack cleanup；配套分配/释放不跨CRT，并导出既有endianness/cdecl Release/stdcall Create9常量facade。状态码DLL仍零导入；内存DLL需要自己的正常CRT初始化，只禁止原目标DLL导入，不能错误要求有分配器的DLL零入口。

完整组每配置10组buffer序列、520次追加、8个析构flags、262144个端序输入，逐字段/全部有效payload原版差分、D/R完整trace与逐次NoOriginal重放。成功域限定有效指针/不重叠源与有界非零初始capacity，未定义/不终止及OOM/跨CRT释放不冒充等价；完整设备创建/释放链仍待恢复，不能以两个已知常量facade代替。该新API不加入已验收生产六DLL包。
