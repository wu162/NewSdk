#!/usr/bin/env python3
"""Prove exact source identity after the one approved Audio project path change.

Reads Git objects only. It neither edits old receipts nor writes Git objects.
The virtual tree must equal the accepted tree byte-for-byte; any source, file
set, mode or other project-content change remains a hard failure.
"""
import argparse
import hashlib
import json
import re
import subprocess
from pathlib import Path

PROJECT = "BinaryAssetBuilder.AudioCompiler.csproj"
NEW_HINT = b"<HintPath>../../../../SDK/tools/BinaryAssetBuilder.Core.dll</HintPath>"
OLD_HINT = b"<HintPath>../../SDK/tools/BinaryAssetBuilder.Core.dll</HintPath>"


def git(repository, *arguments):
    return subprocess.check_output(["git", "-C", str(repository), *arguments], stderr=subprocess.PIPE)


def object_hash(kind, data):
    return hashlib.sha1(kind + b" " + str(len(data)).encode("ascii") + b"\0" + data).hexdigest()


def tree_hash(entries):
    """Reconstruct the exact Git tree, including modes and directory ordering."""
    root = {}
    for mode, name, oid in entries:
        node = root
        parts = name.split(b"/")
        for part in parts[:-1]:
            node = node.setdefault(part, {})
        if parts[-1] in node:
            raise ValueError("Duplicate tree entry")
        node[parts[-1]] = (mode, oid)

    def encode(node):
        data = bytearray()
        for name, item in sorted(node.items(), key=lambda row: row[0] + (b"/" if isinstance(row[1], dict) else b"")):
            mode, oid = (b"40000", encode(item)) if isinstance(item, dict) else item
            data.extend(mode + b" " + name + b"\0" + bytes.fromhex(oid))
        return object_hash(b"tree", bytes(data))

    return encode(root)


def validate(repository, expected_tree, source_root="recovery/src/Managed/Audio"):
    if not re.fullmatch(r"[0-9a-f]{40}", expected_tree):
        raise ValueError("Expected a SHA-1 acceptance tree")
    if git(repository, "rev-parse", "--show-object-format").strip() != b"sha1":
        raise ValueError("This verifier requires the repository's original SHA-1 object format")
    current = git(repository, "rev-parse", "HEAD:" + source_root).decode("ascii").strip()
    if current == expected_tree:
        return {"currentTree": current, "acceptanceTree": expected_tree, "equivalence": "exact", "receiptRewritten": False}
    raw = git(repository, "ls-tree", "-rz", "HEAD:" + source_root)
    entries = []
    project_oid = None
    for record in raw.split(b"\0"):
        if not record:
            continue
        header, name = record.split(b"\t", 1)
        mode, kind, oid = header.split(b" ")
        if kind != b"blob":
            raise ValueError("Unexpected non-blob entry in Audio source closure")
        oid = oid.decode("ascii")
        entries.append((mode, name, oid))
        if name == PROJECT.encode("ascii"):
            if mode != b"100644":
                raise ValueError("Audio project file mode changed")
            project_oid = oid
    if tree_hash(entries) != current:
        raise ValueError("Git tree reconstruction did not match the actual source tree")
    if project_oid is None:
        raise ValueError("Audio project is missing")
    project = git(repository, "cat-file", "blob", project_oid)
    if project.count(NEW_HINT) != 1 or OLD_HINT in project:
        raise ValueError("Audio project does not contain exactly the approved migrated HintPath")
    previous_project = project.replace(NEW_HINT, OLD_HINT, 1)
    previous_oid = object_hash(b"blob", previous_project)
    virtual = tree_hash([(mode, name, previous_oid if name == PROJECT.encode("ascii") else oid) for mode, name, oid in entries])
    if virtual != expected_tree:
        raise ValueError("Audio implementation, file set, mode or other project content differs from accepted source")
    return {"currentTree": current, "acceptanceTree": virtual, "equivalence": "project-hintpath-migration", "normalizedFile": PROJECT, "currentProjectBlob": project_oid, "acceptedProjectBlob": previous_oid, "receiptRewritten": False}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--expected-tree", required=True)
    parser.add_argument("--repository", type=Path, default=Path(__file__).resolve().parents[2])
    args = parser.parse_args()
    try:
        print(json.dumps(validate(args.repository, args.expected_tree), sort_keys=True))
    except (ValueError, subprocess.CalledProcessError) as error:
        parser.exit(1, "Audio source identity rejected: " + str(error) + "\n")


if __name__ == "__main__":
    main()
