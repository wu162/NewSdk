# TextureCompiler：PC续接

分支：`recovery/texturecompiler-source`。本次仅精简交接，业务源码、测试与验收证据不变。

## 当前成果

- PC生产六DLL包已部署；新增独立原生ABI尚未加入生产包，完整混合模式DLL身份未恢复。
- 原生设备/资源、布局/锁定、描述符及复制已完成当前声明范围；转换支持38种布局的数值/浮点、四端序、swizzle和原LoadImageA flags调用。
- 验收源码：`6af9af189a5fb9be63bd02fbab4577d71c583257`；[40965216216](https://github.com/wu162/NewSdk/actions/runs/40965216216)成功，8组通过、55组复用，63组件证据有效，无待收结果或强制补验。
- 详细证据：[最终审计](evidence/texture/handoffs/20261030-native-conversion/final-archive-audit.json)、[组件就绪清单](evidence/texture/handoffs/20261030-native-conversion/current-component-readiness.json)。各组保留原sourceCommit/runId。

## 下一轮计划

1. 恢复 `D3DXCreateTextureFromFileA@0x13c82 → 0x13b53` 的PC文件加载、资源头构造及存储所有权，接入已恢复复制链。
2. 先核对实际调用参数、格式/端序、分配与释放，再连续实现整条消费链。当前fixture loader仅提供原版测试输入，不算已恢复业务。
3. 源码与契约入口见[原生续接](TEXTURE_NATIVE_ABI_NEXT.md)；历史过程不作为当前待办。

## 执行约束

仅PC，禁止强推，编译测试仅Actions。普通检查点只保存；稳定后默认每轮一次完整验收，保留原版逐字节比较、D/R和NoOriginal。CI期间可继续编码，旧结果只认证旧提交。未变组复用；不默认追加审计器、收尾plan或SDK/MOD。

具体规则按需查[AGENTS.md](../AGENTS.md)、[效率规则](EFFICIENCY.md)、[测试规则](TEXTURE_TESTING.md)。历史交接保留在Git历史，原始证据留在`evidence/`，无需全量读取。
