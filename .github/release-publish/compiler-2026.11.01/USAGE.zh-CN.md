# NewModCompiler Joypad 并行发布包

普通使用请选择 NewModCompiler-Win32-Release.zip；Debug 包供调试。
完整解压三个运行文件至同一目录：NewModCompiler.exe、NewModCompiler.exe.config、antlr.runtime.dll。

要求 Windows x86 兼容环境、.NET Framework 4.8 及与清单工具链匹配的 x86 VC 运行库。SDK 与 MOD 单独准备。

```powershell
./NewModCompiler.exe doctor --sdk-root "C:\NewSdk\SDK"
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" --mod-version 1.0 --jobs auto
```

11 个 Joypad 根已加入现有并行调度；资源和内存预算仍生效，提交与诊断由 owner 处理。版本和缓存身份未更换。
本轮仅使用小型 Joypad MOD 和定向测试，没有大型 MOD/全量 SDK 编译，不作为新性能或游戏内验收结论。
SHA256SUMS.txt 用于附件校验，manifest 和 release-metadata.json 记录二进制来源与范围。
该包不含源码。发行说明与二进制来源：https://github.com/wu162/NewSdk/releases/tag/compiler-2026.11.01 。
