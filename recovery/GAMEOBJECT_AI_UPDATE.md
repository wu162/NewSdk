# AI Update 八模块恢复

2026-10-24：实现中，尚未验收，不增加136/322支持计数。

原证据为 ai-update-source 的96个方法及PE枚举，固定原Tokenizer blob 6e40a9ed6c9f961d294d49768d911e6a6f354619，导出源码bae88f824dd018ba5a46ba390ab5f095cc046d07 / run40072829359 success。

首个检查点：GenerateAIUpdateFields.py 已能从固定证据复现整组字段、默认值、Token布局、Horde/Transport独立父层和UnitChooser40；本地 --exported-evidence 通过全部96方法bodySHA及8个共享BaseChooser/ObjectVisionRangeItem/ObjectFilter方法指纹校验。正式Windows检查还会核对原源码blob和PE枚举。下一步接入Marshal/Token/endian、桥接输入拒绝边界和完整差分测试。尚不计作组件支持或SDK支持。
