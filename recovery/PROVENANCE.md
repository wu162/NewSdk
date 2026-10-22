# Reconstructed component provenance

All tokens below refer to the original DLLs pinned by `evidence/*/summary.json`.

| Component | Original assembly | Method tokens | Recovered location |
|---|---|---|---|
| Relocation tracker, allocation, pointer/reference tables, exchange | Both DLLs; see exact per-method tokens in `RELOCATION_RUNTIME.md` | Tokenizer `0x06000007`–`0x06000011`, `0x060002F9`–`0x060002FA`; XmlCompiler `0x06000007`–`0x0600000F`, `0x0600036B`–`0x0600036C` | `src/Native/Relocation.h` / `.cpp` |
| Token-table member-offset bookmarks/fixup | Tokenizer | `0x0600000A`, `0x0600000C`, `0x0600081A` | `src/Native/Relocation.cpp` |
| Time and angle initialization | Tokenizer | `0x0600001B`, `0x0600001C` | `src/Native/DataInitializers.cpp` |
| All-types hash, version, type information | Tokenizer | `0x06000CEA`–`0x06000CEC` | `src/Managed/CompilerCatalog.cs` |
| All-types hash, version, type information | XmlCompiler | `0x06000AEA`–`0x06000AEC` | `src/Managed/CompilerCatalog.cs` |

The C++/CLI bridges and oracle hosts are new interoperability/test code. They are not asserted to be byte-identical reconstructions of original source text. Binary compilation erases comments, formatting, many field names and source-language distinctions; the goal is readable source with matching behavior and layout.

The catalog's source of truth is the exact `GetExtendedTypeInformation` case table in each original plugin. ProcessingHash is addition for Tokenizer and XOR for XmlCompiler; treating them identically would invalidate caches. Version values are 4100/4101/4102 and 4/6/7 respectively for Win32/Xbox360/PlayStation3. Both all-types hashes are 1424942948.

Time units are searched in order `ms`, `s`; angle units in order `r`, `d`. Matching uses case-sensitive substring search, not suffix validation. The parsed numeric double is rounded to float before the scale multiplication and then rounded to float again. The oracle compares the raw 32-bit float representation.

