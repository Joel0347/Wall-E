namespace G_Sharp;

public class EqualOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public EqualOperation(object left, object right, SyntaxToken operationToken)
    {
        Left = left;
        Right = right;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        if (leftType != rightType)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '==' can't be " +
                            $"used between '{leftType}' and '{rightType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        if (SemanticCheck.GetType(Left) == "measure")
        {
            var measure1 = (Measure) Left;
            var measure2 = (Measure) Right;
            return (measure1.Value == measure2.Value) ? 1 : 0;
        }

        return Left.Equals(Right) ? 1 : 0;
    }
}