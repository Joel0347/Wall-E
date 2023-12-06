namespace G_Sharp;

public class SumOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Left);

    private readonly static List<string> compatibility = new()
    {
        "number", "sequence", "measure", "undefined"
    };

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public SumOperation(object left, object right, SyntaxToken operationToken)
    {
        Left = left;
        Right = right;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        bool leftIsCompatible =  compatibility.Contains(leftType);
        bool rightIsCompatible = compatibility.Contains(rightType);
        bool sameType = leftType == rightType;

        if (!leftIsCompatible || !rightIsCompatible)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return false;
        }

        if (!sameType && leftType != "undefined" && rightType != "undefined")
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return false;
        }

        if (leftType == "sequence")
        {
            var seq1 = (SequenceExpressionSyntax)Left;

            if (rightType == "undefined")
                return true;

            var seq2 = (SequenceExpressionSyntax)Right;

            if (seq1.ValuesType != seq2.ValuesType)
            {
                Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                            $"be used between 'sequence of {seq1.ValuesType}' and 'sequence of {seq2.ValuesType}'");
                return false;
            }
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        if (leftType == "measure")
        {
            var leftValue = ((Measure)Left).Value;
            var rightValue = ((Measure)Right).Value;
            return new Measure((float)(leftValue + rightValue));
        }

        else if (leftType == "sequence")
        {
            var sequence1 = (FiniteSequence<object>)Left;
            List<object> parameters = new();
            parameters.AddRange(sequence1.Elements);

            if (SemanticCheck.GetType(Right) == "undefined")
                parameters.Add(null!);

            else
            {
                var sequence2 = (FiniteSequence<object>)Right;
                parameters.AddRange(sequence2.Elements);
            }

            return new FiniteSequence<object>(parameters);
        }

        else if (leftType == "undefined")
            return null!;

        try
        {
            return double.Parse(Left.ToString()!) + double.Parse(Right.ToString()!);
        }

        catch 
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
            return null!;
        }
    }
}