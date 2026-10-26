"""Check the historical W3X dependency closure without invalidating unrelated XML work."""
import argparse
import json
import posixpath
import re
import subprocess


ROOTS = (
    "recovery/src/Native/W3x/", "recovery/src/Bridge/W3x/",
    "recovery/src/Managed/W3x/", "recovery/src/Plugin/W3x/",
)
SHARED = tuple("recovery/src/Native/" + name + ".cpp" for name in (
    "Relocation", "ScalarParsing", "ResourceMarshaling"))
FIXTURES = (
    "recovery/tests/W3xManagedOracle.cs", "recovery/tests/W3xManagedStandalone.cs",
    "recovery/scripts/BuildW3xPlugin.ps1", "recovery/scripts/TestW3xNative.ps1",
    "recovery/scripts/TestW3xPlugin.ps1", "recovery/scripts/TestW3xManaged.ps1",
)
LOCAL_INCLUDE = re.compile(r'^\s*#\s*include\s*"([^"\r\n]+)"', re.MULTILINE)


def git(*args):
    return subprocess.check_output(["git", *args])


def closure(ref):
    files = set(git("ls-tree", "-r", "--name-only", "-z", ref).decode().split("\0"))
    pending = [path for path in files if path.startswith(ROOTS) or
               (path.startswith("recovery/tests/W3x") and path.endswith(".cpp"))]
    pending.extend(SHARED + FIXTURES)
    seen = set()
    while pending:
        path = pending.pop()
        if path in seen:
            continue
        if path not in files:
            raise ValueError(f"Missing W3X dependency at {ref}: {path}")
        seen.add(path)
        source = git("show", f"{ref}:{path}").decode("utf-8-sig")
        for include in LOCAL_INCLUDE.findall(source):
            dependency = posixpath.normpath(posixpath.join(posixpath.dirname(path), include))
            if dependency.startswith("../") or dependency.startswith("/"):
                raise ValueError(f"W3X include escapes repository: {path}: {include}")
            pending.append(dependency)
    return seen


def check(baseline, current):
    # Union both closures: removed or retargeted includes must remain observable.
    inputs = sorted(closure(baseline) | closure(current))
    changed = git("diff", "--name-only", "-z", baseline, current, "--", *inputs)
    changed = sorted(filter(None, changed.decode().split("\0")))
    return {"baselineCommit": baseline, "currentCommit": current,
            "inputs": inputs, "changedInputs": changed, "unchanged": not changed}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("baseline")
    parser.add_argument("current")
    args = parser.parse_args()
    result = check(args.baseline, args.current)
    print(json.dumps(result, indent=2))
    return 0 if result["unchanged"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
