# 编译器优化：当前交接

更新：2026-10-27，北京时间；第四轮17:39开始，用户要求每轮最多120分钟并尽量跑满持续推进，2个子代理；效率优先，正确性是底线。
成果集中`feature/compiler-optimization`，不强推、不自行合并，PR #15保持草稿。编译/测试/微基准仅用Actions。
方案与边界：[README.md](README.md)；准确来源、失败与计量：[VALIDATION.md](VALIDATION.md)。

## 已验收基线

生产/测试/打包 `c65761fed6eed5ad71512e1cea2271eb49a4163f` / [40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571) 整体success：双配置各17个Core fixtures、原DLL/原BAB、协议、Texture、实际CLI、全部缓存/输入根、完整mymod/BIG与独立五文件ZIP通过。硬链接alias unlink已阻断，不以68旧成功代替。
本轮下载校验targeted artifact `12722102881` / SHA256 `dc4f5d4fc3a76ffc7aafe569ca136e881fe1eca1ca38cfecd28956225d56eae8`，source/run匹配。完整artifact `12723670985` 的GitHub digest见VALIDATION，本轮未独立下载校验其ZIP。

## 第四轮已接入组：组件通过，整体验收未完成

- W03：precompiled/base patch两个Core入口及Utility递归patch manifest/发现的StringHashes.xml消费封存流；保留旧API、共享fallback及取消传播。仅登记原实际File.Exists结果，发布前复核候选布尔存在性，冲突/终验变化拒绝本代；预算/输出根等incomplete明确，不声称原子快照或完整查询闭包，不关闭原缓存全根门槛。
- W05：有界scalar/string轨迹记录实际Define绑定/值/源/epoch/read-site、继承消费阶段、精确/派生候选及选择/歧义；canAuthorizeReuse=false，不反推依赖、不额外查询、不缓存值。
- W06子组：root私有map持有有序重复强引用与weak/闭包，manifest/排序/资产报告消费本流结果，成功Prepare发布不可变标量快照，失败不发布。执行期保留legacy live Handle，不泛称完整AssetSemanticResult/并行安全。
- W08：每DP弱生命周期有界exact程序；首遍原parser按原序求值，完整成功后保存语法树；暖路重做原运算、当前Define及文化转换。失败/超限不入缓存，128entry/262144字符/32768nodes与单表达式depth128等门槛保留，DisableExpressionPrograms可回退。

组件来源 `25dde80d4c9355fa67e64679bf1dedd47e5ee65c` / [40486677963](https://github.com/wu162/NewSdk/actions/runs/40486677963)：双配置20个fixtures全部通过，含Expr各1279检查/原SDK DLL264行四文化cold/warm/fallback与真实XML四输出。targeted artifact `12724659580`，下载SHA256 `97a7199e613fb94ac2518ba59fa3318cdbde1f8424c6f5c62154422b0f38e60d` 匹配source/run。
该run随后应用协议失败：新增测试给effectiveOptions.Source写临时标签，已改走实际Options.Parse的/data参数，schema严格枚举保持。生产源码已继续变化，不能用25d组件成功接受当前新组。
保留失败：88e114bc/40483925423缺新增Utility精确签名审计；cff/40484533010与6e54/40485040370仅manifest测试默认缓存配置缺失，f001修复不改断言。6e的W06双配置42检查/3组独立通过，原失败不改写。

## 当前追加检查点：文化复用与transfer buffers，尚未整体验收

- 共享只读en-US CultureInfo，DisableSharedExpressionCulture恢复每Value独立对象；原DLL矩阵扩展五种开关组合。7轮×每路径500次同进程交错微基准分别记录program共享文化、parser共享文化、program独立文化的原始耗时及实际parser/hit差，不报告CLI提速。
- W09仅copy/link scratch归每DP懒分配复用（1MiB/64KiB），保留原copy锁、短尾块拒绝与临时文件错误合同；静态fallback也懒分配，DisableOwnedTransferBuffers可回退。计量自有allocations/bytes与legacy请求分开，不放开backend租约，不声称整个W09/并行就绪。
- 新TransferBuffers fixture实际消费边界、大DataBlob三流、session恢复、失败/取消后重试及四输出fallback；App协议测试来源修复也在统一重验范围。新源必须完整重建与全部SDK/ZIP门槛，不能沿固定68归档复用。

下一步先跟进追加完整Actions终态并集中修复；成功前不标完整验收。继续有价值的输入一致性/状态隔离子组时保留中断检查点和来源区分。Texture隐藏读取、新完整bundle缓存、不可变语义结果与整个并行合同仍未完成；完整mymod仍1流，不外推全游戏/G3/中文Texture验收。
