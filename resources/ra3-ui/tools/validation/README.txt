Local verification of recovered RA3 FLA and ActionScript

Requirements: .NET SDK 8 and this repository's SDK/tools reference DLLs.
No GitHub Actions workflow, external Flash editor or game process is started.
Run the following from the repository root:

dotnet build resources/ra3-ui/tools/validation/UiValidation.csproj -c Release -p:TargetFrameworks=net8.0
dotnet resources/ra3-ui/tools/validation/bin/Release/net8.0/UiValidation.dll resources/ra3-ui/converted work/ra3-ui-validation --mutate-real A01_Global_Briefing
python resources/ra3-ui/tools/validation/compare_frames.py resources/ra3-ui work/ra3-ui-validation/report.json work/ra3-ui-validation/frame-comparison.json

The first argument can also be a single FLA file. The second argument is a
separate writable output directory; use a directory outside the input tree.
Use --structure-only to read all binary FLA pages and media without attempting
production compilation. The default mode performs both checks and records the
results separately. Failed movies do not stop the remaining movie checks.

Each movie produces result.json plus its final .manifest/.bin/.relo/.imp files
under baseline/. The overall report.json records success/failure, complete FLA
consumption of referenced Contents/page/media streams, library/media/text counts,
input dependencies, compiler DLL
identity, and hashes of the four final streams. A failing movie is not counted
as a successful production compilation merely because its FLA parses correctly.

--mutate-real reads the selected movie's script-map.json and selects the original
DOMDocument.xml scriptOrdinal=1 entry, using its current meaningful AS filename.
Both movie-relative and converted/<Movie>/... resource-relative paths work. It
copies the movie into an isolated temporary source directory, appends a trace
marker to that existing included AS file, and recompiles. It checks that the
final binary changes, the marker was absent from baseline and present after
the edit, the AS appears in compiler dependencies, and the FLA hash is unchanged.
The modified AS and changed four streams are retained under mutated/. Original
inputs are never modified. --mutate instead tests an optional same-name timeline
AS file and is intended only for simple fixtures without extracted scripts.

These checks prove binary FLA parsing and the current NewSdk compiler's actual
handling of each movie. They do not replace Adobe Flash editor or in-game UI
playback verification. The compiler's unsupported AS/features remain errors.
Static font outline compilation, if encountered, requires Windows and the
correct installed fonts; dynamic text preserves its runtime font name.
