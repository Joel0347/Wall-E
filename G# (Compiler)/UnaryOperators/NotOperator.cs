namespace G_Sharp;

public class NotOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Operand);

    public object Operand { get; }
    public SyntaxToken OperationToken { get; }

    public NotOperation(object operand, SyntaxToken operationToken)
    {
        Operand = operand;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string operandType = SemanticCheck.GetType(Operand);

        if (operandType != "number")
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator 'not' can't not be used before '{operandType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        return Evaluator.DefaultFalseValues.Contains(Operand) ? 1 : 0;
    }
}