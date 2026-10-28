# Legacy PC compatibility modifications

The source bases remain pinned by upstreamBlobSha/upstreamSha256 in PROVENANCE.json. The current modified source is recorded by blobSha/sha256, not misrepresented as unmodified upstream. This experiment does not read or call the original DLL, and never consults sample-specific output tables.

1. squish/colourblock.cpp: in the four-colour writer, canonicalize equal endpoints using nvDDS's increment/wrap/constant-index rule. The decoded original endpoint is preserved, including black and white boundaries. It changes the actual copied writer.
2. squish/alpha.cpp: canonicalize all16 equal alpha values to(a,min(a+1,255)) with zero indices, as required by immutable original output. Valid-pixel masks other than full16 retain upstream behavior.
3. nvtt/icbc.h: apply the same degenerate four-colour canonical form in output_block4. It preserves decoded pixels and their error metric.
4. amd/compressonatorxcodec.cpp/.h: add CompBlockLegacyPC over the upstream quantizer, considering8 levels,6 levels,6 excluding zero,6 excluding one,6 excluding both. Preserve normalized-float input error, low-endpoint-first ties and integer-palette/index packing. The adapter also tries the upstream normalized-float RGB API rather than discarding original source precision through packed-byte input. Original search APIs remain available for baseline comparison.

The five-candidate logic and canonical forms follow already established, verified runtime contracts in the repository; this is source adaptation, not another DLL reverse-engineering pass. Full original10240-block Debug/Release equality remains mandatory. Matching more cases is progress, not permission to enable a partially matching backend.
