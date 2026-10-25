# W3X mesh XML adapter

`src/Bridge/W3x/MeshCompiler.cpp` exposes `Recovered.W3x.MeshCompiler.CompileXml` as a component adapter for BAB-preprocessed XML. It marshals into `MeshAssetInput` and calls the recovered mesh serializer. It does not change production plugin routing or launch the SDK.

Source: immutable extraction `40162934653-1`, original Plugin.HandleMesh token `0x06000146`, W3DMesh/PipelineVertexData marshaler and nested vector/triangle/AABTree/Shader routines in `decompiled/-Module-.cs`. Child/list navigation uses the existing XmlNodeAdapter; scalar parsing uses the unchanged shared Windows CRT-compatible parser.

Covered input: vertices/normals groups, optional tangents/binormals/colors, UV and bone-influence groups; root geometry/Hidden/CastShadow/SortLevel; bounds and sphere; triangle index lists/normals/distance; optional AABTree poly indices/node min/max/children/polys; embedded ShaderName/TechniqueName/index and four polymorphic constant types. Constants dispatch only by preprocessed TypeId, with unknown IDs preserving the original base Name behavior. Texture strings retain BAB's resource suffix for the Shader writer.

Geometry enum mapping was checked against original PE enum-map entries and their pointed-to strings: CameraAligned=2, CameraOriented=3, Normal=0, Skin=1. Missing attribute leaves zero; an unrecognized present value selects2. Optional scalar attributes preserve zero initialization. An existing FXShader node receives the empty TechniqueName getter default; a missing FXShader does not invoke the writer. ShadeIndices only enters the original temporary pipeline tracker and is never read by BuildVertexData, so it has no serialized output.

Validation: source contract review completed. C++/CLI compilation and complete original mesh three-buffer component differential are separate required gates. No SDK verification is claimed.
