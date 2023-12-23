namespace G_Sharp;

#region Operaci�n Not

public class NotOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    public override string ReturnType => SemanticChecker.GetType(Operand);

    #region Constructor

    public object Operand { get; }
    public SyntaxToken OperationToken { get; }

    public NotOperation(object operand, SyntaxToken operationToken)
    {
        Operand = operand;
        OperationToken = operationToken;
    }

    #endregion

    // Revisi�n
    public override bool Check(Scope scope)
    {   
        return true;
    }


    // Evaluaci�n
    public override object Evaluate(Scope scope)
    {
        var operandType = SemanticChecker.GetType(Operand);
        object operand = Operand;

        if (operandType == "sequence")
        {
            var seq = (SequenceExpressionSyntax)Operand;
            operand = seq.Count <= 0 ? null! : seq.Count;
        }

        return Evaluator.DefaultFalseValues.Contains(operand) ? 1 : 0;
    }
}

#endregion