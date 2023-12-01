namespace G_Sharp;

public class DivisionOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public DivisionOperation(object left, object right, SyntaxToken operationToken)
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

        if (!leftIsCompatible || !rightIsCompatible || !sameType)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '/' can't be used between '{leftType}' and '{rightType}'");
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

            return (int) (leftValue / rightValue);
        }

        else if ((double)Right == 0)
        {
            Error.SetError("SEMANTIC", "Division by '0' is not defined");
            return 0;
        }

        return (double)Left / (double)Right;
    }
}