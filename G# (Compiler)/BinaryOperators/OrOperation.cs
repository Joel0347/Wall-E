namespace G_Sharp;

public class OrOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public OrOperation(object left, object right, SyntaxToken operationToken)
    {
        Left = left;
        Right = right;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        bool leftIsFalse = Evaluator.DefaultFalseValues.Contains(Left);
        bool rightIsFalse = Evaluator.DefaultFalseValues.Contains(Right);

        return (leftIsFalse && rightIsFalse) ? 0 : 1;
    }
}