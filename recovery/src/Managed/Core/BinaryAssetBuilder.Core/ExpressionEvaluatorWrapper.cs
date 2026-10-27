#if NEWSdk_MONOLITHIC
namespace BinaryAssetBuilder.Core;

// Compatibility method name retained for DocumentProcessor; all production
// binding is direct and cannot silently disable expressions on loader failure.
public class ExpressionEvaluatorWrapper
{
    public static void LoadAssembly() { }
    public static object GetProgramMetrics(DocumentProcessor processor)
    {
        return BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator.GetProgramMetrics(processor);
    }
    public static IExpressionEvaluator GetEvaluator(AssetDeclarationDocument document)
    {
        return new BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator(document);
    }
}
#else
using System;
using System.Reflection;

namespace BinaryAssetBuilder.Core;

public class ExpressionEvaluatorWrapper
{
	private static Tracer _Tracer = Tracer.GetTracer("XIncludingReader", "Provides xi:include reading functionality");

	private static Assembly _Lib = null;

	public static void LoadAssembly()
	{
		try
		{
			_Lib = Assembly.Load("BinaryAssetBuilder.ExpressionEval");
			_Lib.GetType("BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator");
		}
		catch (Exception)
		{
			_Lib = null;
			_Tracer.TraceError("Could not load ExpressionEvaluator from BinaryAssetBuilder.ExpressionEval.dll. Expressions are disabled");
		}
	}

    public static object GetProgramMetrics(DocumentProcessor processor)
    {
        if (_Lib == null) return null;
        Type evaluator = _Lib.GetType("BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator");
        if (evaluator == null) return null;
        MethodInfo method = evaluator.GetMethod("GetProgramMetrics", new Type[] { typeof(DocumentProcessor) });
        return method == null ? null : method.Invoke(null, new object[] { processor });
    }

	public static IExpressionEvaluator GetEvaluator(AssetDeclarationDocument document)
	{
		try
		{
			if (_Lib != null)
			{
				object[] args = new object[1] { document };
				return (IExpressionEvaluator)_Lib.CreateInstance("BinaryAssetBuilder.ExpressionEval.ExpressionEvaluator", ignoreCase: false, BindingFlags.CreateInstance, null, args, null, null);
			}
		}
		catch (Exception)
		{
			_Tracer.TraceError("Could not load ExpressionEvaluator from BinaryAssetBuilder.ExpressionEval.dll. Expressions are disabled");
		}
		return null;
	}
}

#endif
