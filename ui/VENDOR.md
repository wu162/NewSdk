# 导入来源与采用决定

- 项目：[lanyizi/swf2apt](https://github.com/lanyizi/swf2apt)，上游jonwil/swf2apt。
- 采用提交：`2b3cb662b7311616e462b884e5611750e5de88f4`。
- 位置：`ui/vendor/swf2apt`，Git归档原样复制，保留目录、版权及各组件许可；不采用Git子模块。未复制未使用的预编译 `zlib/crc_i386.obj`。
- 原始构建项目保留；`verification/DirectCli.xml` 是用于当前环境的.NET8/x86构建适配器。
- 采用依据：与附件工具PDB对应的390个非生成C#源文件SHA256全部一致；样例在两种模式下与原始程序集/原flasm输出全部逐字节一致。
- [成功运行](https://github.com/wu162/NewSdk/actions/runs/40288873546)，机器记录 `evidence/source-consistency.json`。

最初60b15a67候选只在FillStyleFactory.cs的异常消息上与附件不同，故采用其父提交。未为凑校验而改动源文件。

用户已明确批准上传工具。参考ZIP仅包含从附件单文件EXE提取的原始程序集、原生flasm/jpeg和SHA256清单；双方使用同一个.NET8 x86运行时。没有冒称完整.NET5自包含EXE宿主已验证，也没有将样例一致扩大成所有SWF功能一致。

许可保留在各项目原路径（例如eaf2apt/COPYING）；导入不移除第三方声明。下一阶段在此源码基础上改造内存输入输出及MOD编译接入，使用本次原始工具和结果作为回归基线。
