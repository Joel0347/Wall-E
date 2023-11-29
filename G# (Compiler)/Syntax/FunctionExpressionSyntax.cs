namespace G_Sharp;

public sealed class FunctionExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

    public SyntaxToken IdentifierToken { get; }
    public List<ExpressionSyntax> Values { get; }

    public override string ReturnType => throw new NotImplementedException();

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

        if (!scope.Functions.TryGetValue(name, out Function? function))
        {
            if (functionToken.Kind != SyntaxKind.GeometryKeyword)
            {
                Error.SetError("SEMANTIC", $"Function '{name}' is not defined yet");
                return "";
            }
        }

        Dictionary<string, Constant> constants = new();

        for (int i = 0; i < values.Count; i++)
        {
            ConstantExpressionSyntax parameter = (ConstantExpressionSyntax)function!.Parameters[i];
            var value = scope.EvaluateExpression(values[i]);
            var identifier = parameter.IdentifierToken;
            constants[identifier.Text] = new Constant(value);
        }
 
        foreach (var item in scope.Constants.Keys)
        {
            if (!constants.ContainsKey(item))
                constants[item] = scope.Constants[item];
        }

        Scope child = new(constants, scope.Functions);

        if (functionToken.Kind == SyntaxKind.GeometryKeyword)
            return EvaluateGeometricFunctions(child, functionToken.Text, values);

        var evaluator = new Evaluator(function!.Body, child);
        return evaluator.Evaluate();
    }

    public static object EvaluateGeometricFunctions(
        Scope scope, string identifier, List<ExpressionSyntax> parameters
    )
    {
        return EvaluationSupplies.GeometricFunctionEvaluations[identifier](identifier, scope, parameters);
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }
}
