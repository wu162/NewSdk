#!/usr/bin/env python3
"""Fetch fixed, verified library releases, compile, and optionally package the tool."""
import argparse
import hashlib
import io
import os
from pathlib import Path
import shutil
import subprocess
import urllib.request
import zipfile

ROOT = Path(__file__).resolve().parent
DEPENDENCIES = (
    ("fla", "https://github.com/jindrapetrik/jpexs-fla-convert/releases/download/v3.0.0/fla_convert_3.0.0.zip",
     "c875934f4e5e0d826e5e38e10d4bdb0f908970a9e12af97cd7d7a1f91e248cea"),
    ("ffdec", "https://github.com/jindrapetrik/jpexs-decompiler/releases/download/version26.4.0/ffdec_lib_26.4.0.zip",
     "bd4e7d4ccd05d7f7acfc54bdbed79bdec149cd5880e2c08b34f8e7e01be45302"),
)


def fetch():
    lib = ROOT / "lib"
    lib.mkdir(exist_ok=True)
    for name, url, checksum in DEPENDENCIES:
        marker = lib / (name + ".sha256")
        if marker.exists() and marker.read_text().strip() == checksum:
            continue
        print("Downloading", url.rsplit("/", 1)[-1], flush=True)
        request = urllib.request.Request(url, headers={"User-Agent": "NewSdk-Fla2Swf/1.0"})
        with urllib.request.urlopen(request, timeout=120) as response:
            data = response.read()
        if hashlib.sha256(data).hexdigest() != checksum:
            raise RuntimeError("Dependency checksum mismatch: " + name)
        with zipfile.ZipFile(io.BytesIO(data)) as archive:
            for entry in archive.infolist():
                # fla_convert.jar replaces the older flacomdoc bundled with FFDec.
                if entry.is_dir() or entry.filename == "flacomdoc.jar":
                    continue
                path = Path(entry.filename)
                if len(path.parts) != 1 or path.name != entry.filename:
                    raise RuntimeError("Unexpected dependency archive path: " + entry.filename)
                destination = lib / path.name if path.suffix == ".jar" else lib / (name + "-" + path.name)
                destination.write_bytes(archive.read(entry))
        marker.write_text(checksum + "\n")


def build(tests=False):
    classes = ROOT / "build" / "classes"
    if classes.exists():
        shutil.rmtree(classes)
    classes.mkdir(parents=True)
    classpath = os.pathsep.join(str(p) for p in sorted((ROOT / "lib").glob("*.jar")))
    if not (ROOT / "lib" / "fla_convert.jar").exists() or not (ROOT / "lib" / "ffdec_lib.jar").exists():
        raise RuntimeError("Missing libraries; run setup.py without --offline first")
    javac = shutil.which("javac")
    command = [javac] if javac else ["java", "com.sun.tools.javac.Main"]
    sources = sorted((ROOT / "src").rglob("*.java"))
    if tests:
        sources += sorted((ROOT / "tests").rglob("*.java"))
    # Java 8 class files also run on current JREs. Third-party libraries are separate.
    subprocess.run(command + ["-encoding", "UTF-8", "-source", "8", "-target", "8",
                             "-cp", classpath, "-d", str(classes)] + [str(p) for p in sources], check=True)
    with zipfile.ZipFile(ROOT / "fla2swf.jar", "w", zipfile.ZIP_DEFLATED) as jar:
        libraries = " ".join("lib/" + p.name for p in sorted((ROOT / "lib").glob("*.jar")))
        line = "Class-Path: " + libraries
        folded = line[:70] + "\n"
        line = line[70:]
        while line:
            folded += " " + line[:69] + "\n"
            line = line[69:]
        jar.writestr("META-INF/MANIFEST.MF", "Manifest-Version: 1.0\nMain-Class: newsdk.fla.Main\n" + folded + "\n")
        for path in sorted(classes.rglob("*.class")):
            if not path.name.startswith("CompilerTest"):
                jar.write(path, path.relative_to(classes).as_posix())
    if tests:
        test_home = ROOT / "build" / "test-home"
        test_home.mkdir(exist_ok=True)
        subprocess.run(["java", "-Djava.awt.headless=true", "-Duser.home=" + str(test_home),
                        "-cp", str(classes) + os.pathsep + classpath, "newsdk.fla.CompilerTest",
                        str(ROOT / "tests" / "fixtures"), str(ROOT / "build")], check=True)


def package(output):
    output = output.resolve()
    output.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(output, "w", zipfile.ZIP_DEFLATED) as archive:
        for path in sorted(ROOT.rglob("*")):
            relative = path.relative_to(ROOT)
            if path.is_file() and path.suffix != ".zip" and not any(p in {"build", "__pycache__", ".gitignore"} for p in relative.parts):
                if path.resolve() != output:
                    archive.write(path, "Fla2Swf/" + relative.as_posix())
    print("Packaged", output)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--offline", action="store_true", help="build with libraries already in lib/")
    parser.add_argument("--test", action="store_true", help="build and run integration tests")
    parser.add_argument("--package", type=Path, help="write a portable ZIP with source and dependency licenses")
    args = parser.parse_args()
    if not args.offline:
        fetch()
    build(args.test)
    if args.package:
        package(args.package)


if __name__ == "__main__":
    main()
