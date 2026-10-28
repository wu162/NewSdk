# PC 透明块DXT1A：已验收的受限路径

仅Win32、Production=71。当每个实际编码块均有alpha<=0.5的源样本时接受DXT1A，包括每一层mip和volume切片；padding空洞不计透明样本。默认DXT1及任意全不透明DXT1A块保持明确拒绝，不为原未初始化字段规定自定值。原图像loader alpha规则保持不变，全零alpha TGA仍先变为不透明。

## 支持与实现

`TextureDxt1aTransparentBlockEncoder`压缩实际不透明样本，使用已适配公开S3的显式format1/flag0状态；保留默认权重、block area16、全透明端点、partial映射、原单色level4返回、端点相等与模式相关排序/索引映射。没有原DLL/旧恢复颜色编码器回调、样本ID分派或gold输出表。所有实际消费者维护新helper依赖，颜色native调用与DXT3/5共用锁。

能力接入既有block/byte/normalized-float/partial API、二维DDS及base-relative完整float mip、volume及递归float mip、Texture/OnDemandTexture插件。任何层的实际块无透明样本则整体拒绝；已有cdata不会因半成品DDS被改写。PNG/BMP/JPEG/TGA其他既有解码与格式推断规则不扩大范围。生产SDK DLL/config未替换，原monolithic C++/CLI/native指针ABI不冒充已恢复。

## 精确验收锚点

- 业务源码`5882d9afc2cc9f98ce2203a9846379cf0b932508` / [40695394943](https://github.com/wu162/NewSdk/actions/runs/40695394943) completed/success：17个组件组（14旧消费+3新组）和10个SDK profile全部通过，18个未变组件复用。新插件D/R各768输入：744输出和24个匹配原invalid-volume错误，额外14个原默认DXT1/opaque DXT1A/混合块拒绝，三buffer/full cdata/旧文件尾/源不变及逐次NoOriginal通过，每配置3693496字节比较。
- 两个新组件首次结果缺少用于复用的`configurations[].passed`字段，未篡改旧收据。源码`0d37d401f812edb58a4e96a0c9ba2ad033abe996` / [40699070349](https://github.com/wu162/NewSdk/actions/runs/40699070349) completed/success只补2组、复用33组件，未重跑SDK。D/R各4608完整块/3456完整块DDS全部相同；完整二维/volume/mip DDS各2714相同，934不确定域输入明确拒绝；全部逐次NoOriginal和托管/native二进制指纹齐备。
- 独立SDK归档核对发现旧raw两个profile只移除原DLL、缺少逐次守卫。源码`52d90eff668110855f469a45a395dcaf5cec5c44` / [40699857549](https://github.com/wu162/NewSdk/actions/runs/40699857549) completed/success只补PcNoMip/PcMips各600资产，复用35组件和8个SDK；现每个实际NoOriginal调用前后均检查原DLL不存在且未加载，四产物/全部完整轨迹断言保留。业务编码源码未变。

10个SDK profile按Original/Recovered/NoOriginal模式合计各13424资产/13424实际Core调用；每模式46978个完整轨迹文件全部逐文件核对，每个profile四产物在三模式间相同。PcDxt1a为80张TGA/320实际根资产，覆盖standard/volume、有无mip和两类根。独立核对见[验收收据](TEXTURE_DXT1A_ACCEPTANCE.json)和SDK检查脚本`scripts/InspectTextureSdkArtifacts.py`；组件完整记录见`scripts/InspectTexturePublicS3Dxt1aArchive.py`。两个raw profile保留新隔离锚点，不把旧run冒充逐次守卫通过。

## 保留的边界和历史

原显式三色完整state仍为5309/5312，format0/mode0的三个端点/索引orientation差异保持真实；按原包装规范化compact打包5312/5312，随后完整DXT1A产物取得以上独立门槛。该结论不扩展到默认DXT1或不透明DXT1A。首轮40679136957因新宿主误断言level3而failure，修正后2ebb72e3/40692652134及5e994ac0/40693591607的完整实验档案保留，不把comparison-completed当成正式集成验收。

本轮不重跑完整mymod；其旧5a1b8e1d/40657712486锚点仍只证明原输入/源码。完整MOD继续manual_only，后续实际生产集成里程碑再集中必要回归。当前受限DXT1A能力已闭环；新图像/metadata输入与完整生产安装保持独立门槛。
