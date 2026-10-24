"""Export bounded, lossless XmlCompiler source evidence for connector reads."""
import hashlib
import json
import pathlib
import re
import subprocess

BASE = "ba7a3d6845be5e124d08320cb2c56fda552d9fbe"
SOURCE = "recovery/decompiled/BinaryAssetBuilder.XmlCompiler/-Module-.cs"
EXPECTED_BLOB = "2d2f471c8008b7c2e1f78e39752d0726da395cea"
TYPES = [
    "AttributeModifier", "FXList", "SpecialPowerTemplate", "ObjectCreationList",
    "LocomotorTemplate", "LogicCommandSet", "FXParticleSystemTemplate",
    "UpgradeTemplate", "AIPersonalityDefinition", "AIStrategicStateDefinition",
    "AIMicroManagerData", "AIBudgetStateDefinition", "AIStateDefinition",
    "AITargetingHeuristic",
]
OUT = pathlib.Path("recovery/evidence/xml-source")
raw = subprocess.check_output(["git", "show", BASE + ":" + SOURCE])
blob = hashlib.sha1(b"blob " + str(len(raw)).encode() + b"\0" + raw).hexdigest()
if blob != EXPECTED_BLOB:
    raise RuntimeError("Original source blob differs: " + blob)
text = raw.decode("utf-8")
lines = text.splitlines(keepends=True)
OUT.mkdir(parents=True, exist_ok=True)
parts = OUT / "parts"
parts.mkdir(exist_ok=True)
chunks = []
for start in range(0, len(lines), 500):
    name = "part-%04d.cs.txt" % (start // 500)
    body = "".join(lines[start:start + 500])
    (parts / name).write_text(body, encoding="utf-8", newline="")
    chunks.append({"path": "parts/" + name, "firstLine": start + 1,
                   "lastLine": min(start + 500, len(lines)),
                   "sha256": hashlib.sha256(body.encode("utf-8")).hexdigest()})
if b"".join((OUT / p["path"]).read_bytes() for p in chunks) != raw:
    raise RuntimeError("Split source does not reconstruct exact original bytes")
methods = []
for n, line in enumerate(lines):
    if re.match(r"^\t(?:public|private|internal|protected)\b.*\(", line):
        methods.append({"line": n + 1, "part": n // 500,
                        "signature": line.strip()})
index = "\n".join("%d\tparts/part-%04d.cs.txt\t%s" %
                  (m["line"], m["part"], m["signature"]) for m in methods) + "\n"
(OUT / "method-index.txt").write_text(index, encoding="utf-8", newline="")
roots = OUT / "roots"
roots.mkdir(exist_ok=True)
root_manifest = {}
for name in TYPES:
    found = []
    for method in methods:
        signature = method["signature"]
        if not re.search(r"\b" + re.escape(name) + r"\s*\*", signature):
            continue
        start = method["line"] - 1
        end = start + 1
        while end < len(lines) and not re.match(r"^\t\}\s*$", lines[end]):
            end += 1
        if end == len(lines):
            raise RuntimeError("Unclosed source method at line " + str(start + 1))
        body = "".join(lines[start:end + 1])
        found.append("// Original source lines %d-%d\n%s" % (start + 1, end + 1, body))
    path = "roots/" + name + ".cs.txt"
    (OUT / path).write_text("\n".join(found), encoding="utf-8", newline="")
    root_manifest[name] = {"path": path, "methodCount": len(found)}
manifest = {"sourceCommit": BASE, "sourcePath": SOURCE, "sourceBlob": blob,
            "sha256": hashlib.sha256(raw).hexdigest(), "lineCount": len(lines),
            "parts": chunks, "roots": root_manifest,
            "methodIndex": "method-index.txt", "methodCount": len(methods),
            "scope": "Original source evidence only; no recovered support or DLL validation"}
(OUT / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
print(json.dumps({"sourceBlob": blob, "parts": len(chunks), "roots": root_manifest,
                  "methods": len(methods)}, indent=2))
