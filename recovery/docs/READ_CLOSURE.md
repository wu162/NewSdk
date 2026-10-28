# B1：受控读取清单第一阶段

更新：2026-10-29（北京时间）。分支 `feature/cache-parallel`，合同见 [B1](HOT_CACHE_OPTIMIZATION.md)。本阶段接入实际生产读取、清单复验和持久事务，**尚未认证完整闭包，不替换全根保护，不用于文档或stream复用**。旧session、强ActionKey、三缓冲/CDATA、后端准入及有界调度保持原合同。

## 生产路径

`CacheInputBoundary.BeginObservation` 是owner线程作用域，独立于既有根边界。resolver每次实际尝试候选时报告存在状态，包含较高优先级的缺失候选及ART postfix；成功选择后的无关后续候选不伪造为读取。App按路径去重并保留首次观察顺序；这不是保存每次查询完整有序树的通用解析图。

主XML/项目、XInclude、schema及Core文件哈希的实际读取交给App。App打开 `FileShare.Read` 文件，计算SHA256并回卷，consumer读取同一未关闭的handle；不先hash路径再让consumer重开。XML reader保留base URI并负责释放输入。后端APT/Audio等已有经验证租约的外读复用其原handle计算的摘要，由 `ReportLeasedRead` 回传；lease仍持有到既有owner汇合/提交，不增加worker资格，不在worker传播ThreadStatic观察器。

同一stream若同路径存在状态或已读内容身份矛盾，立即拒绝；stream结束时鲜读所有已消费文件的SHA256并检查负候选状态。相同size/mtime不作为身份。原全根首尾盘点仍执行，所以无关SDK成员变化目前仍受旧保护约束。未知native、目录决策及旧session/precompiled/历史哈希读显式保守；完整闭包资格恒为false。

## 清单格式与事务

内层文件 `dependencies.bin` 为明确的小端二进制，不使用对象序列化：

| 顺序 | 内容 |
| --- | --- |
| 头 | int32 magic `0x31434452`（RDC1）、int32 version=1、int32路径数、int32未知原因数 |
| 每路径 | int32 UTF-8字节数及严格UTF-8路径、byte存在状态0/1、int64内容长度；长度-1表示仅存在探测，非负长度随后有32字节SHA256 |
| 未知原因 | int32字节数及严格UTF-8字符串；去重，不能授予复用资格 |
| 尾 | 前面全部字节的32字节SHA256 |

路径字符串去重；每个路径最多一个内容身份，没有payload/base64/XML DOM。最多100000条路径和原因合计，单字符串131068字节，整个文件16MiB以内；累计预算先检查再修改集合。生产观察器只对专用预算异常停止记录并退回原保守读取，清单不落盘，报告单列`budgetFallbackStreams`；真实IO、内容/候选矛盾及取消继续传播。这里没有用partial清单替代全根保护。解析先校验文件长度与尾摘要，再检查版本、计数、文本、路径、布尔值、长度、重复和剩余范围；不允许把尾摘要读成记录。非法UTF-8/路径输入统一作为无效数据拒绝。空记录取消仍生效。

整个内层文件的SHA256为持久key。外层复用现有 `ActionAssetCache` 的成员摘要、私有候选、pending排除、Commit/Complete和替换回滚，而非建立第二套事务引擎。独立目录 `.cache/read-closure-v1/<key>/`，数据在 `files/00000000`；`index.bin` 仍为原store成员合同。容量64MiB/256条，pending、备份和候选遵循原store规则；未确认记录不可作为成功缓存。与可写发布输出不共享字节。

当前key只标识观察内容，不是可查询的文档/stream复用身份；尚缺配置/选项/工具/查询上下文绑定和认证locator。不能扫描到一份同路径记录就绕过正常准备、引用/schema验证或后端。

## 计量与验证

报告 `cache.dependencyObservation` 使用格式 `NewModCompiler.ReadClosure.v1`，记录streams/budgetFallbackStreams/probes/files/unknownReads、直接hash字节、复用现有lease摘要的字节、复验字节、二进制长度、store hash/copy和manifestKeys。直接观察与复验hash纳入总 `hashReadBytes`；leased身份已经由原 `ExternalHashBytes` 计量，不再重复相加。新增store的hash/copy纳入总账本，并单列，避免隐藏记录成本。

read/validation/serialization ticks是包含关系可能重叠的Stopwatch时间；其frequency/semantics随记录返回。读/hash包含在stream准备或文档处理内，复验/持久化在stream完成前；不能与父计时求和。这里新增了记录成本，没有声称热缓存变快。

现有App测试覆盖确定往返、有效摘要的畸形记录、预算拒绝后重试、取消和句柄；Core真实fixture覆盖候选优先级、XML/Include/XInclude/schema与租约回调隔离；CLI缓存回归同时断言清单落盘/pending可见性和失败后缓存字节不变。最终Windows运行、产物身份及体积数据见 [当前交接](../REFACTOR_RESUME.md) 和证据JSON；静态检查不代替验收。

## 下一组的具体前置条件

优先认证 `/sc:false` 的纯metadata stream，保持APT/Audio/Texture与未知plugin保守。必须绑定冻结配置、有效选项、工具/进程身份和stream入口；逐任务读取合同包含准备拒绝/兼容回退；precompiled与base patch搜索完整记录或明确排除；runtime/toolchain和MOD/Additional仍保持适用保护。通过无关SDK成员变化与高优先级候选出现的成对用例后，才能对该认证路径替换全根检查。随后推进文档准备态、必要副作用重放及完整stream快路；不以本阶段记录成功冒称已完成这些能力。
