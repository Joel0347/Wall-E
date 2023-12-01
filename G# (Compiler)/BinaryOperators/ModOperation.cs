namespace G_Sharp;

public class ModOperation : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override string ReturnType => "number";

    public object Left { get; }
    public object Right { get; }
    public SyntaxToken OperationToken { get; }

    public ModOperation(object left, object right, SyntaxToken operationToken)
    {
        Left = left;
        Right = right;
        OperationToken = operationToken;
    }

    public override bool Checker(Scope scope)
    {
        string leftType = SemanticCheck.GetType(Left);
        string rightType = SemanticCheck.GetType(Right);

        if (leftType != "number" || rightType != "number")
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Operator '%' can't be used between '{leftType}' and '{rightType}'");
            return false;
        }
        
        return true;
    }

    public override object Evaluate(Scope scope)
    {
        if ((double)Right == 0)
        {
            Error.SetError("SEMANTIC", $"Line '{OperationToken.Line}' : Division by '0' is not defined");
            return 0;
        }

        return (double)Left % (double)Right;
    }
}