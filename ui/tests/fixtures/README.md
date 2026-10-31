The binary FLA fixtures are from JPEXS FLA Convert 3.0.0, tag `v3.0.0`,
`testdata/fla/f8`, https://github.com/jindrapetrik/jpexs-fla-convert.
They are redistributed under LGPL-2.1-or-later; see
`../../licenses/JPEXS-LGPL-2.1.txt`. `flash8-empty.fla` is the upstream
`0001_empty_doc.fla`. These are real binary Flash 8 source documents, not
renamed SWF or ZIP/XFL files.

`legacy-assets.sha256` contains 54 SHA-256 digests of the **final**
root-prefixed AssetBuffers produced at commit
`e235cededd385a6895895ceb354f46a6667caaa3`, before removing the intermediate
SWF/APT pipeline. Keys are `source-file:asset-type:asset-name`. The baseline
covers all the Flash 8 fixtures plus the source-folder/include/AS2-class
scenario in `Program.cs` after the include changes. Tests compare the direct
compiler's final buffers without retaining the old intermediate encoders.
