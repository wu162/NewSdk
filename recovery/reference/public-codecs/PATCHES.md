# Legacy PC compatibility modifications

The source bases remain pinned by upstreamBlobSha/upstreamSha256 in PROVENANCE.json. The current modified source is recorded by blobSha/sha256, not misrepresented as unmodified upstream. This experiment does not read or call the original DLL, and never consults sample-specific output tables.

1. squish/colourblock.cpp: in the four-colour writer, canonicalize equal endpoints using nvDDS's increment/wrap/constant-index rule. The decoded original endpoint is preserved, including black and white boundaries. It changes the actual copied writer.
2. squish/alpha.cpp: canonicalize all16 equal alpha values to(a,min(a+1,255)) with zero indices, as required by immutable original output. Valid-pixel masks other than full16 retain upstream behavior.
3. nvtt/icbc.h: apply the same degenerate four-colour canonical form in output_block4. It preserves decoded pixels and their error metric.
4. amd/compressonatorxcodec.cpp/.h: add CompBlockLegacyPC over the upstream quantizer, considering8 levels,6 levels,6 excluding zero,6 excluding one,6 excluding both. Preserve normalized-float input error, low-endpoint-first ties and integer-palette/index packing. The adapter also tries the upstream normalized-float RGB API rather than discarding original source precision through packed-byte input. Original search APIs remain available for baseline comparison.

The five-candidate logic and canonical forms follow already established, verified runtime contracts in the repository; this is source adaptation, not another DLL reverse-engineering pass. Full original10240-block Debug/Release equality remains mandatory. Matching more cases is progress, not permission to enable a partially matching backend.

## Second compatibility iteration

The AMD scalar quantizer now uses a constrained Lloyd fit with local bin-extreme
moves, adapted from the repository's readable `TextureDxt5AlphaEncoder` reference.
This replaces AMD's grid search for the compatibility entry only; the original
AMD entry is retained. It does not call the recovered native quantizer or original
DLL. The reference has known scalar/x87 tie limitations, so this remains an
experiment until the complete comparison proves equality. RGB still uses AMD's
public encoder. All source hash changes are explicit in PROVENANCE.json.

## S3 four-colour contract

The initial S3 run40631487946 compiled but stopped at the first grayscale block
that chose a three-colour midpoint. DXT3/5 require four-colour RGB regardless of
endpoint ordering. The copied allSame search now excludes that BC1-only candidate
when the caller requests four colours. This changes a general format contract,
not expected bytes for a fixture. Both original upstream and patched hashes are
retained. The failed partial run cannot establish any matrix acceptance.

## S3 grayscale compatibility data

LegacyGray4.inl contains the existing PC codec's256 fixed grayscale optimization
records, already preserved as owned source in TextureColour.asm at
D_000B9608+8192 / original RVA0xBB608. These are codec algorithm constants, not
saved DXT differential payloads. Their original source, source-data SHA256 and
new file hash are explicit in PROVENANCE.json. The copied S3 single-colour entry
uses them only for one common quantized grayscale value; non-gray search remains
S3. This is a hybrid compatibility adaptation and is not described as unmodified
public S3 code or as independently generated optimization tables. No original
DLL or existing recovered encoder function is called.

## Explicit source search-budget comparison

The original complete payloads differ on22/4096 random blocks after the first
S3 adaptation;21 of those S3 blocks have lower decoded weighted error than the
legacy output. This suggests preserving the legacy stopping rule matters more
than maximizing quality. Add an explicit source budget selector: default16/8
unclipped/clipped iterations, then0/0,1/1,2/2. Each budget runs all four input/weight
precision variants and all10240 original blocks in both configurations. This is
an experiment; lower error or selecting best results per fixture is not accepted.
No original-output data is used to choose a budget at runtime.

## Interpolation rounding comparison

40632315333 completed the full stopping-policy matrix. Gray adaptation improved
complete DXT3/5 equality to5093/5120. Lower0/0 budgets regress4 blocks and fix none;
1/1 and2/2 do not change the remaining27. Restore upstream16/8. The remaining
single-color cases suggest the decoder's one-third interpolation rounding may
matter (e.g. input green3 between endpoints4 and0: rounded gives3, floor gives2,
changing strict endpoint ties). Compare single-colour-table floor, general-ramp
floor and both, independently from baseline, with separate cached lookup tables.
Variant /4 now identifies rounding policy0..3; variant bits0..1 still preserve
source/weight precision. Keep every complete prior matrix; never compare variant
numbers across receipts without checking sourceCommit. This is a general codec
arithmetic hypothesis and not fixture-ID dispatch.

## Stage diagnostic checkpoint

S3_STAGE_TRACE adds observation-only hooks to single-colour errors and final rounding inputs/outputs. A separate diagnostic reference is built from the already accepted colour-state source (40500150864); instrumented outputs must still match immutable original block gold. It is never invoked by the candidate encoder, never supplies candidate output, and does not establish integration eligibility. The 27 outlier identities occur only in the diagnostic host. No rejected interpolation/precision/budget matrix is repeated unchanged.
