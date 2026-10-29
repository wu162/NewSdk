# 原 SDK 缓存 + 全认证后端并行：实现与验收完成

成果分支 `recovery/legacy-sdk-cache`；最终生产源码/二进制 `6d39f833bfbad40e3576b5517e1da685a995a77a`，测量控制器 `02987cbc`，后续为文档/证据归档。禁止强推；未合并其它主线，未发布 Release。

完整资格已一次接入：W3X2、受控 Audio/MP3、XML284 metadata/62族、APT4。默认jobs=auto，原SessionCache/BuildCache及csc/psc默认、Texture缓存、LinkedOutput/FileHashCache和正常BIG/发布优化保留。命中先判位置，零并行准备/新Key/hash。Worker私有输入/CDATA，owner有序验证提交；跨文档/同资产复用屏障，失败/取消先join再部分保存和释放。

功能 [40915444929](https://github.com/wu162/NewSdk/actions/runs/40915444929) 的Debug/Release、全部组件、64真实生产/缓存/worker用例、原BAB、CtrlBreak、完整mymod原SDK差分、BIG/Additional和五文件门槛全部通过；该run整体failure仅独立性能FrameRate测试夹具错误。修正控制器后，严格来源/输入/归档/EXE身份校验复用原二进制，性能 [40920804580](https://github.com/wu162/NewSdk/actions/runs/40920804580) success，22样本全部通过，没有重建Native。

完整Upheaval同VM、off：jobs1/4/auto默认冷74.096/49.165/48.580s（减少33.6%/34.4%），无缓存冷78.095/47.084/47.192s；热中位2.898/2.918/2.908s，零解析/编译/并行准备。真实峰值1/4/4；单动画修改仅1资产/1文档，3.116/3.153/3.141s，与独立串行改变流重建原字节相同。完整BIG/SKUDEF原输入18样本相同，单改4样本相同；全部0错误、1859原警告。两份最终ZIP归档digest已下载复算匹配。

无已知迁移阻塞。未认证类型保持owner；10个大任务预算回退。冷每档一次/热两次，OS缓存未控；不宣称游戏内验收。详细来源、完整资格/调用/性能/剩余边界：[BACKEND_PARALLEL_MIGRATION.md](docs/BACKEND_PARALLEL_MIGRATION.md)。使用：[COMPILER_USAGE.md](COMPILER_USAGE.md)，计量：[PERFORMANCE.md](docs/PERFORMANCE.md)。
