namespace G_Sharp;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }
    public override string ReturnType => throw new NotImplementedException();

    private static readonly Dictionary<SyntaxKind, Func<object, object, double>> binaryOperationEvaluation = new()
    {
        // numeric operations
        [SyntaxKind.PlusToken] = (left, right) => (double)left + (double)right,
        [SyntaxKind.MinusToken] = (left, right) => (double)left - (double)right,
        [SyntaxKind.MultToken] = (left, right) => (double)left * (double)right,
        [SyntaxKind.DivisionToken] = DivisionEval,
        [SyntaxKind.ModToken] = ModuleEval,

        // booleans operations
        [SyntaxKind.AndKeyword] = AndEval,
        [SyntaxKind.OrKeyword] = OrEval,

        [SyntaxKind.EqualToken] = (left, right) => (left == right) ? 1 : 0,
        [SyntaxKind.DifferentToken] = (left, right) => (left != right) ? 1 : 0,
        [SyntaxKind.GreaterToken] = (left, right) => ((double)left > (double)right) ? 1 : 0,
        [SyntaxKind.LessToken] = (left, right) => ((double)left < (double)right) ? 1 : 0,
        [SyntaxKind.GreaterOrEqualToken] = (left, right) => ((double)left >= (double)right) ? 1 : 0,
        [SyntaxKind.LessOrEqualToken] = (left, right) => ((double)left <= (double)right) ? 1 : 0,
    };

    public BinaryExpressionSyntax(
        ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right
    )
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override object Evaluate(Scope scope)
    {
        var left = scope.EvaluateExpression(Left);
        var right = scope.EvaluateExpression(Right);
        var operation = OperatorToken;
        var operatorKind = operation.Kind;

        return binaryOperationEvaluation[operatorKind](left, right);
    }

    public override bool Checker(Scope scope)
    {
        bool leftIsFine = Left.Checker(scope);
        bool rightIsFine = Right.Checker(scope);

        if (leftIsFine && rightIsFine)
        {
            if (!SemanticCheck.BinaryOperatorsCheck[OperatorToken.Kind](Left, Right))
            {
                string leftType = SemanticCheck.GetType(Left);
                string rightType = SemanticCheck.GetType(Right);
                Error.SetError("SEMANTIC", $"Operator '{OperatorToken.Text}' can't be used between '{leftType}' and '{rightType}'");
            }

            else return true;
        }

        
        return false;
    }

    private static double DivisionEval(object left, object right)
    {
        if ((double)right == 0)
        {
            Error.SetError("SEMANTIC", "Division by '0' is not defined");
            return 0;
        }

        return (double)left / (double)right;
    }

    private static double ModuleEval(object left, object right)
    {
        if ((double)right == 0)
        {
            Error.SetError("SEMANTIC", "Division by '0' is not defined");
            return 0;
        }

        return (double)left % (double)right;
    }

    private static double AndEval(object left, object right)
    {
        bool leftIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(left);
        bool rightIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(right);

        return (leftIsFalse || rightIsFalse) ? 0 : 1;
    }
    private static double OrEval(object left, object right)
    {
        bool leftIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(left);
        bool rightIsFalse = EvaluationSupplies.DefaultFalseValues.Contains(right);

        return (leftIsFalse && rightIsFalse) ? 0 : 1;
    }
}