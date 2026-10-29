# 当前：设备、资源与LockRect已验收；描述符与真实原生消费链补验中

本轮新增实现独立保存于 `src/TextureCompiler/Native/TextureNative{Device,Resource,Surface,Descriptor}Api.*`；不加入已部署生产六DLL包。

- Device：PC插件使用的type1及type!=2工厂、完整对象/线程ID/命令分配、引用计数和配套释放，44972e43/40911147535 passed，完整原档独立模型审计通过。type2渲染状态仍明确E_NOTIMPL。
- Resource：原子AddRef/Release、CPU存储与对象所有权、父子/嵌套视图、UnlockRect，44972e43/40911147535 passed，完整原档独立模型审计通过。type9/GPU/当前上下文surface最终释放明确拒绝并保留原对象。
- Surface：完整type3纹理布局与LockRect，64格式/四维度/mip/packed/border/tiled/array/rect/flags及实际VM读写，675b00d6/40912274334 passed。独立审计全部131072布局/同量锁状态及全部实际存储，不归一化产物。
- Descriptor：level32/volume28/XG60完整输出已实现，40912274334因type18/19深度分派误读差分失败；完整原捕获保留。现修复type18取头部高位/type19深度1，135168输入/405504输出待补验。
- Consumers：固定12个真实DDS/TGA，原DLL仅捕获原生资源fixture，原/恢复独立重建所有分配，四库连续执行全部级别/三种Lock+Unlock/描述符/引用与释放，完整头和所有存储逐字节比较及NoOriginal，尚未验收。这个fixture捕获器不是恢复的图像加载器。

当前按RESUME与最新Actions终态收证。各新组使用独立原生DLL、原DLL完整SHA与入口检查、Debug/Release完整对照/逐调用隔离和真实导入库消费者。原进程地址/线程ID先核对实际语义后在测试捕获中标记，其他全部字节精确比较；源码没有gold图像查表、样本ID分派或原DLL回调。

审计工具 `InspectTextureNativePcCaptures.py`，已验收完整审计位于evidence/texture/handoffs/20261030-native-abi。仅PC，编译测试仅Actions，禁止强推。下一业务为实际PC原生加载/XGCopySurface及完整混合模式ABI，不把独立辅助接口成功等同整个原DLL还原。SDK/MOD按明确里程碑降频，已部署包不因本轮独立API变更重装。

以下为早期历史记录。



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
