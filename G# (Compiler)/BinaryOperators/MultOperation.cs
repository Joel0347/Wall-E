namespace G_Sharp;

public class MultOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType {
        get {
            if (SemanticCheck.GetType(Left) == "measure" || 
                SemanticCheck.GetType(Right) == "measure") {
                return "measure";
            }

            return "number";
        }
    }

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public MultOperation(object left, object right, SyntaxToken operationToken)
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

        bool correctMembers = (leftType == "number" && rightType == "measure") ||
        (leftType == "measure" && rightType == "number") || (leftType == "number" && rightType == "number");

        if (!leftIsCompatible || !rightIsCompatible || !correctMembers)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '*' can't be used between '{leftType}' and '{rightType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        if (SemanticCheck.GetType(Left) == "measure") 
        {
            var leftValue = ((Measure)Left).Value;
            var rightValue = Math.Abs(Convert.ToInt32((double) Right));
            return new Measure((float) (leftValue * rightValue));
        }

        else if (SemanticCheck.GetType(Right) == "measure")
        {
            var rightValue = ((Measure)Right).Value;
            var leftValue = Math.Abs(Convert.ToInt32((double) Left));
            return new Measure((float) (leftValue * rightValue));
        }

        return (double) Left * (double) Right;
    }
}