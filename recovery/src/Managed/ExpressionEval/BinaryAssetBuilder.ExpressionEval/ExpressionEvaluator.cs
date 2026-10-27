using System.IO;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ExpressionEval;

public class ExpressionEvaluator : IExpressionEvaluator
{
	private AssetDeclarationDocument _Document;

	public ExpressionEvaluator(AssetDeclarationDocument document)
	{
		_Document = document;
	}

	public string LookupDefine(string name)
	{
		name = name.Remove(0, 1);
		Definition value = null;
		if (_Document.TryGetDefinition(name, out value) && value.EvaluatedValue != null)
		{
			if (value.Document != _Document)
			{
				_Document.UsedDefines[name] = value.EvaluatedValue;
			}
            _Document.ObserveDefineRead(name, value, value.EvaluatedValue, true);
			return value.EvaluatedValue;
		}
        _Document.ObserveDefineRead(name, value, value == null ? null : value.EvaluatedValue, false);
		throw new BinaryAssetBuilderException(ErrorCode.ExpressionEvaluationError, "Unknown define {0}", name);
	}

    public static object GetProgramMetrics(DocumentProcessor owner)
    {
        ExpressionProgramCache cache = ExpressionProgramCache.For(owner);
        return cache == null ? null : cache.GetMetrics();
    }
    public string Evaluate(string expression)
    {
        ExpressionProgramCache cache = ExpressionProgramCache.For(_Document == null ? null : _Document.ExpressionProgramOwner);
        bool enabled = ExpressionProgramCache.Enabled;
        bool eligible = cache != null && enabled && expression != null && expression.Length <= ExpressionProgramCache.ExpressionTextLimit;
        if (cache != null && !eligible) cache.Fallback(!enabled ? "Disabled" : expression == null ? "NullExpression" : "TextLimit");
        ExpressionProgram program;
        if (eligible && cache.TryGet(expression, out program))
        {
            try { return program.Evaluate(this); }
            catch { cache.ReplayFailure(); throw; }
        }
        ExpressionProgramBuilder recorder = eligible ? new ExpressionProgramBuilder() : null;
        if (cache != null) cache.ParserCall();
        try
        {
            ExpressionLexer lexer = new ExpressionLexer(new StringReader(expression));
            ExpressionParser parser = new ExpressionParser(lexer); parser.SetEvaluator(this);
            if (recorder != null) parser.SetProgramRecorder(recorder);
            ExpressionValue value = parser.expr(); string result = value.AsString();
            if (recorder != null)
            {
                program = recorder.Complete(value);
                if (program != null) cache.Publish(expression, program);
                else cache.Fallback(recorder.FallbackReason ?? "UnrecordedValue");
            }
            return result;
        }
        catch { if (cache != null) cache.ParserFailure(); throw; }
    }

	public void EvaluateDefinition(Definition definition)
	{
		if (definition.OriginalValue != null)
		{
			if (definition.OriginalValue.Length > 0 && definition.OriginalValue[0] == '=')
			{
                definition.EvaluatedValue = Evaluate(definition.OriginalValue);
			}
			else
			{
				definition.EvaluatedValue = definition.OriginalValue;
			}
			definition.OriginalValue = null;
		}
	}
}
