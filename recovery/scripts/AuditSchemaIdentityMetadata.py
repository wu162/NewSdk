#!/usr/bin/env python3
"""Inventory schema ID semantics before planning component or SDK verification.

This is a coverage/drift check, not proof that a serializer is byte-correct.
In particular pipelineOnly does not mean every compiler may ignore a value.
"""
import argparse
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET

XS = "{http://www.w3.org/2001/XMLSchema}"
XAS = "{uri:ea.com:eala:asset:schema}"
POLICIES = {
    "ArmorTemplateSet": "flow-identity",
    "LocomotorSet": "flow-identity",
    "CrusherInfo": "flow-identity",
    "WeaponEffectNugget": "flow-identity",
    "GameDependencyType": "flow-identity",
    "AIBuildDelayRange": "flow-identity",
    "AISpecialPowerInfo": "flow-identity",
    "BaseAssetType": "asset-identity",
    "TestModuleA": "excluded-test-schema",
    "TestModuleB": "excluded-test-schema",
    "TerrainAsset": "preserve-serializer-field",
    "MapObject": "preserve-serializer-field",
    "RoadObject": "preserve-serializer-field",
    "GameScript": "preserve-serializer-field",
    "GameScriptGroup": "preserve-serializer-field",
    "WeatherData": "preserve-serializer-field",
    "ObjectFilter": "preserve-serializer-field",
    "MetaDataObject": "preserve-serializer-field",
    "AIStateTactic": "preserve-serializer-field",
    "ModuleData": "preserve-serializer-field",
    "ParticleSysBone": "preserve-serializer-field",
    "ModelConditionState": "preserve-serializer-field",
}
NESTED_BASE_ASSET = {
    "GameStatsModeData", "MicrophoneSettings", "AIStrategicState",
    "AIBudgetState", "AIBuildState", "ObjectVisionRangeItem",
}

def inventory(schema_root):
    root = Path(schema_root)
    files = sorted(root.rglob("*.xsd"), key=lambda p: p.relative_to(root).as_posix())
    types = {}
    elements = []
    pipeline = []
    digest = hashlib.sha256()
    for file in files:
        relative = file.relative_to(root).as_posix()
        # Git checkout may translate line endings; fingerprint canonical LF.
        source = file.read_text(encoding="utf-8-sig").replace("\r\n", "\n")
        digest.update(relative.encode() + b"\0" + ET.canonicalize(source, strip_text=True).encode() + b"\0")
        document = ET.fromstring(source)
        for node in document.findall(XS + "complexType"):
            name = node.get("name")
            extension = node.find(".//" + XS + "extension")
            attributes = [dict(a.attrib) for a in node.findall(".//" + XS + "attribute")]
            types[name] = {
                "type": name, "file": relative,
                "base": extension.get("base") if extension is not None else None,
                "attributes": attributes,
            }
            for attr in attributes:
                if attr.get(XAS + "pipelineOnly") == "true":
                    pipeline.append({"type": name, "attribute": attr["name"], "file": relative})
        # CnC3Types' AssetDeclaration choice is the root registry, not a nested
        # Marshal dependency. Do not conflate that registry with inheritance.
        if relative != "CnC3Types.xsd":
            for element in document.findall(".//" + XS + "element"):
                if element not in list(document) and element.get("type"):
                    elements.append({"element": element.get("name"),
                                     "type": element.get("type"), "file": relative})
    def derives(name, base):
        visited = set()
        while name and name not in visited:
            if name == base:
                return True
            visited.add(name)
            name = types.get(name, {}).get("base")
        return False
    explicit = []
    for name, record in sorted(types.items()):
        ids = [a for a in record["attributes"] if a.get("name") == "id"]
        if ids:
            if name not in POLICIES:
                raise ValueError("Unreviewed schema ID semantics: " + name)
            explicit.append({"type": name, "file": record["file"], "base": record["base"],
                             "policy": POLICIES[name],
                             "pipelineOnly": ids[0].get(XAS + "pipelineOnly") == "true"})
    found = {r["type"] for r in explicit}
    if found != set(POLICIES):
        raise ValueError("ID policy/schema drift: " + repr(sorted(found ^ set(POLICIES))))
    nested = sorted([e for e in elements if derives(e["type"], "BaseAssetType")],
                    key=lambda e: (e["type"], e["element"], e["file"]))
    if {e["type"] for e in nested} != NESTED_BASE_ASSET:
        raise ValueError("Nested BaseAssetType inheritance changed; review metadata coverage")
    return {
        "schemaFiles": len(files), "namedComplexTypes": len(types),
        "schemaFingerprintXml": digest.hexdigest(),
        "scope": "ID declaration/inheritance coverage; not runtime acceptance or a global ignore list",
        "explicitIdPolicies": explicit,
        "nestedAssetIdentityUses": nested,
        "allPipelineOnlyDeclarations": sorted(pipeline, key=lambda e: (e["type"], e["attribute"])),
        "rules": [
            "Keep unqualified ID handling type-specific; qualified foreign attributes stay rejected.",
            "Check XSD inheritance separately from global compiler/root registration.",
            "A pipeline-only value may affect compilation even if it is not a runtime struct member.",
            "Never strip ObjectFilter/ModuleData/AIStateTactic/model-state IDs globally.",
            "Positive integration fixtures must include required nested identity attributes.",
            "Run changed metadata paths against original DLL first; then one SDK batch if needed.",
        ],
    }

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--schema-root", default="SDK/Schemas/xsd")
    group = parser.add_mutually_exclusive_group(required=True)
    group.add_argument("--output")
    group.add_argument("--check")
    parser.add_argument("--report", help="Save actual inventory for diagnostics, even when the check fails")
    args = parser.parse_args()
    result = inventory(args.schema_root)
    if args.report:
        report = Path(args.report)
        report.parent.mkdir(parents=True, exist_ok=True)
        report.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if args.output:
        path = Path(args.output)
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    else:
        expected = json.loads(Path(args.check).read_text(encoding="utf-8"))
        if result != expected:
            raise SystemExit("Schema metadata inventory changed in: " + ", ".join(k for k in sorted(set(result) | set(expected)) if result.get(k) != expected.get(k)))
    print("PASS schema metadata coverage: files=%d explicitIdTypes=%d nestedIdentityTypes=%d" %
          (result["schemaFiles"], len(result["explicitIdPolicies"]), len(NESTED_BASE_ASSET)))

if __name__ == "__main__":
    main()
