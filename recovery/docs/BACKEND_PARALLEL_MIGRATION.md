# 原 SDK 缓存分支后端并行迁移

2026-10-30（上海时间）。目标 `recovery/legacy-sdk-cache`，起点 `096f60db`；参考 `feature/cache-parallel` / `054e8d9a`。

实现中，尚未完成 Windows 验收。资格一次接入 W3DAnimation/W3DMesh、受控 NONE/XAS AudioFile 和 MP3 passthrough、全部 284 XML metadata TypeId（62 族）以及四个 APT 数据类型。未认证类型继续 owner 编译。

缓存判定先于后端准备。命中沿用原 Commit(location)，存在在途任务时仅排有序提交占位。Worker 只处理独立输入；owner 验证、提交和完成文档。跨文档任务在重复 include、manifest/link 和 stream 完成前汇合。失败时先 join 所有 worker，再保存已完成的部分会话，最后释放 Core 状态。

`--jobs auto|N` 默认 auto；1 为原 owner 路径。线程数不进入旧缓存身份。原 sc/bc/csc/psc 默认 true；fsc/atc 默认 false。使用 Win32 可用物理/虚拟内存与 CPU 共同准入，任务队列和 retained bytes 有界。

未引入动作缓存、动作 Key、result.bin、额外文件哈希或输入全根快照。只暂存私有 CDATA，汇合后移动到原逻辑位置。

`cache.parallel` 在 performance summary/detailed 提供真实调用/重叠峰值、prepare/wait/compute/commit、降级原因和采样内存；detailed 最多记录 512 个调用区间。off 不建立逐类型字典、计时或区间；调度所需资源采样仍启用。

待验收：Debug/Release 生产构建、全认证矩阵、旧缓存冷热/失效/取消回滚、原字节差分、完整 BIG/SKUDEF/Additional、同机 Upheaval jobs1/4/auto 对照。旧分支证据只能说明未变后端基础，不能替代本次集成验收。
