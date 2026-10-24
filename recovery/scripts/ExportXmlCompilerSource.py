"""Export bounded, lossless XmlCompiler source evidence for connector reads."""
import hashlib
import json
import pathlib
import re
import subprocess
import struct

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
    if (re.match(r"^\t(?:public|private|internal|protected)\b.*\(", line)
            and n + 1 < len(lines) and lines[n + 1].strip() == "{"):
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

# Resolve all encoded literal fields and PE-backed enum names once, so workers
# need not repeatedly load a multi-megabyte module or download the original PE.
pe = pathlib.Path("SDK/tools/BinaryAssetBuilder.XmlCompiler.dll").read_bytes()
pe_offset = struct.unpack_from("<I", pe, 60)[0]
image_base = struct.unpack_from("<I", pe, pe_offset + 52)[0]
section_base = pe_offset + 24 + struct.unpack_from("<H", pe, pe_offset + 20)[0]
def string_at(va):
    for n in range(struct.unpack_from("<H", pe, pe_offset + 6)[0]):
        _, virtual_size, base, raw_size, offset = struct.unpack_from("<8sIIII", pe, section_base + 40*n)
        if base <= va - image_base < base + max(virtual_size, raw_size):
            start = offset + va - image_base - base
            return pe[start:pe.index(b"\0", start)].decode("ascii")
    raise ValueError("Enum string VA outside PE sections: " + hex(va))
literal_rows = []
enum_manifest = {}
enum_dir = OUT / "enums"
enum_dir.mkdir(exist_ok=True)
for n, line in enumerate(lines):
    match = re.search(r"internal static (\S+) (\S+)\s*/\* Not supported: data\(([^)]+)\)", line)
    if not match:
        continue
    kind, symbol, encoded = match.groups()
    value = bytes.fromhex(encoded)
    row = {"symbol": symbol, "sourceLine": n + 1, "type": kind, "hex": value.hex()}
    if symbol.startswith("_003F_003F_C_") and value.endswith(b"\0"):
        row["text"] = value[:-1].decode("latin1")
    literal_rows.append(row)
    if "enumMap" not in symbol:
        continue
    group_match = re.search(r"W4([^_]+)_0040", symbol)
    group = group_match[1] if group_match else hashlib.sha1(symbol.encode()).hexdigest()
    if len(value) % 8:
        raise RuntimeError("Invalid enum map size for " + group)
    entries = [{"name": string_at(pointer), "value": number}
               for pointer, number in struct.iter_unpack("<Ii", value)]
    path = "enums/" + group + ".json"
    payload = {"sourceBlob": blob, "sourceLine": n + 1, "symbol": symbol, "entries": entries}
    (OUT / path).write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
    enum_manifest[group] = {"path": path, "count": len(entries)}
literal_dir = OUT / "static-data"
literal_dir.mkdir(exist_ok=True)
literal_parts = []
for n in range(0, len(literal_rows), 200):
    path = "static-data/fields-%04d.json" % (n // 200)
    (OUT / path).write_text(json.dumps(literal_rows[n:n+200], indent=2) + "\n", encoding="utf-8")
    literal_parts.append(path)
manifest["staticDataParts"] = literal_parts
manifest["enums"] = enum_manifest
manifest["originalDllSha256"] = hashlib.sha256(pe).hexdigest()
(OUT / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
print("Resolved %d static fields and %d enum maps from original PE" %
      (len(literal_rows), len(enum_manifest)))
