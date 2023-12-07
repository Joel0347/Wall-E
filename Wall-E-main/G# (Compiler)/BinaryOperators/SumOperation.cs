namespace G_Sharp;

public class SumOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType => SemanticChecker.GetType(Left);

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

    public override bool Check(Scope scope)
    {
        string leftType = SemanticChecker.GetType(Left);
        string rightType = SemanticChecker.GetType(Right);

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
            if (rightType == "undefined")
                return true;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        string leftType = SemanticChecker.GetType(Left);
        string rightType = SemanticChecker.GetType(Right);

        if (leftType == "measure")
        {
            var leftValue = ((Measure)Left).Value;
            var rightValue = ((Measure)Right).Value;
            return new Measure((float)(leftValue + rightValue));
        }

        else if (leftType == "sequence")
        {
            return SumSequence(rightType);
        }

        else if (leftType == "undefined" || rightType == "undefined")
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

    private object SumSequence(string type)
    {
        var seq1Type = SequenceExpressionSyntax.GetInternalTypeOfSequence(((SequenceExpressionSyntax)Left));
        var seq2Type = SequenceExpressionSyntax.GetInternalTypeOfSequence(((SequenceExpressionSyntax)Right));


        if (seq1Type != seq2Type)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                        $"be used between 'sequence of {seq1Type}' and " +
                        $"'sequence of {seq2Type}'");
            return false;
        }

        List<object> parameters = new();

        if (((SequenceExpressionSyntax)Left).Count < 0)
            return Left;

        if (((SequenceExpressionSyntax)Right).Count < 0)
        {
            var seq1 = (SequenceExpressionSyntax)Left;
            Dictionary<int, object> values = new();

            for (int i = 0; i < seq1.Count; i++)
            {
                values[i] = seq1[i];
            }

            if (Right is InfiniteSequence infinite)
                return InfiniteSequence.CreateInifniteSequence(infinite, values);

            return InfiniteSequence.CreateInifniteSequence((InfiniteIntegerSequence)Right, values);
        }

        var sequence1 = (SequenceExpressionSyntax)Left;

        for (int i = 0; i < sequence1.Count; i++)
        {
            parameters.Add(sequence1[i]);
        }

        if (type == "undefined")
            parameters.Add(null!);

        else
        {
            var sequence2 = (SequenceExpressionSyntax)Right;
            for (int i = 0; i < sequence2.Count; i++)
            {
                parameters.Add(sequence2[i]);
            }
        }

        return new FiniteSequence<object>(parameters);
    }
}