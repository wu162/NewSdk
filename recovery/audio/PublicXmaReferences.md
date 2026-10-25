# Public XMA implementations used as cross-checks

2026-10-25 (Beijing). Public sources were read alongside the original DLL.
The recovered implementation still follows the pinned original instructions;
public format assumptions never override an observed EA wrapper difference.

| Source | Exact file blob | Use |
|---|---|---|
| [minidx9 xma2defs.h](https://github.com/hrydgard/minidx9/blob/master/Include/xma2defs.h) | `13a4306cb87326f4da66770f82eacb6cdc6c166a` | Independent 2048-byte packet, 32-bit header, 15-bit first-frame offset and frame-length layout. |
| [FFmpeg xma_parser.c](https://github.com/FFmpeg/FFmpeg/blob/master/libavcodec/xma_parser.c) | `5768b603da63da618b9c0281872daf753764f9a0` | Shows the header frame-count/skip-count route used for XMA2 duration analysis. This is not an XMA encoder. |
| [vgmstream ffmpeg_decoder_utils.c](https://github.com/vgmstream/vgmstream/blob/master/src/coding/ffmpeg_decoder_utils.c) | `d9d1f275551b50dc8236f76297dba24e524d8a86` | XMA1/2 RIFF wrappers and stream/channel metadata context; not an encoding replacement. |

The public XMA2 header extracts the first-frame position with
`((packet[0]&3)<<13)|(packet[1]<<5)|(packet[2]>>3)`. This independently agrees
with the original EA helper's `(BE32>>11)&0x7fff`, and is checked when generating
real-packet fixtures. The ordinary 15-bit frame-length field is also checked
independently from the recovered instruction sequence.

The public helpers cannot directly replace this EA packet scanner:

- XMA2 frame-count/skip-count traversal differs from original `0xef60`, which
  follows frame-tail continuation bits with fixed 2048-byte packet steps.
- The EA scanner input includes a four-byte EA block prefix. It is not the
  unwrapped raw XMA2 input accepted by the public helpers.
- Original `0xea80` always reads three bytes for positive counts. A public
  reader that uses two bytes for some aligned fields changes the bounded-read
  contract. Original `0xeae0` explicitly splits a length at `0x3fd1`, skipping
  the next packet's header.
- A public `0x7fff` invalid-length convention is not an observed special case
  in original `0xeae0`; it is not silently added to the recovered reader.

These references inform independent layout assertions and boundary fixtures.
No public implementation is presented as proof of byte-identical EA encoding.
XMA audio encoding, the numerical codec and the production XMA route remain
unrecovered. Existing public decoder/parser code does not fill that gap.
