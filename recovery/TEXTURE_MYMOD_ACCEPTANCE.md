# 完整 mymod 与真实 SDK 贴图集成

源码 `5a1b8e1d0e6c66ac9ebff60906da184eee1e26a7`，运行 [40657712486](https://github.com/wu162/NewSdk/actions/runs/40657712486)，attempt1，completed/success。只新增 SDK 组，复用32个未变组件组和9个既有SDK profile；没有重复其完整矩阵。

`TestTextureMymodSdk.ps1` 在一次性 SDK 副本执行 `SDK/defaultscript.cs` case5 的完整 BAB 命令，只增加 Win32 目标、CLR4兼容和追踪插件路由，禁用贴图插件缓存。未裁剪 mymod 的任何 Include、资产或原始数据文件。生产 `SDK/tools` DLL/config 未替换，其他编译器仍使用原插件。

| 场景 | Original / Recovered / NoOriginal | mod.bin | 证明范围 |
| --- | --- | --- | --- |
| 原样完整 mymod | 四产物全部相同；贴图调用各0 | 209548 bytes | 完整原样 MOD 共存构建；不能单独证明贴图调用 |
| 完整 mymod + ArtOverlay | 各384资产/384真实Core调用；全部三缓冲区和cdata相同 | 65917672 bytes | 全150个仓库DDS、7个TGA；两根；TGA raw A8R8G8B8/DXT3/5，有无完整mip |

扩展场景保留原完整 mymod，仅在临时 Mod.xml 的 Includes 尾部追加一个生成文件。DDS按原字节直通，声明DXT1不代表重新编码DXT1已恢复。TGA扩展名规范为小写，内容字节不变；没有补充未声明图像域。384=150×2+7×2×3×2。NoOriginal实际删除原 TextureCompiler DLL，逐次调用前后确认不存在且未加载。

收据和原四产物/原完整轨迹归档在 `evidence/texture/sdk-mymod/40657712486-1`。两个恢复模式在运行中用 SequenceEqual 比较所有字节；保存每模式每轨迹文件的路径、长度、SHA256及完整调用TSV，避免提交三份重复的大型DDS。独立核对归档中实际四产物SHA256与全部1345个原轨迹文件，三个模式轨迹清单完全相同。

原样四产物SHA256：

- bin `ffc62ed5cc97874a4e9f64ebad42dfc146d0687e05bcffb39deb83c753109012`
- manifest `ad37d83f6a74cc51efa5e579a543d69fb18569cbf0d00884bebbf18f9666eee9`
- imp `a54052b0b90e9fe51f87ff0b691be3488fdb47c2ace0dd0af1b614503c16180c`
- relo `0a7b6337d2a481bf933cd96516e05ea53279524f426ffef4c0e5ab1fa5309648`

扩展bin SHA256 `b702da6a04cfa024d3fbe9ac1fe26ec61b3c7496f45d8107b2be10b9fe48facc`。其余精确值与完整输入指纹见收据。本次完成完整mymod共存集成及全部仓库现有图像验证，不宣称任意MOD、默认DXT1/1A、未声明metadata或全DLL/native ABI替换完成。
