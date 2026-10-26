"""Generate a bounded, file-free backend from a pinned swf2apt checkout.

Upstream-derived generated source is GPL-3.0-or-later; see ../COPYING.
No downloads and no fuzzy replacements: drift fails before generating source.
"""
import argparse
import hashlib
import re
from pathlib import Path

UPSTREAM_COMMIT = '60b15a67a4cbd931f181af8c6c758bd7a8441ebc'
CONVERTER_SHA256 = '210987e8e621fe2e1f1133781b4b65da51c54e69378585f568012409a5a6e86e'


def generate(source: str) -> str:
    source = source.lstrip('\ufeff').replace('\r\n', '\n')
    if hashlib.sha256(source.encode()).hexdigest() != CONVERTER_SHA256:
        raise ValueError('Pinned AptConverter.cs changed; inspect before updating the hash')
    source = source.replace('class AptConverter', 'class MemoryAptConverter', 1)
    start = source.index('    enum ConstPointerType')
    end = source.index('    class MemoryAptConverter', start)
    source = source[:start] + source[end:]
    start = source.index('        public void ConvertToApt(')
    parse = source.index('            using (var source = new MemoryStream(swfBytes))', start)
    # Remove the filename/hash cache, optimizer, and destructive output-directory setup.
    source = source[:start] + '''        public MemoryAssets Convert(byte[] swfBytes, string logicalName)
        {
            ScriptOnlyGuard.Validate(swfBytes, logicalName);
            string basename = logicalName;
            byte[] aptBytes;
            byte[] constBytes;
            GlobalData = new Globals();
            GlobalData.writer = new ActionWriter(GlobalData);
''' + source[parse:]

    def once(old, new):
        nonlocal source
        if source.count(old) != 1:
            raise ValueError('Replacement does not match exactly once: ' + old)
        source = source.replace(old, new, 1)

    once('using (FileStream outfile = new FileStream(basename + ".apt", FileMode.Create))',
         'using (var outfile = new MemoryStream())')
    once('outfile.Write(GlobalData.output.GetBuffer());',
         'aptBytes = GlobalData.output.GetBuffer();')
    once('using (FileStream outfile2 = new FileStream(basename + ".const", FileMode.Create))',
         'using (var outfile2 = new MemoryStream())')
    once('outfile2.Write(GlobalData.output.GetBuffer());',
         'constBytes = GlobalData.output.GetBuffer();')
    start = source.index('                PackTextures(basename);')
    # No visual tags are accepted, so there is no geometry or texture output.
    # Preserve the baseline DAT text bytes (Windows CRLF, UTF-8 without BOM).
    source = source[:start] + '''                return new MemoryAssets(logicalName, aptBytes, constBytes,
                    System.Text.Encoding.UTF8.GetBytes("; Created by AptToXML.\\r\\n; Created by AptToXML.\\r\\n"));
            }
        }
    }
}
'''
    # Remove unreachable file/process helpers from the generated backend as well.
    start = source.index('        public ushort Find_POT(')
    end = source.index('        void FindButtonHitTest()', start)
    source = source[:start] + source[end:]
    start = source.index('        static int RunFlasm(')
    end = source.index('        public MemoryAssets Convert(', start)
    source = source[:start] + source[end:]
    for forbidden in ('File.', 'FileStream', 'Directory.', 'Process.', 'HashReader.', 'RunFlasm('):
        if re.search(r'\b' + re.escape(forbidden), source):
            raise ValueError('Unexpected filesystem/process call in memory backend: ' + forbidden)
    return source


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('upstream', type=Path)
    parser.add_argument('output', type=Path)
    args = parser.parse_args()
    result = generate((args.upstream / 'eaf2apt/AptConverter.cs').read_text(encoding='utf-8-sig'))
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(result, encoding='utf-8', newline='\n')
