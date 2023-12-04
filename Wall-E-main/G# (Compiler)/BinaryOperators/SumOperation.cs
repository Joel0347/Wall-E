namespace G_Sharp;

public class SumOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Left);

    private readonly static List<string> compatibility = new()
    {
        "number", "sequence", "measure"
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

        if (!sameType || !leftIsCompatible || !rightIsCompatible)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '+' can't " +
                            $"be used between '{leftType}' and '{rightType}'");
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
            var sequence1 = (FiniteSequence) Left;
            var sequence2 = (FiniteSequence) Right;
            List<object> parameters = new();
            parameters.AddRange(sequence1.ElementsEvaluation);
            parameters.AddRange(sequence2.ElementsEvaluation);
            return new FiniteSequence(parameters);
        }

        return double.Parse(Left.ToString()!) + double.Parse(Right.ToString()!);
    }
}