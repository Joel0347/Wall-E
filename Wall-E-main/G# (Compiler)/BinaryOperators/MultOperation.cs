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

    private readonly static List<string> compatibility = new()
    {
        "number", "measure", "undefined"
    };

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        bool leftIsCompatible =  compatibility.Contains(leftType);
        bool rightIsCompatible = compatibility.Contains(rightType);

        bool correctMembers = (leftType == "number" && rightType == "measure") ||
        (leftType == "measure" && rightType == "number") || (leftType == "number" && rightType == "number");

        if (!leftIsCompatible || !rightIsCompatible)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '*' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return false;
        }

        if (!correctMembers && leftType != "undefined" && rightType != "undefined")
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '*' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return false;
        }

        return true;
    }

    public override object Evaluate(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        if (leftType == "undefined" || rightType == "undefined")
            return null!;

        if (leftType == "measure") 
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

        try
        {
            return double.Parse(Left.ToString()!) * double.Parse(Right.ToString()!);
        }

        catch
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '*' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return null!;
        }
    }
}