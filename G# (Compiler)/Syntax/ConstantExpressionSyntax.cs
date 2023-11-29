using System.Xml.Linq;

namespace G_Sharp;

public sealed class ConstantExpressionSyntax : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NameExpression;

    public SyntaxToken IdentifierToken { get; }

    public override string ReturnType => throw new NotImplementedException();

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

        if (!scope.Constants.ContainsKey(name))
        {
            Error.SetError("SYNTAX", $"Constant '{name}' is not defined yet");
            return false;
        }

        return true;
    }
}