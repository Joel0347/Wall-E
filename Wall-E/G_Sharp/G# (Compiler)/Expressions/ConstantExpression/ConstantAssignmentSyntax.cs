using System.Xml.Linq;

namespace G_Sharp;

#region Asignaci�n de constantes
public sealed class ConstantAssignmentSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ConstantAssignmentExpression;
    public override string ReturnType => "void expression";

    #region Constructor
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }
    

    public ConstantAssignmentSyntax(
        SyntaxToken identifierToken, SyntaxToken assignmentToken, 
        ExpressionSyntax expression
    )
    {
        IdentifierToken = identifierToken;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }

    #endregion

    // Evaluaci�n
    public override object Evaluate(Scope scope)
    {
        string name = IdentifierToken.Text;
        object value = Expression;

        if ((int)Expression.Kind < 22 || (int)Expression.Kind > 28)
            value = Expression.Evaluate(scope);
 
        if (value is null)
        {
            Error.SetError("SEMANTIC", $"Line '{IdentifierToken.Line}' : Constant '{name}' can't " +
                        $"be assigned to 'undefined' expression");
            return "";
        }

        scope.Constants[name] = new Constant(value);
        return "";
    }

    // Revisi�n
    public override bool Check(Scope scope)
    {
        string name = IdentifierToken.Text;

        if ((int)Expression.Kind < 22 || (int)Expression.Kind > 28)
        {
            if (!Expression.Check(scope))
                return false;

            if (Expression.ReturnType == "void expression")
            {
                Error.SetError("SEMANTIC", $"Line '{IdentifierToken.Line}' : Constant '{name}' " +
                                $"can't be assigned to statement");
                return false;
            }
        }

        if (scope.Constants.ContainsKey(name))
        {
            string constant = name == "undefined" ?  "" : "Constant";
            Error.SetError("SYNTAX", $"Line '{IdentifierToken.Line}' : {constant} '{name}' is " +
                            $"already defined");
            return false;
        }

        scope.Constants[name] = new Constant(Expression);
        return true;
    }
}

#endregion