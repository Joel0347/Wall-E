using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace G_Sharp;

#region Objecto Scope
public class Scope
{
    public Dictionary<string, Constant> Constants { get; }
    public Dictionary<string, Function> Functions { get; }

    public List<Draw> DrawingObjects = new();

    // Constantes por defecto
    private readonly Dictionary<string, Constant> defaultConstants = new()
    {
        ["PI"]        = new Constant(Math.PI),
        ["E"]         = new Constant(Math.E),
        ["undefined"] = new Constant(null!)
    };

    // Funciones por defecto
    public static readonly Dictionary<string, Func<Scope, List<ExpressionSyntax>, object>> DefaultFunctions = new()
    {
        ["point"] = ScopeSupplies.PointFunction,
        ["line"] = ScopeSupplies.LineFunction,
        ["segment"] = ScopeSupplies.SegmentFunction,
        ["ray"] = ScopeSupplies.RayFunction,
        ["circle"] = ScopeSupplies.CircleFunction,
        ["measure"] = ScopeSupplies.MeasureFunction,
        ["arc"] = ScopeSupplies.ArcFunction,
        ["count"] = ScopeSupplies.CountFunction,
        ["randoms"] = ScopeSupplies.RandomsFunction,
        ["samples"] = ScopeSupplies.SamplesFunction,
        ["points"]  = ScopeSupplies.PointsFunction,
        ["intersect"] = ScopeSupplies.IntersectFunction
    };

    // Tipos de retorno por defecto
    public static readonly Dictionary<string, List<List<string>>> TypeOfParams = new()
    {
        ["point"]     = new() { new() { "number" }, new() { "number" } },
        ["line"]      = new() { new() { "point" }, new() { "point" } },
        ["segment"]   = new() { new() { "point" }, new() { "point" } },
        ["ray"]       = new() { new() { "point" }, new() { "point" } },
        ["circle"]    = new() { new() { "point" }, new() { "measure" } },
        ["measure"]   = new() { new() { "point" }, new() { "point" } },
        ["arc"]       = new() { new() { "point" }, new() { "point" }, new() { "point" }, new() { "measure" } },
        ["count"]     = new() { new() { "sequence" } },
        ["randoms"]   = new() {  },
        ["samples"]   = new() {  },
        ["points"]    = new() { new() { "point", "circle", "arc", "segment", "ray", "line" } },
        ["intersect"] = new() 
        { 
            new() { "point", "circle", "arc", "segment", "ray", "line" }, 
            new() { "point", "circle", "arc", "segment", "ray", "line" } 
        }
    };

    // Constructor
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

        foreach (var item in DefaultFunctions.Keys)
        {
            if (!Functions.ContainsKey(item))
            {
                Functions[item] = new Function(item);
            }
        }
    }
}

#endregion

#region Objecto Constant

// Objeto que se guarda en el scope
public sealed class Constant 
{
    public object Expression { get; }
    public string Type => SemanticChecker.GetType(Expression);

    public Constant(object expression)
    {
        Expression = expression;
    }

}

#endregion

#region Objeto Function

// Objeto que se guarda en el scope
public sealed class Function 
{
    public string Name { get; }
    public ExpressionSyntax Body { get; }
    public List<ExpressionSyntax> Parameters { get; }
    public List<List<string>> TypeOfParams {  get; }
    public int NumberOfParams => Parameters.Count;
    public int NumberOfCalls = 0;
    public bool hasBeenCalled = false;
    public Function(ExpressionSyntax body, List<ExpressionSyntax> parameters)
    {
        Name = "";
        Body = body;
        Parameters = parameters;
        TypeOfParams = new();
    }

    public Function(string name)
    {
        Name = name;
        Body = new ErrorExpressionSyntax();
        TypeOfParams = Scope.TypeOfParams[name];
        Parameters = new();

        foreach (var item in Scope.TypeOfParams[name])
        {
            Parameters.Add(new ErrorExpressionSyntax());
        }
    }
}

#endregion

