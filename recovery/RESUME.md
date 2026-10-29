# 原 SDK 缓存 + 全认证后端并行迁移（验收中）

成果分支 `recovery/legacy-sdk-cache`，禁止强推；不合并其他主线，不发布 Release。

本轮从 `096f60db` 定点迁入参考 `054e8d9a` 的全部已认证范围：W3X 2 类型、受控 Audio/MP3、XML 284 metadata / 62 族、APT 4 类型。默认 jobs=auto，原 SessionCache/BuildCache/csc/psc 默认与 Texture 缓存保留。缓存命中先判断位置，不创建后端私有输入或新 Key/hash。Worker 独占输入和 CDATA，owner 验证/提交；跨文档任务在引用、重复 include、manifest/link 边界汇合。无新动作缓存。

实现检查点 `d9de5f69`；首个 Windows 批次 `eb35dc6b` / [40907675933](https://github.com/wu162/NewSdk/actions/runs/40907675933) 已通过 Debug/Release 构建，但组件生命周期出现失败；终态 failure 已确认仅新增 mesh 组件输入和既有 FileHashCache 反射构造测试失败；两处已修正。其 XML284×四平台、52个混合生产用例及原缓存/取消/Texture/BIG 独立组通过，但完整 mymod 与最终五文件因门槛未执行，不能宣称本轮通过。Linux Foundation/独立 App Roslyn 编译与六份混合 fixture 的 50 个原 SDK XSD 定义校验通过，仅作静态预检。

复查首批生产计量发现 LazyPlugin 包装器导致全部真实生产后端被精确类型门禁拒绝；已仅对未命中资产在 owner 解包真实内建插件，保留热态惰性优化，并新增生产真实 worker/重叠断言。15b26f4f / 40912075609 运行属于该修正前候选，不能作为最终并发证明。

主代理继续统一修复、完整验证及同机 Upheaval jobs1/4/auto 冷/热/单资产修改对照；性能脚本只在功能验收成功后启动，不复用旧 EXE 冒充新生产源码。详细状态与证据写在 [BACKEND_PARALLEL_MIGRATION.md](docs/BACKEND_PARALLEL_MIGRATION.md)，参数见 [COMPILER_USAGE.md](COMPILER_USAGE.md)，计量协议见 [PERFORMANCE.md](docs/PERFORMANCE.md)。旧缓存性能的历史入口为 [MOD_SCALE_TIMINGS.md](docs/MOD_SCALE_TIMINGS.md)，历史 Windows 40890949858 的冷75.286s/热2.864s不能代替新同机对照。
