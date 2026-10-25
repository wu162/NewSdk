# W3X FXShader 源码合同

`src/Native/W3x/Shader.h/.cpp` 恢复 FXShaderMaterial 内联28字节和 Bool/Int/Texture/Float 四种常量的 typed input → shared Tracker 写入。依据取证 `40162934653-1/decompiled/-Module-.cs`：Marshal FXShaderMaterial 3407、常量分派3375、派生分配4527–4577、EndianCorrect3454–3501、3645–3730、4579–4650、浮点list4797与MarshalString4234/TextureReference4326。不修改旧GameObject或共用源码。

布局：材质 ShaderName string8@0、TechniqueName string8@8、Constants count/pointer@16/20、TechniqueIndex byte@24。Name string8@4；常量 TypeId@0，Bool byte@12/size16，Int int32@12/size16，Texture reference@12/size16，Float list count/pointer@12/16/size20。未知TypeId分配base12并写TypeId0，仍保留Name。Bool2750958653、Int2300057986、Texture2777716390、Float3741900581。

原分配顺序：ShaderName（若属性存在）、TechniqueName（现存FXShader节点默认空字符串，仍分配NUL）、constants指针数组、按输入顺序逐个派生常量；Float先分配Value数组，再分配Name字符串；其余派生值先写入，再Name。缺失整个FXShader节点不调用writer，因此不产生TechniqueName默认分配。未知常量忽略派生Value。

MarshalString及TextureReference复用现有ResourceMarshaling.cpp匹配方法。字符串长度不含NUL，分配size1/count(length+1)；引用只有BAB预处理文本含反斜杠时，以后缀CRT atoi的uint32表示调用AddReference，引用值与imports由Tracker序列化。资源名字的解析不属于此层。

EndianCorrect：根字符串length、constant count、TypeId、Name length、Int32与Float count/value按32位换序；bool/TechniqueIndex不变。Texture引用最终由Tracker处理，不能提前交换。Float数组原方法通过uint32 store换序，不走float-return EndianToPlatform，故不应用Timecoded发现的swap后sNaN quiet行为。

本地GCC C++11 O2语法检查通过；x86三缓冲区/原版动态验收由本批完整Mesh oracle统一执行。未声明SDK集成或正式插件启用。独立函数CompileShaderMaterial可用于材质根28原版Marshal/EndianCorrect差分。
