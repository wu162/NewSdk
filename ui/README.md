# SWF 编译项目

源码已直接导入：[vendor/swf2apt](vendor/swf2apt)。交接：[RESUME.md](RESUME.md)。来源与验证：[VENDOR.md](VENDOR.md)。

已验证提供的CoronaUIMain.swf：普通RA3、RA3优化两种模式下，源码版与附件原始程序集的全部生成文件逐字节相同。当前仍是会生成APT相关文件的原转换器；直接接入MOD编译器、消除中间文件是下一阶段。

Windows构建需要.NET8 SDK、.NET8 x86运行时、VS C++工具。无需再次克隆上游：

```powershell
New-Item -ItemType Directory -Force ui/.work | Out-Null
Copy-Item ui/verification/DirectCli.xml ui/.work/DirectCli.csproj
dotnet build ui/.work/DirectCli.csproj -c Release -o ui/.work/direct-cli
```

原生zlib/flasm构建命令见 `.github/workflows/ui-source-parity.yml` 的Build source optimizer步骤。把 `ui/vendor/swf2apt/Release` 加入PATH后，用.NET8 x86的dotnet执行：

```powershell
& 'C:/Program Files (x86)/dotnet/dotnet.exe' ui/.work/direct-cli/eaf2apt-source.dll <输入.swf> --ra3 --optimize
```

完整一致性复验按上述工作流运行，证据保存在 `ui/evidence/source-consistency.json`。正式内存接口后续直接改导入源码；旧memory原型仅保留作历史参考。
