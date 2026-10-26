#!/usr/bin/env python3
"""Validate actual App Debug/Release JSONL and reports against protocol 1.0.

Install: python -m pip install -r recovery/scripts/app-protocol-requirements.txt
Example: python recovery/scripts/ValidateAppProtocol.py <extracted-artifact-root>
This validates host contracts only, not real asset compiler output.
"""
import argparse
import hashlib
import importlib.metadata
import json
from pathlib import Path
import platform
import sys

from jsonschema import Draft202012Validator, FormatChecker


def sha256(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("root", type=Path, help="Directory containing Debug/ and Release/ actual test artifacts")
    parser.add_argument("--protocol", type=Path, default=Path(__file__).resolve().parents[1] / "protocol")
    parser.add_argument("--output", type=Path, help="Optional structured validation receipt")
    parser.add_argument("--source-commit")
    parser.add_argument("--artifact-id", type=int)
    parser.add_argument("--archive", type=Path, help="Original downloaded archive, to record its SHA-256")
    parser.add_argument("--expected-archive-sha256")
    args = parser.parse_args()
    expected_versions = {"jsonschema": "4.23.0", "rfc3339-validator": "0.1.4"}
    for package, expected in expected_versions.items():
        actual = importlib.metadata.version(package)
        if actual != expected:
            raise RuntimeError(f"{package} must be {expected}; installed {actual}")
    checker = FormatChecker()
    if "date-time" not in checker.checkers:
        raise RuntimeError("RFC3339 date-time format checker missing; install app-protocol-requirements.txt")
    validators, schemas = {}, []
    for name in ("event", "report"):
        path = args.protocol / f"build-{name}.schema.json"
        schema = json.loads(path.read_text(encoding="utf-8-sig"))
        Draft202012Validator.check_schema(schema)
        validators[name] = Draft202012Validator(schema, format_checker=checker)
        schemas.append({"path": path.name, "sha256": sha256(path)})
    archive_sha = sha256(args.archive) if args.archive else None
    if args.expected_archive_sha256 and archive_sha != args.expected_archive_sha256:
        raise RuntimeError("Downloaded archive SHA-256 does not match the expected GitHub artifact digest")
    matrix = {}
    files = []
    failures = []
    for configuration in ("Debug", "Release"):
        folder = args.root / configuration
        counts = {"configuration": configuration, "reports": 0, "eventFiles": 0, "events": 0}
        for path in sorted(folder.rglob("*")):
            if path.name not in ("build-report.json", "build.events.jsonl") or not path.is_file():
                continue
            relative = path.relative_to(args.root).as_posix()
            files.append({"path": relative, "sha256": sha256(path)})
            if path.name == "build-report.json":
                counts["reports"] += 1
                records = [(None, json.loads(path.read_text(encoding="utf-8-sig")))]
                validator = validators["report"]
            else:
                counts["eventFiles"] += 1
                raw = path.read_bytes()
                if raw and not raw.endswith(b"\n"):
                    failures.append({"path": relative, "message": "JSONL does not end in a complete flushed line"})
                records = [(line, json.loads(value)) for line, value in enumerate(raw.decode("utf-8-sig").splitlines(), 1)]
                counts["events"] += len(records)
                validator = validators["event"]
            for line, record in records:
                for error in validator.iter_errors(record):
                    failures.append({"path": relative, "line": line, "jsonPath": error.json_path, "message": error.message})
        if not all(counts[key] > 0 for key in ("reports", "eventFiles", "events")):
            failures.append({"path": configuration, "message": "Missing actual configuration outputs; empty validation is forbidden"})
        matrix[configuration] = counts
    receipt = {
        "schemaVersion": "1.0", "status": "failed" if failures else "passed",
        "scope": "Actual application host protocol outputs; no asset compilation acceptance implied",
        "sourceCommit": args.source_commit, "artifactId": args.artifact_id,
        "artifactArchiveSha256": archive_sha,
        "validator": "Draft202012Validator with RFC3339 FormatChecker enabled",
        "packages": {name: importlib.metadata.version(name) for name in ("jsonschema", "rfc3339-validator", "attrs", "jsonschema-specifications", "referencing", "rpds-py")},
        "pythonVersion": platform.python_version(), "validatorScriptSha256": sha256(Path(__file__)),
        "schemaInputs": schemas, "matrix": list(matrix.values()), "validatedFiles": files,
        "errorCount": len(failures), "errors": failures,
        "notes": ["The report-write-failure test intentionally has a terminal event and no report file; only existing outputs are schema-validated.", "Runtime assertions separately verify sequence, lifecycle, correlation, exit codes and controlled failure behavior."]
    }
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(receipt, indent=2) + "\n", encoding="utf-8")
    if failures:
        for failure in failures:
            print(json.dumps(failure), file=sys.stderr)
        return 1
    print("PASS App protocol schemas: " + "; ".join(f"{key}: {value['reports']} reports / {value['events']} events" for key, value in matrix.items()) + "; errors=0; date-time formats validated")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
