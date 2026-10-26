# SWF 编译研究与原型

交接入口：[RESUME.md](RESUME.md)。源码调查和完整限制：[RESEARCH.md](RESEARCH.md)。

目标：MOD 编译器直接接受 SWF，内存生成既有 APT 资产，不落地转换中间文件。当前只是固定样例能力的转换组件，不是已启用的 BAB 插件。

Windows 本地复现（需 .NET8、Python、VS2022 C++；上游固定提交）：

```powershell
git clone https://github.com/lanyizi/swf2apt ui/.work/upstream
git -C ui/.work/upstream checkout 60b15a67a4cbd931f181af8c6c758bd7a8441ebc
python ui/tools/prepare_memory_backend.py ui/.work/upstream ui/.work/MemoryAptConverter.cs
# 原生 zlib/flasm 的构建命令见 .github/workflows/ui-memory.yml；将 Release 加入PATH。
dotnet build ui/tests/MemoryBackend.csproj -c Release -o ui/.work/bin
dotnet ui/.work/bin/MemoryBackend.dll ui/fixtures/CoronaUIMain.swf ui/.work/validation.json
```

验证原源码工具的RA3优化编译、受限内存组件的no-opt逐字节差分、CWS/FWS和两种文化设置、拒绝输入和无转换文件。没有把no-opt原型当成原脚本的优化模式。上传工具二进制与源码基线的关系见研究文档。
