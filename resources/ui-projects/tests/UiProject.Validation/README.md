# Tactical 主 FLA 验证

此工具只读取原始恢复目录，并在指定的工作目录写入输出及修改副本。

```bash
dotnet run --project resources/ui-projects/tests/UiProject.Validation -c Release -- resources/ui-projects/Tactical/project.json resources/ra3-ui/converted work/tactical-validation --mutations --flacomdoc work/ffdec/lib/flacomdoc.jar
```

`--flacomdoc` 指向已经解压的固定工具，不下载文件。输出目录必须位于两个源目录之外。

验证内容：

- 每个项目根与原独立 FLA 的 `.manifest/.bin/.relo/.imp` 全字节一致。
- 一次读取主 FLA 的批量编译，与逐根编译结果一致。
- 临时副本中只修改一份共享 HelpBox AS，所有声明使用它的根变化，其余根不变。
- 只修改命令栏私有 AS，其余根不变。
- 沿公共 WhiteRectangle 的真实图形依赖修改颜色，重导临时 FLA 后，仅使用该图形的根变化。

逐根结果、输入依赖、哈希和变更作用域保存在 `report.json`。工作目录保留前后四流与临时源，方便复查。上述检查属于组件发布验收，不代表游戏内视觉和交互验收。
