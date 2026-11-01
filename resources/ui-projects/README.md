# 集中编辑的 UI 工程

本目录在新的 `resources/tactical-editing-project` 分支提供 Tactical 编辑试点。
原 `resources/ra3-ui/` 整棵已恢复资源树保持原样，其基线 Git 树为
`53dddf9509736adb148ff834eaa290e60295c28d`。

当前检查点：四个发布根的 16 个最终流与原独立版本 SHA-256 全部一致，791 项组件检查通过。共享编辑影响验证和完整 Tactical 分组扩展进行中。

- `Tactical/Tactical.fla`：集中编辑四个独立发布根与公共元件。
- `Tactical/project.json`：各发布根、原舞台参数、导出名称及 AS 依赖。
- `Tactical/Common/Scripts/`：由多个组件真正共用的一份 AS。
- `Tactical/Scripts/<Movie>/`：各组件自己的脚本。
- `Tactical/Authoring/`：本次新建主 FLA 的 XFL 构建源与媒体。

构建使用 .NET 8，在指定的新输出目录写入各组件的四流：

```bash
dotnet run --project resources/ui-projects/tools/UiProject.Build -- resources/ui-projects/Tactical/project.json --out work/tactical-build
```

编辑 `Tactical.fla` 时，在库的 `Roots/` 选择组件；公共内容位于 `Common/`。
发布仍保留原游戏电影名称和加载边界。主场景用于集中编辑，不替代游戏内播放验收。

离线重建与最终验收细节将在本轮验证完成后记录。没有新增资源下载 Actions。
