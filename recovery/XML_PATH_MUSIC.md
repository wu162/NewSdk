# PathMusic XmlCompiler entries — component only

组件最终验收（2026-10-25）：源码d9908f57107f11b7ab875ac5787fce2bf23e00a9，运行40132543491 completed/success；/Od、/O2及平台0/1/2/99全部通过，183入口联合DLL也通过。用户明确禁止本轮SDK真实编译，实际0次；本组SDK仍待验收，未加入活跃82类SDK路由。精确汇总见evidence/xml-component-acceptance-20261025.json。 本组两模式合计15968 fixtures、95808数组比较、384拒绝。

本组完整七个原Plugin独立入口：PathMusicGameDynamicState、PathMusicGameDynamicStateSet、PathMusicGameDynamicTransition、PathMusicEventRuntime、PathMusicEventSet、PathMusicMapRuntime、MusicPalette。后两个Runtime命名入口中的EventRuntime/MapRuntime无原CnC3Types全局声明（源资产PathMusicEvent/PathMusicMap另经专用编译器），catalog标schemaGlobal=false；其余五个为全局XML资产。本轮不运行真实SDK，不改活跃82入口路由，不宣称SDK已集成。

## 原版证据

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`。复用evidence/xml-source方法索引/分块/枚举及原Plugin HandleType/dispatch/GetExtendedTypeInformation。精确七根TypeId/TypeHash在catalog/XmlpathmusicTypes.json。

| 根 | 原Marshal/Plugin位置 | Tracker构造行 | 根大小 |
| --- | --- | ---: | ---: |
| PathMusicGameDynamicState | Module30400，Plugin6863 | 94340 | 20 |
| PathMusicGameDynamicStateSet | Plugin6972内联 | 94431 | 16 |
| PathMusicGameDynamicTransition | Module30323，Plugin7104 | 94522 | 12 |
| PathMusicEventRuntime | Module30357/Endian30373，Plugin6631 | 94158 | 16 |
| PathMusicEventSet | Plugin6731内联 | 94249 | 16 |
| PathMusicMapRuntime | Plugin233内联真实no-op | 88607 | 12 |
| MusicPalette | Plugin6511内联 | 94067 | 16 |

各根保留首word0。原Plugin只有Win32走小端，0以外平台含99走大端；多数Endian在Plugin内联，不能凭Module缺少独立Endian函数忽略。

- State：EnterEvent@4、ExitEvent@8为nullable strong reference**；属性存在即先依序分配4字节，即使空串仍分配。Transition列表count/pointer@12/@16，stride4 strong引用，后于这两个nullable处理。
- StateSet：Category enum@4，缺失保持0，已知STANDARD=0/SCRIPTED=1，未知回退1；State列表count/pointer@8/@12，stride4 strong引用。EventSet同布局，Category SHELL=0/SCRIPT=1，未知回退1，列表标签Event。
- Transition：DestinationState@4是整个C字符串小写后的TypedAssetId hash，不是strong引用、不移除prefix；Condition为singleton节点文本的**direct** strong reference@8，present-empty不分配块。源typedAssetId方法84597，原Plugin交换word@4，引用由Tracker处理。
- EventRuntime：EventNameHash@4是signed strtol数值，不能重新hash或当unsigned解析；XferAlternateEvent@8是nullable TypedAssetId*，present-empty分配4字节再写hash0；IsXferable byte@12缺省true。原弱hash116233、nullable65707、Endian30373。没有额外imports来自这个弱hash。
- MusicPalette：Track列表count/pointer@4/@8，stride4 strong MusicScriptTrack引用；IsDefaultForNewMap byte@12缺省false。只需要已resolve索引，不以内联方式编译音乐脚本条件或轨道。
- State/Event/Transition强引用列表helpers65617/65662/65719，Palette Track列表65455；nullable Event strongRef84761；direct State/Transition refs116219/116279；Event directRef72657。统一遵守原index后缀的C atoi语义及Tracker index+1/排序/哨兵输出。

## PathMusicMapRuntime 的精确边界

这是一个**原XmlCompiler的真实no-op入口**：原Plugin233构建12字节Tracker根，读取并释放节点Value，未把其内容写入任何字段，随后只对零word@8做Endian操作并FinalizeTracker。恢复版精确输出12字节全零、空relocation/imports，接受任意原EA命名空间根文本/CDATA（含非空与DOM NUL），id和裸TypeId无payload作用。

这不是未知type的空fallback：桥接要求正确根名/TypeId，Native先查精确七根白名单，不支持的类型明确抛错；测试覆盖错误根名拒绝和所有有效Map文本的独立12字节全0断言。未知属性和child元素明确拒绝，未声称支持这些未命名schema输入。**不代表还原了专用MusicCompiler的PathMusicMap文件解析、音频转换或PathMusic源资产。**

## 验证

Native/Bridge独立Xmlpathmusic、Oracle/Cases和TestXmlpathmusic.ps1。原DLL只用于测试oracle；恢复实现只调用已有Relo/Scalar/Resource helper，内部使用唯一XmlpathmusicDetail命名空间，未改公共helper。

完整/Od、/O2、平台0/1/2/99，原ProcessInstance三数组逐字节和CompileXml另三数组；全部根缺省独立逐字节ABI，所有字段的缺失/空/非缺省、数值边界、两枚举所有值和未知回退、所有strong/weak/nullable引用与分配、列表长度0/1/2/3/7/28/29/30/31/32/65/129，256轮完整每根组合，输入拒绝矩阵。DOM构造NUL覆盖enum strcmp、hash及reference C字符串终止，ANSI/非ASCII值和大小写hash不变性；Map no-op有非空文本、CDATA和NUL的直接原版对照。

本地g++ -Wall/-Wextra/-Werror语法检查和C#词法结构检查通过。Windows完整矩阵已通过最终统一运行，结果写evidence/xml-pathmusic/validation.json与两优化日志；最终须核sourceCommit/runId及实际成功终态。此检查点不计入已验收82，不运行SDK，也不把组件结果冒充SDK支持。

