using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BinaryAssetBuilder.UiCompiler.Swf
{
    /// <summary>AS1/AS2 source to AVM1 bytecode. No editor, shell or Java runtime.</summary>
    internal sealed class ActionScriptCompiler
    {
        private sealed class Token { internal string Text; internal object Value; internal int Line; }
        private sealed class Node
        {
            internal string Kind, Name; internal object Value; internal List<Node> Children = new List<Node>();
            internal Node(string kind, string name = null, object value = null, params Node[] children) { Kind = kind; Name = name; Value = value; Children.AddRange(children); }
        }
        private sealed class Code
        {
            internal readonly MemoryStream Bytes = new MemoryStream();
            private readonly Dictionary<int, int> labels = new Dictionary<int, int>();
            private readonly List<Tuple<int, int>> jumps = new List<Tuple<int, int>>(); private int next;
            internal int Label() => next++;
            internal void Mark(int label) => labels.Add(label, (int)Bytes.Position);
            internal void Op(int opcode, byte[] payload = null) { Bytes.WriteByte((byte)opcode); if (opcode >= 0x80) { payload = payload ?? Array.Empty<byte>(); if (payload.Length > ushort.MaxValue) throw new NotSupportedException("AVM1 action exceeds 64 KiB."); Bytes.WriteByte((byte)payload.Length); Bytes.WriteByte((byte)(payload.Length >> 8)); Bytes.Write(payload, 0, payload.Length); } }
            internal void Jump(int opcode, int label) { Op(opcode, new byte[2]); jumps.Add(Tuple.Create((int)Bytes.Position - 2, label)); }
            internal void Push(object value)
            {
                var w = new SwfWriter();
                if (value is string s) { w.u8(0); w.text(s); }
                else if (value is bool b) { w.u8(5); w.u8(b ? 1 : 0); }
                else if (value == null) w.u8(2);
                else if (value is Undefined) w.u8(3);
                else if (value is int i) { w.u8(7); w.u32(i); }
                else { double d = Convert.ToDouble(value, CultureInfo.InvariantCulture); byte[] data = BitConverter.GetBytes(d); w.u8(6); w.bytes(data.Skip(4).Concat(data.Take(4)).ToArray()); }
                Op(0x96, w.data());
            }
            internal void Register(int register) => Op(0x96, new byte[] { 4, (byte)register });
            internal void Store(int register) => Op(0x87, new[] { (byte)register });
            internal byte[] Finish(bool end = false)
            {
                if (end) Op(0); byte[] data = Bytes.ToArray(); foreach (var j in jumps) { if (!labels.TryGetValue(j.Item2, out int target)) throw new InvalidOperationException("Unbound AVM1 label."); int offset = target - j.Item1 - 2; if (offset < short.MinValue || offset > short.MaxValue) throw new NotSupportedException("AVM1 branch exceeds 32 KiB."); data[j.Item1] = (byte)offset; data[j.Item1 + 1] = (byte)(offset >> 8); } return data;
            }
        }
        private sealed class Undefined { internal static readonly Undefined Value = new Undefined(); }
        private readonly List<Token> tokens = new List<Token>(); private int cursor;
        private Code code = new Code(); private readonly Stack<Tuple<int, int>> loops = new Stack<Tuple<int, int>>();
        private ActionScriptCompiler(string source) { Lex(source); }
        internal static byte[] Compile(string source) { var c = new ActionScriptCompiler(source); while (!c.At("<eof>")) c.Statement(); return c.code.Finish(true); }
        private Token Current => tokens[cursor];
        private bool At(string text) => Current.Text == text;
        private bool Eat(string text) { if (!At(text)) return false; cursor++; return true; }
        private void Need(string text) { if (!Eat(text)) throw Error("Expected '" + text + "', found '" + Current.Text + "'."); }
        private Exception Error(string text) => new InvalidDataException("AS2 line " + Current.Line + ": " + text);
        private string Identifier() { if (Current.Value != null || At("<eof>") || !(char.IsLetter(Current.Text[0]) || Current.Text[0] == '_' || Current.Text[0] == '$')) throw Error("Expected identifier."); return tokens[cursor++].Text; }
        private void Semicolon() { if (!Eat(";") && !At("}") && !At("<eof>") && (cursor == 0 || Current.Line == tokens[cursor - 1].Line)) throw Error("Expected ';'."); }
        private void TypeAnnotation() { if (Eat(":")) { Identifier(); while (Eat(".")) Identifier(); } }
        private void Statement()
        {
            if (Eat(";")) return;
            if (Eat("{")) { while (!Eat("}")) { if (At("<eof>")) throw Error("Unclosed block."); Statement(); } return; }
            if (Eat("var")) { Variables(); Semicolon(); return; }
            if (Eat("if")) { Need("("); var test = Expression(); Need(")"); int no = code.Label(), end = code.Label(); Emit(test); code.Op(0x12); code.Jump(0x9d, no); Statement(); if (Eat("else")) { code.Jump(0x99, end); code.Mark(no); Statement(); code.Mark(end); } else code.Mark(no); return; }
            if (Eat("while"))
            { int begin = code.Label(), end = code.Label(); code.Mark(begin); Need("("); Emit(Expression()); Need(")"); code.Op(0x12); code.Jump(0x9d, end); loops.Push(Tuple.Create(end, begin)); Statement(); loops.Pop(); code.Jump(0x99, begin); code.Mark(end); return; }
            if (Eat("do"))
            { int begin = code.Label(), condition = code.Label(), end = code.Label(); code.Mark(begin); loops.Push(Tuple.Create(end, condition)); Statement(); loops.Pop(); Need("while"); Need("("); code.Mark(condition); Emit(Expression()); Need(")"); code.Jump(0x9d, begin); code.Mark(end); Semicolon(); return; }
            if (Eat("for")) { For(); return; }
            if (Eat("function")) { string name = Identifier(); EmitFunction(Function(name)); return; }
            if (Eat("return")) { if (At(";") || At("}") || At("<eof>") || Current.Line > tokens[cursor - 1].Line) code.Push(Undefined.Value); else Emit(Expression()); code.Op(0x3e); Semicolon(); return; }
            if (Eat("break") || Eat("continue")) { bool cont = tokens[cursor - 1].Text == "continue"; if (loops.Count == 0 || cont && loops.Peek().Item2 < 0) throw Error("No enclosing loop."); code.Jump(0x99, cont ? loops.Peek().Item2 : loops.Peek().Item1); Semicolon(); return; }
            if (Eat("throw")) { Emit(Expression()); code.Op(0x2a); Semicolon(); return; }
            if (Eat("switch")) { Switch(); return; }
            if (At("class") || At("interface") || At("import") || At("#include") || At("try") || At("with")) throw Error("Unsupported AS2 construct '" + Current.Text + "'.");
            Emit(Expression()); code.Op(0x17); Semicolon();
        }
        private void Variables()
        { do { string name = Identifier(); TypeAnnotation(); code.Push(name); if (Eat("=")) { Emit(Expression(2)); code.Op(0x3c); } else code.Op(0x41); } while (Eat(",")); }
        private void For()
        {
            Need("("); if (!Eat(";")) { if (Eat("var")) Variables(); else { Emit(Expression()); code.Op(0x17); } Need(";"); }
            int begin = code.Label(), increment = code.Label(), end = code.Label(); code.Mark(begin);
            if (!Eat(";")) { Emit(Expression()); Need(";"); code.Op(0x12); code.Jump(0x9d, end); }
            Node step = Eat(")") ? null : Expression(); if (step != null) Need(")");
            loops.Push(Tuple.Create(end, increment)); Statement(); loops.Pop(); code.Mark(increment); if (step != null) { Emit(step); code.Op(0x17); } code.Jump(0x99, begin); code.Mark(end);
        }
        private void Switch()
        {
            Need("("); Emit(Expression()); Need(")"); Need("{");
            // Keep the discriminant on the stack until a case is selected.
            int dispatch = code.Label(), end = code.Label(); code.Jump(0x99, dispatch); var cases = new List<Tuple<Node, int>>(); int otherwise = -1;
            loops.Push(Tuple.Create(end, -1));
            while (!Eat("}"))
            { if (Eat("case")) { Node value = Expression(2); Need(":"); int label = code.Label(); code.Mark(label); cases.Add(Tuple.Create(value, label)); }
              else if (Eat("default")) { Need(":"); if (otherwise >= 0) throw Error("Duplicate default."); otherwise = code.Label(); code.Mark(otherwise); }
              else Statement(); }
            loops.Pop(); code.Jump(0x99, end); code.Mark(dispatch);
            var selections = new List<Tuple<int, int>>();
            foreach (var item in cases) { code.Op(0x4c); Emit(item.Item1); code.Op(0x66); int selected = code.Label(); code.Jump(0x9d, selected); selections.Add(Tuple.Create(selected, item.Item2)); }
            code.Op(0x17); code.Jump(0x99, otherwise >= 0 ? otherwise : end);
            foreach (var selection in selections) { code.Mark(selection.Item1); code.Op(0x17); code.Jump(0x99, selection.Item2); }
            code.Mark(end);
        }
        private static int Precedence(string op)
        {
            switch (op) { case ",": return 1; case "=": case "+=": case "-=": case "*=": case "/=": case "%=": case "&=": case "|=": case "^=": case "<<=": case ">>=": case ">>>=": return 2; case "?": return 3; case "||": return 4; case "&&": return 5; case "|": return 6; case "^": return 7; case "&": return 8; case "==": case "!=": case "===": case "!==": return 9; case "<": case ">": case "<=": case ">=": case "instanceof": return 10; case "<<": case ">>": case ">>>": return 11; case "+": case "-": return 12; case "*": case "/": case "%": return 13; default: return 0; }
        }
        private Node Expression(int minimum = 1)
        {
            Node left = Prefix();
            while (true)
            {
                string op = Current.Text; int p = Precedence(op); if (p == 0 || p < minimum) return left; cursor++;
                if (op == "?") { var yes = Expression(2); Need(":"); left = new Node("ternary", null, null, left, yes, Expression(3)); }
                else left = new Node(p == 2 ? "assign" : "binary", op, null, left, Expression(p == 2 ? p : p + 1));
            }
        }
        private Node Prefix()
        {
            string op = Current.Text;
            if (op == "!" || op == "~" || op == "-" || op == "+" || op == "typeof" || op == "delete" || op == "void" || op == "++" || op == "--") { cursor++; return new Node("unary", op, null, Prefix()); }
            if (Eat("new")) { Node target = Primary(); while (true) { if (Eat(".")) target = new Node("member", null, null, target, new Node("literal", null, Identifier())); else if (Eat("[")) { target = new Node("member", null, null, target, Expression()); Need("]"); } else break; } var result = new Node("new", null, null, target); if (Eat("(")) Arguments(result); return Postfix(result); }
            return Postfix(Primary());
        }
        private Node Primary()
        {
            if (Eat("(")) { Node value = Expression(); Need(")"); return value; }
            if (Eat("function")) { string name = At("(") ? "" : Identifier(); return Function(name); }
            if (Eat("[")) { var result = new Node("array"); if (!Eat("]")) { do { result.Children.Add(At(",") ? new Node("literal", null, Undefined.Value) : Expression(2)); } while (Eat(",") && !At("]")); Need("]"); } return result; }
            if (Eat("{")) { var result = new Node("object"); if (!Eat("}")) { do { string name = Current.Value != null ? Convert.ToString(tokens[cursor++].Value, CultureInfo.InvariantCulture) : Identifier(); Need(":"); result.Children.Add(new Node("literal", null, name)); result.Children.Add(Expression(2)); } while (Eat(",") && !At("}")); Need("}"); } return result; }
            if (Eat("true")) return new Node("literal", null, true); if (Eat("false")) return new Node("literal", null, false); if (Eat("null")) return new Node("literal"); if (Eat("undefined")) return new Node("literal", null, Undefined.Value);
            if (Current.Value != null) return new Node("literal", null, tokens[cursor++].Value); return new Node("variable", Identifier());
        }
        private Node Postfix(Node value)
        {
            while (true)
            { if (Eat(".")) value = new Node("member", null, null, value, new Node("literal", null, Identifier())); else if (Eat("[")) { value = new Node("member", null, null, value, Expression()); Need("]"); }
              else if (Eat("(")) { var call = new Node("call", null, null, value); Arguments(call); value = call; }
              else if (Eat("++") || Eat("--")) value = new Node("postfix", tokens[cursor - 1].Text, null, value); else return value; }
        }
        private void Arguments(Node result) { if (!Eat(")")) { do { result.Children.Add(Expression(2)); } while (Eat(",")); Need(")"); } }
        private Node Function(string name)
        {
            Need("("); var parameters = new List<string>(); if (!Eat(")")) { do { parameters.Add(Identifier()); TypeAnnotation(); } while (Eat(",")); Need(")"); } TypeAnnotation();
            Code outer = code; var savedLoops = loops.ToArray(); loops.Clear(); code = new Code(); Need("{"); while (!Eat("}")) { if (At("<eof>")) throw Error("Unclosed function."); Statement(); } code.Push(Undefined.Value); code.Op(0x3e); byte[] body = code.Finish(); code = outer; foreach (var loop in savedLoops.Reverse()) loops.Push(loop);
            return new Node("function", name, Tuple.Create(parameters, body));
        }
        private void EmitFunction(Node n)
        { var f = (Tuple<List<string>, byte[]>)n.Value; var w = new SwfWriter(); w.text(n.Name); w.u16(f.Item1.Count); foreach (string parameter in f.Item1) w.text(parameter); if (f.Item2.Length > 65535) throw Error("Function exceeds 64 KiB."); w.u16(f.Item2.Length); code.Op(0x9b, w.data()); code.Bytes.Write(f.Item2, 0, f.Item2.Length); }
        private void Emit(Node n)
        {
            switch (n.Kind)
            {
                case "literal": code.Push(n.Value); return;
                case "variable": code.Push(n.Name); code.Op(0x1c); return;
                case "member": Emit(n.Children[0]); Emit(n.Children[1]); code.Op(0x4e); return;
                case "function": EmitFunction(n); return;
                case "call": case "new": Call(n); return;
                case "assign": Assign(n.Children[0], n.Children[1], n.Name); return;
                case "unary": case "postfix": Unary(n); return;
                case "array": for (int i = n.Children.Count - 1; i >= 0; i--) Emit(n.Children[i]); code.Push(n.Children.Count); code.Op(0x42); return;
                case "object": for (int i = n.Children.Count - 2; i >= 0; i -= 2) { Emit(n.Children[i]); Emit(n.Children[i + 1]); } code.Push(n.Children.Count / 2); code.Op(0x43); return;
                case "ternary": int no = code.Label(), end = code.Label(); Emit(n.Children[0]); code.Op(0x12); code.Jump(0x9d, no); Emit(n.Children[1]); code.Jump(0x99, end); code.Mark(no); Emit(n.Children[2]); code.Mark(end); return;
                case "binary":
                    if (n.Name == ",") { Emit(n.Children[0]); code.Op(0x17); Emit(n.Children[1]); return; }
                    if (n.Name == "&&" || n.Name == "||") { int done = code.Label(); Emit(n.Children[0]); code.Op(0x4c); if (n.Name == "&&") code.Op(0x12); code.Jump(0x9d, done); code.Op(0x17); Emit(n.Children[1]); code.Mark(done); return; }
                    Emit(n.Children[0]); Emit(n.Children[1]); Binary(n.Name); return;
                default: throw new InvalidOperationException(n.Kind);
            }
        }
        private void Binary(string op)
        {
            switch (op) { case "+": code.Op(0x47); break; case "-": code.Op(0x0b); break; case "*": code.Op(0x0c); break; case "/": code.Op(0x0d); break; case "%": code.Op(0x3f); break; case "==": case "!=": code.Op(0x49); if (op == "!=") code.Op(0x12); break; case "===": case "!==": code.Op(0x66); if (op == "!==") code.Op(0x12); break; case "<": code.Op(0x48); break; case ">": code.Op(0x67); break; case "<=": code.Op(0x67); code.Op(0x12); break; case ">=": code.Op(0x48); code.Op(0x12); break; case "&": code.Op(0x60); break; case "|": code.Op(0x61); break; case "^": code.Op(0x62); break; case "<<": code.Op(0x63); break; case ">>": code.Op(0x64); break; case ">>>": code.Op(0x65); break; case "instanceof": code.Op(0x54); break; default: throw Error("Unsupported operator " + op); }
        }
        private void Target(Node target, bool read)
        {
            if (target.Kind == "variable") { code.Push(target.Name); if (read) { code.Op(0x4c); code.Op(0x1c); } }
            else if (target.Kind == "member") { Emit(target.Children[0]); Emit(target.Children[1]); if (read) { code.Store(2); code.Op(0x17); code.Store(1); code.Register(2); code.Register(1); code.Register(2); code.Op(0x4e); } }
            else throw Error("Expression cannot be assigned.");
        }
        private void Assign(Node target, Node value, string op)
        { Target(target, op != "="); Emit(value); if (op != "=") Binary(op.Substring(0, op.Length - 1)); code.Store(0); code.Op(target.Kind == "variable" ? 0x1d : 0x4f); code.Register(0); }
        private void Unary(Node n)
        {
            Node value = n.Children[0];
            if (n.Name == "++" || n.Name == "--")
            { Target(value, true); if (n.Kind == "postfix") code.Store(3); code.Op(n.Name == "++" ? 0x50 : 0x51); code.Store(0); code.Op(value.Kind == "variable" ? 0x1d : 0x4f); code.Register(n.Kind == "postfix" ? 3 : 0); return; }
            if (n.Name == "delete") { if (value.Kind == "member") { Emit(value.Children[0]); Emit(value.Children[1]); code.Op(0x3a); } else if (value.Kind == "variable") { code.Push(value.Name); code.Op(0x3b); } else throw Error("Invalid delete operand."); return; }
            if (n.Name == "-") { code.Push(0); Emit(value); code.Op(0x0b); return; }
            Emit(value); switch (n.Name) { case "!": code.Op(0x12); break; case "~": code.Push(-1); code.Op(0x62); break; case "+": code.Op(0x4a); break; case "typeof": code.Op(0x44); break; case "void": code.Op(0x17); code.Push(Undefined.Value); break; }
        }
        private void Call(Node n)
        {
            Node target = n.Children[0]; int count = n.Children.Count - 1; bool create = n.Kind == "new";
            if (!create && target.Kind == "variable")
            {
                string name = target.Name; var simple = new Dictionary<string, int> { { "stop", 7 }, { "play", 6 }, { "nextFrame", 4 }, { "prevFrame", 5 }, { "stopAllSounds", 9 }, { "getTimer", 0x34 } };
                if (simple.TryGetValue(name, out int op)) { if (count != 0) throw Error(name + " takes no arguments."); code.Op(op); if (name != "getTimer") code.Push(Undefined.Value); return; }
                if (name == "trace" || name == "random" || name == "int" || name == "Number" || name == "String") { if (count != 1) throw Error(name + " takes one argument."); Emit(n.Children[1]); code.Op(name == "trace" ? 0x26 : name == "random" ? 0x30 : name == "int" ? 0x18 : name == "Number" ? 0x4a : 0x4b); if (name == "trace") code.Push(Undefined.Value); return; }
                if (name == "gotoAndPlay" || name == "gotoAndStop") { if (count != 1) throw Error(name + " takes one frame."); Emit(n.Children[1]); code.Op(0x9f, new[] { (byte)(name == "gotoAndPlay" ? 1 : 0) }); code.Push(Undefined.Value); return; }
                if (name == "getURL" || name == "loadMovie" || name == "loadVariables") { if (count < 1 || count > 3) throw Error("Invalid URL arguments."); Emit(n.Children[1]); if (count >= 2) Emit(n.Children[2]); else code.Push(""); byte flags = (byte)(name == "loadMovie" ? 0x40 : name == "loadVariables" ? 0x80 : 0); if (count == 3) { if (n.Children[3].Value is string method) flags |= (byte)(method.ToUpperInvariant() == "POST" ? 2 : method.ToUpperInvariant() == "GET" ? 1 : 0); else throw Error("URL method must be a literal."); } code.Op(0x9a, new[] { flags }); code.Push(Undefined.Value); return; }
            }
            for (int i = n.Children.Count - 1; i >= 1; i--) Emit(n.Children[i]); code.Push(count);
            if (target.Kind == "variable") { code.Push(target.Name); code.Op(create ? 0x40 : 0x3d); }
            else if (target.Kind == "member") { Emit(target.Children[0]); Emit(target.Children[1]); code.Op(create ? 0x53 : 0x52); }
            else { Emit(target); code.Push(""); code.Op(create ? 0x53 : 0x52); }
        }
        internal static byte[] CompileHandlers(string source, bool button)
        {
            if (string.IsNullOrWhiteSpace(source)) return Array.Empty<byte>(); var c = new ActionScriptCompiler(source); var entries = new List<Tuple<uint, byte, byte[]>>();
            var events = new Dictionary<string, uint>(StringComparer.Ordinal) { { "keyUp", 1 }, { "keyDown", 2 }, { "mouseUp", 4 }, { "mouseDown", 8 }, { "mouseMove", 16 }, { "unload", 32 }, { "enterFrame", 64 }, { "load", 128 }, { "dragOver", 256 }, { "rollOut", 512 }, { "rollOver", 1024 }, { "releaseOutside", 2048 }, { "release", 4096 }, { "press", 8192 }, { "initialize", 16384 }, { "data", 32768 }, { "construct", 65536 }, { "keyPress", 131072 }, { "dragOut", 262144 } };
            while (!c.At("<eof>"))
            {
                if (!c.Eat(button ? "on" : "onClipEvent")) throw c.Error("Instance actions require " + (button ? "on(...)" : "onClipEvent(...)") + " handlers."); c.Need("("); uint flags = 0; byte key = 0;
                do { string name = c.Identifier(); if (!events.TryGetValue(name, out uint value)) throw c.Error("Unknown clip event " + name); flags |= value; if (name == "keyPress") { if (!(c.Current.Value is string text) || text.Length != 1) throw c.Error("keyPress requires a single character."); key = checked((byte)text[0]); c.cursor++; } } while (c.Eat(",")); c.Need(")"); c.code = new Code(); c.Statement(); entries.Add(Tuple.Create(flags, key, c.code.Finish(true)));
            }
            var w = new SwfWriter(); w.u16(0); uint all = 0; foreach (var e in entries) all |= e.Item1; w.u32(all);
            foreach (var e in entries) { w.u32(e.Item1); bool hasKey = (e.Item1 & 131072) != 0; w.u32(e.Item3.Length + (hasKey ? 1 : 0)); if (hasKey) w.u8(e.Item2); w.bytes(e.Item3); } w.u32(0); return w.data();
        }
        private void Lex(string source)
        {
            int i = 0, line = 1; string[] operators = { ">>>=", "===", "!==", ">>>", "<<=", ">>=", "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "==", "!=", "<=", ">=", "&&", "||", "<<", ">>" };
            while (i < source.Length)
            {
                char ch = source[i]; if (char.IsWhiteSpace(ch)) { if (ch == '\n') line++; i++; continue; }
                if (i + 1 < source.Length && source.Substring(i, 2) == "//") { while (i < source.Length && source[i] != '\n') i++; continue; }
                if (i + 1 < source.Length && source.Substring(i, 2) == "/*") { i += 2; while (i + 1 < source.Length && source.Substring(i, 2) != "*/") { if (source[i++] == '\n') line++; } if (i + 1 >= source.Length) throw new InvalidDataException("AS2: unclosed comment."); i += 2; continue; }
                int start = i; object value = null;
                if (ch == '"' || ch == '\'')
                { char quote = ch; i++; var s = new StringBuilder(); while (i < source.Length && source[i] != quote) { ch = source[i++]; if (ch == '\n' || ch == '\r') throw new InvalidDataException("AS2: newline in string."); if (ch == '\\') { if (i == source.Length) break; ch = source[i++]; switch (ch) { case 'n': ch = '\n'; break; case 'r': ch = '\r'; break; case 't': ch = '\t'; break; case 'b': ch = '\b'; break; case 'f': ch = '\f'; break; case 'u': case 'x': int digits = ch == 'u' ? 4 : 2; if (i + digits > source.Length) throw new InvalidDataException("AS2: truncated escape."); ch = (char)int.Parse(source.Substring(i, digits), NumberStyles.HexNumber, CultureInfo.InvariantCulture); i += digits; break; } } s.Append(ch); } if (i == source.Length) throw new InvalidDataException("AS2: unclosed string."); i++; value = s.ToString(); }
                else if (char.IsDigit(ch) || ch == '.' && i + 1 < source.Length && char.IsDigit(source[i + 1]))
                { if (ch == '0' && i + 1 < source.Length && (source[i + 1] == 'x' || source[i + 1] == 'X')) { i += 2; int digits = i; while (i < source.Length && Uri.IsHexDigit(source[i])) i++; value = unchecked((int)uint.Parse(source.Substring(digits, i - digits), NumberStyles.HexNumber, CultureInfo.InvariantCulture)); } else { while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.')) i++; if (i < source.Length && (source[i] == 'e' || source[i] == 'E')) { i++; if (i < source.Length && (source[i] == '+' || source[i] == '-')) i++; while (i < source.Length && char.IsDigit(source[i])) i++; } double number = double.Parse(source.Substring(start, i - start), CultureInfo.InvariantCulture); value = number >= int.MinValue && number <= int.MaxValue && number == Math.Truncate(number) ? (object)(int)number : number; } }
                else if (char.IsLetter(ch) || ch == '_' || ch == '$') { do { i++; } while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_' || source[i] == '$')); }
                else { string op = operators.FirstOrDefault(o => i + o.Length <= source.Length && string.CompareOrdinal(source, i, o, 0, o.Length) == 0); i += op?.Length ?? 1; }
                tokens.Add(new Token { Text = value is string ? "<string>" : value != null ? "<number>" : source.Substring(start, i - start), Value = value, Line = line });
            }
            tokens.Add(new Token { Text = "<eof>", Line = line });
        }
    }
}
