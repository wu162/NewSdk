# 编译器优化：当前交接

更新：2026-10-27，北京时间；第四轮17:39开始，按用户最新要求每轮最多120分钟，2个子代理；效率优先，正确性是底线。
成果分支：`feature/compiler-optimization`；不强推、不自行合并，PR #15保持草稿。
编译/测试/微基准仅用GitHub Actions。方案索引：[README.md](README.md)；准确身份、失败和计量：[VALIDATION.md](VALIDATION.md)。

## 已验收基线：硬链接别名热修整体成功

生产/测试/打包源码 `c65761fed6eed5ad71512e1cea2271eb49a4163f` / [40479845571](https://github.com/wu162/NewSdk/actions/runs/40479845571) 已整体success：Debug/Release各17个Core fixtures、原DLL/原BAB、协议、Texture、实际CLI、全部缓存/输入根、完整mymod/BIG及独立五文件ZIP全部通过。
已下载核验targeted artifact `12722102881`，SHA256 `dc4f5d4fc3a76ffc7aafe569ca136e881fe1eca1ca38cfecd28956225d56eae8`，收据source/run匹配；双配置PhysicalInputs各111检查，hardlink alias unlink被阻断且同内容不同inode不误合并。
完整artifact `12723670985`，GitHub记录SHA256 `d004d4ae92f91e934ec7a1f0056a18e62f5af1ad85d30dfd56be8a0537592af3`；本轮未下载该大包，不冒称独立核验其ZIP字节。68旧成功与55失败历史保留于VALIDATION。

## 第四轮能力组：实现检查点，尚未验收

- W03：预编译/root与base patch两个Core入口接入封存流；Utility递归patch manifest及实际发现的StringHashes.xml消费同一封存字节。保留旧Load/Asset.Update签名、原共享fallback、错误及取消传播。未泛称隐藏插件/全部缓存物化输入封闭。
- 查询一致性：仅登记原来实际执行的File.Exists正负结果；解析不增加探测、不复用答案、不改变原选择。发布前VerifyUnchanged逐个复核候选布尔存在性，代内已观测冲突或终验变化拒绝当前结果；覆盖resolver与manifest发现/asset存在查询。8192路径、1Mi UTF-16字符上限；超限、非支持/输出路径明确incomplete；这是复核时点证据，不是系统原子快照或查询锁。
- W05：实际DefineRead保存源/值/安装epoch/read-site，实际继承消费保存同文档prevalidation/跨文档processed阶段；符号记录精确/派生查找、原集合顺序、缺失/选择/歧义，并关联查询。所有事件只有immutable scalar/string，沿用有界log及DisableDependencyObservations；canAuthorizeReuse=false。
- 测试准备：新增真实Utility+Core ManifestInputs fixture；PhysicalInputs补候选新增/删除/短暂冲突/预算/下一代，ApplicationBuild补stream完成后高优先级候选新增→发布前拒绝→新代重试；DependencyObservations补定义/继承/派生与跨流弱引用。新生产输入必须完整重建，不复用固定68二进制。

## 保留合同与下一步

保留legacy摘要和输出顺序、全部原始差分/拒绝门槛、/bc /sc全根复核及Texture缓存边界。W04仍每流读/Compile、构建内自有元数据1 build+2 restore；W08只减查找工作，不报告完整构建提速。
先统一启动本轮完整Actions并跟进到终态，失败集中修复；未通过前不得把本轮检查点称为已支持。后续依据本轮实际证据推进W06不可变结果等下一组；W09—10并行尚未完成。完整mymod仍仅1流，不泛称全游戏/G3/中文Texture新验收。
