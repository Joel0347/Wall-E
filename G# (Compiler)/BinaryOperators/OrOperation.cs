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
        var left = (SemanticCheck.GetType(Left) == "sequence") ? ((SequenceExpressionSyntax)Left).Count : Left;
        var right = (SemanticCheck.GetType(Right) == "sequence") ? ((SequenceExpressionSyntax)Right).Count : Right;
        bool leftIsFalse = Evaluator.DefaultFalseValues.Contains(left);
        bool rightIsFalse = Evaluator.DefaultFalseValues.Contains(right);

        return (leftIsFalse && rightIsFalse) ? 0 : 1;
    }
}