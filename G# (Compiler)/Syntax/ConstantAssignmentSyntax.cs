using System.Xml.Linq;

namespace G_Sharp;

public sealed class ConstantAssignmentSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ConstantAssignmentExpression;
    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken AssignmentToken { get; }
    public ExpressionSyntax Expression { get; }
    public override string ReturnType => Expression.ReturnType;

    public ConstantAssignmentSyntax(
        SyntaxToken identifierToken, SyntaxToken assignmentToken, 
        ExpressionSyntax expression
    )
    {
        IdentifierToken = identifierToken;
        AssignmentToken = assignmentToken;
        Expression = expression;
    }

    public override object Evaluate(Scope scope)
    {
        string name = IdentifierToken.Text;
        object value = Expression;

        if ((int)Expression.Kind < 22 || (int)Expression.Kind > 28)
        {
            value = Expression.Evaluate(scope);

            if (value.Equals(""))
            {
                Error.SetError("SEMANTIC", $"Line '{IdentifierToken.Line}' : Constant '{name}' can't be assigned to statement");
                return "";
            }
        }
 
        scope.Constants[name] = new Constant(value);
        return value;
    }

    public override bool Checker(Scope scope)
    {
        string name = IdentifierToken.Text;

        if ((int)Expression.Kind < 22 || (int)Expression.Kind > 28)
        {
            var check = Expression.Checker(scope);

            if (!check)
                return false;
        }

        if (scope.Constants.ContainsKey(name))
        {
            Error.SetError("SYNTAX", $"Line '{IdentifierToken.Line}' : Constant '{name}' is already defined");
            return false;
        }

        return true;
    }
}