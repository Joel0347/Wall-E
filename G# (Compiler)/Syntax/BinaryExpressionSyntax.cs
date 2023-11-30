using System.Drawing;

namespace G_Sharp;

public class BinaryExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }
    public override string ReturnType {
        get {
            var operation = binaryOperation[OperatorToken.Kind](Left, Right);
            return operation.ReturnType;
        }
    }

    private static readonly Dictionary<SyntaxKind, Func<object, object, ExpressionSyntax>> binaryOperation = new()
    {
        [SyntaxKind.PlusToken]           = (left, right) => new SumOperation(left, right),
        [SyntaxKind.MinusToken]          = (left, right) => new RestOperation(left, right),
        [SyntaxKind.MultToken]           = (left, right) => new MultOperation(left, right),
        [SyntaxKind.DivisionToken]       = (left, right) => new DivisionOperation(left, right),
        [SyntaxKind.ModToken]            = (left, right) => new ModOperation(left, right),
        [SyntaxKind.AndKeyword]          = (left, right) => new AndOperation(left, right),
        [SyntaxKind.OrKeyword]           = (left, right) => new OrOperation(left, right),
        [SyntaxKind.EqualToken]          = (left, right) => new EqualOperation(left, right),
        [SyntaxKind.DifferentToken]      = (left, right) => new DifferentOperation(left, right),
        [SyntaxKind.GreaterToken]        = (left, right) => new GreaterOperation(left, right),
        [SyntaxKind.LessToken]           = (left, right) => new LessOperation(left, right),
        [SyntaxKind.GreaterOrEqualToken] = (left, right) => new GreaterOrEqualOperation(left, right),
        [SyntaxKind.LessOrEqualToken]    = (left, right) => new LessOrEqualOperation(left, right)
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
        var operatorKind = OperatorToken.Kind;

        var operation = binaryOperation[operatorKind](left, right);
        return operation.Evaluate(scope);
    }

    public override bool Checker(Scope scope)
    {
        bool leftIsFine = Left.Checker(scope);
        bool rightIsFine = Right.Checker(scope);

        if (leftIsFine && rightIsFine)
        {
            var operation = binaryOperation[OperatorToken.Kind](Left, Right);
            return operation.Checker(scope);
        }

        return false;
    }
}