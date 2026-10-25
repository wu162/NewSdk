# Complex FFT: 0xa7de3

The complete 1,016-byte, 366-instruction `a7de3..a81db` table-backed FFT is recovered in `XmaComplexFft.cs`: orders 0 through 15, forward and inverse, interleaved complex binary32 data, DIF butterflies, original twiddle recurrences, exact binary32 stores, bit reversal and inverse normalization. Orders >=16 use unrecovered trigonometric calls and remain unsupported.

The two original 16-float tables at e4698/e46d8 are embedded as exact bit patterns. Their combined 128-byte SHA256 is `56a90d2a8151b5ee2a69408386d3109f398c87f77a0be8510efcad68250fc525`. Explicit physical binary32 store barriers preserve original x87 rounding in Debug and Release.

Actual CPU validation: source `03d98ce656c5df660befd4af5166cbe7f4ac453d`, Actions run `40235031462`. All 192 cases (16 orders, both directions, six input patterns including denormals) matched every complex output bit in isolated Debug/Release candidates. Six separate safe refusals passed. `TestAudioXmaFft.ps1` retains evidence in `xma-fft-validation`, pins the original DLL SHA and actual x87 CW 027f, verifies/restores CW and rejects the original DLL in candidate hosts. This closes an internal numerical group, not an XMA encoder.
