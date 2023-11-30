namespace G_Sharp;

public class SumOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Left);  

    public object Left { get; }
    public object Right { get; }

    public SumOperation(object left, object right)
    {
        Left = left;
        Right = right;
    }

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        bool leftIsCompatible =  leftType == "number" || leftType == "measure" || leftType == "sequence";
        bool rightIsCompatible = rightType == "number" || rightType == "measure" || rightType == "sequence";
        bool sameType = leftType == rightType;

        if (!sameType || !leftIsCompatible || !rightIsCompatible)
        {
            Error.SetError("SEMANTIC", $"Operator '+' can't be used between '{leftType}' and '{rightType}'");
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
            return new Measure((float) (leftValue + rightValue));
        }

        else if (SemanticCheck.GetType(Left) == "sequence")
        {
            var sequence1 = (Sequence) Left;
            var sequence2 = (Sequence) Right;
            List<object> parameters = new();
            parameters.AddRange(sequence1.Values);
            parameters.AddRange(sequence2.Values);
            return new Sequence(parameters);
        }

        return (double) Left + (double) Right;
    }
}