# 当前：PC原生数值/端序/flags与真实复制消费均已验收

2026-10-30执行规则：下一批按PC文件加载、资源构造与真实复制消费链连续实现，开工先核对全部调用参数/端序/所有权；普通检查点不触发Windows，稳定后一次集中验收。CI期间可继续修改共享代码，但旧结果只认证原提交；当前输入须在后续稳定批次通过。不要逐helper建大矩阵、默认追加审计器或收尾plan。当前已验收锚点保留；本次仅更新文档，无新业务验收。见[效率规则](EFFICIENCY.md)。

入口 `src/TextureCompiler/Native/TextureNative{Device,Resource,Surface,Descriptor,Copy,Convert}Api.*`。独立x86 ABI DLL，不加入已部署六DLL包或冒充完整原混合模式身份。

| 组 | 完整原版验收锚点 | 范围 |
| --- | --- | --- |
| native-device/resource |44972e43 /40911147535|非type2设备、CPU资源/所有权/引用释放，GPU/type9拒绝|
| native-surface |675b00d6 /40912274334|131072布局及同量LockRect，packed/tiled/border/mip/array|
| native-descriptor/consumers |118d9500 /40913579297|135168头/405504完整描述符；12真实资源/107级别/321锁及全部存储|
| native-copy |6af9af18 /40965216216|32768完整identity输入，64格式/区域/正负步幅/flags/padding/guard|
| native-convert |6af9af18 /40965216216|100412完整unsigned输入/64.7MB，60全16位源条带|
| native-signed-convert |6af9af18 /40965216216|100472 signed/biased normalized/integer输入/61.6MB，120条带/7864320像素|
| native-float-convert |6af9af18 /40965216216|46262输入/57.9MB；38布局/1444格式对/8属性/4映射；全部half、float指数/符号、signed midpoint邻点|
| native-endian-convert |6af9af18 /40965216216|184928输入/16端序对/所有数值布局/子区域、32全16位条带及负步幅控制|
| native-copy-consumers |6af9af18 /40965216216|五DLL/12真实资源/107级别/321锁；40 raw级别120视图按原LoadImageA 0x12/0调用复制3772098像素|
| native-flags-convert |6af9af18 /40965216216|49328输入/12 flags/同格式强制归一化；80完整条带/5242880像素|

成功组均D/R原版完整字节比较、NoOriginal及适用真实导入库。543cb4cb/40959808438首版166个1位阈值差异保留；修复是原sign-selected half而非固定+0.5。5转换组全原档均已独立模型复审；identity既有完整模型按原captureSHA保留，真实复制全部字节/资源/五DLL也通过独立审计，不把第一版failure写成success。最终8组passed/55复用/63当前组件有效，见handoffs/20261030-native-conversion/final-archive-audit.json。

端序源证据：原PC真实raw资源desc.format含0x80（8-in-32），canonical LoadImageA目标是0x1a200006，故新组优先补真实消费。LoadImageA IL还明确flags=0x12/threshold=0，最终真实调用链使用相同参数；raw flags仅按原规则切换identity或强制转换，压缩/packed特殊flag含义仍在排除域。四种端序按原0x3ed3d/0x3ee8c单位交换；不完整单位会遗留未初始化尾部，恢复版明确拒绝，不冻结随机字节。

保留边界：gamma/压缩转换、packed float10、uint32数值转换、过滤/重叠、signed/biased one-bit仍拒绝；D3DXCreateTextureFromFileA@0x13c82→0x13b53加载/资源头构造、设备type2/type9及完整混合模式身份未恢复。fixture loader只提供原版输入，不算恢复的图像加载器。下一功能从该PC文件加载链继续，不重复旧成功SDK/MOD；仅PC、编译测试仅Actions、禁止强推。

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
