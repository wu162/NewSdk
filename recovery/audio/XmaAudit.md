# EA-XMA recovery boundary

Original DLL SHA256 `a14d15addef502c2df6f6ba272d6098c4c131fbea9c3ed0149c11c4556347139`.

XMA is not merely an absent external codec DLL or an obviously empty registration stub. The original format39 writer selects FourCC `EXm0` (`0x45586d30`, immediate near RVA `0x2e01f`). The registry data at `0xf95ac` points to constructor `0xed60`, which allocates the encoder object, sets vtable `0xac870`, initializes per-stream state and buffers, and records bitrate. The vtable includes initialization `0xecc0`, encode/buffering `0xf400`, maximum-output sizing `0x14250`, reserve sizing `0xeb70`, and destruction `0xeba0`.

`0xf100` assembles source state and calls the substantial statically linked codec API at `0x589d0` (call at `0xf275`), followed by encoded packet processing at `0xee00`. API `0x589d0` creates an internal codec object through `0x5a6d0` and calls its vtable method. Further codec internals remain unrecovered. The wrapper's float-to-signed16 conversion at `0xe9b0` uses `FISTP` rounding rather than the plain PCM encoder's truncation; reusing the PCM conversion blindly would be incorrect.

PE imports are MSVCR80, KERNEL32, msvcm80, MSVCP80, ADVAPI32 and mscoree. This import inventory and the nonempty encoder path rule out treating XMA as a simple missing `xmaencoder.dll` dependency; they do not establish full encoder behavior or portability. The source backend still explicitly rejects representation28.

`xma-wrapper-disassembly.txt` preserves the cited wrapper/API instruction ranges. `AudioUnrecoveredProbeCases` contains eight original-only Xenon XMA cases (mono/stereo, 257/4096 frames, RAM/stream, quality75, rate22050). Their buffers/custom data or exact original errors are evidence for the next complete implementation batch, never candidate acceptance. Actual source `1c0457599f2a78082a283aade83a19e3f60a430e`, run `40199000936` attempt1: all eight original-only cases produced real buffers and CustomData without errors. Full transcripts and source WAVs are in `recovery/evidence/audio/plugin-validation/40199000936-1`. This establishes working original XMA encoding on Windows, not recovered support.
