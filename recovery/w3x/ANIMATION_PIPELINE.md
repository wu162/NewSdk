# W3X animation pipeline recovery

Source: original DLL extraction `40162934653-1`, `Plugin.HandleAnimation` token `0x06000145` (RVA `0x76f4`), original IL and decompiled Plugin; animation marshaler in `-Module-.cs`.

`AnimationPipeline` defines the typed boundary after XML marshaling; it does not reproduce VC8 object ABI. Root input Hierarchy is the resource preprocessing index, not a hash. `PrepareAnimation` expands each channel to NumFrames indexed samples. Scalar usages 0/1/2 start at zero, usage4 starts at one; other scalar usages reject. Quaternion samples start at [0,0,0,1], with no usage restriction in the original switch. Authored samples overlay at FirstFrame+i, including BinaryMove. No interpolation, quaternion sign adjustment or normalization occurs at this layer.

Missing entire CompressionSettings uses platform defaults (.002 translation, .003 rotation, .01 visibility, .001 delta error, reduction1, both algorithms enabled); this branch rejects platforms outside0/1/2. A present node uses marshaler defaults (.001,.01,.01,.03,1,true,true). If both algorithms disabled the original forces timecoded, zeroes translation/rotation/visibility error and reduction1, preserving delta error. Explicit settings permit unknown platforms and all nonzero platform values select big endian.

Selection attempts delta first when allowed; a successful delta with timecoded disabled bypasses timecoded. Otherwise timecoded is attempted even if its allow flag is false (fallback after delta failure). Delta wins ties between the original size estimates.

The shared tracker is used for the runtime root24, hierarchy reference slot4, channels count16/pointer20 (including zero-count pointer allocation), and algorithm output allocations. Runtime type slot0 remains zero. Input frame range overflow and incorrect typed vector widths reject rather than reproducing original unchecked memory access. Zero-frame channels reject at final compilation because original timecoded WriteOut unconditionally dereferences its first frame.

Validation status: independent preparation/selection tests pass locally under g++ C++11 O0 and O2. These tests cover root fields, all platform defaults, present/absent settings, disabled fallback, scalar and quaternion defaults/overlays/BinaryMove, selection ties and rejection boundaries. Original DLL three-buffer differential remains required; local tests do not establish binary equivalence. No SDK validation performed.
