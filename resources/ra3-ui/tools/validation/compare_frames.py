#!/usr/bin/env python3
"""Compare parsed FLA scene lengths/FPS with the corresponding original SWF headers."""
import argparse
import json
from pathlib import Path
import struct
import zlib


def swf_header(path):
    data = path.read_bytes()
    signature = data[:3]
    if signature == b"FWS":
        body = data[8:]
    elif signature == b"CWS":
        body = zlib.decompress(data[8:])
    else:
        raise ValueError(f"Unsupported SWF signature {signature!r}: {path}")
    rect_bytes = (5 + 4 * (body[0] >> 3) + 7) // 8
    rate_word, frame_count = struct.unpack_from("<HH", body, rect_bytes)
    return {"version": data[3], "frames": frame_count, "fps": rate_word / 256}


parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("resource_root", type=Path)
parser.add_argument("validation_report", type=Path)
parser.add_argument("output", type=Path)
args = parser.parse_args()
conversion = json.loads((args.resource_root / "conversion-report.json").read_text())
validation = json.loads(args.validation_report.read_text())
results = {row["movie"]: row for row in validation["movies"]}
rows = []
for converted in conversion["movies"]:
    movie = Path(converted["source"]).stem
    original = swf_header(args.resource_root / "source" / "ra3ui" / converted["source"])
    checked = results.get(movie, {})
    structure = checked.get("structure") or {}
    frames = structure.get("mainTimelineFrames")
    fps = structure.get("framesPerSecond")
    identity_matches = checked.get("flaSha256") == converted.get("flaSha256")
    matches = frames == original["frames"] and fps == original["fps"] and identity_matches
    status = "not-parsed" if frames is None or fps is None else "matched" if matches else "mismatch"
    rows.append({"movie": movie, "sourceSwf": converted["source"], "sourceSha256": converted["sourceSha256"],
                 "sourceHeader": original, "fla": converted["fla"], "flaSha256": converted["flaSha256"],
                 "validatedFlaIdentityMatches": identity_matches, "flaFrames": frames, "flaFps": fps,
                 "status": status, "productionCompilationStatus": checked.get("compilationStatus")})
report = {"total": len(rows), "matched": sum(row["status"] == "matched" for row in rows),
          "mismatched": sum(row["status"] == "mismatch" for row in rows),
          "unverified": sum(row["status"] == "not-parsed" for row in rows),
          "scope": "Original SWF header frame count/FPS versus parsed FLA main scene timelines; FLA identities match conversion report.",
          "movies": rows}
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(json.dumps(report, indent=2) + "\n")
print(json.dumps({key: report[key] for key in ("total", "matched", "mismatched", "unverified")}))
raise SystemExit(1 if report["mismatched"] or report["unverified"] else 0)
