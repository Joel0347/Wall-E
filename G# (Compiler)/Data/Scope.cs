using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace G_Sharp;

public class Scope
{
    public Dictionary<string, Constant> Constants { get; }
    public Dictionary<string, Function> Functions { get; }

    public Scope(
        Dictionary<string, Constant> constants,
        Dictionary<string, Function> functions
    )
    {
        Constants = constants;
        Functions = functions;
    }
}

public sealed class Constant 
{
    // public Type Type => Expression.GetType();
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