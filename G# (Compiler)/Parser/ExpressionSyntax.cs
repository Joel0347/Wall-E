namespace G_Sharp;

public abstract class ExpressionSyntax 
{
    public abstract SyntaxKind Kind { get; }
}

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public SyntaxToken LiteralToken { get; }

    public LiteralExpressionSyntax(SyntaxToken literalToken)
    {
        LiteralToken = literalToken;
    }
}

public sealed class FunctionExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

    public SyntaxToken IdentifierToken { get; }
    public List<ExpressionSyntax> Values { get; }

    public FunctionExpressionSyntax(SyntaxToken identifierToken, List<ExpressionSyntax> values)
    {
        IdentifierToken = identifierToken;
        Values = values;
    }
}

public sealed class AssignmentFunctionExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentFunctionExpression;
    public SyntaxToken FunctionIdentifierToken { get; }
    public List<ExpressionSyntax> IdentifiersToken { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }

    public AssignmentFunctionExpressionSyntax(
        SyntaxToken functionIdentifierToken, List<ExpressionSyntax> identifiersToken,
        SyntaxToken assignmentToken, ExpressionSyntax expression
    )
    {
        foreach (var item in identifiersToken)
        {
            if(item.Kind != SyntaxKind.NameExpression) {
                Error.SetError("SYNTAX", "Expected identifier token in function parameters");
            }
        }

        FunctionIdentifierToken = functionIdentifierToken;
        IdentifiersToken = identifiersToken;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }
}


public sealed class NameExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NameExpression;

    public SyntaxToken IdentifierToken { get; }

    public NameExpressionSyntax(SyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;
    }
}

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }

    public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken assignmentToken, ExpressionSyntax expression)
    {
        IdentifierToken = identifierToken;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }
}

public sealed class Constant : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
    // public Type Type => Expression.GetType();
    public object Expression { get; }

    public Constant(object expression)
    {
        Expression = expression;
    }

}

public sealed class Function : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.AssignmentFunctionExpression;
    public ExpressionSyntax Body { get; }
    public List<ExpressionSyntax> Parameters { get; }

    // public Type Type => Expression.GetType();

    public Function(ExpressionSyntax body, List<ExpressionSyntax> parameters)
    {
        Body = body;
        Parameters = parameters;
    }
}

public sealed class LetInExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.LetInExpression;

    public SyntaxToken LetToken { get; }
    public List<ExpressionSyntax> Instructions { get; }
    public SyntaxToken InToken { get; }

    public LetInExpressionSyntax(
        SyntaxToken letToken, List<ExpressionSyntax> instructions, 
        SyntaxToken inToken, ExpressionSyntax body
    )
    {
        LetToken = letToken;
        Instructions = instructions;
        Instructions.Add(body);
        InToken = inToken;
    }
}

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public BinaryExpressionSyntax(
        ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right
    )
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }
}

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Operand { get; }

    public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }
}

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken OpenParenthesisToken { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken ClosedParenthesisToken { get; }
    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

    public ParenthesizedExpressionSyntax(
        SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closedParenthesisToken
    )
    {
        OpenParenthesisToken = openParenthesisToken;
        Expression = expression;
        ClosedParenthesisToken = closedParenthesisToken;
    }
}

public sealed class ErrorExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ErrorToken;
    public ErrorExpressionSyntax() { }
}