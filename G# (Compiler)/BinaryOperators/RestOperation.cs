namespace G_Sharp;

public class RestOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Left);

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public RestOperation(object left, object right, SyntaxToken operationToken)
    {
        Left = left;
        Right = right;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        bool leftIsCompatible =  leftType == "number" || leftType == "measure";
        bool rightIsCompatible = rightType == "number" || rightType == "measure";
        bool sameType = leftType == rightType;

        if (!sameType || !leftIsCompatible || !rightIsCompatible)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '-' can't be used between '{leftType}' and '{rightType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        if (SemanticCheck.GetType(Left) == "measure") 
        {
            var leftValue = ((Measure)Left).Value;
            var rightValue = ((Measure)Right).Value;
            return new Measure((float) Math.Abs(leftValue - rightValue));
        }

        return (double) Left - (double) Right;
    }
}