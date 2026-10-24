# XmlCompiler：当前续接入口

更新：2026-10-25（北京时间）。目标分支 `recovery/xmlcompiler-priority`。本轮约一小时、最多三个子代理，**SDK真实编译0次**。

## 最终状态

原Plugin301个登记入口中，**287个完整组件入口通过验收**，较上轮183个新增104个，分40个完整组。当前61组完整矩阵及287入口联合DLL全部通过；这不是301全部完成，也不是总代码复杂度比例。

- 验收源码：`0f0ab2fce098fef2d78d6a165f3f5ce86c591077`。
- 组件运行：[40142560815](https://github.com/wu162/NewSdk/actions/runs/40142560815)，completed/success，已核sourceCommit/runId与每组两模式、四平台。
- 完整矩阵：/Od、/O2；平台0/1/2/99；原ProcessInstance三缓冲区逐字节差分、适用直接CompileXml对照、独立ABI与拒绝边界。共1716864 fixtures、9852216数组比较、34264显式拒绝；数组比较含适用直接调用对照，不能全部称为独立原DLL调用。
- 联合DLL每模式287×4=1148默认样本/3444数组比较，另200 completion场景/600比较（包含真实no-op入口输入）。两模式实际进程退出0；287个精确公开ProcessInstance路由均加载。smoke不替代61组完整矩阵。
- 265个组件类型在CnC3Types AssetDeclaration中列为资产，其余有独立Plugin登记但仅嵌套/抽象/runtime schema；不能把每个入口都称作可直接声明的SDK资产。
- 精确证据：[xml-component-acceptance-20261025-287.json](evidence/xml-component-acceptance-20261025-287.json)。完整301项静态登记/路由审计：[xml-coverage-20261025-287.json](evidence/xml-coverage-20261025-287.json)；静态审计本身不代替运行验收。

## SDK边界

`catalog/XmlRecoveredTypes.json`保留历史82类SDK清单；最后SDK基线仍为40098695436、源码92180f9bbd830b9dbd49a696c259e54ffe2d7721。`catalog/XmlComponentOnlyTypes.json`现单列205类，全部sdkValidated=false，其中本轮新增104、上一轮新增101。当前287入口联合DLL整体SDK未验收，不能把旧82类旧DLL成功结果套到新联合DLL。

主Plugin、SetXmlPriorityPlugin、TestXmlPrioritySdk、SDK Oracle/联合probe、SDK工作流、原SDK文件和原DLL均未修改；无SDK触发标记、无手动dispatch SDK。两份联合DLL位于本轮recovered-xmlcompiler-priority-x86 artifact。组件接收BAB预处理后的引用/defaults/继承及数字TypeId，未恢复完整Tokenizer、生产SDK路由或自定义APT/Audio/Map/Texture编译器。

## 本轮40个完整组

| 组 | 新入口 | Fixtures（两模式合计） | 数组比较 | 拒绝 |
| --- | ---: | ---: | ---: | ---: |
| conditions | 4 | 443936 | 2663616 | 504 |
| targeting | 5 | 656 | 3936 | 280 |
| evaachievement | 7 | 13976 | 83856 | 424 |
| healthbars | 2 | 26464 | 158784 | 184 |
| unitoverlay | 1 | 5168 | 31008 | 128 |
| objectsets | 3 | 178224 | 1069344 | 288 |
| visibility | 2 | 65912 | 395472 | 192 |
| emotions | 2 | 48856 | 293136 | 144 |
| projectilepath | 1 | 2712 | 16272 | 168 |
| forcefeedback | 1 | 6536 | 39216 | 64 |
| content | 5 | 4208 | 25248 | 280 |
| visualsystems | 3 | 6400 | 38400 | 360 |
| crowd | 1 | 5576 | 33456 | 192 |
| radar | 2 | 29688 | 178128 | 352 |
| renderdevice | 2 | 29592 | 177552 | 208 |
| localization | 5 | 6440 | 38640 | 416 |
| commandset | 1 | 2040 | 12240 | 144 |
| materialrules | 3 | 3000 | 18000 | 168 |
| uiimages | 3 | 4720 | 28320 | 304 |
| chatcolors | 1 | 37616 | 225696 | 1688 |
| moviearchive | 2 | 3184 | 19104 | 136 |
| systemsettings | 4 | 6776 | 40656 | 216 |
| campaign | 5 | 12584 | 75504 | 392 |
| buildeconomy | 4 | 55328 | 331968 | 336 |
| dynamics | 1 | 1328 | 7968 | 104 |
| connections | 1 | 1992 | 11952 | 312 |
| redalert | 1 | 4616 | 27696 | 224 |
| conquercards | 5 | 5752 | 34512 | 328 |
| armyopening | 2 | 4704 | 28224 | 144 |
| assetindexes | 3 | 4760 | 28560 | 240 |
| renderassets | 3 | 6544 | 39264 | 416 |
| aptdata | 4 | 1752 | 10512 | 208 |
| runtimeopaque | 2 | 3280 | 19680 | 136 |
| mouse | 1 | 1376 | 8256 | 64 |
| abilitybuttons | 2 | 12072 | 72432 | 824 |
| testtextures | 2 | 960 | 5760 | 136 |
| terrain | 2 | 3600 | 21600 | 168 |
| w3d | 4 | 13856 | 83136 | 328 |
| mapmetadata | 1 | 3336 | 20016 | 152 |
| gamescripts | 1 | 2000 | 12000 | 208 |

逐根名称、TypeId、TypeHash、schemaGlobal在对应Xml<group>Types.json及组件专用总表；字段/原方法/布局见对应XML_*.md。条件、目标比较、Eva/成就、健康条、叠层、对象集、隐身、情绪、弹道、力反馈、内容/情报、本地化/字幕、命令集、渲染设备/资源、系统设置、道路/着色器、群体/雷达/图像、征服卡/战役/军队、文件blob、按钮分派、W3D、地图元数据和游戏脚本均按完整根及依赖实现，不把内部helper或未知根fallback另计入口。

## 关键合同与修复

- 原IValue字符串使用StringToHGlobalAnsi；字符串长度和hash按首NUL的ANSI字节合同，不能自行改为UTF8。uiimages独立断言已逐字节验证ANSI、终止NUL及padding，Unicode/NUL样本未删。
- ModelCondition的ALL虽在静态表也有编号，但原Marshal先识别ALL并填满所有word（含padding）；INVALID=-1被范围guard跳过。ALL分支未推进token，后续负号项不执行；已核原控制流并保留独立逐word断言和无ALL负号对照。
- Time保留原float单位比例后乘法/回舍入；1500ms位是3fc00001，不是十进制近似1.5f。未修改共享helper迁就错误预期。
- 强引用输出index+1、-1仍产生import；bookmarks按owner block再按index/target block排序。各分配混合与逆序引用均有独立ABI。
- ShaderOverrideRule/ArchivableMovie/ArchiveMovieList及三个Condition根首word不是统一保留0；按真实结构核验。CommandSet不是CnC3Types全局资产，其大端默认word32断言按完整word验证。
- APT4根和TestTexture确实读本地文件，完整覆盖空文件Push0、target==实例末尾、失败文件、padding及不swap payload；测试仅用临时夹具，未碰SDK真实编译。
- BaseRenderAssetType4与AudioFileRuntime32是原独立Xml入口明确no-op（后者Value读后丢弃），有原Handler合同与hash证据；不是未知TypeId空fallback，也不是恢复自定义Audio/Render编译器。方法digest注明UTF8/LF规范化。
- Ability按钮17种Data、W3D Delta/Timecoded、ConquerCard和GameScript参数保留完整原数字分派及原base fallback；unknown根继续拒绝。W3D矩阵/short/byte、地图StartPosition小写坐标与W3D大写坐标按各自合同；NUL边界用DOM构造，不删样本。
- 前四次失败运行及修复保留在提交/Actions历史；不删断言、不以重试掩盖差异。最终以上述成功源码/运行与完整矩阵为准。

## 剩余14入口

本轮仍未恢复的两根：

1. `UIConfigList`：完整70个内部多态分派，见[XML_UI_CONFIG_NEXT.md](XML_UI_CONFIG_NEXT.md)及evidence/xml-uiconfiglist/contract-index.json。没有登记部分实现。
2. `GameMap`：80字节根、8种列表、Environment/WorldDict56七种pair、MapAnalysis28等完整依赖，见[XML_GAME_MAP_NEXT.md](XML_GAME_MAP_NEXT.md)。原入口不是no-op；不截断根，MapMetaData pipelineOnly忽略等合同已记录。

原范围排除12根：`TestGameObject`, `UIJoypadCommandBarMixedGroup`, `UIJoypadCommandBarHomogenousGroup`, `UIJoypadCommandBarCoopCommunicator`, `JoypadCommandBarButtonTemplate`, `UIJoypadCommandBarStances`, `JoypadCommandBarTemplate`, `UIJoypadCommandBarTopMenu`, `UIJoypadCommandBarPlayerPowers`, `UIJoypadCommandBarButtonBuild`, `UIJoypadCommandBarControlGroups`, `UIJoypadCommandBarSingleUnit`。其中11个Joypad命名根与TestGameObject未计完成；GameObject源码/另会话compiler-source未动。若要301全登记入口，需要之后明确把排除项纳入，并完成它们各自完整组件矩阵；当前仅287/301。

下一轮先按用户范围选择UIConfigList或GameMap完整组；如仍禁SDK，继续component-only。仅未来明确允许时再批量做205类生产SDK路由、合法样例、真实覆盖与四产物/隔离重放，不得暗中触发。

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`、原DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`。复用原证据/共享helper，按完整能力组合批，检查点与完整验证分开；分支recovery/xmlcompiler-20261025-components及主分支保留全部父历史，GitHub插件正常更新，无强推。没有可比账号额度数据，不量化节省。
