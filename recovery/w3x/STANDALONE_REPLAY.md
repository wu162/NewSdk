# Standalone W3X component replay

`tests/W3xStandaloneReplay.cs` consumes exactly two absolute capture paths, one per animation/mesh route. Captures use BinaryWriter's `W3X-FIXTURE-1` string, route string, then records to EOF: Int32 platform, XML string, three Int32-length-prefixed original buffers (instance, relocations, imports). The capture writer records only fixtures whose original/recovered three-buffer comparison already passed.

The host references only Core and the two recovered bridge assemblies. It requires x86, rejects missing/duplicate routes, incomplete capture sets (exactly675 animation and666 mesh fixtures:651 legal and15 raw-source mesh probes), malformed/truncated records, null output, buffer length differences and every differing byte. It refuses an original W3X DLL in its program directory, rejects the original assembly through an AssemblyLoad guard, and checks loaded assemblies at start and successful completion. It reconstructs namespace-aware XML and calls the recovered public CompileXml methods directly.

A successful replay proves those captured component fixtures run without loading the original W3X compiler. It does not establish SDK integration, preprocessing coverage or final DLL replacement. The workflow's immutable validation and run logs are the execution evidence; this document alone does not claim a passed run.
