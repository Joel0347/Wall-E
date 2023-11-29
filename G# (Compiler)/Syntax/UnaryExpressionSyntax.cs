namespace G_Sharp;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Operand { get; }
    public override string ReturnType => "number";

    private static readonly Dictionary<SyntaxKind, Func<object, double>> unaryOperationEvaluation = new()
    {
        [SyntaxKind.PlusToken] = (operand) => (double)operand,
        [SyntaxKind.MinusToken] = (operand) => -(double)operand,
        [SyntaxKind.NotKeyword] = (operand) => EvaluationSupplies.DefaultFalseValues.Contains(operand) ? 1 : 0,
    };

    public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public override object Evaluate(Scope scope)
    {
        var operand = scope.EvaluateExpression(Operand);
        return unaryOperationEvaluation[OperatorToken.Kind](operand);
    }

    public override bool Checker(Scope scope)
    {
        bool operandIsFine = Operand.Checker(scope);

        if (operandIsFine)
        {
            if (!SemanticCheck.UnaryOperatorsCheck[OperatorToken.Kind](Operand))
            {
                var operandType = SemanticCheck.GetType(Operand);
                var operation = OperatorToken.Text;
                Error.SetError("SEMANTIC", $"Operator '{operation}' can't not be used before '{operandType}'");
            }

            else return true;
        }
        
        return false;
    }
}