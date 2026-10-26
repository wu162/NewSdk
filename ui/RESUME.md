# SWF 直接编译 · 续接入口

## 本轮阶段目标完成，先停在这里

分支 `feature/swf-direct-compiler`，起点 `87cf54e254dfb44a129da087316fd04806ed2b10`（recovery/compiler-source）。本目录独立管理交接，不读取 recovery 旧任务清单；运行经验和可复用源码继续使用。

用户最新收尾条件：框架搭好，提供的SWF能由原工具成功编译即可先结束；完整MOD编译器直接编译SWF、不生成中间文件是后续分支目标。

## 已验证的精确锚点

- 源码提交：`2568778f25db5826dcebb2b65a440315598d5d3f`。
- Windows Actions：[40286231285](https://github.com/wu162/NewSdk/actions/runs/40286231285)，最终success，job `121450427267`。机器记录：`ui/evidence/validation.json`；原运行artifact：`ui-memory-evidence`。
- 上游：`lanyizi/swf2apt@60b15a67a4cbd931f181af8c6c758bd7a8441ebc`，原始转换算法+原生flasm/zlib构建成功；宿主为net8.0-windows。
- 原工具源码版 `ra3=true/optimize=true` 编译样例成功：APT 10121字节、CONST 9787字节、DAT 48字节、XML 651字节。
- 受限内存原型与相同上游的 **optimize=false** 基线：CWS/FWS×en-US/zh-CN，共4组APT/CONST/DAT逐字节一致；5组拒绝检查通过；内存转换工作目录没有生成文件。
- **上传的exe没有直接运行，未证明固定源码与上传exe二进制一致。优化模式与no-opt内存原型没有被当成等价。SDK集成和游戏加载均未验证。**

首个运行40285855392在源码指纹检查失败：本地保存副本比原文件多一个尾部换行，已按原始源码正规化文本指纹修正。保留失败记录，不需重跑旧提交。验证后的交接/证据/CI路径过滤变更不修改编译算法，不能把旧运行标为新提交重新验证。

## 框架在哪里

- `ui/RESEARCH.md`：附件调用链、固定源码来源、内存化边界和BAB接入设计。按需读，不加载大历史。
- `ui/tools/prepare_memory_backend.py`：按精确源码指纹生成MemoryAptConverter，不修改上游checkout。生成源码位于 `ui/.work/`，不入库。
- `ui/MemoryAssets.cs`：内存结果与入口能力限制。当前仅SWF7、End/ShowFrame/Background/DoAction/ExportAssets/DoInitAction/空Sprite；不支持视觉、导入和非空时间轴扩展。
- `ui/tests/`：固定源码原工具、原生优化器与内存组件验证。
- `ui/fixtures/CoronaUIMain.swf`：原样例；SHA256 `b661542cf2c8b52ef846c348f817f523f5fba63ee4baa53c365aabde30af9fa7`。
- `.github/workflows/ui-memory.yml`：只检查UI项目；不触发旧XmlCompiler全量差分或真实SDK回归。

## 源码复用决定（2026-10-26 用户补充）

后续直接把 `lanyizi/swf2apt` 项目源码拷入本仓库 `ui/` 下使用并改造，不继续以构建时临时 checkout 和字符串补丁作为正式实现方式。以已经验证的 `60b15a67a4cbd931f181af8c6c758bd7a8441ebc` 为导入起点，保留原目录结构、来源提交、版权和各组件许可证；转换器、SwfLib、flasm及所需原生依赖一起纳入本地构建。先原样导入并复用/核对基线，再直接修改源码暴露内存接口。当前只是记录后续实施决定，源码尚未拷入，不能标为导入完成。

## 下一阶段顺序

1. **先做优化器内存化。** 原脚本使用 `--ra3 --optimize`。目前原优化结果APT=10121、CONST=9787，no-opt=25861/25179，二者确实不同，不能直接启用no-opt替代。保留flasm迭代、RA3字节码和错误语义，拿优化模式固定样例做差分。
2. **接入现有APT三资产。** Core插件ProcessInstance只返回单个AssetBuffer；前端应在GatherUnvalidatedInstances之前展开SWF成既有AptAptData/AptConstData/AptDatData。File可指向真实原SWF，让专用插件按类型选bundle中的内存payload并保留文件hash依赖。不能用mem://冒充现成支持。
3. 复用已有DataBlob布局/三数组经验，补专用APT插件和一次精简BAB对照，比较manifest/bin/relo/imp；普通旧APT文件输入仍保留原路由。未取得真实SDK结果前不改生产配置。
4. 之后再扩展geometry/纹理、字体、非空Sprite等；triangle和图像构造阶段也有落地行为，不能只去掉最后几个FileStream便宣称完整无中间文件。

约束：禁止强推；合入必须PR并经用户明确批准。本轮没有合入。不要重复已验证且依赖未变的测试；若只是交接文字变化，复用上述精确证据。CI只在代码、工具、样例或工作流变化时运行。原XML的File路径含临时目录，XML指纹只记录该次运行，不要求跨目录字节相同。
