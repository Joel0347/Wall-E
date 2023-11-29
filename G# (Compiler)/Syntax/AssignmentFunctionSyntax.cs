namespace G_Sharp;

public sealed class AssignmentFunctionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentFunctionExpression;
    public SyntaxToken FunctionIdentifierToken { get; }
    public List<ExpressionSyntax> IdentifiersToken { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }
    public override string ReturnType => throw new NotImplementedException();

    public AssignmentFunctionSyntax(
        SyntaxToken functionIdentifierToken, List<ExpressionSyntax> identifiersToken,
        SyntaxToken assignmentToken, ExpressionSyntax expression
    )
    {
        FunctionIdentifierToken = functionIdentifierToken;
        IdentifiersToken = identifiersToken;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }

    public override object Evaluate(Scope scope)
    {
        string name = FunctionIdentifierToken.Text;
        var body = Expression;
        var parameters = IdentifiersToken;

        if (!scope.Functions.ContainsKey(name))
            scope.Functions[name] = new Function(body, parameters);

        else Error.SetError("SYNTAX", $"Function '{name}' is already defined");

        return "";
    }

    public override bool Checker(Scope scope)
    {
        throw new NotImplementedException();
    }
}