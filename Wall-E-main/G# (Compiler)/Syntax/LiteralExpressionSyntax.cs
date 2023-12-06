namespace G_Sharp;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public SyntaxToken LiteralToken { get; }

    public override string ReturnType => SemanticCheck.GetType(LiteralToken.Value);

    public LiteralExpressionSyntax(SyntaxToken literalToken)
    {
        LiteralToken = literalToken;
    }

    public override object Evaluate(Scope scope)
    {
        return LiteralToken.Value;
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public override string ToString() => LiteralToken.Value.ToString()!;
}