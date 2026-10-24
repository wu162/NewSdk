# XmlCompiler SDK：82入口最终验收

2026-10-25状态说明：本页仅记录旧82入口及旧DLL的历史SDK基线。本轮用户禁止SDK真实编译，实际0次；新增101入口仅组件验收，当前183入口联合DLL尚未SDK验收。主Plugin/SDK路由与本页旧证据不变，不能把下列历史结果用于宣称新联合DLL已集成。当前状态见XMLCOMPILER_PRIORITY.md。

[SDK运行40098695436](https://github.com/wu162/NewSdk/actions/runs/40098695436)，源码 `92180f9bbd830b9dbd49a696c259e54ffe2d7721`，completed/success。最终十四组组件及联合DLL：[40099095351](https://github.com/wu162/NewSdk/actions/runs/40099095351)，源码 `e7580311f36811265e6ed5af5a184ef623f31284`，completed/success。

两个源码锚点间仅UI组件Oracle的独立断言发生变化；recovery/src、recovery/sdk、recovery/scripts、原SDK、.github子树及SDK Oracle的Git对象相同，详见 `evidence/xml-priority-acceptance.json`。未重复运行未受影响的SDK；不把旧源码结果冒充新实现验收。

## Metadata、schema与范围

82个原Plugin入口在四平台0/1/2/99的Initialize/ReInitialize全部对照，共4608项公开字段/属性/版本/哈希比较、8项未知类型拒绝。版本Win32/unknown=4、PS3=7、Xbox360=6；ProcessingHash=TypeHash XOR version，Tokenized=false。表见 catalog/XmlRecoveredTypes.json。

159个固定源资产完整通过原CnC3Types.xsd，3个负样本按原schema拒绝。80个全局类型逐类必有实际插件调用。GameStatsModeData、MicrophoneSettings仅有嵌套schema；组件证明独立入口，SDK分别经MultiplayerSettings、AudioSettings嵌套覆盖，未增加全局声明。

新增五组含65个源资产：damagefx5、hotkeymaps11、audio13、uisettings29、soundmaps7。每份片段在 sdk/Xml<group>Probe.fragment.xml，联合源为 sdk/XmlPriorityAllProbe.xml。FXList依赖包含NuggetList；AudioSettings内三个mic有必填id；HotKeySlot继承源在schema阶段仍有必填Name。禁止放宽XSD或用不合法inheritFrom制造覆盖。

## 两套实际SDK profile

| profile | 实际调用 | 每份原版/隔离重放数组比较 | 必需资产 | 全局类型 | 原Includes |
| --- | ---: | ---: | ---: | ---: | ---: |
| Reduced | 159 | 477 | 159 | 80 | 同一精简reference集合 |
| FullMymod | 162 | 486 | 159 | 80 | 全部7个保留 |

两profile均原/恢复输入相同；bin、manifest、imp、relo四产物逐字节相同，imports非空。每个成功调用保存预处理后的XML和三缓冲区，全部逐次用原版和隔离恢复版重放，并确认payload进入最终mod.bin。FullMymod的3个额外调用来自原MOD中的LogicCommand/LogicCommandSet。

同一/O2 DLL和Plugin用于两个profile：
- compiler SHA256 `D1DBBA4CC3E7CAC38CDAD1E94FEC242850A4C6A0C4BD4DC07A677D64E7C97200`
- plugin SHA256 `205AA8D09AC71F01D863B6415ABF49AA6BD115C988F08439E01A2BD0212F0652`
- probe SHA256 `370D612E4C578ADAD8D250C8E97041A7F7C30F92C19FBF5025F8C0F47ADB2797`

隔离目录物理排除两原compiler DLL，并持续检查进程没有加载原XmlCompiler/Tokenizer程序集。两原DLL指纹未变。Reduced四产物字节数分别17576/18413/992/1380；FullMymod分别227120/23310/1052/6032，原/恢复各对应相同。完整SHA、计数及sourceCommit/runId在 evidence/xml-priority-sdk 中，汇总收据在 evidence/xml-priority-acceptance.json。

## 使用边界

原SDK配置和原DLL未替换；SetXmlPriorityPlugin.ps1只在指定副本中显式路由这82类，其他类型继续原处理器。组件不调用原编译器；原DLL仅用于oracle。SDK/BAB仍负责引用、defaults、继承和数字多态TypeId预处理。这不是全部301入口或完整Tokenizer的替代。Joypad命名根与GameObject恢复继续排除。
