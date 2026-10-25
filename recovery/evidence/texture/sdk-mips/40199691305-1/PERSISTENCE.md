# SDK mip 验证收据

源码 `aa2b2d80badefaa8a0fe4468d29a458fe2b38b7e`，运行40199691305 attempt1。实际编译及比较步骤成功；最终工作流failure发生于证据git push并发ref锁冲突，不是编译或比较失败。此处保留原始validation.json，不改写原sourceCommit/status。

完整二进制、三buffer/cdata逐资产轨迹及日志保留在原 [Actions artifact](https://github.com/wu162/NewSdk/actions/runs/40199691305/artifacts/12601711027)。本次只持久化不含二进制内容的收据、哈希与产物链接，没有再次向Git上传这些二进制。需要复核时从原artifact取回；若artifact过期，应明确重新取得证据，不能把哈希单独冒充可重放数据。
