using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ExpressionEval;

// Immutable exact-text programs contain syntax only. Every replay creates fresh
// legacy values and performs the current Define lookups and conversions again.
internal sealed class ExpressionProgram
{
    internal enum Op
    {
        Integer, Float, QuotedString, Define, LogicalOr, LogicalAnd, IsEqual,
        IsLessThan, IsGreaterThan, Not, Negate, Multiply, Divide, Modulo, Add, Subtract, FuncRound
    }
    internal sealed class Node
    {
        internal readonly Op Operation;
        internal readonly string Text;
        internal readonly Node Left, Right;
        internal readonly int Depth;
        internal Node(Op operation, string text, Node left, Node right)
        {
            Operation = operation; Text = text; Left = left; Right = right;
            Depth = 1 + Math.Max(left == null ? 0 : left.Depth, right == null ? 0 : right.Depth);
        }
        internal ExpressionValue Evaluate(ExpressionEvaluator evaluator)
        {
            switch (Operation)
            {
                case Op.Integer: return ExpressionValue.NewInteger(Text);
                case Op.Float: return ExpressionValue.NewFloat(Text);
                case Op.QuotedString: return ExpressionValue.NewQuotedString(Text);
                case Op.Define: return ExpressionValue.NewFromLookup(evaluator.LookupDefine(Text));
            }
            ExpressionValue value = Left.Evaluate(evaluator);
            // Preserve parser execution order and its eager right operand.
            ExpressionValue rhs = Right == null ? null : Right.Evaluate(evaluator);
            switch (Operation)
            {
                case Op.LogicalOr: value.LogicalOr(rhs); break;
                case Op.LogicalAnd: value.LogicalAnd(rhs); break;
                case Op.IsEqual: value.IsEqual(rhs); break;
                case Op.IsLessThan: value.IsLessThan(rhs); break;
                case Op.IsGreaterThan: value.IsGreaterThan(rhs); break;
                case Op.Not: value.Not(); break;
                case Op.Negate: value.Negate(); break;
                case Op.Multiply: value.Multiply(rhs); break;
                case Op.Divide: value.Divide(rhs); break;
                case Op.Modulo: value.Modulo(rhs); break;
                case Op.Add: value.Add(rhs); break;
                case Op.Subtract: value.Subtract(rhs); break;
                case Op.FuncRound: value.FuncRound(); break;
                default: throw new InvalidOperationException("Unknown expression opcode.");
            }
            return value;
        }
    }
    internal readonly Node Root;
    internal readonly int NodeCount;
    internal readonly long TextChars;
    internal ExpressionProgram(Node root, int nodeCount, long textChars)
    {
        Root = root; NodeCount = nodeCount; TextChars = textChars;
    }
    internal string Evaluate(ExpressionEvaluator evaluator) { return Root.Evaluate(evaluator).AsString(); }
}

// The authoritative parser evaluates first. This recorder only observes the
// result identity of successful leaves/operations, never replaces their work.
internal sealed class ExpressionProgramBuilder
{
    internal const int MaxNodes = 4096, MaxDepth = 128;
    private sealed class ValueIdentityComparer : IEqualityComparer<ExpressionValue>
    {
        public bool Equals(ExpressionValue left, ExpressionValue right) { return Object.ReferenceEquals(left, right); }
        public int GetHashCode(ExpressionValue value) { return RuntimeHelpers.GetHashCode(value); }
    }
    private Dictionary<ExpressionValue, ExpressionProgram.Node> _Values = new Dictionary<ExpressionValue, ExpressionProgram.Node>(new ValueIdentityComparer());
    private int _Nodes;
    private long _TextChars;
    internal string FallbackReason { get; private set; }
    private void Abandon(string reason) { FallbackReason = reason; _Values = null; }
    private void Record(ExpressionValue value, ExpressionProgram.Op operation, string text, ExpressionProgram.Node left, ExpressionProgram.Node right)
    {
        if (_Values == null) return;
        if (_Nodes >= MaxNodes) { Abandon("NodeLimit"); return; }
        int depth = 1 + Math.Max(left == null ? 0 : left.Depth, right == null ? 0 : right.Depth);
        if (depth > MaxDepth) { Abandon("DepthLimit"); return; }
        _Values[value] = new ExpressionProgram.Node(operation, text, left, right);
        _Nodes++; _TextChars += text == null ? 0 : text.Length;
    }
    internal void Leaf(ExpressionValue value, ExpressionProgram.Op operation, string text)
    {
        Record(value, operation, text, null, null);
    }
    internal void Unary(ExpressionValue value, ExpressionProgram.Op operation)
    {
        if (_Values == null) return;
        ExpressionProgram.Node prior;
        if (!_Values.TryGetValue(value, out prior)) { Abandon("UnrecordedValue"); return; }
        Record(value, operation, null, prior, null);
    }
    internal void Binary(ExpressionValue value, ExpressionValue rhs, ExpressionProgram.Op operation)
    {
        if (_Values == null) return;
        ExpressionProgram.Node left, right;
        if (!_Values.TryGetValue(value, out left) || !_Values.TryGetValue(rhs, out right)) { Abandon("UnrecordedValue"); return; }
        Record(value, operation, null, left, right);
    }
    internal ExpressionProgram Complete(ExpressionValue value)
    {
        if (_Values == null) return null;
        ExpressionProgram.Node root;
        if (!_Values.TryGetValue(value, out root)) { Abandon("UnrecordedValue"); return null; }
        var result = new ExpressionProgram(root, _Nodes, _TextChars);
        _Values = null; return result;
    }
}

internal sealed class ExpressionProgramCache
{
    internal const int EntryLimit = 128, ExpressionTextLimit = 16384, NodeBudget = 32768;
    internal const long TextBudget = 262144;
    private static readonly ConditionalWeakTable<DocumentProcessor, ExpressionProgramCache> Owners = new ConditionalWeakTable<DocumentProcessor, ExpressionProgramCache>();
    private sealed class Entry
    {
        internal readonly string Text;
        internal readonly ExpressionProgram Program;
        internal Entry(string text, ExpressionProgram program) { Text = text; Program = program; }
        internal long TextChars { get { return Text.Length + Program.TextChars; } }
    }
    private readonly object _Gate = new object();
    private readonly Dictionary<string, LinkedListNode<Entry>> _Entries = new Dictionary<string, LinkedListNode<Entry>>(StringComparer.Ordinal);
    private readonly LinkedList<Entry> _Lru = new LinkedList<Entry>();
    private readonly Dictionary<string, long> _FallbackReasons = new Dictionary<string, long>(StringComparer.Ordinal);
    private long _ParserCalls, _Builds, _Hits, _Evictions, _Fallbacks, _ReplayFailures, _ParserFailures, _TextChars, _NodeCount;
    internal static ExpressionProgramCache For(DocumentProcessor owner)
    {
        return owner == null ? null : Owners.GetValue(owner, delegate(DocumentProcessor ignored) { return new ExpressionProgramCache(); });
    }
    internal static bool Enabled
    {
        get { bool disabled; return !(AppContext.TryGetSwitch("NewSdk.Compiler.DisableExpressionPrograms", out disabled) && disabled); }
    }
    internal bool TryGet(string expression, out ExpressionProgram program)
    {
        lock (_Gate)
        {
            LinkedListNode<Entry> entry;
            if (!_Entries.TryGetValue(expression, out entry)) { program = null; return false; }
            _Lru.Remove(entry); _Lru.AddLast(entry); _Hits++; program = entry.Value.Program; return true;
        }
    }
    internal void ParserCall() { lock (_Gate) _ParserCalls++; }
    internal void ParserFailure() { lock (_Gate) _ParserFailures++; }
    internal void ReplayFailure() { lock (_Gate) _ReplayFailures++; }
    internal void Fallback(string reason)
    {
        lock (_Gate)
        {
            _Fallbacks++; long count; _FallbackReasons.TryGetValue(reason, out count); _FallbackReasons[reason] = count + 1;
        }
    }
    internal void Publish(string expression, ExpressionProgram program)
    {
        long chars = expression.Length + program.TextChars;
        if (chars > TextBudget || program.NodeCount > NodeBudget) { Fallback("EntryBudget"); return; }
        lock (_Gate)
        {
            // Concurrent equal successful parsing may construct two syntax-only
            // programs; keep the first one without retaining an extra entry.
            if (_Entries.ContainsKey(expression)) return;
            while (_Entries.Count >= EntryLimit || chars > TextBudget - _TextChars || program.NodeCount > NodeBudget - _NodeCount)
            {
                LinkedListNode<Entry> oldest = _Lru.First;
                _Lru.RemoveFirst(); _Entries.Remove(oldest.Value.Text);
                _TextChars -= oldest.Value.TextChars; _NodeCount -= oldest.Value.Program.NodeCount; _Evictions++;
            }
            var entry = new Entry(expression, program); _Entries.Add(expression, _Lru.AddLast(entry));
            _TextChars += entry.TextChars; _NodeCount += program.NodeCount; _Builds++;
        }
    }
    internal object GetMetrics()
    {
        lock (_Gate)
            return new { enabled = Enabled, scope = "ExactExpressionProgramsPerDocumentProcessor", parserCalls = _ParserCalls,
                builds = _Builds, hits = _Hits, evictions = _Evictions, fallbacks = _Fallbacks, parserFailures = _ParserFailures, replayFailures = _ReplayFailures,
                entries = _Entries.Count, retainedTextChars = _TextChars, retainedNodes = _NodeCount, entryLimit = EntryLimit, textBudget = TextBudget,
                nodeBudget = NodeBudget, expressionTextLimit = ExpressionTextLimit, expressionNodeLimit = ExpressionProgramBuilder.MaxNodes,
                expressionDepthLimit = ExpressionProgramBuilder.MaxDepth, fallbackReasons = new Dictionary<string, long>(_FallbackReasons, StringComparer.Ordinal) };
    }
}
