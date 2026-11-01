# NewModCompiler 2026.11.01

这是修复验收后生成的待确认运行包，尚未发布到 GitHub Release。

普通使用请下载 `NewModCompiler-Win32-Release.zip`。Debug 包供调试使用。

将 ZIP 中的三个文件完整解压到同一目录：

- `NewModCompiler.exe`
- `NewModCompiler.exe.config`
- `antlr.runtime.dll`

运行要求：Windows x86 兼容环境、.NET Framework 4.8、与发行清单所列 MSVC 工具链匹配的 x86 Visual C++ 运行库。Texture 已内建，不需要旧 Texture DLL、VC80 CRT 或 DirectX 9/D3DX。SDK 资源和待编译的 MOD 需单独准备。

在 PowerShell 中进入工具目录，检查环境：

```powershell
./NewModCompiler.exe --help
./NewModCompiler.exe doctor --sdk-root "C:\NewSdk\SDK"
```

编译 MOD，生成 BIG 和 SKUDEF：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\Example" --mod-version 1.0
```

`--mod` 指向 MOD 文件夹。BIG 编译必须在命令行显式填写 `--mod-version`。默认自动发现 base、APT UI 和全局元数据，并包含 Additional。输出目录应与 SDK、MOD 输入目录分离；最终文件在输出目录的 `published` 子目录中。

仅编译基础数据为松散流：

```powershell
./NewModCompiler.exe build --sdk-root "C:\NewSdk\SDK" --mod "C:\Mods\Example" --out "C:\Build\ExampleLoose" --package-content base --package-format loose
```

`--jobs auto` 默认按 CPU 和可用内存安排并行任务。缓存及中间文件集中保存在传入 SDK 的 `SuperCompilerCache/<ModKey>/`，MOD 输入目录只读。默认显示编译进度；重定向输出时使用文本模式。查看高级参数：`./NewModCompiler.exe --help-advanced`。

`SHA256SUMS.txt` 用于校验附件，两个 `.manifest.json` 记录 ZIP 内各文件的长度及 SHA256，`release-metadata.json` 记录构建提交、Windows 验收运行和包身份。运行包不含源码、测试程序、对象文件或调试符号。
