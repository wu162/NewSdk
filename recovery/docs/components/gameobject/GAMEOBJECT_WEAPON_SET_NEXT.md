# WeaponSetUpdate 续接入口

完整四槽及依赖已恢复并通过组件差分，详见 GAMEOBJECT_WEAPON_SET.md。当前源码/验证锚点与下一组以 RESUME.md 为准。

旧交接中关于 Tracker.AlterBookmark“同时涉及源/目标”的表述不准确：原方法仅按a/b元素范围调整bookmark.offset，不修改value。既有Tracker实现已正确，禁止据旧措辞额外交换value。排序继续先交换三组bookmark的位置，再交换72字节记录；不能提前排序XML或仅memcpy。
