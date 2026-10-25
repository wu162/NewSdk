# 下一完整组：W3D模型渲染5类（仍拒绝，未实现）

本轮已实现的Laser/Tracer/Stream与此组分开验收；先以RESUME.md的最终sourceCommit/runId核实已通过范围，不能把draw-source证据数量计为实现数量。

| TypeId | 类型 | native | Marshal / Endian / Token原入口 |
|---:|---|---:|---|
|1322199627|W3DScriptedModelDrawModuleData|216|099 / 026 / 211|
|1121390653|W3DSpotlightDrawModuleData|224|100 / 027 / 212|
|2323717438|W3DTankDrawModuleData|228|102 / polymorphic wrapper / 214|
|2615586235|W3DDynamicsDrawModuleData|252|097 / 024 / 209|
|2756454809|W3DTruckDrawModuleData|396|104 / 030 / 216|

文件均在`recovery/evidence/gameobject-modules/draw-source/NNN.txt`；以manifest签名和SHA256为准。当前283个原方法，17张PE枚举表，原Tokenizer blob6e40a9ed6c9f961d294d49768d911e6a6f354619，PE SHA256 ba5e34e9a44509d5489086f610c3bb4502b441c1513740fb66d6b740ad3eb603。ExportW3DDrawMethods.py已补捕获nested polymorphic helper和FXShaderConstant** source参数名；全局ModuleData**分派是显式source-inventory边界，不能假称已展开全部模块。

共同完整父层Scripted216：Name String8、InitialRecoilSpeed/RecoilSettleSpeed乘原SECONDS_PER_LOGICFRAME_REAL，其他相关float保持原缩放语义；MinLODRequired、WeaponSlot6位集、DependencySharedModelFlags完整模型状态位集、ExtraPublicBone的String列表、AttachToBoneInAnotherModule、左右TrackMarks骨名、WadingParticleSys强引用、完整布尔组、LOD/Alpha/BirthFadeTime。4个完整结构列表ModelConditionState@164、AnimationState@172、AttachModel@180、EmbedPortal@188必须同时实现，不能只启用根属性。

|结构依赖|完整原方法入口|列表stride的直接证据|
|---|---|---|
|ModelConditionState|084 / 016 / 197|165：196字节|
|AnimationState|066 / 001 / 178|151：140字节|
|Animation|065 / 000 / 177|152：60字节|
|ParticleSysBone|088 / 018 / 201|168：36字节|
|ScriptedModelDrawAttachModel|091 / 020 / 204|171：80字节|
|ScriptedModelDrawEmbedPortal|092 / wrapper / 205|172：12字节|
|ScriptedModelDrawModel|093 / wrapper / 206|173：8字节|
|ArticulationBone|067 / wrapper / 179|153：16字节|
|DynamicsBoneVolumeType|072 / wrapper / 185|155：84字节|
|DynamicsBoxShapeType|073 / direct闭包 / 186|156：24字节|

ModelConditionState196含ParseCondStateType、Name、ConditionsYes、Skeleton、PortraitImage/ButtonImage、OverrideTooltip、RetainSubObjects、id；Model列表、nullable FXShaderMaterial28、Texture替换、四种武器骨列表、ParticleSysBone、FXEvent、nullable ShadowInfo60、Turret及SubObject。不要将Name/id/ConditionsYes的哈希、字符串或位集互换；必须读完整原方法/Token/Endian。共享FXShader28与四种常量可复用本轮已验证实现，未知常量保存type0、Token无嵌套表。

AnimationState140含ParseCondStateType/AnimNickName/ConditionsYes/Name/StateName、Flags默认NONE、ShareAnimation=false、EnteringStateFX、FrameForPristineBonePositions=0、AllowRepeatInRandomPick=false、SimilarRestart=false；完整Animation60、Script、FXEvent、LuaEvent、ParticleSysBone列表及字符串/引用分配顺序。Animation60含Flags、AnimationName引用、AnimationMode/AnimNickName、Distance0、BlendTime5、MustCompleteBlend=false、速度范围1、AbsoluteTime0s、UseWeaponTiming=false、WeaponTimingOrdering、WeaponTimingSlotID1、Priority1。结构中可选/默认字段与父层写入顺序须按原方法核对。

Spotlight自身RefreshTime/SweepTime默认1s后完整Scripted父层。Tank的TreadTerrainParticles是nullable强引用（分配4字节），ArticulationBone列表后完整父层；不要将可空引用当inline句柄。Truck含16个TireBone String8、多个旋转float、Cab/Trailer String8、nullable TreadTerrainParticles以及TreadScrollingMap，随后完整父层；原方法104与TreadScrollingMapping095/022/208整组核对。

Dynamics252含Collision、IntuitiveCollision默认COLLIDES_WITH_GROUND_ONLY、DrawPriority默认MUST_BE_DRAWN、LODPriority默认VERY_LOW、Explodiness/FlingPerturbation0、InitiallyActive=true；BoneVolumes与Lifetime均是nullable8字节结构，另有BoneVolume列表。DynamicsVolumeType080/011/193含Mass0、Bounciness30%、Spinniness2、StaticFriction.6/KineticFriction.5、AngularDrag/LinearDrag.1、ReportContacts=false、ContactTagNONE，以及Sphere/Capsule/Box/Cylinder/Triangle完整结构列表；Triangle还依赖顶点/四元数/Vector3。所有结构、单位缩放、枚举未知回退与复合分配必须整体核对。

下一轮继续完整能力组实现，保留全部累计/Od/O2、平台0/1/2/99、三缓冲区逐字节差分及独立布局/Token/拒绝/不变性；现在累计作业上限30分钟，不能通过删样本解决超时。新组先运行，随后所有历史回归，避免构建/新语义错误要等待历史样例跑完才出现。约1小时阶段规则不变，SDK继续按大里程碑集中验证；两个完整替代DLL、完整GameObject/SDK/mymod集成仍是独立门槛。
