# 公开 S3 显式三色状态适配

实验分支 `validation/texture-public-s3-dxt1`；正式目标 `recovery/texturecompiler-source`。实验代码尚未合入或接入业务，默认DXT1/1A仍拒绝。原默认路径有[栈依赖反例](TEXTURE_DXT1_BOUNDARY.md)，不能用一个自定模式覆盖。

在已验收的公开S3源码上继续修改，而非仅测试原样上游：加入显式level3搜索、三点floor调色板/extended x87最终评分、三色快速候选和严格误差选择；灰度常量来自既有恢复源码D_000B9608首8192字节，PROVENANCE.json记录源指纹，不是语料gold输出表。候选不回调原/既有恢复编码器，没有样本ID分派。

首轮源码 `fbc82056f286bf9e7e8ead98655baa6a56d3c4a1` / [40670148058](https://github.com/wu162/NewSdk/actions/runs/40670148058) completed/success。该success是实验比较及旧消费回归完成，**不是三色替换验收**。收据status=comparison-completed、productionAccepted=false。

Debug/Release各5312份完整0x3850初始化状态：format0/1、flag0、mode0/3、count1/2/3/4/8/16、256灰度、固定图案及1024固定随机/归一化float。4862完全一致、450不同；原/候选完整状态及输入归档，独立NoOriginal每配置5312回放/76620292检查通过；Debug/Release完整records SHA256相同。所有450例集中format0/mode0：447仅索引首差，3端点首差。两个格式的显式core3和format1自动选择全部一致。

同批8个受影响完整DXT3/5消费组D/R/NoOriginal通过，24未变组复用；未运行SDK。不能把实验组件证据复用为正式分支或SDK新验收。

原RmsError@0x7BE60的静态规则（owned ASM Q_1007BF3D..Q_1007BF68）解释了剩余格式差异：level3时format0仍检查第4个不透明黑点，非零格式只检查3个点，DXT1A透明索引不参与RGB误差。后续源码`091239f2870001422073ac31ad1aac9935511615`已将此通用格式规则用于full/quick候选评分与reindex，运行[40671760128](https://github.com/wu162/NewSdk/actions/runs/40671760128) completed/success：D/R各5309/5312完整状态一致，修复447例；NoOriginal各5312回放/76620292检查，8个旧消费完整组仍通过、24组复用。比较仍为comparison-completed/productionAccepted=false，未运行SDK。

余下仅format0/mode0的`f0-random457`、`f0-random477`、`f0-random719`，首差均0x3038。逐字节复核证实三例只是两个RGB565端点反序、索引0/1互换；交换后整个0x3850状态完全相同，索引2/3与其他字节未变。**不能据此宣称完整DDS已相同**：下一步先补原完整DXT1A透明包装及字节产物gold，并核对其规范化是否消除该内部表示差异，不重复宽泛x87/预算实验，也不删掉这3例的真实状态差异。

两轮完整比较档案已集中保存到目标分支 `evidence/texture/public-s3-three/{40670148058,40671760128}-1`；[分类统计](TEXTURE_PUBLIC_S3_THREE_RESULTS.json)保留原sourceCommit/runId和SHA，不改写为正式分支生产源码。源码和补丁前后指纹继续在实验分支。

即使初始化状态达到5312/5312，下一门槛仍是完整DXT1A透明/全透明包装、原阈值、浮点/partial映射与端点/索引规范化，以及适用DDS/mip/volume/插件/SDK。不能把状态组件一致扩大为默认DXT1或完整DXT1A支持。

## 2026-10-28 包装优先续接（当前比较中）

已按不可变原包装 `CompressDispatch.c` 的端点相等/排序与三色0/1映射，离线逐条分析40671760128的完整状态归档：D/R各5312个compact状态打包全部相同，原始完整状态仍保持5309/5312与3个0x3038差异。可复现脚本为 `scripts/SummarizeTexturePublicS3Packing.py`，[分析收据](evidence/texture/public-s3-three/packing-40671760128-1/analysis.json)绑定原归档SHA256；这只证明compact状态打包规则，不包含实际尺寸、alpha剔除或DDS头，不能冒充完整DDS验收。

独立实验分支已新增透明DXT1A包装比较，源码 `792aff38a3dfbdda015eddda41f44d03d74e12c9` / [40679136957](https://github.com/wu162/NewSdk/actions/runs/40679136957)，启动时尚在运行。每配置4608个float块、3456个完整byte DDS，覆盖9种partial形状、全透明、灰度、单色/随机及0.5阈值两侧；独立NoOriginal重放完整输入/原输出/候选输出。仅测试入口消费公开三色export，无生产路由变更，opaque DXT1A与默认DXT1仍拒绝。只选择新增evidence-only组；旧成功组/SDK保持原锚点。运行终态和差异待收取，不预判成功。

### 透明完整块与DDS首个闭环

包装修正源码 `2ebb72e3a7f74628c0071929a06cd63256224272` / [40692652134](https://github.com/wu162/NewSdk/actions/runs/40692652134) completed/success：D/R各4608个float/partial完整8字节块和3456份完整136字节DDS全部一致，逐次NoOriginal重放也全部一致。默认阈值实测0.5，覆盖9种1/2/4尺寸、全透明、灰度、单色、随机和阈值邻近float，源不变与原输出canary保留；该组为comparison-completed/productionAccepted=false，尚未启用业务。完整归档及收据已集中在 `evidence/texture/public-s3-dxt1a/40692652134-1`。

首轮40679136957保留failure：新宿主误断言显式三色搜索必返回level3。不可变旧gold显示每格式/模式各222例实际返回level4；原包装在端点相等时才强制三色，否则按实际levels排序并映射。修复包装后全量比较相同，没有修改公开S3搜索、屏蔽样本或放宽产物比较。真实剩余3例内部orientation差异保持原样。

下一批源码 `5e994ac0db6bcf1c4c226bba8cd4e7d26b85c19e` / [40693591607](https://github.com/wu162/NewSdk/actions/runs/40693591607)运行中：将实验块实现统一成独立测试源码，并新增3648个二维/volume、无mip/完整mip输入；遇任一实际编码块无透明样本则明确拒绝且不调用不确定原路径。接受域保存全部原始byte输入和完整双方DDS，逐次NoOriginal重放，同时重新验证4608/3456块组和完整byte源/原options档案。复用已验收2D base-relative与volume递归float mip helper，绝不调用既有颜色编码器。候选仍只在实验分支，无生产路由或SDK声明。
