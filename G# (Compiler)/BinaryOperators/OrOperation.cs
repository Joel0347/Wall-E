namespace G_Sharp;

public class OrOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }

    public OrOperation(object left, object right)
    {
        Left = left;
        Right = right;
    }

    public override bool Checker(Scope scope)
    {
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        bool leftIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(Left);
        bool rightIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(Right);

        return (leftIsFalse && rightIsFalse) ? 0 : 1;
    }
}