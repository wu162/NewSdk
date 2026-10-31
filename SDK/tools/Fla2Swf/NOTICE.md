# Third-party libraries and format references

The converter links to these unmodified, replaceable libraries. The setup script
pins release URLs and verifies their archive SHA-256 values. Distribution keeps
the original libraries separate in `lib/`, with the license and attribution files
provided by their release archives.

- **JPEXS FLA Convert 3.0.0**, by Jindra Petřík / JPEXS, LGPL 2.1 or later.
  [Source and build instructions](https://github.com/jindrapetrik/jpexs-fla-convert/tree/v3.0.0).
  Used for CFB/MFC deserialization, the Flash 8 document model, and bitmap recovery.
  The `tests/fixtures/flash8-empty.fla` fixture is from the same release, with its
  upstream license in `tests/fixtures/LICENSE.md`.
- **JPEXS Free Flash Decompiler Library 26.4.0**, LGPL 3.0 or later.
  [Source and build instructions](https://github.com/jindrapetrik/jpexs-decompiler/tree/version26.4.0).
  Used for AVM1 compilation, font embedding, color matrices, MP3 decoding and
  independent SWF validation/rendering. Its release archive also includes its
  dependencies and attribution notices; see `lib/ffdec-README.md` and the
  `lib/*license*` files after setup. The older bundled `flacomdoc.jar` is replaced
  by the separately pinned `fla_convert.jar` to avoid duplicate classes.

The Flash 8 input revision is taken from JPEXS's `FlaFormatVersion.F8` (`0x3F`),
and checked independently of the SWF output version. The SWF encoder in `src/`
is implemented here; the output is parsed again before it is published.

Additional research reference: [eddiemoore/fla-decoder](https://github.com/eddiemoore/fla-decoder),
MIT, for its binary FLA format notes. No source code from that project is vendored.
