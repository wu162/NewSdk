# GameObject public ImportsData return contract

Original source: recovery/decompiled/BinaryAssetBuilder.Tokenizer/BinaryAssetBuilder.Tokenizer/Plugin.cs, blob 641761fc1420ce2ab6a9a75aa443dd35c970bda8. HandleType<GameObject> starts at original line549; line716 unconditionally assigns ImportsData = new byte[0] after copying InstanceData/RelocationData. ProcessInstance GameObject case calls this handler.

The intermediate Tracker import offset/sentinel table is not the public GameObject ImportsData. Enclose differential Check already compared all three original/recovered buffers successfully before the independent assertion failed. Correct independent assertion: public ImportsData.Length == 0, retaining strong FX DWORD24, String length4, pointer at+140, radius25, total tail at+148 and typed ModuleID/schema checks. No recovery runtime change and no comparison/refusal was removed.
