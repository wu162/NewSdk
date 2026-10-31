#!/bin/sh
set -eu
tool_dir=$(CDPATH='' cd -- "$(dirname -- "$0")" && pwd)
exec java -Djava.awt.headless=true -cp "$tool_dir/fla2swf.jar:$tool_dir/lib/*" newsdk.fla.Main "$@"
