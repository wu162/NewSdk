import argparse
import hashlib
import json
import shutil
import struct
import zipfile
from pathlib import Path


def sha(data):
    return hashlib.sha256(data).hexdigest()


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def require(condition, message):
    if not condition:
        raise ValueError(message)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--artifact", type=Path, required=True)
    parser.add_argument("--assets", type=Path, required=True)
    parser.add_argument("--commit", required=True)
    parser.add_argument("--run", required=True)
    parser.add_argument("--tag", required=True)
    parser.add_argument("--binary-build-run", required=True)
    parser.add_argument("--replay-run", required=True)
    parser.add_argument("--xml-run", required=True)
    args = parser.parse_args()
    root = args.artifact
    assets = args.assets
    assets.mkdir(exist_ok=True, parents=True)
    release_root = root / "artifacts" / "final-release" / (args.run + "-1")
    release = read_json(release_root / "release.json")
    require(release["succeeded"] is True, "Package acceptance did not succeed")
    require(release["sourceCommit"] == args.commit, "Packaging commit differs")
    require(release["binarySourceCommit"] == args.commit, "Binary commit differs")
    require(str(release["runId"]) == args.run, "Package run differs")
    require(release["bigDeployment"]["succeeded"] is True, "Independent packaged BIG gate failed")
    cli = read_json(root / "evidence" / "refactor" / "core-cli" / (args.binary_build_run + "-1") / "validation.json")
    require(cli["succeeded"] is True and cli["sourceCommit"] == args.commit, "CLI acceptance differs")
    evidence = root / "evidence" / "refactor"
    core = read_json(evidence / "core-lifecycle" / (args.xml_run + "-1") / "validation.json")
    require(core["succeeded"] is True and len(core["modes"]) == 2, "Corrected XML owner/worker matrix failed")
    require(all(mode["passed"] for mode in core["modes"]), "A corrected XML matrix configuration failed")
    prior_core = read_json(evidence / "core-lifecycle" / (args.replay_run + "-1") / "validation.json")
    for mode in prior_core["modes"]:
        other_fixtures = [item for item in mode["fixtures"] if item["name"] != "CompilerXmlBackendBatch"]
        require(len(other_fixtures) == 11 and all(item["passed"] for item in other_fixtures), "Other Core fixtures lack independent successful evidence")
    require(read_json(evidence / "performance" / (args.replay_run + "-1") / "validation.json")["status"] == "passed", "Profiling acceptance failed")
    require(read_json(evidence / "backend-parallel" / (args.replay_run + "-1") / "validation.json")["succeeded"] is True, "Parallel/cache acceptance failed")
    sdk = read_json(evidence / "sdk-mymod" / (args.run + "-1") / "validation.json")
    require(sdk["succeeded"] is True and sdk["inputsUnchanged"] is True and len(sdk["modes"]) == 2, "Corrected full SDK differential or input audit failed")
    for mode in sdk["modes"]:
        big_values = mode["big"] if isinstance(mode["big"], list) else [mode["big"]]
        big_receipts = [value for value in big_values if isinstance(value, dict) and value.get("succeeded") is True]
        require(mode["succeeded"] is True and len(big_receipts) == 1 and big_receipts[0]["comparison"]["succeeded"] is True, "A full SDK configuration failed")
    upheaval = read_json(evidence / "backend-parallel-performance" / (args.run + "-1") / "measurement.json")
    require(upheaval["status"] == "passed" and len(upheaval["samples"]) == 22, "Full Upheaval matrix incomplete")
    require(all(item["status"] == "passed" and item["rawOutputIdentical"] for item in upheaval["samples"]), "Upheaval sample failed")
    reuse = read_json(evidence / "release-final" / (args.run + "-1") / "reuse.json")
    allowed = {"NewModCompiler.exe", "NewModCompiler.exe.config", "antlr.runtime.dll"}
    summaries = []
    for configuration in ("Debug", "Release"):
        stem = "NewModCompiler-Win32-" + configuration
        package_path = release_root / (stem + ".zip")
        package = package_path.read_bytes()
        manifest_path = release_root / (stem + ".manifest.json")
        manifest = read_json(manifest_path)
        build_root = root / "artifacts" / "core-lifecycle-product" / configuration
        build = read_json(build_root / "build.json")
        require(manifest["binarySourceCommit"] == args.commit, "Manifest binary source differs")
        require(manifest["packagingSourceCommit"] == args.commit, "Manifest packaging source differs")
        require(str(manifest["runId"]) == args.run, "Manifest run differs")
        require(manifest["archive"]["bytes"] == len(package), "Package length differs")
        require(manifest["archive"]["sha256"] == sha(package), "Package hash differs")
        require(build["sourceCommit"] == args.commit, "Build source differs")
        require(build["containsProbeOrTestSources"] is False, "Build contains probe/test sources")
        require(set(build["runtimeFiles"]) == allowed, "Build runtime inventory differs")
        require(len(manifest["files"]) == 3 and {f["name"] for f in manifest["files"]} == allowed, "Manifest inventory differs")
        with zipfile.ZipFile(package_path) as archive:
            require(len(archive.infolist()) == 3 and set(archive.namelist()) == allowed, "ZIP must contain exactly the three runtime files")
            require(archive.testzip() is None, "ZIP CRC failed")
            for item in manifest["files"]:
                data = archive.read(item["name"])
                require(len(data) == item["bytes"] and sha(data) == item["sha256"], "ZIP member differs from manifest")
                require(data == (build_root / "bin" / item["name"]).read_bytes(), "ZIP member differs from production build")
                if item["name"] == "NewModCompiler.exe":
                    require(sha(data) == build["exeSha256"], "EXE differs from production receipt")
                    require(data[:2] == b"MZ", "EXE lacks DOS header")
                    offset = struct.unpack_from("<I", data, 0x3C)[0]
                    require(data[offset:offset + 4] == b"PE\0\0" and struct.unpack_from("<H", data, offset + 4)[0] == 0x14C, "EXE is not Win32/x86")
        shutil.copyfile(package_path, assets / package_path.name)
        shutil.copyfile(manifest_path, assets / manifest_path.name)
        summaries.append({"configuration": configuration, "archive": manifest["archive"], "files": manifest["files"], "toolchain": manifest["toolchain"]})
    metadata = {
        "schemaVersion": 1,
        "repository": "wu162/NewSdk",
        "sourceBranch": "recovery/legacy-sdk-cache",
        "binarySourceCommit": args.commit,
        "releaseTag": args.tag,
        "publicationStatus": "pending-user-confirmation; no release or release tag created",
        "releaseTagPolicy": "On future approved publication, use an independent documentation/metadata snapshot so generated GitHub archives contain no compiler source. The actual production source is binarySourceCommit.",
        "runId": args.run,
        "binaryBuildRunId": args.binary_build_run,
        "independentReplayRunId": args.replay_run,
        "xmlMatrixRunId": args.xml_run,
        "testSourceCommit": reuse["testSourceCommit"],
        "acceptanceProvenance": "Successful original production build and independent groups are reused; the original aggregate failures are retained. Corrected harness groups, exact owner/worker matrix, ZIP deployment and 22 full Upheaval samples establish the final accepted scope.",
        "buildUrl": "https://github.com/wu162/NewSdk/actions/runs/" + args.run,
        "checks": {"productionCli": True, "packagedDoctor": True, "independentZipBigDeployment": True, "archiveCrc": True, "archiveAndFileSha256": True, "matchesProductionBinaries": True, "exactThreeRuntimeFiles": True, "sourceFilesIncluded": False},
        "packages": summaries,
    }
    (assets / "release-metadata.json").write_text(json.dumps(metadata, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(metadata, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
