using System.Globalization;
using System.Text;

// A deliberately small, independent AVM1 evaluator for semantic compiler tests.
// It decodes the byte stream itself rather than using the production SWF reader.
internal sealed class Avm1Machine
{
    private sealed class Missing { public override string ToString() => "undefined"; }
    private sealed class ObjectValue
    {
        internal readonly Dictionary<string, object> Values = new(StringComparer.Ordinal);
        internal readonly Dictionary<string, (object Getter, object Setter)> Accessors = new(StringComparer.Ordinal);
        internal ObjectValue Parent;
    }
    private sealed class Function
    {
        internal string[] Parameters;
        internal byte[] Body;
        internal Scope Closure;
        internal ObjectValue Prototype = new();
        internal Function Base;
        internal readonly ObjectValue Properties = new();
        internal Func<object, object[], object> Native;
    }
    private sealed class Scope
    {
        internal readonly Dictionary<string, object> Locals = new(StringComparer.Ordinal);
        internal Scope Parent;
        internal object This;
        internal Function Super;
    }
    private static readonly object Undefined = new Missing();
    private readonly ObjectValue global = new();
    private readonly Scope root;
    internal readonly List<string> Traces = new();
    private int steps;
    internal Avm1Machine()
    {
        root = new Scope { This = global };
        global.Values["_global"] = global; global.Values["_root"] = global;
        var obj = new Function { Native = (self, args) => Undefined };
        obj.Properties.Values["registerClass"] = new Function { Native = (self, args) => true };
        global.Values["Object"] = obj; global.Values["MovieClip"] = new Function { Native = (self, args) => Undefined, Base = obj };
    }
    internal void Run(byte[] code) => Execute(code, root, new object[4]);
    internal object Value(string name) => GetVariable(root, name);
    private object GetVariable(Scope scope, string name)
    { if (name == "this") return scope.This; if (name == "super") return scope.Super ?? Undefined; for (Scope s = scope; s != null; s = s.Parent) if (s.Locals.TryGetValue(name, out object v)) return v; return Get(global, name); }
    private void SetVariable(Scope scope, string name, object value)
    { for (Scope s = scope; s != null; s = s.Parent) if (s.Locals.ContainsKey(name)) { s.Locals[name] = value; return; } global.Values[name] = value; }
    private object Get(object owner, string key)
    {
        if (owner is Function f) { if (key == "prototype") return f.Prototype; return Get(f.Properties, key); }
        if (owner is ObjectValue o)
        { for (ObjectValue current = o; current != null; current = current.Parent) { if (current.Accessors.TryGetValue(key, out var accessor)) return Call(accessor.Getter, owner, Array.Empty<object>()); if (current.Values.TryGetValue(key, out object value)) return value; } if (key == "addProperty") return new Function { Native = (self, args) => { ((ObjectValue)self).Accessors[Text(args[0])] = (args[1], args[2]); return true; } }; }
        return Undefined;
    }
    private void Set(object owner, string key, object value)
    { if (owner is Function f) { if (key == "prototype") f.Prototype = (ObjectValue)value; else Set(f.Properties, key, value); return; } if (!(owner is ObjectValue o)) throw new Exception("AVM1: assigning to non-object"); for (var p = o; p != null; p = p.Parent) if (p.Accessors.TryGetValue(key, out var a)) { Call(a.Setter, owner, new[] { value }); return; } o.Values[key] = value; }
    private object Call(object callable, object self, object[] args)
    {
        if (!(callable is Function f)) { if (ReferenceEquals(callable, Undefined) || callable == null) return Undefined; throw new Exception("AVM1: not callable"); }
        if (f.Native != null) return f.Native(self, args); var scope = new Scope { Parent = f.Closure, This = self, Super = f.Base };
        for (int i = 0; i < f.Parameters.Length; i++) scope.Locals[f.Parameters[i]] = i < args.Length ? args[i] : Undefined;
        return Execute(f.Body, scope, new object[4]);
    }
    private static string Text(object value) => value == null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture);
    private static double Number(object value) => value == null ? 0 : value is bool b ? b ? 1 : 0 : double.TryParse(Text(value), NumberStyles.Float, CultureInfo.InvariantCulture, out double n) ? n : double.NaN;
    private static bool Truth(object value) => value != null && !(value is Missing) && (value is bool b ? b : value is string s ? s.Length != 0 : value is ObjectValue || value is Function || Number(value) != 0 && !double.IsNaN(Number(value)));
    private object Execute(byte[] bytes, Scope scope, object[] registers)
    {
        var stack = new List<object>(); int pc = 0;
        object Pop() { if (stack.Count == 0) throw new Exception("AVM1 stack underflow at " + pc); object v = stack[stack.Count - 1]; stack.RemoveAt(stack.Count - 1); return v; }
        int U16(ref int at) { int v = bytes[at] | bytes[at + 1] << 8; at += 2; return v; }
        string String(ref int at) { int start = at; while (bytes[at] != 0) at++; string s = Encoding.UTF8.GetString(bytes, start, at - start); at++; return s; }
        while (pc < bytes.Length)
        {
            if (++steps > 100000) throw new Exception("AVM1 execution limit"); int opcode = bytes[pc++], length = opcode >= 128 ? U16(ref pc) : 0, payload = pc, next = pc + length;
            switch (opcode)
            {
                case 0: return Undefined;
                case 6: case 7: break; // play / stop have no data-stack effect
                case 0x17: Pop(); break;
                case 0x1c: stack.Add(GetVariable(scope, Text(Pop()))); break;
                case 0x1d: { object v = Pop(); SetVariable(scope, Text(Pop()), v); break; }
                case 0x3c: { object v = Pop(); scope.Locals[Text(Pop())] = v; break; }
                case 0x41: scope.Locals[Text(Pop())] = Undefined; break;
                case 0x4c: stack.Add(stack[stack.Count - 1]); break;
                case 0x4d: { object a = Pop(), b = Pop(); stack.Add(a); stack.Add(b); break; }
                case 0x4e: { string key = Text(Pop()); stack.Add(Get(Pop(), key)); break; }
                case 0x4f: { object value = Pop(); string key = Text(Pop()); Set(Pop(), key, value); break; }
                case 0x87: registers[bytes[pc]] = stack[stack.Count - 1]; break;
                case 0x96:
                    while (pc < next)
                    {
                        int type = bytes[pc++]; object value;
                        switch (type) { case 0: value = String(ref pc); break; case 2: value = null; break; case 3: value = Undefined; break; case 4: value = registers[bytes[pc++]] ?? Undefined; break; case 5: value = bytes[pc++] != 0; break; case 7: value = BitConverter.ToInt32(bytes, pc); pc += 4; break; case 6: byte[] data = new byte[8]; Array.Copy(bytes, pc, data, 4, 4); Array.Copy(bytes, pc + 4, data, 0, 4); value = BitConverter.ToDouble(data, 0); pc += 8; break; default: throw new Exception("Unexpected AVM1 push type " + type); } stack.Add(value);
                    } break;
                case 0x12: stack.Add(!Truth(Pop())); break;
                case 0x47: { object b = Pop(), a = Pop(); stack.Add(a is string || b is string ? (object)(Text(a) + Text(b)) : Number(a) + Number(b)); break; }
                case 0x0b: case 0x0c: case 0x0d: case 0x3f: { double b = Number(Pop()), a = Number(Pop()); stack.Add(opcode == 0x0b ? a - b : opcode == 0x0c ? a * b : opcode == 0x0d ? a / b : a % b); break; }
                case 0x48: case 0x67: { double b = Number(Pop()), a = Number(Pop()); stack.Add(opcode == 0x48 ? a < b : a > b); break; }
                case 0x49: case 0x66: { object b = Pop(), a = Pop(); bool equal = ReferenceEquals(a, b) || a != null && a.Equals(b); if (!equal && opcode == 0x49) equal = (a == null || a is Missing) && (b == null || b is Missing) || Number(a) == Number(b); if (!equal && opcode == 0x66 && a != null && b != null && !(a is string || b is string || a is bool || b is bool || a is Missing || b is Missing)) equal = Number(a) == Number(b); stack.Add(equal); break; }
                case 0x50: case 0x51: stack.Add(Number(Pop()) + (opcode == 0x50 ? 1 : -1)); break;
                case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: { int b = (int)Number(Pop()), a = (int)Number(Pop()); stack.Add(opcode == 0x60 ? a & b : opcode == 0x61 ? a | b : opcode == 0x62 ? a ^ b : opcode == 0x63 ? a << b : opcode == 0x64 ? a >> b : (int)((uint)a >> b)); break; }
                case 0x4a: case 0x18: { double n = Number(Pop()); stack.Add(opcode == 0x18 ? Math.Truncate(n) : n); break; }
                case 0x4b: stack.Add(Text(Pop())); break;
                case 0x44: { object v = Pop(); stack.Add(v is Missing ? "undefined" : v is Function ? "function" : v == null || v is ObjectValue ? "object" : v is string ? "string" : v is bool ? "boolean" : "number"); break; }
                case 0x26: Traces.Add(Text(Pop())); break;
                case 0x99: case 0x9d: { int offset = (short)U16(ref pc); if (opcode == 0x99 || Truth(Pop())) next += offset; if (next < 0 || next > bytes.Length) throw new Exception("Invalid AVM1 branch"); break; }
                case 0x9b:
                    { string name = String(ref pc); int count = U16(ref pc); var parameters = new string[count]; for (int i = 0; i < count; i++) parameters[i] = String(ref pc); int size = U16(ref pc); if (pc != next) throw new Exception("Function action length includes body"); var f = new Function { Parameters = parameters, Body = bytes.Skip(next).Take(size).ToArray(), Closure = scope }; if (f.Body.Length != size) throw new Exception("Truncated function body"); if (name.Length == 0) stack.Add(f); else if (scope == root) global.Values[name] = f; else scope.Locals[name] = f; next += size; break; }
                case 0x3e: return Pop();
                case 0x3d: case 0x40: case 0x52: case 0x53:
                    { object callable, self; if (opcode == 0x52 || opcode == 0x53) { string name = Text(Pop()); self = Pop(); callable = name.Length == 0 ? self : Get(self, name); } else { string name = Text(Pop()); self = global; callable = GetVariable(scope, name); } int count = (int)Number(Pop()); var args = new object[count]; for (int i = 0; i < count; i++) args[i] = Pop(); if (opcode == 0x40 || opcode == 0x53) { if (!(callable is Function f)) throw new Exception("Not a constructor"); var instance = new ObjectValue { Parent = f.Prototype }; Call(f, instance, args); stack.Add(instance); } else stack.Add(Call(callable, self, args)); break; }
                case 0x69: { var parent = (Function)Pop(); var child = (Function)Pop(); child.Base = parent; child.Prototype.Parent = parent.Prototype; break; }
                case 0x42: { int count = (int)Number(Pop()); var array = new ObjectValue(); for (int i = 0; i < count; i++) array.Values[i.ToString()] = Pop(); array.Values["length"] = count; stack.Add(array); break; }
                case 0x43: { int count = (int)Number(Pop()); var obj = new ObjectValue(); for (int i = 0; i < count; i++) { object v = Pop(); obj.Values[Text(Pop())] = v; } stack.Add(obj); break; }
                case 0x55: { var obj = (ObjectValue)Pop(); stack.Add(null); foreach (string key in obj.Values.Keys) stack.Add(key); break; }
                default: throw new Exception("Test evaluator does not implement AVM1 opcode 0x" + opcode.ToString("x2"));
            }
            pc = next;
        }
        return Undefined;
    }
}
