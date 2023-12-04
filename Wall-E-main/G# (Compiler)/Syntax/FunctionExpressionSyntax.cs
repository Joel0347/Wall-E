namespace G_Sharp;

public sealed class FunctionExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

    public SyntaxToken IdentifierToken { get; }
    public List<ExpressionSyntax> Values { get; }

    public override string ReturnType => "sequence"; //prueba

    public FunctionExpressionSyntax(SyntaxToken identifierToken, List<ExpressionSyntax> values)
    {
        IdentifierToken = identifierToken;
        Values = values;
    }

    public override object Evaluate(Scope scope)
    {
        var functionToken = IdentifierToken;
        var values = Values;
        string name = functionToken.Text;

        if (!scope.Functions.TryGetValue(name, out Function? function) && 
            !scope.DefaultFunctions.ContainsKey(name))
        {
            
            Error.SetError("SEMANTIC", $"Line '{functionToken.Line}' : Function '{name}' is not defined yet");
            return "";
            
        }

        if (scope.DefaultFunctions.TryGetValue(name, out Func<Scope, List<ExpressionSyntax>, object>? func))
            return func(scope, values);

        Dictionary<string, Constant> constants = new();

        for (int i = 0; i < values.Count; i++)
        {
            ConstantExpressionSyntax parameter = (ConstantExpressionSyntax)function!.Parameters[i];
            var value = values[i].Evaluate(scope);
            var identifier = parameter.IdentifierToken;
            constants[identifier.Text] = new Constant(value);
        }
 
        foreach (var item in scope.Constants.Keys)
        {
            if (!constants.ContainsKey(item))
                constants[item] = scope.Constants[item];
        }

        Scope child = new(constants, scope.Functions);

        //if (functionToken.Kind == SyntaxKind.GeometryKeyword)
        //    return EvaluateGeometricFunctions(child, functionToken.Text, values);
        return child.Evaluate(function!.Body);
    }

    //public static object EvaluateGeometricFunctions(
    //    Scope scope, string identifier, List<ExpressionSyntax> parameters
    //)
    //{
    //return EvaluationSupplies.GeometricFunctionEvaluations[identifier](identifier, scope, parameters);
    //}

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
