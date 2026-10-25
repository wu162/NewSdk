#pragma once
#include "MeshBuilder.h"
#include "../Relocation.h"
namespace Recovered { namespace W3x {
// Isolated W3DMeshVertexData root, not a complete W3DMesh asset. This uses the
// existing x86 Tracker and the original BuildVertexData allocation order.
Relo::Chunk CompileMeshVertexData(const MeshPipelineData& input, int platform);
}}
