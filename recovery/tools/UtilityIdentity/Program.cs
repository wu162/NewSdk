using Mono.Cecil;
// C++ names and custom signature modifiers are valid CLI metadata which C#
// cannot express. Read only original metadata; never reuse original method bodies.
using var module = ModuleDefinition.ReadModule(args[0], new ReaderParameters { InMemory = true });
using var original = ModuleDefinition.ReadModule(args[1]);
foreach (var type in module.Types) {
    if (type.Name.StartsWith("StructWrapper_003CAssetStream_003A_003A")) {
        string name = type.Name.Replace("StructWrapper_003CAssetStream_003A_003A", "").Replace("_003E", "");
        type.Name = original.Types.Single(t => t.Name == "StructWrapper<AssetStream::" + name + ">").Name;
    }
}
string Plain(TypeReference t) {
    if(t is OptionalModifierType om) return Plain(om.ElementType);
    if(t is RequiredModifierType rm) return Plain(rm.ElementType);
    if(t is PointerType p) return Plain(p.ElementType)+"*";
    if(t is ByReferenceType r) return Plain(r.ElementType)+"&";
    if(t is ArrayType a) return Plain(a.ElementType)+"["+new string(',',a.Rank-1)+"]";
    return t.FullName;
}
TypeReference Modifiers(TypeReference source, TypeReference target) {
    while(target is OptionalModifierType || target is RequiredModifierType) target=((TypeSpecification)target).ElementType;
    if(source is OptionalModifierType om) return new OptionalModifierType(module.ImportReference(om.ModifierType),Modifiers(om.ElementType,target));
    if(source is RequiredModifierType rm) return new RequiredModifierType(module.ImportReference(rm.ModifierType),Modifiers(rm.ElementType,target));
    if(source is PointerType p && target is PointerType tp) return new PointerType(Modifiers(p.ElementType,tp.ElementType));
    if(source is ByReferenceType r && target is ByReferenceType tr) return new ByReferenceType(Modifiers(r.ElementType,tr.ElementType));
    if(source is ArrayType a && target is ArrayType ta) return new ArrayType(Modifiers(a.ElementType,ta.ElementType),a.Rank);
    return target;
}
foreach (var type in module.Types) {
    var match = original.Types.FirstOrDefault(t => t.FullName == type.FullName);
    if (match == null) continue;
    type.Attributes = match.Attributes;
    foreach (var method in type.Methods.Where(m => m.IsPublic || m.IsFamily)) {
        var source = match.Methods.SingleOrDefault(m => m.Name == method.Name && m.Parameters.Count == method.Parameters.Count
            && Plain(m.ReturnType)==Plain(method.ReturnType)
            && m.Parameters.Select(p=>Plain(p.ParameterType)).SequenceEqual(method.Parameters.Select(p=>Plain(p.ParameterType))));
        if(source==null) throw new Exception("No original contract for "+method.FullName);
        method.Attributes=source.Attributes;
        method.CallingConvention=source.CallingConvention;
        method.ReturnType=Modifiers(source.ReturnType,method.ReturnType);
        for(int i=0;i<method.Parameters.Count;i++) method.Parameters[i].ParameterType=Modifiers(source.Parameters[i].ParameterType,method.Parameters[i].ParameterType);
    }
}
module.Write(args[0]);
Console.WriteLine("Restored C++/CLI names, type/method flags and custom signature modifiers");
