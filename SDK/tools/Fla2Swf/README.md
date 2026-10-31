# Flash 8 FLA → SWF

独立读取 **Macromedia Flash 8 二进制 FLA**，生成 **SWF 8 / AVM1**。
不调用 Flash、Animate、JSFL、COM 或浏览器，不需要安装 Flash 8。

输入必须是 OLE/CFB 容器，`Contents` 版本为 `0x3F`。MX/MX 2004、CS3/CS4、
ZIP/XFL、AS3 不能作为兼容输入使用。输出支持 FWS 和 zlib 压缩的 CWS。

## 使用

运行需要 Java 8 或更新版本。便携包中的 `fla2swf.jar` 和 `lib/` 必须放在一起。
仓库中的预构建包为 `Fla2Swf-Flash8.zip`，解压后可直接运行，无需 Python 或 JDK。

```bat
Fla2Swf.cmd "D:\UI\菜单.fla" -o "D:\UI\菜单.swf"
Fla2Swf.cmd "D:\UI\菜单.fla" --overwrite --font "D:\Fonts\Original.ttf"
```

Windows、Linux、macOS 也可直接运行：

```sh
java -Djava.awt.headless=true -jar fla2swf.jar input.fla -o output.swf
./run.sh input.fla --no-compress
```

不指定 `-o` 时，输出到输入文件旁边的同名 `.swf`。默认拒绝覆盖已有文件，
使用 `--overwrite` 显式覆盖。失败时返回非零退出码，保留输入和已有输出，
不会发布部分 SWF。

## 已实现

- 场景顺序、图层堆叠、帧时长、帧标签、普通分组、单个显示对象的遮罩。
- 直线、二次曲线、双侧填充和孔洞、实色/透明色、线性/径向/焦点渐变、位图填充。
- 实线的端帽、连接、miter、像素提示和缩放设置。
- 嵌套影片剪辑、图形元件的同步帧/循环/单帧、按钮四态、实例名、导出标识和九宫格。
- 经典运动补间的位移、缩放、旋转、颜色、匹配滤镜和自定义缓动；移动剪辑保留实例。
- 利用 FLA 中保存的对应曲线烘焙形状补间，支持实色、兼容渐变和同一位图之间的过渡。
- AS1/AS2 帧脚本，以及 `on(...)` / `onClipEvent(...)` 实例脚本，编译为 AVM1 字节码。
- 库位图解码后生成带预乘透明度的无损图像；静态文字嵌入字形，单样式动态/输入文字保留变量绑定。
- 颜色变换、混合模式、缓存位图、阴影/模糊/发光/斜角/渐变滤镜/调整颜色。
- PCM/MP3 事件音效和音量包络；PCM/MP3 时间轴流式音频按帧生成 PCM 块。

## 兼容边界

这不是 Flash 8 的全功能替代编译器。遇到以下内容会明确报错，而不会静默丢弃：

- 视频、导入的已编译剪辑、UI 组件、运行时共享库、外部 AS2 类链接和包含文件。
- 引导路径、递归元件、多个对象组成的遮罩。
- 虚线/点线等程序化描边；不兼容的形状补间样式、不同数量的渐变停靠点或滤镜。
- 旧式编辑器命令动作、富样式动态文字、竖排/上标/下标/带链接的静态文字、对齐的混合样式段落。
- 并行流式音轨混音，以及 PCM/MP3 以外的音频编码。

静态文字需要原字体，可通过重复的 `--font` 参数加载 TTF/OTF；缺字或缺失字体会报错，
不会替换成另一种字体。静态文字的复杂段落排版、图形元件中独立播放的嵌套剪辑、
跨场景的数字跳转与原生 Flash 编译器可能存在差异。请对目标 UI 做播放验收。

## 从源码构建和测试

需要 Python 3.8+、JDK 8+。首次构建联网下载固定版本的开源库并检查 SHA-256，
后续运行转换器不联网。

```sh
python setup.py --test
python setup.py --offline --test
python setup.py --offline --package /path/to/Fla2Swf.zip
```

`src/` 是本项目的导出器源码。`tests/` 使用真实 Flash 8 空文档，构造图形、
脚本、元件和音轨；测试包含 FLA 写入/重读、独立 SWF 解析、渲染像素、实例连续性、
补间、字体、版本拒绝和失败时保留已有文件。可用 `--no-compress` 检查产物的原始标签。

构建生成的 JAR、依赖和临时文件放在本目录的忽略项中；便携包包含源码、依赖和许可证。
第三方实现与版本见 [NOTICE.md](NOTICE.md)。
