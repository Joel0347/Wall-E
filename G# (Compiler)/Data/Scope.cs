namespace G_Sharp;

public class Scope
{
    public Dictionary<string, object> Variables { get; }
    public Dictionary<string, List<ExpressionSyntax>> FunctionsVariables { get; }
    public Dictionary<string, ExpressionSyntax> FunctionsBody { get; }

    public Scope(
        Dictionary<string, object> variables, Dictionary<string, List<ExpressionSyntax>> functionsVariables,
        Dictionary<string, ExpressionSyntax> functionsBody
    )
    {
        Variables = variables;
        FunctionsVariables = functionsVariables;
        FunctionsBody = functionsBody;
    }
}

public class ScopeInternal
{
    public Dictionary<string, object> Global { get; }
    public Dictionary<string, object> InternalData { get; }
    public ScopeInternal(Dictionary<string, object> global, Dictionary<string, object> internalData)
    {
        Global = global;
        InternalData = internalData;
    }
}