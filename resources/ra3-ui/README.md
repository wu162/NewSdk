# RA3 UI 资源恢复

基于 `recovery/legacy-sdk-cache` 的 `52d44c6267385055df6da91ecdc6ee2b44f467a9`，专用资源分支 `resources/ra3-ui-assets`。

当前检查点：原始包和全部解压资产已保存，174 个二进制 FLA 与 4,432 个实际 include 使用的 AS 已导出。完整编译兼容修复与最终验收进行中；本检查点不代表全部界面编译通过。

- `source/ra3ui/`：原包全部 6,284 个文件，包含 174 个 SWF 及原有 EAF、APT、CONST、DAT、几何、纹理和其他配套文件。
- `original/ra3ui.7z.001`–`.003`：原始压缩包按 64 MiB 分块，避免 GitHub 单文件 100 MiB 限制，不使用 Git LFS。
- `tooling/ffdec_26.4.0.zip`：固定转换工具，许可包含在 ZIP 内。
- `provenance.json`：原作者公开镜像、原包 SHA-256、Mega 完整性验证和工具版本。

PPM 下载页要求验证码，本次从原作者同名公开镜像取得文件；来源讨论贴明确提供 PPM 镜像。原包由 Jonwil 从 EA 发布的 EAF 重建 SWF，包含其最终补丁版 igm_pauseMenu 修正。FLA/AS 将是反编译恢复产物。原有 EA/作者权利不因格式转换而改变。

所有素材和转换工具随本资源分支提供。本次不新增或触发下载这些文件的 GitHub Actions；转换脚本只使用本地文件。

恢复原包：

```bash
python resources/ra3-ui/tools/restore_archive.py /path/to/ra3ui.7z
```
