namespace G_Sharp;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Operand { get; }
    public override string ReturnType => "number";

    private static readonly Dictionary<SyntaxKind, Func<object, ExpressionSyntax>> unaryOperationEvaluation = new()
    {
        [SyntaxKind.PlusToken] = (operand) => new PlusOperation(operand),
        [SyntaxKind.MinusToken] = (operand) => new MinusOperation(operand),
        [SyntaxKind.NotKeyword] = (operand) => new NotOperation(operand)
    };

    public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public override object Evaluate(Scope scope)
    {
        var operand = scope.EvaluateExpression(Operand);
        var operation = unaryOperationEvaluation[OperatorToken.Kind](operand);
        return operation.Evaluate(scope);
    }

    public override bool Checker(Scope scope)
    {
        bool operandIsFine = Operand.Checker(scope);
        if (operandIsFine)
        {
            var operation = unaryOperationEvaluation[OperatorToken.Kind](Operand);
            return operation.Checker(scope);
        }

        return false;
    }
}