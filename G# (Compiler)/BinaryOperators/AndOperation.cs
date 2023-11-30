namespace G_Sharp;

public class AndOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }

    public AndOperation(object left, object right)
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

        return (leftIsFalse || rightIsFalse) ? 0 : 1;
    }
}