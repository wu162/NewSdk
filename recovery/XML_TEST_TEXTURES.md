# TestTexture / TestTextureCollection

2026-10-25（北京时间）。完整两个原XmlCompiler入口，与TestGameObject实现无关；SDK未集成/未验证。

| 根 | TypeId | TypeHash | 大小 | 全局schema |
| --- | ---: | ---: | ---: | --- |
| TestTexture | 2826773070 | 275489259 | 20 | 是 |
| TestTextureCollection | 49556361 | 2883151314 | 12 | 是 |

固定Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`、Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`。原Module31932、66423、66468、66534、18350（DataBlob完整fopen/fseek/ftell/fread合同）；Plugin7925/8035两个Handler及GetExtendedTypeInformation/Tracker大小；CnC3Types681–684全局声明均核实。

Texture根word0保留；CustomData是string length@4/pointer@8，缺失不分配，present empty也分配NUL。File列表count@12/pointer@16、项8字节，pointer@0/length@4；每个File的节点文本是本地路径。先CustomData，再整个File数组，再按列表顺序分配每个文件原始字节；字符串为原StringToHGlobalAnsi的ANSI C-string，File也依同一IValue窄字符串合同。

DataBlob按原fopen("rb")读取文件，fopen失败留下pointer/length均0（File数组项仍存在）；成功空文件Push(1,0)仍有重定位，target可等于实例末尾，不能误删成空指针。短读仅把长度清0，原已分配字节块保留。payload不做endian转换，只有string/list/file长度按平台swap；指针交Tracker统一处理。非seekable/无效length显式拒绝，不模拟不安全的巨大分配。

Collection根保留word0，Texture文本列表count@4/pointer@8、强引用4字节项，按原index+1与ownerblock/index排序输出imports。

新Xmltesttextures Native/Bridge/Oracle/Test脚本/catalog，无共享helper改动。仅临时夹具文件供组件差分，不修改原SDK文件、也不调用SDK真实编译。

完整Oracle保留/Od/O2、0/1/2/99、原ProcessInstance三数组+直接CompileXml三数组；临时文件0..65537所有padding边界逐字节ABI、空文件target=end、失败路径/NUL前缀、两列表0..1024、CustomData ANSI/Unicode/NUL/0..65535长度、全部强引用边界、独立40字节File1+empty/48字节CustomData先分配、导入/重定位顺序与拒绝。SideArray仅为已证zero-length DataBlob放行末尾target，并核对应count0，不放宽其它指针越界。最终以Windows实际sourceCommit/runId的完整矩阵成功为准。
