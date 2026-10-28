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
