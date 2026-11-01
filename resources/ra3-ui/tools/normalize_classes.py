"""Recover FFDec AS2 classes from initclip scripts without duplicate declarations."""
from pathlib import Path
import hashlib
import re


CLIP = re.compile(r'(?m)^[ \t]*#initclip[^\r\n]*\r?\n([\s\S]*?)^[ \t]*#endinitclip[^\r\n]*(?:\r?\n|$)')
DECL = re.compile(r'\b(class|interface)\s+([\w.$]+)')


def mask(code):
    return re.sub(r'"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|//[^\r\n]*|/\*[\s\S]*?\*/',
                  lambda m: ''.join('\n' if c == '\n' else ' ' for c in m[0]), code)


def normalize(root, packages=None, skip_packages=None):
    """Use default and official skip-field-detection exports of the SAME source.

    FFDec's skipDetectionOfUninitializedClassFields=true output preserves runtime
    assignments and qualifies undeclared static/instance accesses explicitly. It
    avoids inventing derived fields from inherited member reads. Preserve these
    vendor-generated bodies wholesale; do not hand-delete source declarations.
    Inputs are __Packages folders from local FFDec -export script invocations.
    The source SWF and its original initclip code stay in the source inventory.
    """
    root = Path(root)
    classes = {}
    changes = []
    for source in sorted(root.rglob('*.as')):
        if 'classes' in source.relative_to(root).parts:
            continue
        text = source.read_text(encoding='utf-8-sig')
        def replace(match):
            code = match[1].strip()
            decl = DECL.search(mask(code))
            if not decl:
                return match.group(0)
            name = decl[2]
            # FFDec's initclip block contains exactly the recovered declaration.
            if decl.start() != 0:
                raise ValueError('Unexpected pre-declaration code: ' + str(source))
            if name in classes and classes[name] != code:
                raise ValueError('Conflicting definitions of class ' + name)
            classes[name] = code
            changes.append({'source': source.relative_to(root).as_posix(), 'class': name,
                            'sourceDeclarationSha256': hashlib.sha256(code.encode()).hexdigest()})
            return 'import ' + name + ';\n'
        updated = CLIP.sub(replace, text)
        if updated != text:
            source.write_text(updated, encoding='utf-8')
    if packages:
        for source in sorted(Path(packages).rglob('*.as')):
            code = source.read_text(encoding='utf-8-sig').strip()
            decl = DECL.search(mask(code))
            if not decl or decl.start() != 0:
                raise ValueError('Expected pure recovered class in ' + str(source))
            name = decl[2]
            if name in classes and classes[name] != code:
                raise ValueError('Conflicting package class ' + name)
            classes[name] = code
    vendor_normalizations = []
    if skip_packages:
        vendor_classes = {}
        for source in sorted(Path(skip_packages).rglob('*.as')):
            skipped = source.read_text(encoding='utf-8-sig').strip()
            declaration = DECL.search(mask(skipped))
            if not declaration:
                raise ValueError('Vendor output has no class/interface: ' + str(source))
            name = declaration[2]
            vendor_classes[name] = skipped
        missing = set(classes) - set(vendor_classes)
        if missing:
            raise ValueError('Vendor normalization missing classes: ' + ', '.join(sorted(missing)))
        for name, code in list(classes.items()):
            skipped = vendor_classes[name]
            vendor_normalizations.append({'class': name,
                'defaultCodeSha256': hashlib.sha256(code.encode()).hexdigest(),
                'vendorSkipFieldsSha256': hashlib.sha256(skipped.encode()).hexdigest()})
            classes[name] = skipped
    class_files = []
    for name, code in sorted(classes.items()):
        # Flash's default AS2 class path is the FLA directory; preserve qualified
        # names as package directories without an extra artificial classes root.
        target = root / Path(*name.split('.')).with_suffix('.as')
        target.parent.mkdir(parents=True, exist_ok=True)
        # A typeof result is necessarily a string, so old string-equality eq and
        # normal equality agree for these exact decompiled type checks.
        if name == 'AptZombieClass':
            assert '_loc1_ = typeof obj[_loc3_];' in code
            code = code.replace('_loc1_ eq "movieclip"', '_loc1_ == "movieclip"').replace('_loc1_ eq "function"', '_loc1_ == "function"')
        target.write_text(code + '\n', encoding='utf-8')
        class_files.append({'class': name, 'path': target.relative_to(root).as_posix(),
                            'sha256': hashlib.sha256(target.read_bytes()).hexdigest()})
    return {'classCount': len(classes), 'timelineDeclarationsExternalized': len(changes), 'changes': changes,
            'vendorSkipUninitializedFieldDetection': vendor_normalizations, 'classFiles': class_files}


if __name__ == '__main__':
    import argparse, json
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('root', type=Path)
    p.add_argument('--packages', type=Path)
    p.add_argument('--skip-packages', type=Path)
    a = p.parse_args()
    print(json.dumps(normalize(a.root, a.packages, a.skip_packages), indent=2))
