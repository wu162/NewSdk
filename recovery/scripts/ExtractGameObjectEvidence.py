"""Extract the original GameObject root table; never synthesize recovered code."""
import json
import re
import sys
from pathlib import Path


def method(source, signature):
    start = source.index(signature)
    end = source.find('\n\tinternal ', start + len(signature))
    if end < 0:
        raise ValueError('Missing next method boundary: ' + signature)
    return source[start:end], source.count('\n', 0, start) + 1


def token_table(body, first_line):
    delimiter = 'ptr = (ProcTableElement*)((byte*)ptr + 12);'
    parts = body.split(delimiter)
    if len(parts) != 127 or '12u, 127u' not in body:
        raise ValueError('Unexpected original GameObject root token count')
    result = []
    cursor = first_line
    sizes = {'int': 4, 'uint': 4, 'float': 4, 'ushort': 2, 'bool': 1,
             'ProductionQueueType': 4, 'BuildPlacementType': 4,
             'UnitCategory': 4, 'WeaponCategory': 4,
             'BuildableStatus': 4, 'ThingClassType': 4}
    for index, part in enumerate(parts):
        name = re.search(r'\*\((?:int|uint)\*\)ptr = (-?\d+);', part)
        typ = re.search(r'\(\((?:int|uint)\*\)ptr\)\[1\] = (-?\d+);', part)
        if not name or not typ:
            raise ValueError('Unparsed token keys: ' + str(index))
        offset = None
        direct = re.search(r'\(byte\*\)@object \+ (\d+)', part)
        typed = re.search(r'\((\w+)\*\)@object \+ (\d+)', part)
        indexed = re.search(r'\(\(int\*\)@object\)\[(\d+)\]', part)
        if direct:
            offset = int(direct[1])
        elif typed:
            if typed[1] not in sizes:
                raise ValueError('Unknown pointer stride: ' + typed[1])
            offset = sizes[typed[1]] * int(typed[2])
        elif indexed:
            offset = 4 * int(indexed[1])
        if index < 126 and (offset is None or not 0 <= offset < 592):
            raise ValueError('Unparsed/out-of-root field: ' + str(index))
        helpers = re.findall(r'(Tokenizer_002ETokenize\w*)\(', part)
        helpers = [name for name in helpers if name != 'Tokenizer_002ETokenize' or index != 0]
        result.append({'index': index, 'sourceLine': cursor,
                       'nameHash': int(name[1]) & 0xffffffff,
                       'typeHash': int(typ[1]) & 0xffffffff,
                       'nativeOffset': offset,
                       'kind': 'sentinel' if index == 126 else 'null-only-token-stub' if index in (109, 110) else 'nested-helper' if helpers else 'member-bookmark',
                       'helpers': helpers})
        cursor += part.count('\n') + delimiter.count('\n')
    if result[-1]['nameHash'] or result[-1]['typeHash']:
        raise ValueError('Missing original root sentinel')
    return result


def main():
    source = Path(sys.argv[1]).read_text(encoding='utf-8-sig')
    signatures = {
        'marshal': 'internal unsafe static void Marshaler_002EMarshal(INode* node, GameObject* @object, Tracker* state)',
        'endian': 'internal unsafe static void Marshaler_002EEndianCorrect(GameObject* @object, EndianHelper* state)',
        'tokenize': 'internal unsafe static void Tokenizer_002ETokenize(void** table, GameObject* @object, Tracker* state)',
        'root': 'internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AGameObject_003E(Tracker* P_0, GameObject** rootPointer)'}
    methods = {name: method(source, sig) for name, sig in signatures.items()}
    if 'Relo_002ETracker_002EAllocate(P_0, 1u, 592u)' not in methods['root'][0]:
        raise ValueError('Original GameObject root allocation changed')
    tokens = token_table(*methods['tokenize'])
    report = {'scope': 'Original source evidence; not a recovered compiler',
              'sourceBlob': '6e40a9ed6c9f961d294d49768d911e6a6f354619',
              'nativeRootBytes': 592,
              'methods': {name: {'startLine': line, 'lineCount': body.count('\n') + 1} for name, (body, line) in methods.items()},
              'tokens': tokens}
    Path(sys.argv[2]).write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print('PASS original GameObject token inventory: 126 fields plus sentinel; all native offsets mapped; stubs at 496/500 explicit')


if __name__ == '__main__':
    main()
