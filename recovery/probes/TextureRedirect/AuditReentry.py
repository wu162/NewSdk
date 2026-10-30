"""Record the pinned DLL's actual shared native state and CIL use sites.

Requires dnfile==0.18.0 and dncil==1.0.2. This is static evidence, not a
thread-safety certificate for native compression or Direct3D internals.
"""
import argparse
import hashlib
import json
from pathlib import Path

import dnfile
from dncil.cil.body import CilMethodBody
from dncil.cil.body.reader import CilMethodBodyReaderBytes

PIN = '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'


def audit(path):
    digest = hashlib.sha256(path.read_bytes()).hexdigest()
    if digest != PIN:
        raise ValueError('Expected the original audited TextureCompiler DLL')
    pe = dnfile.dnPE(str(path))
    fields = {index: row for index, row in enumerate(pe.net.mdtables.Field, 1)
              if str(row.Name).startswith('BinaryAssetBuilder.TextureCompiler.')
              and any(str(row.Name).endswith('.' + name) for name in
                      ('g_Buffer', 'g_Bundler', 'g_pD3D', 'g_pD3DDevice'))}
    field_rvas = {row.Field.row_index: row.Rva for row in pe.net.mdtables.FieldRva}
    field_attributes = {index: [] for index in fields}
    for attribute in pe.net.mdtables.CustomAttribute:
        if attribute.Parent.table.name == 'Field' and attribute.Parent.row_index in fields:
            constructor = attribute.Type.row
            owner = getattr(constructor, 'Class', None)
            field_attributes[attribute.Parent.row_index].append(str(owner.row.TypeName) if owner else str(constructor.Name))
    sites, methods = [], []

    def operand(value):
        if hasattr(value, 'table'):
            table = pe.net.mdtables.tables.get(value.table)
            if table and 0 < value.rid <= len(table.rows):
                row = table.rows[value.rid - 1]
                return {'token': hex(value.value), 'name': str(getattr(row, 'Name', ''))}
        return str(value) if value is not None else None

    for index, method in enumerate(pe.net.mdtables.MethodDef, 1):
        if not method.Rva or method.ImplFlags.miNative:
            continue
        body = CilMethodBody(CilMethodBodyReaderBytes(pe.get_data(method.Rva, 65536)))
        for instruction in body.instructions:
            value = instruction.operand
            if hasattr(value, 'table') and value.table == 4 and value.rid in fields:
                sites.append({'methodToken': hex(0x06000000 + index), 'method': str(method.Name),
                              'methodRva': hex(method.Rva), 'offset': hex(instruction.offset),
                              'opcode': instruction.opcode.name, 'field': operand(value)})
        if str(method.Name) in ('Initialize', 'CompressAndEmbed', 'CompressAndEmbed3D',
                                'BinaryAssetBuilder.TextureCompiler.nvDXTWriteCallback'):
            methods.append({'token': hex(0x06000000 + index), 'name': str(method.Name),
                            'rva': hex(method.Rva), 'instructions': [
                                {'offset': hex(i.offset), 'opcode': i.opcode.name,
                                 'operand': operand(i.operand)} for i in body.instructions]})
    return {'originalSha256': digest,
            'globals': [{'token': hex(0x04000000 + index), 'name': str(row.Name),
                         'static': row.Flags.fdStatic,
                         'threadStatic': 'ThreadStaticAttribute' in field_attributes[index],
                         'customAttributes': field_attributes[index],
                         'nativeStorageRva': hex(field_rvas[index])} for index, row in fields.items()],
            'useSites': sites, 'methods': methods,
            'finding': '2D and 3D compression reset, populate via callback user data, and embed the same process-global SimpleBuffer. Multiple Plugin instances share this native storage.',
            'scope': 'Pinned managed CIL and native-storage metadata only; not an exhaustive native-code audit.'}


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('dll', type=Path)
    parser.add_argument('output', type=Path)
    args = parser.parse_args()
    args.output.write_text(json.dumps(audit(args.dll), indent=2) + '\n', encoding='utf-8')
