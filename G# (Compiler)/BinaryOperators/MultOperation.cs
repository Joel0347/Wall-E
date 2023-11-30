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

    public MultOperation(object left, object right)
    {
        Left = left;
        Right = right;
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
            Error.SetError("SEMANTIC", $"Operator '*' can't be used between '{leftType}' and '{rightType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        if (SemanticCheck.GetType(Left) == "measure") 
        {
            var leftValue = ((Measure)Left).Value;
            var rightValue = Math.Abs((int) Right);
            return new Measure((float) (leftValue * rightValue));
        }

        else if (Right is Measure measure2)
        {
            var rightValue = measure2.Value;
            var leftValue = Math.Abs((int) Right);
            return new Measure((float) (leftValue * rightValue));
        }

        return (double) Left * (double) Right;
    }
}