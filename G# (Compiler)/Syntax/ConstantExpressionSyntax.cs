using System.Xml.Linq;

namespace G_Sharp;

public sealed class ConstantExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ConstantExpression;

    public SyntaxToken IdentifierToken { get; }

    private string returnType = "all";
    public override string ReturnType => returnType;

    public ConstantExpressionSyntax(SyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    public override object Evaluate(Scope scope)
    {
        return scope.Constants[IdentifierToken.Text].Expression;   
    }

    public override bool Checker(Scope scope)
    {
        string name = IdentifierToken.Text;

        if (!scope.Constants.TryGetValue(name, out Constant? value))
        {
            Error.SetError("SYNTAX", $"Line '{IdentifierToken.Line}' : Constant '{name}' is not defined yet");
            return false;
        }

        returnType = SemanticCheck.GetType(value.Expression);

        return true;
    }
}