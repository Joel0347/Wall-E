namespace G_Sharp;

public class PlusOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    public override string ReturnType => SemanticCheck.GetType(Operand);

    public object Operand { get; }
    public SyntaxToken OperationToken { get; }

    public PlusOperation(object operand, SyntaxToken operationToken)
    {
        Operand = operand;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string operandType = SemanticCheck.GetType(Operand);

        if (operandType != "number")
        {
            Error.SetError("SEMANTIC", $"Line ' {OperationToken.Line} ' : Operator '+' " +
                            $"can't not be used before '{operandType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        return + (double) Operand;
    }
}