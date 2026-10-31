The binary FLA fixtures are from JPEXS FLA Convert 3.0.0, tag `v3.0.0`,
`testdata/fla/f8`, https://github.com/jindrapetrik/jpexs-fla-convert.
They are redistributed under LGPL-2.1-or-later; see
`../../licenses/JPEXS-LGPL-2.1.txt`. `flash8-empty.fla` is the upstream
`0001_empty_doc.fla`. These are real binary Flash 8 source documents, not
renamed SWF or ZIP/XFL files.

`CoronaUIMain.swf` is a reference **test input**, copied from NewSdk's
`feature/swf-direct-compiler` at `148681ad7bfecf571b2ba5c49c000594539bc17a`.
Its SHA-256 is checked before conversion. The APT/const/DAT golden digests
come from that commit's `ui/evidence/source-consistency.json`, run
`40288873546`, where the unchanged attachment-matched .NET 8 x86 tool was
executed with `ra3=true,optimize=false`. The product neither accepts this
SWF fixture as input nor generates intermediate files.
