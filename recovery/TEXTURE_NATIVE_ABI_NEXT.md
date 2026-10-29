# 当前：PC设备与内存资源ABI完整能力组验收中

设备源码入口 `src/TextureCompiler/Native/TextureNativeDeviceApi.cpp`：x86/stdcall工厂、Win32对齐/零初始化、线程ID/VirtualAlloc、引用计数和配套最终释放。原SDK使用type1；支持type!=2成功域，type2完整渲染状态尚未恢复，明确返回E_NOTIMPL。首次40908753738的DLL构建通过，导入库缺少stdcall修饰名失败；已保留裸名且补齐真正stdcall导出，待补验。

资源源码入口 `TextureNativeResourceApi.cpp`：原子引用计数、父资源0→1/1→0传递、GlobalAlloc/VirtualAlloc对象和存储所有权、原UnlockRect位操作。类型0..8的CPU资源及父子视图为成功域；type9和独立GPU/当前上下文surface最终释放显式拒绝并保留原对象。LockRect寻址/格式布局及完整混合模式身份仍待恢复。两库独立构建，不加入生产六DLL包。

本组新矩阵完整比较整个对象/逻辑命令存储和全部有效资源payload；仅不可跨进程稳定的设备地址/真实线程ID先检查原生语义再以固定标记记录。支持原/恢复完整捕获、Debug/Release一致及同量逐次NoOriginal重放和导入库消费者。设备1024工厂输入/16同存对象/64线程输入/6引用计数边界；资源8192解锁、72生命周期、16父子与16嵌套视图、32768并发原子操作及uint32边界。源码未验收前不能称完成。

已验收基础：部署后51组件0fa4acd3/40893243833成功；状态码/内存名称及导入库339656a2/40892135472成功且已普通合入，完整原档独立审计见handoffs/20261030-native-abi。首批新实现40908753738的native-errors/native-memory/base仍passed，按实际依赖保留这些成功锚点。

下一入口：单一texture-validation工作流只补失败/新组与实际变化消费者，不重跑SDK/MOD；收终态并独立审计完整捕获。随后恢复原D3DTexture.LockRect@0x1950→0xdb40→0xd70f→0xd680和0xd24b布局链及XG描述符实际消费；必要原版探针只查未知行为，不把probe当验收。仅PC、编译测试仅Actions、禁止强推。

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
