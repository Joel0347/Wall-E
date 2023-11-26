namespace G_Sharp;

public abstract class StatementSyntax : ExpressionSyntax { }

public sealed class BlockStatementSyntax : StatementSyntax
{
    private readonly SyntaxToken OpenStatementToken;
    private readonly StatementSyntax[] Statements;
    private readonly SyntaxToken ClosedStatementToken;

    public override SyntaxKind Kind => SyntaxKind.BlockStatement;

    public BlockStatementSyntax(
        SyntaxToken openStatementToken, StatementSyntax[] statements, SyntaxToken closedStatementToken
    )
    {
        OpenStatementToken = openStatementToken;
        Statements = statements;
        ClosedStatementToken = closedStatementToken;
    }
}

