# PC原生文件加载：续接入口

本轮实现与待收结果统一见[RESUME](RESUME.md)，详细支持范围及未恢复域见[原生文件加载](TEXTURE_NATIVE_FILE_LOAD.md)。

## 源码和消费

- `src/TextureCompiler/Native/TextureNativeLoaderApi.{h,cpp,def}`：CPU二维资源头/分配/所有权和raw DDS/TGA文件入口；`BuildTextureNativeLoader.ps1`构建独立x86库。
- `TextureNative{Device,Resource,Surface,Descriptor,Copy,Convert}Api.*`：现有设备、引用/释放、布局/锁定、描述符与复制/转换。
- `recovery/tests/TextureNativeLoaderOracle.cpp`、`recovery/scripts/TestTextureNativeLoader.ps1`，集中组`native-loader`：支持域完整原版字节差分与D/R/NoOriginal；原版成功压缩域独立保存取证，恢复版明确拒绝。
- 静态原版证据：`recovery/evidence/texture/native-loader-static/20261030/`，原managed调用IL在`recovery/evidence/texture/runs/40191971356-1/methods.json`（按方法读取）。

## 必须保留的契约

- 原LoadImageA目标`0x1a200006`、flags`0x12`、threshold0；source format先与`0xfffffe3f`相与，清除`0x1c0`。资源描述符仍保留端序标签；CPU文件上传不据此交换原始字节。
- GlobalAlloc资源对象与VirtualAlloc主/secondary存储配对，使用现有ResourceRelease释放；small mip使用原packed tail，不可按逐层紧密存储替代。
- TGA全零alpha改为X8，基础alpha0保持；BOX双轴≥2的整数快速路径与退化轴的float32顺序运算/向零存储分开。
- 独立ABI尚未进入生产包。type2渲染/type9/GPU、gamma/压缩转换、packed float10/uint32数值转换等原有边界不变；文件NPOT、低位深/更多codecs、全部压缩DDS仍未恢复。

下一批只在消费链稳定后集中验收，未变成功组复用；不为helper/收尾重复启动SDK/MOD。当前待收source/run以RESUME为准，不用历史文档的“下一轮”替换当前目标。
