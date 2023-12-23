namespace G_Sharp;

#region Literales
public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
    public override string ReturnType => SemanticChecker.GetType(LiteralToken.Value);

    #region Constructor

    public SyntaxToken LiteralToken { get; }

    public LiteralExpressionSyntax(SyntaxToken literalToken)
    {
        LiteralToken = literalToken;
    }

    #endregion

    // Evaluaci�n
    public override object Evaluate(Scope scope)
    {
        return LiteralToken.Value;
    }

    // Revisi�n
    public override bool Check(Scope scope)
    {
        return true;
    }

    public override string ToString() => LiteralToken.Value.ToString()!;
}

#endregion