**普通使用请下载 `NewModCompiler-Win32-Release.zip`**，完整解压三个运行文件到同一目录。Debug 包供调试。

- 11 个 Joypad 独立根已加入现有 worker 调度，覆盖其 34 个运行时 Data 分支；XML metadata 并行认证从 284 增至 295 路。
- 保持 TypeId、TypeHash、ProcessingHash、版本与缓存身份；保留资源预算和未知类型回退，提交及诊断遵循原合同。
- 缓存位于 SDK 的 `SuperCompilerCache/<ModKey>/`。BIG 编译必须显式传入 `--mod-version`。

```powershell
./NewModCompiler.exe doctor --sdk-root "C:\NewSdk\SDK"
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" --mod-version 1.0 --jobs auto
```

运行要求：Windows x86 兼容环境、.NET Framework 4.8，以及与清单所列 MSVC 14.44 工具链匹配的 x86 VC 运行库。SDK 和 MOD 需单独准备；Texture 已内建，无需旧 Texture DLL、VC80 CRT 或 DirectX 9/D3DX。

生产源提交：`a75840ef7d0351cedc11c91b1b4510d295b911a7`。
Debug/Release 构建与组件来源：[41299605157](https://github.com/wu162/NewSdk/actions/runs/41299605157)。
最终验收：[41300821811](https://github.com/wu162/NewSdk/actions/runs/41300821811)，测试源 `6c098c0c240ae6a29ec2ea3688e53a39cd906f32`。生产与测试来源分别记录，独立成功组复用的原失败总运行保留；详情见 `release-metadata.json` 和 [PR #49](https://github.com/wu162/NewSdk/pull/49)（未合并）。

四平台矩阵、每平台 89 个 Joypad 输入、45 资产小 MOD 的真实 worker 重叠、jobs1/4/auto、冷热缓存、换输出及编辑、原 BAB 四流对照、CLI/取消，以及独立解压 ZIP 验收均通过。本轮没有大型 MOD 或全量 SDK 编译，未增加性能基准或游戏内验收。

两个运行 ZIP 与用户确认的包逐字节相同，各自仅含 `NewModCompiler.exe`、`NewModCompiler.exe.config`、`antlr.runtime.dll`。附两个 manifest、使用说明、构建元数据和 SHA256SUMS，不含源码、测试宿主或对象文件。

使用独立二进制发行标签，标签快照只有说明与元数据，因此 GitHub 自动生成的 “Source code” 归档也不含编译器源码。真实构建提交见上方；开发分支与 PR 未合并。
