using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace G_Sharp;

public class Scope
{
    public Dictionary<string, Constant> Constants { get; }
    public Dictionary<string, Function> Functions { get; }

    private readonly Dictionary<string, Constant> defaultConstants = new()
    {
        ["PI"] = new Constant(Math.PI),
        ["E"]  = new Constant(Math.E)
    };

    public readonly Dictionary<string, Func<Scope, List<ExpressionSyntax>,object>> DefaultFunctions = new()
    {
        ["line"] = ScopeSupplies.LineFunction,
        ["segment"] = ScopeSupplies.SegmentFunction,
        ["ray"] = ScopeSupplies.RayFunction,
        ["circle"] = ScopeSupplies.CircleFunction,
        ["measure"] = ScopeSupplies.MeasureFunction,
        ["arc"] =  ScopeSupplies.ArcFunction,
        ["count"] = ScopeSupplies.CountFunction
    };

    public Scope(
        Dictionary<string, Constant> constants,
        Dictionary<string, Function> functions
    )
    {
        Constants = constants;
        Functions = functions;

        foreach (var item in defaultConstants.Keys)
        {
            if (!Constants.ContainsKey(item))
            {
                Constants[item] = defaultConstants[item];
            }
        }
    }
}

public sealed class Constant 
{
    public object Expression { get; }

    public Constant(object expression)
    {
        Expression = expression;
    }

}

public sealed class Function 
{
    public ExpressionSyntax Body { get; }
    public List<ExpressionSyntax> Parameters { get; }

    // public Type Type => Expression.GetType();

    public Function(ExpressionSyntax body, List<ExpressionSyntax> parameters)
    {
        Body = body;
        Parameters = parameters;
    }
}

